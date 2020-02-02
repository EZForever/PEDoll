#pragma once
#include "pch.h"
#include "PEDollMonitor.h"

// Implements CMD_PS, lists all processes running into `entry`
uint32_t MonProcPs(std::vector<Puppet::PACKET*>& entry);

// Implements CMD_SHELL, executes "$COMSPEC $args"
// Returns 0 on success, or GetLastError() values
uint32_t MonProcShell(wchar_t* args);

// Implements CMD_KILL on a PID
uint32_t MonProcKillByPID(uint32_t pid);

// Implements CMD_KILL on a process name
uint32_t MonProcKillByName(wchar_t* name);
