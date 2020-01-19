// libPuppet.h
// Generic declaration of server, client and packet structures
#pragma once
#pragma warning(disable:4200) // nonstandard extension used : zero-sized array in struct/union
#include <cstdint>

namespace Puppet {

    // ID of packet types
    enum class PACKET_TYPE : uint32_t {
        // Generic packets conveys plain data, the meanings of them are defined by previous packet(s)
        ANY = 0x00,
        ACK = 0x01,
        INTEGER = 0x02,
        STRING = 0x03,
        BINARY = 0x04,

        // Message packets are actively sent from a Monitor or Doll, in order to notify the Controller
        MSG_ANY = 0x10,
        MSG_ONLINE = 0x11,
        MSG_ONHOOK = 0x12,

        // Command packets are sent from Controller to a Monitor or Doll, in order to controll them
        CMD_ANY = 0x20,
        CMD_END = 0x21,
        CMD_DOLL = 0x22,
        CMD_PS = 0x23,
        CMD_EXEC = 0x24,
        CMD_KILL = 0x25,
        CMD_HOOK = 0x26,
        CMD_THREAD = 0x27,
        CMD_CONTEXT = 0x28,
        CMD_MEMORY = 0x29,
        CMD_VERDICT = 0x2a,
    };

#   pragma pack(push, 1)

    // Base/header of a Puppet Protocol packet
    struct PACKET {
        // Total size of the packet, in bytes
        // Must be the first field of a packet
        uint32_t size;
        // Data type of this packet
        PACKET_TYPE type;

        PACKET(decltype(size) s = sizeof(PACKET), decltype(type) t = PACKET_TYPE::ANY)
            : size(s), type(t) {}
    };

    // The sender acknowledges the command/message sent
    // No more command/message should be sent before receiving this
    // May followed by generic packets to provide additional info
    struct PACKET_ACK : PACKET {
        // The response to the command/message
        uint32_t status;

        PACKET_ACK(uint32_t s = 0)
            : PACKET(sizeof(PACKET_ACK), PACKET_TYPE::ACK), status(s) {}
    };

    // A generic integer or a memory address
    struct PACKET_INTEGER : PACKET {
        // Data
        // Use 64-bit integer to prevent data loss
        uint64_t data;

        PACKET_INTEGER(uint64_t d = 0)
            : PACKET(sizeof(PACKET_INTEGER), PACKET_TYPE::INTEGER), data(d) {}
    };

    // A generic (Unicode) string
    struct PACKET_STRING : PACKET {
        // Data
        wchar_t data[0];

        PACKET_STRING(size_t dataLen)
            : PACKET(sizeof(PACKET_STRING) + sizeof(wchar_t) * (uint32_t)dataLen, PACKET_TYPE::STRING) {}
    };

    // A generic binary blob
    struct PACKET_BINARY : PACKET {
        // Data
        unsigned char data[0];

        PACKET_BINARY(size_t dataLen)
            : PACKET(sizeof(PACKET_BINARY) + (uint32_t)dataLen, PACKET_TYPE::BINARY) {}
    };

    // A Monitor or Doll informs Controller its presence
    // < MSG_ONLINE
    // < STRING: hostname (for Monitor) or execuable name (for Doll)
    // > ACK
    struct PACKET_MSG_ONLINE : PACKET {
        // Non-zero if is a Monitor, vice versa
        uint32_t isMonitor;
        // 32 if a x86 process, 64 otherwise
        uint32_t bits;
        // PID of the process, as an unique ID
        uint32_t pid;

        PACKET_MSG_ONLINE()
            : PACKET(sizeof(PACKET_MSG_ONLINE), PACKET_TYPE::MSG_ONLINE),
              isMonitor(0), bits(64), pid(-1) {}
    };

    // A Doll's hook has been triggered
    // < MSG_ONHOOK
    // < INTEGER: hookOEP
    // > ACK
    struct PACKET_MSG_ONHOOK : PACKET {
        ;

        PACKET_MSG_ONHOOK()
            : PACKET(sizeof(PACKET_MSG_ONHOOK), PACKET_TYPE::MSG_ONHOOK) {}
    };

    // End this Puppet connection
    // > CMD_END
    // < ACK
    struct PACKET_CMD_END : PACKET {
        ;

        PACKET_CMD_END()
            : PACKET(sizeof(PACKET_CMD_END), PACKET_TYPE::CMD_END) {}
    };



#   pragma pack(pop)

    // Generic interface for a Puppet Protocol server/client
    class IPuppet {
    public:
        IPuppet() {}
        ~IPuppet() {}

        // For a server: Start waiting for a client.
        // For a client: Establish connection to a server.
        // Synchronous function.
        virtual void start() = 0;

        // Send a packet to connected server/client. Asynchronous function.
        virtual void send(const PACKET& packet) = 0;

        // Wait & receives a packet from connected server/client. Synchronous function.
        // The returned pointer is malloc()'d.
        virtual PACKET* recv() = 0;

    private:
        // Copying of a instance is not allowed
        IPuppet(const IPuppet& x) = delete;
        IPuppet& operator=(IPuppet& x) = delete;
    };

    // errno of last faulty action
    // FIXME: This is just a polyfill, will need a new idea for error handling
    extern int lastError;
}