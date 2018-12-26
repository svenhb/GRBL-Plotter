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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace GRBL_Plotter
{
    class IniFile
    {
        readonly string Path;
        readonly string ExeName = Assembly.GetExecutingAssembly().GetName().Name;

       
        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? ExeName + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            try
            {
                NativeMethods.GetPrivateProfileString(Section ?? ExeName, Key, "", RetVal, 255, Path);
                return RetVal.ToString();
            }
            catch (Exception err) { MessageBox.Show("Error in IniFile-Read: " +err.ToString()); return ""; }
        }

        public void Write(string Key, string Value, string Section = null)
        {
            try
            {
                NativeMethods.WritePrivateProfileString(Section ?? ExeName, Key, Value, Path);
            }
            catch (Exception err) { MessageBox.Show("Error in IniFile-Read: " + err.ToString()); }
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? ExeName);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? ExeName);
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

            section = "Graphics Import";
            Write("Graph Units mm", setup.importUnitmm.ToString(), section);
            Write("Graph Units GCode", setup.importUnitGCode.ToString(), section);
            Write("Bezier Segment count", setup.importSVGBezier.ToString(), section);
            Write("Remove Moves enable", setup.importSVGReduce.ToString(), section);
            Write("Remove Moves units", setup.importSVGReduceLimit.ToString(), section);
            Write("Pause before Path", setup.importSVGPauseElement.ToString(), section);
            Write("Pause before Pen", setup.importSVGPausePenDown.ToString(), section);
            Write("Add Comments", setup.importSVGAddComments.ToString(), section);

            Write("Repeat Code enable", setup.importSVGRepeatEnable.ToString(), section);
            Write("Repeat Code count", setup.importSVGRepeat.ToString(), section);
            Write("SVG resize enable", setup.importSVGRezise.ToString(), section);
            Write("SVG resize units", setup.importSVGMaxSize.ToString(), section);
            Write("SVG Tool sort by nr", setup.importSVGToolUse.ToString(), section);
            Write("SVG Tool sort", setup.importSVGToolSort.ToString(), section);
            Write("SVG Process nodes only", setup.importSVGNodesOnly.ToString(), section);


            section = "GCode generation";
            Write("Dec Places", setup.importGCDecPlaces.ToString(), section);
            Write("Header Code", setup.importGCHeader.ToString(), section);
            Write("Footer Code", setup.importGCFooter.ToString(), section);
            Write("Convert G23 enable", setup.importGCNoArcs.ToString(), section);
            Write("Convert G23 step", setup.importGCSegment.ToString(), section);
            Write("Line segmentation enable", setup.importGCLineSegmentation.ToString(), section);
            Write("Line segmentation length", setup.importGCLineSegmentLength.ToString(), section);
            Write("Line segmentation equidistant", setup.importGCLineSegmentEquidistant.ToString(), section);
            Write("Insert subroutine enable", setup.importGCSubEnable.ToString(), section);
            Write("Insert subroutine file", setup.importGCSubroutine.ToString(), section);
            Write("Drag tool enable", setup.importGCDragKnifeEnable.ToString(), section);
            Write("Drag tool offset", setup.importGCDragKnifeLength.ToString(), section);
            Write("Drag tool angle", setup.importGCDragKnifeAngle.ToString(), section);

            Write("Add Tool Cmd", setup.importGCTool.ToString(), section);
            Write("Add Tool M0", setup.importGCToolM0.ToString(), section);

            Write("Compress", setup.importGCCompress.ToString(), section);
            Write("Relative", setup.importGCRelative.ToString(), section);
            Write("Add Comments", setup.importGCAddComments.ToString(), section);

            Write("XY Feedrate", setup.importGCXYFeed.ToString(), section);
            Write("Spindle Speed", setup.importGCSSpeed.ToString(), section);

            Write("Z Enable", setup.importGCZEnable.ToString(), section);
            Write("Z Up Pos", setup.importGCZUp.ToString(), section);
            Write("Z Down Pos", setup.importGCZDown.ToString(), section);
            Write("Z Feedrate", setup.importGCZFeed.ToString(), section);
            Write("Z Inc Enable", setup.importGCZIncEnable.ToString(), section);
            Write("Z Increment", setup.importGCZIncrement.ToString(), section);

            Write("PWM Enable", setup.importGCPWMEnable.ToString(), section);
            Write("PWM Up Val", setup.importGCPWMUp.ToString(), section);
            Write("PWM Up Dly", setup.importGCPWMDlyUp.ToString(), section);
            Write("PWM Down Val", setup.importGCPWMDown.ToString(), section);
            Write("PWM Down Dly", setup.importGCPWMDlyDown.ToString(), section);

            Write("Spindle Toggle", setup.importGCSpindleToggle.ToString(), section);
            Write("Spindle use M3", setup.importGCSpindleCmd.ToString(), section);

            Write("Individual enable", setup.importGCIndEnable.ToString(), section);
            Write("Individual PenUp", setup.importGCIndPenUp.ToString(), section);
            Write("Individual PenDown", setup.importGCIndPenDown.ToString(), section);

            section = "Tool change";
            Write("Tool remove", setup.ctrlToolScriptPut.ToString(), section);
            Write("Tool select", setup.ctrlToolScriptSelect.ToString(), section);
            Write("Tool pick up", setup.ctrlToolScriptGet.ToString(), section);
            Write("Tool probe", setup.ctrlToolScriptProbe.ToString(), section);
            Write("Tool empty", setup.ctrlToolChangeEmpty.ToString(), section);
            Write("Tool empty Nr", setup.ctrlToolChangeEmptyNr.ToString(), section);
            // Tool table: load csv and write "Tool Nr "+i

            section = "Machine Limits";
            Write("Limit show", setup.machineLimitsShow.ToString(), section);
            Write("Limit alarm", setup.machineLimitsAlarm.ToString(), section);
            Write("Range X", setup.machineLimitsRangeX.ToString(), section);
            Write("Range Y", setup.machineLimitsRangeY.ToString(), section);
            Write("Range Z", setup.machineLimitsRangeZ.ToString(), section);
            Write("Home X", setup.machineLimitsHomeX.ToString(), section);
            Write("Home Y", setup.machineLimitsHomeY.ToString(), section);
            Write("Home Z", setup.machineLimitsHomeZ.ToString(), section);

            section = "4th axis";

            section = "Rotary axis";
            Write("Enable", setup.rotarySubstitutionEnable.ToString(), section);
            Write("Scale", setup.rotarySubstitutionScale.ToString(), section);
            Write("AxisX", setup.rotarySubstitutionX.ToString(), section);
            Write("Diameter", setup.rotarySubstitutionDiameter.ToString(), section);
            Write("SetupEnable", setup.rotarySubstitutionSetupEnable.ToString(), section);
            Write("SetupOn", setup.rotarySubstitutionSetupOn, section);
            Write("SetupOff", setup.rotarySubstitutionSetupOff, section);

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

            section = "Graphics Import";
            setup.importUnitmm          = Convert.ToBoolean(Read("Graph Units mm", section));
            setup.importUnitGCode       = Convert.ToBoolean(Read("Graph Units GCode", section));
            setup.importSVGBezier       = myConvertToDecimal(Read("Bezier Segment count", section));
            setup.importSVGReduce       = Convert.ToBoolean(Read("Remove Moves enable", section));
            setup.importSVGReduceLimit  = myConvertToDecimal(Read("Remove Moves units", section));
            setup.importSVGPauseElement = Convert.ToBoolean(Read("Pause before Path", section));
            setup.importSVGPausePenDown = Convert.ToBoolean(Read("Pause before Pen", section));
            setup.importSVGAddComments  = Convert.ToBoolean(Read("Add Comments", section));

            setup.importSVGRepeatEnable = Convert.ToBoolean(Read("Repeat Code enable", section));
            setup.importSVGRepeat       = myConvertToDecimal(Read("Repeat Code count", section));
            setup.importSVGRezise       = Convert.ToBoolean(Read("SVG resize enable", section));
            setup.importSVGMaxSize      = myConvertToDecimal(Read("SVG resize units", section));
            setup.importSVGToolUse      = Convert.ToBoolean(Read("SVG Tool sort by nr", section));
            setup.importSVGToolSort     = Convert.ToBoolean(Read("SVG Tool sort", section));
            setup.importSVGNodesOnly    = Convert.ToBoolean(Read("SVG Process nodes only", section));

            section = "GCode generation";
            setup.importGCDecPlaces = myConvertToDecimal(Read("Dec Places", section));
            setup.importGCHeader = Read("Header Code", section);
            setup.importGCFooter = Read("Footer Code", section);
            setup.importGCNoArcs    = Convert.ToBoolean(Read("Convert G23 enable", section));
            setup.importGCSegment   = myConvertToDecimal(Read("Convert G23 step", section));

            setup.importGCLineSegmentation  = Convert.ToBoolean(Read("Line segmentation enable", section));
            setup.importGCLineSegmentLength = myConvertToDecimal(Read("Line segmentation length", section));
            setup.importGCLineSegmentEquidistant = Convert.ToBoolean(Read("Line segmentation equidistant", section));
            setup.importGCSubEnable         = Convert.ToBoolean(Read("Insert subroutine enable", section));
            setup.importGCSubroutine = Read("Insert subroutine file", section);
            setup.importGCDragKnifeEnable   = Convert.ToBoolean(Read("Drag tool enable", section));
            setup.importGCDragKnifeLength   = myConvertToDecimal(Read("Drag tool offset", section));
            setup.importGCDragKnifeAngle    = myConvertToDecimal(Read("Drag tool angle", section));

            setup.importGCTool      = Convert.ToBoolean(Read("Add Tool Cmd", section));
            setup.importGCToolM0    = Convert.ToBoolean(Read("Add Tool M0", section));

            setup.importGCCompress = Convert.ToBoolean(Read("Compress", section));
            setup.importGCRelative = Convert.ToBoolean(Read("Relative", section));
            setup.importGCAddComments = Convert.ToBoolean(Read("Add Comments", section));

            setup.importGCXYFeed = myConvertToDecimal(Read("XY Feedrate", section));
            setup.importGCSSpeed = myConvertToDecimal(Read("Spindle Speed", section));

            setup.importGCZEnable   = Convert.ToBoolean(Read("Z Enable", section));
            setup.importGCZUp       = myConvertToDecimal(Read("Z Up Pos", section));
            setup.importGCZDown     = myConvertToDecimal(Read("Z Down Pos", section));
            setup.importGCZFeed     = myConvertToDecimal(Read("Z Feedrate", section));
            setup.importGCZIncEnable = Convert.ToBoolean(Read("Z Inc Enable", section));
            setup.importGCZIncrement = myConvertToDecimal(Read("Z Increment", section));

            setup.importGCSpindleToggle = Convert.ToBoolean(Read("Spindle Toggle", section));
            setup.importGCPWMEnable     = Convert.ToBoolean(Read("PWM Enable", section));
            setup.importGCPWMUp         = myConvertToDecimal(Read("PWM Up Val", section));
            setup.importGCPWMDlyUp      = myConvertToDecimal(Read("PWM Up Dly", section));
            setup.importGCPWMDown       = myConvertToDecimal(Read("PWM Down Val", section));
            setup.importGCPWMDlyDown    = myConvertToDecimal(Read("PWM Down Dly", section));

            setup.importGCSpindleToggle = Convert.ToBoolean(Read("Spindle Toggle", section));
            setup.importGCSpindleCmd = Convert.ToBoolean(Read("Spindle use M3", section));

            setup.importGCIndEnable = Convert.ToBoolean(Read("Individual enable", section));
            setup.importGCIndPenUp = Read("Individual PenUp", section);
            setup.importGCIndPenDown = Read("Individual PenDown", section);

            section = "Tool change";
            setup.ctrlToolScriptPut     = Read("Tool remove", section);
            setup.ctrlToolScriptSelect  = Read("Tool select", section);
            setup.ctrlToolScriptGet     = Read("Tool pick up", section);
            setup.ctrlToolScriptProbe   = Read("Tool probe", section);
            setup.ctrlToolChangeEmpty   = Convert.ToBoolean(Read("Tool empty", section));
            setup.ctrlToolChangeEmptyNr = myConvertToDecimal(Read("Tool empty Nr", section));

            section = "Machine Limits";
            setup.machineLimitsShow =   Convert.ToBoolean(Read("Limit show", section));
            setup.machineLimitsAlarm =  Convert.ToBoolean(Read("Limit alarm", section));
            setup.machineLimitsRangeX = myConvertToDecimal(Read("Range X", section));
            setup.machineLimitsRangeY = myConvertToDecimal(Read("Range Y", section));
            setup.machineLimitsRangeZ = myConvertToDecimal(Read("Range Z", section));
            setup.machineLimitsHomeX =  myConvertToDecimal(Read("Home X", section));
            setup.machineLimitsHomeY =  myConvertToDecimal(Read("Home Y", section));
            setup.machineLimitsHomeZ =  myConvertToDecimal(Read("Home Z", section));

            section = "4th axis";

            section = "Rotary axis";
            setup.rotarySubstitutionEnable  = Convert.ToBoolean(Read("Enable", section));
            setup.rotarySubstitutionScale   = myConvertToDecimal(Read("Scale", section));
            setup.rotarySubstitutionX       = Convert.ToBoolean(Read("AxisX", section));
            setup.rotarySubstitutionDiameter = myConvertToDecimal(Read("Diameter", section));
            setup.rotarySubstitutionSetupEnable = Convert.ToBoolean(Read("SetupEnable", section));
            setup.rotarySubstitutionSetupOn  = (Read("SetupOn", section));
            setup.rotarySubstitutionSetupOff = (Read("SetupOff", section));

            section = "Tool";
            setup.toolDiameter  = myConvertToDecimal(Read("Diameter", section));
            setup.toolZStep     = myConvertToDecimal(Read("Z Step", section));
            setup.toolFeedXY    = myConvertToDecimal(Read("XY Feedrate", section));
            setup.toolFeedZ     = myConvertToDecimal(Read("Z Feedrate", section));
            setup.toolOverlap   = myConvertToDecimal(Read("Overlap", section));
            setup.toolSpindleSpeed = myConvertToDecimal(Read("Spindle Speed", section));

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
            setup.joyXYStep1    = myConvertToDecimal(Read("XY1 Step", section));
            setup.joyXYStep2    = myConvertToDecimal(Read("XY2 Step", section));
            setup.joyXYStep3    = myConvertToDecimal(Read("XY3 Step", section));
            setup.joyXYStep4    = myConvertToDecimal(Read("XY4 Step", section));
            setup.joyXYStep5    = myConvertToDecimal(Read("XY5 Step", section));
            setup.joyZStep1     = myConvertToDecimal(Read("Z1 Step", section));
            setup.joyZStep2     = myConvertToDecimal(Read("Z2 Step", section));
            setup.joyZStep3     = myConvertToDecimal(Read("Z3 Step", section));
            setup.joyZStep4     = myConvertToDecimal(Read("Z4 Step", section));
            setup.joyZStep5     = myConvertToDecimal(Read("Z5 Step", section));
            setup.joyXYSpeed1   = myConvertToDecimal(Read("XY1 Speed", section));
            setup.joyXYSpeed2   = myConvertToDecimal(Read("XY2 Speed", section));
            setup.joyXYSpeed3   = myConvertToDecimal(Read("XY3 Speed", section));
            setup.joyXYSpeed4   = myConvertToDecimal(Read("XY4 Speed", section));
            setup.joyXYSpeed5   = myConvertToDecimal(Read("XY5 Speed", section));
            setup.joyZSpeed1    = myConvertToDecimal(Read("Z1 Speed", section));
            setup.joyZSpeed2    = myConvertToDecimal(Read("Z2 Speed", section));
            setup.joyZSpeed3    = myConvertToDecimal(Read("Z3 Speed", section));
            setup.joyZSpeed4    = myConvertToDecimal(Read("Z4 Speed", section));
            setup.joyZSpeed5    = myConvertToDecimal(Read("Z5 Speed", section));

            section = "Camera";
            setup.cameraindex       = Convert.ToByte(Read("Index", section));
            setup.camerarotation    = myConvertToDouble(Read("Rotation", section));
            setup.cameraPosTop      = myConvertToDouble(Read("Top Pos", section));
            setup.cameraTeachRadiusTop = myConvertToDouble(Read("Top Radius", section));
            setup.camerascalingTop  = myConvertToDouble(Read("Top Scaling", section));
            setup.cameraPosBot      = myConvertToDouble(Read("Bottom Pos", section));
            setup.cameraTeachRadiusBot = myConvertToDouble(Read("Bottom Radius", section));
            setup.camerascalingBot  = myConvertToDouble(Read("Bottom Scaling", section));
            setup.cameraToolOffsetX = myConvertToDouble(Read("X Tool Offset", section));
            setup.cameraToolOffsetY = myConvertToDouble(Read("Y Tool Offset", section));
        }

        private double myConvertToDouble(string tmp)
        {   return double.Parse(tmp.Replace(',','.'), CultureInfo.InvariantCulture.NumberFormat); }
        private decimal myConvertToDecimal(string tmp)
        { return decimal.Parse(tmp.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat); }

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
