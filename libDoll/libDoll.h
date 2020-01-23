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
    Puppet::PACKET_STRING* pServerInfo;

    std::set<DWORD> dollThreads;
    std::set<HANDLE> suspendedThreads;
    HANDLE hTJudger;
    HANDLE hTPuppet;

    std::map<UINT_PTR, LIBDOLL_HOOK*> dollHooks;
    HANDLE hEvtHookVerdict;
    UINT_PTR waitingHookOEP;
    CRITICAL_SECTION lockHook;

    GET_CURRENT_THREAD_ID pRealGetCurrentThreadId;

};

// GUID for DetourFindPayload(), representing the server info payload
// The payload is a libPuppet PACKET_STRING
// {A2062469-2B45-496D-8FE9-7E894ED72270}
extern const GUID PAYLOAD_SERVER_INFO;

// Default port for libPuppet
extern const int PUPPET_PORT;

// Global context
extern LIBDOLL_CTX ctx;

