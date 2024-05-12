/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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
 * 2019-09 add toolInfo.gcode
 * 2020-01 add trace level loggerTraceImport to hide log of any gcode command during import
 * 2020-01 add tiny G1 moves for Pen down/up in lasermode only - to be able making dots
 * 2020-12-30 add N-Number
 * 2021-02-20 add subroutine for pen-up/down for use in tool-change scripts
 * 2021-02-28 in jobStart() line 415, call PenUp() code, to lift also servo
 * 2021-03-07 in jobStart() bug-fix: call PenUp() code only if !gcodeZApply
 * 2021-03-26 line 1130 change comments
 * 2021-04-18 function insertSubroutine line 765 add option to add pen-up /-down before/after subroutine call
 * 2021-05-07 if gcodeLineSegmentLength==0, no segmentation, but at begin of path.
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
 * 2023-03-05 l:1544/1633 f:Drill/IntermediateZ add gcodeZNoUp
 * 2023-03-06 l:1314 f:Tool remove space in output "M0 (Tool:{0}  Color:{1})\r\n"
 * 2023-03-14 l:1133 f:ClearLeadOut()	added
 * 2023-03-15 l:746 f:PenUp add F-value
 * 2023-04-19 l:1316 f:Tool  add the key-word "tool" into the comment
 * 2023-05-31 new class GcodeDefaults in vers 1.7.0.0
 * 2023-09-04 l:1626 f:GetValuesFromToolTable get GcodePwmDown also from tool table
 * 2023-09-05 l:858 f:GetStrGCode allow also 3 digitis
 * 2023-09-23 l:1020 f:JobEnd  don't send M05 if (PreventSpindle)
 * 2023-09-24 l:1500 f:Tool also take care of !PreventSpindle
 * 2023-11-27 l:792 f:Setup add script from Properties.Settings.Default.importCircleToDotScript
*/

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
    {	/* Over write specific 'Properties.Settings.Default' values during import, e.g. SVG meta data */
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
            bool useValueFromToolTable = Properties.Settings.Default.importGCToolTableUse;
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

            if (Properties.Settings.Default.fromFormInsertEnable)
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

            Summary.AppendFormat("<tr><th colspan='3'>{0}</td></tr>\r\n", Localization.GetString("importMessageGeneralSettings"));

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
        private const string formatCode = "00";
        private static string formatNumber = "0.###";

        private static int gcodeLines = 0;              // counter for GCode lines
        private static int gcodeFigureLines = 0;        // counter for GCode lines
        private static float gcodeDistance = 0;         // counter for GCode move distance
        private static float gcodeFigureDistance = 0;   // counter for GCode move distance

        private static int gcodeSubroutineEnable = 0;   // state subroutine
        private static string gcodeSubroutine = "";     //  subroutine

        private static int gcodeDownUp = 0;             // counter for GCode Pen Down / Up
        private static double gcodeTime = 0;             // counter for GCode work time
        private static double gcodeFigureTime = 0;       // counter for GCode work time
        private static int gcodePauseCounter = 0;       // counter for GCode pause M0 commands
        private static int gcodeToolCounter = 0;        // counter for GCode Tools
        private static string gcodeToolText = "";       // counter for GCode Tools

        private static bool useValueFromToolTable = false;
        public static float GcodeXYFeed { get; set; } //= 1999;         // XY feed to apply for G1
        private static bool gcodeXYFeedToolTable = false; // from Tool Table
        private static bool gcodeComments = true;       // if true insert additional comments into GCode

        private static bool gcodeToolChange = false;    // Apply tool exchange command
        private static bool gcodeToolChangeM0 = false;

        // Using Z-Axis for Pen up down
        public static bool GcodeZApply { get; set; } //= true;         // if true insert Z movements for Pen up/down
        public static float GcodeZUp { get; set; } //= 1.999f;          // Z-up position
        public static float GcodeZDown { get; set; } //= -1.999f;       // Z-down position
        public static float GcodeZFeed { get; set; } //= 499;           // Z feed to apply for G1
        private static bool gcodeZFeedToolTable = false;// from Tool Table
        public static float GcodeZInc { get; set; } //= 1;
        private static bool gcodeZNoUp = false;                         // no pen-up after a pass
        public static bool GcodeZPreventSpindle { get; set; } //= 1;

        private static bool PreventSpindle { get; set; }
        //        public static float gcodeZRepitition;          // Z feed to apply for G1

        // Using Spindle pwr. to switch on/off laser
        private static bool gcodeSpindleToggle = false; // Switch on/off spindle for Pen down/up (M3/M5)
        public static float GcodeSpindleSpeed { get; set; } //= 999;    // Spindle speed to apply
        public static string GcodeSpindleCmd { get; set; } //= "3";    // Spindle Command M3 / M4
        private static bool gcodeSpindleToolTable = false;     // from Tool Table
        private static bool gcodeUseLasermode = false;

        // Using Spindle-Speed als PWM output to control RC-Servo
        public static bool GcodePWMEnable { get; set; } //= false;     // Change Spindle speed for Pen down/up
        private static float gcodePwmUp = 199;          // Spindle speed for Pen-up
        public static float GcodePwmDlyUp { get; set; } //= 0;         // Delay to apply after Pen-up (because servo is slow)
        public static float GcodePwmDown { get; set; }        // Spindle speed for Pen-down
        public static float GcodePwmDlyDown { get; set; } //= 0;       // Delay to apply after Pen-down (because servo is slow)

        private static bool gcodeIndividualTool = false;// Use individual Pen down/up
        private static string gcodeIndividualUp = "";
        private static string gcodeIndividualDown = "";

        private static bool gcodeCompress = false;      // reduce code by avoiding sending again same G-Nr and unchanged coordinates
        public static bool GcodeRelative { get; set; } //= false;       // calculate relative coordinates for G91
        private static bool gcodeNoArcs = false;        // replace arcs by line segments
        private static float gcodeAngleStep = 0.1f;
        private static bool gcodeInsertSubroutineEnable = false;
        private static bool gcodeInsertSubroutinePenUpDown = false;
        private static int gcodeSubroutineCount = 0;

        private static bool gcodeLineSegmentationEnable;
        private static float gcodeLineSegmentLength;
        private static bool gcodeLineSegmentEquidistant;
        private static bool gcodeLineSegementSubroutineOnPathStart;

        private static bool gcodeTangentialEnable = false;
        private static string gcodeTangentialName = "C";
        private static double gcodeTangentialAngle = 0;
        private static float gcodeTangentialAngleDevi = 0;
        //    private static float gcodeTangentialAngleLast = 0;
        private static string gcodeTangentialCommand = "";

        private static bool gcodeAuxiliaryValue1Enable = false;
        private static string gcodeAuxiliaryValue1Name = "A";
        //    private static float gcodeAuxiliaryValue1Distance = 1;
        private static string gcodeAuxiliaryValue1Command = "";

        private static bool gcodeAuxiliaryValue2Enable = false;
        private static string gcodeAuxiliaryValue2Name = "A";
        //    private static float gcodeAuxiliaryValue2Distance = 1;
        private static string gcodeAuxiliaryValue2Command = "";

        private static bool gcodeSValueEnable = false;
        private static string gcodeSValueCommand = "";

        private static bool gcodeZLeadInEnable = false;
        private static XyPoint gcodeZLeadInXY = new XyPoint();
        private static bool gcodeZLeadOutEnable = false;
        private static XyPoint gcodeZLeadOutXY = new XyPoint();

        private static readonly StringBuilder headerData = new StringBuilder();
        private static readonly StringBuilder headerMessage = new StringBuilder();

        // depth per pass
        private static readonly StringBuilder figureString = new StringBuilder();    // tool path for pen down path
        private static XyPoint figureStart;                                 // 1st point of figure to move via G0 before pen down
        private static string figureStartAlpha;
        private static bool repeatZ = false;                                // depth per pass enabled
        private static bool repeatZStartZero = false;
        private static float finalZ = -2;                                   // final tool path depth

        private static int decimalPlaces = 3;

        private static Stopwatch stopwatch = new Stopwatch();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool RepeatZ
        {
            get { return repeatZ; }
            set { repeatZ = value; }
        }
        public static string GetSettings()
        {
            string tmp = "";
            tmp += string.Format("XYFeed: {0}, ", GcodeXYFeed);
            if (gcodeToolChange)
                tmp += "ToolChange, ";
            if (GcodeZApply)
            {
                tmp += string.Format("ZFeed: {0}, Up: {1}, Down: {2}, Spindle M{3} S{4}", GcodeZFeed, GcodeZUp, GcodeZDown, GcodeSpindleCmd, GcodeSpindleSpeed);
                if (repeatZ)
                    tmp += string.Format("ZStep: {0}, ", GcodeZInc);
            }
            if (gcodeSpindleToggle)
                tmp += string.Format(" SpindleToggle M{0} S{1} ", GcodeSpindleCmd, GcodeSpindleSpeed);
            if (GcodePWMEnable)
                tmp += " PWM ";
            return tmp;
        }

        public static void Setup(bool convertGraphics)	// true for SVG, DXF, HPGL, CSV		false for shape,
        {
            if (!Properties.Settings.Default.importSVGMetaData)
                GcodeDefaults.Reset();

            decimalPlaces = (int)Properties.Settings.Default.importGCDecPlaces;
            SetDecimalPlaces(decimalPlaces);
            GcodeXYFeed = GcodeDefaults.FeedXY;		//(float)Properties.Settings.Default.importGCXYFeed;

            useValueFromToolTable = Properties.Settings.Default.importGCToolTableUse;
            gcodeXYFeedToolTable = useValueFromToolTable && Properties.Settings.Default.importGCTTXYFeed;

            gcodeComments = Properties.Settings.Default.importGCAddComments;
            gcodeSpindleToggle = GcodeDefaults.SpindleEnable;   // Properties.Settings.Default.importGCSpindleToggle;
            GcodeSpindleSpeed = GcodeDefaults.SpindleSpeed;		//(float)Properties.Settings.Default.importGCSSpeed;
            gcodeSpindleToolTable = useValueFromToolTable && Properties.Settings.Default.importGCTTSSpeed;
            gcodeUseLasermode = GcodeDefaults.LaserEnable;   // Properties.Settings.Default.importGCSpindleToggleLaser;

            if (Properties.Settings.Default.importGCSDirM3)
                GcodeSpindleCmd = "3";
            else
                GcodeSpindleCmd = "4";

            GcodeZApply = GcodeDefaults.ZEnable;    // Properties.Settings.Default.importGCZEnable;
            GcodeZUp = GcodeDefaults.ZUp;			//(float)Properties.Settings.Default.importGCZUp;
            GcodeZDown = GcodeDefaults.ZDown;		//(float)Properties.Settings.Default.importGCZDown;
            GcodeZFeed = GcodeDefaults.FeedZ;		//(float)Properties.Settings.Default.importGCZFeed;
            gcodeZFeedToolTable = useValueFromToolTable && Properties.Settings.Default.importGCTTZAxis;
            GcodeZInc = (float)Properties.Settings.Default.importGCZIncrement;             // depth per pass
            gcodeZNoUp = Properties.Settings.Default.importGCZIncNoZUp;
            GcodeZPreventSpindle = Properties.Settings.Default.importGCZPreventSpindle;

            repeatZ = convertGraphics && Properties.Settings.Default.importGCZIncEnable;    // do final Z in several passes?
            repeatZStartZero = Properties.Settings.Default.importGCZIncStartZero;
            finalZ = (float)Properties.Settings.Default.importGCZDown;                      // final Z


            GcodePWMEnable = GcodeDefaults.PWMEnable;   // Properties.Settings.Default.importGCPWMEnable;
            gcodePwmUp = GcodeDefaults.PWMUp;           // (float)Properties.Settings.Default.importGCPWMUp;
            GcodePwmDlyUp = (float)Properties.Settings.Default.importGCPWMDlyUp;
            GcodePwmDown = GcodeDefaults.PWMDown;         // (float)Properties.Settings.Default.importGCPWMDown;
            GcodePwmDlyDown = (float)Properties.Settings.Default.importGCPWMDlyDown;

            gcodeIndividualTool = Properties.Settings.Default.importGCIndEnable;
            gcodeIndividualUp = Properties.Settings.Default.importGCIndPenUp;
            gcodeIndividualDown = Properties.Settings.Default.importGCIndPenDown;

            gcodeToolChange = Properties.Settings.Default.importGCTool;
            gcodeToolChangeM0 = Properties.Settings.Default.importGCToolM0;

            gcodeCompress = convertGraphics && Properties.Settings.Default.importGCCompress;

            bool auxIsRelative = (Properties.Settings.Default.importGCAux1Enable && !Properties.Settings.Default.importGCAux1SumUp) ||
                                    (Properties.Settings.Default.importGCAux2Enable && !Properties.Settings.Default.importGCAux2SumUp);
            GcodeRelative = (convertGraphics && Properties.Settings.Default.importGCRelative || auxIsRelative);

            gcodeNoArcs = convertGraphics && Properties.Settings.Default.importGCNoArcs;
            gcodeAngleStep = (float)Properties.Settings.Default.importGCSegment;

            gcodeInsertSubroutineEnable = convertGraphics && Properties.Settings.Default.importGCSubEnable;
            gcodeInsertSubroutinePenUpDown = convertGraphics && Properties.Settings.Default.importGCSubPenUpDown;

            gcodeSubroutineCount = 0;
            LastMovewasG0 = true;

            gcodeLines = 1;             // counter for GCode lines
            gcodeDistance = 0;          // counter for GCode move distance
            remainingC = (float)Properties.Settings.Default.importGCLineSegmentLength;

            gcodeLineSegmentationEnable = false;
            if (convertGraphics)
            {
                gcodeLineSegmentationEnable = Properties.Settings.Default.importGCLineSegmentation;
                gcodeLineSegmentLength = (float)Properties.Settings.Default.importGCLineSegmentLength;
                gcodeLineSegmentEquidistant = Properties.Settings.Default.importGCLineSegmentEquidistant;
                gcodeLineSegementSubroutineOnPathStart = Properties.Settings.Default.importGCSubFirst;
            }
            gcodeTangentialEnable = Properties.Settings.Default.importGCTangentialEnable;
            gcodeTangentialName = Properties.Settings.Default.importGCTangentialAxis;
            gcodeTangentialAngle = 0;//gcodeTangentialAngleLast

            gcodeTangentialAngleDevi = (float)Properties.Settings.Default.importGCTangentialAngleDevi;
            gcodeTangentialCommand = figureStartAlpha = "";

            gcodeAuxiliaryValue1Enable = Properties.Settings.Default.importGCAux1Enable;
            gcodeAuxiliaryValue2Enable = Properties.Settings.Default.importGCAux2Enable;
            gcodeAuxiliaryValue1Name = Properties.Settings.Default.importGCAux1Axis;
            gcodeAuxiliaryValue2Name = Properties.Settings.Default.importGCAux2Axis;

            gcodeSValueEnable = Properties.Settings.Default.importPWMFromWidth;

            // prevent spindle/laser-on on job start
            //   PreventSpindle = GcodeZPreventSpindle && !gcodeSpindleToggle; //(&& dragtool && ...)
            PreventSpindle = GcodeZPreventSpindle || gcodeSpindleToggle; //(&& dragtool && ...)

            gcodeZLeadInEnable = false;
            gcodeZLeadOutEnable = false;

            gcodeSubroutineEnable = 0;
            gcodeSubroutine = "";
            gcodeDownUp = 0;            // counter for GCode Down/Up
            gcodeTime = 0;              // counter for GCode work time
            gcodePauseCounter = 0;      // counter for GCode pause M0 commands
            gcodeToolCounter = 0;
            gcodeToolText = "";

            gcodeFigureTime = 0;
            gcodeFigureLines = 0;
            gcodeFigureDistance = 0;

            docTitle = "";
            docDescription = "";

            headerData.Clear();
            headerMessage.Clear();
            figureString.Clear();                                                           // 

            lastx = -0.001; lasty = -0.001; lastz = +0.001; lasta = 0;

            if (GcodeRelative)
            { lastx = 0; lasty = 0; }

            stopwatch = new Stopwatch();
            stopwatch.Start();

            gcodeSValueCommand = "";
            gcodeTangentialCommand = "";
            gcodeAuxiliaryValue1Command = "";
            gcodeAuxiliaryValue2Command = "";

            if ((gcodeInsertSubroutineEnable && gcodeLineSegmentationEnable) || gcodeToolChange || Properties.Settings.Default.ctrlToolChange || (Properties.Settings.Default.importSVGCircleToDot && (Properties.Settings.Default.importCircleToDotScriptCount > 0)))
            {
                bool insertSubroutine = false;
                if (gcodeInsertSubroutineEnable && gcodeLineSegmentationEnable && FileContainsSubroutineCall(Properties.Settings.Default.importGCSubroutine))
                { insertSubroutine = true; }
                else if (gcodeToolChange)
                {
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptPut);
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptSelect);
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptGet);
                    insertSubroutine = insertSubroutine || FileContainsSubroutineCall(Properties.Settings.Default.ctrlToolScriptProbe);
                }
                if (insertSubroutine)
                {
                    double tmp_lastz = lastz;
                    StringBuilder tmp = new StringBuilder();
                    Logger.Trace("setup create PenUp/Down subroutine gcodeInsertSubroutine:{0} gcodeToolChange:{1} ctrlToolChange:{2}", gcodeInsertSubroutineEnable, gcodeToolChange, Properties.Settings.Default.ctrlToolChange);
                    PenUp(tmp, "");
                    gcodeSubroutine += "\r\n(subroutine)\r\nO90 (Pen up)\r\n";
                    gcodeSubroutine += tmp.ToString();
                    gcodeSubroutine += "M99\r\n";
                    tmp.Clear();
                    PenDown(tmp, "");
                    gcodeSubroutine += "\r\n(subroutine)\r\nO92 (Pen down)\r\n";
                    gcodeSubroutine += tmp.ToString();
                    gcodeSubroutine += "M99\r\n";

                    if (Properties.Settings.Default.importSVGCircleToDot)   // && File.Exists(Properties.Settings.Default.importCircleToDotScript))
                    {
                        SetSubroutine(Properties.Settings.Default.importCircleToDotScript, 95);
                    }

                    lastz = tmp_lastz;

                    if (GcodePWMEnable)
                    {
                        tmp.Clear();
                        SetPwm(tmp, (float)Properties.Settings.Default.importGCPWMZero, (float)Properties.Settings.Default.importGCPWMDlyDown);
                        gcodeSubroutine += "\r\n(subroutine)\r\nO91 (Pen zero)\r\n";
                        gcodeSubroutine += tmp.ToString();
                        gcodeSubroutine += "M99\r\n";
                        tmp.Clear();
                        SetPwm(tmp, (float)Properties.Settings.Default.importGCPWMP93, (float)Properties.Settings.Default.importGCPWMDlyP93);
                        gcodeSubroutine += "\r\n(subroutine)\r\nO93 (" + Properties.Settings.Default.importGCPWMTextP93 + ")\r\n";
                        gcodeSubroutine += tmp.ToString();
                        gcodeSubroutine += "M99\r\n";
                        tmp.Clear();
                        SetPwm(tmp, (float)Properties.Settings.Default.importGCPWMP94, (float)Properties.Settings.Default.importGCPWMDlyP94);
                        gcodeSubroutine += "\r\n(subroutine)\r\nO94 (" + Properties.Settings.Default.importGCPWMTextP94 + ")\r\n";
                        gcodeSubroutine += tmp.ToString();
                        gcodeSubroutine += "M99\r\n";
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
            get { return gcodeCompress; }
            set
            {
                gcodeCompress = value;
                SetDecimalPlaces((int)Properties.Settings.Default.importGCDecPlaces);
            }
        }

        public static bool IsEqual(double vala, double valb)
        { return (Math.Round(vala, decimalPlaces) == Math.Round(valb, decimalPlaces)); }

        public static void SetDecimalPlaces(int num)
        {
            formatNumber = "0.";
            if (gcodeCompress)
                formatNumber = formatNumber.PadRight(num + 2, '#'); //'0'
            else
                formatNumber = formatNumber.PadRight(num + 2, '0'); //'0'
        }

        // get GCode one or two digits
        public static int GetCodeNrFromGCode(char code, string tmp)
        {
            string cmdG = GetStrGCode(code, tmp);       // find number string
            if (cmdG.Length > 0)
            { return Convert.ToInt16(cmdG.Substring(1)); }
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

        // get value from X,Y,Z F, S etc.
        public static int GetParameterValue(char code, string tmp)
        {
            string cmdG = GetStringValue(code, tmp);
            if (cmdG.Length > 0)
            { return Convert.ToInt16(cmdG.Substring(1)); }
            return -1;
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
                gcodeLines++;
                gcodePauseCounter++;
            }
        }

        public static void SpindleOn(StringBuilder gcodeValue, string cmt)
        {
            if (gcodeValue != null)
            {
                if (string.IsNullOrEmpty(cmt)) cmt = "";
                if (gcodeSpindleToolTable && gcodeComments) { cmt += " spindle speed from tool-table"; }
                if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
                if (gcodeUseLasermode)  // in SpindleOn
                    gcodeValue.AppendFormat("S{0} {1}\r\n", GcodeSpindleSpeed, cmt);
                else
                {
                    gcodeValue.AppendFormat("M{0} S{1} {2}\r\n", GcodeSpindleCmd, GcodeSpindleSpeed, cmt);
                    gcodeLines++;
                    double delay = (double)Properties.Settings.Default.importGCSpindleDelay;
                    if (delay > 0)
                    {
                        string tmp = gcodeComments ? "( Delay )" : "";
                        gcodeValue.AppendFormat("G{0} P{1} {2}\r\n", FrmtCode(4), delay, tmp);
                        gcodeLines++;
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
                if (gcodeUseLasermode)  // in SpindleOff
                    gcodeValue.AppendFormat("S{0} {1}\r\n", 0, cmt);
                else
                    gcodeValue.AppendFormat("M{0} {1}\r\n", FrmtCode(5), cmt);
                gcodeLines++;
            }
        }

        public static void SetPwm(StringBuilder gcodeValue, float pwm, float delay)
        {
            if (gcodeValue != null)
            {
                gcodeValue.AppendFormat("M{0} S{1}\r\n", GcodeSpindleCmd, pwm);
                if (delay > 0)
                    gcodeValue.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(delay));
            }
        }

        public static void JobStart(StringBuilder gcodeValue, string cmto)
        {
            if (gcodeValue != null)
            {
                string cmt = cmto;
                if (!gcodeComments) cmt = "";

                if (GcodeZApply)    // pen up
                {
                    if (gcodeTangentialEnable && (gcodeTangentialName == "Z"))
                        gcodeValue.AppendFormat("( {0}-3001: Z is used as axis AND as tangential axis )\r\n", CodeMessage.Warning);
                    if ((gcodeAuxiliaryValue1Enable && (gcodeAuxiliaryValue1Name == "Z")) ||
                        (gcodeAuxiliaryValue2Enable && (gcodeAuxiliaryValue2Name == "Z")))
                        gcodeValue.AppendFormat("({0}-3002: Z is used as axis AND as auxiliary axis )\r\n", CodeMessage.Warning);

                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Z-Axis");
                    float tmpZUp = (float)Properties.Settings.Default.importGCZUp;
                    double z_relative = tmpZUp - lastz;
                    if (gcodeZFeedToolTable && gcodeComments) { cmt = cmto + " Z feed from tool-table"; }
                    if (cmt.Length > 0) { cmt = " (" + cmt + ")"; }
                    if (GcodeRelative)
                        gcodeValue.AppendFormat("G{0} Z{1}{2}\r\n", FrmtCode(0), FrmtNum(z_relative), cmt); // use G0 without feedrate
                    else
                        gcodeValue.AppendFormat("G{0} Z{1}{2}\r\n", FrmtCode(0), FrmtNum(tmpZUp), cmt); // use G0 without feedrate
                    gcodeTime += Math.Abs((tmpZUp - GcodeZDown) / GcodeZFeed);
                    gcodeLines++;
                }
                else
                {
                    PenUp(gcodeValue, "PU");
                }

                if (GcodeZApply || gcodeSpindleToggle)
                {
                    if (gcodeUseLasermode)  // in jobStart
                    {
                        if (gcodeComments) cmt = " (" + cmto + " lasermode )";
                        else cmt = "";
                        gcodeValue.AppendFormat("M{0} S{1}{2}\r\n", GcodeSpindleCmd, 0, cmt); // switch on laser with power=0
                    }
                    else
                    {
                        if (!gcodeToolChange)   // spindle on if no tool change
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
                if (GcodeZApply || gcodeSpindleToggle)
                {
                    if (!PreventSpindle)
                    { gcodeValue.AppendFormat("M{0}{1}\r\n", FrmtCode(5), cmt); }
                }
            }
        }

        private static StringBuilder tmpString = new StringBuilder();
        public static void PenDown(StringBuilder gcodeValue, string cmto)
        {
            if (LoggerTraceImport) Logger.Trace("    PenDown gcodeZDown:{0} lastz:{1}", GcodeZDown, lastz);

            tmpString = new StringBuilder();
            string cmt = cmto;
            bool penDownApplied = false;
            if (Properties.Settings.Default.importGCTTZAxis && gcodeComments && useValueFromToolTable) { cmt += " Z values from tool-table"; }
            if (GcodeRelative) { cmt += string.Format("rel {0}", lastz); }
            if (cmt.Length > 0) { cmt = string.Format("({0})", cmt); }

            ApplyXYFeedRate = true;     // apply XY Feed Rate after each PenDown command (not just after Z-axis)

            if (!(GcodeZApply && repeatZ))  // if true, do action in intermediateZ
            {
                if (gcodeSpindleToggle && !gcodeUseLasermode)   // 1st spindel, then Z
                {
                    if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Spindle-On");
                    SpindleOn(tmpString, cmto); penDownApplied = true;
                }
            }
            if (GcodeZApply)
            {
                if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Z-Axis");
                penDownApplied = true;
                if (repeatZ)
                {
                    figureString.Clear();   // Router down: a new figure will start
                    gcodeFigureTime = 0;
                    gcodeFigureLines = 0;  //
                    gcodeFigureDistance = 0;
                    if (LoggerTraceImport) Logger.Trace("    figureString.Clear()");
                }
                else
                {
                    double z_relative = GcodeZDown - lastz;
                    if (Math.Abs(z_relative) > 0)
                    {
                        if (GcodeRelative)
                            tmpString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(z_relative), GcodeZFeed, cmt);
                        else
                        {
                            if (gcodeZLeadInEnable)
                            { tmpString.AppendFormat("G{0} X{1} Y{2} Z{3} F{4} {5}\r\n", FrmtCode(1), FrmtNum(gcodeZLeadInXY.X), FrmtNum(gcodeZLeadInXY.Y), FrmtNum(GcodeZDown), GcodeZFeed, cmt); }
                            else
                            { tmpString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(GcodeZDown), GcodeZFeed, cmt); }
                        }
                        gcodeTime += Math.Abs((GcodeZUp - GcodeZDown) / GcodeZFeed);
                        gcodeLines++;
                        //						penDownApplied = true;
                    }
                }
                lastg = 1; lastf = GcodeZFeed;
                lastz = GcodeZDown;
            }
            if (!(GcodeZApply && repeatZ))  // if true, do action in intermediateZ
            {
                if (gcodeUseLasermode && !GcodePWMEnable)  // 1st Z, then Laser added 2023-06-26 
                {
                    gcodeValue.AppendFormat("M{0} S{1:0} {2}\r\n", GcodeSpindleCmd, GcodeSpindleSpeed, cmt);  //2022-03-15
                    if ((!GcodeZApply) && (LastMovewasG0))
                    {
                        // %NM tag, to keep code-line when synthezising code
                        gcodeValue.AppendFormat("G91 G1 X0.001 F{0} ( %NM use Laser mode)\r\n", GcodeXYFeed);
                        gcodeValue.AppendFormat("G91 G1 X-0.001     ( %NM G1 move to activate laser)\r\n");
                        if (!GcodeRelative)
                        { gcodeValue.AppendFormat("G90 ( %NM )\r\n"); }
                    }
                    penDownApplied = true;
                }
                if (GcodePWMEnable)
                {
                    if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Servo control");
                    tmpString.AppendFormat("M{0} S{1} {2}\r\n", GcodeSpindleCmd, GcodePwmDown, cmt);
                    if (GcodePwmDlyDown > 0)
                        tmpString.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(GcodePwmDlyDown));
                    gcodeTime += GcodePwmDlyDown;
                    gcodeLines++;
                    penDownApplied = true;
                }
                if (gcodeIndividualTool)
                {
                    if (gcodeComments) tmpString.AppendFormat("({0})\r\n", "Pen down: Individual Cmd");
                    string[] commands = gcodeIndividualDown.Split(';');
                    foreach (string cmd in commands)
                    { tmpString.AppendFormat("{0} {1}\r\n", cmd.Trim(), cmt); }     // 2022-03-25 add {1}
                    penDownApplied = true;
                }
            }
            gcodeDownUp++;
            if (!penDownApplied)
                tmpString.AppendLine(" (PD)");  // mark pen down
                                                //   lastCharCount = tmpString.Length;
            gcodeValue?.Append(tmpString);

        }

        public static void PenUp(StringBuilder gcodeValue, string cmto)
        {
            if (LoggerTraceImport) Logger.Trace("    PenUp");
            if (gcodeValue == null)
            { return; }

            string cmt = cmto;
            string comment = "";
            bool penUpApplied = false;
            if (GcodeRelative) { cmt += string.Format("rel {0}", lastz); }
            if (cmt.Length > 0) { comment = string.Format("({0})", cmt); }      // 2022-03-25 move up

            if (!(GcodeZApply && repeatZ))  // if true, do action in intermediateZ
            {
                if (gcodeIndividualTool)
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Individual Cmd");
                    string[] commands = gcodeIndividualUp.Split(';');
                    foreach (string cmd in commands)
                    { gcodeValue.AppendFormat("{0} {1}\r\n", cmd.Trim(), comment); }    // 2022-03-25 add {1}
                    penUpApplied = true;
                }

                if (GcodePWMEnable)
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Servo control");
                    //    if (cmt.Length > 0) { comment = string.Format("({0})", cmt); }		// 2022-03-25 move up
                    gcodeValue.AppendFormat("M{0} S{1} {2}\r\n", GcodeSpindleCmd, gcodePwmUp, comment);
                    if (GcodePwmDlyUp > 0)
                        gcodeValue.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(GcodePwmDlyUp));
                    gcodeTime += GcodePwmDlyUp;
                    gcodeLines++;
                    penUpApplied = true;
                }

                if (gcodeUseLasermode && !GcodePWMEnable)  // 1st Z, then Laser
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Laser-Off");
                    //    SpindleOff(gcodeValue, cmto);
                    gcodeValue.AppendFormat("M{0} S0 ({1} Lasermode: S0 instead of M5 to switch laser off)\r\n", GcodeSpindleCmd, cmt);  //2022-03-15
                    /*     if ((!GcodeZApply) && (LastMovewasG0))
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
            if (GcodeZApply)
            {
                if (repeatZ)
                {
                    if (figureString.Length > 0)
                        IntermediateZ(gcodeValue);      // figure finished, repeat code for several depth++ per pass
                    else
                        Drill(gcodeValue);
                }
                else
                {
                    if (gcodeComments) gcodeValue.AppendFormat("({0})\r\n", "Pen up: Z-Axis");

                    double z_relative = GcodeZUp - lastz;
                    if (Math.Abs(z_relative) > 0)
                    {
                        if (gcodeZFeedToolTable && gcodeComments) { cmt += " Z feed from tool-table"; }
                        if (cmt.Length > 0) { comment = string.Format("({0})", cmt); }

                        if (GcodeRelative)
                        { gcodeValue.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(z_relative), comment); }// use G0 without feedrate
                        else
                        {
                            if (gcodeZLeadOutEnable)
                            { gcodeValue.AppendFormat("G{0} X{1} Y{2} Z{3} F{4} {5}\r\n", FrmtCode(1), FrmtNum(gcodeZLeadOutXY.X), FrmtNum(gcodeZLeadOutXY.Y), FrmtNum(GcodeZUp), GcodeZFeed, comment); }
                            else
                            { gcodeValue.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(GcodeZUp), comment); }
                        }// use G0 without feedrate
                        gcodeTime += Math.Abs((GcodeZUp - GcodeZDown) / GcodeZFeed);
                        gcodeLines++;
                    }
                }
                lastg = 1; lastf = GcodeZFeed;
                lastz = GcodeZUp;
                penUpApplied = true;
            }

            if (!(GcodeZApply && repeatZ))
            {
                if (gcodeSpindleToggle && !gcodeUseLasermode)
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
        public static bool LastMovewasG0 { get; set; } //= true;
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
            figureStartAlpha = gcodeTangentialCommand;
            if (!(GcodeZApply && repeatZ))
            { Move(gcodeValue, 0, coord.X, coord.Y, false, cmt); LastMovewasG0 = true; }
            else
            {
                lastx = coord.X; lasty = coord.Y; lastg = 0;
            }
        }
        public static void MoveToRapid(StringBuilder gcodeValue, double mx, double my, string cmt)
        {
            figureStart.X = mx; figureStart.Y = my;
            figureStartAlpha = gcodeTangentialCommand;
            if (!(GcodeZApply && repeatZ))
            { Move(gcodeValue, 0, mx, my, false, cmt); LastMovewasG0 = true; }
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
            if (LastMovewasG0)
            {
                if ((finalx == lastx) && (finaly == lasty)) // discard G1 without any move
                { return; }
            }

            StringBuilder gcodeString = gcodeStringFinal;

            if (gnr != 0)
            {
                if (GcodeZApply && repeatZ)
                { gcodeString = figureString; }// if (loggerTrace) Logger.Trace("    gcodeString = figureString"); }
            }

            if (gcodeLineSegmentationEnable)       // apply segmentation
            {
                double segFinalX = finalx, segFinalY = finaly;
                double dx = finalx - lastx;       // remaining distance until full move
                double dy = finaly - lasty;       // lastXY is global
                double moveLength = (float)Math.Sqrt(dx * dx + dy * dy);
                float segmentLength = gcodeLineSegmentLength;
                if (gcodeLineSegmentLength <= 0.01)
                    segmentLength = float.MaxValue;
                bool equidistance = gcodeLineSegmentEquidistant;

                // add subroutine at the beginning of each path (after G0 move)
                if (gcodeLineSegementSubroutineOnPathStart && (LastMovewasG0 || (moveLength >= segmentLength)))       // also subroutine at first point
                {
                    if (gcodeInsertSubroutineEnable)
                        applyFeed = InsertSubroutine(gcodeString, lastx, lasty);//, lastz, applyFeed);
                    remainingC = segmentLength;
                }

                if (gcodeLineSegmentLength > 0) // only do calculations if segmentation length is > 0
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
                                if (gcodeInsertSubroutineEnable)
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
                                if (gcodeInsertSubroutineEnable)
                                    //    applyFeed = insertSubroutine(gcodeString, lastx, lasty, lastz, applyFeed);    // edit 2021-06-20
                                    applyFeed = InsertSubroutine(gcodeString, tmpX, tmpY);//, lastz, applyFeed);
                            }
                        }
                        finalx = segFinalX; finaly = segFinalY;
                        remainingC = segmentLength - Fdistance(finalx, finaly, lastx, lasty);
                        Move(gcodeString, 1, finalx, finaly, z, applyFeed, cmt);
                        if ((equidistance) || (remainingC == 0))
                        {
                            if (gcodeInsertSubroutineEnable)
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
            LastMovewasG0 = false;
        }

        //    private static float origLastX, origLastY, origFinalX, origFinalY;

        // process subroutine, afterwards move back to last regular position before subroutine        
        private static bool InsertSubroutine(StringBuilder gcodeString, double lX, double lY)//, float lZ, bool applyFeed)
        {
            //Logger.Trace("insertSubroutine");
            bool xyfeedneeded = true;
            if (gcodeInsertSubroutinePenUpDown) PenUp(gcodeString, "PU");
            gcodeString.AppendFormat("M98 P99 (call subroutine)\r\n");
            if (gcodeInsertSubroutinePenUpDown)
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

            gcodeSubroutineCount++;
            if (gcodeSubroutineEnable == 0)     // read file once
            {
                SetSubroutine(Properties.Settings.Default.importGCSubroutine, 99);
                /*    string file = Properties.Settings.Default.importGCSubroutine;
                    gcodeSubroutine += "\r\n(subroutine)\r\nO99\r\n";
                    if (File.Exists(file))
                        gcodeSubroutine += File.ReadAllText(file);
                    else
                        gcodeSubroutine += "(file " + file + " not found)\r\n";
                    gcodeSubroutine += "M99\r\n";*/

                SetSubroutine(Properties.Settings.Default.importCircleToDotScript, 95);
                /*    file = Properties.Settings.Default.importCircleToDotScript;
                    gcodeSubroutine += "\r\n(subroutine)\r\nO95\r\n";
                    if (File.Exists(file))
                        gcodeSubroutine += File.ReadAllText(file);
                    else
                        gcodeSubroutine += "(file " + file + " not found)\r\n";
                    gcodeSubroutine += "M99\r\n";*/

                gcodeSubroutineEnable++;
            }
            return xyfeedneeded;    // applyFeed is needed
        }

        public static void SetSubroutine(string file, int nr)
        {
            gcodeSubroutine += "\r\n(subroutine)\r\nO" + nr.ToString() + "\r\n";
            if (File.Exists(file))
                gcodeSubroutine += File.ReadAllText(file);
            else
                gcodeSubroutine += "(file " + file + " not found)\r\n";
            gcodeSubroutine += "M99\r\n";
        }
        public static void CallSubroutine(StringBuilder gcodeString, int nr, string cmt)
        {
            gcodeString.AppendFormat("M98 P{0} ({1})\r\n", nr, cmt);
        }


        public static void SplitLine(StringBuilder gcodeValue, int gnr, float x1, float y1, float x2, float y2, float maxStep, bool applyFeed, string cmt)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            float c = (float)Math.Sqrt(dx * dx + dy * dy);
            float tmpX, tmpY;
            int divid = (int)Math.Ceiling(c / maxStep);
            lastg = -1;
            for (int i = 1; i <= divid; i++)
            {
                tmpX = x1 + i * dx / divid;
                tmpY = y1 + i * dy / divid;
                if (i > 1) { applyFeed = false; cmt = ""; }
                if (gnr == 0)
                { Move(gcodeValue, gnr, tmpX, tmpY, false, cmt); }
                else
                { Move(gcodeValue, gnr, tmpX, tmpY, applyFeed, cmt); }
            }
        }
        public static void SplitLineZ(StringBuilder gcodeValue, int gnr, float x1, float y1, float z1, float x2, float y2, float z2, float maxStep, bool applyFeed, string cmt)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            float dz = z2 - z1;
            float c = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);

            float tmpX, tmpY, tmpZ;
            int divid = (int)Math.Ceiling(c / maxStep);
            lastg = -1;
            for (int i = 1; i <= divid; i++)
            {
                tmpX = x1 + i * dx / divid;
                tmpY = y1 + i * dy / divid;
                tmpZ = z1 + i * dz / divid;
                if (i > 1) { applyFeed = false; cmt = ""; }
                if (gnr == 0)
                { Move(gcodeValue, gnr, tmpX, tmpY, tmpZ, false, cmt); }
                else
                { Move(gcodeValue, gnr, tmpX, tmpY, tmpZ, applyFeed, cmt); }
            }
        }

        public static void ClearLeadIn()
        { gcodeZLeadInEnable = false; }
        public static void SetZStartPos(Point xy)
        {
            gcodeZLeadInXY = new XyPoint(xy);
            gcodeZLeadInEnable = true;
        }
        public static void ClearLeadOut()
        { gcodeZLeadOutEnable = false; }
        public static void SetZEndPos(Point xy)
        {
            gcodeZLeadOutXY = new XyPoint(xy);
            gcodeZLeadOutEnable = true;
        }

        public static void SplitArc(StringBuilder gcodeValue, int gnr, double x1, double y1, double x2, double y2, double i1, double j2, string cmt)
        {
            if (string.IsNullOrEmpty(cmt)) cmt = "";
            double segmentLength = (double)Properties.Settings.Default.importGCLineSegmentLength;
            bool equidistance = Properties.Settings.Default.importGCLineSegmentEquidistant;

            ArcProperties arcMove;
            XyPoint p1 = new XyPoint(x1, y1);
            XyPoint p2 = new XyPoint(x2, y2);
            p1.Round(); p2.Round();
            arcMove = GcodeMath.GetArcMoveProperties(p1, p2, i1, j2, (gnr == 2)); // 2020-04-14 add round()
            double step = Math.Asin(gcodeAngleStep / arcMove.radius);     // in RAD
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

            if (arcMove.angleDiff > 0)   //(da > 0)                                             // if delta >0 go counter clock wise
            {
                for (double angle = (arcMove.angleStart + step); angle < (arcMove.angleStart + arcMove.angleDiff); angle += step)
                {
                    double x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                    double y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                    moveLength += Fdistance(x, y, lastx, lasty);
                    Move(gcodeValue, 1, x, y, ApplyXYFeedRate, cmt);
                    if (moveLength >= (count * segmentLength))
                    {
                        if (gcodeInsertSubroutineEnable)
                        { if (gcodeValue != null) ApplyXYFeedRate = InsertSubroutine(gcodeValue, lastx, lasty); }//, lastz, ApplyXYFeedRate); }
                        count++;
                    }
                    if (cmt.Length > 1) cmt = "";
                }
            }
            else                                                       // else go clock wise
            {
                for (double angle = (arcMove.angleStart - step); angle > (arcMove.angleStart + arcMove.angleDiff); angle -= step)
                {
                    double x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                    double y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                    moveLength += Fdistance(x, y, lastx, lasty);
                    Move(gcodeValue, 1, x, y, ApplyXYFeedRate, cmt);
                    if (moveLength >= (count * segmentLength))
                    {
                        if (gcodeInsertSubroutineEnable)
                        //    applyXYFeedRate = insertSubroutine(gcodeString, lastx, lasty, lastz, applyXYFeedRate);    //2021-06-20
                        { if (gcodeValue != null) ApplyXYFeedRate = InsertSubroutine(gcodeValue, (float)x, (float)y); }//, lastz, ApplyXYFeedRate); }
                        count++;
                    }
                    if (cmt.Length > 1) cmt = "";
                }
            }
            Move(gcodeValue, 1, x2, y2, ApplyXYFeedRate, "End Arc conversion");
            if ((moveLength >= (count * segmentLength)) || equidistance)
            {
                if (gcodeInsertSubroutineEnable)
                //    applyXYFeedRate = insertSubroutine(gcodeString, lastx, lasty, lastz, applyXYFeedRate);    //2021-06-20
                { if (gcodeValue != null) ApplyXYFeedRate = InsertSubroutine(gcodeValue, (float)x2, (float)y2); }//, lastz, ApplyXYFeedRate); }
                                                                                                                 //    moveLength = 0;
            }
        }

        public static void Tool(StringBuilder gcodeValue, int toolnr, int objectCount, string cmt)
        {
            if (string.IsNullOrEmpty(cmt)) cmt = "";
            if (gcodeValue == null) return;

            if (Properties.Settings.Default.importGCToolTableUse && Properties.Settings.Default.importGCToolDefNrUse)
                toolnr = (int)Properties.Settings.Default.importGCToolDefNr;

            string toolCmd;
            if (gcodeToolChange)                // otherweise no command needed
            {
                if (GcodeZApply && !PreventSpindle)
                { Gcode.SpindleOff(gcodeValue, "Stop spindle - Option Z-Axis"); }

                toolCmd = string.Format("T{0:D2} M{1} (Tool:{2} {3})", toolnr, FrmtCode(6), toolnr, cmt);

                if (gcodeToolChangeM0)
                { gcodeValue.AppendFormat("M0 (Tool:{0}  Color:{1})\r\n", toolnr, cmt); gcodeLines++; }
                else
                { gcodeValue.AppendFormat("{0}\r\n", toolCmd); gcodeLines++; }
                gcodeToolCounter++;
                gcodeLines++;
                string objects = "";
                if (objectCount > 0)
                    objects = string.Format("Cnt: {0,3} ", objectCount);
                gcodeToolText += string.Format("( {0} ToolNr: {1:D2} {2} Name: {3} )\r\n", gcodeToolCounter, toolnr, objects, cmt);

                remainingC = (float)Properties.Settings.Default.importGCLineSegmentLength;	// start with full segment length

                if (GcodeZApply && !gcodeSpindleToggle && !PreventSpindle)
                { Gcode.SpindleOn(gcodeValue, "Start spindle - Option Z-Axis"); gcodeLines++; }
            }

            // add gcode from tool table
            ToolProp toolInfo = ToolTable.GetToolProperties(toolnr);
            GetValuesFromToolTable(toolInfo);
            if (LoggerTraceImport) Logger.Trace("   toolInfo toolNr {0} {1} {2}", toolnr, toolInfo.SpindleSpeed, toolInfo.Gcode.Length);
            if (Properties.Settings.Default.importGCToolTableUse)
            {
                if (toolInfo.Gcode.Length > 1)
                {
                    string[] commands = toolInfo.Gcode.Split(';');
                    string comment1 = "", comment2 = "", comment3 = "(PU) ";
                    if (gcodeComments) { comment1 = "(tool-table use gcode)"; comment2 = "(tool-table wrap gcode)"; comment3 += "(tool-table finally)"; }

                    gcodeValue.AppendFormat("G04 P0.5 {0}\r\n", comment2);
                    foreach (string btncmd in commands)
                    { gcodeValue.AppendFormat("{0} {1}\r\n", btncmd.Trim(), comment1); }
                    gcodeValue.AppendFormat("G04 P0.5 {0}\r\n", comment2);

                    if (GcodeZApply)
                    { gcodeValue.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(GcodeZUp), comment3); }
                }
            }
        }

        internal static void GetValuesFromToolTable(ToolProp toolInfo)
        {
            //   Logger.Info("GetValuesFromToolTable");
            if (toolInfo == null)
            { Logger.Error("GetValuesFromToolTable toolInfo is null"); return; }
            if (Properties.Settings.Default.importGCToolTableUse)
            {
                if (Properties.Settings.Default.importGCTTSSpeed)
                    GcodeSpindleSpeed = GcodePwmDown = toolInfo.SpindleSpeed;
                if (Properties.Settings.Default.importGCTTXYFeed)
                    GcodeXYFeed = toolInfo.FeedXY;
                if (Properties.Settings.Default.importGCTTZAxis)
                {
                    GcodeZFeed = toolInfo.FeedZ;
                    GcodeZUp = toolInfo.SaveZ;
                    GcodeZDown = toolInfo.FinalZ;   // stepZ;
                    GcodeZInc = Math.Abs(toolInfo.StepZ);
                }
            }
        }

        public static void AddToHeader(string cmt, bool insideTag = true)
        {
            if (insideTag)
                headerData.AppendFormat("({0})\r\n", cmt);
            else
                headerMessage.AppendFormat("({0})\r\n", cmt);
        }

        private static string docTitle = "";
        private static string docDescription = "";

        public static void SetHeaderInfo(string title, float distance, float feed, int lines, int downUp)
        {
            Logger.Trace("SetHeaderInfo title:{0} distance:{1} feed:{2} lines:{3} downUp:{4}", title, distance, feed, lines, downUp);
            docTitle = title;
            gcodeDistance = distance;
            GcodeXYFeed = feed;
            gcodeLines = lines;
            gcodeDownUp = downUp;
            GcodeZApply = true;
        }
        public static string GetHeader(string cmt, string source)
        {
            gcodeTime += gcodeDistance / GcodeXYFeed;
            string header = string.Format("( {0} by GRBL-Plotter {1} )\r\n", cmt, MyApplication.GetVersion());
            string header_end = headerData.ToString();
            header_end += string.Format("({0} >)\r\n", XmlMarker.HeaderEnd);
            header_end += headerMessage.ToString();

            if (Properties.Settings.Default.importGCConvertToPolar)
                header_end += string.Format("({0} X=radius, Y=angle/>)\r\n", "Polar");

            if (gcodeTangentialEnable)
                header_end += string.Format("({0} Axis=\"{1}\"/>)\r\n", XmlMarker.TangentialAxis, gcodeTangentialName);

            string[] commands = Properties.Settings.Default.importGCHeader.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                { header_end += string.Format("{0} (Setup - GCode-Header)\r\n", cmd.Trim()); gcodeLines++; }
            if (GcodeRelative)
            { header_end += string.Format("G91 (Setup RELATIVE movement)\r\n"); gcodeLines++; }
            else
            { header_end += string.Format("G90\r\n"); gcodeLines++; }

            if (Properties.Settings.Default.importUnitGCode)
            {
                if (Properties.Settings.Default.importUnitmm)
                { header_end += "G21 (use mm as unit - check setup)\r\n"; gcodeLines++; }
                else
                { header_end += "G20 (use inch as unit - check setup)\r\n"; gcodeLines++; }
            }

            // take account of footer lines
            commands = Properties.Settings.Default.importGCFooter.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                { gcodeLines++; }
            if (gcodeToolChange && Properties.Settings.Default.ctrlToolChangeEmpty)
            { gcodeLines++; }
            gcodeLines++;


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

            if (!(GcodeZApply || GcodePWMEnable || gcodeSpindleToggle || gcodeIndividualTool))
            { header += string.Format("( !!! No Pen up/down translation !!! )\r\n"); }
            if (Properties.Settings.Default.importRemoveShortMovesEnable && ((float)Properties.Settings.Default.importRemoveShortMovesLimit > 0.2))
            { header += string.Format("( !!! Remove short moves < {0:0.00} !!! )\r\n", Properties.Settings.Default.importRemoveShortMovesLimit); }
            if ((float)Properties.Settings.Default.importAssumeAsEqualDistance > 0.01)
            { header += string.Format("( !!! Assume as equal distance {0:0.00} !!! )\r\n", Properties.Settings.Default.importAssumeAsEqualDistance); }

            header += string.Format("({0} >)\r\n", XmlMarker.HeaderStart);

            if (Properties.Settings.Default.importRepeatEnable)
            {
                header += string.Format("( G-Code repetitions: {0:0} times)\r\n", Properties.Settings.Default.importRepeatCnt);
                Logger.Info("◆◆  Header: G-Code repetitions:{0}", Properties.Settings.Default.importRepeatCnt);
            }

            header += string.Format("( G-Code lines: {0} )\r\n", gcodeLines);
            Logger.Info("◆◆  Header: G-Code lines:{0}", gcodeLines);

            header += string.Format("( Pen Down/Up : {0} times )\r\n", gcodeDownUp);
            //           header += string.Format("( Path length : {0:0.0} units )\r\n", gcodeDistance);
            header += string.Format("( Duration ca.: {0:0.0} min. )\r\n", gcodeTime);
            if (gcodeSubroutineCount > 0)
            {
                header += string.Format("( Call to subs.: {0} )\r\n", gcodeSubroutineCount);
                Logger.Info("◆◆  Header: Subroutine calls:{0}", gcodeSubroutineCount);
            }

            stopwatch.Stop();
            header += string.Format("( Conv. time  : {0} )\r\n", stopwatch.Elapsed);

            if (Properties.Settings.Default.importGCToolTableUse)
            {
                header += "( Values from tool-table: ";
                if (Properties.Settings.Default.importGCTTSSpeed) { header += "spindle speed, "; }
                if (Properties.Settings.Default.importGCTTXYFeed) { header += "XY feed, "; }
                if (Properties.Settings.Default.importGCTTZAxis) { header += "Z Values "; }
                header += ")\r\n";
            }

            if (gcodeToolChange)
            {
                header += string.Format("( Tool changes: {0})\r\n", gcodeToolCounter);
                header += gcodeToolText;
                Logger.Info("◆◆  Header: Tool changes:{0}", gcodeToolCounter);
            }
            if (gcodePauseCounter > 0)
                header += string.Format("( M0 count    : {0})\r\n", gcodePauseCounter);

            //			header_end += string.Format("({0} >)\r\n", xmlMarker.headerEnd);
            return header + header_end;
        }

        public static string GetFooter()
        {
            string footer = "";

            if (gcodeToolChange && Properties.Settings.Default.ctrlToolChangeEmpty)
            { footer += string.Format("T{0} M{1} (Remove tool)\r\n", FrmtCode((int)Properties.Settings.Default.ctrlToolChangeEmptyNr), FrmtCode(6)); }

            string[] commands = Properties.Settings.Default.importGCFooter.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                { footer += string.Format("{0} (Setup - GCode-Footer)\r\n", cmd.Trim()); }

            if (Properties.Settings.Default.importGCPWMEnable && Properties.Settings.Default.importGCPWMSkipM30)
            { footer += "M30 (SKIP M30)\r\n"; }
            else
                footer += "M30\r\n";

            return footer + gcodeSubroutine;
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
            finalZ = GcodeZDown;
            string cmt = "";
            bool fromTT = false;
            if (Properties.Settings.Default.importGCTTZAxis) { cmt += " Z final, "; fromTT = true; }
            //     if (Properties.Settings.Default.importGCTTZIncrement) { cmt += " Z step, "; fromTT = true; }
            if (gcodeZFeedToolTable) { cmt += " Z feed "; fromTT = true; }
            if (fromTT && gcodeComments) { cmt += " from tool-table"; }
            cmt = "( " + cmt + " )";
            if (repeatZStartZero)       // perfom 1st pass at zero
                zStep = GcodeZInc;
            while (zStep > finalZ)      // repeat, until finalZ reached
            {
                zStep -= GcodeZInc;     // reduce Z by inc
                if (zStep < finalZ)
                    zStep = finalZ;
                GcodeZDown = zStep;
                Comment(gcodeString, string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\" >", XmlMarker.PassStart, passCount, zStep, finalZ));
                if (passCount <= 1)
                {
                    gcodeTangentialCommand = figureStartAlpha;
                    Move(gcodeString, 0, (float)figureStart.X, (float)figureStart.Y, false, "");
                    LastMovewasG0 = true;
                }
                gcodeString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(zStep), GcodeZFeed, cmt);    // Router down
                gcodeDownUp++;

                if (!gcodeZNoUp)
                    gcodeString.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(GcodeZUp), "");                  // Router up

                Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ passCount.ToString() + ">");
                passCount++;

                gcodeTime += gcodeFigureTime;
                gcodeLines += gcodeFigureLines + 3;
                gcodeDistance += gcodeFigureDistance;
            }
            figureString.Clear();
        }
        private static void IntermediateZ(StringBuilder gcodeString)
        {
            if (LoggerTraceImport) Logger.Trace("   intermediateZ");

            float zStep = 0;
            int passCount = 1;
            finalZ = GcodeZDown;
            string cmt = "";
            string xml;
            bool fromTT = false;
            if (Properties.Settings.Default.importGCTTZAxis) { cmt += " Z final, "; fromTT = true; }
            //     if (Properties.Settings.Default.importGCTTZIncrement) { cmt += " Z step, "; fromTT = true; }
            if (gcodeZFeedToolTable) { cmt += " Z feed "; fromTT = true; }
            if (fromTT && gcodeComments) { cmt += " from tool-table"; }
            cmt = "( " + cmt + " )";
            if (repeatZStartZero)       // perfom 1st pass at zero
                zStep = GcodeZInc;
            while (zStep > finalZ)      // repeat, until finalZ reached
            {
                zStep -= GcodeZInc;     // reduce Z by inc
                if (zStep < finalZ)
                    zStep = finalZ;
                GcodeZDown = zStep;
                xml = string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\" >", XmlMarker.PassStart, passCount, zStep, finalZ);
                Comment(gcodeString, xml);
                if (LoggerTraceImport) Logger.Trace("{0}", xml);
                gcodeTangentialCommand = figureStartAlpha;
                Move(gcodeString, 0, (float)figureStart.X, (float)figureStart.Y, false, ""); LastMovewasG0 = true;
                gcodeFigureLines--; // avoid double count

                // PenDown
                if (gcodeSpindleToggle && !gcodeUseLasermode) SpindleOn(gcodeString, "toggle");                     // send M3/4 + G4 delay
                gcodeString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", FrmtCode(1), FrmtNum(zStep), GcodeZFeed, cmt);   // Router down
                if (gcodeUseLasermode) SpindleOn(gcodeString, "lasermode");                                         // send S1000

                if (GcodePWMEnable)
                {
                    if (gcodeComments) gcodeString.AppendFormat("({0})\r\n", "Pen down: Servo control");
                    gcodeString.AppendFormat("M{0} S{1} {2}\r\n", GcodeSpindleCmd, GcodePwmDown, cmt);
                    if (GcodePwmDlyDown > 0)
                        gcodeString.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(GcodePwmDlyDown));
                }
                if (gcodeIndividualTool)
                {
                    foreach (string cmd in gcodeIndividualDown.Split(';'))
                    { gcodeString.AppendFormat("{0}\r\n", cmd.Trim()); }
                }

                gcodeDownUp++;
                gcodeString.Append(figureString.ToString());                                                        // draw figure
                if (LoggerTraceImport) Logger.Trace(" intermediateZ Copy code");

                // PenUp
                if (gcodeIndividualTool)
                {
                    foreach (string cmd in gcodeIndividualUp.Split(';'))
                    { gcodeString.AppendFormat("{0}\r\n", cmd.Trim()); }
                }
                if (GcodePWMEnable)
                {
                    gcodeString.AppendFormat("M{0} S{1}\r\n", GcodeSpindleCmd, gcodePwmUp);
                    if (GcodePwmDlyUp > 0)
                        gcodeString.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(GcodePwmDlyUp));
                    gcodeTime += GcodePwmDlyUp;
                    gcodeLines++;
                }

                if (!gcodeZNoUp)
                {
                    if (gcodeUseLasermode) SpindleOff(gcodeString, "lasermode");                        // send S0
                    gcodeString.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(GcodeZUp), "");  // Router up
                    if (gcodeSpindleToggle && !gcodeUseLasermode) SpindleOff(gcodeString, "toggle");    // send M5
                }

                Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ passCount.ToString() + ">");
                passCount++;

                gcodeTime += gcodeFigureTime;
                gcodeLines += gcodeFigureLines + 3;
                gcodeDistance += gcodeFigureDistance;
            }
            figureString.Clear();
        }

        // helper functions
        private static double Fsqrt(double x) { return Math.Sqrt(x); }
        private static double Fvmag(double x, double y) { return Fsqrt(x * x + y * y); }
        private static float Fdistance(double x1, double y1, double x2, double y2) { return (float)Fvmag(x2 - x1, y2 - y1); }
    }
}
