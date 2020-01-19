// HookStub.h
// Platform-independent declarations for C++-Assembly interop
// Although platform-independent, they're not code-independent; I/O must follow the commented rules strictly
#pragma once
#include "libDoll.h"

extern "C" {
    
    // They are not "char"s; they are pieces of machine code

    extern char HookStubBefore;
    extern char HookStubA;
    extern char HookStubB;
    extern char HookStubOnDeny;

    extern const UINT_PTR HookStubBefore_len;
    extern const UINT_PTR HookStubBefore_HookOEPOffset;
    extern const UINT_PTR HookStubBefore_AddrOffset;

    UINT_PTR DollThreadIsCurrent();

    // context[0] = [in]HookOEP
    UINT_PTR DollGetCurrentHook(UINT_PTR* context);

    // context[0] = [inout]HookOEP, context[1] = [inout]returnAddr
    // context[n] for n > 1 should not be accessed directly
    // context[-1] = [in]{e|r}ax, context[-2] = [in]{e|r}bx, etc
    void DollOnHook(UINT_PTR* context);

    // Same as DollOnHook() but context[0] = [out]returnAddr and context[1] is now gone
    void DollOnAfterHook(UINT_PTR* context);

}

