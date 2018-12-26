using DWORD = System.UInt32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace GRBL_Plotter
{
    /// <summary>
    /// Providing all native methods
    /// </summary>
    internal class NativeMethods {
        #region PowerSaving

        #region Win7 functions
        internal const int POWER_REQUEST_CONTEXT_VERSION = 0;
        internal const int POWER_REQUEST_CONTEXT_SIMPLE_STRING = 0x1;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr PowerCreateRequest(ref POWER_REQUEST_CONTEXT Context);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool PowerSetRequest(IntPtr PowerRequestHandle, PowerRequestType RequestType);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool PowerClearRequest(IntPtr PowerRequestHandle, PowerRequestType RequestType);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct POWER_REQUEST_CONTEXT
        {
            public UInt32 Version;
            public UInt32 Flags;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string SimpleReasonString;
        }

        internal enum PowerRequestType
        {
            PowerRequestDisplayRequired = 0, // Not to be used by drivers
            PowerRequestSystemRequired,
            PowerRequestAwayModeRequired, // Not to be used by drivers
            PowerRequestExecutionRequired // Not to be used by drivers
        }

        internal static IntPtr currentPowerRequest;

        internal static void SuppressStandbyWin7()
        {
            // Clear current power request if there is any.
            if (currentPowerRequest != IntPtr.Zero)
            {
                PowerClearRequest(currentPowerRequest, PowerRequestType.PowerRequestSystemRequired);
                currentPowerRequest = IntPtr.Zero;
            }

            // Create new power request.
            POWER_REQUEST_CONTEXT pContext;
            pContext.Flags = POWER_REQUEST_CONTEXT_SIMPLE_STRING;
            pContext.Version = POWER_REQUEST_CONTEXT_VERSION;
            pContext.SimpleReasonString = "Standby suppressed by PowerAvailabilityRequests.exe";

            currentPowerRequest = PowerCreateRequest(ref pContext);

            if (currentPowerRequest == IntPtr.Zero)
            {
                // Failed to create power request.
                var error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);
            }

            bool success = PowerSetRequest(currentPowerRequest, PowerRequestType.PowerRequestSystemRequired);

            if (!success)
            {
                // Failed to set power request.
                currentPowerRequest = IntPtr.Zero;
                var error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);
            }
        }

        internal static void EnableStandbyWin7()
        {
            // Only try to clear power request if any power request is set.
            if (currentPowerRequest != IntPtr.Zero)
            {
                var success = PowerClearRequest(currentPowerRequest, PowerRequestType.PowerRequestSystemRequired);

                if (!success)
                {
                    // Failed to clear power request.
                    currentPowerRequest = IntPtr.Zero;
                    var error = Marshal.GetLastWin32Error();

                    if (error != 0)
                        throw new Win32Exception(error);
                }
                else
                {
                    currentPowerRequest = IntPtr.Zero;
                }
            }
        }


        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string dllToLoad);

        internal static bool PowerAvailabilityRequestsSupported()
        {
            var ptr = LoadLibrary("kernel32.dll");
            var ptr2 = GetProcAddress(ptr, "PowerSetRequest");

            if (ptr2 == IntPtr.Zero)
            {
                // Power availability requests NOT suppoted.              
                return false;
            }
            else
            {
                // Power availability requests ARE suppoted.                
                return true;
            }
        }
        #endregion

        #region winXP function
        internal const uint ES_SYSTEM_REQUIRED = 0x00000001;
        internal const uint ES_DISPLAY_REQUIRED = 0x00000002;
        internal const uint ES_USER_PRESENT = 0x00000004; // Only supported by Windows XP/Windows Server 2003
        internal const uint ES_AWAYMODE_REQUIRED = 0x00000040; // Not supported by Windows XP/Windows Server 2003
        internal const uint ES_CONTINUOUS = 0x80000000;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint SetThreadExecutionState(uint esFlags);

        internal static void SuppressStandbyXP()
        {
            var success = SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

            if (success == 0)
            {
                // Failed to suppress standby
                var error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);
            }
        }

        internal static void EnableStandbyXP()
        {
            var success = SetThreadExecutionState(ES_CONTINUOUS);

            if (success == 0)
            {
                // Failed to enable standby
                var error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);
            }
        }
        #endregion

        #endregion

        #region
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern DWORD WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern DWORD GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        #endregion


    }

}
