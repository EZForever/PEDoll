#include <windows.h>
#include <iostream>

using namespace std;

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

