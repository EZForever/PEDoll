#pragma once
#include "pch.h"
#include "libDoll.h"

void HookAdd(NATIVEWORD hookOEP, NATIVEWORD denySPOffset, NATIVEWORD denyAX);
void HookRemove(NATIVEWORD hookOEP);

