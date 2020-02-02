#include "pch.h"
#include "PEDollMonitor.h"

#include "Proc.h"
#include "../libPuppet/libPuppet.h"

uint32_t MonProcPs(std::vector<Puppet::PACKET*> &entry)
{
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hSnapshot == INVALID_HANDLE_VALUE)
        return GetLastError();

    PROCESSENTRY32W procEntry;
    procEntry.dwSize = sizeof(PROCESSENTRY32W);
    if (!Process32FirstW(hSnapshot, &procEntry))
    {
        CloseHandle(hSnapshot);
        return GetLastError();
    }

    do {
        entry.push_back(new Puppet::PACKET_INTEGER(procEntry.th32ProcessID));
        entry.push_back(Puppet::PacketAllocString(procEntry.szExeFile));
    } while (Process32NextW(hSnapshot, &procEntry));

    entry.push_back(new Puppet::PACKET_INTEGER(-1));

    CloseHandle(hSnapshot);
    return 0;
}

uint32_t MonProcShell(wchar_t* args)
{
    // Obtain the value of $COMSPEC, and fail if it does not exist
    wchar_t* comSpec = new wchar_t[MAX_PATH];
    size_t comSpecSize = MAX_PATH;
    if (_wgetenv_s(&comSpecSize, comSpec, comSpecSize, L"COMSPEC"))
    {
        delete[] comSpec;
        return GetLastError();
    }

    STARTUPINFOW si = { sizeof(STARTUPINFOW) };
    PROCESS_INFORMATION pi = { 0 };
    if (CreateProcessW(comSpec, args, NULL, NULL, FALSE, CREATE_NEW_CONSOLE | CREATE_UNICODE_ENVIRONMENT, NULL, NULL, &si, &pi))
    {
        delete[] comSpec;
        CloseHandle(pi.hProcess);
        CloseHandle(pi.hThread);
        return 0;
    }
    else
    {
        delete[] comSpec;
        return GetLastError();
    }
}

uint32_t MonProcKillByPID(uint32_t pid)
{
    HANDLE hProc = OpenProcess(PROCESS_TERMINATE, FALSE, pid);
    if (!hProc)
        return GetLastError();

    uint32_t ret = 0;
    if (!TerminateProcess(hProc, 9))
        ret = GetLastError();

    CloseHandle(hProc);
    return ret;
}

uint32_t MonProcKillByName(wchar_t* name)
{
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hSnapshot == INVALID_HANDLE_VALUE)
        return GetLastError();

    uint32_t ret = 0;
    PROCESSENTRY32W procEntry;
    procEntry.dwSize = sizeof(PROCESSENTRY32W);
    if (!Process32FirstW(hSnapshot, &procEntry))
    {
        ret = GetLastError();
        CloseHandle(hSnapshot);
        return ret;
    }

    do {
        if (_wcsicmp(procEntry.szExeFile, name))
            continue;

        ret = MonProcKillByPID(procEntry.th32ProcessID);
        if (ret)
            break;
    } while (Process32NextW(hSnapshot, &procEntry));

    CloseHandle(hSnapshot);
    return ret;
}