; TODO: Rewrite this file according to HookStub_x86.asm

; All the comments are identital to the x86 counterpart.
extern DollOnHook:byte, DollOnAfterHook:byte

public HookStubPhase1, HookStubPhase3, HookStubOnDeny, \
    HookStubPhase1_len, \
    hookOriginalSP, hookOriginalIP, \
    hookDenySPOffset, hookDenyReturn

.code

; pushad/popad not supported on x64 :(

pushad macro
    push rax
    mov rax, rsp
    add rax, 8 ; rax == original rsp
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
    xor rax, rsp
    xor rsp, rax
    xor rax, rsp
    ;   swap(rax, rsp)
    ;   stack == (original rsp), (red zone...)
    pop rsp
endm

; push for 64-bit immediates is not supported on x64 too :(

; TODO: Test this
pushimm64 macro x
    sub rsp, 8
    mov [rsp], x
endm

HookStubPhase1:
    push rax
    mov rax, HookStubPhase2
    call rax
HookStubPhase1_end:

HookStubPhase2:
    pop rax
    sub rax, [HookStubPhase1_len]

    xor rax, [rsp]
    xor [rsp], rax
    xor rax, [rsp]

    pushad

    mov rcx, rsp
    add rcx, 8 * 10

    lea rax, DollOnHook
    call rax

    popad

    ret

HookStubPhase3:
    push rax

    pushad

    mov rax, [hookOriginalIP]
    mov rcx, rsp
    add rcx, 8 * 10
    mov [rcx], rax

    lea rax, DollOnAfterHook
    call rax

    popad

    add rsp, 8

    push [hookOriginalSP]
    ret

HookStubOnDeny:
    pop rax
    add rsp, [hookDenySPOffset]
    push rax
    
    mov rax, [hookDenyReturn]
    ret

.data

HookStubPhase1_len \
    dq HookStubPhase1_end - HookStubPhase1

hookOriginalSP \
    dq ?

hookOriginalIP \
    dq ?

hookDenySPOffset \
    dq ?

hookDenyReturn \
    dq ?

end