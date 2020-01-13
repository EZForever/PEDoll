.model flat, C

; Machine code (functions) disguised as bytes
extern DollOnHook:byte, DollOnAfterHook:byte

public HookStubPhase1, HookStubPhase3, HookStubOnDeny, \
    HookStubPhase1_len, \
    hookOriginalSP, hookOriginalIP, \
    hookDenySPOffset, hookDenyReturn

.code

; Phase 1: Inserted into hooked code
; Context:
;     stack == (return addr), (red zone...)
HookStubPhase1:
    ;   Preserve eax before being overwritten
    push eax
    mov eax, HookStubPhase2
    ;   push eip onto stack
    call eax
HookStubPhase1_end:

; Phase 2: Standalone, save current context then hand execution over to C function
; Context:
;     eax == HookStubPhase2 (garbage)
;     stack == (HookOEP + HookStubPhase1_len), (original eax), (return addr), (red zone...)
HookStubPhase2:
    pop eax
    sub eax, [HookStubPhase1_len]
    ;   eax == HookOEP
    ;   stack == (original eax), (return addr), (red zone...)
    xor eax, [esp] ; eax = eaxOrig ^ [esp]Orig
    xor [esp], eax ; [esp] = [esp]Orig ^ eax = [esp]Orig ^ eaxOrig ^ [esp]Orig = eaxOrig
    xor eax, [esp] ; eax = eax ^ [esp] = eaxOrig ^ [esp]Orig ^ eaxOrig = [esp]Orig
    ;   swap(eax, [esp])
    ;   stack == (HookOEP), (return addr), (red zone...)
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    mov eax, esp
    add eax, 4 * 8
    ;   eax == &HookOEP
    push eax
    lea eax, DollOnHook
    call eax
    add esp, 4
    ;   DollOnHook(&HookOEP)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (return addr), (red zone...)
    popad
    ;   stack == (HookOEP), (return addr), (red zone...)
    ret
    ;   Control returns to original code
    ;   stack == (return addr), (red zone...)

; Phase 3: Called via modified [esp] (i.e. after hooked function being called)
; Context:
;     eax == (return of called procedure)
;     stack == ((stack-balanced) red zone...)
HookStubPhase3:
    push eax
    ;   Leave space for HookOEP
    ;   stack == (garbage), (red zone...)
    pushad
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (garbage), (red zone...)
    mov eax, [hookOriginalIP]
    mov ecx, esp
    add ecx, 4 * 8
    mov [ecx], eax
    ;   eax == HookOEP
    ;   ecx == &HookOEP
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (red zone...)
    ;   NOTE: This time HookOEP is read-only
    push ecx
    lea eax, DollOnAfterHook
    call eax
    add esp, 4
    ;   DollOnAfterHook(&HookOEP)
    ;   stack == (pushad: eax, ecx, edx, ebx, esp, ebp, esi, edi), (HookOEP), (red zone...)
    popad
    ;   stack == (HookOEP), (red zone...)
    add esp, 4
    ;   stack == (red zone...)
    push [hookOriginalSP]
    ret
    ;   Return to original ESP, i.e. where the call should return

; Called in place of a denied procedure
; Context:
;     stack == (return addr), (red zone...)
; NOTE: should act like a __nakedcall function, except that a stack balance may apply
; NOTE: Stack balance is happened in the "red zone"
HookStubOnDeny:
    pop eax
    add esp, [hookDenySPOffset]
    push eax
    ;   Preserve return addr while balancing the stack
    mov eax, [hookDenyReturn]
    ret
    ;   Pseudo-function ends, should return to HookStubPhase3

.data

; Length of HookStubPhase1_len, used for copying Phase 1 code & calculate HookOEP
; Consider this a constant
HookStubPhase1_len \
    dd HookStubPhase1_end - HookStubPhase1

; These varibles are global because it would be so much trouble to keep them on the stack
; Instead, they are initialized by DollOnHookAfter, once per call
; Nothing bad should really happen once EnterCriticalSection() is called

hookOriginalSP \
    dd ?

hookOriginalIP \
    dd ?

hookDenySPOffset \
    dd ?

hookDenyReturn \
    dd ?

end