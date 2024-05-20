/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2020-07-05 new class to collect graphic primitives
 * 2020-07-24 switch process-order of tangential and drag-knife - do tangential as a last step
			  clipping/tilig: show tiling cuts only for tiling
 * 2020-07-28 bug fix in ReversePath() add depth information to line, clipping background raster
 * 2020-08-10 createGraphicsPath logDetailed
 * 2020-11-10 line 1565 also reverse last item in sortOrderFigure
 * 2020-11-25 add RemoveIntermediateSteps
 * 2020-12-02 bug fix missing tile-commands, because of no grouping in line 476
 * 2020-12-08 add BackgroundWorker updates
 * 2020-12-16 line 440 lock (lockObject) to protect VisuGCode.pathBackground from other access
 * 2021-01-21 addFrame, MultiplyGraphics
 * 2021-03-25 line 375 in SetPenWidth remove leading '#'
 * 2021-04-16 add offset for tiling in function ClipCode line 924 addOnX
 * 2021-06-03 GroupAllGraphics - allow grouping if count == 1 but don't sort then, to get tool by layer #202 (was disabled for #147)
 * 2021-07-14 code clean up / code quality
 * 2021-07-30 check coordinates for NaN in StartPath, AddLine, AddDot, AddCircle, AddArc
 * 2021-08-06 line 467 SetGeometry activate setNewId=true, before: multiple lines within one figure
 * 2021-09-02 add viewOffset for tiles
 * 2021-09-09 add AlignLines
 * 2021-09-21 new GroupOption 'Label' SetLabel
 * 0221-09-30 setgraphicInformation.FigureEnable = false - if to many figures
 * 2021-11-16 bugfix RemoveIntermediateSteps <- include RemoveShortMoves
 * 2022-04-06 add DragToolModificationTangential, add header info about some applied options
 * 2022-04-21 line 773 make limit variable: if (completeGraphic.Count > maxCount)
 * 2022-07-22 line 509 SetPenFill - also convert from "rgb("
 * 2022-12-14 line 612 CountProperty check if txt IsNullOrEmpty
 * 2023-01-02 possibility to overwrite maxObjectCountBeforeReducingXML
 * 2023-04-10 l:524 f:ConvertFromRGB new function to parse also % -> rgb(80.392%, 52.157%, 24.706%)
 * 2023-04-11 l:694 f:CreateGCode && !graphicInformation.OptionClipCode
 * 2023-04-12 l:726 f:CreateGCode    disable VisuGCode.CalcDrawingArea();    to get rid of InvalidOperationException
 * 2023-04-18 l:700 f:CreateGCode add GCode-Header output Graphic offset
 * 2023-04-25 l:745 f:CreateGCode add filtering
 * 2023-05-31 l:454 f:GetActualZ add OptionSFromWidth
 * 2023-07-02 l:296 f:StartPath ->  actualPath = new ItemPath(xy, GetActualZ());  
 * 2023-08-06 l:830 f:CreateGCode set SortByDistance start-pos to maxy
 * 2023-08-16 l:271 f:StartPath f:UpdateGUI pull request Speed up merge and sort #348
 * 2023-09-16 l:774 f:CreateGCode wrong call to RemoveOffset(minx,minx) -> miny
 * 2024-01-25 l:1100 f:CreateGCode export graphics dimension, to be able to calculate point-marker-size in 2Dview
 * 2024-02-04 l:426 f:AddLine add noise to line
 * 2024-03-02 l:547 f:AddCircle / AddArc seperate processing of OptionDashPattern
 * 2024-03-15 l:1325 f:ReDoReversePath add calculate tangential
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace GrblPlotter
{
    public static partial class Graphic
    {
        internal static StringBuilder GCode = new StringBuilder();

        private static List<PathObject> completeGraphic = new List<PathObject>();
        private static List<PathObject> finalPathList = new List<PathObject>();
        private static List<PathObject> tileGraphicAll = new List<PathObject>();
        private static List<GroupObject> groupedGraphic = new List<GroupObject>();
        private static readonly List<TileObject> tiledGraphic = new List<TileObject>();

        private static Dictionary<string, int> keyToIndex = new Dictionary<string, int>();
        private static GroupObject tmpGroup = new GroupObject();

        private static List<String> headerInfo = new List<String>();
        private static List<String> headerMessage = new List<String>();
        private static ItemPath actualPath = new ItemPath();
        private static PathInformation actualPathInfo = new PathInformation();
        private static double[] actualDashArray = new double[0];

        private static readonly Dictionary<string, int>[] groupPropertiesCount = new Dictionary<string, int>[10];

        internal static Dimensions actualDimension = new Dimensions();
        private static Point lastPoint = new Point();       // System.Windows
        private static bool lastPointIsStart = true;
        private static PathObject lastPath = new ItemPath();
        private static CreationOption lastOption = CreationOption.none;

        internal static GraphicsPath pathBackground = new GraphicsPath();             // show complete graphic as background if tiles activated
        private static double equalPrecision = 0.00001;
        private static int objectCount = 0;

        private static bool continuePath = false;
        private static bool setNewId = true;
        private static bool cancelByWorker = false;

        private static Stopwatch stopwatch = new Stopwatch();
        private static Stopwatch totalTime = new Stopwatch();
        private static int countGeometry = 0;
        private const int maxGeometry = 50000;
        public static int maxObjectCountBeforeReducingXML = 1000; // (int)Properties.Settings.Default.importFigureMaxAmount;

        private static int countAuxInfo = 0;

        private static int countWarnDimNotSet = 0;

        //    private static bool noiseAdd = false;
        private static double noiseAmplitude = 1;
        private static double noiseDensity = 1;

        private static int pathAddOnCount = 0;
        //private static int pathAddOnPosition = 0;
        //private static double pathAddOnScale = 1;
        private static double pathAddOnDimx = 0;
        private static double pathAddOnDimy = 0;
        private static bool pathAddOnCompletion = false;
        //private static string addFramelayer = "";

        internal static GraphicInformationClass graphicInformation = new GraphicInformationClass();

        private static BackgroundWorker backgroundWorker = null;  // will be set by GCodeFromxxx
        private static DoWorkEventArgs backgroundEvent = null;        //

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logDetailed = false;
        private static bool logSortMerge = false;
        private static bool logCoordinates = false;
        private static bool logProperties = false;
        private static bool logModification = false;

        public static void CleanUp()
        {
            Logger.Trace("----  CleanUp()  before WorkingSet: {0} kb   Total Memory: {1:N0} kb", System.Environment.WorkingSet / 1024, GC.GetTotalMemory(false) / 1024);
            Graphic2GCode.CleanUp();
            ClearList(completeGraphic); // = null;
            ClearList(finalPathList);   // = null;
            ClearList(tileGraphicAll);  // = null;
            ClearList(groupedGraphic);  // = null;
            headerInfo.Clear();         // = null;
            headerMessage.Clear();      // = null;
            //backgroundWorker = null; // will be set by GCodeFromxxx
            //backgroundEvent = null;
            GCode.Clear();  // = null;
            GC.Collect();
            Logger.Trace("----  CleanUp()  after  WorkingSet: {0} kb   Total Memory: {1:N0} kb", System.Environment.WorkingSet / 1024, GC.GetTotalMemory(true) / 1024);
        }
        private static void ClearList(List<PathObject> lista)
        {
            lista.Clear();
            int identificador = GC.GetGeneration(lista);
            GC.Collect(identificador, GCCollectionMode.Forced);
        }
        private static void ClearList(List<GroupObject> lista)
        {
            lista.Clear();
            int identificador = GC.GetGeneration(lista);
            GC.Collect(identificador, GCCollectionMode.Forced);
        }

        #region notifications
        public static bool SizeOk()
        { return countGeometry <= maxGeometry; }

        public static int GetObjectCount()
        { return countGeometry; }


        //private static bool updateMarker = false;
        private static int lastUpdateMilliseconds = 0;
        private static bool UpdateGUI()
        {
            //bool time = ((stopwatch.Elapsed.Milliseconds % 500) > 250);
            //if (time)
            int elapsed = stopwatch.Elapsed.Milliseconds - lastUpdateMilliseconds;
            if ((elapsed < 0) || (elapsed > 500))
            {
                /*if (updateMarker)     // 2023-08-16 pull request Speed up merge and sort #348 Reduce CPU used updating progress bars
                {
                    updateMarker = false;
                    return true;
                }*/
                lastUpdateMilliseconds = stopwatch.Elapsed.Milliseconds;
                return true;
            }
            //else
            //{ updateMarker = true; }
            return false;
        }
        #endregion


        #region collect data
        public static void SetBackgroundWorker(BackgroundWorker worker, DoWorkEventArgs e)
        {
            backgroundWorker = worker;
            backgroundEvent = e;
            Logger.Trace("SetBackgroundWorker");
        }

        public static void Init(SourceType type, string filePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level2) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnables.Detailed) > 0);
            logSortMerge = logEnable && ((logFlags & (uint)LogEnables.Sort) > 0);
            logCoordinates = logEnable && ((logFlags & (uint)LogEnables.Coordinates) > 0);
            logProperties = logEnable && ((logFlags & (uint)LogEnables.Properties) > 0);
            logModification = logEnable && ((logFlags & (uint)LogEnables.PathModification) > 0);

            VisuGCode.logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level4) > 0);
            VisuGCode.logDetailed = VisuGCode.logEnable && ((logFlags & (uint)LogEnables.Detailed) > 0);
            VisuGCode.logCoordinates = VisuGCode.logEnable && ((logFlags & (uint)LogEnables.Coordinates) > 0);

            backgroundWorker = worker;
            backgroundEvent = e;
            cancelByWorker = false;
            GCode = new StringBuilder("(Import failed)");

            Logger.Trace("●●● Graphic - Init Graphic {0}  loggerTrace:{1}", type.ToString(), Convert.ToString(logFlags, 2));

            graphicInformation = new GraphicInformationClass	// get all setups and correct e.g. ConvertArcToLine
            {
                Title = type.ToString() + " import",    // fill up structure
                SourceType = type,
                FilePath = filePath
            };          // get Default settings
            graphicInformation.DxfImportZ = (graphicInformation.SourceType == SourceType.DXF) && Properties.Settings.Default.importDXFUseZ;

            if (type == SourceType.SVG)
            { graphicInformation.ApplyHatchFill = graphicInformation.ApplyHatchFill || Properties.Settings.Default.importSVGApplyFill; }    // no G2/G3 if hatch fill

            maxObjectCountBeforeReducingXML = (int)Properties.Settings.Default.importFigureMaxAmount;

            pathBackground = new GraphicsPath();

            //noiseAdd = Properties.Settings.Default.importGraphicNoiseEnable;
            noiseAmplitude = (double)Properties.Settings.Default.importGraphicNoiseAmplitude;
            noiseDensity = (double)Properties.Settings.Default.importGraphicNoiseDensity;

            completeGraphic = new List<PathObject>();
            finalPathList = new List<PathObject>();
            tileGraphicAll = new List<PathObject>();
            groupedGraphic = new List<GroupObject>();
            actualPath = new ItemPath();

            headerInfo = new List<String>();
            headerMessage = new List<String>();
            actualPathInfo = new PathInformation();
            actualDashArray = new double[0];
            actualDimension = new Dimensions();
            lastPoint = new Point();
            lastPath = new ItemPath();
            lastOption = CreationOption.none;
            lastPointIsStart = true;

            for (int i = 0; i < groupPropertiesCount.Length; i++)
                groupPropertiesCount[i] = new Dictionary<string, int>();

            objectCount = 0;
            countGeometry = 0;
            countAuxInfo = 0;
            continuePath = false;
            setNewId = true;
            equalPrecision = (double)Properties.Settings.Default.importAssumeAsEqualDistance;

            pathAddOnCompletion = false;
            pathAddOnCount = 0;
            //pathAddOnScale = 1;
            pathAddOnDimx = 0;
            pathAddOnDimy = 0;
            //addFramelayer = "";

            stopwatch = new Stopwatch();
            stopwatch.Start();
            totalTime = new Stopwatch();
            totalTime.Start();

            countWarnDimNotSet = 0;
        }

        public static bool StartPath(float x, float y)
        { return StartPath(new Point(x, y)); }

        public static bool StartPath(Point xy, double? useZ = null)//, CreationOptions addFunction = CreationOptions.none)
        {   // preset informations
            bool success = true;
            if (Double.IsNaN(xy.X) || Double.IsNaN(xy.Y))
            {
                Logger.Error("StartPath NaN use last valid point - lastX:{0} lastY:{1} ", lastPoint.X, lastPoint.Y);
                xy = lastPoint;
                success = false;
            }

            if (completeGraphic.Any())
            { lastPath = completeGraphic[completeGraphic.Count - 1]; }		// continoue last stored path
            else
            { lastPath = actualPath; }                                      // continoue actualPath

            actualPath.Info.CopyData(actualPathInfo);                       // preset global info for GROUP

            // only continue last path if same layer, color, dash-pattern - if enabled
            //if ((lastPath is ItemPath apath) && (objectCount > 0) && HasSameProperties(apath, (ItemPath)actualPath) && (IsEqual(xy, lastPoint)))	// 2023-08-16 pull request Speed up merge and sort #348
            if ((lastPath is ItemPath apath) && (objectCount > 0) && HasSameProperties(apath, (ItemPath)actualPath, Properties.Settings.Default.importLineDashPattern) && (IsEqual(xy, lastPoint)))
            {
                actualPath = apath;             // only continoue last path if it was finished
                actualPath.Options = lastOption;
                if (logCoordinates) Logger.Trace("► StartPath-ADD (same properties and pos) Id:{0} at X:{1:0.00} Y:{2:0.00} {3}  start.X:{4:0.00} start.Y:{5:0.00}", objectCount, xy.X, xy.Y, actualPath.Info.List(), actualPath.Start.X, actualPath.Start.Y);
                continuePath = true;
            }
            else
            {
                if (logCoordinates) Logger.Trace("► StartPath-NEW (diff properties or pos) Id:{0}  xy X:{1:0.00} Y:{2:0.00}   lastPoint X:{3:0.00} Y:{4:0.00}   setNewId:{5}", objectCount, xy.X, xy.Y, lastPoint.X, lastPoint.Y, setNewId);

                if (actualPath.Path.Count > 1)
                    StopPath("in StartPath");   // save previous path, before starting an new one

                if (setNewId)
                    objectCount++;

                if (useZ != null)
                    actualPath = new ItemPath(xy, (double)useZ);
                else
                {
                    if (graphicInformation.OptionZFromWidth || graphicInformation.OptionSFromWidth)
                    { actualPath = new ItemPath(xy, GetActualZ()); }
                    else
                    { actualPath = new ItemPath(xy); }
                }
                actualPathInfo.Id = objectCount;
                actualPath.Info.CopyData(actualPathInfo);       // preset global info for GROUP
                actualPath.Options = lastOption;
                if (actualDashArray.Length > 0)
                {
                    actualPath.DashArray = new double[actualDashArray.Length];
                    actualDashArray.CopyTo(actualPath.DashArray, 0);
                }

                if (logCoordinates) Logger.Trace("► StartPath-NEW Id:{0} at X:{1:0.00} Y:{2:0.00} Z:{3:0.00} {4}  start.X:{5:0.00} start.Y:{6:0.00}", objectCount, xy.X, xy.Y, useZ, actualPath.Info.List(), actualPath.Start.X, actualPath.Start.Y);
            }

            actualPath.Dimension.SetDimensionXY(xy.X, xy.Y);
            lastPoint = xy;
            setNewId = false;
            lastOption = CreationOption.none;
            lastPointIsStart = true;

            return success;
        }
        public static void StopPath()
        { StopPath(""); }

        public static void StopPath(string cmt)
        {
            if (!actualPath.Dimension.IsXYSet())                    // 2020-10-31
            {
                if (logEnable && (countWarnDimNotSet++ < 10)) { Logger.Trace("⚠ StopPath dimension not set - skip1"); }
                return;
            }

            if (continuePath && (completeGraphic.Count > 0))
            { completeGraphic[completeGraphic.Count - 1] = actualPath; }
            else
            {
                if (actualPath.Dimension.IsXYSet())
                {
                    if (graphicInformation.OptionNoise) 
                        actualPath.Add(lastPoint, GetActualZ(), 0);

                    completeGraphic.Add(actualPath);
                    if (logCoordinates) { Logger.Trace("▲ StopPath completeGraphic.Add {0}", completeGraphic.Count); }
                }
                else
                { if (logEnable && (countWarnDimNotSet++ < 10)) Logger.Trace("⚠ StopPath dimension not set - skip2"); }
            }
            if (logCoordinates) { Logger.Trace("▲ StopPath  cnt:{0}  cmt:{1} {2}", (objectCount - 1), cmt, actualPath.Info.List()); }
            if (logCoordinates) { Logger.Trace("▲   StopPath Dimension  {0}  cmt:{1} {2}", actualPath.Dimension.GetXYString(), cmt, actualPath.Info.List()); }

            if (actualPath.Path.Count > 0)
            { actualDimension.AddDimensionXY(actualPath.Dimension); }

            continuePath = false;
            actualPath = new ItemPath();                    // reset path
            actualPathInfo.Id = objectCount;
            actualPath.Info.CopyData(actualPathInfo);       // preset global info for GROUP
            actualPath.Options = lastOption;
            actualPath.Dimension.ResetDimension();          // 2020-10-31
        }

        public static bool AddLine(double x, double y)
        { return AddLine(new Point(x, y)); }
        public static bool AddLine(Point xy, double? useZ = null)//, string cmt = "")
        {
            bool success = true;
            double z = GetActualZ();                        // apply penWidth if enabled
            if (useZ != null)
                z = (double)useZ;

            if (Double.IsNaN(xy.X) || Double.IsNaN(xy.Y))
            {
                Logger.Error("AddLine NaN use last valid point - lastX:{0} lastY:{1} ", lastPoint.X, lastPoint.Y);
                xy = lastPoint;
                success = false;
            }

            if (IsEqual(lastPoint, xy))
            { if (logCoordinates) Logger.Trace("⚠ AddLine SKIP, same coordinates! X:{0:0.00} Y:{1:0.00}", xy.X, xy.Y); }
            else
            {
                if (graphicInformation.OptionNoise)
                {
                    AddNoiseToPath(xy, z, actualPath.Path.Count, noiseAmplitude, noiseDensity);
                }
                else
                {
                    actualPath.Add(xy, z, 0);
                }
                if (logCoordinates) Logger.Trace("● AddLine to X:{0:0.00} Y:{1:0.00}  Z:{2:0.00} new dist {3:0.00}   start.X:{4:0.00}  start.Y:{5:0.00}", xy.X, xy.Y, z, actualPath.PathLength, actualPath.Start.X, actualPath.Start.Y);
                actualDimension.SetDimensionXY(xy.X, xy.Y);
                lastPoint = xy;
            }
            lastPointIsStart = false;
            return success;
        }
        private static Point AddNoiseToPath(Point end, double z, int index, double amplitude, double density)
        {
            double x = lastPoint.X;
            double y = lastPoint.Y;
            double dx = end.X - lastPoint.X;
            double dy = end.Y - lastPoint.Y;
            double lineLength = Math.Sqrt(dx * dx + dy * dy);
            double stepWidth = density;
            int step = (int)Math.Ceiling(lineLength / stepWidth);

            if (step == 0)
            {
                actualPath.Add(end, z, 0);
                return end;
            }
            double dix = dx / step;
            double diy = dy / step;

            double fx, fy;
            if (dx == 0)
            { fx = 1; fy = 0; }
            else if (dy == 0)
            { fx = 0; fy = 1; }
            else
            {
                fx = dy / lineLength; fy = dx / lineLength;
            }
            fx *= (amplitude / 2); ;
            fy *= (-amplitude / 2); ;

            float scale, n, nx = 0, ny = 0;
            scale = 1;// (float)stepWidth / 2000;

            //Logger.Trace("AddNoiseToPath  step:{0}",step);

            if (step <= 1)
            {
                n = Noise.CalcPixel2D(index, 1, scale);
                nx = (float)fx * n;
                ny = (float)fy * n;
                actualPath.Add(new Point(x + nx, y + ny), z, 0);
                //actualPath.Add(end, z, 0);
            }
            else
            {
                for (int i = 1; i < step; i++)
                {
                    n = Noise.CalcPixel2D(index, i, scale);
                    nx = (float)fx * n;
                    ny = (float)fy * n;
                    x += dix;
                    y += diy;
                    actualPath.Add(new Point(x + nx, y + ny), z, 0);
                }
                actualPath.Add(end, z, 0);
            }
            return new Point(x + nx, y + ny);
        }

        public static bool AddDot(Point xy)//, string cmt = "")
        { return AddDot(xy.X, xy.Y); }//, cmt); 	
        public static bool AddDot(double dx, double dy)//, string cmt = "")
        {
            bool success = true;
            if (Double.IsNaN(dx) || Double.IsNaN(dy))
            { Logger.Error("⚠ AddDot NaN skip the dot X:{0:0.00} Y:{1:0.00}", dx, dy); success = false; }
            else
            {
                ItemDot dot = new ItemDot(dx, dy);
                dot.Info.CopyData(actualPathInfo);
                dot.OptZ = GetActualZ();                           // apply penWidth if enabled

                completeGraphic.Add(dot);
                lastPoint = new Point(dx, dy);
                actualDimension.SetDimensionXY(dx, dy);
                if (logCoordinates) Logger.Trace("● AddDot at X:{0:0.00} Y:{1:0.00}", dx, dy);
                actualPath = new ItemPath();                    // reset path
            }
            return success;
        }
        public static bool AddDotWithZ(Point xy, double dz)//, string cmt)
        { return AddDotWithZ(xy.X, xy.Y, dz); }//, cmt); 
        public static bool AddDotWithZ(double dx, double dy, double dZ)//, string cmt = "")
        {
            bool success = true;
            if (Double.IsNaN(dx) || Double.IsNaN(dy) || Double.IsNaN(dZ))
            { Logger.Error("AddDotWithZ NaN skip the dot X:{0:0.00} Y:{1:0.00} Z:{2:0.00} ", dx, dy, dZ); success = false; }
            else
            {
                ItemDot dot = new ItemDot(dx, dy, dZ);
                dot.Info.CopyData(actualPathInfo);
                completeGraphic.Add(dot);
                lastPoint = new Point(dx, dy);
                actualDimension.SetDimensionXY(dx, dy);
                if (logCoordinates) Logger.Trace("  AddDot at X:{0:0.00} Y:{1:0.00} Z:{2:0.00} ", dx, dy, dZ);
                actualPath = new ItemPath();                    // reset path
                graphicInformation.SetDotZ(dZ);
            }
            return success;
        }

        public static bool AddCircle(double centerX, double centerY, double radius)
        {
            bool success = true;
            if (Double.IsNaN(centerX) || Double.IsNaN(centerY) || Double.IsNaN(radius))
            { Logger.Error("AddCircle NaN skip the circle X:{0:0.00} Y:{1:0.00} r:{2:0.00} ", centerX, centerY, radius); success = false; }
            else
            {
                bool arcToLine = graphicInformation.ConvertArcToLine || (graphicInformation.OptionDashPattern && (actualDashArray.Length > 1));
                actualPath.AddArc(new Point(centerX + radius, centerY), new Point(-radius, 0), GetActualZ(), true, arcToLine, graphicInformation.OptionNoise);// convertArcToLine);
                actualPath.Info.CopyData(actualPathInfo);    // preset global info for GROUP
                if (logCoordinates) Logger.Trace("  AddCircle to X:{0:0.00} Y:{1:0.00} r:{2:0.00}  angleStep:{3}", centerX, centerY, radius, Properties.Settings.Default.importGCSegment);
            }
            return success;
        }

        public static bool AddArc(bool isG2, Point xy, Point ij)//, string cmt)
        { return AddArc(isG2, xy.X, xy.Y, ij.X, ij.Y); }
        /*   public static void AddArc(int gnr, double x, double y, double i, double j, string cmt = "")
           { AddArc((gnr == 2), x, y, i, j, cmt); }*/
        public static bool AddArc(bool isg2, double ax, double ay, double ai, double aj)//, string cmt)
        {
            bool success = true;
            if (Double.IsNaN(ax) || Double.IsNaN(ay) || Double.IsNaN(ai) || Double.IsNaN(aj))
            { Logger.Error("AddArc NaN skip the circle X:{0:0.00} Y:{1:0.00} i:{2:0.00} j:{3:0.00} ", ax, ay, ai, aj); success = false; }
            else
            {
                lastPoint = new Point(ax, ay);
                bool arcToLine = graphicInformation.ConvertArcToLine || (graphicInformation.OptionDashPattern && (actualDashArray.Length > 1));
                actualPath.AddArc(new Point(ax, ay), new Point(ai, aj), GetActualZ(), isg2, arcToLine, graphicInformation.OptionNoise);
                actualPath.Info.CopyData(actualPathInfo);    // preset global info for GROUP
                if (logCoordinates) Logger.Trace("  AddArc to X:{0:0.00} Y:{1:0.00} i:{2:0.00} j:{3:0.00}  angleStep:{4}  isG2:{5}", ax, ay, ai, aj, Properties.Settings.Default.importGCSegment, isg2);
            }
            return success;
        }

        public static double GetActualZ()
        {
            double z = 0;
            if (graphicInformation.OptionZFromWidth || graphicInformation.OptionSFromWidth)
                if (!double.TryParse(actualPathInfo.GroupAttributes[(int)GroupOption.ByWidth], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out z))
                { }
            return z;
        }

        // set marker, to take-over the flag on next "StartPath"
        public static void OptionInsertPause()
        { lastOption |= CreationOption.AddPause; }

        public static void SetType(string txt)
        {
            if (logProperties) Logger.Trace("Set Type '{0}'", txt);
            setNewId = true;
            int tmpIndex = (int)GroupOption.ByType;
            CountProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
                Logger.Error(" Error SetType '{0}'", txt);
        }

        public static void SetLayer(string txt)
        {
            if (logProperties) Logger.Trace("Set Layer '{0}'", txt);
            setNewId = true;
            int tmpIndex = (int)GroupOption.ByLayer;
            CountProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
                Logger.Error(" Error SetLayer '{0}'", txt);
        }
        public static void SetLabel(string txt)	// from inkscape:label, will be added to layer-name
        {
            if (logProperties) Logger.Trace("Set Label '{0}'", txt);
            //         setNewId = true;
            int tmpIndex = (int)GroupOption.Label;
            //        CountProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
                Logger.Error(" Error SetLabel '{0}'", txt);
        }

        public static bool SetPenWidth(string txt)	// DXF: 0 - 2.11mm = 0 - 211		SVG: 0.000 - ?  Convert with to mm, then to string in importClass
        {
            if (logProperties)  
                Logger.Trace("SetPenWidth '{0}'  called by:{1}", txt, (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            if (txt.Contains("NaN"))
            {
                Logger.Error("SetPenWidth contains NaN '{0}'", txt);
                return true;
            }
            int tmpIndex = (int)GroupOption.ByWidth;
            CountProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
                Logger.Error(" Error SetPenWidth '{0}'", txt);

            graphicInformation.SetPenWidth(txt);        // find min / max values

            if (!Properties.Settings.Default.importGraphicFilterEnable)
                return true;                                                                // no filter
            if (Properties.Settings.Default.importGraphicFilterChoiceRemove)                // check remove list
            {
                if (Properties.Settings.Default.importGraphicFilterListRemove.Contains(txt))
                    return false;                                                           // value is in remove list
                else
                    return true;
            }
            else
            {
                if (Properties.Settings.Default.importGraphicFilterListKeep.Contains(txt))
                    return true;                                                            // value is in keep list
                else
                    return false;
            }
            return true;
        }

        public static bool SetPenColor(string txt)
        {
            if (string.IsNullOrEmpty(txt)) return true;
            if (txt.StartsWith("rgb("))
            {
                txt = ConvertFromRGB(txt);
            }
            if (logProperties) Logger.Trace("SetPenColor '{0}'", txt);
            if (txt.ToLower().Contains("currentcolor"))
            { return true; }

            setNewId = true;
            int tmpIndex = (int)GroupOption.ByColor;
            CountProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
                Logger.Error(" Error SetPenColor '{0}'", txt);

            if (!Properties.Settings.Default.importGraphicFilterEnable)
                return true;
            if (Properties.Settings.Default.importGraphicFilterChoiceRemove)
            {
                if (Properties.Settings.Default.importGraphicFilterListRemove.Contains(txt))
                    return false;
                else
                    return true;
            }
            else
            {
                if (Properties.Settings.Default.importGraphicFilterListKeep.Contains(txt))
                    return true;
                else
                    return false;
            }
            return true;
        }

        public static void SetPenFill(string txt)
        {
            if (string.IsNullOrEmpty(txt)) return;
            if (txt.StartsWith("rgb("))
            {
                txt = ConvertFromRGB(txt);
            }
            if (logProperties) Logger.Trace("SetPenFill '{0}'", txt);

            int tmpIndex = (int)GroupOption.ByFill;
            if (txt.ToLower().Contains("currentcolor"))
            {
                string tmp = actualPathInfo.GetGroupAttribute((int)GroupOption.ByColor);
                if (!actualPathInfo.SetGroupAttribute(tmpIndex, tmp))
                    Logger.Error(" Error SetPenFill '{0}'", txt);
                return;
            }

            setNewId = true;
            CountProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
                Logger.Error(" Error SetPenFill '{0}'", txt);
        }

        private static string ConvertFromRGB(string txt)
        {
            bool isPercent = txt.Contains("%");
            string txt1 = txt.Substring(4);
            String[] colors = txt1.Split(',');

            string num1 = colors[0].Replace("%", "").Trim(), num2 = colors[1].Replace("%", "").Trim(), num3 = colors[2].Replace("%", "").Replace(")", "").Trim();
            string ntxt = "";
            if (double.TryParse(num1, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double col1) &&
                double.TryParse(num2, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double col2) &&
                double.TryParse(num3, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double col3))
            {
                //    Logger.Info("ConvertFromRGB1 {0} {1} {2}", col1, col2, col3);

                if (isPercent)
                { col1 *= 2.55; col2 *= 2.55; col3 *= 2.55; }
                col1 = Math.Round(col1);
                col2 = Math.Round(col2);
                col3 = Math.Round(col3);
                //    Logger.Info("ConvertFromRGB2 {0} {1} {2}", col1, col2, col3);

                if (col1 > 255) col1 = 255;
                if (col2 > 255) col2 = 255;
                if (col3 > 255) col3 = 255;
                System.Drawing.Color color = System.Drawing.Color.FromArgb((int)col1, (int)col2, (int)col3);
                ntxt = System.Drawing.ColorTranslator.ToHtml(color);
            }
            //    Logger.Info("ConvertFromRGB convert '{0}' to '{1}'", txt, ntxt);
            txt = ntxt;
            if (txt.StartsWith("#"))
                txt = txt.Substring(1);

            return txt;
        }

        public static void SetPenColorId(int id)
        {
            if (logProperties) Logger.Trace("Set PenColorId '{0}'", id);
            actualPathInfo.PenColorId = id;
            setNewId = true;
        }

        public static void SetPathId(string txt)
        {
            if (logProperties) Logger.Trace("Set PathId '{0}'", txt);
            actualPathInfo.PathId = txt;
            setNewId = true;
        }

        public static void SetGeometry(string txt)
        {
            if (logProperties) Logger.Trace("Set Geometry '{0}'", txt);
            actualPathInfo.PathGeometry = txt;
            countGeometry++;
            setNewId = true;
        }
        public static void SetAuxInfo(int id)
        {
            if (logProperties) Logger.Trace("Set AuxInfo '{0}'", id);
            actualPathInfo.AuxInfo = id;
            countAuxInfo = Math.Max(countAuxInfo, id);
            setNewId = true;
        }

        public static void SetComment(string txt)
        {
            if (logProperties) Logger.Trace("Set Comment '{0}'", txt);
            //            actualPathInfo.pathComment = txt;
        }

        public static void SetHeaderInfo(string txt)
        {
            if (logProperties) Logger.Trace("Set HeaderInfo '{0}'", txt);
            headerInfo.Add(txt);
        }
        public static void SetHeaderMessage(string txt)
        {
            // 1st-digit: 1-Import, 2-CollectData, 3-Gernerate GCode
            // 2nd-digit: 1-SVG, 2-DXF, 3-HPGL, 
            if (logProperties) Logger.Trace("Set HeaderMessage '{0}'", txt);
            if (headerMessage == null)
                return;
            if (headerMessage.Count > 0)
            {
                for (int i = 0; i < headerMessage.Count; i++)   // don't add if already existing
                { if (headerMessage[i] == txt) return; }
            }
            headerMessage.Add(txt);
        }

        public static void SetDash(double[] tmp)
        {
            if (logProperties) Logger.Trace("Set Dash '{0}'", String.Join(", ", tmp.Select(p => p.ToString()).ToArray()));
            if (tmp == null) return;
            if (tmp.Length > 0)
            {
                actualDashArray = new double[tmp.Length];
                tmp.CopyTo(actualDashArray, 0);
            }
            else
                actualDashArray = new double[0];

            setNewId = true;
            if (logProperties)
            {
                string dash = "";
                foreach (double d in actualDashArray)
                    dash += d.ToString() + ", ";
                if (logEnable) Logger.Trace("Set Dash in ID:{0} {1}", actualPath.Info.Id, dash);
            }
        }

        private static void CountProperty(int index, string txt)
        {
            //    if (String.IsNullOrEmpty(txt))
            if (txt == null)
                return;
            if (index < groupPropertiesCount.Length)
                if (!groupPropertiesCount[index].ContainsKey(txt))
                    groupPropertiesCount[index].Add(txt, 1);
        }

        #endregion

        // #######################################################################
        // Do modifications, then create GCode in graphic2GCode
        // #######################################################################
        public static bool ImportCompletion(string tag, double scale, int position) // do thinks before adding further paths
        {
            //addFramelayer = tag;

            if (actualPath.Path.Count > 0)
                StopPath("in CreateCode");  // save previous path

            /* add frame */
            if (Properties.Settings.Default.importGraphicAddFrameEnable)
            {
                Logger.Info("{0} Add frame, distance: {1} radius: {2}", "►►►", Properties.Settings.Default.importGraphicAddFrameDistance, Properties.Settings.Default.importGraphicAddFrameApplyRadius);
                AddFrame(actualDimension,
                (double)Properties.Settings.Default.importGraphicAddFrameDistance,
                Properties.Settings.Default.importGraphicAddFrameApplyRadius);
                SetHeaderInfo(string.Format(" Option: Add frame distance:{0:0.00}  with radius:{1}", Properties.Settings.Default.importGraphicAddFrameDistance, Properties.Settings.Default.importGraphicAddFrameApplyRadius));
            } // distance from graphics dimension, corner radius

            pathAddOnDimx = actualDimension.dimx;
            pathAddOnDimy = actualDimension.dimy;

            if (position == 1)  // center
            {
                Scale(completeGraphic, scale, scale);
                RemoveOffset(completeGraphic, actualDimension.minx + actualDimension.dimx / 2, actualDimension.miny + actualDimension.dimy / 2);    // center object
            }
            else if (position > 0)
            {
                RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny);    // origin = bottom left
            }
            actualDimension.ResetDimension();
            pathAddOnCount = completeGraphic.Count;
            //pathAddOnScale = scale;
            //    pathAddOnPosition = position;
            pathAddOnCompletion = true;
            return true;
        }

        public static bool CreateGCode()//Final(BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            if (logEnable && (countWarnDimNotSet > 0)) { Logger.Trace("⚠ StopPath dimension not set - skip1 further count: {0}", countWarnDimNotSet); }

            if (completeGraphic.Count == 0) //actualPath.Path.Count == 0)
            {
                Logger.Warn("◆◆◆◆  Graphic - CreateGCode - path is empty");
                return Graphic2GCode.CreateGCode(completeGraphic, headerInfo, headerMessage, graphicInformation); // Graphic.Gcode will be filled, return true
            }

            if (actualPath.Path.Count > 1)
                StopPath("in CreateCode");  // save previous path

            Logger.Info("▼▼▼▼  Graphic - CreateGCode count:{0}  dimX:{1:0.0}  dimY:{2:0.0}", completeGraphic.Count, actualDimension.dimx, actualDimension.dimy);

            if (Properties.Settings.Default.importGCRelative)
            { SetHeaderMessage(string.Format(" {0}-2010: GCode for relative movement commands G91 will be generated", CodeMessage.Warning)); }

            int maxOpt = GetOptionsAmount();
            int actOpt = 0;
            string loggerTag = "►►►";

            /* remove short moves*/
            if (!cancelByWorker && Properties.Settings.Default.importRemoveShortMovesEnable)
            {
                if (!(graphicInformation.DxfImportZ))// || Properties.Settings.Default.importDepthFromWidth || Properties.Settings.Default.importPWMFromWidth))
                {
                    Logger.Info("{0} Remove short moves below: {1}", loggerTag, Properties.Settings.Default.importRemoveShortMovesLimit);
                    backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Remove short moves" });
                    RemoveIntermediateSteps(completeGraphic);
                    RemoveShortMoves(completeGraphic, (double)Properties.Settings.Default.importRemoveShortMovesLimit);
                }
                else
                    Logger.Info("{0} NO Remove of short moves", loggerTag, Properties.Settings.Default.importRemoveShortMovesLimit);
            }

            /* process add-on data - frame, sign, watermark */
            if (pathAddOnCompletion && Properties.Settings.Default.importSVGAddOnEnable)
            {
                int position = Properties.Settings.Default.importSVGAddOnPosition;
                double scale = (double)Properties.Settings.Default.importSVGAddOnScale;
                if (position > 1)    // scale
                {
                    Scale(completeGraphic, scale, scale, pathAddOnCount);
                }

                if (position == 1)    // center
                {
                    RemoveOffset(completeGraphic, actualDimension.minx + actualDimension.dimx / 2, actualDimension.miny + actualDimension.dimy / 2, pathAddOnCount);    // center object
                }
                else if (position == 2) // bottom left
                {
                    RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny, pathAddOnCount);
                }
                else if (position == 3) // bottom right
                {
                    RemoveOffset(completeGraphic, actualDimension.minx - pathAddOnDimx + actualDimension.dimx, actualDimension.miny, pathAddOnCount);
                }
                else if (position == 4) // top left
                {
                    RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny - pathAddOnDimy + actualDimension.dimy, pathAddOnCount);
                }
                else if (position == 5) // top right
                {
                    RemoveOffset(completeGraphic, actualDimension.minx - pathAddOnDimx + actualDimension.dimx, actualDimension.miny - pathAddOnDimy + actualDimension.dimy, pathAddOnCount);
                }
                Logger.Info("CreateGCode process add-on data   min x:{0}  y:{1}  dimy:{2}  dimy:{3}  addx:{4}    addy:{5}", actualDimension.minx, actualDimension.miny, actualDimension.dimx, actualDimension.dimy, pathAddOnDimx, pathAddOnDimy);

                if (position > 1)    // reset size
                {
                    actualDimension.minx = actualDimension.miny = 0;
                    actualDimension.dimx = pathAddOnDimx;
                    actualDimension.dimy = pathAddOnDimy;
                }
                Logger.Info("CreateGCode process add-on data   min x:{0}  y:{1}  dimy:{2}  dimy:{3}  ", actualDimension.minx, actualDimension.miny, actualDimension.dimx, actualDimension.dimy);
            }

            /* add frame */
            if (!pathAddOnCompletion && Properties.Settings.Default.importGraphicAddFrameEnable)
            {
                Logger.Info("{0} Add frame, distance: {1} radius: {2}", loggerTag, Properties.Settings.Default.importGraphicAddFrameDistance, Properties.Settings.Default.importGraphicAddFrameApplyRadius);
                AddFrame(actualDimension,
                (double)Properties.Settings.Default.importGraphicAddFrameDistance,
                Properties.Settings.Default.importGraphicAddFrameApplyRadius);
                SetHeaderInfo(string.Format(" Option: Add frame distance:{0:0.00}  with radius:{1}", Properties.Settings.Default.importGraphicAddFrameDistance, Properties.Settings.Default.importGraphicAddFrameApplyRadius));
            } // distance from graphics dimension, corner radius


            /* remove offset */
            if (!cancelByWorker && graphicInformation.OptionCodeOffset && !graphicInformation.OptionClipCode)  // || (Properties.Settings.Default.importGraphicTile) 
            {
                double offX = GuiVariables.offsetOriginX;   // (double)Properties.Settings.Default.importGraphicOffsetOriginX;
                double offY = GuiVariables.offsetOriginY;   // (double)Properties.Settings.Default.importGraphicOffsetOriginY;
                double gap = (double)Properties.Settings.Default.multipleLoadGap;
                double dimX = actualDimension.dimx;
                double dimY = actualDimension.dimy;

                /*********** Adapt offset on mutlifile import ****************************/
                if (Graphic2GCode.multiImport && !Properties.Settings.Default.multipleLoadLimitNo)
                {
                    double limitX = (double)(Properties.Settings.Default.machineLimitsRangeX + Properties.Settings.Default.machineLimitsHomeX);
                    double limitY = (double)(Properties.Settings.Default.machineLimitsRangeY + Properties.Settings.Default.machineLimitsHomeY);
                    Logger.Info("{0} Remove offset: X:{1:0.0}  Y:{2:0.0}  dimX:{3:0.0}  dimY:{4:0.0} maxX:{5:0.0} maxY:{6:0.0}", loggerTag, offX, offY, dimX, dimY, limitX, limitY);
                    if (Properties.Settings.Default.multipleLoadByX)
                    {
                        if ((offX + dimX) > limitX)
                        {
                            offX = (double)Graphic2GCode.multiImportOffsetX;
                            GuiVariables.offsetOriginX = (double)Graphic2GCode.multiImportOffsetX + (dimX + gap);
                            offY = Graphic2GCode.multiImportMaxY + gap;
                            GuiVariables.offsetOriginY = offY;
                        }
                        else
                        {   //offX += dimX + gap;
                            GuiVariables.offsetOriginX = offX + dimX + gap;
                        }
                    }
                    else
                    {
                        if ((offY + dimY) > limitY)
                        {
                            offY = (double)Graphic2GCode.multiImportOffsetY;
                            GuiVariables.offsetOriginY = (double)Graphic2GCode.multiImportOffsetY + (dimY + gap);
                            offX = Graphic2GCode.multiImportMaxX + gap;
                            GuiVariables.offsetOriginX = offX;
                        }
                        else
                        {   //offY += dimY + gap;
                            GuiVariables.offsetOriginY = offY + dimY + gap;
                        }
                    }
                }
                Logger.Info("{0} Remove offset: X:{1:0.000} Y:{2:0.000} new origin: X:{3:0.00} Y:{4:0.00}", loggerTag, actualDimension.minx, actualDimension.miny, offX, offY);
                SetHeaderInfo(string.Format(" Graphic offset: {0:0.00} {1:0.00} new origin: {2:0.00} {3:0.00}", -actualDimension.minx, -actualDimension.miny, offX, offY));
                SetHeaderInfo(string.Format(" Original graphic dimension min:{0:0.000};{1:0.000}  max:{2:0.000};{3:0.000}", actualDimension.minx, actualDimension.miny, actualDimension.maxx, actualDimension.maxy));

                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Remove Offset..." });
                RemoveOffset(completeGraphic, actualDimension.minx - offX, actualDimension.miny - offY);
            }

            /* multiply graphics */
            if (Properties.Settings.Default.importGraphicMultiplyGraphicsEnable)
            {
                int nX = (int)Properties.Settings.Default.importGraphicMultiplyGraphicsDimX;
                int nY = (int)Properties.Settings.Default.importGraphicMultiplyGraphicsDimY;
                double dist = (double)Properties.Settings.Default.importGraphicMultiplyGraphicsDistance;
                Logger.Info("{0} Multiply graphics nx:{1} ny:{2} distance:{3:0.000}", loggerTag, nX, nY, dist);
                MultiplyGraphics(completeGraphic, actualDimension, dist, nX, nY);
                SetHeaderInfo(string.Format(" Option: Multiply graphics X:{0} Y:{1} distance:{2:0.00}", nX, nY, dist));
            }     // repititions in x and y direction

            //           if (Properties.Settings.Default.importSVGNodesOnly)         { SetDotOnly(); }

            /* show original graphics in 2D-view */
            backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Create backgroud graphic..." });
            object lockObject = new object();
            Logger.Info("●●● Show original graphics in 2D-view");
            lock (lockObject)
            { CreateGraphicsPath(completeGraphic, VisuGCode.pathBackground); }

            VisuGCode.xyzSize.AddDimensionXY(Graphic.actualDimension);
            //    VisuGCode.CalcDrawingArea();                                // calc ruler dimension
            backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt * 100 / maxOpt), Content = "show" });

            if (logModification) { ListGraphicObjects(completeGraphic); }

            /* Filter (remove or keep paths with specific properties) */
            if (!cancelByWorker && Properties.Settings.Default.importGraphicFilterEnable)
            {
                Logger.Info("{0} Filter properties: ", loggerTag);
                FilterProperties(completeGraphic);
                SetHeaderInfo(string.Format(" Option: Filter properties"));
            }

            /* hatch fill */
            if (!cancelByWorker && (graphicInformation.ApplyHatchFill || graphicInformation.OptionHatchFill))
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Generate hatch fill..." });
                Logger.Info("{0} Hatch fill  distance:{1:0.00} angle:{2:0.00}", loggerTag, Properties.Settings.Default.importGraphicHatchFillDistance, Properties.Settings.Default.importGraphicHatchFillAngle);
                HatchFill(completeGraphic);
                SetHeaderInfo(string.Format(" Option: Hatch fill distance:{0:0.00} angle:{1:0.00}", Properties.Settings.Default.importGraphicHatchFillDistance, Properties.Settings.Default.importGraphicHatchFillAngle));
            }

            /* repeate paths */
            if (!cancelByWorker && graphicInformation.OptionRepeatCode && !Properties.Settings.Default.importRepeatComplete)
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Repeat paths(" + countGeometry.ToString() + " elements)..." });
                Logger.Info("{0} Repeate paths, count: {1}", loggerTag, Properties.Settings.Default.importRepeatCnt);
                RepeatPaths(completeGraphic, (int)Properties.Settings.Default.importRepeatCnt);
                SetHeaderInfo(string.Format(" Option: Repeat paths/code count:{0} ", Properties.Settings.Default.importRepeatCnt));
            }

            /* sort by distance and merge paths with same start / end coordinates*/
            if (!cancelByWorker && graphicInformation.OptionCodeSortDistance)
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Sort elements 1) merge paths (" + countGeometry.ToString() + " elements)" });
                Logger.Info("{0} Merge figures", loggerTag);
                MergeFigures(completeGraphic);
                if (!cancelByWorker)
                {
                    backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Sort elements 2) sort by distance (" + countGeometry.ToString() + " elements)" });
                    Logger.Info("{0} Sort by distance", loggerTag);
                    SortByDistance(completeGraphic, new Point(0, actualDimension.maxy), false);            // CreateGCode
                }
            }

            if (!cancelByWorker && graphicInformation.OptionCodeSortDimension)
            {
                Logger.Info("{0} Sort by dimension", loggerTag);
                SortByDimension(completeGraphic);
            }

            /* Drag Tool path modification*/
            if (!cancelByWorker && graphicInformation.OptionDragTool)
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Calculate drag tool paths (of " + countGeometry.ToString() + " elements)..." });
                Logger.Info("◆◆◆ Drag Tool path modification - use also tangential axis:{0}", graphicInformation.OptionTangentialAxis);
                if (graphicInformation.OptionTangentialAxis)
                {
                    DragToolModificationTangential(completeGraphic);
                    SetHeaderInfo(" Option: DragTool-Tangential");
                }
                else
                {
                    DragToolModification(completeGraphic);
                    SetHeaderInfo(" Option: DragTool");
                }
            }

            /* clipping or tiling of whole graphic */
            if (!cancelByWorker && graphicInformation.OptionClipCode)
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Apply clipping (on " + countGeometry.ToString() + " elements)..." });
                Logger.Info("◆◆◆ clipping or tiling of whole graphic");
                ClipCode((double)Properties.Settings.Default.importGraphicTileX, (double)Properties.Settings.Default.importGraphicTileY, (double)Properties.Settings.Default.importGraphicTileAddOnX);
                SetHeaderInfo(string.Format(" Option: Clipping/Tiling size X:{0:0.00} Y:{1:0.00}", Properties.Settings.Default.importGraphicTileX, Properties.Settings.Default.importGraphicTileY));
            }

            /* extend closed path for laser cutting to avoid a "nose" at start/end position */
            if (!cancelByWorker && graphicInformation.OptionExtendPath)
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Extend closed paths..." });
                Logger.Info("◆◆◆ Extend closed path");
                ExtendClosedPaths(completeGraphic, (double)Properties.Settings.Default.importGraphicExtendPathValue);
                SetHeaderInfo(string.Format(" Option: Extend path by {0:0.00}", Properties.Settings.Default.importGraphicExtendPathValue));
            }

            /* calculate tangential axis - paths may be splitted, do not merge afterwards!*/
            if (!cancelByWorker && graphicInformation.OptionTangentialAxis)
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Calculate tangential axis (for " + countGeometry.ToString() + " elements)..." });
                Logger.Info("●●● Calculate tangential axis ");
                CalculateTangentialAxis();
                SetHeaderInfo(string.Format(" Option: Tangential axis: '{0}'", Properties.Settings.Default.importGCTangentialAxis));
                if ((Properties.Settings.Default.importGCTangentialAxis == "Z") && (Properties.Settings.Default.importGCZEnable))
                    SetHeaderMessage(string.Format(" {0}-2001: Z is used as tangential axis AND as Pen-up/down axis", CodeMessage.Warning));
            }
            else
            {
                Logger.Info("●●● Calculate start angle");
                CalculateStartAngle();
            }

            if (Properties.Settings.Default.importGCAux1Enable)
            {
                Logger.Info("●●● CalculateDistances ");
                CalculateDistances();
            }

            /* List option data */
            if (!cancelByWorker && (graphicInformation.OptionZFromWidth || (graphicInformation.OptionDotFromCircle && graphicInformation.OptionZFromRadius)))
            {
                string tmp1 = string.Format(" penMin: {0:0.00} penMax: {1:0.00}",
                graphicInformation.PenWidthMin, graphicInformation.PenWidthMax);
                string tmp2 = string.Format("   zMin:{0:0.00}   zMax:{1:0.00}  zLimit:{2:0.00}",
                Properties.Settings.Default.importDepthFromWidthMin, Properties.Settings.Default.importDepthFromWidthMax,
                Properties.Settings.Default.importGCZDown);
                SetHeaderInfo(" Option: Z from width or radius");
                SetHeaderInfo(tmp1);
                SetHeaderInfo(tmp2);
                if (logEnable) Logger.Trace("----OptionZFromWidth {0}{1}", tmp1, tmp2);
            }


            if (Properties.Settings.Default.importGCConvertToPolar)
            {
                Logger.Info("◆◆◆ Convert to polar coordinates");
                PolarCoordinates();
                SetHeaderInfo(" Option: Polar coordinates: X=radius, Y=angle");
            }

            if (Properties.Settings.Default.importGraphicWireBenderEnable)
            {
                Logger.Info("◆◆◆ Wire bender");
                WireBender();
                SetHeaderInfo(" Option: Wire bender");
            }

            if (Properties.Settings.Default.importGraphicDevelopmentEnable)
            {
                Logger.Info("◆◆◆ Develop path");
                Develop();
                SetHeaderInfo(" Option: Develop path");
            }


            VisuGCode.xyzSize.AddDimensionXY(Graphic.actualDimension);
            SetHeaderInfo(string.Format(" Dimension XY: {0:0.0} {1:0.0} ", Graphic.actualDimension.dimx, Graphic.actualDimension.dimy));

            if ((maxObjectCountBeforeReducingXML > 0) && (completeGraphic.Count > maxObjectCountBeforeReducingXML))
            {
                Logger.Warn("███► CreateGCode disable figure XML code to keep usability (completeGraphic.Count > {0}), count:{1}  ⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠", maxObjectCountBeforeReducingXML, completeGraphic.Count);
                SetHeaderMessage(string.Format(" {0}-2002: high amount of Figure objects, count:{1},  max:{2}", CodeMessage.Warning, completeGraphic.Count.ToString(), maxObjectCountBeforeReducingXML.ToString()));
                SetHeaderMessage(" Figure codes will be summarized to one Figure.");
                graphicInformation.FigureEnable = false;
            }
            else
                Logger.Info("●●● CreateGCode completeGraphic.Count:{0}", completeGraphic.Count);

            /* group objects by color/width/layer/tile-nr */
            if (!cancelByWorker && graphicInformation.GroupEnable)
            {
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = (actOpt++ * 100 / maxOpt), Content = "Group " + countGeometry.ToString() + " elements..." });

                // add tile-tags and group
                if (graphicInformation.OptionClipCode && !Properties.Settings.Default.importGraphicClip)
                {
                    Logger.Info("▲▲▲▲  Graphic - Return group tiledGraphic");
                    GroupTileContent(graphicInformation);
                    return Graphic2GCode.CreateGCode(tiledGraphic, headerInfo, headerMessage, graphicInformation);
                }

                if (!GroupAllGraphics(completeGraphic, groupedGraphic, graphicInformation))
                {
                    Logger.Info("▲▲▲▲  Graphic - Return completeGraphic");
                    return Graphic2GCode.CreateGCode(completeGraphic, headerInfo, headerMessage, graphicInformation);
                }

                Logger.Info("▲▲▲▲  Graphic - Return groupedGraphic");
                return Graphic2GCode.CreateGCode(groupedGraphic, headerInfo, headerMessage, graphicInformation, false);   // useTiles
            }
            if (!cancelByWorker && !graphicInformation.GroupEnable)
            {   // add tile-tags, don't group
                if (graphicInformation.OptionClipCode && !Properties.Settings.Default.importGraphicClip)
                {
                    Logger.Info("▲▲▲▲  Graphic - Return tiledGraphic");
                    return Graphic2GCode.CreateGCode(tiledGraphic, headerInfo, headerMessage, graphicInformation);
                }
            }

            if (cancelByWorker)
            {
                bool tmpResult = Graphic2GCode.CreateGCode(completeGraphic, headerInfo, headerMessage, graphicInformation);
                backgroundEvent.Cancel = true;
                Logger.Info("▲▲▲▲  Graphic - Return completeGraphic - after cancelation");
                return tmpResult;
            }

            Logger.Info("▲▲▲▲  Graphic - Return completeGraphic - final");
            return Graphic2GCode.CreateGCode(completeGraphic, headerInfo, headerMessage, graphicInformation); // Graphic.Gcode will be filled, return true
        }
        // #######################################################################

        private static int GetOptionsAmount()
        {
            int amount = 1; // backgroud
            if (Properties.Settings.Default.importRemoveShortMovesEnable) amount++;/* remove short moves*/
            if (graphicInformation.OptionCodeOffset) amount++;/* remove offset */
            if (graphicInformation.ApplyHatchFill || graphicInformation.OptionHatchFill) amount++;/* hatch fill */
            if (graphicInformation.OptionRepeatCode && !Properties.Settings.Default.importRepeatComplete) amount++;/* repeate paths */
            if (graphicInformation.OptionCodeSortDistance) amount++;/* sort by distance and merge paths with same start / end coordinates*/
            if (graphicInformation.OptionDragTool) amount++;/* Drag Tool path modification*/
            if (graphicInformation.OptionClipCode) amount++;/* clipping or tiling of whole graphic */
            if (graphicInformation.OptionExtendPath) amount++;/* extend closed path for laser cutting to avoid a "nose" at start/end position */
            if (graphicInformation.OptionTangentialAxis) amount++;/* calculate tangential axis - paths may be splitted, do not merge afterwards!*/
            if (graphicInformation.OptionZFromWidth || (graphicInformation.OptionDotFromCircle && graphicInformation.OptionZFromRadius)) amount++;
            if (graphicInformation.GroupEnable) amount++;

            return amount;
        }


        internal static bool ReDoReversePath(int figureNr, XyPoint aP, int[] sortOrderFigure, int[] sortOrderGroup, bool reverse)	// called in MainFormPictureBox
        {
            if (reverse)
                Logger.Info("▌ ReDoReversePath reverse figure:{0}  posX:{1:0.00}  posY:{2:0.00}  GroupEnable:{3}  ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄", figureNr, aP.X, aP.Y, graphicInformation.GroupEnable);
            else
                Logger.Info("▌ ReDoReversePath rotate  figure:{0}  posX:{1:0.00}  posY:{2:0.00}  GroupEnable:{3}  ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄", figureNr, aP.X, aP.Y, graphicInformation.GroupEnable);

            if (sortOrderFigure == null) { Logger.Warn("▌ ReDoReversePath sortOrderFigure == null"); return false; }

            if (graphicInformation.GroupEnable && (groupedGraphic != null) && (groupedGraphic.Count > 0))
            {
                Logger.Info("▌ ReDoReversePath process groupedGraphic:{0}", groupedGraphic.Count);
                foreach (GroupObject tmpGrp in groupedGraphic)
                {
                    for (int i = 0; i < tmpGrp.GroupPath.Count; i++)
                    {
                        if ((figureNr > 0) && (figureNr <= sortOrderFigure.Length) && (tmpGrp.GroupPath[i].FigureId == sortOrderFigure[figureNr - 1]))
                        //                        if (tmpGrp.groupPath[i].FigureId == figureNr)     
                        {
                            if (tmpGrp.GroupPath[i] is ItemPath ipath)
                            { ReverseOrRotatePath(ipath, aP, reverse); }
                        }
                    }
                    SetOrderOfFigures(tmpGrp.GroupPath, sortOrderFigure);     // restore order from GCode
                }


                List<GroupObject> groupedGraphicTmp = new List<GroupObject>();
                if (sortOrderGroup == null) return false;
                foreach (int groupId in sortOrderGroup)
                {
                    foreach (GroupObject tmpGrp in groupedGraphic)
                    {
                        if (tmpGrp.GroupId == groupId)
                        {
                            groupedGraphicTmp.Add(tmpGrp);
                            if (logSortMerge) Logger.Trace(" setOrder {0}", groupId);
                        }
                    }
                }
                groupedGraphic.Clear();
                foreach (GroupObject tmpGrp in groupedGraphicTmp)
                    groupedGraphic.Add(tmpGrp);

                for (int k = 0; k < groupedGraphic.Count; k++)
                    Logger.Trace("sortOrderGroup {0}  ->  {1}", k, groupedGraphic[k].GroupId);
            }
            else
            {
                if (completeGraphic == null) { Logger.Warn("▌ ReDoReversePath completeGraphic == null"); return false; }

                Logger.Info("▌ ReDoReversePath process completeGraphic:{0}", completeGraphic.Count);
                foreach (PathObject tmp in completeGraphic)
                {
                    if ((figureNr > 0) && (figureNr <= sortOrderFigure.Length) && (tmp.FigureId == sortOrderFigure[figureNr - 1]))
                    //                    if (tmp.FigureId == figureNr)       
                    {
                        if (tmp is ItemPath path)
                        { ReverseOrRotatePath(path, aP, reverse); }
                    }
                }
                SetOrderOfFigures(completeGraphic, sortOrderFigure);        // restore order from GCode
            }

            /* calculate tangential axis - paths may be splitted, do not merge afterwards!*/
            if (graphicInformation.OptionTangentialAxis)
            {
                Logger.Info("●●● Calculate tangential axis ");
                CalculateTangentialAxis();
            }
            else
            {
                Logger.Info("●●● Calculate start angle");
                CalculateStartAngle();
            }

            graphicInformation.ReProcess = true;

            if (graphicInformation.GroupEnable) // group objects by color/width/layer/tile-nr 
            {
                return Graphic2GCode.CreateGCode(groupedGraphic, headerInfo, headerMessage, graphicInformation, false);    // Grouped graphic - useTiles
            }
            return Graphic2GCode.CreateGCode(completeGraphic, headerInfo, headerMessage, graphicInformation);			// complete graphic
        }

        private static void ReverseOrRotatePath(ItemPath tmpItemPath, XyPoint aP, bool reverse)
        {
            //ItemPath tmpItemPath = (ItemPath)tmp;
            if (!reverse && (IsEqual(tmpItemPath.Start, tmpItemPath.End)))
            {
                /* find closest point*/
                Point actualPos = new Point(aP.X, aP.Y);
                if (tmpItemPath.Path.Count > 2)
                {
                    PathDistanceSquared(actualPos, tmpItemPath);	// fill tmpItemPath.TmpIndex and tmpItemPath.Distance
                    int index = tmpItemPath.TmpIndex;               // (int)tmpItemPath.StartAngle;
                    if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  path index:{2}   distance:{3:0.00}", tmpItemPath.Info.Id, tmpItemPath.Info.PathGeometry, index, tmpItemPath.Distance);
                    if ((index >= 0) && (index < tmpItemPath.Path.Count))
                    { tmpItemPath.Start = tmpItemPath.End = tmpItemPath.Path[tmpItemPath.TmpIndex].MoveTo; } //(int)tmpItemPath.StartAngle
                }
                else if (tmpItemPath.Path.Count == 2)       // is circle
                {
                    if (tmpItemPath.Path[1] is GCodeArc)
                    {
                        CircleDistanceSquared(actualPos, tmpItemPath);
                        if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  angle:{2:0.0}  distance:{3:0.00}", tmpItemPath.Info.Id, tmpItemPath.Info.PathGeometry, (tmpItemPath.StartAngle * 180 / Math.PI), tmpItemPath.Distance);
                    }
                }

                RotatePath(tmpItemPath);
            }
            else
                ReversePath(tmpItemPath);
        }
    }
}