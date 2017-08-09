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
 * 2016-12-31   Add GRBL 1.1 information
*/

using System;
using System.Drawing;

namespace GRBL_Plotter
{
    public struct strStruct
    {
        public string Bf,Ln,FS,Pn,Ov,A;
        public strStruct(string bf, string ln, string fs, string pn, string ov, string a)
        { Bf = bf; Ln = ln; FS = fs; Pn = pn; Ov = ov; A=a; }
    };

    public enum grblState { idle, run, hold, jog, alarm, door, check, home, sleep, probe, unknown };
    public enum grblStreaming { ok, error, reset, finish, pause, waitidle, toolchange, stop, lasermode };
    public static class grbl
    {   // http://www.shapeoko.com/wiki/index.php/G-Code#G-code_Not_supported_by_Grbl
        public static int[] unknownG = { 41, 64, 81, 83 };
        public static grblState parseStatus(string status)    // {idle, run, hold, home, alarm, check, door}
        {
            if (status.IndexOf("Idle") >= 0) { return grblState.idle; }
            if (status.IndexOf("Run") >= 0) { return grblState.run; }
            if (status.IndexOf("Hold") >= 0) { return grblState.hold; }
            if (status.IndexOf("Jog") >= 0) { return grblState.jog; }
            if (status.IndexOf("Alarm") >= 0) { return grblState.alarm; }
            if (status.IndexOf("Door") >= 0) { return grblState.door; }
            if (status.IndexOf("Check") >= 0) { return grblState.check; }
            if (status.IndexOf("Home") >= 0) { return grblState.home; }
            if (status.IndexOf("Sleep") >= 0) { return grblState.sleep; }
            return grblState.unknown;
        }
        public static string statusToText(grblState state)
        {
            switch (state)
            {
                case grblState.idle: return "Idle";
                case grblState.run: return "Run";
                case grblState.hold: return "Hold";
                case grblState.jog: return "Jogging";
                case grblState.alarm: return "Alarm";
                case grblState.door: return "Door";
                case grblState.check: return "Check code";
                case grblState.home: return "Homing";
                case grblState.sleep: return "Sleep";
                case grblState.probe: return "Probing";
                case grblState.unknown:
                default:
                    return "Unknown";
            }
        }
        public static Color grblStateColor(grblState state)
        {
            switch (state)
            {
                case grblState.run:
                    return Color.Yellow;
                case grblState.hold:
                    return Color.YellowGreen;
                case grblState.home:
                    return Color.Magenta;
                case grblState.check:
                    return Color.Orange;
                case grblState.idle:
                    return Color.Lime;
                case grblState.probe:
                    return Color.LightBlue;
                case grblState.unknown:
                    return Color.Red;
                case grblState.alarm:
                case grblState.door:
                default:
                    return Color.Fuchsia;
            }
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
        }

        public static string getError(string rxString)
        {
            string[] tmp = rxString.Split(':');
            int id = -1;
            if (tmp.Length > 0)
                if (!int.TryParse(tmp[1], out id))
                    return tmp[1];   id = Convert.ToInt16(tmp[1]);
            switch (id)
            {
                case 1:
                    return "G - code words consist of a letter and a value. Letter was not found.";
                case 2:
                    return "Numeric value format is not valid or missing an expected value.";
                case 3:
                    return "Grbl '$' system command was not recognized or supported.";
                case 4:
                    return "Negative value received for an expected positive value.";
                case 5:
                    return "Homing cycle is not enabled via settings.";
                case 6:
                    return "Minimum step pulse time must be greater than 3usec";
                case 7:
                    return "EEPROM read failed.Reset and restored to default values.";
                case 8:
                    return "Grbl '$' command cannot be used unless Grbl is IDLE.Ensures smooth operation during a job.";
                case 9:
                    return "G - code locked out during alarm or jog state";
                case 10:
                    return "Soft limits cannot be enabled without homing also enabled.";
                case 11:
                    return "Max characters per line exceeded.Line was not processed and executed.";
                case 12:
                    return "(Compile Option) Grbl '$' setting value exceeds the maximum step rate supported.";
                case 13:
                    return "Safety door detected as opened and door state initiated.";
                case 14:
                    return "(Grbl - Mega Only) Build info or startup line exceeded EEPROM line length limit.";
                case 15:
                    return "Jog target exceeds machine travel.Command ignored.";
                case 16:
                    return "Jog command with no '=' or contains prohibited g - code.";
                case 20:
                    return "Unsupported or invalid g - code command found in block.";
                case 21:
                    return "More than one g - code command from same modal group found in block.";
                case 22:
                    return "Feed rate has not yet been set or is undefined.";
                case 23:
                    return "G - code command in block requires an integer value.";
                case 24:
                    return "Two G - code commands that both require the use of the XYZ axis words were detected in the block.";
                case 25:
                    return "A G - code word was repeated in the block.";
                case 26:
                    return "A G - code command implicitly or explicitly requires XYZ axis words in the block, but none were detected.";
                case 27:
                    return "N line number value is not within the valid range of 1 - 9, 999, 999.";
                case 28:
                    return "A G - code command was sent, but is missing some required P or L value words in the line.";
                case 29:
                    return "Grbl supports six work coordinate systems G54 - G59.G59.1, G59.2, and G59.3 are not supported.";
                case 30:
                    return "The G53 G - code command requires either a G0 seek or G1 feed motion mode to be active.A different motion was active.";
                case 31:
                    return "There are unused axis words in the block and G80 motion mode cancel is active.";
                case 32:
                    return "A G2 or G3 arc was commanded but there are no XYZ axis words in the selected plane to trace the arc.";
                case 33:
                    return "The motion command has an invalid target.G2, G3, and G38.2 generates this error, if the arc is impossible to generate or if the probe target is the current position.";
                case 34:
                    return "A G2 or G3 arc, traced with the radius definition, had a mathematical error when computing the arc geometry.Try either breaking up the arc into semi-circles or quadrants, or redefine them with the arc offset definition.";
                case 35:
                    return "A G2 or G3 arc, traced with the offset definition, is missing the IJK offset word in the selected plane to trace the arc.";
                case 36:
                    return "There are unused, leftover G-code words that aren't used by any command in the block.";
                case 37:
                    return "The G43.1 dynamic tool length offset command cannot apply an offset to an axis other than its configured axis.The Grbl default axis is the Z - axis.";
                default:
                    return "unknown error " + id.ToString();
            }
        }
        public static string getAlarm(string rxString)
        {
            string[] tmp = rxString.Split(':');
            int id = -1;
            if (tmp.Length > 0)
                id = Convert.ToInt16(tmp[1]);
            switch (id)
            {
                case 1:
                    return "Hard limit triggered.Machine position is likely lost due to sudden and immediate halt.Re - homing is highly recommended.";
                case 2:
                    return "G - code motion target exceeds machine travel.Machine position safely retained. Alarm may be unlocked.";
                case 3:
                    return "Reset while in motion.Grbl cannot guarantee position. Lost steps are likely. Re - homing is highly recommended.";
                case 4:
                    return "Probe fail. The probe is not in the expected initial state before starting probe cycle, where G38.2 and G38.3 is not triggered and G38.4 and G38.5 is triggered.";
                case 5:
                    return "Probe fail. Probe did not contact the workpiece within the programmed travel for G38.2 and G38.4.";
                case 6:
                    return "Homing fail.Reset during active homing cycle.";
                case 7:
                    return "Homing fail.Safety door was opened during active homing cycle.";
                case 8:
                    return "Homing fail.Cycle failed to clear limit switch when pulling off. Try increasing pull - off setting or check wiring.";
                case 9:
                    return "Homing fail. Could not find limit switch within search distance. Defined as 1.5 * max_travel on search and 5 * pulloff on locate phases.";
                default:
                    return "unknown alarm " + id.ToString();
            }
        }

        public static string getSetting(int id)
        {
            switch (id)
            {
                case 0:
                    return "Step pulse time, microseconds";
                case 1:
                    return "Step idle delay, milliseconds";
                case 2:
                    return "Step pulse invert, mask";
                case 3:
                    return "Step direction invert, mask";
                case 4:
                    return "Invert step enable pin, boolean";
                case 5:
                    return "Invert limit pins, boolean";
                case 6:
                    return "Invert probe pin, boolean";
                case 10:
                    return "Status report options, mask";
                case 11:
                    return "Junction deviation, millimeters";
                case 12:
                    return "Arc tolerance, millimeters";
                case 13:
                    return "Report in inches, boolean";
                case 20:
                    return "Soft limits enable, boolean";
                case 21:
                    return "Hard limits enable, boolean";
                case 22:
                    return "Homing cycle enable, boolean";
                case 23:
                    return "Homing direction invert, mask";
                case 24:
                    return "Homing locate feed rate, mm/min";
                case 25:
                    return "Homing search seek rate, mm/min";
                case 26:
                    return "Homing switch debounce delay, milliseconds";
                case 27:
                    return "Homing switch pull-off distance, millimeters";
                case 30:
                    return "Maximum spindle speed, RPM";
                case 31:
                    return "Minimum spindle speed, RPM";
                case 32:
                    return "Laser -mode enable, boolean";
                case 100:
                    return "X -axis steps per millimeter";
                case 101:
                    return "Y -axis steps per millimeter";
                case 102:
                    return "Z -axis steps per millimeter";
                case 110:
                    return "X -axis maximum rate, mm/min";
                case 111:
                    return "Y -axis maximum rate, mm/min";
                case 112:
                    return "Z -axis maximum rate, mm/min";
                case 120:
                    return "X -axis acceleration, mm/sec^2";
                case 121:
                    return "Y -axis acceleration, mm/sec^2";
                case 122:
                    return "Z -axis acceleration, mm/sec^2";
                case 130:
                    return "X -axis maximum travel, millimeters";
                case 131:
                    return "Y -axis maximum travel, millimeters";
                case 132:
                    return "Z -axis maximum travel, millimeters";
                default:
                    return "unknown setting " + id.ToString();
            }
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
        private strStruct statMsg;
        private string lastCmd;
        public PosEventArgs(xyzPoint world, xyzPoint machine, grblState stat, strStruct msg, string last)
        {
            posWorld = world;
            posMachine = machine;
            status = stat;
            statMsg = msg;
            lastCmd = last;
        }
        public xyzPoint PosWorld
        { get { return posWorld; } }
        public xyzPoint PosMachine
        { get { return posMachine; } }
        public grblState Status
        { get { return status; } }
        public strStruct StatMsg
        { get { return statMsg; } }
        public string lastCommand
        { get { return lastCmd; } }
    }
}
