#pragma once
#include "pch.h"
#include "libDoll.h"

bool DollHookIsHappened();
void DollHookAdd(UINT_PTR hookOEP, UINT_PTR denySPOffset = 0, UINT_PTR denyAX = 0);
void DollHookRemove(UINT_PTR hookOEP);

