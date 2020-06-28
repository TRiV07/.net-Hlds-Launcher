// ErrorsHook.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

extern "C" __declspec(dllexport) LRESULT CALLBACK MessageBoxShowProc(int code, WPARAM wParam, LPARAM lParam)
{
	if (code == HC_ACTION)
	{
		CWPSTRUCT *pwp = (CWPSTRUCT*)lParam;
		if (pwp->message == WM_CTLCOLORDLG)
		{
			Sleep(500);
			
			HWND hwndDlg = (HWND)pwp->lParam;
			int capLen = GetWindowTextLength(hwndDlg) + 1;
			wchar_t *caption = new wchar_t[capLen];
			capLen = GetWindowText(hwndDlg, caption, capLen);

			HWND hStatic = GetWindow(GetWindow(hwndDlg, GW_CHILD), GW_HWNDLAST);
			int textLen = GetWindowTextLength(hStatic) + 1;
			wchar_t *text = new wchar_t[textLen];
			textLen = GetWindowText(hStatic, text, textLen);

			int processId = GetCurrentProcessId();

			HANDLE hNamedPipe;
			DWORD cbWritten;
			wchar_t szBuf[1024];

			hNamedPipe = INVALID_HANDLE_VALUE;
			while (hNamedPipe == INVALID_HANDLE_VALUE)
			{
				hNamedPipe = CreateFile(L"\\\\.\\pipe\\andOneHldsLauncher2", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
				Sleep(10);
			}
			int recLen = wsprintf(szBuf, L"%d|%s|%s|", processId, caption, text);
			WriteFile(hNamedPipe, szBuf, 1024, &cbWritten, NULL);
			CloseHandle(hNamedPipe);

			delete[] caption;
			delete[] text;
		}
	}
	return CallNextHookEx(NULL, code, wParam, lParam);
}