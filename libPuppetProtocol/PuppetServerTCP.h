// PuppetServerTCP.h
// Forward TCP server for Puppet Protocol
#pragma once
#include "pch.h"
#include <WinSock2.h>
#include <WS2tcpip.h> // For some reason IPv6 things are here
#include "libPuppetProtocol.h"

namespace Puppet {

    // Forward TCP server for Puppet Protocol
    class PuppetServerTCP : public IPuppetServer
    {
    private:
        WSADATA wsa;
        SOCKADDR* addr;
        int addrSize;
        SOCKET serverSocket, clientSocket;

        static void assert(bool expr, const char* msg);

    public:
        PuppetServerTCP(int port, const char* host = NULL, bool ipv6 = false);
        ~PuppetServerTCP();

        // IPuppetServer implemention

        void listen();
        void send(const PACKET& packet);
        PACKET* recv();
    };

}
