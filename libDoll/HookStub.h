// HookStub.h
// Platform-independent declarations for C++-Assembly interop 
#pragma once

extern "C" {
    
    // They are not "char"s; they are pieces of machine code

    extern char HookStubPhase1;
    extern char HookStubPhase3;
    extern char HookStubOnDeny;

    extern const unsigned long HookStubPhase1_len;

    // Must be initialized before HookStubPhase3

    extern unsigned long hookOriginalSP;
    extern unsigned long hookOriginalIP;

    // Must be initialized before HookStubOnDeny

    extern unsigned long hookDenySPOffset;
    extern unsigned long hookDenyReturn;

    // context[0] = HookOEP, context[1] = OriginalSP, n > 1 should not be accessed directly
    // context[-1] = {e|r}ax, context[-2] = {e|r}bx, so on

    extern void DollOnHook(unsigned long* context);
    extern void DollOnAfterHook(unsigned long* context);

}

