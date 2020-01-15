#include "pch.h"
#include "libDoll.h"

extern "C" unsigned long DollThreadIsCurrent()
{
    // Avoid hook on GetCurrentThreadId() to cause endless loop
    return (unsigned long)(ctx.dollThreads.find(ctx.pRealGetCurrentThreadId()) != ctx.dollThreads.end());
}

extern "C" unsigned long DollGetCurrentHook(unsigned long* context)
{
    return (unsigned long)(ctx.dollHooks.find(*context)->second);
}

extern "C" void DollOnHook(unsigned long* context)
{
    // TODO: Procedure:
    // register current thread
    // EnterCriticalSection
    // hookOriginalSP = context[1];
    // "Before..." operations
    //    Set context[0] & context[1] based on the reply
    //    If reply is Deny, set hookDenySPOffset & hookDenyReturn

    // Replies are implemented by modifying HookOEP & OriginalSP
    //    Allow: HookOEP to pTrampoline, OriginalSP to pBeforeB
    //    Deny: HookOEP to HookStubOnDeny, OriginalSP to pBeforeB
    //    Terminate: HookOEP to DebugBreak / __fastfail, OriginalSP unchanged (This way the call stack is like called DebugBreak() by hand)

    DollThreadRegisterCurrent();

    LIBDOLL_HOOK* hook = (LIBDOLL_HOOK*)DollGetCurrentHook(context);
    EnterCriticalSection(&hook->lock);

    hook->originalSP = context[1];

    // TODO: data report & wait for verdict

#if 0
    // Allow
    context[0] = hook->pTrampoline;
    context[1] = (unsigned long)hook->pBeforeB;

    // Terminate
    context[0] = (unsigned long)DebugBreak;
#endif

    // Deny
    context[0] = (unsigned long)hook->pBeforeDeny;
    context[1] = (unsigned long)hook->pBeforeB;
}

extern "C" void DollOnAfterHook(unsigned long* context)
{
    // TODO: Procedure:
    // "After..." operations
    //    If prompted to Terminate, overwrite hookOriginalSP with DebugBreak / __fastfail
    //    since it is not possible to do it "the pretty way" // TODO: Revise this sentence
    // context[0] = hookOriginalSP;
    // LeaveCriticalSection
    // unregister current thread

    LIBDOLL_HOOK* hook = (LIBDOLL_HOOK*)DollGetCurrentHook(context);

    // TODO: data report & wait for verdict

    // Continue
    context[0] = hook->originalSP;

#if 0
    // Terminate
    context[0] = (unsigned long)DebugBreak;
#endif

    LeaveCriticalSection(&hook->lock);

    DollThreadUnregisterCurrent();
}

