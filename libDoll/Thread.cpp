#pragma once
#include "pch.h"
#include "libDoll.h"

#include "Thread.h"

void DollThreadRegisterCurrent()
{
    ctx.dollThreads.emplace(ctx.pRealGetCurrentThreadId());
}

void DollThreadUnregisterCurrent()
{
    ctx.dollThreads.erase(ctx.pRealGetCurrentThreadId());
}

void DollThreadPanic(const char* msg)
{
    FatalAppExitA(0, msg);
}

void DollThreadPanic(const wchar_t* msg)
{
    FatalAppExitW(0, msg);
}

void DollThreadSuspendAll(bool skipDollThreads)
{
    if (!ctx.suspendedThreads.empty())
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
        if(thIter == thSelf || (skipDollThreads && (ctx.dollThreads.find(thIter) != ctx.dollThreads.end())))
            continue;

        HANDLE hThIter = OpenThread(THREAD_SUSPEND_RESUME, FALSE, thIter);
        if (!hThIter)
            continue;

        SuspendThread(hThIter);
        ctx.suspendedThreads.emplace(hThIter);
    } while (Thread32Next(hSnapshot, &thEntry));

    CloseHandle(hSnapshot);
}

void DollThreadResumeAll()
{
    for (auto iter = ctx.suspendedThreads.begin(); iter != ctx.suspendedThreads.end(); iter++)
    {
        ResumeThread(*iter);
        CloseHandle(*iter);
    }
    ctx.suspendedThreads.clear();
}

