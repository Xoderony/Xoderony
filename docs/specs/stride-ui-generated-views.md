# Stride UI 强类型 Page View 与 Library 生命周期规格

## Problem Statement

当前源生成器为 `.sduipage` 和 `.sduilib` 生成纯 C# 绑定类。调用方必须先取得实际 UI 根节点，再手动构造绑定对象；页面代码因而同时依赖 `UIComponent`、根节点类型和生成类型，使用入口分散。页面与 UI 库虽然都需要强类型节点访问，但前者随 Entity 和 `UIComponent` 生命周期存在，后者则是可重复实例化的模板集合，不能用同一种运行时所有权模型强行统一。

## Solution

页面资产生成可挂载的 Page View 组件。Page View 与 `UIComponent` 位于同一 Entity，在 `Start()` 中取得并验证页面根节点，然后建立一次性节点缓存；业务脚本直接引用 Page View，不再接触 `UIComponent`。

UI 库资产生成可内联序列化的 Library。Library 保存 `UILibrary` 引用，并为每个命名根提供强类型创建方法；每次调用都克隆一个独立 UI 子树并返回相应的 Library Instance View。Page View 与 Library 保留相近的强类型访问体验，但分别遵循场景组件和动态 UI 子树的真实生命周期。

## User Stories

1. 作为使用 Stride UI Page 的开发者，我希望把生成的 Page View 挂到 Entity 上，以便业务代码直接依赖页面语义而不是底层 `UIComponent`。
2. 作为业务脚本开发者，我希望通过可序列化属性引用 Page View 组件，以便在 Game Studio 中完成依赖装配。
3. 作为业务脚本开发者，我希望 Page View 在我的脚本启动前完成绑定，以便在 `Start()` 中直接使用强类型节点属性。
4. 作为场景作者，我希望缺少 `UIComponent` 或 Page 时尽早收到明确错误，以便定位错误的 Entity 配置。
5. 作为场景作者，我希望错误页面根类型或节点结构被立即拒绝，以免错误延迟到交互发生时才暴露。
6. 作为 UI 代码开发者，我希望节点属性继续返回实际 UI 实例并保持缓存，以便属性读取不重复搜索视觉树。
7. 作为 UI 代码开发者，我希望 Page View 绑定后保持同一个页面实例语义，以便节点引用不会因隐式重绑而变化。
8. 作为 UI 库使用者，我希望 Library 作为调用方成员时自动具有默认值，并能在 Game Studio 中配置 `UILibrary`，以便调用方不必手动创建包装对象或维护内容 URL。
9. 作为 UI 库使用者，我希望通过按命名根生成的 `Create...()` 方法创建强类型实例，以便不再手写导出键、根类型转换和绑定构造。
10. 作为 UI 库使用者，我希望每次 `Create...()` 都返回独立克隆，以便同一个模板能安全地用于多个 UI 实例。
11. 作为 UI 库使用者，我希望创建失败时获得包含库类型、导出根和预期类型的错误，以便识别错误资源或资产结构漂移。
12. 作为库维护者，我希望页面与库继续共享节点发现、类型解析、名称验证和缓存生成逻辑，以便核心绑定语义只有一条实现路径。
13. 作为库维护者，我希望 Page View 与 Library Instance View 使用不同的生命周期外壳，以便不把动态 UI 子树伪装成 Entity 组件。
14. 作为 NuGet 使用者，我希望资产路径唯一决定生成命名空间和类型名，以便不维护按资产配置，并让 Page View 与 Library 通过不同后缀表达各自语义。
15. 作为 NuGet 使用者，我希望目录、文件、库根和节点标识符继续保留原始大小写并报告非法名称，以便生成 API 的稳定性规则不发生旁路变化。
16. 作为维护者，我希望生成类型仍能在消费项目中直接编译并被 Stride 识别，以便不增加运行时 Xoderony 依赖。
17. 作为测试维护者，我希望通过现有生成器测试接缝验证公开行为，以便避免引入只检查生成字符串的脆弱测试。

## Implementation Decisions

- `.sduipage` 继续生成以文件名加 `View` 命名的顶层公开密封类型，但该类型改为继承 `StartupScript`，代表 Page View。
- Page View 与原生 `UIComponent` 组成同一 Entity 上的一对组件。生成类型添加 `RequireComponentAttribute` 以表达意图，但不能依赖该特性自动添加或验证组件；`Start()` 必须主动取得并检查 `UIComponent`。
- Page View 的默认脚本优先级为 `-10000`。Stride 中较小数值先执行；调用方若在自身 `Start()` 中使用 Page View，其优先级必须大于该值。场景中显式覆盖后的优先级仍以实例配置为准。
- Page View 只在 `Start()` 中按资产声明的 `Children[index]` 与 `Content` 逻辑路径绑定 UI 树，不在节点 getter 中延迟执行树绑定。节点属性在 `Start()` 成功前不可使用，并在错误访问时报告明确的未初始化错误。
- Page View 从同 Entity 的 `UIComponent.Page.RootElement` 取得实际根。缺少组件、Page 或根、根类型不兼容、路径节点缺失、节点类型不兼容以及资产命名节点的名称不匹配均视为配置错误并抛出包含逻辑路径的 `InvalidOperationException`。不遍历 `VisualChildren`，因此内部视觉节点不参与绑定。
- Page View 成功启动后缓存根和全部命名节点。属性读取返回缓存引用，不重新遍历 UI 树。
- Page View 不监视 `UIComponent.Page` 的后续变化，也不提供自动或显式重绑。一次 Page View 生命周期只对应启动时绑定的一个页面实例。
- Page View 不拥有 UI 节点、不销毁页面、不订阅业务事件。调用方继续负责解除自己建立的事件订阅。
- 业务脚本通过公开可序列化属性直接引用 Page View 组件。业务脚本不再需要引用或查询 `UIComponent`。
- `.sduilib` 生成以文件名加 `Library` 命名的顶层类型。该类型是带 Stride `DataContract` 的公开 `readonly struct`，代表 Library；值类型成员自动具有默认值，调用方不需要通过属性初始化器手动创建包装对象。
- Library 提供可空、仅初始化的 `UILibrary` 配置属性。该引用由调用方或 Game Studio 提供；生成代码不接受 `UrlReference<UIComponent>`，也不负责通过 `UrlReference<UILibrary>` 加载内容。代码需要更换资源时必须构造新的完整 Library 值。
- 不在 Library 类型上生成 `InlinePropertyAttribute`：目标 Stride 版本不允许该特性标注结构体。调用方若需要强制内联展开，应在持有 Library 的属性或字段上显式添加该特性。
- Library 不拥有传入的 `UILibrary`，不调用 Content 加载或卸载 API。
- Library 为资产中的每个命名根生成 `Create{RootName}()` 实例方法。方法使用对应导出键和具体根类型实例化 UI 子树，随后按资产 `Children[index]` 与 `Content` 逻辑路径完成节点验证与缓存。
- 每次 `Create{RootName}()` 调用都创建独立克隆，不缓存或复用此前创建的 Library Instance View。
- 每个库根继续生成嵌套的公开密封 Library Instance View 类型。该类型持有实际克隆根及命名节点缓存，但不参与 Stride 序列化，也不是 Entity 组件。
- Library Instance View 的构造器不公开，只能由所属 Library 的对应创建方法构造，从而保证克隆和绑定沿同一入口完成。
- Library Instance View 的 `Root` 和节点属性保持非空、具体 Stride UI 类型和只读语义。
- 不为 Page View、Library 和 Library Instance View 引入共同基类或接口；相近的强类型访问体验不掩盖三者不同的所有权与生命周期。
- 页面和库继续复用同一个内部路径收集、关系验证、类型解析、名称验证和绑定代码生成路径；仅外层类型形态与初始化入口按资产种类分支。
- 现有资产发现行为保持不变：包的 MSBuild 文件收集 `Assets/**/*.sduipage` 与 `Assets/**/*.sduilib` 作为 `AdditionalFiles`，保留已有条目元数据并避免重复。
- 生成命名空间固定为 `RootNamespace` 加相对 `Assets` 的目录；Page View 类型名固定为文件名加 `View`，Library 类型名固定为文件名加 `Library`。后缀始终追加，不检测文件名是否已经包含后缀。
- 不提供按资产覆盖生成命名空间或类型名的元数据入口。非法资产目录或文件名报告 `XSUI003`；生成类型完整名称冲突报告 `XSUI001`。
- 现有标识符规则保持不变：保留大小写，关键字转义，非法标识符与最终类型冲突报告诊断，不自动清理、改写或追加数字。
- 现有资产解析和诊断支持边界保持不变，除非新公开类型形态要求更新保留成员名冲突检查。
- README 应分别说明 Page View 的组件装配与优先级契约、Library 的序列化配置，以及 Library Instance View 的创建和所有权。

## Testing Decisions

- 主要测试接缝沿用现有 Roslyn generator driver：输入代表性的 `.sduipage` 或 `.sduilib` AdditionalText，编译生成代码，再通过符号或反射验证公开类型和运行时行为。
- 测试关注外部契约，不断言无业务意义的完整生成字符串、私有字段名或局部实现顺序。
- 页面测试应验证生成类型继承 `StartupScript`、表达对 `UIComponent` 的组件需求并具有约定的默认优先级。
- 页面运行时测试应构造 Entity、`UIComponent`、`UIPage` 和真实 Stride UI 根，挂载生成的 Page View，调用 `Start()`，并验证根与命名节点属性返回原始实例。
- 页面失败测试应覆盖缺少 `UIComponent`、Page 为空、根类型错误、逻辑路径 Children 索引缺失、Content 为空、节点类型错误和按路径取得的节点名称不匹配。
- 页面生命周期测试应验证绑定后修改视觉树不会改变已缓存引用，并验证更换 `UIComponent.Page` 不会隐式重绑。
- 消费代码编译测试应验证业务脚本可以公开引用生成的 Page View，并在不引用 `UIComponent` 的情况下访问强类型节点。
- 库类型测试应验证生成的 Library 是带可序列化契约的只读值类型，并公开仅初始化的 `UILibrary` 配置属性，而不是组件或静态容器。
- 库创建测试应使用真实 `UILibrary` 模板验证 `Create{RootName}()` 返回正确嵌套类型、克隆实例而非模板引用，并按逻辑路径缓存克隆树中的节点。
- 库独立性测试应验证同一创建方法连续调用得到不同根和不同节点实例。
- 库失败测试应覆盖未配置 `UILibrary`、传入错误库、缺少导出根、根类型错误以及克隆结构不兼容。
- 库公开表面测试应验证 Library Instance View 不能由调用方直接构造，同时其 `Root` 和节点属性保持公开只读。
- 既有测试继续覆盖目录命名空间推导、固定的 `View` / `Library` 后缀、非法名称、类型冲突、资产移动/改名/删除导致旧代码失效，以及页面与库根的独立名称作用域。
- MSBuild 资产自动发现继续通过现有资产发现测试验证，不因运行时类型形态调整而创建第二套收集测试接缝。
- 按仓库约定，本规格不要求在设计阶段执行测试；实现完成后的验证命令由用户明确授权后运行。

## Out of Scope

- 不修改 Stride 的 `RequireComponentAttribute` 行为，也不尝试自动向 Entity 添加 `UIComponent`。
- 不让 Page View 创建、替换或拥有 `UIComponent.Page`。
- 不支持 Page View 自动检测或重新绑定运行时更换的 Page。
- 不把 Library 或 Library Instance View 挂到 Entity。
- 不让 Library Instance View 参与场景或 Prefab 序列化。
- 不由生成器加载、缓存或卸载 `UILibrary` Content。
- 不公开接受任意根节点的 Library Instance View 构造器。
- 不为 Page View、Library 和 Library Instance View 增加共同运行时基类、服务定位器、全局注册表或隐藏的自动初始化处理器。
- 不迁移其他项目的 UI 资产，不处理 UI 编辑器裁剪问题。
- 不扩大既有外部基资产合并、自定义 DataContract 别名、程序集限定标签或自定义子节点存储支持范围；只支持可由 Panel `Children` 或 ContentControl `Content` 表达的资产关系。
- 不改变资产发现目录、开关或排除规则。

## Further Notes

- `RequireComponentAttribute` 在目标 Stride 版本中仅作为声明性意图使用，生成代码的正确性必须来自 `Start()` 中的显式检查。
- `StartupScript.Priority` 是可序列化实例配置；生成类型提供默认值，但不能阻止场景作者覆盖。README 必须明确调用脚本与 Page View 的优先级关系。
- `View` 后缀只表示实际 UI 实例；Library 使用 `Library` 后缀，Library Instance View 继续使用 `View` 后缀。
- 该设计取代当前要求调用方手动把根节点传给纯 C# 绑定构造器的公开使用方式。
