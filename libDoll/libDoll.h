#pragma once
#include "pch.h"

// UINT_PTR represents the machine's native word size as a unsigned integer type
// See https://docs.microsoft.com/en-us/windows/win32/winprog/windows-data-types#uint-ptr

// The prototype of GetCurrentThreadId()
typedef DWORD (__stdcall *GET_CURRENT_THREAD_ID)();

#pragma pack(push, 1)

// The context of an active hook
// This struct is not code-independent; will be visited by assembly code
struct LIBDOLL_HOOK {
    UINT_PTR pTrampoline;
    UINT_PTR denySPOffset;
    UINT_PTR denyAX;
    UINT_PTR originalSP;
    char* pBeforeA;
    char* pBeforeB;
    char* pBeforeDeny;
    DWORD pBeforeAProtect;
    DWORD pBeforeBProtect;
    DWORD pBeforeDenyProtect;
};

#pragma pack(pop)

struct LIBDOLL_CTX {
    std::set<DWORD> dollThreads;
    std::map<UINT_PTR, LIBDOLL_HOOK*> dollHooks;
    GET_CURRENT_THREAD_ID pRealGetCurrentThreadId;
    CRITICAL_SECTION lockHook;
};

// Global context
extern LIBDOLL_CTX ctx;

