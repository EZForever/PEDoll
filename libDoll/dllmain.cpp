#include "pch.h"
#include "libDoll.h"

void __cdecl ThreadHookDispatcher(void*);

LIBDOLL_CTX ctx;

BOOL DollDllAttach()
{
    // Initialize all the global contexts

    // This will get updated if DetourAttach() / DetourDetach() happened on GetCurrentThreadId
    ctx.pRealGetCurrentThreadId = GetCurrentThreadId;

    InitializeCriticalSection(&ctx.lockHook);
    // TODO: Other necessary varibles

    uintptr_t ret = _beginthread(ThreadHookDispatcher, 0, NULL);
    if (ret == 0 || ret == -1)
        return FALSE; // these status means error occurred

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

