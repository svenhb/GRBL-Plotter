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
 * 2019-06-14 add code markers: xmlMarker.figureStart
 * 2019-08-15 add logger
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2020-12-20 bug fix: missing xmlMarker.passEnd if cBNoZUp.Checked
 * 2020-12-23 bug fix: rect-pocket innerst path some times missing
 * 2021-01-07 add edge bevel / rounding
 * 2021-02-16 presets when switch between tabs
 * 2021-06-26 gcode.setup(false) disable InsertSubroutine, LineSegmentation. Tab 2,3 disable cBNoZUp
 * 2021-07-14 code clean up / code quality
 * 2021-11-23 set default for tprop
 * 2023-01-17 add try catch in ShapeToGCode_Load
 * 2023-03-04 add save/load/select shapes
 * 2023-03-14 l:1226 f:BtnApplyShape_Click check min/max before setting the value; save also avoidZUp, finalMove0, insertShape
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GrblPlotter
{
    public partial class GCodeFromShape : Form
    {
        private string shapegcode = "";
        public string ShapeGCode
        { get { return shapegcode; } }
        public GraphicsPath PathBackground = new GraphicsPath();
        private GraphicsPath path;

        private static readonly StringBuilder gcodeString = new StringBuilder();

        private float offsetX = 0, offsetY = 0;
        private static bool gcodeTangEnable = false;
        private Image picBevelOff = Properties.Resources.rndOff;
        private Image picBevelOn = Properties.Resources.rndOn;
        private ToolProp tprop = new ToolProp();
        private readonly ToolProp torig = new ToolProp();

		internal class AppProp
		{
			internal bool avoidZUp = false;
			internal bool finalMove0=false;
			internal bool insertShape = false;
		}

        internal class PathProp
        {
            internal int origin = 7;	// 1 - 9
            internal int type = 1;		// 1 - 3 on, outside, inside shape
        }
        internal class ShapeProp
        {
            internal int type = 1;		// 1 - 3 rect, round-rect, circle
            internal double dimX = 10, dimY = 10, dimR = 1;
            internal bool pocket = false;
        }
        internal class ShapeFull
        {
            internal string name = "";
            internal ToolProp tool = new ToolProp();
            internal PathProp path = new PathProp();
            internal ShapeProp shape = new ShapeProp();
			internal AppProp app = new AppProp();
        }
        internal List<ShapeFull> Shapes = new List<ShapeFull>();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public GCodeFromShape()
        {
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Logger.Info("++++++ GCodeFromShape START ++++++");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        { this.Close(); }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Gcode.Setup(false);                  // load defaults from setup-tab - disable InsertSubroutine, LineSegmentation

            Gcode.GcodeXYFeed = (float)nUDToolFeedXY.Value;    // override devault values
            Gcode.GcodeZFeed = (float)nUDToolFeedZ.Value;    // override devault values
            Gcode.GcodeSpindleSpeed = (float)nUDToolSpindleSpeed.Value;    // override devault values
            Gcode.GcodeZDown = (float)nUDImportGCZDown.Value;
            gcodeTangEnable = Properties.Settings.Default.importGCTangentialEnable;


            Logger.Debug(culture, "Create GCode {0}", Gcode.GetSettings());

            gcodeString.Clear();
            PathBackground.Reset();
            path = PathBackground;      // create background path
            path.StartFigure();

            Gcode.Tool(gcodeString, tprop.Toolnr, 0, tprop.Name + " [" + ColorTranslator.ToHtml(tprop.Color) + "]");                                                                                                                                                //       if (!Properties.Settings.Default.importGCSpindleToggle) gcode.SpindleOn(gcodeString, "Start");

            float x, y, rShape, d, dTool, dToolOffset, overlap, rTool, rToolOffset, zStep;
            float zStart = 0;
            x = (float)nUDShapeX.Value;
            y = (float)nUDShapeY.Value;
            rShape = (float)nUDShapeR.Value;
            d = 2 * rShape;
            dTool = dToolOffset = (float)nUDToolDiameter.Value;               // tool diameter;
            rTool = dTool / 2;
            overlap = dToolOffset * (float)nUDToolOverlap.Value / 100;  // tool overlap
            if (rBToolpath1.Checked) { dToolOffset = 0; }             // engrave
            if (rBToolpath3.Checked) { dToolOffset = -dToolOffset; }        // inside
            rToolOffset = dToolOffset / 2;                                  // tool radius

            int counter = 0, safety = 100;
            int zStepCount = 0;
            float dx, dy, rDelta;
            int passCount = 0;
            int figureCount = 1;
            Gcode.JobStart(gcodeString, "StartJob");
            //    gcode.PenUp(gcodeString);

            bool inOneStep = (nUDToolZStep.Value >= -nUDImportGCZDown.Value); // if step >= final Z

            Gcode.Comment(gcodeString, XmlMarker.GroupStart + " Id=\"" + figureCount.ToString(culture) + "\" Type=\"Shape\" >");
            Gcode.Comment(gcodeString, XmlMarker.FigureStart + " Id=\"" + figureCount.ToString(culture) + "\" >");

            if (tabControl1.SelectedTab == tabPage1)    // rectangle, circle
            {
                #region shape
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (rBShape1.Checked)                               // rectangle
                {
                    GetOffset(x, y);

                    path.AddLine(offsetX, offsetY, offsetX + x, offsetY);           // create background path
                    path.AddLine(offsetX + x, offsetY, offsetX + x, offsetY + y);
                    path.AddLine(offsetX + x, offsetY + y, offsetX, offsetY + y);
                    path.AddLine(offsetX, offsetY + y, offsetX, offsetY);

                    offsetX -= rToolOffset; offsetY -= rToolOffset;
                    x += dToolOffset; y += dToolOffset;                          // width +/- tool diameter (outline / inline)
                    zStep = zStart;

                    while (zStep > (float)nUDImportGCZDown.Value)   // nUDImportGCZDown.Value e.g. -2
                    {
                        zStep -= (float)nUDToolZStep.Value;         // nUDToolZStep.Value e.g.  0.5
                        if (zStep < (float)nUDImportGCZDown.Value)
                            zStep = (float)nUDImportGCZDown.Value;

                        if ((overlap > x / 2) || (overlap > y / 2))
                        { overlap = (float)(Math.Min(x, y) / 2.1); }

                        //if ((zStepCount++ == 0) || !cBNoZUp.Checked)    // move up the 1st time 
                        if (!(zStepCount++ == 0))     // move up the 1st time 
                        {
                            if (!cBNoZUp.Checked) Gcode.PenUp(gcodeString, "");
                            if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ "  " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if (gcodeTangEnable)
                            Gcode.SetTangential(gcodeString, 90, true);

                        if (!inOneStep) Gcode.Comment(gcodeString, string.Format(culture, "{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", XmlMarker.PassStart, passCount, zStep, nUDImportGCZDown.Value));

                        if (cBToolpathPocket.Checked)
                            Gcode.MoveToRapid(gcodeString, offsetX + overlap, offsetY + overlap, "");
                        else
                            Gcode.MoveToRapid(gcodeString, offsetX, offsetY, "");
                        Gcode.GcodeZDown = zStep;               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");
                        if (cBToolpathPocket.Checked)           // 1st pocket
                        {
                            if ((x > Math.Abs(dToolOffset)) && (y > Math.Abs(dToolOffset)))      // wide enough for pocket
                            {
                                dx = overlap; dy = overlap;
                                while (((dx < (x + overlap) / 2) && (dy < (y + overlap) / 2)) && (counter++ < safety))
                                {
                                    MakeRect(offsetX + dx, offsetY + dy, offsetX + x - dx, offsetY + y - dy, 0, false);  // rectangle clockwise
                                    dx += overlap; dy += overlap;
                                    if ((dx < (x + overlap) / 2) && (dy < (y + overlap) / 2))
                                        Gcode.MoveTo(gcodeString, offsetX + dx, offsetY + dy, "Pocket");
                                }
                                if (cBNoZUp.Checked)
                                    Gcode.MoveTo(gcodeString, offsetX, offsetY, "Pocket finish");
                                else
                                {
                                    Gcode.PenUp(gcodeString, "Pocket finish");
                                    Gcode.MoveToRapid(gcodeString, offsetX, offsetY, "Pocket finish");
                                    Gcode.PenDown(gcodeString, "");
                                }
                            }
                        }

                        MakeRect(offsetX, offsetY, offsetX + x, offsetY + y, 0, true);  // final rectangle clockwise
                    }
                    Gcode.PenUp(gcodeString, "");
                    if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ " " + passCount.ToString() + ">");
                }
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                else if (rBShape2.Checked)           // rectangle with round edge
                {
                    GetOffset(x, y);

                    path.AddLine(offsetX + rShape, offsetY, offsetX + x - rShape, offsetY);     // create background path
                    path.AddArc(offsetX + x - d, offsetY, d, d, -90, 90);

                    path.AddLine(offsetX + x, offsetY + rShape, offsetX + x, offsetY + y - rShape);
                    path.AddArc(offsetX + x - d, offsetY + y - d, d, d, 0, 90);

                    path.AddLine(offsetX + x - rShape, offsetY + y, offsetX + rShape, offsetY + y);
                    path.AddArc(offsetX, offsetY + y - d, d, d, 90, 90);

                    path.AddLine(offsetX, offsetY + y - rShape, offsetX, offsetY + rShape);
                    path.AddArc(offsetX, offsetY, d, d, 180, 90);

                    offsetX -= rToolOffset; offsetY -= rToolOffset;
                    x += dToolOffset; y += dToolOffset;

                    if ((overlap > x / 2) || (overlap > y / 2))
                    { overlap = (float)(Math.Min(x, y) / 2.1); }
                    //                   gcode.Move(gcodeString, 0, offsetX, offsetY + r, false, "");
                    zStep = zStart;

                    while (zStep > (float)nUDImportGCZDown.Value)
                    {
                        zStep -= (float)nUDToolZStep.Value;
                        if (zStep < (float)nUDImportGCZDown.Value)
                            zStep = (float)nUDImportGCZDown.Value;

                        //                    if ((zStepCount++ == 0) || !cBNoZUp.Checked)    // move up the 1st time 
                        if (!(zStepCount++ == 0))     // move up the 1st time 
                        {
                            if (!cBNoZUp.Checked) Gcode.PenUp(gcodeString, "");
                            if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ " " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if (gcodeTangEnable)
                            Gcode.SetTangential(gcodeString, 90, true);

                        if (!inOneStep) Gcode.Comment(gcodeString, string.Format(culture, "{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", XmlMarker.PassStart, passCount, zStep, nUDImportGCZDown.Value));

                        if (cBToolpathPocket.Checked)
                            Gcode.MoveToRapid(gcodeString, offsetX + overlap, offsetY + rShape, "");
                        else
                            Gcode.MoveToRapid(gcodeString, offsetX, offsetY + rShape, "");
                        Gcode.GcodeZDown = zStep;               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");
                        if (cBToolpathPocket.Checked)
                        {
                            dx = overlap; dy = overlap; rDelta = rShape - overlap;
                            while (((dx < (x + overlap) / 2) && (dy < (y + overlap) / 2)) && (counter++ < safety))
                            {
                                MakeRect(offsetX + dx, offsetY + dy, offsetX + x - dx, offsetY + y - dy, rDelta, false);  // rectangle clockwise
                                dx += overlap; dy += overlap; rDelta -= overlap;
                                if (dx > (x + overlap) / 2) { dx = (x + overlap) / 2; }
                                if (dy > (x + overlap) / 2) { dy = (y + overlap) / 2; }
                                if (rDelta < overlap) { rDelta = 0; }
                                if ((dx < (x + overlap) / 2) && (dy < (y + overlap) / 2))
                                    Gcode.MoveTo(gcodeString, offsetX + dx, offsetY + dy + rDelta, "");
                            }
                            if (cBNoZUp.Checked)
                                Gcode.MoveTo(gcodeString, offsetX, offsetY + rShape, "");
                            else
                            {
                                Gcode.PenUp(gcodeString, "");
                                Gcode.MoveToRapid(gcodeString, offsetX, offsetY + rShape, "");
                                Gcode.PenDown(gcodeString, "");
                            }
                        }
                        MakeRect(offsetX, offsetY, offsetX + x, offsetY + y, rShape, true);  // rectangle clockwise
                    }
                    Gcode.PenUp(gcodeString, "");
                    if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ " " + passCount.ToString() + ">");
                }
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                else if (rBShape3.Checked)           // circle
                {
                    GetOffset(d, d);
                    path.AddArc(offsetX, offsetY, d, d, 0, 360);        // create background path

                    offsetX -= rToolOffset; offsetY -= rToolOffset;
                    rShape += rToolOffset;                    // take care of tool diameter if set

                    if (overlap > rShape)
                    { overlap = (float)(rShape / 2.1); }

                    zStep = zStart;

                    while (zStep > (float)nUDImportGCZDown.Value)
                    {
                        //if ((zStepCount++ == 0) || !cBNoZUp.Checked)    // move up the 1st time 
                        if (!(zStepCount++ == 0))     // move up the 1st time 
                        {
                            if (!cBNoZUp.Checked) Gcode.PenUp(gcodeString, "");
                            if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ " " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if (gcodeTangEnable)
                            Gcode.SetTangential(gcodeString, 90, true);


                        if (!inOneStep) Gcode.Comment(gcodeString, string.Format(culture, "{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", XmlMarker.PassStart, passCount, zStep, nUDImportGCZDown.Value));

                        if (cBToolpathPocket.Checked)
                            Gcode.MoveToRapid(gcodeString, offsetX + rShape - overlap, offsetY + rShape, "");
                        else
                            Gcode.MoveToRapid(gcodeString, offsetX, offsetY + rShape, "");
                        zStep -= (float)nUDToolZStep.Value;
                        if (zStep < (float)nUDImportGCZDown.Value)
                            zStep = (float)nUDImportGCZDown.Value;
                        Gcode.GcodeZDown = zStep;               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");
                        rDelta = overlap;
                        counter = 0;
                        if ((cBToolpathPocket.Checked) && (rShape > 2 * rToolOffset))
                        {
                            while ((rDelta < rShape) && (counter++ < safety))
                            {
                                Gcode.SetTangential(gcodeString, -270, false);  //
                                Gcode.Arc(gcodeString, 2, offsetX + rShape - rDelta, offsetY + rShape, rDelta, 0, "");
                                rDelta += overlap;
                                if (rDelta < rShape)
                                    Gcode.MoveTo(gcodeString, offsetX + rShape - rDelta, offsetY + rShape, "");
                            }
                            Gcode.MoveTo(gcodeString, offsetX, offsetY + rShape, "");
                        }
                        Gcode.SetTangential(gcodeString, -270, false);  //
                        Gcode.Arc(gcodeString, 2, offsetX, offsetY + rShape, rShape, 0, "");
                    }
                    Gcode.PenUp(gcodeString, "");
                    if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ " " + passCount.ToString() + ">");
                }
                #endregion
            }
            else if (tabControl1.SelectedTab == tabPage2)   // bevel / round off
            {
                {
                    rShape = (float)nUDBevelR.Value;
                    GetOffset(rShape, rShape);
                    double rPath = rShape + rToolOffset;                                // rTool is neg if inside shape, 0 if on shape, pos if outside shape
                    double xStart = 0, yStart = 0;
                    double xEnd = 0, yEnd = 0;
                    double xOff = 0, yOff = 0;
                    double i = 0, j = 0;
                    double aStart = 0, aEnd = 0;

                    bool isRound = rBBevel1.Checked;

                    /* Calculate start-pos from circle-center, then move to lower left of quadrant*/
                    if (rB1.Checked)
                    {  // add background path
                        xStart = 0; yStart = rPath; xOff = rShape; yOff = 0; aStart = isRound ? 180 : 225; aEnd = isRound ? 270 : 225;
                        xEnd = -rPath; yEnd = 0; i = 0; j = -rPath;
                        // create background path
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));      // draw edge
                        path.AddLine((float)(-rShape + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY));
                        path.StartFigure();
                        if (isRound)
                            path.AddArc((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(2 * rShape), (float)(2 * rShape), 90, 90);      // draw edge
                        else
                            path.AddLine((float)(0 + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(0 + yOff + offsetY));
                    }
                    if (rB2.Checked)
                    {  // add background path
                        xStart = rPath; yStart = 0; xOff = 0; yOff = 0; aStart = isRound ? 90 : 135; aEnd = isRound ? 180 : 135;
                        xEnd = 0; yEnd = rPath; i = -rPath; j = 0;
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));
                        path.StartFigure();
                        if (isRound)
                            path.AddArc((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(2 * rShape), (float)(2 * rShape), 0, 90);      // draw edge
                        else
                            path.AddLine((float)(rShape + xOff + offsetX), (float)(0 + yOff + offsetY), (float)(0 + xOff + offsetX), (float)(rShape + yOff + offsetY));
                    }
                    if (rB3.Checked)
                    {  // add background path  
                        xStart = 0; yStart = -rPath; xOff = 0; yOff = rShape; aStart = isRound ? 0 : 45; aEnd = isRound ? 90 : 45;
                        xEnd = rPath; yEnd = 0; i = 0; j = rPath;
                        path.AddLine((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY));
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));
                        path.StartFigure();
                        if (isRound)
                            path.AddArc((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(2 * rShape), (float)(2 * rShape), 270, 90);      // draw edge
                        else
                            path.AddLine((float)(0 + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(0 + yOff + offsetY));
                    }
                    if (rB4.Checked)
                    {  // add background path  
                        xStart = -rPath; yStart = 0; xOff = rShape; yOff = rShape; aStart = isRound ? 270 : 315; aEnd = isRound ? 360 : 315;
                        xEnd = 0; yEnd = -rPath; i = rPath; j = 0;
                        path.AddLine((float)(-rShape + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY));
                        path.AddLine((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY));
                        path.StartFigure();
                        if (isRound)
                            path.AddArc((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(2 * rShape), (float)(2 * rShape), 180, 90);      // draw edge
                        else
                            path.AddLine((float)(-rShape + xOff + offsetX), (float)(0 + yOff + offsetY), (float)(0 + xOff + offsetX), (float)(-rShape + yOff + offsetY));
                    }


                    //           x += dTool;y += dTool;                          // width +/- tool diameter (outline / inline)
                    zStep = zStart;
                    while (zStep > (float)nUDImportGCZDown.Value)   // nUDImportGCZDown.Value e.g. -2
                    {
                        zStep -= (float)nUDToolZStep.Value;         // nUDToolZStep.Value e.g.  0.5
                        if (zStep < (float)nUDImportGCZDown.Value)
                            zStep = (float)nUDImportGCZDown.Value;

                        if (!(zStepCount++ == 0))     // move up the 1st time 
                        {
                            //if (!cBNoZUp.Checked) 
                            Gcode.PenUp(gcodeString, "");
                            if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ "  " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if (!inOneStep) Gcode.Comment(gcodeString, string.Format(culture, "{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", XmlMarker.PassStart, passCount, zStep, nUDImportGCZDown.Value));

                        if (gcodeTangEnable) Gcode.SetTangential(gcodeString, aStart, true);
                        Gcode.MoveToRapid(gcodeString, (float)(xStart + xOff + offsetX), (float)(yStart + yOff + offsetY), "");

                        Gcode.GcodeZDown = zStep;               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");

                        Gcode.SetTangential(gcodeString, aEnd, false);  //
                        if (isRound)
                            Gcode.Arc(gcodeString, 3, (float)(xEnd + xOff + offsetX), (float)(yEnd + yOff + offsetY), (float)i, (float)j, "");
                        else
                            Gcode.MoveTo(gcodeString, (float)(xEnd + xOff + offsetX), (float)(yEnd + yOff + offsetY), "");

                    }
                    Gcode.PenUp(gcodeString, "");
                    if (!inOneStep) Gcode.Comment(gcodeString, XmlMarker.PassEnd + ">"); //+ " " + passCount.ToString() + ">");
                }

            }
            else if (tabControl1.SelectedTab == tabPage3)   // round off Z
            {
                float xStart, yStart;
                float xEnd, yEnd;

                float roundR = (float)nUDRZRadius.Value;
                float stepZ = (float)nUDRZStep.Value;
                if (stepZ > roundR)
                { nUDRZStep.Value = (decimal)(stepZ = roundR); }

                float width = (float)nUDRZWidth.Value;

                float actualZ = roundR;
                float tmpX1, tmpX2 = 0, tmpY1, tmpY2 = 0;

                #region horizontal
                if (rBRoundZYT.Checked || rBRoundZYB.Checked)
                {
                    GetOffset(width, roundR);
                    path.AddRectangle(new RectangleF(offsetX, offsetY, width, roundR));
                    path.StartFigure();

                    float noffX = width + (float)Math.Ceiling(rTool) + 1;
                    float lastZX = 0;
                    //               float tmpX1=0, tmpX2=0, tmpY1 = 0, tmpY2 = 0;
                    int cnt = 0;
                    float y1 = 0, y2 = 0;
                    float starta = 90;
                    PointF circlePos = new PointF
                    {
                        Y = roundR
                    };
                    float lastZ = circlePos.Y;
                    while (circlePos.Y > 0.001)    // add steps
                    {
                        circlePos = GetCirclePos(roundR, stepZ, circlePos.Y);
                        tmpX1 = roundR - lastZ;
                        tmpX2 = roundR - circlePos.Y;
                        tmpY1 = lastZX;
                        tmpY2 = circlePos.X;
                        if (rBRoundZYB.Checked)
                        { tmpY1 = (roundR - lastZX); tmpY2 = (roundR - circlePos.X); }
                        if (cnt++ == 0)
                        { y1 = tmpY2; if (rBRoundZYB.Checked) { y1 -= dTool; starta = 180; y2 = roundR; } }
                        path.AddLine((tmpX1 + noffX + offsetX), (tmpY1 + offsetY), (tmpX1 + noffX + offsetX), (tmpY2 + offsetY));   // draw steps
                        path.AddLine((tmpX1 + noffX + offsetX), (tmpY2 + offsetY), (tmpX2 + noffX + offsetX), (tmpY2 + offsetY));   // draw steps
                        lastZ = circlePos.Y;
                        lastZX = circlePos.X;
                    }
                    path.AddLine((roundR + noffX + offsetX), (tmpY2 + offsetY), (roundR + noffX + offsetX), (y2 + offsetY));    // draw final line
                    path.StartFigure();
                    path.AddArc((noffX + offsetX), (-tmpY2 + offsetY), (2 * roundR), (2 * roundR), starta, 90);                 // draw 1/4 circle
                    path.StartFigure();
                    path.AddArc((-rTool + width + offsetX), (y1 + offsetY), (dTool), (dTool), 0, 360);                          // draw tool diameter
                    path.StartFigure();

                    circlePos.Y = roundR;
                    xStart = -rToolOffset; xEnd = width + rToolOffset;
                    cnt = 0;
                    while (circlePos.Y > 0.001)
                    {   // horizontal zig zag
                        circlePos = GetCirclePos(roundR, stepZ, circlePos.Y);
                        yStart = yEnd = circlePos.X + rTool;
                        if (rBRoundZYB.Checked)
                        { yStart = yEnd = (roundR - (circlePos.X + rTool)); }

                        if (cnt++ == 0)
                            Gcode.MoveToRapid(gcodeString, (xStart + offsetX), (yStart + offsetY), "");

                        Gcode.MoveTo(gcodeString, (xStart + offsetX), (yEnd + offsetY), "");
                        Gcode.GcodeZDown = (circlePos.Y - roundR);               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");
                        Gcode.MoveTo(gcodeString, (xEnd + offsetX), (yEnd + offsetY), "");
                        if (circlePos.Y <= 0)
                            break;

                        circlePos = GetCirclePos(roundR, stepZ, circlePos.Y);
                        yEnd = circlePos.X + rTool;
                        if (rBRoundZYB.Checked)
                        { yEnd = (roundR - (circlePos.X + rTool)); }
                        Gcode.MoveTo(gcodeString, (xEnd + offsetX), (yEnd + offsetY), "");
                        Gcode.GcodeZDown = (circlePos.Y - roundR);               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");
                        Gcode.MoveTo(gcodeString, (xStart + offsetX), (yEnd + offsetY), "");
                    }
                    Gcode.PenUp(gcodeString, "");
                }
                #endregion
                #region vertical
                #region vpath
                else if (rBRoundZXR.Checked || rBRoundZXL.Checked)
                {
                    GetOffset(roundR, width);
                    path.AddRectangle(new RectangleF(offsetX, offsetY, roundR, width));
                    path.StartFigure();

                    float noffY = -((float)Math.Ceiling(rTool) + 1);
                    float lastZ = actualZ;
                    float lastZX = 0;
                    int cnt = 0;
                    float starta = 0;
                    float x1 = 0, x2 = -roundR, x3 = 0;
                    PointF circlePos = new PointF
                    {
                        Y = roundR
                    };
                    while (circlePos.Y > 0.001)
                    {
                        circlePos = GetCirclePos(roundR, stepZ, circlePos.Y);
                        tmpY1 = -(roundR - lastZ);
                        tmpY2 = -(roundR - circlePos.Y);
                        tmpX1 = lastZX;
                        tmpX2 = circlePos.X;
                        if (rBRoundZXL.Checked)
                        { tmpX1 = (roundR - lastZX); tmpX2 = (roundR - circlePos.X); }
                        if (cnt++ == 0)
                        { x1 = tmpX2; if (rBRoundZXL.Checked) { x1 -= dTool; starta = 90; x2 = 0; x3 = roundR; } }
                        path.AddLine((tmpX1 + offsetX), (tmpY1 + noffY + offsetY), (tmpX2 + offsetX), (tmpY1 + noffY + offsetY)); // draw steps
                        path.AddLine((tmpX2 + offsetX), (tmpY1 + noffY + offsetY), (tmpX2 + offsetX), (tmpY2 + noffY + offsetY)); // draw steps
                        lastZ = circlePos.Y;
                        lastZX = circlePos.X;
                    }
                    path.AddLine((tmpX2 + offsetX), (tmpY2 + noffY + offsetY), (x3 + offsetX), (tmpY2 + noffY + offsetY)); // draw final line
                    path.StartFigure();
                    path.AddArc((x2 + offsetX), (-2 * roundR + noffY + offsetY), (2 * roundR), (2 * roundR), starta, 90);       // draw 1/4 circle
                    path.StartFigure();
                    path.AddArc((x1 + offsetX), (-rTool + offsetY), (dTool), (dTool), 0, 360);                              // draw tool diameter
                    path.StartFigure();
                    #endregion

                    circlePos.Y = roundR;
                    yStart = -rToolOffset; yEnd = width + rToolOffset;
                    //    if (rBRoundZXL.Checked)
                    //    { xStart = xEnd = (roundR - (circlePos.X + rTool)); }
                    while (circlePos.Y > 0.001)
                    {   // vertical zig zag

                        circlePos = GetCirclePos(roundR, stepZ, circlePos.Y);
                        xStart = xEnd = circlePos.X + rTool;
                        if (rBRoundZXL.Checked)
                        { xStart = xEnd = (roundR - (circlePos.X + rTool)); }

                        if (cnt++ == 0)
                            Gcode.MoveToRapid(gcodeString, (xStart + offsetX), (yStart + offsetY), "");

                        Gcode.MoveTo(gcodeString, (xStart + offsetX), (yStart + offsetY), "");
                        Gcode.GcodeZDown = (circlePos.Y - roundR);               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");
                        Gcode.MoveTo(gcodeString, (xEnd + offsetX), (yEnd + offsetY), "");

                        circlePos = GetCirclePos(roundR, stepZ, circlePos.Y);
                        xStart = xEnd = circlePos.X + rTool;
                        if (rBRoundZXL.Checked)
                        { xStart = xEnd = (roundR - (circlePos.X + rTool)); }
                        if (circlePos.Y <= 0)
                            break;

                        Gcode.MoveTo(gcodeString, (xEnd + offsetX), (yEnd + offsetY), "");
                        Gcode.GcodeZDown = (circlePos.Y - roundR);               // adapt Z-deepth
                        Gcode.PenDown(gcodeString, "");
                        Gcode.MoveTo(gcodeString, (xStart + offsetX), (yStart + offsetY), "");
                    }
                    Gcode.PenUp(gcodeString, "");
                }
                #endregion
                //        MessageBox.Show(tmp);
            }
            Gcode.Comment(gcodeString, string.Format(culture, "{0}>", XmlMarker.FigureEnd)); // Graphic2GCode.SetFigureEnd(figureCount));
            Gcode.Comment(gcodeString, string.Format(culture, "{0}>", XmlMarker.GroupEnd)); // Graphic2GCode.SetFigureEnd(figureCount));

            if (cBMoveTo00.Checked)
                Gcode.MoveToRapid(gcodeString, 0, 0, "");

            Gcode.JobEnd(gcodeString, "EndJob");      // Spindle / laser off
                                                      //    if (Properties.Settings.Default.importGCZEnable)
                                                      //         gcode.SpindleOff(gcodeString, "Finish - Spindle off");

            string header = Gcode.GetHeader("Simple Shape", "");
            string footer = Gcode.GetFooter();

            gcodeString.Replace(',', '.');
            shapegcode = header + gcodeString.ToString() + footer;
        }

        private static PointF GetCirclePos(float r, float err, float startZ)
        {
            float rextend = r + err;
            float actualZX = (float)Math.Sqrt(rextend * rextend - startZ * startZ);
            if (actualZX > r) actualZX = r;
            double actualA = Math.Asin(actualZX / r);
            float actualZ = (float)Math.Cos(actualA) * r;
            if (actualZ < 0) actualZ = 0;
            return new PointF(actualZX, actualZ);
        }

        /*     private static void CreateGraphicsPathAddCorner()
             {
                 GraphicsPath path = VisuGCode.pathBackground;
                 path.StartFigure();
             }*/

        private static void MakeRect(float x1, float y1, float x2, float y2, float r, bool cw = true)
        {   // start bottom left
            if (cw)
            {
                Gcode.MoveTo(gcodeString, x1, y2 - r, "cw 1");          //BL to TL
                SetTangentialUpDown(0, !(r > 0));
                if (r > 0) { Gcode.Arc(gcodeString, 2, x1 + r, y2, r, 0, ""); }
                Gcode.MoveTo(gcodeString, x2 - r, y2, "cw 2");          // TL to TR
                SetTangentialUpDown(-90, !(r > 0));
                if (r > 0) { Gcode.Arc(gcodeString, 2, x2, y2 - r, 0, -r, ""); }
                Gcode.MoveTo(gcodeString, x2, y1 + r, "cw 3");          // TR to BR
                SetTangentialUpDown(-180, !(r > 0));
                if (r > 0) { Gcode.Arc(gcodeString, 2, x2 - r, y1, -r, 0, ""); }
                Gcode.MoveTo(gcodeString, x1 + r, y1, "cw 4");          // BR to BL
                if (r > 0) Gcode.SetTangential(gcodeString, -270, !(r > 0));  //
                if (r > 0) { Gcode.Arc(gcodeString, 2, x1, y1 + r, 0, r, ""); }
            }
            else
            {
                SetTangentialUpDown(0, !(r > 0));
                if (r > 0) { Gcode.Arc(gcodeString, 3, x1 + r, y1, r, 0, ""); }
                Gcode.MoveTo(gcodeString, x2 - r, y1, "ccw 1");          // to BR
                SetTangentialUpDown(90, !(r > 0));
                if (r > 0) { Gcode.Arc(gcodeString, 3, x2, y1 + r, 0, r, ""); }
                Gcode.MoveTo(gcodeString, x2, y2 - r, "ccw 2");           // to TR
                SetTangentialUpDown(180, !(r > 0));
                if (r > 0) { Gcode.Arc(gcodeString, 3, x2 - r, y2, -r, 0, ""); }
                Gcode.MoveTo(gcodeString, x1 + r, y2, "ccw 3");           // to TL
                SetTangentialUpDown(270, !(r > 0));
                if (r > 0) { Gcode.Arc(gcodeString, 3, x1, y2 - r, 0, -r, ""); }
                Gcode.MoveTo(gcodeString, x1, y1 + r, "ccw 4");           // to BL 
            }
        }

        private static void SetTangentialUpDown(double angle, bool penUp)
        {
            double angleNew = (double)Properties.Settings.Default.importGCTangentialTurn * angle / 360;
            if (gcodeTangEnable)
            {
                if (penUp)
                {
                    Gcode.PenUp(gcodeString, "");
                    gcodeString.AppendFormat("G00 {0}{1:0.000}\r\n", Properties.Settings.Default.importGCTangentialAxis, angleNew);
                    Gcode.PenDown(gcodeString, "");
                }
                Gcode.SetTangential(gcodeString, angle, false);  //
            }
        }

        private void GetOffset(float x, float y)
        {
            if (rBOrigin1.Checked) { offsetX = 0; offsetY = -y; }               // upper left
            if (rBOrigin2.Checked) { offsetX = -x / 2; offsetY = -y; }
            if (rBOrigin3.Checked) { offsetX = -x; offsetY = -y; }              // upper right
            if (rBOrigin4.Checked) { offsetX = 0; offsetY = -y / 2; }
            if (rBOrigin5.Checked) { offsetX = -x / 2; offsetY = -y / 2; }        // center
            if (rBOrigin6.Checked) { offsetX = -x; offsetY = -y / 2; }
            if (rBOrigin7.Checked) { offsetX = 0; offsetY = 0; }                // origin - lower left
            if (rBOrigin8.Checked) { offsetX = -x / 2; offsetY = 0; }
            if (rBOrigin9.Checked) { offsetX = -x; offsetY = 0; }               // lower right
        }

        private void ShapeToGCode_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationShapeForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            try
            {
                nUDToolDiameter.Value = Properties.Settings.Default.createShapeToolDiameter;
                nUDToolZStep.Value = Properties.Settings.Default.createShapeToolZStep;
                nUDToolFeedXY.Value = Properties.Settings.Default.createShapeToolFeedXY;
                nUDToolFeedZ.Value = Properties.Settings.Default.createShapeToolFeedZ;
                nUDToolOverlap.Value = Properties.Settings.Default.createShapeToolOverlap;
                nUDToolSpindleSpeed.Value = Properties.Settings.Default.createShapeToolSpindleSpeed;
                nUDShapeX.Value = Properties.Settings.Default.createShapeX;
                nUDShapeY.Value = Properties.Settings.Default.createShapeY;
                nUDShapeR.Value = Properties.Settings.Default.createShapeR;
                cBMoveTo00.Checked = Properties.Settings.Default.createShapeMovo00;
                cBNoZUp.Checked = Properties.Settings.Default.createShapeNoZUp;

                nUDRZRadius.Value = Properties.Settings.Default.createShapeRZRadius;
                nUDRZWidth.Value = Properties.Settings.Default.createShapeRZWidth;
                nUDRZStep.Value = Properties.Settings.Default.createShapeRZStep;

                nUDImportGCZDown.Value = Properties.Settings.Default.importGCZDown;
            }
            catch { }

            SetShapeID(Properties.Settings.Default.createShapeType);
            SetToolpathID(Properties.Settings.Default.createShapeToolPath);
            SetOriginID(Properties.Settings.Default.createShapeOrigin);

            SaveOrigToolProperties();	// needed in CBToolSet_CheckedChanged

            int toolCount = ToolTable.Init(" (ShapeToGCode_Load)");   // get max index
            ToolProp tmpTool;
            bool defaultToolFound = false;
            for (int i = 0; i < toolCount; i++)
            {
                tmpTool = ToolTable.GetToolProperties(i);
                if (i == tmpTool.Toolnr)
                {
                    cBTool.Items.Add(i.ToString(culture) + ") " + tmpTool.Name);
                    if (i == Properties.Settings.Default.importGCToolDefNr)
                    {
                        cBTool.SelectedIndex = cBTool.Items.Count - 1;
                        defaultToolFound = true;
                    }
                }
            }
            if (!defaultToolFound)
            {
                cBTool.Items.Add("No tools found");
                cBTool.SelectedIndex = 0;
            }
            tprop = ToolTable.GetToolProperties(1);
            CBToolSet_CheckedChanged(sender, e);

            rB1.Image = (Image)picBevelOff.Clone();
            rB2.Image = (Image)picBevelOff.Clone(); rB2.Image.RotateFlip((RotateFlipType.Rotate90FlipNone));
            rB3.Image = (Image)picBevelOff.Clone(); rB3.Image.RotateFlip((RotateFlipType.Rotate180FlipNone));
            rB4.Image = (Image)picBevelOff.Clone(); rB4.Image.RotateFlip((RotateFlipType.Rotate270FlipNone));

            LoadXML(Datapath.Data + "\\simpleshapes.xml");
            Update_ShapeSelection(-1);
        }

        private void ShapeToGCode_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ GCodeFromShape STOP ++++++");
            Properties.Settings.Default.locationShapeForm = Location;
            SaveSettings();
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.createShapeToolDiameter = nUDToolDiameter.Value;
            Properties.Settings.Default.createShapeToolZStep = nUDToolZStep.Value;
            Properties.Settings.Default.createShapeToolFeedXY = nUDToolFeedXY.Value;
            Properties.Settings.Default.createShapeToolFeedZ = nUDToolFeedZ.Value;
            Properties.Settings.Default.createShapeToolOverlap = nUDToolOverlap.Value;
            Properties.Settings.Default.createShapeToolSpindleSpeed = nUDToolSpindleSpeed.Value;
            Properties.Settings.Default.createShapeX = nUDShapeX.Value;
            Properties.Settings.Default.createShapeY = nUDShapeY.Value;
            Properties.Settings.Default.createShapeR = nUDShapeR.Value;
            Properties.Settings.Default.createShapeMovo00 = cBMoveTo00.Checked;
            Properties.Settings.Default.createShapeNoZUp = cBNoZUp.Checked;

            Properties.Settings.Default.createShapeRZRadius = nUDRZRadius.Value;
            Properties.Settings.Default.createShapeRZWidth = nUDRZWidth.Value;
            Properties.Settings.Default.createShapeRZStep = nUDRZStep.Value;

            Properties.Settings.Default.createShapeType = GetShapeID();

            Properties.Settings.Default.importGCZDown = nUDImportGCZDown.Value;
            Properties.Settings.Default.createShapeToolPath = GetToolpathID();

            Properties.Settings.Default.createShapeOrigin = GetOriginID();
            Properties.Settings.Default.Save();
        }
        private int GetShapeID()
        {
            int id = 1;
            if (rBShape2.Checked) id = 2;
            else if (rBShape3.Checked) id = 3;
            return id;
        }
        private void SetShapeID(int id)
        {
            if (id == 1) { rBShape1.Checked = true; }        // rectangle
            else if (id == 2) { rBShape2.Checked = true; }
            else if (id == 3) { rBShape3.Checked = true; }
            else { rBShape1.Checked = true; }
        }
        private int GetToolpathID()
        {
            int id = 1;
            if (rBToolpath2.Checked) id = 2;
            else if (rBToolpath3.Checked) id = 3;
            return id;
        }
        private void SetToolpathID(int id)
        {
            if (id == 1) { rBToolpath1.Checked = true; }
            else if (id == 2) { rBToolpath2.Checked = true; }
            else if (id == 3) { rBToolpath3.Checked = true; }
            else { rBToolpath1.Checked = true; }
        }
        private int GetOriginID()
        {
            int id = 0;
            if (rBOrigin1.Checked) id = 1;
            else if (rBOrigin2.Checked) id = 2;
            else if (rBOrigin3.Checked) id = 3;
            else if (rBOrigin4.Checked) id = 4;
            else if (rBOrigin5.Checked) id = 5;
            else if (rBOrigin6.Checked) id = 6;
            else if (rBOrigin7.Checked) id = 7;
            else if (rBOrigin8.Checked) id = 8;
            else if (rBOrigin9.Checked) id = 9;
            return id;
        }
        private void SetOriginID(int id)
        {
            if (id == 1) { rBOrigin1.Checked = true; }
            else if (id == 2) { rBOrigin2.Checked = true; }
            else if (id == 3) { rBOrigin3.Checked = true; }
            else if (id == 4) { rBOrigin4.Checked = true; }
            else if (id == 5) { rBOrigin5.Checked = true; }
            else if (id == 6) { rBOrigin6.Checked = true; }
            else if (id == 7) { rBOrigin7.Checked = true; }
            else if (id == 8) { rBOrigin8.Checked = true; }
            else if (id == 9) { rBOrigin9.Checked = true; }
            else { rBOrigin5.Checked = true; }
        }
        private void NudShapeR_ValueChanged(object sender, EventArgs e)
        {
            if (rBShape2.Checked)
            {
                decimal min = Math.Min(nUDShapeX.Value, nUDShapeY.Value);
                if (nUDShapeR.Value > min / 2)
                    CheckSetValue(nUDShapeR, min / 2);
            }
            CreateSaveName();
        }

        private void CBToolSet_CheckedChanged(object sender, EventArgs e)
        {
            bool valueFromToolTable = cBToolSet.Checked;
            label1.Enabled = valueFromToolTable;
            cBTool.Enabled = valueFromToolTable;
            if (valueFromToolTable)                     // switch to tooltable - save current settings
            {
                SaveOrigToolProperties();
                RefreshToolPropertiesFromSelectedTool();        // CBTool_SelectedIndexChanged(sender, e);
            }
            else
            {
                SetToolProperties(torig);               // switch back to manual tool settings
            }
            EnableToolPropertiesControls(!valueFromToolTable);
        }

        private void EnableToolPropertiesControls(bool state)
        {
            nUDToolDiameter.Enabled = state;
            nUDToolFeedXY.Enabled = state;
            nUDToolZStep.Enabled = state;
            nUDToolFeedZ.Enabled = state;
            nUDToolOverlap.Enabled = state;
            nUDToolSpindleSpeed.Enabled = state;
        }

        private void SaveOrigToolProperties()
        {
            FillToolProp(torig);
            Logger.Info(culture, "Save Tool dia:{0}, feedXY:{1}, finalZ:{2}, stepZ:{3}, feedZ:{4}, overlap:{5}, spindle:{6}", torig.Diameter, torig.FeedXY, torig.FinalZ, torig.StepZ, torig.FeedZ, torig.Overlap, torig.SpindleSpeed);
        }
        private void CBTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshToolPropertiesFromSelectedTool();
        }
        private void RefreshToolPropertiesFromSelectedTool()
        {
            string tmp = cBTool.SelectedItem.ToString();
            if (tmp.IndexOf(")") > 0)
            {
                int tnr = int.Parse(tmp.Substring(0, tmp.IndexOf(")")), culture);
                Properties.Settings.Default.importGCToolDefNr = tnr;
                if (cBToolSet.Checked)
                {
                    tprop = ToolTable.GetToolProperties(tnr);
                    SetToolProperties(tprop);
                }
            }
        }

        private void SetToolProperties(ToolProp tmp)
        {
            Logger.Info(culture, "Set Tool dia:{0}, feedXY:{1}, finalZ:{2}, stepZ:{3}, feedZ:{4}, overlap:{5}, spindle:{6}", tmp.Diameter, tmp.FeedXY, tmp.FinalZ, tmp.StepZ, tmp.FeedZ, tmp.Overlap, tmp.SpindleSpeed);
            try
            {
                CheckSetValue(nUDToolDiameter, (decimal)tmp.Diameter);
                CheckSetValue(nUDToolFeedXY, (decimal)tmp.FeedXY);
                CheckSetValue(nUDImportGCZDown, (decimal)tmp.FinalZ);
                CheckSetValue(nUDToolZStep, (decimal)Math.Abs(tmp.StepZ));
                CheckSetValue(nUDToolFeedZ, (decimal)tmp.FeedZ);
                CheckSetValue(nUDToolOverlap, (decimal)tmp.Overlap);
                CheckSetValue(nUDToolSpindleSpeed, (decimal)tmp.SpindleSpeed);
            }
            catch (Exception err)
            {
                Logger.Error(err, "CBTool_SelectedIndexChanged Set numeric Up Downs");
            }
        }

        private static void CheckSetValue(NumericUpDown nUDcontrol, decimal val, bool setExtreme=false)
        {
			if (setExtreme)
			{
				if (val < nUDcontrol.Minimum) {val = nUDcontrol.Minimum;}
				if (val > nUDcontrol.Maximum) {val = nUDcontrol.Maximum;}
				nUDcontrol.Value = val;
				return;
			}
			
            if ((val <= nUDcontrol.Maximum) && (val >= nUDcontrol.Minimum))
            { nUDcontrol.Value = val; }
            else
            {
                Logger.Error(culture, "{0} Value:{1} out of range min:{2} max:{3}", nUDcontrol.Name, val, nUDcontrol.Minimum, nUDcontrol.Maximum);
                nUDcontrol.Enabled = true;
            }
        }


        RadioButton cBold = null, cBnow = null;
        private void RB1_CheckedChanged(object sender, EventArgs e)
        {
            cBnow = ((RadioButton)sender);
            SetImage(cBnow, true);
            if (cBnow.Name == "rB1") rBOrigin1.PerformClick();
            if (cBnow.Name == "rB2") rBOrigin3.PerformClick();
            if (cBnow.Name == "rB3") rBOrigin9.PerformClick();
            if (cBnow.Name == "rB4") rBOrigin7.PerformClick();
            if (cBold != null)
            { SetImage(cBold, false); }
            cBold = cBnow;
            btnApply.Enabled = true;
        }
        private void SetImage(RadioButton tmp, bool on)
        {
            if (tmp.Name == "rB1") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); }
            if (tmp.Name == "rB2") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); tmp.Image.RotateFlip((RotateFlipType.Rotate90FlipNone)); }
            if (tmp.Name == "rB3") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); tmp.Image.RotateFlip((RotateFlipType.Rotate180FlipNone)); }
            if (tmp.Name == "rB4") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); tmp.Image.RotateFlip((RotateFlipType.Rotate270FlipNone)); }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            { cBNoZUp.Enabled = true; }

            if (tabControl1.SelectedTab == tabPage2)    // Bevel
            {
                cBNoZUp.Enabled = false;
                btnApply.Enabled = (rB1.Checked || rB2.Checked || rB3.Checked || rB4.Checked);
                rBToolpath2.PerformClick();             // preset 'Outside the shape'
            }
            else
                btnApply.Enabled = true;

            if (tabControl1.SelectedTab == tabPage3)    // Round off Z
            {
                cBNoZUp.Enabled = false;
                RBRoundZYT_CheckedChanged(sender, e);
            }
        }

        private void RBRoundZYT_CheckedChanged(object sender, EventArgs e)
        {
            if (rBRoundZYT.Checked) rBOrigin1.PerformClick();
            else if (rBRoundZYB.Checked) rBOrigin7.PerformClick();
            else if (rBRoundZXR.Checked) rBOrigin9.PerformClick();
            else if (rBRoundZXL.Checked) rBOrigin7.PerformClick();

        }

        private void RBBevel1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBBevel1.Checked)
            {
                picBevelOff = Properties.Resources.rndOff;
                picBevelOn = Properties.Resources.rndOn;
            }
            else
            {
                picBevelOff = Properties.Resources.bevOff;
                picBevelOn = Properties.Resources.bevOn;
            }
            SetImage(rB1, rB1.Checked);
            SetImage(rB2, rB2.Checked);
            SetImage(rB3, rB3.Checked);
            SetImage(rB4, rB4.Checked);
        }



        private readonly XmlReaderSettings settings = new XmlReaderSettings()
        { DtdProcessing = DtdProcessing.Prohibit };

        private void SaveXML(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            XmlWriterSettings set = new XmlWriterSettings
            {
                Indent = true
            };
            XmlWriter xmlWrite = XmlWriter.Create(fileName, set);
            xmlWrite.WriteStartDocument();
            xmlWrite.WriteStartElement("simpleshapes");

            // https://www.c-sharpcorner.com/UploadFile/mahesh/understanding-and-using-graphics-paths-in-gdi/
            foreach (var shape in Shapes)
            {

                xmlWrite.WriteStartElement("simpleshape");
                xmlWrite.WriteAttributeString("name", shape.name);

                xmlWrite.WriteStartElement("tool");
                shape.tool.WriteAttributes(xmlWrite);
                xmlWrite.WriteEndElement();

                xmlWrite.WriteStartElement("path");
                xmlWrite.WriteAttributeString("origin", shape.path.origin.ToString().Replace(',', '.'));
                xmlWrite.WriteAttributeString("type", shape.path.type.ToString().Replace(',', '.'));
                xmlWrite.WriteEndElement();

                xmlWrite.WriteStartElement("shape");
                xmlWrite.WriteAttributeString("type", shape.shape.type.ToString().Replace(',', '.'));
                xmlWrite.WriteAttributeString("x", shape.shape.dimX.ToString().Replace(',', '.'));
                xmlWrite.WriteAttributeString("y", shape.shape.dimY.ToString().Replace(',', '.'));
                xmlWrite.WriteAttributeString("r", shape.shape.dimR.ToString().Replace(',', '.'));
                xmlWrite.WriteAttributeString("p", shape.shape.pocket.ToString());
                xmlWrite.WriteEndElement();

                xmlWrite.WriteStartElement("app");
                xmlWrite.WriteAttributeString("azup", shape.app.avoidZUp.ToString());
                xmlWrite.WriteAttributeString("finm", shape.app.finalMove0.ToString());
                xmlWrite.WriteAttributeString("insh", shape.app.insertShape.ToString());
                xmlWrite.WriteEndElement();

                xmlWrite.WriteEndElement();
            }
            xmlWrite.WriteEndElement();
            xmlWrite.Close();
            Logger.Trace("SaveXML to '{0}'", fileName);
        }


        private void LoadXML(string fileName)
        {

            if (!File.Exists(fileName))
                return;// false;

            Shapes.Clear();
            int shapeIndex = -1;
            XmlReader xmlRead = XmlReader.Create(fileName, settings);
            while (xmlRead.Read())
            {
                if (!xmlRead.IsStartElement())
                    continue;

                switch (xmlRead.Name)
                {
                    case "simpleshape":
                        Shapes.Add(new ShapeFull());
                        shapeIndex = Shapes.Count - 1;
                        if (xmlRead["name"].Length > 0) { Shapes[shapeIndex].name = xmlRead["name"]; }
                        break;
                    case "tool":
                        if (shapeIndex >= 0)
                        {
                            Shapes[shapeIndex].tool.ReadAttributes(xmlRead);
                        }
                        break;
                    case "path":
                        if (shapeIndex >= 0)
                        {
                            if (xmlRead["origin"].Length > 0) { Shapes[shapeIndex].path.origin = int.Parse(xmlRead["origin"], NumberFormatInfo.InvariantInfo); }
                            if (xmlRead["type"].Length > 0) { Shapes[shapeIndex].path.type = int.Parse(xmlRead["type"], NumberFormatInfo.InvariantInfo); }
                        }
                        break;
                    case "shape":
                        {
                            if (xmlRead["type"].Length > 0) { Shapes[shapeIndex].shape.type = int.Parse(xmlRead["type"], NumberFormatInfo.InvariantInfo); }
                            if (xmlRead["x"].Length > 0) { Shapes[shapeIndex].shape.dimX = double.Parse(xmlRead["x"].Replace(',', '.'), NumberFormatInfo.InvariantInfo); }
                            if (xmlRead["y"].Length > 0) { Shapes[shapeIndex].shape.dimY = double.Parse(xmlRead["y"].Replace(',', '.'), NumberFormatInfo.InvariantInfo); }
                            if (xmlRead["r"].Length > 0) { Shapes[shapeIndex].shape.dimR = double.Parse(xmlRead["r"].Replace(',', '.'), NumberFormatInfo.InvariantInfo); }
                            if (xmlRead["p"].Length > 0) { Shapes[shapeIndex].shape.pocket = (xmlRead["p"].ToLower() == "true"); }
                        }
                        break;
                    case "app":
                        {
                            if (xmlRead["azup"].Length > 0) { Shapes[shapeIndex].app.avoidZUp = (xmlRead["azup"].ToLower() == "true"); }
                            if (xmlRead["finm"].Length > 0) { Shapes[shapeIndex].app.finalMove0 = (xmlRead["finm"].ToLower() == "true"); }
                            if (xmlRead["insh"].Length > 0) { Shapes[shapeIndex].app.insertShape = (xmlRead["insh"].ToLower() == "true"); }
						}
                        break;
                }
            }
            xmlRead.Close();

        }

        private void Update_ShapeSelection(int select)
        {
            CbShapeSelection.Items.Clear();
            if (Shapes.Count > 0)
            {
                foreach (var shape in Shapes)
                { CbShapeSelection.Items.Add(shape.name); }

                int index = 0;
                if (select >= 0)
                {
                    if (select < Shapes.Count)
                        index = select;

                    CbShapeSelection.SelectedItem = index;

                    CbShapeSelection.Text = CbShapeSelection.Items[index].ToString();
                }
            }
        }

        private void BtnAddShape_Click(object sender, EventArgs e)
        {
            Shapes.Add(new ShapeFull());
            int shapeIndex = Shapes.Count - 1;

            FillToolProp(Shapes[shapeIndex].tool);

            Shapes[shapeIndex].path.type = GetToolpathID();
            Shapes[shapeIndex].path.origin = GetOriginID();

            Shapes[shapeIndex].shape.dimX = (double)nUDShapeX.Value;
            Shapes[shapeIndex].shape.dimY = (double)nUDShapeY.Value;
            Shapes[shapeIndex].shape.dimR = (double)nUDShapeR.Value;
            Shapes[shapeIndex].shape.pocket = cBToolpathPocket.Checked;
            Shapes[shapeIndex].shape.type = GetShapeID();

            Shapes[shapeIndex].app.avoidZUp = cBNoZUp.Checked;
            Shapes[shapeIndex].app.finalMove0 = cBMoveTo00.Checked;
            Shapes[shapeIndex].app.insertShape = CbInsertCode.Checked;

            Shapes[shapeIndex].name = TbShapeName.Text;

            SaveXML(Datapath.Data + "\\simpleshapes.xml");
            Update_ShapeSelection(shapeIndex);
        }

        private void BtnApplyShape_Click(object sender, EventArgs e)
        {
            int index = CbShapeSelection.SelectedIndex;
            if ((index >= 0) && (index < Shapes.Count))
            {
                cBToolSet.Checked = false;

                SetToolProperties(Shapes[index].tool);
                SetToolpathID(Shapes[index].path.type);
                SetOriginID(Shapes[index].path.origin);
				
                SetShapeID(Shapes[index].shape.type);
                CheckSetValue(nUDShapeX, (decimal)Shapes[index].shape.dimX, true);
                CheckSetValue(nUDShapeY, (decimal)Shapes[index].shape.dimY, true);
                CheckSetValue(nUDShapeR, (decimal)Shapes[index].shape.dimR, true);
				cBToolpathPocket.Checked = Shapes[index].shape.pocket;
				
				cBNoZUp.Checked = Shapes[index].app.avoidZUp;
				cBMoveTo00.Checked = Shapes[index].app.finalMove0;
				CbInsertCode.Checked = Shapes[index].app.insertShape;
				
                TbShapeName.Text = Shapes[index].name;
            }
        }

        private void BtnDeleteShape_Click(object sender, EventArgs e)
        {
            int index = CbShapeSelection.SelectedIndex;
            if ((index >= 0) && (index < Shapes.Count))
            {
                Shapes.RemoveAt(index);
            }
            else
            { CbShapeSelection.Text = "-"; }
            SaveXML(Datapath.Data + "\\simpleshapes.xml");
            Update_ShapeSelection(index);
        }

        private void FillToolProp(ToolProp tmp)
        {
            tmp.Diameter = (float)nUDToolDiameter.Value;
            tmp.FeedXY = (float)nUDToolFeedXY.Value;
            tmp.FinalZ = (float)nUDImportGCZDown.Value;
            tmp.StepZ = (float)nUDToolZStep.Value;
            tmp.FeedZ = (float)nUDToolFeedZ.Value;
            tmp.Overlap = (float)nUDToolOverlap.Value;
            tmp.SpindleSpeed = (float)nUDToolSpindleSpeed.Value;
        }

        private void CreateSaveName()
        {
            string[] shapes = { "", "Rect", "RndRect", "Circle", "" };
            string name = string.Format("{0}_{1:0.0}_{2:0.0}_{3:0.0}", shapes[GetShapeID()], nUDShapeX.Value, nUDShapeY.Value, nUDShapeR.Value);
            TbShapeName.Text = name;
        }
    }
}
