# Stride UILibrary 强类型 View 规格

## 目标

生成器只从 `.sduilib` 生成可序列化的 Library 工厂和普通强类型 View。它负责实例化已导出的 UI 根并绑定资产中已命名的节点，不负责场景组件、脚本生命周期、依赖注入、挂载、输入、事件订阅、内容加载或释放。

## 公开契约

- `Assets/**/*.sduilib` 自动作为 `AdditionalFiles`；`.sduipage` 不自动收集，显式传入也会被忽略。
- Library 类型名为文件名加 `Library`，命名空间为 `RootNamespace` 加相对 `Assets` 的目录。
- Library 是带 `DataContract` 的 `public readonly struct`，通过带 `DataMember` 的 `UILibrary? Library { get; init; }` 保留 Game Studio 配置与资产引用链。
- 每个命名导出根生成 `Create{RootName}()` 和嵌套的 `public sealed {RootName}View`。
- `Create...()` 使用 Stride `InstantiateElement<T>()` 创建新子树，在返回前完成根和全部命名成员的类型、路径及名称校验。
- View 公开只读 `Root` 与命名元素属性；构造器不公开，不支持空壳对象、二阶段初始化、重绑或 `Unbind`。
- View 直接继承 `object`，不继承 `EntityComponent` / `StartupScript`，没有 `Start()` / `Cancel()` 或脚本优先级，也不实现 `IDisposable`。
- View 不加载内容、不订阅事件、不拥有 `UILibrary`。调用方拥有创建出的子树，并负责挂载、事件与对应清理。
- 每次创建返回独立根和独立可变子元素；修改一个实例不影响另一个实例或原始模板。返回根未挂载父元素。

## 装配前置条件与诊断

- Library 必须已配置且与生成代码配套；导出根、节点路径、类型及名称不得在生成后被篡改。
- 生成代码使用 `Debug.Assert` 表达这些开发期装配前置条件，不保证错误配置的异常类型或固定消息。
- 不定义 `DEBUG` 时断言调用由消费项目编译器移除；Release 仍直接执行必要的实例化、索引、转换和赋值。
- 生成代码不是不可信 Mod、热替换资源或外部任意 UILibrary 的运行时验证器；这类输入由加载边界负责校验。
- 非法配置或生成类型冲突报告 `XSUI001`。
- 无效或不支持的 UILibrary 资产报告 `XSUI002`。
- 非法、重复或保留名称报告 `XSUI003`。
- 无法解析或不支持的 UIElement 类型报告 `XSUI004`。
- 失败资产不输出部分绑定，不提供静默跳过或空 View 后备行为。

## 验证

生成器测试使用真实 Stride `UILibrary` 模板和 Roslyn 编译结果验证：

- 生成代码可编译，Library 保持可序列化配置表面；
- View 是无脚本生命周期的普通对象，成员在工厂返回时全部可用；
- 连续创建的根、命名节点及可变状态相互隔离，模板保持不变；
- 显式带 `DEBUG` 与不带 `DEBUG` 的消费编译均无 nullable 警告并能执行合法布局；仅前者的 IL 保留 `Debug.Assert` 调用；
- 命名、类型、导出根、路径和诊断行为保持编译期或创建期失败；
- MSBuild 在普通与设计时求值中只自动发现 `.sduilib`，并保留显式条目元数据。

## 兼容变化

旧版本为 `.sduipage` 生成 `StartupScript` Page View。本契约移除该生成路径且不提供兼容开关。现有消费者需将需要运行时实例化的面板改为 UILibrary 导出根，由控制器持有生成的 Library、调用 `Create...()`，再显式管理挂载和事件生命周期。没有真实消费者要求包装既有 UI 子树，因此不提供 `Bind(root)`。
