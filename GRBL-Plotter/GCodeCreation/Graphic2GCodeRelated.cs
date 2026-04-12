/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2018-07 add line segmentation and subroutine insertion
 * 2018-08 add drag tool compensation
 * 2019-05 line 398 correct start pos. for gcodeDragCompensation if lastMovewasG0
 * 2019-06 add depth per pass for final Z depth
 * 2019-09 add dtp.gcode
 * 2020-01 add trace level loggerTraceImport to hide log of any gcode command during import
 * 2020-01 add tiny G1 moves for Pen down/up in lasermode only - to be able making dots
 * 2020-12-30 add N-Number
 * 2021-02-20 add subroutine for pen-up/down for use in tool-change scripts
 * 2021-02-28 in jobStart() line 415, call PenUp() code, to lift also servo
 * 2021-03-07 in jobStart() bug-fix: call PenUp() code only if !gcodeZApply
 * 2021-03-26 line 1130 change comments
 * 2021-04-18 function insertSubroutine line 765 add option to add pen-up /-down before/after subroutine call
 * 2021-05-07 if Length==0, no segmentation, but at begin of path.
 * 2021-05-12 new order of subroutine numbering line 300
 * 2021-06-26 gcode.setup(false) disable InsertSubroutine, LineSegmentation
 * 2021-07-27 code clean up / code quality
 * 2021-07-29 add PD, PU marker if no Pen-up/down translation is selected
 * 2022-01-02 make gcodeZApply and gcodePWMEnable public
 * 2022-02-08 For functions Arc, MoveArc and SplitArc switch to double (15 digits) for coordinates (before float with 7 digits)
 * 2022-03-25 pen-up/down individual, add PU/PD
 * 2022-04-07 line 500 add warning if Z is used as normal AND tangential axis, add 
 * 2022-07-12 line 1048, 1055, 1196 add gcodeAux1Cmd, gcodeAux2Cmd at code for relative movement
 * 2022-12-02 add function SetHeaderInfo
 * 2023-01-28 add %NM tag, to keep code-line when synthezising code
 * 2023-02-18 line 280 preventSpindle
 * 2023-03-05 l:1544/1633 f:Drill/IntermediateZ add IncrementNoToolUp
 * 2023-03-06 l:1314 f:Tool remove space in output "M0 (Tool:{0}  GroupColor:{1})\r\n"
 * 2023-03-14 l:1133 f:ClearLeadOut()	added
 * 2023-03-15 l:746 f:PenUp add F-value
 * 2023-04-19 l:1316 f:Tool  add the key-word "tool" into the comment
 * 2023-05-31 new class GcodeDefaults in vers 1.7.0.0
 * 2023-09-04 l:1626 f:GetValuesFromToolList get Down also from tool table
 * 2023-09-05 l:858 f:GetStrGCode allow also 3 digitis
 * 2023-09-23 l:1020 f:JobEnd  don't send M05 if (PreventSpindle)
 * 2023-09-24 l:1500 f:Tool also take care of !PreventSpindle
 * 2023-11-27 l:792 f:Setup add script from Properties.ListSettings.Default.importCircleToDotScript
 * 2024-03-23 l:1456 f:SplitLine split also A,B,C
 * 2024-03-24 l:1540 f:SplitArc  split also A,B,C
 * 2024-04-13 l:1815 f:SetHeaderInfo add output of path length and new format for process time
 * 2024-05-06 l:1817 f:GetHeader avoid timespan overflow
 * 2024-05-07 l:643 f:Setup check GcodeSummary.MetadataUse  instead of Properties.ListSettings.Default.importSVGMetaData)
 * 2024-05-28 l:699 f:Setup gcodeAngleStep set min to 0.01
 * 2024-07-08 l:2004 f:IntermediateZ - Z-Up at least on final pass
 * 2024-11-28 l:1810 f:GetHeader add pen up/down translation info
 * 2025-01-02 l:880 f:GetCodeNrFromGCode replace Convert.ToInt16 by int.TryParse
 * 2026-04-08 replace tooltable by toollist
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.UserControls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace GrblPlotter
{
    public static class GcodeDefaults
    {	/* Over write specific 'Properties.ListSettings.Default' values during import, e.g. SVG meta data */
        public static float FeedXY { get; set; }
        public static float FeedZ { get; set; }
        public static bool ZEnable { get; set; }
        public static float ZUp { get; set; }
        public static float ZDown { get; set; }

        public static bool PWMEnable { get; set; }
        public static float PWMUp { get; set; }
        public static float PWMZero { get; set; }
        public static float PWMDown { get; set; }
        public static bool SpindleEnable { get; set; }
        public static float SpindleSpeed { get; set; }
        public static bool LaserEnable { get; set; }
        public static float LaserPower { get; set; }

        public static string OverwriteLog { get; set; }
        private static bool ModeWasSet = false;
        private static int ValuesFound = 0;
        public static int ErrorsFound = 0;
        public static string Errors = "";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Reset()
        {
            ModeWasSet = false;
            OverwriteLog = "";
            ValuesFound = 0;
            ErrorsFound = 0;
            Errors = "";
            FeedXY = (float)Properties.Settings.Default.importGCXYFeed;
            FeedZ = (float)Properties.Settings.Default.importGCZFeed;
            ZEnable = Properties.Settings.Default.importGCZEnable;
            ZUp = (float)Properties.Settings.Default.importGCZUp;
            ZDown = (float)Properties.Settings.Default.importGCZDown;
            PWMEnable = Properties.Settings.Default.importGCPWMEnable;
            PWMUp = (float)Properties.Settings.Default.importGCPWMUp;
            PWMZero = (float)Properties.Settings.Default.importGCPWMZero;
            PWMDown = (float)Properties.Settings.Default.importGCPWMDown;
            SpindleEnable = Properties.Settings.Default.importGCSpindleToggle; // spindle on/off
            SpindleSpeed = LaserPower = (float)Properties.Settings.Default.importGCSSpeed;
            LaserEnable = Properties.Settings.Default.importGCSpindleToggleLaser; //lasermode
            Logger.Trace("GcodeDefaults Reset");
        }
        public static int Set(string cmd)
        {
            Reset();

            if (cmd.Contains(";"))
            {
                var commands = cmd.Split(';');
                foreach (string line in commands)
                { SetValue(line.Trim()); }
            }
            else if (cmd.Contains("\n"))
            {
                var commands = cmd.Split('\n');
                foreach (string line in commands)
                { SetValue(line.Trim()); }
            }
            if (OverwriteLog != "")
                OverwriteLog = "Overwritten defaults:" + OverwriteLog;
            return ValuesFound;
        }
        private static void SetValue(string line)
        {
            /*  Expected format:
             FeedXY=1234;FeedZ=321;ZUp=5;ZDown=-1.2;SpindleSpeed=789;
             */
            string[] part;
            if (line.Contains("="))
                part = line.Split('=');
            else
                part = line.Split(' ');

            if (part[0].ToLower().Contains("feedxy"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { FeedXY = val; ValuesFound++; GcodeSummary.SetFeedXY = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("feedz"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { FeedZ = val; ValuesFound++; GcodeSummary.SetFeedZ = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("zup"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { ZUp = val; ValuesFound++; GcodeSummary.SetZUp = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("zdown"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { ZDown = val; ValuesFound++; GcodeSummary.SetZDown = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("pwmup"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { PWMUp = val; ValuesFound++; GcodeSummary.SetPWMUp = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("pwmzero"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { PWMZero = val; ValuesFound++; GcodeSummary.SetPWMZero = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("pwmdown"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { PWMDown = val; ValuesFound++; GcodeSummary.SetPWMDown = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("spindlespeed") || part[0].ToLower().Contains("laserpower"))
            {
                if (float.TryParse(part[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out float val))
                { SpindleSpeed = val; ValuesFound++; GcodeSummary.SetSpindleSpeed = true; AddLog(line); }
                else
                { ErrorsFound++; Errors += line + ";"; }
            }
            else if (part[0].ToLower().Contains("zenable"))
            {
                ClearModes();
                ZEnable = true; GcodeSummary.SetZEnable = true; ValuesFound++; AddLog(line);
            }
            else if (part[0].ToLower().Contains("pwmenable"))
            {
                ClearModes();
                PWMEnable = true; GcodeSummary.SetPWMEnable = true; ValuesFound++; AddLog(line);
            }
            else if (part[0].ToLower().Contains("spindleenable"))
            {
                ClearModes();
                SpindleEnable = true; GcodeSummary.SetSpindleEnable = true; ValuesFound++; AddLog(line);
            }
            else if (part[0].ToLower().Contains("laserenable"))
            {
                ClearModes();
                LaserEnable = true; GcodeSummary.SetLaserEnable = true; ValuesFound++; AddLog(line);
            }

            void ClearModes()
            {
                if (!ModeWasSet) { ZEnable = PWMEnable = SpindleEnable = LaserEnable = false; }
                ModeWasSet = true;
            }
            void AddLog(string txt)
            { OverwriteLog += txt + "; "; }
        }

    }
    public static class GcodeSummary
    {
        //   private static string Summary = "";
        private static readonly StringBuilder Summary = new StringBuilder();

        //	private static readonly string highlightWarn = "style='background-color:yellow;'";
        //	private static readonly string highlightError = "style='background-color:pink;'";
        //    private static readonly string highlightGood = "style='background-color:lightgreen;'";
        //    private static readonly string highlightInfo = "style='background-color:lightyellow;'";

        private static readonly string highlightWarn = " class='highlightWarn'";
        private static readonly string highlightError = " class='highlightError'";
        private static readonly string highlightGood = " class='highlightGood'";
        private static readonly string highlightInfo = " class='highlightInfo'";
        private static readonly string highlightInfo2 = " class='highlightInfo line2'";
        private static readonly string fromTable = "<td class = 'highlightWarn'>" + Localization.GetString("importMessageSourceTooltable") + "</td>";
        private static readonly string fromMetadata = "<td class = 'highlightWarn'>" + Localization.GetString("importMessageSourceMetadata") + "</td>";
        public static string Filename = "";
        public static string Metadata = "";
        public static bool MetadataUse = false;

        public static bool SetFeedXY = false;
        public static bool SetFeedZ = false;
        public static bool SetZEnable = false;
        public static bool SetZUp = false;
        public static bool SetZDown = false;
        public static bool SetPWMEnable = false;
        public static bool SetPWMUp = false;
        public static bool SetPWMZero = false;
        public static bool SetPWMDown = false;
        public static bool SetSpindleEnable = false;
        public static bool SetSpindleSpeed = false;
        public static bool SetLaserEnable = false;


        public static void Reset()
        {
            Filename = "";
            Metadata = "";
            SetFeedXY = SetFeedZ = SetZEnable = SetZUp = SetZDown = false;
            SetPWMEnable = SetPWMUp = SetPWMZero = SetPWMDown = false;
            SetSpindleEnable = SetSpindleSpeed = SetLaserEnable = false;
        }
        public static string Get()
        {
            string tmpWarn;
            string tmpTT;
            string tmpMetadata;
            string tmpHL;
            string background;
            bool useValueFromToolTable = Properties.Settings.Default.importGCToolListUse;
            bool connected = Grbl.isConnected;
            int span;

            Summary.Clear();
            Summary.Append(MessageText.HtmlHeader);
            Summary.AppendFormat("<body>\r\n");
            Summary.AppendFormat("<h2>{0}</h2>\r\n", Localization.GetString("importMessageTitle"));
            Summary.AppendFormat("{0} {1}<br>", Localization.GetString("importMessageFileName"), Path.GetFileName(Filename));
            Summary.AppendFormat("{0} {1}<br>", Localization.GetString("importMessageFilePath"), Path.GetDirectoryName(Filename));

            if (Metadata != "")
            {
                if (GcodeDefaults.ErrorsFound > 0)
                    Summary.AppendFormat("<span {0}>{1} Errors:{2}<br>{3}</span><br>\r\n", highlightError, Metadata, GcodeDefaults.ErrorsFound, GcodeDefaults.Errors);
                else
                    Summary.AppendFormat("<span {0}>{1}</span><br>\r\n", highlightWarn, Metadata);
            }
            if (Properties.Settings.Default.importGCRelative)
                Summary.AppendFormat("<p {0}>{1}</p>", highlightWarn, Localization.GetString("importMessageRelMovement"));

            if (LoadProperties.MultipleImportFromForm)
            {
                Summary.AppendFormat("<span {0}>{1}</span>\r\n", highlightWarn, Localization.GetString("importMessageAddGraphic"));
            }
            if (useValueFromToolTable)
            {
                Summary.AppendFormat("<span {0}>{1} {2}</span>\r\n", highlightWarn, Localization.GetString("importMessageToolTableNeeded"), Path.GetFileName(Properties.Settings.Default.toolTableLastLoaded));
            }


            /* Start general settings */
            Summary.AppendFormat("<br><table class='optlist'>\r\n");
            Summary.AppendFormat(MessageText.HtmlColgroup3);

            string title = Localization.GetString("importMessageGeneralSettings");
            if (MyControl.UseToolList())
                title += " Device: " + MyControl.GetSelectedDeviceName();
            Summary.AppendFormat("<tr><th colspan='3'>{0}</td></tr>\r\n", title);

            tmpMetadata = "<td></td>"; tmpHL = "";
            if (SetFeedXY) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; }
            tmpTT = (useValueFromToolTable && Properties.Settings.Default.importGCTTXYFeed) ? fromTable : tmpMetadata;
            Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</tr>\r\n", tmpHL, Localization.GetString("importMessageXYFeed"), GcodeDefaults.FeedXY, tmpTT);
            if (connected)
            {
                background = ((GcodeDefaults.FeedXY > Grbl.GetSetting(110)) || (GcodeDefaults.FeedXY > Grbl.GetSetting(111))) ? highlightError : highlightGood;
                Summary.AppendFormat("<tr{0}><td colspan='3'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#110-111-and-112--xyz-max-rate-mmmin' title='Link to grbl on GitHub' target='_blank'>{1}</a> {2} / {3}</td></tr>\r\n", background, Localization.GetString("importMessageMaxXYFeed"), Grbl.GetSetting(110), Grbl.GetSetting(111));
            }
            Summary.AppendFormat("<tr class='line2'><td>{0}</td><td colspan='2'>{1}</td></tr>\r\n", Localization.GetString("importMessageGcodeHeader"), Properties.Settings.Default.importGCHeader);
            Summary.AppendFormat("<tr><td>{0}</td><td colspan='2'>{1}</td></tr>\r\n", Localization.GetString("importMessageGcodeFooter"), Properties.Settings.Default.importGCFooter);

            tmpMetadata = "<td></td>"; tmpHL = " class='line2'";
            if (SetSpindleSpeed) { tmpMetadata = fromMetadata; tmpHL = highlightInfo2; }
            tmpTT = (useValueFromToolTable && Properties.Settings.Default.importGCTTSSpeed) ? fromTable : tmpMetadata;
            string speedPower = SetLaserEnable ? Localization.GetString("importMessageLaserPower") : Localization.GetString("importMessageSpindleSpeed");

            Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</tr>\r\n", tmpHL, speedPower, GcodeDefaults.SpindleSpeed, tmpTT);
            if (connected)
            {
                background = (GcodeDefaults.SpindleSpeed > Grbl.GetSetting(30)) ? highlightError : highlightGood;
                Summary.AppendFormat("<tr{0}><td colspan='3'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#30---max-spindle-speed-rpm' title='Link to grbl on GitHub' target='_blank'>{1}</a> ($30):{2}</td></tr>\r\n", background, Localization.GetString("importMessageSpindleSpeedMax"), Grbl.GetSetting(30));
            }
            if (GcodeDefaults.LaserEnable)
            {
                Summary.AppendFormat("<tr style='background-color:yellow;'><td colspan='3'>{0}</td></tr>\r\n", Localization.GetString("importMessageLaserMode"));
                tmpMetadata = Properties.Settings.Default.importGCSDirM3 ? Localization.GetString("importMessageSpindleDirectionM3") : Localization.GetString("importMessageSpindleDirectionM4");
                Summary.AppendFormat("<tr><td><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Laser-Mode#laser-mode-operation' title='Link to grbl on GitHub' target='_blank'>{0}</a></td><td colspan='2'>{1}</td></tr>\r\n", Localization.GetString("importMessageSpindleDirection"), tmpMetadata);
                if (connected && (Grbl.GetSetting(32) < 1))
                    Summary.AppendFormat("<tr style='background-color:yellow;'><td colspan='3'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Laser-Mode' title='Link to grbl on GitHub' target='_blank'>{0}</a></td></tr>\r\n", Localization.GetString("importMessageLaserModeNok"));
            }
            Summary.AppendFormat("</table>\r\n");
            /* End general settings */

            /* Start Pen up/down translation */
            Summary.AppendFormat("<br><table class='optlist'>\r\n");
            Summary.AppendFormat(MessageText.HtmlColgroup3);
            Summary.AppendFormat("<tr><th colspan='3'><h2>{0}</h2></th></tr>\r\n", Localization.GetString("importMessagePUD"));
            if (GcodeDefaults.ZEnable)
            {
                tmpMetadata = ""; tmpHL = ""; span = 3;
                if (SetZEnable) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; span = 2; }
                Summary.AppendFormat("<tr{0}><th colspan='{1}' align='left'>{2}</th>{3}</tr>\r\n", tmpHL, span, Localization.GetString("importMessagePUDZ"), tmpMetadata);
                if (Properties.Settings.Default.importGCZPreventSpindle)
                {
                    Summary.AppendFormat("<tr style='background-color:yellow;'><td colspan='3'>{0}</td></tr>\r\n", Localization.GetString("importMessagePUDZNoSpindle"));
                }
                tmpMetadata = "<td></td>"; tmpHL = "";
                if (SetFeedZ) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; }
                tmpTT = (useValueFromToolTable && Properties.Settings.Default.importGCTTZAxis) ? fromTable : tmpMetadata;
                Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</td>\r\n", tmpHL, Localization.GetString("importMessagePUDZFeed"), GcodeDefaults.FeedZ, tmpTT);

                tmpMetadata = "<td></td>"; tmpHL = " class='line2'";
                if (SetZUp) { tmpMetadata = fromMetadata; tmpHL = highlightInfo2; }
                tmpTT = (useValueFromToolTable && Properties.Settings.Default.importGCTTZAxis) ? fromTable : tmpMetadata;
                Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</tr>\r\n", tmpHL, Localization.GetString("importMessagePUDZUp"), GcodeDefaults.ZUp, tmpTT);

                tmpMetadata = "<td></td>"; tmpHL = "";
                if (SetZDown) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; }
                tmpTT = (useValueFromToolTable && Properties.Settings.Default.importGCTTZAxis) ? fromTable : tmpMetadata;
                Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</tr>\r\n", tmpHL, Localization.GetString("importMessagePUDZDown"), GcodeDefaults.ZDown, tmpTT);
                if (Properties.Settings.Default.importGCZIncEnable)
                {
                    Summary.AppendFormat("<tr class='line2'><td>{0}</td><td>{1}</td>{2}</tr>\r\n", Localization.GetString("importMessagePUDZDepthPass"), Properties.Settings.Default.importGCZIncrement, tmpTT);
                    if (Properties.Settings.Default.importGCZIncNoZUp)
                    {
                        Summary.AppendFormat("<tr style='background-color:yellow;'><td colspan='3'>{0}</td></tr>\r\n", Localization.GetString("importMessagePUDZDepthPassNoZUp"));
                    }
                }
                if (connected)
                {
                    background = (GcodeDefaults.FeedZ > Grbl.GetSetting(112)) ? highlightWarn : highlightGood;
                    Summary.AppendFormat("<tr{0}><td colspan='3'><a href='https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#30---max-spindle-speed-rpm' title='Link to grbl on GitHub' target='_blank'>{1}</a> {2}</td></tr>\r\n", background, Localization.GetString("importMessagePUDZFeedMax"), Grbl.GetSetting(112));
                }
            }
            if (GcodeDefaults.PWMEnable)
            {
                tmpMetadata = ""; tmpHL = ""; span = 3;
                if (SetPWMEnable) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; span = 2; }
                Summary.AppendFormat("<tr{0}><th colspan='{1}'>{2}</th>{3}</tr>\r\n", tmpHL, span, Localization.GetString("importMessagePUDPWM"), tmpMetadata);
                tmpMetadata = "<td></td>"; tmpHL = "";
                if (SetPWMUp) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; }
                Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</tr>\r\n", tmpHL, Localization.GetString("importMessagePUDPWMUp"), GcodeDefaults.PWMUp, tmpMetadata);
                tmpMetadata = "<td></td>"; tmpHL = " class='line2'";
                if (SetPWMZero) { tmpMetadata = fromMetadata; tmpHL = highlightInfo2; }
                Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</tr>\r\n", tmpHL, Localization.GetString("importMessagePUDPWMZero"), GcodeDefaults.PWMZero, tmpMetadata);
                tmpMetadata = "<td></td>"; tmpHL = "";
                if (SetPWMDown) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; }
                Summary.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td>{3}</tr>\r\n", tmpHL, Localization.GetString("importMessagePUDPWMDown"), GcodeDefaults.PWMDown, tmpMetadata);
                if (connected)
                {
                    tmpWarn = ((Math.Min(GcodeDefaults.PWMUp, GcodeDefaults.PWMDown) < Grbl.GetSetting(31)) ||
                        (Math.Max(GcodeDefaults.PWMUp, GcodeDefaults.PWMDown) > Grbl.GetSetting(30))) ? highlightWarn : highlightGood;
                    Summary.AppendFormat("<tr{0}><td colspan='3'><a href='https://grbl-plotter.de/index.php?id=quick-guide&setlang=en#pwm' title='Link to grbl-Plotter website' target='_blank'>grbl PWM</a> Min.($31):{1}  Max.($30):{2}</td></tr>\r\n", tmpWarn, Grbl.GetSetting(31), Grbl.GetSetting(30));
                }
            }
            if (GcodeDefaults.SpindleEnable)
            {
                tmpMetadata = ""; tmpHL = ""; span = 3;
                if (SetSpindleEnable) { tmpMetadata = fromMetadata; tmpHL = highlightInfo; span = 2; }
                Summary.AppendFormat("<tr{0}><th colspan='{1}'>{2}</th>{3}</tr>\r\n", tmpHL, span, Localization.GetString("importMessagePUDSpindle"), tmpMetadata);
                Summary.AppendFormat("<tr><td colspan='3'>-</td></tr>\r\n");
            }
            if (Properties.Settings.Default.importGCIndEnable)
            {
                Summary.AppendFormat("<tr><th colspan='3' align='left'>{0}</th></tr>\r\n", Localization.GetString("importMessagePUDIndividual"));
                Summary.AppendFormat("<tr><td>{0}</td><td colspan='2'>{1}</td></tr>\r\n", Localization.GetString("importMessagePUDPenUp"), Properties.Settings.Default.importGCIndPenUp);
                Summary.AppendFormat("<tr><td>{0}</td><td colspan='2'>{1}</td></tr>\r\n", Localization.GetString("importMessagePUDPenDown"), Properties.Settings.Default.importGCIndPenDown);
            }
            Summary.AppendFormat("</table>\r\n");
            /* End Pen up/down translation */

            /* Start options1 */
            Summary.AppendFormat("<br><table class='optlist'>\r\n");

            StringBuilder options = new StringBuilder();
            if (Properties.Settings.Default.importSVGNodesOnly) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionNodesOnly"));
            if (Properties.Settings.Default.importDepthFromWidth)
            { options.AppendFormat("<li>{0} (Z {1} - {2})</li>\r\n", Localization.GetString("importMessageOptionZFromWidth"), Properties.Settings.Default.importDepthFromWidthMin, Properties.Settings.Default.importDepthFromWidthMax); }
            if (Properties.Settings.Default.importPWMFromWidth)
            { options.AppendFormat("<li>{0} (S {1} - {2})</li>\r\n", Localization.GetString("importMessageOptionSFromWidth"), Properties.Settings.Default.importImageSMin, Properties.Settings.Default.importImageSMax); }
            if (Properties.Settings.Default.importSVGCircleToDot) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionCircleToDot"));
            if (Properties.Settings.Default.importSVGCircleToDotZ) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionCircleRadiusToZ"));
            if (Properties.Settings.Default.importSVGCircleToDotS) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionCircleRadiusToS"));
            if (options.Length > 3)
            {
                Summary.AppendFormat("<tr><th>{0}</th></tr>\r\n", Localization.GetString("importMessageOption1"));
                Summary.Append("<tr><td align='left'>\r\n");
                Summary.Append("<ul>\r\n");
                Summary.Append(options);
                Summary.Append("</ul>\r\n");
                Summary.Append("</td></tr>\r\n");
            }
            else
                Summary.AppendFormat("<tr><th><a href='https://grbl-plotter.de/index.php?id=form-setup#pathinterpretation' title='Link to grbl-Plotter website' target='_blank'>{0}</a></th></tr>\r\n", Localization.GetString("importMessageOption1Link"));

            Summary.AppendFormat("</table>\r\n");

            /* Start options2 */
            Summary.AppendFormat("<br><table class='optlist'>\r\n");

            options.Clear();
            if (Properties.Settings.Default.importGraphicFilterEnable) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionFilter"));
            if (Properties.Settings.Default.importGraphicHatchFillEnable ||
                Properties.Settings.Default.importSVGApplyFill)
            { options.AppendFormat("<li>{0} ({1} units)</li>\r\n", Localization.GetString("importMessageOptionHatchFill"), Properties.Settings.Default.importGraphicHatchFillDistance); }
            if (Properties.Settings.Default.importGraphicClipEnable) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionClip"));
            if (Properties.Settings.Default.importGCTangentialEnable) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionTangential"));
            if (Properties.Settings.Default.importGCDragKnifeEnable) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionDragTool"));
            if (Properties.Settings.Default.importGraphicExtendPathEnable)
            { options.AppendFormat("<li>{0} ({1} units)</li>\r\n", Localization.GetString("importMessageOptionExtend"), Properties.Settings.Default.importGraphicExtendPathValue); }
            if (Properties.Settings.Default.importGraphicDevelopmentEnable) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionOutlineDevelop"));
            if (Properties.Settings.Default.importGraphicWireBenderEnable) options.AppendFormat("<li>{0}</li>\r\n", Localization.GetString("importMessageOptionWireBender"));
            if (options.Length > 3)
            {
                Summary.AppendFormat("<tr><th>{0}</th></tr>\r\n", Localization.GetString("importMessageOption2"));
                Summary.Append("<tr><td align='left'>\r\n");
                Summary.Append("<ul>\r\n");
                Summary.Append(options);
                Summary.Append("</ul>\r\n");
                Summary.Append("</td></tr>\r\n");
            }
            else
                Summary.AppendFormat("<tr><th><a href='https://grbl-plotter.de/index.php?id=form-setup#pathmodification' title='Link to grbl-Plotter website' target='_blank'>{0}</a></th></tr>\r\n", Localization.GetString("importMessageOption2Link"));


            Summary.Append("</table><br>\r\n");
            Summary.Append(MessageText.GetStreamingOptions());
            //        Summary.Append("<br>\r\n");
            Summary.Append(MessageText.GetGrblSettings());
            //       Summary.Append("<br>\r\n");
            Summary.Append("<br><br></body></html>\r\n");
            return Summary.ToString();
        }
    }


    public static partial class Gcode
    {
        public static bool LoggerTrace { get; set; } //= true;// false;
        public static bool LoggerTraceImport { get; set; } //= false;
        private static readonly StringBuilder figureString = new StringBuilder();    // tool path for pen down path
        private static readonly StringBuilder headerData = new StringBuilder();
        private static readonly StringBuilder headerMessage = new StringBuilder();
        private static string docTitle = "";
        private static string docDescription = "";

        private static int decimalPlaces = 3;
        private const string formatCode = "00";
        private static string formatNumber = "0.###";

        private static bool useValueFromToolTable = false;
        public static float GcodeXYFeed { get; set; } //= 1999;         // XY feed to apply for G1
        private static bool gcodeXYFeedToolTable = false; // from Tool Table
        private static bool gcodeComments = true;       // if true insert additional comments into GCode

        private static bool PreventSpindle { get; set; }
        //        public static float gcodeZRepitition;          // Z feed to apply for G1
        private static bool gcodeSValueEnable = false;
        private static string gcodeSValueCommand = "";


        // depth per pass
        private static XyPoint figureStart;                                 // 1st point of figure to move via G0 before pen down
        private static string figureStartAlpha;

        //    private static bool IncrementEnable = false;                                // depth per pass enabled
        //    private static bool IncrementStartAtZero = false;
        //    private static float finalZ = -2;                                   // final tool path depth
        public static bool RepeatZ
        {
            get { return OptionZAxis.IncrementEnable; }
            set { OptionZAxis.IncrementEnable = value; }
        }

        //    private static Stopwatch stopwatch = new Stopwatch();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static class Import
        {
            internal static DeviceSelection SelectedDevice = DeviceSelection.Laser;
            internal static int SelectedPlotterMode = 0;
        }
        public static class Tracker
        {
            internal static long gcodeLines = 0;              // counter for GCode lines
            internal static int gcodeFigureLines = 0;        // counter for GCode lines
            internal static double gcodeDistancePD = 0;         // counter for GCode move distance
            internal static double gcodeDistancePU = 0;         // counter for GCode move distance
            internal static float gcodeFigureDistance = 0;   // counter for GCode move distance
            internal static int gcodeDownUp = 0;             // counter for GCode Pen Down / Up
            internal static double gcodeExecutionSeconds = 0;             // counter for GCode work time
            internal static double gcodeFigureTime = 0;       // counter for GCode work time
            internal static int gcodePauseCounter = 0;       // counter for GCode pause M0 commands
            internal static int gcodeToolCounter = 0;        // counter for GCode Tools
            internal static string gcodeToolText = "";       // counter for GCode Tools

            internal static void Reset()
            {
                gcodeLines = 1;             // counter for GCode lines
                gcodeDistancePD = 0;          // counter for GCode move distance
                gcodeDistancePU = 0;          // counter for GCode move distance
                gcodeDownUp = 0;            // counter for GCode Down/Up
                gcodeExecutionSeconds = 0;              // counter for GCode work time
                gcodePauseCounter = 0;      // counter for GCode pause M0 commands
                gcodeToolCounter = 0;
                gcodeFigureTime = 0;
                gcodeFigureLines = 0;
                gcodeFigureDistance = 0;
                gcodeToolText = "";       // counter for GCode Tools
            }

        }
        internal static class Control
        {
            internal static bool InsertSubroutineEnable = false;
            internal static bool InsertSubroutinePenUpDown = false;
            internal static int SubroutineCount = 0;
            internal static int SubroutineFound = 0;   // state subroutine
            internal static string SubroutineCode = "";     //  subroutine
            internal static bool ToolChangeAddCommand = false;    // Apply tool exchange command
            internal static bool ToolChangeM0Enable = false;
            internal static bool ToolChangePerform = false;

            internal static bool gcodeCompress = false;      // reduce code by avoiding sending again same G-ToolNr and unchanged coordinates
            internal static bool GcodeRelative { get; set; } //= false;       // calculate relative coordinates for G91
            internal static bool gcodeNoArcs = false;        // replace arcs by line segments
            internal static Stopwatch stopwatch = new Stopwatch();
            internal static bool LastMovewasG0 { get; set; } //= true;
            internal static double gcodeAngleStep = 0.1;

            internal static void Reset(bool convertGraphics)
            {
                ToolChangeAddCommand = Properties.Settings.Default.importGCTool;
                ToolChangeM0Enable = Properties.Settings.Default.importGCToolM0;
                ToolChangePerform = Properties.Settings.Default.ctrlToolChange;
                InsertSubroutineEnable = convertGraphics && Properties.Settings.Default.importGCSubEnable;
                InsertSubroutinePenUpDown = convertGraphics && Properties.Settings.Default.importGCSubPenUpDown;
                SubroutineCount = 0;
                SubroutineFound = 0;
                SubroutineCode = "";
                gcodeCompress = convertGraphics && Properties.Settings.Default.importGCCompress;

                bool auxIsRelative = (Properties.Settings.Default.importGCAux1Enable && !Properties.Settings.Default.importGCAux1SumUp) ||
                                        (Properties.Settings.Default.importGCAux2Enable && !Properties.Settings.Default.importGCAux2SumUp);
                GcodeRelative = (convertGraphics && Properties.Settings.Default.importGCRelative || auxIsRelative);

                gcodeNoArcs = convertGraphics && ImportParameter.AvoidArcCommand;
                gcodeAngleStep = Math.Max(ImportParameter.ArcCircumfenceStep, 0.01);
                stopwatch = new Stopwatch();
                stopwatch.Start();
                LastMovewasG0 = true;
            }
        }

        internal static class OptionZAxis
        {
            // Using Z-Axis for Pen up down
            internal static bool Enable { get; set; } //= true;         // if true insert Z movements for Pen up/down
            internal static float Up { get; set; } //= 1.999f;          // Z-up position
            internal static float Down { get; set; } //= -1.999f;       // Z-down position
            internal static float Feed { get; set; } //= 499;           // Z feed to apply for G1
            internal static bool gcodeZFeedToolTable = false;// from Tool Table

            internal static bool IncrementEnable { get; set; }                             // depth per pass enabled
            internal static float IncrementStep { get; set; } //= 1;
            internal static bool IncrementStartAtZero { get; set; }
            internal static bool IncrementNoToolUp = false;                         // no pen-up after a pass
            internal static bool PreventSpindle { get; set; } //= 1;

            internal static float finalZ = -2;                                   // final tool path depth

            internal static bool LeadInEnable = false;
            internal static XyPoint LeadInXY = new XyPoint();
            internal static bool LeadOutEnable = false;
            internal static XyPoint LeadOutXY = new XyPoint();

            internal static void Reset(bool convertGraphics)
            {
                Enable = GcodeDefaults.ZEnable;    // Properties.ListSettings.Default.importGCZEnable;
                Up = GcodeDefaults.ZUp;         //(float)Properties.ListSettings.Default.importGCZUp;
                Down = GcodeDefaults.ZDown;     //(float)Properties.ListSettings.Default.importGCZDown;
                Feed = GcodeDefaults.FeedZ;     //(float)Properties.ListSettings.Default.importGCZFeed;
            //    gcodeZFeedToolTable = useValueFromToolTable && Properties.Settings.Default.importGCTTZAxis;
                IncrementStep = (float)Properties.Settings.Default.importGCZIncrement;             // depth per pass
                IncrementNoToolUp = Properties.Settings.Default.importGCZIncNoZUp;
                PreventSpindle = Properties.Settings.Default.importGCZPreventSpindle;
                IncrementEnable = convertGraphics && Properties.Settings.Default.importGCZIncEnable;    // do final Z in several passes?
                IncrementStartAtZero = Properties.Settings.Default.importGCZIncStartZero;
                finalZ = (float)Properties.Settings.Default.importGCZDown;                      // final Z
                LeadInEnable = false;
                LeadOutEnable = false;
            }
        }

        internal static class Spindle
        {
            // Using Spindle pwr. to switch on/off laser
            internal static bool ToggleEnable = false; // Switch on/off spindle for Pen down/up (M3/M5)
            internal static float Speed { get; set; } //= 999;    // Spindle speed to apply
            internal static string SpindleCmd { get; set; } //= "3";    // Spindle Command M3 / M4
                                                            //       internal static bool gcodeSpindleToolTable = false;     // from Tool Table
            internal static bool LasermodeEnable = false;
            internal static void Reset()
            {
                Spindle.ToggleEnable = GcodeDefaults.SpindleEnable;   // Properties.ListSettings.Default.importGCSpindleToggle;
                Spindle.Speed = GcodeDefaults.SpindleSpeed;     //(float)Properties.ListSettings.Default.importGCSSpeed;
                                                                //            Spindle.gcodeSpindleToolTable = useValueFromToolTable && Properties.Settings.Default.importGCTTSSpeed;
                Spindle.LasermodeEnable = GcodeDefaults.LaserEnable;   // Properties.ListSettings.Default.importGCSpindleToggleLaser;

                Spindle.SpindleCmd = Properties.Settings.Default.importGCSDirM3 ? "3" : "4";
                if (Properties.Settings.Default.importGCSDirM3)
                    Spindle.SpindleCmd = "3";
                else
                    Spindle.SpindleCmd = "4";
            }
        }

        public static class OptionPWM
        {
            // Using Spindle-Speed als PWM output to control RC-Servo
            internal static bool Enable { get; set; } //= false;     // Change Spindle speed for Pen down/up
            internal static float Up = 199;          // Spindle speed for Pen-up
            internal static float DlyUp { get; set; } //= 0;         // Delay to apply after Pen-up (because servo is slow)
            internal static float Down { get; set; }        // Spindle speed for Pen-down
            internal static float DlyDown { get; set; } //= 0;       // Delay to apply after Pen-down (because servo is slow)

            internal static void Reset()
            {
                OptionPWM.Enable = GcodeDefaults.PWMEnable;   // Properties.ListSettings.Default.importGCPWMEnable;
                OptionPWM.Up = GcodeDefaults.PWMUp;           // (float)Properties.ListSettings.Default.importGCPWMUp;
                OptionPWM.DlyUp = (float)Properties.Settings.Default.importGCPWMDlyUp;
                OptionPWM.Down = GcodeDefaults.PWMDown;         // (float)Properties.ListSettings.Default.importGCPWMDown;
                OptionPWM.DlyDown = (float)Properties.Settings.Default.importGCPWMDlyDown;
            }
        }

        public static class DepthFromWidth
        {
            internal static float ZMin { get; set; }
            internal static float ZMax { get; set; }
            internal static float SMin { get; set; }
            internal static float SMax { get; set; }
            internal static bool SEnable { get; set; }

            internal static void Reset()
            {
                DepthFromWidth.ZMin = (float)Properties.Settings.Default.importDepthFromWidthMin;
                DepthFromWidth.ZMax = (float)Properties.Settings.Default.importDepthFromWidthMax;
                DepthFromWidth.SMin = (float)Properties.Settings.Default.importImageSMin;
                DepthFromWidth.SMax = (float)Properties.Settings.Default.importImageSMax;
                DepthFromWidth.SEnable = Properties.Settings.Default.importPWMFromWidth;
            }
        }

        public static class OptionIndividual
        {
            internal static bool Enable = false;// Use individual Pen down/up
            internal static string Up = "";
            internal static string Down = "";
            internal static void Reset()
            {
                OptionIndividual.Enable = Properties.Settings.Default.importGCIndEnable;
                OptionIndividual.Up = Properties.Settings.Default.importGCIndPenUp;
                OptionIndividual.Down = Properties.Settings.Default.importGCIndPenDown;
            }
        }

        public static class ModificationSegmentation
        {
            internal static bool Enable;
            internal static float Length;
            internal static bool Equidistant;
            internal static bool SubroutineOnPathStart;
            internal static void Reset(bool convertGraphics)
            {
                ModificationSegmentation.Enable = false;
                if (convertGraphics)
                {
                    ModificationSegmentation.Enable = Properties.Settings.Default.importGCLineSegmentation;
                    ModificationSegmentation.Length = (float)Properties.Settings.Default.importGCLineSegmentLength;
                    ModificationSegmentation.Equidistant = Properties.Settings.Default.importGCLineSegmentEquidistant;
                    ModificationSegmentation.SubroutineOnPathStart = Properties.Settings.Default.importGCSubFirst;
                }
            }
        }

        public static class ModificationTangential
        {
            internal static bool Enable = false;
            internal static string Name = "C";
            internal static double Angle = 0;
            internal static float AngleDevi = 0;
            //    private static float gcodeTangentialAngleLast = 0;
            internal static string Command = "";
            internal static void Reset()
            {
                Enable = Properties.Settings.Default.importGCTangentialEnable;
                Name = Properties.Settings.Default.importGCTangentialAxis;
                Angle = 0;//gcodeTangentialAngleLast

                AngleDevi = (float)Properties.Settings.Default.importGCTangentialAngleDevi;
                Command = figureStartAlpha = "";
            }
        }

        public static class ModificationAux
        {
            internal static bool Value1Enable = false;
            internal static string Value1Name = "A";
            //    private static float gcodeAuxiliaryValue1Distance = 1;
            internal static string Value1Command = "";

            internal static bool Value2Enable = false;
            internal static string Value2Name = "A";
            //    private static float gcodeAuxiliaryValue2Distance = 1;
            internal static string Value2Command = "";
            internal static void Reset()
            {
                ModificationAux.Value1Enable = Properties.Settings.Default.importGCAux1Enable;
                ModificationAux.Value2Enable = Properties.Settings.Default.importGCAux2Enable;
                ModificationAux.Value1Name = Properties.Settings.Default.importGCAux1Axis;
                ModificationAux.Value2Name = Properties.Settings.Default.importGCAux2Axis;
            }
        }

        public static string GetSettings()
        {
            string tmp = "";
            tmp += string.Format("XYFeed: {0}, ", GcodeXYFeed);
            if (Control.ToolChangeAddCommand)
                tmp += "ToolChange, ";
            if (OptionZAxis.Enable)
            {
                tmp += string.Format("ZFeed: {0}, Up: {1}, Down: {2}, Spindle M{3} S{4}", OptionZAxis.Feed, OptionZAxis.Up, OptionZAxis.Down, Spindle.SpindleCmd, Spindle.Speed);
                if (OptionZAxis.IncrementEnable)
                    tmp += string.Format("ZStep: {0}, ", OptionZAxis.IncrementStep);
            }
            if (Spindle.ToggleEnable)
                tmp += string.Format(" SpindleToggle M{0} S{1} ", Spindle.SpindleCmd, Spindle.Speed);
            if (OptionPWM.Enable)
                tmp += " PWM ";
            return tmp;
        }

        public static void Setup(bool convertGraphics)	// true for SVG, DXF, HPGL, CSV		false for shape,
        {
            Logger.Trace("Setup - Reset GCode options");
            if (!GcodeSummary.MetadataUse)//            Properties.ListSettings.Default.importSVGMetaData)
                GcodeDefaults.Reset();

            decimalPlaces = (int)Properties.Settings.Default.importGCDecPlaces;
            SetDecimalPlaces(decimalPlaces);
            GcodeXYFeed = GcodeDefaults.FeedXY;		//(float)Properties.ListSettings.Default.importGCXYFeed;

            useValueFromToolTable = Properties.Settings.Default.importGCToolListUse;
            gcodeXYFeedToolTable = useValueFromToolTable && Properties.Settings.Default.importGCTTXYFeed;

            gcodeComments = Properties.Settings.Default.importGCAddComments;
            Spindle.Reset();
            OptionZAxis.Reset(convertGraphics);
            OptionPWM.Reset();
            DepthFromWidth.Reset();

            OptionIndividual.Reset();
            Control.Reset(convertGraphics);
            Tracker.Reset();

            remainingC = (float)Properties.Settings.Default.importGCLineSegmentLength;

            ModificationSegmentation.Reset(convertGraphics);
            ModificationTangential.Reset();
            ModificationAux.Reset();

            // prevent spindle/laser-on on job start
            //   PreventSpindle = PreventSpindle && !ToggleEnable; //(&& dragtool && ...)
            PreventSpindle = OptionZAxis.PreventSpindle || Spindle.ToggleEnable; //(&& dragtool && ...)

            docTitle = "";
            docDescription = "";

            headerData.Clear();
            headerMessage.Clear();
            figureString.Clear();                                                           // 

            lastx = -0.001; lasty = -0.001; lastz = +0.001; lasta = 0;

            if (Control.GcodeRelative)
            { lastx = 0; lasty = 0; }

            //      stopwatch = new Stopwatch();
            //      stopwatch.Start();

            gcodeSValueCommand = "";
            ModificationTangential.Command = "";
            ModificationAux.Value1Command = "";
            ModificationAux.Value2Command = "";

            MyControl.OverideGcodeSetup();

            if ((Control.InsertSubroutineEnable && ModificationSegmentation.Enable) || Control.ToolChangeAddCommand || Control.ToolChangePerform ||
                (Properties.Settings.Default.importSVGCircleToDot && (Properties.Settings.Default.importCircleToDotScriptCount > 0)))
            {
                bool insertSubroutine = false;
                if (Control.ToolChangeAddCommand)
                {
                    insertSubroutine = false;
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptPut);
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptSelect);
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptGet);
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptProbe);
                }
                if (Control.InsertSubroutineEnable && ModificationSegmentation.Enable && FileContainsSubroutineCall(Properties.Settings.Default.importGCSubroutine))
                { insertSubroutine = true; }
                if (Properties.Settings.Default.importSVGCircleToDot && (Properties.Settings.Default.importCircleToDotScriptCount > 0))
                { insertSubroutine = true; }
                if (insertSubroutine)
                {
                    double tmp_lastz = lastz;
                    StringBuilder tmp = new StringBuilder();
                    Logger.Trace("setup create PenUp/Down subroutine gcodeInsertSubroutine:{0} gcodeToolChange:{1} ctrlToolChange:{2}", Control.InsertSubroutineEnable, Control.ToolChangeAddCommand, Control.ToolChangePerform);
                    PenUp(tmp, "");
                    Control.SubroutineCode += "\r\n(subroutine)\r\nO90 (Pen up)\r\n";
                    Control.SubroutineCode += tmp.ToString();
                    Control.SubroutineCode += "M99\r\n";
                    AddToHeader("Add SR 90 Pen up");
                    tmp.Clear();
                    PenDown(tmp, "");
                    Control.SubroutineCode += "\r\n(subroutine)\r\nO92 (Pen down)\r\n";
                    Control.SubroutineCode += tmp.ToString();
                    Control.SubroutineCode += "M99\r\n";
                    AddToHeader("Add SR 92 Pen down");

                    if (Properties.Settings.Default.importSVGCircleToDot)   // && File.Exists(Properties.ListSettings.Default.importCircleToDotScript))
                    {
                        SetSubroutine(Properties.Settings.Default.importCircleToDotScript, 95);
                    }

                    lastz = tmp_lastz;

                    if (OptionPWM.Enable)
                    {
                        tmp.Clear();
                        SetPwm(tmp, (float)Properties.Settings.Default.importGCPWMZero, (float)Properties.Settings.Default.importGCPWMDlyDown);
                        Control.SubroutineCode += "\r\n(subroutine)\r\nO91 (Pen zero)\r\n";
                        Control.SubroutineCode += tmp.ToString();
                        Control.SubroutineCode += "M99\r\n";
                        AddToHeader("Add SR 91 Pen zero");
                        tmp.Clear();
                        SetPwm(tmp, (float)Properties.Settings.Default.importGCPWMP93, (float)Properties.Settings.Default.importGCPWMDlyP93);
                        Control.SubroutineCode += "\r\n(subroutine)\r\nO93 (" + Properties.Settings.Default.importGCPWMTextP93 + ")\r\n";
                        Control.SubroutineCode += tmp.ToString();
                        Control.SubroutineCode += "M99\r\n";
                        AddToHeader("Add SR 93 " + Properties.Settings.Default.importGCPWMTextP93);
                        tmp.Clear();
                        SetPwm(tmp, (float)Properties.Settings.Default.importGCPWMP94, (float)Properties.Settings.Default.importGCPWMDlyP94);
                        Control.SubroutineCode += "\r\n(subroutine)\r\nO94 (" + Properties.Settings.Default.importGCPWMTextP94 + ")\r\n";
                        Control.SubroutineCode += tmp.ToString();
                        Control.SubroutineCode += "M99\r\n";
                        AddToHeader("Add SR 94 " + Properties.Settings.Default.importGCPWMTextP94);
                    }
                    else if (OptionZAxis.Enable)
                    {
                        tmp.Clear();
                        Control.SubroutineCode += "\r\n(subroutine)\r\nO91 (Pen zero)\r\n";
                        Control.SubroutineCode += "G00 Z0.000";
                        Control.SubroutineCode += "M99\r\n";
                        AddToHeader("Add SR 91 Pen zero");
                        tmp.Clear();
                        //    PenUp(tmp, "");
                        Control.SubroutineCode += "\r\n(subroutine)\r\nO93 (Far up)\r\n";
                        Control.SubroutineCode += "G01 Z" + FrmtNum(Properties.Settings.Default.DevicePlotterZP93);
                        Control.SubroutineCode += "M99\r\n";
                        AddToHeader("Add SR 93 Pen far up");
                        tmp.Clear();
                        //    PenDown(tmp, "");
                        Control.SubroutineCode += "\r\n(subroutine)\r\nO93 (Down stir)\r\n";
                        Control.SubroutineCode += "G01 Z" + FrmtNum(Properties.Settings.Default.DevicePlotterZP94);
                        Control.SubroutineCode += "M99\r\n";
                        AddToHeader("Add SR 94 Pen stir");
                    }
                }
            }
        }

        private static bool FileContainsSubroutineCall(string filename)
        {
            if (File.Exists(filename))
            {
                string subroutine = File.ReadAllText(filename);
                if (subroutine.Contains("M98"))     // subroutine call
                {
                    Logger.Trace("fileContainsSubroutineCall {0}  found subroutine call", filename);
                    return true;
                }
            }
            return false;
        }

        public static bool ReduceGCode
        {
            get { return Control.gcodeCompress; }
            set
            {
                Control.gcodeCompress = value;
                SetDecimalPlaces((int)Properties.Settings.Default.importGCDecPlaces);
            }
        }

        public static bool IsEqual(double vala, double valb)
        { return (Math.Round(vala, decimalPlaces) == Math.Round(valb, decimalPlaces)); }

        public static void SetDecimalPlaces(int num)
        {
            formatNumber = "0.";
            if (Control.gcodeCompress)
                formatNumber = formatNumber.PadRight(num + 2, '#'); //'0'
            else
                formatNumber = formatNumber.PadRight(num + 2, '0'); //'0'
        }

        // get GCode one or two digits
        public static int GetCodeNrFromGCode(char code, string tmp)
        {
            string cmdG = GetStrGCode(code, tmp);       // find number string
            if (cmdG.Length > 0)
            {
                int retval = -1;
                if (!int.TryParse(cmdG.Substring(1), out retval))
                { Logger.Error("int.TryParse nok: code:'{0}' string:'{1}'  convert:'{2}'", code, tmp, cmdG.Substring(1)); }
                return retval;
            }
            return -1;
        }
        public static string GetStrGCode(char code, string tmp)
        {
            if (string.IsNullOrEmpty(tmp)) return "";
            int cmt = tmp.IndexOf("(");
            if (cmt == 0)
                return "";                      // nothing to do
            if (cmt >= 0)
                tmp = tmp.Substring(0, cmt);     // don't check inside comment
            var cmdG = Regex.Matches(tmp, code + "\\d{1,3}"); // find code and 1 or 2 digits - switch to ",3"
            if (cmdG.Count > 0)
            { return cmdG[0].ToString(); }
            return "";
        }

        public static string GetStringValue(char code, string tmp)
        {
            var cmdG = Regex.Matches(tmp, code + "-?\\d+(.\\d+)?");
            if (cmdG.Count > 0)
            { return cmdG[cmdG.Count - 1].ToString(); }
            return "";
        }

        public static string FrmtCode(int number)      // convert int to string using format pattern
        { return number.ToString(formatCode); }

        public static string FrmtNum(float number)     // convert float to string using format pattern
        { return number.ToString(formatNumber); }
        public static string FrmtNum(double number)    // convert double to string using format pattern
        { return number.ToString(formatNumber); }
        public static string FrmtNum(decimal number)   // convert decimal to string using format pattern
        { return number.ToString(formatNumber); }

        //    private static StringBuilder secondMove = new StringBuilder();
        public static bool ApplyXYFeedRate { get; set; } //= true; // apply XY feed after each Pen-move

        public static void Pause(StringBuilder gcodeValue, string cmt)  // add M0 (cmt)
        {
            if (gcodeValue != null)
            {
                if (string.IsNullOrEmpty(cmt)) cmt = "";
                else if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
                gcodeValue.AppendFormat("M{0} {1}\r\n", FrmtCode(0), cmt);
                Tracker.gcodeLines++;
                Tracker.gcodePauseCounter++;
            }
        }

        public static void SpindleOn(StringBuilder gcodeValue, string cmt)
        {
            if (gcodeValue != null)
            {
                if (string.IsNullOrEmpty(cmt)) cmt = "";
                //        if (Spindle.gcodeSpindleToolTable && gcodeComments) { cmt += " spindle speed from tool-table"; }
                if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
                if (Spindle.LasermodeEnable)  // in SpindleOn
                    gcodeValue.AppendFormat("S{0} {1}\r\n", Spindle.Speed, cmt);
                else
                {
                    gcodeValue.AppendFormat("M{0} S{1} {2}\r\n", Spindle.SpindleCmd, Spindle.Speed, cmt);
                    Tracker.gcodeLines++;
                    double delay = (double)Properties.Settings.Default.importGCSpindleDelay;
                    if (delay > 0)
                    {
                        string tmp = gcodeComments ? "( Delay )" : "";
                        gcodeValue.AppendFormat("G{0} P{1} {2}\r\n", FrmtCode(4), delay, tmp);
                        Tracker.gcodeLines++;
                    }
                }
            }
        }

        public static void SpindleOff(StringBuilder gcodeValue, string cmt)
        {
            if (gcodeValue != null)
            {
                if (string.IsNullOrEmpty(cmt)) cmt = "";
                if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
                if (Spindle.LasermodeEnable)  // in SpindleOff
                    gcodeValue.AppendFormat("S{0} {1}\r\n", 0, cmt);
                else
                    gcodeValue.AppendFormat("M{0} {1}\r\n", FrmtCode(5), cmt);
                Tracker.gcodeLines++;
            }
        }

        public static void SetPwm(StringBuilder gcodeValue, float pwm, float delay)
        {
            if (gcodeValue != null)
            {
                gcodeValue.AppendFormat("M{0} S{1}\r\n", Spindle.SpindleCmd, pwm);
                if (delay > 0)
                    gcodeValue.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(delay));
            }
        }

        public static void JobStart(StringBuilder gcodeValue, string cmto)
        {
            if (MyControl.UseToolList())
            { JobStartDevice(gcodeValue, cmto); return; }

            bool penup = false;
            if (gcodeValue != null)
            {
                string cmt = cmto;
                if (!gcodeComments) cmt = "";

                if (OptionZAxis.Enable)    // pen up
                {
                    if (ModificationTangential.Enable && (ModificationTangential.Name == "Z"))
                        gcodeValue.AppendFormat("( {0}-3001: Z is used as axis AND as tangential axis )\r\n", CodeMessage.Warning);
                    if ((ModificationAux.Value1Enable && (ModificationAux.Value1Name == "Z")) ||
                        (ModificationAux.Value2Enable && (ModificationAux.Value2Name == "Z")))
                        gcodeValue.AppendFormat("({0}-3002: Z is used as axis AND as auxiliary axis )\r\n", CodeMessage.Warning);

                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Z-Axis");
                    float tmpZUp = (float)Properties.Settings.Default.importGCZUp;
                    double z_relative = tmpZUp - lastz;
                    if (OptionZAxis.gcodeZFeedToolTable && gcodeComments) { cmt = cmto + " Z feed from tool-table"; }
                    if (cmt.Length > 0) { cmt = " (" + cmt + ")"; }
                    if (Control.GcodeRelative)
                        gcodeValue.AppendFormat("G{0} Z{1}{2}\r\n", FrmtCode(0), FrmtNum(z_relative), cmt); // use G0 without feedrate
                    else
                        gcodeValue.AppendFormat("G{0} Z{1}{2}\r\n", FrmtCode(0), FrmtNum(tmpZUp), cmt); // use G0 without feedrate
                    Tracker.gcodeExecutionSeconds += 60 * Math.Abs((tmpZUp - OptionZAxis.Down) / Math.Max(OptionZAxis.Feed, 10));
                    Tracker.gcodeLines++;
                }
                else
                {
                    PenUp(gcodeValue, "PU");
                    penup = true;
                }

                if (OptionZAxis.Enable || Spindle.ToggleEnable)
                {
                    if (Spindle.LasermodeEnable)  // in jobStart
                    {
                        if (gcodeComments) cmt = " (" + cmto + " lasermode )";
                        else cmt = "(JobStart)";
                        if (!penup)
                            gcodeValue.AppendFormat("M{0} S{1}{2}\r\n", Spindle.SpindleCmd, 0, cmt); // switch on laser with power=0
                    }
                    else
                    {
                        if (!Control.ToolChangeAddCommand)   // spindle on if no tool change
                        {
                            if (gcodeComments) cmt = " (" + cmto + " spindle )";
                            else cmt = "StartJob";
                            if (!PreventSpindle)
                            { SpindleOn(gcodeValue, cmt); }
                            else
                            { gcodeValue.AppendFormat("( {0}-3003: Spindle stays off )\r\n", CodeMessage.Attention); }
                        }
                    }
                }

                if (Properties.Settings.Default.importSVGCircleToDot && (Properties.Settings.Default.importCircleToDotScriptCount > 0))
                    CallSubroutine(gcodeValue, 95, "refresh stamp");
            }
        }
        public static void JobEnd(StringBuilder gcodeValue, string cmt)
        {
            if (gcodeValue != null)
            {
                if (gcodeComments) cmt = " (" + cmt + ")";
                else cmt = "(EndJob)";
                if (OptionZAxis.Enable || Spindle.ToggleEnable)
                {
                    if (!PreventSpindle)
                    { gcodeValue.AppendFormat("M{0}{1}\r\n", FrmtCode(5), cmt); }
                }
            }
        }

        private static StringBuilder tmpString = new StringBuilder();

        public static void PenDown(StringBuilder gcodeValue, string cmto)
        {
            if (MyControl.UseToolList())
            { PenDownDevice(gcodeValue, cmto); return; }
            if (LoggerTraceImport) Logger.Trace("    PenDown gcodeZDown:{0} lastz:{1}", OptionZAxis.Down, lastz);

            tmpString = new StringBuilder();
            string cmt = cmto;
            bool penDownApplied = false;
        //    if (Properties.Settings.Default.importGCTTZAxis && gcodeComments && useValueFromToolTable) { cmt += " Z values from tool-table"; }
            if (Control.GcodeRelative) { cmt += string.Format("rel {0}", lastz); }
            if (cmt.Length > 0) { cmt = string.Format("({0})", cmt); }

            ApplyXYFeedRate = true;     // apply XY FeedXY Rate after each PenDown command (not just after Z-axis)

            if (!(OptionZAxis.Enable && OptionZAxis.IncrementEnable))  // if true, do action in intermediateZ
            {
                if (Spindle.ToggleEnable && !Spindle.LasermodeEnable)   // 1st spindel, then Z
                {
                    if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Spindle-On");
                    SpindleOn(tmpString, cmto); penDownApplied = true;
                }
            }
            if (OptionZAxis.Enable)
            {
                if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Z-Axis");
                penDownApplied = true;
                if (OptionZAxis.IncrementEnable)
                {
                    figureString.Clear();   // Router down: a new figure will start
                    Tracker.gcodeFigureTime = 0;
                    Tracker.gcodeFigureLines = 0;  //
                    Tracker.gcodeFigureDistance = 0;
                    if (LoggerTraceImport) Logger.Trace("    figureString.Clear()");
                }
                else
                {
                    double z_relative = OptionZAxis.Down - lastz;
                    if (Math.Abs(z_relative) > 0)
                    {
                        if (Control.GcodeRelative)
                            tmpString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(z_relative), OptionZAxis.Feed, cmt);
                        else
                        {
                            if (OptionZAxis.LeadInEnable)
                            { tmpString.AppendFormat("G{0} X{1} Y{2} Z{3} F{4} {5}\r\n", FrmtCode(1), FrmtNum(OptionZAxis.LeadInXY.X), FrmtNum(OptionZAxis.LeadInXY.Y), FrmtNum(OptionZAxis.Down), OptionZAxis.Feed, cmt); }
                            else
                            { tmpString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(OptionZAxis.Down), OptionZAxis.Feed, cmt); }
                        }
                        Tracker.gcodeExecutionSeconds += 60 * Math.Abs((OptionZAxis.Up - OptionZAxis.Down) / Math.Max(OptionZAxis.Feed, 10));
                        Tracker.gcodeLines++;
                        //						penDownApplied = true;
                    }
                }
                lastg = 1; lastf = OptionZAxis.Feed;
                lastz = OptionZAxis.Down;
            }
            if (!(OptionZAxis.Enable && OptionZAxis.IncrementEnable))  // if true, do action in intermediateZ
            {
                if (Spindle.LasermodeEnable && !OptionPWM.Enable)  // 1st Z, then Laser added 2023-06-26 
                {
                    gcodeValue.AppendFormat("M{0} S{1:0} {2}\r\n", Spindle.SpindleCmd, Spindle.Speed, cmt);  //2022-03-15
                    if ((!OptionZAxis.Enable) && (Control.LastMovewasG0))
                    {
                        // %NM tag, to keep code-line when synthezising code
                        //        gcodeValue.AppendFormat("G91 G1 X0.001 F{0} ( %NM use Laser mode)\r\n", GcodeXYFeed);
                        //        gcodeValue.AppendFormat("G91 G1 X-0.001     ( %NM G1 move to activate laser)\r\n");
                        if (!Control.GcodeRelative)
                        { gcodeValue.AppendFormat("G90 ( %NM )\r\n"); }
                    }
                    penDownApplied = true;
                }
                if (OptionPWM.Enable)
                {
                    if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Servo control");
                    tmpString.AppendFormat("M{0} S{1} {2}\r\n", Spindle.SpindleCmd, OptionPWM.Down, cmt);
                    if (OptionPWM.DlyDown > 0)
                        tmpString.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(OptionPWM.DlyDown));
                    Tracker.gcodeExecutionSeconds += OptionPWM.DlyDown;
                    Tracker.gcodeLines++;
                    penDownApplied = true;
                }
                if (OptionIndividual.Enable)
                {
                    if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Individual Cmd");
                    string[] commands = OptionIndividual.Down.Split(';');
                    foreach (string cmd in commands)
                    { tmpString.AppendFormat("{0} {1}\r\n", cmd.Trim(), cmt); }     // 2022-03-25 add {1}
                    penDownApplied = true;
                }
            }
            Tracker.gcodeDownUp++;
            if (!penDownApplied)
                tmpString.AppendLine(" (PD)");  // mark pen down
                                                //   lastCharCount = tmpString.Length;
            gcodeValue?.Append(tmpString);

        }

        public static void PenUp(StringBuilder gcodeValue, string cmto)
        {
            if (MyControl.UseToolList())
            { PenUpDevice(gcodeValue, cmto); return; }

            if (gcodeValue == null)
            { return; }

            string cmt = cmto;
            string comment = "";
            bool penUpApplied = false;
            if (Control.GcodeRelative) { cmt += string.Format("rel {0}", lastz); }
            if (cmt.Length > 0) { comment = string.Format("({0})", cmt); }      // 2022-03-25 move up

            if (!(OptionZAxis.Enable && OptionZAxis.IncrementEnable))  // if true, do action in intermediateZ
            {
                if (OptionIndividual.Enable)
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Individual Cmd");
                    string[] commands = OptionIndividual.Up.Split(';');
                    foreach (string cmd in commands)
                    { gcodeValue.AppendFormat("{0} {1}\r\n", cmd.Trim(), comment); }    // 2022-03-25 add {1}
                    penUpApplied = true;
                }

                if (OptionPWM.Enable)
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Servo control");
                    //    if (cmt.Length > 0) { comment = string.Format("({0})", cmt); }		// 2022-03-25 move up
                    gcodeValue.AppendFormat("M{0} S{1} {2}\r\n", Spindle.SpindleCmd, OptionPWM.Up, comment);
                    if (OptionPWM.DlyUp > 0)
                        gcodeValue.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(OptionPWM.DlyUp));
                    Tracker.gcodeExecutionSeconds += OptionPWM.DlyUp;
                    Tracker.gcodeLines++;
                    penUpApplied = true;
                }

                if (Spindle.LasermodeEnable && !OptionPWM.Enable)  // 1st Z, then Laser
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Laser-Off");
                    //    SpindleOff(gcodeValue, cmto);
                    gcodeValue.AppendFormat("M{0} S0 ({1} Lasermode: S0 instead of M5 to switch laser off)\r\n", Spindle.SpindleCmd, cmt);  //2022-03-15
                    /*     if ((!Enable) && (LastMovewasG0))
                         {
                             gcodeValue.AppendFormat("G91 G1 X0.001 F{0} ( %NM use Laser mode)\r\n", GcodeXYFeed);
                             gcodeValue.AppendFormat("G91 G1 X-0.001     ( %NM G1 move to activate laser)\r\n");
                             if (!GcodeRelative)
                             { gcodeValue.AppendFormat("G90 ( %NM )\r\n"); }
                             //        Move(gcodeValue, 1, lastx + 0.001f, lasty + 0.001f, false, "");
                             //        Move(gcodeValue, 1, lastx - 0.001f, lasty - 0.001f, false, "");
                         }*/ // removed  2023-06-26  not needed for pen-up
                    penUpApplied = true;
                }
            }
            if (OptionZAxis.Enable)
            {
                if (OptionZAxis.IncrementEnable)
                {
                    if (figureString.Length > 0)
                        IntermediateZ(gcodeValue);      // figure finished, repeat code for several depth++ per pass
                    else
                        Drill(gcodeValue);
                }
                else
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Z-Axis");

                    double z_relative = OptionZAxis.Up - lastz;
                    if (Math.Abs(z_relative) > 0)
                    {
                        if (OptionZAxis.gcodeZFeedToolTable && gcodeComments) { cmt += " Z feed from tool-table"; }
                        if (cmt.Length > 0) { comment = string.Format("({0})", cmt); }

                        if (Control.GcodeRelative)
                        { gcodeValue.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(z_relative), comment); }// use G0 without feedrate
                        else
                        {
                            if (OptionZAxis.LeadOutEnable)
                            { gcodeValue.AppendFormat("G{0} X{1} Y{2} Z{3} F{4} {5}\r\n", FrmtCode(1), FrmtNum(OptionZAxis.LeadOutXY.X), FrmtNum(OptionZAxis.LeadOutXY.Y), FrmtNum(OptionZAxis.Up), OptionZAxis.Feed, comment); }
                            else
                            { gcodeValue.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up), comment); }
                        }// use G0 without feedrate
                        Tracker.gcodeExecutionSeconds += 60 * Math.Abs((OptionZAxis.Up - OptionZAxis.Down) / Math.Max(OptionZAxis.Feed, 10));
                        Tracker.gcodeLines++;
                    }
                }
                lastg = 1; lastf = OptionZAxis.Feed;
                lastz = OptionZAxis.Up;
                penUpApplied = true;
            }

            if (!(OptionZAxis.Enable && OptionZAxis.IncrementEnable))
            {
                if (Spindle.ToggleEnable && !Spindle.LasermodeEnable)
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Spindle-Off");
                    SpindleOff(gcodeValue, cmto); penUpApplied = true;
                }
            }
            if (!penUpApplied)
                gcodeValue.AppendFormat(" {0}\r\n", comment); //(" (PU)"); // mark pen up
        }

        private static double lastx, lasty;
        public static double lastz;
        private static float lastg, lastf;
        public static void SetLastxyz(double lx, double ly, double lz)
        { lastx = lx; lasty = ly; lastz = lz; }

        private static double lasta;
        //   public static bool LastMovewasG0 { get; set; } //= true;
        public static void MoveToNoFeed(StringBuilder gcodeValue, Point coord, string cmt)//, bool avoidFeed)
        { ApplyXYFeedRate = false; MoveSplit(gcodeValue, 1, coord.X, coord.Y, ApplyXYFeedRate, cmt); }
        public static void MoveTo(StringBuilder gcodeValue, Point coord, string cmt)
        { MoveSplit(gcodeValue, 1, coord.X, coord.Y, ApplyXYFeedRate, cmt); }
        public static void MoveTo(StringBuilder gcodeValue, double mx, double my, string cmt)
        { MoveSplit(gcodeValue, 1, mx, my, ApplyXYFeedRate, cmt); }
        public static void MoveTo(StringBuilder gcodeValue, double mx, double my, double mz, string cmt)
        { if (gcodeValue != null) MoveSplit(gcodeValue, 1, mx, my, mz, ApplyXYFeedRate, cmt); }

        public static void MoveToRapid(StringBuilder gcodeValue, Point coord, string cmt)
        {
            figureStart.X = coord.X; figureStart.Y = coord.Y;
            figureStartAlpha = ModificationTangential.Command;
            if (!(OptionZAxis.Enable && OptionZAxis.IncrementEnable))
            { Move(gcodeValue, 0, coord.X, coord.Y, false, cmt); Control.LastMovewasG0 = true; }
            else
            {
                lastx = coord.X; lasty = coord.Y; lastg = 0;
            }
        }
        public static void MoveToRapid(StringBuilder gcodeValue, double mx, double my, string cmt)
        {
            figureStart.X = mx; figureStart.Y = my;
            figureStartAlpha = ModificationTangential.Command;
            if (!(OptionZAxis.Enable && OptionZAxis.IncrementEnable))
            { Move(gcodeValue, 0, mx, my, false, cmt); Control.LastMovewasG0 = true; }
            else
            {
                lastx = mx; lasty = my; lastg = 0;
            }
        }

        // MoveSplit breaks down a line to line segments with given max. length
        private static void MoveSplit(StringBuilder gcodeString, int gnr, double mx, double my, bool applyFeed, string cmt)
        { MoveSplit(gcodeString, gnr, mx, my, null, applyFeed, cmt); }

        private static double remainingX = 10, remainingY = 10, remainingC = 10;
        //private static float segFinalX = 0, segFinalY = 0;//, segLastFinalX = 0, segLastFinalY = 0;
        //   private static int lastCharCount = 0;
        private static void MoveSplit(StringBuilder gcodeStringFinal, int gnr, double finalx, double finaly, double? z, bool applyFeed, string cmt)
        {
            if (Control.LastMovewasG0)
            {
                if ((finalx == lastx) && (finaly == lasty)) // discard G1 without any move
                { return; }
            }

            StringBuilder gcodeString = gcodeStringFinal;

            if (gnr != 0)
            {
                if (OptionZAxis.Enable && OptionZAxis.IncrementEnable)
                { gcodeString = figureString; }// if (loggerTrace) Logger.Trace("    gcodeString = figureString"); }
            }

            if (ModificationSegmentation.Enable)       // apply segmentation
            {
                double segFinalX = finalx, segFinalY = finaly;
                double dx = finalx - lastx;       // remaining distance until full move
                double dy = finaly - lasty;       // lastXY is global
                double moveLength = (float)Math.Sqrt(dx * dx + dy * dy);
                float segmentLength = ModificationSegmentation.Length;
                if (ModificationSegmentation.Length <= 0.01)
                    segmentLength = float.MaxValue;
                bool equidistance = ModificationSegmentation.Equidistant;

                // add subroutine at the beginning of each path (after G0 move)
                if (ModificationSegmentation.SubroutineOnPathStart && (Control.LastMovewasG0 || (moveLength >= segmentLength)))       // also subroutine at first point
                {
                    if (Control.InsertSubroutineEnable)
                        applyFeed = InsertSubroutine(gcodeString, lastx, lasty);//, lastz, applyFeed);
                    remainingC = segmentLength;
                }

                if (ModificationSegmentation.Length > 0) // only do calculations if segmentation length is > 0
                {
                    if ((moveLength <= remainingC))//  && !equidistance)           // nothing to split 
                    {
                        if (gcodeComments)
                            cmt += string.Format("{0:0.0} until subroutine", remainingC);
                        Move(gcodeString, 1, finalx, finaly, z, ApplyXYFeedRate, cmt);      // remainingC.ToString()
                        remainingC -= moveLength;
                    }
                    else
                    {
                        double tmpX, tmpY, origX, origY, deltaX, deltaY;
                        int count = (int)Math.Ceiling(moveLength / segmentLength);
                        gcodeString.AppendFormat("(count {0})\r\n", count.ToString());
                        origX = lastx; origY = lasty;
                        if (equidistance)               // all segments in same length (but shorter than set)
                        {
                            for (int i = 1; i < count; i++)
                            {
                                deltaX = i * dx / count;
                                deltaY = i * dy / count;
                                tmpX = origX + deltaX;
                                tmpY = origY + deltaY;
                                Move(gcodeString, 1, tmpX, tmpY, z, applyFeed, cmt);
                                if (i >= 1) { applyFeed = false; cmt = ""; }
                                if (Control.InsertSubroutineEnable)
                                    //  applyFeed = insertSubroutine(gcodeString, lastx, lasty, lastz, applyFeed);  // edit 2021-06-20
                                    applyFeed = InsertSubroutine(gcodeString, tmpX, tmpY);//, lastz, applyFeed);
                            }
                        }
                        else
                        {
                            remainingX = dx * remainingC / moveLength;
                            remainingY = dy * remainingC / moveLength;
                            for (int i = 0; i < count; i++)
                            {
                                deltaX = remainingX + i * segmentLength * dx / moveLength;        // n-1 segments in exact length, last segment is shorter
                                deltaY = remainingY + i * segmentLength * dy / moveLength;
                                tmpX = origX + deltaX;
                                tmpY = origY + deltaY;
                                remainingC = segmentLength;
                                if ((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY) >= moveLength)
                                    break;
                                Move(gcodeString, 1, tmpX, tmpY, z, applyFeed, cmt);
                                if (i >= 1) { applyFeed = false; cmt = ""; }
                                if (Control.InsertSubroutineEnable)
                                    //    applyFeed = insertSubroutine(gcodeString, lastx, lasty, lastz, applyFeed);    // edit 2021-06-20
                                    applyFeed = InsertSubroutine(gcodeString, tmpX, tmpY);//, lastz, applyFeed);
                            }
                        }
                        finalx = segFinalX; finaly = segFinalY;
                        remainingC = segmentLength - Fdistance(finalx, finaly, lastx, lasty);
                        Move(gcodeString, 1, finalx, finaly, z, applyFeed, cmt);
                        if ((equidistance) || (remainingC == 0))
                        {
                            if (Control.InsertSubroutineEnable)
                                //    insertSubroutine(gcodeString, lastx, lasty, lastz, applyFeed);    // edit 2021-06-20
                                InsertSubroutine(gcodeString, finalx, finaly);//, lastz, applyFeed);
                            remainingC = segmentLength;
                        }
                    }
                }
                else    // segmentation length is = 0, do normal move
                { Move(gcodeString, 1, finalx, finaly, z, ApplyXYFeedRate, cmt); }

            }
            else    // no gcodeLineSegmentation
            { Move(gcodeString, 1, finalx, finaly, z, ApplyXYFeedRate, cmt); }
            Control.LastMovewasG0 = false;
        }

        // process subroutine, afterwards move back to last regular position before subroutine        
        private static bool InsertSubroutine(StringBuilder gcodeString, double lX, double lY)//, float lZ, bool applyFeed)
        {
            //Logger.Trace("insertSubroutine");
            bool xyfeedneeded = true;
            if (Control.InsertSubroutinePenUpDown) PenUp(gcodeString, "PU");
            gcodeString.AppendFormat("M98 P99 (call subroutine)\r\n");
            if (Control.InsertSubroutinePenUpDown)
            {
                gcodeString.AppendFormat("G90 G0 X{0} Y{1}\r\n", FrmtNum(lX), FrmtNum(lY));
                PenDown(gcodeString, "PD");
                ApplyXYFeedRate = true;
            }
            else
            {
                gcodeString.AppendFormat("G90 G1 X{0} Y{1} F{2}\r\n", FrmtNum(lX), FrmtNum(lY), GcodeXYFeed);
                xyfeedneeded = false;
            }

            Control.SubroutineCount++;
            if (Control.SubroutineFound == 0)     // read file once
            {
                SetSubroutine(Properties.Settings.Default.importGCSubroutine, 99);
                SetSubroutine(Properties.Settings.Default.importCircleToDotScript, 95);
                Control.SubroutineFound++;
            }
            return xyfeedneeded;    // applyFeed is needed
        }

        public static void SetSubroutine(string file, int nr)
        {
            Control.SubroutineCode += "\r\n(subroutine)\r\nO" + nr.ToString() + "\r\n";
            string result = "";
            if (File.Exists(file))
            { Control.SubroutineCode += File.ReadAllText(file); }
            else
            {
                Control.SubroutineCode += "(file " + file + " not found)\r\n";
                result = "not found: ";
            }

            AddToHeader("Add SR " + nr.ToString() + " " + result + file);
            Control.SubroutineCode += "M99\r\n";
        }
        public static void CallSubroutine(StringBuilder gcodeString, int nr, string cmt)
        {
            gcodeString.AppendFormat("M98 P{0} ({1})\r\n", nr, cmt);
        }


        internal static void SplitLine(StringBuilder gcodeValue, int gnr, XyzabcuvwPoint pos1, XyzabcuvwPoint pos2, float maxStep, bool applyFeed, string cmt)
        {
            // will be called if (c > heightMapGridWidth) -> divid is >= 2
            XyzabcuvwPoint d = pos2 - pos1;
            double c = Math.Sqrt(d.X * d.X + d.Y * d.Y);
            double tmpX, tmpY;
            int divid = (int)Math.Ceiling(c / maxStep);
            lastg = -1;
            ModificationTangential.Command = "";
            bool setTangential = ((Math.Abs(d.A) > 0) || (Math.Abs(d.B) > 0) || (Math.Abs(d.C) > 0));

            for (int i = 1; i <= divid; i++)
            {
                tmpX = pos1.X + i * d.X / divid;
                tmpY = pos1.Y + i * d.Y / divid;
                if (i > 1) { applyFeed = false; cmt = ""; }

                if (setTangential)
                {
                    ModificationTangential.Command = "";
                    if (Math.Abs(d.A) > 0)
                        ModificationTangential.Command = string.Format("A{0}", FrmtNum(pos1.A + i * d.A / divid));
                    if (Math.Abs(d.B) > 0)
                        ModificationTangential.Command += string.Format("B{0}", FrmtNum(pos1.B + i * d.B / divid));
                    if (Math.Abs(d.C) > 0)
                        ModificationTangential.Command += string.Format("C{0}", FrmtNum(pos1.C + i * d.C / divid));
                }

                if (gnr == 0)
                { Move(gcodeValue, gnr, tmpX, tmpY, false, cmt); }
                else
                { Move(gcodeValue, gnr, tmpX, tmpY, applyFeed, cmt); }
            }
        }
        internal static void SplitLineZ(StringBuilder gcodeValue, int gnr, XyzabcuvwPoint pos1, XyzabcuvwPoint pos2, float maxStep, bool applyFeed, string cmt)
        {
            // will be called if (c > heightMapGridWidth) -> divid is >= 2 
            XyzabcuvwPoint d = pos2 - pos1;
            double c = Math.Sqrt(d.X * d.X + d.Y * d.Y + d.Z * d.Z);

            double tmpX, tmpY, tmpZ;
            int divid = (int)Math.Ceiling(c / maxStep);
            lastg = -1;
            ModificationTangential.Command = "";

            bool setTangential = ((Math.Abs(d.A) > 0) || (Math.Abs(d.B) > 0) || (Math.Abs(d.C) > 0));

            for (int i = 1; i <= divid; i++)
            {
                tmpX = pos1.X + i * d.X / divid;
                tmpY = pos1.Y + i * d.Y / divid;
                tmpZ = pos1.Z + i * d.Z / divid;
                if (i > 1) { applyFeed = false; cmt = ""; }

                if (setTangential)
                {
                    ModificationTangential.Command = "";
                    if (Math.Abs(d.A) > 0)
                        ModificationTangential.Command = string.Format("A{0}", FrmtNum(pos1.A + i * d.A / divid));
                    if (Math.Abs(d.B) > 0)
                        ModificationTangential.Command += string.Format("B{0}", FrmtNum(pos1.B + i * d.B / divid));
                    if (Math.Abs(d.C) > 0)
                        ModificationTangential.Command += string.Format("C{0}", FrmtNum(pos1.C + i * d.C / divid));
                }

                if (gnr == 0)
                { Move(gcodeValue, gnr, tmpX, tmpY, tmpZ, false, cmt); }
                else
                { Move(gcodeValue, gnr, tmpX, tmpY, tmpZ, applyFeed, cmt); }
            }
        }

        public static void ClearLeadIn()
        { OptionZAxis.LeadInEnable = false; }
        public static void SetZStartPos(Point xy)
        {
            OptionZAxis.LeadInXY = new XyPoint(xy);
            OptionZAxis.LeadInEnable = true;
        }
        public static void ClearLeadOut()
        { OptionZAxis.LeadOutEnable = false; }
        public static void SetZEndPos(Point xy)
        {
            OptionZAxis.LeadOutXY = new XyPoint(xy);
            OptionZAxis.LeadOutEnable = true;
        }

        internal static void SplitArc(StringBuilder gcodeValue, int gnr, XyzabcuvwPoint pos1, XyzabcuvwPoint pos2, double i1, double j2, string cmt)
        {
            XyzabcuvwPoint d = pos2 - pos1;

            if (string.IsNullOrEmpty(cmt)) cmt = "";
            double segmentLength = (double)Properties.Settings.Default.importGCLineSegmentLength;
            bool equidistance = Properties.Settings.Default.importGCLineSegmentEquidistant;

            ArcProperties arcMove;
            XyPoint p1 = new XyPoint(pos1.X, pos1.Y);
            XyPoint p2 = new XyPoint(pos2.X, pos2.Y);
            p1.Round(); p2.Round();
            arcMove = GcodeMath.GetArcMoveProperties(p1, p2, i1, j2, (gnr == 2)); // 2020-04-14 add round()
            double step = Math.Abs(Math.Asin(Control.gcodeAngleStep / arcMove.radius));     // in RAD
            if (step <= 0)
                step = 0.1;
            if (step > Math.Abs(arcMove.angleDiff))
                step = Math.Abs(arcMove.angleDiff / 2);

            ApplyXYFeedRate = true;
            double moveLength = remainingC;
            int count;
            if (equidistance)
            {
                double circum = (double)(arcMove.radius * arcMove.angleDiff);    // radius * da* (float)Math.PI / 180;
                count = (int)Math.Ceiling(circum / segmentLength);
                segmentLength = circum / count;
                Comment(gcodeValue, circum.ToString() + " " + count.ToString() + " " + segmentLength.ToString());
                moveLength = 0;
            }
            count = 1;
            if (string.IsNullOrEmpty(cmt)) cmt = "";

            bool setTangential = ((Math.Abs(d.A) > 0) || (Math.Abs(d.B) > 0) || (Math.Abs(d.C) > 0));
            double angleDeg;

            if (arcMove.angleDiff > 0)   //(da > 0)                                             // if delta >0 go counter clock wise
            {
                for (double angleRad = (arcMove.angleStart + step); angleRad < (arcMove.angleStart + arcMove.angleDiff); angleRad += step)
                {
                    double x = arcMove.center.X + arcMove.radius * Math.Cos(angleRad);
                    double y = arcMove.center.Y + arcMove.radius * Math.Sin(angleRad);
                    moveLength += Fdistance(x, y, lastx, lasty);
                    if (setTangential)
                    {
                        ModificationTangential.Command = "";
                        angleDeg = angleRad * 180 / Math.PI;
                        if (Math.Abs(d.A) > 0)
                            ModificationTangential.Command = string.Format("A{0}", FrmtNum(angleDeg + 90));
                        if (Math.Abs(d.B) > 0)
                            ModificationTangential.Command += string.Format("B{0}", FrmtNum(angleDeg + 90));
                        if (Math.Abs(d.C) > 0)
                            ModificationTangential.Command += string.Format("C{0}", FrmtNum(angleDeg + 90));
                    }
                    Move(gcodeValue, 1, x, y, ApplyXYFeedRate, cmt);
                    if (moveLength >= (count * segmentLength))
                    {
                        if (Control.InsertSubroutineEnable)
                        { if (gcodeValue != null) ApplyXYFeedRate = InsertSubroutine(gcodeValue, lastx, lasty); }//, lastz, ApplyXYFeedRate); }
                        count++;
                    }
                    if (cmt.Length > 1) cmt = "";
                }
            }
            else                                                       // else go clock wise
            {
                for (double angleRad = (arcMove.angleStart - step); angleRad > (arcMove.angleStart + arcMove.angleDiff); angleRad -= step)
                {
                    double x = arcMove.center.X + arcMove.radius * Math.Cos(angleRad);
                    double y = arcMove.center.Y + arcMove.radius * Math.Sin(angleRad);
                    moveLength += Fdistance(x, y, lastx, lasty);
                    if (setTangential)
                    {
                        ModificationTangential.Command = "";
                        angleDeg = angleRad * 180 / Math.PI;
                        if (Math.Abs(d.A) > 0)
                            ModificationTangential.Command = string.Format("A{0}", FrmtNum(angleDeg - 90));
                        if (Math.Abs(d.B) > 0)
                            ModificationTangential.Command += string.Format("B{0}", FrmtNum(angleDeg - 90));
                        if (Math.Abs(d.C) > 0)
                            ModificationTangential.Command += string.Format("C{0}", FrmtNum(angleDeg - 90));
                    }
                    Move(gcodeValue, 1, x, y, ApplyXYFeedRate, cmt);
                    if (moveLength >= (count * segmentLength))
                    {
                        if (Control.InsertSubroutineEnable)
                        //    applyXYFeedRate = insertSubroutine(gcodeString, lastx, lasty, lastz, applyXYFeedRate);    //2021-06-20
                        { if (gcodeValue != null) ApplyXYFeedRate = InsertSubroutine(gcodeValue, (float)x, (float)y); }//, lastz, ApplyXYFeedRate); }
                        count++;
                    }
                    if (cmt.Length > 1) cmt = "";
                }
            }
            Move(gcodeValue, 1, pos2.X, pos2.Y, ApplyXYFeedRate, "End Arc conversion");
            if ((moveLength >= (count * segmentLength)) || equidistance)
            {
                if (Control.InsertSubroutineEnable)
                //    applyXYFeedRate = insertSubroutine(gcodeString, lastx, lasty, lastz, applyXYFeedRate);    //2021-06-20
                { if (gcodeValue != null) ApplyXYFeedRate = InsertSubroutine(gcodeValue, pos2.X, pos2.Y); }//, lastz, ApplyXYFeedRate); }
                                                                                                           //    moveLength = 0;
            }
        }

        public static void Tool(StringBuilder gcodeValue, int toolnr, int objectCount, string cmt)
        {
            if (string.IsNullOrEmpty(cmt)) cmt = "";
            if (gcodeValue == null) return;

            if (!MyControl.UseToolList() && Properties.Settings.Default.importGCToolListUse && Properties.Settings.Default.importGCToolDefNrUse)
                toolnr = (int)Properties.Settings.Default.importGCToolDefNr;

            string toolCmd;
            if (Control.ToolChangeAddCommand)                // otherweise no command needed
            {
                if (OptionZAxis.Enable && !PreventSpindle)
                { Gcode.SpindleOff(gcodeValue, "Stop spindle - Option Z-Axis"); }

                toolCmd = string.Format("T{0:D2} M{1} (Tool:{2} {3})", toolnr, FrmtCode(6), toolnr, cmt);

                if (Control.ToolChangeM0Enable)
                { gcodeValue.AppendFormat("M0 (Tool:{0}  Color:{1})\r\n", toolnr, cmt); Tracker.gcodeLines++; }
                else
                { gcodeValue.AppendFormat("{0}\r\n", toolCmd); Tracker.gcodeLines++; }
                Tracker.gcodeToolCounter++;
                Tracker.gcodeLines++;
                string objects = "";
                if (objectCount > 0)
                    objects = string.Format("Cnt: {0,3} ", objectCount);
                Tracker.gcodeToolText += string.Format("( {0} ToolNr: {1:D2} {2} Name: {3} )\r\n", Tracker.gcodeToolCounter, toolnr, objects, cmt);

                remainingC = (float)Properties.Settings.Default.importGCLineSegmentLength;	// start with full segment length

                if (OptionZAxis.Enable && !Spindle.ToggleEnable && !PreventSpindle)
                { Gcode.SpindleOn(gcodeValue, "Start spindle - Option Z-Axis"); Tracker.gcodeLines++; }
            }

            // add gcode from tool table
            if (Properties.Settings.Default.importGCToolListUse || MyControl.UseToolList())
            {
                ToolProperty toolProperty = ToolList.GetToolProperties(toolnr);
                DeviceToolProperties dtp = toolProperty.Laser;
                string airCmd = "", gcode = "";

                if (Import.SelectedDevice == DeviceSelection.Plotter)
                {
                    dtp = toolProperty.Plotter;
                    // activate laser?
                    if (Import.SelectedPlotterMode == 1)
                    {
                        if (dtp.UseSorZ)
                        {
                            Spindle.LasermodeEnable = true;
                            OptionZAxis.Enable = false;
                            dtp = toolProperty.Laser;
                            airCmd = dtp.UseAir ? Properties.Settings.Default.DeviceLaserCmndAirOn : Properties.Settings.Default.DeviceLaserCmndAirOff;
                            gcode = airCmd + ";";
                        }
                        else
                        {
                            Spindle.LasermodeEnable = false;
                            OptionZAxis.Enable = true;
                        }
                    }
                }
                else if (Import.SelectedDevice == DeviceSelection.Router)
                    dtp = toolProperty.Router;
				
                if (LoggerTraceImport) Logger.Trace("   from ToolList device {0} tool nr {1} ", Import.SelectedDevice, toolnr);
                GetValuesFromToolList(dtp,Import.SelectedDevice.ToString(), toolnr);


				/* create additional GCode */
                if (Import.SelectedDevice == DeviceSelection.Laser)
                {
                    airCmd = dtp.UseAir ? Properties.Settings.Default.DeviceLaserCmndAirOn : Properties.Settings.Default.DeviceLaserCmndAirOff;
                    gcode = airCmd + ";" + toolProperty.Gcode;
                }
                else
                { gcode += toolProperty.Gcode; }

                if (gcode.Length > 1)
                {
                    string[] commands = gcode.Split(';');
                    string comment1 = "", comment2 = "", comment3 = "(PU) ";
                    if (gcodeComments) { comment1 = "(tool-list use gcode)"; comment2 = "(tool-list wrap gcode)"; comment3 += "(tool-list finally)"; }

                    //     gcodeValue.AppendFormat("G04 P0.1 {0}\r\n", comment2);
                    foreach (string btncmd in commands)
                    {
                        if (btncmd != "")
                            gcodeValue.AppendFormat("{0} {1}\r\n", btncmd.Trim(), comment1);
                    }
                    //     gcodeValue.AppendFormat("G04 P0.1 {0}\r\n", comment2);

                    if (OptionZAxis.Enable)
                    { gcodeValue.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up), comment3); }
                }
            }
        }

        internal static void GetValuesFromToolList(DeviceToolProperties dtp, string device, int tnr)//, DeviceSelection device)
        {
            Logger.Info("►►► GetValuesFromToolList  {0}  T-Nr:{1}  {2}", device, tnr, dtp.Settings());
            if (dtp == null)
            { Logger.Error("GetValuesFromToolTable ToolList is null"); return; }
            GcodeXYFeed = dtp.FeedXY;
            OptionZAxis.Feed = dtp.FeedZ;
            OptionZAxis.Up = dtp.SaveZ;
            OptionZAxis.Down = dtp.FinalZ;
            OptionPWM.Up = dtp.SaveS;
            OptionPWM.Down = dtp.FinalS;
            Spindle.Speed = dtp.FinalS;
            Spindle.SpindleCmd = dtp.UseM3 ? "3" : "4";
        }

        public static void AddToHeader(string cmt, bool insideTag = true)
        {
            if (insideTag)
                headerData.AppendFormat("({0})\r\n", cmt);
            else
                headerMessage.AppendFormat("({0})\r\n", cmt);
        }

        public static void SetHeaderInfo(string title, float distance, float feed, int lines, int downUp)
        {
            Logger.Trace("SetHeaderInfo title:{0} distance:{1} feed:{2} lines:{3} downUp:{4}", title, distance, feed, lines, downUp);
            docTitle = title;
            Tracker.gcodeDistancePD = distance;
            GcodeXYFeed = feed;
            Tracker.gcodeLines = lines;
            Tracker.gcodeDownUp = downUp;
            OptionZAxis.Enable = true;
        }
        public static string GetHeader(string cmt, string source, long lineCount = 0)
        {
            Tracker.gcodeLines += lineCount;
            Tracker.gcodeExecutionSeconds += 60 * Tracker.gcodeDistancePD / GcodeXYFeed;
            Tracker.gcodeExecutionSeconds += 60 * Tracker.gcodeDistancePU / 5000;
            string header = string.Format("( {0} by GRBL-Plotter {1} )\r\n", cmt, MyApplication.GetVersion());
            string header_end = headerData.ToString();
            header_end += string.Format("({0} >)\r\n", XmlMarker.HeaderEnd);
            header_end += headerMessage.ToString();

            if (Properties.Settings.Default.importGCConvertToPolar)
                header_end += string.Format("({0} X=radius, Y=angle/>)\r\n", "Polar");

            if (ModificationTangential.Enable)
                header_end += string.Format("({0} Axis=\"{1}\" UnitsFullTurn=\"{2}\"/>)\r\n", XmlMarker.TangentialAxis, ModificationTangential.Name, Properties.Settings.Default.importGCTangentialTurn);

            string[] commands = Properties.Settings.Default.importGCHeader.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                { header_end += string.Format("{0} (Setup - GCode-Header)\r\n", cmd.Trim()); Tracker.gcodeLines++; }
            if (Control.GcodeRelative)
            { header_end += string.Format("G91 (Setup RELATIVE movement)\r\n"); Tracker.gcodeLines++; }
            else
            { header_end += string.Format("G90\r\n"); Tracker.gcodeLines++; }

            if (Properties.Settings.Default.importUnitGCode)
            {
                if (Properties.Settings.Default.importUnitmm)
                { header_end += "G21 (use mm as unit - check setup)\r\n"; Tracker.gcodeLines++; }
                else
                { header_end += "G20 (use inch as unit - check setup)\r\n"; Tracker.gcodeLines++; }
            }

            // take account of footer lines
            commands = Properties.Settings.Default.importGCFooter.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                { Tracker.gcodeLines++; }
            if (Control.ToolChangeAddCommand && Properties.Settings.Default.ctrlToolChangeEmpty)
            { Tracker.gcodeLines++; }
            Tracker.gcodeLines++;


            if (!string.IsNullOrEmpty(source) && (source.Length > 1))
                header += string.Format("( Source: {0} )\r\n", source);

            if (docTitle.Length > 1)
                header += string.Format("( Title : {0} )\r\n", docTitle);
            if (docDescription.Length > 1)
            {
                string[] lines = docDescription.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (line.Length > 1)
                        header += string.Format("( Description : {0} )\r\n", line.Trim());
                }
            }

            if (!(OptionZAxis.Enable || OptionPWM.Enable || Spindle.ToggleEnable || OptionIndividual.Enable))
            { header += string.Format("( !!! No Pen up/down translation !!! )\r\n"); }
            else
            {
                header += string.Format("( Pen U/D trans: {0} {1} {2} {3})\r\n", (OptionZAxis.Enable ? string.Format("Z|{0:0.0}|{1:0.0}", OptionZAxis.Up, OptionZAxis.Down) : "noZ"),
                                                                            (OptionPWM.Enable ? string.Format("PWM|{0:0}|{1:0}", OptionPWM.Up, OptionPWM.Down) : "noPWM"),
                                                                            (Spindle.ToggleEnable ? "SpndTog" : "noSpndTog"),
                                                                            (Spindle.LasermodeEnable ? "LsrMd" : "noLsrMd"));
            }
            if (ImportParameter.RemoveShortMovesEnable && ((float)ImportParameter.RemoveShortMovesLimit > 0.2))
            { header += string.Format("( !!! Remove short moves < {0:0.00} !!! )\r\n", ImportParameter.RemoveShortMovesLimit); }
            if (ImportParameter.AssumeAsEqualDistance > 0.01)
            { header += string.Format("( !!! Assume as equal distance {0:0.00} !!! )\r\n", ImportParameter.AssumeAsEqualDistance); }

            header += string.Format("({0} >)\r\n", XmlMarker.HeaderStart);

            if (Properties.Settings.Default.importRepeatEnable)
            {
                header += string.Format("( G-Code repetitions: {0:0} times)\r\n", Properties.Settings.Default.importRepeatCnt);
                Logger.Info("◆◆  Header: G-Code repetitions:{0}", Properties.Settings.Default.importRepeatCnt);
            }

            header += string.Format("( G-Code lines      : {0} )\r\n", Tracker.gcodeLines);
            Logger.Info("◆◆  Header: G-Code lines:{0}", Tracker.gcodeLines);

            header += string.Format("( Pen Down/Up PD/PU : {0} times )\r\n", Tracker.gcodeDownUp);
            header += string.Format("( Path length (PD)  : {0:0.0} mm )\r\n", Tracker.gcodeDistancePD);
            header += string.Format("( Path length (PU)  : {0:0.0} mm )\r\n", Tracker.gcodeDistancePU);

            try
            {
                TimeSpan t = TimeSpan.FromSeconds(Tracker.gcodeExecutionSeconds);
                header += string.Format("( Duration ca.      : {0:D2}:{1:D2}:{2:D2} h:m:s )\r\n", t.Hours, t.Minutes, t.Seconds);
            }
            catch (Exception err)
            {
                header += string.Format("( Duration ca.      : {0:0.0} min. )\r\n", Tracker.gcodeExecutionSeconds / 60);
            }

            if (Control.SubroutineCount > 0)
            {
                header += string.Format("( Call to subs.     : {0} )\r\n", Control.SubroutineCount);
                Logger.Info("◆◆  Header: Subroutine calls:{0}", Control.SubroutineCount);
            }

            Control.stopwatch.Stop();
            header += string.Format("( Conv. time        : {0} )\r\n", Control.stopwatch.Elapsed);

            if (Properties.Settings.Default.importGCToolListUse)
            {
                header += "( Values from tool-table: All";
                //    if (Properties.Settings.Default.importGCTTSSpeed) { header += "spindle speed, "; }
                //    if (Properties.Settings.Default.importGCTTXYFeed) { header += "XY feed, "; }
                //    if (Properties.Settings.Default.importGCTTZAxis) { header += "Z Values "; }
                header += ")\r\n";
            }

            if (Control.ToolChangeAddCommand)
            {
                header += string.Format("( Tool changes: {0})\r\n", Tracker.gcodeToolCounter);
                header += Tracker.gcodeToolText;
                Logger.Info("◆◆  Header: Tool changes:{0}", Tracker.gcodeToolCounter);
            }
            if (Tracker.gcodePauseCounter > 0)
                header += string.Format("( M0 count    : {0})\r\n", Tracker.gcodePauseCounter);

            //			header_end += string.Format("({0} >)\r\n", xmlMarker.headerEnd);
            return header + header_end;
        }

        public static string GetFooter()
        {
            string footer = "";

            if (Control.ToolChangeAddCommand && Properties.Settings.Default.ctrlToolChangeEmpty)
            { footer += string.Format("T{0} M{1} (Remove tool)\r\n", FrmtCode((int)Properties.Settings.Default.ctrlToolChangeEmptyNr), FrmtCode(6)); }

            string[] commands = Properties.Settings.Default.importGCFooter.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                { footer += string.Format("{0} (Setup - GCode-Footer)\r\n", cmd.Trim()); }

            if (Properties.Settings.Default.importGCPWMEnable && Properties.Settings.Default.importGCPWMSkipM30)
            { footer += "M30 (SKIP M30)\r\n"; }
            else
                footer += "M30\r\n";

            return footer + Control.SubroutineCode;
        }

        public static void Comment(StringBuilder gcodeValue, string cmt)
        {
            if (!string.IsNullOrEmpty(cmt) && (cmt.Length > 1))
            { gcodeValue?.AppendFormat("({0})\r\n", cmt); }
        }

        private static void Drill(StringBuilder gcodeString)
        {
            float zStep = 0;
            int passCount = 1;
            OptionZAxis.finalZ = OptionZAxis.Down;
            string cmt = "";
            bool fromTT = false;
      //      if (Properties.Settings.Default.importGCTTZAxis) { cmt += " Z final, "; fromTT = true; }
            //     if (Properties.ListSettings.Default.importGCTTZIncrement) { cmt += " Z step, "; fromTT = true; }
            if (OptionZAxis.gcodeZFeedToolTable) { cmt += " Z feed "; fromTT = true; }
            if (fromTT && gcodeComments) { cmt += " from tool-table"; }
            cmt = "( " + cmt + " )";
            if (OptionZAxis.IncrementStartAtZero)       // perfom 1st pass at zero
                zStep = OptionZAxis.IncrementStep;
            while (zStep > OptionZAxis.finalZ)      // repeat, until finalZ reached
            {
                zStep -= OptionZAxis.IncrementStep;     // reduce Z by inc
                if (zStep < OptionZAxis.finalZ)
                    zStep = OptionZAxis.finalZ;
                OptionZAxis.Down = zStep;
                Comment(gcodeString, string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\" >", XmlMarker.PassStart, passCount, zStep, OptionZAxis.finalZ));
                if (passCount <= 1)
                {
                    ModificationTangential.Command = figureStartAlpha;
                    Move(gcodeString, 0, (float)figureStart.X, (float)figureStart.Y, false, "");
                    Control.LastMovewasG0 = true;
                }
                gcodeString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(zStep), OptionZAxis.Feed, cmt);    // Router down
                Tracker.gcodeDownUp++;

                if (!OptionZAxis.IncrementNoToolUp)
                    gcodeString.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up), "");                  // Router up

                Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ passCount.ToString() + ">");
                passCount++;

                Tracker.gcodeExecutionSeconds += Tracker.gcodeFigureTime;
                Tracker.gcodeLines += Tracker.gcodeFigureLines + 3;
                Tracker.gcodeDistancePD += Tracker.gcodeFigureDistance;
            }
            figureString.Clear();
        }
        private static void IntermediateZ(StringBuilder gcodeString)
        {
            if (LoggerTraceImport) Logger.Trace("   intermediateZ");

            float zStep = 0;
            int passCount = 1;
            OptionZAxis.finalZ = OptionZAxis.Down;
            string cmt = "";
            string xml;
            bool fromTT = false;
      //      if (Properties.Settings.Default.importGCTTZAxis) { cmt += " Z final, "; fromTT = true; }
            //     if (Properties.ListSettings.Default.importGCTTZIncrement) { cmt += " Z step, "; fromTT = true; }
            if (OptionZAxis.gcodeZFeedToolTable) { cmt += " Z feed "; fromTT = true; }
            if (fromTT && gcodeComments) { cmt += " from tool-table"; }
            cmt = "( " + cmt + " )";
            if (OptionZAxis.IncrementStartAtZero)       // perfom 1st pass at zero
                zStep = OptionZAxis.IncrementStep;
            while (zStep > OptionZAxis.finalZ)      // repeat, until finalZ reached
            {
                zStep -= OptionZAxis.IncrementStep;     // reduce Z by inc
                if (zStep < OptionZAxis.finalZ)
                    zStep = OptionZAxis.finalZ;
                OptionZAxis.Down = zStep;
                xml = string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\" >", XmlMarker.PassStart, passCount, zStep, OptionZAxis.finalZ);
                Comment(gcodeString, xml);
                if (LoggerTraceImport) Logger.Trace("{0}", xml);
                ModificationTangential.Command = figureStartAlpha;
                Move(gcodeString, 0, (float)figureStart.X, (float)figureStart.Y, false, ""); Control.LastMovewasG0 = true;
                Tracker.gcodeFigureLines--; // avoid double count

                // PenDown
                if (Spindle.ToggleEnable && !Spindle.LasermodeEnable) SpindleOn(gcodeString, "toggle");                     // send M3/4 + G4 delay
                gcodeString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(zStep), OptionZAxis.Feed, cmt);   // Router down
                if (Spindle.LasermodeEnable) SpindleOn(gcodeString, "lasermode");                                         // send S1000

                if (OptionPWM.Enable)
                {
                    if (gcodeComments) gcodeString.AppendFormat("({0})\r\n", "Pen down: Servo control");
                    gcodeString.AppendFormat("M{0} S{1} {2}\r\n", Spindle.SpindleCmd, OptionPWM.Down, cmt);
                    if (OptionPWM.DlyDown > 0)
                        gcodeString.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(OptionPWM.DlyDown));
                }
                if (OptionIndividual.Enable)
                {
                    foreach (string cmd in OptionIndividual.Down.Split(';'))
                    { gcodeString.AppendFormat("{0}\r\n", cmd.Trim()); }
                }

                Tracker.gcodeDownUp++;
                gcodeString.Append(figureString.ToString());                                                        // draw figure
                if (LoggerTraceImport) Logger.Trace(" intermediateZ Copy code");

                // PenUp
                if (OptionIndividual.Enable)
                {
                    foreach (string cmd in OptionIndividual.Up.Split(';'))
                    { gcodeString.AppendFormat("{0}\r\n", cmd.Trim()); }
                }
                if (OptionPWM.Enable)
                {
                    gcodeString.AppendFormat("M{0} S{1}\r\n", Spindle.SpindleCmd, OptionPWM.Up);
                    if (OptionPWM.DlyUp > 0)
                        gcodeString.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(OptionPWM.DlyUp));
                    Tracker.gcodeExecutionSeconds += OptionPWM.DlyUp;
                    Tracker.gcodeLines++;
                }

                if (!OptionZAxis.IncrementNoToolUp || (zStep <= OptionZAxis.finalZ))	// Z-Up at least on final pass
                {
                    if (Spindle.LasermodeEnable) SpindleOff(gcodeString, "lasermode");                        // send S0
                    gcodeString.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up), "");  // Router up
                    if (Spindle.ToggleEnable && !Spindle.LasermodeEnable) SpindleOff(gcodeString, "toggle");    // send M5
                }

                Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ passCount.ToString() + ">");
                passCount++;

                Tracker.gcodeExecutionSeconds += Tracker.gcodeFigureTime;
                Tracker.gcodeLines += Tracker.gcodeFigureLines + 3;
                Tracker.gcodeDistancePD += Tracker.gcodeFigureDistance;
            }
            figureString.Clear();
        }

        // helper functions
        private static double Fsqrt(double x) { return Math.Sqrt(x); }
        private static double Fvmag(double x, double y) { return Fsqrt(x * x + y * y); }
        private static float Fdistance(double x1, double y1, double x2, double y2) { return (float)Fvmag(x2 - x1, y2 - y1); }
    }
}
