# Changelog

## [Unreleased]

- 其余向量、Quaternion、Color、Color32 和 Rect 的编解码改为完整切片后按固定偏移读写，统一推进位置；公开字段类型的解码直接初始化字段，保持原有二进制格式。
- Vector3 解码改为固定偏移读取并直接初始化字段，减少范围检查、位置更新和构造函数调用，保持小端 xyz 格式。
- 新增独立的 Serialization Unity 运行时适配包，单向依赖 Serialization。
- 提供 Vector2、Vector3、Vector4、Vector2Int、Vector3Int、Quaternion、Color、Color32、Rect 和 Bounds 的固定格式 Codec 与 ByteCount。
- 明确小端分量顺序、Bounds 的 center/extents 表示及调用方缓冲区契约，提供安装和组合读写示例。
