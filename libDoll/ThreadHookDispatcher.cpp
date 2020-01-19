#include "pch.h"
#include "libDoll.h"

#include "Thread.h"
#include "Hook.h"

void __cdecl ThreadHookDispatcher(void* arg)
{
    DollThreadRegisterCurrent();

    // TODO: Wait for OnHook / AddHook / RemoveHook events

    // NOTE: this is just a simple PoC
    // If mix Debug & Release, they will have different system() address, thus fail to hook
    UINT_PTR hookOEP = (UINT_PTR)system;
    HookAdd(hookOEP, 0, 42);


}