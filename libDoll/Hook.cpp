#include "pch.h"
#include "libDoll.h"
#include "HookStub.h"
#include "Hook.h"

#include "../Detours/repo/src/detours.h"

void HookAllocBeforeStub(char* &pBefore, DWORD &pBeforeProtect, NATIVEWORD hookOEP, char* target)
{
    // NOTE: VirtualProtect() works on memory pages, not bytes
    //       If protection above is set to PAGE_EXECUTE_READ, the memory allocation below will fail
    //       due to no write permission to the memory page

    pBefore = new char[HookStubBefore_len];
    memcpy(pBefore, &HookStubBefore, HookStubBefore_len);
    *(NATIVEWORD*)(pBefore + HookStubBefore_HookOEPOffset) = hookOEP;
    *(NATIVEWORD*)(pBefore + HookStubBefore_AddrOffset) = (NATIVEWORD)target;
    VirtualProtect(pBefore, HookStubBefore_len, PAGE_EXECUTE_READWRITE, &pBeforeProtect);
}

void HookFreeBeforeStub(char* &pBefore, DWORD& pBeforeProtect)
{
    VirtualProtect(pBefore, HookStubBefore_len, pBeforeProtect, &pBeforeProtect);
    delete[] pBefore;
    pBefore = NULL;
}

void HookAdd(NATIVEWORD hookOEP, NATIVEWORD denySPOffset, NATIVEWORD denyAX)
{
    if (ctx.dollHooks.find(hookOEP) != ctx.dollHooks.end())
        return;

    DetourTransactionBegin();

    // Is this necessary?
    DetourUpdateThread(GetCurrentThread());

    LIBDOLL_HOOK* hook = new LIBDOLL_HOOK;

    HookAllocBeforeStub(hook->pBeforeA, hook->pBeforeAProtect, hookOEP, &HookStubA);
    HookAllocBeforeStub(hook->pBeforeB, hook->pBeforeBProtect, hookOEP, &HookStubB);
    HookAllocBeforeStub(hook->pBeforeDeny, hook->pBeforeDenyProtect, hookOEP, &HookStubOnDeny);

    hook->denySPOffset = denySPOffset;
    hook->denyAX = denyAX;

    ctx.dollHooks.emplace(std::make_pair(hookOEP, hook));

    hook->pTrampoline = hookOEP;
    DetourAttach((void**)&hook->pTrampoline, hook->pBeforeA);

    if (hookOEP == (NATIVEWORD)GetCurrentThreadId)
        ctx.pRealGetCurrentThreadId = (GET_CURRENT_THREAD_ID)hook->pTrampoline;

    DetourTransactionCommit();
}

void HookRemove(NATIVEWORD hookOEP)
{
    auto hookIter = ctx.dollHooks.find(hookOEP);
    if (hookIter == ctx.dollHooks.end())
        return;

    DetourTransactionBegin();

    // Is this necessary?
    DetourUpdateThread(GetCurrentThread());

    LIBDOLL_HOOK* hook = hookIter->second;

    DetourDetach((void**)&hook->pTrampoline, hook->pBeforeA);
    
    if (hookOEP == (NATIVEWORD)GetCurrentThreadId)
        ctx.pRealGetCurrentThreadId = GetCurrentThreadId;

    ctx.dollHooks.erase(hookIter);

    HookFreeBeforeStub(hook->pBeforeDeny, hook->pBeforeDenyProtect);
    HookFreeBeforeStub(hook->pBeforeB, hook->pBeforeBProtect);
    HookFreeBeforeStub(hook->pBeforeA, hook->pBeforeAProtect);

    delete hook;

    DetourTransactionCommit();
}