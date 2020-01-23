#include "pch.h"
#include "libDoll.h"
#include "HookStub.h"
#include "Hook.h"

#include "../Detours/repo/src/detours.h"

void DollHookAllocBeforeStub(char* &pBefore, DWORD &pBeforeProtect, UINT_PTR hookOEP, char* target)
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

void DollHookFreeBeforeStub(char* &pBefore, DWORD& pBeforeProtect)
{
    VirtualProtect(pBefore, HookStubBefore_len, pBeforeProtect, &pBeforeProtect);
    delete[] pBefore;
    pBefore = NULL;
}

// DetourUpdateThread(GetCurrentThread()) is not necessary
// but a call to DetourUpdateThread() for each thread is required
void DollHookUpdateAllThreads(std::set<HANDLE> &updatedThreads)
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

void DollHookEndUpdateAllThreads(std::set<HANDLE>& updatedThreads)
{
    for (auto iter = updatedThreads.begin(); iter != updatedThreads.end(); iter++)
    {
        CloseHandle(*iter);
    }
    updatedThreads.clear();
}

bool DollHookIsHappened()
{
    if (TryEnterCriticalSection(&ctx.lockHook))
    {
        LeaveCriticalSection(&ctx.lockHook);
        return false;
    }
    return true;
}

void DollHookAdd(UINT_PTR hookOEP, UINT_PTR denySPOffset, UINT_PTR denyAX)
{
    if (ctx.dollHooks.find(hookOEP) != ctx.dollHooks.end())
        return;

    DetourTransactionBegin();

    std::set<HANDLE> updatedThreads;
    DollHookUpdateAllThreads(updatedThreads);

    LIBDOLL_HOOK* hook = new LIBDOLL_HOOK;

    DollHookAllocBeforeStub(hook->pBeforeA, hook->pBeforeAProtect, hookOEP, &HookStubA);
    DollHookAllocBeforeStub(hook->pBeforeB, hook->pBeforeBProtect, hookOEP, &HookStubB);
    DollHookAllocBeforeStub(hook->pBeforeDeny, hook->pBeforeDenyProtect, hookOEP, &HookStubOnDeny);

    hook->denySPOffset = denySPOffset;
    hook->denyAX = denyAX;

    ctx.dollHooks.emplace(std::make_pair(hookOEP, hook));

    hook->pTrampoline = hookOEP;
    DetourAttach((void**)&hook->pTrampoline, hook->pBeforeA);

    if (hookOEP == (UINT_PTR)GetCurrentThreadId)
        ctx.pRealGetCurrentThreadId = (GET_CURRENT_THREAD_ID)hook->pTrampoline;

    DetourTransactionCommit();
    DollHookEndUpdateAllThreads(updatedThreads);
}

void DollHookRemove(UINT_PTR hookOEP)
{
    auto hookIter = ctx.dollHooks.find(hookOEP);
    if (hookIter == ctx.dollHooks.end())
        return;

    DetourTransactionBegin();

    std::set<HANDLE> updatedThreads;
    DollHookUpdateAllThreads(updatedThreads);

    LIBDOLL_HOOK* hook = hookIter->second;

    DetourDetach((void**)&hook->pTrampoline, hook->pBeforeA);
    
    if (hookOEP == (UINT_PTR)GetCurrentThreadId)
        ctx.pRealGetCurrentThreadId = GetCurrentThreadId;

    ctx.dollHooks.erase(hookIter);

    DollHookFreeBeforeStub(hook->pBeforeDeny, hook->pBeforeDenyProtect);
    DollHookFreeBeforeStub(hook->pBeforeB, hook->pBeforeBProtect);
    DollHookFreeBeforeStub(hook->pBeforeA, hook->pBeforeAProtect);

    delete hook;

    DetourTransactionCommit();
    DollHookEndUpdateAllThreads(updatedThreads);
}