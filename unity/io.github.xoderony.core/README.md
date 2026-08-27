# Xoderony Core

可复用的基础集合、序列化缓冲、委托通道、通用扩展和对象池。不依赖 `UnityEngine`。

## 程序集

- `Xoderony.Core`

根命名空间是 `Xoderony`，并按职责使用 `Xoderony.Collections`、`Xoderony.Serialization`、`Xoderony.Extensions`、`Xoderony.ObjectPool` 等子命名空间。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.core
```

权威源码在仓库 `src/Xoderony.Core/`；编译 `net10.0` 后同步到本目录。

## 兼容性

- Unity 7（技术版本 `7000.0`）或更高版本

## 对象池约定

- `IPool<T>`：Rent/Return；调用方不得归还 null，归还后不得继续使用对象。
- `PooledObjectScope<T>`：自动归还作用域。
- 集合池归还时清空元素；容量固定；`Shared` 为共享实例。
- `CollectionPool` 供 List/HashSet/Dictionary 共用；Stack/Queue 单独实现。

## 设计边界

本包不包含依赖特定第三方包或 Unity 子系统的集成代码。Unity 对象池、Netcode、ZString、Hjson 等扩展由其他 Xoderony 包提供。
