#include "pch.h"
#include "PuppetClientTCP.h"

namespace Puppet {

#   define ASSERT(expr, msg) assert((expr), "PuppetClientTCP::" msg)

    void PuppetClientTCP::assert(bool expr, const char* msg) {
        if (expr)
            return;

        lastError = WSAGetLastError();
        throw std::runtime_error(msg);
    }

    PuppetClientTCP::PuppetClientTCP(int port, const char* host, bool ipv6)
        : clientSocket(INVALID_SOCKET)
    {
        int ret;

        // According to MSDN, multiple calls to WSAStartup() is fine, as long as keep balance with WSACleanup()
        // Calling WSAStartup() inside of DllMain() is not recommended though,
        // since WSAStartup() may load other DLLs and may cause deadlocks
        ret = WSAStartup(MAKEWORD(2, 2), &wsa);
        ASSERT(!ret, "(): WSAStartup() failed");

        clientSocket = socket(ipv6 ? AF_INET6 : AF_INET, SOCK_STREAM, 0);
        ASSERT(clientSocket != INVALID_SOCKET, "(): socket() failed");

        if (ipv6)
        {
            SOCKADDR_IN6* a = new SOCKADDR_IN6{ 0 };
            a->sin6_family = AF_INET6;
            a->sin6_port = htons(port);

            if (!host)
                host = "::1";
            ret = inet_pton(AF_INET6, host, &a->sin6_addr);
            ASSERT(ret, "(): Invalid host");

            addr = (SOCKADDR*)a;
            addrSize = sizeof(*a);
        }
        else
        {
            SOCKADDR_IN* a = new SOCKADDR_IN{ 0 };
            a->sin_family = AF_INET;
            a->sin_port = htons(port);

            if (!host)
                host = "127.0.0.1";
            ret = inet_pton(AF_INET, host, &a->sin_addr);
            ASSERT(ret, "(): Invalid host");
            
            addr = (SOCKADDR*)a;
            addrSize = sizeof(*a);
        }
    }

    PuppetClientTCP::~PuppetClientTCP()
    {
        if (clientSocket != INVALID_SOCKET) {
            shutdown(clientSocket, SD_BOTH);
            closesocket(clientSocket);
        }

        delete addr; // XXX: Will type confusion cause something here?
        WSACleanup();

        // __declspec(nothrow) is default on destructors - warning C2497
        //ASSERT(!WSACleanup(), "~(): WSACleanup() failed");
    }

    void PuppetClientTCP::start()
    {
        if (clientSocket == INVALID_SOCKET)
            return; // FIXME: How to tell if a connection has ended?

        int ret;

        ret = ::connect(clientSocket, addr, addrSize); // Blocks
        ASSERT(!ret, "connect(): connect() failed");
    }

    void PuppetClientTCP::send(const PACKET& packet)
    {
        ASSERT(clientSocket != INVALID_SOCKET, "send(): Connection not established");

        int ret;

        ret = ::send(clientSocket, (const char*)&packet, packet.size, 0);
        ASSERT(ret == packet.size, "send(): send() failed");
    }

    PACKET* PuppetClientTCP::recv()
    {
        ASSERT(clientSocket != INVALID_SOCKET, "recv(): Connection not established");

        int ret;

        // Get packet size first

        decltype(PACKET::size) size;
        ret = ::recv(clientSocket, (char*)&size, sizeof(size), 0); // Blocks
        ASSERT(ret == sizeof(size), "recv(): recv() failed");

        // Then get rest of the data

        char* packet = new char[size];
        ((PACKET*)packet)->size = size;

        ret = ::recv(clientSocket, packet + sizeof(size), size - sizeof(size), 0);
        ASSERT(ret == size - sizeof(size), "recv(): recv() failed");

        return (PACKET*)packet; // XXX: Will type confusion cause something here?
    }

}