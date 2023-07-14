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
 * 2023-06-01 New file, split from Graphic2GCodeRelated.cs
*/

using System;
using System.Text;

namespace GrblPlotter
{
    public static partial class Gcode
    {
        public static void SetTangential(StringBuilder gcodeValue, double angle, bool writeCode)
        {
            gcodeTangentialAngle = (float)((double)Properties.Settings.Default.importGCTangentialTurn * angle / 360);
            if (gcodeTangentialEnable)
            {
                gcodeTangentialCommand = string.Format(" {0}{1}", gcodeTangentialName, FrmtNum(gcodeTangentialAngle));
                if (writeCode && (Math.Abs(lasta - angle) > gcodeTangentialAngleDevi))
                { gcodeValue?.AppendFormat("G{0}{1}\r\n", FrmtCode(1), gcodeTangentialCommand); }
                lasta = angle;
            }
            else gcodeTangentialCommand = "";
        }

        public static void SetSValue(double sVal)
        {
            if (gcodeSValueEnable)
            {
                gcodeSValueCommand = string.Format("S{0:0}", sVal);
            }
            else { gcodeSValueCommand = ""; }
        }

        public static void SetAux1DistanceCommand(double distance)
        {
            if (gcodeAuxiliaryValue1Enable)
            {
                gcodeAuxiliaryValue1Command = string.Format(" {0}{1}", gcodeAuxiliaryValue1Name, FrmtNum(distance));
                //        Logger.Info("SetAux1Distance {0}", gcodeAuxiliaryValue1Command);
            }
            else { gcodeAuxiliaryValue1Command = ""; }
        }

        public static void SetAux2DistanceCommand(double distance)
        {
            if (gcodeAuxiliaryValue2Enable)
            {
                gcodeAuxiliaryValue2Command = string.Format(" {0}{1}", gcodeAuxiliaryValue2Name, FrmtNum(distance));
                //        Logger.Info("SetAux2Distance {0}", gcodeAuxiliaryValue2Command);
            }
            else { gcodeAuxiliaryValue2Command = ""; }
        }



        private static void Move(StringBuilder gcodeString, int gnr, double x, double y, bool applyFeed, string cmt)
        { Move(gcodeString, gnr, x, y, null, applyFeed, cmt); }
        public static void Move(StringBuilder gcodeValueFinal, int gnr, double mx, double my, double? mz, bool applyFeed, string cmt)
        {
            if (gcodeValueFinal == null) return;

            StringBuilder gcodeString = gcodeValueFinal;
            if (gnr != 0)
            {
                if (GcodeZApply && repeatZ)
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

            if (GcodeRelative)
            {
                xCmd = mx - lastx;
                yCmd = my - lasty;
                if (mz != null)
                { zCmd = (double)mz - lastz; tz = (double)mz; }
            }

            float delta = Fdistance(lastx, lasty, mx, my);

            double x;

            if (applyFeed && (gnr > 0))
            {
                if (gcodeXYFeedToolTable && gcodeComments) { cmt += " XY feed from tool-table"; }
                feed = string.Format("F{0}", GcodeXYFeed);
                ApplyXYFeedRate = false;                        // don't set feed next time
            }

            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);

            if (gcodeCompress)
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
                    gcodeTmp.AppendFormat("{0}", gcodeTangentialCommand);

                    if (gnr != 0)
                    {
                        gcodeTmp.AppendFormat("{0}{1}{2}", gcodeAuxiliaryValue1Command, gcodeAuxiliaryValue2Command, gcodeSValueCommand);
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

                gcodeTmp.AppendFormat("{0}", gcodeTangentialCommand);
                if (gnr != 0)
                {
                    gcodeTmp.AppendFormat("{0}{1}{2}", gcodeAuxiliaryValue1Command, gcodeAuxiliaryValue2Command, gcodeSValueCommand);
                }
                gcodeTmp.AppendFormat(" {0} {1}\r\n", feed, cmt);

                gcodeString?.Append(gcodeTmp);
            }

            if (GcodeZApply && repeatZ)
            {
                gcodeFigureTime += delta / GcodeXYFeed; ;
                gcodeFigureLines++;
            }
            else
            {
                gcodeTime += delta / GcodeXYFeed;
                gcodeLines++;
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
            if (GcodeZApply && repeatZ)
            { gcodeString = figureString; if (LoggerTraceImport) Logger.Trace("    gcodeString = figureString"); }

            string feed = "";
            StringBuilder gcodeTmp = new StringBuilder();

            double xCmd = x;
            double yCmd = y;

            if (GcodeRelative)
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
            if (gcodeNoArcs || avoidG23)
            {
                SplitArc(gcodeString, gnr, lastx, lasty, x, y, i, j, cmt);
            }
            else
            {
                gcodeTmp.AppendFormat("G{0} X{1} Y{2} I{3} J{4}", FrmtCode(gnr), FrmtNum(xCmd), FrmtNum(yCmd), FrmtNum(i), FrmtNum(j));
                gcodeTmp.AppendFormat(" {0}{1}{2}{3}", gcodeTangentialCommand, gcodeAuxiliaryValue1Command, gcodeAuxiliaryValue2Command, gcodeSValueCommand);
                gcodeTmp.AppendFormat(" {0} {1}\r\n", feed, cmt);

                gcodeString?.Append(gcodeTmp);
                lastg = gnr;
            }
            if (GcodeZApply && repeatZ)
            {
                gcodeFigureTime += Fdistance(lastx, lasty, x, y) / GcodeXYFeed;
                gcodeFigureLines++;
            }
            else
            {
                gcodeTime += Fdistance(lastx, lasty, x, y) / GcodeXYFeed;
                gcodeLines++;
            }
            lastx = (float)x; lasty = (float)y; lastf = GcodeXYFeed;
            LastMovewasG0 = false;
        }

    }
}
