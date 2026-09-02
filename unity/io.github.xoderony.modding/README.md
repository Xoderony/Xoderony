# Xoderony Modding

游戏宿主与 Mod 共用的契约与默认加载约定。不依赖 `UnityEngine`。

## 类型与职责

- `ModManifest`：清单数据，包括标识、版本、展示信息和依赖。
- `Mod`：入口基类，持有 `Manifest` 与 `RootDirectory`，提供返回 `ValueTask` 的 `Load()` / `Unload()`；默认实现不执行操作。
- `ModManager`：负责清单发现、依赖检查和生命周期调度，通过 `CreateMod()` / `ReleaseMod()` 交由派生类创建、释放实例。
- `AssemblyLoadContextModManager`：通过可回收的 `AssemblyLoadContext` 加载程序集并创建入口实例。

## 目录与清单

```text
modsDirectory/
  SomeMod/
    manifest.json
    *.dll
```

扫描仅覆盖 Mods 目录的直接子目录，忽略没有 `manifest.json` 的目录。Mod ID 区分大小写，不要求与目录名相同。

```json
{
  "id": "example.content",
  "version": "1.0.0",
  "displayName": "Example Content",
  "author": "Example Author",
  "description": "Example mod",
  "dependencies": {
    "example.base": "1.0.0"
  }
}
```

`id` 和 `version` 必须是非空字符串。`displayName`、`name`、`author`、`description` 默认是空字符串，`dependencies` 默认是空字典。`displayName` 与 `name` 是独立字段，库不会自动回退或合并。

读取清单时会拒绝 JSON 根值为 `null`、字段类型错误、上述字段显式为 `null`、依赖 ID 为空或依赖版本约束为 `null` 的输入，并通过 `InvalidDataException` 报告文件路径和字段位置。可选字段可以省略；空字符串版本约束仍表示接受任意版本。

`dependencies` 将依赖 ID 映射到版本约束。默认比较规则为：

- 空约束接受任意版本，但依赖清单仍须存在。
- 实际版本和约束均能解析为 `System.Version` 时，实际版本必须大于或等于约束版本。
- 否则按区分大小写的字符串相等比较，不解析 `>=`、`^` 等范围表达式。

宿主可重写 `IsVersionSatisfied()` 定义其他版本规则。

## 使用流程

```csharp
var manager = new AssemblyLoadContextModManager(modsDirectory, (modId, exception) => {
    Console.Error.WriteLine($"Failed to unload mod '{modId}': {exception}");
});
await manager.Refresh();
await manager.Load("example.content");
bool loaded = manager.IsLoaded("example.content");
await manager.Unload("example.content");
```

### 扫描与刷新

`Refresh()` 在 Mods 目录不存在时创建目录，读取清单并检查空 ID、空版本、重复 ID 和已发现清单之间的循环依赖。扫描或检查失败时，尚未开始卸载现有实例。

扫描成功后，管理器按实际加载顺序的逆序逐个卸载现有实例。处理完旧实例后替换清单，再按原加载顺序尝试加载刷新前已加载且仍存在的 Mod。因此刷新会重新触发生命周期；首次刷新只发现清单，不自动加载全部 Mod。新发现的 Mod 可以作为原有 Mod 的依赖被加载。

重新加载期间的创建、加载异常会被收集，在处理结束后通过 `AggregateException` 抛出，已完成的状态变更不会回滚。卸载和释放错误由各自实现就地处理并记录，不参与刷新异常汇总。缺失依赖或版本不满足不会阻止清单进入 `Manifests`，但会阻止对应 Mod 加载。

### 加载与查询

`Load(id)` 递归加载依赖，再调用 `CreateMod()` 和当前实例的 `Load()`，成功后将实例加入 `Loaded`。若实例的 `Load()` 抛出异常，管理器先调用一次该实例的 `Unload()`，再调用 `ReleaseMod()`，然后重新抛出原始加载异常。两个清理方法均须自行处理并记录错误，不得向外传播异常。

失败实例不会加入 `Loaded`，再次加载时创建新实例。此前成功加载的依赖不会自动回滚。

已加载、未知 ID 或依赖不满足时，`Load(id)` 直接返回。调用完成后可用 `IsLoaded()` 或 `TryGet()` 查询结果；`TryGet()` 返回 `true` 时，输出实例保证非空。`AreDependenciesSatisfied(id)` 仅检查直接依赖的清单与版本，不递归检查，也不表示依赖已加载；未知 ID 返回 `false`。

`Manifests` 和 `Loaded` 是当前集合的只读视图，不是快照；清单对象本身可变，管理器不会复制它们。调用方应保持管理期间的清单稳定，并串行调用管理器操作，避免并发或在生命周期回调中重入修改操作。

### 卸载

Mod 作者应将业务资源清理集中在 `Unload()`，无需在 `Load()` 的异常处理中自行调用它。`Unload()` 必须支持部分初始化状态，只清理已创建的资源和已完成的注册，并自行处理、记录清理错误，不得向外传播异常。遵守串行调用、禁止重入的约定时，管理器对每个实例至多调用一次 `Unload()`。

`Unload(id)` 先递归卸载依赖当前 Mod 的其他已加载实例，再调用当前实例的 `Unload()` 并释放它。例如 A 依赖 B，加载 A 的顺序是 B → A，卸载 B 的顺序是 A → B。卸载 A 不会自动卸载 B。

未加载的 ID 直接返回。当前实例的 `Unload()` 完成后，管理器移除其记录，再调用 `ReleaseMod()`。管理器依赖清理方法不向外抛异常的契约，不再捕获或聚合清理异常；违反该契约时，后续清理流程不保证继续执行。

## 程序集加载与宿主扩展

`AssemblyLoadContextModManager` 为每个 Mod 创建独立、可回收的加载上下文，加载根目录中的顶层 `*.dll`，跳过触发 `BadImageFormatException` 的文件。在已加载程序集的公开类型中，必须恰好有一个非抽象 `Mod` 子类，并提供公开构造函数 `(ModManifest manifest, string rootDirectory)`。

构造管理器时传入 `Action<string, Exception> onUnloadError`，参数依次为 Mod ID 和上下文卸载异常。宿主通过回调记录或报告错误，回调不得向外抛异常。日志设施由宿主决定，本包不依赖日志项目。

加载器按程序集简单名称识别契约程序集，跳过 Mod 目录中的契约副本，并将契约引用解析到宿主已加载的 `Xoderony.Modding` 程序集，以保持 `Mod` 和 `ModManifest` 的类型一致。Mod 应针对宿主提供的兼容契约版本编译；打包时无需包含 `Xoderony.Modding.dll`，也不能通过携带副本替换宿主契约。

创建失败时会请求卸载上下文，然后重新抛出原始创建异常。释放实例时也会调用上下文的 `Unload()`。这两条路径共用上下文清理方法，由它捕获底层卸载异常并通知 `onUnloadError`。Mod 自身的业务资源清理由生命周期实现负责。

清单依赖用于生命周期排序与版本检查；当前实现没有配置 Mod 之间的程序集解析映射，也没有检查或禁止 Mod 引用宿主程序集。

需要其他加载方式的宿主（例如 Unity 宿主）可派生 `ModManager`，实现 `CreateMod()` / `ReleaseMod()`。`CreateMod()` 每次应创建新实例；创建失败且未返回实例时，资源清理由该实现负责，清理错误应就地处理并记录，不能覆盖原始创建异常。创建成功后，管理器在加载失败或正常卸载时先调用实例的 `Unload()`，再调用 `ReleaseMod()`；`ReleaseMod()` 实现须自行处理并记录底层释放错误，不得向外传播异常。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.modding
```

权威源码在本包 `Runtime/`；`src/Xoderony.Modding` 链接同一份源码。

## 兼容性

- .NET 项目目标框架为 `net10.0`。
- Unity 7（技术版本 `7000.0`）或更高版本
