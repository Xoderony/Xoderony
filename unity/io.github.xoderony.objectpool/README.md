# Xoderony ObjectPool

提供对象池契约、自动归还作用域和 BCL 集合池。不依赖 UnityEngine 或其他 Xoderony 包。

## 程序集与 API

- 程序集：`Xoderony.ObjectPool`。
- `Xoderony.ObjectPool`：IPool、PooledObjectScope 和 PoolExtensions。
- `Xoderony.ObjectPool.Generic`：CollectionPool、ListPool、HashSetPool、DictionaryPool、QueuePool 和 StackPool。

## 所有权与生命周期

IPool 使用 Rent/Return；调用方不得归还 null，归还后不得继续使用对象。PooledObjectScope 在 Dispose 时归还对象；不得复制作用域，Dispose 不会清空调用方持有的变量。

集合池在缓存归还的集合前清空元素；Capacity 限制池中缓存的集合数量，池满时丢弃归还对象。Shared 提供共享实例，也可以创建独立池。实现不提供线程同步。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.objectpool
```

支持 Unity 7（7000.0）和 .NET 10。权威源码位于 Runtime/，src/Xoderony.ObjectPool/Xoderony.ObjectPool.csproj 链接同一份源码。

## 从 Core 迁移

这些类型从 Xoderony.Core 移入本程序集，类型名和命名空间保持不变。调用方改为引用 Xoderony.ObjectPool 并重新编译。
