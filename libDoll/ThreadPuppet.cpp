#include "pch.h"
#include "libDoll.h"

#include "../libPuppet/libPuppet.h"
#include "../libPuppet/PuppetClientTCP.h"
#include "Thread.h"
#include "Hook.h"

Puppet::PuppetClientTCP* TPuppetInitializeClientTCP(const wchar_t* serverInfo)
{
    size_t serverInfoSize = wcslen(serverInfo);
    char* str = new char[serverInfoSize + 1];
    wcstombs_s(&serverInfoSize, str, serverInfoSize + 1, serverInfo, serverInfoSize);
    
    int port = PUPPET_PORT;
    bool ipv6 = false;
    char* pSpr = strrchr(str, '.');
    if(pSpr)
    {
        // If '.' is found, this must be an IPv4 address
        pSpr = strrchr(str, ':');
        if (pSpr)
        {
            // serverInfo == L"$host:$port"
            *pSpr++ = 0;
            port = strtol(pSpr, NULL, 10);
        }
        // Otherwise serverInfo == L"$host"
    }
    else
    {
        ipv6 = true;
        pSpr = strrchr(str, ']');
        if (pSpr)
        {
            // serverInfo == L"[$v6host]:$port"
            *pSpr++ = 0; // Remove ']'
            memmove(str, str + 1, strlen(str) - 1); // Remove '['
            *pSpr++ = 0; // Remove ':'
            port = strtol(pSpr, NULL, 10);
        }
        // Otherwise serverInfo == L"$v6host"
    }

    Puppet::PuppetClientTCP* puppet = NULL;
    try {
        puppet = new Puppet::PuppetClientTCP(port, str, ipv6);
        puppet->start();
    }
    catch (const std::runtime_error & e) {
        DollThreadPanic(e.what());
        delete puppet;
        puppet = NULL;
    }

    delete[] str;
    return puppet;
}

void TPuppetSendAck(uint32_t status)
{
    Puppet::PACKET_ACK packet(status);
    ctx.puppet->send(packet);
}

template<class TPacket, Puppet::PACKET_TYPE type>
TPacket* TPuppetExpect()
{
    Puppet::PACKET* packet = ctx.puppet->recv();
    if (packet->type != type)
        throw std::runtime_error("TPuppetExpect(): Packet type mismatch");
    return (TPacket*)packet;
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

        }
        case Puppet::PACKET_TYPE::CMD_VERDICT:
        {
            hook->verdict = ((Puppet::PACKET_CMD_VERDICT*)packet)->verdict;
            if (hook->verdict == 1)
            {
                auto packetSPOffset = TPuppetExpect<Puppet::PACKET_INTEGER, Puppet::PACKET_TYPE::INTEGER>();
                auto packetAX = TPuppetExpect<Puppet::PACKET_INTEGER, Puppet::PACKET_TYPE::INTEGER>();
                hook->denySPOffset = packetSPOffset->data;
                hook->denyAX = packetAX->data;
                Puppet::PacketFree(packetSPOffset);
                Puppet::PacketFree(packetAX);
            }
            TPuppetSendAck(0);
        }
            // TODO
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
            // TODO
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
    ctx.puppet = TPuppetInitializeClientTCP(ctx.pServerInfo->data);
    if (!ctx.puppet)
        DollThreadPanic(L"TPuppet(): TPuppetInitializeClientTCP() failed");

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
            Puppet::PACKET* packet = ctx.puppet->recv();
            TPuppetOnRecv(packet);
            Puppet::PacketFree(packet);
            packet = NULL;
        }
    }
    catch (const std::runtime_error & e) {
        // TODO: Process e.what() && end current thread/process peacefully
    }
}