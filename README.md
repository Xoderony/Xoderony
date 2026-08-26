# Xoderony

GameKit 全家桶仓库，面向 **Unity 7**（CoreCLR / .NET 10）和其它 .NET 10 项目。当前只发布 **Core** 这一层纯 BCL（集合、委托通道、扩展、对象池），不依赖 `UnityEngine`。

其它 Xoderony 包（Unity / logging / jog / networking 等）仍留在各自项目里。

## 安装

### Unity（UPM Git）

Window → Package Manager → **Add package from git URL**：

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.core
```

可锁定分支：

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.core#main
```

### .NET（NuGet）

```bash
dotnet add package Xoderony.Core
```

NuGet 目标框架：`net10.0`。UPM 清单最低编辑器：`7000.0`（Unity 7）。尚未推送到 nuget.org 时可本地打包：

```bash
dotnet pack src/Xoderony.Core/Xoderony.Core.csproj -c Release
```

## 目录

```text
src/Xoderony.Core/              # 权威源码 → NuGet
unity/io.github.xoderony.core/  # UPM（编译后从 src 同步）
tests/Xoderony.Core.Tests/
Xoderony.sln
```

在 VS 或 Cursor 里编译 `Xoderony.Core` 会把 `.cs` 拷到 UPM `Runtime/`（与 ZString 相同）。

## 开发

```bash
dotnet build Xoderony.sln
dotnet test Xoderony.sln
```

默认分支是 `main`。
