#pragma once
#include "pch.h"

// A type repersenting the machine's native word size
// Because M$ made sizeof(unsigned long) == 4 on x64
#ifdef _WIN64
typedef uint64_t NATIVEWORD;
#else
typedef uint32_t NATIVEWORD;
#endif
// XXX: Use uintptr_t from <cstdint> instead?
// That type is "capable of holding a pointer", but not necessary to be exactly the same size
// So that may not be an option

// The prototype of GetCurrentThreadId()
typedef DWORD (__stdcall *GET_CURRENT_THREAD_ID)();

#pragma pack(push, 1)

// The context of an active hook
// This struct is not code-independent; will be visited by assembly code
struct LIBDOLL_HOOK {
    NATIVEWORD pTrampoline;
    NATIVEWORD denySPOffset;
    NATIVEWORD denyAX;
    NATIVEWORD originalSP;
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
    std::map<NATIVEWORD, LIBDOLL_HOOK*> dollHooks;
    GET_CURRENT_THREAD_ID pRealGetCurrentThreadId;
    CRITICAL_SECTION lockHook;
};

// Global context
extern LIBDOLL_CTX ctx;

