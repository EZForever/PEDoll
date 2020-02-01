#include "pch.h"
#include "PEDollMonitor.h"

#include "../libPuppet/libPuppet.h"
#include "../libPuppet/PuppetClientTCP.h"

void __cdecl TPuppet(void*);

MONITOR_CTX ctx;

void MonPanic(const char* msg)
{
    std::cerr << msg << std::endl;
    exit(1);
}

void MonPanic(const wchar_t* msg)
{
    std::wcerr << msg << std::endl;
    exit(1);
}

void isr_sigint(int)
{
    // FIXME: TerminateThread() will cause huge resource leaks
    // Should inform TPuppet
    TerminateThread(ctx.hTPuppet, 0);

    CloseHandle(ctx.hTPuppet);

    exit(0);
}

int main(int argc, char* argv[])
{
    std::cout << "PEDoll Monitor InDev" << std::endl << std::endl;

    const char* serverInfo = NULL;
    if (argc > 1)
    {
        // PEDollMonitor.exe $serverInfo
        serverInfo = argv[1];
    }
    else
    {
        std::string serverInfoInput;
        std::cout << "Input connection string [127.0.0.1]: ";
        std::cin >> serverInfoInput;
        if (serverInfoInput.empty())
            serverInfoInput = "127.0.0.1";
        serverInfo = serverInfoInput.c_str();
    }

    uintptr_t hTPuppet = _beginthread(TPuppet, 0, (void*)serverInfo);
    if (hTPuppet == 0 || hTPuppet == -1) // these status means error occurred
    {
        MonPanic("main(): _beginthread(TPuppet) failed");
    }
    ctx.hTPuppet = (HANDLE)hTPuppet;

    // Register SIGINT (Ctrl-C) handler
    signal(SIGINT, isr_sigint);

    std::cout << "Initialization complete, press Ctrl-C to stop" << std::endl;

    while (true)
        Sleep(1000);

    return 42; // Not reached
}

