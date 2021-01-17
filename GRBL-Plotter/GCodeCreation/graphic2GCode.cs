/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2021 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using static GRBL_Plotter.Graphic;

namespace GRBL_Plotter
{
    public static class Graphic2GCode
    {
        private static uint logFlags = 0;
        private const int gcodeStringMax = 260;                 // max amount of tools
//        private static int gcodeStringIndex = 0;                // index for stringBuilder-Array

        private static StringBuilder gcodeString = new StringBuilder();
        private static StringBuilder finalGcodeString = new StringBuilder();

        public static bool penIsDown = false;
        private static bool comments = false;
        private static bool pauseBeforePath = false;
        private static bool pauseBeforePenDown = false;

        private static bool figureEnable = true;

        private static Point lastGC;//, lastSetGC;             // store last position

        public static int PathCount { get; set; } = 0;
        public static bool FigureEndTagWasSet { get; set; } = true;
        public static double[] PathDashArray { get; set; } = { };

        private static bool useIndividualZ = false;


        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool logEnable;
        private static bool logDetailed;
        private static bool logCoordinates;

        public static void CleanUp()
		{	Logger.Trace("CleanUp()");
			gcodeString.Clear();
            gcodeString.Length = 0;
            finalGcodeString.Clear();
		}

        public static void Init()
        {   logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnable.Level3) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnable.Detailed) > 0);
            logCoordinates = logEnable && ((logFlags & (uint)LogEnable.Coordinates) > 0);

            Logger.Trace("Init Graphic2GCode   loggerTrace:{0}", Convert.ToString(logFlags, 2));

            pauseBeforePath = Properties.Settings.Default.importPauseElement;
            pauseBeforePenDown = Properties.Settings.Default.importPausePenDown;
            comments = Properties.Settings.Default.importSVGAddComments;
            penIsDown = false;
			
			gcodeString.Clear();
            finalGcodeString.Clear();

            PathCount = 0;
            FigureEndTagWasSet  = true;
            gcode.setup();                              // initialize GCode creation (get stored settings for export)
			pathInfo = new PathInformation();
        }

        /// <summary>
        /// Create GCode from tiles, no further sorting needed.
        /// </summary>		
        private static int mainGroupID = 0;
        public static bool CreateGCode(List<Graphic.TileObject> tiledGraphic, List<string> headerInfo, Graphic.GraphicInformation graphicInfo)    
        {   string xmlTag = "";
            int iDToSet = 1;
            mainGroupID = 0;
            
			if (logEnable) Logger.Trace("-CreateGCode from Tiles");
            
            Init();                                         // initalize variables, toolTable.init(), gcode.setup()
            foreach (string info in headerInfo)		        // set header info
            {   gcode.AddToHeader(info); }
            gcode.jobStart(finalGcodeString, "StartJob");

            foreach (TileObject tileObject in tiledGraphic)
            {   
                xmlTag = string.Format("{0} Id=\"{1}\" Pos=\"{2}\">", xmlMarker.tileStart, iDToSet, tileObject.key);
				gcode.Comment(finalGcodeString, xmlTag);
                if (logEnable) Logger.Trace("-CreateGCode {0} ", xmlTag);

                if (tileObject.tileRelatedGCode != "")
                {   string[] commands= { };
                    commands = tileObject.tileRelatedGCode.Split(';'); 
                    foreach (string cmd in commands)
                    {   
                        finalGcodeString.AppendLine(cmd);
                    }
                }
                
                if (graphicInfo.GroupEnable)
                    CreateGCode(tileObject.tile, headerInfo, graphicInfo, true);        // create grouped code
                else
                    CreateGCode(tileObject.groupPath, headerInfo, graphicInfo, true);   // create path code
                
                gcode.Comment(finalGcodeString, xmlMarker.tileEnd + ">"); 
                iDToSet++;                
            }      
            
            gcode.jobEnd(finalGcodeString, "EndJob");       // Spindle / laser off
            
            return FinalGCode(graphicInfo.Title, graphicInfo.FilePath);
        }

        /// <summary>
        /// Create GCode from already sorted groups, no further sorting needed.
        /// </summary>		
        public static bool CreateGCode(List<Graphic.GroupObject> completeGraphic, List<string> headerInfo, Graphic.GraphicInformation graphicInfo, bool useTiles = false)    
        {
//            Init();                                 // initalize variables, toolTable.init(), gcode.setup()
			overWriteId = graphicInfo.ReProcess;	// keep IDs from previous conversion
            useIndividualZ = graphicInfo.OptionZFromWidth;
		
			if (logEnable) Logger.Trace("-CreateGCode from Groups");
            
            if (!useTiles)
            {   Init();                                 // initalize variables, toolTable.init(), gcode.setup()
                foreach (string info in headerInfo)		// set header info
                {   gcode.AddToHeader(info); }
                gcode.jobStart(finalGcodeString, "StartJob");
                mainGroupID = 0;
            }

            int groupID = mainGroupID;
			int iDToSet = 0;
			string groupAttributes;

            foreach (GroupObject groupObject in completeGraphic)
            {	groupAttributes = getGroupAttributes(groupObject, graphicInfo);
				
				groupID++;			
				iDToSet = groupID;	
				
				if (overWriteId && (groupObject.GroupId > 0))
					iDToSet = groupObject.GroupId;

                groupObject.GroupId = iDToSet;	// track id
				gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\"{2}>", xmlMarker.groupStart, iDToSet, groupAttributes));
                if (logEnable) Logger.Trace("-CreateGCode {0} Id=\"{1}\"{2}>", xmlMarker.groupStart, iDToSet, groupAttributes);

                if (logEnable) Logger.Trace("CreateGCode-Group  toolNr:{0}  name:{1}", groupObject.toolNr, groupObject.toolName);
                ToolChange(groupObject.toolNr, groupObject.toolName);   // add tool change commands (if enabled) and set XYFeed etc.

                foreach (PathObject pathObject in groupObject.groupPath)
                {
                    if (logEnable) Logger.Trace(" ProcessPathObject id:{0} ", pathObject.Info.id);
                    ProcessPathObject(pathObject, graphicInfo, -1, "");	// create Dot or Path GCode, but no tool change
                }
                PenUp(" CreateGCode 1", true);      // set xmlMarker.figureEnd

                finalGcodeString.Append(gcodeString);
                gcodeString.Clear();                            // don't add gcode a 2nd time

                gcode.Comment(finalGcodeString, xmlMarker.groupEnd + ">"); 
                if (logEnable) Logger.Trace("-CreateGCode {0} >", xmlMarker.groupEnd);
            }
            mainGroupID = groupID;
            if (!useTiles)
            {   gcode.jobEnd(finalGcodeString, "EndJob");       // Spindle / laser off
                return FinalGCode(graphicInfo.Title, graphicInfo.FilePath);
            }
            else
                return true;        // go on with next tile
        }

        /// <summary>
        /// Create GCode from already sorted paths, no further sorting needed.
        /// </summary>		
        public static bool CreateGCode(List<Graphic.PathObject> completeGraphic, List<string> headerInfo, Graphic.GraphicInformation graphicInfo, bool useTiles = false)    
        {
//            Init();        							// initalize variables, toolTable.init(), gcode.setup()
			overWriteId = graphicInfo.ReProcess;	// keep IDs from previous conversion
			useIndividualZ = graphicInfo.OptionZFromWidth;

			if (logEnable) Logger.Trace("-CreateGCode from paths");

            if (!useTiles)
            {   Init();                                 // initalize variables, toolTable.init(), gcode.setup()
                foreach (string info in headerInfo)		// set header info
                {   gcode.AddToHeader(info); }
            }

            int toolNr = 1;
            string toolName = "";
            string toolColor = "";
            foreach (PathObject pathObject in completeGraphic)		// go through all graphics elements
            {				
// get tool-nr by color or use color-id		 
                if (Properties.Settings.Default.importDXFToolIndex)
                {   toolNr = pathObject.Info.penColorId + 1; }     // avoid ID=0 to start tool-table with index 1
                else
                {   toolColor = pathObject.Info.groupAttributes[(int)GroupOptions.ByColor];
                    toolNr = toolTable.getToolNrByToolColor(toolColor, 0);
                }

                // real tool to use: default or from graphic	   
                int toolToUse = toolNr;
                if (Properties.Settings.Default.importGCToolTableUse && Properties.Settings.Default.importGCToolDefNrUse)
                    toolToUse = (int)Properties.Settings.Default.importGCToolDefNr;

		        toolName = toolTable.getToolName(toolToUse);
                toolColor = toolTable.getToolColor(toolNr);

                if (logEnable) Logger.Trace("CreateGCode2  toolNr:{0}  name:{1}",toolToUse,toolName);
// add tool change after <Figure tag	   
				ProcessPathObject(pathObject, graphicInfo, toolToUse, toolName + " = " + toolColor);	// create Dot or Path GCode
            }
            PenUp(" CreateGCode 2", true);    // set xmlMarker.figureEnd

            if (!useTiles)
            {
                gcode.jobStart(finalGcodeString, "StartJob");
                finalGcodeString.Append(gcodeString);
                gcode.jobEnd(finalGcodeString, "EndJob");      // Spindle / laser off
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
		private static void ProcessPathObject(PathObject pathObject, Graphic.GraphicInformation graphicInfo, int toolNr, string toolCmt)
		{
            if (logDetailed) Logger.Trace("ProcessPathObject start");
            figureEnable = graphicInfo.FigureEnable;
            float origZ = gcode.gcodeZDown;

/* Create Dot */
            if (pathObject is ItemDot)
			{   ItemDot DotData = (ItemDot)pathObject;
                if (DotData.UseZ)
                {	
                    double setZ = calculateZFromRange(graphicInfo.DotZMin, graphicInfo.DotZMax, DotData.Z);//-Math.Abs(DotData.Z);      // be sure for right sign
                    if (logEnable) Logger.Trace("---Dot DotData.UseZ: RangeMin:{0:0.00}  RangeMax:{1:0.00}  DotData.Z:{2:0.00}  -> setZ:{3:0.00}", graphicInfo.DotZMin, graphicInfo.DotZMax, DotData.Z, setZ);
                    setZ = Math.Max(origZ, setZ);    // don't go deeper than set Z
                    if (logCoordinates) Logger.Trace("  PenDownWithZ z:{0:0.00}  setZ:{1:0.00}  gcodeZDown:{2:0.00}", DotData.Z, setZ, origZ);
                    gcode.gcodeZDown = (float)setZ;
                    penIsDown = false;
                }

                else if (graphicInfo.OptionZFromWidth)
                {
                    double newZ = calculateZFromRange(graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, DotData.Z);
                    if (logEnable) Logger.Trace("---Dot OptionZFromWidth: RangeMin:{0:0.00}  RangeMax:{1:0.00}  DotData.Z:{2:0.00}  -> setZ:{3:0.00}", graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, DotData.Z, newZ);
                    newZ = Math.Max(origZ, newZ);        // don't go deeper than set Z
                    gcode.gcodeZDown = (float)newZ;
                    penIsDown = false;
                }

                pathObject.FigureId = StartPath(DotData,toolNr,toolCmt);
				PenDown();
                StopPath();
                gcode.gcodeZDown = origZ;
            }
            else
			{
				if (graphicInfo.OptionZFromWidth)
					gcode.gcodeZDown = 0;

				ItemPath PathData = (ItemPath)pathObject;
				if (logDetailed) Logger.Trace(" {0}  cnt:{1}", PathData.Info.List(), PathData.path.Count);

				if (PathData.path.Count == 0)
				{	if (logEnable) Logger.Trace("--ProcessPathObject: Empty path ID:{0}", PathData.Info.id);
				    return;
				}
                pathObject.FigureId = StartPath(PathData,toolNr,toolCmt);
                PathDashArray = new double[PathData.dashArray.Length];
                PathData.dashArray.CopyTo(PathDashArray, 0);

                double newZ = gcode.gcodeZDown;		// default
			
                for (int index=1; index < PathData.path.Count; index++) // 0 was already processed in StartPath
				{
                    GCodeMotion entity = PathData.path[index];
                    if (graphicInfo.OptionZFromWidth)
                    {
						newZ = calculateZFromRange(graphicInfo.PenWidthMin, graphicInfo.PenWidthMax, entity.Depth);
                        newZ = Math.Max(origZ, newZ);        // don't go deeper than set Z
                        gcode.gcodeZDown = (float)newZ;
                        if (!Properties.Settings.Default.importDepthFromWidthRamp)
                            penIsDown = false;
                        if (logEnable) Logger.Trace("--ProcessPathObject: penWidth:{0:0.00}  -> setZ:{1:0.00}", entity.Depth, newZ);
                    }

/* Create Line */
                    if (entity is GCodeLine)
						MoveTo(entity.MoveTo, newZ, entity.Angle, "");
					else if (entity is GCodeArc)
					{	
/* Create Arc */
						GCodeArc ArcData = (GCodeArc)entity;
						Arc(ArcData.IsCW, ArcData.MoveTo, ArcData.CenterIJ, newZ, ArcData.AngleStart, ArcData.Angle, "");// entity.comment);
					}
				}
				StopPath("");
			}
            gcode.gcodeZDown = origZ;
            if (logDetailed) Logger.Trace("ProcessPathObject end");
        }

		public static double calculateZFromRange(double min, double max, double penWidth)
		{
            if (logEnable) Logger.Trace("----calculateZFromRange: min:{0:0.00}  max: {1:0.00}  input: {2:0.00}", min, max, penWidth);
            if (penWidth == 0)
				return (double)Properties.Settings.Default.importDepthFromWidthMin;
			
			double penMin = Math.Abs(min);
			double penMax = Math.Abs(max);
			double penDelta = (penMax - penMin);
			
			if (penDelta == 0)
				return (double)Properties.Settings.Default.importDepthFromWidthMin;
			double nPen = (Math.Abs(penWidth) - penMin) / penDelta;
			
			double zMin = Math.Abs((double)Properties.Settings.Default.importDepthFromWidthMin);
			double zMax = Math.Abs((double)Properties.Settings.Default.importDepthFromWidthMax);
			double zDelta = (zMax - zMin);

			if (zDelta == 0)
				return (double)Properties.Settings.Default.importDepthFromWidthMin;
						
			double z;
			if (zDelta > 0)		
				z = nPen * zDelta + Math.Min(zMin,zMax);		// min to min, max to max
			else
				z = (penDelta - nPen) * Math.Abs(zDelta) + Math.Min(zMin,zMax);                // min to max, max to min

            if (logEnable) Logger.Trace("---calculateZFromPenWidth: penWidth:{0:0.00}  nPen:{1:0.00}  zDelta:{2:0.00}   z:{3:0.00}", penWidth, nPen, zDelta, z);

            return -z;
		}

        private static string getGroupAttributes(GroupObject groupObject, GraphicInformation graphicInfo)
		{	string[] groupAttribute = new string[] {"none=", "PenColor=", "PenWidth=", "Layer=", "Tile=", "Type="};		// check public enum GroupOptions
			string groupVal1 = string.Format(" {0}\"{1}\"",groupAttribute[(int)graphicInfo.GroupOption], groupObject.key);
            string groupVal2 = string.Format(" ToolNr=\"{0}\"",groupObject.toolNr);
            string groupVal3 = string.Format(" ToolName=\"{0}\"",groupObject.toolName);
            string groupVal4 = string.Format(" PathLength=\"{0:0.0}\"",groupObject.pathLength);
            string groupVal5 = string.Format(" PathArea=\"{0:0.0}\"",groupObject.pathArea);	
			return string.Format("{0}{1}{2}{3}{4}", groupVal1, groupVal2, groupVal3, groupVal4, groupVal5);			
		}
		private static StringBuilder getFigureAttributes(PathObject pathObject)
		{	StringBuilder attributes = new StringBuilder();

			if (pathObject.Info.pathGeometry.Length > 0) 		attributes.Append(string.Format(" Geometry=\"{0}\"", pathObject.Info.pathGeometry));
            if (pathObject.Info.groupAttributes[1].Length > 0)	attributes.Append(string.Format(" PenColor=\"{0}\"", pathObject.Info.groupAttributes[1]));
            if (pathObject.Info.groupAttributes[2].Length > 0) 	attributes.Append(string.Format(" PenWidth=\"{0}\"", pathObject.Info.groupAttributes[2]));
            if (pathObject.Info.groupAttributes[3].Length > 0) 	attributes.Append(string.Format(" Layer=\"{0}\"", pathObject.Info.groupAttributes[3]));
            if (pathObject.Info.groupAttributes[4].Length > 0) 	attributes.Append(string.Format(" Type=\"{0}\"", pathObject.Info.groupAttributes[4]));
            if (pathObject.Info.groupAttributes[5].Length > 0) 	attributes.Append(string.Format(" Tile=\"{0}\"", pathObject.Info.groupAttributes[5]));
            if (pathObject.Info.pathId.Length > 0) 				attributes.Append(string.Format(" PathID=\"{0}\"", pathObject.Info.pathId));
            if (pathObject.PathLength > 0) 						attributes.Append(string.Format(" PathLength=\"{0:0.0}\"", pathObject.PathLength));						
		
			return attributes;
		}
		

        /// <summary>
        /// Set start tag, move to beginning of path via G0, finish old path
        /// </summary>
		private static PathInformation pathInfo = new PathInformation();
		private static bool overWriteId = false;
        public static int StartPath(PathObject pathObject, int toolNr, string toolCmt)//string cmt)
        {
			Point coordxy = pathObject.Start;
			double angle = pathObject.StartAngle;
			int iDToSet = PathCount;
			if (overWriteId && (pathObject.FigureId > 0))
				iDToSet = pathObject.FigureId;
			
            PenUp();   // Don't set xmlMarker.figureEnd

            if (!pathInfo.IsSameAs(pathObject.Info) || FigureEndTagWasSet)
			{	if (!FigureEndTagWasSet)
				{   SetFigureEndTag(); }
				FigureEndTagWasSet = true;

				iDToSet = ++PathCount;
				if (overWriteId && (pathObject.FigureId > 0))
					iDToSet = pathObject.FigureId;
				
				string xml = string.Format("{0} Id=\"{1}\"{2}> ", xmlMarker.figureStart, iDToSet, getFigureAttributes(pathObject).ToString());//attributeGeometry, attributeId, attributeColor, attributeToolNr);
				if (figureEnable)
					Comment(xml);
                if (logCoordinates) Logger.Trace(" StartPath Option:{0}  {1}", pathObject.Options, xml);
				FigureEndTagWasSet = false;

				pathInfo = pathObject.Info.Copy();

                if ((pathObject.Options & CreationOptions.AddPause) > 0)
                    gcode.Pause(gcodeString,"StartPath");
                if (pauseBeforePath) { gcode.Pause(gcodeString, "Pause before path"); }

                if (toolNr >= 0)
                    ToolChange(toolNr, toolCmt);   // add tool change commands (if enabled) and set XYFeed etc.
            }

            double setangle = 180 * angle / Math.PI;
            gcode.setTangential(gcodeString, setangle);
			gcode.MoveToRapid(gcodeString, coordxy);
			PenDown("");

            if (logCoordinates) Logger.Trace("  StartPath at x{0:0.000} y{1:0.000} a={2:0.00}", coordxy.X, coordxy.Y, setangle);

            lastGC = coordxy;
            return iDToSet;	//PathCount;
        }

        public static void SetFigureEndTag()
        {   if (figureEnable)
            {   string xml = string.Format("{0}>", xmlMarker.figureEnd);//, nr);    //string.Format("{0} nr=\"{1}\" >", xmlMarker.figureEnd, nr);
                Comment(xml);
                if (logCoordinates) Logger.Trace(" {0}", xml);
            }
        }

        /// <summary>
        /// Finish path
        /// </summary>
        public static void StopPath(string cmt="")
        {
            if (logCoordinates) Logger.Trace("  StopPath {0}",cmt);
            PenUp(cmt + " Stop path"); 
        }

        /// <summary>
        /// Move to next coordinate
        /// </summary>
        public static void MoveTo(Point coordxy, double newZ, double tangAngle, string cmt)
        {
            if (!Properties.Settings.Default.importDepthFromWidthRamp)
                PenDown(cmt);   //  + " moveto"                      // also process tangetial axis
            double setangle = 180 * tangAngle / Math.PI;
            if (logCoordinates) Logger.Trace(" MoveTo X{0:0.000} Y{1:0.000} A{2:0.00}", coordxy.X, coordxy.Y, setangle);
            gcode.setTangential(gcodeString, setangle, true);
            if(Properties.Settings.Default.importDepthFromWidthRamp)
                gcode.Move(gcodeString, 1, (float)coordxy.X, (float)coordxy.Y, (float)newZ, true, cmt);        
            else
                MoveToDashed(coordxy, cmt);
            lastGC = coordxy;
        }

        private static void MoveToDashed(Point coordxy, string cmt)
        {
            if (logCoordinates) Logger.Trace("  MoveToDashed X{0:0.000} Y{1:0.000}", coordxy.X, coordxy.Y);

            bool showDashInfo = false;
            string dashInfo = "";

            if (logCoordinates)  Logger.Trace("  MoveToDashed enabled:{0} length:{1}", Properties.Settings.Default.importLineDashPattern, PathDashArray.Length);
            if (!Properties.Settings.Default.importLineDashPattern || (PathDashArray.Length <= 1))
            {   gcode.MoveTo(gcodeString, coordxy, cmt); }
            else
            {
                bool penUpG1 = !Properties.Settings.Default.importLineDashPatternG0;
                double dX = coordxy.X - lastGC.X;
                double dY = coordxy.Y - lastGC.Y;
                double xx = lastGC.X, yy = lastGC.Y, dd ;
                int i = 0;
                int save = 1000;
                if (dX == 0)
                {
                    if (dY > 0)
                    {
                        while (yy < coordxy.Y)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenDown("MoveToDashed");
                            dd = PathDashArray[i++];
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            yy += dd;
                            if (yy < coordxy.Y)
                            { gcode.MoveTo(gcodeString, new Point(coordxy.X, yy), dashInfo); }
                            else
                            { gcode.MoveTo(gcodeString, coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp("MoveToDashed");
                            dd = PathDashArray[i++];
                            yy += dd;
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (yy < coordxy.Y)
                            {   if (penUpG1) gcode.MoveTo(gcodeString, new Point(coordxy.X, yy), dashInfo, true);
                                else         gcode.MoveToRapid(gcodeString, new Point(coordxy.X, yy), dashInfo);
                            }
                            else
                            {   if (penUpG1) gcode.MoveTo(gcodeString, coordxy, cmt,true);
                                else         gcode.MoveToRapid(gcodeString, coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 3"); break; }
                        }
                    }
                    else
                    {
                        while (yy > coordxy.Y)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            yy -= PathDashArray[i++];
                            PenDown("MoveToDashed");
                            if (yy > coordxy.Y)
                            { gcode.MoveTo(gcodeString, new Point(coordxy.X, yy), cmt); }
                            else
                            { gcode.MoveTo(gcodeString, coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp("MoveToDashed");
                            yy -= PathDashArray[i++];
                            if (yy > coordxy.Y)
                            {   if (penUpG1) gcode.MoveTo(gcodeString, new Point(coordxy.X, yy), cmt, true);
                                else         gcode.MoveToRapid(gcodeString, new Point(coordxy.X, yy), cmt);
                            }
                            else
                            {   if (penUpG1) gcode.MoveTo(gcodeString, coordxy, cmt, true);
                                else         gcode.MoveToRapid(gcodeString, coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 4"); break; }
                        }
                    }
                }
                else
                {   double dC = Math.Sqrt(dX * dX + dY * dY);
                    double fX = dX / dC;        // factor X
                    double fY = dY / dC;
                    if (dX > 0)
                    {
                        while (xx < coordxy.X)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            PenDown("");
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx < coordxy.X)
                            { gcode.MoveTo(gcodeString, new Point(xx, yy), dashInfo); }
                            else
                            { gcode.MoveTo(gcodeString, coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            PenUp();
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx < coordxy.X)
                            {   if (penUpG1) gcode.MoveTo(gcodeString, new Point(xx, yy), dashInfo, true);
                                else         gcode.MoveToRapid(gcodeString, new Point(xx, yy), dashInfo);
                            }
                            else
                            {   if (penUpG1) gcode.MoveTo(gcodeString, coordxy, cmt, true);
                                else         gcode.MoveToRapid(gcodeString, coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 1"); break; }
                        }
                    }
                    else
                    {
                        while (xx > coordxy.X)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            PenDown("");
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx > coordxy.X)
                            { gcode.MoveTo(gcodeString, new Point(xx, yy), dashInfo); }
                            else
                            { gcode.MoveTo(gcodeString, coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp();
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx > coordxy.X)
                            {   if (penUpG1)    gcode.MoveTo(gcodeString, new Point(xx, yy), dashInfo);
                                else            gcode.MoveToRapid(gcodeString, new Point(xx, yy), dashInfo);
                            }
                            else
                            {   if (penUpG1)    gcode.MoveTo(gcodeString, coordxy, cmt);
                                else            gcode.MoveToRapid(gcodeString, coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 2"); break; }
                        }
                    }
                }
            }
        }

        public static void Arc(bool isG2, Point endPos, Point centerPos, double newZ, double tangStart, double tangEnd, string cmt)
        {
            int gnr = 2; if (!isG2) { gnr = 3; }
            Arc(gnr, endPos.X, endPos.Y, centerPos.X, centerPos.Y, newZ, tangStart,  tangEnd, "");
        }
        public static void Arc(bool isG2, xyPoint endPos, xyPoint centerPos, double newZ, double tangStart, double tangEnd, string cmt)
        {   int gnr = 2; if (!isG2) { gnr = 3; }
            Arc(gnr, endPos.X, endPos.Y, centerPos.X, centerPos.Y, newZ, tangStart, tangEnd, "");

        }
        public static void Arc(int gnr, double x, double y, double i, double j, double newZ, double tangStartRad, double tangEndRad, string cmt = "")
        {
            Point coordxy = new Point(x,y);
			Point center = new Point(lastGC.X + i, lastGC.Y + j);
            double offset = +Math.PI / 2;
            if (logCoordinates) Logger.Trace("  Start Arc G{0} X{1:0.000} Y{2:0.000} cx{3:0.000} cy{4:0.000} ", gnr, x, y, center.X,center.Y);
            if (gnr > 2) { offset = -offset; }

            gcode.setTangential(gcodeString, 180 * tangStartRad / Math.PI, true);
            if (logCoordinates) Logger.Trace("   Start Arc alpha{0:0.000} offset{1:0.000}  ", 180 * tangStartRad / Math.PI, 180 * offset / Math.PI);

            PenDown(cmt + " from Arc");

            if (gcodeMath.isEqual(coordxy,lastGC))				// end = start position? Full circle!
            {   if (gnr > 2)
                    tangEndRad += 2 * Math.PI;        			// CW 360°
                else
                    tangEndRad -= 2 * Math.PI;        			// CCW 360°
            }
            gcode.setTangential(gcodeString, 180 * tangEndRad / Math.PI);
            gcode.Arc(gcodeString, gnr, (float)x, (float)y, (float)i, (float)j, cmt, false);

            lastGC = coordxy;
        }

        /// <summary>
        /// add header and footer, return string of gcode
        /// </summary>
        public static bool FinalGCode(string titel, string file)
        {
            Logger.Trace("FinalGCode() ");
            StringBuilder header = new StringBuilder();
            StringBuilder footer = new StringBuilder(gcode.GetFooter());
            StringBuilder output = new StringBuilder();
            
            header.AppendFormat("( Use case: {0} )\r\n", Properties.Settings.Default.useCaseLastLoaded);
            header.Append(gcode.GetHeader(titel, file));

            if (Properties.Settings.Default.importRepeatEnable && Properties.Settings.Default.importRepeatComplete)      // repeat code x times
            {   for (int i = 0; i<Properties.Settings.Default.importRepeatCnt; i++)
                    output.Append(finalGcodeString);
                header.Append(output);
                header.Append(footer);
            }
            else
            {   header.Append(finalGcodeString);
                header.Append(footer);
            }

            if (Properties.Settings.Default.ctrlLineNumbers || Properties.Settings.Default.ctrlLineEndEnable)
            {   int n = 1;
                string[] lines = header.ToString().Split('\n');
                string end = "";
                if (Properties.Settings.Default.ctrlLineEndEnable)
                    end = Properties.Settings.Default.ctrlLineEndText;
                bool addNr = Properties.Settings.Default.ctrlLineNumbers;
                for (int i = 0; i < lines.Count(); i++)
                {   if (addNr)
                        lines[i] = string.Format("N{0} {1}{2}", n++, lines[i].Trim(),end);
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
        /// add additional header info
        /// </summary>
        public static void AddToHeader(string cmt)
        {   gcode.AddToHeader(cmt);
            if (logEnable) Logger.Trace("AddToHeader: {0}", cmt);
        }

        /// <summary>
        /// return figure end tag string
        /// </summary>
        public static string SetFigureEnd(int nr)
        { return string.Format("{0}>", xmlMarker.figureEnd); }//, nr); }

        /// <summary>
        /// Insert Pen-up gcode command
        /// </summary>
        public static bool PenUp(string cmt = "", bool setEndFigureTag = false)
        {
            if (logCoordinates) Logger.Trace("  PenUp {0}",cmt);

            if (!comments)
                cmt = "";
            bool penWasDown = penIsDown;
            if (penIsDown)
            {   gcode.PenUp(gcodeString, cmt);   }
            penIsDown = false;

            if (setEndFigureTag)
            {   if ((PathCount > 0) && !Graphic2GCode.FigureEndTagWasSet)
                {
                    SetFigureEndTag(); 
                    FigureEndTagWasSet = true;
                }
            }
            return penWasDown;
        }

        /// <summary>
        /// Insert Pen-down gcode command
        /// </summary>
        public static void PenDownWithZ(float z, string cmt)
        {   float orig = gcode.gcodeZDown;
            float setZ = -Math.Abs(z);      // be sure for right sign
            setZ = Math.Max(orig, setZ);    // don't go deeper than set Z
			
            if (logCoordinates) Logger.Trace("  PenDownWithZ z:{0:0.00}  setZ:{1:0.00}  gcodeZDown:{2:0.00}",z, setZ, orig);
			gcode.gcodeZDown = setZ;
            PenDown(cmt);
            gcode.gcodeZDown = orig;
        }
        public static void PenDown(string cmt="")
        {
            if (logCoordinates) Logger.Trace("   PenDown penIsDown:{0}  cmt:{1}", penIsDown, cmt);
            if (!comments)
                cmt = "";

            if (!penIsDown)
            {   
                if (pauseBeforePenDown) { gcode.Pause(gcodeString, "Pause before pen down"); }
                gcode.PenDown(gcodeString, cmt);
            }
            penIsDown = true;
        }

        /// <summary>
        /// Insert tool change command
        /// </summary>
        public static void ToolChange(int toolnr, string cmt = "")
        {   gcode.Tool(gcodeString, toolnr, cmt ); }  // add tool change commands (if enabled) and set XYFeed etc.


        /// <summary>
        /// set comment
        /// </summary>
        public static void Comment(string cmt)
        {   gcode.Comment(gcodeString, cmt); }
		
        public static void Comment(StringBuilder cmt)
        {   gcodeString.Append("(");
			gcodeString.Append(cmt);
			gcodeString.AppendLine(")");			
        }

    }

}
