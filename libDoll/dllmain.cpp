#include "pch.h"
#include "libDoll.h"

#include "../Detours/repo/src/detours.h"
#include "HookStub.h"
#include "Thread.h"
#include "Hook.h"

void __cdecl TPuppet(void*);

LIBDOLL_CTX ctx;

char* DollDllFindServerInfo()
{
    HMODULE hIter = DetourEnumerateModules(NULL);
    char* payload;
    DWORD payloadSize;

    while(hIter)
    {
        payload = (char*)DetourFindPayload(hIter, Puppet::PAYLOAD_SERVER_INFO, &payloadSize);
        if (payload)
            return payload;
        hIter = DetourEnumerateModules(hIter);
    }

    return NULL;
}

BOOL DollDllAttach(BOOL isInjectedByDetours)
{
    // This will get updated if DetourAttach() / DetourDetach() happened on GetCurrentThreadId
    // Initialize this before usage in DollThreadSuspendAll()
    ctx.pRealGetCurrentThreadId = GetCurrentThreadId;

    if (isInjectedByDetours)
    {
        // Set up a one-time hook at the entry point of the main executable
        // NOTE: Try to DollThreadSuspendAll() now will just suspend DLL loader threads on Windows 10, thus breaking the first "break"
        // NOTE: DetourCreateProcessWithDllEx() will erase the original IAT, so we'll always be the first DLL initializing

        // Get & store EP
        MODULEINFO mi;
        GetModuleInformation(GetCurrentProcess(), GetModuleHandle(NULL), &mi, sizeof(MODULEINFO));
        ctx.pEP = mi.EntryPoint;

        // Initialize the EP event
        ctx.hEvtEP = CreateEvent(NULL, TRUE, FALSE, NULL);

        // Set the actual hook
        DetourTransactionBegin();
        DetourAttach(&ctx.pEP, &HookStubEP);
        DetourTransactionCommit();
    }
    else
    {
        // Suspend any victim thread, in case of attaching
        // There should be no libDoll threads for now
        DollThreadSuspendAll(false);

        // Invalidate states meant for other situations
        ctx.pEP = NULL;
        ctx.hEvtEP = INVALID_HANDLE_VALUE;
    }

    // Initialize all the global contexts

    // Fetch server infomation stored by Monitor
    // NOTE: This pointer points to a static area in another module, do not "delete[] pServerInfo;"
    char* pServerInfo = DollDllFindServerInfo();
    if (!pServerInfo)
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
    uintptr_t hTPuppet = _beginthread(TPuppet, 0, pServerInfo);
    if (hTPuppet == 0 || hTPuppet == -1) // these status means error occurred
    {
        DollThreadPanic(L"DollDllAttach(): _beginthread(ThreadPuppet) failed");
        return FALSE;
    }
    ctx.hTPuppet = (HANDLE)hTPuppet;

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
        // Restore IAT modified by inject procedure
        // It is here, not in DollDllAttach(), because IAT must be restored before any API call
        // If any error occurs, the injection might be done in ways other than DetourCreateProcessWithDllEx()
        BOOL isInjectedByDetours = DetourRestoreAfterWith();

        // XXX: DisableThreadLibraryCalls() will interfere with a statically-linked CRT
        // https://docs.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-disablethreadlibrarycalls#remarks
        //DisableThreadLibraryCalls(hModule);

        return DollDllAttach(isInjectedByDetours);
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