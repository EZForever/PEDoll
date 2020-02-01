#include "pch.h"
#include "PEDollMonitor.h"

#include "Proc.h"

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