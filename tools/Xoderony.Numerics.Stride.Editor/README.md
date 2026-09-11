# Xoderony.Numerics.Stride.Editor

Stride Game Studio 4.4.0-beta6 的 Q16 小数属性编辑器。通用 Q16 类型不依赖 Stride，资产仍保存 RawValue。

本项目已加入 Expriverse.slnx。关闭 Game Studio，编译本项目后重新打开 Expriverse.slnx；Game Studio 在加载解决方案库程序集时注册编辑器。修改此编辑器后需要重启 Game Studio。

在脚本中正常声明 Q16 属性即可，不需要逐个属性添加适配代码：

```csharp
public Q16 Multiplier { get; set; } = Q16.One;
```

属性面板以原属性名称显示小数输入，隐藏 RawValue 子树。编辑范围为 -32768 至 32767.9999847412109375，最小步长为 1/65536；输入按 Q16 精度向零截断。显示使用 double，能够精确往返所有 Q16 原始值。

回写通过原属性节点完成，使用 Stride 的撤销、重做和资产持久化机制；预制体继承状态与重置操作转发给原节点。

此项目只供 Game Studio 使用，不添加到 Expriverse.Game 或 Windows 启动项目的运行时引用中。NuGet 包仅供编辑器扩展项目引用，不能当作普通游戏运行时包导入。
