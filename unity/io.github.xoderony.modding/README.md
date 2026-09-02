# Xoderony Modding

游戏宿主与 Mod 共用的契约与默认加载约定。不依赖 `UnityEngine`。

## 流程

1. `new AssemblyLoadContextModManager(modsDirectory)`（Unity 则派生 `ModManager`）
2. `Refresh()` 扫描子目录中的 `manifest.json`，结果在 `Manifests`（给列表 UI）
3. `Load(id)` 按依赖加载程序集并构造 `Mod`，再调用 `Mod.Load()`
4. `Unload(id)` 先卸被依赖项，再 `Mod.Unload()` 并释放加载上下文

```text
modsDirectory/
  SomeMod/
    manifest.json
    *.dll
```

`manifest.json`：`id`、`version`，可选 `displayName`（或 `name`）、`author`、`description`、`dependencies`。

`AssemblyLoadContextModManager` 用可回收 `AssemblyLoadContext` 加载顶层 DLL，且必须恰好有一个公开 `Mod` 子类，构造为 `(ModManifest, string rootDirectory)`。Unity 宿主派生 `ModManager`，实现 `CreateMod` / `ReleaseMod`。

Mod 程序集只应引用本包契约，不要引用宿主游戏程序集。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.modding
```

权威源码在本包 `Runtime/`；`src/Xoderony.Modding` 链接同一份源码。

## 兼容性

- Unity 7（技术版本 `7000.0`）或更高版本
