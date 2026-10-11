## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.Initialization.InitializationBenchmarks.CreateConventional()
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       rcx,offset MT_Xoderony.Benchmarks.Initialization.InitializationBenchmarks+Conventional
       call      CORINFO_HELP_NEWSFAST
       mov       rsi,rax
       mov       rdx,[rbx+8]
       mov       rdi,[rbx+10]
       mov       rbx,[rbx+18]
       lea       rcx,[rsi+8]
       call      CORINFO_HELP_ASSIGN_REF
       lea       rcx,[rsi+10]
       mov       rdx,rdi
       call      CORINFO_HELP_ASSIGN_REF
       lea       rcx,[rsi+18]
       mov       rdx,rbx
       call      CORINFO_HELP_ASSIGN_REF
       mov       dword ptr [rsi+20],7
       mov       rax,rsi
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 91
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.Initialization.InitializationBenchmarks.CreatePrimary()
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       rcx,offset MT_Xoderony.Benchmarks.Initialization.InitializationBenchmarks+Primary
       call      CORINFO_HELP_NEWSFAST
       mov       rsi,rax
       mov       rdx,[rbx+8]
       mov       rdi,[rbx+10]
       mov       rbx,[rbx+18]
       lea       rcx,[rsi+8]
       call      CORINFO_HELP_ASSIGN_REF
       lea       rcx,[rsi+10]
       mov       rdx,rdi
       call      CORINFO_HELP_ASSIGN_REF
       lea       rcx,[rsi+18]
       mov       rdx,rbx
       call      CORINFO_HELP_ASSIGN_REF
       mov       dword ptr [rsi+20],7
       mov       rax,rsi
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 91
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.Initialization.InitializationBenchmarks.CreateRequiredInit()
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rbx,rcx
       mov       rcx,offset MT_Xoderony.Benchmarks.Initialization.InitializationBenchmarks+RequiredInit
       call      CORINFO_HELP_NEWSFAST
       mov       rsi,rax
       mov       rdx,[rbx+8]
       lea       rcx,[rsi+8]
       call      CORINFO_HELP_ASSIGN_REF
       mov       rdx,[rbx+10]
       lea       rcx,[rsi+10]
       call      CORINFO_HELP_ASSIGN_REF
       mov       rdx,[rbx+18]
       lea       rcx,[rsi+18]
       call      CORINFO_HELP_ASSIGN_REF
       mov       dword ptr [rsi+20],7
       mov       rax,rsi
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
; Total bytes of code 83
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.Initialization.InitializationBenchmarks.ReadConventional()
       mov       rax,[rcx+20]
       mov       rcx,[rax+8]
       mov       ecx,[rcx+10]
       mov       rdx,[rax+10]
       add       ecx,[rdx+10]
       mov       rdx,[rax+18]
       add       ecx,[rdx+10]
       add       ecx,[rax+20]
       mov       eax,ecx
       ret
; Total bytes of code 31
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.Initialization.InitializationBenchmarks.ReadPrimary()
       mov       rax,[rcx+28]
       mov       rcx,[rax+8]
       mov       ecx,[rcx+10]
       mov       rdx,[rax+10]
       add       ecx,[rdx+10]
       mov       rdx,[rax+18]
       add       ecx,[rdx+10]
       add       ecx,[rax+20]
       mov       eax,ecx
       ret
; Total bytes of code 31
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.Initialization.InitializationBenchmarks.ReadRequiredInit()
       mov       rax,[rcx+30]
       mov       rcx,[rax+8]
       mov       ecx,[rcx+10]
       mov       rdx,[rax+10]
       add       ecx,[rdx+10]
       mov       rdx,[rax+18]
       add       ecx,[rdx+10]
       add       ecx,[rax+20]
       mov       eax,ecx
       ret
; Total bytes of code 31
```

