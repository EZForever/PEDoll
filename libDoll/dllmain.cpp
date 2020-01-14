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

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        // Try my best to avoid infinite loop
        DisableThreadLibraryCalls(hModule);

        // Initialize all the global contexts
        ctx.pRealGetCurrentThreadId = GetCurrentThreadId;
        // TODO: Update this if DetourAttach() / DetourDetach() happened on GetCurrentThreadId
        // TODO: Other necessary varibles

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

