## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: Job-JESZGN(IterationCount=8, IterationTime=300ms, LaunchCount=2, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks.Assertions()
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,0A8
       xor       eax,eax
       mov       [rsp+38],rax
       vxorps    xmm4,xmm4,xmm4
       vmovdqu   ymmword ptr [rsp+40],ymm4
       vmovdqu   ymmword ptr [rsp+60],ymm4
       vmovdqu   ymmword ptr [rsp+80],ymm4
       mov       [rsp+0A0],rax
       mov       rbx,rcx
       xor       eax,eax
       mov       rsi,[rbx+10]
       mov       [rsp+30],rsi
       xor       edi,edi
       cmp       [rsi+8],edi
       jg        near ptr M00_L11
M00_L00:
       vzeroupper
       add       rsp,0A8
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       ret
M00_L01:
       mov       r15d,eax
M00_L02:
       mov       ecx,[rsp+70]
       imul      edx,r15d,0C2B2AE3D
       add       edx,0D6BAB35E
       rol       edx,11
       imul      edx,27D4EB2F
       imul      ecx,0C2B2AE3D
       add       ecx,edx
       rol       ecx,11
       imul      ecx,27D4EB2F
       mov       edx,ecx
       shr       edx,0F
       xor       edx,ecx
       imul      ecx,edx,85EBCA77
       mov       edx,ecx
       shr       edx,0D
       xor       edx,ecx
       imul      ecx,edx,0C2B2AE3D
       mov       r15d,ecx
       shr       r15d,10
       xor       r15d,ecx
       xor       ecx,ecx
       mov       [rsp+60],rcx
       mov       rcx,[r14+8]
       mov       edx,r15d
       imul      rdx,[r14+30]
       shr       rdx,20
       inc       rdx
       mov       eax,[rcx+8]
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,20
       cmp       edx,eax
       jae       near ptr M00_L40
       mov       edx,edx
       lea       rcx,[rcx+rdx*4+10]
       mov       r13d,[rcx]
       mov       r14,[r14+10]
       xor       r12d,r12d
       dec       r13d
       mov       eax,[r14+8]
       cmp       eax,r13d
       jbe       near ptr M00_L14
M00_L03:
       mov       ecx,r13d
       shl       rcx,5
       lea       r13,[r14+rcx+10]
       cmp       [r13+8],r15d
       jne       near ptr M00_L13
       mov       rcx,[r13+10]
       mov       eax,[r13+18]
       mov       [rsp+5C],eax
       test      rcx,rcx
       je        near ptr M00_L18
       test      rbp,rbp
       je        near ptr M00_L13
       mov       rdx,offset MT_System.RuntimeType
       cmp       [rcx],rdx
       jne       near ptr M00_L17
       cmp       rbp,rcx
       sete      r8b
       movzx     r8d,r8b
M00_L04:
       test      r8d,r8d
       je        near ptr M00_L13
M00_L05:
       cmp       dword ptr [rsp+5C],0
       jne       near ptr M00_L13
M00_L06:
       test      r13,r13
       je        near ptr M00_L23
       mov       rcx,[r13]
       cmp       qword ptr [rcx+8],0
       jne       near ptr M00_L24
       mov       r14,[rcx+18]
M00_L07:
       mov       rcx,offset MT_System.RuntimeType
       cmp       [rbp],rcx
       jne       near ptr M00_L27
       mov       [rsp+20],r14
       mov       rcx,[rbp+18]
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+28]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L25
M00_L08:
       cmp       ebx,1D
       je        near ptr M00_L26
M00_L09:
       mov       r14,[rsp+20]
       mov       rax,r14
M00_L10:
       test      rax,rax
       je        near ptr M00_L31
       inc       edi
       mov       rsi,[rsp+30]
       cmp       [rsi+8],edi
       mov       rbx,[rsp+0F0]
       jle       near ptr M00_L00
M00_L11:
       mov       rbp,[rsi+rdi*8+10]
       mov       [rsp+28],rbp
       mov       [rsp+0F0],rbx
       mov       rcx,[rbx+8]
       cmp       [rcx],cl
       vxorps    ymm0,ymm0,ymm0
       vmovdqu   ymmword ptr [rsp+80],ymm0
       vmovdqu   xmmword ptr [rsp+98],xmm0
       mov       r14,[rcx+8]
       mov       rax,rbp
       cmp       qword ptr [r14+8],0
       je        near ptr M00_L14
       mov       r15,[r14+18]
       test      r15,r15
       jne       near ptr M00_L19
       mov       [rsp+68],rax
       xor       ecx,ecx
       mov       [rsp+70],ecx
       mov       [rsp+60],rcx
       lea       rcx,[rsp+68]
       cmp       qword ptr [rsp+60],0
       jne       short M00_L12
       mov       rcx,[rsp+68]
       mov       [rsp+60],rcx
       lea       rcx,[rsp+60]
       cmp       qword ptr [rsp+60],0
       je        short M00_L15
M00_L12:
       mov       r15,[rcx]
       mov       rcx,offset MT_System.RuntimeType
       cmp       [r15],rcx
       jne       short M00_L16
       mov       rcx,r15
       call      00007FFF68F03E20
       test      eax,eax
       jne       near ptr M00_L01
       mov       rcx,r15
       call      qword ptr [7FFF094D5FF8]; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       jmp       near ptr M00_L01
M00_L13:
       mov       r13d,[r13+0C]
       inc       r12d
       mov       eax,[r14+8]
       cmp       eax,r12d
       jb        near ptr M00_L30
       cmp       eax,r13d
       ja        near ptr M00_L03
M00_L14:
       xor       r13d,r13d
       jmp       near ptr M00_L06
M00_L15:
       xor       r15d,r15d
       jmp       near ptr M00_L02
M00_L16:
       mov       rcx,r15
       mov       rax,[r15]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       mov       r15d,eax
       jmp       near ptr M00_L02
M00_L17:
       mov       rdx,rbp
       mov       r8,[rcx]
       mov       r8,[r8+40]
       call      qword ptr [r8+10]
       mov       r8d,eax
       jmp       near ptr M00_L04
M00_L18:
       test      rbp,rbp
       jne       short M00_L13
       jmp       near ptr M00_L05
M00_L19:
       mov       [rsp+48],rax
       xor       edx,edx
       mov       [rsp+50],edx
       lea       rdx,[rsp+48]
       mov       rcx,r15
       mov       r11,7FFF09250568
       call      qword ptr [r11]
       mov       r13d,eax
       mov       r8,[r14+8]
       mov       edx,r13d
       imul      rdx,[r14+30]
       shr       rdx,20
       inc       rdx
       mov       ecx,[r8+8]
       imul      rdx,rcx
       shr       rdx,20
       cmp       edx,[r8+8]
       jae       near ptr M00_L40
       mov       edx,edx
       lea       r8,[r8+rdx*4+10]
       mov       r12d,[r8]
       mov       r14,[r14+10]
       xor       eax,eax
       mov       [rsp+7C],eax
       dec       r12d
M00_L20:
       cmp       [r14+8],r12d
       jbe       near ptr M00_L14
       mov       r8d,r12d
       shl       r8,5
       lea       r8,[r14+r8+10]
       mov       r12,r8
       cmp       [r12+8],r13d
       jne       short M00_L21
       vmovdqu   xmm0,xmmword ptr [r12+10]
       vmovdqu   xmmword ptr [rsp+48],xmm0
       mov       [rsp+38],rbp
       xor       r8d,r8d
       mov       [rsp+40],r8d
       lea       r8,[rsp+38]
       lea       rdx,[rsp+48]
       mov       rcx,r15
       mov       r11,7FFF09250570
       call      qword ptr [r11]
       test      eax,eax
       jne       short M00_L22
M00_L21:
       mov       r12d,[r12+0C]
       mov       eax,[rsp+7C]
       inc       eax
       cmp       [r14+8],eax
       mov       [rsp+7C],eax
       jae       short M00_L20
       jmp       near ptr M00_L30
M00_L22:
       mov       r13,r12
       jmp       near ptr M00_L06
M00_L23:
       xor       eax,eax
       jmp       near ptr M00_L10
M00_L24:
       call      qword ptr [7FFF095D5968]; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       mov       [rsp+20],rax
       mov       r14,[rsp+20]
       jmp       near ptr M00_L07
M00_L25:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L08
M00_L26:
       mov       r14,[rsp+20]
       jmp       short M00_L28
M00_L27:
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+60]
       call      qword ptr [rax+18]
       test      eax,eax
       je        short M00_L29
M00_L28:
       mov       rdx,r14
       mov       rcx,offset MT_System.Collections.ICollection
       call      qword ptr [7FFF094DECA0]; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       mov       r14,rax
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+68]
       call      qword ptr [rax+8]
       mov       r15,rax
       mov       rcx,r14
       mov       r11,7FFF09250558
       call      qword ptr [r11]
       mov       edx,eax
       mov       rcx,r15
       call      qword ptr [7FFF095D5BF0]; System.Array.CreateInstance(System.Type, Int32)
       mov       r15,rax
       mov       rcx,r14
       mov       rdx,r15
       mov       r11,7FFF09250560
       xor       r8d,r8d
       call      qword ptr [r11]
       mov       rax,r15
       jmp       near ptr M00_L10
M00_L29:
       mov       [rsp+20],r14
       jmp       near ptr M00_L09
M00_L30:
       call      qword ptr [7FFF0930F480]
       int       3
M00_L31:
       lea       rcx,[rsp+80]
       mov       edx,1D
       mov       r8d,2
       call      qword ptr [7FFF0930C270]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       mov       ecx,[rsp+90]
       cmp       ecx,[rsp+0A0]
       ja        near ptr M00_L36
       mov       rdx,[rsp+98]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0A0]
       sub       eax,ecx
       cmp       eax,14
       jb        short M00_L32
       vmovups   ymm0,[7FFF09387D20]
       vmovups   [rdx],ymm0
       mov       rcx,200072006F0066
       mov       [rdx+20],rcx
       mov       ecx,[rsp+90]
       add       ecx,14
       mov       [rsp+90],ecx
       jmp       short M00_L33
M00_L32:
       lea       rcx,[rsp+80]
       mov       rdx,1440CF2B920
       call      qword ptr [7FFF0966E340]
M00_L33:
       lea       rcx,[rsp+80]
       mov       r8,rbp
       mov       rdx,7FFF09692DA0
       call      qword ptr [7FFF094DEA60]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       mov       ecx,[rsp+90]
       cmp       ecx,[rsp+0A0]
       ja        near ptr M00_L36
       mov       rdx,[rsp+98]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0A0]
       sub       eax,ecx
       cmp       eax,7
       jb        short M00_L34
       mov       rcx,65006B00280020
       mov       [rdx],rcx
       mov       dword ptr [rdx+8],3A0079
       mov       word ptr [rdx+0C],20
       mov       ecx,[rsp+90]
       add       ecx,7
       mov       [rsp+90],ecx
       jmp       short M00_L35
M00_L34:
       lea       rcx,[rsp+80]
       mov       rdx,1440CF2B960
       call      qword ptr [7FFF0966E340]
M00_L35:
       lea       rcx,[rsp+80]
       xor       edx,edx
       call      qword ptr [7FFF09307FD8]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       mov       ecx,[rsp+90]
       cmp       ecx,[rsp+0A0]
       jbe       short M00_L37
M00_L36:
       call      qword ptr [7FFF094D7D50]
       int       3
M00_L37:
       mov       rdx,[rsp+98]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0A0]
       sub       eax,ecx
       cmp       eax,2
       jb        short M00_L38
       mov       dword ptr [rdx],2E0029
       mov       ecx,[rsp+90]
       add       ecx,2
       mov       [rsp+90],ecx
       jmp       short M00_L39
M00_L38:
       lea       rcx,[rsp+80]
       mov       rdx,1440CF29BF8
       call      qword ptr [7FFF0966E340]
M00_L39:
       mov       rcx,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rcx,[rsp+80]
       call      qword ptr [7FFF0930C2A0]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFF095D7BD0]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L40:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 1651
```
```assembly
; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,88
       lea       rbp,[rsp+0C0]
       mov       [rbp+10],rcx
       cmp       qword ptr [rbp+10],0
       je        short M01_L01
       lea       rcx,[rbp+10]
       mov       [rbp-98],rcx
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-98]
       call      qword ptr [rax]
       mov       ebx,eax
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       eax,ebx
M01_L00:
       add       rsp,88
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M01_L01:
       xor       eax,eax
       jmp       short M01_L00
; Total bytes of code 119
```
```assembly
; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       push      rsi
       push      rbx
       sub       rsp,28
       mov       [rsp+20],rcx
       mov       rbx,rcx
M02_L00:
       mov       rsi,[rbx+8]
       test      rsi,rsi
       je        short M02_L01
       mov       ecx,[rsi+10]
       cmp       ecx,8
       jne       short M02_L03
       mov       rcx,rbx
       mov       rdx,rsi
       xor       r8d,r8d
       call      qword ptr [7FFF68D7E1B0]
M02_L01:
       cmp       qword ptr [rbx+8],0
       jne       short M02_L02
       mov       rax,[rbx+18]
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M02_L02:
       mov       rcx,rbx
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax]
M02_L03:
       cmp       ecx,8
       ja        short M02_L04
       mov       ecx,ecx
       lea       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       edx,[rdx+rcx*4]
       lea       r8,[M02_L00]
       add       rdx,r8
       jmp       rdx
M02_L04:
       mov       rcx,[rsi+8]
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D74C78]
       int       3
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1A0]
       jmp       short M02_L01
       mov       rcx,rbx
       xor       edx,edx
       call      qword ptr [7FFF68D7E1A8]; Precode of System.Lazy`1[[System.__Canon, System.Private.CoreLib]].ViaFactory(System.Threading.LazyThreadSafetyMode)
       jmp       short M02_L01
       mov       rcx,[rbx]
       call      qword ptr [7FFF68D5B838]
       mov       rcx,rax
       call      qword ptr [7FFF68D62EA0]
       mov       rcx,rax
       call      qword ptr [7FFF68D84008]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
       mov       rax,[rbx+10]
       test      rax,rax
       je        short M02_L05
       mov       rcx,[rax+8]
       call      qword ptr [rax+18]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
M02_L05:
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1C8]
       jmp       near ptr M02_L01
       mov       rcx,rbx
       mov       rdx,rsi
       mov       r8d,1
       call      qword ptr [7FFF68D7E1B0]
       jmp       near ptr M02_L01
; Total bytes of code 269
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       test      rdx,rdx
       je        short M03_L02
       mov       rax,[rdx]
       movzx     r8d,word ptr [rax+0E]
       test      r8,r8
       je        short M03_L05
       mov       rax,[rax+38]
       cmp       r8,4
       jl        short M03_L01
       cmp       [rax],rcx
       je        short M03_L02
M03_L00:
       cmp       [rax+8],rcx
       je        short M03_L02
       cmp       [rax+10],rcx
       je        short M03_L02
       cmp       [rax+18],rcx
       je        short M03_L02
       add       rax,20
       add       r8,0FFFFFFFFFFFFFFFC
       cmp       r8,4
       jge       short M03_L04
       test      r8,r8
       je        short M03_L05
M03_L01:
       cmp       [rax],rcx
       jne       short M03_L03
M03_L02:
       mov       rax,rdx
       ret
M03_L03:
       add       rax,8
       dec       r8
       test      r8,r8
       jle       short M03_L05
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L03
M03_L04:
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L00
M03_L05:
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       jmp       qword ptr [rax]
; Total bytes of code 115
```
```assembly
; System.Array.CreateInstance(System.Type, Int32)
       sub       rsp,28
       mov       [rsp+38],edx
       test      rcx,rcx
       je        short M04_L01
       mov       eax,[rsp+38]
       test      eax,eax
       jl        short M04_L02
       mov       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       cmp       r11,[rcx]
       jne       short M04_L03
M04_L00:
       call      qword ptr [7FFF68D67080]
       test      rax,rax
       je        short M04_L04
       lea       r8,[rsp+38]
       mov       rcx,rax
       mov       edx,1
       xor       r9d,r9d
       call      qword ptr [7FFF68D6A298]; Precode of System.Array.InternalCreate(System.RuntimeType, Int32, Int32*, Int32*)
       nop
       add       rsp,28
       ret
M04_L01:
       mov       rcx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rcx]
       call      qword ptr [7FFF68D6C210]
       int       3
M04_L02:
       mov       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rdx,[rdx]
       mov       ecx,eax
       call      qword ptr [7FFF68D81908]
       int       3
M04_L03:
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rcx,rax
       jmp       short M04_L00
M04_L04:
       mov       ecx,39
       mov       edx,4E
       call      qword ptr [7FFF68D6F3B0]
       int       3
; Total bytes of code 140
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       esi,edx
       mov       edi,r8d
       xor       eax,eax
       mov       [rbx],rax
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       imul      edx,edi,0B
       add       edx,esi
       mov       eax,100
       cmp       edx,100
       cmovle    edx,eax
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798B8]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Rent(Int32)
       mov       [rbx+8],rax
       test      rax,rax
       je        short M05_L01
       lea       rcx,[rax+10]
       mov       eax,[rax+8]
M05_L00:
       mov       [rbx+18],rcx
       mov       [rbx+20],eax
       xor       eax,eax
       mov       [rbx+10],eax
       mov       byte ptr [rbx+14],0
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
M05_L01:
       xor       ecx,ecx
       xor       eax,eax
       jmp       short M05_L00
; Total bytes of code 102
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       push      rsi
       push      rbx
       sub       rsp,58
       xor       eax,eax
       mov       [rsp+28],rax
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       mov       [rsp+40],rax
       mov       [rsp+50],rdx
       mov       rbx,rcx
       mov       rcx,rdx
       mov       rsi,r8
       cmp       byte ptr [rbx+14],0
       jne       near ptr M06_L05
       test      rsi,rsi
       je        near ptr M06_L06
       mov       rcx,rsi
       call      qword ptr [7FFF68D67170]
       test      rax,rax
       jne       short M06_L01
       mov       rcx,rsi
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rdx,rax
M06_L00:
       test      rdx,rdx
       je        near ptr M06_L06
       lea       r8,[rbx+18]
       mov       ecx,[rbx+10]
       mov       eax,[r8+8]
       cmp       ecx,eax
       ja        near ptr M06_L07
       mov       r8,[r8]
       mov       r10d,ecx
       lea       r10,[r8+r10*2]
       sub       eax,ecx
       mov       esi,[rdx+8]
       cmp       esi,eax
       ja        near ptr M06_L08
       mov       r8d,esi
       add       r8,r8
       add       rdx,0C
       mov       rcx,r10
       call      qword ptr [7FFF68D6D928]; Precode of System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       add       [rbx+10],esi
       jmp       near ptr M06_L06
M06_L01:
       mov       rcx,rsi
       call      qword ptr [7FFF68D671A8]
       test      rax,rax
       je        near ptr M06_L04
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E8]
       mov       rsi,rax
M06_L02:
       mov       rcx,rsi
       lea       rdx,[rbx+18]
       mov       r9d,[rbx+10]
       mov       r8d,[rdx+8]
       cmp       r9d,r8d
       ja        near ptr M06_L07
       mov       rdx,[rdx]
       mov       r11d,r9d
       lea       rdx,[rdx+r11*2]
       sub       r8d,r9d
       mov       [rsp+38],rdx
       mov       [rsp+40],r8d
       xor       edx,edx
       mov       [rsp+28],rdx
       mov       [rsp+30],edx
       mov       rdx,[rbx]
       mov       [rsp+20],rdx
       lea       rdx,[rsp+38]
       lea       r9,[rsp+28]
       lea       r8,[rsp+48]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       test      eax,eax
       jne       short M06_L03
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M06_L02
M06_L03:
       mov       ecx,[rsp+48]
       add       [rbx+10],ecx
       jmp       short M06_L06
M06_L04:
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E0]
       mov       rcx,rax
       mov       r8,[rbx]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       xor       edx,edx
       call      qword ptr [r11]
       mov       rdx,rax
       jmp       near ptr M06_L00
M06_L05:
       call      qword ptr [7FFF68D600E8]
       mov       rdx,rax
       mov       rcx,rbx
       mov       r8,rsi
       xor       r9d,r9d
       call      qword ptr [7FFF68D84678]
M06_L06:
       nop
       add       rsp,58
       pop       rbx
       pop       rsi
       ret
M06_L07:
       call      qword ptr [7FFF68D6F2B8]
       int       3
M06_L08:
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FD0]
       jmp       short M06_L06
; Total bytes of code 397
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,58
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       movaps    [rsp+40],xmm4
       mov       rbx,rcx
       mov       esi,edx
       cmp       byte ptr [rbx+14],0
       jne       short M07_L03
M07_L00:
       lea       rdx,[rbx+18]
       mov       r8d,[rbx+10]
       mov       edi,[rdx+8]
       cmp       r8d,edi
       ja        near ptr M07_L10
       mov       rdx,[rdx]
       mov       ecx,r8d
       lea       rbp,[rdx+rcx*2]
       sub       edi,r8d
       mov       rcx,[rbx]
       test      esi,esi
       jl        short M07_L05
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       rdx,[rsp+40]
       lea       r8,[rsp+50]
       mov       ecx,esi
       call      qword ptr [7FFF68D80CF8]; Precode of System.Number.TryUInt32ToDecStr[[System.Char, System.Private.CoreLib]](UInt32, System.Span`1<Char>, Int32 ByRef)
M07_L01:
       test      eax,eax
       jne       short M07_L02
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M07_L00
M07_L02:
       mov       eax,[rsp+50]
       add       [rbx+10],eax
       jmp       short M07_L04
M07_L03:
       mov       rcx,rbx
       mov       edx,esi
       xor       r8d,r8d
       call      qword ptr [7FFF68D81B50]
M07_L04:
       nop
       add       rsp,58
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M07_L05:
       test      rcx,rcx
       je        short M07_L06
       call      qword ptr [7FFF68D71260]
       jmp       short M07_L07
M07_L06:
       call      qword ptr [7FFF68D71248]; Precode of System.Globalization.NumberFormatInfo.get_CurrentInfo()
M07_L07:
       mov       r8,[rax+28]
       test      r8,r8
       jne       short M07_L08
       xor       r9d,r9d
       xor       r8d,r8d
       jmp       short M07_L09
M07_L08:
       lea       r9,[r8+0C]
       mov       r8d,[r8+8]
M07_L09:
       mov       [rsp+30],r9
       mov       [rsp+38],r8d
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       r8,[rsp+50]
       mov       [rsp+20],r8
       lea       r8,[rsp+30]
       lea       r9,[rsp+40]
       mov       ecx,esi
       mov       edx,0FFFFFFFF
       call      qword ptr [7FFF68D80CE0]
       jmp       near ptr M07_L01
M07_L10:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 255
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,38
       xor       eax,eax
       mov       [rsp+28],rax
       mov       rbx,rcx
       lea       rsi,[rbx+18]
       mov       rcx,rsi
       mov       eax,[rbx+10]
       cmp       eax,[rcx+8]
       ja        short M08_L01
       mov       rcx,[rcx]
       mov       [rsp+28],rcx
       mov       [rsp+30],eax
       lea       rcx,[rsp+28]
       call      qword ptr [7FFF68D6BB28]
       mov       rdi,rax
       mov       rbp,[rbx+8]
       xor       eax,eax
       mov       [rbx+8],rax
       mov       [rsi],rax
       mov       [rsi+8],rax
       mov       [rbx+10],eax
       test      rbp,rbp
       je        short M08_L00
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       mov       rdx,rbp
       xor       r8d,r8d
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798C0]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Return(Char[], Boolean)
M08_L00:
       mov       rax,rdi
       add       rsp,38
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M08_L01:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 126
```
```assembly
; System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,30
       mov       [rsp+28],rcx
       mov       rbx,rdx
       mov       rsi,r8
       mov       rdi,r9
       call      qword ptr [7FFF68D5EAC8]
       mov       rcx,rax
       mov       r9,rdi
       mov       rdx,rbx
       mov       r8,rsi
       lea       rax,[7FFF68D7FBE0]
       add       rsp,30
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax]
; Total bytes of code 56
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: Job-JESZGN(IterationCount=8, IterationTime=300ms, LaunchCount=2, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks.NullCheck()
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,0A8
       xor       eax,eax
       mov       [rsp+38],rax
       vxorps    xmm4,xmm4,xmm4
       vmovdqu   ymmword ptr [rsp+40],ymm4
       vmovdqu   ymmword ptr [rsp+60],ymm4
       vmovdqu   ymmword ptr [rsp+80],ymm4
       mov       [rsp+0A0],rax
       mov       rbx,rcx
       xor       eax,eax
       mov       rsi,[rbx+10]
       mov       [rsp+30],rsi
       xor       edi,edi
       cmp       [rsi+8],edi
       jg        near ptr M00_L10
M00_L00:
       vzeroupper
       add       rsp,0A8
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       ret
M00_L01:
       mov       r15d,eax
M00_L02:
       mov       ecx,[rsp+70]
       imul      edx,r15d,0C2B2AE3D
       add       edx,0D1EF096
       rol       edx,11
       imul      edx,27D4EB2F
       imul      ecx,0C2B2AE3D
       add       ecx,edx
       rol       ecx,11
       imul      ecx,27D4EB2F
       mov       edx,ecx
       shr       edx,0F
       xor       edx,ecx
       imul      ecx,edx,85EBCA77
       mov       edx,ecx
       shr       edx,0D
       xor       edx,ecx
       imul      ecx,edx,0C2B2AE3D
       mov       r15d,ecx
       shr       r15d,10
       xor       r15d,ecx
       xor       ecx,ecx
       mov       [rsp+60],rcx
       mov       rcx,[r14+8]
       mov       edx,r15d
       imul      rdx,[r14+30]
       shr       rdx,20
       inc       rdx
       mov       eax,[rcx+8]
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,20
       cmp       edx,eax
       jae       near ptr M00_L39
       mov       edx,edx
       lea       rcx,[rcx+rdx*4+10]
       mov       r13d,[rcx]
       mov       r14,[r14+10]
       xor       r12d,r12d
       dec       r13d
       mov       eax,[r14+8]
       cmp       eax,r13d
       jbe       near ptr M00_L13
M00_L03:
       mov       ecx,r13d
       shl       rcx,5
       lea       r13,[r14+rcx+10]
       cmp       [r13+8],r15d
       jne       near ptr M00_L12
       mov       rcx,[r13+10]
       mov       eax,[r13+18]
       mov       [rsp+5C],eax
       test      rcx,rcx
       je        near ptr M00_L12
       mov       rdx,offset MT_System.RuntimeType
       cmp       [rcx],rdx
       jne       near ptr M00_L16
       cmp       rbp,rcx
       sete      r8b
       movzx     r8d,r8b
M00_L04:
       test      r8d,r8d
       je        near ptr M00_L12
       cmp       dword ptr [rsp+5C],0
       jne       near ptr M00_L12
M00_L05:
       test      r13,r13
       je        near ptr M00_L21
       mov       rcx,[r13]
       cmp       qword ptr [rcx+8],0
       jne       near ptr M00_L22
       mov       r14,[rcx+18]
M00_L06:
       mov       rcx,offset MT_System.RuntimeType
       cmp       [rbp],rcx
       jne       near ptr M00_L25
       mov       [rsp+20],r14
       mov       rcx,[rbp+18]
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+28]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L23
M00_L07:
       cmp       ebx,1D
       je        near ptr M00_L24
M00_L08:
       mov       r14,[rsp+20]
       mov       rax,r14
M00_L09:
       test      rax,rax
       je        near ptr M00_L30
       inc       edi
       mov       rsi,[rsp+30]
       cmp       [rsi+8],edi
       mov       rbx,[rsp+0F0]
       jle       near ptr M00_L00
M00_L10:
       mov       rbp,[rsi+rdi*8+10]
       mov       [rsp+28],rbp
       test      rbp,rbp
       je        near ptr M00_L28
       mov       [rsp+0F0],rbx
       mov       rcx,[rbx+8]
       cmp       [rcx],cl
       vxorps    ymm0,ymm0,ymm0
       vmovdqu   ymmword ptr [rsp+80],ymm0
       vmovdqu   xmmword ptr [rsp+98],xmm0
       mov       r14,[rcx+8]
       mov       rax,rbp
       cmp       qword ptr [r14+8],0
       je        near ptr M00_L13
       mov       r15,[r14+18]
       test      r15,r15
       jne       near ptr M00_L17
       mov       [rsp+68],rax
       xor       ecx,ecx
       mov       [rsp+70],ecx
       mov       [rsp+60],rcx
       lea       rcx,[rsp+68]
       cmp       qword ptr [rsp+60],0
       jne       short M00_L11
       mov       rcx,[rsp+68]
       mov       [rsp+60],rcx
       lea       rcx,[rsp+60]
       cmp       qword ptr [rsp+60],0
       je        short M00_L14
M00_L11:
       mov       r15,[rcx]
       mov       rcx,offset MT_System.RuntimeType
       cmp       [r15],rcx
       jne       short M00_L15
       mov       rcx,r15
       call      00007FFF68F03E20
       test      eax,eax
       jne       near ptr M00_L01
       mov       rcx,r15
       call      qword ptr [7FFF094A5FF8]; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       jmp       near ptr M00_L01
M00_L12:
       mov       r13d,[r13+0C]
       inc       r12d
       mov       eax,[r14+8]
       cmp       eax,r12d
       jb        near ptr M00_L29
       cmp       eax,r13d
       ja        near ptr M00_L03
M00_L13:
       xor       r13d,r13d
       jmp       near ptr M00_L05
M00_L14:
       xor       r15d,r15d
       jmp       near ptr M00_L02
M00_L15:
       mov       rcx,r15
       mov       rax,[r15]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       mov       r15d,eax
       jmp       near ptr M00_L02
M00_L16:
       mov       rdx,rbp
       mov       r8,[rcx]
       mov       r8,[r8+40]
       call      qword ptr [r8+10]
       mov       r8d,eax
       jmp       near ptr M00_L04
M00_L17:
       mov       [rsp+48],rax
       xor       edx,edx
       mov       [rsp+50],edx
       lea       rdx,[rsp+48]
       mov       rcx,r15
       mov       r11,7FFF09220568
       call      qword ptr [r11]
       mov       r13d,eax
       mov       r8,[r14+8]
       mov       edx,r13d
       imul      rdx,[r14+30]
       shr       rdx,20
       inc       rdx
       mov       ecx,[r8+8]
       imul      rdx,rcx
       shr       rdx,20
       cmp       edx,[r8+8]
       jae       near ptr M00_L39
       mov       edx,edx
       lea       r8,[r8+rdx*4+10]
       mov       r12d,[r8]
       mov       r14,[r14+10]
       xor       eax,eax
       mov       [rsp+7C],eax
       dec       r12d
M00_L18:
       cmp       [r14+8],r12d
       jbe       near ptr M00_L13
       mov       r8d,r12d
       shl       r8,5
       lea       r8,[r14+r8+10]
       mov       r12,r8
       cmp       [r12+8],r13d
       jne       short M00_L19
       vmovdqu   xmm0,xmmword ptr [r12+10]
       vmovdqu   xmmword ptr [rsp+48],xmm0
       mov       [rsp+38],rbp
       xor       r8d,r8d
       mov       [rsp+40],r8d
       lea       r8,[rsp+38]
       lea       rdx,[rsp+48]
       mov       rcx,r15
       mov       r11,7FFF09220570
       call      qword ptr [r11]
       test      eax,eax
       jne       short M00_L20
M00_L19:
       mov       r12d,[r12+0C]
       mov       eax,[rsp+7C]
       inc       eax
       cmp       [r14+8],eax
       mov       [rsp+7C],eax
       jae       short M00_L18
       jmp       near ptr M00_L29
M00_L20:
       mov       r13,r12
       jmp       near ptr M00_L05
M00_L21:
       xor       eax,eax
       jmp       near ptr M00_L09
M00_L22:
       call      qword ptr [7FFF095A5968]; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       mov       [rsp+20],rax
       mov       r14,[rsp+20]
       jmp       near ptr M00_L06
M00_L23:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L07
M00_L24:
       mov       r14,[rsp+20]
       jmp       short M00_L26
M00_L25:
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+60]
       call      qword ptr [rax+18]
       test      eax,eax
       je        short M00_L27
M00_L26:
       mov       rdx,r14
       mov       rcx,offset MT_System.Collections.ICollection
       call      qword ptr [7FFF094AECA0]; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       mov       r14,rax
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+68]
       call      qword ptr [rax+8]
       mov       r15,rax
       mov       rcx,r14
       mov       r11,7FFF09220558
       call      qword ptr [r11]
       mov       edx,eax
       mov       rcx,r15
       call      qword ptr [7FFF095A5BF0]; System.Array.CreateInstance(System.Type, Int32)
       mov       r15,rax
       mov       rcx,r14
       mov       rdx,r15
       mov       r11,7FFF09220560
       xor       r8d,r8d
       call      qword ptr [r11]
       mov       rax,r15
       jmp       near ptr M00_L09
M00_L27:
       mov       [rsp+20],r14
       jmp       near ptr M00_L08
M00_L28:
       mov       ecx,592
       mov       rdx,7FFF09518420
       call      qword ptr [7FFF092DF210]
       mov       rcx,rax
       call      qword ptr [7FFF096D61C0]
       int       3
M00_L29:
       call      qword ptr [7FFF092DF480]
       int       3
M00_L30:
       lea       rcx,[rsp+80]
       mov       edx,1D
       mov       r8d,2
       call      qword ptr [7FFF092DC270]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       mov       ecx,[rsp+90]
       cmp       ecx,[rsp+0A0]
       ja        near ptr M00_L35
       mov       rdx,[rsp+98]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0A0]
       sub       eax,ecx
       cmp       eax,14
       jb        short M00_L31
       vmovups   ymm0,[7FFF09357DE0]
       vmovups   [rdx],ymm0
       mov       rcx,200072006F0066
       mov       [rdx+20],rcx
       mov       ecx,[rsp+90]
       add       ecx,14
       mov       [rsp+90],ecx
       jmp       short M00_L32
M00_L31:
       lea       rcx,[rsp+80]
       mov       rdx,2DC925FB920
       call      qword ptr [7FFF0963E340]
M00_L32:
       lea       rcx,[rsp+80]
       mov       r8,rbp
       mov       rdx,7FFF09662DA0
       call      qword ptr [7FFF094AEA60]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       mov       ecx,[rsp+90]
       cmp       ecx,[rsp+0A0]
       ja        near ptr M00_L35
       mov       rdx,[rsp+98]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0A0]
       sub       eax,ecx
       cmp       eax,7
       jb        short M00_L33
       mov       rcx,65006B00280020
       mov       [rdx],rcx
       mov       dword ptr [rdx+8],3A0079
       mov       word ptr [rdx+0C],20
       mov       ecx,[rsp+90]
       add       ecx,7
       mov       [rsp+90],ecx
       jmp       short M00_L34
M00_L33:
       lea       rcx,[rsp+80]
       mov       rdx,2DC925FB960
       call      qword ptr [7FFF0963E340]
M00_L34:
       lea       rcx,[rsp+80]
       xor       edx,edx
       call      qword ptr [7FFF092D7FD8]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       mov       ecx,[rsp+90]
       cmp       ecx,[rsp+0A0]
       jbe       short M00_L36
M00_L35:
       call      qword ptr [7FFF094A7D50]
       int       3
M00_L36:
       mov       rdx,[rsp+98]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0A0]
       sub       eax,ecx
       cmp       eax,2
       jb        short M00_L37
       mov       dword ptr [rdx],2E0029
       mov       ecx,[rsp+90]
       add       ecx,2
       mov       [rsp+90],ecx
       jmp       short M00_L38
M00_L37:
       lea       rcx,[rsp+80]
       mov       rdx,2DC925F9BF8
       call      qword ptr [7FFF0963E340]
M00_L38:
       mov       rcx,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rcx,[rsp+80]
       call      qword ptr [7FFF092DC2A0]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFF095A7BD0]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L39:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 1672
```
```assembly
; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,88
       lea       rbp,[rsp+0C0]
       mov       [rbp+10],rcx
       cmp       qword ptr [rbp+10],0
       je        short M01_L01
       lea       rcx,[rbp+10]
       mov       [rbp-98],rcx
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-98]
       call      qword ptr [rax]
       mov       ebx,eax
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       eax,ebx
M01_L00:
       add       rsp,88
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M01_L01:
       xor       eax,eax
       jmp       short M01_L00
; Total bytes of code 119
```
```assembly
; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       push      rsi
       push      rbx
       sub       rsp,28
       mov       [rsp+20],rcx
       mov       rbx,rcx
M02_L00:
       mov       rsi,[rbx+8]
       test      rsi,rsi
       je        short M02_L01
       mov       ecx,[rsi+10]
       cmp       ecx,8
       jne       short M02_L03
       mov       rcx,rbx
       mov       rdx,rsi
       xor       r8d,r8d
       call      qword ptr [7FFF68D7E1B0]
M02_L01:
       cmp       qword ptr [rbx+8],0
       jne       short M02_L02
       mov       rax,[rbx+18]
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M02_L02:
       mov       rcx,rbx
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax]
M02_L03:
       cmp       ecx,8
       ja        short M02_L04
       mov       ecx,ecx
       lea       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       edx,[rdx+rcx*4]
       lea       r8,[M02_L00]
       add       rdx,r8
       jmp       rdx
M02_L04:
       mov       rcx,[rsi+8]
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D74C78]
       int       3
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1A0]
       jmp       short M02_L01
       mov       rcx,rbx
       xor       edx,edx
       call      qword ptr [7FFF68D7E1A8]; Precode of System.Lazy`1[[System.__Canon, System.Private.CoreLib]].ViaFactory(System.Threading.LazyThreadSafetyMode)
       jmp       short M02_L01
       mov       rcx,[rbx]
       call      qword ptr [7FFF68D5B838]
       mov       rcx,rax
       call      qword ptr [7FFF68D62EA0]
       mov       rcx,rax
       call      qword ptr [7FFF68D84008]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
       mov       rax,[rbx+10]
       test      rax,rax
       je        short M02_L05
       mov       rcx,[rax+8]
       call      qword ptr [rax+18]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
M02_L05:
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1C8]
       jmp       near ptr M02_L01
       mov       rcx,rbx
       mov       rdx,rsi
       mov       r8d,1
       call      qword ptr [7FFF68D7E1B0]
       jmp       near ptr M02_L01
; Total bytes of code 269
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       test      rdx,rdx
       je        short M03_L02
       mov       rax,[rdx]
       movzx     r8d,word ptr [rax+0E]
       test      r8,r8
       je        short M03_L05
       mov       rax,[rax+38]
       cmp       r8,4
       jl        short M03_L01
       cmp       [rax],rcx
       je        short M03_L02
M03_L00:
       cmp       [rax+8],rcx
       je        short M03_L02
       cmp       [rax+10],rcx
       je        short M03_L02
       cmp       [rax+18],rcx
       je        short M03_L02
       add       rax,20
       add       r8,0FFFFFFFFFFFFFFFC
       cmp       r8,4
       jge       short M03_L04
       test      r8,r8
       je        short M03_L05
M03_L01:
       cmp       [rax],rcx
       jne       short M03_L03
M03_L02:
       mov       rax,rdx
       ret
M03_L03:
       add       rax,8
       dec       r8
       test      r8,r8
       jle       short M03_L05
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L03
M03_L04:
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L00
M03_L05:
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       jmp       qword ptr [rax]
; Total bytes of code 115
```
```assembly
; System.Array.CreateInstance(System.Type, Int32)
       sub       rsp,28
       mov       [rsp+38],edx
       test      rcx,rcx
       je        short M04_L01
       mov       eax,[rsp+38]
       test      eax,eax
       jl        short M04_L02
       mov       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       cmp       r11,[rcx]
       jne       short M04_L03
M04_L00:
       call      qword ptr [7FFF68D67080]
       test      rax,rax
       je        short M04_L04
       lea       r8,[rsp+38]
       mov       rcx,rax
       mov       edx,1
       xor       r9d,r9d
       call      qword ptr [7FFF68D6A298]; Precode of System.Array.InternalCreate(System.RuntimeType, Int32, Int32*, Int32*)
       nop
       add       rsp,28
       ret
M04_L01:
       mov       rcx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rcx]
       call      qword ptr [7FFF68D6C210]
       int       3
M04_L02:
       mov       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rdx,[rdx]
       mov       ecx,eax
       call      qword ptr [7FFF68D81908]
       int       3
M04_L03:
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rcx,rax
       jmp       short M04_L00
M04_L04:
       mov       ecx,39
       mov       edx,4E
       call      qword ptr [7FFF68D6F3B0]
       int       3
; Total bytes of code 140
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       esi,edx
       mov       edi,r8d
       xor       eax,eax
       mov       [rbx],rax
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       imul      edx,edi,0B
       add       edx,esi
       mov       eax,100
       cmp       edx,100
       cmovle    edx,eax
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798B8]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Rent(Int32)
       mov       [rbx+8],rax
       test      rax,rax
       je        short M05_L01
       lea       rcx,[rax+10]
       mov       eax,[rax+8]
M05_L00:
       mov       [rbx+18],rcx
       mov       [rbx+20],eax
       xor       eax,eax
       mov       [rbx+10],eax
       mov       byte ptr [rbx+14],0
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
M05_L01:
       xor       ecx,ecx
       xor       eax,eax
       jmp       short M05_L00
; Total bytes of code 102
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       push      rsi
       push      rbx
       sub       rsp,58
       xor       eax,eax
       mov       [rsp+28],rax
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       mov       [rsp+40],rax
       mov       [rsp+50],rdx
       mov       rbx,rcx
       mov       rcx,rdx
       mov       rsi,r8
       cmp       byte ptr [rbx+14],0
       jne       near ptr M06_L05
       test      rsi,rsi
       je        near ptr M06_L06
       mov       rcx,rsi
       call      qword ptr [7FFF68D67170]
       test      rax,rax
       jne       short M06_L01
       mov       rcx,rsi
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rdx,rax
M06_L00:
       test      rdx,rdx
       je        near ptr M06_L06
       lea       r8,[rbx+18]
       mov       ecx,[rbx+10]
       mov       eax,[r8+8]
       cmp       ecx,eax
       ja        near ptr M06_L07
       mov       r8,[r8]
       mov       r10d,ecx
       lea       r10,[r8+r10*2]
       sub       eax,ecx
       mov       esi,[rdx+8]
       cmp       esi,eax
       ja        near ptr M06_L08
       mov       r8d,esi
       add       r8,r8
       add       rdx,0C
       mov       rcx,r10
       call      qword ptr [7FFF68D6D928]; Precode of System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       add       [rbx+10],esi
       jmp       near ptr M06_L06
M06_L01:
       mov       rcx,rsi
       call      qword ptr [7FFF68D671A8]
       test      rax,rax
       je        near ptr M06_L04
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E8]
       mov       rsi,rax
M06_L02:
       mov       rcx,rsi
       lea       rdx,[rbx+18]
       mov       r9d,[rbx+10]
       mov       r8d,[rdx+8]
       cmp       r9d,r8d
       ja        near ptr M06_L07
       mov       rdx,[rdx]
       mov       r11d,r9d
       lea       rdx,[rdx+r11*2]
       sub       r8d,r9d
       mov       [rsp+38],rdx
       mov       [rsp+40],r8d
       xor       edx,edx
       mov       [rsp+28],rdx
       mov       [rsp+30],edx
       mov       rdx,[rbx]
       mov       [rsp+20],rdx
       lea       rdx,[rsp+38]
       lea       r9,[rsp+28]
       lea       r8,[rsp+48]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       test      eax,eax
       jne       short M06_L03
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M06_L02
M06_L03:
       mov       ecx,[rsp+48]
       add       [rbx+10],ecx
       jmp       short M06_L06
M06_L04:
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E0]
       mov       rcx,rax
       mov       r8,[rbx]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       xor       edx,edx
       call      qword ptr [r11]
       mov       rdx,rax
       jmp       near ptr M06_L00
M06_L05:
       call      qword ptr [7FFF68D600E8]
       mov       rdx,rax
       mov       rcx,rbx
       mov       r8,rsi
       xor       r9d,r9d
       call      qword ptr [7FFF68D84678]
M06_L06:
       nop
       add       rsp,58
       pop       rbx
       pop       rsi
       ret
M06_L07:
       call      qword ptr [7FFF68D6F2B8]
       int       3
M06_L08:
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FD0]
       jmp       short M06_L06
; Total bytes of code 397
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,58
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       movaps    [rsp+40],xmm4
       mov       rbx,rcx
       mov       esi,edx
       cmp       byte ptr [rbx+14],0
       jne       short M07_L03
M07_L00:
       lea       rdx,[rbx+18]
       mov       r8d,[rbx+10]
       mov       edi,[rdx+8]
       cmp       r8d,edi
       ja        near ptr M07_L10
       mov       rdx,[rdx]
       mov       ecx,r8d
       lea       rbp,[rdx+rcx*2]
       sub       edi,r8d
       mov       rcx,[rbx]
       test      esi,esi
       jl        short M07_L05
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       rdx,[rsp+40]
       lea       r8,[rsp+50]
       mov       ecx,esi
       call      qword ptr [7FFF68D80CF8]; Precode of System.Number.TryUInt32ToDecStr[[System.Char, System.Private.CoreLib]](UInt32, System.Span`1<Char>, Int32 ByRef)
M07_L01:
       test      eax,eax
       jne       short M07_L02
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M07_L00
M07_L02:
       mov       eax,[rsp+50]
       add       [rbx+10],eax
       jmp       short M07_L04
M07_L03:
       mov       rcx,rbx
       mov       edx,esi
       xor       r8d,r8d
       call      qword ptr [7FFF68D81B50]
M07_L04:
       nop
       add       rsp,58
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M07_L05:
       test      rcx,rcx
       je        short M07_L06
       call      qword ptr [7FFF68D71260]; Precode of System.Globalization.NumberFormatInfo.<GetInstance>g__GetProviderNonNull|58_0(System.IFormatProvider)
       jmp       short M07_L07
M07_L06:
       call      qword ptr [7FFF68D71248]; Precode of System.Globalization.NumberFormatInfo.get_CurrentInfo()
M07_L07:
       mov       r8,[rax+28]
       test      r8,r8
       jne       short M07_L08
       xor       r9d,r9d
       xor       r8d,r8d
       jmp       short M07_L09
M07_L08:
       lea       r9,[r8+0C]
       mov       r8d,[r8+8]
M07_L09:
       mov       [rsp+30],r9
       mov       [rsp+38],r8d
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       r8,[rsp+50]
       mov       [rsp+20],r8
       lea       r8,[rsp+30]
       lea       r9,[rsp+40]
       mov       ecx,esi
       mov       edx,0FFFFFFFF
       call      qword ptr [7FFF68D80CE0]
       jmp       near ptr M07_L01
M07_L10:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 255
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,38
       xor       eax,eax
       mov       [rsp+28],rax
       mov       rbx,rcx
       lea       rsi,[rbx+18]
       mov       rcx,rsi
       mov       eax,[rbx+10]
       cmp       eax,[rcx+8]
       ja        short M08_L01
       mov       rcx,[rcx]
       mov       [rsp+28],rcx
       mov       [rsp+30],eax
       lea       rcx,[rsp+28]
       call      qword ptr [7FFF68D6BB28]; Precode of System.String.Ctor(System.ReadOnlySpan`1<Char>)
       mov       rdi,rax
       mov       rbp,[rbx+8]
       xor       eax,eax
       mov       [rbx+8],rax
       mov       [rsi],rax
       mov       [rsi+8],rax
       mov       [rbx+10],eax
       test      rbp,rbp
       je        short M08_L00
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       mov       rdx,rbp
       xor       r8d,r8d
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798C0]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Return(Char[], Boolean)
M08_L00:
       mov       rax,rdi
       add       rsp,38
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M08_L01:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 126
```
```assembly
; System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,30
       mov       [rsp+28],rcx
       mov       rbx,rdx
       mov       rsi,r8
       mov       rdi,r9
       call      qword ptr [7FFF68D5EAC8]
       mov       rcx,rax
       mov       r9,rdi
       mov       rdx,rbx
       mov       r8,rsi
       lea       rax,[7FFF68D7FBE0]
       add       rsp,30
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax]
; Total bytes of code 56
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: Job-JESZGN(IterationCount=8, IterationTime=300ms, LaunchCount=2, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks.FullTypeChecks()
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,0B8
       xor       eax,eax
       mov       [rsp+48],rax
       vxorps    xmm4,xmm4,xmm4
       vmovdqu   ymmword ptr [rsp+50],ymm4
       vmovdqu   ymmword ptr [rsp+70],ymm4
       vmovdqu   ymmword ptr [rsp+90],ymm4
       mov       [rsp+0B0],rax
       mov       rbx,rcx
       xor       eax,eax
       mov       [rsp+100],rbx
       mov       rsi,[rbx+10]
       mov       [rsp+40],rsi
       xor       edi,edi
       cmp       [rsi+8],edi
       jg        near ptr M00_L11
M00_L00:
       vzeroupper
       add       rsp,0B8
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       ret
M00_L01:
       mov       esi,1
       jmp       near ptr M00_L15
M00_L02:
       mov       r13d,eax
M00_L03:
       mov       ecx,[rsp+80]
       imul      edx,r13d,0C2B2AE3D
       add       edx,89BC9785
       rol       edx,11
       imul      edx,27D4EB2F
       imul      ecx,0C2B2AE3D
       add       ecx,edx
       rol       ecx,11
       imul      ecx,27D4EB2F
       mov       edx,ecx
       shr       edx,0F
       xor       edx,ecx
       imul      ecx,edx,85EBCA77
       mov       edx,ecx
       shr       edx,0D
       xor       edx,ecx
       imul      ecx,edx,0C2B2AE3D
       mov       r13d,ecx
       shr       r13d,10
       xor       r13d,ecx
       xor       ecx,ecx
       mov       [rsp+70],rcx
       mov       rcx,[r14+8]
       mov       edx,r13d
       imul      rdx,[r14+30]
       shr       rdx,20
       inc       rdx
       mov       eax,[rcx+8]
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,20
       cmp       edx,eax
       jae       near ptr M00_L63
       mov       edx,edx
       lea       rcx,[rcx+rdx*4+10]
       mov       r12d,[rcx]
       mov       r14,[r14+10]
       xor       r15d,r15d
       dec       r12d
       mov       eax,[r14+8]
       cmp       eax,r12d
       jbe       near ptr M00_L24
M00_L04:
       mov       ecx,r12d
       shl       rcx,5
       lea       r12,[r14+rcx+10]
       cmp       [r12+8],r13d
       jne       near ptr M00_L23
       mov       rcx,[r12+10]
       mov       eax,[r12+18]
       mov       [rsp+6C],eax
       test      rcx,rcx
       je        near ptr M00_L23
       mov       rdx,offset MT_System.RuntimeType
       mov       r8,rdx
       cmp       [rcx],r8
       jne       near ptr M00_L38
       cmp       rbp,rcx
       sete      r8b
       movzx     r8d,r8b
M00_L05:
       test      r8d,r8d
       je        near ptr M00_L23
       cmp       dword ptr [rsp+6C],0
       jne       near ptr M00_L23
M00_L06:
       test      r12,r12
       je        near ptr M00_L42
       mov       rcx,[r12]
       cmp       qword ptr [rcx+8],0
       jne       near ptr M00_L43
       mov       r14,[rcx+18]
M00_L07:
       mov       rcx,[rbp]
       mov       r15,rcx
       mov       r8,offset MT_System.RuntimeType
       mov       r13,r8
       cmp       r15,r13
       jne       near ptr M00_L46
       mov       [rsp+28],r14
       mov       rcx,[rbp+18]
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+38]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L44
M00_L08:
       cmp       ebx,1D
       je        near ptr M00_L45
M00_L09:
       mov       r14,[rsp+28]
       mov       rax,r14
M00_L10:
       test      rax,rax
       je        near ptr M00_L54
       inc       edi
       mov       rsi,[rsp+40]
       cmp       [rsi+8],edi
       jle       near ptr M00_L00
M00_L11:
       cmp       edi,[rsi+8]
       jae       near ptr M00_L63
       mov       rbp,[rsi+rdi*8+10]
       mov       [rsp+38],rbp
       test      rbp,rbp
       je        near ptr M00_L49
       mov       r14,[rbp]
       mov       r15,offset MT_System.RuntimeType
       cmp       r14,r15
       jne       near ptr M00_L29
       mov       r13,rbp
       mov       rbp,[rsp+38]
M00_L12:
       cmp       [r13],r15
       jne       near ptr M00_L27
       mov       [rsp+30],r13
       mov       rcx,[r13+18]
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       r13,[rsp+30]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L26
M00_L13:
       cmp       ebx,1D
       ja        short M00_L14
       mov       ecx,1FEF7FFF
       bt        ecx,ebx
       jae       near ptr M00_L01
M00_L14:
       cmp       ebx,10
       sete      sil
       movzx     esi,sil
M00_L15:
       mov       rbp,[rsp+38]
M00_L16:
       test      esi,esi
       jne       near ptr M00_L25
       cmp       [r13],r15
       jne       near ptr M00_L28
M00_L17:
       test      r13,r13
       je        near ptr M00_L50
       mov       rcx,r13
       call      00007FFF68F39140
M00_L18:
       test      eax,eax
       jne       near ptr M00_L51
       cmp       [rbp],r15
       jne       near ptr M00_L31
       mov       rcx,[rbp+18]
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     r14d,al
       mov       rbp,[rsp+38]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L30
M00_L19:
       cmp       r14d,10
       je        near ptr M00_L52
       mov       rcx,[rbp+18]
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+38]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L32
M00_L20:
       cmp       ebx,0F
       je        near ptr M00_L52
       mov       rcx,[rbp+18]
       test      cl,2
       jne       near ptr M00_L34
       mov       ecx,[rcx]
       and       ecx,80001000
       cmp       ecx,1000
       sete      al
       movzx     eax,al
M00_L21:
       test      eax,eax
       jne       near ptr M00_L52
       mov       rcx,22A125F1990
       cmp       rbp,rcx
       je        near ptr M00_L52
       mov       rbx,[rsp+100]
       mov       rcx,[rbx+8]
       cmp       [rcx],cl
       vxorps    ymm0,ymm0,ymm0
       vmovdqu   ymmword ptr [rsp+90],ymm0
       vmovdqu   xmmword ptr [rsp+0A8],xmm0
       mov       r14,[rcx+8]
       mov       rax,rbp
       cmp       qword ptr [r14+8],0
       je        near ptr M00_L24
       mov       r13,[r14+18]
       test      r13,r13
       jne       near ptr M00_L39
       mov       [rsp+78],rax
       xor       ecx,ecx
       mov       [rsp+80],ecx
       mov       [rsp+70],rcx
       lea       rcx,[rsp+78]
       cmp       qword ptr [rsp+70],0
       jne       short M00_L22
       mov       rcx,[rsp+78]
       mov       [rsp+70],rcx
       lea       rcx,[rsp+70]
       cmp       qword ptr [rsp+70],0
       je        near ptr M00_L36
M00_L22:
       mov       r13,[rcx]
       cmp       [r13],r15
       jne       near ptr M00_L37
       mov       rcx,r13
       call      00007FFF68F03E20
       test      eax,eax
       jne       near ptr M00_L02
       mov       rcx,r13
       call      qword ptr [7FFF094C5FF8]; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       jmp       near ptr M00_L02
M00_L23:
       mov       r12d,[r12+0C]
       inc       r15d
       mov       eax,[r14+8]
       cmp       eax,r15d
       jb        near ptr M00_L53
       cmp       eax,r12d
       ja        near ptr M00_L04
M00_L24:
       xor       r12d,r12d
       jmp       near ptr M00_L06
M00_L25:
       mov       rcx,r13
       mov       rax,[r13]
       mov       rax,[rax+68]
       call      qword ptr [rax+8]
       mov       r13,rax
       jmp       near ptr M00_L12
M00_L26:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L13
M00_L27:
       mov       rcx,r13
       mov       rax,[r13]
       mov       rax,[rax+68]
       call      qword ptr [rax]
       mov       esi,eax
       jmp       near ptr M00_L16
M00_L28:
       mov       rcx,r13
       mov       rax,[r13]
       mov       rax,[rax+98]
       call      qword ptr [rax+8]
       mov       r13,rax
       jmp       near ptr M00_L17
M00_L29:
       mov       rcx,rbp
       mov       rax,[r14+0B0]
       call      qword ptr [rax]
       jmp       near ptr M00_L18
M00_L30:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L19
M00_L31:
       mov       rcx,rbp
       mov       rax,[r14+58]
       call      qword ptr [rax+18]
       test      eax,eax
       jne       near ptr M00_L52
       jmp       short M00_L33
M00_L32:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L20
M00_L33:
       mov       rcx,rbp
       mov       rax,[r14+58]
       call      qword ptr [rax+20]
       test      eax,eax
       jne       near ptr M00_L52
       jmp       short M00_L35
M00_L34:
       xor       eax,eax
       jmp       near ptr M00_L21
M00_L35:
       mov       rcx,rbp
       mov       rax,[r14+60]
       call      qword ptr [rax+28]
       jmp       near ptr M00_L21
M00_L36:
       xor       r13d,r13d
       jmp       near ptr M00_L03
M00_L37:
       mov       rcx,r13
       mov       rax,[r13]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       mov       r13d,eax
       jmp       near ptr M00_L03
M00_L38:
       mov       rdx,rbp
       mov       r8,[rcx]
       mov       r8,[r8+40]
       call      qword ptr [r8+10]
       mov       r8d,eax
       jmp       near ptr M00_L05
M00_L39:
       mov       [rsp+58],rax
       xor       edx,edx
       mov       [rsp+60],edx
       lea       rdx,[rsp+58]
       mov       rcx,r13
       mov       r11,7FFF09240568
       call      qword ptr [r11]
       mov       r15d,eax
       mov       r8,[r14+8]
       mov       edx,r15d
       imul      rdx,[r14+30]
       shr       rdx,20
       inc       rdx
       mov       ecx,[r8+8]
       imul      rdx,rcx
       shr       rdx,20
       cmp       edx,[r8+8]
       jae       near ptr M00_L63
       mov       edx,edx
       lea       r8,[r8+rdx*4+10]
       mov       r12d,[r8]
       mov       r14,[r14+10]
       xor       eax,eax
       mov       [rsp+8C],eax
       dec       r12d
M00_L40:
       cmp       [r14+8],r12d
       jbe       near ptr M00_L24
       mov       r8d,r12d
       shl       r8,5
       lea       r12,[r14+r8+10]
       cmp       [r12+8],r15d
       jne       short M00_L41
       vmovdqu   xmm0,xmmword ptr [r12+10]
       vmovdqu   xmmword ptr [rsp+58],xmm0
       mov       [rsp+48],rbp
       xor       r8d,r8d
       mov       [rsp+50],r8d
       lea       r8,[rsp+48]
       lea       rdx,[rsp+58]
       mov       rcx,r13
       mov       r11,7FFF09240570
       call      qword ptr [r11]
       test      eax,eax
       jne       near ptr M00_L06
M00_L41:
       mov       r12d,[r12+0C]
       mov       eax,[rsp+8C]
       inc       eax
       cmp       [r14+8],eax
       mov       [rsp+8C],eax
       jae       short M00_L40
       jmp       near ptr M00_L53
M00_L42:
       xor       eax,eax
       jmp       near ptr M00_L10
M00_L43:
       call      qword ptr [7FFF095C5968]; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       mov       [rsp+28],rax
       mov       r14,[rsp+28]
       jmp       near ptr M00_L07
M00_L44:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L08
M00_L45:
       mov       r14,[rsp+28]
       jmp       short M00_L47
M00_L46:
       mov       rcx,rbp
       mov       rax,[r15+60]
       call      qword ptr [rax+18]
       test      eax,eax
       je        short M00_L48
M00_L47:
       mov       rdx,r14
       mov       rcx,offset MT_System.Collections.ICollection
       call      qword ptr [7FFF094CECA0]; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       mov       r14,rax
       mov       rcx,rbp
       mov       rax,[r15+68]
       call      qword ptr [rax+8]
       mov       r15,rax
       mov       rcx,r14
       mov       r11,7FFF09240558
       call      qword ptr [r11]
       mov       edx,eax
       mov       rcx,r15
       call      qword ptr [7FFF095C5BF0]; System.Array.CreateInstance(System.Type, Int32)
       mov       r15,rax
       mov       rcx,r14
       mov       rdx,r15
       mov       r11,7FFF09240560
       xor       r8d,r8d
       call      qword ptr [r11]
       mov       rax,r15
       jmp       near ptr M00_L10
M00_L48:
       mov       [rsp+28],r14
       jmp       near ptr M00_L09
M00_L49:
       mov       ecx,592
       mov       rdx,7FFF09538420
       call      qword ptr [7FFF092FF210]
       mov       rcx,rax
       call      qword ptr [7FFF096F61F0]
       int       3
M00_L50:
       mov       rcx,offset MT_System.ArgumentNullException
       call      CORINFO_HELP_NEWSFAST
       mov       rbp,rax
       call      qword ptr [7FFF096FC0F0]
       mov       r8,rax
       mov       rcx,rbp
       xor       edx,edx
       call      qword ptr [7FFF096FC108]
       mov       rcx,rbp
       call      CORINFO_HELP_THROW
       int       3
M00_L51:
       mov       rcx,offset MT_System.ArgumentException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,5AA
       mov       rdx,7FFF09538420
       call      qword ptr [7FFF092FF210]
       mov       rsi,rax
       mov       ecx,592
       mov       rdx,7FFF09538420
       call      qword ptr [7FFF092FF210]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF095C7C90]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L52:
       mov       rcx,offset MT_System.ArgumentException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,600
       mov       rdx,7FFF09538420
       call      qword ptr [7FFF092FF210]
       mov       rsi,rax
       mov       ecx,592
       mov       rdx,7FFF09538420
       call      qword ptr [7FFF092FF210]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF095C7C90]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L53:
       call      qword ptr [7FFF092FF480]
       int       3
M00_L54:
       lea       rcx,[rsp+90]
       mov       edx,1D
       mov       r8d,2
       call      qword ptr [7FFF092FC270]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       mov       ecx,[rsp+0A0]
       cmp       ecx,[rsp+0B0]
       ja        near ptr M00_L59
       mov       rdx,[rsp+0A8]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0B0]
       sub       eax,ecx
       cmp       eax,14
       jb        short M00_L55
       vmovups   ymm0,[7FFF09379100]
       vmovups   [rdx],ymm0
       mov       rcx,200072006F0066
       mov       [rdx+20],rcx
       mov       ecx,[rsp+0A0]
       add       ecx,14
       mov       [rsp+0A0],ecx
       jmp       short M00_L56
M00_L55:
       lea       rcx,[rsp+90]
       mov       rdx,22A125FB920
       call      qword ptr [7FFF0965E340]
M00_L56:
       lea       rcx,[rsp+90]
       mov       r8,rbp
       mov       rdx,7FFF09682DA0
       call      qword ptr [7FFF094CEA60]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       mov       ecx,[rsp+0A0]
       cmp       ecx,[rsp+0B0]
       ja        near ptr M00_L59
       mov       rdx,[rsp+0A8]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0B0]
       sub       eax,ecx
       cmp       eax,7
       jb        short M00_L57
       mov       rcx,65006B00280020
       mov       [rdx],rcx
       mov       dword ptr [rdx+8],3A0079
       mov       word ptr [rdx+0C],20
       mov       ecx,[rsp+0A0]
       add       ecx,7
       mov       [rsp+0A0],ecx
       jmp       short M00_L58
M00_L57:
       lea       rcx,[rsp+90]
       mov       rdx,22A125FB960
       call      qword ptr [7FFF0965E340]
M00_L58:
       lea       rcx,[rsp+90]
       xor       edx,edx
       call      qword ptr [7FFF092F7FD8]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       mov       ecx,[rsp+0A0]
       cmp       ecx,[rsp+0B0]
       jbe       short M00_L60
M00_L59:
       call      qword ptr [7FFF094C7D50]
       int       3
M00_L60:
       mov       rdx,[rsp+0A8]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+0B0]
       sub       eax,ecx
       cmp       eax,2
       jb        short M00_L61
       mov       dword ptr [rdx],2E0029
       mov       ecx,[rsp+0A0]
       add       ecx,2
       mov       [rsp+0A0],ecx
       jmp       short M00_L62
M00_L61:
       lea       rcx,[rsp+90]
       mov       rdx,22A125F9BF8
       call      qword ptr [7FFF0965E340]
M00_L62:
       mov       rcx,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rcx,[rsp+90]
       call      qword ptr [7FFF092FC2A0]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFF095C7BD0]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L63:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 2438
```
```assembly
; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,88
       lea       rbp,[rsp+0C0]
       mov       [rbp+10],rcx
       cmp       qword ptr [rbp+10],0
       je        short M01_L01
       lea       rcx,[rbp+10]
       mov       [rbp-98],rcx
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-98]
       call      qword ptr [rax]
       mov       ebx,eax
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       eax,ebx
M01_L00:
       add       rsp,88
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M01_L01:
       xor       eax,eax
       jmp       short M01_L00
; Total bytes of code 119
```
```assembly
; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       push      rsi
       push      rbx
       sub       rsp,28
       mov       [rsp+20],rcx
       mov       rbx,rcx
M02_L00:
       mov       rsi,[rbx+8]
       test      rsi,rsi
       je        short M02_L01
       mov       ecx,[rsi+10]
       cmp       ecx,8
       jne       short M02_L03
       mov       rcx,rbx
       mov       rdx,rsi
       xor       r8d,r8d
       call      qword ptr [7FFF68D7E1B0]
M02_L01:
       cmp       qword ptr [rbx+8],0
       jne       short M02_L02
       mov       rax,[rbx+18]
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M02_L02:
       mov       rcx,rbx
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax]
M02_L03:
       cmp       ecx,8
       ja        short M02_L04
       mov       ecx,ecx
       lea       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       edx,[rdx+rcx*4]
       lea       r8,[M02_L00]
       add       rdx,r8
       jmp       rdx
M02_L04:
       mov       rcx,[rsi+8]
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D74C78]
       int       3
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1A0]
       jmp       short M02_L01
       mov       rcx,rbx
       xor       edx,edx
       call      qword ptr [7FFF68D7E1A8]; Precode of System.Lazy`1[[System.__Canon, System.Private.CoreLib]].ViaFactory(System.Threading.LazyThreadSafetyMode)
       jmp       short M02_L01
       mov       rcx,[rbx]
       call      qword ptr [7FFF68D5B838]
       mov       rcx,rax
       call      qword ptr [7FFF68D62EA0]
       mov       rcx,rax
       call      qword ptr [7FFF68D84008]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
       mov       rax,[rbx+10]
       test      rax,rax
       je        short M02_L05
       mov       rcx,[rax+8]
       call      qword ptr [rax+18]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
M02_L05:
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1C8]
       jmp       near ptr M02_L01
       mov       rcx,rbx
       mov       rdx,rsi
       mov       r8d,1
       call      qword ptr [7FFF68D7E1B0]
       jmp       near ptr M02_L01
; Total bytes of code 269
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       test      rdx,rdx
       je        short M03_L02
       mov       rax,[rdx]
       movzx     r8d,word ptr [rax+0E]
       test      r8,r8
       je        short M03_L05
       mov       rax,[rax+38]
       cmp       r8,4
       jl        short M03_L01
       cmp       [rax],rcx
       je        short M03_L02
M03_L00:
       cmp       [rax+8],rcx
       je        short M03_L02
       cmp       [rax+10],rcx
       je        short M03_L02
       cmp       [rax+18],rcx
       je        short M03_L02
       add       rax,20
       add       r8,0FFFFFFFFFFFFFFFC
       cmp       r8,4
       jge       short M03_L04
       test      r8,r8
       je        short M03_L05
M03_L01:
       cmp       [rax],rcx
       jne       short M03_L03
M03_L02:
       mov       rax,rdx
       ret
M03_L03:
       add       rax,8
       dec       r8
       test      r8,r8
       jle       short M03_L05
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L03
M03_L04:
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L00
M03_L05:
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       jmp       qword ptr [rax]
; Total bytes of code 115
```
```assembly
; System.Array.CreateInstance(System.Type, Int32)
       sub       rsp,28
       mov       [rsp+38],edx
       test      rcx,rcx
       je        short M04_L01
       mov       eax,[rsp+38]
       test      eax,eax
       jl        short M04_L02
       mov       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       cmp       r11,[rcx]
       jne       short M04_L03
M04_L00:
       call      qword ptr [7FFF68D67080]
       test      rax,rax
       je        short M04_L04
       lea       r8,[rsp+38]
       mov       rcx,rax
       mov       edx,1
       xor       r9d,r9d
       call      qword ptr [7FFF68D6A298]; Precode of System.Array.InternalCreate(System.RuntimeType, Int32, Int32*, Int32*)
       nop
       add       rsp,28
       ret
M04_L01:
       mov       rcx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rcx]
       call      qword ptr [7FFF68D6C210]
       int       3
M04_L02:
       mov       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rdx,[rdx]
       mov       ecx,eax
       call      qword ptr [7FFF68D81908]
       int       3
M04_L03:
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rcx,rax
       jmp       short M04_L00
M04_L04:
       mov       ecx,39
       mov       edx,4E
       call      qword ptr [7FFF68D6F3B0]
       int       3
; Total bytes of code 140
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       esi,edx
       mov       edi,r8d
       xor       eax,eax
       mov       [rbx],rax
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       imul      edx,edi,0B
       add       edx,esi
       mov       eax,100
       cmp       edx,100
       cmovle    edx,eax
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798B8]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Rent(Int32)
       mov       [rbx+8],rax
       test      rax,rax
       je        short M05_L01
       lea       rcx,[rax+10]
       mov       eax,[rax+8]
M05_L00:
       mov       [rbx+18],rcx
       mov       [rbx+20],eax
       xor       eax,eax
       mov       [rbx+10],eax
       mov       byte ptr [rbx+14],0
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
M05_L01:
       xor       ecx,ecx
       xor       eax,eax
       jmp       short M05_L00
; Total bytes of code 102
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       push      rsi
       push      rbx
       sub       rsp,58
       xor       eax,eax
       mov       [rsp+28],rax
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       mov       [rsp+40],rax
       mov       [rsp+50],rdx
       mov       rbx,rcx
       mov       rcx,rdx
       mov       rsi,r8
       cmp       byte ptr [rbx+14],0
       jne       near ptr M06_L05
       test      rsi,rsi
       je        near ptr M06_L06
       mov       rcx,rsi
       call      qword ptr [7FFF68D67170]
       test      rax,rax
       jne       short M06_L01
       mov       rcx,rsi
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rdx,rax
M06_L00:
       test      rdx,rdx
       je        near ptr M06_L06
       lea       r8,[rbx+18]
       mov       ecx,[rbx+10]
       mov       eax,[r8+8]
       cmp       ecx,eax
       ja        near ptr M06_L07
       mov       r8,[r8]
       mov       r10d,ecx
       lea       r10,[r8+r10*2]
       sub       eax,ecx
       mov       esi,[rdx+8]
       cmp       esi,eax
       ja        near ptr M06_L08
       mov       r8d,esi
       add       r8,r8
       add       rdx,0C
       mov       rcx,r10
       call      qword ptr [7FFF68D6D928]; Precode of System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       add       [rbx+10],esi
       jmp       near ptr M06_L06
M06_L01:
       mov       rcx,rsi
       call      qword ptr [7FFF68D671A8]
       test      rax,rax
       je        near ptr M06_L04
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E8]
       mov       rsi,rax
M06_L02:
       mov       rcx,rsi
       lea       rdx,[rbx+18]
       mov       r9d,[rbx+10]
       mov       r8d,[rdx+8]
       cmp       r9d,r8d
       ja        near ptr M06_L07
       mov       rdx,[rdx]
       mov       r11d,r9d
       lea       rdx,[rdx+r11*2]
       sub       r8d,r9d
       mov       [rsp+38],rdx
       mov       [rsp+40],r8d
       xor       edx,edx
       mov       [rsp+28],rdx
       mov       [rsp+30],edx
       mov       rdx,[rbx]
       mov       [rsp+20],rdx
       lea       rdx,[rsp+38]
       lea       r9,[rsp+28]
       lea       r8,[rsp+48]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       test      eax,eax
       jne       short M06_L03
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M06_L02
M06_L03:
       mov       ecx,[rsp+48]
       add       [rbx+10],ecx
       jmp       short M06_L06
M06_L04:
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E0]
       mov       rcx,rax
       mov       r8,[rbx]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       xor       edx,edx
       call      qword ptr [r11]
       mov       rdx,rax
       jmp       near ptr M06_L00
M06_L05:
       call      qword ptr [7FFF68D600E8]
       mov       rdx,rax
       mov       rcx,rbx
       mov       r8,rsi
       xor       r9d,r9d
       call      qword ptr [7FFF68D84678]
M06_L06:
       nop
       add       rsp,58
       pop       rbx
       pop       rsi
       ret
M06_L07:
       call      qword ptr [7FFF68D6F2B8]
       int       3
M06_L08:
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FD0]
       jmp       short M06_L06
; Total bytes of code 397
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,58
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       movaps    [rsp+40],xmm4
       mov       rbx,rcx
       mov       esi,edx
       cmp       byte ptr [rbx+14],0
       jne       short M07_L03
M07_L00:
       lea       rdx,[rbx+18]
       mov       r8d,[rbx+10]
       mov       edi,[rdx+8]
       cmp       r8d,edi
       ja        near ptr M07_L10
       mov       rdx,[rdx]
       mov       ecx,r8d
       lea       rbp,[rdx+rcx*2]
       sub       edi,r8d
       mov       rcx,[rbx]
       test      esi,esi
       jl        short M07_L05
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       rdx,[rsp+40]
       lea       r8,[rsp+50]
       mov       ecx,esi
       call      qword ptr [7FFF68D80CF8]; Precode of System.Number.TryUInt32ToDecStr[[System.Char, System.Private.CoreLib]](UInt32, System.Span`1<Char>, Int32 ByRef)
M07_L01:
       test      eax,eax
       jne       short M07_L02
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M07_L00
M07_L02:
       mov       eax,[rsp+50]
       add       [rbx+10],eax
       jmp       short M07_L04
M07_L03:
       mov       rcx,rbx
       mov       edx,esi
       xor       r8d,r8d
       call      qword ptr [7FFF68D81B50]
M07_L04:
       nop
       add       rsp,58
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M07_L05:
       test      rcx,rcx
       je        short M07_L06
       call      qword ptr [7FFF68D71260]; Precode of System.Globalization.NumberFormatInfo.<GetInstance>g__GetProviderNonNull|58_0(System.IFormatProvider)
       jmp       short M07_L07
M07_L06:
       call      qword ptr [7FFF68D71248]; Precode of System.Globalization.NumberFormatInfo.get_CurrentInfo()
M07_L07:
       mov       r8,[rax+28]
       test      r8,r8
       jne       short M07_L08
       xor       r9d,r9d
       xor       r8d,r8d
       jmp       short M07_L09
M07_L08:
       lea       r9,[r8+0C]
       mov       r8d,[r8+8]
M07_L09:
       mov       [rsp+30],r9
       mov       [rsp+38],r8d
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       r8,[rsp+50]
       mov       [rsp+20],r8
       lea       r8,[rsp+30]
       lea       r9,[rsp+40]
       mov       ecx,esi
       mov       edx,0FFFFFFFF
       call      qword ptr [7FFF68D80CE0]
       jmp       near ptr M07_L01
M07_L10:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 255
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,38
       xor       eax,eax
       mov       [rsp+28],rax
       mov       rbx,rcx
       lea       rsi,[rbx+18]
       mov       rcx,rsi
       mov       eax,[rbx+10]
       cmp       eax,[rcx+8]
       ja        short M08_L01
       mov       rcx,[rcx]
       mov       [rsp+28],rcx
       mov       [rsp+30],eax
       lea       rcx,[rsp+28]
       call      qword ptr [7FFF68D6BB28]; Precode of System.String.Ctor(System.ReadOnlySpan`1<Char>)
       mov       rdi,rax
       mov       rbp,[rbx+8]
       xor       eax,eax
       mov       [rbx+8],rax
       mov       [rsi],rax
       mov       [rsi+8],rax
       mov       [rbx+10],eax
       test      rbp,rbp
       je        short M08_L00
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       mov       rdx,rbp
       xor       r8d,r8d
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798C0]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Return(Char[], Boolean)
M08_L00:
       mov       rax,rdi
       add       rsp,38
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M08_L01:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 126
```
```assembly
; System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,30
       mov       [rsp+28],rcx
       mov       rbx,rdx
       mov       rsi,r8
       mov       rdi,r9
       call      qword ptr [7FFF68D5EAC8]
       mov       rcx,rax
       mov       r9,rdi
       mov       rdx,rbx
       mov       r8,rsi
       lea       rax,[7FFF68D7FBE0]
       add       rsp,30
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax]
; Total bytes of code 56
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: Job-JESZGN(IterationCount=8, IterationTime=300ms, LaunchCount=2, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks.GenericAssertions()
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,98
       xor       eax,eax
       mov       [rsp+38],rax
       vxorps    xmm4,xmm4,xmm4
       vmovdqu   ymmword ptr [rsp+40],ymm4
       vmovdqu   ymmword ptr [rsp+60],ymm4
       vmovdqa   xmmword ptr [rsp+80],xmm4
       mov       [rsp+90],rax
       mov       rbx,rcx
       mov       esi,400
       jmp       near ptr M00_L08
M00_L00:
       mov       ebp,eax
M00_L01:
       mov       ecx,[rsp+68]
       imul      edx,ebp,0C2B2AE3D
       add       edx,1E409BF7
       rol       edx,11
       imul      edx,27D4EB2F
       imul      ecx,0C2B2AE3D
       add       ecx,edx
       rol       ecx,11
       imul      ecx,27D4EB2F
       mov       edx,ecx
       shr       edx,0F
       xor       edx,ecx
       imul      ecx,edx,85EBCA77
       mov       edx,ecx
       shr       edx,0D
       xor       edx,ecx
       imul      ecx,edx,0C2B2AE3D
       mov       r14d,ecx
       shr       r14d,10
       xor       r14d,ecx
       xor       ecx,ecx
       mov       [rsp+58],rcx
       mov       rcx,[rdi+8]
       mov       edx,r14d
       imul      rdx,[rdi+30]
       shr       rdx,20
       inc       rdx
       mov       eax,[rcx+8]
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,20
       cmp       edx,eax
       jae       near ptr M00_L34
       mov       edx,edx
       lea       rcx,[rcx+rdx*4+10]
       mov       ebp,[rcx]
       mov       rdi,[rdi+10]
       xor       r15d,r15d
       dec       ebp
       mov       r13d,[rdi+8]
       cmp       r13d,ebp
       jbe       near ptr M00_L10
M00_L02:
       mov       ecx,ebp
       shl       rcx,5
       lea       r12,[rdi+rcx+10]
       cmp       [r12+8],r14d
       jne       near ptr M00_L16
       mov       rcx,[r12+10]
       mov       ebp,[r12+18]
       test      rcx,rcx
       je        near ptr M00_L16
       mov       rdx,offset MT_System.RuntimeType
       cmp       [rcx],rdx
       jne       near ptr M00_L15
       mov       rdx,1DD925F70B8
       cmp       rcx,rdx
       sete      r13b
       movzx     r13d,r13b
M00_L03:
       test      r13d,r13d
       je        near ptr M00_L16
       test      ebp,ebp
       jne       near ptr M00_L16
M00_L04:
       test      r12,r12
       je        near ptr M00_L20
       mov       rcx,[r12]
       cmp       qword ptr [rcx+8],0
       jne       near ptr M00_L21
       mov       rdi,[rcx+18]
       mov       [rsp+30],rdi
M00_L05:
       mov       rbp,1DD925F70B8
       mov       [rsp+28],rbp
       mov       rcx,offset MT_Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks+Service<System.Int32, System.Int32>
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+28]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L22
M00_L06:
       cmp       ebx,1D
       je        near ptr M00_L23
       mov       rdi,[rsp+30]
       mov       rax,rdi
M00_L07:
       test      rax,rax
       je        near ptr M00_L25
       dec       esi
       mov       rbx,[rsp+0E0]
       je        near ptr M00_L11
M00_L08:
       mov       [rsp+0E0],rbx
       mov       rcx,[rbx+8]
       cmp       [rcx],cl
       vxorps    ymm0,ymm0,ymm0
       vmovdqu   ymmword ptr [rsp+70],ymm0
       vmovdqu   xmmword ptr [rsp+88],xmm0
       mov       rdi,[rcx+8]
       mov       rbp,1DD925F70B8
       cmp       qword ptr [rdi+8],0
       je        short M00_L10
       mov       r14,[rdi+18]
       test      r14,r14
       jne       near ptr M00_L17
       mov       [rsp+60],rbp
       xor       ecx,ecx
       mov       [rsp+68],ecx
       mov       [rsp+58],rcx
       lea       rcx,[rsp+60]
       cmp       qword ptr [rsp+58],0
       jne       short M00_L09
       mov       rcx,[rsp+60]
       mov       [rsp+58],rcx
       lea       rcx,[rsp+58]
       cmp       qword ptr [rsp+58],0
       je        short M00_L12
M00_L09:
       mov       rbp,[rcx]
       mov       rcx,offset MT_System.RuntimeType
       cmp       [rbp],rcx
       jne       short M00_L13
       mov       rcx,rbp
       call      00007FFF68F03E20
       test      eax,eax
       jne       near ptr M00_L00
       mov       rcx,rbp
       call      qword ptr [7FFF094D5FF8]; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       jmp       near ptr M00_L00
M00_L10:
       xor       r12d,r12d
       jmp       near ptr M00_L04
M00_L11:
       vzeroupper
       add       rsp,98
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       ret
M00_L12:
       xor       ebp,ebp
       jmp       near ptr M00_L01
M00_L13:
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       mov       ebp,eax
       jmp       near ptr M00_L01
M00_L14:
       cmp       r13d,ebp
       jbe       short M00_L10
       jmp       near ptr M00_L02
M00_L15:
       mov       rdx,1DD925F70B8
       mov       rax,[rcx]
       mov       rax,[rax+40]
       call      qword ptr [rax+10]
       mov       r13d,eax
       jmp       near ptr M00_L03
M00_L16:
       mov       ebp,[r12+0C]
       inc       r15d
       mov       r13d,[rdi+8]
       cmp       r13d,r15d
       jae       short M00_L14
       jmp       near ptr M00_L24
M00_L17:
       mov       [rsp+48],rbp
       xor       edx,edx
       mov       [rsp+50],edx
       lea       rdx,[rsp+48]
       mov       rcx,r14
       mov       r11,7FFF09250588
       call      qword ptr [r11]
       mov       ebp,eax
       mov       r8,[rdi+8]
       mov       edx,ebp
       imul      rdx,[rdi+30]
       shr       rdx,20
       inc       rdx
       mov       ecx,[r8+8]
       imul      rdx,rcx
       shr       rdx,20
       cmp       edx,[r8+8]
       jae       near ptr M00_L34
       mov       edx,edx
       lea       r8,[r8+rdx*4+10]
       mov       r15d,[r8]
       mov       rdi,[rdi+10]
       xor       r13d,r13d
       dec       r15d
M00_L18:
       cmp       [rdi+8],r15d
       jbe       near ptr M00_L10
       mov       r8d,r15d
       shl       r8,5
       lea       r12,[rdi+r8+10]
       cmp       [r12+8],ebp
       jne       short M00_L19
       vmovdqu   xmm0,xmmword ptr [r12+10]
       vmovdqu   xmmword ptr [rsp+48],xmm0
       mov       r8,1DD925F70B8
       mov       [rsp+38],r8
       xor       r8d,r8d
       mov       [rsp+40],r8d
       lea       r8,[rsp+38]
       lea       rdx,[rsp+48]
       mov       rcx,r14
       mov       r11,7FFF09250590
       call      qword ptr [r11]
       test      eax,eax
       jne       near ptr M00_L04
M00_L19:
       mov       r15d,[r12+0C]
       inc       r13d
       cmp       [rdi+8],r13d
       jae       short M00_L18
       jmp       near ptr M00_L24
M00_L20:
       xor       eax,eax
       jmp       near ptr M00_L07
M00_L21:
       call      qword ptr [7FFF095D5968]; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       mov       [rsp+30],rax
       jmp       near ptr M00_L05
M00_L22:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L06
M00_L23:
       mov       rdi,[rsp+30]
       mov       rdx,rdi
       mov       rcx,offset MT_System.Collections.ICollection
       call      qword ptr [7FFF094DECA0]; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       mov       rbx,rax
       mov       rcx,rbp
       call      qword ptr [7FFF094DE2E0]; System.RuntimeTypeHandle.GetElementType(System.RuntimeType)
       mov       rdi,rax
       mov       rcx,rbx
       mov       r11,7FFF09250578
       call      qword ptr [r11]
       mov       edx,eax
       mov       rcx,rdi
       call      qword ptr [7FFF095D5BF0]; System.Array.CreateInstance(System.Type, Int32)
       mov       rdi,rax
       mov       rcx,rbx
       mov       rdx,rdi
       mov       r11,7FFF09250580
       xor       r8d,r8d
       call      qword ptr [r11]
       mov       rax,rdi
       jmp       near ptr M00_L07
M00_L24:
       call      qword ptr [7FFF0930F480]
       int       3
M00_L25:
       lea       rcx,[rsp+70]
       mov       edx,1D
       mov       r8d,2
       call      qword ptr [7FFF0930C270]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       ja        near ptr M00_L30
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,14
       jb        short M00_L26
       vmovups   ymm0,[7FFF093883C0]
       vmovups   [rdx],ymm0
       mov       rcx,200072006F0066
       mov       [rdx+20],rcx
       mov       ecx,[rsp+80]
       add       ecx,14
       mov       [rsp+80],ecx
       jmp       short M00_L27
M00_L26:
       lea       rcx,[rsp+70]
       mov       rdx,1DD925FB920
       call      qword ptr [7FFF0966E340]
M00_L27:
       lea       rcx,[rsp+70]
       mov       rdx,7FFF09692DA0
       mov       rbp,1DD925F70B8
       mov       r8,rbp
       call      qword ptr [7FFF094DEA60]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       ja        short M00_L30
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,7
       jb        short M00_L28
       mov       rcx,65006B00280020
       mov       [rdx],rcx
       mov       dword ptr [rdx+8],3A0079
       mov       word ptr [rdx+0C],20
       mov       ecx,[rsp+80]
       add       ecx,7
       mov       [rsp+80],ecx
       jmp       short M00_L29
M00_L28:
       lea       rcx,[rsp+70]
       mov       rdx,1DD925FB960
       call      qword ptr [7FFF0966E340]
M00_L29:
       lea       rcx,[rsp+70]
       xor       edx,edx
       call      qword ptr [7FFF09307FD8]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       jbe       short M00_L31
M00_L30:
       call      qword ptr [7FFF094D7D50]
       int       3
M00_L31:
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,2
       jb        short M00_L32
       mov       dword ptr [rdx],2E0029
       mov       ecx,[rsp+80]
       add       ecx,2
       mov       [rsp+80],ecx
       jmp       short M00_L33
M00_L32:
       lea       rcx,[rsp+70]
       mov       rdx,1DD925F9BF8
       call      qword ptr [7FFF0966E340]
M00_L33:
       mov       rcx,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rcx,[rsp+70]
       call      qword ptr [7FFF0930C2A0]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFF095D7BD0]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L34:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 1549
```
```assembly
; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,88
       lea       rbp,[rsp+0C0]
       mov       [rbp+10],rcx
       cmp       qword ptr [rbp+10],0
       je        short M01_L01
       lea       rcx,[rbp+10]
       mov       [rbp-98],rcx
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-98]
       call      qword ptr [rax]
       mov       ebx,eax
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       eax,ebx
M01_L00:
       add       rsp,88
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M01_L01:
       xor       eax,eax
       jmp       short M01_L00
; Total bytes of code 119
```
```assembly
; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       push      rsi
       push      rbx
       sub       rsp,28
       mov       [rsp+20],rcx
       mov       rbx,rcx
M02_L00:
       mov       rsi,[rbx+8]
       test      rsi,rsi
       je        short M02_L01
       mov       ecx,[rsi+10]
       cmp       ecx,8
       jne       short M02_L03
       mov       rcx,rbx
       mov       rdx,rsi
       xor       r8d,r8d
       call      qword ptr [7FFF68D7E1B0]
M02_L01:
       cmp       qword ptr [rbx+8],0
       jne       short M02_L02
       mov       rax,[rbx+18]
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M02_L02:
       mov       rcx,rbx
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax]
M02_L03:
       cmp       ecx,8
       ja        short M02_L04
       mov       ecx,ecx
       lea       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       edx,[rdx+rcx*4]
       lea       r8,[M02_L00]
       add       rdx,r8
       jmp       rdx
M02_L04:
       mov       rcx,[rsi+8]
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D74C78]
       int       3
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1A0]
       jmp       short M02_L01
       mov       rcx,rbx
       xor       edx,edx
       call      qword ptr [7FFF68D7E1A8]; Precode of System.Lazy`1[[System.__Canon, System.Private.CoreLib]].ViaFactory(System.Threading.LazyThreadSafetyMode)
       jmp       short M02_L01
       mov       rcx,[rbx]
       call      qword ptr [7FFF68D5B838]
       mov       rcx,rax
       call      qword ptr [7FFF68D62EA0]
       mov       rcx,rax
       call      qword ptr [7FFF68D84008]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
       mov       rax,[rbx+10]
       test      rax,rax
       je        short M02_L05
       mov       rcx,[rax+8]
       call      qword ptr [rax+18]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
M02_L05:
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1C8]
       jmp       near ptr M02_L01
       mov       rcx,rbx
       mov       rdx,rsi
       mov       r8d,1
       call      qword ptr [7FFF68D7E1B0]
       jmp       near ptr M02_L01
; Total bytes of code 269
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       test      rdx,rdx
       je        short M03_L02
       mov       rax,[rdx]
       movzx     r8d,word ptr [rax+0E]
       test      r8,r8
       je        short M03_L05
       mov       rax,[rax+38]
       cmp       r8,4
       jl        short M03_L01
       cmp       [rax],rcx
       je        short M03_L02
M03_L00:
       cmp       [rax+8],rcx
       je        short M03_L02
       cmp       [rax+10],rcx
       je        short M03_L02
       cmp       [rax+18],rcx
       je        short M03_L02
       add       rax,20
       add       r8,0FFFFFFFFFFFFFFFC
       cmp       r8,4
       jge       short M03_L04
       test      r8,r8
       je        short M03_L05
M03_L01:
       cmp       [rax],rcx
       jne       short M03_L03
M03_L02:
       mov       rax,rdx
       ret
M03_L03:
       add       rax,8
       dec       r8
       test      r8,r8
       jle       short M03_L05
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L03
M03_L04:
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L00
M03_L05:
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       jmp       qword ptr [rax]
; Total bytes of code 115
```
```assembly
; System.RuntimeTypeHandle.GetElementType(System.RuntimeType)
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rbx,rcx
       mov       rcx,[rbx+18]
       call      00007FFF68F3E8E0
       mov       rsi,rax
       test      rsi,rsi
       jne       short M04_L00
       mov       rcx,7FFF0977B278
       call      CORINFO_HELP_COUNTPROFILE32
       xor       eax,eax
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M04_L00:
       mov       rcx,7FFF0977B27C
       call      CORINFO_HELP_COUNTPROFILE32
       mov       rcx,rsi
       call      qword ptr [7FFF09305860]; System.RuntimeTypeHandle.GetRuntimeTypeFromHandle(IntPtr)
       nop
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
; Total bytes of code 82
```
```assembly
; System.Array.CreateInstance(System.Type, Int32)
       sub       rsp,28
       mov       [rsp+38],edx
       test      rcx,rcx
       je        short M05_L01
       mov       eax,[rsp+38]
       test      eax,eax
       jl        short M05_L02
       mov       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       cmp       r11,[rcx]
       jne       short M05_L03
M05_L00:
       call      qword ptr [7FFF68D67080]
       test      rax,rax
       je        short M05_L04
       lea       r8,[rsp+38]
       mov       rcx,rax
       mov       edx,1
       xor       r9d,r9d
       call      qword ptr [7FFF68D6A298]; Precode of System.Array.InternalCreate(System.RuntimeType, Int32, Int32*, Int32*)
       nop
       add       rsp,28
       ret
M05_L01:
       mov       rcx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rcx]
       call      qword ptr [7FFF68D6C210]
       int       3
M05_L02:
       mov       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rdx,[rdx]
       mov       ecx,eax
       call      qword ptr [7FFF68D81908]
       int       3
M05_L03:
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rcx,rax
       jmp       short M05_L00
M05_L04:
       mov       ecx,39
       mov       edx,4E
       call      qword ptr [7FFF68D6F3B0]
       int       3
; Total bytes of code 140
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       esi,edx
       mov       edi,r8d
       xor       eax,eax
       mov       [rbx],rax
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       imul      edx,edi,0B
       add       edx,esi
       mov       eax,100
       cmp       edx,100
       cmovle    edx,eax
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798B8]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Rent(Int32)
       mov       [rbx+8],rax
       test      rax,rax
       je        short M06_L01
       lea       rcx,[rax+10]
       mov       eax,[rax+8]
M06_L00:
       mov       [rbx+18],rcx
       mov       [rbx+20],eax
       xor       eax,eax
       mov       [rbx+10],eax
       mov       byte ptr [rbx+14],0
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
M06_L01:
       xor       ecx,ecx
       xor       eax,eax
       jmp       short M06_L00
; Total bytes of code 102
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       push      rsi
       push      rbx
       sub       rsp,58
       xor       eax,eax
       mov       [rsp+28],rax
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       mov       [rsp+40],rax
       mov       [rsp+50],rdx
       mov       rbx,rcx
       mov       rcx,rdx
       mov       rsi,r8
       cmp       byte ptr [rbx+14],0
       jne       near ptr M07_L05
       test      rsi,rsi
       je        near ptr M07_L06
       mov       rcx,rsi
       call      qword ptr [7FFF68D67170]
       test      rax,rax
       jne       short M07_L01
       mov       rcx,rsi
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rdx,rax
M07_L00:
       test      rdx,rdx
       je        near ptr M07_L06
       lea       r8,[rbx+18]
       mov       ecx,[rbx+10]
       mov       eax,[r8+8]
       cmp       ecx,eax
       ja        near ptr M07_L07
       mov       r8,[r8]
       mov       r10d,ecx
       lea       r10,[r8+r10*2]
       sub       eax,ecx
       mov       esi,[rdx+8]
       cmp       esi,eax
       ja        near ptr M07_L08
       mov       r8d,esi
       add       r8,r8
       add       rdx,0C
       mov       rcx,r10
       call      qword ptr [7FFF68D6D928]; Precode of System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       add       [rbx+10],esi
       jmp       near ptr M07_L06
M07_L01:
       mov       rcx,rsi
       call      qword ptr [7FFF68D671A8]
       test      rax,rax
       je        near ptr M07_L04
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E8]
       mov       rsi,rax
M07_L02:
       mov       rcx,rsi
       lea       rdx,[rbx+18]
       mov       r9d,[rbx+10]
       mov       r8d,[rdx+8]
       cmp       r9d,r8d
       ja        near ptr M07_L07
       mov       rdx,[rdx]
       mov       r11d,r9d
       lea       rdx,[rdx+r11*2]
       sub       r8d,r9d
       mov       [rsp+38],rdx
       mov       [rsp+40],r8d
       xor       edx,edx
       mov       [rsp+28],rdx
       mov       [rsp+30],edx
       mov       rdx,[rbx]
       mov       [rsp+20],rdx
       lea       rdx,[rsp+38]
       lea       r9,[rsp+28]
       lea       r8,[rsp+48]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       test      eax,eax
       jne       short M07_L03
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M07_L02
M07_L03:
       mov       ecx,[rsp+48]
       add       [rbx+10],ecx
       jmp       short M07_L06
M07_L04:
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E0]
       mov       rcx,rax
       mov       r8,[rbx]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       xor       edx,edx
       call      qword ptr [r11]
       mov       rdx,rax
       jmp       near ptr M07_L00
M07_L05:
       call      qword ptr [7FFF68D600E8]
       mov       rdx,rax
       mov       rcx,rbx
       mov       r8,rsi
       xor       r9d,r9d
       call      qword ptr [7FFF68D84678]
M07_L06:
       nop
       add       rsp,58
       pop       rbx
       pop       rsi
       ret
M07_L07:
       call      qword ptr [7FFF68D6F2B8]
       int       3
M07_L08:
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FD0]
       jmp       short M07_L06
; Total bytes of code 397
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,58
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       movaps    [rsp+40],xmm4
       mov       rbx,rcx
       mov       esi,edx
       cmp       byte ptr [rbx+14],0
       jne       short M08_L03
M08_L00:
       lea       rdx,[rbx+18]
       mov       r8d,[rbx+10]
       mov       edi,[rdx+8]
       cmp       r8d,edi
       ja        near ptr M08_L10
       mov       rdx,[rdx]
       mov       ecx,r8d
       lea       rbp,[rdx+rcx*2]
       sub       edi,r8d
       mov       rcx,[rbx]
       test      esi,esi
       jl        short M08_L05
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       rdx,[rsp+40]
       lea       r8,[rsp+50]
       mov       ecx,esi
       call      qword ptr [7FFF68D80CF8]; Precode of System.Number.TryUInt32ToDecStr[[System.Char, System.Private.CoreLib]](UInt32, System.Span`1<Char>, Int32 ByRef)
M08_L01:
       test      eax,eax
       jne       short M08_L02
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M08_L00
M08_L02:
       mov       eax,[rsp+50]
       add       [rbx+10],eax
       jmp       short M08_L04
M08_L03:
       mov       rcx,rbx
       mov       edx,esi
       xor       r8d,r8d
       call      qword ptr [7FFF68D81B50]
M08_L04:
       nop
       add       rsp,58
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M08_L05:
       test      rcx,rcx
       je        short M08_L06
       call      qword ptr [7FFF68D71260]; Precode of System.Globalization.NumberFormatInfo.<GetInstance>g__GetProviderNonNull|58_0(System.IFormatProvider)
       jmp       short M08_L07
M08_L06:
       call      qword ptr [7FFF68D71248]; Precode of System.Globalization.NumberFormatInfo.get_CurrentInfo()
M08_L07:
       mov       r8,[rax+28]
       test      r8,r8
       jne       short M08_L08
       xor       r9d,r9d
       xor       r8d,r8d
       jmp       short M08_L09
M08_L08:
       lea       r9,[r8+0C]
       mov       r8d,[r8+8]
M08_L09:
       mov       [rsp+30],r9
       mov       [rsp+38],r8d
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       r8,[rsp+50]
       mov       [rsp+20],r8
       lea       r8,[rsp+30]
       lea       r9,[rsp+40]
       mov       ecx,esi
       mov       edx,0FFFFFFFF
       call      qword ptr [7FFF68D80CE0]
       jmp       near ptr M08_L01
M08_L10:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 255
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,38
       xor       eax,eax
       mov       [rsp+28],rax
       mov       rbx,rcx
       lea       rsi,[rbx+18]
       mov       rcx,rsi
       mov       eax,[rbx+10]
       cmp       eax,[rcx+8]
       ja        short M09_L01
       mov       rcx,[rcx]
       mov       [rsp+28],rcx
       mov       [rsp+30],eax
       lea       rcx,[rsp+28]
       call      qword ptr [7FFF68D6BB28]; Precode of System.String.Ctor(System.ReadOnlySpan`1<Char>)
       mov       rdi,rax
       mov       rbp,[rbx+8]
       xor       eax,eax
       mov       [rbx+8],rax
       mov       [rsi],rax
       mov       [rsi+8],rax
       mov       [rbx+10],eax
       test      rbp,rbp
       je        short M09_L00
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       mov       rdx,rbp
       xor       r8d,r8d
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798C0]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Return(Char[], Boolean)
M09_L00:
       mov       rax,rdi
       add       rsp,38
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M09_L01:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 126
```
```assembly
; System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,30
       mov       [rsp+28],rcx
       mov       rbx,rdx
       mov       rsi,r8
       mov       rdi,r9
       call      qword ptr [7FFF68D5EAC8]
       mov       rcx,rax
       mov       r9,rdi
       mov       rdx,rbx
       mov       r8,rsi
       lea       rax,[7FFF68D7FBE0]
       add       rsp,30
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax]
; Total bytes of code 56
```
```assembly
; System.RuntimeTypeHandle.GetRuntimeTypeFromHandle(IntPtr)
       test      cl,2
       jne       short M11_L01
       mov       rax,[rcx+20]
       add       rax,10
       mov       rax,[rax]
M11_L00:
       test      rax,rax
       je        short M11_L02
       ret
M11_L01:
       mov       rax,rcx
       and       rax,0FFFFFFFFFFFFFFFD
       add       rax,8
       mov       rax,[rax]
       jmp       short M11_L00
M11_L02:
       jmp       qword ptr [7FFF09305C80]; System.RuntimeTypeHandle.GetRuntimeTypeFromHandleSlow(IntPtr)
; Total bytes of code 44
```
```assembly
; System.RuntimeTypeHandle.GetRuntimeTypeFromHandleSlow(IntPtr)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,98
       lea       rbp,[rsp+0D0]
       xor       eax,eax
       mov       [rbp-40],rax
       mov       [rbp-0A0],rcx
       lea       rdx,[rbp-40]
       mov       [rbp-0A8],rdx
       lea       rcx,[rbp-98]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-0A0]
       mov       rdx,[rbp-0A8]
       call      qword ptr [rax]
       lea       rcx,[rbp-98]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       rax,[rbp-40]
       add       rsp,98
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
; Total bytes of code 124
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: Job-JESZGN(IterationCount=8, IterationTime=300ms, LaunchCount=2, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks.GenericNullCheck()
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,98
       xor       eax,eax
       mov       [rsp+38],rax
       vxorps    xmm4,xmm4,xmm4
       vmovdqu   ymmword ptr [rsp+40],ymm4
       vmovdqu   ymmword ptr [rsp+60],ymm4
       vmovdqa   xmmword ptr [rsp+80],xmm4
       mov       [rsp+90],rax
       mov       rbx,rcx
       mov       esi,400
       jmp       near ptr M00_L08
M00_L00:
       mov       ebp,eax
M00_L01:
       mov       ecx,[rsp+68]
       imul      edx,ebp,0C2B2AE3D
       add       edx,626A1325
       rol       edx,11
       imul      edx,27D4EB2F
       imul      ecx,0C2B2AE3D
       add       ecx,edx
       rol       ecx,11
       imul      ecx,27D4EB2F
       mov       edx,ecx
       shr       edx,0F
       xor       edx,ecx
       imul      ecx,edx,85EBCA77
       mov       edx,ecx
       shr       edx,0D
       xor       edx,ecx
       imul      ecx,edx,0C2B2AE3D
       mov       r14d,ecx
       shr       r14d,10
       xor       r14d,ecx
       xor       ecx,ecx
       mov       [rsp+58],rcx
       mov       rcx,[rdi+8]
       mov       edx,r14d
       imul      rdx,[rdi+30]
       shr       rdx,20
       inc       rdx
       mov       eax,[rcx+8]
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,20
       cmp       edx,eax
       jae       near ptr M00_L34
       mov       edx,edx
       lea       rcx,[rcx+rdx*4+10]
       mov       ebp,[rcx]
       mov       rdi,[rdi+10]
       xor       r15d,r15d
       dec       ebp
       mov       r13d,[rdi+8]
       cmp       r13d,ebp
       jbe       near ptr M00_L10
M00_L02:
       mov       ecx,ebp
       shl       rcx,5
       lea       r12,[rdi+rcx+10]
       cmp       [r12+8],r14d
       jne       near ptr M00_L16
       mov       rcx,[r12+10]
       mov       ebp,[r12+18]
       test      rcx,rcx
       je        near ptr M00_L16
       mov       rdx,offset MT_System.RuntimeType
       cmp       [rcx],rdx
       jne       near ptr M00_L15
       mov       rdx,276125F70B8
       cmp       rcx,rdx
       sete      r13b
       movzx     r13d,r13b
M00_L03:
       test      r13d,r13d
       je        near ptr M00_L16
       test      ebp,ebp
       jne       near ptr M00_L16
M00_L04:
       test      r12,r12
       je        near ptr M00_L20
       mov       rcx,[r12]
       cmp       qword ptr [rcx+8],0
       jne       near ptr M00_L21
       mov       rdi,[rcx+18]
       mov       [rsp+30],rdi
M00_L05:
       mov       rbp,276125F70B8
       mov       [rsp+28],rbp
       mov       rcx,offset MT_Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks+Service<System.Int32, System.Int32>
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+28]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L22
M00_L06:
       cmp       ebx,1D
       je        near ptr M00_L23
       mov       rdi,[rsp+30]
       mov       rax,rdi
M00_L07:
       test      rax,rax
       je        near ptr M00_L25
       dec       esi
       mov       rbx,[rsp+0E0]
       je        near ptr M00_L11
M00_L08:
       mov       [rsp+0E0],rbx
       mov       rcx,[rbx+8]
       cmp       [rcx],cl
       vxorps    ymm0,ymm0,ymm0
       vmovdqu   ymmword ptr [rsp+70],ymm0
       vmovdqu   xmmword ptr [rsp+88],xmm0
       mov       rdi,[rcx+8]
       mov       rbp,276125F70B8
       cmp       qword ptr [rdi+8],0
       je        short M00_L10
       mov       r14,[rdi+18]
       test      r14,r14
       jne       near ptr M00_L17
       mov       [rsp+60],rbp
       xor       ecx,ecx
       mov       [rsp+68],ecx
       mov       [rsp+58],rcx
       lea       rcx,[rsp+60]
       cmp       qword ptr [rsp+58],0
       jne       short M00_L09
       mov       rcx,[rsp+60]
       mov       [rsp+58],rcx
       lea       rcx,[rsp+58]
       cmp       qword ptr [rsp+58],0
       je        short M00_L12
M00_L09:
       mov       rbp,[rcx]
       mov       rcx,offset MT_System.RuntimeType
       cmp       [rbp],rcx
       jne       short M00_L13
       mov       rcx,rbp
       call      00007FFF68F03E20
       test      eax,eax
       jne       near ptr M00_L00
       mov       rcx,rbp
       call      qword ptr [7FFF094D5FF8]; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       jmp       near ptr M00_L00
M00_L10:
       xor       r12d,r12d
       jmp       near ptr M00_L04
M00_L11:
       vzeroupper
       add       rsp,98
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       ret
M00_L12:
       xor       ebp,ebp
       jmp       near ptr M00_L01
M00_L13:
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       mov       ebp,eax
       jmp       near ptr M00_L01
M00_L14:
       cmp       r13d,ebp
       jbe       short M00_L10
       jmp       near ptr M00_L02
M00_L15:
       mov       rdx,276125F70B8
       mov       rax,[rcx]
       mov       rax,[rax+40]
       call      qword ptr [rax+10]
       mov       r13d,eax
       jmp       near ptr M00_L03
M00_L16:
       mov       ebp,[r12+0C]
       inc       r15d
       mov       r13d,[rdi+8]
       cmp       r13d,r15d
       jae       short M00_L14
       jmp       near ptr M00_L24
M00_L17:
       mov       [rsp+48],rbp
       xor       edx,edx
       mov       [rsp+50],edx
       lea       rdx,[rsp+48]
       mov       rcx,r14
       mov       r11,7FFF09250588
       call      qword ptr [r11]
       mov       ebp,eax
       mov       r8,[rdi+8]
       mov       edx,ebp
       imul      rdx,[rdi+30]
       shr       rdx,20
       inc       rdx
       mov       ecx,[r8+8]
       imul      rdx,rcx
       shr       rdx,20
       cmp       edx,[r8+8]
       jae       near ptr M00_L34
       mov       edx,edx
       lea       r8,[r8+rdx*4+10]
       mov       r15d,[r8]
       mov       rdi,[rdi+10]
       xor       r13d,r13d
       dec       r15d
M00_L18:
       cmp       [rdi+8],r15d
       jbe       near ptr M00_L10
       mov       r8d,r15d
       shl       r8,5
       lea       r12,[rdi+r8+10]
       cmp       [r12+8],ebp
       jne       short M00_L19
       vmovdqu   xmm0,xmmword ptr [r12+10]
       vmovdqu   xmmword ptr [rsp+48],xmm0
       mov       r8,276125F70B8
       mov       [rsp+38],r8
       xor       r8d,r8d
       mov       [rsp+40],r8d
       lea       r8,[rsp+38]
       lea       rdx,[rsp+48]
       mov       rcx,r14
       mov       r11,7FFF09250590
       call      qword ptr [r11]
       test      eax,eax
       jne       near ptr M00_L04
M00_L19:
       mov       r15d,[r12+0C]
       inc       r13d
       cmp       [rdi+8],r13d
       jae       short M00_L18
       jmp       near ptr M00_L24
M00_L20:
       xor       eax,eax
       jmp       near ptr M00_L07
M00_L21:
       call      qword ptr [7FFF095D5968]; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       mov       [rsp+30],rax
       jmp       near ptr M00_L05
M00_L22:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L06
M00_L23:
       mov       rdi,[rsp+30]
       mov       rdx,rdi
       mov       rcx,offset MT_System.Collections.ICollection
       call      qword ptr [7FFF094DECA0]; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       mov       rbx,rax
       mov       rcx,rbp
       call      qword ptr [7FFF094DE2E0]; System.RuntimeTypeHandle.GetElementType(System.RuntimeType)
       mov       rdi,rax
       mov       rcx,rbx
       mov       r11,7FFF09250578
       call      qword ptr [r11]
       mov       edx,eax
       mov       rcx,rdi
       call      qword ptr [7FFF095D5BF0]; System.Array.CreateInstance(System.Type, Int32)
       mov       rdi,rax
       mov       rcx,rbx
       mov       rdx,rdi
       mov       r11,7FFF09250580
       xor       r8d,r8d
       call      qword ptr [r11]
       mov       rax,rdi
       jmp       near ptr M00_L07
M00_L24:
       call      qword ptr [7FFF0930F480]
       int       3
M00_L25:
       lea       rcx,[rsp+70]
       mov       edx,1D
       mov       r8d,2
       call      qword ptr [7FFF0930C270]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       ja        near ptr M00_L30
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,14
       jb        short M00_L26
       vmovups   ymm0,[7FFF09388420]
       vmovups   [rdx],ymm0
       mov       rcx,200072006F0066
       mov       [rdx+20],rcx
       mov       ecx,[rsp+80]
       add       ecx,14
       mov       [rsp+80],ecx
       jmp       short M00_L27
M00_L26:
       lea       rcx,[rsp+70]
       mov       rdx,276125FB920
       call      qword ptr [7FFF0966E340]
M00_L27:
       lea       rcx,[rsp+70]
       mov       rdx,7FFF09692DA0
       mov       rbp,276125F70B8
       mov       r8,rbp
       call      qword ptr [7FFF094DEA60]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       ja        short M00_L30
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,7
       jb        short M00_L28
       mov       rcx,65006B00280020
       mov       [rdx],rcx
       mov       dword ptr [rdx+8],3A0079
       mov       word ptr [rdx+0C],20
       mov       ecx,[rsp+80]
       add       ecx,7
       mov       [rsp+80],ecx
       jmp       short M00_L29
M00_L28:
       lea       rcx,[rsp+70]
       mov       rdx,276125FB960
       call      qword ptr [7FFF0966E340]
M00_L29:
       lea       rcx,[rsp+70]
       xor       edx,edx
       call      qword ptr [7FFF09307FD8]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       jbe       short M00_L31
M00_L30:
       call      qword ptr [7FFF094D7D50]
       int       3
M00_L31:
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,2
       jb        short M00_L32
       mov       dword ptr [rdx],2E0029
       mov       ecx,[rsp+80]
       add       ecx,2
       mov       [rsp+80],ecx
       jmp       short M00_L33
M00_L32:
       lea       rcx,[rsp+70]
       mov       rdx,276125F9BF8
       call      qword ptr [7FFF0966E340]
M00_L33:
       mov       rcx,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rcx,[rsp+70]
       call      qword ptr [7FFF0930C2A0]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFF095D7BD0]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L34:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 1549
```
```assembly
; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,88
       lea       rbp,[rsp+0C0]
       mov       [rbp+10],rcx
       cmp       qword ptr [rbp+10],0
       je        short M01_L01
       lea       rcx,[rbp+10]
       mov       [rbp-98],rcx
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-98]
       call      qword ptr [rax]
       mov       ebx,eax
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       eax,ebx
M01_L00:
       add       rsp,88
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M01_L01:
       xor       eax,eax
       jmp       short M01_L00
; Total bytes of code 119
```
```assembly
; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       push      rsi
       push      rbx
       sub       rsp,28
       mov       [rsp+20],rcx
       mov       rbx,rcx
M02_L00:
       mov       rsi,[rbx+8]
       test      rsi,rsi
       je        short M02_L01
       mov       ecx,[rsi+10]
       cmp       ecx,8
       jne       short M02_L03
       mov       rcx,rbx
       mov       rdx,rsi
       xor       r8d,r8d
       call      qword ptr [7FFF68D7E1B0]
M02_L01:
       cmp       qword ptr [rbx+8],0
       jne       short M02_L02
       mov       rax,[rbx+18]
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M02_L02:
       mov       rcx,rbx
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax]
M02_L03:
       cmp       ecx,8
       ja        short M02_L04
       mov       ecx,ecx
       lea       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       edx,[rdx+rcx*4]
       lea       r8,[M02_L00]
       add       rdx,r8
       jmp       rdx
M02_L04:
       mov       rcx,[rsi+8]
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D74C78]
       int       3
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1A0]
       jmp       short M02_L01
       mov       rcx,rbx
       xor       edx,edx
       call      qword ptr [7FFF68D7E1A8]; Precode of System.Lazy`1[[System.__Canon, System.Private.CoreLib]].ViaFactory(System.Threading.LazyThreadSafetyMode)
       jmp       short M02_L01
       mov       rcx,[rbx]
       call      qword ptr [7FFF68D5B838]
       mov       rcx,rax
       call      qword ptr [7FFF68D62EA0]
       mov       rcx,rax
       call      qword ptr [7FFF68D84008]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
       mov       rax,[rbx+10]
       test      rax,rax
       je        short M02_L05
       mov       rcx,[rax+8]
       call      qword ptr [rax+18]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
M02_L05:
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1C8]
       jmp       near ptr M02_L01
       mov       rcx,rbx
       mov       rdx,rsi
       mov       r8d,1
       call      qword ptr [7FFF68D7E1B0]
       jmp       near ptr M02_L01
; Total bytes of code 269
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       test      rdx,rdx
       je        short M03_L02
       mov       rax,[rdx]
       movzx     r8d,word ptr [rax+0E]
       test      r8,r8
       je        short M03_L05
       mov       rax,[rax+38]
       cmp       r8,4
       jl        short M03_L01
       cmp       [rax],rcx
       je        short M03_L02
M03_L00:
       cmp       [rax+8],rcx
       je        short M03_L02
       cmp       [rax+10],rcx
       je        short M03_L02
       cmp       [rax+18],rcx
       je        short M03_L02
       add       rax,20
       add       r8,0FFFFFFFFFFFFFFFC
       cmp       r8,4
       jge       short M03_L04
       test      r8,r8
       je        short M03_L05
M03_L01:
       cmp       [rax],rcx
       jne       short M03_L03
M03_L02:
       mov       rax,rdx
       ret
M03_L03:
       add       rax,8
       dec       r8
       test      r8,r8
       jle       short M03_L05
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L03
M03_L04:
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L00
M03_L05:
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       jmp       qword ptr [rax]
; Total bytes of code 115
```
```assembly
; System.RuntimeTypeHandle.GetElementType(System.RuntimeType)
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rbx,rcx
       mov       rcx,[rbx+18]
       call      00007FFF68F3E8E0
       mov       rsi,rax
       test      rsi,rsi
       jne       short M04_L00
       mov       rcx,7FFF0977B198
       call      CORINFO_HELP_COUNTPROFILE32
       xor       eax,eax
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M04_L00:
       mov       rcx,7FFF0977B19C
       call      CORINFO_HELP_COUNTPROFILE32
       mov       rcx,rsi
       call      qword ptr [7FFF09305860]; System.RuntimeTypeHandle.GetRuntimeTypeFromHandle(IntPtr)
       nop
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
; Total bytes of code 82
```
```assembly
; System.Array.CreateInstance(System.Type, Int32)
       sub       rsp,28
       mov       [rsp+38],edx
       test      rcx,rcx
       je        short M05_L01
       mov       eax,[rsp+38]
       test      eax,eax
       jl        short M05_L02
       mov       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       cmp       r11,[rcx]
       jne       short M05_L03
M05_L00:
       call      qword ptr [7FFF68D67080]
       test      rax,rax
       je        short M05_L04
       lea       r8,[rsp+38]
       mov       rcx,rax
       mov       edx,1
       xor       r9d,r9d
       call      qword ptr [7FFF68D6A298]; Precode of System.Array.InternalCreate(System.RuntimeType, Int32, Int32*, Int32*)
       nop
       add       rsp,28
       ret
M05_L01:
       mov       rcx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rcx]
       call      qword ptr [7FFF68D6C210]
       int       3
M05_L02:
       mov       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rdx,[rdx]
       mov       ecx,eax
       call      qword ptr [7FFF68D81908]
       int       3
M05_L03:
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rcx,rax
       jmp       short M05_L00
M05_L04:
       mov       ecx,39
       mov       edx,4E
       call      qword ptr [7FFF68D6F3B0]
       int       3
; Total bytes of code 140
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       esi,edx
       mov       edi,r8d
       xor       eax,eax
       mov       [rbx],rax
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       imul      edx,edi,0B
       add       edx,esi
       mov       eax,100
       cmp       edx,100
       cmovle    edx,eax
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798B8]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Rent(Int32)
       mov       [rbx+8],rax
       test      rax,rax
       je        short M06_L01
       lea       rcx,[rax+10]
       mov       eax,[rax+8]
M06_L00:
       mov       [rbx+18],rcx
       mov       [rbx+20],eax
       xor       eax,eax
       mov       [rbx+10],eax
       mov       byte ptr [rbx+14],0
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
M06_L01:
       xor       ecx,ecx
       xor       eax,eax
       jmp       short M06_L00
; Total bytes of code 102
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       push      rsi
       push      rbx
       sub       rsp,58
       xor       eax,eax
       mov       [rsp+28],rax
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       mov       [rsp+40],rax
       mov       [rsp+50],rdx
       mov       rbx,rcx
       mov       rcx,rdx
       mov       rsi,r8
       cmp       byte ptr [rbx+14],0
       jne       near ptr M07_L05
       test      rsi,rsi
       je        near ptr M07_L06
       mov       rcx,rsi
       call      qword ptr [7FFF68D67170]
       test      rax,rax
       jne       short M07_L01
       mov       rcx,rsi
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rdx,rax
M07_L00:
       test      rdx,rdx
       je        near ptr M07_L06
       lea       r8,[rbx+18]
       mov       ecx,[rbx+10]
       mov       eax,[r8+8]
       cmp       ecx,eax
       ja        near ptr M07_L07
       mov       r8,[r8]
       mov       r10d,ecx
       lea       r10,[r8+r10*2]
       sub       eax,ecx
       mov       esi,[rdx+8]
       cmp       esi,eax
       ja        near ptr M07_L08
       mov       r8d,esi
       add       r8,r8
       add       rdx,0C
       mov       rcx,r10
       call      qword ptr [7FFF68D6D928]; Precode of System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       add       [rbx+10],esi
       jmp       near ptr M07_L06
M07_L01:
       mov       rcx,rsi
       call      qword ptr [7FFF68D671A8]
       test      rax,rax
       je        near ptr M07_L04
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E8]
       mov       rsi,rax
M07_L02:
       mov       rcx,rsi
       lea       rdx,[rbx+18]
       mov       r9d,[rbx+10]
       mov       r8d,[rdx+8]
       cmp       r9d,r8d
       ja        near ptr M07_L07
       mov       rdx,[rdx]
       mov       r11d,r9d
       lea       rdx,[rdx+r11*2]
       sub       r8d,r9d
       mov       [rsp+38],rdx
       mov       [rsp+40],r8d
       xor       edx,edx
       mov       [rsp+28],rdx
       mov       [rsp+30],edx
       mov       rdx,[rbx]
       mov       [rsp+20],rdx
       lea       rdx,[rsp+38]
       lea       r9,[rsp+28]
       lea       r8,[rsp+48]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       test      eax,eax
       jne       short M07_L03
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M07_L02
M07_L03:
       mov       ecx,[rsp+48]
       add       [rbx+10],ecx
       jmp       short M07_L06
M07_L04:
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E0]
       mov       rcx,rax
       mov       r8,[rbx]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       xor       edx,edx
       call      qword ptr [r11]
       mov       rdx,rax
       jmp       near ptr M07_L00
M07_L05:
       call      qword ptr [7FFF68D600E8]
       mov       rdx,rax
       mov       rcx,rbx
       mov       r8,rsi
       xor       r9d,r9d
       call      qword ptr [7FFF68D84678]
M07_L06:
       nop
       add       rsp,58
       pop       rbx
       pop       rsi
       ret
M07_L07:
       call      qword ptr [7FFF68D6F2B8]
       int       3
M07_L08:
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FD0]
       jmp       short M07_L06
; Total bytes of code 397
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,58
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       movaps    [rsp+40],xmm4
       mov       rbx,rcx
       mov       esi,edx
       cmp       byte ptr [rbx+14],0
       jne       short M08_L03
M08_L00:
       lea       rdx,[rbx+18]
       mov       r8d,[rbx+10]
       mov       edi,[rdx+8]
       cmp       r8d,edi
       ja        near ptr M08_L10
       mov       rdx,[rdx]
       mov       ecx,r8d
       lea       rbp,[rdx+rcx*2]
       sub       edi,r8d
       mov       rcx,[rbx]
       test      esi,esi
       jl        short M08_L05
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       rdx,[rsp+40]
       lea       r8,[rsp+50]
       mov       ecx,esi
       call      qword ptr [7FFF68D80CF8]; Precode of System.Number.TryUInt32ToDecStr[[System.Char, System.Private.CoreLib]](UInt32, System.Span`1<Char>, Int32 ByRef)
M08_L01:
       test      eax,eax
       jne       short M08_L02
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M08_L00
M08_L02:
       mov       eax,[rsp+50]
       add       [rbx+10],eax
       jmp       short M08_L04
M08_L03:
       mov       rcx,rbx
       mov       edx,esi
       xor       r8d,r8d
       call      qword ptr [7FFF68D81B50]
M08_L04:
       nop
       add       rsp,58
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M08_L05:
       test      rcx,rcx
       je        short M08_L06
       call      qword ptr [7FFF68D71260]; Precode of System.Globalization.NumberFormatInfo.<GetInstance>g__GetProviderNonNull|58_0(System.IFormatProvider)
       jmp       short M08_L07
M08_L06:
       call      qword ptr [7FFF68D71248]; Precode of System.Globalization.NumberFormatInfo.get_CurrentInfo()
M08_L07:
       mov       r8,[rax+28]
       test      r8,r8
       jne       short M08_L08
       xor       r9d,r9d
       xor       r8d,r8d
       jmp       short M08_L09
M08_L08:
       lea       r9,[r8+0C]
       mov       r8d,[r8+8]
M08_L09:
       mov       [rsp+30],r9
       mov       [rsp+38],r8d
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       r8,[rsp+50]
       mov       [rsp+20],r8
       lea       r8,[rsp+30]
       lea       r9,[rsp+40]
       mov       ecx,esi
       mov       edx,0FFFFFFFF
       call      qword ptr [7FFF68D80CE0]
       jmp       near ptr M08_L01
M08_L10:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 255
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,38
       xor       eax,eax
       mov       [rsp+28],rax
       mov       rbx,rcx
       lea       rsi,[rbx+18]
       mov       rcx,rsi
       mov       eax,[rbx+10]
       cmp       eax,[rcx+8]
       ja        short M09_L01
       mov       rcx,[rcx]
       mov       [rsp+28],rcx
       mov       [rsp+30],eax
       lea       rcx,[rsp+28]
       call      qword ptr [7FFF68D6BB28]; Precode of System.String.Ctor(System.ReadOnlySpan`1<Char>)
       mov       rdi,rax
       mov       rbp,[rbx+8]
       xor       eax,eax
       mov       [rbx+8],rax
       mov       [rsi],rax
       mov       [rsi+8],rax
       mov       [rbx+10],eax
       test      rbp,rbp
       je        short M09_L00
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       mov       rdx,rbp
       xor       r8d,r8d
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798C0]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Return(Char[], Boolean)
M09_L00:
       mov       rax,rdi
       add       rsp,38
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M09_L01:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 126
```
```assembly
; System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,30
       mov       [rsp+28],rcx
       mov       rbx,rdx
       mov       rsi,r8
       mov       rdi,r9
       call      qword ptr [7FFF68D5EAC8]
       mov       rcx,rax
       mov       r9,rdi
       mov       rdx,rbx
       mov       r8,rsi
       lea       rax,[7FFF68D7FBE0]
       add       rsp,30
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax]
; Total bytes of code 56
```
```assembly
; System.RuntimeTypeHandle.GetRuntimeTypeFromHandle(IntPtr)
       test      cl,2
       jne       short M11_L01
       mov       rax,[rcx+20]
       add       rax,10
       mov       rax,[rax]
M11_L00:
       test      rax,rax
       je        short M11_L02
       ret
M11_L01:
       mov       rax,rcx
       and       rax,0FFFFFFFFFFFFFFFD
       add       rax,8
       mov       rax,[rax]
       jmp       short M11_L00
M11_L02:
       jmp       qword ptr [7FFF09305C80]; System.RuntimeTypeHandle.GetRuntimeTypeFromHandleSlow(IntPtr)
; Total bytes of code 44
```
```assembly
; System.RuntimeTypeHandle.GetRuntimeTypeFromHandleSlow(IntPtr)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,98
       lea       rbp,[rsp+0D0]
       xor       eax,eax
       mov       [rbp-40],rax
       mov       [rbp-0A0],rcx
       lea       rdx,[rbp-40]
       mov       [rbp-0A8],rdx
       lea       rcx,[rbp-98]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-0A0]
       mov       rdx,[rbp-0A8]
       call      qword ptr [rax]
       lea       rcx,[rbp-98]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       rax,[rbp-40]
       add       rsp,98
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
; Total bytes of code 124
```

## .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3 (Job: Job-JESZGN(IterationCount=8, IterationTime=300ms, LaunchCount=2, WarmupCount=3))

```assembly
; Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks.GenericFullTypeChecks()
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,98
       xor       eax,eax
       mov       [rsp+38],rax
       vxorps    xmm4,xmm4,xmm4
       vmovdqu   ymmword ptr [rsp+40],ymm4
       vmovdqu   ymmword ptr [rsp+60],ymm4
       vmovdqa   xmmword ptr [rsp+80],xmm4
       mov       [rsp+90],rax
       mov       [rsp+0E0],rcx
       mov       esi,400
       jmp       near ptr M00_L08
M00_L00:
       mov       edi,eax
M00_L01:
       mov       ecx,[rsp+68]
       imul      edx,edi,0C2B2AE3D
       add       edx,543DE444
       rol       edx,11
       imul      edx,27D4EB2F
       imul      ecx,0C2B2AE3D
       add       ecx,edx
       rol       ecx,11
       imul      ecx,27D4EB2F
       mov       edx,ecx
       shr       edx,0F
       xor       edx,ecx
       imul      ecx,edx,85EBCA77
       mov       edx,ecx
       shr       edx,0D
       xor       edx,ecx
       imul      ecx,edx,0C2B2AE3D
       mov       r15d,ecx
       shr       r15d,10
       xor       r15d,ecx
       xor       ecx,ecx
       mov       [rsp+58],rcx
       mov       rcx,[rbp+8]
       mov       edx,r15d
       imul      rdx,[rbp+30]
       shr       rdx,20
       inc       rdx
       mov       eax,[rcx+8]
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,20
       cmp       edx,eax
       jae       near ptr M00_L51
       mov       edx,edx
       lea       rcx,[rcx+rdx*4+10]
       mov       edi,[rcx]
       mov       rbp,[rbp+10]
       xor       r13d,r13d
       dec       edi
       mov       r12d,[rbp+8]
       cmp       r12d,edi
       jbe       near ptr M00_L17
M00_L02:
       mov       ecx,edi
       shl       rcx,5
       lea       r12,[rbp+rcx+10]
       cmp       [r12+8],r15d
       jne       near ptr M00_L30
       mov       rcx,[r12+10]
       mov       edi,[r12+18]
       test      rcx,rcx
       je        near ptr M00_L30
       mov       r14,offset MT_System.RuntimeType
       cmp       [rcx],r14
       jne       near ptr M00_L29
       mov       rdx,20E834D70B8
       cmp       rcx,rdx
       sete      r14b
       movzx     r14d,r14b
M00_L03:
       test      r14d,r14d
       je        near ptr M00_L30
       test      edi,edi
       jne       near ptr M00_L30
M00_L04:
       test      r12,r12
       je        near ptr M00_L34
       mov       rcx,[r12]
       cmp       qword ptr [rcx+8],0
       jne       near ptr M00_L35
       mov       rdi,[rcx+18]
       mov       [rsp+28],rdi
M00_L05:
       mov       rcx,20E834D70B8
       mov       [rsp+20],rcx
       mov       rcx,offset MT_Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks+Service<System.Int32, System.Int32>
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+20]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L36
M00_L06:
       cmp       ebx,1D
       je        near ptr M00_L37
       mov       rdi,[rsp+28]
       mov       rax,rdi
M00_L07:
       test      rax,rax
       je        near ptr M00_L42
       dec       esi
       je        near ptr M00_L18
M00_L08:
       mov       rdi,20E834D70B8
       mov       [rsp+20],rdi
       mov       rbp,rdi
       mov       r14,offset MT_System.RuntimeType
M00_L09:
       cmp       [rbp],r14
       jne       near ptr M00_L22
       mov       [rsp+30],rbp
       mov       rcx,[rbp+18]
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rbp,[rsp+30]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L21
M00_L10:
       cmp       ebx,1D
       ja        short M00_L11
       mov       ecx,1FEF7FFF
       bt        ecx,ebx
       jae       near ptr M00_L19
M00_L11:
       cmp       ebx,10
       sete      dil
       movzx     edi,dil
M00_L12:
       test      edi,edi
       jne       near ptr M00_L20
       cmp       [rbp],r14
       jne       near ptr M00_L23
M00_L13:
       test      rbp,rbp
       je        near ptr M00_L38
       mov       rcx,rbp
       call      00007FFF68F39140
       test      eax,eax
       jne       near ptr M00_L39
       mov       rcx,offset MT_Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks+Service<System.Int32, System.Int32>
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L24
M00_L14:
       cmp       ebx,10
       je        near ptr M00_L40
       mov       rcx,offset MT_Xoderony.Benchmarks.DependencyInjection.ValidationBenchmarks+Service<System.Int32, System.Int32>
       mov       rax,7FFF68F33DF0
       vzeroupper
       call      rax
       movzx     ebx,al
       mov       rdi,[rsp+20]
       cmp       dword ptr [7FFF69244A90],0
       jne       near ptr M00_L25
M00_L15:
       cmp       ebx,0F
       je        near ptr M00_L40
       mov       rbx,[rsp+0E0]
       mov       rcx,[rbx+8]
       cmp       [rcx],cl
       vxorps    ymm0,ymm0,ymm0
       vmovdqu   ymmword ptr [rsp+70],ymm0
       vmovdqu   xmmword ptr [rsp+88],xmm0
       mov       rbp,[rcx+8]
       cmp       qword ptr [rbp+8],0
       je        short M00_L17
       mov       r15,[rbp+18]
       test      r15,r15
       jne       near ptr M00_L31
       mov       [rsp+60],rdi
       xor       ecx,ecx
       mov       [rsp+68],ecx
       mov       [rsp+58],rcx
       lea       rcx,[rsp+60]
       cmp       qword ptr [rsp+58],0
       jne       short M00_L16
       mov       rcx,[rsp+60]
       mov       [rsp+58],rcx
       lea       rcx,[rsp+58]
       cmp       qword ptr [rsp+58],0
       je        near ptr M00_L26
M00_L16:
       mov       rdi,[rcx]
       cmp       [rdi],r14
       jne       near ptr M00_L27
       mov       rcx,rdi
       call      00007FFF68F03E20
       test      eax,eax
       jne       near ptr M00_L00
       mov       rcx,rdi
       call      qword ptr [7FFF094B5FF8]; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       jmp       near ptr M00_L00
M00_L17:
       xor       r12d,r12d
       jmp       near ptr M00_L04
M00_L18:
       vzeroupper
       add       rsp,98
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       ret
M00_L19:
       mov       edi,1
       jmp       near ptr M00_L12
M00_L20:
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+68]
       call      qword ptr [rax+8]
       mov       rbp,rax
       jmp       near ptr M00_L09
M00_L21:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L10
M00_L22:
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+68]
       call      qword ptr [rax]
       mov       edi,eax
       jmp       near ptr M00_L12
M00_L23:
       mov       rcx,rbp
       mov       rax,[rbp]
       mov       rax,[rax+98]
       call      qword ptr [rax+8]
       mov       rbp,rax
       jmp       near ptr M00_L13
M00_L24:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L14
M00_L25:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L15
M00_L26:
       xor       edi,edi
       jmp       near ptr M00_L01
M00_L27:
       mov       rcx,rdi
       mov       rax,[rdi]
       mov       rax,[rax+40]
       call      qword ptr [rax+18]
       mov       edi,eax
       jmp       near ptr M00_L01
M00_L28:
       cmp       r12d,edi
       jbe       near ptr M00_L17
       jmp       near ptr M00_L02
M00_L29:
       mov       rdx,20E834D70B8
       mov       rax,[rcx]
       mov       rax,[rax+40]
       call      qword ptr [rax+10]
       mov       r14d,eax
       jmp       near ptr M00_L03
M00_L30:
       mov       edi,[r12+0C]
       inc       r13d
       mov       r12d,[rbp+8]
       cmp       r12d,r13d
       jae       short M00_L28
       jmp       near ptr M00_L41
M00_L31:
       mov       [rsp+48],rdi
       xor       edx,edx
       mov       [rsp+50],edx
       lea       rdx,[rsp+48]
       mov       rcx,r15
       mov       r11,7FFF09230588
       call      qword ptr [r11]
       mov       edi,eax
       mov       r8,[rbp+8]
       mov       edx,edi
       imul      rdx,[rbp+30]
       shr       rdx,20
       inc       rdx
       mov       ecx,[r8+8]
       imul      rdx,rcx
       shr       rdx,20
       cmp       edx,[r8+8]
       jae       near ptr M00_L51
       mov       edx,edx
       lea       r8,[r8+rdx*4+10]
       mov       r14d,[r8]
       mov       rbp,[rbp+10]
       xor       r13d,r13d
       dec       r14d
M00_L32:
       cmp       [rbp+8],r14d
       jbe       near ptr M00_L17
       mov       r8d,r14d
       shl       r8,5
       lea       r12,[rbp+r8+10]
       cmp       [r12+8],edi
       jne       short M00_L33
       vmovdqu   xmm0,xmmword ptr [r12+10]
       vmovdqu   xmmword ptr [rsp+48],xmm0
       mov       r8,20E834D70B8
       mov       [rsp+38],r8
       xor       r8d,r8d
       mov       [rsp+40],r8d
       lea       r8,[rsp+38]
       lea       rdx,[rsp+48]
       mov       rcx,r15
       mov       r11,7FFF09230590
       call      qword ptr [r11]
       test      eax,eax
       jne       near ptr M00_L04
M00_L33:
       mov       r14d,[r12+0C]
       inc       r13d
       cmp       [rbp+8],r13d
       jae       short M00_L32
       jmp       near ptr M00_L41
M00_L34:
       xor       eax,eax
       jmp       near ptr M00_L07
M00_L35:
       call      qword ptr [7FFF095B5968]; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       mov       [rsp+28],rax
       jmp       near ptr M00_L05
M00_L36:
       call      CORINFO_HELP_POLL_GC
       jmp       near ptr M00_L06
M00_L37:
       mov       rdi,[rsp+28]
       mov       rdx,rdi
       mov       rcx,offset MT_System.Collections.ICollection
       call      qword ptr [7FFF094BECA0]; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       mov       rbx,rax
       mov       rcx,rbp
       call      qword ptr [7FFF094BE2E0]; System.RuntimeTypeHandle.GetElementType(System.RuntimeType)
       mov       rdi,rax
       mov       rcx,rbx
       mov       r11,7FFF09230578
       call      qword ptr [r11]
       mov       edx,eax
       mov       rcx,rdi
       call      qword ptr [7FFF095B5BF0]; System.Array.CreateInstance(System.Type, Int32)
       mov       rdi,rax
       mov       rcx,rbx
       mov       rdx,rdi
       mov       r11,7FFF09230580
       xor       r8d,r8d
       call      qword ptr [r11]
       mov       rax,rdi
       jmp       near ptr M00_L07
M00_L38:
       mov       rcx,offset MT_System.ArgumentNullException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       call      qword ptr [7FFF096E61F0]
       mov       r8,rax
       mov       rcx,rbx
       xor       edx,edx
       call      qword ptr [7FFF096E6208]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L39:
       mov       rcx,offset MT_System.ArgumentException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,5AA
       mov       rdx,7FFF09528420
       call      qword ptr [7FFF092EF210]
       mov       rsi,rax
       mov       ecx,592
       mov       rdx,7FFF09528420
       call      qword ptr [7FFF092EF210]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF095B7C90]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L40:
       mov       rcx,offset MT_System.ArgumentException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,600
       mov       rdx,7FFF09528420
       call      qword ptr [7FFF092EF210]
       mov       rsi,rax
       mov       ecx,592
       mov       rdx,7FFF09528420
       call      qword ptr [7FFF092EF210]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF095B7C90]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L41:
       call      qword ptr [7FFF092EF480]
       int       3
M00_L42:
       lea       rcx,[rsp+70]
       mov       edx,1D
       mov       r8d,2
       call      qword ptr [7FFF092EC270]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       ja        near ptr M00_L47
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,14
       jb        short M00_L43
       vmovups   ymm0,[7FFF093698A0]
       vmovups   [rdx],ymm0
       mov       rcx,200072006F0066
       mov       [rdx+20],rcx
       mov       ecx,[rsp+80]
       add       ecx,14
       mov       [rsp+80],ecx
       jmp       short M00_L44
M00_L43:
       lea       rcx,[rsp+70]
       mov       rdx,20E834DB920
       call      qword ptr [7FFF0964E340]
M00_L44:
       lea       rcx,[rsp+70]
       mov       rdx,7FFF09672DA0
       mov       rbp,20E834D70B8
       mov       r8,rbp
       call      qword ptr [7FFF094BEA60]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       ja        short M00_L47
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,7
       jb        short M00_L45
       mov       rcx,65006B00280020
       mov       [rdx],rcx
       mov       dword ptr [rdx+8],3A0079
       mov       word ptr [rdx+0C],20
       mov       ecx,[rsp+80]
       add       ecx,7
       mov       [rsp+80],ecx
       jmp       short M00_L46
M00_L45:
       lea       rcx,[rsp+70]
       mov       rdx,20E834DB960
       call      qword ptr [7FFF0964E340]
M00_L46:
       lea       rcx,[rsp+70]
       xor       edx,edx
       call      qword ptr [7FFF092E7FD8]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       mov       ecx,[rsp+80]
       cmp       ecx,[rsp+90]
       jbe       short M00_L48
M00_L47:
       call      qword ptr [7FFF094B7D50]
       int       3
M00_L48:
       mov       rdx,[rsp+88]
       mov       eax,ecx
       lea       rdx,[rdx+rax*2]
       mov       eax,[rsp+90]
       sub       eax,ecx
       cmp       eax,2
       jb        short M00_L49
       mov       dword ptr [rdx],2E0029
       mov       ecx,[rsp+80]
       add       ecx,2
       mov       [rsp+80],ecx
       jmp       short M00_L50
M00_L49:
       lea       rcx,[rsp+70]
       mov       rdx,20E834D9BF8
       call      qword ptr [7FFF0964E340]
M00_L50:
       mov       rcx,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       lea       rcx,[rsp+70]
       call      qword ptr [7FFF092EC2A0]; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFF095B7BD0]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L51:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 2125
```
```assembly
; System.Runtime.CompilerServices.RuntimeHelpers.<GetHashCode>g__GetHashCodeWorker|15_0(System.Object)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,88
       lea       rbp,[rsp+0C0]
       mov       [rbp+10],rcx
       cmp       qword ptr [rbp+10],0
       je        short M01_L01
       lea       rcx,[rbp+10]
       mov       [rbp-98],rcx
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-98]
       call      qword ptr [rax]
       mov       ebx,eax
       lea       rcx,[rbp-90]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       eax,ebx
M01_L00:
       add       rsp,88
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
M01_L01:
       xor       eax,eax
       jmp       short M01_L00
; Total bytes of code 119
```
```assembly
; System.Lazy`1[[System.__Canon, System.Private.CoreLib]].CreateValue()
       push      rsi
       push      rbx
       sub       rsp,28
       mov       [rsp+20],rcx
       mov       rbx,rcx
M02_L00:
       mov       rsi,[rbx+8]
       test      rsi,rsi
       je        short M02_L01
       mov       ecx,[rsi+10]
       cmp       ecx,8
       jne       short M02_L03
       mov       rcx,rbx
       mov       rdx,rsi
       xor       r8d,r8d
       call      qword ptr [7FFF68D7E1B0]
M02_L01:
       cmp       qword ptr [rbx+8],0
       jne       short M02_L02
       mov       rax,[rbx+18]
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M02_L02:
       mov       rcx,rbx
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax]
M02_L03:
       cmp       ecx,8
       ja        short M02_L04
       mov       ecx,ecx
       lea       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       edx,[rdx+rcx*4]
       lea       r8,[M02_L00]
       add       rdx,r8
       jmp       rdx
M02_L04:
       mov       rcx,[rsi+8]
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D74C78]
       int       3
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1A0]
       jmp       short M02_L01
       mov       rcx,rbx
       xor       edx,edx
       call      qword ptr [7FFF68D7E1A8]; Precode of System.Lazy`1[[System.__Canon, System.Private.CoreLib]].ViaFactory(System.Threading.LazyThreadSafetyMode)
       jmp       short M02_L01
       mov       rcx,[rbx]
       call      qword ptr [7FFF68D5B838]
       mov       rcx,rax
       call      qword ptr [7FFF68D62EA0]
       mov       rcx,rax
       call      qword ptr [7FFF68D84008]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
       mov       rax,[rbx+10]
       test      rax,rax
       je        short M02_L05
       mov       rcx,[rax+8]
       call      qword ptr [rax+18]
       mov       r8,rax
       mov       rdx,rsi
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1B8]
       jmp       near ptr M02_L01
M02_L05:
       mov       rcx,rbx
       call      qword ptr [7FFF68D7E1C8]
       jmp       near ptr M02_L01
       mov       rcx,rbx
       mov       rdx,rsi
       mov       r8d,1
       call      qword ptr [7FFF68D7E1B0]
       jmp       near ptr M02_L01
; Total bytes of code 269
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastInterface(Void*, System.Object)
       test      rdx,rdx
       je        short M03_L02
       mov       rax,[rdx]
       movzx     r8d,word ptr [rax+0E]
       test      r8,r8
       je        short M03_L05
       mov       rax,[rax+38]
       cmp       r8,4
       jl        short M03_L01
       cmp       [rax],rcx
       je        short M03_L02
M03_L00:
       cmp       [rax+8],rcx
       je        short M03_L02
       cmp       [rax+10],rcx
       je        short M03_L02
       cmp       [rax+18],rcx
       je        short M03_L02
       add       rax,20
       add       r8,0FFFFFFFFFFFFFFFC
       cmp       r8,4
       jge       short M03_L04
       test      r8,r8
       je        short M03_L05
M03_L01:
       cmp       [rax],rcx
       jne       short M03_L03
M03_L02:
       mov       rax,rdx
       ret
M03_L03:
       add       rax,8
       dec       r8
       test      r8,r8
       jle       short M03_L05
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L03
M03_L04:
       cmp       [rax],rcx
       je        short M03_L02
       jmp       short M03_L00
M03_L05:
       lea       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       jmp       qword ptr [rax]
; Total bytes of code 115
```
```assembly
; System.RuntimeTypeHandle.GetElementType(System.RuntimeType)
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rbx,rcx
       mov       rcx,[rbx+18]
       call      00007FFF68F3E8E0
       mov       rsi,rax
       test      rsi,rsi
       jne       short M04_L00
       mov       rcx,7FFF0975BDA8
       call      CORINFO_HELP_COUNTPROFILE32
       xor       eax,eax
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M04_L00:
       mov       rcx,7FFF0975BDAC
       call      CORINFO_HELP_COUNTPROFILE32
       mov       rcx,rsi
       call      qword ptr [7FFF092E5860]; System.RuntimeTypeHandle.GetRuntimeTypeFromHandle(IntPtr)
       nop
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
; Total bytes of code 82
```
```assembly
; System.Array.CreateInstance(System.Type, Int32)
       sub       rsp,28
       mov       [rsp+38],edx
       test      rcx,rcx
       je        short M05_L01
       mov       eax,[rsp+38]
       test      eax,eax
       jl        short M05_L02
       mov       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       cmp       r11,[rcx]
       jne       short M05_L03
M05_L00:
       call      qword ptr [7FFF68D67080]
       test      rax,rax
       je        short M05_L04
       lea       r8,[rsp+38]
       mov       rcx,rax
       mov       edx,1
       xor       r9d,r9d
       call      qword ptr [7FFF68D6A298]; Precode of System.Array.InternalCreate(System.RuntimeType, Int32, Int32*, Int32*)
       nop
       add       rsp,28
       ret
M05_L01:
       mov       rcx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rcx]
       call      qword ptr [7FFF68D6C210]
       int       3
M05_L02:
       mov       rdx,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rdx,[rdx]
       mov       ecx,eax
       call      qword ptr [7FFF68D81908]
       int       3
M05_L03:
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rcx,rax
       jmp       short M05_L00
M05_L04:
       mov       ecx,39
       mov       edx,4E
       call      qword ptr [7FFF68D6F3B0]
       int       3
; Total bytes of code 140
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor(Int32, Int32)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rbx,rcx
       mov       esi,edx
       mov       edi,r8d
       xor       eax,eax
       mov       [rbx],rax
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       imul      edx,edi,0B
       add       edx,esi
       mov       eax,100
       cmp       edx,100
       cmovle    edx,eax
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798B8]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Rent(Int32)
       mov       [rbx+8],rax
       test      rax,rax
       je        short M06_L01
       lea       rcx,[rax+10]
       mov       eax,[rax+8]
M06_L00:
       mov       [rbx+18],rcx
       mov       [rbx+20],eax
       xor       eax,eax
       mov       [rbx+10],eax
       mov       byte ptr [rbx+14],0
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
M06_L01:
       xor       ecx,ecx
       xor       eax,eax
       jmp       short M06_L00
; Total bytes of code 102
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.__Canon, System.Private.CoreLib]](System.__Canon)
       push      rsi
       push      rbx
       sub       rsp,58
       xor       eax,eax
       mov       [rsp+28],rax
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       mov       [rsp+40],rax
       mov       [rsp+50],rdx
       mov       rbx,rcx
       mov       rcx,rdx
       mov       rsi,r8
       cmp       byte ptr [rbx+14],0
       jne       near ptr M07_L05
       test      rsi,rsi
       je        near ptr M07_L06
       mov       rcx,rsi
       call      qword ptr [7FFF68D67170]
       test      rax,rax
       jne       short M07_L01
       mov       rcx,rsi
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       mov       rdx,rax
M07_L00:
       test      rdx,rdx
       je        near ptr M07_L06
       lea       r8,[rbx+18]
       mov       ecx,[rbx+10]
       mov       eax,[r8+8]
       cmp       ecx,eax
       ja        near ptr M07_L07
       mov       r8,[r8]
       mov       r10d,ecx
       lea       r10,[r8+r10*2]
       sub       eax,ecx
       mov       esi,[rdx+8]
       cmp       esi,eax
       ja        near ptr M07_L08
       mov       r8d,esi
       add       r8,r8
       add       rdx,0C
       mov       rcx,r10
       call      qword ptr [7FFF68D6D928]; Precode of System.SpanHelpers.Memmove(Byte ByRef, Byte ByRef, UIntPtr)
       add       [rbx+10],esi
       jmp       near ptr M07_L06
M07_L01:
       mov       rcx,rsi
       call      qword ptr [7FFF68D671A8]
       test      rax,rax
       je        near ptr M07_L04
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E8]
       mov       rsi,rax
M07_L02:
       mov       rcx,rsi
       lea       rdx,[rbx+18]
       mov       r9d,[rbx+10]
       mov       r8d,[rdx+8]
       cmp       r9d,r8d
       ja        near ptr M07_L07
       mov       rdx,[rdx]
       mov       r11d,r9d
       lea       rdx,[rdx+r11*2]
       sub       r8d,r9d
       mov       [rsp+38],rdx
       mov       [rsp+40],r8d
       xor       edx,edx
       mov       [rsp+28],rdx
       mov       [rsp+30],edx
       mov       rdx,[rbx]
       mov       [rsp+20],rdx
       lea       rdx,[rsp+38]
       lea       r9,[rsp+28]
       lea       r8,[rsp+48]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       call      qword ptr [r11]
       test      eax,eax
       jne       short M07_L03
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M07_L02
M07_L03:
       mov       ecx,[rsp+48]
       add       [rbx+10],ecx
       jmp       short M07_L06
M07_L04:
       mov       rcx,rsi
       call      qword ptr [7FFF68D683E0]
       mov       rcx,rax
       mov       r8,[rbx]
       lea       r11,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       xor       edx,edx
       call      qword ptr [r11]
       mov       rdx,rax
       jmp       near ptr M07_L00
M07_L05:
       call      qword ptr [7FFF68D600E8]
       mov       rdx,rax
       mov       rcx,rbx
       mov       r8,rsi
       xor       r9d,r9d
       call      qword ptr [7FFF68D84678]
M07_L06:
       nop
       add       rsp,58
       pop       rbx
       pop       rsi
       ret
M07_L07:
       call      qword ptr [7FFF68D6F2B8]
       int       3
M07_L08:
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FD0]
       jmp       short M07_L06
; Total bytes of code 397
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted[[System.Int32, System.Private.CoreLib]](Int32)
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,58
       xorps     xmm4,xmm4
       movaps    [rsp+30],xmm4
       movaps    [rsp+40],xmm4
       mov       rbx,rcx
       mov       esi,edx
       cmp       byte ptr [rbx+14],0
       jne       short M08_L03
M08_L00:
       lea       rdx,[rbx+18]
       mov       r8d,[rbx+10]
       mov       edi,[rdx+8]
       cmp       r8d,edi
       ja        near ptr M08_L10
       mov       rdx,[rdx]
       mov       ecx,r8d
       lea       rbp,[rdx+rcx*2]
       sub       edi,r8d
       mov       rcx,[rbx]
       test      esi,esi
       jl        short M08_L05
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       rdx,[rsp+40]
       lea       r8,[rsp+50]
       mov       ecx,esi
       call      qword ptr [7FFF68D80CF8]; Precode of System.Number.TryUInt32ToDecStr[[System.Char, System.Private.CoreLib]](UInt32, System.Span`1<Char>, Int32 ByRef)
M08_L01:
       test      eax,eax
       jne       short M08_L02
       mov       rcx,rbx
       call      qword ptr [7FFF68D74FF8]
       jmp       short M08_L00
M08_L02:
       mov       eax,[rsp+50]
       add       [rbx+10],eax
       jmp       short M08_L04
M08_L03:
       mov       rcx,rbx
       mov       edx,esi
       xor       r8d,r8d
       call      qword ptr [7FFF68D81B50]
M08_L04:
       nop
       add       rsp,58
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M08_L05:
       test      rcx,rcx
       je        short M08_L06
       call      qword ptr [7FFF68D71260]; Precode of System.Globalization.NumberFormatInfo.<GetInstance>g__GetProviderNonNull|58_0(System.IFormatProvider)
       jmp       short M08_L07
M08_L06:
       call      qword ptr [7FFF68D71248]; Precode of System.Globalization.NumberFormatInfo.get_CurrentInfo()
M08_L07:
       mov       r8,[rax+28]
       test      r8,r8
       jne       short M08_L08
       xor       r9d,r9d
       xor       r8d,r8d
       jmp       short M08_L09
M08_L08:
       lea       r9,[r8+0C]
       mov       r8d,[r8+8]
M08_L09:
       mov       [rsp+30],r9
       mov       [rsp+38],r8d
       mov       [rsp+40],rbp
       mov       [rsp+48],edi
       lea       r8,[rsp+50]
       mov       [rsp+20],r8
       lea       r8,[rsp+30]
       lea       r9,[rsp+40]
       mov       ecx,esi
       mov       edx,0FFFFFFFF
       call      qword ptr [7FFF68D80CE0]
       jmp       near ptr M08_L01
M08_L10:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 255
```
```assembly
; System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear()
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,38
       xor       eax,eax
       mov       [rsp+28],rax
       mov       rbx,rcx
       lea       rsi,[rbx+18]
       mov       rcx,rsi
       mov       eax,[rbx+10]
       cmp       eax,[rcx+8]
       ja        short M09_L01
       mov       rcx,[rcx]
       mov       [rsp+28],rcx
       mov       [rsp+30],eax
       lea       rcx,[rsp+28]
       call      qword ptr [7FFF68D6BB28]; Precode of System.String.Ctor(System.ReadOnlySpan`1<Char>)
       mov       rdi,rax
       mov       rbp,[rbx+8]
       xor       eax,eax
       mov       [rbx+8],rax
       mov       [rsi],rax
       mov       [rsi+8],rax
       mov       [rbx+10],eax
       test      rbp,rbp
       je        short M09_L00
       call      qword ptr [7FFF68D5A098]
       mov       rcx,[rax]
       mov       rdx,rbp
       xor       r8d,r8d
       cmp       [rcx],ecx
       call      qword ptr [7FFF68D798C0]; Precode of System.Buffers.SharedArrayPool`1[[System.Char, System.Private.CoreLib]].Return(Char[], Boolean)
M09_L00:
       mov       rax,rdi
       add       rsp,38
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M09_L01:
       call      qword ptr [7FFF68D6F2B8]
       int       3
; Total bytes of code 126
```
```assembly
; System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,30
       mov       [rsp+28],rcx
       mov       rbx,rdx
       mov       rsi,r8
       mov       rdi,r9
       call      qword ptr [7FFF68D5EAC8]
       mov       rcx,rax
       mov       r9,rdi
       mov       rdx,rbx
       mov       r8,rsi
       lea       rax,[7FFF68D7FBE0]
       add       rsp,30
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax]
; Total bytes of code 56
```
```assembly
; System.RuntimeTypeHandle.GetRuntimeTypeFromHandle(IntPtr)
       test      cl,2
       jne       short M11_L01
       mov       rax,[rcx+20]
       add       rax,10
       mov       rax,[rax]
M11_L00:
       test      rax,rax
       je        short M11_L02
       ret
M11_L01:
       mov       rax,rcx
       and       rax,0FFFFFFFFFFFFFFFD
       add       rax,8
       mov       rax,[rax]
       jmp       short M11_L00
M11_L02:
       jmp       qword ptr [7FFF092E5C80]; System.RuntimeTypeHandle.GetRuntimeTypeFromHandleSlow(IntPtr)
; Total bytes of code 44
```
```assembly
; System.RuntimeTypeHandle.GetRuntimeTypeFromHandleSlow(IntPtr)
       push      rbp
       push      r15
       push      r14
       push      r13
       push      r12
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,98
       lea       rbp,[rsp+0D0]
       xor       eax,eax
       mov       [rbp-40],rax
       mov       [rbp-0A0],rcx
       lea       rdx,[rbp-40]
       mov       [rbp-0A8],rdx
       lea       rcx,[rbp-98]
       call      qword ptr [7FFF68D59030]; CORINFO_HELP_JIT_PINVOKE_BEGIN
       mov       rax,[System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.Dictionary`2<System.__Canon,System.__Canon>, System.__Canon, Boolean ByRef)]
       mov       rcx,[rbp-0A0]
       mov       rdx,[rbp-0A8]
       call      qword ptr [rax]
       lea       rcx,[rbp-98]
       call      qword ptr [7FFF68D59038]; CORINFO_HELP_JIT_PINVOKE_END
       mov       rax,[rbp-40]
       add       rsp,98
       pop       rbx
       pop       rsi
       pop       rdi
       pop       r12
       pop       r13
       pop       r14
       pop       r15
       pop       rbp
       ret
; Total bytes of code 124
```

