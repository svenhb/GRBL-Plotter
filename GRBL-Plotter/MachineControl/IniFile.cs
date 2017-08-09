/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2017 Sven Hasemann contact: svenhb@web.de

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
/*
* Thanks to http://stackoverflow.com/questions/217902/reading-writing-an-ini-file
*/

using DWORD = System.UInt32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern DWORD WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern DWORD GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            try
            {
                GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
                return RetVal.ToString();
            }
            catch (Exception err) { MessageBox.Show("Error in IniFile-Read: " +err.ToString()); return ""; }
        }

        public void Write(string Key, string Value, string Section = null)
        {
            try
            {
                WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
            }
            catch (Exception err) { MessageBox.Show("Error in IniFile-Read: " + err.ToString()); }
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

        public void WriteAll(List<string> GRBLSettings)
        {
            var setup = Properties.Settings.Default;
            string section = "Info";
            DateTime localDate = DateTime.Now;
            Write("Date", localDate.ToString(), section);

            section = "GCode generation";
            Write("Dec Places", setup.importGCDecPlaces.ToString(), section);
            Write("XY Feedrate", setup.importGCXYFeed.ToString(), section);
            Write("Header Code", setup.importGCHeader.ToString(), section);
            Write("Footer Code", setup.importGCFooter.ToString(), section);
            Write("Compress", setup.importGCCompress.ToString(), section);
            Write("Relative", setup.importGCRelative.ToString(), section);
            Write("No Arcs", setup.importGCNoArcs.ToString(), section);

            Write("Add Comments", setup.importGCAddComments.ToString(), section);
            Write("Add Tool Cmd", setup.importGCTool.ToString(), section);

            Write("Z Enable", setup.importGCZEnable.ToString(), section);
            Write("Z Up Pos", setup.importGCZUp.ToString(), section);
            Write("Z Down Pos", setup.importGCZDown.ToString(), section);
            Write("Z Feedrate", setup.importGCZFeed.ToString(), section);
            Write("Z Inc Enable", setup.importGCZIncEnable.ToString(), section);
            Write("Z Increment", setup.importGCZIncrement.ToString(), section);

            Write("Spindle Toggle", setup.importGCSpindleToggle.ToString(), section);
            Write("Spindle Speed", setup.importGCSSpeed.ToString(), section);
            Write("PWM Enable", setup.importGCPWMEnable.ToString(), section);
            Write("PWM Up Val", setup.importGCPWMUp.ToString(), section);
            Write("PWM Up Dly", setup.importGCPWMDlyUp.ToString(), section);
            Write("PWM Down Val", setup.importGCPWMDown.ToString(), section);
            Write("PWM Down Dly", setup.importGCPWMDlyDown.ToString(), section);

            section = "Tool";
            Write("Diameter", setup.toolDiameter.ToString(), section);
            Write("Z Step", setup.toolZStep.ToString(), section);
            Write("XY Feedrate", setup.toolFeedXY.ToString(), section);
            Write("Z Feedrate", setup.toolFeedZ.ToString(), section);
            Write("Overlap", setup.toolOverlap.ToString(), section);
            Write("Spindle Speed", setup.toolSpindleSpeed.ToString(), section);

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

            section = "Camera";
            Write("Index", setup.cameraindex.ToString(), section);
            Write("Rotation", setup.camerarotation.ToString(), section);
            Write("Top Pos", setup.cameraPosTop.ToString(), section);
            Write("Top Radius", setup.cameraTeachRadiusTop.ToString(), section);
            Write("Top Scaling", setup.camerascalingTop.ToString(), section);
            Write("Bottom Pos", setup.cameraPosBot.ToString(), section);
            Write("Bottom Radius", setup.cameraTeachRadiusBot.ToString(), section);
            Write("Bottom Scaling", setup.camerascalingBot.ToString(), section);
            Write("X Tool Offset", setup.cameraToolOffsetX.ToString(), section);
            Write("Y Tool Offset", setup.cameraToolOffsetY.ToString(), section);

            section = "Rotary axis";
            Write("Enable", setup.rotarySubstitutionEnable.ToString(), section);
            Write("Scale", setup.rotarySubstitutionScale.ToString(), section);
            Write("AxisX", setup.rotarySubstitutionX.ToString(), section);
            Write("Diameter", setup.rotarySubstitutionDiameter.ToString(), section);
            Write("SetupEnable", setup.rotarySubstitutionSetupEnable.ToString(), section);
            Write("SetupOn", setup.rotarySubstitutionSetupOn, section);
            Write("SetupOff", setup.rotarySubstitutionSetupOff, section);

            section = "GRBL Settings";
            if (GRBLSettings.Count > 0)
            {   foreach (string setting in GRBLSettings)
                {   string[] splt = setting.Split('=');
                    if (splt.Length > 1)
                        Write(splt[0], splt[1], section);
                }
            }
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

            section = "GCode generation";
            setup.importGCDecPlaces = Convert.ToDecimal(Read("Dec Places", section));
            setup.importGCXYFeed = Convert.ToDecimal(Read("XY Feedrate", section));
            setup.importGCHeader =Read("Header Code", section);
            setup.importGCFooter = Read("Footer Code", section);
            setup.importGCAddComments = Convert.ToBoolean(Read("Add Comments", section));
            setup.importGCTool = Convert.ToBoolean(Read("Add Tool Cmd", section));
            setup.importGCCompress = Convert.ToBoolean(Read("Compress", section));
            setup.importGCRelative = Convert.ToBoolean(Read("Relative", section));
            setup.importGCNoArcs   = Convert.ToBoolean(Read("No Arcs", section));

            setup.importGCZEnable = Convert.ToBoolean(Read("Z Enable", section));
            setup.importGCZUp = Convert.ToDecimal(Read("Z Up Pos", section));
            setup.importGCZDown = Convert.ToDecimal(Read("Z Down Pos", section));
            setup.importGCZFeed = Convert.ToDecimal(Read("Z Feedrate", section));
            setup.importGCZIncEnable = Convert.ToBoolean(Read("Z Inc Enable", section));
            setup.importGCZIncrement = Convert.ToDecimal(Read("Z Increment", section));

            setup.importGCSpindleToggle = Convert.ToBoolean(Read("Spindle Toggle", section));
            setup.importGCSSpeed = Convert.ToDecimal(Read("Spindle Speed", section));
            setup.importGCPWMEnable = Convert.ToBoolean(Read("PWM Enable", section));
            setup.importGCPWMUp = Convert.ToDecimal(Read("PWM Up Val", section));
            setup.importGCPWMDlyUp = Convert.ToDecimal(Read("PWM Up Dly", section));
            setup.importGCPWMDown = Convert.ToDecimal(Read("PWM Down Val", section));
            setup.importGCPWMDlyDown = Convert.ToDecimal(Read("PWM Down Dly", section));

            section = "Tool";
            setup.toolDiameter = Convert.ToDecimal(Read("Diameter", section));
            setup.toolZStep = Convert.ToDecimal(Read("Z Step", section));
            setup.toolFeedXY = Convert.ToDecimal(Read("XY Feedrate", section));
            setup.toolFeedZ = Convert.ToDecimal(Read("Z Feedrate", section));
            setup.toolOverlap = Convert.ToDecimal(Read("Overlap", section));
            setup.toolSpindleSpeed = Convert.ToDecimal(Read("Spindle Speed", section));

            section = "Camera";
            setup.cameraindex = Convert.ToByte(Read("Index", section));
            setup.camerarotation = Convert.ToDouble(Read("Rotation", section));
            setup.cameraPosTop = Convert.ToDouble(Read("Top Pos", section));
            setup.cameraTeachRadiusTop = Convert.ToDouble(Read("Top Radius", section));
            setup.camerascalingTop = Convert.ToDouble(Read("Top Scaling", section));
            setup.cameraPosBot = Convert.ToDouble(Read("Bottom Pos", section));
            setup.cameraTeachRadiusBot = Convert.ToDouble(Read("Bottom Radius", section));
            setup.camerascalingBot = Convert.ToDouble(Read("Bottom Scaling", section));
            setup.cameraToolOffsetX = Convert.ToDouble(Read("X Tool Offset", section));
            setup.cameraToolOffsetY = Convert.ToDouble(Read("Y Tool Offset", section));

            section = "Rotary axis";
            setup.rotarySubstitutionEnable      = Convert.ToBoolean(Read("Enable", section));
            setup.rotarySubstitutionScale       = Convert.ToDecimal(Read("Scale", section));
            setup.rotarySubstitutionX           = Convert.ToBoolean(Read("AxisX", section));
            setup.rotarySubstitutionDiameter    = Convert.ToDecimal(Read("Diameter", section));
            setup.rotarySubstitutionSetupEnable = Convert.ToBoolean(Read("SetupEnable", section));
            setup.rotarySubstitutionSetupOn     = (Read("SetupOn", section));
            setup.rotarySubstitutionSetupOff    = (Read("SetupOff", section));
        }
    }
/*    internal static class NativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern DWORD WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.SysInt)]
        internal static extern DWORD GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
    }*/
}
