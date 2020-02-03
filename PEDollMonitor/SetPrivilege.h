#pragma once
#include "pch.h"

// Enable or disable the given privilege on a process
BOOL SetPrivilege(HANDLE hProcess, LPCTSTR lpszPrivilege, BOOL bEnablePrivilege);

