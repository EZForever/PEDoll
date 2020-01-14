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

    extern const unsigned long HookStubBefore_len;

    // context[0] = [inout]ret
    extern void DollThreadIsCurrent(unsigned long* context);

    // context[0] = [out]ret, context[-1] = [in]HookOEP
    extern void DollGetCurrentHook(unsigned long* context);

    // context[-1] = [in]HookOEP, context[0] = [out]newOEP, context[1] = [inout]returnAddr
    // context[n] for n > 1 should not be accessed directly
    // context[-2] = [in]{e|r}ax, context[-3] = [in]{e|r}bx, etc
    extern void DollOnHook(unsigned long* context);

    // Same as DollOnHook() but context[0] = [out]returnAddr and context[1] is now gone
    extern void DollOnAfterHook(unsigned long* context);

}

