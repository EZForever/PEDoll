#pragma once
#include "pch.h"

struct LIBDOLL_CTX {
    std::set<DWORD> dollThreads;
};

extern LIBDOLL_CTX ctx;

// Register current thread as a libDoll thread
// libDoll threads will not be affected by hooks, i.e. will follow the hook but sliently continues
extern void DollThreadRegisterCurrent();

// Unregister current thread
extern void DollThreadUnregisterCurrent();

// Check if current thread is registered
extern bool DollThreadIsCurrent();

