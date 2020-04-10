#include <windows.h>
#include <iostream>

using namespace std;

// FIXME: Debug build of this dummy does not work
#define DETOUR_PAYLOAD

#ifdef DETOUR_PAYLOAD

// Include a Detours payload section for easlier debugging
#include "../Detours/repo/src/detours.h"

# pragma pack(push, 1)

struct PAYLOAD_SERVER_INFO
{
    DETOUR_SECTION_HEADER header;
    DETOUR_SECTION_RECORD record;
    char data[32];
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
    "127.0.0.1"
};
#pragma data_seg()

extern "C" int DollDllHelloWorld();

#endif // DETOUR_PAYLOAD

int main()
{
    cout << "sizeof(UINT_PTR) = " << sizeof(UINT_PTR) << endl;
    cout << "&payload = " << &payload << endl;

#   ifdef DETOUR_PAYLOAD
    DollDllHelloWorld();
#   endif // DETOUR_PAYLOAD

    getchar();

    int ret = WinExec("ver", SW_SHOW);
    cout << "ret = " << ret << endl;

    return 0;
}

