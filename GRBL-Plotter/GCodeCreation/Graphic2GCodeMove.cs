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
 * 2023-06-01 New file, split from Graphic2GCodeRelated.cs
 * 2023-11-11 replace float by double
 * 2024-04-13 l:178, 241 bug fix process time calculation
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using System;
using System.Text;

namespace GrblPlotter
{
    public static partial class Gcode
    {
        public static void SetTangential(StringBuilder gcodeValue, double angle, bool writeCode)
        {
            ModificationTangential.Angle = ((double)Properties.Settings.Default.importGCTangentialTurn * angle / 360);
            if (ModificationTangential.Enable)
            {
                ModificationTangential.Command = string.Format(" {0}{1}", ModificationTangential.Name, FrmtNum(ModificationTangential.Angle));
                if (writeCode && (Math.Abs(lasta - angle) > ModificationTangential.AngleDevi))
                { gcodeValue?.AppendFormat("G{0}{1}\r\n", FrmtCode(1), ModificationTangential.Command); }
                lasta = angle;
            }
            else ModificationTangential.Command = "";
        }

        public static void SetSValue(double sVal)
        {
            if (DepthFromWidth.SEnable)
            {
                gcodeSValueCommand = string.Format("S{0:0}", sVal);
            }
            else { gcodeSValueCommand = ""; }
        }

        public static void SetAux1DistanceCommand(double distance)
        {
            if (ModificationAux.Value1Enable)
            {
                ModificationAux.Value1Command = string.Format(" {0}{1}", ModificationAux.Value1Name, FrmtNum(distance));
                //        Logger.Info("SetAux1Distance {0}", Value1Command);
            }
            else { ModificationAux.Value1Command = ""; }
        }

        public static void SetAux2DistanceCommand(double distance)
        {
            if (ModificationAux.Value2Enable)
            {
                ModificationAux.Value2Command = string.Format(" {0}{1}", ModificationAux.Value2Name, FrmtNum(distance));
                //        Logger.Info("SetAux2Distance {0}", Value2Command);
            }
            else { ModificationAux.Value2Command = ""; }
        }



        private static void Move(StringBuilder gcodeString, int gnr, double x, double y, bool applyFeed, string cmt)
        { Move(gcodeString, gnr, x, y, null, applyFeed, cmt); }
        public static void Move(StringBuilder gcodeValueFinal, int gnr, double mx, double my, double? mz, bool applyFeed, string cmt)
        {
            if (gcodeValueFinal == null) return;

            StringBuilder gcodeString = gcodeValueFinal;
            if (gnr != 0)
            {
                if (OptionZAxis.Enable && OptionZAxis.IncrementEnable)
                { gcodeString = figureString; }// if (loggerTrace) Logger.Trace("    gcodeString = figureString"); }
            }
            string feed = "";
            StringBuilder gcodeTmp = new StringBuilder();
            bool isneeded = false;


            double xCmd = mx;
            double yCmd = my;
            double zCmd = lastz;
            double tz = 0;

            if (mz != null)
            { zCmd = (double)mz; tz = (double)mz; }

            if (Control.GcodeRelative)
            {
                xCmd = mx - lastx;
                yCmd = my - lasty;
                if (mz != null)
                { zCmd = (double)mz - lastz; tz = (double)mz; }
            }

            double delta = Fdistance(lastx, lasty, mx, my);

            double x;

            if (applyFeed && (gnr > 0))
            {
                if (gcodeXYFeedToolTable && gcodeComments) { cmt += " XY feed from tool-table"; }
                feed = string.Format("F{0}", GcodeXYFeed);
                ApplyXYFeedRate = false;                        // don't set feed next time
            }

            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);

            if (Control.gcodeCompress)
            {
                if (((gnr > 0) || (lastx != mx) || (lasty != my) || (lastz != tz)))  // else nothing to do
                {
                    if (lastg != gnr) { gcodeTmp.AppendFormat("G{0}", FrmtCode(gnr)); isneeded = true; }

                    if (lastx != mx) { gcodeTmp.AppendFormat("X{0}", FrmtNum(xCmd)); isneeded = true; }
                    if (lasty != my) { gcodeTmp.AppendFormat("Y{0}", FrmtNum(yCmd)); isneeded = true; }
                    if (mz != null)
                    {
                        if (lastz != mz) { gcodeTmp.AppendFormat("Z{0}", FrmtNum(zCmd)); isneeded = true; }
                    }
                    gcodeTmp.AppendFormat("{0}", ModificationTangential.Command);

                    if (gnr != 0)
                    {
                        gcodeTmp.AppendFormat("{0}{1}{2}", ModificationAux.Value1Command, ModificationAux.Value2Command, gcodeSValueCommand);
                    }

                    if ((gnr == 1) && (lastf != GcodeXYFeed) || applyFeed)
                    {
                        gcodeTmp.AppendFormat("F{0} ", GcodeXYFeed);
                        lastf = GcodeXYFeed;
                        isneeded = true;
                        if (gcodeXYFeedToolTable && gcodeComments) { cmt += " XY feed from tool-table"; }
                    }
                    gcodeTmp.AppendFormat("{0}\r\n", cmt);
                    if (isneeded)
                    {
                        gcodeString?.Append(gcodeTmp);
                    }
                }
            }
            else
            {   // no compress
                gcodeTmp.AppendFormat("G{0} X{1} Y{2}", FrmtCode(gnr), FrmtNum(xCmd), FrmtNum(yCmd));
                if (mz != null)
                    gcodeTmp.AppendFormat(" Z{0}", FrmtNum(zCmd));

                gcodeTmp.AppendFormat("{0}", ModificationTangential.Command);
                if (gnr != 0)
                {
                    gcodeTmp.AppendFormat("{0}{1}{2}", ModificationAux.Value1Command, ModificationAux.Value2Command, gcodeSValueCommand);
                }
                gcodeTmp.AppendFormat(" {0} {1}\r\n", feed, cmt);

                gcodeString?.Append(gcodeTmp);
            }

            if (OptionZAxis.Enable && OptionZAxis.IncrementEnable)
            {
                Tracker.gcodeFigureTime += 60 * delta / GcodeXYFeed; ;
                Tracker.gcodeFigureLines++;
            }
            else
            {
                Tracker.gcodeExecutionSeconds += 0;// delta / GcodeXYFeed;
                Tracker.gcodeLines++;
            }
            lastx = mx; lasty = my; lastg = gnr; // lastz = tz;
        }


        public static void Arc(StringBuilder gcodeValue, int gnr, double ax, double ay, double ai, double aj, string cmt)//, bool avoidG23 = false)
        {
            if (string.IsNullOrEmpty(cmt)) cmt = "";
            if (gcodeValue != null) MoveArc(gcodeValue, gnr, ax, ay, ai, aj, ApplyXYFeedRate, cmt, false);
        }
        private static void MoveArc(StringBuilder gcodeStringFinal, int gnr, double x, double y, double i, double j, bool applyFeed, string cmt, bool avoidG23)
        {
            if (string.IsNullOrEmpty(cmt)) cmt = "";
            if (gcodeStringFinal == null) return;

            StringBuilder gcodeString = gcodeStringFinal;
            if (OptionZAxis.Enable && OptionZAxis.IncrementEnable)
            { gcodeString = figureString; if (LoggerTraceImport) Logger.Trace("    gcodeString = figureString"); }

            string feed = "";
            StringBuilder gcodeTmp = new StringBuilder();

            double xCmd = x;
            double yCmd = y;

            if (Control.GcodeRelative)
            {
                xCmd = x - lastx;
                yCmd = y - lasty;
            }

            if (applyFeed)
            {
                feed = string.Format("F{0}", GcodeXYFeed);
                ApplyXYFeedRate = false;                        // don't set feed next time
            }
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
            if (Control.gcodeNoArcs || avoidG23)
            {
                XyzabcuvwPoint last = new XyzabcuvwPoint();
                last.X = lastx; last.Y = lasty;
                XyzabcuvwPoint now = new XyzabcuvwPoint();
                now.X = x; now.Y = y;
                SplitArc(gcodeString, gnr, last, now, i, j, cmt);
            }
            else
            {
                gcodeTmp.AppendFormat("G{0} X{1} Y{2} I{3} J{4}", FrmtCode(gnr), FrmtNum(xCmd), FrmtNum(yCmd), FrmtNum(i), FrmtNum(j));
                gcodeTmp.AppendFormat(" {0}{1}{2}{3}", ModificationTangential.Command, ModificationAux.Value1Command, ModificationAux.Value2Command, gcodeSValueCommand);
                gcodeTmp.AppendFormat(" {0} {1}\r\n", feed, cmt);

                gcodeString?.Append(gcodeTmp);
                lastg = gnr;
            }
            if (OptionZAxis.Enable && OptionZAxis.IncrementEnable)
            {
                Tracker.gcodeFigureTime += 60 * Fdistance(lastx, lasty, x, y) / GcodeXYFeed;
                Tracker.gcodeFigureLines++;
            }
            else
            {
                Tracker.gcodeExecutionSeconds += 0;// Fdistance(lastx, lasty, x, y) / GcodeXYFeed;
                Tracker.gcodeLines++;
            }
            lastx = x; lasty = y; lastf = GcodeXYFeed;
            Control.LastMovewasG0 = false;
        }

    }
}
