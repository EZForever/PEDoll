#include "pch.h"
#include "libDoll.h"

#include "../Detours/repo/src/detours.h"
#include "../libPuppet/libPuppet.h"
#include "../libPuppet/PuppetClientTCP.h"
#include "Thread.h"
#include "Hook.h"
#include "BoyerMoore.h"

// Registers saved by the pushad instruction
#ifdef _WIN64
#   define PUSHAD_COUNT 10
#else
#   define PUSHAD_COUNT 8
#endif
// This should really be in HookStub_*.asm as a constant
// But for some reason I can't get it compiled
//#include "HookStub.h"

inline void TPuppetSendAck(uint32_t status)
{
    ctx.puppet->send(Puppet::PACKET_ACK(status));
}

inline void TPuppetSendInteger(uint64_t data)
{
    ctx.puppet->send(Puppet::PACKET_INTEGER(data));
}

template<class TPacket, Puppet::PACKET_TYPE type>
inline TPacket* TPuppetExpect()
{
    Puppet::PACKET* packet = ctx.puppet->recv();
    if (packet->type != type)
        throw std::runtime_error("TPuppetExpect(): Packet type mismatch");
    return (TPacket*)packet;
}

UINT_PTR TPuppetOEPFromString(wchar_t* strOEP)
{
    size_t strSize = wcslen(strOEP);
    char* str = new char[strSize + 1];
    wcstombs_s(&strSize, str, strSize + 1, strOEP, strSize);
    UINT_PTR ret = 0;

    char* pSpr = strchr(str, '!');
    if (pSpr)
    {
        // str == "$module!$proc"
        *pSpr++ = 0;
        ret = (UINT_PTR)DetourCodeFromPointer(DetourFindFunction(str, pSpr), NULL);
    }
    else
    {
        // str == "$proc"
        MODULEENTRY32 modEntry;
        modEntry.dwSize = sizeof(MODULEENTRY32);

        HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, 0);
        if (hSnapshot == INVALID_HANDLE_VALUE)
            return 0;

        if (!Module32First(hSnapshot, &modEntry))
        {
            CloseHandle(hSnapshot);
            return 0;
        }

        char* modPath = new char[MAX_PATH];
        size_t modPathSize;
        do
        {
            modPathSize = MAX_PATH - 1;
            wcstombs_s(&modPathSize, modPath, modPathSize + 1, modEntry.szExePath, modPathSize);
            ret = (UINT_PTR)DetourCodeFromPointer(DetourFindFunction(modPath, str), NULL);
            if (ret)
                break;
        } while (Module32Next(hSnapshot, &modEntry));

        delete[] modPath;
        CloseHandle(hSnapshot);
    }

    delete[] str;
    return ret;
}

UINT_PTR TPuppetOEPFromBinary(unsigned char* data, uint32_t dataLen)
{
    // BoyerMoore searcher cannot work if given pattern is empty
    if (!dataLen)
        return 0;

    // Initialize searcher
    BoyerMoore searcher((const char*)data, dataLen);
    
    // Get the bound for application's virtual memory
    SYSTEM_INFO si;
    GetSystemInfo(&si);

    // Enum through all memory regions
    // Credit: https://stackoverflow.com/a/4035387
    MEMORY_BASIC_INFORMATION mi;
    for (void* pMem = si.lpMinimumApplicationAddress; pMem < si.lpMaximumApplicationAddress; pMem = (void*)((UINT_PTR)mi.BaseAddress + mi.RegionSize))
    {
        if (!VirtualQuery(pMem, &mi, sizeof(MEMORY_BASIC_INFORMATION)))
            break;

        // Skip non-accessible regions
        if (!(mi.State & MEM_COMMIT) || (mi.Protect & PAGE_NOACCESS))
            continue;

        // Skip non-execuable regions (since we're looking for code)
        if(!(mi.Protect & (PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE | PAGE_EXECUTE_WRITECOPY)))
            continue;

        // Perform Boyer-Moore search
        UINT_PTR ret = (UINT_PTR)searcher.search((const char*)mi.BaseAddress, mi.RegionSize);
        if (ret)
            return ret;
    }

    return 0;
}

// The thread for async DLL loading (CMD_LOADDLL)
void __cdecl TPuppetLoadDll(void* arg)
{
    wchar_t* str = (wchar_t*)arg;
    LoadLibraryW(str);
    delete[] str;
}

// Called on received the 1st packet of a packet series
void TPuppetOnRecv(Puppet::PACKET* packet)
{
    bool isHooked = DollHookIsHappened();
    LIBDOLL_HOOK* hook = NULL;
    if (isHooked)
    {
        hook = ctx.dollHooks.find(ctx.waitingHookOEP)->second;

        switch (packet->type)
        {
        case Puppet::PACKET_TYPE::CMD_CONTEXT:
        {
            uint32_t idx = ((Puppet::PACKET_CMD_CONTEXT*)packet)->idx;
            if (idx >= PUSHAD_COUNT)
            {
                // Register index not valid
                TPuppetSendAck(1);
            }
            else
            {
                TPuppetSendAck(0);

                // 0 -> -1, 1 -> -2, ...
                TPuppetSendInteger(*(hook->context - idx - 1));
            }
            break;
        }
        case Puppet::PACKET_TYPE::CMD_MEMORY:
        {
            auto pktVA = TPuppetExpect<Puppet::PACKET_INTEGER, Puppet::PACKET_TYPE::INTEGER>();
            unsigned char* pBuf = (unsigned char*)pktVA->data;
            Puppet::PacketFree(pktVA);

            uint32_t size = ((Puppet::PACKET_CMD_MEMORY*)packet)->len;
            while (size && IsBadReadPtr(pBuf, size))
                size--;

            if (size)
            {
                auto pktReply = Puppet::PacketAllocBinary(pBuf, size);
                TPuppetSendAck(size);
                ctx.puppet->send(*pktReply);
                Puppet::PacketFree(pktReply);
            }
            else
            {
                TPuppetSendAck(0);
            }

            break;
        }
        case Puppet::PACKET_TYPE::CMD_VERDICT:
        {
            hook->verdict = ((Puppet::PACKET_CMD_VERDICT*)packet)->verdict;
            if (hook->verdict == 1)
            {
                auto pktSPOffset = TPuppetExpect<Puppet::PACKET_INTEGER, Puppet::PACKET_TYPE::INTEGER>();
                auto pktAX = TPuppetExpect<Puppet::PACKET_INTEGER, Puppet::PACKET_TYPE::INTEGER>();
                hook->denySPOffset = (UINT_PTR)pktSPOffset->data;
                hook->denyAX = (UINT_PTR)pktAX->data;
                Puppet::PacketFree(pktSPOffset);
                Puppet::PacketFree(pktAX);
            }
            SetEvent(ctx.hEvtHookVerdict);
            TPuppetSendAck(0);
            break;
        }
        case Puppet::PACKET_TYPE::ACK:
            // Should be the reply to MSG_ONHOOK. Do nothing
            break;
        default:
            // A unknown packet to Doll being sent
            TPuppetSendAck(-1);
        }
    }
    else
    {
        switch (packet->type)
        {
        case Puppet::PACKET_TYPE::CMD_END:
        {
            TPuppetSendAck(0);
            DebugBreak();
            break;
        }
        case Puppet::PACKET_TYPE::CMD_HOOK:
        {
            UINT_PTR hookOEP = 0;
            switch (((Puppet::PACKET_CMD_HOOK*)packet)->method)
            {
            case 0:
            {
                auto pktString = TPuppetExpect<Puppet::PACKET_STRING, Puppet::PACKET_TYPE::STRING>();
                hookOEP = TPuppetOEPFromString(pktString->data);
                Puppet::PacketFree(pktString);
                break;
            }
            case 1:
            {
                auto pktBinary = TPuppetExpect<Puppet::PACKET_BINARY, Puppet::PACKET_TYPE::BINARY>();
                hookOEP = TPuppetOEPFromBinary(pktBinary->data, pktBinary->size - sizeof(Puppet::PACKET_BINARY));
                Puppet::PacketFree(pktBinary);
                break;
            }
            case 2:
            {
                auto pktInteger = TPuppetExpect<Puppet::PACKET_INTEGER, Puppet::PACKET_TYPE::INTEGER>();
                hookOEP = (UINT_PTR)pktInteger->data;
                Puppet::PacketFree(pktInteger);
                break;
            }
            default:
                ;
            }
            if (hookOEP && !IsBadCodePtr((FARPROC)hookOEP))
            {
                DollHookAdd(hookOEP);
                TPuppetSendAck(0);
                TPuppetSendInteger(hookOEP);
            }
            else
            {
                TPuppetSendAck(1);
            }
            break;
        }
        case Puppet::PACKET_TYPE::CMD_UNHOOK:
        {
            UINT_PTR hookOEP = 0;
            auto pktInteger = TPuppetExpect<Puppet::PACKET_INTEGER, Puppet::PACKET_TYPE::INTEGER>();
            hookOEP = (UINT_PTR)pktInteger->data;
            Puppet::PacketFree(pktInteger);
            if (hookOEP && !IsBadCodePtr((FARPROC)hookOEP))
            {
                DollHookRemove(hookOEP);
                TPuppetSendAck(0);
            }
            else
            {
                TPuppetSendAck(1);
            }
            break;
        }
        case Puppet::PACKET_TYPE::CMD_BREAK:
        {
            if (ctx.suspendedThreads.size())
            {
                DollThreadResumeAll();
            }
            else
            {
                DollThreadSuspendAll(true);
            }
        }
        case Puppet::PACKET_TYPE::CMD_LOADDLL:
        {
            auto pktString = TPuppetExpect<Puppet::PACKET_STRING, Puppet::PACKET_TYPE::STRING>();
            wchar_t* pStr = new wchar_t[wcslen(pktString->data) + 1];
            wcscpy_s(pStr, wcslen(pktString->data) + 1, pktString->data);
            Puppet::PacketFree(pktString);

            uintptr_t hTNew = _beginthread(TPuppetLoadDll, 0, pStr);
            if (hTNew == 0 || hTNew == -1)
            {
                delete[] pStr;
                TPuppetSendAck(GetLastError());
            }
            else
            {
                CloseHandle((HANDLE)hTNew);
                TPuppetSendAck(0);
            }

            break;
        }
        default:
            // A unknown packet to Doll being sent
            TPuppetSendAck(-1);
        }
    }
}

void __cdecl TPuppet(void* arg)
{
    DollThreadRegisterCurrent();

    // Initialize ctx.puppet
    try {
        ctx.puppet = Puppet::ClientTCPInitialize((const char*)arg);
    }
    catch (const std::runtime_error & e) {
        DollThreadPanic(e.what());
    }
    if (!ctx.puppet)
        DollThreadPanic(L"TPuppet(): ClientTCPInitialize() failed");

    // Prepare MSG_ONLINE packet & current process name
    Puppet::PACKET_MSG_ONLINE packetOnline;
    packetOnline.isMonitor = 0;
    packetOnline.bits = sizeof(UINT_PTR) * 8;
    packetOnline.pid = GetCurrentProcessId();

    wchar_t* baseName = new wchar_t[MAX_PATH];
    GetModuleBaseNameW(GetCurrentProcess(), NULL, baseName, MAX_PATH);
    Puppet::PACKET_STRING* packetString = Puppet::PacketAllocString(baseName);
    delete[] baseName;

    // Send the packet & wait for reply
    try {
        ctx.puppet->send(packetOnline);
        ctx.puppet->send(*packetString);
        auto packetAck = TPuppetExpect<Puppet::PACKET_ACK, Puppet::PACKET_TYPE::ACK>();
        Puppet::PacketFree(packetAck);
    }
    catch (const std::runtime_error & e) {
        DollThreadPanic(e.what());
    }
    Puppet::PacketFree(packetString);

    // Main loop
    Puppet::PACKET* packet = NULL;
    try {
        while (true)
        {
            packet = ctx.puppet->recv();
            TPuppetOnRecv(packet);
            Puppet::PacketFree(packet);
            packet = NULL;
        }
    }
    catch (const std::runtime_error & e) {
        // FIXME: Should end current thread/process peacefully
        DollThreadPanic(e.what());
    }
}