// PuppetClientTCP.h
// Forward TCP Client for Puppet Protocol
#pragma once
#include "pch.h"
#include <WinSock2.h>
#include <WS2tcpip.h> // For some reason IPv6 things are here
#include "libPuppet.h"

namespace Puppet {

    // Forward TCP Client for Puppet Protocol
    class PuppetClientTCP : public IPuppet
    {
    private:
        WSADATA wsa;
        SOCKADDR* addr;
        int addrSize;
        SOCKET clientSocket;

        void assert(bool expr, const char* msg);

    public:
        PuppetClientTCP(int port, const char* host = NULL, bool ipv6 = false);
        ~PuppetClientTCP();

        // IPuppet implemention

        void send(const PACKET& packet);
        PACKET* recv();
    };

    // Construct a PuppetClientTCP instance from a serverInfo string
    // e.g. "127.0.0.1", "127.0.0.1:12345", "::1", "[::1]:12345"
    PuppetClientTCP* ClientTCPInitialize(const char* serverInfo);

}
