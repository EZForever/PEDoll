#include <windows.h>
#include <iostream>

// Include a Detours payload section for easlier debugging
#include "../Detours/repo/src/detours.h"
#include "../libPuppet/libPuppet.h"

using namespace std;

# pragma pack(push, 1)

struct PAYLOAD_SERVER_INFO
{
    DETOUR_SECTION_HEADER header;
    DETOUR_SECTION_RECORD record;
    uint32_t size;
    Puppet::PACKET_TYPE type;
    wchar_t data[32];
};

# pragma pack(pop)

#pragma data_seg(".detour")

static PAYLOAD_SERVER_INFO payload = {
    DETOUR_SECTION_HEADER_DECLARE(sizeof(PAYLOAD_SERVER_INFO)),
    {
        (sizeof(PAYLOAD_SERVER_INFO) - sizeof(DETOUR_SECTION_HEADER)),
        0,
        { 0xa2062469, 0x2b45, 0x496d, { 0x8f, 0xe9, 0x7e, 0x89, 0x4e, 0xd7, 0x22, 0x70 } }
    },
    (uint32_t)(sizeof(Puppet::PACKET_STRING) + sizeof(wchar_t) * 32),
    Puppet::PACKET_TYPE::STRING,
    L"127.0.0.1"
};
#pragma data_seg()

int main()
{
    cout << "sizeof(unsigned long) = " << sizeof(unsigned long) << endl;

    HMODULE hDollDLL = LoadLibrary(L"libDoll.dll");
    if (!hDollDLL)
    {
        cerr << "LoadLibrary() failed" << endl;
        return 1;
    }

    getchar();

    int ret = system("ver");
    cout << "ret = " << ret << endl;

    FreeLibrary(hDollDLL);
    return 0;
}

