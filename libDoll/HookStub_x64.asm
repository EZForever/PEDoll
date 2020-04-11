; This file is translated from HookStub_x86.asm
; All the comments are identital to the x86 counterpart.
extern DollThreadIsCurrent:byte, DollHookGetCurrent:byte
extern DollOnHook:byte, DollOnAfterHook:byte, DollOnEPHook:byte

public HookStubBefore, HookStubA, HookStubB, HookStubOnDeny, HookStubEP
public HookStubBefore_len, HookStubBefore_HookOEPOffset, HookStubBefore_AddrOffset
public pushad_count

.code

; Machine's word size, in bytes
; Could have used @WordSize (https://docs.microsoft.com/en-us/cpp/assembler/masm/at-wordsize) but it is not supported in x64
WORDSZ equ 8

; Registers saved by the pushall macro, as a macro for usage in assembly code
; Currently: rax, rcx, rdx, rbx, rbp, rsp, rdi, rsi, r8, r9, rflags, (garbage)
PUSHAD_CNT equ 12

; Size of register shadow space
; NOTE: The `sub/add rsp, 8 * 4`s around calling a function are MANDATORY
;    That's the "shadow space" left for the caller function
;    Under Debug it's safe to remove them, but under Release this space may get utilized
;    See https://stackoverflow.com/a/30194393 for details
SHADOWSZ equ WORDSZ * 4

; pushad/popad are not supported on x64 :(

; NOTE: this macro corrupts rax
pushall macro
    push rax
    mov rax, rsp
    lea rax, [rax + WORDSZ] ; lea instead of add to avoid flag corruption
    ;   rax == original rsp
    push rcx
    push rdx
    push rbx
    push rax ; original rsp
    push rbp
    push rsi
    push rdi
    push r8
    push r9
    pushfq
    push rax
    ;   Stack alignment is also MANDATORY when it comes to optimized STL functions
    ;   pushall must maintain an even number of push operations, so here's a garbage value
endm

popall macro
    pop rax ; the garbage value
    popfq
    pop r9
    pop r8
    pop rdi
    pop rsi
    pop rbp
    pop rax ; original rsp
    pop rbx
    pop rdx
    pop rcx
    ;   rax == original rsp
    ;   stack == (original rax), (red zone...)
    xchg rax, [rsp]
    ;   swap(rax, [rsp])
    ;   stack == (original rsp), (red zone...)
    pop rsp
endm

; push for 64-bit immediates is not supported on x64 too :(

pushimm64 macro x
    push rax
    mov rax, x
    ;   rax == x
    ;   stack == (original rax), (red zone...)
    xchg rax, [rsp]
    ;   swap(rax, [rsp])
    ;   stack == (x), (red zone...)
endm

HookStubBefore:
    pushimm64 0CCCCCCCCCCCCCCCCh ; HookOEP placeholder
    pushimm64 0CCCCCCCCCCCCCCCCh ; Address pointer placeholder
    ret
HookStubBefore_end:

HookStubA:
    pushall

    sub rsp, SHADOWSZ
    lea rax, DollThreadIsCurrent
    call rax
    add rsp, SHADOWSZ

    lea rcx, [rsp + WORDSZ * PUSHAD_CNT]

    test rax, rax
    jnz __HookStubA_isDoll

    sub rsp, SHADOWSZ
    lea rax, DollOnHook
    call rax
    add rsp, SHADOWSZ

    popall

    ret

__HookStubA_isDoll:

    sub rsp, SHADOWSZ
    lea rax, DollHookGetCurrent
    call rax
    add rsp, SHADOWSZ

    mov rdx, [rax + WORDSZ * 0] ; offset LIBDOLL_HOOK::pTrampoline

    mov [rsp + WORDSZ * PUSHAD_CNT], rdx

    popall

    ret

HookStubB:
    pushall

    mov rcx, rsp
    add rcx, WORDSZ * PUSHAD_CNT

    sub rsp, SHADOWSZ
    lea rax, DollOnAfterHook
    call rax
    add rsp, SHADOWSZ

    popall

    ret

HookStubOnDeny:
    pushall

    lea rcx, [rsp + WORDSZ * PUSHAD_CNT]

    sub rsp, SHADOWSZ
    lea rax, DollHookGetCurrent
    call rax
    add rsp, SHADOWSZ

    mov rdx, [rax + WORDSZ * 1] ; offset LIBDOLL_HOOK::denySPOffset

    add [rsp + WORDSZ * 7], rdx ; offset pushad::rsp

    lea rcx, [rsp + WORDSZ * (PUSHAD_CNT + 1)] ; &(return addr)

    mov rsi, [rcx]

    mov [rcx + rdx], rsi

    mov rdx, [rax + WORDSZ * 2] ; offset LIBDOLL_HOOK::denyAX

    mov [rsp + WORDSZ * 11], rdx ; offset pushad::eax

    popall

    add rsp, WORDSZ

    ret

HookStubEP:
    push rax

    pushall

    lea rcx, [rsp + WORDSZ * PUSHAD_CNT]

    sub rsp, SHADOWSZ
    lea rax, DollOnEPHook
    call rax
    add rsp, SHADOWSZ

    popall

    ret

.const

HookStubBefore_len \
    dq HookStubBefore_end - HookStubBefore

; Offset to HookOEP placeholder
HookStubBefore_HookOEPOffset \
    dq 3

; Offset to address pointer placeholder
HookStubBefore_AddrOffset \
    dq 18

; Registers saved by the pushad/pushall instruction
pushad_count \
    dq PUSHAD_CNT

end