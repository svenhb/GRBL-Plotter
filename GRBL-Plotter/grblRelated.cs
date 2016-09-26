/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2016 Sven Hasemann contact: svenhb@web.de

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GRBL_Plotter
{
    public enum grblState { idle, run, hold, home, alarm, check, door, probe, unknown };
    public enum grblStreaming { ok, error, reset, finish, pause, waitidle, toolchange, stop };
    public static class grbl
    {   // http://www.shapeoko.com/wiki/index.php/G-Code#G-code_Not_supported_by_Grbl
        public static int[] unknownG = {41,64,81,83};
        public static grblState parseStatus(string status)    // {idle, run, hold, home, alarm, check, door}
        {
            if (status.IndexOf("Idle") >= 0) { return grblState.idle; }
            if (status.IndexOf("Run") >= 0) { return grblState.run; }
            if (status.IndexOf("Hold") >= 0) { return grblState.hold; }
            if (status.IndexOf("Home") >= 0) { return grblState.home; }
            if (status.IndexOf("Alarm") >= 0) { return grblState.alarm; }
            if (status.IndexOf("Check") >= 0) { return grblState.check; }
            if (status.IndexOf("Door") >= 0) { return grblState.door; }
            return grblState.unknown;
        }
        public static string statusToText(grblState state)
        {
            switch (state)
            {
                case grblState.run: return "Run";
                case grblState.hold: return "Hold";
                case grblState.home: return "Homing";
                case grblState.check: return "Check code";
                case grblState.idle: return "Idle";
                case grblState.probe: return "Probing";
                case grblState.alarm: return "Alarm";
                case grblState.door: return "Door";
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
        private string lastCmd;
        public PosEventArgs(xyzPoint world, xyzPoint machine, grblState stat, string last)
        {
            posWorld = world;
            posMachine = machine;
            status = stat;
            lastCmd = last;
        }
        public xyzPoint PosWorld
        { get { return posWorld; } }
        public xyzPoint PosMachine
        { get { return posMachine; } }
        public grblState Status
        { get { return status; } }
        public string lastCommand
        { get { return lastCmd; } }
    }
}
