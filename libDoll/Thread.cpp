#pragma once
#include "pch.h"
#include "libDoll.h"

#include "Thread.h"

std::set<HANDLE> suspendedThreads;

void DollThreadRegisterCurrent()
{
    ctx.dollThreads.emplace(ctx.pRealGetCurrentThreadId());
}

void DollThreadUnregisterCurrent()
{
    ctx.dollThreads.erase(ctx.pRealGetCurrentThreadId());
}

void DollThreadSuspendAll(bool skipDollThreads)
{
    if (!suspendedThreads.empty())
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
        suspendedThreads.emplace(hThIter);
    } while (Thread32Next(hSnapshot, &thEntry));

    CloseHandle(hSnapshot);
}

void DollThreadResumeAll()
{
    for (auto iter = suspendedThreads.begin(); iter != suspendedThreads.end(); iter++)
    {
        ResumeThread(*iter);
        CloseHandle(*iter);
    }
    suspendedThreads.clear();
}

