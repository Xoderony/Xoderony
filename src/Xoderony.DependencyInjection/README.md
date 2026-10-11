# Xoderony.DependencyInjection

面向 .NET 10 的服务容器，仅依赖 BCL。支持类型、实例和工厂注册，注册级自动激活、父子作用域、按键服务、构建时自动集合、闭合泛型及反射注入。

## 基本使用

```csharp
using Xoderony.DependencyInjection;

var builder = new ContainerBuilder();
builder.Register<AudioService>().As<IAudioService>();
builder.RegisterInstance(gameConfig);
builder.AddBuildCallback(resolver => resolver.Resolve<IAudioService>().Initialize());

using var root = builder.Build();
using var battle = root.CreateChild(local => {
    local.RegisterInstance(battleConfig);
    local.Register<BattleService>();
});
var service = battle.Resolve<BattleService>();
```

示例中的服务与配置类型由业务定义。类型注册需要可构造的引用类型；泛型 API 使用 `class` 约束。模块注册可直接通过业务静态方法接收 `ContainerBuilder` 完成。

`Register<T>` 默认暴露具体类型；调用 `As<TService>` 后只暴露显式列出的服务类型，需要同时保留具体类型时调用 `AsSelf()`。同一注册的别名共享实例；重复添加相同别名不会重复注册。模块可通过 `AsImplementedInterfaces()` 暴露实现接口。开放泛型不支持，运行时 Type 必须完整闭合。Build 为本次容器建立独立注册映射；后续更改构建器不会修改已构建容器。

## 生命周期与调用约定

| 生命周期或来源 | 创建与复用 | 释放责任 |
|---|---|---|
| 类型注册 | 首次解析创建并缓存，依赖从注册所属容器解析 | 注册所属容器 |
| 已有实例 | 原样返回 | 外部 |
| 工厂返回对象 | 首次解析调用一次并缓存，工厂接收注册所属容器 | 注册所属容器 |

容器仅自动调用其拥有的 `IDisposable` 服务，按完成创建的逆序释放。异步释放尚未提供。工厂返回对象由容器管理；容器不释放已有实例，不递归释放子作用域。

调用方保证先释放子作用域再释放父容器，不再使用已释放容器及其服务。同一容器和共享父容器的作用域体系必须串行使用，包括构建、解析、注入和释放。已取得的服务是否允许跨线程使用由服务自身契约决定。

## 自动激活

```csharp
builder.Register<StrideLogReceiver>().AutoActivate();
builder.Register<BuiltInConsoleCommands>().AutoActivate();
// 无需 AsSelf，也支持工厂、别名和键。
builder.RegisterFactory<LogReceiver>(resolver => new LogReceiver(resolver.Resolve<LogConfig>()))
    .As<ILogReceiver>().WithKey(1).AutoActivate();
```

`AutoActivate()` 返回当前 RegistrationBuilder，可与其他注册配置任意顺序链式调用，重复调用只标记一次。每次 Build 在声明判重、服务分组、集合映射和父层继承全部完成后，按注册顺序激活本层标记的注册，再按添加顺序执行全部 `AddBuildCallback`，最后返回容器。依赖解析可能使后注册的服务先创建；未标记且未被依赖或回调解析的服务仍延迟创建。

自动激活直接求值该注册的原始缓存，不按暴露的服务类型查找；只提供别名、有键、同服务多候选和工厂注册均可使用。后续普通解析、别名和集合复用同一实例。类型注册沿用构造和成员注入流程；工厂仍负责完整装配。已有实例上的标记不额外注入或接管释放。成功创建的类型与工厂对象仍由所属容器按完成创建逆序释放。

子容器仅自动激活本层标记注册，不重复激活父层注册；依赖父层服务时仍由父层创建并拥有。重复 Build 为每个新容器分别激活，后续修改构建器不影响已有容器。

激活异常直接传播并中止后续激活及所有构建回调；回调异常中止后续回调。沿用现有失败行为，不自动释放已创建服务、不回滚副作用、不包装异常或重试。失败时 Build 不返回容器，已创建资源可能无法经该容器释放。成员注入失败的对象不登记释放，Dispose 遇异常即中断；本功能不增加专门异常处理。

## 注册选择与集合

```csharp
// 可以分别放在不同模块的注册方法中。
builder.Register<DamageHandler>().As<IHandler>().AsSelf();
builder.Register<ScoreHandler>().As<IHandler>().AsSelf();

IReadOnlyList<IHandler> handlers = container.Resolve<IReadOnlyList<IHandler>>();
DamageHandler damage = container.Resolve<DamageHandler>();
// handlers[0] 与 damage 是同一个对象。
// Resolve<IHandler>() 有两个候选，报告歧义。
```

Build 按完整服务类型和 Key 分组。至少一条注册就自动提供 `T[]`、`IEnumerable<T>`、`IReadOnlyList<T>`；无需额外集合声明。集合按注册顺序取原始实例，同一条注册的多个别名和集合共享实例，只登记一次释放。

单个请求只有一个候选时返回该实例；多个候选时抛出 `InvalidOperationException`，不创建任何候选。Build 允许不同实现共同提供同一服务，单个解析不隐式选择最后一项。`IsRegistered<T>()` 表示入口存在，多候选时仍为 true；`TryResolve<T>()` 仅在缺失时返回 false，歧义或创建失败仍抛出。

同层相同声明类型与 Key 不允许重复，Build 时抛出 `ArgumentException`，不执行构建回调。类型注册按实现类型、实例注册按实际类型、工厂注册按 `RegisterFactory<T>` 的声明 T 判断，并跨来源统一检查。因此两个 `RegisterFactory<IHandler>` 即使返回不同实现也会冲突，应使用具体声明类型或不同 Key。单条注册重复添加相同 As 别名会去重。

子层同服务类型和 Key 的注册整组替换父层，对单个入口和三种集合入口同时生效，不合并父层成员。普通类型和闭合泛型遵循相同规则；例如子层可以同时替换父层 `IRepository<int>` 并增加 `IRepository<string>`。未覆盖的别名仍继承父层原始注册，依赖解析和释放归父层。

集合首次解析取得全部元素并缓存只读结果，`IEnumerable<T>` 与 `IReadOnlyList<T>` 返回同一结果，`T[]` 每次返回副本。只读性不限制元素自身的可变状态。集合失败也被缓存、不重试；失败前已创建的元素仍由原注册容器释放。

零条注册视为未注册，不自动返回空集合：`Resolve` 抛错，`TryResolve` 返回 false，`IsRegistered` 返回 false。可选的零元素业务通过 `TryResolve` 处理。只为原始服务组追加集合入口，不递归生成集合的集合。

`T[]`（一维零下界数组）、`IEnumerable<T>`、`IReadOnlyList<T>` 是容器保留服务类型，外部不得直接注册，校验包含注册声明类型、实例类型、工厂声明类型和 As 别名。因此配置数组需要包装为配置对象；具体 `List<T>` 可以正常注册，但不能再 As 到保留接口。`AsImplementedInterfaces()` 遇到保留接口也会报告错误，不静默跳过。

无键 `Container` 和 `IResolver` 固定为当前容器，自动生成的对应集合也只包含当前容器；外部同类型的无键注册不能替换这些入口，非零键仍按普通注册处理。容器不会因这些内置入口接管自身或父容器的释放责任。

## 按键服务

```csharp
builder.Register<AudioChannel>()
    .As<IAudioChannel>()
    .WithKey(AudioKey.Music);

var music = container.Resolve<IAudioChannel, AudioKey>(AudioKey.Music);
```

一条注册的所有别名与自动集合使用同一个键，指定键后不额外产生无键注册。集合只收集相同服务类型与 Key 的注册。键以 `int` 存储，`0` 表示默认键。支持直接传入整数，或通过泛型入口传入以 `int` 为底层类型的枚举；其他底层类型抛出 `ArgumentException`。不同枚举类型的相同数值视为同一键，调用方负责协调编号。不同键分别注册，无键集合不包含有键服务。

枚举由业务定义，例如 `enum AudioKey { Default = 0, Music = 1 }`。`WithKey` 和运行时类型入口 `Resolve(typeof(IAudioChannel), AudioKey.Music)` 自动推断枚举类型；泛型服务入口需要显式指定服务与枚举类型。`TryResolve<T, TKey>(out instance, key)`、`IsRegistered<T, TKey>(key)` 使用相同约定。原有字符串、任意对象和 null Key 不再支持。枚举泛型入口不装箱；`InjectKey` 特性中的枚举在反射分析阶段转换，解析路径只保存整数。

## 注入

```csharp
public sealed class MusicPlayer {
    private IAudioChannel? _channel;

    [Inject]
    private void SetChannel([InjectKey(AudioKey.Music)] IAudioChannel channel) {
        _channel = channel;
    }
}

container.Inject(existingPlayer);
```

- 唯一的 `[Inject]` 构造器优先，允许非公开；没有标记时要求唯一公开构造器。多个标记或无法唯一选择时报告错误。
- `[Inject]` 标记实例方法、属性或字段；`[InjectKey]` 只选择服务，不触发注入。
- 公开、非公开及基类成员均可注入。先构造，再依次注入所有字段、所有属性、所有方法；每个阶段先基类后派生类，同层成员之间不保证顺序。
- 方法必须非泛型、非虚、返回 `void`，参数按值传递；不支持异步注入流程。属性必须有普通 setter，不能是索引器或 `init`；字段不能只读。静态成员不参与注入。
- 类型注册会自动完成注入；已有实例和工厂返回对象视为已经准备好，需要时显式调用 `Inject`。
- 每次显式注入都会重新赋值并调用方法，副作用由调用方管理，不记录已注入状态。

## 工厂与对象池

```csharp
builder.RegisterFactory<IResourceService>(
    resolver => new ResourceService(resolver.Resolve<GameConfig>()));

builder.RegisterFactory<Func<Projectile>>(
    resolver => () => new Projectile(resolver.Resolve<GameConfig>()));
```

容器不自动生成 `Func<T>`。工厂在首次解析时执行一次，负责完整装配，不自动成员注入；需要时显式调用 `resolver.Inject(instance)`。工厂必须返回非空对象，并将所有权交给注册所属容器，不应返回另一个已由容器管理的服务或外部共享实例。显式工厂捕获作用域时，调用方保证在该作用域有效期间使用；注册的 `Func<T>` 后续调用所产生的对象不由容器自动管理。工厂注册以泛型参数 `T` 作为默认服务类型，别名兼容性也按 `T` 检查。

对象池可以作为普通实例注册。池首次创建对象时显式注入，后续租借是否重新注入由业务决定；容器不接管租借对象。

## 闭合泛型与初始化

```csharp
builder.Register<Repository<int>>().As<IRepository<int>>();
builder.Register(typeof(Repository<string>)).As(typeof(IRepository<string>));
```

仅支持完整闭合类型。`Repository<>` 或包含未绑定类型参数的类型在注册时被拒绝，不存在运行时自动闭合、模板匹配或泛型服务族限制。不同完整 Type 是不同服务，未声明的类型参数不会自动获得注册。

Build 先完成声明判重、服务分组、自动集合映射与父层继承，此阶段不分析构造和成员，也不创建服务；随后自动激活标记注册并执行构建回调。所有解析入口共用一张 `(Type, Key)` 字典：一次查询后求值 Lazy；真正未注册的请求直接失败，不在解析时补注册。数组命中后需要复制结果。

类型服务在首次求值时分析、创建和注入；工厂首次求值时调用。外部对象首次注入时按实际类型分析成员，不检查构造函数。成员分析在父子容器间共享缓存；使用反射 Invoker 执行，参数缓冲区按次租借并在 finally 中清空归还。固定映射使用 Dictionary，不代表采用 FrozenDictionary 或支持并发调用。

`AddBuildCallback` 在全部自动激活完成后、容器返回前执行，可主动解析其他需要提前准备的服务，沿用普通解析路径。它不在对象创建或注入后重复执行，也不能用于准备自动激活服务的构造前置条件。

## 非法输入

开放泛型、指针、ByRef、ByRefLike 和 void 无法注册，解析时统一按未注册处理，不重复进行完整类型校验。显式 IServiceProvider.GetService 对未注册返回 null；Resolve 对未注册抛出异常。工厂、构建回调必须非空，工厂返回 null 会抛出并缓存 InvalidOperationException，不能被当作服务缺失。

## API 命名调整

CreateScope 已改名为 CreateChild，明确创建的是继承注册的子容器；AddBuildCallback 替代 RegisterBuiltCallback，表示追加每次构建完成时执行的回调。调用方需更新方法名，运行行为不变。
