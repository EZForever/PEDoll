#include "pch.h"
#include "PEDollMonitor.h"

#include "../libPuppet/libPuppet.h"
#include "../libPuppet/PuppetClientTCP.h"
#include "Proc.h"
#include "Doll.h"

// These are directly copied from libDoll/ThreadPuppet.cpp

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

void TPuppetOnRecv(Puppet::PACKET* packet)
{
    switch (packet->type)
    {
    case Puppet::PACKET_TYPE::CMD_END:
    {
        TPuppetSendAck(0);
        MonPanic("TPuppetOnRecv(): CMD_END received");
        break;
    }
    case Puppet::PACKET_TYPE::CMD_DOLL:
    {
        uint32_t pid = ((Puppet::PACKET_CMD_DOLL*)packet)->pid;
        uint32_t ret;
        if (pid)
        {
            ret = MonDollAttach(pid);
        }
        else
        {
            auto pktString = TPuppetExpect<Puppet::PACKET_STRING, Puppet::PACKET_TYPE::STRING>();
            ret = MonDollLaunch(pktString->data);
            Puppet::PacketFree(pktString);
        }
        TPuppetSendAck(ret);
        break;
    }
    case Puppet::PACKET_TYPE::CMD_PS:
    {
        std::vector<Puppet::PACKET*> entry;
        TPuppetSendAck(MonProcPs(entry));
        for (auto iter = entry.cbegin(); iter != entry.cend(); iter++)
        {
            ctx.puppet->send(**iter);
            Puppet::PacketFree(*iter);
        }
        break;
    }
    case Puppet::PACKET_TYPE::CMD_SHELL:
    {
        auto pktString = TPuppetExpect<Puppet::PACKET_STRING, Puppet::PACKET_TYPE::STRING>();
        TPuppetSendAck(MonProcShell(pktString->data));
        Puppet::PacketFree(pktString);
        break;
    }
    case Puppet::PACKET_TYPE::CMD_KILL:
    {
        uint32_t pid = ((Puppet::PACKET_CMD_KILL*)packet)->pid;
        uint32_t ret;
        if (pid)
        {
            ret = MonProcKillByPID(pid);
        }
        else
        {
            auto pktString = TPuppetExpect<Puppet::PACKET_STRING, Puppet::PACKET_TYPE::STRING>();
            ret = MonProcKillByName(pktString->data);
            Puppet::PacketFree(pktString);
        }
        TPuppetSendAck(ret);
        break;
    }
    default:
        // A unknown packet to Doll being sent
        TPuppetSendAck(-1);
    }
}

void __cdecl TPuppet(void* arg)
{
    // Initialize ctx.puppet
    try {
        ctx.puppet = Puppet::ClientTCPInitialize(ctx.serverInfo);
    }
    catch (const std::runtime_error & e) {
        MonPanic(e.what());
    }
    if (!ctx.puppet)
        MonPanic(L"TPuppet(): ClientTCPInitialize() failed");

    // Prepare MSG_ONLINE packet & current process name
    Puppet::PACKET_MSG_ONLINE packetOnline;
    packetOnline.isMonitor = 1;
    packetOnline.bits = sizeof(UINT_PTR) * 8;
    packetOnline.pid = GetCurrentProcessId();

    DWORD baseNameSize = MAX_COMPUTERNAME_LENGTH + 1;
    wchar_t* baseName = new wchar_t[baseNameSize];
    GetComputerNameW(baseName, &baseNameSize);
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
        MonPanic(e.what());
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
        MonPanic(e.what());
    }
}