#pragma once
#include "pch.h"

#include "../libPuppet/libPuppet.h"

// UINT_PTR represents the machine's native word size as a unsigned integer type
// See https://docs.microsoft.com/en-us/windows/win32/winprog/windows-data-types#uint_ptr

// The prototype of GetCurrentThreadId()
typedef DWORD (__stdcall *GET_CURRENT_THREAD_ID)();

#pragma pack(push, 1)

// The context of an active hook
// This struct is not code-independent; will be visited by assembly code
struct LIBDOLL_HOOK {

    // Members being used in assembly level

    UINT_PTR pTrampoline;
    UINT_PTR denySPOffset;
    UINT_PTR denyAX;
    UINT_PTR originalSP;

    // Dynamically allocated HookStubBefore* stubs

    char* pBeforeA;
    char* pBeforeB;
    char* pBeforeDeny;
    DWORD pBeforeAProtect;
    DWORD pBeforeBProtect;
    DWORD pBeforeDenyProtect;

    // Interop with TPuppet

    UINT_PTR* context;
    uint32_t verdict;

};

#pragma pack(pop)

struct LIBDOLL_CTX {

    Puppet::IPuppet* puppet;
    HANDLE hTPuppet;

    std::set<DWORD> dollThreads;
    std::set<HANDLE> suspendedThreads;

    std::map<UINT_PTR, LIBDOLL_HOOK*> dollHooks;
    HANDLE hEvtHookVerdict;
    UINT_PTR waitingHookOEP;
    CRITICAL_SECTION lockHook;

    void* pEP;
    HANDLE hEvtEP;

    GET_CURRENT_THREAD_ID pRealGetCurrentThreadId;

};

// Global context
extern LIBDOLL_CTX ctx;

