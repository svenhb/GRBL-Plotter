/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2025 Sven Hasemann contact: svenhb@web.de

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
/*  GCodeSynthesize.cs
	Synthsize GCode after transforming
*/
/* 
* 2021-07-09 split code from GCodeVisuAndTransform
* 2022-01-04 fix convertZtoS problem #245
* 2023-01-28 add %NM tag, to keep code-line when synthezising code
* 2024-03-23 l:92 f:CreateGCodeProg use XyzabcuvwPoint for lastActual
* 2024-05-28 l:85 f:CreateGCodeProg add logs
* 2025-04-07 l:141 f:CreateGCodeProg keep (PD) and (PU) comment information
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrblPlotter
{
    internal static partial class VisuGCode
    {
        public enum ConvertMode { Nothing, RemoveZ, ConvertZToS };

        internal static float heightMapGridWidth = 100;

        public static string ReplaceG23()
        { return CreateGCodeProg(true, false, false, ConvertMode.Nothing); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        public static string ConvertZ()
        { return CreateGCodeProg(false, false, false, ConvertMode.ConvertZToS); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)
        public static string RemoveZ()
        { return CreateGCodeProg(false, false, false, ConvertMode.RemoveZ); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        /// <summary>
        /// apply new z-value to all gcode coordinates
        /// </summary>
        internal static string ApplyHeightMap(HeightMap Map)//IList<string> oldCode,
        {
            heightMapGridWidth = (float)Map.GridX;
            //getGCodeLines(oldCode, null, null, true);                // read gcode and process subroutines
            Logger.Debug("ApplyHeightMap  splitMoves by:{0}", heightMapGridWidth);
            IList<string> tmp = CreateGCodeProg(true, true, false, ConvertMode.Nothing, null, "Split moves").Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();      // split lines and arcs createGCodeProg(bool replaceG23, bool applyNewZ, bool removeZ, HeightMap Map=null)
            Logger.Debug("ApplyHeightMap  reload code");
            GetGCodeLines(tmp, null, null, false);                  // reload code
            Logger.Debug("ApplyHeightMap  apply map");
            return CreateGCodeProg(false, false, true, ConvertMode.Nothing, Map, "Apply Map");        // apply new Z-value;
        }

        /// <summary>
        /// undo height map (reload saved backup)
        /// </summary>
        public static void ClearHeightMap()
        {
            pathHeightMap.Reset();
            pathBackground.Reset();
        }

        /// <summary>
        /// Generate GCode from given coordinates in GCodeList
        /// only replace lines with coordinate information
        /// </summary>
        private static string CreateGCodeProg(string info = "")
        { return CreateGCodeProg(false, false, false, ConvertMode.Nothing, null, info); }
        internal static string CreateGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, ConvertMode specialCmd, HeightMap Map = null, string info = "")
        {
            Logger.Debug("+++ CreateGCodeProg replaceG23: {0}, splitMoves: {1}, applyNewZ: {2}, specialCmd: {3}, info: '{4}'", replaceG23, splitMoves, applyNewZ, specialCmd, info);
            if (replaceG23)
                Logger.Debug("--- CreateGCodeProg replaceG23 Arc circumfence step:{0}, segmentLength:{1}, equidistance:{2}", Properties.Settings.Default.importGCSegment, Properties.Settings.Default.importGCLineSegmentLength, Properties.Settings.Default.importGCLineSegmentEquidistant);
            if (splitMoves)
                Logger.Debug("--- CreateGCodeProg splitMoves heightMapGridWidth:{0}", heightMapGridWidth);

            if (gcodeList == null) return "";
            pathMarkSelection.Reset();
            lastFigureNumber = -1; lastFigureNumbers.Clear();
            StringBuilder newCode = new StringBuilder();
            StringBuilder tmpCode = new StringBuilder();
        //    string infoCode = "";
            string comment = "";
            bool getCoordinateXY, getCoordinateZ, forceM;
            double feedRate = 0;
            double spindleSpeed = -1; // force change
            byte spindleState = 5;
            byte coolantState = 9;

            double i, j;
            XyzabcuvwPoint lastActual = new XyzabcuvwPoint();
            double newZ;

            double convertMinZ = xyzSize.minz;          // 1st get last minimum
            double convertMaxZ = xyzSize.maxz;          // 1st get last minimum
            xyzSize.ResetDimension();                   // then reset
            bool hide_code = false;
            bool isArc;

            bool addInfo = info.Length > 0;

            Gcode.Setup(false);                             // don't apply intermediate Z steps in certain sub functions
                                                            //    MyApplication.ESCwasPressed = false;
            for (int iCode = 0; iCode < gcodeList.Count; iCode++)     // go through all code lines
            {
                GcodeByLine gcline = gcodeList[iCode];
                tmpCode.Clear();
                getCoordinateXY = false;
                getCoordinateZ = false;
                forceM = false;
                if (gcline.codeLine.Length == 0)
                    continue;

                if (addInfo && gcline.codeLine.Contains("/Header"))
                {
                    newCode.AppendLine(gcline.codeLine.Trim('\r', '\n'));
                    newCode.AppendLine("(" + info + ")");
                    addInfo = false;
                    continue;
                }

                if (gcline.codeLine.Contains("("))
                {
                    if (gcline.codeLine.IndexOf("%START_HIDECODE") >= 0) { hide_code = true; }
                    if (gcline.codeLine.IndexOf("%STOP_HIDECODE") >= 0) { hide_code = false; }
                    if (gcline.codeLine.IndexOf("%NM ") >= 0) { gcline.isNoMove = true; }
                    if (!gcline.codeLine.StartsWith("("))
                        comment = " " + gcline.codeLine.Substring(gcline.codeLine.IndexOf("("));
                }
                else
                    comment = "";

                #region replace circle by lines
                // replace code-line G1,G2,G3 by new codelines and add to newCode
                if ((!hide_code) && (replaceG23))                   // replace circles
                {
                    //Gcode.Setup(false);   moved 2024-03-11                          // don't apply intermediate Z steps in certain sub functions
                    Gcode.SetLastxyz(lastActual.X, lastActual.Y, lastActual.Z);
                    Gcode.GcodeXYFeed = gcline.feedRate;
                    if (gcline.isdistanceModeG90)
                        Gcode.GcodeRelative = false;
                    else
                        Gcode.GcodeRelative = true;

                    if ((gcline.motionMode > 1) && (gcline.motionMode <= 3))    // handle arc
                    {
                        i = (double)((gcline.i != null) ? gcline.i : 0.0);
                        j = (double)((gcline.j != null) ? gcline.j : 0.0);
                        Gcode.SplitArc(newCode, gcline.motionMode, lastActual, gcline.actualPos, i, j, gcline.codeLine);
                    }
                    else if (gcline.motionMode == 1)                            // handle straight move
                    {
                        if (((gcline.x != null) || (gcline.y != null) || (gcline.z != null)) && splitMoves)     // any movement command?
                        {
                            XyzabcuvwPoint d = gcline.actualPos - lastActual;
                            double c = Math.Sqrt(d.X * d.X + d.Y * d.Y);
                            if (c > heightMapGridWidth)                                                         // movement long enough to split?
                            {
                                if (gcline.z != null)
                                {
                                    if ((gcline.x != null) || (gcline.y != null))
                                    { Gcode.SplitLineZ(newCode, gcline.motionMode, lastActual, gcline.actualPos, heightMapGridWidth, true, gcline.codeLine); }
                                    else
                                    { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }
                                }
                                else
                                {
                                    Gcode.SplitLine(newCode, gcline.motionMode, lastActual, gcline.actualPos, heightMapGridWidth, true, gcline.codeLine);
                                }
                            }
                            else
                            { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); comment = ""; }   // not long enough
                        }
                        else
                        { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); comment = ""; }       // no movement
                    }
                    else
                    { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); comment = ""; }           // no G1,2,3 command

                }
                #endregion
                // if no replacement, stitch code together
                else
                {
                    if (gcline.isNoMove)
                    {
                        comment = "";
                        newCode.AppendLine(gcline.codeLine.Trim('\r', '\n'));    // add orignal code-line
                    }
                    else
                    {
                        if (gcline.x != null)
                        { tmpCode.AppendFormat(" X{0}", Gcode.FrmtNum((double)gcline.x)); getCoordinateXY = true; }
                        if (gcline.y != null)
                        { tmpCode.AppendFormat(" Y{0}", Gcode.FrmtNum((double)gcline.y)); getCoordinateXY = true; }

                        if ((getCoordinateXY || (gcline.z != null)) && applyNewZ && (Map != null))  //(gcline.motionMode != 0) &&       if (getCoordinateXY && applyNewZ && (Map != null))
                        {
                            if (!gcline.ismachineCoordG53 && gcline.isdistanceModeG90)				//  ismachineCoordG53 includes G28 (GCodeParser.cs)
                            {
                                newZ = Map.InterpolateZ(gcline.actualPos.X, gcline.actualPos.Y);
                                if (gcline.z == null)
                                    gcline.z = gcline.actualPos.Z;
                                //      infoCode = string.Format("( dZ:{0:0.000} actZ:{1:0.000} )", newZ, gcline.z);
                                if (gcline.motionMode != 0)
                                { gcline.z += newZ; }
                            }
                        }

                        if (specialCmd == ConvertMode.ConvertZToS)
                        {
                            gcline.spindleSpeed = (int)spindleSpeed;    // reset old speed
                            if (gcline.spindleState < 5)                // if spindle on
                                forceM = true;
                        }

                        if (gcline.z != null)
                        {
                            if (specialCmd == ConvertMode.Nothing)               //  !removeZ
                            { tmpCode.AppendFormat(" Z{0}", Gcode.FrmtNum((double)gcline.z)); }
                            else if ((specialCmd == ConvertMode.ConvertZToS))// && (convertMinZ != 0))
                            {
                                gcline.spindleSpeed = ConvertZtoS(gcline.actualPos.Z, convertMaxZ, convertMinZ);
                                spindleSpeed = -1;  // force output
                            }
                            getCoordinateZ = true;
                        }
                        if (gcline.a != null)
                        { tmpCode.AppendFormat(" A{0}", Gcode.FrmtNum((double)gcline.a)); getCoordinateZ = true; }
                        if (gcline.b != null)
                        { tmpCode.AppendFormat(" B{0}", Gcode.FrmtNum((double)gcline.b)); getCoordinateZ = true; }
                        if (gcline.c != null)
                        { tmpCode.AppendFormat(" C{0}", Gcode.FrmtNum((double)gcline.c)); getCoordinateZ = true; }
                        if (gcline.u != null)
                        { tmpCode.AppendFormat(" U{0}", Gcode.FrmtNum((double)gcline.u)); getCoordinateZ = true; }
                        if (gcline.v != null)
                        { tmpCode.AppendFormat(" V{0}", Gcode.FrmtNum((double)gcline.v)); getCoordinateZ = true; }
                        if (gcline.w != null)
                        { tmpCode.AppendFormat(" W{0}", Gcode.FrmtNum((double)gcline.w)); getCoordinateZ = true; }
                        if (gcline.i != null)
                        { tmpCode.AppendFormat(" I{0}", Gcode.FrmtNum((double)gcline.i)); getCoordinateXY = true; }
                        if (gcline.j != null)
                        { tmpCode.AppendFormat(" J{0}", Gcode.FrmtNum((double)gcline.j)); getCoordinateXY = true; }

                        if ((getCoordinateXY || getCoordinateZ) && (!gcline.ismachineCoordG53) && (!hide_code))
                        {
                            //    bool keepComment = false;
                            if ((gcline.motionMode > 0) && (feedRate != gcline.feedRate) && (getCoordinateXY || getCoordinateZ)) //((getCoordinateXY && !getCoordinateZ) || (!getCoordinateXY && getCoordinateZ)))
                            { tmpCode.AppendFormat(" F{0,0}", gcline.feedRate); }       // feed
                            if (spindleState != gcline.spindleState)
                            { tmpCode.AppendFormat(" M{0,0}", gcline.spindleState); }   // state
                            if (spindleSpeed != gcline.spindleSpeed)
                            {
                                tmpCode.AppendFormat(" S{0,0}", gcline.spindleSpeed);   // speed
                                                                                        //        keepComment = true; // keep (PU) / (PD)
                            }
                            if (coolantState != gcline.coolantState)
                            { tmpCode.AppendFormat(" M{0,0}", gcline.coolantState); }   // state

                            /*    if (keepComment)
                                {
                                    int strtCmt = gcline.codeLine.IndexOf("(");
                                    if (strtCmt > 0)
                                        tmpCode.AppendFormat(" {0}", gcline.codeLine.Substring(strtCmt));
                                }
    */
                            tmpCode.Replace(',', '.');
                            if (gcline.codeLine.IndexOf("(Setup - GCode") > 1)  // ignore coordinates from setup footer
                                newCode.AppendLine(gcline.codeLine);
                            else
                            {
                                // newCode.AppendLine(gcline.otherCode + "G" + gcode.frmtCode(gcline.motionMode) + tmpCode.ToString() + infoCode);
                                if (gcline.nNumber >= 0)
                                    newCode.AppendLine("N" + gcline.nNumber + " " + gcline.otherCode + "G" + Gcode.FrmtCode(gcline.motionMode) + tmpCode.ToString() + comment);
                                else
                                    newCode.AppendFormat("{0}G{1}{2}{3}\r\n", gcline.otherCode, Gcode.FrmtCode(gcline.motionMode), tmpCode.ToString(), comment);
                                //       newCode.AppendLine(gcline.otherCode + "G" + Gcode.FrmtCode(gcline.motionMode) + tmpCode.ToString() + infoCode + comment);
                            }
                        }
                        else
                        {
                            if (forceM && ((spindleState != gcline.spindleState) || (spindleSpeed != gcline.spindleSpeed)))
                            {
                                tmpCode.AppendFormat("M{0,0} S{1,0}", Gcode.FrmtCode(gcline.spindleState), gcline.spindleSpeed);    // state
                                newCode.AppendLine(tmpCode.ToString());
                            }
                            else
                        //        newCode.AppendFormat("{0}\r\n", gcline.codeLine.Trim('\r', '\n'));    // add orignal code-line
                                newCode.AppendLine(gcline.codeLine.Trim('\r', '\n'));    // add orignal code-line
                    //    newCode.AppendLine(gcline.codeLine.Trim('\r', '\n') + infoCode + comment);    // add orignal code-line
                        }
                        //     lastMotionMode = gcline.motionMode;
                    }
                }
                feedRate = gcline.feedRate;
                spindleSpeed = gcline.spindleSpeed;
                spindleState = gcline.spindleState;
                coolantState = gcline.coolantState;
                lastActual = gcline.actualPos;

                if ((!hide_code) && (!gcline.ismachineCoordG53) && (gcline.codeLine.IndexOf("(Setup - GCode") < 1)) // ignore coordinates from setup footer
                {
                    if (!((gcline.actualPos.X == Grbl.posWork.X) && (gcline.actualPos.Y == Grbl.posWork.Y)))            // don't add actual tool pos
                        xyzSize.SetDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);
                }

                isArc = ((gcline.motionMode == 2) || (gcline.motionMode == 3));
                coordList.Add(new CoordByLine(iCode, gcline.figureNumber, (XyzPoint)gcline.actualPos, (XyzPoint)gcline.actualPos, gcline.motionMode, gcline.alpha, isArc));

                //    Application.DoEvents();
                if (MyApplication.ESCwasPressed)
                {
                    Logger.Warn("CreateGCodeProg abort by ESC");
                    break;
                }
            }
            return newCode.ToString().Replace(',', '.');
        }

        private static int ConvertZtoS(double z, double maxZ, double minZ)
        {
            double convertMaxSpeed = (double)Properties.Settings.Default.convertZtoSMax;
            double convertMinSpeed = (double)Properties.Settings.Default.convertZtoSMin;
            double convertSpeedRange = Math.Abs(convertMaxSpeed - convertMinSpeed);
            double convertOffSpeed = (double)Properties.Settings.Default.convertZtoSOff;
            //    Logger.Info("convertZtoS z:{0} max:{1} min:{2}",z,maxZ,minZ);
            if (z > 0)
            { return (int)convertOffSpeed; }
            int result = (int)(z * convertSpeedRange / minZ + convertMinSpeed);
            //    Logger.Info("convertZtoS z:{0} convertSpeedRange:{1} convertMinSpeed:{2}  result:{3}", z, convertSpeedRange, convertMinSpeed, result);
            return result;
        }
    }
}