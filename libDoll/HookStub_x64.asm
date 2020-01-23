; This file is translated from HookStub_x86.asm
; All the comments are identital to the x86 counterpart.
extern DollThreadIsCurrent:byte, DollHookGetCurrent:byte, \
    DollOnHook:byte, DollOnAfterHook:byte

public HookStubBefore, HookStubA, HookStubB, HookStubOnDeny, \
    HookStubBefore_len, HookStubBefore_HookOEPOffset, HookStubBefore_AddrOffset

.code

; pushad/popad are not supported on x64 :(
; sequence: rax, rcx, rdx, rbx, rbp, rsp, rdi, rsi, r8, r9

pushad macro
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

popad macro
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
    xor rax, [rsp]
    xor [rsp], rax
    xor rax, [rsp]
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
    xor rax, [rsp]
    xor [rsp], rax
    xor rax, [rsp]
    ;   swap(rax, [rsp])
    ;   stack == (x), (red zone...)
endm

; NOTE: The `sub/add rsp, 8 * 4`s around calling a function are MANDATORY
;    That's the "shadow space" left for the caller function
;    Under Debug it's safe to remove them, but under Release this space may get utilized
;    See https://stackoverflow.com/a/30194393 for details

HookStubBefore:
    pushimm64 0CCCCCCCCCCCCCCCCh ; HookOEP placeholder
    pushimm64 0CCCCCCCCCCCCCCCCh ; Address pointer placeholder
    ret
HookStubBefore_end:

HookStubA:
    pushad

    sub rsp, 8 * 4
    lea rax, DollThreadIsCurrent
    call rax
    add rsp, 8 * 4

    mov rcx, rsp
    add rcx, 8 * 10

    test rax, rax
    jnz __HookStubA_isDoll

    sub rsp, 8 * 4
    lea rax, DollOnHook
    call rax
    add rsp, 8 * 4

    popad

    ret

__HookStubA_isDoll:

    sub rsp, 8 * 4
    lea rax, DollHookGetCurrent
    call rax
    add rsp, 8 * 4

    mov rdx, [rax + 4 * 0] ; offset LIBDOLL_HOOK::pTrampoline

    mov rcx, rsp
    add rcx, 8 * 10
    mov [rcx], rdx

    popad

    ret

HookStubB:
    pushad

    mov rcx, rsp
    add rcx, 8 * 10

    sub rsp, 8 * 4
    lea rax, DollOnAfterHook
    call rax
    add rsp, 8 * 4

    popad

    ret

HookStubOnDeny:
    pushad

    mov rcx, rsp
    add rcx, 8 * 10

    sub rsp, 8 * 4
    lea rax, DollHookGetCurrent
    call rax
    add rsp, 8 * 4

    mov rdx, [rax + 8 * 1] ; offset LIBDOLL_HOOK::denySPOffset

    add [rsp + 8 * 5], edx ; offset pushad::rsp

    mov rcx, rsp
    add rcx, 8 * (10 + 1) ; &(return addr)

    mov rsi, [rcx]

    mov [rcx + rdx], rsi

    mov rdx, [rax + 8 * 2] ; offset LIBDOLL_HOOK::denyAX

    mov [rsp + 8 * 9], rdx ; offset pushad::eax

    popad

    add rsp, 8

    ret

.const

HookStubBefore_len \
    dq HookStubBefore_end - HookStubBefore

; Offset to HookOEP placeholder
HookStubBefore_HookOEPOffset \
    dq 3

; Offset to address pointer placeholder
HookStubBefore_AddrOffset \
    dq 26

end