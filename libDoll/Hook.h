#pragma once
#include "pch.h"
#include "libDoll.h"

void HookAdd(UINT_PTR hookOEP, UINT_PTR denySPOffset, UINT_PTR denyAX);
void HookRemove(UINT_PTR hookOEP);

