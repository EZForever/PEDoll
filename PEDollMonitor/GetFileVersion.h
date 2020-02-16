#pragma once
#include "pch.h"

// Get the given module's version string in the format of "x.x.x"
BOOL GetFileVersion(HMODULE hModule, std::string &str);

