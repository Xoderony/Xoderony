using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

#if DEBUG
throw new InvalidOperationException("Run benchmarks with the Release configuration and without a debugger.");
#else
// 部分 Unity 版本的程序集带有未优化标记；基准项目及被测库仍使用 Release 编译。
var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
#endif
