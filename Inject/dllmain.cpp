// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	/*switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}*/

	if (ul_reason_for_call == DLL_PROCESS_ATTACH)
	{
		HANDLE hStdout = GetStdHandle(STD_OUTPUT_HANDLE);
		SetConsoleCXCY(hStdout, 80, 24, 80, 200);
		SetConsoleOutputCP(65001);
		/*Sleep(10000);
		HINSTANCE instNtDll = LoadLibrary(L"NTDLL.dll");
		if (instNtDll)
		{
			Ptr_NtSetTimerResolution NtSetTimerResolution = (Ptr_NtSetTimerResolution)GetProcAddress(instNtDll, "NtQueryTimerResolution");
			if (NtSetTimerResolution)
			{
				PULONG actual = 0;
				NtSetTimerResolution(5000, TRUE, actual);
			}
		}*/
	}
	return TRUE;
}


