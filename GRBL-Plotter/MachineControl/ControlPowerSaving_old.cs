/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

/* Thanks to:
 * https://decatec.de/programmierung/c-sharp-windows-standby-unterdruecken/
 * 
*/

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GRBL_Plotter
{
    public static class ControlPowerSaving
    {
        public static void SuppressStandby()
        {   if (ControlPowerSaving.PowerAvailabilityRequestsSupported())
                SuppressStandbyWin7();
            else
                SuppressStandbyXP();
        }
        public static void EnableStandby()
        {   if (ControlPowerSaving.PowerAvailabilityRequestsSupported())
                EnableStandbyWin7();
            else
                EnableStandbyXP();
        }

        #region Win7 functions
        private const int POWER_REQUEST_CONTEXT_VERSION = 0;
        private const int POWER_REQUEST_CONTEXT_SIMPLE_STRING = 0x1;

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

        private static IntPtr currentPowerRequest;
        private static void SuppressStandbyWin7()
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

        private static void EnableStandbyWin7()
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

        private static bool PowerAvailabilityRequestsSupported()
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
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;
        private const uint ES_USER_PRESENT = 0x00000004; // Only supported by Windows XP/Windows Server 2003
        private const uint ES_AWAYMODE_REQUIRED = 0x00000040; // Not supported by Windows XP/Windows Server 2003
        private const uint ES_CONTINUOUS = 0x80000000;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint SetThreadExecutionState(uint esFlags);

        private static void SuppressStandbyXP()
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

        private static void EnableStandbyXP()
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
    }
}
