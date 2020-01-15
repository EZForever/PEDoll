.model flat, C

; Machine code (functions) disguised as bytes
extern DollThreadIsCurrent:byte, DollGetCurrentHook:byte, \
    DollOnHook:byte, DollOnAfterHook:byte

public HookStubBefore, HookStubA, HookStubB, HookStubOnDeny, \
    HookStubBefore_len, HookStubBefore_HookOEPOffset, HookStubBefore_AddrOffset

.code

; Before: Each API keeps a copy, work as the "Detoured function"
; Purpose is to provide HookOEP
; Context (Before A):
;     stack == (return addr), (red zone...)
; Context (Before B):
;     stack == (red zone...)
; Context (Before OnDeny):
;     stack == (return addr), (red zone...)
HookStubBefore:
    push 0CCCCCCCCh ; HookOEP placeholder
    push 0CCCCCCCCh ; Address pointer placeholder
    ret
HookStubBefore_end:

; Side A: Standalone, called from HookStubBeforeA
;     Check if current thread a libDoll thread. If not so, save current context then hand execution over to C function
; Context:
;     stack == (HookOEP), (return addr), (red zone...)
HookStubA:
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    lea eax, DollThreadIsCurrent
    call eax
    ;   DollThreadIsCurrent()
    ;   eax == ret
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * 8
    ;   ecx == &HookOEP
    test eax, eax
    jnz __HookStubA_isDoll
    ;   ecx == &HookOEP
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    push ecx
    lea eax, DollOnHook
    call eax
    add esp, 4
    ;   DollOnHook(&HookOEP)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (newOEP), (return addr), (red zone...)
    popad
    ;   stack == (newOEP), (return addr), (red zone...)
    ret
    ;   Hand control over to newOEP
    ;   stack == (return addr), (red zone...)

__HookStubA_isDoll:
    ;   ecx == &HookOEP
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    push ecx
    lea eax, DollGetCurrentHook
    call eax
    add esp, 4
    ;   DollGetCurrentHook(&HookOEP)
    ;   eax == &LIBDOLL_HOOK
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    mov edx, [eax + 4 * 0] ; offset LIBDOLL_HOOK::pTrampoline
    ;   edx == pTrampoline
    mov ecx, esp
    add ecx, 4 * 8
    mov [ecx], edx
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (pTrampoline), (return addr), (red zone...)
    popad
    ;   stack == (pTrampoline), (return addr), (red zone...)
    ret
    ;   Hand control over to pTrampoline
    ;   stack == (return addr), (red zone...)

; Side B: Standalone, called from HookStubBeforeB
;     Save current context then hand execution over to C function
; Context:
;     stack == (HookOEP), (red zone...)
HookStubB:
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (red zone...)
    mov ecx, esp
    add ecx, 4 * 8
    ;   ecx == &HookOEP
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (red zone...)
    push ecx
    lea eax, DollOnAfterHook
    call eax
    add esp, 4
    ;   DollOnAfterHook(&HookOEP)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (return addr), (red zone...)
    popad
    ;   stack == (return addr), (red zone...)
    ret
    ;   Hand control over to caller code
    ;   stack == (red zone...)

; Called in place of a denied procedure
; Context:
;     stack == (HookOEP), (return addr), (red zone...)
; NOTE: should act like a __nakedcall function, except that a stack balance will apply
; NOTE: Stack balance is happened in the "red zone"
HookStubOnDeny:
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * 8
    ;   ecx == &HookOEP
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    push ecx
    lea eax, DollGetCurrentHook
    call eax
    add esp, 4
    ;   DollGetCurrentHook(&HookOEP)
    ;   eax == &LIBDOLL_HOOK
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    mov edx, [eax + 4 * 1] ; offset LIBDOLL_HOOK::denySPOffset
    ;   edx == denySPOffset
    add [esp + 4 * 3], edx ; offset pushad::esp
    ;   stack == (pushad: eax, ecx, edx, ebx, espModded, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * (8 + 1) ; &(return addr)
    ;   ecx == &(return addr)
    mov esi, [ecx]
    ;   esi == return addr
    ;   Start to use edx already makes me feel pity, now here's another one... :(
    mov [ecx + edx], esi
    ; return addr sent to where it should be
    ;   stack == (pushad: eax, ecx, edx, ebx, espModded, ebp, esi, edi), (HookOEP), (return addr), [stack balance area], (return addr), (red zone...)
    mov edx, [eax + 4 * 2] ; offset LIBDOLL_HOOK::denyAX
    ;   edx == denyAX
    mov [esp + 4 * 7], edx ; offset pushad::eax
    ;   stack == (pushad: eaxModded, ecx, edx, ebx, espModded, ebp, esi, edi), (HookOEP), (return addr), [stack balance area], (return addr), (red zone...)
    popad
    ;   Magic happens since we modifyed esp
    ;   stack == (stack balance area the size of HookOEP), (return addr), (red zone...)
    add esp, 4
    ;   stack == (return addr), (red zone...)
    ret
    ;   Hand control back to caller code
    ;   stack == (red zone...)

.const

; Length of HookStubBefore*, used for copying & fill in HookOEP
HookStubBefore_len \
    dd HookStubBefore_end - HookStubBefore

; Offset to HookOEP placeholder
HookStubBefore_HookOEPOffset \
    dd 1

; Offset to address pointer placeholder
HookStubBefore_AddrOffset \
    dd 6

end