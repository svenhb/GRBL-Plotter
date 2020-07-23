/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
 * 2018-01-01   edit parseStatus to identify also Hold:0
 * 2019-05-10   move _serial_form.isGrblVers0 to here grbl.isVersion_0
 * https://github.com/fra589/grbl-Mega-5X
 * 2019-08-13   add PRB, TLO status
 * 2020-01-04   add "errorBecauseOfBadCode"
 * 2020-01-13   localization of grblStatus (Idle, run, hold...)
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public static class grbl
    {       // need to have global access to this data?
        public static xyzPoint posWCO     = new xyzPoint(0, 0, 0);
        public static xyzPoint posWork    = new xyzPoint(0, 0, 0);
        public static xyzPoint posMachine = new xyzPoint(0, 0, 0);
        public static bool posChanged = true;
        public static bool wcoChanged = true;

        public static bool isVersion_0 = true;  // note if grbl version <=0.9 or >=1.1

        public static int axisCount = 0;
        public static bool axisA = false;       // axis A available?
        public static bool axisB = false;       // axis B available?
        public static bool axisC = false;       // axis C available?
        public static bool axisUpdate = false;  // update of GUI needed
        public static int RX_BUFFER_SIZE = 127; // grbl buffer size inside Arduino
        public static int pollInterval = 200;
        public static int bufferSize = -1;
        public static string lastMessage = "";

        public static bool grblSimulate = false;
        private static Dictionary<int, float> settings = new Dictionary<int, float>();    // keep $$-settings
        private static Dictionary<string, xyzPoint> coordinates = new Dictionary<string, xyzPoint>();    // keep []-settings

        private static xyPoint _posMarker = new xyPoint(0, 0);
        private static double _posMarkerAngle = 0;
        private static xyPoint _posMarkerOld = new xyPoint(0, 0);
        public static xyPoint posMarker
        {   get
            {   return _posMarker;  }
            set
            {   _posMarkerOld = _posMarker;
                _posMarker = value;
            }
        }
        public static xyPoint posMarkerOld
        {   get
            {   return _posMarkerOld; }
            set
            {   _posMarkerOld = value; }
        }
        public static double posMarkerAngle
        {   get
            { return _posMarkerAngle; }
            set
            { _posMarkerAngle = value; }
        }

        // Trace, Debug, Info, Warn, Error, Fatal
        //     private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // private mState machineState = new mState();     // Keep info about Bf, Ln, FS, Pn, Ov, A;
        //  private pState mParserState = new pState();     // keep info about last M and G settings
        //        public static pState parserState;
        //        public static bool isVers0 = true;
        //        public List<string> GRBLSettings = new List<string>();  // keep $$ settings

        public static double resolution = 0.000001;

        public static Dictionary<string, string> messageAlarmCodes = new Dictionary<string, string>();
        public static Dictionary<string, string> messageErrorCodes = new Dictionary<string, string>();
        public static Dictionary<string, string> messageSettingCodes = new Dictionary<string, string>();
        private static sConvert[] statusConvert = new sConvert[10];

        public static void init()   // initialize lists
        {
            setMessageString(ref messageAlarmCodes, Properties.Resources.alarm_codes_en_US);
            setMessageString(ref messageErrorCodes, Properties.Resources.error_codes_en_US);
            setMessageString(ref messageSettingCodes, Properties.Resources.setting_codes_en_US);

            //    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };
            statusConvert[0].msg = "Idle";  statusConvert[0].text = Localization.getString("grblIdle");  statusConvert[0].state = grblState.idle; statusConvert[0].color = Color.Lime;
            statusConvert[1].msg = "Run";   statusConvert[1].text = Localization.getString("grblRun");   statusConvert[1].state = grblState.run;  statusConvert[1].color = Color.Yellow;
            statusConvert[2].msg = "Hold";  statusConvert[2].text = Localization.getString("grblHold");  statusConvert[2].state = grblState.hold; statusConvert[2].color = Color.YellowGreen;
            statusConvert[3].msg = "Jog";   statusConvert[3].text = Localization.getString("grblJog");   statusConvert[3].state = grblState.jog;  statusConvert[3].color = Color.LightGreen;
            statusConvert[4].msg = "Alarm"; statusConvert[4].text = Localization.getString("grblAlarm"); statusConvert[4].state = grblState.alarm;statusConvert[4].color = Color.Red;
            statusConvert[5].msg = "Door";  statusConvert[5].text = Localization.getString("grblDoor");  statusConvert[5].state = grblState.door; statusConvert[5].color = Color.Orange;
            statusConvert[6].msg = "Check"; statusConvert[6].text = Localization.getString("grblCheck"); statusConvert[6].state = grblState.check;statusConvert[6].color = Color.Orange;
            statusConvert[7].msg = "Home";  statusConvert[7].text = Localization.getString("grblHome");  statusConvert[7].state = grblState.home; statusConvert[7].color = Color.Magenta;
            statusConvert[8].msg = "Sleep"; statusConvert[8].text = Localization.getString("grblSleep"); statusConvert[8].state = grblState.sleep;statusConvert[8].color = Color.Yellow;
            statusConvert[9].msg = "Probe"; statusConvert[9].text = Localization.getString("grblProbe"); statusConvert[9].state = grblState.probe;statusConvert[9].color = Color.LightBlue;

            settings.Clear();
            coordinates.Clear();
        }

        public static void Clear()
        {
            axisA = false; axisB = false; axisC = false; axisUpdate = false;
            bufferSize = -1;        // readout buffer size
            settings.Clear();       // clear $$ values
            coordinates.Clear();    // clear gcode parameters
        }

        // store grbl settings https://github.com/gnea/grbl/wiki/Grbl-v1.1-Configuration#grbl-settings
        public static void setSettings(int id, string value)
        {   float tmp = 0;
            if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmp))
            {   if (settings.ContainsKey(id))
                    settings[id] = tmp;
                else
                    settings.Add(id, tmp);
            }
        }
        public static float getSetting(int key)
        {   if (settings.ContainsKey(key))
                return settings[key];
            else
                return -1;
        }

        // store gcode parameters https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#---view-gcode-parameters
        public static void setCoordinates(string id, string value, string info)
        {   xyzPoint tmp = new xyzPoint();
            string allowed = "PRBG54G55G56G57G58G59G28G30G92TLO";
            if (allowed.Contains(id))
            {   getPosition("abc:" + value, ref tmp);   // parse string [PRB:-155.000,-160.000,-28.208:1]
                if (coordinates.ContainsKey(id))
                    coordinates[id] = tmp;
                else
                    coordinates.Add(id, tmp);

                if ((info.Length > 0) && (id == "PRB"))
                {   xyzPoint tmp2 = new xyzPoint();
                    tmp2 = coordinates["PRB"];
                    tmp2.A = info == "1" ? 1 : 0;
                    coordinates["PRB"] = tmp2;
                }
            }
        }

        public static string displayCoord(string key)
        {   if (coordinates.ContainsKey(key))
            {   if (key == "TLO")
                    return String.Format("                  {0,8:###0.000}", coordinates[key].Z);
                else
                {   string coordString = String.Format("{0,8:###0.000} {1,8:###0.000} {2,8:###0.000}", coordinates[key].X, coordinates[key].Y, coordinates[key].Z);
                    if (axisA) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].A);
                    if (axisB) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].B);
                    if (axisC) coordString = String.Format("{0} {1,8:###0.000}", coordString, coordinates[key].C);
                    return coordString;
                }
            }
            else
                return "no data";
        }
        public static xyzPoint getCoord(string key)
        {   if (coordinates.ContainsKey(key))
                return coordinates[key];
            return new xyzPoint();
        }

        public static bool getPRBStatus()
        {   if (coordinates.ContainsKey("PRB"))
            {   return (coordinates["PRB"].A==0.0)? false:true;    }
            return false;
        }

        private static void setMessageString(ref Dictionary<string, string> myDict, string resource)
        {   string[] tmp = resource.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string s in tmp)
            {   string[] col = s.Split(',');
                string message = col[col.Length - 1].Trim('"');
                myDict.Add(col[0].Trim('"'), message);
            }
        }

        /// <summary>
        /// parse single gcode line to set parser state
        /// </summary>
        private static bool getTLO = false;
        public static void updateParserState(string line, ref pState myParserState)
        {
            char cmd = '\0';
            string num = "";
            bool comment = false;
            double value = 0;
            getTLO = false;
            myParserState.changed = false;

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
//            myParserState.changed = false;
            switch (Char.ToUpper(cmd))
            {   case 'G':
                    if (value <= 3)
                    {   myParserState.motion = (byte)value;
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
        public static grblState parseStatus(string status)    // {idle, run, hold, home, alarm, check, door}
        {   for (int i = 0; i < statusConvert.Length; i++)
            {   if (status.StartsWith(statusConvert[i].msg))     // status == statusConvert[i].msg
                    return statusConvert[i].state;
            }
            return grblState.unknown;
        }
        public static string statusToText(grblState state)
        {
            for (int i = 0; i < statusConvert.Length; i++)
            {
                if (state == statusConvert[i].state)
                { if (Properties.Settings.Default.grblTranslateMessage)
                        return statusConvert[i].text;
                    else
                        return statusConvert[i].state.ToString();
                }
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
        public static bool getBufferSize(string text)
        {   if (bufferSize <= 0)    // only get if not done already
            {   string[] dataValue = text.Split(',');
                if (dataValue.Length > 1)
                { int.TryParse(dataValue[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out bufferSize); }
                return true;
            }
            return false;
        }
        public static void getPosition(string text, ref xyzPoint position)
        {
            string[] dataField = text.Split(':');
            string[] dataValue = dataField[1].Split(',');
            //            axisA = false; axisB = false; axisC = false;
            axisCount = 0;
            if (dataValue.Length == 1)
            {
                Double.TryParse(dataValue[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Z);
                position.X = 0;
                position.Y = 0;
            }
            if (dataValue.Length > 2)
            {
                Double.TryParse(dataValue[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.X);
                Double.TryParse(dataValue[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Y);
                Double.TryParse(dataValue[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.Z);
                axisCount = 3;
            }
            if (dataValue.Length > 3)
            {   Double.TryParse(dataValue[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.A);
                axisA = true; axisCount++;
            }
            if (dataValue.Length > 4)
            {   Double.TryParse(dataValue[4], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.B);
                axisB = true; axisCount++;
            }
            if (dataValue.Length > 5)
            {   Double.TryParse(dataValue[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out position.C);
                axisC = true; axisCount++;
            }
            //axisA = true; axisB = true; axisC = true;     // for test only
        }

        public static string getSettingDescription(string msgNr)
        {   string msg = " no information found '" + msgNr + "'";
            try { msg = grbl.messageSettingCodes[msgNr]; }
            catch { }
            return msg;
        }
        public static string getErrorDescription(string rxString)
        {   string[] tmp = rxString.Split(':');
            string msg = " no information found for error-nr. '" + tmp[1] + "'";
            try {   if (messageErrorCodes.ContainsKey(tmp[1].Trim()))
                    {   msg = grbl.messageErrorCodes[tmp[1].Trim()];
                        int errnr = Convert.ToInt16(tmp[1].Trim());
                        if ((errnr >= 32) && (errnr <= 34))
                            msg += "\r\n\r\nPossible reason: scale down of GCode with G2/3 commands.\r\nSolution: use more decimal places.";
                    }
                }
            catch { }
            return msg;
        }
        public static bool errorBecauseOfBadCode(string rxString)
        {   string[] tmp = rxString.Split(':');
            try {   int[] notByGCode = {3,5,6,7,8,9,10,12,13,14,15,16,17,18,19};
                    int errnr = Convert.ToInt16(tmp[1].Trim());
                    if (Array.Exists(notByGCode, element => element == errnr))
                        return false; 
                    else
                        return true;
                }
            catch { }
            return true;
        }
        public static string getAlarmDescription(string rxString)
        {   string[] tmp = rxString.Split(':');
            string msg = " no information found for alarm-nr. '" + tmp[1] + "'";
            try {   if (messageAlarmCodes.ContainsKey(tmp[1].Trim()))
                        msg = grbl.messageAlarmCodes[tmp[1].Trim()];
                }
            catch { }
            return msg;
        }
        public static string getRealtimeDescription(int id)
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

    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };
    public enum grblStreaming { ok, error, reset, finish, pause, waitidle, toolchange, stop, lasermode };

    public struct sConvert
    {
        public string msg;
        public string text;
        public grblState state;
        public Color color;
    };

    public class pState
    {
        public bool changed=false;
        public int motion=0;           // {G0,G1,G2,G3,G38.2,G80} 
        public int feed_rate=94;       // {G93,G94} 
        public int units=21;           // {G20,G21} 
        public int distance=90;        // {G90,G91} 
                                        // uint8_t distance_arc; // {G91.1} NOTE: Don't track. Only default supported. 
        public int plane_select=17;    // {G17,G18,G19} 
                                        // uint8_t cutter_comp;  // {G40} NOTE: Don't track. Only default supported. 
        public double tool_length=0;       // {G43.1,G49} 
        public int coord_select=54;    // {G54,G55,G56,G57,G58,G59} 
                                        // uint8_t control;      // {G61} NOTE: Don't track. Only default supported. 
        public int program_flow=0;    // {M0,M1,M2,M30} 
        public int coolant=9;         // {M7,M8,M9} 
        public int spindle=5;         // {M3,M4,M5} 
        public bool toolchange=false;
        public int tool=0;            // tool number
        public double FR=0;           // feedrate
        public double SS=0;           // spindle speed
        public bool TLOactive = false;// Tool length offset

        public void reset()
        {
            motion = 0; plane_select = 17; units = 21;
            coord_select = 54; distance = 90; feed_rate = 94;
            program_flow = 0; coolant = 9; spindle = 5;
            toolchange = false; tool = 0; FR = 0; SS = 0;
            TLOactive = false; tool_length = 0;
            changed = false;
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
        private int codeLineSent;
        private int codeLineConfirmed;
        private grblStreaming status;
        public StreamEventArgs(int c1, int c2, float a1, float a2, grblStreaming stat)
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
