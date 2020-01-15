#pragma once
#include "pch.h"

struct LIBDOLL_HOOKEVENTS {
    // TODO: fill this struct (will need a LIBDOLL_HOOKEVENT)
};

#pragma pack(push, 1)

// The context of an active hook
// This struct is not code-independent; will be visited by assembly code
struct LIBDOLL_HOOK {
    unsigned long pTrampoline;
    unsigned long denySPOffset;
    unsigned long denyAX;
    unsigned long originalSP;
    char* pBeforeA;
    char* pBeforeB;
    char* pBeforeDeny;
    DWORD pBeforeAProtect;
    DWORD pBeforeBProtect;
    DWORD pBeforeDenyProtect;
    LIBDOLL_HOOKEVENTS* onBefore;
    LIBDOLL_HOOKEVENTS* onAfter;
    CRITICAL_SECTION lock;
};

#pragma pack(pop)

struct LIBDOLL_CTX {
    std::set<DWORD> dollThreads;
    std::map<unsigned long, LIBDOLL_HOOK*> dollHooks;
    DWORD (__stdcall *pRealGetCurrentThreadId)();
};

// Global context
extern LIBDOLL_CTX ctx;

// Register current thread as a libDoll thread
// libDoll threads will not be affected by hooks, i.e. will follow the hook but sliently continues
extern void DollThreadRegisterCurrent();

// Unregister current thread
extern void DollThreadUnregisterCurrent();

