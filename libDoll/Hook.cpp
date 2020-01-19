#include "pch.h"
#include "libDoll.h"
#include "HookStub.h"
#include "Hook.h"

#include "../Detours/repo/src/detours.h"

void HookAllocBeforeStub(char* &pBefore, DWORD &pBeforeProtect, UINT_PTR hookOEP, char* target)
{
    // NOTE: VirtualProtect() works on memory pages, not bytes
    //       If protection above is set to PAGE_EXECUTE_READ, the memory allocation below will fail
    //       due to no write permission to the memory page

    pBefore = new char[HookStubBefore_len];
    memcpy(pBefore, &HookStubBefore, HookStubBefore_len);
    *(UINT_PTR*)(pBefore + HookStubBefore_HookOEPOffset) = hookOEP;
    *(UINT_PTR*)(pBefore + HookStubBefore_AddrOffset) = (UINT_PTR)target;
    VirtualProtect(pBefore, HookStubBefore_len, PAGE_EXECUTE_READWRITE, &pBeforeProtect);
}

void HookFreeBeforeStub(char* &pBefore, DWORD& pBeforeProtect)
{
    VirtualProtect(pBefore, HookStubBefore_len, pBeforeProtect, &pBeforeProtect);
    delete[] pBefore;
    pBefore = NULL;
}

// DetourUpdateThread(GetCurrentThread()) is not necessary
// but a call to DetourUpdateThread() for each thread is required
void HookUpdateAllThreads(std::set<HANDLE> &updatedThreads)
{
    if (!updatedThreads.empty())
        return;

    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
    if (hSnapshot == INVALID_HANDLE_VALUE)
        return;

    DWORD thSelf = ctx.pRealGetCurrentThreadId();
    THREADENTRY32 thEntry;
    thEntry.dwSize = sizeof(THREADENTRY32);

    if (!Thread32First(hSnapshot, &thEntry))
    {
        CloseHandle(hSnapshot);
        return;
    }

    do
    {
        DWORD thIter = thEntry.th32ThreadID;
        if (thIter == thSelf)
            continue;

        HANDLE hThIter = OpenThread(THREAD_SUSPEND_RESUME, FALSE, thIter);
        if (!hThIter)
            continue;

        DetourUpdateThread(hThIter);
        updatedThreads.emplace(hThIter);
    } while (Thread32Next(hSnapshot, &thEntry));

    CloseHandle(hSnapshot);
}

void HookEndUpdateAllThreads(std::set<HANDLE>& updatedThreads)
{
    for (auto iter = updatedThreads.begin(); iter != updatedThreads.end(); iter++)
    {
        CloseHandle(*iter);
    }
    updatedThreads.clear();
}

void HookAdd(UINT_PTR hookOEP, UINT_PTR denySPOffset, UINT_PTR denyAX)
{
    if (ctx.dollHooks.find(hookOEP) != ctx.dollHooks.end())
        return;

    DetourTransactionBegin();

    std::set<HANDLE> updatedThreads;
    HookUpdateAllThreads(updatedThreads);

    LIBDOLL_HOOK* hook = new LIBDOLL_HOOK;

    HookAllocBeforeStub(hook->pBeforeA, hook->pBeforeAProtect, hookOEP, &HookStubA);
    HookAllocBeforeStub(hook->pBeforeB, hook->pBeforeBProtect, hookOEP, &HookStubB);
    HookAllocBeforeStub(hook->pBeforeDeny, hook->pBeforeDenyProtect, hookOEP, &HookStubOnDeny);

    hook->denySPOffset = denySPOffset;
    hook->denyAX = denyAX;

    ctx.dollHooks.emplace(std::make_pair(hookOEP, hook));

    hook->pTrampoline = hookOEP;
    DetourAttach((void**)&hook->pTrampoline, hook->pBeforeA);

    if (hookOEP == (UINT_PTR)GetCurrentThreadId)
        ctx.pRealGetCurrentThreadId = (GET_CURRENT_THREAD_ID)hook->pTrampoline;

    DetourTransactionCommit();
    HookEndUpdateAllThreads(updatedThreads);
}

void HookRemove(UINT_PTR hookOEP)
{
    auto hookIter = ctx.dollHooks.find(hookOEP);
    if (hookIter == ctx.dollHooks.end())
        return;

    DetourTransactionBegin();

    std::set<HANDLE> updatedThreads;
    HookUpdateAllThreads(updatedThreads);

    LIBDOLL_HOOK* hook = hookIter->second;

    DetourDetach((void**)&hook->pTrampoline, hook->pBeforeA);
    
    if (hookOEP == (UINT_PTR)GetCurrentThreadId)
        ctx.pRealGetCurrentThreadId = GetCurrentThreadId;

    ctx.dollHooks.erase(hookIter);

    HookFreeBeforeStub(hook->pBeforeDeny, hook->pBeforeDenyProtect);
    HookFreeBeforeStub(hook->pBeforeB, hook->pBeforeBProtect);
    HookFreeBeforeStub(hook->pBeforeA, hook->pBeforeAProtect);

    delete hook;

    DetourTransactionCommit();
    HookEndUpdateAllThreads(updatedThreads);
}