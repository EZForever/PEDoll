#include "pch.h"
#include "libDoll.h"
#include "HookStub.h"

#include "../libPuppetProtocol/libPuppetProtocol.h"
#include "../libPuppetProtocol/PuppetClientTCP.h"

void __cdecl ThreadPuppetClient(void*)
{
    DollThreadRegisterCurrent();

    // TODO: ALL THE THINGS

}

