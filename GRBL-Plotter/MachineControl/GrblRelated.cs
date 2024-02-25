/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2016-12-31 Add GRBL 1.1 information
 * 2018-04-07 reorder
 * 2018-01-01 edit parseStatus to identify also Hold:0
 * 2019-05-10 move _serial_form.isGrblVers0 to here grbl.isVersion_0
 * https://github.com/fra589/grbl-Mega-5X
 * 2019-08-13 add PRB, TLO status
 * 2020-01-04 add "errorBecauseOfBadCode"
 * 2020-01-13 localization of grblStatus (Idle, run, hold...)
 * 2020-08-08 #145
 * 2021-01-16 StreamEventArgs : EventArgs -> switch from float to int for codeFinish, buffFinish %
 * 2021-05-01 return last index of splitted error, to catch "error: Invalid gcode ID:24" line 417
 * 2021-07-26 code clean up / code quality
 * 2021-09-29 add Status
 * 2021-11-03 support VoidMicro controller: https://github.com/arkypita/LaserGRBL/issues/1640
 * 2024-02-14 add grblDigialIn -Out to process grbl-Mega-5X I/O status
 * 2024-02-24 add Grbl.StatMsg
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace GrblPlotter
{
    internal static class Grbl
    {       // need to have global access to this data?
        internal static XyzPoint posWCO = new XyzPoint(0, 0, 0);
        internal static XyzPoint posWork = new XyzPoint(0, 0, 0);
        internal static XyzPoint posMachine = new XyzPoint(0, 0, 0);
        internal static GrblState Status = GrblState.unknown;
        internal static ModState StatMsg = new ModState();
        internal static Byte grblDigitalIn = 0;
		internal static Byte grblDigitalOut = 0;
		
        public static bool posChanged = true;
        public static bool wcoChanged = true;

        public static bool isVersion_0 = true;  // note if grbl version <=0.9 or >=1.1
        public static bool isMarlin = false;
        public static bool isConnected = false;

        public static int axisCount = 0;
        public static bool axisA = false;       // axis A available?
        public static bool axisB = false;       // axis B available?
        public static bool axisC = false;       // axis C available?
        public static bool axisUpdate = false;  // update of GUI needed
        public static int RX_BUFFER_SIZE = 127; // grbl buffer size inside Arduino
        public static int pollInterval = 200;
        public static int bufferSize = -1;
        public static string lastMessage = "";
        public static short lastErrorNr = 0;
        public static int DefaultFeed = 1000;

        public static bool grblSimulate = false;
        private static readonly Dictionary<int, float> settings = new Dictionary<int, float>();    // keep $$-settings
        private static readonly Dictionary<string, XyzPoint> coordinates = new Dictionary<string, XyzPoint>();    // keep []-settings
        private static readonly Dictionary<string, string> messages = new Dictionary<string, string>();

        private static XyzPoint _posMarker = new XyzPoint(0, 0, 0);
        private static double _posMarkerAngle = 0;
        //      private static XyzPoint _posMarkerOld = new XyzPoint(0, 0, 0);
        internal static XyzPoint PosMarker
        {
            get
            { return _posMarker; }
            set
            { _posMarker = value; }
        }
        public static double PosMarkerAngle
        {
            get
            { return _posMarkerAngle; }
            set
            { _posMarkerAngle = value; }
        }

        public static Dictionary<string, string> messageAlarmCodes = new Dictionary<string, string>();
        public static Dictionary<string, string> messageErrorCodes = new Dictionary<string, string>();
        public static Dictionary<string, string> messageSettingCodes = new Dictionary<string, string>();
        private static readonly StatConvert[] statusConvert = new StatConvert[11];

        public static void Init()   // initialize lists
        {
            SetMessageString(ref messageAlarmCodes, Properties.Resources.alarm_codes_en_US);
            SetMessageString(ref messageErrorCodes, Properties.Resources.error_codes_en_US);
            SetMessageString(ref messageSettingCodes, Properties.Resources.setting_codes_en_US);

            //    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };
            statusConvert[0].msg = "Idle"; statusConvert[0].text = Localization.GetString("grblIdle"); statusConvert[0].state = GrblState.idle; statusConvert[0].color = Color.Lime;
            statusConvert[1].msg = "Run"; statusConvert[1].text = Localization.GetString("grblRun"); statusConvert[1].state = GrblState.run; statusConvert[1].color = Color.Yellow;
            statusConvert[2].msg = "Hold"; statusConvert[2].text = Localization.GetString("grblHold"); statusConvert[2].state = GrblState.hold; statusConvert[2].color = Color.YellowGreen;
            statusConvert[3].msg = "Jog"; statusConvert[3].text = Localization.GetString("grblJog"); statusConvert[3].state = GrblState.jog; statusConvert[3].color = Color.LightGreen;
            statusConvert[4].msg = "Alarm"; statusConvert[4].text = Localization.GetString("grblAlarm"); statusConvert[4].state = GrblState.alarm; statusConvert[4].color = Color.Red;
            statusConvert[5].msg = "Door"; statusConvert[5].text = Localization.GetString("grblDoor"); statusConvert[5].state = GrblState.door; statusConvert[5].color = Color.Orange;
            statusConvert[6].msg = "Check"; statusConvert[6].text = Localization.GetString("grblCheck"); statusConvert[6].state = GrblState.check; statusConvert[6].color = Color.Orange;
            statusConvert[7].msg = "Home"; statusConvert[7].text = Localization.GetString("grblHome"); statusConvert[7].state = GrblState.home; statusConvert[7].color = Color.Magenta;
            statusConvert[8].msg = "Sleep"; statusConvert[8].text = Localization.GetString("grblSleep"); statusConvert[8].state = GrblState.sleep; statusConvert[8].color = Color.Yellow;
            statusConvert[9].msg = "Probe"; statusConvert[9].text = Localization.GetString("grblProbe"); statusConvert[9].state = GrblState.probe; statusConvert[9].color = Color.LightBlue;
            statusConvert[10].msg = "Marlin"; statusConvert[10].text = "Marlin mode"; statusConvert[10].state = GrblState.Marlin; statusConvert[10].color = Color.Yellow;

            settings.Clear();
            coordinates.Clear();
        }

        public static void Clear()
        {
            axisA = false; axisB = false; axisC = false; axisUpdate = false;
            bufferSize = -1;        // readout buffer size
            settings.Clear();       // clear $$ values
            coordinates.Clear();    // clear gcode parameters
            lastErrorNr = 0;
            lastMessage = "";
            Status = GrblState.unknown;
        }

        // store grbl settings https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#grbl-settings
        public static void SetSettings(int id, string value)
        {
            if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float tmp))
            {
                if (settings.ContainsKey(id))
                    settings[id] = tmp;
                else
                    settings.Add(id, tmp);
            }
        }
        public static float GetSetting(int key)
        {
            if (settings.ContainsKey(key))
                return settings[key];
            else
                return -1;
        }

        /***** processGrblFeedbackMessage *****/
        // store gcode parameters https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#---view-gcode-parameters
        public static void SetCoordinates(string id, string value, string info)
        {
            XyzPoint tmp = new XyzPoint();
            string allowed = "PRBG54G55G56G57G58G59G28G30G92TLO";
            if (allowed.Contains(id))
            {
                GetPosition(0, "abc:" + value, ref tmp);   // parse string [PRB:-155.000,-160.000,-28.208:1]
                if (coordinates.ContainsKey(id))
                    coordinates[id] = tmp;
                else
                    coordinates.Add(id, tmp);

                if ((info.Length > 0) && (id == "PRB"))
                {
                    XyzPoint tmp2 = coordinates["PRB"];
                    tmp2.A = info == "1" ? 1 : 0;
                    coordinates["PRB"] = tmp2;
                }
            }
        }

        public static string DisplayCoord(string key)
        {
            if (coordinates.ContainsKey(key))
            {
                if (key == "TLO")
                    return String.Format("                  {0,8:###0.000}", coordinates[key].Z);
                else
                {
                    string coordString = String.Format("{0,8:###0.000} {1,8:###0.000} {2,8:###0.000}", coordinates[key].X, coordinates[key].Y, coordinates[key].Z);
                    if (axisA) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].A);
                    if (axisB) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].B);
                    if (axisC) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].C);
                    return coordString;
                }
            }
            else
                return "no data";
        }
        internal static XyzPoint GetCoord(string key)
        {
            if (coordinates.ContainsKey(key))
                return coordinates[key];
            return new XyzPoint();
        }

        public static bool GetPRBStatus()
        {
            if (coordinates.ContainsKey("PRB"))
            { return coordinates["PRB"].A != 0.0; }
            return false;
        }

        private static void SetMessageString(ref Dictionary<string, string> myDict, string resource)
        {
            string[] tmp = resource.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string s in tmp)
            {
                string[] col = s.Split(new[] { "\",\"" }, StringSplitOptions.None);
                string message = col[col.Length - 1].Trim('"');
                myDict.Add(col[0].Trim('"'), message);
            }
        }

        /// <summary>
        /// parse single gcode line to set parser state
        /// </summary>
        private static bool getTLO = false;
        public static void UpdateParserState(int serNr, string line, ref ParsState myParserState)
        {
            char cmd = '\0';
            string num = "";
            bool comment = false;
            double value;
            bool TLO = getTLO;
            getTLO = false;
            myParserState.changed = false;

            if (!(line.StartsWith("$") || line.StartsWith("("))) //do not parse grbl commands
            {
                try
                {
                    foreach (char c in line)
                    {
                        if (c == ';')
                            break;
                        if (c == '(')
                            comment = true;
                        if (!comment)
                        {
                            if (Char.IsLetter(c))
                            {
                                if (cmd != '\0')
                                {
                                    value = 0;
                                    if (num.Length > 0)
                                    {
                                        try { value = double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo); }
                                        catch { }
                                    }
                                    try { SetParserState(cmd, value, ref myParserState); }
                                    catch { }
                                }
                                cmd = c;
                                num = "";
                            }
                            else if (Char.IsNumber(c) || c == '.' || c == '-')
                            { num += c; }
                        }
                        if (c == ')')
                        { comment = false; }
                    }
                    if (cmd != '\0')
                    {
                        try { SetParserState(cmd, double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo), ref myParserState); }
                        catch { }
                    }
                }
                catch { }
            }
            if (serNr != 1)
                getTLO = TLO;	// restore value;
        }

        /// <summary>
        /// set parser state
        /// </summary>
        private static void SetParserState(char cmd, double value, ref ParsState myParserState)
        {
            switch (Char.ToUpper(cmd))
            {
                case 'G':
                    if (value <= 3)
                    {
                        myParserState.motion = (byte)value;
                        break;
                    }
                    if ((value >= 17) && (value <= 19))
                        myParserState.plane_select = (byte)value;
                    else if ((value == 20) || (value == 21))
                        myParserState.units = (byte)value;
                    else if ((value >= 43) && (value < 44))
                    { myParserState.TLOactive = true; getTLO = true; }
                    else if (value == 49)
                        myParserState.TLOactive = false;
                    else if ((value >= 54) && (value <= 59))
                        myParserState.coord_select = (byte)value;
                    else if ((value == 90) || (value == 91))
                        myParserState.distance = (byte)value;
                    else if ((value == 93) || (value == 94))
                        myParserState.feed_rate = (byte)value;
                    myParserState.changed = true;
                    //                    MessageBox.Show("set parser state "+cmd + "  " + value.ToString()+ "  "+ myParserState.TLOactive.ToString());
                    break;
                case 'M':
                    if ((value <= 2) || (value == 30))
                        myParserState.program_flow = (byte)value;    // M0, M1 pause, M2, M30 stop
                    else if ((value >= 3) && (value <= 5))
                        myParserState.spindle = (byte)value;    // M3, M4 start, M5 stop
                    else if ((value >= 7) && (value <= 9))
                        myParserState.coolant = (byte)value;    // M7, M8 on   M9 coolant off
                    else if (value == 6)
                        myParserState.toolchange = true;
                    myParserState.changed = true;
                    break;
                case 'F':
                    myParserState.FR = value;
                    myParserState.changed = true;
                    break;
                case 'S':
                    myParserState.SS = value;
                    myParserState.changed = true;
                    break;
                case 'T':
                    myParserState.tool = (byte)value;
                    myParserState.changed = true;
                    break;
                case 'Z':
                    if (getTLO)
                        myParserState.tool_length = value;
                    break;
            }
        }
        // check https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#g---view-gcode-parser-state
        public static int[] unknownG = { 41, 64, 81, 83 };
        public static GrblState ParseStatus(string status)    // {idle, run, hold, home, alarm, check, door}
        {
            for (int i = 0; i < statusConvert.Length; i++)
            {
                if (status.StartsWith(statusConvert[i].msg))     // status == statusConvert[i].msg
                    return statusConvert[i].state;
            }
            return GrblState.unknown;
        }
        public static string StatusToText(GrblState state)
        {
            for (int i = 0; i < statusConvert.Length; i++)
            {
                if (state == statusConvert[i].state)
                {
                    if (Properties.Settings.Default.grblTranslateMessage)
                        return statusConvert[i].text;
                    else
                        return statusConvert[i].state.ToString();
                }
            }
            return "Unknown";
        }
        public static Color GrblStateColor(GrblState state)
        {
            for (int i = 0; i < statusConvert.Length; i++)
            {
                if (state == statusConvert[i].state)
                    return statusConvert[i].color;
            }
            return Color.Fuchsia;
        }
        public static bool GetBufferSize(string text)
        {
            if (bufferSize <= 0)    // only get if not done already
            {
                string[] dataValue = text.Split(',');
                int tmp = -1;
                if (dataValue.Length > 1)
                { int.TryParse(dataValue[1], NumberStyles.Any, CultureInfo.InvariantCulture, out tmp); }
                if (tmp > 0)
                    bufferSize = tmp;
                return true;
            }
            return false;
        }
        internal static int GetPosition(int serNr, string text, ref XyzPoint position)
        {
            string[] dataField = text.Split(':');
            if (dataField.Length <= 1)
                return 0;
            string[] dataValue = dataField[1].Split(',');
            //            axisA = false; axisB = false; axisC = false;
            int axisCountLocal = 0;
            if (dataValue.Length == 1)
            {
                Double.TryParse(dataValue[0], NumberStyles.Any, CultureInfo.InvariantCulture, out position.Z);
                position.X = 0;
                position.Y = 0;
            }
            if (dataValue.Length == 2)	// 2021-11-03 just two coordinates
            {
                Double.TryParse(dataValue[0], NumberStyles.Any, CultureInfo.InvariantCulture, out position.X);
                Double.TryParse(dataValue[1], NumberStyles.Any, CultureInfo.InvariantCulture, out position.Y);
                position.Z = 0;
                axisCountLocal = 2;
            }
            if (dataValue.Length > 2)
            {
                Double.TryParse(dataValue[0], NumberStyles.Any, CultureInfo.InvariantCulture, out position.X);
                Double.TryParse(dataValue[1], NumberStyles.Any, CultureInfo.InvariantCulture, out position.Y);
                Double.TryParse(dataValue[2], NumberStyles.Any, CultureInfo.InvariantCulture, out position.Z);
                axisCountLocal = 3;
            }
            if (dataValue.Length > 3)
            {
                Double.TryParse(dataValue[3], NumberStyles.Any, CultureInfo.InvariantCulture, out position.A);
                axisCountLocal++;
                if (serNr == 1) axisA = true;
            }
            if (dataValue.Length > 4)
            {
                Double.TryParse(dataValue[4], NumberStyles.Any, CultureInfo.InvariantCulture, out position.B);
                axisCountLocal++;
                if (serNr == 1) axisB = true;
            }
            if (dataValue.Length > 5)
            {
                Double.TryParse(dataValue[5], NumberStyles.Any, CultureInfo.InvariantCulture, out position.C);
                axisCountLocal++;
                if (serNr == 1) axisC = true;
            }
            if (serNr == 1)
                axisCount = axisCountLocal;
            return axisCountLocal;
            //axisA = true; axisB = true; axisC = true;     // for test only
        }

        internal static void GetOtherFeedbackMessage(string[] dataField)
        {
            string tmp = string.Join(":", dataField);
            if (messages.ContainsKey(dataField[0]))
                messages[dataField[0]] = tmp;
            else
                messages.Add(dataField[0], tmp);
        }



        public static string GetSettingDescription(string msgNr)
        {
            string msg = " no information found '" + msgNr + "'";
            try { msg = Grbl.messageSettingCodes[msgNr]; }
            catch { }
            return msg;
        }
        public static string GetMsgNr(string msg)
        {
            string[] tmp = msg.Split(':');
            if (tmp.Length > 1)
            { return tmp[tmp.Length - 1].Trim(); }      // 2021-05-01 change from [1]
            return "";
        }
        public static string GetErrorDescription(string rxString)
        {   //string[] tmp = rxString.Split(':');
            string msgNr = GetMsgNr(rxString);
            if (msgNr.Length >= 1)
            {
                string msg = " no information found for error-nr. '" + msgNr + "'";
                try
                {
                    if ((messageErrorCodes != null) && messageErrorCodes.ContainsKey(msgNr))
                    {
                        msg = Grbl.messageErrorCodes[msgNr];
                        //int errnr = Convert.ToInt16(tmp[1].Trim());
                        lastErrorNr = 0;
                        lastMessage = rxString + " " + msg;
                        if (!short.TryParse(msgNr, NumberStyles.Any, CultureInfo.InvariantCulture, out lastErrorNr))
                            return msg;
                        if ((lastErrorNr >= 32) && (lastErrorNr <= 34))
                            msg += "\r\n\r\nPossible reason: scale down of GCode with G2/3 commands.\r\nSolution: use more decimal places.";
                    }
                }
                catch { }
                return msg;
            }
            else
            {
                return " no info ";
            }
        }

        public static string GetAlarmDescription(string rxString)
        {
            string[] tmp = rxString.Split(':');
            if (tmp.Length <= 1) return "no info " + tmp;

            string msg = " no information found for alarm-nr. '" + tmp[1] + "'";
            try
            {
                if ((messageAlarmCodes != null) && messageAlarmCodes.ContainsKey(tmp[1].Trim()))
                    msg = Grbl.messageAlarmCodes[tmp[1].Trim()];
            }
            catch { }
            return msg;
        }
        public static string GetRealtimeDescription(int id)
        {
            switch (id)
            {
                case 24:
                    return "Soft-Reset";
                case '?':
                    return "Status Report Query";
                case '~':
                    return "Cycle Start / Resume";
                case '!':
                    return "Feed Hold";
                case 132:
                    return "Safety Door";
                case 133:
                    return "Jog Cancel";
                case 144:
                    return "Set 100% of programmed feed rate.";
                case 145:
                    return "Feed Rate increase 10%";
                case 146:
                    return "Feed Rate decrease 10%";
                case 147:
                    return "Feed Rate increase 1%";
                case 148:
                    return "Feed Rate decrease 1%";
                case 149:
                    return "Set to 100% full rapid rate.";
                case 150:
                    return "Set to 50% of rapid rate.";
                case 151:
                    return "Set to 25% of rapid rate.";
                case 153:
                    return "Set 100% of programmed spindle speed";
                case 154:
                    return "Spindle Speed increase 10%";
                case 155:
                    return "Spindle Speed decrease 10%";
                case 156:
                    return "Spindle Speed increase 1%";
                case 157:
                    return "Spindle Speed decrease 1%";
                case 158:
                    return "Toggle Spindle Stop";
                case 160:
                    return "Toggle Flood Coolant";
                case 161:
                    return "Toggle Mist Coolant";
                default:
                    return "unknown setting " + id.ToString();
            }
        }
    }

    internal enum GrblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, reset, unknown, Marlin, notConnected };
    internal enum GrblStreaming { ok, error, reset, finish, pause, waitidle, toolchange, stop, lasermode, waitstop, setting };

    internal struct StatConvert
    {
        public string msg;
        public string text;
        internal GrblState state;
        public Color color;
    };

    internal class ParsState
    {
        public bool changed = false;
        public int motion = 0;           // {G0,G1,G2,G3,G38.2,G80} 
        public int feed_rate = 94;       // {G93,G94} 
        public int units = 21;           // {G20,G21} 
        public int distance = 90;        // {G90,G91} 
                                         // uint8_t distance_arc; // {G91.1} NOTE: Don't track. Only default supported. 
        public int plane_select = 17;    // {G17,G18,G19} 
                                         // uint8_t cutter_comp;  // {G40} NOTE: Don't track. Only default supported. 
        public double tool_length = 0;       // {G43.1,G49} 
        public int coord_select = 54;    // {G54,G55,G56,G57,G58,G59} 
                                         // uint8_t control;      // {G61} NOTE: Don't track. Only default supported. 
        public int program_flow = 0;    // {M0,M1,M2,M30} 
        public int coolant = 9;         // {M7,M8,M9} 
        public int spindle = 5;         // {M3,M4,M5} 
        public bool toolchange = false;
        public int tool = 0;            // tool number
        public double FR = 0;           // feedrate
        public double SS = 0;           // spindle speed
        public bool TLOactive = false;// Tool length offset

        public void Reset()
        {
            motion = 0; plane_select = 17; units = 21;
            coord_select = 54; distance = 90; feed_rate = 94;
            program_flow = 0; coolant = 9; spindle = 5;
            toolchange = false; tool = 0; FR = 0; SS = 0;
            TLOactive = false; tool_length = 0;
            changed = false;
        }

    };

    internal class ModState
    {
        public string Bf, Ln, FS, Pn, Ov, A;
        /*   public ModState(string bf, string ln, string fs, string pn, string ov, string a)
           { Bf = bf; Ln = ln; FS = fs; Pn = pn; Ov = ov; A = a; }*/
        public ModState()
        { Clear(); }
        public void Clear()
        { Bf = ""; Ln = ""; FS = ""; Pn = ""; Ov = ""; A = ""; }
    };

    public class StreamEventArgs : EventArgs
    {
        private readonly int codeFinish;
        private readonly int buffFinish;
        private readonly int codeLineSent;
        private readonly int codeLineConfirmed;
        private readonly GrblStreaming status;
        internal StreamEventArgs(int c1, int c2, int a1, int a2, GrblStreaming stat)
        {
            codeLineSent = c1;
            codeLineConfirmed = c2;
            codeFinish = a1;
            buffFinish = a2;
            status = stat;
        }
        public int CodeLineSent
        { get { return codeLineSent; } }
        public int CodeLineConfirmed
        { get { return codeLineConfirmed; } }
        public int CodeProgress
        { get { return codeFinish; } }
        public int BuffProgress
        { get { return buffFinish; } }
        internal GrblStreaming Status
        { get { return status; } }
    }

    public class PosEventArgs : EventArgs
    {
        private XyzPoint posWorld, posMachine;
        private readonly GrblState status;
        private readonly ModState statMsg;
        private readonly ParsState lastCmd;
        private readonly string raw;
        internal PosEventArgs(XyzPoint world, XyzPoint machine, GrblState stat, ModState msg, ParsState last, string sraw)
        {
            posWorld = world;
            posMachine = machine;
            status = stat;
            statMsg = msg;
            lastCmd = last;
            raw = sraw;
        }
        internal XyzPoint PosWorld
        { get { return posWorld; } }
        internal XyzPoint PosMachine
        { get { return posMachine; } }
        internal GrblState Status
        { get { return status; } }
        internal ModState StatMsg
        { get { return statMsg; } }
        internal ParsState ParserState
        { get { return lastCmd; } }
        public string Raw
        { get { return raw; } }
    }
}
