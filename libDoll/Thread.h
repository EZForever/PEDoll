#pragma once
#include "pch.h"
#include "libDoll.h"

// Register current thread as a libDoll thread
// libDoll threads will not be affected by hooks, i.e. will follow the hook but sliently continues
void DollThreadRegisterCurrent();

// Unregister current thread
void DollThreadUnregisterCurrent();

// Suspend all threads in current process, exclude the current one, and optionally libDoll ones
void DollThreadSuspendAll(bool skipDollThreads);

// Resume all threads suspended by DollThreadSuspendAll()
void DollThreadResumeAll();

