#include "pch.h"
#include "PEDollMonitor.h"

#include "../libPuppet/libPuppet.h"
#include "../libPuppet/PuppetClientTCP.h"
#include "SetPrivilege.h"
#include "GetFileVersion.h"

void __cdecl TPuppet(void*);

MONITOR_CTX ctx;

void isr_sigint(int signalId)
{
    // FIXME: TerminateThread() will cause huge resource leaks
    // Should inform TPuppet
    //TerminateThread(ctx.hTPuppet, 0);

    if(ctx.hTPuppet != INVALID_HANDLE_VALUE)
        CloseHandle(ctx.hTPuppet);

    if(ctx.puppet)
        delete ctx.puppet;

    if(ctx.serverInfo)
        delete[] ctx.serverInfo;

    if(ctx.libDollPath)
        delete[] ctx.libDollPath;

    exit(signalId == SIGINT ? 0 : 1);
}

void MonPanic(const char* msg)
{
    std::cerr << msg << std::endl;
    isr_sigint(SIGTERM);
}

/*
// stdout is set to multi-byte mode after the first call to std::cout
// Using std::wcout or std::wcerr may cause trouble
void MonPanic(const wchar_t* msg)
{
    std::wcerr << msg << std::endl;
    isr_sigint(SIGTERM);
}
*/

int main(int argc, char* argv[])
{
    std::string version;
    if (!GetFileVersion(NULL, version))
        version = "???";

    std::cout << "PEDoll Monitor v" << version << " InDev" << std::endl << std::endl;

    // Pre-initialize all contexts for isr_sigint()
    ctx.puppet = NULL;
    ctx.hTPuppet = INVALID_HANDLE_VALUE;
    ctx.serverInfo = NULL;
    ctx.libDollPath = NULL;

    if (!SetPrivilege(GetCurrentProcess(), SE_DEBUG_NAME, TRUE))
    {
        std::cerr << "main(): SetPrivilege(SE_DEBUG_NAME, TRUE) failed, GetLastError() = " << GetLastError() << std::endl;
        MonPanic("main(): Debug privilege not held");
    }

    ctx.libDollPath = new wchar_t[MAX_PATH];
    wchar_t* pFilePart;
    if (!SearchPathW(NULL, L"libDoll.dll", NULL, MAX_PATH, ctx.libDollPath, &pFilePart))
    {
        std::cerr << "main(): SearchPathW() failed, GetLastError() = " << GetLastError() << std::endl;
        MonPanic("main(): libDoll.dll not found");
    }
    // SearchPathW() is, actually, not designed for searching a DLL for LoadLibrary()
    // See https://docs.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryw#security-remarks

    const char* serverInfo = NULL;
    std::string serverInfoInput;
    if (argc > 1)
    {
        // PEDollMonitor.exe $serverInfo
        serverInfo = argv[1];
    }
    else
    {
        std::cout << "Input connection string [127.0.0.1]: ";
        std::getline(std::cin, serverInfoInput);
        serverInfo = serverInfoInput.empty() ? "127.0.0.1" : serverInfoInput.c_str();
    }

    size_t serverInfoLen = strlen(serverInfo);
    ctx.serverInfo = new char[serverInfoLen + 1];
    strcpy_s(ctx.serverInfo, serverInfoLen + 1, serverInfo);

    uintptr_t hTPuppet = _beginthread(TPuppet, 0, NULL);
    if (hTPuppet == 0 || hTPuppet == -1) // these status means error occurred
    {
        MonPanic("main(): _beginthread(TPuppet) failed");
    }
    ctx.hTPuppet = (HANDLE)hTPuppet;

    // Register SIGINT (Ctrl-C) handler
    signal(SIGINT, isr_sigint);

    std::cout << "Initialization complete, press Ctrl-C to stop" << std::endl;

    // Suspend current thread
    Sleep(INFINITE);

    return 42; // Not reached
}

