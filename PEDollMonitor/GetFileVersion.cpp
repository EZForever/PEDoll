#include "pch.h"

#include "GetFileVersion.h"

BOOL GetFileVersion(HMODULE hModule, std::string& str)
{
    char* moduleName = new char[MAX_PATH];
    if (!GetModuleFileNameA(hModule, moduleName, MAX_PATH))
    {
        delete[] moduleName;
        return FALSE;
    }

    DWORD handle;
    DWORD verInfoSize = GetFileVersionInfoSizeA(moduleName, &handle);
    if (!verInfoSize)
    {
        delete[] moduleName;
        return FALSE;
    }

    char* verInfo = new char[verInfoSize];
    if (!GetFileVersionInfoA(moduleName, 0, verInfoSize, verInfo))
    {
        delete[] moduleName;
        delete[] verInfo;
        return FALSE;
    }
    delete[] moduleName;

    VS_FIXEDFILEINFO* fileInfo;
    UINT fileInfoSize = sizeof(VS_FIXEDFILEINFO);
    if (!VerQueryValueA(verInfo, "\\", (LPVOID*)&fileInfo, &fileInfoSize))
    {
        delete[] verInfo;
        return FALSE;
    }

    std::stringstream builder;
    builder << (fileInfo->dwFileVersionMS >> 16)
        << '.'
        << (fileInfo->dwFileVersionMS & 0xffff)
        << '.'
        << (fileInfo->dwFileVersionLS >> 16);
    //<< '.'
    //<< (fileInfo->dwFileVersionLS & 0xffff);

    // For versions between release tags, a "*" is appended after the version string
    if (fileInfo->dwFileVersionLS & 0xffff)
        builder << '*';

    builder >> str;

    delete[] verInfo;
    return TRUE;
}