# Xoderony - 项目规则

## 规则适用与维护

- 本仓库是公开基础库；当前源码与构建配置优先于 README，README 只保留使用者需要看到的公开信息。
- 涉及陌生模块、公共契约或跨模块改动时，先读相关项目文件、入口代码、测试与可达调用方。
- `AGENTS.md` 只记录跨模块的稳定约定；一次性任务事实、具体实现状态和临时决策不写入本文件。

## 源码与构建边界

- SDK、目标框架、C# 语言版本和 nullable 模式以 `global.json`、`Directory.Build.props` 与项目文件为准；未经用户明确要求不改变这些基线。
- `src/` 中的 `.cs` 是权威源码；`unity/*/Runtime` 中的 `.cs` 由现有构建目标同步。修改实现时只编辑 `src/`，需要更新 Unity 包时使用现有同步流程，不分别维护两套源码。
- `Xoderony.Core` 不得依赖 Unity；无 Unity 依赖的实现使用 BCL 与 `System.Diagnostics.Debug.Assert`。
- Unity 包的 `package.json`、README、CHANGELOG、程序集定义和 `.meta` 是独立的发布资产，只在任务涉及对应内容时修改。

## 公共 API

- 遵循 nullable 契约；可能缺失的值使用可空类型表达，不以 `null!` 或无依据的保护代码掩盖契约。
- 公共 API 保持通用、职责清晰并避免泄漏项目侧概念；修改契约时同步检查实现、测试和 Unity 包中的公开表面。
- 接口用于表达能力边界；拥有者需要完整能力或直接调用时可以依赖具体类型，不为形式统一强制接口化。
- 优先清晰签名、直接所有权和具体类型；只有确实降低复杂度或有效重复时才增加抽象。

## 性能

- 热路径避免使用异常控制流；预期结果通过返回值表达，异常仅用于违反契约或非预期故障。
- 默认依赖 .NET 10 JIT 自动内联；仅在基准或生成代码证明必要时显式使用 `[MethodImpl(MethodImplOptions.AggressiveInlining)]`。
