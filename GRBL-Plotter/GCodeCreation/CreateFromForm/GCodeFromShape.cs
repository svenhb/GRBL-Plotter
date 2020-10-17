/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Drawing;

namespace GRBL_Plotter
{
    public partial class GCodeFromShape : Form
    {
        private string shapegcode = "";
        public string shapeGCode
        { get { return shapegcode; } }
        private static StringBuilder gcodeString = new StringBuilder();

        public float offsetX = 0, offsetY = 0;
        private static bool gcodeTangEnable = false;

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
            if (rBToolpath3.Checked) { dTool = -dTool; }        // outside
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
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (rBShape1.Checked)                               // rectangle
            {   getOffset(x,y);
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
                    if (!(zStepCount++ == 0) && !cBNoZUp.Checked)     // move up the 1st time 
                    {   gcode.PenUp(gcodeString);
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
                            while (((dx < x / 2) && (dy < y / 2)) && (counter++ < safety))
                            {
                                makeRect(offsetX + dx, offsetY + dy, offsetX + x - dx, offsetY + y - dy, 0, false);  // rectangle clockwise
                                dx += overlap; dy += overlap;
                                if ((dx < x / 2) && (dy < y / 2))
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
                    if (!(zStepCount++ == 0) && !cBNoZUp.Checked)     // move up the 1st time 
                    {   gcode.PenUp(gcodeString);
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
                        while (((dx < x / 2) && (dy < y / 2)) && (counter++ < safety))
                        {
                            makeRect(offsetX + dx, offsetY + dy, offsetX + x - dx, offsetY + y - dy, rDelta, false);  // rectangle clockwise
                            dx += overlap; dy += overlap; rDelta -= overlap;
                            if (dx > x / 2) { dx = x / 2; }
                            if (dy > x / 2) { dy = y / 2; }
                            if (rDelta < 0) { rDelta = 0; }
                            if ((dx < x / 2) && (dy < y / 2))
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
                offsetX -= rTool; offsetY -= rTool;
                rShape += rTool;                    // take care of tool diameter if set

                if (overlap > rShape)
                { overlap = (float)(rShape / 2.1); }

                zStep = zStart;
                while (zStep > (float)nUDImportGCZDown.Value)
                {
                    //if ((zStepCount++ == 0) || !cBNoZUp.Checked)    // move up the 1st time 
                    if (!(zStepCount++ == 0) && !cBNoZUp.Checked)     // move up the 1st time 
                    {   gcode.PenUp(gcodeString);
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
            gcode.Comment(gcodeString, Graphic2GCode.SetFigureEnd(figureCount));

            gcode.jobEnd(gcodeString, "EndJob");      // Spindle / laser off
        //    if (Properties.Settings.Default.importGCZEnable)
       //         gcode.SpindleOff(gcodeString, "Finish - Spindle off");

            string header = gcode.GetHeader("Simple Shape");
            string footer = gcode.GetFooter();

            gcodeString.Replace(',', '.');
            shapegcode = header + gcodeString.ToString() + footer;
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
        {   if (rBOrigin1.Checked) { offsetX = 0; offsetY = -y; }
            if (rBOrigin2.Checked) { offsetX = -x/2; offsetY = -y; }
            if (rBOrigin3.Checked) { offsetX = -x; offsetY = -y; }
            if (rBOrigin4.Checked) { offsetX = 0; offsetY = -y/2; }
            if (rBOrigin5.Checked) { offsetX = -x / 2; offsetY = -y/2; }
            if (rBOrigin6.Checked) { offsetX = -x; offsetY = -y/2; }
            if (rBOrigin7.Checked) { offsetX = 0; offsetY = 0; }
            if (rBOrigin8.Checked) { offsetX = -x / 2; offsetY = 0; }
            if (rBOrigin9.Checked) { offsetX = -x; offsetY = 0; }
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
                    rBShape2.Checked = true;
                    break;
                case 3:
                    rBShape3.Checked = true;
                    break;
                default:
                    rBShape1.Checked = true;
                    break;
            }
            nUDImportGCZDown.Value = Properties.Settings.Default.importGCZDown;
            switch (Properties.Settings.Default.createShapeToolPath)
            {
                case 2:
                    rBToolpath1.Checked = true;
                    break;
                case 3:
                    rBToolpath3.Checked = true;
                    break;
                default:
                    rBToolpath1.Checked = true;
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

        private void updateControls()
        { }
    }
}
