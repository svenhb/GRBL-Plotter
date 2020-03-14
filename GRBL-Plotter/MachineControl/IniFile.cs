/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
/* 2018-12-26 Commits from RasyidUFA via Github
 * 2019-08-15 add logger
 * 2019-09-17 update settings
 * 2019-11-16 add gamepad
 * 2019-12-07 showIniSettings -> selection between actual (Properties.Settings.Default.x) and ini-file values
 * 2020-03-10 add tangential axis
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

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public IniFile(string IniPath = null)
        {
            //Logger.Trace("++++++ IniFile START ++++++");
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

        public void WriteImport()
        { 
            var setup = Properties.Settings.Default;
            string section = "Info";
            DateTime localDate = DateTime.Now;
            Write("Date", localDate.ToString(), section);
            Write("Use case info", setup.importUseCaseInfo.ToString().Replace("\r","\\r").Replace("\n", "\\n"), section);

            section = "Graphics Import";
            Write("Graph Units mm", setup.importUnitmm.ToString(), section);
            if (setup.importUnitGCode) { Write("Graph Units GCode", setup.importUnitGCode.ToString(), section); }
            Write("Bezier Segment count", setup.importBezierLineSegmentsCnt.ToString(), section);
            if (setup.importRemoveShortMovesEnable)
            {   Write("Remove Moves enable", setup.importRemoveShortMovesEnable.ToString(), section);
                Write("Remove Moves units", setup.importRemoveShortMovesLimit.ToString(), section);
            }
            if (setup.importLineDashPattern) { Write("Process Dashed Lines", setup.importLineDashPattern.ToString(), section); }
            if (setup.importPauseElement) { Write("Pause before Path", setup.importPauseElement.ToString(), section); }
            if (setup.importPausePenDown) { Write("Pause before Pen", setup.importPausePenDown.ToString(), section); }
            if (setup.importRepeatEnable)
            {   Write("Repeat Code enable", setup.importRepeatEnable.ToString(), section);
                Write("Repeat Code count", setup.importRepeatCnt.ToString(), section);
            }
            Write("Fold blocks", setup.importCodeFold.ToString(), section);
            if (setup.importSVGAddComments) { Write("Add Comments", setup.importSVGAddComments.ToString(), section); }

            Write("SVG DPI 96 enable", setup.importSVGDPI96.ToString(), section);
            if (setup.importSVGRezise)
            {   Write("SVG resize enable", setup.importSVGRezise.ToString(), section);
                Write("SVG resize units", setup.importSVGMaxSize.ToString(), section);
            }
            if (setup.importSVGNodesOnly) { Write("SVG Process nodes only", setup.importSVGNodesOnly.ToString(), section); }

            if (setup.importDXFToolIndex) { Write("DXF use color index", setup.importDXFToolIndex.ToString(), section); }
            if (setup.importDXFSwitchWhite) { Write("DXF handle white as black", setup.importDXFSwitchWhite.ToString(), section); }

            if (setup.importGroupObjects)
            {   Write("Grouping enable", setup.importGroupObjects.ToString(), section);
                Write("Grouping sort option", setup.importGroupSort.ToString(), section);
                Write("Grouping sort invert", setup.importGroupSortInvert.ToString(), section);
            }

            if (setup.importGCToolTableUse)
            {   Write("Tool table enable", setup.importGCToolTableUse.ToString(), section);
                if (setup.importGCToolDefNrUse)
                {   Write("Tool table default enable", setup.importGCToolDefNrUse.ToString(), section);
                    Write("Tool table default number", setup.importGCToolDefNr.ToString(), section);
                }
                Write("XY Feedrate from TT", setup.importGCTTXYFeed.ToString(), "GCode generation");
                Write("Spindle Speed from TT", setup.importGCTTSSpeed.ToString(), "GCode generation");
                Write("Z Values from TT", setup.importGCTTZAxis.ToString(), "GCode generation");
     //           Write("Z Down Pos from TT", setup.importGCTTZDeepth.ToString(), section);
     //           Write("Z Increment from TT", setup.importGCTTZIncrement.ToString(), section);
            }

            section = "GCode generation";
            Write("Dec Places", setup.importGCDecPlaces.ToString(), section);
            Write("Header Code", setup.importGCHeader.ToString(), section);
            Write("Footer Code", setup.importGCFooter.ToString(), section);

            Write("XY Feedrate", setup.importGCXYFeed.ToString(), section);

            Write("Spindle Speed", setup.importGCSSpeed.ToString(), section);
            Write("Spindle Use Laser", setup.importGCSpindleToggleLaser.ToString(), section);
            Write("Spindle Direction M3", setup.importGCSDirM3.ToString(), section);
            Write("Spindle Delay", setup.importGCSpindleDelay.ToString(), section);

            Write("Add Tool Cmd", setup.importGCTool.ToString(), section);
            Write("Add Tool M0", setup.importGCToolM0.ToString(), section);
            Write("Add Comments", setup.importGCAddComments.ToString(), section);

            Write("Z Enable", setup.importGCZEnable.ToString(), section);
            if (setup.importGCZEnable)
            {   Write("Z Feedrate", setup.importGCZFeed.ToString(), section);
                Write("Z Up Pos", setup.importGCZUp.ToString(), section);
                Write("Z Down Pos", setup.importGCZDown.ToString(), section);
                Write("Z Inc Enable", setup.importGCZIncEnable.ToString(), section);
                Write("Z Increment at zero", setup.importGCZIncStartZero.ToString(), section);
                Write("Z Increment", setup.importGCZIncrement.ToString(), section);
            }

            Write("Spindle Toggle", setup.importGCSpindleToggle.ToString(), section);

            if (setup.importGCPWMEnable)
            {   Write("PWM Enable", setup.importGCPWMEnable.ToString(), section);
                Write("PWM Up Val", setup.importGCPWMUp.ToString(), section);
                Write("PWM Up Dly", setup.importGCPWMDlyUp.ToString(), section);
                Write("PWM Down Val", setup.importGCPWMDown.ToString(), section);
                Write("PWM Down Dly", setup.importGCPWMDlyDown.ToString(), section);
            }

            if (setup.importGCIndEnable)
            {   Write("Individual enable", setup.importGCIndEnable.ToString(), section);
                Write("Individual PenUp", setup.importGCIndPenUp.ToString(), section);
                Write("Individual PenDown", setup.importGCIndPenDown.ToString(), section);
            }


            section = "GCode modification";
            if (setup.importGCNoArcs)
            {   Write("Convert G23 enable", setup.importGCNoArcs.ToString(), section);
                Write("Convert G23 step", setup.importGCSegment.ToString(), section);
            }

            if (setup.importGCLineSegmentation)
            {   Write("Line segmentation enable", setup.importGCLineSegmentation.ToString(), section);
                Write("Line segmentation length", setup.importGCLineSegmentLength.ToString(), section);
                Write("Line segmentation equidistant", setup.importGCLineSegmentEquidistant.ToString(), section);
                Write("Insert subroutine enable", setup.importGCSubEnable.ToString(), section);
                Write("Insert subroutine file", setup.importGCSubroutine.ToString(), section);
                Write("Insert subroutine at beginn", setup.importGCSubFirst.ToString(), section);
            }

            if (setup.importGCDragKnifeEnable)
            {   Write("Drag tool enable", setup.importGCDragKnifeEnable.ToString(), section);
                Write("Drag tool offset", setup.importGCDragKnifeLength.ToString(), section);
                Write("Drag tool percent", setup.importGCDragKnifePercent.ToString(), section);
                Write("Drag tool percent enable", setup.importGCDragKnifePercentEnable.ToString(), section);
                Write("Drag tool angle", setup.importGCDragKnifeAngle.ToString(), section);
            }

            if (setup.importGCTangentialEnable)
            {   Write("Tangential axis enable", setup.importGCTangentialEnable.ToString(), section);
                Write("Tangential axis name", setup.importGCTangentialAxis.ToString(), section);
                Write("Tangential axis angle", setup.importGCTangentialAngle.ToString(), section);
                Write("Tangential axis turn", setup.importGCTangentialTurn.ToString(), section);
            }

            if (setup.importGCCompress) { Write("Compress", setup.importGCCompress.ToString(), section); }
            if (setup.importGCRelative) { Write("Relative", setup.importGCRelative.ToString(), section); }
//            Write("Spindle use M3", setup.importGCSpindleCmd.ToString(), section);

            section = "Tool change";
            Write("Tool change enable", setup.ctrlToolChange.ToString(), section);
            if (setup.ctrlToolChange)
            {   Write("Tool remove", setup.ctrlToolScriptPut.ToString(), section);
                Write("Tool select", setup.ctrlToolScriptSelect.ToString(), section);
                Write("Tool pick up", setup.ctrlToolScriptGet.ToString(), section);
                Write("Tool probe", setup.ctrlToolScriptProbe.ToString(), section);
                Write("Tool empty", setup.ctrlToolChangeEmpty.ToString(), section);
                Write("Tool empty Nr", setup.ctrlToolChangeEmptyNr.ToString(), section);
                Write("Tool table offset X", setup.toolTableOffsetX.ToString(), section);
                Write("Tool table offset Y", setup.toolTableOffsetX.ToString(), section);
                Write("Tool table offset Z", setup.toolTableOffsetX.ToString(), section);
            }
            // Tool table: load csv and write "Tool Nr "+i
            if (setup.importGCToolTableUse || setup.ctrlToolChange)
                Write ("Tool table loaded",setup.toolTableLastLoaded.ToString(), section);
        }
        public void WriteAll(List<string> GRBLSettings)
        {
            WriteImport();
            var setup = Properties.Settings.Default;
            string section = "Info";

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
            Write("Diameter", setup.createShapeToolDiameter.ToString(), section);
            Write("Z Step", setup.createShapeToolZStep.ToString(), section);
            Write("XY Feedrate", setup.createShapeToolFeedXY.ToString(), section);
            Write("Z Feedrate", setup.createShapeToolFeedZ.ToString(), section);
            Write("Overlap", setup.createShapeToolOverlap.ToString(), section);
            Write("Spindle Speed", setup.createShapeToolSpindleSpeed.ToString(), section);

            section = "Buttons";
            Write("Button1", setup.guiCustomBtn1.ToString(), section);
            Write("Button2", setup.guiCustomBtn2.ToString(), section);
            Write("Button3", setup.guiCustomBtn3.ToString(), section);
            Write("Button4", setup.guiCustomBtn4.ToString(), section);
            Write("Button5", setup.guiCustomBtn5.ToString(), section);
            Write("Button6", setup.guiCustomBtn6.ToString(), section);
            Write("Button7", setup.guiCustomBtn7.ToString(), section);
            Write("Button8", setup.guiCustomBtn8.ToString(), section);
            Write("Button9", setup.guiCustomBtn9.ToString(), section);
            Write("Button10", setup.guiCustomBtn10.ToString(), section);
            Write("Button11", setup.guiCustomBtn11.ToString(), section);
            Write("Button12", setup.guiCustomBtn12.ToString(), section);
            Write("Button13", setup.guiCustomBtn13.ToString(), section);
            Write("Button14", setup.guiCustomBtn14.ToString(), section);
            Write("Button15", setup.guiCustomBtn15.ToString(), section);
            Write("Button16", setup.guiCustomBtn16.ToString(), section);
            Write("Button17", setup.guiCustomBtn17.ToString(), section);
            Write("Button18", setup.guiCustomBtn18.ToString(), section);
            Write("Button19", setup.guiCustomBtn19.ToString(), section);
            Write("Button20", setup.guiCustomBtn20.ToString(), section);
            Write("Button21", setup.guiCustomBtn21.ToString(), section);
            Write("Button22", setup.guiCustomBtn22.ToString(), section);
            Write("Button23", setup.guiCustomBtn23.ToString(), section);
            Write("Button24", setup.guiCustomBtn24.ToString(), section);
            Write("Button25", setup.guiCustomBtn25.ToString(), section);
            Write("Button26", setup.guiCustomBtn26.ToString(), section);
            Write("Button27", setup.guiCustomBtn27.ToString(), section);
            Write("Button28", setup.guiCustomBtn28.ToString(), section);
            Write("Button29", setup.guiCustomBtn29.ToString(), section);
            Write("Button30", setup.guiCustomBtn30.ToString(), section);
            Write("Button31", setup.guiCustomBtn31.ToString(), section);
            Write("Button32", setup.guiCustomBtn32.ToString(), section);

            section = "Joystick";
            Write("XY1 Step", setup.guiJoystickXYStep1.ToString(), section);
            Write("XY2 Step", setup.guiJoystickXYStep2.ToString(), section);
            Write("XY3 Step", setup.guiJoystickXYStep3.ToString(), section);
            Write("XY4 Step", setup.guiJoystickXYStep4.ToString(), section);
            Write("XY5 Step", setup.guiJoystickXYStep5.ToString(), section);
            Write("Z1 Step", setup.guiJoystickZStep1.ToString(), section);
            Write("Z2 Step", setup.guiJoystickZStep2.ToString(), section);
            Write("Z3 Step", setup.guiJoystickZStep3.ToString(), section);
            Write("Z4 Step", setup.guiJoystickZStep4.ToString(), section);
            Write("Z5 Step", setup.guiJoystickZStep5.ToString(), section);
            Write("XY1 Speed", setup.guiJoystickXYSpeed1.ToString(), section);
            Write("XY2 Speed", setup.guiJoystickXYSpeed2.ToString(), section);
            Write("XY3 Speed", setup.guiJoystickXYSpeed3.ToString(), section);
            Write("XY4 Speed", setup.guiJoystickXYSpeed4.ToString(), section);
            Write("XY5 Speed", setup.guiJoystickXYSpeed5.ToString(), section);
            Write("Z1 Speed", setup.guiJoystickZSpeed1.ToString(), section);
            Write("Z2 Speed", setup.guiJoystickZSpeed2.ToString(), section);
            Write("Z3 Speed", setup.guiJoystickZSpeed3.ToString(), section);
            Write("Z4 Speed", setup.guiJoystickZSpeed4.ToString(), section);
            Write("Z5 Speed", setup.guiJoystickZSpeed5.ToString(), section);
            Write("A1 Speed", setup.guiJoystickASpeed1.ToString(), section);
            Write("A2 Speed", setup.guiJoystickASpeed2.ToString(), section);
            Write("A3 Speed", setup.guiJoystickASpeed3.ToString(), section);
            Write("A4 Speed", setup.guiJoystickASpeed4.ToString(), section);
            Write("A5 Speed", setup.guiJoystickASpeed5.ToString(), section);
            Write("A1 Step", setup.guiJoystickAStep1.ToString(), section);
            Write("A2 Step", setup.guiJoystickAStep2.ToString(), section);
            Write("A3 Step", setup.guiJoystickAStep3.ToString(), section);
            Write("A4 Step", setup.guiJoystickAStep4.ToString(), section);
            Write("A5 Step", setup.guiJoystickAStep5.ToString(), section);

            section = "Camera";
            Write("Index", setup.cameraIndex.ToString(), section);
            Write("Rotation", setup.cameraRotation.ToString(), section);
            Write("Top Pos", setup.cameraPosTop.ToString(), section);
            Write("Top Radius", setup.cameraTeachRadiusTop.ToString(), section);
            Write("Top Scaling", setup.cameraScalingTop.ToString(), section);
            Write("Bottom Pos", setup.cameraPosBot.ToString(), section);
            Write("Bottom Radius", setup.cameraTeachRadiusBot.ToString(), section);
            Write("Bottom Scaling", setup.cameraScalingBot.ToString(), section);
            Write("X Tool Offset", setup.cameraToolOffsetX.ToString(), section);
            Write("Y Tool Offset", setup.cameraToolOffsetY.ToString(), section);

            section = "GamePad";
            Write("gamePadButtons0", setup.gamePadButtons0.ToString(), section);
            Write("gamePadButtons1", setup.gamePadButtons1.ToString(), section);
            Write("gamePadButtons2", setup.gamePadButtons2.ToString(), section);
            Write("gamePadButtons3", setup.gamePadButtons3.ToString(), section);
            Write("gamePadButtons4", setup.gamePadButtons4.ToString(), section);
            Write("gamePadButtons5", setup.gamePadButtons5.ToString(), section);
            Write("gamePadButtons6", setup.gamePadButtons6.ToString(), section);
            Write("gamePadButtons7", setup.gamePadButtons7.ToString(), section);
            Write("gamePadButtons8", setup.gamePadButtons8.ToString(), section);
            Write("gamePadButtons9", setup.gamePadButtons9.ToString(), section);
            Write("gamePadButtons10", setup.gamePadButtons10.ToString(), section);
            Write("gamePadButtons11", setup.gamePadButtons11.ToString(), section);
            Write("gamePadButtons12", setup.gamePadButtons12.ToString(), section);
            Write("gamePadButtons13", setup.gamePadButtons13.ToString(), section);
            Write("gamePadButtons14", setup.gamePadButtons14.ToString(), section);
            Write("gamePadButtons15", setup.gamePadButtons15.ToString(), section);
            Write("gamePadXAxis", setup.gamePadXAxis.ToString(), section);
            Write("gamePadYAxis", setup.gamePadYAxis.ToString(), section);
            Write("gamePadZAxis", setup.gamePadZAxis.ToString(), section);
            Write("gamePadRAxis", setup.gamePadRAxis.ToString(), section);
            Write("gamePadXInvert", setup.gamePadXInvert.ToString(), section);
            Write("gamePadYInvert", setup.gamePadYInvert.ToString(), section);
            Write("gamePadZInvert", setup.gamePadZInvert.ToString(), section);
            Write("gamePadRInvert", setup.gamePadRInvert.ToString(), section);
            Write("gamePadEnable", setup.gamePadEnable.ToString(), section);

            section = "GRBL Settings";
            if (GRBLSettings.Count > 0)
            {   foreach (string setting in GRBLSettings)
                {   string[] splt = setting.Split('=');
                    if (splt.Length > 1)
                        Write(splt[0], splt[1], section);
                }
            }
        }

        public string ReadUseCaseInfo()
        {   return Read("Use case info", "Info").Replace("\\r", "\r").Replace("\\n", "\n"); }

        private void setDefaults()
        {   var setup = Properties.Settings.Default;
            setup.importUnitGCode = false;
            setup.importRemoveShortMovesEnable = false;
            setup.importLineDashPattern = false;
            setup.importPauseElement = false;
            setup.importPausePenDown = false;
            setup.importRepeatEnable = false;
            setup.importSVGAddComments = false;
            setup.importSVGRezise = false;
            setup.importSVGNodesOnly = false;
            setup.importDXFToolIndex = false;
            setup.importDXFSwitchWhite = false;
            setup.importGroupObjects = false;
            setup.importGroupSortInvert = false;
            setup.importGCToolTableUse = false;
            setup.importGCToolDefNrUse = false;

            setup.importGCSpindleToggleLaser = false;
            setup.importGCTool = false;
            setup.importGCZEnable = false;
            setup.importGCPWMEnable = false;

            setup.importGCTTSSpeed = false;
            setup.importGCTTXYFeed = false;
            setup.importGCTTZAxis = false;

            setup.importGCPWMEnable = false;
            setup.importGCIndEnable = false;
            setup.importGCNoArcs = false;
            setup.importGCLineSegmentation = false;
            setup.importGCLineSegmentEquidistant = false;
            setup.importGCSubEnable = false;
            setup.importGCSubFirst = false;

            setup.importGCDragKnifeEnable = false;
            setup.importGCDragKnifePercentEnable = false;
            setup.importGCTangentialEnable = false;
            setup.importGCCompress = false;
            setup.importGCRelative = false;
        }
        public void ReadImport()
        {
            setDefaults();
            var setup = Properties.Settings.Default;
            string section = "GCode generation";
            bool tmpbool = false;
            decimal tmpdeci = 0;
            int tmpint = 0;
            string tmpstr = "";
            section = "Info";
            setup.importUseCaseInfo = Read("Use case info", section).Replace("\\r","\r").Replace("\\n","\n");
            Logger.Trace("Load Inifile");

            section = "Graphics Import";
            if (setVariable(ref tmpbool, section, "Graph Units mm"))            { setup.importUnitmm = tmpbool; }
            if (setVariable(ref tmpbool, section, "Graph Units GCode"))         { setup.importUnitGCode = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Bezier Segment count"))      { setup.importBezierLineSegmentsCnt = tmpdeci; }
            if (setVariable(ref tmpbool, section, "Process Dashed Lines"))      { setup.importLineDashPattern = tmpbool; }
            if (setVariable(ref tmpbool, section, "Remove Moves enable"))       { setup.importRemoveShortMovesEnable = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Remove Moves units"))        { setup.importRemoveShortMovesLimit = tmpdeci; }
            if (setVariable(ref tmpbool, section, "Pause before Path"))         { setup.importPauseElement = tmpbool; }
            if (setVariable(ref tmpbool, section, "Pause before Pen"))          { setup.importPausePenDown = tmpbool; }
            if (setVariable(ref tmpbool, section, "Repeat Code enable"))        { setup.importRepeatEnable = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Repeat Code count"))         { setup.importRepeatCnt = tmpdeci; }
            if (setVariable(ref tmpbool, section, "Fold blocks"))               { setup.importCodeFold = tmpbool; }
            if (setVariable(ref tmpbool, section, "Add Comments"))              { setup.importSVGAddComments = tmpbool; }

            if (setVariable(ref tmpbool, section, "SVG DPI 96 enable"))         { setup.importSVGDPI96 = tmpbool; }
            if (setVariable(ref tmpbool, section, "SVG resize enable"))         { setup.importSVGRezise = tmpbool; }
            if (setVariable(ref tmpdeci, section, "SVG resize units"))          { setup.importSVGMaxSize = tmpdeci; }
            if (setVariable(ref tmpbool, section, "SVG Process nodes only"))    { setup.importSVGNodesOnly = tmpbool; }

            if (setVariable(ref tmpbool, section, "DXF use color index"))       { setup.importDXFToolIndex = tmpbool; }
            if (setVariable(ref tmpbool, section, "DXF handle white as black")) { setup.importDXFSwitchWhite= tmpbool; }

            if (setVariable(ref tmpbool, section, "Grouping enable"))           { setup.importGroupObjects= tmpbool; }
            if (setVariable(ref tmpint, section, "Grouping sort option"))       { setup.importGroupSort= tmpint; }
            if (setVariable(ref tmpbool, section, "Grouping sort invert"))      { setup.importGroupSortInvert= tmpbool; }

            if (setVariable(ref tmpbool, section, "Tool table enable"))         { setup.importGCToolTableUse = tmpbool; }
            if (setVariable(ref tmpbool, section, "Tool table default enable")) { setup.importGCToolDefNrUse= tmpbool; }
            if (setVariable(ref tmpbool, section, "Tool table default number")) { setup.importGCToolDefNr= tmpdeci; }

            section = "GCode generation";
            if (setVariable(ref tmpdeci, section, "Dec Places"))            { setup.importGCDecPlaces= tmpdeci; }
            if (setVariable(ref tmpstr, section, "Header Code"))           { setup.importGCHeader= tmpstr; }
            if (setVariable(ref tmpstr, section, "Footer Code"))           { setup.importGCFooter= tmpstr; }

            if (setVariable(ref tmpdeci, section, "XY Feedrate"))           { setup.importGCXYFeed= tmpdeci; }
            if (setVariable(ref tmpbool, section, "XY Feedrate from TT"))   { setup.importGCTTXYFeed= tmpbool; }

            if (setVariable(ref tmpdeci, section, "Spindle Speed"))         { setup.importGCSSpeed= tmpdeci; }
            if (setVariable(ref tmpbool, section, "Spindle Speed from TT")) { setup.importGCTTSSpeed= tmpbool; }
            if (setVariable(ref tmpbool, section, "Spindle Use Laser"))     { setup.importGCSpindleToggleLaser= tmpbool; }

            if (setVariable(ref tmpbool, section, "Spindle Direction M3"))  { setup.importGCSDirM3 = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Spindle Delay"))         { setup.importGCSpindleDelay= tmpdeci; }

            if (setVariable(ref tmpbool, section, "Add Tool Cmd"))      { setup.importGCTool = tmpbool; }

            if (setVariable(ref tmpbool, section, "Add Tool M0"))       { setup.importGCToolM0 = tmpbool; }
            if (setVariable(ref tmpbool, section, "Add Comments"))      { setup.importGCAddComments = tmpbool; }

            if (setVariable(ref tmpbool, section, "Z Enable"))          { setup.importGCZEnable = tmpbool; }
            if (setVariable(ref tmpbool, section, "Z Values from TT")) { setup.importGCTTZAxis = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Z Feedrate"))        { setup.importGCZFeed = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z Up Pos"))          { setup.importGCZUp = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z Down Pos"))        { setup.importGCZDown = tmpdeci; }
   //         if (setVariable(ref tmpbool, section, "Z Down Pos from TT")){ setup.importGCTTZDeepth = tmpbool; }
            if (setVariable(ref tmpbool, section, "Z Inc Enable"))      { setup.importGCZIncEnable = tmpbool; }
            if (setVariable(ref tmpbool, section, "Z Increment at zero")){ setup.importGCZIncStartZero = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Z Increment"))        { setup.importGCZIncrement = tmpdeci; }
   //         if (setVariable(ref tmpbool, section, "Z Increment from TT")){ setup.importGCTTZIncrement = tmpbool; }

            if (setVariable(ref tmpbool, section, "Spindle Toggle"))     { setup.importGCSpindleToggle = tmpbool; }
            //          setup.importGCSpindleCmd = Convert.ToBoolean(Read("Spindle use M3", section));

            if (setVariable(ref tmpbool, section, "PWM Enable")) { setup.importGCPWMEnable = tmpbool; }
            if (setVariable(ref tmpdeci, section, "PWM Up Val")) { setup.importGCPWMUp = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "PWM Up Dly")) { setup.importGCPWMDlyUp = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "PWM Down Val")) { setup.importGCPWMDown = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "PWM Down Dly")) { setup.importGCPWMDlyDown = tmpdeci; }

            if (setVariable(ref tmpbool, section, "Individual enable")) { setup.importGCIndEnable = tmpbool; }
            if (setVariable(ref tmpstr, section, "Individual PenUp")) { setup.importGCIndPenUp = tmpstr; }
            if (setVariable(ref tmpstr, section, "Individual PenDown")) { setup.importGCIndPenDown = tmpstr; }

            section = "GCode modification";
            if (setVariable(ref tmpbool, section, "Convert G23 enable")) { setup.importGCNoArcs = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Convert G23 step")) { setup.importGCSegment = tmpdeci; }

            if (setVariable(ref tmpbool, section, "Line segmentation enable")) { setup.importGCLineSegmentation = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Line segmentation length")) { setup.importGCLineSegmentLength = tmpdeci; }
            if (setVariable(ref tmpbool, section, "Line segmentation equidistant")) { setup.importGCLineSegmentEquidistant = tmpbool; }
            if (setVariable(ref tmpbool, section, "Insert subroutine enable")) { setup.importGCSubEnable = tmpbool; }
            if (setVariable(ref tmpstr,  section, "Insert subroutine file")) { setup.importGCSubroutine = tmpstr; } 
            if (setVariable(ref tmpbool, section, "Insert subroutine at beginn")) { setup.importGCSubFirst = tmpbool; }

            if (setVariable(ref tmpbool, section, "Drag tool enable"))          { setup.importGCDragKnifeEnable = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Drag tool offset"))          { setup.importGCDragKnifeLength = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Drag tool percent"))         { setup.importGCDragKnifePercent = tmpdeci; }
            if (setVariable(ref tmpbool, section, "Drag tool percent enable"))  { setup.importGCDragKnifePercentEnable = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Drag tool angle"))           { setup.importGCDragKnifeAngle = tmpdeci; }

            if (setVariable(ref tmpbool, section, "Tangential axis enable")) { setup.importGCTangentialEnable = tmpbool; }
            if (setVariable(ref tmpstr,  section, "Tangential axis name")) { setup.importGCTangentialAxis = tmpstr; }
            if (setVariable(ref tmpdeci, section, "Tangential axis angle")) { setup.importGCTangentialAngle = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Tangential axis turn")) { setup.importGCTangentialTurn = tmpdeci; }

            if (setVariable(ref tmpbool, section, "Compress")) { setup.importGCCompress = tmpbool; }
            if (setVariable(ref tmpbool, section, "Relative")) { setup.importGCRelative = tmpbool; }

             section = "Tool change";
            if (setVariable(ref tmpbool, section, "Tool change enable")) { setup.ctrlToolChange = tmpbool; }
            if (setVariable(ref tmpstr, section, "Tool remove")) { setup.ctrlToolScriptPut = tmpstr; }
            if (setVariable(ref tmpstr, section, "Tool select")) { setup.ctrlToolScriptSelect = tmpstr; }
            if (setVariable(ref tmpstr, section, "Tool pick up")) { setup.ctrlToolScriptGet = tmpstr; }
            if (setVariable(ref tmpstr, section, "Tool probe")) { setup.ctrlToolScriptProbe = tmpstr; }
            if (setVariable(ref tmpbool, section, "Tool empty")) { setup.ctrlToolChangeEmpty = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Tool empty Nr")) { setup.ctrlToolChangeEmptyNr = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Tool table offset X")) { setup.toolTableOffsetX = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Tool table offset Y")) { setup.toolTableOffsetY = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Tool table offset Z")) { setup.toolTableOffsetZ = tmpdeci; }

            if (setVariable(ref tmpstr, section, "Tool table loaded"))
            {   setup.toolTableLastLoaded = tmpstr;
                Logger.Trace("Copy Tool table {0} to {1}", tmpstr, toolTable.defaultFileName);
                string fpath = Application.StartupPath + datapath.tools + "\\" + tmpstr;
                if (File.Exists(fpath))
                {
                    File.Copy(Application.StartupPath + datapath.tools + "\\" + toolTable.defaultFileName, Application.StartupPath + datapath.tools + "\\_beforeUseCase.csv", true);
                    File.Copy(Application.StartupPath + datapath.tools + "\\" + tmpstr, Application.StartupPath + datapath.tools + "\\" + toolTable.defaultFileName, true);
                    setup.toolTableOriginal = true;
                }
                else
                {
                    Logger.Error("File does not exist {0}", tmpstr);
                    MessageBox.Show("Tool table not found: " + fpath, "Attention");
                }
            }
            setup.Save();
        }

        public void ReadAll()
        {
            ReadImport();
            var setup = Properties.Settings.Default;
            string section = "GCode generation";
            bool tmpbool = false;
            decimal tmpdeci = 0;
            double tmpdouble = 0;
            int tmpint = 0;
            string tmpstr = "";

            section = "Machine Limits";
            if (setVariable(ref tmpbool, section, "Limit show")) { setup.machineLimitsShow = tmpbool; }
            if (setVariable(ref tmpbool, section, "Limit alarm")) { setup.machineLimitsAlarm = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Range X")) { setup.machineLimitsRangeX = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Range Y")) { setup.machineLimitsRangeY = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Range Z")) { setup.machineLimitsRangeZ = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Home X")) { setup.machineLimitsHomeX = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Home Y")) { setup.machineLimitsHomeY = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Home Z")) { setup.machineLimitsHomeZ = tmpdeci; }

   //         section = "4th axis";

            section = "Rotary axis";
            if (setVariable(ref tmpbool, section, "Enable")) { setup.rotarySubstitutionEnable = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Scale")) { setup.rotarySubstitutionScale = tmpdeci; }
            if (setVariable(ref tmpbool, section, "AxisX")) { setup.rotarySubstitutionX = tmpbool; }
            if (setVariable(ref tmpdeci, section, "Diameter")) { setup.rotarySubstitutionDiameter = tmpdeci; }
            if (setVariable(ref tmpbool, section, "SetupEnable")) { setup.rotarySubstitutionSetupEnable = tmpbool; }
            if (setVariable(ref tmpstr, section, "SetupOn"))  { setup.rotarySubstitutionSetupOn = tmpstr; }
            if (setVariable(ref tmpstr, section, "SetupOff")) { setup.rotarySubstitutionSetupOff= tmpstr; }

            section = "Tool";
            if (setVariable(ref tmpdeci, section, "Diameter")) { setup.createShapeToolDiameter = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z Step")) { setup.createShapeToolZStep = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY Feedrate")) { setup.createShapeToolFeedXY = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z Feedrate")) { setup.createShapeToolFeedZ = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Overlap")) { setup.createShapeToolOverlap = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Spindle Speed")) { setup.createShapeToolSpindleSpeed = tmpdeci; }

            section = "Buttons";
            if (setVariable(ref tmpstr, section, "Button1")) { setup.guiCustomBtn1 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button2")) { setup.guiCustomBtn2 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button3")) { setup.guiCustomBtn3 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button4")) { setup.guiCustomBtn4 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button5")) { setup.guiCustomBtn5 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button6")) { setup.guiCustomBtn6 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button7")) { setup.guiCustomBtn7 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button8")) { setup.guiCustomBtn8 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button9")) { setup.guiCustomBtn9 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button10")) { setup.guiCustomBtn10 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button11")) { setup.guiCustomBtn11 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button12")) { setup.guiCustomBtn12 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button13")) { setup.guiCustomBtn13 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button14")) { setup.guiCustomBtn14 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button15")) { setup.guiCustomBtn15 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button16")) { setup.guiCustomBtn16 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button17")) { setup.guiCustomBtn17 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button18")) { setup.guiCustomBtn18 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button19")) { setup.guiCustomBtn19 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button20")) { setup.guiCustomBtn20 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button21")) { setup.guiCustomBtn21 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button22")) { setup.guiCustomBtn22 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button23")) { setup.guiCustomBtn23 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button24")) { setup.guiCustomBtn24 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button25")) { setup.guiCustomBtn25 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button26")) { setup.guiCustomBtn26 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button27")) { setup.guiCustomBtn27 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button28")) { setup.guiCustomBtn28 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button29")) { setup.guiCustomBtn29 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button30")) { setup.guiCustomBtn30 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button31")) { setup.guiCustomBtn31 = tmpstr; }
            if (setVariable(ref tmpstr, section, "Button32")) { setup.guiCustomBtn32 = tmpstr; }

            section = "Joystick";
            if (setVariable(ref tmpdeci, section, "XY1 Step")) { setup.guiJoystickXYStep1 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY2 Step")) { setup.guiJoystickXYStep2 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY3 Step")) { setup.guiJoystickXYStep3 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY4 Step")) { setup.guiJoystickXYStep4 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY5 Step")) { setup.guiJoystickXYStep5 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z1 Step")) { setup.guiJoystickZStep1 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z2 Step")) { setup.guiJoystickZStep2 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z3 Step")) { setup.guiJoystickZStep3 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z4 Step")) { setup.guiJoystickZStep4 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z5 Step")) { setup.guiJoystickZStep5 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY1 Speed")) { setup.guiJoystickXYSpeed1 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY2 Speed")) { setup.guiJoystickXYSpeed2 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY3 Speed")) { setup.guiJoystickXYSpeed3 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY4 Speed")) { setup.guiJoystickXYSpeed4 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "XY5 Speed")) { setup.guiJoystickXYSpeed5 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z1 Speed")) { setup.guiJoystickZSpeed1 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z2 Speed")) { setup.guiJoystickZSpeed2 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z3 Speed")) { setup.guiJoystickZSpeed3 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z4 Speed")) { setup.guiJoystickZSpeed4 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "Z5 Speed")) { setup.guiJoystickZSpeed5 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A1 Step")) { setup.guiJoystickAStep1 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A2 Step")) { setup.guiJoystickAStep2 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A3 Step")) { setup.guiJoystickAStep3 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A4 Step")) { setup.guiJoystickAStep4 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A5 Step")) { setup.guiJoystickAStep5 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A1 Speed")) { setup.guiJoystickASpeed1 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A2 Speed")) { setup.guiJoystickASpeed2 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A3 Speed")) { setup.guiJoystickASpeed3 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A4 Speed")) { setup.guiJoystickASpeed4 = tmpdeci; }
            if (setVariable(ref tmpdeci, section, "A5 Speed")) { setup.guiJoystickASpeed5 = tmpdeci; }

            section = "Camera";
            if (setVariable(ref tmpint, section, "Index"))              { setup.cameraIndex = Convert.ToByte(tmpint); }
            if (setVariable(ref tmpdouble, section, "Rotation"))        { setup.cameraRotation = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "Top Pos"))         { setup.cameraPosTop = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "Top Radius"))      { setup.cameraTeachRadiusTop = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "Top Scaling"))     { setup.cameraScalingTop = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "Bottom Pos"))      { setup.cameraPosBot = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "Bottom Radius"))   { setup.cameraTeachRadiusBot = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "Bottom Scaling"))  { setup.cameraScalingBot = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "X Tool Offset"))   { setup.cameraToolOffsetX = tmpdouble; }
            if (setVariable(ref tmpdouble, section, "Y Tool Offset"))   { setup.cameraToolOffsetY = tmpdouble; }

            section = "GamePad";
            if (setVariable(ref tmpstr, section, "gamePadButtons0")) { setup.gamePadButtons0 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons1")) { setup.gamePadButtons1 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons2")) { setup.gamePadButtons2 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons3")) { setup.gamePadButtons3 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons4")) { setup.gamePadButtons4 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons5")) { setup.gamePadButtons5 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons6")) { setup.gamePadButtons6 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons7")) { setup.gamePadButtons7 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons8")) { setup.gamePadButtons8= tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons9")) { setup.gamePadButtons9 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons10")) { setup.gamePadButtons10 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons11")) { setup.gamePadButtons11 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons12")) { setup.gamePadButtons12 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons13")) { setup.gamePadButtons13 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons14")) { setup.gamePadButtons14 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadButtons15")) { setup.gamePadButtons15 = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadXAxis")) { setup.gamePadXAxis = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadYAxis")) { setup.gamePadYAxis = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadZAxis")) { setup.gamePadZAxis = tmpstr; }
            if (setVariable(ref tmpstr, section, "gamePadRAxis")) { setup.gamePadRAxis = tmpstr; }
            if (setVariable(ref tmpbool, section, "gamePadXInvert")) { setup.gamePadXInvert = tmpbool; }
            if (setVariable(ref tmpbool, section, "gamePadYInvert")) { setup.gamePadYInvert = tmpbool; }
            if (setVariable(ref tmpbool, section, "gamePadZInvert")) { setup.gamePadZInvert = tmpbool; }
            if (setVariable(ref tmpbool, section, "gamePadRInvert")) { setup.gamePadRInvert = tmpbool; }
            if (setVariable(ref tmpbool, section, "gamePadEnable")) { setup.gamePadEnable = tmpbool; }

            setup.Save();
        }

        public string showIniSettings(bool fromSettings = false)
        {
            StringBuilder tmp = new StringBuilder();
            bool fromTTZ  = false;  setVariable(ref fromTTZ, "GCode generation", "Z Values from TT");
            bool fromTTXY = false; setVariable(ref fromTTXY, "GCode generation", "XY Feedrate from TT");
            bool fromTTSS = false; setVariable(ref fromTTSS, "GCode generation", "Spindle Speed from TT"); 
            bool ZEnable  = false; setVariable(ref ZEnable, "GCode generation", "Z Enable");
            bool TTImport = false; setVariable(ref TTImport, "Graphics Import", "Tool table enable");
            bool TangEnable = false; setVariable(ref TangEnable, "GCode modification", "Tangential axis enable");

            if (fromSettings)
            {   fromTTZ = Properties.Settings.Default.importGCTTZAxis;
                fromTTXY = Properties.Settings.Default.importGCTTXYFeed;
                fromTTSS = Properties.Settings.Default.importGCTTSSpeed;
                ZEnable = Properties.Settings.Default.importGCZEnable;
                TTImport = Properties.Settings.Default.importGCToolTableUse;
                TangEnable = Properties.Settings.Default.importGCTangentialEnable;
            }
 //           string state;
            addInfo(tmp,"Process dashed: {0}\r\n", fromSettings? Properties.Settings.Default.importLineDashPattern.ToString() : Read("Process Dashed Lines", "Graphics Import"));
            addInfo(tmp,"Laser mode    : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCSpindleToggleLaser.ToString() : Read("Spindle Use Laser", "GCode generation"));
            if (!(TTImport && fromTTZ))
                tmp.AppendFormat("XY Feedrate   : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCXYFeed.ToString() : Read("XY Feedrate", "GCode generation"));
            else
                tmp.AppendFormat("XY Feedrate   : from tool table!\r\n");

            if (!(TTImport && fromTTSS))
                tmp.AppendFormat("Spindle Speed : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCSSpeed.ToString() : Read("Spindle Speed", "GCode generation"));
            else
                tmp.AppendFormat("Spindle Speed : from tool table!\r\n");

            tmp.AppendFormat("Z Enable      : {0}\r\n", ZEnable.ToString());
            if (ZEnable)
            {   if (!(TTImport && fromTTZ))
                {   tmp.AppendFormat("  Z Feedrate  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZFeed.ToString() : Read("Z Feedrate", "GCode generation"));
                    tmp.AppendFormat("  Z Save      : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZUp.ToString() : Read("Z Up Pos", "GCode generation"));
                    tmp.AppendFormat("  Z Down Pos  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZDown.ToString() : Read("Z Down Pos", "GCode generation"));
                    tmp.AppendFormat("  Z in passes : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZIncEnable.ToString() : Read("Z Inc Enable", "GCode generation"));
                    tmp.AppendFormat("  Z step/pass : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCZIncrement.ToString() : Read("Z Increment", "GCode generation"));
                }
                else
                {   tmp.AppendFormat("  Z values    : from tool table!\r\n"); }
            }
            addInfo(tmp, "Spindle Toggle: {0}\r\n", fromSettings ? Properties.Settings.Default.importGCSpindleToggle.ToString() : Read("Spindle Toggle", "GCode generation"));
            addInfo(tmp, "PWM RC-Servo  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCPWMEnable.ToString() : Read("PWM Enable", "GCode generation"));
            tmp.AppendFormat("Tangent.Enable: {0}\r\n", TangEnable.ToString());
            if (TangEnable)
            {   tmp.AppendFormat("  Tang. Axis  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCTangentialAxis.ToString() : Read("Tangential axis name", "GCode modification"));
                tmp.AppendFormat("  Tang. angle : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCTangentialAngle.ToString(): Read("Tangential axis angle", "GCode modification"));
                tmp.AppendFormat("  Tang. turn  : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCTangentialTurn.ToString() : Read("Tangential axis turn", "GCode modification"));
            }
            tmp.AppendLine();
            addInfo(tmp,"Tool table enable : {0}\r\n", fromSettings ? Properties.Settings.Default.importGCToolTableUse.ToString() : Read("Tool table enable", "Graphics Import"));
            addInfo(tmp,"Tool table apply  : {0}\r\n", fromSettings ? Properties.Settings.Default.toolTableLastLoaded.ToString() : Read("Tool table loaded", "Tool change"));
            addInfo(tmp,"Tool change enable: {0}\r\n", fromSettings ? Properties.Settings.Default.ctrlToolChange.ToString() : Read("Tool change enable", "Tool change"));
            return tmp.ToString();
        }
        private void addInfo(StringBuilder tmp, string key, string value)
        {   if (value.Length > 0)
                tmp.AppendFormat(key, value);
        }

        private double myConvertToDouble(string tmp)
        {   return double.Parse(tmp.Replace(',','.'), CultureInfo.InvariantCulture.NumberFormat); }
        private decimal myConvertToDecimal(string tmp)
        { return decimal.Parse(tmp.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat); }

        private bool setVariable(ref string variable, string section, string key)
        {
            string valString = Read(key, section);
            if (valString == "") return false;
            variable = valString;
            return true;
        }
        private bool setVariable(ref bool variable, string section, string key)
        {
            string valString = Read(key, section);
            if (valString == "") return false;
            bool tmp;
            if (bool.TryParse(valString, out tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool setVariable(ref int variable, string section, string key)
        {
            string valString = Read(key, section);
            if (valString == "") return false;
            int tmp;
            if (int.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool setVariable(ref decimal variable, string section, string key)
        {
            string valString = Read(key, section);
            if (valString == "") return false;
            decimal tmp;
            if (decimal.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out tmp))
            { variable = tmp; return true; }
            return false;
        }
        private bool setVariable(ref double variable, string section, string key)
        {
            string valString = Read(key, section);
            if (valString == "") return false;
            double tmp;
            if (double.TryParse(valString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out tmp))
            { variable = tmp; return true; }
            return false;
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
