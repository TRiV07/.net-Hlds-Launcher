// Inject.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

BOOL SetConsoleCXCY(HANDLE hStdout, int cx, int cy, int bx, int by)
{
	CONSOLE_SCREEN_BUFFER_INFO	info;
	COORD						coordMax;
 
	coordMax = GetLargestConsoleWindowSize(hStdout);

	if (cy > coordMax.Y)
		cy = coordMax.Y;

	/*if (by > coordMax.Y)
		by = coordMax.Y;*/

	if (cx > coordMax.X)
		cx = coordMax.X;
 
	if (!GetConsoleScreenBufferInfo(hStdout, &info))
		return FALSE;

// height
    info.srWindow.Left = 0;         
    info.srWindow.Right = info.dwSize.X - 1;                
    info.srWindow.Top = 0;
    info.srWindow.Bottom = cy - 1;          
 
	if (by < info.dwSize.Y)
	{
		if (!SetConsoleWindowInfo(hStdout, TRUE, &info.srWindow))
			return FALSE;
 
		info.dwSize.Y = by;
 
		if (!SetConsoleScreenBufferSize(hStdout, info.dwSize))
			return FALSE;
    }
    else if (by > info.dwSize.Y)
    {
		info.dwSize.Y = by;
 
		if (!SetConsoleScreenBufferSize(hStdout, info.dwSize))
			return FALSE;
 
		if (!SetConsoleWindowInfo(hStdout, TRUE, &info.srWindow))
			return FALSE;
    }
 
	if (!GetConsoleScreenBufferInfo(hStdout, &info))
		return FALSE;
 
// width
	info.srWindow.Left = 0;         
	info.srWindow.Right = cx - 1;
	info.srWindow.Top = 0;
	info.srWindow.Bottom = info.dwSize.Y - 1;               
 
	if (bx < info.dwSize.X)
	{
		if (!SetConsoleWindowInfo(hStdout, TRUE, &info.srWindow))
			return FALSE;
 
		info.dwSize.X = bx;
    
		if (!SetConsoleScreenBufferSize(hStdout, info.dwSize))
			return FALSE;
	}
	else if (bx > info.dwSize.X)
	{
		info.dwSize.X = bx;
 
		if (!SetConsoleScreenBufferSize(hStdout, info.dwSize))
			return FALSE;
 
		if (!SetConsoleWindowInfo(hStdout, TRUE, &info.srWindow))
			return FALSE;
	}
 
	return TRUE;
}