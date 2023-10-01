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
/*
 * 2022-01-17 new class to process frame with handles at selected figure
 * 2022-01-21 snap on grid
 * 2022-02-21 strange zooming behavior: GetScaleFactor switch to selectionBoundsOrig
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace GrblPlotter
{
    internal static class SelectionHandle
    {
        private static readonly float handleSizeFix = 15;        // handle size in px
        private static float handleSize = 15;                   // handle size adapted on view-range and zoom
        private static readonly float handlePenWidth = 0.1f;

        private static int moveHandlePos = 0;                   // like on keyboard: 1=lower-left corner, 2=lower edge...

        private static readonly GraphicsPath pathBounds = new GraphicsPath();
        private static readonly GraphicsPath pathArrows = new GraphicsPath();
        private static readonly GraphicsPath arrow0 = new GraphicsPath();
        private static readonly GraphicsPath arrow1 = new GraphicsPath();
        private static readonly GraphicsPath arrow2 = new GraphicsPath();
        private static readonly GraphicsPath arrow3 = new GraphicsPath();

        private static RectangleF selectionBoundsOrig;      // selected figure bounds
        private static RectangleF selectionBounds;      // selected figure bounds
                                                        //    private static PointF upperEdgeLeft;          	// move
        private static PointF upperEdgeCenter;          // scale y
        private static PointF upperEdgeRight;           // scale xy
        private static PointF rightEdgeCenter;          // scale x
        private static PointF rightEdgeBottom;          // rotate
                                                        //    private static PointF lowerEdgeLeft;          	// move
                                                        //    private static PointF lowerEdgeRight;          	// move

        private static PointF nodePathBefore;
        private static PointF nodePathAfter;
        private static PointF nodePathActual;
        private static bool nodePathBeforeOk = false;
        private static bool nodePathAfterOk = false;


        internal enum Handle { None, Move, SizeX, SizeY, SizeXY, Rotate }
        internal static RectangleF Bounds { get; set; }      // selected figure bounds
        internal static XmlMarkerType SelectedMarkerType { get; set; } = XmlMarkerType.Figure;
        internal static int SelectedMarkerLine { get; set; } = 0;
        internal static int SelectedCollection { get; set; } = -1;
        internal static int SelectedTile { get; set; } = -1;
        internal static int SelectedGroup { get; set; } = -1;
        internal static int SelectedFigure { get; set; } = -1;
        internal static PointF center = new PointF();

        public static float scalingX = 1, scalingY = 1;
        //      public static float offsetX, offsetY;
        public static PointF transformPoint;
        public static float angleDeg = 0;
        public static XyPoint correctedDifference = new XyPoint();

        private static double scalingHandle = 1;

        private static bool isactive;
        public static bool IsActive  // property
        {
            get { return isactive; }
            set
            {
                isactive = value; scalingX = 1; scalingY = 1; angleDeg = 0;
                if (!isactive) { nodePathBeforeOk = nodePathAfterOk = false; }
            }
        }
        public static void ClearSelected()
        {
            SelectedCollection = -1;
            SelectedTile = -1;
            SelectedGroup = -1;
            SelectedFigure = -1;
            SelectedMarkerLine = 0;
            IsActive = false;
        }
        //    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SetBounds(RectangleF bounds, bool setActive = true)
        {
            if ((bounds.Width == 0) || (bounds.Height == 0))
            { SelectedMarkerType = XmlMarkerType.Node; }
            Bounds = bounds;
            selectionBoundsOrig = bounds;
            selectionBounds = bounds;
            SetHandlePositions(bounds);
            DrawHandles();
            IsActive = setActive;
            scalingX = 1; scalingY = 1; angleDeg = 0;
        }

        public static void SetSelectionPath(PointF pBefore, bool bBefore, PointF pActual, PointF pAfter, bool bAfter)
        {
            nodePathBefore = pBefore; nodePathBeforeOk = bBefore;
            nodePathAfter = pAfter; nodePathAfterOk = bAfter;
            nodePathActual = pActual;
            SelectedMarkerType = XmlMarkerType.Node;
            DrawHandles();
        }
        public static void UpdateSelectionPath(PointF pActual)
        {
            nodePathActual = pActual;
            DrawHandles();
        }
        private static void SetHandlePositions(RectangleF bounds)
        {
            upperEdgeCenter = new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height + 2 * handleSize);
            upperEdgeRight = new PointF(bounds.X + bounds.Width + 2 * handleSize, bounds.Y + bounds.Height + 2 * handleSize);
            rightEdgeCenter = new PointF(bounds.X + bounds.Width + 2 * handleSize, bounds.Y + bounds.Height / 2);
            rightEdgeBottom = new PointF(bounds.X + bounds.Width + 2 * handleSize, bounds.Y - 2 * handleSize);
            center = new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            //        offsetX = bounds.Left; offsetY = bounds.Top;
            transformPoint = new PointF(bounds.Left, bounds.Top);// - bounds.Height);

            //	upperEdgeLeft = new PointF(bounds.X, bounds.Y + bounds.Height);
            //	lowerEdgeLeft = new PointF(bounds.X, bounds.Y);
            //	lowerEdgeRight= new PointF(bounds.X + bounds.Width, bounds.Y);
        }
        public static Handle IsHandlePosition(XyPoint tmp)
        {
            moveHandlePos = 0;
            if (SelectedMarkerType != XmlMarkerType.Node)
            {
                if (InHandle(tmp, upperEdgeCenter, handleSize)) { return Handle.SizeY; }
                if (InHandle(tmp, upperEdgeRight, handleSize)) { return Handle.SizeXY; }
                if (InHandle(tmp, rightEdgeCenter, handleSize)) { return Handle.SizeX; }
                if (InHandle(tmp, rightEdgeBottom, handleSize)) { return Handle.Rotate; }
            }
            if (InHandle(tmp, center, handleSize)) { moveHandlePos = 5; return Handle.Move; }
            if (OnFrame(tmp, handleSize)) { return Handle.Move; }
            return Handle.None;
        }
        private static bool InHandle(XyPoint tmp, PointF p, float r)
        { return ((tmp.X > (p.X - r)) && (tmp.X < (p.X + r)) && (tmp.Y > (p.Y - r)) && (tmp.Y < (p.Y + r))); }

        private static bool OnFrame(XyPoint tmp, float r)
        {
            if ((tmp.X < (selectionBounds.X - r)) || (tmp.X > (selectionBounds.X + selectionBounds.Width + r))) return false;   // too far outside frame
            if ((tmp.Y < (selectionBounds.Y - r)) || (tmp.Y > (selectionBounds.Y + selectionBounds.Height + r))) return false;
            if (tmp.X < (selectionBounds.X + r))
            {
                moveHandlePos = 4;                                              // left edge
                if (tmp.Y < (selectionBounds.Y + r)) { moveHandlePos = 1; }         // lower-left corner
                if (tmp.Y > (selectionBounds.Y + selectionBounds.Height - r))   // upper-left corner
                { moveHandlePos = 7; }
                return true;
            }
            if (tmp.X > (selectionBounds.X + selectionBounds.Width - r))
            {
                moveHandlePos = 6;                                              // right edge
                if (tmp.Y < (selectionBounds.Y + r)) { moveHandlePos = 3; }         // lower-right corner
                if (tmp.Y > (selectionBounds.Y + selectionBounds.Height - r))   // upper-right corner
                { moveHandlePos = 9; }
                return true;
            }
            if (tmp.Y < (selectionBounds.Y + r))
            {
                moveHandlePos = 2; return true;
            }                               // lower edge

            if (tmp.Y > (selectionBounds.Y + selectionBounds.Height - r))
            {
                moveHandlePos = 8; return true;
            }                               // upper edge
            return false;
        }

        private static void DrawHandles()
        {
            pathBounds.Reset();
            pathBounds.StartFigure();
            pathArrows.Reset();
            pathArrows.StartFigure();
            if (SelectedMarkerType != XmlMarkerType.Node)
            {
                pathBounds.AddRectangle(selectionBounds);
                AddPath(pathArrows, arrow0, GetRectHandle(upperEdgeCenter, handleSize), 0);     // arrow vertical
                AddPath(pathArrows, arrow0, GetRectHandle(upperEdgeRight, handleSize), -45);    // arrow diagonal
                AddPath(pathArrows, arrow0, GetRectHandle(rightEdgeCenter, handleSize), 90);    // arrow horizontal
                AddPath(pathArrows, arrow1, GetRectHandle(rightEdgeBottom, handleSize), 0, 1);  // turn
                AddPath(pathArrows, arrow2, GetRectHandle(center, handleSize), 0, 2);           // center cross
            }
            else
            {
                if (nodePathBeforeOk) pathBounds.AddLine(nodePathBefore, nodePathActual);
                if (nodePathAfterOk) pathBounds.AddLine(nodePathActual, nodePathAfter);
                AddPath(pathArrows, arrow3, GetRectHandle(center, handleSize), 0, 3);           // center box
            }
        }
        private static void AddPath(GraphicsPath finalPath, GraphicsPath tmpPath, RectangleF rect, float angle, int type = 0)
        {
            if (tmpPath.PointCount < 1)
            { DrawArrow(tmpPath, type); }

            GraphicsPath arrowTmp = (GraphicsPath)tmpPath.Clone();
            Matrix tmp = new Matrix();
            tmp.Rotate(angle);
            tmp.Scale(rect.Width / 8, rect.Height / 8);
            arrowTmp.Transform(tmp);
            tmp.Reset();
            tmp.Translate(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            arrowTmp.Transform(tmp);
            finalPath.AddPath(arrowTmp, false);
            tmp.Dispose();
            arrowTmp.Dispose();
        }

        private static void DrawArrow(GraphicsPath tmpPath, int type)
        {
            int[] arrowPoint0 = { 0, -4, 3, -1, 1, -1, 1, 1, 3, 1, 0, 4, -3, 1, -1, 1, -1, -1, -3, -1 };
            int[] arrowPoint1 = { -4, -2, -2, -4, -2, -3, 1, -3, 3, -1, 3, 2, 4, 2, 2, 4, 0, 2, 1, 2, 1, 0, 0, -1, -2, -1, -2, 0, -4, -2 };
            int[] arrowPoint2 = { 0, -4, 1, -3, 1, -1, 3, -1, 4, 0, 3, 1, 1, 1, 1, 3, 0, 4, -1, 3, -1, 1, -3, 1, -4, 0, -3, -1, -1, -1, -1, -3 };
            int[] arrowPoint3 = { -2, -2, -2, 2, 2, 2, 2, -2, -2, -2 };
            List<Point> points = new List<Point>();
            tmpPath.Reset();
            if (type == 0)
            {
                for (int i = 0; i < arrowPoint0.Count(); i += 2)
                { points.Add(new Point(arrowPoint0[i], arrowPoint0[i + 1])); }
            }
            else if (type == 1)
            {
                for (int i = 0; i < arrowPoint1.Count(); i += 2)
                { points.Add(new Point(arrowPoint1[i], arrowPoint1[i + 1])); }
            }
            else if (type == 2)
            {
                for (int i = 0; i < arrowPoint2.Count(); i += 2)
                { points.Add(new Point(arrowPoint2[i], arrowPoint2[i + 1])); }
            }
            else
            {
                for (int i = 0; i < arrowPoint3.Count(); i += 2)
                { points.Add(new Point(arrowPoint3[i], arrowPoint3[i + 1])); }
            }
            tmpPath.AddLines(points.ToArray());
            tmpPath.CloseFigure();
        }
        private static RectangleF GetRectHandle(PointF tmp, float r)
        { return new RectangleF(tmp.X - r, tmp.Y - r, 2 * r, 2 * r); }

        public static PointF GetScaleFactor(XyPoint diff, bool sameScaling, bool snap)
        {
            if (snap)
            {
                diff.X = Math.Round(selectionBoundsOrig.X + selectionBounds.Width + diff.X) - (selectionBoundsOrig.X + selectionBounds.Width);
                diff.Y = Math.Round(selectionBoundsOrig.Y + selectionBounds.Height + diff.Y) - (selectionBoundsOrig.Y + selectionBounds.Height);
            }

            float x = (selectionBoundsOrig.Width + (float)diff.X) / selectionBoundsOrig.Width;		// switch to selectionBoundsOrig
            float y = (selectionBoundsOrig.Height + (float)diff.Y) / selectionBoundsOrig.Height;	// switch to selectionBoundsOrig
            if (sameScaling)
                y = x = Math.Max(x, y);

            return new PointF(x, y);
        }

        private static XyPoint CorrectOffsetOnSnap(XyPoint diff)
        {
            if ((moveHandlePos == 1) || (moveHandlePos == 4) || (moveHandlePos == 7))   // correct X-left
            { diff.X = Math.Round(selectionBoundsOrig.X + diff.X) - selectionBoundsOrig.X; }
            if ((moveHandlePos == 3) || (moveHandlePos == 6) || (moveHandlePos == 9))   // correct X-right
            { diff.X = Math.Round(selectionBoundsOrig.X + selectionBoundsOrig.Width + diff.X) - (selectionBoundsOrig.X + selectionBoundsOrig.Width); }

            if ((moveHandlePos == 1) || (moveHandlePos == 2) || (moveHandlePos == 3))   // correct Y-bottom
            { diff.Y = Math.Round(selectionBoundsOrig.Y + diff.Y) - selectionBoundsOrig.Y; }
            if ((moveHandlePos == 7) || (moveHandlePos == 8) || (moveHandlePos == 9))   // correct Y-top
            { diff.Y = Math.Round(selectionBoundsOrig.Y + selectionBoundsOrig.Height + diff.Y) - (selectionBoundsOrig.Y + selectionBoundsOrig.Height); }

            if (moveHandlePos == 5)
            {
                PointF centerTmp = new PointF(selectionBoundsOrig.X + selectionBoundsOrig.Width / 2, selectionBoundsOrig.Y + selectionBoundsOrig.Height / 2);
                diff.X = Math.Round(centerTmp.X + diff.X) - centerTmp.X;
                diff.Y = Math.Round(centerTmp.Y + diff.Y) - centerTmp.Y;
            }
            correctedDifference = diff;
            return diff;
        }

        public static XyPoint Translate(XyPoint diff, bool snap)
        {
            if (snap)
            { diff = CorrectOffsetOnSnap(diff); }
            selectionBounds.X = selectionBoundsOrig.X + (float)diff.X;
            selectionBounds.Y = selectionBoundsOrig.Y + (float)diff.Y;
            SetHandlePositions(selectionBounds);		// move coordinates
            DrawHandles();                              // then create paths
            return diff;
        }
        public static float GetAngleDeg(XyPoint pa, XyPoint pb, bool snap)
        {
            XyPoint centPos = new XyPoint(center);
            float angle = (float)(centPos.AngleTo(pa) - centPos.AngleTo(pb));
            if (snap)
                angle = (float)Math.Round(angle);

            angleDeg = angle;
            Matrix tmp = new Matrix();
            tmp.RotateAt(angle, center);
            SetHandlePositions(selectionBounds);		// move coordinates
            DrawHandles();                              // then create paths
            pathBounds.Transform(tmp);					// rotate path
            pathArrows.Transform(tmp);
            tmp.Dispose();
            return angle;
        }

        public static void Scale(XyPoint diff, Handle type, bool sameScaling, bool snap)
        {
            PointF factor = GetScaleFactor(diff, sameScaling, snap);
            if ((type == Handle.SizeX) || (type == Handle.SizeXY))
                selectionBounds.Width = selectionBoundsOrig.Width * factor.X;
            if ((type == Handle.SizeY) || (type == Handle.SizeXY))
                selectionBounds.Height = selectionBoundsOrig.Height * factor.Y;
            SetHandlePositions(selectionBounds);		// scale coordinates
            DrawHandles();								// then create paths
            scalingX = factor.X;
            scalingY = factor.Y;
        }

        public static void DrawPath(Graphics e, double scal)
        {
            if (!IsActive)
                return;

            if (scalingHandle != scal)  // zooming in 2D view changed... Keep handle size relative to view-box
            {
                scalingHandle = scal;
                handleSize = (float)(handleSizeFix / scalingHandle);
                if (handleSize == 0)
                    handleSize = 1;
                SetHandlePositions(selectionBounds);
                DrawHandles();
            }
            Pen myPen = new Pen(Color.Black, handlePenWidth) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            if (SelectedMarkerType == XmlMarkerType.Node)
                myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            e.DrawPath(myPen, pathBounds);
            myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            e.DrawPath(myPen, pathArrows);
            myPen.Dispose();
        }
    }
}