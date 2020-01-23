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
        CMD_MONITOR_ANY = 0x20,
        CMD_END = 0x21,
        CMD_DOLL = 0x22,
        CMD_PS = 0x23,
        CMD_SHELL = 0x24,
        CMD_KILL = 0x25,

        CMD_DOLL_ANY = 0x30,
        CMD_HOOK = 0x31,
        CMD_UNHOOK = 0x32,
        CMD_BREAK = 0x33,
        CMD_CONTEXT = 0x34,
        CMD_MEMORY = 0x35,
        CMD_VERDICT = 0x36,
        CMD_LOADDLL = 0x37,
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

    // A generic integer, mainly used for memory address
    struct PACKET_INTEGER : PACKET {
        // Data
        // Use 64-bit integer to prevent data loss
        uint64_t data;

        PACKET_INTEGER(uint64_t d = 0)
            : PACKET(sizeof(PACKET_INTEGER), PACKET_TYPE::INTEGER), data(d) {}
    };

    // A generic (Unicode) string
    // Use PacketAllocString() to create instance
    struct PACKET_STRING : PACKET {
        // Data
        wchar_t data[0];

        PACKET_STRING()
            : PACKET(sizeof(PACKET_STRING), PACKET_TYPE::STRING) {}
    };

    // A generic binary blob
    // Use PacketAllocBinary() to create instance
    struct PACKET_BINARY : PACKET {
        // Data
        unsigned char data[0];

        PACKET_BINARY()
            : PACKET(sizeof(PACKET_BINARY), PACKET_TYPE::BINARY) {}
    };

    // A Monitor or Doll informs Controller its presence
    // < MSG_ONLINE: isMonitor, bits, pid
    // < STRING: hostname (for Monitor) or execuable name (for Doll)
    // > ACK: 0
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
    // > ACK: 0
    struct PACKET_MSG_ONHOOK : PACKET {
        ;

        PACKET_MSG_ONHOOK()
            : PACKET(sizeof(PACKET_MSG_ONHOOK), PACKET_TYPE::MSG_ONHOOK) {}
    };

    // End this Puppet connection
    // > CMD_END
    // < ACK: 0 (42?)
    struct PACKET_CMD_END : PACKET {
        ;

        PACKET_CMD_END()
            : PACKET(sizeof(PACKET_CMD_END), PACKET_TYPE::CMD_END) {}
    };

    // (Monitor) Start a new Doll process
    // > CMD_DOLL
    // > STRING (if pid != 0): execuable path
    // < ACK: 0 on ok, or corresponding Win32 error codes
    struct PACKET_CMD_DOLL : PACKET {
        // The PID of target process
        // If == 0, launch a new execuable instead
        uint32_t pid;

        PACKET_CMD_DOLL()
            : PACKET(sizeof(PACKET_CMD_DOLL), PACKET_TYPE::CMD_DOLL),
              pid(-1) {}
    };

    // (Monitor) Enumerate processes, like linux command `ps`
    // > CMD_PS
    // < ACK: 0 on fail, or n ( > 0) for entry count
    // (repeat `n` times: )
    //     < INTEGER: pid
    //     < STRING: execuable name
    struct PACKET_CMD_PS : PACKET {
        ;

        PACKET_CMD_PS()
            : PACKET(sizeof(PACKET_CMD_PS), PACKET_TYPE::CMD_PS) {}
    };

    // (Monitor) Execute a shell command
    // > CMD_SHELL
    // > STRING: arguments passed to %COMSPEC%
    // < ACK: 0 on ok, or corresponding Win32 error codes
    struct PACKET_CMD_SHELL : PACKET {
        ;

        PACKET_CMD_SHELL()
            : PACKET(sizeof(PACKET_CMD_SHELL), PACKET_TYPE::CMD_SHELL) {}
    };

    // (Monitor) Kill a process
    // > CMD_KILL
    // > STRING (if pid != 0): execuable name
    // < ACK: 0 on ok, or corresponding Win32 error codes
    struct PACKET_CMD_KILL : PACKET {
        // The PID of target process
        // If == 0, kill all processes with the name (linux `killall`)
        uint32_t pid;

        PACKET_CMD_KILL()
            : PACKET(sizeof(PACKET_CMD_KILL), PACKET_TYPE::CMD_KILL),
            pid(-1) {}
    };

    // (Doll) Install a new hook
    // > CMD_HOOK
    // > STRING (if method == 0): module name (optional) and procedure name
    // > BINARY (if method == 1): binary blob to search for in memory space
    // > INTEGER (if method == 2): VA of hookOEP
    // < ACK: 0 on ok, or corresponding Win32 error codes
    // < INTEGER: hookOEP
    struct PACKET_CMD_HOOK : PACKET {
        // The method of seeking for hookOEP
        uint32_t method;

        PACKET_CMD_HOOK()
            : PACKET(sizeof(PACKET_CMD_HOOK), PACKET_TYPE::CMD_HOOK),
            method(0) {}
    };

    // (Doll) Uninstall a hook
    // > CMD_UNHOOK
    // > INTEGER: hookOEP
    // < ACK: 0 on ok, or corresponding Win32 error codes
    struct PACKET_CMD_UNHOOK : PACKET {
        ;

        PACKET_CMD_UNHOOK()
            : PACKET(sizeof(PACKET_CMD_UNHOOK), PACKET_TYPE::CMD_UNHOOK) {}
    };

    // (Doll) Suspend/resume all non-libDoll threads
    // > CMD_BREAK
    // < ACK: 0 on ok, or corresponding Win32 error codes
    struct PACKET_CMD_BREAK : PACKET {
        ;

        PACKET_CMD_BREAK()
            : PACKET(sizeof(PACKET_CMD_BREAK), PACKET_TYPE::CMD_BREAK) {}
    };

    // (Doll, on hook) Read a context register
    // > CMD_CONTEXT
    // < ACK: 0 (no error should really happen)
    // < INTEGER: register value
    struct PACKET_CMD_CONTEXT : PACKET {
        // The register's index
        // Ordered from 0: AX, CX, DX, BX, SP, BP, SI, DI, R8, R9
        uint32_t idx;

        PACKET_CMD_CONTEXT()
            : PACKET(sizeof(PACKET_CMD_CONTEXT), PACKET_TYPE::CMD_CONTEXT),
            idx(-1) {}
    };

    // (Doll, on hook) Read data from memory
    // > CMD_MEMORY
    // > INTEGER: start VA
    // < ACK: actual read size (0 == fail)
    // < BINARY: data
    struct PACKET_CMD_MEMORY : PACKET {
        // Expected read size
        uint32_t size;

        PACKET_CMD_MEMORY()
            : PACKET(sizeof(PACKET_CMD_MEMORY), PACKET_TYPE::CMD_MEMORY),
            size(-1) {}
    };

    // (Doll, on hook) Verdict the current hook
    // > CMD_VERDICT
    // > INTEGER (if verdict == 1): hooked procedure's SP offset
    // > INTEGER (if verdict == 1): faked return value
    // < ACK: 0
    struct PACKET_CMD_VERDICT : PACKET {
        // Controller's verdict to the current hook
        // 0 == Approved, 1 == Rejected, 2 == Terminated
        uint32_t verdict;

        PACKET_CMD_VERDICT()
            : PACKET(sizeof(PACKET_CMD_VERDICT), PACKET_TYPE::CMD_VERDICT),
            verdict(0) {}
    };

    // (Doll) Load a new DLL into the process via LoadLibrary()
    // > CMD_LOADDLL
    // > STRING: DLL path passed to LoadLibrary()
    // < ACK: 0 on ok, or corresponding Win32 error codes
    struct PACKET_CMD_LOADDLL : PACKET {
        ;

        PACKET_CMD_LOADDLL()
            : PACKET(sizeof(PACKET_CMD_LOADDLL), PACKET_TYPE::CMD_LOADDLL) {}
    };

#   pragma pack(pop)

    // Generic interface for a Puppet Protocol server/client
    class IPuppet {
    public:
        IPuppet() {}
        ~IPuppet() {}

        // errno of last faulty action
        // FIXME: This is just a polyfill, will need a new idea for error handling
        int lastError = 0;

        // For a server: Start waiting for a client
        // For a client: Establish connection to a server
        virtual void start() = 0;

        // Send a packet to connected server/client
        virtual void send(const PACKET& packet) = 0;

        // Wait & receives a packet from connected server/client
        // Use PacketFree() to free the returned pointer
        virtual PACKET* recv() = 0;

    private:
        // Copying of a instance is not allowed
        IPuppet(const IPuppet& x) = delete;
        IPuppet& operator=(IPuppet& x) = delete;
    };

    // Helper functions to create vardic-length packets
    // Use PacketFree() to free the returned pointer
    PACKET_STRING* PacketAllocString(const wchar_t* data);
    PACKET_BINARY* PacketAllocBinary(const unsigned char* data, uint32_t size);

    // Free a packet returned by PacketAlloc*() or IPuppet::recv()
    void PacketFree(PACKET* packet);
}