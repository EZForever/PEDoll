// PuppetClientTCP.h
// Forward TCP Client for Puppet Protocol
#pragma once
#include "pch.h"
#include <WinSock2.h>
#include <WS2tcpip.h>
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

        static void assert(bool expr, const char* msg);

    public:
        PuppetClientTCP(int port, const char* host = NULL, bool ipv6 = false);
        ~PuppetClientTCP();

        // IPuppet implemention

        void start();
        void send(const PACKET& packet);
        PACKET* recv();
    };

}
