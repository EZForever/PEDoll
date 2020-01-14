#include "pch.h"
#include "libDoll.h"

extern "C" void DollThreadIsCurrent(unsigned long* context)
{
    // Avoid hook on GetCurrentThreadId() to cause endless loop
    *context = (unsigned long)(ctx.dollThreads.find(ctx.pRealGetCurrentThreadId()) != ctx.dollThreads.end());
}

extern "C" void DollGetCurrentHook(unsigned long* context)
{
    *context = (unsigned long)(ctx.dollHooks.find(*(context - 1))->second);
}

extern "C" void DollOnHook(unsigned long* context)
{
    // TODO: Procedure:
    //  register current thread
    // EnterCriticalSection
    // hookOriginalSP = context[1];
    // "Before..." operations
    //    Set context[0] & context[1] based on the reply
    //    If reply is Deny, set hookDenySPOffset & hookDenyReturn

    // Replies are implemented by modifying HookOEP & OriginalSP
    //    Allow: HookOEP to pTrampoline, OriginalSP to pBeforeB
    //    Deny: HookOEP to HookStubOnDeny, OriginalSP to pBeforeB
    //    Terminate: HookOEP to DebugBreak / __fastfail, OriginalSP unchanged (This way the call stack is like called DebugBreak() by hand)

}

extern "C" void DollOnAfterHook(unsigned long* context)
{
    // TODO: Procedure:
    // "After..." operations
    //    If prompted to Terminate, overwrite hookOriginalSP with DebugBreak / __fastfail
    //    since it is not possible to do it "the pretty way" // TODO: Revise this sentence
    // LeaveCriticalSection
    // (if not isDoll) unregister current thread
    // context[0] = hookOriginalSP;

}

