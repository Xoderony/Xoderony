# Changelog

## [Unreleased]

- JSON 语言表改为 `keys.json` 键权威 + 各 Locale 扁平译文文件，移除多文件嵌套树合并。
- 新增 `Xoderony.Localization` / `io.github.xoderony.localization` 最小包骨架。
- 新增基于 `CultureInfo` 的固定 culture 契约、字符串查询契约和不可变默认实现。
- 新增 `Xoderony.Localization.Tooling` 编辑器与构建期工具程序集，以及手动调用的强类型本地化键生成器。
- 将旧语言表数据层完整迁移为 `Xoderony.Localization.Json`，改用严格标准 JSON，并移除旧解析器依赖与预编译 DLL 引用。
- 更新独立 WPF `Xoderony.Localization.Editor`，用于直接编辑 JSON 语言表目录。
