// libPuppetProtocol.h
// Generic declaration of server, client and packet structures
#pragma once
#pragma warning(disable:4200) // nonstandard extension used : zero-sized array in struct/union
#include <cstdint>

namespace Puppet {

    // Special packet Doll ids
#   define PUPPET_PACKET_DOLL_DEFAULT ((uint32_t)0)
#   define PUPPET_PACKET_DOLL_MONITOR ((uint32_t)-1)
#   define PUPPET_PACKET_DOLL_CONTROLLER ((uint32_t)-2)


    // ID of packet types
    enum class PACKET_TYPE : uint32_t {
        INVALID = 0,

        CMD_PING = 0x10,

        MSG_ACK = 0x20,
        MSG_TEXT = 0x21,
        MSG_BINARY = 0x22
    };

#   pragma pack(push, 1)

    // Base struct (header) for every Puppet Protocol packet
    struct PACKET {
        // Total size of the packet, in bytes
        // Must be the first field of a packet
        uint32_t size;
        // Doll ID
        // Receiver for commands packets, sender otherwise
        uint32_t doll;
        // Data type of this packet
        PACKET_TYPE type;

        PACKET(decltype(size) s = sizeof(PACKET), decltype(type) t = PACKET_TYPE::INVALID)
            : size(s), doll(PUPPET_PACKET_DOLL_MONITOR), type(t) {}
    };

    // Controller tests if the doll is still there
    struct PACKET_CMD_PING : PACKET {
        ;

        PACKET_CMD_PING()
            : PACKET(sizeof(PACKET_CMD_PING), PACKET_TYPE::CMD_PING) {}
    };

    // Monitor/Doll acknowledges the command sent
    // May followed by other MSG_* packets
    struct PACKET_MSG_ACK : PACKET {
        // Monitor/Doll's response to the command
        // Meanings may vary
        // Use 64-bit integer to prevent data loss
        uint64_t status;

        PACKET_MSG_ACK(uint64_t s = 0)
            : PACKET(sizeof(PACKET_MSG_ACK), PACKET_TYPE::MSG_ACK), status(s) {}
    };

    // A plaintext message sent from a Monitor or a Doll
    struct MSG_TEXT : PACKET {
        // Message
        wchar_t msg[0];

        MSG_TEXT(size_t msgLen)
            : PACKET(sizeof(MSG_TEXT) + sizeof(wchar_t) * (uint32_t)msgLen, PACKET_TYPE::MSG_TEXT) {}
    };

    // A binary message sent from a Monitor or a Doll
    struct MSG_BINARY : PACKET {
        // Message
        uint8_t msg[0];

        MSG_BINARY(size_t msgLen)
            : PACKET(sizeof(MSG_BINARY) + sizeof(uint8_t) * (uint32_t)msgLen, PACKET_TYPE::MSG_BINARY) {}
    };

#   pragma pack(pop)

    // Generic interface for a Puppet Protocol server
    class IPuppetServer {
    public:
        IPuppetServer() {}
        ~IPuppetServer() {}

        // Start waiting for client(s). Synchronous function.
        virtual void listen() = 0;

        // Send a packet to connected client. Asynchronous function.
        virtual void send(const PACKET& packet) = 0;

        // Wait & receives a packet from connected client. Synchronous function.
        // The returned pointer is malloc()'d.
        virtual PACKET* recv() = 0;

    private:
        // Copying of a server instance is not allowed
        IPuppetServer(const IPuppetServer& x) = delete;
        IPuppetServer& operator=(IPuppetServer& x) = delete;
    };

    // Generic interface for a Puppet Protocol client
    class IPuppetClient {
    public:
        IPuppetClient() {}
        virtual ~IPuppetClient() {}

        // Establish connection to server. Synchronous function.
        virtual void connect() = 0;

        // Send a packet to connected server. Asynchronous function.
        virtual void send(const PACKET& packet) = 0;

        // Wait & receives a packet from connected server. Synchronous function.
        // The returned pointer is malloc()'d.
        virtual PACKET* recv() = 0;

    private:
        // Copying of a client instance is not allowed
        IPuppetClient(const IPuppetClient& x) = delete;
        IPuppetClient& operator=(IPuppetClient& x) = delete;
    };

    // errno of last faulty action
    extern int lastError;
}