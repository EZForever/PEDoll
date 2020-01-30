#include "pch.h"
#include "libDoll.h"

#include "../Detours/repo/src/detours.h"
#include "Thread.h"
#include "Hook.h"

void __cdecl TPuppet(void*);

LIBDOLL_CTX ctx;

Puppet::PACKET_STRING* DollDllFindServerInfo()
{
    HMODULE hIter = DetourEnumerateModules(NULL);
    Puppet::PACKET_STRING* payload;
    DWORD payloadSize;

    while(hIter)
    {
        payload = (Puppet::PACKET_STRING*)DetourFindPayload(hIter, Puppet::PAYLOAD_SERVER_INFO, &payloadSize);
        if (payload && payloadSize == payload->size && payload->type == Puppet::PACKET_TYPE::STRING)
            return payload;
        hIter = DetourEnumerateModules(hIter);
    }

    return NULL;
}

BOOL DollDllAttach()
{
    // This will get updated if DetourAttach() / DetourDetach() happened on GetCurrentThreadId
    // Initialize this before usage in DollThreadSuspendAll()
    ctx.pRealGetCurrentThreadId = GetCurrentThreadId;

    // Suspend any victim thread, in case of attaching
    // There should be no libDoll threads for now
    DollThreadSuspendAll(false);

    // Initialize all the global contexts

    // Fetch server infomation stored by Monitor
    ctx.pServerInfo = DollDllFindServerInfo();
    if (!ctx.pServerInfo)
    {
        DollThreadPanic(L"DollDllAttach(): No server information found");
        return FALSE;
    }

    // The event object handle for informing hooked thread
    ctx.hEvtHookVerdict = CreateEvent(NULL, FALSE, FALSE, NULL);

    // The global lock for hooks
    InitializeCriticalSection(&ctx.lockHook);

    // _beginthread() returns a (uintptr_t)HANDLE to the created thread
    // i.e. the return value of CreateThread()

    // ThreadPuppet(TPuppet) establishes the connection to Controller
    uintptr_t hTPuppet = _beginthread(TPuppet, 0, NULL);
    if (hTPuppet == 0 || hTPuppet == -1) // these status means error occurred
    {
        DollThreadPanic(L"DollDllAttach(): _beginthread(ThreadPuppet) failed");
        return FALSE;
    }
    ctx.hTPuppet = (HANDLE)hTPuppet;

    // This is not happened until a CMD_BREAK
    //DollThreadResumeAll();
    
    return TRUE;
}

BOOL DollDllDetach()
{
    // Register current thread to avoid hook happen in unhook process
    DollThreadRegisterCurrent();

    // Clean up
    DollThreadSuspendAll(false);
    
    // Free all hooks but leave all entries, avoiding concurrent modification
    for (auto iter = ctx.dollHooks.cbegin(); iter != ctx.dollHooks.cend(); iter++)
        DollHookRemove(iter->first, false);
    ctx.dollHooks.clear();

    // FIXME: TerminateThread() will cause huge resource leaks
    // Should inform TPuppet about DLL detachment
    TerminateThread(ctx.hTPuppet, 0);

    // For now no hook should exist, unregister self
    DollThreadUnregisterCurrent();

    CloseHandle(ctx.hTPuppet);

    DeleteCriticalSection(&ctx.lockHook);

    CloseHandle(ctx.hEvtHookVerdict);

    DollThreadResumeAll();
    return TRUE;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        // Restore IAT modifyed by inject procedure
        // It is here, not in DollDllAttach(), because IAT must be restored before any API call
        // Ignore any errors though, since the injection can be done in ways other than DetourCreateProcessWithDllEx()
        DetourRestoreAfterWith();

        // Try my best to avoid infinite loop
        DisableThreadLibraryCalls(hModule);

        return DollDllAttach();
    }
    case DLL_PROCESS_DETACH:
    {
        return DollDllDetach();
    }
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }
    return TRUE;
}

// DetourCreateProcessWithDllEx() requires at least 1 export function (ordinal #1)
extern "C" int __declspec(dllexport) DollDllHelloWorld()
{
    return 42;
}