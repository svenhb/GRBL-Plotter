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

/* Level 1: import graphics SVG, DXF, HPGL, Drill, CSV
 *			- collect colors, pen-widths, layer-names for grouping
 *          - extract objects, get coordinates, convert Bezier to line-segments
 *			- convert circle to dot (option)
 *
 * Level 2: graphicRelated: collect dots, lines, arcs; sorting by distance, merging, clipping, grouping, tangential axis
 *			- collect path-data (pen-down path): either path with line and arc or just a dot
 *			- path modifications: remove offset, hatch fill, repeat paths, sort by distance and merge, 
 *			- tangential axis, drag-knife, clipping and tiling, path extension
 *
 * Level 3: graphic2Gcode: translate graphic-paths into GCode commands
 *
 * Level 4: gcodeRelated: implement Pen up/down options, cutter correction, write GCode commands 
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
 * 2020-03-30 Grouping also by Layer-Name
 * 2020-04-15 DXFArc automatic correction if R is > DXF ARC step width
 * 2020-04-26 Replace class Plotter by class Graphic for sorting
 * 2020-04-27 DXFArc implement G3 instead of line segments
 * 2020-07-20 clean up
 * 2020-11-30 don't set layer, pen etc. if there is no change (to avoid starting a new object in 'graphicRelated.cs') line 339
 * 2020-12-08 add BackgroundWorker updates
 * 2021-05-18 line 435 check for DXFLWPolyLine.FlagsEnum.closed
 * 2021-07-14 code clean up / code quality
 * 2021-07-28 handle black as white line 359 : index 7 <=> 0
 * 2021-07-30 bug fix: AddRoundCorner if distance=0 -> no arc
 * 2021-08-16 use Z information - not for point, spline, text
 * 2021-10-29 logger output format
 * 2021-11-02 add DXFText (before just DXFMText)
 * 2021-12-09 line 401 check if (layerLType != null)
 * 2022-02-08 extend DXFPolyline by bulge
 * 2022-02-18 line 405 add check (layerName != null)
 * 2022-03-19 line 729 2nd CalcQuadraticBezier start index at 2 not 3
 * 2022-05-18 line 295 check via ContainsKey
 * 2022-06-14 line 387 skip entity if layer is invisible or printing is disabled
 * 2022-11-10 line 445 check IsNullOrEmpty(dashType)
 * 2023-02-05 line 324 check if (!lineTypes.ContainsKey(lt.LineTypeName)) before add
 * 2023-04-10 l:187 f:ConvertFromFile check length for substring
 * 2023-06-02 l:809 f:ProcessEntities bug fix importSVGCircleToDot
 * 2023-09-09 l:987 f:CalcEllipse don't add offsetAngle to start/end angle issue #359
 * 2023-09-11 l:855 f:ProcessEntities - DXFArc add offsetAngle to start/end angle issue #359
 * 2023-11-24 l:645 f:ProcessEntities Spline: new algorythmus from  //https://github.com/ixmilia/converters/blob/main/src/IxMilia.Converters/DxfToSvgConverter.cs
 * 2023-12-13 l:374 f:GetVectorDXF add block scaling, issue #365
*/

using DXFLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace GrblPlotter //DXFImporter
{
    public static class GCodeFromDxf
    {
        private static DXFDocument doc;

        private static int dxfColorNr = -1;
        private static int dxfLineWeigth = 0;
        private static int dxfBlockColorNr = -1;
        private static int dxfBlockLineWeigth = 0;

        private static string wasSetLayer = "";
        private static string wasSetPenColor = "";
        private static int wasSetPenColorId = -1;
        private static string wasSetPenFill = "";
        private static int wasSetPenWidth = -1;

        private static int countDXFLayers = 0;
        private static int countDXFEntities = 0;
        private static int countDXFBlocks = 0;
        private static int countDXFEntity = 0;

        private static string dxfColorFill = "none";
        private static string dxfColorHex = "";
        private static bool nodesOnly = false;              // if true only do pen-down -up on given coordinates
        private static bool useZ = false;
        private static double? lastSetZ = null;
        private static Point lastUsedCoord = new Point();

        private static readonly Dictionary<string, int> layerColor = new Dictionary<string, int>();
        private static readonly Dictionary<string, string> layerLType = new Dictionary<string, string>();
        private static readonly Dictionary<string, double[]> lineTypes = new Dictionary<string, double[]>();
        private static readonly Dictionary<string, int> layerLineWeigth = new Dictionary<string, int>();
        private static readonly Dictionary<string, bool> layerPlot = new Dictionary<string, bool>();

        public static string ConversionInfo { get; set; }
        private static int shapeCounter = 0;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logPosition = false;

        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent = null;

        /// <summary>
        /// Entrypoint for conversion: apply file-path 
        /// </summary>
        /// <param name="file">String keeping file-name or URL</param>
        /// <returns>String with GCode of imported data</returns>
        public static bool ConvertFromText(string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                LoadDXF(stream);
            }
            return ConvertDXF("from Clipboard");
        }
        public static bool ConvertFromFile(string filePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            backgroundWorker = worker;
            backgroundEvent = e;

            if (String.IsNullOrEmpty(filePath))
            { MessageBox.Show("Empty file name"); return false; }
            else if (filePath.Substring(0, 4) == "http")
            {
                string content = "";
                using (var wc = new System.Net.WebClient())
                {
                    try { content = wc.DownloadString(filePath); }
                    catch { MessageBox.Show("Could not load content from " + filePath); }
                }
                int pos = content.IndexOf("dxfrw");
                if ((!String.IsNullOrEmpty(content)) && (pos >= 0) && (pos < 8))
                {
                    try
                    {
                        byte[] byteArray = Encoding.Unicode.GetBytes(content); // Encoding.UTF8.GetBytes(content);
                        using (MemoryStream stream = new MemoryStream(byteArray))
                        {
                            Logger.Info("●●●● load from stream");
                            if (!LoadDXF(stream))
                                return false;// "(File could not be loaded)";
                        }
                    }
                    catch (Exception err)
                    { MessageBox.Show("Error '" + err.ToString() + "' in DXF file " + filePath); }
                }
                else
                {
                    int len = Math.Min(50, content.Length);
                    MessageBox.Show("This is probably not a DXF document.\r\nFirst line: " + content.Substring(0, len));
                }
            }
            else
            {
                if (File.Exists(filePath))
                {
                    Logger.Info("●●●● load from file");
                    try
                    {
                        if (!LoadDXF(filePath))
                            return false;// "(File could not be loaded)";
                    }
                    catch (Exception err)
                    { MessageBox.Show("Error '" + err.ToString() + "' in DXF file " + filePath); }
                }
                else { MessageBox.Show("File does not exist: " + filePath); return false; }
            }
            return ConvertDXF(filePath);
        }

        /// <summary>
        /// Convert DXF and create GCode
        /// </summary>
        private static bool ConvertDXF(string filePath)
        {
            Logger.Info("▼▼▼▼ ConvertDXF  logEnable:{0}  logPosition:{1}", logEnable, logPosition);
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
            logPosition = logEnable && ((logFlags & (uint)LogEnables.Coordinates) > 0);
            //    Logger.Trace(" logEnable:{0}  logPosition:{1}", logEnable, logPosition);

            nodesOnly = Properties.Settings.Default.importSVGNodesOnly;
            useZ = Properties.Settings.Default.importDXFUseZ;
            lastSetZ = null;
            shapeCounter = 0;
            ConversionInfo = "";
            dxfColorNr = 259;

            Graphic.Init(Graphic.SourceType.DXF, filePath, backgroundWorker, backgroundEvent);
            GetVectorDXF();                     // convert graphics
            //Logger.Info("●●●● Disable Z use? {0}", lastSetZ);
            if (lastSetZ == null)
            {
                Logger.Info("●●●● DxfImportZ = false (no Z information)");
                Graphic.graphicInformation.DxfImportZ = false;    // no Z value, no need for export
            }
			else
                Logger.Info("●●●● DxfImportZ = {0}", Graphic.graphicInformation.DxfImportZ);
			
            ConversionInfo += string.Format("{0} elements imported", shapeCounter);
            Logger.Info("▲▲▲▲ ConvertDXF Finish: shapeCounter: {0}", shapeCounter);
            doc = null;
            return Graphic.CreateGCode();
        }

        /// <summary>
        /// Load and parse DXF code
        /// </summary>
        /// <param name="filename">String keeping file-name</param>
        /// <returns></returns>
        private static bool LoadDXF(string filename)
        {
            doc = new DXFDocument();
            try { doc.Load(filename); return true; }
            catch (IOException err)
            {
                Logger.Error(err, "loading the file failed IOException ");
                MessageBox.Show("The file could not be opened - perhaps already open in other application?\r\n" + err.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "loading the file failed 2 ");
                throw;      // unknown exception...
            }
            return false;
        }
        private static bool LoadDXF(Stream content)
        {
            doc = new DXFDocument();
            try { doc.Load(content); return true; }
            catch (IOException err)
            {
                Logger.Error(err, "loading the file failed IOException ");
                MessageBox.Show("The file could not be opened - perhaps already open in other application?\r\n" + err.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "loading the file failed 2 ");
                throw;      // unknown exception...
            }
            return false;
        }
        private static void GetVectorDXF()
        {
            layerColor.Clear();
            layerLType.Clear();
            lineTypes.Clear();
            layerLineWeigth.Clear();
            layerPlot.Clear();

            wasSetLayer = "";
            wasSetPenColor = "";
            wasSetPenColorId = -1;
            wasSetPenFill = "";
            wasSetPenWidth = -1;

            List<DXFLayerRecord> lrecord = new List<DXFLayerRecord>();
            try
            {
                lrecord = doc.Tables.Layers;
                countDXFLayers = lrecord.Count;
            }
            catch (Exception err) { Logger.Error(err, "Could not read doc.Tables.Layers "); }
            countDXFEntities = doc.Entities.Count;
            countDXFBlocks = doc.Blocks.Count;
            countDXFEntity = 0;

            Logger.Info("●●●● AutoCADVersion:{0}", doc.Header.AutoCADVersion);
            Logger.Info("●●●● Amount Layers:{0}  Entities:{1}  Blocks:{2}", countDXFLayers, countDXFEntities, countDXFBlocks);
            backgroundWorker?.ReportProgress(0, new MyUserState { Value = 10, Content = "Read DXF vector data of " + countDXFEntities.ToString() + " elements" });

            List<DXFStyleRecord> srecord;// = new List<DXFStyleRecord>();
            srecord = doc.Tables.Styles;
            foreach (DXFStyleRecord str in srecord)
            {
                Logger.Trace("Style {0}", str.FontFileName);
            }

            int plotflag = 0;
            foreach (DXFLayerRecord record in lrecord)
            {
                Graphic.SetHeaderInfo(string.Format(" Layer:{0} , color:{1} , line type:{2}", record.LayerName, record.Color, record.LineType));
                if (!layerColor.ContainsKey(record.LayerName)) layerColor.Add(record.LayerName, record.Color);
                if (!layerLType.ContainsKey(record.LayerName)) layerLType.Add(record.LayerName, record.LineType);
                if (!layerLineWeigth.ContainsKey(record.LayerName)) layerLineWeigth.Add(record.LayerName, record.LineWeight);
                try
                {
                    plotflag = record.PlottingFlag; // new property in DXFLib 1.0.2.0
                }
                catch (Exception err)
                { Logger.Error(err, "GetVectorDXF record.PlottingFlag - probably old DXFLib.dll in use - Ver. 1.0.2.0 is needed "); }

                // http://docs.autodesk.com/ACD/2011/DEU/filesDXF/WS1a9193826455f5ff18cb41610ec0a2e719-7a51.htm
                // record.Flags==0 - visible;	plotflag==1 - plot
                if (!layerPlot.ContainsKey(record.LayerName)) layerPlot.Add(record.LayerName, ((record.Flags == 0) && (plotflag == 1)));   // true when plot
                if (logEnable) Logger.Trace("Record Layer:{0,-10}   color:{1,3}   ltype:{2}   lweight:{3}   flags:{4}   plot:{5}", record.LayerName, record.Color, record.LineType, record.LineWeight, record.Flags, plotflag);
            }

            List<DXFLineTypeRecord> ltypes = doc.Tables.LineTypes;
            foreach (DXFLineTypeRecord lt in ltypes)
            {
                string pattern = "";
                if ((lt.PatternLength > 0) && (lt.ElementCount > 0))
                {
                    double[] tmp = new double[lt.ElementCount];
                    for (int i = 0; i < lt.ElementCount; i++)
                    {
                        if (lt.Elements[i].Length == 0)
                            tmp[i] = 0.5;
                        else
                            tmp[i] = Math.Abs(lt.Elements[i].Length);
                        pattern += string.Format(" {0} ", lt.Elements[i].Length);
                    }
                    if (!lineTypes.ContainsKey(lt.LineTypeName))
                        lineTypes.Add(lt.LineTypeName, tmp);
                }
            }

            foreach (DXFEntity dxfEntity in doc.Entities)				// process all entities
            {
                if ((backgroundWorker != null) && backgroundWorker.CancellationPending)
                {
                    backgroundEvent.Cancel = true;
                    break;
                }

                if (dxfEntity.GetType() == typeof(DXFInsert))			// any block to insert?
                {
                    DXFInsert insert = (DXFInsert)dxfEntity;               // get block coordinates

                    DXFPoint insertionPoint = insert.InsertionPoint;
                    double insertionAngle = 0;
                    DXFPoint insertionScaling = new DXFPoint();
                    insertionScaling.X = 1;
                    insertionScaling.Y = 1;
                    insertionScaling.Z = 1;

                    if (insert.RotationAngle != null)                                  // insertion angle in degrees
                        insertionAngle = (double)insert.RotationAngle;
                    if (insert.Scaling != null)
                    {
                        if (insert.Scaling.X != null) insertionScaling.X = insert.Scaling.X;
                        if (insert.Scaling.Y != null) insertionScaling.Y = insert.Scaling.Y;
                        if (insert.Scaling.Z != null) insertionScaling.Z = insert.Scaling.Z;
                    }
                    if (logEnable) 
                        Logger.Trace(" Block: at X:{0:0.00}  Y:{1:0.00}  a:{2:0.00}  scaleX:{3}  scaleY:{4}   layer:{5}", insertionPoint.X, insertionPoint.Y, insert.RotationAngle, insertionScaling.X, insertionScaling.Y, dxfEntity.LayerName);

                    foreach (DXFBlock block in doc.Blocks)
                    {
                        if (block.BlockName.ToString() == insert.BlockName)
                        {
                            if (logEnable) Logger.Trace("Block: {0}  -invisible:{1}", block.BlockName, block.IsInvisible);
                            dxfBlockColorNr = block.ColorNumber;
                            dxfBlockLineWeigth = block.LineWeight;
                            Graphic.SetComment("Block:" + block.BlockName);
                            Graphic.SetHeaderInfo(string.Format(" Block: {0} at X{1:0.000}  Y{2:0.000}  a{3:0.00}", block.BlockName, insertionPoint.X, insertionPoint.Y, insert.RotationAngle));
                            foreach (DXFEntity blockEntity in block.Children)
                            { ProcessEntities(blockEntity, insertionPoint, insertionAngle, insertionScaling, dxfEntity.LayerName); }
                        }
                    }
                }
                else
                {
                    DXFPoint emptyOffset = new DXFPoint
                    {
                        X = 0,
                        Y = 0,
                        Z = 0
                    };
                    DXFPoint emptyScaling = new DXFPoint
                    {
                        X = 1,
                        Y = 1,
                        Z = 1
                    };

                    ProcessEntities(dxfEntity, emptyOffset, 0, emptyScaling, dxfEntity.LayerName);
                }
            }
        }

        /// <summary>
        /// Parse DXF entities
        /// </summary>
        /// <param name="entity">Entity to convert</param>
        /// <param name="offsetX">Offset to apply if called by block insertion</param>
        /// <returns></returns>                       
        private static void ProcessEntities(DXFEntity entity, DXFPoint offset, double offsetAngle, DXFPoint offsetScaling, string layerName)		// double offsetX=0, double offsetY=0
        {
            shapeCounter++;

            if ((backgroundWorker != null) && (countDXFEntities > 0)) backgroundWorker.ReportProgress(countDXFEntity++ * 100 / countDXFEntities);

            /* skip entity if layer is invisible or printing is disabled */
            if (Properties.Settings.Default.importDXFDontPlot && layerPlot.ContainsKey(layerName) && !layerPlot[layerName])
            {
                Graphic.SetHeaderInfo(string.Format(" Hide DXF Entity:{0}  of Layer:{1}  ", entity.GetType(), layerName));
                Graphic.SetHeaderMessage(string.Format(" {0}-1202: DXF Layer '{1}' is not visible or plotting is disabled and will not be imported", CodeMessage.Attention, layerName));
                return;
            }

            /* get color        */
            dxfColorNr = entity.ColorNumber;
            if (dxfColorNr > 255)                           // DXF 256 = color BYLAYER
                if (!String.IsNullOrEmpty(layerName) && layerColor.ContainsKey(layerName))
                    dxfColorNr = layerColor[layerName];
                else if (dxfColorNr == 0)                       // DXF 0 = color BYBLOCK
                    dxfColorNr = dxfBlockColorNr;

            if (dxfColorNr < 0) dxfColorNr = 0;             // for safety
            if (dxfColorNr > 255) dxfColorNr = 7;

            if (Properties.Settings.Default.importDXFSwitchWhite && (dxfColorNr == 7))	// == 7		7=FFFFFF=white; 0=000000=black
            {
                dxfColorNr = 0; // = 0
            }

            dxfColorHex = GetColorFromID(dxfColorNr);

            /* get fill */
            #region Hatch
            if (entity.GetType() == typeof(DXFHatch))
            {
                DXFHatch hatch = (DXFHatch)entity;
                dxfColorFill = dxfColorHex;
                Logger.Warn("⚠⚠⚠ Hatch-fill is not fully implemented: Layer:'{0}' ColorNr.:{1} LineWeight:{2} Handle:{3} ", hatch.LayerName, hatch.ColorNumber, hatch.LineWeight, hatch.Handle);
                Graphic.SetHeaderInfo(string.Format(" DXFHatch - fill is not fully implemented - Layer:{0}  ", hatch.LayerName));
            }
            #endregion

            /* get lineWeight   */
            dxfLineWeigth = entity.LineWeight;
            if (dxfLineWeigth == -1) dxfLineWeigth = dxfBlockLineWeigth;        // -1 = ByBlock
            else if (dxfLineWeigth == -2)                                       // -2 = ByLayer
            {
                if (!String.IsNullOrEmpty(layerName) && layerLineWeigth.ContainsKey(layerName))
                    dxfLineWeigth = layerLineWeigth[layerName];
            }
            else if (dxfLineWeigth == -3)                                       // -3 = Standard
                dxfLineWeigth = 0;

            /* get Dash pattern   */
            if ((entity.LineType == null) || (entity.LineType == "ByLayer"))
            {
                if ((layerLType != null) && (layerName != null) && (layerLType.ContainsKey(layerName)))              // check if layer name is known
                {
                    string dashType = layerLType[layerName];                                    // get name of pattern
                    if (!String.IsNullOrEmpty(dashType) && lineTypes.ContainsKey(dashType))    // check if pattern name is known
                        Graphic.SetDash(lineTypes[dashType]);
                }
            }
            else
            {
                if (lineTypes.ContainsKey(entity.LineType))         // check if pattern name is known
                    Graphic.SetDash(lineTypes[entity.LineType]);
            }

            if (logEnable) Logger.Trace("ProcessEntity Layer: {0,-10}  Entity: {1,-22}  EntityColorNumber:{2} -use:{3}   EntityLineWeight:{4,3} -use:{5,3}  -invisible:{6}", layerName, entity.GetType(), entity.ColorNumber, dxfColorNr, entity.LineWeight, dxfLineWeigth, entity.IsInvisible);

            if (layerName != wasSetLayer) { Graphic.SetLayer(layerName); Graphic.SetComment("Layer:" + layerName); wasSetLayer = layerName; }
            if (dxfColorHex != wasSetPenColor) { Graphic.SetPenColor(dxfColorHex); wasSetPenColor = dxfColorHex; }
            if (dxfColorNr != wasSetPenColorId) { Graphic.SetPenColorId(dxfColorNr); wasSetPenColorId = dxfColorNr; }
            if (dxfColorFill != wasSetPenFill) { Graphic.SetPenFill(dxfColorFill); wasSetPenFill = dxfColorFill; }
            if (dxfLineWeigth != wasSetPenWidth) { Graphic.SetPenWidth(string.Format("{0:0.00}", ((double)dxfLineWeigth / 100)).Replace(',', '.')); wasSetPenWidth = dxfLineWeigth; }//convert to mm, then to string

            DXFPoint tmp = new DXFPoint();
            DXFPoint position = new DXFPoint
            {
                X = 0,
                Y = 0
            };
            int index = 0;

            #region DXFPoint
            if (entity.GetType() == typeof(DXFPointEntity))
            {
                DXFPointEntity point = (DXFPointEntity)entity;
                position = ApplyOffsetAndAngle(point.Location, offset, offsetAngle, offsetScaling);
                if (logPosition) Logger.Trace(" Point: {0:0.00};{1:0.00} ", position.X, position.Y);
                GCodeDotOnly(position);//, "Start Point");
            }
            #endregion
            #region DXFLWPolyline
            else if (entity.GetType() == typeof(DXFLWPolyLine))
            {
                Graphic.SetGeometry("Polyline");
                DXFLWPolyLine lp = (DXFLWPolyLine)entity;
                index = 0;
                double bulge = 0;
                DXFLWPolyLine.Element coordinate;
                bool roundcorner = false;
                for (int i = 0; i < lp.VertexCount; i++)
                {
                    coordinate = lp.Elements[i];
                    bulge = coordinate.Bulge;
                    position = ApplyOffsetAndAngle(coordinate.Vertex, offset, offsetAngle, offsetScaling);

                    if (i == 0)
                    {
                        if (!nodesOnly)
                        {
                            DXFStartPath(position);//, "Start LWPolyLine - Nr pts " + lp.VertexCount.ToString( ));
                        }
                        else { GCodeDotOnly(position); }//, "Start LWPolyLine"); }
                        if (logPosition) Logger.Trace("Start DXFLWPolyLine count:{0} X:{1:0.000} Y:{2:0.000} Z:{3:0.000}", lp.VertexCount, position.X, position.Y, position.Z);
                    }

                    if ((!roundcorner) && (i > 0))
                    {
                        if (logPosition) Logger.Trace("PolyLine moveTo index:{0} X:{1:0.000} Y:{2:0.000} Z:{3:0.000}", i, position.X, position.Y, position.Z);
                        DXFMoveTo(position);    //, "");
                    }
                    if (bulge != 0)
                    {
                        if (logPosition) Logger.Trace("PolyLine bulge  index:{0} val1X:{1:0.000} val1Y:{2:0.000} val2X:{3:0.000} val2Y:{4:0.000}", i, lp.Elements[i].Vertex.X, lp.Elements[i].Vertex.Y, lp.Elements[i + 1].Vertex.X, lp.Elements[i + 1].Vertex.Y);

                        if (i < (lp.VertexCount - 1))
                            AddRoundCorner((DXFPoint)lp.Elements[i].Vertex, (DXFPoint)lp.Elements[i + 1].Vertex, bulge, offset, offsetAngle);
                        else
                            if (lp.Flags.HasFlag(DXFLWPolyLine.FlagsEnum.closed))        // == DXFLWPolyLine.FlagsEnum.closed)
                            AddRoundCorner((DXFPoint)lp.Elements[i].Vertex, (DXFPoint)lp.Elements[0].Vertex, bulge, offset, offsetAngle);
                        roundcorner = true;
                    }
                    else
                        roundcorner = false;
                }
                if (logPosition) Logger.Trace(" LWPolyLine lp.Flags {0}", lp.Flags);
                if (lp.Flags.HasFlag(DXFLWPolyLine.FlagsEnum.closed))        // == DXFLWPolyLine.FlagsEnum.closed)
                {
                    position = ApplyOffsetAndAngle(lp.Elements[0].Vertex, offset, offsetAngle, offsetScaling); // move to start position
                    if (logPosition) Logger.Trace(" LWPolyLine close X:{0:0.000}  Y:{1:0.000}", position.X, position.Y);
                    DXFMoveTo(position);    //, "End LWPolyLine " + lp.Flags.ToString());
                }
                DXFStopPath();
            }
            #endregion
            #region DXFPolyline
            else if (entity.GetType() == typeof(DXFPolyLine))
            {
                Graphic.SetGeometry("Polyline");
                DXFPolyLine lp = (DXFPolyLine)entity;
                double bulge = 0;
                //    bool roundcorner = false;
                DXFEntity vertex;
                DXFVertex coordinate;
                index = 0;
                tmp = new DXFPoint();
                //   foreach (DXFVertex coordinate in lp.Children)
                for (int i = 0; i < lp.Children.Count; i++)
                {
                    vertex = lp.Children[i];
                    if (vertex.GetType() == typeof(DXFVertex))
                    {
                        coordinate = (DXFVertex)vertex;
                        bulge = coordinate.Buldge;
                        if (coordinate.Location.X != null && coordinate.Location.Y != null)
                        {
                            position = ApplyOffsetAndAngle(coordinate.Location, offset, offsetAngle, offsetScaling);
                            if (!nodesOnly)
                            {
                                if (index == 0)
                                {
                                    tmp = position;
                                    DXFStartPath(position);//, "Start PolyLine");
                                    if (logPosition) Logger.Trace("Start DXFPolyLine count:{0} X:{1:0.000} Y:{2:0.000} Z:{3:0.000}", lp.Children.Count, position.X, position.Y, position.Z);
                                }
                                else
                                    DXFMoveTo(position);    //, "");
                            }
                            else { GCodeDotOnly(position); }//, "PolyLine"); }
                            index++;
                        }
                        if (bulge != 0)
                        {
                            if (logPosition) Logger.Trace("PolyLine bulge  index:{0} val1X:{1:0.000} val1Y:{2:0.000} val2X:{3:0.000} val2Y:{4:0.000}", i, ((DXFVertex)lp.Children[i]).Location.X, ((DXFVertex)lp.Children[i]).Location.Y, ((DXFVertex)lp.Children[i + 1]).Location.X, ((DXFVertex)lp.Children[i + 1]).Location.Y);

                            if (i < (lp.Children.Count - 1))
                                AddRoundCorner((DXFPoint)((DXFVertex)lp.Children[i]).Location, (DXFPoint)((DXFVertex)lp.Children[i + 1]).Location, bulge, offset, offsetAngle);
                            else
                                if (lp.Flags.HasFlag(DXFPolyLine.FlagsEnum.closed))        // == DXFPolyLine.FlagsEnum.closed)
                                AddRoundCorner((DXFPoint)((DXFVertex)lp.Children[i]).Location, (DXFPoint)((DXFVertex)lp.Children[0]).Location, bulge, offset, offsetAngle);
                            //        roundcorner = true;
                        }
                        //    else
                        //        roundcorner = false;
                    }
                }
                if (lp.Flags.HasFlag(DXFPolyLine.FlagsEnum.closed))        // == DXFPolyLine.FlagsEnum.closed) //if ((lp.Flags > 0))
                {
                    if (logPosition) Logger.Trace("Close path Flags:{0}", lp.Flags);
                    DXFMoveTo(tmp);    //, "End PolyLine " + lp.Flags.ToString());
                }
                DXFStopPath();
            }
            #endregion
            #region DXFLine
            else if (entity.GetType() == typeof(DXFLine))
            {
                Graphic.SetGeometry("Line");
                DXFLine line = (DXFLine)entity;
                position = ApplyOffsetAndAngle(line.Start, offset, offsetAngle, offsetScaling);
                tmp = ApplyOffsetAndAngle(line.End, offset, offsetAngle, offsetScaling);
                if (logPosition) Logger.Trace(" Line from: {0:0.000};{1:0.000}  To: {2:0.000};{3:0.000}", position.X, position.Y, tmp.X, tmp.Y);
                if (!nodesOnly)
                {
                    DXFStartPath(position);//, "Start Line");
                    DXFMoveTo(tmp);    //, "");
                }
                else
                {
                    GCodeDotOnly(position);//, "Start Line");
                    GCodeDotOnly(tmp);//, "End Line");
                }
                DXFStopPath();
            }
            #endregion
            #region DXFSpline
            else if (entity.GetType() == typeof(DXFSpline))
            {

                bool newAlgorythm = false;

                Graphic.SetGeometry("Spline");
                DXFSpline spline = (DXFSpline)entity;
                index = 0;
                DXFPoint last = ApplyOffsetAndAngle(spline.ControlPoints[0], offset, offsetAngle, offsetScaling);

                int knots = spline.KnotCount;
                int ctrls = spline.ControlPoints.Count;//spline.ControlPointCount;

                if (logPosition) Logger.Info("Spline Flag:{0}  Degree:{1}  Knots:{2}  ControlPoints:{3}  FitPoints:{4}", spline.Flags, spline.Degree, spline.KnotCount, spline.ControlPoints.Count, spline.FitPoints.Count);

                if (true)
                {
                    //https://github.com/ixmilia/converters/blob/fc469324e90cb09daa437089cc04f2e1baa5a352/src/IxMilia.Converters/Spline2.cs
                    //https://github.com/ixmilia/converters/blob/main/src/IxMilia.Converters/DxfToSvgConverter.cs l:555
                    var spline2 = new Spline2(spline.Degree,
                        spline.ControlPoints.Select(p => new SplinePoint2(ApplyOffsetAndAngle(p, offset, offsetAngle, offsetScaling))),
                        spline.KnotValues);
                    var beziers = spline2.ToBeziers();

                    DXFStartPath(beziers[0].Start.ToDXFPoint());       // last);
                    foreach (Bezier2 b in beziers)
                    {
                        ImportMath.CalcCubicBezier(b.Start.ToPoint(), b.Control1.ToPoint(), b.Control2.ToPoint(), b.End.ToPoint(), DXFMoveTo, "spline3");
                    }
                    DXFStopPath();
                }
                else
                {
                    // from Inkscape DXF import - modified
                    // https://gitlab.com/inkscape/extensions/blob/master/dxf_input.py#L106
                    // https://help.autodesk.com/view/OARX/2021/ENU/?guid=GUID-235B22E0-A567-4CF6-92D3-38A2306D73F3
                    if ((ctrls > 3) && (knots == ctrls + 4))    //  # cubic
                    {
                        if (ctrls > 4)
                        {
                            for (int i = (knots - 5); i > 3; i--)
                            {
                                if ((spline.KnotValues[i] != spline.KnotValues[i - 1]) && (spline.KnotValues[i] != spline.KnotValues[i + 1]))
                                {
                                    double a0 = (spline.KnotValues[i] - spline.KnotValues[i - 2]) / (spline.KnotValues[i + 1] - spline.KnotValues[i - 2]);
                                    double a1 = (spline.KnotValues[i] - spline.KnotValues[i - 1]) / (spline.KnotValues[i + 2] - spline.KnotValues[i - 1]);
                                    tmp = new DXFPoint
                                    {
                                        X = (double)((1.0 - a1) * spline.ControlPoints[i - 2].X + a1 * spline.ControlPoints[i - 1].X),
                                        Y = (double)((1.0 - a1) * spline.ControlPoints[i - 2].Y + a1 * spline.ControlPoints[i - 1].Y)
                                    };
                                    spline.ControlPoints.Insert(i - 1, tmp);
                                    spline.ControlPoints[i - 2].X = (1.0 - a0) * spline.ControlPoints[i - 3].X + a0 * spline.ControlPoints[i - 2].X;
                                    spline.ControlPoints[i - 2].Y = (1.0 - a0) * spline.ControlPoints[i - 3].Y + a0 * spline.ControlPoints[i - 2].Y;
                                    spline.KnotValues.Insert(i, spline.KnotValues[i]);
                                }
                            }
                            knots = spline.KnotValues.Count;
                            for (int i = (knots - 6); i > 3; i -= 2)
                            {
                                if ((spline.KnotValues[i] != spline.KnotValues[i - 2]) && (spline.KnotValues[i - 1] != spline.KnotValues[i + 1]) && (spline.KnotValues[i - 2] != spline.KnotValues[i]))
                                {
                                    double a1 = (spline.KnotValues[i] - spline.KnotValues[i - 1]) / (spline.KnotValues[i + 2] - spline.KnotValues[i - 1]);
                                    tmp = new DXFPoint
                                    {
                                        X = (double)((1.0 - a1) * spline.ControlPoints[i - 2].X + a1 * spline.ControlPoints[i - 1].X),
                                        Y = (double)((1.0 - a1) * spline.ControlPoints[i - 2].Y + a1 * spline.ControlPoints[i - 1].Y)
                                    };
                                    spline.ControlPoints.Insert(i - 1, tmp);
                                }
                            }
                        }
                        ctrls = spline.ControlPoints.Count;
                        DXFStartPath(last);//, cmt);
                        for (int i = 0; i < Math.Floor((ctrls - 1) / 3d); i++)     // for i in range(0, (ctrls - 1) // 3):
                        {
                            if (!nodesOnly)
                                ImportMath.CalcCubicBezier(ToWindowsSystemPoint(last),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[3 * i + 1], offset, offsetAngle, offsetScaling)),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[3 * i + 2], offset, offsetAngle, offsetScaling)),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[3 * i + 3], offset, offsetAngle, offsetScaling)),
                                                            DXFMoveTo, "C");
                            else
                            {
                                GCodeDotOnly(last);//, "");
                                GCodeDotOnly(ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[3 * i + 3], offset, offsetAngle, offsetScaling)));//, "");
                            }
                            last = ApplyOffsetAndAngle(spline.ControlPoints[3 * i + 3], offset, offsetAngle, offsetScaling);
                        }
                        DXFStopPath();// true);
                                      //Logger.Trace(" stop path");

                    }
                    if ((ctrls == 3) && (knots == 6))           //  # quadratic
                    {   //  path = 'M %f,%f Q %f,%f %f,%f' % (vals[groups['10']][0], vals[groups['20']][0], vals[groups['10']][1], vals[groups['20']][1], vals[groups['10']][2], vals[groups['20']][2])
                        if (!nodesOnly)
                        {
                            DXFStartPath(last);//, cmt);
                            ImportMath.CalcQuadraticBezier(ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[0], offset, offsetAngle, offsetScaling)),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[1], offset, offsetAngle, offsetScaling)),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[2], offset, offsetAngle, offsetScaling)),
                                                            DXFMoveTo, "Q");
                        }
                        else
                        {
                            GCodeDotOnly(last);//, "");
                            GCodeDotOnly(ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[2], offset, offsetAngle, offsetScaling)));//, "");
                        }
                        DXFStopPath();// true);
                    }
                    if ((ctrls == 5) && (knots == 8))           //  # spliced quadratic
                    {   //  path = 'M %f,%f Q %f,%f %f,%f Q %f,%f %f,%f' % (vals[groups['10']][0], vals[groups['20']][0], vals[groups['10']][1], vals[groups['20']][1], vals[groups['10']][2], vals[groups['20']][2], vals[groups['10']][3], vals[groups['20']][3], vals[groups['10']][4], vals[groups['20']][4])
                        if (!nodesOnly)
                        {
                            DXFStartPath(last);//, cmt);
                            ImportMath.CalcQuadraticBezier(ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[0], offset, offsetAngle, offsetScaling)),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[1], offset, offsetAngle, offsetScaling)),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[2], offset, offsetAngle, offsetScaling)), DXFMoveTo, "SQ");
                            ImportMath.CalcQuadraticBezier(ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[2], offset, offsetAngle, offsetScaling)),                     // 2022-03-19 start at 2 not 3
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[3], offset, offsetAngle, offsetScaling)),
                                                            ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[4], offset, offsetAngle, offsetScaling)), DXFMoveTo, "SQ");
                        }
                        else
                        {
                            GCodeDotOnly(last);//, "");
                            GCodeDotOnly(ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[2], offset, offsetAngle, offsetScaling)));//, "");
                            GCodeDotOnly(ToWindowsSystemPoint(ApplyOffsetAndAngle(spline.ControlPoints[5], offset, offsetAngle, offsetScaling)));//, "");
                        }
                        DXFStopPath();// true);
                    }
                }
            }
            #endregion
            #region DXFCircle
            else if (entity.GetType() == typeof(DXFCircle))
            {
                DXFCircle circle = (DXFCircle)entity;
                position = ApplyOffsetAndAngle(circle.Center, offset, offsetAngle, offsetScaling);
                if (logPosition) Logger.Trace(" Circle center: {0:0.000};{1:0.000}  R: {2:0.000} Scaling:{3}", position.X, position.Y, circle.Radius, (double)offsetScaling.X);
                if (!nodesOnly)
                {
                    DXFStartPath((double)position.X + circle.Radius * (double)offsetScaling.X, (double)position.Y, position.Z);//, "Start Circle");
                    if (Properties.Settings.Default.importSVGCircleToDot)
                    {
                        if (Properties.Settings.Default.importSVGCircleToDotZ)
                        {
                            Graphic.SetGeometry("CircleToDot");
                            GCodeDotOnlyWithZ((double)position.X, (double)position.Y, (double)circle.Radius * (double)offsetScaling.X, "Dot r=Z");
                        }
                        else
                        {
                            Graphic.SetGeometry("CircleToDotZ");
                            GCodeDotOnly((double)position.X, (double)position.Y);//, "Circle");
                        }
                    }
                    else
                    {
                        Graphic.SetGeometry("Circle");
                        Graphic.AddCircle((double)position.X, (double)position.Y, circle.Radius * (double)offsetScaling.X);
                    }
                }
                else
                {   //DXFStartPath((double)position.X, (double)position.Y, "Start Circle");
                    Graphic.SetGeometry("Circle");
                    if (Properties.Settings.Default.importSVGCircleToDotZ)
                        GCodeDotOnlyWithZ((double)position.X, (double)position.Y, (double)circle.Radius * (double)offsetScaling.X, "Dot r=Z");
                    else
                        GCodeDotOnly((double)position.X, (double)position.Y);//, "Circle");
                }
                DXFStopPath();
            }
            #endregion
            #region DXFEllipse
            else if (entity.GetType() == typeof(DXFEllipse))
            {
                CalcEllipse((DXFEllipse)entity, offset, offsetAngle, offsetScaling);
            }
            #endregion
            #region DXFArc
            else if (entity.GetType() == typeof(DXFArc))
            {
                Graphic.SetGeometry("Arc");
                DXFArc arc = (DXFArc)entity;
                double X = (double)arc.Center.X;
                double Y = (double)arc.Center.Y;
                double R = arc.Radius;

                // ARC entites are always conter-clockwise.
                if (!nodesOnly)
                {
                    double startA = arc.StartAngle * Math.PI / 180;
                    double endA = arc.EndAngle * Math.PI / 180;
                    double startX = (double)(X + R * Math.Cos(startA) * (double)offsetScaling.X);
                    double startY = (double)(Y + R * Math.Sin(startA) * (double)offsetScaling.Y);
                    double endX = (double)(X + R * Math.Cos(endA) * (double)offsetScaling.X);
                    double endY = (double)(Y + R * Math.Sin(endA) * (double)offsetScaling.Y);

                    if (logEnable)
                        Logger.Trace(" Arc center: x:{0:0.000}  y:{1:0.000}  R:{2:0.000}  start:{3:0.0}   end:{4:0.0}", X, Y, R, arc.StartAngle, arc.EndAngle);

                    double stx = startX, sty = startY, gx = endX, gy = endY, gi = (X - startX), gj = (Y - startY);
                    if (offsetAngle != 0)
                    {
                        stx = (startX) * Math.Cos(offsetAngle * Math.PI / 180) - (startY) * Math.Sin(offsetAngle * Math.PI / 180);
                        sty = (startX) * Math.Sin(offsetAngle * Math.PI / 180) + (startY) * Math.Cos(offsetAngle * Math.PI / 180);
                        gx = (endX) * Math.Cos(offsetAngle * Math.PI / 180) - (endY) * Math.Sin(offsetAngle * Math.PI / 180);
                        gy = (endX) * Math.Sin(offsetAngle * Math.PI / 180) + (endY) * Math.Cos(offsetAngle * Math.PI / 180);
                        gi = (X - startX) * Math.Cos(offsetAngle * Math.PI / 180) - (Y - startY) * Math.Sin(offsetAngle * Math.PI / 180);
                        gj = (X - startX) * Math.Sin(offsetAngle * Math.PI / 180) + (Y - startY) * Math.Cos(offsetAngle * Math.PI / 180);
                    }

                    DXFStartPath(stx + (double)offset.X, sty + (double)offset.Y, arc.Center.Z);//, "Start Arc");
                    Graphic.AddArc(false, gx + (double)offset.X, gy + (double)offset.Y, gi, gj);//, "Arc");
                    DXFStopPath();
                }
                else
                { GCodeDotOnly(X, Y); }//, "Arc"); }

            }
            #endregion
            #region DXFMText
            else if (entity.GetType() == typeof(DXFMText))
            {   // https://www.autodesk.com/techpubs/autocad/acad2000/dxf/mtext_dxf_06.htm
                Graphic.SetGeometry("Text");
                DXFMText txt = (DXFMText)entity;
                //          xyPoint origin = new xyPoint(0, 0);
                GCodeFromFont.Reset();
                double angle = 0;
                foreach (var entry in txt.Entries)
                {
                    try
                    {
                        if (entry.GroupCode == 1) { GCodeFromFont.GCText = entry.Value.ToString(); }
                        else if (entry.GroupCode == 40) { GCodeFromFont.GCHeight = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat) * (double)offsetScaling.X; }
                        else if (entry.GroupCode == 41) { GCodeFromFont.GCWidth = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat) * (double)offsetScaling.Y; }
                        else if (entry.GroupCode == 71) { GCodeFromFont.GCAttachPoint = Convert.ToInt16(entry.Value); }
                        else if (entry.GroupCode == 10) { GCodeFromFont.GCOffX = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                        else if (entry.GroupCode == 20) { GCodeFromFont.GCOffY = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                        else if (entry.GroupCode == 50) { angle = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }// + offsetAngle)* Math.PI / 180; } 
                        else if (entry.GroupCode == 44) { GCodeFromFont.GCSpacing = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                        else if (entry.GroupCode == 7)
                        {
                            GCodeFromFont.GCFontName = "lff\\" + entry.Value.ToString();
                            if (!GCodeFromFont.GCFontName.ToLower().EndsWith("lff"))
                            { GCodeFromFont.GCFontName += ".lff"; }
                        }
                    }
                    catch (Exception err)
                    { Logger.Error(err, "DXFMText entry.GroupCode:{0}   entry.Value:{1}", entry.GroupCode, entry.Value); }
                }
                tmp = new DXFPoint
                {
                    X = GCodeFromFont.GCOffX,
                    Y = GCodeFromFont.GCOffY
                };
                tmp = ApplyOffsetAndAngle(tmp, offset, offsetAngle, offsetScaling);
                GCodeFromFont.GCOffX = (double)tmp.X;
                GCodeFromFont.GCOffY = (double)tmp.Y;

                GCodeFromFont.GCAngleRad = (angle + offsetAngle) * Math.PI / 180;
                if (logEnable) Logger.Trace(" Font:{0} Text: {1} X{2:0.00} Y{3:0.00} a{4:0.00} oa{5:0.00}", GCodeFromFont.GCFontName, GCodeFromFont.GCText, GCodeFromFont.GCOffX, GCodeFromFont.GCOffY, angle, offsetAngle);
                GCodeFromFont.GetCode(0);   // no page break
            }
            #endregion
            #region DXFText
            else if (entity.GetType() == typeof(DXFText))
            {   // https://www.autodesk.com/techpubs/autocad/acad2000/dxf/mtext_dxf_06.htm
                Graphic.SetGeometry("Text");
                DXFText txt = (DXFText)entity;
                //          xyPoint origin = new xyPoint(0, 0);
                GCodeFromFont.Reset();
                double angle = 0;
                foreach (var entry in txt.Entries)
                {
                    try
                    {
                        if (entry.GroupCode == 1) { GCodeFromFont.GCText = entry.Value.ToString(); }
                        else if (entry.GroupCode == 40) { GCodeFromFont.GCHeight = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat) * (double)offsetScaling.X; }
                        else if (entry.GroupCode == 41) { GCodeFromFont.GCWidth = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat) * (double)offsetScaling.Y; }
                        else if (entry.GroupCode == 71) { GCodeFromFont.GCAttachPoint = Convert.ToInt16(entry.Value); }
                        else if (entry.GroupCode == 10) { GCodeFromFont.GCOffX = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                        else if (entry.GroupCode == 20) { GCodeFromFont.GCOffY = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                        else if (entry.GroupCode == 50) { angle = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }// + offsetAngle)* Math.PI / 180; } 
                        else if (entry.GroupCode == 44) { GCodeFromFont.GCSpacing = double.Parse(entry.Value, CultureInfo.InvariantCulture.NumberFormat); }
                        else if (entry.GroupCode == 7)
                        {
                            GCodeFromFont.GCFontName = "lff\\" + entry.Value.ToString();
                            if (!GCodeFromFont.GCFontName.ToLower().EndsWith("lff"))
                            { GCodeFromFont.GCFontName += ".lff"; }
                        }
                    }
                    catch (Exception err)
                    { Logger.Error(err, "DXFText entry.GroupCode:{0}   entry.Value:{1}", entry.GroupCode, entry.Value); }
                }
                tmp = new DXFPoint
                {
                    X = GCodeFromFont.GCOffX,
                    Y = GCodeFromFont.GCOffY
                };
                tmp = ApplyOffsetAndAngle(tmp, offset, offsetAngle, offsetScaling);
                GCodeFromFont.GCOffX = (double)tmp.X;
                GCodeFromFont.GCOffY = (double)tmp.Y;

                GCodeFromFont.GCAngleRad = (angle + offsetAngle) * Math.PI / 180;
                if (logEnable) Logger.Trace(" Font:{0} Text: {1} X{2:0.00} Y{3:0.00} a{4:0.00} oa{5:0.00}", GCodeFromFont.GCFontName, GCodeFromFont.GCText, GCodeFromFont.GCOffX, GCodeFromFont.GCOffY, angle, offsetAngle);
                GCodeFromFont.GetCode(0);   // no page break
            }
            #endregion
            #region Hatch
            else if (entity.GetType() == typeof(DXFHatch))
            {
                // already processed
            }
            #endregion
            else
            {
                Graphic.SetHeaderInfo(" Unknown DXF Entity: " + entity.GetType().ToString());
                Graphic.SetHeaderMessage(string.Format(" {0}-1201: Unknown DXF Entity: {1}", CodeMessage.Attention, entity.GetType().ToString()));
            }
        }

        private static double RotateGetX(DXFPoint r, double angleRad)
        { return (double)(r.X * Math.Cos(angleRad) - r.Y * Math.Sin(angleRad)); }
        private static double RotateGetY(DXFPoint r, double angleRad)
        { return (double)(r.X * Math.Sin(angleRad) + r.Y * Math.Cos(angleRad)); }

        private static void CalcEllipse(DXFEllipse ellipse, DXFPoint offset, double offsetAngle, DXFPoint scaling)
        {   // from Inkscape DXF import - modified
            // https://gitlab.com/inkscape/extensions/blob/master/dxf_input.py#L341
            Graphic.SetGeometry("Ellipse");
            double angleRad = offsetAngle * Math.PI / 180;
            float xc = (float)(RotateGetX(ellipse.Center, angleRad) + offset.X); // (ellipse.Center.X * Math.Cos(angleRad) - ellipse.Center.Y * Math.Sin(angleRad) + offset.X);
            float yc = (float)(RotateGetY(ellipse.Center, angleRad) + offset.Y); //(ellipse.Center.X * Math.Sin(angleRad) + ellipse.Center.Y * Math.Cos(angleRad) + offset.Y);

            float xm = (float)(RotateGetX(ellipse.MainAxis, angleRad) * (double)scaling.X); //(ellipse.MainAxis.X * Math.Cos(angleRad) - ellipse.MainAxis.Y * Math.Sin(angleRad));
            float ym = (float)(RotateGetY(ellipse.MainAxis, angleRad) * (double)scaling.Y); //(ellipse.MainAxis.X * Math.Sin(angleRad) + ellipse.MainAxis.Y * Math.Cos(angleRad));
            float w = (float)ellipse.AxisRatio;
            double a2 = -ellipse.StartParam;// + offsetAngle;   issue #359
            double a1 = -ellipse.EndParam;// + offsetAngle;

            float rm = (float)Math.Sqrt(xm * xm + ym * ym);
            double a = Math.Atan2(-ym, xm);
            float diff = (float)((a2 - a1 + 2 * Math.PI) % (2 * Math.PI));

            if (logPosition) Logger.Trace(" Ellipse center: {0:0.000};{1:0.000}  R1: {2:0.000} R2: {3:000} Start: {4:0.000} End: {5:000} Handle:{6}", xc, yc, rm, w * rm, ellipse.StartParam, ellipse.EndParam, ellipse.Handle);
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
                DXFStartPath(xc + x1, yc - y1, ellipse.Center.Z);//, "Start Ellipse 1");
                ImportMath.CalcArc(xc + x1, yc - y1, rm, w * rm, (float)(-180.0 * a / Math.PI), large, 0, (xc + x2), (yc - y2), DXFMoveTo);
                //  path = 'M %f,%f A %f,%f %f %d 0 %f,%f' % (xc + x1, yc - y1, rm, w* rm, -180.0 * a / math.pi, large, xc + x2, yc - y2)
            }
            else
            {
                DXFStartPath(xc + xm, yc + ym, ellipse.Center.Z);//, "Start Ellipse 2");
                ImportMath.CalcArc(xc + xm, yc + ym, rm, w * rm, (float)(-180.0 * a / Math.PI), 1, 0, xc - xm, yc - ym, DXFMoveTo);
                ImportMath.CalcArc(xc - xm, yc - ym, rm, w * rm, (float)(-180.0 * a / Math.PI), 1, 0, xc + xm, yc + ym, DXFMoveTo);
                //    path = 'M %f,%f A %f,%f %f 1 0 %f,%f %f,%f %f 1 0 %f,%f z' % (xc + xm, yc - ym, rm, w* rm, -180.0 * a / math.pi, xc - xm, yc + ym, rm, w* rm, -180.0 * a / math.pi, xc + xm, yc - ym)
            }
            DXFStopPath();
        }

        private static DXFPoint ApplyOffsetAndAngle(DXFPoint location, DXFPoint offset, double offsetAngleDegree, DXFPoint scaling)
        {
            DXFPoint tmp = new DXFPoint();
            double offsetAngle = Math.PI * offsetAngleDegree / 180;
            tmp.X = (double)scaling.X * (location.X * Math.Cos(offsetAngle) - location.Y * Math.Sin(offsetAngle)) + offset.X;
            tmp.Y = (double)scaling.Y * (location.X * Math.Sin(offsetAngle) + location.Y * Math.Cos(offsetAngle)) + offset.Y;
            tmp.Z = (double)scaling.Z * location.Z + offset.Z;;
            return tmp;
        }

        private static Point ToWindowsSystemPoint(DXFPoint tmp)
        { return new Point((double)tmp.X, (double)tmp.Y); }

        /// <summary>
        /// Calculate round corner of DXFLWPolyLine if Bulge is given
        /// </summary>
        /// <param name="var1">First vertex coord</param>
        /// <param name="var2">Second vertex</param>
        /// <returns></returns>
        //    private static void AddRoundCorner(DXFLWPolyLine.Element var1, DXFLWPolyLine.Element var2, DXFPoint offset, double angleDegree)
        private static void AddRoundCorner(DXFPoint var1, DXFPoint var2, double bulge, DXFPoint offset, double angleDegree)
        {
            double angleRad = angleDegree * Math.PI / 180;
            double p1x = (double)(var1.X * Math.Cos(angleRad) - var1.Y * Math.Sin(angleRad));       //var1.Vertex.X;
            double p1y = (double)(var1.X * Math.Sin(angleRad) + var1.Y * Math.Cos(angleRad));       //var1.Vertex.Y;
            double p2x = (double)(var2.X * Math.Cos(angleRad) - var2.Y * Math.Sin(angleRad));       //var2.Vertex.X;
            double p2y = (double)(var2.X * Math.Sin(angleRad) + var2.Y * Math.Cos(angleRad));       //var2.Vertex.Y;

            //Definition of bulge, from Autodesk DXF fileformat specs
            double angle = Math.Abs(Math.Atan(bulge) * 4);
            bool girou = false;

            //For my method, this angle should always be less than 180. 
            if (angle > Math.PI)
            {
                angle = Math.PI * 2 - angle;
                girou = true;
            }

            //Distance between the two vertexes, the angle between Center-P1 and P1-P2 and the arc radius
            double distance = Math.Sqrt(Math.Pow(p1x - p2x, 2) + Math.Pow(p1y - p2y, 2));
            double alpha = (Math.PI - angle) / 2;
            double ratio = distance * Math.Sin(alpha) / Math.Sin(angle);
            if (angle == Math.PI)
                ratio = distance / 2;

            double xc, yc, direction;
            if (logPosition) Logger.Trace(" AddRoundCorner p1x:{0:0.00} p1y:{1:0.00}  p2x:{2:0.00} p2y:{3:0.00} distance:{4:0.00} alpha:{5:0.00} ratio:{6:0.00} ", p1x, p1y, p2x, p2y, distance, alpha, ratio);
            if (distance == 0)  // no round off needed
            {
                Graphic.AddLine(new Point((p2x + (double)offset.X), (p2y + (double)offset.Y)));
                if (logPosition) Logger.Trace(" AddRoundCorner distance=0, make straight move");
                return;
            }

            //Used to invert the signal of the calculations below
            bool isg2 = bulge < 0;
            if (bulge < 0)
                direction = 1;
            else
                direction = -1;

            //calculate the arc center
            if (distance == 0) { distance = 0.001; }
            double pow = Math.Pow(2 * ratio / distance, 2) - 1;
            if (pow < 0) { pow = 0; }
            double part = Math.Sqrt(pow);
            if (!girou)
            {
                xc = ((p1x + p2x) / 2) - direction * ((p1y - p2y) / 2) * part;
                yc = ((p1y + p2y) / 2) + direction * ((p1x - p2x) / 2) * part;
            }
            else
            {
                xc = ((p1x + p2x) / 2) + direction * ((p1y - p2y) / 2) * part;
                yc = ((p1y + p2y) / 2) - direction * ((p1x - p2x) / 2) * part;
            }

            if (nodesOnly)
            { GCodeDotOnly((p2x + (double)offset.X), (p2y + (double)offset.Y)); }//, "Arc"); }
            else
                Graphic.AddArc(isg2, (p2x + (double)offset.X), (p2y + (double)offset.Y), (xc - p1x), (yc - p1y));//, cmt);	// Arc(gnr, x, y, i, j, cmt = "", avoidG23 = false)
        }

        /// <summary>
        /// Transform XY coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static System.Windows.Point TranslateXY(double x, double y)
        {
            System.Windows.Point coord = new System.Windows.Point(x, y);
            return TranslateXY(coord);
        }
        private static System.Windows.Point TranslateXY(System.Windows.Point pointStart)
        {
            return pointStart;
        }

        private static void GCodeDotOnlyWithZ(double x, double y, double z, string cmt)
        {
            Graphic.SetGeometry(cmt);
            Graphic.AddDotWithZ(new Point(x, y), z);//, cmt);  
        }

        private static void GCodeDotOnly(DXFPoint tmp)//, string cmt)
        { GCodeDotOnly((double)tmp.X, (double)tmp.Y); }//, cmt); }
        private static void GCodeDotOnly(Point tmp)//, string cmt)
        { GCodeDotOnly((double)tmp.X, (double)tmp.Y); }//, cmt); }
        private static void GCodeDotOnly(double x, double y)//, string cmt)
        {
            DXFStartPath(x, y, 0);//, cmt);
            Graphic.SetGeometry("Point");
            Graphic.AddDot(x, y);//, cmt);
        }

        /// <summary>
        /// Insert G0, Pen down gcode command
        /// </summary>
        private static void DXFStartPath(DXFPoint tmp)//, string cmt = "")
        {
            Point coord = TranslateXY((double)tmp.X, (double)tmp.Y);
            DXFStartTrsanslatedPath(coord, tmp.Z);//, cmt);
        }
        private static void DXFStartPath(double x, double y, double? z)//, string cmt = "")
        {
            Point coord = TranslateXY((double)x, (double)y);
            DXFStartTrsanslatedPath(coord, z);//, cmt);
        }
        private static void DXFStartTrsanslatedPath(Point coord, double? z)//, string cmt)
        {
            if (logPosition) Logger.Trace("DXFStartTrsanslatedPath");
            if (!GcodeMath.IsEqual(coord, lastUsedCoord))
            {
                if (requestStopPath)
                { DXFStopPath(); requestStopPath = false; }   // stop previous path
                if (useZ && (z != null))
                    Graphic.StartPath(coord, z);                  // start next path
                else
                    Graphic.StartPath(coord);                  // start next path
                if (logPosition) Logger.Trace("DXFStartTrsanslatedPath !equal X:{0:0.00} Y:{1:0.00} Z:{2:0.00} useZ:{3}", coord.X, coord.Y, z, useZ);
            }
            else
            {
                if (requestStopPath)
                { requestStopPath = false; }  // skip stop
                else
                {
                    if (useZ && (z != null))
                        Graphic.StartPath(coord, z);                  // start next path
                    else
                        Graphic.StartPath(coord);                  // start next path
                    if (logPosition) Logger.Trace("DXFStartTrsanslatedPath  equal X:{0:0.00} Y:{1:0.00} Z:{2:0.00} useZ:{3}", coord.X, coord.Y, z, useZ);
                }
            }
            lastSetZ = z;
            lastUsedCoord = coord;
        }

        private static bool requestStopPath = false;
        private static void DXFStopPath()
        {
            if (logPosition) Logger.Trace("DXFStopPath");
            Graphic.StopPath();
            dxfColorFill = "none";
        }              // start next path

        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void DXFMoveTo(DXFPoint tmp) //, string cmt)
        {
            DXFMoveTo((double)tmp.X, (double)tmp.Y, tmp.Z); //, cmt);
        }

        private static void DXFMoveTo(double x, double y, double? z)    //, string cmt)
        {
            System.Windows.Point coord = new System.Windows.Point(x, y);
            DXFMoveTo(coord, z); //, cmt);
        }
        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void DXFMoveTo(System.Windows.Point orig, string cmt)
        {
            DXFMoveTo(orig, lastSetZ);   //, cmt);
        }
        private static void DXFMoveTo(System.Windows.Point orig, double? z)//, string cmt)
        {
            System.Windows.Point coord = TranslateXY(orig);
            if (logPosition) Logger.Trace(" DXFMoveTo   X:{0:0.00} Y:{1:0.00} Z:{2:0.00} useZ:{3}", coord.X, coord.Y, z, useZ);
            if (!nodesOnly)
            {
                if (useZ && (z != null))
                    Graphic.AddLine(coord, z);
                else
                    Graphic.AddLine(coord);
            }
            else
                GCodeDotOnly(coord.X, coord.Y);//, "");

            lastUsedCoord = coord;
        }

        private static string GetColorFromID(int id)
        {
            string[] DXFcolors = {"000000","FF0000","FFFF00","00FF00","00FFFF","0000FF","FF00FF","FFFFFF",
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

    public struct SplinePoint2
    {
        public double X;
        public double Y;

        public SplinePoint2(double x, double y)
        {
            X = x;
            Y = y;
        }
        public SplinePoint2(DXFPoint tmp)
        {
            X = (double)tmp.X;
            Y = (double)tmp.Y;
        }


        public static bool operator ==(SplinePoint2 a, SplinePoint2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(SplinePoint2 a, SplinePoint2 b)
        {
            return !(a == b);
        }

        public static SplinePoint2 operator +(SplinePoint2 a, SplinePoint2 b)
        {
            return new SplinePoint2(a.X + b.X, a.Y + b.Y);
        }

        public static SplinePoint2 operator *(SplinePoint2 p, double scalar)
        {
            return new SplinePoint2(p.X * scalar, p.Y * scalar);
        }

        public override bool Equals(object obj)
        {
            return obj is SplinePoint2 point &&
                this == point;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public Point ToPoint()
        { return new Point(X, Y); }
        public DXFPoint ToDXFPoint()
        {
            DXFPoint tmp = new DXFPoint();
            tmp.X = X; tmp.Y = Y;   
            return tmp; 
        }
    }

    public struct Bezier2
    {
        public SplinePoint2 Start;
        public SplinePoint2 Control1;
        public SplinePoint2 Control2;
        public SplinePoint2 End;

        public Bezier2(SplinePoint2 start, SplinePoint2 control1, SplinePoint2 control2, SplinePoint2 end)
        {
            Start = start;
            Control1 = control1;
            Control2 = control2;
            End = end;
        }
    }

    public class Spline2
    {
        private List<SplinePoint2> _controlPoints;
        private readonly List<double> _knotValues;

        public int Degree { get; }
        public IEnumerable<SplinePoint2> ControlPoints => _controlPoints;
        public IEnumerable<double> KnotValues => _knotValues;

        public Spline2(int degree, IEnumerable<SplinePoint2> controlPoints, IEnumerable<double> knotValues)
        {
            if (degree < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(degree), "Degree must be greater than or equal to 2.");
            }

            if (controlPoints == null)
            {
                throw new ArgumentNullException(nameof(controlPoints));
            }

            if (knotValues == null)
            {
                throw new ArgumentNullException(nameof(knotValues));
            }

            Degree = degree;
            _controlPoints = controlPoints.ToList();
            _knotValues = knotValues.ToList();

            ValidateValues();
        }

        private void ValidateValues()
        {
            if (_controlPoints.Count < Degree + 1)
            {
                throw new InvalidOperationException("There must be at least one more control point than the degree of the curve.");
            }

            if (_knotValues.Count < 1)
            {
                throw new InvalidOperationException("Minimum knot value count is 1.");
            }

            if (_knotValues.Count != _controlPoints.Count + Degree + 1)
            {
                throw new InvalidOperationException("Invalid combination of knot value count, control point count, and degree.");
            }

            // knot values must be ascending
            var lastKnotValue = _knotValues[0];
            foreach (var kv in _knotValues.Skip(1))
            {
                if (kv < lastKnotValue)
                {
                    throw new InvalidOperationException($"Knot values must be ascending.  Found values {lastKnotValue} -> {kv}.");
                }

                lastKnotValue = kv;
            }
        }

        public void InsertKnot(double t)
        {
            // find the knot span that contains t
            var knotInsertionIndex = _knotValues.Count(k => k < t);

            // replace points at index [k-p, k]
            // first new point is _controlPoints[index - degree]
            var lowerIndex = knotInsertionIndex - Degree;
            var upperIndex = knotInsertionIndex;
            var pointsToInsert = new List<SplinePoint2>();
            for (int i = lowerIndex; i < upperIndex; i++)
            {
                var a = (t - _knotValues[i]) / (_knotValues[i + Degree] - _knotValues[i]);
                var q = _controlPoints[i - 1] * (1.0 - a) + _controlPoints[i] * a;
                pointsToInsert.Add(q);
            }

            // insert new values
            _knotValues.Insert(knotInsertionIndex, t);
            var newControlPoints = new List<SplinePoint2>();
            newControlPoints.AddRange(_controlPoints.Take(lowerIndex));
            newControlPoints.AddRange(pointsToInsert);
            newControlPoints.AddRange(_controlPoints.Skip(lowerIndex + pointsToInsert.Count - 1));
            _controlPoints = newControlPoints;
            ValidateValues();
        }

        public IList<Bezier2> ToBeziers()
        {
            var expectedIdenticalKnots = Degree + 1;
            if (expectedIdenticalKnots != 4)
            {
                throw new NotSupportedException("Only cubic Bezier curves of 4 points are supported.");
            }

            var result = new Spline2(Degree, ControlPoints, KnotValues);

            for (int offset = 0; ; offset++)
            {
                // get next set of values
                var values = result.KnotValues.Skip(offset * expectedIdenticalKnots).Take(expectedIdenticalKnots).ToList();

                if (values.Count == 0 && result.KnotValues.Count() % expectedIdenticalKnots == 0)
                {
                    // done
                    break;
                }

                var expectedValue = values[0];
                int missingValueCount;
                if (values.Count < expectedIdenticalKnots)
                {
                    // not enough values
                    missingValueCount = expectedIdenticalKnots - values.Count;
                }
                else if (values.Count < expectedIdenticalKnots || values.Any(v => v != expectedValue))
                {
                    // not all the same
                    missingValueCount = expectedIdenticalKnots - values.Count(v => v == expectedValue);
                }
                else
                {
                    missingValueCount = 0;
                }

                for (int i = 0; i < missingValueCount; i++)
                {
                    result.InsertKnot(expectedValue);
                }
            }

            var points = result.ControlPoints.ToList();
            var curves = new List<Bezier2>();
            for (int startIndex = 0; startIndex < points.Count; startIndex += expectedIdenticalKnots)
            {
                var curve = new Bezier2(points[startIndex], points[startIndex + 1], points[startIndex + 2], points[startIndex + 3]);
                curves.Add(curve);
            }

            return curves;
        }
    }
}