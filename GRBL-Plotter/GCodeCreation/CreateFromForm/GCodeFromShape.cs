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
/*
 * 2019-06-14 add code markers: xmlMarker.figureStart
 * 2019-08-15 add logger
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2020-12-20 bug fix: missing xmlMarker.passEnd if cBNoZUp.Checked
 * 2020-12-23 bug fix: rect-pocket innerst path some times missing
 * 2021-01-07 add edge bevel / rounding
*/

using System;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GRBL_Plotter
{
    public partial class GCodeFromShape : Form
    {
        private string shapegcode = "";
        public string shapeGCode
        { get { return shapegcode; } }
        public GraphicsPath pathBackground = new GraphicsPath();
        private GraphicsPath path;

        private static StringBuilder gcodeString = new StringBuilder();

        public float offsetX = 0, offsetY = 0;
        private static bool gcodeTangEnable = false;
        private Image picBevelOff = Properties.Resources.rndOff;
        private Image picBevelOn = Properties.Resources.rndOn;
        
//        private List<Image> edgePicOff = new List<Image>();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public GCodeFromShape()
        {
            Logger.Trace("++++++ GCodeFromShape START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {   this.Close();  }

        private void btnApply_Click(object sender, EventArgs e)
        {
            saveSettings();
            gcode.setup(false);                  // load defaults from setup-tab
            gcode.gcodeXYFeed = (float)nUDToolFeedXY.Value;    // override devault values
            gcode.gcodeZFeed  = (float)nUDToolFeedZ.Value;    // override devault values
            gcode.gcodeSpindleSpeed = (float)nUDToolSpindleSpeed.Value;    // override devault values
            gcode.gcodeZDown = (float)nUDImportGCZDown.Value;
            gcodeTangEnable = Properties.Settings.Default.importGCTangentialEnable;

            Logger.Debug("Create GCode {0}", gcode.getSettings());

            gcodeString.Clear();
            pathBackground.Reset();
            path = pathBackground;
            path.StartFigure();

            gcode.Tool(gcodeString, tprop.toolnr, tprop.name);
     //       if (!Properties.Settings.Default.importGCSpindleToggle) gcode.SpindleOn(gcodeString, "Start");

            float x, y, rShape,d,dTool,overlap,rTool,zStep;
            float zStart = 0;
            x = (float)nUDShapeX.Value;
            y = (float)nUDShapeY.Value;
            rShape = (float)nUDShapeR.Value;
            d = 2 * rShape;
            dTool = (float)nUDToolDiameter.Value;               // tool diameter;
            overlap = dTool * (float)nUDToolOverlap.Value/100;  // tool overlap
            if (rBToolpath1.Checked) { dTool = 0; }             // engrave
            if (rBToolpath3.Checked) { dTool = -dTool; }        // inside
            rTool = dTool / 2;                                  // tool radius

            int counter=0,safety = 100;
            int zStepCount = 0;
            float dx = 0, dy = 0, rDelta=0;
            int passCount = 0;
            int figureCount = 1;
            gcode.jobStart(gcodeString, "StartJob");
        //    gcode.PenUp(gcodeString);

            bool inOneStep = (nUDToolZStep.Value >= -nUDImportGCZDown.Value);

            gcode.Comment(gcodeString, xmlMarker.figureStart + " Id=\"" + figureCount.ToString() + "\" >");
            if (tabControl1.SelectedTab == tabPage1)
            {
                #region shape
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (rBShape1.Checked)                               // rectangle
                {
                    getOffset(x, y);

                    path.AddLine(offsetX, offsetY, offsetX + x, offsetY);
                    path.AddLine(offsetX + x, offsetY, offsetX + x, offsetY + y);
                    path.AddLine(offsetX + x, offsetY + y, offsetX, offsetY + y);
                    path.AddLine(offsetX, offsetY + y, offsetX, offsetY);

                    offsetX -= rTool; offsetY -= rTool;
                    x += dTool;y += dTool;                          // width +/- tool diameter (outline / inline)
                    zStep = zStart;

                    while (zStep > (float)nUDImportGCZDown.Value)   // nUDImportGCZDown.Value e.g. -2
                    {
                        zStep -= (float)nUDToolZStep.Value;         // nUDToolZStep.Value e.g.  0.5
                        if (zStep < (float)nUDImportGCZDown.Value)
                            zStep = (float)nUDImportGCZDown.Value;

                        if ((overlap > x / 2) || (overlap > y / 2))
                        {    overlap = (float)(Math.Min(x, y) / 2.1); }

                        //if ((zStepCount++ == 0) || !cBNoZUp.Checked)    // move up the 1st time 
                        if (!(zStepCount++ == 0) )     // move up the 1st time 
                        {
                            if(!cBNoZUp.Checked) gcode.PenUp(gcodeString);
                            if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ "  " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if(gcodeTangEnable)
                            gcode.setTangential(gcodeString, 90, true);

                        if (!inOneStep) gcode.Comment(gcodeString, string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", xmlMarker.passStart,passCount, zStep, nUDImportGCZDown.Value));

                        if (cBToolpathPocket.Checked)
                            gcode.MoveToRapid(gcodeString, offsetX + overlap, offsetY + overlap, ""); 
                        else
                            gcode.MoveToRapid(gcodeString, offsetX, offsetY, "");
                        gcode.gcodeZDown = zStep;               // adapt Z-deepth
                        gcode.PenDown(gcodeString);
                        if (cBToolpathPocket.Checked)           // 1st pocket
                        {   if ((x > Math.Abs(dTool)) && (y > Math.Abs(dTool)))      // wide enough for pocket
                            {
                                dx = overlap; dy = overlap;
                                while (((dx < (x + overlap)/ 2) && (dy < (y + overlap) / 2)) && (counter++ < safety))
                                {
                                    makeRect(offsetX + dx, offsetY + dy, offsetX + x - dx, offsetY + y - dy, 0, false);  // rectangle clockwise
                                    dx += overlap; dy += overlap;
                                    if ((dx < (x + overlap) / 2) && (dy < (y + overlap) / 2))
                                        gcode.MoveTo(gcodeString, offsetX + dx, offsetY + dy, "Pocket");
                                }
                                if (cBNoZUp.Checked)
                                    gcode.MoveTo(gcodeString, offsetX, offsetY, "Pocket finish");
                                else
                                {
                                    gcode.PenUp(gcodeString,"Pocket finish");
                                    gcode.MoveToRapid(gcodeString, offsetX, offsetY, "Pocket finish");
                                    gcode.PenDown(gcodeString);
                                }
                            }
                        }

                        makeRect(offsetX, offsetY, offsetX + x, offsetY + y, 0, true);  // final rectangle clockwise
                    }
                    gcode.PenUp(gcodeString);
                    if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ " " + passCount.ToString() + ">");
                }
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                else if (rBShape2.Checked)           // rectangle with round edge
                {
                    getOffset(x, y);

                    path.AddLine(offsetX + rShape, offsetY, offsetX + x - rShape, offsetY);
                    path.AddArc(offsetX + x - d, offsetY, d, d, -90, 90);

                    path.AddLine(offsetX + x, offsetY + rShape, offsetX + x, offsetY + y - rShape);
                    path.AddArc(offsetX + x - d, offsetY + y - d, d, d, 0, 90);

                    path.AddLine(offsetX + x - rShape, offsetY + y, offsetX + rShape, offsetY + y);
                    path.AddArc(offsetX , offsetY + y - d, d, d, 90, 90);

                    path.AddLine(offsetX, offsetY + y - rShape, offsetX, offsetY + rShape);
                    path.AddArc(offsetX , offsetY , d, d, 180, 90);

                    offsetX -= rTool; offsetY -= rTool;
                    x += dTool; y += dTool;

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
                            if (!cBNoZUp.Checked) gcode.PenUp(gcodeString);
                            if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ " " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if (gcodeTangEnable)
                            gcode.setTangential(gcodeString, 90, true);

                        if (!inOneStep) gcode.Comment(gcodeString, string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", xmlMarker.passStart, passCount, zStep, nUDImportGCZDown.Value));

                        if (cBToolpathPocket.Checked)
                            gcode.MoveToRapid(gcodeString, offsetX + overlap, offsetY + rShape , "");
                        else
                            gcode.MoveToRapid(gcodeString, offsetX, offsetY + rShape, "");
                        gcode.gcodeZDown = zStep;               // adapt Z-deepth
                        gcode.PenDown(gcodeString);
                        if (cBToolpathPocket.Checked)
                        {
                            dx = overlap; dy = overlap; rDelta = rShape - overlap;
                            while (((dx < (x + overlap) / 2) && (dy < (y + overlap) / 2)) && (counter++ < safety))
                            {
                                makeRect(offsetX + dx, offsetY + dy, offsetX + x - dx, offsetY + y - dy, rDelta, false);  // rectangle clockwise
                                dx += overlap; dy += overlap; rDelta -= overlap;
                                if (dx > (x + overlap) / 2) { dx = (x + overlap) / 2; }
                                if (dy > (x + overlap) / 2) { dy = (y + overlap) / 2; }
                                if (rDelta < overlap) { rDelta = 0; }
                                if ((dx < (x + overlap) / 2) && (dy < (y + overlap) / 2))
                                    gcode.MoveTo(gcodeString, offsetX + dx, offsetY + dy + rDelta, "");
                            }
                            if (cBNoZUp.Checked)
                                gcode.MoveTo(gcodeString, offsetX, offsetY + rShape, "");
                            else
                            {
                                gcode.PenUp(gcodeString);
                                gcode.MoveToRapid(gcodeString, offsetX, offsetY + rShape, "");
                                gcode.PenDown(gcodeString);
                            }
                        }
                        makeRect(offsetX, offsetY, offsetX + x, offsetY + y, rShape, true);  // rectangle clockwise
                    }
                    gcode.PenUp(gcodeString);
                    if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ " " + passCount.ToString() + ">");
                }
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                else if (rBShape3.Checked)           // circle
                {
                    getOffset(d, d);
                    path.AddArc(offsetX , offsetY , d, d, 0, 360);

                    offsetX -= rTool; offsetY -= rTool;
                    rShape += rTool;                    // take care of tool diameter if set

                    if (overlap > rShape)
                    { overlap = (float)(rShape / 2.1); }

                    zStep = zStart;

                    while (zStep > (float)nUDImportGCZDown.Value)
                    {
                        //if ((zStepCount++ == 0) || !cBNoZUp.Checked)    // move up the 1st time 
                        if (!(zStepCount++ == 0) )     // move up the 1st time 
                        {
                            if (!cBNoZUp.Checked) gcode.PenUp(gcodeString);
                            if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ " " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if (gcodeTangEnable)
                            gcode.setTangential(gcodeString, 90, true);


                        if (!inOneStep) gcode.Comment(gcodeString, string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", xmlMarker.passStart, passCount, zStep, nUDImportGCZDown.Value));

                        if (cBToolpathPocket.Checked)
                            gcode.MoveToRapid(gcodeString, offsetX + rShape-overlap, offsetY + rShape, "");
                        else
                            gcode.MoveToRapid(gcodeString, offsetX, offsetY + rShape, "");
                        zStep -= (float)nUDToolZStep.Value;
                        if (zStep < (float)nUDImportGCZDown.Value)
                            zStep = (float)nUDImportGCZDown.Value;
                        gcode.gcodeZDown = zStep;               // adapt Z-deepth
                        gcode.PenDown(gcodeString);
                        rDelta = overlap;
                        counter = 0;
                        if ((cBToolpathPocket.Checked) && (rShape > 2*rTool))
                        {   while ((rDelta < rShape) && (counter++ < safety))
                            {
                                gcode.setTangential(gcodeString, -270, false);  //
                                gcode.Arc(gcodeString, 2, offsetX + rShape - rDelta, offsetY + rShape, rDelta, 0, "");
                                rDelta += overlap;
                                if (rDelta < rShape)
                                    gcode.MoveTo(gcodeString, offsetX + rShape - rDelta, offsetY + rShape, "");
                            }
                            gcode.MoveTo(gcodeString, offsetX , offsetY + rShape, "");
                        }
                        gcode.setTangential(gcodeString, -270, false);  //
                        gcode.Arc(gcodeString, 2, offsetX, offsetY + rShape, rShape, 0, "");
                    }
                    gcode.PenUp(gcodeString);
                    if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ " " + passCount.ToString() + ">");
                }
                #endregion
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                {
                    rShape = (float)nUDBevelR.Value;
                    getOffset(rShape, rShape);
                    double rPath = rShape + rTool;                                // rTool is neg if inside shape, 0 if on shape, pos if outside shape
                    double xStart=0, yStart=0;
                    double xEnd=0, yEnd=0;
                    double xOff=0, yOff=0;
                    double i=0, j=0;
                    double aStart=0, aEnd=0;
                    
                    bool isRound = rBBevel1.Checked;
                                        
/* Calculate start-pos from circle-center, then move to lower left of quadrant*/
                    if (rB1.Checked) {  
                        xStart = 0; yStart = rPath;     xOff = rShape; yOff = 0;    aStart = isRound? 180:225; aEnd = isRound? 270:225;
                        xEnd = -rPath; yEnd = 0;    i = 0; j = -rPath; 
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));      // draw edge
                        path.AddLine((float)(-rShape + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY));
                        path.StartFigure();
                        if (isRound)
                            path.AddArc((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(2*rShape), (float)(2*rShape), 90, 90);      // draw edge
                        else
                            path.AddLine((float)(0 + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(0 + yOff + offsetY));                            
                        }
                    if (rB2.Checked) {  
                        xStart = rPath; yStart = 0;     xOff = 0; yOff = 0;         aStart =  isRound? 90:135; aEnd = isRound? 180:135;
                        xEnd = 0; yEnd = rPath;     i = -rPath; j = 0; 
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY), (float)(-rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));
                        path.StartFigure();
                        if (isRound)
                            path.AddArc((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(2 * rShape), (float)(2 * rShape), 0, 90);      // draw edge
                        else
                            path.AddLine((float)(rShape + xOff + offsetX), (float)(0 + yOff + offsetY), (float)(0 + xOff + offsetX), (float)(rShape + yOff + offsetY));
                        }
                    if (rB3.Checked) {  
                        xStart = 0; yStart = -rPath;    xOff = 0; yOff = rShape;    aStart =  isRound? 0:45; aEnd = isRound? 90:45;
                        xEnd = rPath; yEnd = 0;     i = 0; j = rPath; 
                        path.AddLine((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY));
                        path.AddLine((float)(rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(rShape + yOff + offsetY));
                        path.StartFigure();
                        if (isRound)
                            path.AddArc((float)(-rShape + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(2 * rShape), (float)(2 * rShape), 270, 90);      // draw edge
                        else
                            path.AddLine((float)(0 + xOff + offsetX), (float)(-rShape + yOff + offsetY), (float)(rShape + xOff + offsetX), (float)(0 + yOff + offsetY));
                        }
                    if (rB4.Checked) {  
                        xStart = -rPath; yStart = 0;    xOff = rShape; yOff = rShape; aStart =  isRound? 270:315; aEnd =  isRound? 360:315;
                        xEnd = 0; yEnd = -rPath;    i = rPath; j = 0; 
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

                        if (!(zStepCount++ == 0) )     // move up the 1st time 
                        {
                            if(!cBNoZUp.Checked) gcode.PenUp(gcodeString);
                            if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ "  " + passCount.ToString() + ">");
                        }
                        passCount++;

                        if (!inOneStep) gcode.Comment(gcodeString, string.Format("{0} Nr=\"{1}\" step=\"{2:0.000}\" final=\"{3:0.000}\">", xmlMarker.passStart,passCount, zStep, nUDImportGCZDown.Value));
                        
                        if (gcodeTangEnable) gcode.setTangential(gcodeString, aStart, true);
                        gcode.MoveToRapid(gcodeString, (float)(xStart + xOff + offsetX), (float)(yStart + yOff + offsetY), "");
                        
                        gcode.gcodeZDown = zStep;               // adapt Z-deepth
                        gcode.PenDown(gcodeString);

                        gcode.setTangential(gcodeString, aEnd, false);  //
                        if (isRound)
                            gcode.Arc(gcodeString, 3, (float)(xEnd + xOff + offsetX), (float)(yEnd + yOff + offsetY), (float)i, (float)j,  ""); 
                        else
                            gcode.MoveTo(gcodeString, (float)(xEnd + xOff + offsetX), (float)(yEnd + yOff + offsetY),  ""); 

                    }
                    gcode.PenUp(gcodeString);
                    if (!inOneStep) gcode.Comment(gcodeString, xmlMarker.passEnd + ">"); //+ " " + passCount.ToString() + ">");
                }

            }
            gcode.Comment(gcodeString, Graphic2GCode.SetFigureEnd(figureCount));

            gcode.jobEnd(gcodeString, "EndJob");      // Spindle / laser off
        //    if (Properties.Settings.Default.importGCZEnable)
       //         gcode.SpindleOff(gcodeString, "Finish - Spindle off");

            string header = gcode.GetHeader("Simple Shape");
            string footer = gcode.GetFooter();

            gcodeString.Replace(',', '.');
            shapegcode = header + gcodeString.ToString() + footer;
        }

        private static void createGraphicsPathAddCorner()
        {
            GraphicsPath path = VisuGCode.pathBackground;
            path.StartFigure();
        }

        private void makeRect(float x1, float y1, float x2, float y2, float r, bool cw=true )
        {   // start bottom left
            if (cw)
            {
                gcode.MoveTo(gcodeString, x1, y2 - r, "cw 1");          //BL to TL
                setTangentialUpDown(0, !(r > 0));
                if (r > 0) { gcode.Arc(gcodeString, 2, x1 + r, y2, r, 0, ""); }
                gcode.MoveTo(gcodeString, x2 - r, y2, "cw 2");          // TL to TR
                setTangentialUpDown(-90, !(r > 0));
                if (r > 0) { gcode.Arc(gcodeString, 2, x2, y2 - r, 0, -r,  ""); }
                gcode.MoveTo(gcodeString, x2, y1 + r, "cw 3");          // TR to BR
                setTangentialUpDown(-180, !(r > 0));
                if (r > 0) { gcode.Arc(gcodeString, 2, x2 - r, y1, -r, 0,  ""); }
                gcode.MoveTo(gcodeString, x1 + r, y1, "cw 4");          // BR to BL
                if (r > 0) gcode.setTangential(gcodeString, -270, !(r > 0));  //
                if (r > 0) { gcode.Arc(gcodeString, 2, x1, y1 + r, 0, r,  ""); }
            }
            else
            {
                setTangentialUpDown(0, !(r > 0));
                if (r > 0) { gcode.Arc(gcodeString, 3, x1 + r, y1, r, 0,  ""); }
                gcode.MoveTo(gcodeString, x2 - r, y1, "ccw 1");          // to BR
                setTangentialUpDown(90, !(r > 0));
                if (r > 0) { gcode.Arc(gcodeString, 3, x2, y1 + r, 0, r,  ""); }
                gcode.MoveTo(gcodeString, x2, y2 - r, "ccw 2");           // to TR
                setTangentialUpDown(180, !(r > 0));
                if (r > 0) { gcode.Arc(gcodeString, 3, x2 - r, y2, -r, 0,  ""); }
                gcode.MoveTo(gcodeString, x1 + r, y2, "ccw 3");           // to TL
                setTangentialUpDown(270, !(r > 0));
                if (r > 0) { gcode.Arc(gcodeString, 3, x1, y2 - r, 0, -r,  ""); }
                gcode.MoveTo(gcodeString, x1, y1 + r, "ccw 4");           // to BL 
            }
        }

        private void setTangentialUpDown(double angle, bool penUp)
        {
            double angleNew = (double)Properties.Settings.Default.importGCTangentialTurn * angle / 360;
            if (gcodeTangEnable)
            {   if (penUp)
                {   gcode.PenUp(gcodeString);
                    gcodeString.AppendFormat("G00 {0}{1:0.000}\r\n", Properties.Settings.Default.importGCTangentialAxis, angleNew);
                    gcode.PenDown(gcodeString);
                }
                gcode.setTangential(gcodeString, angle);  //
            }
        }

        private void getOffset(float x, float y)
        {   if (rBOrigin1.Checked) { offsetX = 0; offsetY = -y; }               // upper left
            if (rBOrigin2.Checked) { offsetX = -x/2; offsetY = -y; }
            if (rBOrigin3.Checked) { offsetX = -x; offsetY = -y; }              // upper right
            if (rBOrigin4.Checked) { offsetX = 0; offsetY = -y/2; }
            if (rBOrigin5.Checked) { offsetX = -x / 2; offsetY = -y/2; }        // center
            if (rBOrigin6.Checked) { offsetX = -x; offsetY = -y/2; }
            if (rBOrigin7.Checked) { offsetX = 0; offsetY = 0; }                // origin - lower left
            if (rBOrigin8.Checked) { offsetX = -x / 2; offsetY = 0; }
            if (rBOrigin9.Checked) { offsetX = -x; offsetY = 0; }               // lower right
        }

        private void ShapeToGCode_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationShapeForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            nUDToolDiameter.Value = Properties.Settings.Default.createShapeToolDiameter;
            nUDToolZStep.Value = Properties.Settings.Default.createShapeToolZStep;
            nUDToolFeedXY.Value = Properties.Settings.Default.createShapeToolFeedXY;
            nUDToolFeedZ.Value = Properties.Settings.Default.createShapeToolFeedZ;
            nUDToolOverlap.Value = Properties.Settings.Default.createShapeToolOverlap;
            nUDToolSpindleSpeed.Value = Properties.Settings.Default.createShapeToolSpindleSpeed;
            nUDShapeX.Value = Properties.Settings.Default.createShapeX;
            nUDShapeY.Value = Properties.Settings.Default.createShapeY;
            nUDShapeR.Value = Properties.Settings.Default.createShapeR;
            switch (Properties.Settings.Default.createShapeType)
            {
                case 2:
                    rBShape2.Checked = true;    // circle
                    break;
                case 3:
                    rBShape3.Checked = true;    // rectangle round edge
                    break;
                default:
                    rBShape1.Checked = true;    // rectangle
                    break;
            }
            nUDImportGCZDown.Value = Properties.Settings.Default.importGCZDown;
            switch (Properties.Settings.Default.createShapeToolPath)
            {
                case 2:
                    rBToolpath2.Checked = true;     // outside shape
                    break;
                case 3:
                    rBToolpath3.Checked = true;     // inside shape
                    break;
                default:
                    rBToolpath1.Checked = true;     // on shape
                    break;
            }
            switch (Properties.Settings.Default.createShapeOrigin)
            {
                case 1:
                    rBOrigin1.Checked = true;
                    break;
                case 2:
                    rBOrigin2.Checked = true;
                    break;
                case 3:
                    rBOrigin3.Checked = true;
                    break;
                case 4:
                    rBOrigin4.Checked = true;
                    break;
                case 6:
                    rBOrigin6.Checked = true;
                    break;
                case 7:
                    rBOrigin7.Checked = true;
                    break;
                case 8:
                    rBOrigin8.Checked = true;
                    break;
                case 9:
                    rBOrigin9.Checked = true;
                    break;
                default:
                    rBOrigin5.Checked = true;
                    break;
            }
            int toolCount = toolTable.init();
            toolProp tmpTool;
            bool defaultToolFound = false;
            for (int i = 0; i < toolCount; i++)
            {
                tmpTool = toolTable.getToolProperties(i);
                if (i == tmpTool.toolnr)
                {
                    cBTool.Items.Add(i.ToString() + ") " + tmpTool.name);
                    if (i == Properties.Settings.Default.importGCToolDefNr)
                    {   cBTool.SelectedIndex = cBTool.Items.Count - 1;
                        defaultToolFound=true;
                    }
                }
            }
            if (!defaultToolFound)
                cBTool.SelectedIndex = 0;
            tprop = toolTable.getToolProperties(1);
            enableTool(!cBToolSet.Checked);
            cBToolSet_CheckedChanged(sender, e);

            rB1.Image = (Image)picBevelOff.Clone();
            rB2.Image = (Image)picBevelOff.Clone(); rB2.Image.RotateFlip((RotateFlipType.Rotate90FlipNone));
            rB3.Image = (Image)picBevelOff.Clone(); rB3.Image.RotateFlip((RotateFlipType.Rotate180FlipNone));
            rB4.Image = (Image)picBevelOff.Clone(); rB4.Image.RotateFlip((RotateFlipType.Rotate270FlipNone));
        }

        private void ShapeToGCode_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ GCodeFromShape STOP ++++++");
            Properties.Settings.Default.locationShapeForm = Location;
            saveSettings();
        }
        private void saveSettings()
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
            if (rBShape1.Checked) Properties.Settings.Default.createShapeType = 1;
            if (rBShape2.Checked) Properties.Settings.Default.createShapeType = 2;
            if (rBShape3.Checked) Properties.Settings.Default.createShapeType = 3;
            Properties.Settings.Default.importGCZDown = nUDImportGCZDown.Value;
            if (rBToolpath1.Checked) Properties.Settings.Default.createShapeToolPath = 1;
            if (rBToolpath2.Checked) Properties.Settings.Default.createShapeToolPath = 2;
            if (rBToolpath3.Checked) Properties.Settings.Default.createShapeToolPath = 3;
            if (rBOrigin1.Checked) Properties.Settings.Default.createShapeOrigin = 1;
            if (rBOrigin2.Checked) Properties.Settings.Default.createShapeOrigin = 2;
            if (rBOrigin3.Checked) Properties.Settings.Default.createShapeOrigin = 3;
            if (rBOrigin4.Checked) Properties.Settings.Default.createShapeOrigin = 4;
            if (rBOrigin5.Checked) Properties.Settings.Default.createShapeOrigin = 5;
            if (rBOrigin6.Checked) Properties.Settings.Default.createShapeOrigin = 6;
            if (rBOrigin7.Checked) Properties.Settings.Default.createShapeOrigin = 7;
            if (rBOrigin8.Checked) Properties.Settings.Default.createShapeOrigin = 8;
            if (rBOrigin9.Checked) Properties.Settings.Default.createShapeOrigin = 9;
            Properties.Settings.Default.Save();
        }

        private void nUDShapeR_ValueChanged(object sender, EventArgs e)
        {   if (rBShape2.Checked)
            {
                decimal min = Math.Min(nUDShapeX.Value, nUDShapeY.Value);
                if (nUDShapeR.Value > min / 2)
                    nUDShapeR.Value = min / 2;
            }
        }

        private void cBToolSet_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = cBToolSet.Checked;
            label1.Enabled = enabled;
            cBTool.Enabled = enabled;
            enableTool(!cBToolSet.Checked);
            cBTool_SelectedIndexChanged(sender, e);
        }

        private void enableTool(bool state)
        {   nUDToolDiameter.Enabled = state;
            nUDToolFeedXY.Enabled = state;
            nUDToolZStep.Enabled = state;
            nUDToolFeedZ.Enabled = state;
            nUDToolOverlap.Enabled = state;
            nUDToolSpindleSpeed.Enabled = state;
        }

        toolProp tprop;
        private void cBTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tmp = cBTool.SelectedItem.ToString();
            if (tmp.IndexOf(")") > 0)
            {
                int tnr = int.Parse(tmp.Substring(0, tmp.IndexOf(")")));
                Properties.Settings.Default.importGCToolDefNr = tnr;
                if (cBToolSet.Checked)
                {
                    tprop = toolTable.getToolProperties(tnr);
                    Logger.Info("Tool dia:{0}, feedXY:{1}, finalZ:{2}, stepZ:{3}, feedZ:{4}, overlap:{5}, spindle:{6}", tprop.diameter, tprop.feedXY, tprop.finalZ, tprop.stepZ, tprop.feedZ, tprop.overlap, tprop.spindleSpeed);
                    try
                    {
                        checkSetValue(nUDToolDiameter, (decimal)tprop.diameter);
                        checkSetValue(nUDToolFeedXY, (decimal)tprop.feedXY);
                        checkSetValue(nUDImportGCZDown, (decimal)tprop.finalZ);
                        checkSetValue(nUDToolZStep, (decimal)Math.Abs(tprop.stepZ));
                        checkSetValue(nUDToolFeedZ, (decimal)tprop.feedZ);
                        checkSetValue(nUDToolOverlap, (decimal)tprop.overlap);
                        checkSetValue(nUDToolSpindleSpeed, (decimal)tprop.spindleSpeed);
                    }
                    catch (Exception err)
                    { Logger.Error(err, "Set numeric Up Downs"); }
                }
            }
        }
        private void checkSetValue(NumericUpDown nUDcontrol, decimal val)
        {   if ((val <= nUDcontrol.Maximum) && (val >= nUDcontrol.Minimum))
            {   nUDcontrol.Value = val; }
            else
            {   Logger.Error("{0} Value:{1} out of range min:{2} max:{3}", nUDcontrol.Name, val, nUDcontrol.Minimum, nUDcontrol.Maximum);
                nUDcontrol.Enabled = true;
            }
        }


        RadioButton cBold = null, cBnow = null;
        private void rB1_CheckedChanged(object sender, EventArgs e)
        {
            cBnow = ((RadioButton)sender);
            setImage(cBnow, true);
            if (cBnow.Name == "rB1") rBOrigin1.PerformClick();
            if (cBnow.Name == "rB2") rBOrigin3.PerformClick();
            if (cBnow.Name == "rB3") rBOrigin9.PerformClick();
            if (cBnow.Name == "rB4") rBOrigin7.PerformClick();
            if (cBold != null)
            {   setImage(cBold, false);            }
            cBold = cBnow;
            btnApply.Enabled = true;
        }
        private void setImage(RadioButton tmp, bool on)
        {   if (tmp.Name == "rB1") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); }
            if (tmp.Name == "rB2") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); tmp.Image.RotateFlip((RotateFlipType.Rotate90FlipNone)); }
            if (tmp.Name == "rB3") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); tmp.Image.RotateFlip((RotateFlipType.Rotate180FlipNone)); }
            if (tmp.Name == "rB4") { tmp.Image = on ? (Image)picBevelOn.Clone() : (Image)picBevelOff.Clone(); tmp.Image.RotateFlip((RotateFlipType.Rotate270FlipNone)); }
        }
        private void setRBEnable(RadioButton tmp, bool en)
        {   rB1.Enabled = en;
            rB2.Enabled = en;
            rB3.Enabled = en;
            rB4.Enabled = en;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2)
            {   btnApply.Enabled = (rB1.Checked || rB2.Checked || rB3.Checked || rB4.Checked);  }
            else
                btnApply.Enabled = true;
        }

        private void rBBevel1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBBevel1.Checked)
            {   picBevelOff = Properties.Resources.rndOff;
                picBevelOn = Properties.Resources.rndOn;
            }
            else            
            {   picBevelOff = Properties.Resources.bevOff;
                picBevelOn = Properties.Resources.bevOn;
            }
            setImage(rB1, rB1.Checked);
            setImage(rB2, rB2.Checked);
            setImage(rB3, rB3.Checked);
            setImage(rB4, rB4.Checked);
        }

        private void updateControls()
        { }
    }
}
