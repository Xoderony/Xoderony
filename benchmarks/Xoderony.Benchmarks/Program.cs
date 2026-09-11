using System;
using BenchmarkDotNet.Running;

#if DEBUG
throw new InvalidOperationException("Run benchmarks with the Release configuration and without a debugger.");
#else
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif
