// Copyright 2010-2014 Zach Saw
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

/*
 * 2020-03-19 move some functions to NativeMethods.cs 
*/
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GrblPlotter
{
    public class SerialPortFixer : IDisposable
    {
        public static void Execute(string portName)
        {
            using (new SerialPortFixer(portName))
            {
            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (m_Handle != null)
                {
                    m_Handle.Close();
                    m_Handle = null;
                }
            }
        }

        #endregion

        #region Implementation

        private const int DcbFlagAbortOnError = 14;
        private const int CommStateRetries = 10;
        private SafeFileHandle m_Handle;

        private SerialPortFixer(string portName)
        {
            const int dwFlagsAndAttributes = 0x40000000;
            const int dwAccess = unchecked((int)0xC0000000);

            if ((portName == null) || !portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid Serial Port", "portName");
            }
            SafeFileHandle hFile = NativeMethods.CreateFile(@"\\.\" + portName, dwAccess, 0, IntPtr.Zero, 3, dwFlagsAndAttributes,
                                              IntPtr.Zero);
            if (hFile.IsInvalid)
            {
                WinIoError();
            }
            try
            {
                int fileType = NativeMethods.GetFileType(hFile);
                if ((fileType != 2) && (fileType != 0))
                {
                    throw new ArgumentException("Invalid Serial Port", "portName");
                }
                m_Handle = hFile;
                InitializeDcb();
            }
            catch
            {
                hFile.Close();
                m_Handle = null;
                //         throw;
            }
        }
        /*
                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                private static extern int FormatMessage(int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId,
                                                        StringBuilder lpBuffer, int nSize, IntPtr arguments);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                private static extern bool GetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                private static extern bool SetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                private static extern bool ClearCommError(SafeFileHandle hFile, ref int lpErrors, ref Comstat lpStat);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
                                                                IntPtr securityAttrs, int dwCreationDisposition,
                                                                int dwFlagsAndAttributes, IntPtr hTemplateFile);

                [DllImport("kernel32.dll", SetLastError = true)]
                private static extern int GetFileType(SafeFileHandle hFile);
                */
        private void InitializeDcb()
        {
            Dcb dcb = new Dcb();
            GetCommStateNative(ref dcb);
            dcb.Flags &= ~(1u << DcbFlagAbortOnError);
            SetCommStateNative(ref dcb);
        }

        private static string GetMessage(int errorCode)
        {
            StringBuilder lpBuffer = new StringBuilder(0x200);
            if (
                NativeMethods.FormatMessage(0x3200, new HandleRef(null, IntPtr.Zero), errorCode, 0, lpBuffer, lpBuffer.Capacity,
                              IntPtr.Zero) != 0)
            {
                return lpBuffer.ToString();
            }
            return "Unknown Error";
        }

        private static int MakeHrFromErrorCode(int errorCode)
        {
            return (int)(0x80070000 | (uint)errorCode);
        }

        private static void WinIoError()
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new IOException(GetMessage(errorCode), MakeHrFromErrorCode(errorCode));
        }

        private void GetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat();

            for (int i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (NativeMethods.GetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }

        private void SetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat();

            for (int i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (NativeMethods.SetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }

        #region Nested type: COMSTAT

        [StructLayout(LayoutKind.Sequential)]
        internal struct Comstat
        {
            public readonly uint Flags;
            public readonly uint cbInQue;
            public readonly uint cbOutQue;
        }

        #endregion

        #region Nested type: DCB

        [StructLayout(LayoutKind.Sequential)]
        internal struct Dcb
        {
            public readonly uint DCBlength;
            public readonly uint BaudRate;
            public uint Flags;
            public readonly ushort wReserved;
            public readonly ushort XonLim;
            public readonly ushort XoffLim;
            public readonly byte ByteSize;
            public readonly byte Parity;
            public readonly byte StopBits;
            public readonly byte XonChar;
            public readonly byte XoffChar;
            public readonly byte ErrorChar;
            public readonly byte EofChar;
            public readonly byte EvtChar;
            public readonly ushort wReserved1;
        }
        #endregion
        #endregion
    }
}

