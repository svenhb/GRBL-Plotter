/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

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
*/
using System;
using System.Text;

#pragma warning disable CA1305
#pragma warning disable CA1307

namespace GrblPlotter
{
    internal static partial class VisuGCode
    {


        public static string ReplaceG23()
        { return CreateGCodeProg(true, false, false, ConvertMode.Nothing); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        public static string ConvertZ()
        { return CreateGCodeProg(false, false, false, ConvertMode.ConvertZToS); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)
        public static string RemoveZ()
        { return CreateGCodeProg(false, false, false, ConvertMode.RemoveZ); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        /// <summary>
        /// Generate GCode from given coordinates in GCodeList
        /// only replace lines with coordinate information
        /// </summary>
        private static string CreateGCodeProg()
        { return CreateGCodeProg(false, false, false, ConvertMode.Nothing); }
        private static string CreateGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, ConvertMode specialCmd, HeightMap Map = null)
        {
            Logger.Debug("createGCodeProg replaceG23: {0}, splitMoves: {1}, applyNewZ: {2}, specialCmd: {3}", replaceG23, splitMoves, applyNewZ, specialCmd);
            if (gcodeList == null) return "";
            pathMarkSelection.Reset(); lastFigureNumber = -1;
            StringBuilder newCode = new StringBuilder();
            StringBuilder tmpCode = new StringBuilder();
            string infoCode = "";
            bool getCoordinateXY, getCoordinateZ, forceM;
            double feedRate = 0;
            double spindleSpeed = -1; // force change
            byte spindleState = 5;
            byte coolantState = 9;
            double lastActualX = 0, lastActualY = 0, lastActualZ = 0, i, j;
            double newZ;
            //     int lastMotionMode;
            double convertMinZ = xyzSize.minz;          // 1st get last minimum
            xyzSize.ResetDimension();                   // then reset
            bool hide_code = false;
            double convertMaxSpeed = (double)Properties.Settings.Default.convertZtoSMax;
            double convertMinSpeed = (double)Properties.Settings.Default.convertZtoSMin;
            double convertSpeedRange = Math.Abs(convertMaxSpeed - convertMinSpeed);
            double convertOffSpeed = (double)Properties.Settings.Default.convertZtoSOff;
            bool isArc;

            for (int iCode = 0; iCode < gcodeList.Count; iCode++)     // go through all code lines
            {
                GcodeByLine gcline = gcodeList[iCode];
                tmpCode.Clear();
                getCoordinateXY = false;
                getCoordinateZ = false;
                forceM = false;
                if (gcline.codeLine.Length == 0)
                    continue;

                if (gcline.codeLine.IndexOf("%START_HIDECODE") >= 0) { hide_code = true; }
                if (gcline.codeLine.IndexOf("%STOP_HIDECODE") >= 0) { hide_code = false; }

                #region replace circle by lines
                // replace code-line G1,G2,G3 by new codelines and add to newCode
                if ((!hide_code) && (replaceG23))                   // replace circles
                {
                    Gcode.Setup(false);                             // don't apply intermediate Z steps in certain sub functions
                                                                    //gcode.lastx = (float)lastActualX;
                                                                    // gcode.lasty = (float)lastActualY;
                                                                    //gcode.lastz = (float)lastActualZ;
                    Gcode.SetLastxyz(lastActualX, lastActualY, lastActualZ);
                    Gcode.GcodeXYFeed = gcline.feedRate;
                    if (gcline.isdistanceModeG90)
                        Gcode.GcodeRelative = false;
                    else
                        Gcode.GcodeRelative = true;
                    if ((gcline.motionMode > 1) && (gcline.motionMode <= 3))    // handle arc
                    {
                        i = (double)((gcline.i != null) ? gcline.i : 0.0);
                        j = (double)((gcline.j != null) ? gcline.j : 0.0);
                        Gcode.SplitArc(newCode, gcline.motionMode, (float)lastActualX, (float)lastActualY, (float)gcline.actualPos.X, (float)gcline.actualPos.Y, (float)i, (float)j, gcline.codeLine);
                    }
                    else if (gcline.motionMode == 1)                            // handle straight move
                    {
                        if (((gcline.x != null) || (gcline.y != null) || (gcline.z != null)) && splitMoves)
                        {
                            if (gcline.z != null)
                            {
                                if ((gcline.x != null) || (gcline.y != null))
                                { Gcode.SplitLineZ(newCode, gcline.nNumber, gcline.motionMode, (float)lastActualX, (float)lastActualY, (float)lastActualZ, (float)gcline.actualPos.X, (float)gcline.actualPos.Y, (float)gcline.actualPos.Z, maxStep, true, gcline.codeLine); }
                                else
                                { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }
                            }
                            else
                                Gcode.SplitLine(newCode, gcline.nNumber, gcline.motionMode, (float)lastActualX, (float)lastActualY, (float)gcline.actualPos.X, (float)gcline.actualPos.Y, maxStep, true, gcline.codeLine);
                        }
                        else
                        { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }
                    }
                    else
                    { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }

                }
                #endregion
                // if no replacement, stitch code together
                else
                {
                    if (gcline.isNoMove)
                    {
                        newCode.AppendLine(gcline.codeLine.Trim('\r', '\n') + infoCode);    // add orignal code-line
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
                            gcline.spindleSpeed = 0;                //  reset old speed
                            spindleSpeed = -1;                      // force output
                            if (gcline.spindleState < 5)            // if spindle on
                                forceM = true;
                        }

                        if (gcline.z != null)
                        {
                            if (specialCmd == ConvertMode.Nothing)               //  !removeZ
                            { tmpCode.AppendFormat(" Z{0}", Gcode.FrmtNum((double)gcline.z)); }
                            else if ((specialCmd == ConvertMode.ConvertZToS) && (convertMinZ != 0))
                            {
                                double convertTmp = (double)gcline.z * convertSpeedRange / convertMinZ + convertMinSpeed;
                                if (gcline.z > 0)
                                    convertTmp = convertOffSpeed;
                                gcline.spindleSpeed = (int)convertTmp;
                                //tmpCode.AppendFormat(" S{0}", Math.Round(convertTmp));
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
                            if ((gcline.motionMode > 0) && (feedRate != gcline.feedRate) && (getCoordinateXY || getCoordinateZ)) //((getCoordinateXY && !getCoordinateZ) || (!getCoordinateXY && getCoordinateZ)))
                            { tmpCode.AppendFormat(" F{0,0}", gcline.feedRate); }       // feed
                            if (spindleState != gcline.spindleState)
                            { tmpCode.AppendFormat(" M{0,0}", gcline.spindleState); }   // state
                            if (spindleSpeed != gcline.spindleSpeed)
                            { tmpCode.AppendFormat(" S{0,0}", gcline.spindleSpeed); }   // speed
                            if (coolantState != gcline.coolantState)
                            { tmpCode.AppendFormat(" M{0,0}", gcline.coolantState); }   // state

                            tmpCode.Replace(',', '.');
                            if (gcline.codeLine.IndexOf("(Setup - GCode") > 1)  // ignore coordinates from setup footer
                                newCode.AppendLine(gcline.codeLine);
                            else
                            {
                                // newCode.AppendLine(gcline.otherCode + "G" + gcode.frmtCode(gcline.motionMode) + tmpCode.ToString() + infoCode);
                                if (gcline.nNumber >= 0)
                                    newCode.AppendLine("N" + gcline.nNumber + " " + gcline.otherCode + "G" + Gcode.FrmtCode(gcline.motionMode) + tmpCode.ToString() + infoCode);
                                else
                                    newCode.AppendLine(gcline.otherCode + "G" + Gcode.FrmtCode(gcline.motionMode) + tmpCode.ToString() + infoCode);
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
                                newCode.AppendLine(gcline.codeLine.Trim('\r', '\n') + infoCode);    // add orignal code-line
                        }
                        //     lastMotionMode = gcline.motionMode;
                    }
                }
                feedRate = gcline.feedRate;
                spindleSpeed = gcline.spindleSpeed;
                spindleState = gcline.spindleState;
                coolantState = gcline.coolantState;
                lastActualX = gcline.actualPos.X; lastActualY = gcline.actualPos.Y; lastActualZ = gcline.actualPos.Z;

                if ((!hide_code) && (!gcline.ismachineCoordG53) && (gcline.codeLine.IndexOf("(Setup - GCode") < 1)) // ignore coordinates from setup footer
                {
                    if (!((gcline.actualPos.X == Grbl.posWork.X) && (gcline.actualPos.Y == Grbl.posWork.Y)))            // don't add actual tool pos
                        xyzSize.SetDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);
                }

                isArc = ((gcline.motionMode == 2) || (gcline.motionMode == 3));
                coordList.Add(new CoordByLine(iCode, gcline.figureNumber, (XyzPoint)gcline.actualPos, gcline.alpha, isArc));
            }
            return newCode.ToString().Replace(',', '.');
        }

    }
}