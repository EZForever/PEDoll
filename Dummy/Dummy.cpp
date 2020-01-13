#include <windows.h>
#include <iostream>

using namespace std;

int main()
{
    HMODULE hDollDLL = LoadLibrary(L"libDoll.dll");
    if (!hDollDLL)
    {
        cerr << "LoadLibrary() failed" << endl;
        return 1;
    }

    while (getchar() != ' ')
        ;
    FreeLibrary(hDollDLL);
    return 0;
}

