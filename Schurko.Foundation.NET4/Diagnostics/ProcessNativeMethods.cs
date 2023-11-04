using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Schurko.Foundation.Diagnostics
{
    internal static class ProcessNativeMethods
    {
        [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(nint tokenHandle, [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, ref TOKEN_PRIVILEGES newState, uint bufferLength, nint previousState, nint returnLength);

        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", SetLastError = true)]
        public static extern bool OpenProcessToken(nint processHandle, TokenAccess desiredAccess, out nint tokenHandle);

        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValue", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LookupPrivilegeValue(string systemName, string name, out LUID lpLuid);

        [DllImport("userenv.dll", EntryPoint = "CreateEnvironmentBlock", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(out nint environmentBlock, nint tokenHandle, bool inheritProcessEnvironment);

        [DllImport("userenv.dll", EntryPoint = "DestroyEnvironmentBlock", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyEnvironmentBlock(nint lpEnvironment);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        public static extern bool CloseHandle(nint handle);

        [DllImport("wtsapi32.dll", EntryPoint = "WTSQueryUserToken", SetLastError = true)]
        public static extern bool WTSQueryUserToken(uint sessionId, out nint tokenHandle);

        [DllImport("kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId", SetLastError = true)]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("Wtsapi32.dll", EntryPoint = "WTSQuerySessionInformation", SetLastError = true)]
        public static extern bool WTSQuerySessionInformation(nint serverHandle, int sessionId, WTS_INFO_CLASS wtsInfoClass, out nint ppBuffer, out uint pBytesReturned);

        [DllImport("wtsapi32.dll", EntryPoint = "WTSFreeMemory", SetLastError = false)]
        public static extern void WTSFreeMemory(nint memory);

        [DllImport("userenv.dll", EntryPoint = "LoadUserProfile", SetLastError = true)]
        public static extern bool LoadUserProfile(nint tokenHandle, ref PROFILEINFO profileInfo);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateProcessAsUser(nint userTokenHandle, string applicationName, string commandLine, nint lpProcessAttributes, nint lpThreadAttributes, bool inheritHandles, CreationFlags in_eCreationFlags, nint environmentBlock, string currentDirectory, ref STARTUPINFO startupInfo, ref PROCESS_INFORMATION processInformation);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessWithTokenW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateProcessWithTokenW(nint userTokenHandle, LogonFlags logonFlags, string applicationName, string commandLine, CreationFlags creationFlags, nint environmentBlock, string currentDirectory, ref STARTUPINFO startupInfo, ref PROCESS_INFORMATION processInformation);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx", SetLastError = true, CharSet = CharSet.Auto)]
        public extern static bool DuplicateTokenEx(nint existingTokenHandle, TokenAccess desiredAccess, nint tokenAttributes, SECURITY_IMPERSONATION_LEVEL impersonationLevel, TOKEN_TYPE tokenType, out nint newTokenHandle);

        [DllImport("user32.dll", EntryPoint = "PostThreadMessage", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, nint wParam, nint lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
        public static extern int PostMessage(nint hWnd, uint msg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        public static extern void EnumWindows(EnumWindowsCallbackDelegate d, uint lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);
    }
}