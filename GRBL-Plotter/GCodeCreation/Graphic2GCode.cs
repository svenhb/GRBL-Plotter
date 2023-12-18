/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2023 Sven Hasemann contact: svenhb@web.de

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
 * 2019-09-06 new class for high level commands
 * 2019-11-28 add penup in line 180
 * 2019-11-30 add line 381- Arc added code - for DXF circle multiple pass 
 * 2020-01-01 add public enum xmlMarkerType
 * 2020-01-10 add Use-case to output line 501
 * 2020-02-18 add tangential axis support (doesn't work with 'repeatZ' because of inserted PenUp/Down to process swivel angle)
 * 2020-02-28 remove empty figure sections FigureCheck[] lastFigureStart
 * 2020-04-04 replace ArcToCCW
 * 2020-04-09 extend class xmlMarker
 * 2020-04-11 fix splitting problem for attributes in element-string (if value contains ' ')
 * 2020-04-13 add splitArc to support tangential axis
 * 2021-01-16 bug fix: code from tiles without grouping are generated multiple times -> line 264 add gcodeString.Clear(); 
 * 2021-07-14 code clean up / code quality
 * 2021-08-08 if graphicInfo.OptionSpecialDevelop get Z from path
 * 2021-08-26 line 220, 290 use tool colors if tools are used
 * 2021-09-02 CreateGCode-TileObject add XML-Tag OffsetX,-Y
 * 2021-09-21 new GroupOption 'Label' - add txt to layer
 * 2022-01-23 line 466 switch index of "layer" and "type"
 * 2022-01-23 l:265 f:CreateGCode (group) add proforma figure-tag if not figureEnable
 * 2022-03-29 function 'arc' line 900 if full circle, end_angle = start_angle+360° issue #270
 * 2022-04-04 line 547 change "PathID" to "PathId"
 * 2022-11-04 change dash-apply algorithm in MoveToDashed to continue pattern in next move-segement 
 * 2023-03-07 l:256 f:CreateGCode  add color to ToolChange call "[color]"
 * 2023-03-14 l:610 f:StartPath	importGraphicLeadInEnable optional start at GcodeZUp value
 * 2023-09-14 f:CreateGCode add CollectionStart /-End Tags
 * 2023-11-03 l:357 f:CreateGCode (figure) add proforma figure-tag if not figureEnable
 * 2023-11-27 l:465 f:ProcessPathObject call subroutine only if needed
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using static GrblPlotter.Graphic;

namespace GrblPlotter
{
    public static class Graphic2GCode
    {
        private static uint logFlags = 0;

        private static readonly StringBuilder gcodeString = new StringBuilder();
        private static readonly StringBuilder finalGcodeString = new StringBuilder();
        public static bool multiImport = false;
        public static int multiImportNr = 0;
        public static string multiImportName = "";
        public static decimal multiImportOffsetX = 0;
        public static decimal multiImportOffsetY = 0;
        public static double multiImportMaxX = 0;
        public static double multiImportMaxY = 0;

        private static bool penIsDown = false;
        private static bool comments = false;
        private static bool pauseBeforePath = false;
        private static bool pauseBeforePenDown = false;

        private static bool useAlternitveZ = false;
        private static bool useToolTable = false;

        private static bool figureEnable = true;

        private static Point lastGC;//, lastSetGC;             // store last position

        public static int PathCount { get; set; } = 0;
        public static bool FigureEndTagWasSet { get; set; } = true;

        private static double[] PathDashArray;
        private static int PathDashArrayIndex = 0;
        private static bool PathDashArrayIndexChanged = false;
        private static double PathDashArrayDistance = 0;
        private static bool PathDashArrayPenIsUp = false;

        private static double setAux1FinalDistance = 0;     // sum-up path distances 396
        private static double setAux2FinalDistance = 0;

        private static int dotCounter = 0;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //    private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static bool logEnable;
        private static bool logDetailed;
        private static bool logCoordinates;

        private static bool gcodeComments = false;

        public static void CleanUp()
        {
            //   Logger.Trace("CleanUp()");
            gcodeString.Clear();
            gcodeString.Length = 0;
            finalGcodeString.Clear();
        }

        public static void Init()
        {
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level3) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnables.Detailed) > 0);
            logCoordinates = logEnable && ((logFlags & (uint)LogEnables.Coordinates) > 0);

            gcodeComments = Properties.Settings.Default.importGCAddComments;

            Logger.Trace("▽▽▽Graphic2GCode - Init   loggerTrace:{0}", Convert.ToString(logFlags, 2));

            pauseBeforePath = Properties.Settings.Default.importPauseElement;
            pauseBeforePenDown = Properties.Settings.Default.importPausePenDown;
            comments = Properties.Settings.Default.importSVGAddComments;
            penIsDown = false;

            gcodeString.Clear();
            finalGcodeString.Clear();

            PathCount = 0;
            FigureEndTagWasSet = true;
            Gcode.Setup(true);  // convertGraphics=true (repeat, inser sub)                              // initialize GCode creation (get stored settings for export)
            pathInfo = new PathInformation();

            useAlternitveZ = Properties.Settings.Default.importDepthFromWidthRamp;

            useToolTable = Properties.Settings.Default.importGCToolTableUse;
        }

        /// <summary>
        /// Create GCode from tiles, no further sorting needed.
        /// </summary>		
        private static int mainGroupID = 0;
        internal static bool CreateGCode(List<Graphic.TileObject> tiledGraphic, List<string> headerInfo, List<string> headerMessage, Graphic.GraphicInformationClass graphicInfo)
        {
            string xmlTag;
            int iDToSet = 1;
            mainGroupID = 0;
            setAux1FinalDistance = 0;
            setAux2FinalDistance = 0;
            dotCounter = 0;

            if (logEnable) Logger.Trace("-CreateGCode from Tiles");

            Init();                                         // initalize variables, toolTable.init(), gcode.setup()
            if (headerInfo != null)
            {
                foreach (string info in headerInfo)             // set header info
                { Gcode.AddToHeader(info); }
            }
            if (headerMessage != null)
            {
                foreach (string info in headerMessage)     // set header info
                { Gcode.AddToHeader(info, false); }
            }
            Gcode.JobStart(finalGcodeString, "StartJob");

            if (graphicInfo == null) return false;
            if (tiledGraphic == null) return false;
            //    double offsetX, offsetY;

            SetHalftoneMode(graphicInfo);

            if (multiImport) { Gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\" Name=\"{2}\">", XmlMarker.CollectionStart, multiImportNr, multiImportName)); Logger.Info("Collection Tile"); }
            foreach (TileObject tileObject in tiledGraphic)
            {
                xmlTag = string.Format("{0} Id=\"{1}\" Pos=\"{2}\" OffsetX=\"{3:0.000}\"  OffsetY=\"{4:0.000}\">", XmlMarker.TileStart, iDToSet, tileObject.Key, tileObject.Offset.X, tileObject.Offset.Y);
                Gcode.Comment(finalGcodeString, xmlTag);
                if (logEnable) Logger.Trace("-CreateGCode {0} ", xmlTag);

                if (!string.IsNullOrEmpty(tileObject.TileRelatedGCode))
                {
                    string[] commands = { };
                    commands = tileObject.TileRelatedGCode.Split(';');
                    foreach (string cmd in commands)
                    {
                        finalGcodeString.AppendLine(cmd);
                    }
                }

                if (graphicInfo.GroupEnable)
                    CreateGCode(tileObject.Tile, headerInfo, headerMessage, graphicInfo, true);        // create grouped code
                else
                    CreateGCode(tileObject.GroupPath, headerInfo, headerMessage, graphicInfo, true);   // create path code

                Gcode.Comment(finalGcodeString, XmlMarker.TileEnd + ">");
                iDToSet++;
            }
            if (multiImport) { Gcode.Comment(finalGcodeString, string.Format("{0}>", XmlMarker.CollectionEnd)); }

            Gcode.JobEnd(finalGcodeString, "EndJob");       // Spindle / laser off

            return FinalGCode(graphicInfo.Title, graphicInfo.FilePath);
        }

        /// <summary>
        /// Create GCode from already sorted groups, no further sorting needed.
        /// </summary>		
        internal static bool CreateGCode(List<Graphic.GroupObject> completeGraphic, List<string> headerInfo, List<string> headerMessage, Graphic.GraphicInformationClass graphicInfo, bool useTiles)
        {
            if (graphicInfo == null) return false;

            overWriteId = graphicInfo.ReProcess;    // keep IDs from previous conversion
                                                    //    useIndividualZ = graphicInfo.OptionZFromWidth;

            if (logEnable) Logger.Trace("-CreateGCode from Groups");

            if (!useTiles)
            {
                Init();                                 // initalize variables, toolTable.init(), gcode.setup()
                if (headerInfo != null)
                {
                    foreach (string info in headerInfo)     // set header info
                    { Gcode.AddToHeader(info); }
                }
                if (headerMessage != null)
                {
                    foreach (string info in headerMessage)     // set header info
                    { Gcode.AddToHeader(info, false); }
                }
                Gcode.JobStart(finalGcodeString, "StartJob");
                mainGroupID = 0;
                setAux1FinalDistance = 0;
                setAux2FinalDistance = 0;
                dotCounter = 0;
            }

            int groupID = mainGroupID;
            int iDToSet;
            string groupAttributes;

            if (completeGraphic == null) return false;

            SetHalftoneMode(graphicInfo);

            if (multiImport && !useTiles) { Gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\" Name=\"{2}\">", XmlMarker.CollectionStart, multiImportNr, multiImportName)); Logger.Info("Collection Group"); }
            foreach (GroupObject groupObject in completeGraphic)
            {
                groupAttributes = GetGroupAttributes(groupObject, graphicInfo);

                groupID++;
                iDToSet = groupID;

                if (overWriteId && (groupObject.GroupId > 0))
                    iDToSet = groupObject.GroupId;

                groupObject.GroupId = iDToSet;  // track id
                Gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\"{2}>", XmlMarker.GroupStart, iDToSet, groupAttributes));
                if (logEnable) Logger.Trace("-CreateGCode {0} Id=\"{1}\"{2}>", XmlMarker.GroupStart, iDToSet, groupAttributes);

                if (!graphicInfo.FigureEnable)  // proforma figure tag
                {
                    string figColor = "", figWidth = "";
                    if (groupObject.GroupPath.Count > 0)
                    {
                        figColor = string.Format(" PenColor=\"{0}\"", groupObject.GroupPath[0].Info.GroupAttributes[(int)GroupOption.ByColor]);
                        figWidth = string.Format(" PenWidth=\"{0}\"", groupObject.GroupPath[0].Info.GroupAttributes[(int)GroupOption.ByWidth]);
                    }
                    Gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\" {2} {3}>", XmlMarker.FigureStart, iDToSet, figColor, figWidth));
                }

                if (logEnable) Logger.Trace("CreateGCode-Group  toolNr:{0}  name:{1}", groupObject.ToolNr, groupObject.ToolName);
                ToolChange(groupObject.ToolNr, groupObject.GroupPath.Count, groupObject.ToolName + " [" + groupObject.GroupPath[0].Info.GroupAttributes[(int)GroupOption.ByColor] + "]");   // add tool change commands (if enabled) and set XYFeed etc.

                foreach (PathObject pathObject in groupObject.GroupPath)
                {
                    if (logEnable) Logger.Trace(" ProcessPathObject id:{0} ", pathObject.Info.Id);

                    if (useToolTable)                                                   // 2021-08-26 #217
                    {
                        int toolToUse = groupObject.ToolNr;
                        if (Properties.Settings.Default.importGCToolDefNrUse)
                            toolToUse = (int)Properties.Settings.Default.importGCToolDefNr;
                        string toolColor = ToolTable.GetToolColor(toolToUse);
                        pathObject.Info.GroupAttributes[(int)GroupOption.ByColor] = toolColor;
                    }
                    ProcessPathObject(pathObject, graphicInfo, -1, "");	// create Dot or Path GCode, but no tool change
                }
                PenUp(" CreateGCode 1", true);      // set xmlMarker.figureEnd

                finalGcodeString.Append(gcodeString);
                gcodeString.Clear();                            // don't add gcode a 2nd time

                if (!graphicInfo.FigureEnable)  // proforma figure tag
                {
                    Gcode.Comment(finalGcodeString, XmlMarker.FigureEnd + ">");
                }

                Gcode.Comment(finalGcodeString, XmlMarker.GroupEnd + ">");
                if (logEnable) Logger.Trace("-CreateGCode {0} >", XmlMarker.GroupEnd);
            }
            if (multiImport && !useTiles) { Gcode.Comment(finalGcodeString, string.Format("{0}>", XmlMarker.CollectionEnd)); }

            mainGroupID = groupID;
            if (!useTiles)
            {
                Gcode.JobEnd(finalGcodeString, "EndJob");       // Spindle / laser off
                return FinalGCode(graphicInfo.Title, graphicInfo.FilePath);
            }
            else
                return true;        // go on with next tile
        }

        /// <summary>
        /// Create GCode from already sorted paths, no further sorting needed.
        /// </summary>		
        internal static bool CreateGCode(List<Graphic.PathObject> completeGraphic, List<string> headerInfo, List<string> headerMessage, Graphic.GraphicInformationClass graphicInfo, bool useTiles = false)
        {
            if (graphicInfo == null) return false;

            overWriteId = graphicInfo.ReProcess;    // keep IDs from previous conversion
                                                    //	useIndividualZ = graphicInfo.OptionZFromWidth;

            if (logEnable) Logger.Trace("-CreateGCode from paths");

            if (!useTiles)
            {
                Init();                                 // initalize variables, toolTable.init(), gcode.setup()
                if (headerInfo != null)
                {
                    foreach (string info in headerInfo)     // set header info
                    { Gcode.AddToHeader(info); }
                }
                if (headerMessage != null)
                {
                    foreach (string info in headerMessage)     // set header info
                    { Gcode.AddToHeader(info, false); }
                }
                setAux1FinalDistance = 0;
                setAux2FinalDistance = 0;
                dotCounter = 0;
                if (Properties.Settings.Default.importSVGCircleToDot && (Properties.Settings.Default.importCircleToDotScriptCount > 0))
                    Gcode.SetSubroutine(Properties.Settings.Default.importCircleToDotScript, 95);
            }

            SetHalftoneMode(graphicInfo);

            int toolNr;
            string toolName;
            string toolColor;
            if (completeGraphic == null) return false;

            if (!graphicInfo.FigureEnable)  // proforma figure tag
            {
                string figColor = string.Format(" PenColor=\"{0}\"", completeGraphic[0].Info.GroupAttributes[(int)GroupOption.ByColor]);
                string figWidth = string.Format(" PenWidth=\"{0}\"", completeGraphic[0].Info.GroupAttributes[(int)GroupOption.ByWidth]);
                Gcode.Comment(gcodeString, string.Format("{0} Id=\"{1}\" {2} {3}>", XmlMarker.FigureStart, 0, figColor, figWidth));
            }

            foreach (PathObject pathObject in completeGraphic)		// go through all graphics elements
            {
                // get tool-nr by color or use color-id		 
                if (Properties.Settings.Default.importDXFToolIndex)
                { toolNr = pathObject.Info.PenColorId + 1; }     // avoid ID=0 to start tool-table with index 1
                else
                {
                    toolColor = pathObject.Info.GroupAttributes[(int)GroupOption.ByColor];
                    toolNr = ToolTable.GetToolNRByToolColor(toolColor, 0);
                }

                // real tool to use: default or from graphic	   
                int toolToUse = toolNr;
                if (useToolTable && Properties.Settings.Default.importGCToolDefNrUse)
                    toolToUse = (int)Properties.Settings.Default.importGCToolDefNr;

                toolName = ToolTable.GetToolName(toolToUse);
                toolColor = ToolTable.GetToolColor(toolToUse);          // 2021-08-26 before toolNr

                if (useToolTable)                                       // 2021-08-26 #217
                    pathObject.Info.GroupAttributes[(int)GroupOption.ByColor] = toolColor;

                if (logEnable) Logger.Trace("CreateGCode2  toolNr:{0}  name:{1}", toolToUse, toolName);
                // add tool change after <Figure tag	   
                ProcessPathObject(pathObject, graphicInfo, toolToUse, toolName + "=[" + toolColor + "]");	// create Dot or Path GCode
            }

            PenUp(" CreateGCode 2", true);    // set xmlMarker.figureEnd

            if (!graphicInfo.FigureEnable)  // proforma figure tag
            {
                Gcode.Comment(gcodeString, XmlMarker.FigureEnd + ">");
            }

            if (!useTiles)
            {
                Gcode.JobStart(finalGcodeString, "StartJob");
                if (multiImport) { Gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\" Name=\"{2}\">", XmlMarker.CollectionStart, multiImportNr, multiImportName)); Logger.Info("Collection Figure"); }
                finalGcodeString.Append(gcodeString);
                if (multiImport) { Gcode.Comment(finalGcodeString, string.Format("{0}>", XmlMarker.CollectionEnd)); }
                Gcode.JobEnd(finalGcodeString, "EndJob");      // Spindle / laser off
                return FinalGCode(graphicInfo.Title, graphicInfo.FilePath);
            }
            else
            {
                finalGcodeString.Append(gcodeString);
                gcodeString.Clear();                            // don't add gcode a 2nd time
                return true;                                    // go on with next tile
            }
        }

        //convert graphic to gcode ##################################################################
        private static void ProcessPathObject(PathObject pathObject, Graphic.GraphicInformationClass graphicInfo, int toolNr, string toolCmt)
        {
            if (logDetailed) Logger.Trace("ProcessPathObject start");
            figureEnable = graphicInfo.FigureEnable;
            float origZ = Gcode.GcodeZDown;
            float origPWM = Gcode.GcodePwmDown;
            float origSpindle = Gcode.GcodeSpindleSpeed;

            useAlternitveZ = Properties.Settings.Default.importDepthFromWidthRamp || graphicInfo.DxfImportZ;

            /* Create Dot */
            if (pathObject is ItemDot DotData)
            {   //ItemDot DotData = (ItemDot)pathObject;
                if (DotData.UseZ)
                {
                    double setZ = CalculateZFromRange(graphicInfo.DotZMin, graphicInfo.DotZMax, DotData.OptZ);//-Math.Abs(DotData.Z);      // be sure for right sign
                    if (logEnable)
                        Logger.Trace("---Dot DotData.UseZ: RangeMin:{0:0.00}  RangeMax:{1:0.00}  DotData.Z:{2:0.00}  -> setZ:{3:0.00}", graphicInfo.DotZMin, graphicInfo.DotZMax, DotData.OptZ, setZ);
                    setZ = Math.Max(origZ, setZ);    // don't go deeper than set Z
                    if (logCoordinates) Logger.Trace("  PenDownWithZ z:{0:0.00}  setZ:{1:0.00}  gcodeZDown:{2:0.00}", DotData.OptZ, setZ, origZ);
                    Gcode.GcodeZDown = (float)setZ;

                    double newS = CalculateSFromRange(graphicInfo.DotZMin, graphicInfo.DotZMax, DotData.OptZ);
                    Gcode.GcodePwmDown = Gcode.GcodeSpindleSpeed = (float)newS;   //???

                    penIsDown = false;
                }
                else if (graphicInfo.OptionZFromWidth)
                {
                //    dotCounter++;
                    double newZ = CalculateZFromRange(graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, DotData.OptZ);
                    if (logEnable) Logger.Trace("---Dot OptionZFromWidth: RangeMin:{0:0.00}  RangeMax:{1:0.00}  DotData.Z:{2:0.00}  -> setZ:{3:0.00}", graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, DotData.OptZ, newZ);
                    newZ = Math.Max(origZ, newZ);        // don't go deeper than set Z
                    Gcode.GcodeZDown = (float)newZ;
                    penIsDown = false;
                }
                if (graphicInfo.OptionSFromWidth)
                {
                //    dotCounter++;
                    double newS = CalculateSFromRange(graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, DotData.OptZ);
                    if (logEnable) Logger.Trace("--ProcessPathObject: penWidth:{0:0.00}  -> setS:{1:0.00}", DotData.OptZ, newS);
                    Gcode.GcodePwmDown = Gcode.GcodeSpindleSpeed = (float)newS;   //???
                    penIsDown = false;
                }

                dotCounter++;
                pathObject.FigureId = StartPath(DotData, toolNr, toolCmt, "PD");
                PenDown("PD");
                StopPath("PU DOT");

                if (Properties.Settings.Default.importSVGCircleToDot && (Properties.Settings.Default.importCircleToDotScriptCount > 0) && (dotCounter >= Properties.Settings.Default.importCircleToDotScriptCount))
                {
                    dotCounter = 0;
                    Gcode.CallSubroutine(gcodeString, 95, "refresh stamp");
                }
                //    Gcode.GcodeZDown = origZ;
            }
            else
            {
                if (graphicInfo.OptionZFromWidth)
                    Gcode.GcodeZDown = (float)Math.Max(Properties.Settings.Default.importDepthFromWidthMin, 0); // 0;

                ItemPath PathData = (ItemPath)pathObject;
                if (logDetailed) Logger.Trace(" {0}  cnt:{1}", PathData.Info.List(), PathData.Path.Count);

                if (PathData.Path.Count == 0)
                {
                    if (logEnable) Logger.Trace("--ProcessPathObject: Empty path ID:{0}", PathData.Info.Id);
                    return;
                }

                /* *.Depth also used in GenerateMisc-CalculateDistances() to store distance */
                if (graphicInfo.OptionSpecialDevelop || graphicInfo.DxfImportZ)	// apply given z-value from *.Depth (Develop: zCut, zNotch; or from DXF-z)
                {
                    Gcode.GcodeZDown = (float)PathData.Path[0].Depth;
                    //  Logger.Info("ProcessPathObject OptionSpecialDevelop start Z:{0:0.000}", PathData.Path[0].Depth);
                }
                if (graphicInfo.OptionSpecialWireBend)
                {
                    InsertCode(Properties.Settings.Default.importGraphicWireBenderCodePegOff);
                }

                pathObject.FigureId = StartPath(PathData, toolNr, toolCmt, "PD");
                PathDashArray = new double[PathData.DashArray.Length];
                PathData.DashArray.CopyTo(PathDashArray, 0);

                double newZ = Gcode.GcodeZDown;     // default
                double? newS = null;
                bool optionSpecialWireBendOn = false;

                int index;
                GCodeMotion entity;
                bool setAux1Enable = Properties.Settings.Default.importGCAux1Enable;
                bool setAux2Enable = Properties.Settings.Default.importGCAux2Enable;

                for (index = 1; index < PathData.Path.Count; index++) // 0 was already processed in StartPath
                {
                    entity = PathData.Path[index];
                    if (graphicInfo.OptionZFromWidth)
                    {
                        newZ = CalculateZFromRange(graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, entity.Depth);
                        newZ = Math.Max(origZ, newZ);        // don't go deeper than set Z
                        Gcode.GcodeZDown = (float)newZ;
                        if (!Properties.Settings.Default.importDepthFromWidthRamp)
                            penIsDown = false;
                        if (logEnable) Logger.Trace("--ProcessPathObject-OptionZFromWidth: penWidth:{0:0.00}  -> setZ:{1:0.00}  min:{2:0.00}  max:{3:0.00}  orig:{4:0.00}", entity.Depth, newZ, graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, origZ);
                    }
                    if (graphicInfo.OptionSFromWidth)
                    {
                        newS = CalculateSFromRange(graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, entity.Depth);
                        if (logEnable) Logger.Trace("--ProcessPathObject-OptionSFromWidth: penWidth:{0:0.00}  -> setS:{1:0.00}", entity.Depth, newS);
                        Gcode.GcodePwmDown = Gcode.GcodeSpindleSpeed = (float)newS;	//???
                    }

                    if (graphicInfo.OptionSpecialDevelop || graphicInfo.DxfImportZ)
                    {
                        newZ = Gcode.GcodeZDown = (float)entity.Depth;
                    }
                    if (graphicInfo.OptionSpecialWireBend)
                    {
                        if ((entity.Depth > 0.9) && !optionSpecialWireBendOn)
                        {
                            optionSpecialWireBendOn = true;
                            InsertCode(Properties.Settings.Default.importGraphicWireBenderCodePegOn);
                        }
                        else if ((entity.Depth < 0.1) && optionSpecialWireBendOn)
                        {
                            optionSpecialWireBendOn = false;
                            InsertCode(Properties.Settings.Default.importGraphicWireBenderCodePegOff);
                        }
                    }
                    if (setAux1Enable) { CalculateAux1(entity); }	// update setAux1FinalDistance
                    if (setAux2Enable) { CalculateAux2(entity); }

                    /* Create Line */
                    if (entity is GCodeLine)
                    {
                        MoveTo(entity.MoveTo, newZ, newS, entity.Angle, "");
                    }
                    else if (entity is GCodeArc ArcData)
                    {
                        /* Create Arc */
                        Arc(ArcData.IsCW, ArcData.MoveTo, ArcData.CenterIJ, ArcData.AngleStart, ArcData.Angle);//, "");// entity.comment);
                    }
                }
                StopPath("PU");
                if (graphicInfo.OptionSpecialWireBend)
                {
                    InsertCode(Properties.Settings.Default.importGraphicWireBenderCodeCut);
                }

            }
            Gcode.GcodeZDown = origZ;
            Gcode.GcodePwmDown = origPWM;
            Gcode.GcodeSpindleSpeed = origSpindle;
            if (logDetailed) Logger.Trace("ProcessPathObject end");
        }

        private static void InsertCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string[] commands;
                if (File.Exists(code))
                {
                    string fileCmd = File.ReadAllText(code);
                    commands = fileCmd.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }
                else
                { commands = code.Split(';'); }

                foreach (string cmd in commands)
                {
                    gcodeString.AppendLine(cmd);
                }
            }
        }
        private static void CalculateAux1(GCodeMotion entity)
        {
            int setAux1ZMode = Properties.Settings.Default.importGCAux1ZMode;
            double setAux1ZFactor = (double)Properties.Settings.Default.importGCAux1ZFactor;
            double distValue = entity.Depth * (double)Properties.Settings.Default.importGCAux1Factor;
            if (Properties.Settings.Default.importGCAux1ZUse)
            {
                if (setAux1ZMode == 0) { distValue *= Gcode.GcodeZDown * setAux1ZFactor; }
                else if (setAux1ZMode == 1) { distValue *= Math.Abs(Gcode.GcodeZDown) * setAux1ZFactor; }                                   // +/-Z but only +result
                else if ((setAux1ZMode == 2) && (Gcode.GcodeZDown >= 0)) { distValue *= Math.Abs(Gcode.GcodeZDown) * setAux1ZFactor; }  // +Z only
                else if ((setAux1ZMode == 3) && (Gcode.GcodeZDown <= 0)) { distValue *= Math.Abs(Gcode.GcodeZDown) * setAux1ZFactor; }  // -Z only
            }
            if (Properties.Settings.Default.importGCAux1SumUp)
                setAux1FinalDistance += distValue;
            else
                setAux1FinalDistance = distValue;
        }

        private static void CalculateAux2(GCodeMotion entity)
        {
            int setAux2ZMode = Properties.Settings.Default.importGCAux2ZMode;
            double setAux2ZFactor = (double)Properties.Settings.Default.importGCAux2ZFactor;
            double distValue = entity.Depth * (double)Properties.Settings.Default.importGCAux2Factor;
            if (Properties.Settings.Default.importGCAux2ZUse)
            {
                if (setAux2ZMode == 0) { distValue *= Gcode.GcodeZDown * setAux2ZFactor; }
                else if (setAux2ZMode == 1) { distValue *= Math.Abs(Gcode.GcodeZDown) * setAux2ZFactor; }                                   // +/-Z but only +result
                else if ((setAux2ZMode == 2) && (Gcode.GcodeZDown >= 0)) { distValue *= Math.Abs(Gcode.GcodeZDown) * setAux2ZFactor; }  // +Z only
                else if ((setAux2ZMode == 3) && (Gcode.GcodeZDown <= 0)) { distValue *= Math.Abs(Gcode.GcodeZDown) * setAux2ZFactor; }  // -Z only
            }
            if (Properties.Settings.Default.importGCAux2SumUp)
                setAux2FinalDistance += distValue;
            else
                setAux2FinalDistance = distValue;
        }

        private static void SetHalftoneMode(Graphic.GraphicInformationClass graphicInfo)
        {
            if (graphicInfo.OptionZFromWidth)
            {
                double zMin = Math.Abs((double)Properties.Settings.Default.importDepthFromWidthMin);
                double zMax = Math.Abs((double)Properties.Settings.Default.importDepthFromWidthMax);
                Gcode.Comment(gcodeString, string.Format("{0} Min=\"{1}\" Max=\"{2}\" Width=\"{3}\" />", XmlMarker.HalftoneZ, zMin, zMax, zMax));
            }
            else if (graphicInfo.OptionSFromWidth)
            {
                double zMin = Math.Abs((double)Properties.Settings.Default.importImageSMin);
                double zMax = Math.Abs((double)Properties.Settings.Default.importImageSMax);
                double width = Math.Abs((double)Properties.Settings.Default.importDepthFromWidthMax);
                Gcode.Comment(gcodeString, string.Format("{0} Min=\"{1}\" Max=\"{2}\" Width=\"{3}\" />", XmlMarker.HalftoneS, zMin, zMax, width));
            }
        }

        /*Input: min, max and actual pen-width value from graphics import*/
        public static double CalculateZFromRange(double min, double max, double penWidth)
        {
            if (logDetailed)
                Logger.Trace("----calculateZFromRange: min:{0:0.00}  max: {1:0.00}  input: {2:0.00}", min, max, penWidth);
            if (penWidth == 0)
                return (double)Properties.Settings.Default.importDepthFromWidthMin;

            double penMin = Math.Abs(min);
            double penMax = Math.Abs(max);
            double penDelta = (penMax - penMin);

            if (penDelta == 0)
                return (double)Properties.Settings.Default.importDepthFromWidthMin;
            double nPen = (Math.Abs(penWidth) - penMin) / penDelta;

            /*Get desired range, where to transform the pen-width*/
            double zMin = Math.Abs((double)Properties.Settings.Default.importDepthFromWidthMin);
            double zMax = Math.Abs((double)Properties.Settings.Default.importDepthFromWidthMax);
            double zDelta = (zMax - zMin);

            if (zDelta == 0)
                return (double)Properties.Settings.Default.importDepthFromWidthMin;

            double z;
            if (zDelta > 0)
                z = nPen * zDelta + Math.Min(zMin, zMax);       // min to min, max to max
            else
                z = (penDelta - nPen) * Math.Abs(zDelta) + Math.Min(zMin, zMax);                // min to max, max to min

            if (logDetailed)
                Logger.Trace("---calculateZFromPenWidth: penWidth:{0:0.00}  nPen:{1:0.00}  zDelta:{2:0.00}   z:{3:0.00}", penWidth, nPen, zDelta, z);

            return -z;
        }

        /*Input: min, max and actual pen-width value from graphics import*/
        public static double CalculateSFromRange(double min, double max, double penWidth)
        {
            if (logDetailed)
                Logger.Trace("----calculateSFromRange: min:{0:0.00}  max: {1:0.00}  input: {2:0.00}", min, max, penWidth);
            if (penWidth == 0)
                return (double)Properties.Settings.Default.importImageSMin;

            double penMin = Math.Abs(min);
            double penMax = Math.Abs(max);
            double penDelta = Math.Abs(penMax - penMin);

            if (penDelta == 0)
                return (double)Properties.Settings.Default.importImageSMin;
            double nPen = (Math.Abs(penWidth) - penMin) / penDelta;

            /*Get desired range, where to transform the pen-width*/
            double zMin = Math.Abs((double)Properties.Settings.Default.importImageSMin);
            double zMax = Math.Abs((double)Properties.Settings.Default.importImageSMax);
            double zDelta = (zMax - zMin);

            if (zDelta == 0)
                return (double)Properties.Settings.Default.importImageSMin;

            double z;
            if (zDelta > 0)
                z = nPen * zDelta + Math.Min(zMin, zMax);       // min to min, max to max
            else
                z = (penDelta - nPen) * Math.Abs(zDelta) + Math.Min(zMin, zMax);                // min to max, max to min

            if (logDetailed)
                Logger.Trace("---calculateSFromPenWidth: penWidth:{0:0.00}  nPen:{1:0.00}  SDelta:{2:0.00}   S:{3:0.00}", penWidth, nPen, zDelta, z);

            return Math.Abs(z);
        }

        private static string GetGroupAttributes(GroupObject groupObject, GraphicInformationClass graphicInfo)
        {
            string[] groupAttribute = new string[] { "none=", "PenColor=", "PenWidth=", "Layer=", "Type=", "Tile=" };       // check public enum GroupOptions
            string groupVal1 = string.Format(" {0}\"{1}\"", groupAttribute[(int)graphicInfo.GroupOption], groupObject.Key);
            string groupVal2 = "";// string.Format(culture, " Type=\"{0}\"", groupObject.Type);
            string groupVal3 = string.Format(" ToolNr=\"{0}\"", groupObject.ToolNr);
            string groupVal4 = string.Format(" ToolName=\"{0}\"", groupObject.ToolName);
            string groupVal5 = string.Format(" PathLength=\"{0:0.0}\"", groupObject.PathLength);
            string groupVal6 = string.Format(" PathArea=\"{0:0.0}\"", groupObject.PathArea);
            return string.Format("{0}{1}{2}{3}{4}{5}", groupVal1, groupVal2, groupVal3, groupVal4, groupVal5, groupVal6);
        }
        private static StringBuilder GetFigureAttributes(PathObject pathObject)
        {
            StringBuilder attributes = new StringBuilder();

            if (pathObject.Info.PathGeometry.Length > 0) attributes.Append(string.Format(" Geometry=\"{0}\"", pathObject.Info.PathGeometry));
            if (pathObject.Info.GroupAttributes[(int)GroupOption.ByColor].Length > 0) attributes.Append(string.Format(" PenColor=\"{0}\"", pathObject.Info.GroupAttributes[(int)GroupOption.ByColor]));
            if (pathObject.Info.GroupAttributes[(int)GroupOption.ByWidth].Length > 0) attributes.Append(string.Format(" PenWidth=\"{0}\"", pathObject.Info.GroupAttributes[(int)GroupOption.ByWidth]));

            string layerLabel = "";
            if (pathObject.Info.GroupAttributes[(int)GroupOption.Label].Length > 0) layerLabel = string.Format("-{0}", pathObject.Info.GroupAttributes[(int)GroupOption.Label]);
            if (pathObject.Info.GroupAttributes[(int)GroupOption.ByLayer].Length > 0) attributes.Append(string.Format(" Layer=\"{0}{1}\"", pathObject.Info.GroupAttributes[(int)GroupOption.ByLayer], layerLabel));

            if (pathObject.Info.GroupAttributes[(int)GroupOption.ByType].Length > 0) attributes.Append(string.Format(" Type=\"{0}\"", pathObject.Info.GroupAttributes[(int)GroupOption.ByType]));
            if (pathObject.Info.GroupAttributes[(int)GroupOption.ByTile].Length > 0) attributes.Append(string.Format(" Tile=\"{0}\"", pathObject.Info.GroupAttributes[(int)GroupOption.ByTile]));
            if (pathObject.Info.PathId.Length > 0) attributes.Append(string.Format(" PathId=\"{0}\"", pathObject.Info.PathId));
            if (pathObject.PathLength > 0) attributes.Append(string.Format(" PathLength=\"{0:0.0}\"", pathObject.PathLength));

            return attributes;
        }


        /// <summary>
        /// Set start tag, move to beginning of path via G0, finish old path
        /// </summary>
        private static PathInformation pathInfo = new PathInformation();
        private static bool overWriteId = false;
        private static int StartPath(PathObject pathObject, int toolNr, string toolCmt, string penCmt = "")//string cmt)
        {
            Point startRamp = pathObject.Start;
            Point startPenDown = new Point(startRamp.X, startRamp.Y);
            double angle = pathObject.StartAngle;
            int iDToSet = PathCount;
            if (overWriteId && (pathObject.FigureId > 0))
                iDToSet = pathObject.FigureId;

            if (Properties.Settings.Default.importLineDashPattern && (PathDashArray != null) && (PathDashArray.Length > 1))
            {
                string dash = "";
                for (int k = 0; k < PathDashArray.Length; k++)
                { dash += string.Format("{0}:{1:0.000}; ", k, PathDashArray[k]); }

                if (logCoordinates) Logger.Trace("  StartPath dash pattern: {0}", dash);
            }

            PenUp("StartPath");   // Don't set xmlMarker.figureEnd
            PathDashArrayIndex = 0;
            PathDashArrayDistance = 0;
            PathDashArrayIndexChanged = false;
            PathDashArrayPenIsUp = true;

            if (!pathInfo.IsSameAs(pathObject.Info) || FigureEndTagWasSet)	// IsSameAs Id, PenColorId, PathId, AuxInfo, PathGeometry, GroupAttributes
            {
                if (!FigureEndTagWasSet)
                { SetFigureEndTag(); }
                FigureEndTagWasSet = true;

                iDToSet = ++PathCount;
                if (overWriteId && (pathObject.FigureId > 0))
                    iDToSet = pathObject.FigureId;

                string xml = "no xml";
                if (figureEnable)
                {
                    xml = string.Format("{0} Id=\"{1}\"{2}> ", XmlMarker.FigureStart, iDToSet, GetFigureAttributes(pathObject).ToString());//attributeGeometry, attributeId, attributeColor, attributeToolNr);                
                    Comment(xml);
                }
                if (logCoordinates) Logger.Trace(" StartPath Option:{0}  {1}", pathObject.Options, xml);
                FigureEndTagWasSet = false;

                pathInfo = pathObject.Info.Copy();

                if ((pathObject.Options & CreationOption.AddPause) > 0)
                    Gcode.Pause(gcodeString, "StartPath");
                if (pauseBeforePath) { Gcode.Pause(gcodeString, "Pause before path"); }

                if (toolNr >= 0)
                    ToolChange(toolNr, 1, toolCmt);   // add tool change commands (if enabled) and set XYFeed etc.
            }

            double setangle = 180 * angle / Math.PI;
            Gcode.SetTangential(gcodeString, setangle, false);

            /* create ramp on pen-down AND pen-up
			---p0\            /p3----
				  \p1------p2/
			without ramp: p1 pen-down,  p2 pen-up
			with  leadin :  p0 ramp to p1
			with  leadout:  p2 ramp to p3
			*/
            if (Properties.Settings.Default.importGraphicLeadInEnable)
            {
                if (Properties.Settings.Default.importGCZEnable)
                {      // Z movement
                    double leadIn = (double)Properties.Settings.Default.importGraphicLeadInDistance;

                    if (true)		//Properties.Settings.Default.importGraphicLeadTopZUp)        //startAtPenUp)
                    {
                        leadIn = (Math.Abs(Gcode.GcodeZDown) + Math.Abs(Gcode.GcodeZUp)) * leadIn / Math.Abs(Gcode.GcodeZDown);   // extend distance
                    }
                    startRamp.X += leadIn * Math.Cos(angle + Math.PI);
                    startRamp.Y += leadIn * Math.Sin(angle + Math.PI);

                    Gcode.MoveToRapid(gcodeString, startRamp, ""); 	// move to start of ramp

                    Gcode.SetZStartPos(startPenDown);           	// set pos where Z is completly down
                    PenDown(penCmt + " ramp");                     	// will do XYZ move
                    Gcode.ClearLeadIn();
                }
                else if (Properties.Settings.Default.importGCPWMEnable)
                {   // PWM in steps
                    double steps;
                    double leadIn = (double)Properties.Settings.Default.importGraphicLeadInDistance;
                    double pwmUp = (double)Properties.Settings.Default.importGCPWMZero;
                    double pwmDelta = pwmUp - (double)Properties.Settings.Default.importGCPWMDown;
                    double factorX = Math.Cos(angle + Math.PI);
                    double factorY = Math.Sin(angle + Math.PI);
                    startRamp.X += factorX * leadIn;
                    startRamp.Y += factorY * leadIn;
                    Gcode.MoveToRapid(gcodeString, startRamp, "");
                    double pwmDown;

                    steps = Math.Abs(pwmDelta / 10);
                    double pwmDownDlyMax = (double)Properties.Settings.Default.importGCPWMDlyDown;
                    //double partDly = pwmDownDlyMax / steps;
                    Gcode.ApplyXYFeedRate = true;
                    for (int stp = 0; stp < steps; stp++)     // create ramp step by step
                    {
                        startPenDown.X = startRamp.X - factorX * stp * leadIn / steps;
                        startPenDown.Y = startRamp.Y - factorY * stp * leadIn / steps;
                        Gcode.MoveTo(gcodeString, startPenDown, "");

                        pwmDown = pwmUp - stp * pwmDelta / steps;
                        gcodeString.AppendFormat("M{0} S{1}\r\n", Gcode.GcodeSpindleCmd, (int)pwmDown);
                        //                    if (pwmDownDlyMax > 0)
                        //                         gcodeString.AppendFormat("G{0} P{1}\r\n", gcode.frmtCode(4), gcode.frmtNum(partDly));
                    }
                    Gcode.GcodePwmDlyDown = 0;// (float)partDly;
                    Gcode.MoveTo(gcodeString, pathObject.Start, "");
                    PenDown(penCmt);    // will do final PenDown to track state
                    Gcode.GcodePwmDlyDown = (float)pwmDownDlyMax;
                }
            }
            else
            {
                Gcode.MoveToRapid(gcodeString, startRamp, "");
                PenDown(penCmt);
            }
            if (logCoordinates) Logger.Trace("  StartPath at x{0:0.000} y{1:0.000} a={2:0.00}", startRamp.X, startRamp.Y, setangle);

            if (Properties.Settings.Default.importGraphicLeadInEnable)		// importGraphicLeadOutEnable)
            {
                if (Properties.Settings.Default.importGCZEnable)
                {
                    Point endRamp = pathObject.End;
                    if (pathObject is ItemPath pathObjectPath)
                    {
                        if (pathObjectPath.Path.Count >= 2)
                        {
                            angle = GcodeMath.GetAlpha(pathObjectPath.Path[pathObjectPath.Path.Count - 2].MoveTo, pathObjectPath.Path[pathObjectPath.Path.Count - 1].MoveTo);
                            double leadOut = (double)Properties.Settings.Default.importGraphicLeadInDistance;       // importGraphicLeadOutDistance;
                            leadOut = (Math.Abs(Gcode.GcodeZDown) + Math.Abs(Gcode.GcodeZUp)) * leadOut / Math.Abs(Gcode.GcodeZDown);  // extend distance
                            endRamp.X -= leadOut * Math.Cos(angle + Math.PI);   //offsetX;
                            endRamp.Y -= leadOut * Math.Sin(angle + Math.PI);   //offsetY;
                            Gcode.SetZEndPos(endRamp);           // set pos where Z is completly up
                        }
                    }
                }
            }

            lastGC = startPenDown;  // startRamp;
            return iDToSet;	//PathCount;
        }

        private static void SetFigureEndTag()
        {
            if (figureEnable)
            {
                string xml = string.Format("{0}>", XmlMarker.FigureEnd);//, nr);    //string.Format("{0} nr=\"{1}\" >", xmlMarker.figureEnd, nr);
                Comment(xml);
                if (logCoordinates) Logger.Trace(" {0}", xml);
            }
        }

        /// <summary>
        /// Finish path
        /// </summary>
        private static void StopPath(string cmt)
        {
            if (Properties.Settings.Default.importGraphicLeadInEnable) // importGraphicLeadOutEnable)
                cmt += " ramp";
            if (logCoordinates) Logger.Trace("  StopPath {0}", cmt);
            PenUp(cmt);
            Gcode.ClearLeadOut();
        }

        /// <summary>
        /// Move to next coordinate
        /// </summary>
        private static void MoveTo(Point coordxy, double newZ, double? newS, double tangAngle, string cmt)
        {
            bool applyDashPattern = Properties.Settings.Default.importLineDashPattern && (PathDashArray != null) && (PathDashArray.Length > 1);

            if (!useAlternitveZ && !applyDashPattern)    //Properties.Settings.Default.importDepthFromWidthRamp)
                PenDown(cmt);   //  + " moveto"                      // also process tangetial axis
            double setangle = 180 * tangAngle / Math.PI;

            if (logCoordinates) Logger.Trace(" MoveTo X{0:0.000} Y{1:0.000} A{2:0.00}  useAlternitveZ:{3}  applyDashPattern:{4}", coordxy.X, coordxy.Y, setangle, useAlternitveZ, applyDashPattern);
            Gcode.SetTangential(gcodeString, setangle, true);
            Gcode.SetAux1DistanceCommand(setAux1FinalDistance);		// Create command-snipped, to be added in G1 command (Graphic2GCodeMove.cs - Move)
            Gcode.SetAux2DistanceCommand(setAux2FinalDistance);
            if (newS != null) Gcode.SetSValue((double)newS);

            //Logger.Trace("MoveTo useAlternitveZ:{0}  newZ:{1}", useAlternitveZ, newZ);
            if (useAlternitveZ) 		//Properties.Settings.Default.importDepthFromWidthRamp|| Properties.Settings.Default.importDXFUseZ)
                Gcode.Move(gcodeString, 1, coordxy.X, coordxy.Y, (float)newZ, Gcode.ApplyXYFeedRate, cmt);
            else if (applyDashPattern)
                MoveToDashed(coordxy);
            else
                Gcode.MoveTo(gcodeString, coordxy, cmt);    // note: Gcode.GcodeZDown is may set with newZ

            lastGC = coordxy;
        }



        private static void MoveToDashed(Point coordxy)
        {
            if (logCoordinates) Logger.Trace("  MoveToDashed from X:{0:0.000} Y:{1:0.000}  to X:{0:0.000} Y:{1:0.000}", lastGC.X, lastGC.Y, coordxy.X, coordxy.Y);

            if (logCoordinates) Logger.Trace("  MoveToDashed enabled:{0} length:{1}", Properties.Settings.Default.importLineDashPattern, PathDashArray.Length);

            bool penUpG1 = !Properties.Settings.Default.importLineDashPatternG0;
            double dX = coordxy.X - lastGC.X;           // full distance
            double dY = coordxy.Y - lastGC.Y;
            double dFull = Math.Sqrt(dX * dX + dY * dY);
            double dToGo = dFull;
            double xx = lastGC.X, yy = lastGC.Y;        // intermediate pos
            double ddx, ddy;                    // dash distance
            string dashInfo = "";
            Point pNext;

            while (dToGo > 0)
            {
                if (logCoordinates) Logger.Trace("  -0 dToGo:{0:0.000}  PathDashArrayIndex:{1}  PathDashArrayDistance:{2:0.000}   PathDashArrayPenIsUp:{3}", dToGo, PathDashArrayIndex, PathDashArrayDistance, PathDashArrayPenIsUp);

                if (PathDashArrayDistance <= 0)         // get distance
                {
                    if (PathDashArrayIndex >= PathDashArray.Length) PathDashArrayIndex = 0;
                    PathDashArrayDistance = Math.Abs(PathDashArray[PathDashArrayIndex]);        // only allow positive distance

                    if (logCoordinates) Logger.Trace("   1 Load PathDashArrayIndex:{0}  PathDashArrayDistance:{1:0.00}", PathDashArrayIndex, PathDashArrayDistance);

                    PathDashArrayIndex++;
                    PathDashArrayIndexChanged = true;
                }

                if (PathDashArrayIndexChanged)
                {
                    if ((PathDashArrayIndex % 2) == 0)      // pen-up/ down depending on dash index
                    {
                        if (logCoordinates) Logger.Trace("   1 PenUp");
                        PenUp("MoveToDashed"); PathDashArrayPenIsUp = true;
                    }
                    else
                    {
                        if (logCoordinates) Logger.Trace("   1 PenDown");
                        PenDown("MoveToDashed"); PathDashArrayPenIsUp = false;
                    }
                    PathDashArrayIndexChanged = false;
                }
                if (logCoordinates) Logger.Trace("   2 dToGo:{0:0.000}  PathDashArrayDistance:{1:0.000}   PathDashArrayPenIsUp:{2}", dToGo, PathDashArrayDistance, PathDashArrayPenIsUp);

                if (dToGo <= PathDashArrayDistance)     // no dash-change in this move
                {
                    //dashInfo = String.Format("dash match dToGo:{0:0.000}  array:{1:0.000}", dToGo, PathDashArrayDistance);
                    if (gcodeComments) dashInfo = String.Format("{0:0.000}", PathDashArrayDistance);
                    PathDashArrayDistance -= dToGo;
                    dToGo = 0;
                    pNext = coordxy;
                }
                else                                    // do inermediate step
                {
                    dToGo -= PathDashArrayDistance;
                    ddx = PathDashArrayDistance * dX / dFull;
                    ddy = PathDashArrayDistance * dY / dFull;
                    xx += ddx;                          // new pos.
                    yy += ddy;
                    //dashInfo = PathDashArrayDistance.ToString();
                    if (gcodeComments) dashInfo = String.Format("{0:0.000}", PathDashArrayDistance);
                    PathDashArrayDistance = 0;
                    pNext = new Point(xx, yy);
                }

                if (PathDashArrayPenIsUp)
                {
                    if (gcodeComments) dashInfo = "pen-up dash:" + dashInfo;
                    if (logCoordinates) Logger.Trace("   3 MoveTo PenUp x:{0:0.00}  y:{1:0.00}  dToGo:{2:0.000}  PathDashArrayDistance:{3:0.000}", pNext.X, pNext.Y, dToGo, PathDashArrayDistance);
                    if (penUpG1) Gcode.MoveToNoFeed(gcodeString, pNext, dashInfo);
                    else Gcode.MoveToRapid(gcodeString, pNext, dashInfo);
                }
                else
                {
                    if (gcodeComments) dashInfo = "pen-down dash:" + dashInfo;
                    if (logCoordinates) Logger.Trace("   3 MoveTo PenDown x:{0:0.00}  y:{1:0.00}  dToGo:{2:0.000}  PathDashArrayDistance:{3:0.000}", pNext.X, pNext.Y, dToGo, PathDashArrayDistance);
                    Gcode.MoveTo(gcodeString, pNext, dashInfo);
                }
            }
        }

        private static void Arc(bool isG2, Point endPos, Point centerPos, double tangStart, double tangEnd)//, string cmt) remove newZ
        {
            int gnr = 2; if (!isG2) { gnr = 3; }
            Arc(gnr, endPos.X, endPos.Y, centerPos.X, centerPos.Y, tangStart, tangEnd, "");
        }

        private static void Arc(int gnr, double x, double y, double i, double j, double tangStartRad, double tangEndRad, string cmt = "")// remove newZ 2021-08-09
        {
            Point coordxy = new Point(x, y);
            Point center = new Point(lastGC.X + i, lastGC.Y + j);
            double offset = +Math.PI / 2;
            if (logCoordinates) Logger.Trace("  Start Arc G{0} X{1:0.000} Y{2:0.000} angle:{3:0.000}    cx{4:0.000} cy{5:0.000} angle:{6:0.000}", gnr, x, y, (180 * tangStartRad / Math.PI), center.X, center.Y, (180 * tangEndRad / Math.PI));
            if (gnr > 2) { offset = -offset; }

            Gcode.SetTangential(gcodeString, 180 * tangStartRad / Math.PI, true);
            Gcode.SetAux1DistanceCommand(setAux1FinalDistance);
            Gcode.SetAux2DistanceCommand(setAux2FinalDistance);

            if (logCoordinates) Logger.Trace("   Start Arc alpha{0:0.000} offset{1:0.000}  ", 180 * tangStartRad / Math.PI, 180 * offset / Math.PI);

            PenDown(cmt + " from Arc");

            if (GcodeMath.IsEqual(coordxy, lastGC))				// end = start position? Full circle!
            {
                if (gnr > 2)
                    tangEndRad += 2 * Math.PI + tangStartRad;        			// CW 360°
                else
                    tangEndRad -= 2 * Math.PI + tangStartRad;        			// CCW 360°
            }
            Gcode.SetTangential(gcodeString, 180 * tangEndRad / Math.PI, false);
            Gcode.Arc(gcodeString, gnr, x, y, i, j, cmt);

            lastGC = coordxy;
        }

        /// <summary>
        /// add header and footer, return string of gcode
        /// </summary>
        private static bool FinalGCode(string titel, string file)
        {
            Logger.Trace("△△△Graphic2GCode - FinalGCode() ");
            StringBuilder header = new StringBuilder();
            StringBuilder footer = new StringBuilder(Gcode.GetFooter());
            StringBuilder output = new StringBuilder();

            header.AppendFormat("( Use case: {0} )\r\n", Properties.Settings.Default.useCaseLastLoaded);
            header.Append(Gcode.GetHeader(titel, file));

            if (Properties.Settings.Default.importRepeatEnable && Properties.Settings.Default.importRepeatComplete)      // repeat code x times
            {
                if (Properties.Settings.Default.importRepeatEnableAll)
                {
                    Logger.Trace("FinalGCode() importRepeatEnableAll {0}", Properties.Settings.Default.importRepeatCnt);
                    header.Append(finalGcodeString);
                    header.Append(footer);
                    for (int i = 0; i < Properties.Settings.Default.importRepeatCnt; i++)
                    { output.AppendFormat("(----- Repeate All {0} of {1} ---------)\r\n", (i + 1), Properties.Settings.Default.importRepeatCnt); output.Append(header); output.AppendLine("(----------------------------------)\r\n"); }
                    header = output;
                }
                else
                {
                    Logger.Trace("FinalGCode() !importRepeatEnableAll {0}", Properties.Settings.Default.importRepeatCnt);
                    for (int i = 0; i < Properties.Settings.Default.importRepeatCnt; i++)
                    { output.AppendFormat("(----- Repeate code {0} of {1} --------)\r\n", (i + 1), Properties.Settings.Default.importRepeatCnt); output.Append(finalGcodeString); output.AppendLine("(----------------------------------)\r\n"); }
                    header.Append(output);
                    header.Append(footer);
                }
            }
            else
            {
                header.Append(finalGcodeString);
                header.Append(footer);
            }
            //header.AppendLine("M30"); // 2021-05-07 reomved

            if (Properties.Settings.Default.ctrlLineNumbers || Properties.Settings.Default.ctrlLineEndEnable)
            {
                int n = 1;
                string[] lines = header.ToString().Split('\n');
                string end = "";
                if (Properties.Settings.Default.ctrlLineEndEnable)
                    end = Properties.Settings.Default.ctrlLineEndText;
                bool addNr = Properties.Settings.Default.ctrlLineNumbers;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (addNr)
                        lines[i] = string.Format("N{0} {1}{2}", n++, lines[i].Trim(), end);
                    else
                        lines[i] = string.Format("{0}{1}", lines[i].Trim(), end);
                }
                header.Clear();
                header.Append(String.Join("\n", lines));
            }
            Graphic.GCode = header.Replace(',', '.');
            return true;
        }

        /// <summary>
        /// Insert Pen-up gcode command
        /// </summary>
        private static bool PenUp(string cmt = "", bool setEndFigureTag = false)
        {
            if (logCoordinates) Logger.Trace("   PenUp penIsDown:{0}  cmt:{1}", penIsDown, cmt);

            if (!comments && !cmt.Contains("PU"))
                cmt = "";
            bool penWasDown = penIsDown;
            if (penIsDown)
            { Gcode.PenUp(gcodeString, cmt); }
            penIsDown = false;

            if (setEndFigureTag)
            {
                if ((PathCount > 0) && !Graphic2GCode.FigureEndTagWasSet)
                {
                    SetFigureEndTag();
                    FigureEndTagWasSet = true;
                }
            }
            return penWasDown;
        }

        private static void PenDown(string cmt)
        {
            if (logCoordinates) Logger.Trace("   PenDown penIsDown:{0}  cmt:{1}", penIsDown, cmt);
            if (!comments && !cmt.Contains("PD"))
                cmt = "";

            if (!penIsDown)
            {
                if (pauseBeforePenDown) { Gcode.Pause(gcodeString, "Pause before pen down"); }
                Gcode.PenDown(gcodeString, cmt);
            }
            penIsDown = true;
        }

        /// <summary>
        /// Insert tool change command
        /// </summary>
        private static void ToolChange(int toolnr, int objectCount, string cmt = "")
        { Gcode.Tool(gcodeString, toolnr, objectCount, cmt); }  // add tool change commands (if enabled) and set XYFeed etc.


        /// <summary>
        /// set comment
        /// </summary>
        private static void Comment(string cmt)
        { Gcode.Comment(gcodeString, cmt); }

    }

}
