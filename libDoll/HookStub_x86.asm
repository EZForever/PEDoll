.model flat, C

; Machine code (functions) disguised as bytes
extern DollThreadIsCurrent:byte, DollGetCurrentHook:byte, \
    DollOnHook:byte, DollOnAfterHook:byte

public HookStubBefore, HookStubBefore_len, \
    HookStubA, HookStubB, HookStubOnDeny \

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
    push eax
    push 0CCCCCCCCh ; HookOEP placeholder
    mov eax, 0CCCCCCCCh ; Address pointer placeholder
    jmp eax
HookStubBefore_end:

; Side A: Standalone, called from HookStubBeforeA
;     Check if current thread a libDoll thread. If not so, save current context then hand execution over to C function
; Context:
;     stack == (HookOEP), (original eax), (return addr), (red zone...)
HookStubA:
    mov eax, [esp + 4]
    ;   Restore eax while leave space for ret
    ;   stack == (HookOEP), (ret), (return addr), (red zone...)
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * (8 + 1)
    ;   ecx == &ret
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    push ecx
    lea eax, DollThreadIsCurrent
    call eax
    add esp, 4
    ;   DollThreadIsCurrent(&ret)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * (8 + 1)
    mov eax, [ecx]
    ;   eax == ret
    test eax, eax
    ; NOTE: For now ecx is still &ret
    jnz __HookStubA_isDoll
    ;   ecx == &ret
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    push ecx
    lea eax, DollOnHook
    call eax
    add esp, 4
    ;   DollOnHook(&ret)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (newOEP), (return addr), (red zone...)
    popad
    add esp, 4
    ;   stack == (newOEP), (return addr), (red zone...)
    ret
    ;   Hand control over to newOEP
    ;   stack == (return addr), (red zone...)

__HookStubA_isDoll:
    ;   ecx == &ret
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    push ecx
    lea eax, DollGetCurrentHook
    call eax
    add esp, 4
    ;   DollGetCurrentHook(&ret)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * (8 + 1)
    mov ecx, [ecx]
    ;   ecx == &LIBDOLL_HOOK
    mov edx, [ecx + 4 * 0] ; offset LIBDOLL_HOOK::pTrampoline
    ;   edx == pTrampoline
    mov eax, esp
    add eax, 4 * (8 + 1)
    mov [eax], edx
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (pTrampoline), (return addr), (red zone...)
    popad
    add esp, 4
    ;   stack == (pTrampoline), (return addr), (red zone...)
    ret
    ;   Hand control over to pTrampoline
    ;   stack == (return addr), (red zone...)

; Side B: Standalone, called from HookStubBeforeB
;     Save current context then hand execution over to C function
; Context:
;     stack == (HookOEP), (original eax), (red zone...)
HookStubB:
    mov eax, [esp + 4]
    ;   Restore eax while leave space for ret
    ;   stack == (HookOEP), (ret), (red zone...)
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (red zone...)
    mov ecx, esp
    add ecx, 4 * (8 + 1)
    ;   ecx == &ret
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (red zone...)
    push ecx
    lea eax, DollOnAfterHook
    call eax
    add esp, 4
    ;   DollOnAfterHook(&ret)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (pTrampoline), (red zone...)
    popad
    add esp, 4
    ;   stack == (pTrampoline), (red zone...)
    ret
    ;   Hand control over to pTrampoline
    ;   stack == (red zone...)

; Called in place of a denied procedure
; Context:
;     stack == (HookOEP), (original eax), (return addr), (red zone...)
; NOTE: should act like a __nakedcall function, except that a stack balance may apply
; NOTE: Stack balance is happened in the "red zone"
HookStubOnDeny:
    mov eax, [esp + 4]
    ;   Restore eax while leave space for ret
    ;   stack == (HookOEP), (ret), (return addr), (red zone...)
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * (8 + 1)
    ;   ecx == &ret
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    push ecx
    lea eax, DollGetCurrentHook
    call eax
    add esp, 4
    ;   DollGetCurrentHook(&ret)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    mov ecx, esp
    add ecx, 4 * (8 + 1)
    mov ecx, [ecx]
    ;   ecx == &LIBDOLL_HOOK
    mov edx, [ecx + 4 * 1] ; offset LIBDOLL_HOOK::denySPOffset
    ;   edx == denySPOffset
    mov eax, esp
    add eax, 4 * 3 ; offset pushad::esp
    add [eax], edx
    ;   stack == (pushad: eax, ecx, edx, ebx, espModded, ebp, esi, edi), (HookOEP), (ret), (return addr), (red zone...)
    mov eax, esp
    add eax, 4 * (8 + 1 + 2) ; &(return addr)
    ;   eax == &(return addr)
    mov esi, [eax]
    ;   esi == return addr
    ;   Start to use edx already makes me feel pity, now here's another one... :(
    mov [eax + edx], esi
    ; return addr sent to where it should be
    ;   stack == (pushad: eax, ecx, edx, ebx, espModded, ebp, esi, edi), (HookOEP), (ret), (return addr), (stack balance area), (return addr), (red zone...)
    mov edx, [ecx + 4 * 2] ; offset LIBDOLL_HOOK::denyAX
    ;   edx == denyAX
    mov eax, esp
    add eax, 4 * 7 ; offset pushad::eax
    mov [eax], edx
    ;   stack == (pushad: eaxModded, ecx, edx, ebx, espModded, ebp, esi, edi), (HookOEP), (ret), (return addr), (stack balance area), (return addr), (red zone...)
    popad
    ;   Magic happens since we modifyed esp
    ;   stack == (return addr), (red zone...)
    ret
    ;   Hand control back to caller code
    ;   stack == (red zone...)

.data

; Length of HookStubBefore*, used for copying & fill in HookOEP
; Consider these as constants
HookStubBefore_len \
    dd HookStubBefore_end - HookStubBefore

end