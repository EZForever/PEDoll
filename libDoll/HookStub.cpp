#include "pch.h"
#include "libDoll.h"
#include "Thread.h"

extern "C" UINT_PTR DollThreadIsCurrent()
{
    // Avoid hook on GetCurrentThreadId() to cause endless loop
    return (UINT_PTR)(ctx.dollThreads.find(ctx.pRealGetCurrentThreadId()) != ctx.dollThreads.end());
}

extern "C" UINT_PTR DollHookGetCurrent(UINT_PTR* context)
{
    return (UINT_PTR)(ctx.dollHooks.find(*context)->second);
}

extern "C" void DollOnHook(UINT_PTR* context)
{
    // Procedure:
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

    EnterCriticalSection(&ctx.lockHook);

    LIBDOLL_HOOK* hook = (LIBDOLL_HOOK*)DollHookGetCurrent(context);
    hook->context = context;
    hook->originalSP = context[1];

    // TODO: Send MSG_ONHOOK packet

    ctx.waitingHookOEP = context[0];
    WaitForSingleObject(ctx.hEvtHookVerdict, INFINITE);

    if (hook->verdict == 0)
    {
        // Approved
        context[0] = hook->pTrampoline;
        context[1] = (UINT_PTR)hook->pBeforeB;
    }
    else if (hook->verdict == 1)
    {
        // Rejected
        // Parameters are set by TPuppetOnRecv*()
        context[0] = (UINT_PTR)hook->pBeforeDeny;
        context[1] = (UINT_PTR)hook->pBeforeB;
    }
    else
    {
        // Terminate
        context[0] = (UINT_PTR)DebugBreak;
    }
}

extern "C" void DollOnAfterHook(UINT_PTR* context)
{
    // Procedure:
    // "After..." operations
    //    If prompted to Terminate, overwrite hookOriginalSP with DebugBreak / __fastfail
    //    since it is not possible to do it "the pretty way" // TODO: Revise this sentence
    // context[0] = hookOriginalSP;
    // LeaveCriticalSection
    // unregister current thread

    LIBDOLL_HOOK* hook = (LIBDOLL_HOOK*)DollHookGetCurrent(context);

    // TODO: Send MSG_ONHOOK packet

    ctx.waitingHookOEP = context[0];
    WaitForSingleObject(ctx.hEvtHookVerdict, INFINITE);

    if (hook->verdict == 0)
    {
        // Approved / Continue
        context[0] = hook->originalSP;
    }
    else
    {
        // Terminate
        context[0] = (UINT_PTR)DebugBreak;
    }

    LeaveCriticalSection(&ctx.lockHook);

    DollThreadUnregisterCurrent();
}

