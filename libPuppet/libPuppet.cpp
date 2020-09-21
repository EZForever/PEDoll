#include "pch.h"
#include "libPuppet.h"

namespace Puppet {

    const GUID PAYLOAD_SERVER_INFO = { 0xa2062469, 0x2b45, 0x496d, { 0x8f, 0xe9, 0x7e, 0x89, 0x4e, 0xd7, 0x22, 0x70 } };

    const int DEFAULT_PORT = 31415;

    PACKET_STRING* PacketAllocString(const wchar_t* data)
    {
        uint32_t packetSize = (uint32_t)(sizeof(PACKET_STRING) + sizeof(wchar_t) * (wcslen(data) + 1));
        PACKET_STRING* packet = (PACKET_STRING*)new char[packetSize];
        new (packet) PACKET_STRING; // Call constructor to initialize header
        packet->size = packetSize;
        wcscpy_s(packet->data, wcslen(data) + 1, data);
        return packet;
    }

    PACKET_BINARY* PacketAllocBinary(const unsigned char* data, uint32_t size)
    {
        uint32_t packetSize = (uint32_t)(sizeof(PACKET_BINARY) + size);
        PACKET_BINARY* packet = (PACKET_BINARY*)new char[packetSize];
        new (packet) PACKET_BINARY; // Call constructor to initialize header
        packet->size = packetSize;
        memcpy(packet->data, data, size);
        return packet;
    }

    void PacketFree(PACKET* packet)
    {
        delete[] (char*)packet;
    }

}