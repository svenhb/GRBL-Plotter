/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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

/* Level 1: import graphics SVG, DXF, HPGL, Drill, 
 *          extract objects, coordinates, colors, groups
 *
 * Level 2: plotterRelated: collect blocks, sort groups, code repition, tangential axis
 *
 * Level 3: gcodeRelated: implement Pen up/down options, avoid G23, cutter correction, write GCode commands 
*/

/*  GCodeFromDXF.cs a static class to convert DXF data into G-Code 
 *  Many thanks to https://github.com/mkernel/DXFLib
 *  
 *  Spline conversion is faulty if more than 4 point are given
 *  Not implemented by me up to now: 
 *      Image, Dimension
 *      Transform: rotation, scaling
 *      
 * 2019-02-06 bug fix block-offset for LWPolyline and Text
 * 2019-05-10 reactivate else in line 298 to avoid double coordinates
 * 2019-06-10 define "<PD" as xmlMarker.figureStart
 * 2019-07-04 fix sorting problem with <figure tag
 * 2019-08-15 add logger
 * 2019-08-25 get layer color
 * 2019-09-06 swap code to new class 'plotter'
 * 2019-10-02 add nodes only
 * 2019-11-24 fix DXFLib.dll for ellipse support, fix spline support, Code outsourcing to importMath.cs
 * 2019-11-26 add try/catch for dxf.load
 * 2019-12-07 add extended log
 * 2020-01-08 bug fix convert 'Point' line 323
 * 2020-02-19 bug fix round corners in blocks
 * 2020-02-22 updated DXFLib.dll is needed (DXFInsert-RotationAngle, DXFEllipse)
*/

using System;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using System.IO;
using DXFLib;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;

namespace GRBL_Plotter //DXFImporter
{
    class GCodeFromDXF
    {
        private static bool gcodeReduce = false;            // if true remove G1 commands if distance is < limit
        private static double gcodeReduceVal = .1;          // limit when to remove G1 commands
        private static bool dxfComments = true;             // if true insert additional comments into GCode
        private static bool dxfUseColorIndex = false;       // use DXF color index instead real color
        private static int toolNr = 1;
        private static int toolToUse = 1;
        private static int dxfBezierAccuracy = 6;           // applied line segments at bezier curves
        private static int dxfColorID = -1;
        private static int dxfColorIDold = -1;
        private static string dxfColorHex = "";
        private static bool groupObjects = false;
        private static bool nodesOnly = false;              // if true only do pen-down -up on given coordinates

        private static Dictionary<string, int> layerColor = new Dictionary<string, int>();
        private static Dictionary<string, string> layerLType = new Dictionary<string, string>();
        private static Dictionary<string, double[]> lineTypes = new Dictionary<string, double[]>();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Entrypoint for conversion: apply file-path 
        /// </summary>
        /// <param name="file">String keeping file-name or URL</param>
        /// <returns>String with GCode of imported data</returns>
        public static string ConvertFromText(string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(byteArray);
            loadDXF(stream);
            return convertDXF("from Clipboard");                  
        }
        public static string ConvertFromFile(string file)
        {   if (file == "")
            { MessageBox.Show("Empty file name"); return ""; }

            if (file.Substring(0, 4) == "http")
            {
                string content = "";
                using (var wc = new System.Net.WebClient())
                { try { content = wc.DownloadString(file); }
                    catch { MessageBox.Show("Could not load content from " + file); return ""; }
                }
                int pos = content.IndexOf("dxfrw");
                if ((content != "") && (pos >= 0) && (pos < 8))
                { try
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(content);
                        MemoryStream stream = new MemoryStream(byteArray);
                        if (!loadDXF(stream))
                            return "(File could not be loaded)";
                    }
                    catch (Exception e)
                    { MessageBox.Show("Error '" + e.ToString() + "' in DXF file " + file); }
                }
                else
                    MessageBox.Show("This is probably not a DXF document.\r\nFirst line: " + content.Substring(0, 50));
            }
            else
            {
                if (File.Exists(file))
                {
                    try
                    {   if (!loadDXF(file))
                            return "(File could not be loaded)";
                    }
                    catch (Exception e)
                    { MessageBox.Show("Error '" + e.ToString() + "' in DXF file " + file); return ""; }
                }
                else { MessageBox.Show("File does not exist: " + file); return ""; }
            }
            return convertDXF(file);
        }

        /// <summary>
        /// Convert DXF and create GCode
        /// </summary>
        private static string convertDXF(string txt)
        {
            Logger.Debug("convertDXF {0}", txt);

            dxfBezierAccuracy = (int)Properties.Settings.Default.importBezierLineSegmentsCnt;
            gcodeReduce = Properties.Settings.Default.importRemoveShortMovesEnable;
            gcodeReduceVal = (double)Properties.Settings.Default.importRemoveShortMovesLimit;
            dxfComments = Properties.Settings.Default.importSVGAddComments;
            dxfUseColorIndex = Properties.Settings.Default.importDXFToolIndex;       // use DXF color index instead real color
            groupObjects = Properties.Settings.Default.importGroupObjects;           // DXF-Import group objects
            nodesOnly = Properties.Settings.Default.importSVGNodesOnly;

            dxfColorID = 259;
            dxfColorIDold = dxfColorID;
            toolToUse = toolNr = 1;

            if (gcode.loggerTrace) Logger.Trace("convertDXF init");
            Plotter.StartCode();        // initalize variables
            GetVectorDXF();             // convert graphics
            Plotter.SortCode();         // sort objects
            return Plotter.FinalGCode("DXF import", txt);
        }

        /// <summary>
        /// Load and parse DXF code
        /// </summary>
        /// <param name="filename">String keeping file-name</param>
        /// <returns></returns>
        private static DXFDocument doc;
        private static bool loadDXF(string filename)
        {   doc = new DXFDocument();
            try { doc.Load(filename); }
            catch (Exception er)
            {   Logger.Error(er, "loading the file failed ");
                MessageBox.Show("The file could not be opened - perhaps already open in other application?\r\n" + er.ToString());
                return false;
            }
            return true;
        }
        private static bool loadDXF(Stream content)
        {   doc = new DXFDocument();
            try { doc.Load(content); }
            catch (Exception er)
            {   Logger.Error(er, "loading the file failed ");
                MessageBox.Show("The file could not be opened - perhaps already open in other application?\r\n" + er.ToString());
                return false;
            }
            return true;
        }
        private static void GetVectorDXF()
        {
            Plotter.PenUp("DXF Start");
            layerColor.Clear();
            layerLType.Clear();
            lineTypes.Clear();

            List<DXFLayerRecord> tst = doc.Tables.Layers;
            foreach (DXFLayerRecord rec in tst)
            {
                Plotter.AddToHeader(string.Format("Layer: {0} , color: {1} , line type: {2}", rec.LayerName, rec.Color, rec.LineType));
                layerColor.Add(rec.LayerName, rec.Color);
                layerLType.Add(rec.LayerName, rec.LineType);
            }

            List<DXFLineTypeRecord> ltypes = doc.Tables.LineTypes;
            foreach (DXFLineTypeRecord lt in ltypes)
            {
                //  Plotter.AddToHeader(string.Format("Description: {0} , Name: {1} , length: {2}", lt.Description, lt.LineTypeName, lt.PatternLength));
                string pattern = "";
                if ((lt.PatternLength > 0)&&(lt.ElementCount > 0))
                {   double[] tmp = new double[lt.ElementCount];
                    for (int i = 0; i < lt.ElementCount; i++)
                    {   if (lt.Elements[i].Length == 0)
                            tmp[i] = 0.5;
                        else
                            tmp[i] = Math.Abs(lt.Elements[i].Length);
                        pattern += string.Format(" {0} ", lt.Elements[i].Length);
                    }
                    lineTypes.Add(lt.LineTypeName,tmp);
    //                Plotter.AddToHeader(string.Format("Name: {0} , length: {1}", lt.LineTypeName, pattern));
                }
            }

            foreach (DXFEntity dxfEntity in doc.Entities)				// process all entities
            {
                if (dxfEntity.GetType() == typeof(DXFInsert))			// any block to insert?
                {
                    DXFInsert ins = (DXFInsert)dxfEntity;               // get block coordinates

                    DXFPoint insertion = ins.InsertionPoint;
                    double insertionAngle = 0;

                    if (ins.RotationAngle != null)                                  // insertion angle in degrees
                        insertionAngle = (double)ins.RotationAngle;

                    if (gcode.loggerTrace) Logger.Trace("Block: at X{0:0.00}  Y{1:0.00}  a{2:0.00}", insertion.X, insertion.Y, ins.RotationAngle);

                    foreach (DXFBlock block in doc.Blocks)
                    {   if (block.BlockName.ToString() == ins.BlockName)
                        {   if (gcode.loggerTrace) Logger.Trace("Block: {0}", block.BlockName);
                            dxfColorID = block.ColorNumber;
                            Plotter.PathName = "Block:"+block.BlockName;
                            Plotter.AddToHeader(string.Format("Block: {0} at X{1:0.000}  Y{2:0.000}  a{3:0.00}" ,block.BlockName, insertion.X, insertion.Y, ins.RotationAngle));
                            foreach (DXFEntity blockEntity in block.Children)
                            {   processEntities(blockEntity, insertion, insertionAngle, false);   }
                        }
                    }
                }
                else
                {
                    DXFPoint empty = new DXFPoint();
                    empty.X = 0; empty.Y = 0;
                    processEntities(dxfEntity, empty, 0); }
            }
            Plotter.PenUp("DXF End");
        }

        /// <summary>
        /// Parse DXF entities
        /// </summary>
        /// <param name="entity">Entity to convert</param>
        /// <param name="offsetX">Offset to apply if called by block insertion</param>
        /// <returns></returns>                       
        private static void processEntities(DXFEntity entity, DXFPoint offset, double offsetAngle=0, bool updateColor=true)		// double offsetX=0, double offsetY=0
        {
            int index = 0;
			DXFPoint position = new DXFPoint();
            position.X = 0; position.Y = 0;

            if (updateColor)
            {   dxfColorID = entity.ColorNumber;
                Plotter.PathName = "Layer:"+entity.LayerName;
            }

            Plotter.PathDashArray = new double[0];                  // default no dashes
            if ((entity.LineType==null) || (entity.LineType == "ByLayer"))
            {   if (layerLType.ContainsKey(entity.LayerName))       // check if layer name is known
                {   string dashType = layerLType[entity.LayerName]; // get name of pattern
                    if (lineTypes.ContainsKey(dashType))            // check if pattern name is known
                        Plotter.PathDashArray = lineTypes[dashType];// apply pattern
                }
            }
            else
            {   if (lineTypes.ContainsKey(entity.LineType))            // check if pattern name is known
                     Plotter.PathDashArray = lineTypes[entity.LineType];// apply pattern
            }

            if (dxfColorID > 255)
                if (layerColor.ContainsKey(entity.LayerName))
                    dxfColorID = layerColor[entity.LayerName];

            if (dxfColorID < 0) dxfColorID = 0;
            if (dxfColorID >255) dxfColorID = 7;
            if (Properties.Settings.Default.importDXFSwitchWhite && (dxfColorID == 7))
                dxfColorID = 0;

            dxfColorHex = getColorFromID(dxfColorID);
            Plotter.PathColor = dxfColorHex;

            if (dxfUseColorIndex)
                toolNr = dxfColorID + 1;      // avoid ID=0 to start tool-table with index 1
            else
            {   toolNr = toolTable.getToolNr(dxfColorHex, 0);
            }

            Plotter.SetGroup(toolNr);       // set index if grouping and tool

            if (dxfColorIDold != dxfColorID)
            {   
                Plotter.PenUp("DXF new ID");

                toolToUse = toolNr;
                if (Properties.Settings.Default.importGCToolTableUse && Properties.Settings.Default.importGCToolDefNrUse)
                    toolToUse = (int)Properties.Settings.Default.importGCToolDefNr;

                Plotter.PathToolNr = toolToUse;

                if (!groupObjects)
                {   if (dxfUseColorIndex)
                        Plotter.ToolChange(toolToUse, dxfColorID.ToString());   // add tool change commands (if enabled) and set XYFeed etc.
                    else
                        Plotter.ToolChange(toolToUse, dxfColorHex);
                }
            }
            dxfColorIDold = dxfColorID;

            if (gcode.loggerTrace) Logger.Trace("Entity: {0}", entity.GetType().ToString());

            #region DXFPoint
            if (entity.GetType() == typeof(DXFPointEntity))
            {
                DXFPointEntity point = (DXFPointEntity)entity;
				position = calcPosition(point.Location, offset, offsetAngle);
                if (gcode.loggerTrace) Logger.Trace(" Point: {0:0.00};{1:0.00} ", position.X, position.Y);
                gcodeDotOnly(position, "Start Point");
            }
			#endregion
            #region DXFLWPolyline
            else if (entity.GetType() == typeof(DXFLWPolyLine))
            {
                DXFLWPolyLine lp = (DXFLWPolyLine)entity;
                index = 0;
                double bulge = 0;
                DXFLWPolyLine.Element coordinate;
                bool roundcorner = false;
                for (int i = 0; i < lp.VertexCount; i++)
                {
                    coordinate = lp.Elements[i];
                    bulge = coordinate.Bulge;
					position = calcPosition(coordinate.Vertex, offset, offsetAngle);

                    if (i == 0)
                    {   if (!nodesOnly)
                        {   dxfStartPath(position, "Start LWPolyLine - Nr pts " + lp.VertexCount.ToString());
                            Plotter.IsPathReduceOk = true;
                        }
                        else { gcodeDotOnly(position, "Start LWPolyLine"); }
                        if (gcode.loggerTrace) Logger.Trace("Start LWPolyLine {0}", lp.VertexCount);
                    }

                    if ((!roundcorner) && (i > 0))
                    {   if (gcode.loggerTrace) Logger.Trace(" dxfMoveTo {0}",i);
                        dxfMoveTo(position, "");
                    }
                    if (bulge != 0)
                    {
                        if (gcode.loggerTrace) Logger.Trace(" addRoundCorner {0}", i);

                        if (i < (lp.VertexCount - 1))
                            AddRoundCorner(lp.Elements[i], lp.Elements[i + 1], offset, offsetAngle);
                        else
                            if (lp.Flags == DXFLWPolyLine.FlagsEnum.closed)
                                AddRoundCorner(lp.Elements[i], lp.Elements[0], offset, offsetAngle);
                        roundcorner = true;
                    }
                    else
                        roundcorner = false;
                }
                if ((lp.Flags > 0))// && (x2 != x) && (y2 != y))   // only move if prev pos is differnent
                {	position = calcPosition(lp.Elements[0].Vertex, offset, offsetAngle);
					dxfMoveTo(position, "End LWPolyLine "+lp.Flags.ToString());
				}
                dxfStopPath();
            }
            #endregion
            #region DXFPolyline
            else if (entity.GetType() == typeof(DXFPolyLine))
            {
                DXFPolyLine lp = (DXFPolyLine)entity;
                index = 0;
                DXFPoint start = new DXFPoint();
                foreach (DXFVertex coordinate in lp.Children)
                {
                    if (coordinate.GetType() == typeof(DXFVertex))
                        if (coordinate.Location.X != null && coordinate.Location.Y != null)
                        {
                            position = calcPosition(coordinate.Location, offset, offsetAngle);
                            if (!nodesOnly)
                            {   if (index == 0)
                                {   start = position;
                                    dxfStartPath(position, "Start PolyLine");
                                }
                                else
                                    dxfMoveTo(position, "");
                            }
                            else { gcodeDotOnly(position, "PolyLine"); }
                            index++;
                        }
                }
                if ((lp.Flags > 0))
                    dxfMoveTo(start, "End PolyLine " + lp.Flags.ToString());
                dxfStopPath();
            }
            #endregion
            #region DXFLine
            else if (entity.GetType() == typeof(DXFLine))
            {
                DXFLine line = (DXFLine)entity;
                position = calcPosition(line.Start, offset, offsetAngle);
                DXFPoint pos2 = calcPosition(line.End, offset, offsetAngle);
                Plotter.IsPathReduceOk = false;
                if (gcode.loggerTrace) Logger.Trace(" Line from: {0};{1}  To: {2};{3}", position.X, position.Y, pos2.X, pos2.Y);
                if (!nodesOnly)
                {   dxfStartPath(position, "Start Line");
                    dxfMoveTo(pos2, "");
                }
                else {
                    gcodeDotOnly(position, "Start Line");
                    gcodeDotOnly(pos2, "End Line");
                }
                dxfStopPath();
            }
            #endregion
            #region DXFSpline
            else if (entity.GetType() == typeof(DXFSpline))
            {   // from Inkscape DXF import - modified
                // https://gitlab.com/inkscape/extensions/blob/master/dxf_input.py#L106
                DXFSpline spline = (DXFSpline)entity;
                index = 0;
                DXFPoint last = calcPosition(spline.ControlPoints[0], offset, offsetAngle);

                string cmt = "Start Spline " + spline.KnotValues.Count.ToString() + " " + spline.ControlPoints.Count.ToString() + " " + spline.FitPoints.Count.ToString();
                Plotter.IsPathReduceOk = true;

                int knots = spline.KnotCount;
                int ctrls = spline.ControlPointCount;
                if (gcode.loggerTrace) Logger.Trace(" Spline ControlPointCnt: {0} KnotsCount: {1}", ctrls, knots);

                if ((ctrls > 3) && (knots == ctrls + 4))    //  # cubic
                {   if (ctrls > 4)
                    {   for (int i = (knots - 5); i > 3; i--)
                        {   if ((spline.KnotValues[i] != spline.KnotValues[i - 1]) && (spline.KnotValues[i] != spline.KnotValues[i + 1]))
                            {   double a0 = (spline.KnotValues[i] - spline.KnotValues[i - 2]) / (spline.KnotValues[i + 1] - spline.KnotValues[i - 2]);
                                double a1 = (spline.KnotValues[i] - spline.KnotValues[i - 1]) / (spline.KnotValues[i + 2] - spline.KnotValues[i - 1]);
                                DXFPoint tmp = new DXFPoint();
                                tmp.X = (double)((1.0 - a1) * spline.ControlPoints[i - 2].X + a1 * spline.ControlPoints[i - 1].X);
                                tmp.Y = (double)((1.0 - a1) * spline.ControlPoints[i - 2].Y + a1 * spline.ControlPoints[i - 1].Y);
                                spline.ControlPoints.Insert(i - 1, tmp);
                                spline.ControlPoints[i - 2].X = (1.0 - a0) * spline.ControlPoints[i - 3].X + a0 * spline.ControlPoints[i - 2].X;
                                spline.ControlPoints[i - 2].Y = (1.0 - a0) * spline.ControlPoints[i - 3].Y + a0 * spline.ControlPoints[i - 2].Y;
                                spline.KnotValues.Insert(i, spline.KnotValues[i]);
                            }
                        }
                        knots = spline.KnotValues.Count;
                        for (int i=(knots - 6); i> 3; i-=2)
                        {   if ((spline.KnotValues[i] != spline.KnotValues[i - 2]) && (spline.KnotValues[i-1] != spline.KnotValues[i + 1]) && (spline.KnotValues[i - 2] != spline.KnotValues[i]))
                            {   double a1 = (spline.KnotValues[i] - spline.KnotValues[i - 1]) / (spline.KnotValues[i + 2] - spline.KnotValues[i - 1]);
                                DXFPoint tmp = new DXFPoint();
                                tmp.X = (double)((1.0 - a1) * spline.ControlPoints[i - 2].X + a1 * spline.ControlPoints[i - 1].X);
                                tmp.Y = (double)((1.0 - a1) * spline.ControlPoints[i - 2].Y + a1 * spline.ControlPoints[i - 1].Y);
                                spline.ControlPoints.Insert(i - 1, tmp);
                            }
                        }        
                    }
                    ctrls = spline.ControlPoints.Count;
                    dxfStartPath(last, cmt);
                    for (int i = 0; i < Math.Floor((ctrls - 1) / 3d); i++)     // for i in range(0, (ctrls - 1) // 3):
                    {
                        if (!nodesOnly)
                            importMath.calcCubicBezier( toWSPoint(last), 
                                                        toWSPoint(calcPosition(spline.ControlPoints[3 * i + 1], offset, offsetAngle)), 
                                                        toWSPoint(calcPosition(spline.ControlPoints[3 * i + 2], offset, offsetAngle)), 
                                                        toWSPoint(calcPosition(spline.ControlPoints[3 * i + 3], offset, offsetAngle)), 
                                                        dxfMoveTo, "C");
                      else
                        {   gcodeDotOnly(last, "");
                            gcodeDotOnly(toWSPoint(calcPosition(spline.ControlPoints[3 * i + 3], offset,offsetAngle)), "");
                        }
                        last = calcPosition(spline.ControlPoints[3 * i + 3], offset, offsetAngle);
                    }
                    dxfStopPath();
                }
                if ((ctrls == 3) && (knots == 6))           //  # quadratic
                {   //  path = 'M %f,%f Q %f,%f %f,%f' % (vals[groups['10']][0], vals[groups['20']][0], vals[groups['10']][1], vals[groups['20']][1], vals[groups['10']][2], vals[groups['20']][2])
                    if (!nodesOnly)
                    {   dxfStartPath(last, cmt);
                        importMath.calcQuadraticBezier( toWSPoint(calcPosition(spline.ControlPoints[0], offset,offsetAngle)),
                                                        toWSPoint(calcPosition(spline.ControlPoints[1], offset,offsetAngle)),
                                                        toWSPoint(calcPosition(spline.ControlPoints[2], offset,offsetAngle)), 
                                                        dxfMoveTo, "Q");
                    }
                    else
                    {   gcodeDotOnly(last, "");
                        gcodeDotOnly(toWSPoint(calcPosition(spline.ControlPoints[2], offset, offsetAngle)), "");
                    }
                    dxfStopPath();
                }
                if ((ctrls == 5) && (knots == 8))           //  # spliced quadratic
                {   //  path = 'M %f,%f Q %f,%f %f,%f Q %f,%f %f,%f' % (vals[groups['10']][0], vals[groups['20']][0], vals[groups['10']][1], vals[groups['20']][1], vals[groups['10']][2], vals[groups['20']][2], vals[groups['10']][3], vals[groups['20']][3], vals[groups['10']][4], vals[groups['20']][4])
                    if (!nodesOnly)
                    {   dxfStartPath(last, cmt);
                        importMath.calcQuadraticBezier( toWSPoint(calcPosition(spline.ControlPoints[0], offset,offsetAngle)),
                                                        toWSPoint(calcPosition(spline.ControlPoints[1], offset,offsetAngle)),
                                                        toWSPoint(calcPosition(spline.ControlPoints[2], offset,offsetAngle)), dxfMoveTo, "SQ");
                        importMath.calcQuadraticBezier( toWSPoint(calcPosition(spline.ControlPoints[3], offset,offsetAngle)),
                                                        toWSPoint(calcPosition(spline.ControlPoints[4], offset, offsetAngle)),
                                                        toWSPoint(calcPosition(spline.ControlPoints[5], offset,offsetAngle)), dxfMoveTo, "SQ");
                    }
                    else
                    {   gcodeDotOnly(last, "");
                        gcodeDotOnly(toWSPoint(calcPosition(spline.ControlPoints[2], offset,offsetAngle)), "");
                        gcodeDotOnly(toWSPoint(calcPosition(spline.ControlPoints[5], offset,offsetAngle)), "");
                    }
                    dxfStopPath();
                }
            }
            #endregion
            #region DXFCircle
            else if (entity.GetType() == typeof(DXFCircle))
            {
                DXFCircle circle = (DXFCircle)entity;
                position = calcPosition(circle.Center, offset, offsetAngle);
                if (gcode.loggerTrace) Logger.Trace(" Circle center: {0};{1}  R: {2}", position.X, position.Y, circle.Radius);
                dxfStartPath((double)position.X + circle.Radius, (double)position.Y, "Start Circle");
                Plotter.Arc( 2, (float)position.X + (float)circle.Radius, (float)position.Y, -(float)circle.Radius, 0, "");
                dxfStopPath();
            }
            #endregion
            #region DXFEllipse
            else if (entity.GetType() == typeof(DXFEllipse))
            {   // from Inkscape DXF import - modified
                // https://gitlab.com/inkscape/extensions/blob/master/dxf_input.py#L341
                DXFEllipse ellipse = (DXFEllipse)entity;
                double angleRad = offsetAngle * Math.PI / 180;
                float xc = (float)(ellipse.Center.X * Math.Cos(angleRad) - ellipse.Center.Y * Math.Sin(angleRad) + offset.X);
                float yc = (float)(ellipse.Center.X * Math.Sin(angleRad) + ellipse.Center.Y * Math.Cos(angleRad) + offset.Y);

                float xm = (float)(ellipse.MainAxis.X * Math.Cos(angleRad) - ellipse.MainAxis.Y * Math.Sin(angleRad));
                float ym = (float)(ellipse.MainAxis.X * Math.Sin(angleRad) + ellipse.MainAxis.Y * Math.Cos(angleRad));
                float w = (float)ellipse.AxisRatio;
                double a2 = -ellipse.StartParam + offsetAngle;
                double a1 = -ellipse.EndParam + offsetAngle;

                float rm = (float)Math.Sqrt(xm * xm + ym * ym);
                double a = Math.Atan2(-ym, xm);
                float diff = (float)((a2 - a1 + 2 * Math.PI) % (2 * Math.PI));

                if (gcode.loggerTrace) Logger.Trace(" Ellipse center: {0};{1}  R1: {2} R2: {3} Start: {4} End: {5}", xc, yc, rm, w * rm, ellipse.StartParam, ellipse.EndParam);
                if ((Math.Abs(diff) > 0.0001) && (Math.Abs(diff - 2 * Math.PI) > 0.0001))
                {
                    int large = 0;
                    if (diff > Math.PI)
                        large = 1;
                    float xt = rm * (float)Math.Cos(a1);
                    float yt = w * rm * (float)Math.Sin(a1);
                    float x1 = (float)(xt * Math.Cos(a) - yt * Math.Sin(a));
                    float y1 = (float)(xt * Math.Sin(a) + yt * Math.Cos(a));
                    xt = rm * (float)Math.Cos(a2);
                    yt = w * rm * (float)Math.Sin(a2);
                    float x2 = (float)(xt * Math.Cos(a) - yt * Math.Sin(a));
                    float y2 = (float)(xt * Math.Sin(a) + yt * Math.Cos(a));
                    dxfStartPath(xc + x1, yc - y1, "Start Ellipse 1");
                    importMath.calcArc(xc + x1, yc - y1, rm, w * rm, (float)(-180.0 * a / Math.PI), large, 0, (xc + x2), (yc - y2), dxfMoveTo);
                    //  path = 'M %f,%f A %f,%f %f %d 0 %f,%f' % (xc + x1, yc - y1, rm, w* rm, -180.0 * a / math.pi, large, xc + x2, yc - y2)
                }
                else
                {
                    dxfStartPath(xc + xm, yc + ym, "Start Ellipse 2");
                    importMath.calcArc(xc + xm, yc + ym, rm, w * rm, (float)(-180.0 * a / Math.PI), 1, 0, xc - xm, yc - ym, dxfMoveTo);
                    importMath.calcArc(xc - xm, yc - ym, rm, w * rm, (float)(-180.0 * a / Math.PI), 1, 0, xc + xm, yc + ym, dxfMoveTo);
                    //    path = 'M %f,%f A %f,%f %f 1 0 %f,%f %f,%f %f 1 0 %f,%f z' % (xc + xm, yc - ym, rm, w* rm, -180.0 * a / math.pi, xc - xm, yc + ym, rm, w* rm, -180.0 * a / math.pi, xc + xm, yc - ym)
                }
                dxfStopPath();
            }
            #endregion
            #region DXFArc
            else if (entity.GetType() == typeof(DXFArc))
            {
                DXFArc arc = (DXFArc)entity;
                
                double X = (double)arc.Center.X + (double)offset.X;
                double Y = (double)arc.Center.Y + (double)offset.Y;
                double R = arc.Radius;
                double startAngle = arc.StartAngle;
                double endAngle = arc.EndAngle;
                if (startAngle > endAngle) endAngle += 360;
                double stepwidth = (double)Properties.Settings.Default.importDXFStepWidth; 
                float StepAngle = (float)(Math.Asin(stepwidth / R) * 180 / Math.PI);
                double currAngle = startAngle;
                index = 0;

                if (gcode.loggerTrace) Logger.Trace(" Arc center: {0};{1}  R: {2}", X, Y, R);
                if (!nodesOnly)
                {
                    while (currAngle < endAngle)
                    {
                        double angle = currAngle * Math.PI / 180;
                        double rx = (double)(X + R * Math.Cos(angle));
                        double ry = (double)(Y + R * Math.Sin(angle));

                        if (index == 0)
                        {
                            dxfStartPath(rx, ry, "Start Arc");
                            Plotter.IsPathReduceOk = true;
                        }
                        else
                            dxfMoveTo(rx, ry, "");
                        currAngle += StepAngle;
                        if (currAngle > endAngle)
                        {
                            double angle2 = endAngle * Math.PI / 180;
                            double rx2 = (double)(X + R * Math.Cos(angle2));
                            double ry2 = (double)(Y + R * Math.Sin(angle2));

                            if (index == 0)
                            {
                                dxfStartPath(rx2, ry2, "Start Arc");
                            }
                            else
                                dxfMoveTo(rx2, ry2, "");
                        }
                        index++;
                    }
                    dxfStopPath();
                }
            }
            #endregion
            #region DXFMText
            else if (entity.GetType() == typeof(DXFMText))
            {   // https://www.autodesk.com/techpubs/autocad/acad2000/dxf/mtext_dxf_06.htm
                DXFMText txt = (DXFMText)entity;
                xyPoint origin = new xyPoint(0,0);
                GCodeFromFont.reset();
                double angle = 0;
                foreach (var entry in txt.Entries)
                {        if (entry.GroupCode == 1) { GCodeFromFont.gcText = entry.Value.ToString(); }
                    else if (entry.GroupCode == 40) { GCodeFromFont.gcHeight = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                    else if (entry.GroupCode == 41) { GCodeFromFont.gcWidth = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                    else if (entry.GroupCode == 71) { GCodeFromFont.gcAttachPoint = Convert.ToInt16(entry.Value); }
                    else if (entry.GroupCode == 10) { GCodeFromFont.gcOffX = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                    else if (entry.GroupCode == 20) { GCodeFromFont.gcOffY = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                    else if (entry.GroupCode == 50) { angle = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }// + offsetAngle)* Math.PI / 180; } 
                    else if (entry.GroupCode == 44) { GCodeFromFont.gcSpacing = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                    else if (entry.GroupCode == 7) { GCodeFromFont.gcFontName = entry.Value.ToString(); }
                }
                DXFPoint tmpPos = new DXFPoint();
                tmpPos.X = GCodeFromFont.gcOffX;
                tmpPos.Y = GCodeFromFont.gcOffY;
                tmpPos = calcPosition(tmpPos, offset, offsetAngle);
                GCodeFromFont.gcOffX = (double)tmpPos.X;
                GCodeFromFont.gcOffY = (double)tmpPos.Y;

                GCodeFromFont.gcAngleRad = (angle + offsetAngle) * Math.PI / 180;
                string tmp = string.Format("Id=\"{0}\" Color=\"#{1}\" ToolNr=\"{2}\"", dxfColorID, dxfColorHex, toolToUse);
                if (gcode.loggerTrace) Logger.Trace(" Text: {0} X{1:0.00} Y{2:0.00} a{3:0.00} oa{4:0.00}", GCodeFromFont.gcText, GCodeFromFont.gcOffX, GCodeFromFont.gcOffY, angle, offsetAngle);
                Plotter.InsertText(tmp);
                Plotter.IsPathFigureEnd = true;
            }
            #endregion
            else
                Plotter.Comment( "Unknown: " + entity.GetType().ToString());
        }

        private static Point toWSPoint(DXFPoint tmp)
        { return new Point((double)tmp.X,(double)tmp.Y);
        }
        private static DXFPoint calcPosition(DXFPoint location, DXFPoint offset, double offsetAngleDegree)
		{	DXFPoint tmp=new DXFPoint();
            double offsetAngle = Math.PI * offsetAngleDegree / 180;
			tmp.X = location.X * Math.Cos(offsetAngle) - location.Y * Math.Sin(offsetAngle) + offset.X;
            tmp.Y = location.X * Math.Sin(offsetAngle) + location.Y * Math.Cos(offsetAngle) + offset.Y;
            return tmp;	
		}

        private static Point toPoint(DXFPoint tmp, Point Off)
        {   Point p = new Point();
            p.X = (double)tmp.X + Off.X; p.Y = (double)tmp.Y + Off.Y;
            return p;
        }
        private static Point toPoint(DXFPoint tmp, DXFPoint Off)
        {   Point p = new Point();
            p.X = (double)(tmp.X + Off.X); p.Y = (double)(tmp.Y + Off.Y);
            return p;
        }

        private static PolyLineSegment GetBezierApproximation(System.Windows.Point[] controlPoints, int outputSegmentCount)
        {
            System.Windows.Point[] points = new System.Windows.Point[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return new PolyLineSegment(points, true);
        }
        private static System.Windows.Point GetBezierPoint(double t, System.Windows.Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            double x = (1 - t) * P0.X + t * P1.X;
            return new System.Windows.Point(x, (1 - t) * P0.Y + t * P1.Y);
        }

        /// <summary>
        /// Calculate round corner of DXFLWPolyLine if Bulge is given
        /// </summary>
        /// <param name="var1">First vertex coord</param>
        /// <param name="var2">Second vertex</param>
        /// <returns></returns>
        private static void AddRoundCorner(DXFLWPolyLine.Element var1, DXFLWPolyLine.Element var2, DXFPoint offset, double angleDegree)
        {   double angleRad = angleDegree * Math.PI / 180;
            double bulge = var1.Bulge;
            double p1x = (double)(var1.Vertex.X * Math.Cos(angleRad) - var1.Vertex.Y * Math.Sin(angleRad));       //var1.Vertex.X;
            double p1y = (double)(var1.Vertex.X * Math.Sin(angleRad) + var1.Vertex.Y * Math.Cos(angleRad));       //var1.Vertex.Y;
            double p2x = (double)(var2.Vertex.X * Math.Cos(angleRad) - var2.Vertex.Y * Math.Sin(angleRad));       //var2.Vertex.X;
            double p2y = (double)(var2.Vertex.X * Math.Sin(angleRad) + var2.Vertex.Y * Math.Cos(angleRad));       //var2.Vertex.Y;

            //Definition of bulge, from Autodesk DXF fileformat specs
            double angle = Math.Abs(Math.Atan(bulge) * 4);
            bool girou = false;

            //For my method, this angle should always be less than 180. 
            if (angle > Math.PI)
            {   angle = Math.PI * 2 - angle;
                girou = true;
            }

            //Distance between the two vertexes, the angle between Center-P1 and P1-P2 and the arc radius
            double distance = Math.Sqrt(Math.Pow(p1x - p2x, 2) + Math.Pow(p1y - p2y, 2));
            double alpha = (Math.PI - angle) / 2;
            double ratio = distance * Math.Sin(alpha) / Math.Sin(angle);
            if (angle == Math.PI)
                ratio = distance / 2;

            double xc, yc, direction;

            //Used to invert the signal of the calculations below
            if (bulge < 0)
                direction = 1;
            else
                direction = -1;

            //calculate the arc center
            double part = Math.Sqrt(Math.Pow(2 * ratio / distance, 2) - 1);
            if (!girou)
            {   xc = ((p1x + p2x) / 2) - direction * ((p1y - p2y) / 2) * part;
                yc = ((p1y + p2y) / 2) + direction * ((p1x - p2x) / 2) * part;
            }
            else
            {   xc = ((p1x + p2x) / 2) + direction * ((p1y - p2y) / 2) * part;
                yc = ((p1y + p2y) / 2) - direction * ((p1x - p2x) / 2) * part;
            }

            string cmt = "";
            if (dxfComments) { cmt = "Bulge " + bulge.ToString(); }

            if (bulge > 0)
                Plotter.Arc( 3, (float)(p2x+(double)offset.X), (float)(p2y+ (double)offset.Y), (float)(xc-p1x), (float)(yc-p1y), cmt);	// Arc(gnr, x, y, i, j, cmt = "", avoidG23 = false)
            else
                Plotter.Arc( 2, (float)(p2x+ (double)offset.X), (float)(p2y+ (double)offset.Y), (float)(xc-p1x), (float)(yc-p1y), cmt);
        }

        /// <summary>
        /// Transform XY coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static System.Windows.Point translateXY(float x, float y)
        {
            System.Windows.Point coord = new System.Windows.Point(x, y);
            return translateXY(coord);
        }
        private static System.Windows.Point translateXY(System.Windows.Point pointStart)
        {   return pointStart;
        }
        /// <summary>
        /// Transform I,J coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static System.Windows.Point translateIJ(float i, float j)
        {   System.Windows.Point coord = new System.Windows.Point(i, j);
            return translateIJ(coord);
        }
        private static System.Windows.Point translateIJ(System.Windows.Point pointStart)
        {   System.Windows.Point pointResult = pointStart;
            double tmp_i = pointStart.X, tmp_j = pointStart.Y;
            return pointResult;
        }

        private static void gcodeDotOnly(DXFPoint tmp, string cmt)
        { gcodeDotOnly((double)tmp.X, (double)tmp.Y, cmt); }
        private static void gcodeDotOnly(Point tmp, string cmt)
        { gcodeDotOnly((double)tmp.X, (double)tmp.Y, cmt); }
        private static void gcodeDotOnly(double x, double y, string cmt)
        {
            if (!dxfComments)
                cmt = "";
            dxfStartPath(x, y, cmt);
            Plotter.PenDown(cmt);
            Plotter.PenUp(cmt, false);
            Plotter.StopPath(cmt);
        }

        /// <summary>
        /// Insert G0, Pen down gcode command
        /// </summary>
        private static void dxfStartPath(DXFPoint tmp, string cmt = "")
        { Plotter.StartPath(translateXY((float)tmp.X, (float)tmp.Y), cmt); }

        private static void dxfStartPath(double x, double y, string cmt = "")
        {   Plotter.StartPath(translateXY((float)x, (float)y), cmt);  }

        private static void dxfStopPath(string cmt="")
        {   Plotter.StopPath(cmt);  }

        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void dxfMoveTo(DXFPoint tmp, string cmt)
        {   dxfMoveTo((double)tmp.X, (double)tmp.Y, cmt);
        }

        private static void dxfMoveTo(double x, double y, string cmt)
        {   System.Windows.Point coord = new System.Windows.Point(x, y);
            dxfMoveTo(coord, cmt);
        }
        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void dxfMoveTo(float x, float y, string cmt)
        {   System.Windows.Point coord = new System.Windows.Point(x, y);
            dxfMoveTo(coord, cmt);
        }
        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void dxfMoveTo(System.Windows.Point orig, string cmt)
        {   System.Windows.Point coord = translateXY(orig);
//            if (gcode.loggerTrace) Logger.Trace(" dxfMoveTo x{0:0.000} y{1:0.000} cmt {2}",orig.X,orig.Y,cmt);
            if (!nodesOnly)
                Plotter.MoveTo(coord, cmt);
            else
                gcodeDotOnly(coord.X, coord.Y, "");
        }

        private static string getColorFromID(int id)
        {     string[] DXFcolors = {"000000","FF0000","FFFF00","00FF00","00FFFF","0000FF","FF00FF","FFFFFF",
                                    "414141","808080","FF0000","FFAAAA","BD0000","BD7E7E","810000","815656",
                                    "680000","684545","4F0000","4F3535","FF3F00","FFBFAA","BD2E00","BD8D7E",
                                    "811F00","816056","681900","684E45","4F1300","4F3B35","FF7F00","FFD4AA",
                                    "BD5E00","BD9D7E","814000","816B56","683400","685645","4F2700","4F4235",
                                    "FFBF00","FFEAAA","BD8D00","BDAD7E","816000","817656","684E00","685F45",
                                    "4F3B00","4F4935","FFFF00","FFFFAA","BDBD00","BDBD7E","818100","818156",
                                    "686800","686845","4F4F00","4F4F35","BFFF00","EAFFAA","8DBD00","ADBD7E",
                                    "608100","768156","4E6800","5F6845","3B4F00","494F35","7FFF00","D4FFAA",
                                    "5EBD00","9DBD7E","408100","6B8156","346800","566845","274F00","424F35",
                                    "3FFF00","BFFFAA","2EBD00","8DBD7E","1F8100","608156","196800","4E6845",
                                    "134F00","3B4F35","00FF00","AAFFAA","00BD00","7EBD7E","008100","568156",
                                    "006800","456845","004F00","354F35","00FF3F","AAFFBF","00BD2E","7EBD8D",
                                    "00811F","568160","006819","45684E","004F13","354F3B","00FF7F","AAFFD4",
                                    "00BD5E","7EBD9D","008140","56816B","006834","456856","004F27","354F42",
                                    "00FFBF","AAFFEA","00BD8D","7EBDAD","008160","568176","00684E","45685F",
                                    "004F3B","354F49","00FFFF","AAFFFF","00BDBD","7EBDBD","008181","568181",
                                    "006868","456868","004F4F","354F4F","00BFFF","AAEAFF","008DBD","7EADBD",
                                    "006081","567681","004E68","455F68","003B4F","35494F","007FFF","AAD4FF",
                                    "005EBD","7E9DBD","004081","566B81","003468","455668","00274F","35424F",
                                    "003FFF","AABFFF","002EBD","7E8DBD","001F81","566081","001968","454E68",
                                    "00134F","353B4F","0000FF","AAAAFF","0000BD","7E7EBD","000081","565681",
                                    "000068","454568","00004F","35354F","3F00FF","BFAAFF","2E00BD","8D7EBD",
                                    "1F0081","605681","190068","4E4568","13004F","3B354F","7F00FF","D4AAFF",
                                    "5E00BD","9D7EBD","400081","6B5681","340068","564568","27004F","42354F",
                                    "BF00FF","EAAAFF","8D00BD","AD7EBD","600081","765681","4E0068","5F4568",
                                    "3B004F","49354F","FF00FF","FFAAFF","BD00BD","BD7EBD","810081","815681",
                                    "680068","684568","4F004F","4F354F","FF00BF","FFAAEA","BD008D","BD7EAD",
                                    "810060","815676","68004E","68455F","4F003B","4F3549","FF007F","FFAAD4",
                                    "BD005E","BD7E9D","810040","81566B","680034","684556","4F0027","4F3542",
                                    "FF003F","FFAABF","BD002E","BD7E8D","81001F","815660","680019","68454E",
                                    "4F0013","4F353B","333333","505050","696969","828282","BEBEBE","FFFFFF" };
            if (id < 0) id = 0;
            if (id > 255) id = 255;
            return DXFcolors[id];
        }
    }
}
