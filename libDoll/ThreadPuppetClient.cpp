#include "pch.h"
#include "libDoll.h"
#include "HookStub.h"

#include "../Detours/repo/src/detours.h"
#include "../libPuppetProtocol/libPuppetProtocol.h"
#include "../libPuppetProtocol/PuppetClientTCP.h"

void __cdecl ThreadPuppetClient(void*)
{
    DollThreadRegisterCurrent();

    // TODO: ALL THE THINGS
    unsigned long HookOEP = (unsigned long)system;

    if (ctx.dollHooks.find(HookOEP) != ctx.dollHooks.end())
        return;

    DetourTransactionBegin();
    
    // Is this necessary?
    DetourUpdateThread(GetCurrentThread());

    LIBDOLL_HOOK* hook = new LIBDOLL_HOOK;

    InitializeCriticalSection(&hook->lock);

    hook->pBeforeA = new char[HookStubBefore_len];
    memcpy(hook->pBeforeA, &HookStubBefore, HookStubBefore_len);
    *(unsigned long*)(hook->pBeforeA + HookStubBefore_HookOEPOffset) = HookOEP;
    *(unsigned long*)(hook->pBeforeA + HookStubBefore_AddrOffset) = (unsigned long)&HookStubA;
    VirtualProtect(hook->pBeforeA, HookStubBefore_len, PAGE_EXECUTE_READWRITE, &hook->pBeforeAProtect);
    
    hook->pBeforeB = new char[HookStubBefore_len];
    memcpy(hook->pBeforeB, &HookStubBefore, HookStubBefore_len);
    *(unsigned long*)(hook->pBeforeB + HookStubBefore_HookOEPOffset) = HookOEP;
    *(unsigned long*)(hook->pBeforeB + HookStubBefore_AddrOffset) = (unsigned long)&HookStubB;
    VirtualProtect(hook->pBeforeB, HookStubBefore_len, PAGE_EXECUTE_READWRITE, &hook->pBeforeBProtect);

    hook->pBeforeDeny = new char[HookStubBefore_len];
    memcpy(hook->pBeforeDeny, &HookStubBefore, HookStubBefore_len);
    *(unsigned long*)(hook->pBeforeDeny + HookStubBefore_HookOEPOffset) = HookOEP;
    *(unsigned long*)(hook->pBeforeDeny + HookStubBefore_AddrOffset) = (unsigned long)&HookStubOnDeny;
    VirtualProtect(hook->pBeforeDeny, HookStubBefore_len, PAGE_EXECUTE_READWRITE, &hook->pBeforeDenyProtect);

    hook->denySPOffset = 0;
    hook->denyAX = 42;

    ctx.dollHooks.emplace(std::make_pair(HookOEP, hook));

    hook->pTrampoline = HookOEP;
    DetourAttach((void**)&hook->pTrampoline, hook->pBeforeA);
    DetourTransactionCommit();
}

