/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
/*  GCodeVisuAndTransform.cs
    Scaling, Rotation, Remove OffsetXY, Mirror X or Y
    During transformation the drawing path will be generated, because cooridantes are already parsed.
    Return transformed GCode 
*/
/* 2020-01-13 convert GCodeVisuAndTransform to a static class
 * 2020-02-18 extend simulation for tangetial angle
 * 2020-03-09 bug-fix simulation of G2/3 code without tangential line 525
 * 2020-04-04 bug-fix simulation of G2/3 code without tangential line 518 - never ending rotation
 * 2020-07-22 show ProcessedPath
 * 2020-07-25 bug fix in simulaation -> only addArc if r > 0
 * 2020-08-13 bug fix transformGCodeMirror with G91 
 * 2021-07-12 code clean up / code quality
 * 2022-01-17 process more than one figures (e.g. selected group) for scaling, rotation, move
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GrblPlotter
{
    internal static partial class VisuGCode
    {
        public enum Translate { None, ScaleX, ScaleY, Offset1, Offset2, Offset3, Offset4, Offset5, Offset6, Offset7, Offset8, Offset9, MirrorX, MirrorY, MirrorRotary };

        private static bool XyMove(GcodeByLine tmp)
        { return ((tmp.x != null) || (tmp.y != null)); }

        private static bool SameXYPos(GcodeByLine tmp1, GcodeByLine tmp2)
        {
            return ((tmp1.x == tmp2.x) && (tmp1.y == tmp2.y) && (tmp1.z == tmp2.z));
        }

        internal static XyPoint GetCenterOfMarkedFigure()
        {
            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
            return new XyPoint((double)centerX, (double)centerY);
        }


        /// <summary>
        /// CmsPicBoxReverseSelectedPath_Click
        /// </summary>
        public static string TransformPathReverseDirection()
        {
            return CreateGCodeProg();
        }

        /// <summary>
        /// CmsPicBoxRotateSelectedPath_Click
        /// </summary>
        public static string TransformPathRotate()
        {
            return CreateGCodeProg();
        }

        /// <summary>
        /// mirror gcode
        /// </summary>
        public static string TransformGCodeMirror(Translate shiftToZero = Translate.MirrorX)
        {
            Logger.Debug("..transformGCodeMirror {0}", shiftToZero);
            if (gcodeList == null) return "";

            XyPoint centerOfFigure = xyzSize.GetCenter();		// center of whole graphics
            if (lastFigureNumber > 0)
                centerOfFigure = GetCenterOfMarkedFigure();		// center of selected figure

            oldLine.ResetAll(Grbl.posWork);         			// reset coordinates and parser modes
            ClearDrawingPath();                    			// reset path, dimensions

            bool offsetApplied = false;
            double lastAbsPos = 0;

            foreach (GcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))	// process all graphics or selected figure
                {
                    if (!gcline.isdistanceModeG90 && offsetApplied)         // correct relative movement of next figure
                    {
                        if ((gcline.motionMode == 0) && ((gcline.x != null) || (gcline.y != null)))
                        {
                            if (shiftToZero == Translate.MirrorX)
                            { gcline.x = gcline.actualPos.X - lastAbsPos; }
                            if (shiftToZero == Translate.MirrorY)
                            { gcline.y = gcline.actualPos.Y - lastAbsPos; }
                            offsetApplied = false;
                        }
                    }
                    continue;
                }

                if (!gcline.ismachineCoordG53)
                {
                    // switch circle direction
                    if ((shiftToZero == Translate.MirrorX) || (shiftToZero == Translate.MirrorY))	// mirror xy 
                    {
                        if (gcline.motionMode == 2) { gcline.motionMode = 3; }
                        else if (gcline.motionMode == 3) { gcline.motionMode = 2; }
                    }
                    if (shiftToZero == Translate.MirrorX)           // mirror x
                    {
                        if (gcline.x != null)
                        {
                            if (gcline.isdistanceModeG90)
                                gcline.x = -gcline.x + 2 * centerOfFigure.X;
                            else
                            {
                                gcline.x = -gcline.x;
                                if ((gcline.motionMode == 0) && !offsetApplied)     // at relative movement, apply offset only once on G0
                                {
                                    gcline.x = -gcline.x + 2 * (centerOfFigure.X - gcline.actualPos.X);
                                    offsetApplied = true;
                                }
                                lastAbsPos = -gcline.actualPos.X + 2 * centerOfFigure.X;
                            }
                        }
                        gcline.i = -gcline.i;
                    }
                    if (shiftToZero == Translate.MirrorY)           // mirror y
                    {
                        if (gcline.y != null)
                        {
                            if (gcline.isdistanceModeG90)
                                gcline.y = -gcline.y + 2 * centerOfFigure.Y;
                            else
                            {
                                gcline.y = -gcline.y;
                                if ((gcline.motionMode == 0) && !offsetApplied)
                                {
                                    gcline.y = -gcline.y + 2 * (centerOfFigure.Y - centerOfFigure.Y);
                                    offsetApplied = true;
                                }
                                lastAbsPos = -gcline.actualPos.Y + 2 * centerOfFigure.Y;
                            }
                        }
                        gcline.j = -gcline.j;
                    }
                    if (shiftToZero == Translate.MirrorRotary)           // mirror rotary
                    {
                        string rotary = Properties.Settings.Default.ctrl4thName;
                        if ((rotary == "A") && (gcline.a != null)) { gcline.a = -gcline.a; }
                        else if ((rotary == "B") && (gcline.b != null)) { gcline.b = -gcline.b; }
                        else if ((rotary == "C") && (gcline.c != null)) { gcline.c = -gcline.c; }
                    }

                    CalcAbsPosition(gcline, oldLine);
                    oldLine = new GcodeByLine(gcline);   // get copy of newLine
                }
            }
            return CreateGCodeProg();
        }

        /// <summary>
        /// rotate and scale arround offset
        /// </summary>
        internal static string TransformGCodeRotate(double angle, double scale, XyPoint offset, bool calcCenter = true)
        {
            Logger.Debug("Rotate angle: {0}", angle);
            if (gcodeList == null) return "";
            XyPoint centerOfFigure = xyzSize.GetCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = GetCenterOfMarkedFigure();

            if (calcCenter)
                offset = centerOfFigure;

            double? newvalx, newvaly, newvali, newvalj;
            oldLine.ResetAll(Grbl.posWork);         // reset coordinates and parser modes
            ClearDrawingPath();                    // reset path, dimensions
            bool offsetApplied = false;
            double lastAbsPosX = 0;
            double lastAbsPosY = 0;

            if (lastFigureNumbers.Count == 0)
                lastFigureNumbers.Add(lastFigureNumber);

            foreach (GcodeByLine gcline in gcodeList)
            {
                // if only a single figure is marked to be rotated
                // and motion mode is 91, the relative position to the next (not rotated) figure must be adapted
                //    if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))
                if ((lastFigureNumber > 0) && !lastFigureNumbers.Contains(gcline.figureNumber))
                {
                    if (!gcline.isdistanceModeG90 && offsetApplied)         // correct relative movement of next figure
                    {
                        if ((gcline.motionMode == 0) && ((gcline.x != null) || (gcline.y != null)))
                        {
                            gcline.x = gcline.actualPos.X - lastAbsPosX;
                            gcline.y = gcline.actualPos.Y - lastAbsPosY;
                            offsetApplied = false;
                        }
                    }
                    continue;
                }

                if (!gcline.ismachineCoordG53)
                {
                    if ((gcline.x != null) || (gcline.y != null))
                    {

                        newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                        newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                        if (gcline.isdistanceModeG90)	// absolute
                        {
                            //                            newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            //                            newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                            gcline.x = (newvalx * scale) + offset.X;
                            gcline.y = (newvaly * scale) + offset.Y;
                        }
                        else
                        {
                            if ((gcline.motionMode == 0) && !offsetApplied)
                            {
                                //                                newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                                //                                newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                                gcline.x = gcline.x + ((newvalx * scale) + offset.X) - gcline.actualPos.X;
                                gcline.y = gcline.y + ((newvaly * scale) + offset.Y) - gcline.actualPos.Y;
                                if ((gcline.x != null) && (gcline.y != null))
                                    offsetApplied = true;
                            }
                            else
                            {
                                //  newvalx = gcline.x * Math.Cos(angle * Math.PI / 180) - gcline.y * Math.Sin(angle * Math.PI / 180);
                                //  newvaly = gcline.x * Math.Sin(angle * Math.PI / 180) + gcline.y * Math.Cos(angle * Math.PI / 180);
                                gcline.x = gcline.x * Math.Cos(angle * Math.PI / 180) - gcline.y * Math.Sin(angle * Math.PI / 180);//newvalx;
                                gcline.y = gcline.x * Math.Sin(angle * Math.PI / 180) + gcline.y * Math.Cos(angle * Math.PI / 180);//newvaly;
                            }
                            //                            newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            //                            newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                            lastAbsPosX = ((double)newvalx * scale) + offset.X; //gcline.actualPos.X;
                            lastAbsPosY = ((double)newvaly * scale) + offset.Y; //gcline.actualPos.Y;
                        }
                    }
                    if ((gcline.i != null) || (gcline.j != null))
                    {
                        newvali = (double)gcline.i * Math.Cos(angle * Math.PI / 180) - (double)gcline.j * Math.Sin(angle * Math.PI / 180);
                        newvalj = (double)gcline.i * Math.Sin(angle * Math.PI / 180) + (double)gcline.j * Math.Cos(angle * Math.PI / 180);
                        gcline.i = newvali * scale;
                        gcline.j = newvalj * scale;
                    }
                    if (tangentialAxisEnable)
                    {
                        if ((tangentialAxisName == "C") && (gcline.c != null)) { gcline.c += angle; }
                        else if ((tangentialAxisName == "B") && (gcline.b != null)) { gcline.b += angle; }
                        else if ((tangentialAxisName == "A") && (gcline.a != null)) { gcline.a += angle; }
                        else if ((tangentialAxisName == "Z") && (gcline.z != null)) { gcline.z += angle; }
                    }

                    //       calcAbsPosition(gcline, oldLine);
                    //       oldLine = new gcodeByLine(gcline);   // get copy of newLine
                }
            }
            pathBackground.Reset();
            return CreateGCodeProg();
        }

        /// <summary>
        /// scale x and y seperatly in %
        /// </summary>
        public static string TransformGCodeScale(double scaleX, double scaleY)
        {
            XyPoint centerOfFigure = xyzSize.GetCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = GetCenterOfMarkedFigure();
            return TransformGCodeScale(scaleX, scaleY, centerOfFigure);
        }
        public static string TransformGCodeScale(double scaleX, double scaleY, XyPoint centerOfFigure)
        {
            Logger.Debug("Scale scaleX: {0}, scale Y: {1}", scaleX, scaleY);
            if (gcodeList == null) return "";

            double factor_x = scaleX / 100;
            double factor_y = scaleY / 100;
            bool offsetApplied = false;
            double lastAbsPosX = 0;
            double lastAbsPosY = 0;

            oldLine.ResetAll(Grbl.posWork);         // reset coordinates and parser modes
            ClearDrawingPath();                    // reset path, dimensions

            if (lastFigureNumbers.Count == 0)
                lastFigureNumbers.Add(lastFigureNumber);

            foreach (GcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && lastFigureNumbers.Contains(gcline.figureNumber))
                {   if (gcline.motionMode > 1)
                    {
                        Logger.Warn("TransformGCodeScale found G2/G3 command, set scaleX=scaleY");
                        factor_x = factor_y = Math.Max(factor_x, factor_y);
                        break;
                    }
                }
            }

            foreach (GcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && !lastFigureNumbers.Contains(gcline.figureNumber))//(gcline.figureNumber != lastFigureNumber)) 
                {
                    if (!gcline.isdistanceModeG90 && offsetApplied)         // correct relative movement of next figure
                    {
                        if ((gcline.motionMode == 0) && ((gcline.x != null) || (gcline.y != null)))
                        {
                            gcline.x = gcline.actualPos.X - lastAbsPosX;
                            gcline.y = gcline.actualPos.Y - lastAbsPosY;
                            offsetApplied = false;
                        }
                    }
                    continue;
                }

                if (!gcline.ismachineCoordG53)
                {
                    if (gcline.isdistanceModeG90)           // absolute move: apply offset to any XY position
                    {
                        if (gcline.x != null)
                            gcline.x = gcline.x * factor_x - centerOfFigure.X * (factor_x - 1);
                        if (gcline.y != null)
                            gcline.y = gcline.y * factor_y - centerOfFigure.Y * (factor_y - 1);
                    }
                    else
                    {   //if (!offsetApplied)                 // relative move: apply offset only once
                        if ((gcline.motionMode == 0) && !offsetApplied)
                        {
                            if (gcline.x != null)
                                gcline.x -= (centerOfFigure.X - gcline.actualPos.X) * (factor_x - 1);
                            if (gcline.y != null)
                                gcline.y -= (centerOfFigure.Y - gcline.actualPos.Y) * (factor_y - 1);
                            if ((gcline.x != null) && (gcline.y != null))
                                offsetApplied = true;
                        }
                        else
                        {
                            if (gcline.x != null)
                                gcline.x *= factor_x;
                            if (gcline.y != null)
                                gcline.y *= factor_y;
                        }
                        lastAbsPosX = gcline.actualPos.X * factor_x - centerOfFigure.X * (factor_x - 1);
                        lastAbsPosY = gcline.actualPos.Y * factor_y - centerOfFigure.Y * (factor_y - 1);
                    }

                    if (gcline.i != null)
                        gcline.i *= factor_x;
                    if (gcline.j != null)
                        gcline.j *= factor_y;

                    CalcAbsPosition(gcline, oldLine);
                    oldLine = new GcodeByLine(gcline);   // get copy of newLine
                }
            }
            pathBackground.Reset();
            return CreateGCodeProg();
        }

        public static string TransformGCodeOffset(double tx, double ty, Translate shiftToZero)
        {
            Logger.Debug("Transform X: {0}, Y: {1}, Offset: {2}", tx, ty, shiftToZero);
            if (gcodeList == null) return "";
            if ((lastFigureNumber <= 0) || (!(shiftToZero == Translate.None)))
            { pathMarkSelection.Reset(); lastFigureNumber = -1; }
            double offsetX = 0;
            double offsetY = 0;
            bool offsetApplied = false;
            bool noInsertNeeded = false;

            oldLine.ResetAll(Grbl.posWork);         // reset coordinates and parser modes

            GetTransaltionOffset(ref offsetX, ref offsetY, tx, ty, shiftToZero);

            if (modal.containsG91)    // relative move: insert rapid movement before pen down, to be able applying offset
            {
                newLine.ResetAll();
                int i, k;
                bool foundG91 = false;
                for (i = 0; i < gcodeList.Count; i++)       // find first relative move
                {
                    if ((!gcodeList[i].isdistanceModeG90) && (!gcodeList[i].isSubroutine) && (gcodeList[i].motionMode == 0) && (gcodeList[i].z != null))
                    { foundG91 = true; break; }
                }
                if (foundG91)
                {
                    for (k = i + 1; k < gcodeList.Count; k++)   // find G0 x y
                    {
                        if ((gcodeList[k].motionMode == 0) && (gcodeList[k].x != null) && (gcodeList[k].y != null))
                        { noInsertNeeded = true; break; }
                        if (gcodeList[k].motionMode > 0)
                            break;
                    }
                    if (!noInsertNeeded)
                    {
                        if ((gcodeList[i + 1].motionMode != 0) || ((gcodeList[i + 1].motionMode == 0) && ((gcodeList[i + 1].x == null) || (gcodeList[i + 1].y == null))))
                        {
                            if ((!noInsertNeeded) && (!gcodeList[i + 1].ismachineCoordG53))
                            {
                                ModalGroup tmp = new ModalGroup();
                                newLine.ParseLine(i, "G0 X0 Y0 (Insert offset movement)", ref tmp);
                                gcodeList.Insert(i + 1, newLine);
                            }
                        }
                    }
                }
            }
            bool hide_code = false;

            if (lastFigureNumbers.Count == 0)
                lastFigureNumbers.Add(lastFigureNumber);

            foreach (GcodeByLine gcline in gcodeList)
            {
                if (gcline.codeLine.Contains("%START_HIDECODE")) { hide_code = true; }
                if (gcline.codeLine.Contains("%STOP_HIDECODE")) { hide_code = false; }
                if ((!hide_code) && (!gcline.isSubroutine) && (!gcline.ismachineCoordG53) && (gcline.codeLine.IndexOf("(Setup - GCode") < 1)) // ignore coordinates from setup footer
                {
                    //   if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))    // 2019-11-30
                    if ((lastFigureNumber > 0) && !lastFigureNumbers.Contains(gcline.figureNumber))
                    { continue; }

                    if (gcline.isdistanceModeG90)           // absolute move: apply offset to any XY position
                    {
                        if (gcline.x != null)
                            gcline.x -= offsetX;      // apply offset
                        if (gcline.y != null)
                            gcline.y -= offsetY;      // apply offset
                    }
                    else
                    {
                        if (!offsetApplied)                 // relative move: apply offset only once
                        {
                            if (gcline.motionMode == 0)
                            {
                                gcline.x -= offsetX;
                                gcline.y -= offsetY;
                                if ((gcline.x != null) && (gcline.y != null))
                                    offsetApplied = true;
                            }
                        }
                    }
                    CalcAbsPosition(gcline, oldLine);
                    oldLine = new GcodeByLine(gcline);   // get copy of newLine
                }
            }
            pathBackground.Reset();
            return CreateGCodeProg();
        }
        private static void GetTransaltionOffset(ref double offsetX, ref double offsetY, double tx, double ty, Translate shiftToZero)
        {
            if (shiftToZero == Translate.Offset1) { offsetX = tx + xyzSize.minx; offsetY = ty + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == Translate.Offset2) { offsetX = tx + xyzSize.minx + xyzSize.dimx / 2; offsetY = ty + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == Translate.Offset3) { offsetX = tx + xyzSize.minx + xyzSize.dimx; offsetY = ty + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == Translate.Offset4) { offsetX = tx + xyzSize.minx; offsetY = ty + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == Translate.Offset5) { offsetX = tx + xyzSize.minx + xyzSize.dimx / 2; offsetY = ty + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == Translate.Offset6) { offsetX = tx + xyzSize.minx + xyzSize.dimx; offsetY = ty + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == Translate.Offset7) { offsetX = tx + xyzSize.minx; offsetY = ty + xyzSize.miny; }
            if (shiftToZero == Translate.Offset8) { offsetX = tx + xyzSize.minx + xyzSize.dimx / 2; offsetY = ty + xyzSize.miny; }
            if (shiftToZero == Translate.Offset9) { offsetX = tx + xyzSize.minx + xyzSize.dimx; offsetY = ty + xyzSize.miny; }
            if (shiftToZero == Translate.None) { offsetX = tx; offsetY = ty; }
        }

        public static string TransformGCodeRadiusCorrection(double radius)
        {
            Logger.Debug("..transformGCodeRadiusCorrection r: {0}", radius);
            if (gcodeList == null) return "";

            if (lastFigureNumber > 0)
            {
                pathBackground = (GraphicsPath)pathMarkSelection.Clone();
                origWCOLandMark = (XyPoint)Grbl.posWCO;
            }
            else
            {
                pathBackground = (GraphicsPath)pathPenDown.Clone();
                origWCOLandMark = (XyPoint)Grbl.posWCO;
            }

            XyPoint[] offset = new XyPoint[4];
            int i, figureStart, figure2nd, prev, act, next;
            int counter = 0, isFirst = 0;
            bool figureProcessed = false;
            bool closeFigure;
            bool endFigure = false;

            figure2nd = figureStart = prev = act = next = 0;
            int offType;

            for (i = 1; i < gcodeList.Count; i++)
            {
                if ((i == (gcodeList.Count - 1)) || ((lastFigureNumber > 0) && (gcodeList[i].figureNumber != lastFigureNumber)))  // if wrong selection, nothing to do
                {
                    if (figureProcessed)        // correct last point
                    {
                        figureProcessed = false;
                        goto ProcessesPath;
                    }
                    continue;
                }

                if (gcodeList[i].ismachineCoordG53)     // machine coordinates, do not change
                { continue; }

                if (!XyMove(gcodeList[i]))              // no xy moves, nothing to do - except G0 Z
                { continue; }

                while ((gcodeList[i].codeLine == gcodeList[i + 1].codeLine) && (i < gcodeList.Count))  // remove double lines (lff font)
                { gcodeList.RemoveAt(i + 1); }

                while (SameXYPos(gcodeList[i], gcodeList[i + 1]) && (i < (gcodeList.Count - 1)))  // remove double coordinates
                { gcodeList.RemoveAt(i + 1); }

                //                Logger.Trace("   ----- {0} -----",i);

                if (gcodeList[i].motionMode > 1)
                {
                    double tmpR = Math.Sqrt((double)gcodeList[i].i * (double)gcodeList[i].i + (double)gcodeList[i].j * (double)gcodeList[i].j);
                    bool remove = false;
                    double abs_radius = Math.Abs(radius);
                    if (gcodeList[i].motionMode == 2)
                    {
                        if ((radius < 0) && (tmpR < abs_radius)) remove = true;
                    }
                    if (gcodeList[i].motionMode == 3)
                    {
                        if ((radius > 0) && (tmpR < abs_radius)) remove = true;
                    }
                    if (remove)
                    {
                        gcodeList[i].i = null; gcodeList[i].j = null;
                        gcodeList[i].motionMode = 1;
                        Logger.Trace("   transformGCodeRadiusCorrection - Radius too small, do G1 {0}", gcodeList[act].codeLine);
                    }
                }
                figureProcessed = true;                 // must stay before jump label
                next = i;
                endFigure = false;

            ProcessesPath:
                //               gcodeList[i].info += " "+i.ToString()+" "+ figureProcessed.ToString()+" "+lastFigureNumber.ToString();
                if (counter == 0)
                { figureStart = prev = act = next; }  // preset indices

                if ((gcodeList[prev].motionMode == 0) && (gcodeList[act].motionMode >= 1))  // find start of figure
                { figureStart = prev; figure2nd = act; isFirst = 0; }//gcodeList[prev].info += " #start "; }

                if ((gcodeList[act].motionMode >= 1) && (gcodeList[next].motionMode == 0))  // find end of figure
                { endFigure = true; figureProcessed = false; }// gcodeList[act].info += " #end ";            }

                closeFigure = false;
                if (act != prev)
                {
                    XyArcPoint p1 = FillPointData(prev, prev);
                    XyArcPoint p2 = FillPointData(prev, act);

                    if ((gcodeList[act].actualPos.X == gcodeList[figureStart].actualPos.X) && (gcodeList[act].actualPos.Y == gcodeList[figureStart].actualPos.Y))
                    { next = figure2nd; closeFigure = true; }//                    gcodeList[act].info += " closefig "; }

                    //                    Logger.Trace("   {0}",gcodeList[act].codeLine);

                    offType = CreateOffsetedPath((isFirst++ == 0), (endFigure && !closeFigure), figureStart, prev, act, next, radius, ref offset);

                    //                   Logger.Trace(" typ {0} {1} {2} {3} {4} {5} ", offType, endFigure, figureStart, prev, act, next);

                    if (closeFigure)// && !endFigure)
                    {

                        //                      Logger.Trace(" close Figure {0:0.00} {1:0.00}  ", offset[2].X, offset[2].Y);

                        gcodeList[figureStart].x = offset[2].X; gcodeList[figureStart].y = offset[2].Y;    // close figure
                        if (gcodeList[figure2nd].motionMode > 1)    // act
                        {
                            bool isFullCircle = ((p1.X == p2.X) && (p1.Y == p2.Y));
                            if (!isFullCircle)
                            {
                                XyArcPoint p3 = FillPointData(prev, act); //fillPointData(act, next);
                                gcodeList[figure2nd].i = p3.CX - offset[2].X;
                                gcodeList[figure2nd].j = p3.CY - offset[2].Y;   // offset radius

                                //                               Logger.Trace(" correct Arc center of f2nd {0} origX {1:0.00} origY {2:0.00}  ", figure2nd, p3.CX, p3.CY);

                            }
                        }
                    }
                    next = i;   // restore next
                    if (offType >= 1)           // arc or line was inserted to connect points
                    { act++; next++; counter++; i++; }    // inc. counters
                }

                prev = act; act = next; counter++;

                if (endFigure)
                { prev = act; figureStart = act = i; }  // preset indices

                if (closeFigure)
                { isFirst = 0; }
            }
            return CreateGCodeProg();
        }

        private static bool SameSign(double? a, double? b)
        {
            if (double.IsNaN((double)a) || double.IsNaN((double)b))
                return false;
            if ((a.HasValue && b.HasValue))
                return (Math.Sign((double)a) == Math.Sign((double)b));
            else
                return false;
        }

        // calculate and apply offset for given coordinates in gcodeList[prev,act,next] (which should have xy moves)
        internal static int CreateOffsetedPath(bool isFirst, bool isEnd, int iInitial, int prev, int act, int next, double radius, ref XyPoint[] offset)
        {
            XyArcPoint p1 = FillPointData(prev, prev);
            XyArcPoint p2 = FillPointData(prev, act);
            XyArcPoint p3 = FillPointData(act, next);

            bool isArc = (gcodeList[act].motionMode > 1);
            bool isFullCircle = ((p1.X == p2.X) && (p1.Y == p2.Y));
            int offsetType = Crc.GetPointOffsets(ref offset, p1, p2, p3, radius, isEnd);

            //           Logger.Trace(" offset typ{0} x{1:0.000} y{2:0.000}", offsetType, offset[1].X, offset[1].Y);

            /*       if (offsetType == -2)   // intersection not successfull
                   {
                       gcodeList[act].motionMode = 1; gcodeList[act].x = null; gcodeList[act].y = null;    // clear move
                       p2 = p3; p3 = fillPointData(next, next+1);      // next+1 not save
                       offsetType = crc.getPointOffsets(ref offset, p1, p2, p3, radius, isEnd);
       #if (debuginfo)
                       log.Add(string.Format(" redo offset x{0:0.000} y{1:0.000}", offset[1].X, offset[1].Y));
       #endif
                   }*/

            if (isArc && !isFullCircle)   // replace arc by line, if start and end-point are too close
            {
                double dist = offset[0].DistanceTo(offset[1]);
                //     double a1 = offset[0].AngleTo(offset[1]);
                //      double a2 = ((XyPoint)p1).AngleTo((XyPoint)p2);
                if (dist < 0.1)
                {
                    gcodeList[act].motionMode = 1;
                    gcodeList[act].i = null;
                    gcodeList[act].j = null;
                }
            }

            if (isFirst)                   // offset 1st point
            {
                gcodeList[iInitial].x = offset[0].X; gcodeList[iInitial].y = offset[0].Y;    // offset 1st point
            }

            gcodeList[act].x = offset[1].X; gcodeList[act].y = offset[1].Y;         // offset point

            //          Logger.Trace(" createOffsetedPath Offset x{0:0.000} y{1:0.000}", gcodeList[act].x, gcodeList[act].y);

            if (isArc)                                                              // offset radius     
            {
                double iNew = p2.CX - (double)gcodeList[prev].x;
                double jNew = p2.CY - (double)gcodeList[prev].y; ;
                gcodeList[act].i = iNew; gcodeList[act].j = jNew;      // offset radius    

                if (!SameSign(gcodeList[act].i, iNew) || !SameSign(gcodeList[act].j, jNew)) // radius now negative
                { }

                //            if ((gcodeList[act].i == 0) && (gcodeList[act].j == 0)) // radius = 0, command not needed
                //             {   gcodeList[act].motionMode = 1; gcodeList[act].i = null; gcodeList[act].j = null;}
            }

            if (offsetType >= 1)     // insert arc to connect lines
            {
                int insert = act + 1;
                gcodeList.Insert(insert, new GcodeByLine(gcodeList[act]));             // insert a copy of actual move
                gcodeList[insert].x = offset[2].X; gcodeList[insert].y = offset[2].Y;   // set end-pos.

                double dist = offset[1].DistanceTo(offset[2]);                          // if distance great enough use arc
                if (dist > 0.1)      // make arc, if start and end-point are not too close
                {
                    gcodeList[insert].motionMode = (byte)((radius > 0) ? 2 : 3);
                    gcodeList[insert].i = gcodeList[insert].actualPos.X - offset[1].X;
                    gcodeList[insert].j = gcodeList[insert].actualPos.Y - offset[1].Y;
                }
                else
                    gcodeList[insert].motionMode = 1;

                //             Logger.Trace(" createOffsetedPath Insert G{0} x{1:0.000} y{2:0.000}", gcodeList[insert].motionMode, gcodeList[insert].x, gcodeList[insert].y);

            }
            return offsetType;
        }

        private static XyArcPoint FillPointData(int prevLine, int tmpLine)
        {
            XyArcPoint tmpPoint;
            tmpPoint = (XyArcPoint)gcodeList[tmpLine].actualPos;
            if ((tmpPoint.mode = gcodeList[tmpLine].motionMode) > 1)
            { tmpPoint.CX = gcodeList[prevLine].actualPos.X + (double)gcodeList[tmpLine].i; tmpPoint.CY = gcodeList[prevLine].actualPos.Y + (double)gcodeList[tmpLine].j; }
            return tmpPoint;
        }


        /// <summary>
        /// Calculate equidistant line = cutter radius compensation
        /// </summary>
        public static class Crc   // cutter radius compensation
        {   // http://www.hinterseher.de/Diplomarbeit/GeometrischeFunktionen.html

            // Trace, Debug, Info, Warn, Error, Fatal
            private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
            private static readonly bool logenable = ((uint)Properties.Settings.Default.importLoggerSettings & 32) > 0;

            // get two connected lines and calc offsetted points
            // P0 start of 1st line, P1 end of first / start of 2nd line, P2 end of 2nd line
            internal static int GetPointOffsets(ref XyPoint[] offset, XyArcPoint P0, XyArcPoint P1, XyArcPoint P2, double distance, bool isEnd)
            {
                XyPoint[] S1off = new XyPoint[2];
                XyPoint[] S2off = new XyPoint[2];

                double a0, a1, a2, a3, adelta;
                double newRadius1 = distance;
                double newRadius2 = distance;

                if (P1.mode <= 1)   // is a line
                {
                    a1 = GetAlphaLine(P0, P1);
                    a0 = a1;
                    CalcOffsetLine(ref S1off, P0, P1, a1 + Math.PI / 2, distance); // offset by 90°
                }
                else
                {
                    a0 = GetAlphaCenterToPoint(P1, P0);    // from center to start
                    a1 = GetAlphaCenterToPoint(P1, P1);    // from center to end

                    double usea0 = a0, usea1 = a1;
                    a0 -= Math.PI / 2; a1 -= Math.PI / 2;   // tangente

                    if (P1.mode == 3)
                    {
                        usea0 += Math.PI; usea1 += Math.PI; // add 180°
                        a0 += Math.PI; a1 += Math.PI;        // tangente reverse
                    }

                    S1off[0] = CalcOffsetPoint(P0, usea0, distance); // extend radius
                    S1off[1] = CalcOffsetPoint(P1, usea1, distance); // extend radius
                    newRadius1 = Math.Sqrt((P1.CX - S1off[0].X) * (P1.CX - S1off[0].X) + (P1.CY - S1off[0].Y) * (P1.CY - S1off[0].Y));
                }
                offset[0] = S1off[0];
                offset[1] = S1off[1];

                //         Logger.Trace(" getPointOffsets P0-P1: P1mode: {0} S1offX {1:0.000} S1offX {2:0.000} S1offX {3:0.000} S1offX {4:0.000}", P1.mode, S1off[0].X, S1off[0].Y, S1off[1].X, S1off[1].Y);

                if (P2.mode <= 1)   // is a line
                {
                    a2 = GetAlphaLine(P1, P2);
                    //        a3 = a2;
                    CalcOffsetLine(ref S2off, P1, P2, a2 + Math.PI / 2, distance); // offset by 90°
                }
                else
                {
                    a2 = GetAlphaCenterToPoint(P2, P1);   // from center to start
                    a3 = GetAlphaCenterToPoint(P2, P2);   // from center to end
                    double usea2 = a2, usea3 = a3;
                    a2 -= Math.PI / 2; //a3 -= Math.PI / 2;   // tangente

                    if (P2.mode == 3)
                    {
                        usea2 += Math.PI; usea3 += Math.PI;     // add 180°
                        a2 += Math.PI; //a3 += Math.PI;        // tangente reverse
                    }

                    S2off[0] = CalcOffsetPoint(P1, usea2, distance); // extend radius
                    S2off[1] = CalcOffsetPoint(P2, usea3, distance); // extend radius
                    newRadius2 = Math.Sqrt((P2.CX - S2off[0].X) * (P2.CX - S2off[0].X) + (P2.CY - S2off[0].Y) * (P2.CY - S2off[0].Y));
                }
                offset[2] = S2off[0];
                offset[3] = S2off[1];

                if (logenable) Logger.Trace(" getPointOffsets P1-P2: P2mode: {0} S1offX {1:0.000} S1offX {2:0.000} S1offX {3:0.000} S1offX {4:0.000}", P2.mode, S2off[0].X, S2off[0].Y, S2off[1].X, S2off[1].Y);

                if ((P1.mode == P2.mode) && (P1.X == P2.X) && (P1.Y == P2.Y))
                { a2 = a0; }//                a3 = a1; }

                // compare angle of both lines P0-P1 and P1-P2
                adelta = a2 - a1;
                double dist = offset[1].DistanceTo(offset[2]);

                if (logenable) Logger.Trace(" getPointOffsets Angles: a1 {0:0.000} a2 {1:0.000} delta {2:0.000}", (a1 * 180 / Math.PI), (a2 * 180 / Math.PI), (adelta * 180 / Math.PI));

                if (adelta >= (Math.PI))
                    adelta -= 2 * Math.PI;
                if (adelta <= -(Math.PI))
                    adelta += 2 * Math.PI;

                if (logenable) Logger.Trace(" getPointOffsets adelta corrected {0:0.000}", (adelta * 180 / Math.PI));
                if (logenable) Logger.Trace(" getPointOffsets offset [0]x{0:0.000} [0]y{1:0.000} [1]x{2:0.000} [1]y{3:0.000} [2]x{4:0.000} [2]y{5:0.000}", offset[0].X, offset[0].Y, offset[1].X, offset[1].Y, offset[2].X, offset[2].Y);

                if (isEnd || (Math.Abs(adelta) <= 0.2) || (dist < 0.2))
                    return 0;           // S1-angle == S2-angle, no correction needed

                if (Math.Abs(Math.Abs(adelta) - Math.PI) <= 0.2)
                    return 1;           // 180°

                if ((adelta > 0) && (distance < 0))
                    return 1;           // connect lines with additional arc
                if ((adelta < 0) && (distance > 0))
                    return 1;           // connect lines with additional arc

                if (logenable) Logger.Trace(" getPointOffsets Find intersection {0} {1}", P1.mode, P2.mode);

                bool result = false;
                // find common intersection
                if ((P1.mode <= 1) && (P2.mode <= 1))   // line to line
                {   // https://www.java-forum.org/thema/algorithmus-fuer-pruefung-auf-ueberschneidende-linien.117102/
                    double d = (S1off[1].X - S1off[0].X) * (S2off[0].Y - S2off[1].Y) - (S2off[0].X - S2off[1].X) * (S1off[1].Y - S1off[0].Y);
                    if (d == 0)
                    { offset[2] = offset[1] = offset[3]; }
                    else
                    {
                        double m = ((S2off[0].X - S1off[0].X) * (S2off[0].Y - S2off[1].Y) - (S2off[0].X - S2off[1].X) * (S2off[0].Y - S1off[0].Y)) / d;
                        //            double n = ((S1off[1].X - S1off[0].X) * (S2off[0].X - S1off[0].X) - (S2off[0].Y - S1off[0].Y) * (S1off[1].Y - S1off[0].Y)) / d;
                        offset[1].X = S1off[0].X + m * (S1off[1].X - S1off[0].X);
                        offset[1].Y = S1off[0].Y + m * (S1off[1].Y - S1off[0].Y);
                        offset[2] = offset[1];
                    }
                }
                else if ((P1.mode <= 1) && (P2.mode >= 2))    // 1st line then arc
                {
                    result = CalcIntersectionLineArc(ref offset, S1off, P2, newRadius2);
                }
                else if ((P1.mode >= 2) && (P2.mode <= 1))      // 1st arc then line
                {
                    XyPoint tmp = S2off[0];
                    S2off[0] = S2off[1]; S2off[1] = tmp; // switch points, p[1] should be connection to arc
                    result = CalcIntersectionLineArc(ref offset, S2off, P1, newRadius1);
                }
                else
                {   // 1st arc 2nd arc, transfer one arc to line to use available function calcIntersectionLineArc
                    // http://www2.math.uni-wuppertal.de/~volkert/Das%20Apollonische%20Beruehrproblem,%202007.pdf
                    double dy = P2.CY - P1.CY;
                    double dx = P2.CX - P1.CX;
                    if (dy == 0)        // center points of arcs on same y -> chordale = vertical line
                    {
                        double a = (newRadius2 * newRadius2 - newRadius1 * newRadius1 - dx * dx) / (-2 * dx);
                        double px = P1.CX + a;  // vertical line
                        S1off[0].X = S1off[1].X = px;
                        S1off[0].Y = -P1.Y;
                        S1off[1].Y = P1.Y;
                        result = CalcIntersectionLineArc(ref offset, S1off, P2, newRadius2);
                    }
                    else if (dx == 0)   // center points of arcs on same x -> chordale = horizontal line
                    {
                        double a = (newRadius2 * newRadius2 - newRadius1 * newRadius1 - dy * dy) / (-2 * dy);
                        double py = P1.CY + a;  // horizontal line
                        S1off[0].Y = S1off[1].Y = py;
                        S1off[0].X = -P1.X;
                        S1off[1].X = P1.X;
                        result = CalcIntersectionLineArc(ref offset, S1off, P2, newRadius2);
                    }
                    else
                    {
                        double c = Math.Sqrt(dx * dx + dy * dy);
                        double a = (newRadius2 * newRadius2 - newRadius1 * newRadius1 - c * c) / (-2 * c);
                        //          double m = dy / dx;
                        double angle = GetAlphaCenterToCenter(P1, P2);
                        XyPoint aP = CalcOffsetPoint(new XyPoint(P1.CX, P1.CY), angle, a);
                        angle += Math.PI / 2;

                        S1off[0] = CalcOffsetPoint(aP, angle, newRadius1);  // create line from point
                        S1off[1] = CalcOffsetPoint(aP, angle, -newRadius1); // create line from point
                        double d0 = S1off[0].DistanceTo((XyPoint)P1);
                        double d1 = S1off[1].DistanceTo((XyPoint)P1);

                        if (d1 > d0)    // index 1 should be closer to final pos
                        {
                            S1off[1] = CalcOffsetPoint(aP, angle, newRadius1);  // create line from point
                            S1off[0] = CalcOffsetPoint(aP, angle, -newRadius1); // create line from point
                        }
                        result = CalcIntersectionLineArc(ref offset, S1off, P2, newRadius2);
                    }

                    if ((double.IsNaN(offset[1].X)) || double.IsNaN(offset[1].Y))
                    {
                        offset[1].X = 0; offset[1].Y = 0;
                        offset[2] = offset[1];
                    }
                }
                if (result == true) // intersection successful
                    return -1;
                else
                    return -2;
            }

            private static bool CalcIntersectionLineArc(ref XyPoint[] resultOffset, XyPoint[] linePoint, XyArcPoint arc, double radius) // return success
            {   // circular equation: r^2 = (x-xm)^2 + (y - ym)^2 = r^2    =>    y = ym ± √(r2 - (x-xm)2) 
                // linear equation: y = m*x + n 
                double x, x1, x2, y, y1, y2;
                double dx = (linePoint[1].X - linePoint[0].X);
                double dy = (linePoint[1].Y - linePoint[0].Y);

                if (logenable) Logger.Trace("   calcIntersectionLineArc 0x {0:0.00} 0y {1:0.00} 1x {2:0.00} 1y {3:0.00} Arcx {4:0.00} Arcy {5:0.00} ArcCx {6:0.00} ArcCy {7:0.00}", linePoint[0].X, linePoint[0].Y, linePoint[1].X, linePoint[1].Y, arc.X, arc.Y, arc.CX, arc.CY);
                if (logenable) Logger.Trace("   dx {0:0.00} dy {1:0.00} ", dx, dy);

                if (dx == 0)        // vertical line, x is known
                {
                    double a2minusb2 = GetA2minusB2(radius, (linePoint[0].X - arc.CX));
                    if (a2minusb2 >= 0)
                    {
                        double tmpRoot = Math.Sqrt(a2minusb2);//  getRoot(radius, (linePoint[0].X - arc.CX));
                        y1 = arc.CY + tmpRoot;   // y = ym ± √(r2 - (x-xm)2) 
                        y2 = arc.CY - tmpRoot;
                        y = y1;
                        if (Math.Abs(linePoint[1].Y - y2) < Math.Abs(linePoint[1].Y - y1))  // find closer point
                            y = y2;
                        resultOffset[1].X = linePoint[0].X;
                        resultOffset[1].Y = y;
                        resultOffset[2] = resultOffset[1];

                        if (logenable) Logger.Trace("   intersection at x{0:0.000} y{1:0.000}", resultOffset[1].X, resultOffset[1].Y);

                        return true;
                    }
                    else
                    {
                        resultOffset[1].X = linePoint[0].X;
                        resultOffset[1].Y = resultOffset[3].Y;// arc.Y-radius;
                        resultOffset[2] = resultOffset[1];

                        if (logenable) Logger.Trace("   no intersection! ");

                        return false;
                    }
                }

                else if (dy == 0)   // horizontal line, y is known
                {
                    double a2minusb2 = GetA2minusB2(radius, (linePoint[0].Y - arc.CY));
                    if (a2minusb2 >= 0)
                    {
                        double tmpRoot = Math.Sqrt(a2minusb2);
                        x1 = arc.CX + tmpRoot;// getRoot(radius, (linePoint[0].Y - arc.CY));   // y = ym ± √(r2 - (x-xm)2) 
                        x2 = arc.CX - tmpRoot;// getRoot(radius, (linePoint[0].Y - arc.CY));
                        x = x1;
                        if (Math.Abs(linePoint[1].X - x2) < Math.Abs(linePoint[1].X - x1))
                            x = x2;
                        resultOffset[1].X = x;
                        resultOffset[1].Y = linePoint[0].Y;
                        resultOffset[2] = resultOffset[1];

                        if (logenable) Logger.Trace("   intersection at x{0:0.000} y{1:0.000}", resultOffset[1].X, resultOffset[1].Y);

                        return true;
                    }
                    else
                    {
                        resultOffset[1].X = resultOffset[2].X;// arc.X - radius;
                        resultOffset[1].Y = linePoint[0].Y;
                        resultOffset[2] = resultOffset[1];

                        if (logenable) Logger.Trace("   no intersection! ");

                        return false;
                    }
                }
                else
                {   // intersection line-arc
                    // circular equation: r^2 = (x-xm)^2 + (y - ym)^2 = r^2    =>    y = ym ± √(r2 - (x-xm)2) 
                    // linear equation: y = m*x + n     =>   n = y - m*x
                    resultOffset[2] = resultOffset[1] = resultOffset[0];
                    double m = dy / dx;                                 // y=m*x+n
                    double n = linePoint[1].Y - m * linePoint[1].X;     // n=y-m*x
                    double a = 1 + m * m;                               // r²=(x-cx)² + (y-cy)²
                    double b = 2 * (m * n - arc.CX - arc.CY * m);       // 0=x²-2*x*cx+cx² + y²-2*y*cy+cy²
                    double c = arc.CX * arc.CX + arc.CY * arc.CY + n * n - radius * radius - 2 * arc.CY * n;
                    double a2minusb2 = GetA2minusB2((b * b), (4 * a * c));
                    if (a2minusb2 >= 0)
                    {

                        double root = Math.Sqrt((b * b) - (4 * a * c));

                        x1 = (-b + root) / (2 * a);           // ax²+bx+c=0
                        x2 = (-b - root) / (2 * a);           // x=(-b±√b²-4ac)/(2a)
                        x = x1;

                        if (Math.Abs(linePoint[1].X - x2) < Math.Abs(linePoint[1].X - x1))
                            x = x2;

                        if (logenable) Logger.Trace("   x1 {0:0.00} x2 {1:0.00} lp1x {2:0.00} x {3:0.00}", x1, x2, linePoint[1].X, x);

                        y = m * x + n;
                        resultOffset[1].X = x; resultOffset[1].Y = y;
                        resultOffset[2] = resultOffset[1];
                        return true;
                    }
                    else
                    { return false; }
                }
            }

            private static void CalcOffsetLine(ref XyPoint[] offset, XyArcPoint P0, XyArcPoint P1, double angle, double radius)
            {
                offset[0].X = P0.X + Math.Cos(angle) * radius;   // 1st point of 1st line
                offset[0].Y = P0.Y + Math.Sin(angle) * radius;
                offset[1].X = P1.X + Math.Cos(angle) * radius;   // 2nd point of 1st line
                offset[1].Y = P1.Y + Math.Sin(angle) * radius;
            }

            private static XyPoint CalcOffsetPoint(XyArcPoint P, double angle, double radius)
            { return CalcOffsetPoint(new XyPoint(P.X, P.Y), angle, radius); }
            private static XyPoint CalcOffsetPoint(XyPoint P, double angle, double radius)
            {
                XyPoint tmp = new XyPoint
                {
                    X = P.X + Math.Cos(angle) * radius,
                    Y = P.Y + Math.Sin(angle) * radius
                };
                return tmp;
            }
            private static double GetA2minusB2(double a, double b)
            { return (a * a - b * b); }
            //     private static double getRoot(double radius, double b)
            //      { return Math.Sqrt(radius * radius - b * b); }

            private static double GetAlphaLine(XyArcPoint P1, XyArcPoint P2)
            { return GetAlpha(P1.X, P1.Y, P2.X, P2.Y); }
            private static double GetAlphaCenterToPoint(XyArcPoint P1, XyArcPoint P2)
            { return GetAlpha(P1.CX, P1.CY, P2.X, P2.Y); }
            private static double GetAlphaCenterToCenter(XyArcPoint P1, XyArcPoint P2)
            { return GetAlpha(P1.CX, P1.CY, P2.CX, P2.CY); }

            private static double GetAlpha(double P1x, double P1y, double P2x, double P2y)
            {
                double s, a;
                double dx = P2x - P1x;
                double dy = P2y - P1y;
                if (dx == 0)
                {
                    if (dy > 0)
                        a = Math.PI / 2;
                    else
                        a = 3 * Math.PI / 2;
                }
                else if (dy == 0)
                {
                    if (dx > 0)
                        a = 0;
                    else
                        a = Math.PI;
                }
                else
                {
                    s = dy / dx;
                    a = Math.Atan(s);
                    if (dx < 0)
                        a += Math.PI;
                }
                return a;
            }
        }
    }
}
