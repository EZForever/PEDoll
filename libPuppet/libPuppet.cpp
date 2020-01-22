﻿#include "pch.h"
#include "libPuppet.h"

namespace Puppet {

    PACKET_STRING* PacketAllocString(const wchar_t* data)
    {
        uint32_t packetSize = (uint32_t)(sizeof(PACKET_STRING) + sizeof(wchar_t) * (wcslen(data) + 1));
        PACKET_STRING* packet = (PACKET_STRING*)new char[packetSize];
        // Packet created that way will not call the constructor
        packet->size = packetSize;
        packet->type = PACKET_TYPE::STRING;
        wcscpy_s(packet->data, wcslen(data) + 1, data);
        return packet;
    }

    PACKET_BINARY* PacketAllocBinary(const unsigned char* data, uint32_t size)
    {
        uint32_t packetSize = (uint32_t)(sizeof(PACKET_BINARY) + size);
        PACKET_BINARY* packet = (PACKET_BINARY*)new char[packetSize];
        // Packet created that way will not call the constructor
        packet->size = packetSize;
        packet->type = PACKET_TYPE::BINARY;
        memcpy(packet->data, data, size);
        return packet;
    }

    void PacketFree(PACKET* packet)
    {
        delete[] (char*)packet;
    }

}