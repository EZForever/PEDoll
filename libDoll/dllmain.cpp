#include "pch.h"
#include "libDoll.h"

extern void __cdecl ThreadPuppetClient(void*);

LIBDOLL_CTX ctx;

void DollThreadRegisterCurrent()
{
    ctx.dollThreads.insert(GetCurrentThreadId());
}

void DollThreadUnregisterCurrent()
{
    ctx.dollThreads.erase(ctx.dollThreads.find(GetCurrentThreadId()));
}

bool DollThreadIsCurrent() {
    return ctx.dollThreads.find(GetCurrentThreadId()) != ctx.dollThreads.end();
}

// FIXME: Only here for testing purposes. Should move to a separate file.
extern "C" void DollOnHook(unsigned long* context)
{
    // TODO: Procedure:
    // Save isDoll status
    // (if not isDoll) register current thread
    // EnterCriticalSection
    // // FIXME: What if I hook GetCurrentThreadId() / EnterCriticalSection() / LeaveCriticalSection()? It would loop endlessly!
    // hookOriginalIP = context[0];
    // hookOriginalSP = context[1];
    // unhook
    // if not isDoll:
    //    "Before..." operations
    //    Set context[0] & context[1] based on the reply
    //    If reply is Deny, set hookDenySPOffset & hookDenyReturn
    // else:
    //    run "Allow" reply procdures

    // Replies are implemented by modifying HookOEP & OriginalSP
    //    Allow: HookOEP unchanged, OriginalSP to HookStubPhase3
    //    Deny: HookOEP to HookStubOnDeny, OriginalSP to HookStubPhase3
    //    Terminate: HookOEP to DebugBreak, OriginalSP unchanged (This way the call stack is like called DebugBreak() by hand)

}

// FIXME: Only here for testing purposes. Should move to a separate file.
extern "C" void DollOnAfterHook(unsigned long* context)
{
    // TODO: Procedure:
    // if not isDoll:
    //    "After..." operations
    //    If prompted to Terminate, overwrite hookOriginalSP with DebugBreak / __fastfail
    //    since it is not possible to do it "the pretty way"
    // rehook
    // LeaveCriticalSection
    // (if not isDoll) unregister current thread

}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        DisableThreadLibraryCalls(hModule);
        uintptr_t ret = _beginthread(ThreadPuppetClient, 0, NULL);
        if (ret == 0 || ret == -1)
            return FALSE; // these status means error occurred
        break;
    }
    case DLL_PROCESS_DETACH:
    {
        // TODO: clean up (?)
        break;
    }
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }
    return TRUE;
}

