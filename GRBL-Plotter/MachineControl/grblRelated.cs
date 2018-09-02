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
/*
 * 2016-12-31   Add GRBL 1.1 information
 * 2018-04-07   reorder
*/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace GRBL_Plotter
{
    public static class grbl
    {       // need to have global access to this data?
//        public static xyzPoint posWorld   = new xyzPoint(0, 0, 0);
//        public static xyzPoint posMachine = new xyzPoint(0, 0, 0);
//        public static pState parserState;
//        public static bool isVers0 = true;

        public static Dictionary<string, string> messageAlarmCodes = new Dictionary<string, string>();
        public static Dictionary<string, string> messageErrorCodes = new Dictionary<string, string>();
        public static Dictionary<string, string> messageSettingCodes = new Dictionary<string, string>();
        private static sConvert[] statusConvert = new sConvert[12];

        static grbl()   // initialize lists
        {
            setMessageString(ref messageAlarmCodes, Properties.Resources.alarm_codes_en_US);
            setMessageString(ref messageErrorCodes, Properties.Resources.error_codes_en_US);
            setMessageString(ref messageSettingCodes, Properties.Resources.setting_codes_en_US);
            string fourthAxis = Properties.Settings.Default.ctrl4thName;
            messageSettingCodes.Add("113", fourthAxis + " -axis maximum rate, mm/min");
            messageSettingCodes.Add("123", fourthAxis + " -axis acceleration, mm/sec^2");
            messageSettingCodes.Add("133", fourthAxis + " -axis maximum travel, millimeters");

            //    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };
            statusConvert[0].msg = "Idle";  statusConvert[0].state = grblState.idle; statusConvert[0].color = Color.Lime;
            statusConvert[1].msg = "Run";   statusConvert[1].state = grblState.run;  statusConvert[1].color = Color.Yellow;
            statusConvert[2].msg = "Hold";  statusConvert[2].state = grblState.hold; statusConvert[2].color = Color.YellowGreen;
            statusConvert[3].msg = "Jog";   statusConvert[3].state = grblState.jog;  statusConvert[3].color = Color.LightGreen;
            statusConvert[4].msg = "Alarm"; statusConvert[4].state = grblState.alarm;statusConvert[4].color = Color.Red;
            statusConvert[5].msg = "Door";  statusConvert[5].state = grblState.door; statusConvert[5].color = Color.Orange;
            statusConvert[6].msg = "Check"; statusConvert[6].state = grblState.check;statusConvert[6].color = Color.Orange;
            statusConvert[7].msg = "Home";  statusConvert[7].state = grblState.home; statusConvert[7].color = Color.Magenta;
            statusConvert[8].msg = "Sleep"; statusConvert[8].state = grblState.sleep;statusConvert[8].color = Color.Yellow;
            statusConvert[9].msg = "Probe"; statusConvert[9].state = grblState.probe;statusConvert[9].color = Color.LightBlue;
        }

        private static void setMessageString(ref Dictionary<string, string> myDict, string resource)
        {   string[] tmp = resource.Split('\n');
            foreach (string s in tmp)
            {   string[] col = s.Split(',');
                string message = col[col.Length - 1].Trim('"');
                myDict.Add(col[0].Trim('"'), message);
            }
        }

        /// <summary>
        /// parse single gcode line to set parser state
        /// </summary>
        public static void updateParserState(string line, ref pState myParserState)
        {
            char cmd = '\0';
            string num = "";
            bool comment = false;
            double value = 0;

            if (!(line.StartsWith("$") || line.StartsWith("("))) //do not parse grbl commands
            {   try
                {   foreach (char c in line)
                    {   if (c == ';')
                            break;
                        if (c == '(')
                            comment = true;
                        if (!comment)
                        {   if (Char.IsLetter(c))
                            {   if (cmd != '\0')
                                {   value = 0;
                                    if (num.Length > 0)
                                    {   try { value = double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo); }
                                        catch { }
                                    }
                                    try { setParserState(cmd, value, ref myParserState); }
                                    catch { }
                                }
                                cmd = c;
                                num = "";
                            }
                            else if (Char.IsNumber(c) || c == '.' || c == '-')
                            {   num += c;  }
                        }
                        if (c == ')')
                        { comment = false; }
                    }
                    if (cmd != '\0')
                    {   try { setParserState(cmd, double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo), ref myParserState); }
                        catch { }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// set parser state
        /// </summary>
        private static void setParserState(char cmd, double value, ref pState myParserState)
        {
            myParserState.changed = false;
            switch (Char.ToUpper(cmd))
            {   case 'G':
                    if (value <= 3)
                    {   myParserState.motion = (byte)value;
                        break;
                    }
                    if ((value >= 17) && (value <= 19))
                        myParserState.plane_select = (byte)value;
                    if ((value == 20) || (value == 21))
                        myParserState.units = (byte)value;
                    if ((value >= 54) && (value <= 59))
                        myParserState.coord_select = (byte)value;
                    if ((value == 90) || (value == 91))
                        myParserState.distance = (byte)value;
                    if ((value == 93) || (value == 94))
                        myParserState.feed_rate = (byte)value;
                    myParserState.changed = true;
                    break;
                case 'M':
                    if ((value <= 2) || (value == 30))
                        myParserState.program_flow = (byte)value;    // M0, M1 pause, M2, M30 stop
                    if ((value >= 3) && (value <= 5))
                        myParserState.spindle = (byte)value;    // M3, M4 start, M5 stop
                    if ((value >= 7) && (value <= 9))
                        myParserState.coolant = (byte)value;    // M7, M8 on   M9 coolant off
                    if (value == 6)
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
            }
        }
        // check https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#g---view-gcode-parser-state
        public static int[] unknownG = { 41, 64, 81, 83 };
        public static grblState parseStatus(string status)    // {idle, run, hold, home, alarm, check, door}
        {   for (int i = 0; i < statusConvert.Length; i++)
            {   if (status == statusConvert[i].msg)
                    return statusConvert[i].state;
            }
            return grblState.unknown;
        }
        public static string statusToText(grblState state)
        {
            for (int i = 0; i < statusConvert.Length; i++)
            {
                if (state == statusConvert[i].state)
                    return statusConvert[i].msg;
            }
            return "Unknown";
        }
        public static Color grblStateColor(grblState state)
        {
            for (int i = 0; i < statusConvert.Length; i++)
            {
                if (state == statusConvert[i].state)
                    return statusConvert[i].color;
            }
            return Color.Fuchsia;
        }
        public static void getPosition(string text, ref xyzPoint position)
        {
            string[] dataField = text.Split(':');
            string[] dataValue = dataField[1].Split(',');
            if (dataValue.Length > 2)
            {
                Double.TryParse(dataValue[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.X);
                Double.TryParse(dataValue[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Y);
                Double.TryParse(dataValue[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Z);
            }
            if (dataValue.Length > 3)
                Double.TryParse(dataValue[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.A);
        }

        public static string getSetting(string msgNr)
        {   string msg = " no information found '" + msgNr + "'";
            try { msg = grbl.messageSettingCodes[msgNr]; }
            catch { }
            return msg;
        }
        public static string getError(string rxString)
        {   string[] tmp = rxString.Split(':');
            string msg = " no information found '" + tmp[1] + "'";
            try {   msg = grbl.messageErrorCodes[tmp[1].Trim()];
                    if (Convert.ToInt16(tmp[1].Trim()) >= 32)
                        msg += "\r\n\r\nPossible reason: scale down of GCode with G2/3 commands.\r\nSolution: use more decimal places.";
                }
            catch { }
            return msg;
        }
        public static string getAlarm(string rxString)
        {   string[] tmp = rxString.Split(':');
            string msg = " no information found '" + tmp[1] + "'";
            try {    msg = grbl.messageAlarmCodes[tmp[1].Trim()];
                }
            catch { }
            return msg;
        }
        public static string getRealtime(int id)
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
                    return "Increase 10%";
                case 146:
                    return "Decrease 10%";
                case 147:
                    return "Increase 1%";
                case 148:
                    return "Decrease 1%";
                case 149:
                    return "Set to 100% full rapid rate.";
                case 150:
                    return "Set to 50% of rapid rate.";
                case 151:
                    return "Set to 25% of rapid rate.";
                case 153:
                    return "Set 100% of programmed spindle speed";
                case 154:
                    return "Increase 10%";
                case 155:
                    return "Decrease 10%";
                case 156:
                    return "Increase 1%";
                case 157:
                    return "Decrease 1%";
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

    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };
    public enum grblStreaming { ok, error, reset, finish, pause, waitidle, toolchange, stop, lasermode };

    public struct sConvert
    {
        public string msg;
        public grblState state;
        public Color color;
    };

    public class pState
    {
        public bool changed=true;
        public int motion=0;           // {G0,G1,G2,G3,G38.2,G80} 
        public int feed_rate=94;       // {G93,G94} 
        public int units=21;           // {G20,G21} 
        public int distance=90;        // {G90,G91} 
                                    // uint8_t distance_arc; // {G91.1} NOTE: Don't track. Only default supported. 
        public int plane_select=17;    // {G17,G18,G19} 
                                    // uint8_t cutter_comp;  // {G40} NOTE: Don't track. Only default supported. 
                                    //        int tool_length;     // {G43.1,G49} 
        public int coord_select=54;    // {G54,G55,G56,G57,G58,G59} 
                                    // uint8_t control;      // {G61} NOTE: Don't track. Only default supported. 
        public int program_flow=0;    // {M0,M1,M2,M30} 
        public int coolant=9;         // {M7,M8,M9} 
        public int spindle=5;         // {M3,M4,M5} 
        public bool toolchange=false;
        public int tool=0;            // tool number
        public double FR=0;           // feedrate
        public double SS=0;           // spindle speed

        public void reset()
        {
            motion = 0; plane_select = 17; units = 21;
            coord_select = 54; distance = 90; feed_rate = 94;
            program_flow = 0; coolant = 9; spindle = 5;
            toolchange = false; tool = 0; FR = 0; SS = 0;
            changed = true;
        }

    };

    public class mState
    {   public string Bf, Ln, FS, Pn, Ov, A;
        public mState(string bf, string ln, string fs, string pn, string ov, string a)
        { Bf = bf; Ln = ln; FS = fs; Pn = pn; Ov = ov; A = a; }
        public mState()
        { Clear(); }
        public void Clear()
        {   Bf = ""; Ln = ""; FS = ""; Pn = ""; Ov = ""; A = ""; }
    };

    public class StreamEventArgs : EventArgs
    {
        private float codeFinish, buffFinish;
        private int codeLine;
        private grblStreaming status;
        public StreamEventArgs(int c1, float a1, float a2, grblStreaming stat)
        {
            codeLine = c1;
            codeFinish = a1;
            buffFinish = a2;
            status = stat;
        }
        public int CodeLine
        { get { return codeLine; } }
        public float CodeProgress
        { get { return codeFinish; } }
        public float BuffProgress
        { get { return buffFinish; } }
        public grblStreaming Status
        { get { return status; } }
    }
    public class PosEventArgs : EventArgs
    {
        private xyzPoint posWorld, posMachine;
        private grblState status;
        private mState statMsg;
        private pState lastCmd;
        private string raw;
        public PosEventArgs(xyzPoint world, xyzPoint machine, grblState stat, mState msg, pState last, string sraw)
        {
            posWorld = world;
            posMachine = machine;
            status = stat;
            statMsg = msg;
            lastCmd = last;
            raw = sraw;
        }
        public xyzPoint PosWorld
        { get { return posWorld; } }
        public xyzPoint PosMachine
        { get { return posMachine; } }
        public grblState Status
        { get { return status; } }
        public mState StatMsg
        { get { return statMsg; } }
        public pState parserState
        { get { return lastCmd; } }
        public string Raw
        { get { return raw; } }
    }
}
