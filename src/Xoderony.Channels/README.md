# Xoderony Channels

提供委托订阅与分发通道，以及共享值的读写通道。不依赖其他 Xoderony 包。

## 程序集与 API

- 程序集：`Xoderony.Channels`；命名空间：`Xoderony`。
- `DelegateChannel<TDelegate>`：通过 `IDelegateSubscriber<TDelegate>` 订阅，通过 `IDelegateDispatcher<TDelegate>.Handlers` 访问委托并由调用方执行。
- `ValueChannel<T>`：通过 `IValueReader<T>` 和 `IValueWriter<T>` 共享同一存储。
- `ValueChannelMap<T>`：按 int 键获取或创建值通道。

通道实例由调用方创建和管理；订阅与取消订阅应成对处理。通道不提供跨线程同步或消息队列语义。

## 安装

通过 NuGet 引用 `Xoderony.Channels`；本地开发可直接引用同目录的项目文件。

## 从 Core 迁移

这些类型从 Xoderony.Core 移入本程序集，类型名和命名空间保持不变。调用方改为引用 Xoderony.Channels 并重新编译。
