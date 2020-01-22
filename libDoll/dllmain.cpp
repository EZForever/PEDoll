#include "pch.h"
#include "libDoll.h"

#include "../Detours/repo/src/detours.h"
#include "Thread.h"

void __cdecl TJudger(void*);
void __cdecl TPuppet(void*);

const GUID PAYLOAD_SERVER_INFO = { 0xa2062469, 0x2b45, 0x496d, { 0x8f, 0xe9, 0x7e, 0x89, 0x4e, 0xd7, 0x22, 0x70 } };

const int PUPPET_PORT = 31415;

LIBDOLL_CTX ctx;

Puppet::PACKET_STRING* DollDllFindServerInfo()
{
    HMODULE hIter = DetourEnumerateModules(NULL);
    Puppet::PACKET_STRING* payload;
    DWORD payloadSize;

    while(hIter)
    {
        payload = (Puppet::PACKET_STRING*)DetourFindPayload(hIter, PAYLOAD_SERVER_INFO, &payloadSize);
        if (payload && payloadSize == payload->size)
            return payload;
        hIter = DetourEnumerateModules(hIter);
    }

    return NULL;
}

BOOL DollDllAttach()
{
    // Restore IAT modifyed by inject procedure
    // Ignore any errors though, since the injection can be done in other ways than DetourCreateProcessWithDllEx()
    DetourRestoreAfterWith();

    // Initialize all the global contexts

    // Fetch server infomation stored by Monitor
    ctx.pServerInfo = DollDllFindServerInfo();
    if (!ctx.pServerInfo)
    {
        DollThreadPanic(L"DollDllAttach(): No server information found");
        return FALSE;
    }

    // This will get updated if DetourAttach() / DetourDetach() happened on GetCurrentThreadId
    ctx.pRealGetCurrentThreadId = GetCurrentThreadId;

    // The global lock for hooks
    InitializeCriticalSection(&ctx.lockHook);

    uintptr_t ret;

    // ThreadHookJudger(TJudger) manages the verdict of a hooked procedure
    ret = _beginthread(TJudger, 0, NULL);
    if (ret == 0 || ret == -1) // these status means error occurred
    {
        DollThreadPanic(L"DollDllAttach(): _beginthread(ThreadHookJudger) failed");
        return FALSE;
    }

    // ThreadPuppet(TPuppet) establish the connection to Controller
    ret = _beginthread(TPuppet, 0, NULL);
    if (ret == 0 || ret == -1) // these status means error occurred
    {
        DollThreadPanic(L"DollDllAttach(): _beginthread(ThreadPuppet) failed");
        return FALSE;
    }

    return TRUE;
}

BOOL DollDllDetach()
{
    // Clean up

    DeleteCriticalSection(&ctx.lockHook);

    return TRUE;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
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
int __declspec(dllexport) DollDllHelloWorld()
{
    return 42;
}