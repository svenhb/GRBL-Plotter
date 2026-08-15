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
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.UserControls;
using System;
using System.Text;

namespace GrblPlotter
{
    public static partial class Gcode
    {
        public static void JobStartDevice(StringBuilder gcodeValue, string cmto)
        {
        //    gcodeComments = LoggerTraceImport = true;
            string cmt = "(JobStart)";

            if (Import.SelectedDevice == DeviceSelection.Router)
            {
                gcodeValue.AppendFormat("G{0} Z{1} {2}\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up), "(PU)"+cmt); // use G0 without feedrate
                gcodeValue.AppendFormat("M{0} S{1} {2}\r\n", Spindle.SpindleCmd, Spindle.Speed, cmt);
                Tracker.gcodeLines++;

                double delay = (double)Properties.Settings.Default.importGCSpindleDelay;
                if (delay > 0)
                {
                    string tmp = "(Delay)";
                    gcodeValue.AppendFormat("G{0} P{1} {2}\r\n", FrmtCode(4), delay, tmp);
                    Tracker.gcodeLines++;
                }
            }
            else
            {
                PenUpDevice(gcodeValue, "PU");
            }
            return;
        }
        public static void JobEndDevice(StringBuilder gcodeValue, string cmt)
        {
            if (Import.SelectedDevice == DeviceSelection.Router)
            {
                gcodeValue.AppendFormat("M{0}{1}\r\n", FrmtCode(5), " (JobEnd)");
            }
        }

        public static void PenDownDevice(StringBuilder gcodeValue, string cmto)
        {
            if (gcodeValue == null)
            { return; }
            ApplyXYFeedRate = true;     // apply XY FeedXY Rate after each PenDown command (not just after Z-axis)

            if (Import.SelectedDevice == DeviceSelection.Laser)
            {
                if (OptionZAxis.Enable)
                {
                    gcodeValue.AppendFormat("G{0} Z{1} F{2} (Z-down)\r\n", FrmtCode(1), FrmtNum(OptionZAxis.Down), OptionZAxis.Feed);
                    Tracker.gcodeLines++;
                }
                float s = DepthFromWidth.SEnable ? DepthFromWidth.SMin : Spindle.Speed;
                gcodeValue.AppendFormat("M{0} S{1:0} ({2})(laser on)\r\n", Spindle.SpindleCmd, s, cmto);
            }

            if (Import.SelectedDevice == DeviceSelection.Plotter)
            {
                if (OptionPWM.Enable)
                {
                    gcodeValue.AppendFormat("M{0} S{1} ({2})(S-down)\r\n", Spindle.SpindleCmd, OptionPWM.Down, cmto);
                    if (OptionPWM.DlyDown > 0)
                        gcodeValue.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(OptionPWM.DlyDown));
                    Tracker.gcodeExecutionSeconds += OptionPWM.DlyDown;
                    Tracker.gcodeLines++;
                }
                else if (OptionZAxis.Enable)
                {
                    gcodeValue.AppendFormat("G{0} Z{1} F{2} ({3})(Z-down)\r\n", FrmtCode(1), FrmtNum(OptionZAxis.Down), OptionZAxis.Feed, cmto);
                }
                else if (Spindle.LasermodeEnable)
                { gcodeValue.AppendFormat("M{0} S{1:0} ({2})(laser on)\r\n", Spindle.SpindleCmd, Spindle.Speed, cmto); }
            }

            if (Import.SelectedDevice == DeviceSelection.Router)
            {
                //    SpindleOn(gcodeValue, cmt);
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
                    gcodeValue.AppendFormat("G{0} Z{1} F{2} ({3})(Z-down)\r\n", FrmtCode(1), FrmtNum(OptionZAxis.Down), OptionZAxis.Feed, cmto);
                }
            }

            Tracker.gcodeLines++;
            Tracker.gcodeDownUp++;
            lastg = 1; lastf = OptionZAxis.Feed;
            lastz = OptionZAxis.Down;
            return;
        }

        public static void PenUpDevice(StringBuilder gcodeValue, string cmt)
        {
            if (gcodeValue == null)
            { return; }
            if (Import.SelectedDevice == DeviceSelection.Laser)
            {
                gcodeValue.AppendFormat("M{0} S0 ({1})(laser off)\r\n", Spindle.SpindleCmd, cmt);
                if (OptionZAxis.Enable)
                {
                    gcodeValue.AppendFormat("G{0} Z{1} (Z-up)\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up));
                }
                Tracker.gcodeExecutionSeconds += 60 * Math.Abs((OptionZAxis.Up - OptionZAxis.Down) / Math.Max(OptionZAxis.Feed, 10));
                Tracker.gcodeLines++;
                return;
            }
            if (Import.SelectedDevice == DeviceSelection.Plotter)
            {
                if (OptionPWM.Enable)
                {
                    gcodeValue.AppendFormat("M{0} S{1} ({2})(S-up)\r\n", Spindle.SpindleCmd, OptionPWM.Up, cmt);
                    if (OptionPWM.DlyUp > 0)
                        gcodeValue.AppendFormat("G{0} P{1}\r\n", FrmtCode(4), FrmtNum(OptionPWM.DlyUp));
                    Tracker.gcodeExecutionSeconds += OptionPWM.DlyUp;
                    Tracker.gcodeLines++;
                }
                else if (OptionZAxis.Enable)
                { gcodeValue.AppendFormat("G{0} Z{1} ({2})(Z-up)\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up), cmt); }
                else if (Spindle.LasermodeEnable)
                { gcodeValue.AppendFormat("M{0} S0 ({1})(laser off)\r\n", Spindle.SpindleCmd, cmt); }
            }
            if (Import.SelectedDevice == DeviceSelection.Router)
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
                    gcodeValue.AppendFormat("G{0} Z{1} ({2})(Z-up)\r\n", FrmtCode(0), FrmtNum(OptionZAxis.Up), cmt);
                }
            }
            Tracker.gcodeLines++;
            lastg = 1; lastf = OptionZAxis.Feed;
            lastz = OptionZAxis.Up;
            return;
        }

    }
}