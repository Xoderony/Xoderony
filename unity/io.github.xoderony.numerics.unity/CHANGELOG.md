# Changelog

## [Unreleased]

- 新增独立的 Numerics Unity 适配包，提供仅在 Editor 编译的 Q16 PropertyDrawer。
- 使用 double 精度的小数编辑和分数表达式输入，按现有 Q16 契约向零截断。
- 提供延迟提交、精确值 Tooltip、混合值显示和越界输入拒绝；保持 RawValue 序列化结构不变。
