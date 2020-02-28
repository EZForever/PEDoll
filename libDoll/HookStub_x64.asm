; This file is translated from HookStub_x86.asm
; All the comments are identital to the x86 counterpart.
extern DollThreadIsCurrent:byte, DollHookGetCurrent:byte, \
    DollOnHook:byte, DollOnAfterHook:byte

public HookStubBefore, HookStubA, HookStubB, HookStubOnDeny, \
    HookStubBefore_len, HookStubBefore_HookOEPOffset, HookStubBefore_AddrOffset, \
    pushad_count

.code

; Machine's work size, in bytes
; Could have used @WordSize (https://docs.microsoft.com/en-us/cpp/assembler/masm/at-wordsize) but it is not supported in x64
WORDSZ equ 8

; Registers saved by the pushad/pushall instruction, as a macro for usage in assembly code
PUSHAD_CNT equ 10

; Size of register shadow space
; NOTE: The `sub/add rsp, 8 * 4`s around calling a function are MANDATORY
;    That's the "shadow space" left for the caller function
;    Under Debug it's safe to remove them, but under Release this space may get utilized
;    See https://stackoverflow.com/a/30194393 for details
SHADOWSZ equ WORDSZ * 4

; pushad/popad are not supported on x64 :(
; sequence: rax, rcx, rdx, rbx, rbp, rsp, rdi, rsi, r8, r9

pushall macro
    push rax
    mov rax, rsp
    add rax, 8
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
endm

popall macro
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

    mov rcx, rsp
    add rcx, WORDSZ * PUSHAD_CNT

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

    mov rcx, rsp
    add rcx, WORDSZ * PUSHAD_CNT
    mov [rcx], rdx

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

    mov rcx, rsp
    add rcx, WORDSZ * PUSHAD_CNT

    sub rsp, SHADOWSZ
    lea rax, DollHookGetCurrent
    call rax
    add rsp, SHADOWSZ

    mov rdx, [rax + WORDSZ * 1] ; offset LIBDOLL_HOOK::denySPOffset

    add [rsp + WORDSZ * 5], edx ; offset pushad::rsp

    mov rcx, rsp
    add rcx, WORDSZ * (PUSHAD_CNT + 1) ; &(return addr)

    mov rsi, [rcx]

    mov [rcx + rdx], rsi

    mov rdx, [rax + WORDSZ * 2] ; offset LIBDOLL_HOOK::denyAX

    mov [rsp + WORDSZ * 9], rdx ; offset pushad::eax

    popall

    add rsp, WORDSZ

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