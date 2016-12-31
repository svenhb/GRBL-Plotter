using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

// http://stackoverflow.com/questions/217902/reading-writing-an-ini-file

namespace GRBL_Plotter
{
    class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern IntPtr WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        //       [DllImport("kernel32", CharSet = CharSet.Unicode)]
        //       static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            NativeMethods.GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            try
            {
               WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
            }
            catch (Exception err) { }
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        public void WriteAll()
        {
            var setup = Properties.Settings.Default;
            string section = "GCode generation";
            Write("Dec Places", setup.importGCDecPlaces.ToString(), section);
            Write("XY Feedrate", setup.importGCXYFeed.ToString(), section);
            Write("Header Code", setup.importGCHeader.ToString(), section);
            Write("Footer Code", setup.importGCFooter.ToString(), section);
            Write("Add Comments", setup.importGCAddComments.ToString(), section);
            Write("Add Tool Cmd", setup.importGCTool.ToString(), section);
            Write("Z Enable", setup.importGCZEnable.ToString(), section);
            Write("Z Up Pos", setup.importGCZUp.ToString(), section);
            Write("Z Down Pos", setup.importGCZDown.ToString(), section);
            Write("Z Feedrate", setup.importGCZFeed.ToString(), section);
            Write("Spindle Toggle", setup.importGCSpindleToggle.ToString(), section);
            Write("Spindle Speed", setup.importGCSSpeed.ToString(), section);
            Write("PWM Enable", setup.importGCPWMEnable.ToString(), section);
            Write("PWM Up Val", setup.importGCPWMUp.ToString(), section);
            Write("PWM Up Dly", setup.importGCPWMDlyUp.ToString(), section);
            Write("PWM Down Val", setup.importGCPWMDown.ToString(), section);
            Write("PWM Down Dly", setup.importGCPWMDlyDown.ToString(), section);

            section = "Buttons";
            Write("Button1", setup.custom1.ToString(), section);
            Write("Button2", setup.custom2.ToString(), section);
            Write("Button3", setup.custom3.ToString(), section);
            Write("Button4", setup.custom4.ToString(), section);
            Write("Button5", setup.custom5.ToString(), section);
            Write("Button6", setup.custom6.ToString(), section);
            Write("Button7", setup.custom7.ToString(), section);
            Write("Button8", setup.custom8.ToString(), section);

            section = "Joystick";
            Write("XY1 Step", setup.joyXYStep1.ToString(), section);
            Write("XY2 Step", setup.joyXYStep2.ToString(), section);
            Write("XY3 Step", setup.joyXYStep3.ToString(), section);
            Write("XY4 Step", setup.joyXYStep4.ToString(), section);
            Write("XY5 Step", setup.joyXYStep5.ToString(), section);
            Write("Z1 Step", setup.joyZStep1.ToString(), section);
            Write("Z2 Step", setup.joyZStep2.ToString(), section);
            Write("Z3 Step", setup.joyZStep3.ToString(), section);
            Write("Z4 Step", setup.joyZStep4.ToString(), section);
            Write("Z5 Step", setup.joyZStep5.ToString(), section);
            Write("XY1 Speed", setup.joyXYSpeed1.ToString(), section);
            Write("XY2 Speed", setup.joyXYSpeed2.ToString(), section);
            Write("XY3 Speed", setup.joyXYSpeed3.ToString(), section);
            Write("XY4 Speed", setup.joyXYSpeed4.ToString(), section);
            Write("XY5 Speed", setup.joyXYSpeed5.ToString(), section);
            Write("Z1 Speed", setup.joyZSpeed1.ToString(), section);
            Write("Z2 Speed", setup.joyZSpeed2.ToString(), section);
            Write("Z3 Speed", setup.joyZSpeed3.ToString(), section);
            Write("Z4 Speed", setup.joyZSpeed4.ToString(), section);
            Write("Z5 Speed", setup.joyZSpeed5.ToString(), section);
        }
        public void ReadAll()
        {
            var setup = Properties.Settings.Default;
            string section = "GCode generation";

            section = "Buttons";
            setup.custom1 = Read("Button1", section);
            setup.custom2 = Read("Button2", section);
            setup.custom3 = Read("Button3", section);
            setup.custom4 = Read("Button4", section);
            setup.custom5 = Read("Button5", section);
            setup.custom6 = Read("Button6", section);
            setup.custom7 = Read("Button7", section);
            setup.custom8 = Read("Button8", section);

            section = "Joystick";
            setup.joyXYStep1 = Convert.ToDecimal(Read("XY1 Step", section));
            setup.joyXYStep2 = Convert.ToDecimal(Read("XY2 Step", section));
            setup.joyXYStep3 = Convert.ToDecimal(Read("XY3 Step", section));
            setup.joyXYStep4 = Convert.ToDecimal(Read("XY4 Step", section));
            setup.joyXYStep5 = Convert.ToDecimal(Read("XY5 Step", section));
            setup.joyZStep1 = Convert.ToDecimal(Read("Z1 Step", section));
            setup.joyZStep2 = Convert.ToDecimal(Read("Z2 Step", section));
            setup.joyZStep3 = Convert.ToDecimal(Read("Z3 Step", section));
            setup.joyZStep4 = Convert.ToDecimal(Read("Z4 Step", section));
            setup.joyZStep5 = Convert.ToDecimal(Read("Z5 Step", section));
            setup.joyXYSpeed1 = Convert.ToDecimal(Read("XY1 Speed", section));
            setup.joyXYSpeed2 = Convert.ToDecimal(Read("XY2 Speed", section));
            setup.joyXYSpeed3 = Convert.ToDecimal(Read("XY3 Speed", section));
            setup.joyXYSpeed4 = Convert.ToDecimal(Read("XY4 Speed", section));
            setup.joyXYSpeed5 = Convert.ToDecimal(Read("XY5 Speed", section));
            setup.joyZSpeed1 = Convert.ToDecimal(Read("Z1 Speed", section));
            setup.joyZSpeed2 = Convert.ToDecimal(Read("Z2 Speed", section));
            setup.joyZSpeed3 = Convert.ToDecimal(Read("Z3 Speed", section));
            setup.joyZSpeed4 = Convert.ToDecimal(Read("Z4 Speed", section));
            setup.joyZSpeed5 = Convert.ToDecimal(Read("Z5 Speed", section));

        }
    }
    internal static class NativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern IntPtr WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
    }

}
