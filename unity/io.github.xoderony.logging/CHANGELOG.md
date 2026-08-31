# Changelog

## [Unreleased]

- 新增无 Unity 依赖的通用日志核心、`ILogger` 实例契约与日志扩展。
- 新增基于 `Debug.unityLogger` 的 Unity 日志器和默认便捷扩展。
- `UnityDebugLogger` 作为公开的无状态 Unity 后端，可用于依赖注入和显式日志器调用。
- 支持 `Debug`、`Information`、`Warning`、`Error` 与 `Critical` 日志等级。
- 日志自动添加编译期文件名与调用成员名；Unity 对象自动作为 Console context。
- 普通消息与插值消息均在日志级别被过滤时避免构造最终消息。
