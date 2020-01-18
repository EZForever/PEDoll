#include "pch.h"
#include "PuppetServerTCP.h"

namespace Puppet {

#   define ASSERT(expr, msg) PuppetServerTCP::assert((expr), "PuppetServerTCP::" msg)

    void PuppetServerTCP::assert(bool expr, const char* msg) {
        if (expr)
            return;

        lastError = WSAGetLastError();
        throw std::runtime_error(msg);
    }

    PuppetServerTCP::PuppetServerTCP(int port, const char *host, bool ipv6)
        : serverSocket(INVALID_SOCKET), clientSocket(INVALID_SOCKET)
    {
        int ret;

        // According to MSDN, multiple calls to WSAStartup() is fine, as long as keep balance with WSACleanup()
        // Calling WSAStartup() inside of DllMain() is not recommended though,
        // since WSAStartup() may load other DLLs and may cause deadlocks
        ret = WSAStartup(MAKEWORD(2, 2), &wsa);
        ASSERT(!ret, "(): WSAStartup() failed");

        serverSocket = socket(ipv6 ? AF_INET6 : AF_INET, SOCK_STREAM, 0);
        ASSERT(serverSocket != INVALID_SOCKET, "(): socket() failed");

        if (ipv6)
        {
            SOCKADDR_IN6* a = new SOCKADDR_IN6{ 0 };
            a->sin6_family = AF_INET6;
            a->sin6_port = htons(port);

            if (host)
            {
                ret = inet_pton(AF_INET6, host, &a->sin6_addr);
                ASSERT(ret, "(): Invalid host");
            }
            else
            {
                a->sin6_addr = in6addr_any;
            }
            addr = (SOCKADDR*)a;
            addrSize = sizeof(*a);
        }
        else
        {
            SOCKADDR_IN* a = new SOCKADDR_IN{ 0 };
            a->sin_family = AF_INET;
            a->sin_port = htons(port);

            if (host)
            {
                ret = inet_pton(AF_INET, host, &a->sin_addr);
                ASSERT(ret, "(): Invalid host");
            }
            else
            {
                a->sin_addr.s_addr = INADDR_ANY;
            }
            addr = (SOCKADDR*)a;
            addrSize = sizeof(*a);
        }
        ret = bind(serverSocket, addr, addrSize);
        ASSERT(!ret, "(): bind() failed");
    }

    PuppetServerTCP::~PuppetServerTCP()
    {
        if (clientSocket != INVALID_SOCKET) {
            shutdown(clientSocket, SD_BOTH);
            closesocket(clientSocket);
        }
        
        shutdown(serverSocket, SD_BOTH);
        closesocket(serverSocket);

        delete addr; // XXX: Will type confusion cause something here?
        WSACleanup();

        // __declspec(nothrow) is default on destructors - warning C2497
        //ASSERT(!WSACleanup(), "~(): WSACleanup() failed");
    }

    void PuppetServerTCP::start()
    {
        if (clientSocket != INVALID_SOCKET)
            return; // FIXME: How to tell if a connection has ended?

        int ret;

        ret = ::listen(serverSocket, 1);
        ASSERT(!ret, "listen(): listen() failed");

        clientSocket = accept(serverSocket, addr, &addrSize); // Blocks
        ASSERT(clientSocket != INVALID_SOCKET, "listen(): accept() failed");

        // The listener socket can be closed, since we only have 1 client
        closesocket(serverSocket);
        serverSocket = INVALID_SOCKET;
    }

    void PuppetServerTCP::send(const PACKET& packet)
    {
        ASSERT(clientSocket != INVALID_SOCKET, "send(): Connection not established");

        int ret;

        ret = ::send(clientSocket, (const char*)&packet, packet.size, 0);
        ASSERT(ret == packet.size, "send(): send() failed");
    }

    PACKET* PuppetServerTCP::recv()
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