#include "pch.h"
#include "libDoll.h"

#include "Thread.h"
#include "Hook.h"

void __cdecl TJudger(void* arg)
{
    DollThreadRegisterCurrent();

    // TODO: Wait for OnHook / AddHook / RemoveHook events

}