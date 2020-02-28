#pragma once
#include "pch.h"

#include "../libPuppet/libPuppet.h"

struct MONITOR_CTX {
    Puppet::IPuppet* puppet;
    HANDLE hTPuppet;
    char* serverInfo;
    wchar_t* libDollPath;
};

// Called on a fatal error
void MonPanic(const char* msg);
//void MonPanic(const wchar_t* msg);

// Global context
extern MONITOR_CTX ctx;

