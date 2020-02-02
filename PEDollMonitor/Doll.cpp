#include "pch.h"
#include "PEDollMonitor.h"

#include "Doll.h"
#include "../libPuppet/libPuppet.h"
#include "../Detours/repo/src/detours.h"

uint32_t MonDollAttach(uint32_t pid)
{
    uint32_t ret = 0;

    HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
    if (!hProc)
    {
        return GetLastError();
    }
    // CreateRemoteThread() requires so many permissions

    size_t libDollPathSize = sizeof(wchar_t) * (wcslen(ctx.libDollPath) + 1);
    LPVOID libDollPath = NULL;
    HANDLE hTNew = NULL;
    if ((libDollPath = VirtualAllocEx(hProc, NULL, libDollPathSize, MEM_COMMIT, PAGE_READWRITE))
        && WriteProcessMemory(hProc, libDollPath, ctx.libDollPath, libDollPathSize, &libDollPathSize)
        && DetourCopyPayloadToProcess(hProc, Puppet::PAYLOAD_SERVER_INFO, ctx.serverInfo, strlen(ctx.serverInfo) + 1)
        && (hTNew = CreateRemoteThread(hProc, NULL, 0, (LPTHREAD_START_ROUTINE)LoadLibraryW, libDollPath, 0, NULL)))
    {
        CloseHandle(hTNew);
    }
    else
    {
        ret = GetLastError();
    }

    // That VirtualAllocEx()'ed memory region is leaked, but whatever

    CloseHandle(hProc);
    return ret;
}

uint32_t MonDollLaunch(wchar_t* path)
{
    uint32_t ret = 0;

    size_t libDollPathSize = wcslen(ctx.libDollPath);
    char* libDollPath = new char[libDollPathSize + 1];
    wcstombs_s(&libDollPathSize, libDollPath, libDollPathSize + 1, ctx.libDollPath, libDollPathSize);

    STARTUPINFOW si = { sizeof(STARTUPINFOW) };
    PROCESS_INFORMATION pi = { 0 };
    if (!DetourCreateProcessWithDllEx(NULL, path, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NEW_CONSOLE | CREATE_UNICODE_ENVIRONMENT, NULL, NULL, &si, &pi, libDollPath, NULL))
    {
        ret = GetLastError();
        delete[] libDollPath;
        return ret;
    }
    delete[] libDollPath;

    if (!DetourCopyPayloadToProcess(pi.hProcess, Puppet::PAYLOAD_SERVER_INFO, ctx.serverInfo, strlen(ctx.serverInfo) + 1))
    {
        ret = GetLastError();
        TerminateProcess(pi.hProcess, 9);
    }
    else
    {
        ResumeThread(pi.hThread);
    }

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    return ret;
}