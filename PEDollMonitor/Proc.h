#pragma once
#include "pch.h"
#include "PEDollMonitor.h"

// Implements CMD_SHELL, executes "$COMSPEC $args"
// Returns 0 on success, or GetLastError() values
uint32_t MonProcShell(wchar_t* args);

