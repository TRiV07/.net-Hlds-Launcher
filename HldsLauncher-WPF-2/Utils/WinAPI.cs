using System;
using System.Runtime.InteropServices;

using DWORD = System.UInt32;

namespace HldsLauncher.Utils
{
    public static class WinAPI
    {
        #region bool SetWindowPos (hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags) *via imported dll*
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        

        public const UInt32 SWP_NOSIZE = 0x0001;
        #endregion

        #region bool ShowWindow (hWnd, nCmdShow) *via imported dll*
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
        #endregion

        #region IntPtr SendMessage(hWnd, Msg, wParam, lParam) *via imported dll*
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, Char wParam, IntPtr lParam);
        public const int WM_CHAR = 0x0102;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int VK_ENTER = 13;
        public const int CCM_FIRST = 0x2000;
        public const int CCM_SETUNICODEFORMAT = (CCM_FIRST + 5);
        public const int CCM_GETUNICODEFORMAT = (CCM_FIRST + 6);
        #endregion

        #region IntPtr CreateEvent(lpEventAttributes, bManualReset, bInitialState, lpName) *via imported dll*
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public DWORD nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }
        #endregion

        #region IntPtr CreateFileMapping(hFile, lpFileMappingAttributes, flProtect, dwMaximumSizeHigh, dwMaximumSizeLow, lpName) *via imported dll*
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, FileMapProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, [MarshalAs(UnmanagedType.LPTStr)] string lpName);
        [Flags]
        public enum FileMapProtection : uint
        {
            PageReadonly = 0x02,
            PageReadWrite = 0x04,
            PageWriteCopy = 0x08,
            PageExecuteRead = 0x20,
            PageExecuteReadWrite = 0x40,
            SectionCommit = 0x8000000,
            SectionImage = 0x1000000,
            SectionNoCache = 0x10000000,
            SectionReserve = 0x4000000,
        }
        #endregion

        #region IntPtr MapViewOfFile(hFileMappingObject, dwDesiredAccess, dwFileOffsetHigh, dwFileOffsetLow, dwNumberOfBytesToMap) *via imported dll*
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            fileMapExecute = 0x0020,
        }
        #endregion

        #region IntPtr UnmapViewOfFile(lpBaseAddress) *via imported dll*
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static unsafe extern bool UnmapViewOfFile(char* lpBaseAddress);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static unsafe extern bool UnmapViewOfFile(int* lpBaseAddress);
        #endregion

        #region bool CloseHandle(hObject) *via imported dll*
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        #endregion

        #region bool SetEvent(hEvent) *via imported dll*
        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(IntPtr hEvent);
        #endregion

        #region UInt32 WaitForSingleObject(hHandle, dwMilliseconds) *via imported dll*
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        public const UInt32 INFINITE = 0xFFFFFFFF;
        public const UInt32 WAIT_ABANDONED = 0x00000080;
        public const UInt32 WAIT_OBJECT_0 = 0x00000000;
        public const UInt32 WAIT_TIMEOUT = 0x00000102;
        #endregion

        #region UInt32 CreateProcess(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles, dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation) *via imported dll*
        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }
        #endregion

        #region int NtSetTimerResolution(DesiredResolution, SetResolution, CurrentResolution);
        [DllImport("ntdll.dll")]
        public static extern int NtSetTimerResolution(int DesiredResolution, bool SetResolution, out int CurrentResolution);
        #endregion

        #region int NtQueryTimerResolution(MinimumResolution, MaximumResolution, ActualResolution);
        [DllImport("ntdll.dll")]
        public static extern int NtQueryTimerResolution(out int MinimumResolution, out int MaximumResolution, out int ActualResolution);
        #endregion

        #region hlds hook constants
        // Each ENGINE command token is a 32 bit integer

        public const int ENGINE_ISSUE_COMMANDS = 0x2;
        // Param1 : char *		text to issue

        public const int ENGINE_RETRIEVE_CONSOLE_CONTENTS = 0x3;
        // Param1 : int32		Begin line
        // Param2 : int32		End line

        public const int ENGINE_RETRIEVE_GET_CONSOLE_HEIGHT = 0x4;
        // No params

        public const int ENGINE_RETRIEVE_SET_CONSOLE_HEIGHT = 0x5;
        // Param1 : int32		Number of lines
        #endregion

        #region IntPtr SetWindowsHookEx(idHook, lpfn, hInstance, threadId);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, UIntPtr lpfn, IntPtr hInstance, int threadId);
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam, int serverIndex);
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }
        /*public struct CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        }
        public const int WM_INITDIALOG = 0x0110;
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);*/
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(int hhk);

        #endregion

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr FreeLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, byte[] procedureName);

    }
}
