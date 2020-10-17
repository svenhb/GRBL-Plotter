/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2020 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace GRBL_Plotter
{
    public static partial class Graphic
    {
        public static List<PathObject> completeGraphic = new List<PathObject>();
        public static List<PathObject> finalPathList = new List<PathObject>();
        public static List<PathObject> tileGraphicAll = new List<PathObject>();
		public static List<GroupObject> groupedGraphic = new List<GroupObject>();
		public static Dictionary<string, int> keyToIndex = new Dictionary<string, int>();
		public static Dictionary<string,string> tileCommands = new Dictionary<string,string>();
        public static GroupObject tmpGroup =  new GroupObject();

        public static List<String> headerInfo = new List<String>();
        private static ItemPath actualPath = new ItemPath();
        private static PathInformation actualPathInfo = new PathInformation();
        private static double[] actualDashArray = new double[0];

        private static Dictionary<string, int>[] groupPropertiesCount = new Dictionary<string, int>[10];

        private static Dimensions actualDimension = new Dimensions();
        private static Point lastPoint = new Point();
		private static PathObject lastPath = new ItemPath();
		private static CreationOptions lastOption = CreationOptions.none;

        private static int objectCount = 0;
        public static GraphicsPath pathBackground = new GraphicsPath();             // show complete graphic as background if tiles activated
        private static double equalPrecision = 0.00001;

        private static bool continuePath = false;
        private static bool setNewId = true;

        public static GraphicInformation graphicInformation = new GraphicInformation();

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
		{	Logger.Trace("CleanUp()  before:{0}",System.Environment.WorkingSet/1000);
			completeGraphic.Clear();
			finalPathList.Clear();
			tileGraphicAll.Clear();
			groupedGraphic.Clear();
			headerInfo.Clear();
			Graphic2GCode.CleanUp();
			Logger.Trace("CleanUp()  after:{0}",System.Environment.WorkingSet/1000);			
		}
		
		#region collect data - loggerTrace & 2
		
        public static void Init(SourceTypes type, string filePath)
        {
			logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnable.Level2) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnable.Detailed) > 0);
            logSortMerge = logEnable && ((logFlags & (uint)LogEnable.Sort) > 0);
            logCoordinates = logEnable && ((logFlags & (uint)LogEnable.Coordinates) > 0);
            logProperties = logEnable && ((logFlags & (uint)LogEnable.Properties) > 0);
            logModification = logEnable && ((logFlags & (uint)LogEnable.PathModification) > 0);

            VisuGCode.logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnable.Level4) > 0);
            VisuGCode.logDetailed = VisuGCode.logEnable && ((logFlags & (uint)LogEnable.Detailed) > 0);
            VisuGCode.logCoordinates = VisuGCode.logEnable && ((logFlags & (uint)LogEnable.Coordinates) > 0);

            Logger.Trace("Init Graphic {0}  loggerTrace:{1}",type.ToString(), Convert.ToString(logFlags, 2));
			
			graphicInformation = new GraphicInformation();			// get Default settings
            graphicInformation.Title = type.ToString() + " import";	// fill up structure
            graphicInformation.SourceType = type;
            graphicInformation.FilePath = filePath;
            pathBackground = new GraphicsPath();

            completeGraphic = new List<PathObject>();
			finalPathList = new List<PathObject>();
			tileGraphicAll = new List<PathObject>();
//			groupedGraphic = new List<GroupObject>();
			actualPath = new ItemPath();
			
            headerInfo = new List<String>();
            actualPathInfo = new PathInformation();
            actualDashArray = new double[0];
            actualDimension = new Dimensions();
            lastPoint = new Point();
            lastPath = new ItemPath();
			lastOption = CreationOptions.none;
			
			for (int i=0; i< groupPropertiesCount.Count(); i++)
				groupPropertiesCount[i] = new Dictionary<string, int>();
			
			objectCount = 0;
            continuePath = false;
            setNewId = true;
            equalPrecision = (double)Properties.Settings.Default.importAssumeAsEqualDistance;
        }

        public static void StartPath(Point xy, CreationOptions addFunction = CreationOptions.none)
        {   // preset informations
            if (completeGraphic.Count() > 0)
            {   lastPath = completeGraphic[completeGraphic.Count() - 1];			}

            actualPath.Info.CopyData(actualPathInfo);       // preset global info for GROUP

            // only continue last path if same layer, color, dash-pattern - if enabled
            if ((lastPath is ItemPath) && (objectCount > 0) && hasSameProperties((ItemPath)lastPath, (ItemPath)actualPath) && (isEqual(xy,lastPoint))) 
            {   actualPath = (ItemPath)lastPath;
				actualPath.Options = lastOption;
                if (logCoordinates) Logger.Trace("AddToPath Id:{0} at X:{1:0.00} Y:{2:0.00} {3}", objectCount, xy.X, xy.Y, actualPath.Info.List());
                continuePath = true;
            }
            else
            {   if (actualPath.path.Count > 1)
					StopPath("in StartPath");   // save previous path, before starting an new one

                if (setNewId)
                    objectCount++;
                actualPath = new ItemPath(xy);					// reset path
				actualPathInfo.id = objectCount;
                actualPath.Info.CopyData(actualPathInfo);       // preset global info for GROUP
                actualPath.Options = lastOption;
                if (actualDashArray.Length > 0)
                {   actualPath.dashArray = new double[actualDashArray.Length];
                    actualDashArray.CopyTo(actualPath.dashArray, 0);
                }
//                actualDashArray = new double[0];
                if (logCoordinates) Logger.Trace("StartPath Id:{0} at X:{1:0.00} Y:{2:0.00} {3}", objectCount, xy.X, xy.Y, actualPath.Info.List());
            }
            lastPoint = xy;
            setNewId = false;
			lastOption = CreationOptions.none;
        }
        public static void StopPath(string cmt = "")
        {
            if (continuePath && (completeGraphic.Count() > 0))
                completeGraphic[completeGraphic.Count() - 1] = actualPath;
            else
            {
                if (actualPath.Dimension.isXYSet())
                { completeGraphic.Add(actualPath); }
                else
                { if (logEnable) Logger.Trace("   StopPath dimension not set - skip"); }
            }
            if (logCoordinates) Logger.Trace("StopPath  cnt:{0}  cmt:{1} {2}", (objectCount-1), cmt, actualPath.Info.List());
            if (logEnable) Logger.Trace("    StopPath Dimension  {0}", actualPath.Dimension.getXYString());

            continuePath = false;
//            actualDashArray = new double[0];
            actualPath = new ItemPath();                    // reset path
            actualPathInfo.id = objectCount;
            actualPath.Info.CopyData(actualPathInfo);       // preset global info for GROUP
            actualPath.Options = lastOption;

            if (actualPath.path.Count > 0)
            {   actualDimension.addDimensionXY(actualPath.Dimension);  }
        }

        public static void AddLine(Point xy, string cmt = "")
        {   
			double z = getActualZ();                        // apply penWidth if enabled
            if (isEqual(lastPoint, xy))
			{	if (logCoordinates) Logger.Trace("  AddLine SKIP, same coordinates! X:{0:0.00} Y:{1:0.00}", xy.X, xy.Y);	}
			else
			{	actualPath.Add(xy,z,0);
				if (logCoordinates) Logger.Trace("  AddLine to X:{0:0.00} Y:{1:0.00}  Z:{2:0.00} new dist {3:0.00}", xy.X, xy.Y, z, actualPath.PathLength);
				actualDimension.setDimensionXY(xy.X, xy.Y);
				lastPoint = xy;
			}
        }
        public static void AddDot(Point xy, string cmt = "")
        {   AddDot(xy.X,xy.Y, cmt);  }
        public static void AddDot(double x, double y, string cmt = "")
        {   ItemDot dot = new ItemDot(x, y);
            dot.Info.CopyData(actualPathInfo);
            dot.Z = getActualZ();                           // apply penWidth if enabled

            completeGraphic.Add(dot);
            lastPoint = new Point(x, y);
            actualDimension.setDimensionXY(x, y);
            if (logCoordinates) Logger.Trace("  AddDot at X:{0:0.00} Y:{1:0.00}",x, y);
            actualPath = new ItemPath();					// reset path
        }
        public static void AddDotWithZ(Point xy, double Z, string cmt = "")
		{	AddDotWithZ(xy.X,xy.Y, Z, cmt);  }
        public static void AddDotWithZ(double x, double y, double Z, string cmt = "")
        {   ItemDot dot = new ItemDot(x, y, Z);
            dot.Info.CopyData(actualPathInfo);
            completeGraphic.Add(dot);
            lastPoint = new Point(x, y);
            actualDimension.setDimensionXY(x, y);
            if (logCoordinates) Logger.Trace("  AddDot at X:{0:0.00} Y:{1:0.00} Z:{2:0.00} ", x, y, Z);
            actualPath = new ItemPath();                    // reset path
            graphicInformation.SetDotZ(Z);
        }

        public static void AddCircle(int gnr, double centerX, double centerY, double radius)
        { 	actualPath.AddArc(new Point(centerX + radius, centerY), new Point(-radius, 0), getActualZ(), true, graphicInformation.ConvertArcToLine);// convertArcToLine);
            actualPath.Info.CopyData(actualPathInfo);    // preset global info for GROUP
            if (logCoordinates) Logger.Trace("  AddCircle to X:{0:0.00} Y:{1:0.00} r:{2:0.00}  angleStep:{3}", centerX, centerY, radius, Properties.Settings.Default.importGCSegment);
        }

        public static void AddArc(bool isG2, Point xy, Point ij, string cmt = "")
        {   AddArc(isG2, xy.X, xy.Y, ij.X, ij.Y, cmt); }
        public static void AddArc(int gnr, double x, double y, double i, double j, string cmt = "")
        {   AddArc((gnr == 2), x, y, i, j, cmt); }
        public static void AddArc(bool isg2, double x, double y, double i, double j, string cmt = "")
        {   lastPoint = new Point(x, y);
            actualPath.AddArc(new Point(x, y), new Point(i, j), getActualZ(), isg2, graphicInformation.ConvertArcToLine);
            actualPath.Info.CopyData(actualPathInfo);    // preset global info for GROUP
            if (logCoordinates) Logger.Trace("  AddArc to X:{0:0.00} Y:{1:0.00} i:{2:0.00} j:{3:0.00}  angleStep:{4}  isG2:{5}", x, y, i, j, Properties.Settings.Default.importGCSegment, isg2);
        }
		
		public static double getActualZ()
		{	double z = 0;
			if (graphicInformation.OptionZFromWidth)
				if (!double.TryParse(actualPathInfo.groupAttributes[(int)GroupOptions.ByWidth], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out z))
				{}
			return z;
		}
		
		// set marker, to take-over the flag on next "StartPath"
		public static void OptionInsertPause()
		{	lastOption |= CreationOptions.AddPause;}
		
        public static void SetType(string txt)
        { 	if (logProperties) Logger.Trace("Set Type '{0}'",txt);
            setNewId = true;
			int tmpIndex = (int)GroupOptions.ByType;
			countProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex,txt))
				Logger.Error(" Error SetType '{0}'",txt);}

        public static void SetLayer(string txt)
        {   if (logProperties) Logger.Trace("Set Layer '{0}'",txt);
            setNewId = true;
			int tmpIndex = (int)GroupOptions.ByLayer;
			countProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex,txt))
				Logger.Error(" Error SetLayer '{0}'",txt);}

        public static void SetPenWidth(string txt)	// DXF: 0 - 2.11mm = 0 - 211		SVG: 0.000 - ?  Convert with to mm, then to string in importClass
        {   if (logProperties) Logger.Trace("Set PenWidth '{0}'",txt);
 //           setNewId = true;
			int tmpIndex = (int)GroupOptions.ByWidth;
			countProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
				Logger.Error(" Error SetPenWidth '{0}'",txt);
			graphicInformation.SetPenWidth(txt);		// find min / max values
		}

        public static void SetPenColor(string txt)
        {   if (logProperties) Logger.Trace("Set PenColor '{0}'",txt);
            setNewId = true;
			int tmpIndex = (int)GroupOptions.ByColor;
			countProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
				Logger.Error(" Error SetPenColor '{0}'",txt);}

        public static void SetPenFill(string txt)
        {   if (logProperties) Logger.Trace("Set PenFill '{0}'",txt);
            setNewId = true;
			int tmpIndex = (int)GroupOptions.ByFill;
			countProperty(tmpIndex, txt);
            if (!actualPathInfo.SetGroupAttribute(tmpIndex, txt))
				Logger.Error(" Error SetPenFill '{0}'",txt);}

        public static void SetPenColorId(int id)
        { 	if (logProperties) Logger.Trace("Set PenColorId '{0}'",id);
            actualPathInfo.penColorId = id;
            setNewId = true;
        }

        public static void SetPathId(string txt)
        { 	if (logProperties) Logger.Trace("Set PathId '{0}'",txt);
            actualPathInfo.pathId = txt;
            setNewId = true;
        }

        public static void SetGeometry(string txt)
        { 	if (logProperties) Logger.Trace("Set Geometry '{0}'",txt);
            actualPathInfo.pathGeometry = txt;
//            setNewId = true;
        }

        public static void SetComment(string txt)
        { 	if (logProperties) Logger.Trace("Set Comment '{0}'",txt);
//            actualPathInfo.pathComment = txt;
        }

        public static void SetHeaderInfo(string txt)
        { 	if (logProperties) Logger.Trace("Set HeaderInfo '{0}'",txt);
            headerInfo.Add(txt); }

        public static void SetDash(double[] tmp)
        {   if (logProperties) Logger.Trace("Set Dash '{0}'",String.Join(", ", tmp.Select(p=>p.ToString()).ToArray()));
            if (tmp.Length > 0)
            {   actualDashArray = new double[tmp.Length];
                tmp.CopyTo(actualDashArray, 0);
            }
            else
                actualDashArray = new double[0];

            setNewId = true;
            if (logProperties)
            {   string dash = "";
                foreach (double d in actualDashArray)
                    dash += d.ToString() + ", ";
                if (logEnable) Logger.Trace("SetDash in ID:{0} {1}", actualPath.Info.id, dash);
            }
        }
		
		private static void countProperty(int index, string txt)
		{   if (index < groupPropertiesCount.Count())
			    if (!groupPropertiesCount[index].ContainsKey(txt))
				    groupPropertiesCount[index].Add(txt,1);
		}

		#endregion

// #######################################################################
// Do modifications, then create GCode in graphic2GCode
// #######################################################################
        public static string CreateGCode()
        {
			if (actualPath.path.Count > 1)
				StopPath("in CreateCode");	// save previous path
				
			if (logEnable) Logger.Trace("CreateGCode() count:{0}", completeGraphic.Count());

/* remove short moves*/
            if (Properties.Settings.Default.importRemoveShortMovesEnable)
            {   RemoveShortMoves(completeGraphic, (double)Properties.Settings.Default.importRemoveShortMovesLimit); }

/* remove offset */
            if (graphicInformation.OptionOffsetCode)  // || (Properties.Settings.Default.importGraphicTile) 
            {   SetHeaderInfo(string.Format(" Original graphic dimension min:{0:0.000};{1:0.000}  max:{2:0.000};{3:0.000}", actualDimension.minx, actualDimension.miny, actualDimension.maxx, actualDimension.maxy));
                RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny);
            }

 //           if (Properties.Settings.Default.importSVGNodesOnly)         { SetDotOnly(); }

/* show original graphics in 2D-view */
            createGraphicsPath(completeGraphic, pathBackground);

/* hatch fill */
            if (graphicInformation.ApplyHatchFill || graphicInformation.OptionHatchFill)
                HatchFill(completeGraphic);

/* repeate paths */
            if (graphicInformation.OptionRepeatCode && !Properties.Settings.Default.importRepeatComplete)
				RepeatPaths(completeGraphic, (int)Properties.Settings.Default.importRepeatCnt);

/* sort by distance and merge paths with same start / end coordinates*/
            if (graphicInformation.OptionSortCode)  { MergeFigures(completeGraphic); SortByDistance(completeGraphic);  }

/* Drag Tool path modification*/
            if (graphicInformation.OptionDragTool)
                DragToolModification(completeGraphic);

/* clipping or tiling of whole graphic */
            if (graphicInformation.OptionClipCode)  
            {   //OffsetCoordinates();
				if (!Properties.Settings.Default.importGraphicClip)			// for tiles, grouping is needed
				{	graphicInformation.GroupOption = GroupOptions.ByTile;
					graphicInformation.SortOption = SortOptions.none;
                    graphicInformation.GroupEnable = true;
				}
                ClipCode((double)Properties.Settings.Default.importGraphicTileX,(double)Properties.Settings.Default.importGraphicTileY); 
            }
			
/* extend closed path for laser cutting to avoid a "nose" at start/end position */
            if (graphicInformation.OptionExtendPath)
				ExtendClosedPaths(completeGraphic, (double)Properties.Settings.Default.importGraphicExtendPathValue);

/* calculate tangential axis - paths may be splitted, do not merge afterwards!*/
            if (graphicInformation.OptionTangentialAxis)
                CalculateTangentialAxis(); 

/* List option data */
            if (graphicInformation.OptionZFromWidth || (graphicInformation.OptionDotFromCircle && graphicInformation.OptionZFromRadius))
			{   string tmp1 = string.Format(" penMin: {0:0.00} penMax: {1:0.00}",
                    graphicInformation.PenWidthMin, graphicInformation.PenWidthMax);
                string tmp2 = string.Format("   zMin:{0:0.00}   zMax:{1:0.00}  zLimit:{2:0.00}",
                    Properties.Settings.Default.importDepthFromWidthMin, Properties.Settings.Default.importDepthFromWidthMax,
                    Properties.Settings.Default.importGCZDown);
                SetHeaderInfo(tmp1);
                SetHeaderInfo(tmp2);
                if (logEnable) Logger.Trace("----OptionZFromWidth {0}{1}", tmp1, tmp2);
//                if (loggerTrace > 0) ListGraphicObjects(completeGraphic,true);
            }

/* group objects by color/width/layer/tile-nr */
            if (graphicInformation.GroupEnable)
			{	
				if (!GroupAllGraphics(graphicInformation))
					return Graphic2GCode.CreateGCode(completeGraphic, headerInfo, graphicInformation);

				return Graphic2GCode.CreateGCode(groupedGraphic, headerInfo, graphicInformation);
			}

			return Graphic2GCode.CreateGCode(completeGraphic, headerInfo, graphicInformation);
        }
// #######################################################################

        // create GraphicsPath for 2-D view
        private static void createGraphicsPath(List<PathObject> graphicToOffset, GraphicsPath path)
        {
            if (logEnable) Logger.Trace("...createGraphicsPath of original graphic (background)" );
            foreach (PathObject item in graphicToOffset)    // dot or path
            {
                if (item is ItemPath)
                {
                    ItemPath PathData = (ItemPath)item;
					if (logDetailed) 	Logger.Trace("   createGraphicsPath - StartFigure id:{0} geo:{1}", item.Info.id, item.Info.pathGeometry);
                    path.StartFigure();

                    if (PathData.path.Count > 1)
                    {
                        for (int i = 1; i < PathData.path.Count; i++)
                        {
                            if (PathData.path[i] is GCodeLine)
                            {  	if (logDetailed) 	Logger.Trace("    createGraphicsPath - AddLine {0:0.00} {1:0.00} {2:0.00} {3:0.00}", (float)PathData.path[i - 1].MoveTo.X, (float)PathData.path[i - 1].MoveTo.Y, (float)PathData.path[i].MoveTo.X, (float)PathData.path[i].MoveTo.Y);	
								path.AddLine((float)PathData.path[i - 1].MoveTo.X, (float)PathData.path[i - 1].MoveTo.Y, (float)PathData.path[i].MoveTo.X, (float)PathData.path[i].MoveTo.Y);
							}
                            else
                            {
                                GCodeArc arcPath = (GCodeArc)PathData.path[i];
                                ArcProperties arcMove;
                                arcMove = gcodeMath.getArcMoveProperties((xyPoint)PathData.path[i - 1].MoveTo, (xyPoint)arcPath.MoveTo, arcPath.CenterIJ.X, arcPath.CenterIJ.Y, arcPath.IsCW);
                                float x1 = (float)(arcMove.center.X - arcMove.radius);
                                float x2 = (float)(arcMove.center.X + arcMove.radius);
                                float y1 = (float)(arcMove.center.Y - arcMove.radius);
                                float y2 = (float)(arcMove.center.Y + arcMove.radius);
                                float r2 = 2 * (float)arcMove.radius;
                                float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
                                float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);
                                if (arcMove.radius > equalPrecision)
                                {
                                    if (logDetailed) Logger.Trace("    createGraphicsPath - AddArc {0:0.00} {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00}", x1, y1, r2, r2, aStart, aDiff);
                                    path.AddArc(x1, y1, r2, r2, aStart, aDiff);
                                }
                                else
                                {
                                    Logger.Error("....createGraphicsPath radius too small r:{0} - convert arc to line  id:{1} lastX:{2:0.00} lastY:{3:0.00} newX:{4:0.00} newY:{5:0.00} I:{6:0.00} J:{7:0.00}", arcMove.radius, item.Info.id, PathData.path[i - 1].MoveTo.X, PathData.path[i - 1].MoveTo.Y, arcPath.MoveTo.X, arcPath.MoveTo.Y, arcPath.CenterIJ.X, arcPath.CenterIJ.Y);
                                    // replace arc by line
                                    PathData.path[i] = new GCodeLine(arcPath.MoveTo);
                                }	
                            }
                        }
                    }
                }
                else
                {
                    if (logDetailed) Logger.Trace("    createGraphicsPath - AddDot {0:0.00} {1:0.00} ", item.Start.X, item.Start.Y);
                    path.StartFigure();
                    path.AddLine((float)item.Start.X- (float)equalPrecision, (float)item.Start.Y, (float)item.Start.X+ (float)equalPrecision, (float)item.Start.Y);
                } // is Dot
            }
        }

        #region process paths

        private static bool	GroupAllGraphics(GraphicInformation graphicInformation, bool preventReversal = false)
		{   
            groupedGraphic = new List<GroupObject>();
            bool log = logEnable && ((logFlags & (uint)LogEnable.GroupAllGraphics) > 0);
			keyToIndex = new Dictionary<string, int>();
			string tmpKey="";
			int groupCount = -1;
			int toolNr = (int)Properties.Settings.Default.importGCToolDefNr; 
			string toolName = "";

			if (logEnable) Logger.Trace("...GroupAllGraphics by:{0}  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++", graphicInformation.GroupOption.ToString());				
            if (logEnable) 
			{	foreach (GroupOptions tmp in (GroupOptions[]) Enum.GetValues(typeof(GroupOptions)))
				{   Logger.Trace("   Property count {0} {1}",tmp, groupPropertiesCount[(int)tmp].Count);	}
			}
			
			int differentGroupValues = groupPropertiesCount[(int)graphicInformation.GroupOption].Count;		// 2020-08-11
			if (differentGroupValues < 2)	// too less values to group
			{	SetHeaderInfo(" Too less values to group ");
				if (logEnable) Logger.Trace("----Too less values to group {0}",differentGroupValues);
				graphicInformation.GroupEnable = false; // diable in case of ReDoReversePath
				return false;
			}

            foreach (PathObject pathObject in completeGraphic)
			{
                if (pathObject is ItemPath)
                {   if (((ItemPath)pathObject).path.Count <= 0)
                    {   continue;
					}
                }
				tmpKey = pathObject.Info.groupAttributes[(int)graphicInformation.GroupOption];      // get value to group by GroupOption-key
								
                if (log) Logger.Trace("    GroupAll {0} type:{1} count:{2}  key:'{3}'", pathObject.Info.id, pathObject.GetType().ToString(), groupCount, tmpKey);

                if (keyToIndex.ContainsKey(tmpKey))
				{
                    if (groupCount < groupedGraphic.Count)
					{	groupedGraphic[keyToIndex[tmpKey]].AddInfo(pathObject);		// add length, dimension, area
						groupedGraphic[keyToIndex[tmpKey]].groupPath.Add(pathObject);
					}
                    if (log) Logger.Trace("     Add {0} to key:'{1}'", pathObject.Info.id, tmpKey);
                }
                else
				{
                    groupCount++;
                    keyToIndex.Add(tmpKey, groupCount);
					
					switch (graphicInformation.GroupOption)
					{	case GroupOptions.ByColor:
							toolNr = toolTable.getToolNrByToolColor(pathObject.Info.groupAttributes[(int)GroupOptions.ByColor], 0);
		                    toolName = toolTable.getToolName(toolNr);
							break;
                        case GroupOptions.ByWidth:
                            toolNr = toolTable.getToolNrByToolDiameter(pathObject.Info.groupAttributes[(int)GroupOptions.ByWidth]);
                            toolName = toolTable.getToolName(toolNr);
                            break;
                        case GroupOptions.ByLayer:
                            toolNr = toolTable.getToolNrByToolName(pathObject.Info.groupAttributes[(int)GroupOptions.ByLayer]);
                            toolName = toolTable.getToolName(toolNr);
                            break;
                        default: break;
					}

				    tmpGroup = new GroupObject(tmpKey, toolNr, toolName, pathObject);
                    groupedGraphic.Add(tmpGroup);
                    if (log) Logger.Trace("     Create {0} new key:'{1}'", pathObject.Info.id, tmpKey);
                }
            }

// Sort Groups by		
            bool invert = Properties.Settings.Default.importGroupSortInvert;
			if (graphicInformation.SortOption != SortOptions.none)		// 		public enum SortOption  { none=0, ByToolNr=1, ByCodeSize=2, ByGraphicDimension=3};
			{
                if (log) Logger.Trace("    Sort by {0}", graphicInformation.SortOption);
                switch (graphicInformation.SortOption)
                {
                    case SortOptions.ByProperty:
                        if (!invert)
                            groupedGraphic.Sort((a, b) => a.key.CompareTo(b.key));
                        else
                            groupedGraphic.Sort((a, b) => b.key.CompareTo(a.key));
                        break;
                    case SortOptions.ByToolNr:
                        if (!invert)
                            groupedGraphic.Sort((a, b) => a.toolNr.CompareTo(b.toolNr));
						else
                            groupedGraphic.Sort((a, b) => b.toolNr.CompareTo(a.toolNr));
                        break;
					case SortOptions.ByCodeSize:
						if (invert)
                            groupedGraphic.Sort((a, b) => a.pathLength.CompareTo(b.pathLength));
                        else
                            groupedGraphic.Sort((a, b) => b.pathLength.CompareTo(a.pathLength));
                        break;
					case SortOptions.ByGraphicDimension:
						if (invert)
                            groupedGraphic.Sort((a, b) => a.pathArea.CompareTo(b.pathArea));
                        else
                            groupedGraphic.Sort((a, b) => b.pathArea.CompareTo(a.pathArea));
                        break;
					default:
						break;			
				}
			}

// Sort paths in group by distance		
            if (Properties.Settings.Default.importGraphicSortDistance)
            {   foreach (GroupObject groupObject in groupedGraphic)
                {   if (groupObject.groupPath.Count > 1)
                        SortByDistance(groupObject.groupPath, preventReversal);
                }
            }

            if (log)
            {   Logger.Trace(" Grouping result +++++++++++++++++++++++++++++++++++++++++++++++++");
                int i = 0;
                foreach (GroupObject groupObject in groupedGraphic)
                {   Logger.Trace("  GroupCount i:'{0}' key:'{1}' toolNr:'{2}' toolName:'{3}' pathLength:{4:0.2}", i++, groupObject.key, groupObject.toolNr, groupObject.toolName, groupObject.pathLength);
                    ListGraphicObjects(groupObject.groupPath);
                }
            }
			
			return true;
        }
		
		private static void ListGraphicObjects(List<PathObject> graphicToShow, bool showCoord=false)
		{	if (logEnable) Logger.Trace("  ListGraphicObjects Count:{0}  ##########################################", graphicToShow.Count);
            int cnt = 1;
            string coordByLine;
            string info;
			double length;
			bool reversed=false;

            foreach (PathObject graphicItem in graphicToShow)
			{   if (graphicItem is ItemPath)
                {   cnt = ((ItemPath)graphicItem).path.Count;
					length = graphicItem.PathLength;
					reversed = ((ItemPath)graphicItem).Reversed;
				}
                else
                {   cnt = 1;
					length = 0;
				}
                coordByLine = string.Format("x1:{0,6:0.00} y1:{1,6:0.00}  x2:{2,6:0.00} y2:{3,6:0.00}  reversed:{4}", graphicItem.Start.X, graphicItem.Start.Y, graphicItem.End.X, graphicItem.End.Y, reversed);
                info = string.Format(" color:'{0}' width:'{1}' layer:'{2}' length:'{3}'", graphicItem.Info.groupAttributes[1], graphicItem.Info.groupAttributes[2], graphicItem.Info.groupAttributes[3], length);   // index 0 not used
                if (logEnable) Logger.Trace("    graphicItem Id:{0,3} pathId:{1,-12} Geo:{2,-8}  {3}   Points:{4,3}  Coord:{5}", graphicItem.Info.id, graphicItem.Info.pathId, graphicItem.Info.pathGeometry, info, cnt, coordByLine);

				if (logCoordinates)
				{	if (showCoord && (graphicItem is ItemPath))
					{	foreach (GCodeMotion ent in ((ItemPath)graphicItem).path)
						{    Logger.Trace("       X:{0:0.00} Y:{1:0.00} Z:{2:0.00}  Line:{3} ", ent.MoveTo.X, ent.MoveTo.Y, ent.Depth, (ent is GCodeLine).ToString()); }
					}
				}
            }
            if (logEnable) Logger.Trace("  ListGraphicObjects          ##########################################", graphicToShow.Count);
        }

		# region clipPath
        public static void ClipCode(double tileSizeX, double tileSizeY)//xyPoint p1, xyPoint p2)      // set dot only extra behandeln
        {	//const uint loggerSelect = (uint)LogEnable.ClipCode;
			bool log = logEnable && ((logFlags & (uint)LogEnable.ClipCode) > 0);
            finalPathList = new List<PathObject>();    // figures of one tile
            tileGraphicAll = new List<PathObject>();    // figures of all tiles  // PathObject contains List<grblMotion>, PathInformation, pathStart, pathEnd, dimension
            Dimensions tileOneDimension = new Dimensions();
			tileCommands = new Dictionary<string,string>();
			
            ItemPath tilePath = new ItemPath();

            Point pStart, pEnd;
            Point origStart, origEnd;
            
            Point clipMin = new Point(0,0);
            Point clipMax = new Point(tileSizeX, tileSizeY);
            if (logEnable) Logger.Trace("...ClipCode Path min X:{0:0.0} Y:{1:0.0} max X:{2:0.0} Y:{3:0.0} ##################################################################", clipMin.X, clipMin.Y, clipMax.X, clipMax.Y);

            RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny);

			if (log) ListGraphicObjects(completeGraphic);

            if ((tileSizeX <= 0) || (tileSizeY <= 0))
            {
                Logger.Error("  Tile size is <= 0  x:{0}  y:{1}",tileSizeX, tileSizeY);
                return;
            }

            int tilesX = (int)Math.Ceiling(actualDimension.maxx/tileSizeX);        // loop through all possible tiles
            int tilesY = (int)Math.Ceiling(actualDimension.maxy/tileSizeY);
            int tileNr = 0;
			
			string tileID ="";
			
			bool doTilingNotClipping = !Properties.Settings.Default.importGraphicClip;

/* show cut lines in backgroud */
            float tmpX, tmpY;
            if (doTilingNotClipping)	// show cut lines in backgroud
            {
                for (int indexX = 0; indexX < tilesX; indexX++)
                {
                    pathBackground.StartFigure();
                    tmpX = (float)(indexX * tileSizeX);
                    pathBackground.AddLine(tmpX, 0, tmpX, (float)actualDimension.maxy);
                }
                for (int indexY = 0; indexY < tilesY; indexY++)
                {
                    pathBackground.StartFigure();
                    tmpY = (float)(indexY * tileSizeY);
                    pathBackground.AddLine(0, tmpY, (float)actualDimension.maxx, tmpY);
                }
            }
            else
            {
                pathBackground.StartFigure();
                tmpX = (float)Properties.Settings.Default.importGraphicClipOffsetX;
                tmpY = (float)Properties.Settings.Default.importGraphicClipOffsetY;
                System.Drawing.RectangleF pathRect = new System.Drawing.RectangleF(tmpX, tmpY, (float)tileSizeX, (float)tileSizeY);
                pathBackground.AddRectangle(pathRect);
            }

            for (int indexY = 0; indexY < tilesY; indexY++)
            {				
                for (int indexX = 0; indexX < tilesX; indexX++)
                {
                    clipMin.X = indexX * tileSizeX;
                    clipMin.Y = indexY * tileSizeY;
                    clipMax.X = (indexX + 1) * tileSizeX ;
                    clipMax.Y = (indexY + 1) * tileSizeY ;
                    tileNr++;

                    if (doTilingNotClipping)
                    {                                                       // Add micro-offset to avoid double consideration of points on clip-border
                        if (clipMin.X > 0) clipMin.X += 0.00001;
                        if (clipMin.Y > 0) clipMin.Y += 0.00001;
                    }
					else
                    {
                        clipMin.X = (double)Properties.Settings.Default.importGraphicClipOffsetX;
                        clipMin.Y = (double)Properties.Settings.Default.importGraphicClipOffsetY;
                        clipMax.X = clipMin.X + tileSizeX;
                        clipMax.Y = clipMin.Y + tileSizeY;
                    }
					
					tileID = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
					tileCommands.Add(tileID, getTileCommand(indexX, indexY, tileSizeX, tileSizeY));
					
                    if (log) Logger.Trace("New tile {0} +++++++++++++++++++++++++++++++++++++++", tileID);
                    int foreachcnt = 1;
					
                    foreach (PathObject graphicItem in completeGraphic)
                    {
                        foreachcnt++;
                        if (!(graphicItem is ItemDot))
                        {
                            ItemPath item = (ItemPath)graphicItem;

                            if (item.path.Count == 0)
                            {   continue;   }
							
							if (!WithinRectangle(item.Dimension, clipMin, clipMax))
							{	continue;	}
							
                            pStart = item.path[0].MoveTo;
                            actualDashArray = new double[0];
                            if (item.dashArray.Length > 0)
                            {   actualDashArray = new double[item.dashArray.Length];
                                item.dashArray.CopyTo(actualDashArray, 0);
                            }

                            tilePath = new ItemPath(new Point(pStart.X, pStart.Y));
                            tilePath.Info.CopyData(graphicItem.Info);              // preset global info for GROUP
                                                                                   //               tilePath.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                            tilePath.Info.SetGroupAttribute((int)GroupOptions.ByTile, tileID);
                            if (actualDashArray.Length > 0)
                            {   tilePath.dashArray = new double[actualDashArray.Length];
                                actualDashArray.CopyTo(tilePath.dashArray, 0);
                            }

                            for (int i = 1; i < item.path.Count; i++)           // go through path objects { lineStart=0, line=1, dot=2, arcCW=3, arcCCW=4 }
                            {
                                //                        if (loggerTrace) Logger.Trace(" foreach i {0} of {1}", i, item.path.Count);
                                origStart = item.path[i - 1].MoveTo;
                                pStart = new Point(origStart.X, origStart.Y);
                                origEnd = item.path[i].MoveTo;
                                pEnd = new Point(origEnd.X, origEnd.Y);

                                if (ClipLine(ref pStart, ref pEnd, clipMin, clipMax))
                                {
                                    if (origStart != pStart)		// path was clipped, store old path, start new path, becaue pen-up/down is needed
                                    {
                                        if (tilePath.path.Count > 1)
                                            finalPathList.Add(tilePath);                   	// save prev path
                                        tilePath = new ItemPath(new Point(pStart.X, pStart.Y));  	// start new path with clipped start position
                                        tilePath.Info.CopyData(graphicItem.Info);               // preset global info for GROUP
                                                                                                //                           tilePath.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                                        tilePath.Info.SetGroupAttribute((int)GroupOptions.ByTile, tileID);// string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
										if (actualDashArray.Length > 0)
                                        {   tilePath.dashArray = new double[actualDashArray.Length];
                                            actualDashArray.CopyTo(tilePath.dashArray, 0);
                                        }
                                    }

                                    if (pStart != pEnd)             // avoid zero length path
                                    {
                                        tilePath.Add(pEnd, item.path[i].Depth, 0); //Logger.Trace("   start!=end ID:{0} i:{1} at end start x:{2} y:{3}  end x:{4} y:{5}", item.Info.id, i, pStart.X, pStart.Y, pEnd.X, pEnd.Y);
                                    }
                                    if (origEnd != pEnd)			// path was clipped, store old path, start new path, becaue pen-up/down is needed
                                    {
                                        if (tilePath.path.Count > 1)
                                        { finalPathList.Add(tilePath); tilePath = new ItemPath(); }            // save prev path, clipped end position already added     

                                        if (pStart != pEnd)							// clipped path has a length, start new...
                                        {	tilePath = new ItemPath();
											tilePath.Info.CopyData(graphicItem.Info);              // preset global info for GROUP
                                                                                                   //							tilePath.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                                            tilePath.Info.SetGroupAttribute((int)GroupOptions.ByTile, tileID);  // string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
											if (actualDashArray.Length > 0)
											{   tilePath.dashArray = new double[actualDashArray.Length];
												actualDashArray.CopyTo(tilePath.dashArray, 0);
											}
										}
                                    }
                                }
                            }
                            if (tilePath.path.Count > 1)
                                finalPathList.Add(tilePath);
                        }
                        else
                        {
                            if (WithinRectangle(graphicItem.Start, clipMin, clipMax))        // else must be Dot
                            {
                                ItemDot dot = new ItemDot(graphicItem.Start.X, graphicItem.Start.Y);
                                dot.Info.CopyData(graphicItem.Info);              // preset global info for GROUP
                                                                                  //                dot.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                                dot.Info.SetGroupAttribute((int)GroupOptions.ByTile, string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
                                finalPathList.Add(dot);
                            }
                        }
                    }
                    tileOneDimension.addDimensionXY(tilePath.Dimension);
					
					if (log) ListGraphicObjects(finalPathList);

					if (Properties.Settings.Default.importGraphicClipOffsetApply)
						RemoveOffset(finalPathList, clipMin.X, clipMin.Y);
					
                    SortByDistance(finalPathList);                 // sort objects of current tile
					
                    foreach (PathObject tile in finalPathList)     // add tile to full graphic
                        tileGraphicAll.Add(tile);
							
					finalPathList = new List<PathObject>();		// 

                    if (Properties.Settings.Default.importGraphicClip)	// just clipping one tile: stop X
                        break;
                }	// for X
                if (Properties.Settings.Default.importGraphicClip)		// just clipping one tile: stop Y
                    break;
            }	// for Y
            completeGraphic.Clear();
            foreach (PathObject tile in tileGraphicAll)     // add tile to full graphic
                completeGraphic.Add(tile);          
        }
	
		private static string getTileCommand(int indexX, int indexY, double sizeX, double sizeY)
		{	string command = Properties.Settings.Default.importGraphicClipGCode;
			if (command.Contains("#INDX")) command = command.Replace("#INDX", string.Format("{0}", indexX));	// index X
			if (command.Contains("#INDY")) command = command.Replace("#INDY", string.Format("{0}", indexY));	// index Y
			if (command.Contains("#OFFX")) command = command.Replace("#OFFX", string.Format("{0:0.000}", (indexX*sizeX)));	// offset X
			if (command.Contains("#OFFY")) command = command.Replace("#OFFY", string.Format("{0:0.000}", (indexY*sizeY)));	// offset X	
            if ((logFlags & (uint)LogEnable.ClipCode) > 0) Logger.Trace("   getTileCommand {0}", command);

            return command;
		}
		
        private static bool WithinRectangle(Point a, Point min, Point max)
        {   if (a.X < min.X) return false;
            if (a.X > max.X) return false;
            if (a.Y < min.Y) return false;
            if (a.Y > max.Y) return false;
            return true;
        }
        private static bool WithinRectangle(Dimensions a, Point min, Point max)
        {   if (a.maxx < min.X) return false;
            if (a.minx > max.X) return false;
            if (a.maxy < min.Y) return false;
            if (a.miny > max.Y) return false;
            return true;
        }

        // adaption of code example: https://de.wikipedia.org/wiki/Algorithmus_von_Cohen-Sutherland
        private enum ClipType { CLIPLEFT=1, CLIPRIGHT=2, CLIPLOWER=4, CLIPUPPER=8};  //        
        private static bool ClipLine(ref Point p1, ref Point p2, Point clipMin, Point clipMax)
        {
            int K1=0,K2=0;             // Variablen mit je 4 Flags für rechts, links, oben, unten.
            double dx,dy;

            dx = p2.X - p1.X;                 // Die Breite der Linie vorberechnen für spätere Koordinaten Interpolation
            dy = p2.Y - p1.Y;                 // Die Höhe   der Linie vorberechnen für spätere Koordinaten Interpolation

            if (p1.Y < clipMin.Y) K1 = (int)ClipType.CLIPLOWER;  // Ermittle, wo der Anfangspunkt außerhalb des Clip Rechtecks liegt.
            if (p1.Y > clipMax.Y) K1 = (int)ClipType.CLIPUPPER;  // Es werden entweder CLIPLOWER oder CLIPUPPER und/oder CLIPLEFT oder CLIPRIGHT gesetzt
            if (p1.X < clipMin.X) K1|= (int)ClipType.CLIPLEFT;   // Es gibt 8 zu clippende Kombinationen, je nachdem in welchem der 8 angrenzenden
            if (p1.X > clipMax.X) K1|= (int)ClipType.CLIPRIGHT;  // Bereiche des Clip Rechtecks die Koordinate liegt.

            if (p2.Y < clipMin.Y) K2 = (int)ClipType.CLIPLOWER;  // Ermittle, wo der Endpunkt außerhalb des Clip Rechtecks liegt.
            if (p2.Y > clipMax.Y) K2 = (int)ClipType.CLIPUPPER;  // Wenn keines der Flags gesetzt ist, dann liegt die Koordinate
            if (p2.X < clipMin.X) K2|= (int)ClipType.CLIPLEFT;   // innerhalb des Rechtecks und muss nicht geändert werden.
            if (p2.X > clipMax.X) K2|= (int)ClipType.CLIPRIGHT;

            // Schleife nach Cohen Sutherland, die maximal zweimal durchlaufen wird
            bool pEndChanged = K2 > 0;
            while((K1 > 0) || (K2 > 0))    // muss wenigstens eine der Koordinaten irgendwo geclippt werden ?
            {
                if ((K1 & K2) > 0)     // logisches AND der beiden Koordinaten Flags ungleich 0 ?
                    return false;  // beide Punkte liegen jeweils auf der gleichen Seite außerhalb des Rechtecks

                if (K1 > 0)                       // schnelle Abfrage, muss Koordinate 1 geclippt werden ?
                {
                    if ((K1 & (int)ClipType.CLIPLEFT) > 0)          // ist Anfangspunkt links außerhalb ?
                    {   p1.Y+=(clipMin.X-p1.X)*dy/dx;  p1.X=clipMin.X; }
                    else if ((K1 & (int)ClipType.CLIPRIGHT) > 0)   // ist Anfangspunkt rechts außerhalb ?
                    {   p1.Y+=(clipMax.X-p1.X)*dy/dx;  p1.X=clipMax.X; }
                    if ((K1 & (int)ClipType.CLIPLOWER) > 0)        // ist Anfangspunkt unterhalb des Rechtecks ?
                    {   p1.X+=(clipMin.Y-p1.Y)*dx/dy;  p1.Y=clipMin.Y; }
                    else if ((K1 & (int)ClipType.CLIPUPPER) > 0)   // ist Anfangspunkt oberhalb des Rechtecks ?
                    {   p1.X+=(clipMax.Y-p1.Y)*dx/dy;  p1.Y=clipMax.Y; }
                    K1 = 0;                    // Erst davon ausgehen, dass der Punkt jetzt innerhalb liegt,
                                 // falls das nicht zutrifft, wird gleich korrigiert.
                    if(p1.Y<clipMin.Y) K1 = (int)ClipType.CLIPLOWER; // 4 ermittle erneut, wo der Anfangspunkt jetzt außerhalb des Clip Rechtecks liegt
                    if(p1.Y>clipMax.Y) K1 = (int)ClipType.CLIPUPPER; // 8 Für einen Punkt, bei dem im ersten Durchlauf z.&nbsp;B. CLIPLEFT gesetzt war,
                    if(p1.X<clipMin.X) K1|= (int)ClipType.CLIPLEFT;  // 1 kann im zweiten Durchlauf z.&nbsp;B. CLIPLOWER gesetzt sein
                    if(p1.X>clipMax.X) K1|= (int)ClipType.CLIPRIGHT; // 2
                }

                if ((K1 & K2) > 0)     // logisches AND der beiden Koordinaten Flags ungleich 0 ?
                    return false;  // beide Punkte liegen jeweils auf der gleichen Seite außerhalb des Rechtecks

                if (K2 > 0)                       // schnelle Abfrage, muss Koordinate 2 geclippt werden ?
                {                            // ja
                    if ((K2 & (int)ClipType.CLIPLEFT) > 0)         // liegt die Koordinate links außerhalb des Rechtecks ?
                    {   p2.Y+=(clipMin.X-p2.X)*dy/dx;  p2.X=clipMin.X;  }
                    else if ((K2 & (int)ClipType.CLIPRIGHT) > 0)   // liegt die Koordinate rechts außerhalb des Rechtecks ?
                    {   p2.Y+=(clipMax.X-p2.X)*dy/dx;  p2.X=clipMax.X;  }
                    if ((K2 & (int)ClipType.CLIPLOWER) > 0)        // liegt der Endpunkt unten außerhalb des Rechtecks ?
                    {   p2.X+=(clipMin.Y-p2.Y)*dx/dy;  p2.Y=clipMin.Y;  }
                    else if ((K2 & (int)ClipType.CLIPUPPER) > 0)    // liegt der Endpunkt oben außerhalb des Rechtecks ?
                    {   p2.X+=(clipMax.Y-p2.Y)*dx/dy;  p2.Y=clipMax.Y;  }
                    K2 = 0;                    // Erst davon ausgehen, dass der Punkt jetzt innerhalb liegt,
                                 // falls das nicht zutrifft, wird gleich korrigiert.
                    if(p2.Y<clipMin.Y) K2 = (int)ClipType.CLIPLOWER; // ermittle erneut, wo der Endpunkt jetzt außerhalb des Clip Rechtecks liegt
                    if(p2.Y>clipMax.Y) K2 = (int)ClipType.CLIPUPPER; // Ein Punkt, der z.&nbsp;B. zuvor über dem Clip Rechteck lag, kann jetzt entweder
                    if(p2.X<clipMin.X) K2|= (int)ClipType.CLIPLEFT;  // rechts oder links davon liegen. Wenn er innerhalb liegt wird kein
                    if(p2.X>clipMax.X) K2|= (int)ClipType.CLIPRIGHT; // Flag gesetzt.
                }
            }             // Ende der while Schleife, die Schleife wird maximal zweimal durchlaufen.
            return true;  // jetzt sind die Koordinaten geclippt und die gekürzte Linie kann gezeichnet werden
        }
        #endregion

        #region remove short moves
        public static void RemoveShortMoves(List<PathObject> graphicToOffset, double minDistance)
        {
            if (logEnable) Logger.Trace("...RemoveOffset before min X:{0:0.00} Y:{1:0.00} --------------------------------------", actualDimension.minx, actualDimension.miny);
            foreach (PathObject item in graphicToOffset)    // dot or path
            {
                if (item is ItemPath)
                {
                    ItemPath PathData = (ItemPath)item;
                    Point lastPoint = PathData.End;
                    double distance = 0;
                    if (PathData.path.Count > 5)
                    {   
                        for (int i = (PathData.path.Count - 2); i > 1; i--)
                        {
                            if (PathData.path[i] is GCodeLine)
                                distance = Math.Sqrt(PointDistanceSquared(lastPoint, PathData.path[i].MoveTo));
                            else
                            {
                                distance = minDistance;
                //                CircleDistanceSquared(lastPoint, PathData.path[i]);
                //                distance = PathData.path[i].distance;
                            }

                            if (distance < minDistance)
                            { PathData.path.RemoveAt(i); }
                            else { lastPoint = PathData.path[i].MoveTo; }
                        }
                    }
                }
            }
        }
        #endregion

        #region remove offset
        public static void RemoveOffset(List<PathObject> graphicToOffset, double offsetX, double offsetY)
        {
            if (logEnable) Logger.Trace("...RemoveOffset before min X:{0:0.00} Y:{1:0.00} --------------------------------------", actualDimension.minx, actualDimension.miny);
            foreach (PathObject item in graphicToOffset)    // dot or path
            {
                item.Start = new Point(item.Start.X - offsetX, item.Start.Y - offsetY);
                item.End   = new Point(item.End.X - offsetX, item.End.Y - offsetY);
                if (item is ItemPath)
                {
                    ItemPath PathData = (ItemPath)item;
                    foreach (GCodeMotion entity in PathData.path)
                    {   entity.MoveTo = new Point(entity.MoveTo.X - offsetX, entity.MoveTo.Y - offsetY);  }
                    PathData.Dimension.offsetXY(-offsetX, -offsetY);
                }
            }
            actualDimension.offsetXY(-offsetX, -offsetY);
        }
		# endregion
		
        public static void ScaleXY(double scaleX, double scaleY)	// scaleX != scaleY will not work for arc!
        {   Scale(completeGraphic, scaleX, scaleY); }
        public static void Scale(List<PathObject> graphicToOffset, double scaleX, double scaleY)
        {
            if (logEnable) Logger.Trace("...Scale scaleX:{0:0.00} scaleY:{1:0.00} ", scaleX, scaleY);
            foreach (PathObject item in graphicToOffset)    // dot or path
            {
                item.Start = new Point(item.Start.X * scaleX, item.Start.Y * scaleY);
                item.End = new Point(item.End.X * scaleX, item.End.Y * scaleY);
                if (item is ItemPath)
                {
                    ItemPath PathData = (ItemPath)item;
                    if (logEnable) Logger.Trace("  ID:{0} Geo:{1} Size:{2}", item.Info.id, item.Info.pathGeometry, PathData.path.Count);
                    foreach (GCodeMotion entity in PathData.path)
                    {   entity.MoveTo = new Point(entity.MoveTo.X * scaleX, entity.MoveTo.Y * scaleY);  }
                    PathData.Dimension.scaleXY(scaleX, scaleY);
                }
            }
        }

        public static void RepeatPaths(List<PathObject> graphicToRepeat, int repetitions)
        {
            if (logEnable) Logger.Trace("...RepeatPaths({0})",repetitions);
            if (logDetailed) ListGraphicObjects(graphicToRepeat, true);

            List<PathObject> repeatedGraphic = new List<PathObject>();

            foreach (PathObject item in graphicToRepeat)      // replace original list
			{	for(int i=0; i < repetitions; i++)
				{	 repeatedGraphic.Add(item.Copy());  	}
			}
			
            graphicToRepeat.Clear();
            foreach (PathObject item in repeatedGraphic)      // replace original list
                graphicToRepeat.Add(item);

            repeatedGraphic.Clear();
			if (logDetailed) ListGraphicObjects(graphicToRepeat,true);
        }

		public static void ExtendClosedPaths(List<PathObject> graphicToExtend, double extensionOrig)
		{	
			//const uint loggerSelect = (uint)LogEnable.PathModification;
			bool log = logEnable && ((logFlags & (uint)LogEnable.ClipCode) > 0);

            double dx,dy,newX,newY,length,maxElements;
			double extension;
            if (logEnable) Logger.Trace("...ExtendClosedPaths extend:{0}", extensionOrig);

			foreach(PathObject item in graphicToExtend)
			{
                if (item is ItemPath)
                {
                    ItemPath PathData = (ItemPath)item; // is closed polygon?

                    if (isEqual(PathData.Start, PathData.End))      //(PathData.Start.X == PathData.End.X) && (PathData.Start.Y == PathData.End.Y))
                    {
						if (PathData.path.Count > 1)
						{	// first line could be shorter than needed extension
							// Loop
							int index = 1;
                            maxElements = PathData.path.Count;

                            extension = Math.Abs(extensionOrig);
                            while (extension > 0)
							{
                                if (PathData.path[index] is GCodeLine)
								{
									dx = PathData.path[index].MoveTo.X - PathData.path[index-1].MoveTo.X; 
									dy = PathData.path[index].MoveTo.Y - PathData.path[index-1].MoveTo.Y; 
									length = Math.Sqrt(dx*dx + dy*dy);
									if (log) Logger.Trace("    {0} Line length:{1:0.00}  extend:{2:0.00}", PathData.Info.id, length, extension);
									if ((extension - length) >= 0)
									{	extension -= length;
										PathData.Add(PathData.path[index].MoveTo, PathData.path[index].Depth, 0 );
										goto NextElement;
									}
									// interpolate new pos.
									newX = PathData.path[index-1].MoveTo.X + dx * extension / length;
									newY = PathData.path[index-1].MoveTo.Y + dy * extension / length;
									PathData.Add(new Point(newX, newY), PathData.path[index].Depth, 0 );
									break;
								}
								
								else 	// is Arc
								{	GCodeArc ArcData = (GCodeArc)PathData.path[index];
									ArcProperties arcProp = gcodeMath.getArcMoveProperties(PathData.path[index-1].MoveTo, ArcData.MoveTo, ArcData.CenterIJ, ArcData.IsCW); // in radians
									double circum = Math.Abs(arcProp.angleDiff * arcProp.radius);
                                    if (log) Logger.Trace("      diff:{0:0.00}  radius:{1:0.00}    circ:{2:0.00}", arcProp.angleDiff, arcProp.radius, circum);

                                    if (log) Logger.Trace("    {0} Arc  circum:{1:0.00}  extend:{2:0.00}", PathData.Info.id, circum, extension);
									if ((extension - circum) >= 0)
									{	extension -= circum;
										PathData.AddArc(ArcData.MoveTo, ArcData.CenterIJ, ArcData.IsCW, ArcData.Depth, ArcData.AngleStart, ArcData.Angle);
										goto NextElement;
									}
									// interpolate new pos.
									double angleNewDiff = arcProp.angleDiff * extension / circum;
									double angleEnd = arcProp.angleStart + angleNewDiff;
									newX = arcProp.center.X + arcProp.radius * Math.Cos(angleEnd);
									newY = arcProp.center.Y + arcProp.radius * Math.Sin(angleEnd);
                                    if (log) Logger.Trace("      diff:{0:0.00}  new diff:{1:0.00}    end:{2:0.00}  ext.{3}", arcProp.angleDiff, angleNewDiff, angleEnd, extension);

                                    PathData.AddArc(new Point(newX, newY), ArcData.CenterIJ, ArcData.IsCW, ArcData.Depth, ArcData.AngleStart, ArcData.Angle);
									break;
								}
								
							NextElement:
								index++;
								if (index >= maxElements)
									break;
							}							
						}
					}
				}				
			}			
		}

        public static void SortByDistance(List<PathObject> graphicToSort, bool preventReversal = false)
        {
            if (logEnable) Logger.Trace("...SortByDistance() count:{0}",graphicToSort.Count);

            List<PathObject> sortedGraphic = new List<PathObject>();
            Point actualPos = new Point(0,0);
            double distanceReverse;
            bool allowReverse = false;
            PathObject tmp;
            bool closedPathRotate = false;
            if (!graphicInformation.OptionTangentialAxis)   // angle calc is not ok
            {   allowReverse = true;
                closedPathRotate = Properties.Settings.Default.importGraphicSortDistanceAllowRotate;
            }

            while (graphicToSort.Count > 0)                       // items will be removed step by step from completeGraphic
            {
//                Logger.Trace("   Start point x:{0:0.00}  y:{1:0.00}", actualPos.X, actualPos.Y);
                for (int i = 0; i < graphicToSort.Count; i++)     // calculate distance to all remaining items check start and end position
                {   tmp = graphicToSort[i];
                    tmp.Distance = PointDistanceSquared(actualPos, tmp.Start);
                    if (tmp is ItemPath)
                    {
/* if closed path, find closest point */
                        if (isEqual(tmp.Start, tmp.End))
                        {
                            if (closedPathRotate && !preventReversal)
                            {
                                ItemPath tmpItemPath = (ItemPath)tmp;
                                if (tmpItemPath.path.Count > 2)
                                {   PathDistanceSquared(actualPos, tmpItemPath);
                                    int index = tmp.TmpIndex;   //(int)tmp.StartAngle;
                                    if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  path index:{2}   distance:{3:0.00}", tmp.Info.id, tmp.Info.pathGeometry, index, tmp.Distance);
                                    if ((index >= 0) && (index < tmpItemPath.path.Count))
                                    { tmp.Start = tmp.End = tmpItemPath.path[tmp.TmpIndex].MoveTo; } //(int)tmp.StartAngle
                                }
                                else if (tmpItemPath.path.Count == 2)       // is circle
                                {   if (tmpItemPath.path[1] is GCodeArc)
                                    {
                                        CircleDistanceSquared(actualPos, tmpItemPath);
                                        if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  angle:{2:0.0}  distance:{3:0.00}", tmp.Info.id, tmp.Info.pathGeometry, (tmp.StartAngle*180/Math.PI), tmp.Distance);
                                    }
                                }
                            }
                        }
                        else
                        {
/* if other end is closer, reverse path */
                            distanceReverse = PointDistanceSquared(actualPos, tmp.End);
                            if (allowReverse && !preventReversal && !(isEqual(tmp.Start, tmp.End)) && (distanceReverse < tmp.Distance))
                            {
                                tmp.Distance = distanceReverse;
                                ((ItemPath)tmp).Reversed = !((ItemPath)tmp).Reversed;
                                Point start = tmp.Start;            //##################### new Point
                                tmp.Start = tmp.End;
                                tmp.End = start;
                            }
                            if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  distance:{2:0.00}", tmp.Info.id, tmp.Info.pathGeometry, tmp.Distance);
                        }
                        graphicToSort[i] = tmp;
                    }
                }
                graphicToSort.Sort((x, y) => x.Distance.CompareTo(y.Distance));   // sort by distance

                sortedGraphic.Add(graphicToSort[0]);       	// get closest item = first in list
                actualPos = graphicToSort[0].End;         	// set new start pos
                if (logSortMerge) Logger.Trace("   remove id:{0} ", graphicToSort[0].Info.id);
                graphicToSort.RemoveAt(0);                  // remove item from remaining list
            }

            PathObject sortedItem;
            for (int i=0; i < sortedGraphic.Count; i++)     // loop through all items
            {   sortedItem = sortedGraphic[i];

/* now rotate path-points */
                if ((sortedItem is ItemPath) && (closedPathRotate) && (isEqual(sortedItem.Start, sortedItem.End)))
                {   RotatePath((ItemPath)sortedItem); }            // finally reverse path if needed

/* now reverse path */
                if ((sortedItem is ItemPath) && ((ItemPath)sortedItem).Reversed)
                {   ReversePath((ItemPath)sortedItem); }            // finally reverse path if needed
            }

            graphicToSort.Clear();
            foreach (PathObject item in sortedGraphic)      // replace original list
                graphicToSort.Add(item);

            sortedGraphic.Clear();
            if (logEnable) Logger.Trace("...SortByDistance()  finish");
        }
        private static double PointDistanceSquared(Point a, Point b)	// avoid square-root, to save time
        {   double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (dx * dx + dy * dy);	// speed up
        }
        private static void CircleDistanceSquared(Point a, ItemPath tmp)
        {
            ArcProperties arcMove = gcodeMath.getArcMoveProperties((xyPoint)tmp.path[0].MoveTo, (xyPoint)tmp.path[1].MoveTo, ((GCodeArc)tmp.path[1]).CenterIJ.X, ((GCodeArc)tmp.path[1]).CenterIJ.Y, ((GCodeArc)tmp.path[1]).IsCW);
            double aLine = gcodeMath.getAlpha( toPointF(arcMove.center), a);
            Point ptmp = new Point();
            ptmp.X = arcMove.center.X + arcMove.radius * Math.Cos(aLine);
            ptmp.Y = arcMove.center.Y + arcMove.radius * Math.Sin(aLine);

            double distance = PointDistanceSquared(a, ptmp);
//            Logger.Trace("       circle angle:{0:0.00}  radius:{1:0.00}  x:{2:0.00} y:{3:0.00}  distance:{4:0.00}  ", (aLine*180/Math.PI), arcMove.radius, ptmp.X, ptmp.Y, distance);

            tmp.Distance = distance;
            tmp.StartAngle = aLine;
        }
        private static void PathDistanceSquared(Point a, ItemPath tmp)
        {
            if (tmp.path.Count < 2)
                return;
            double distance = PointDistanceSquared(a, tmp.path[0].MoveTo);
            double distTemp;
            int i,index=0;
            for (i = 0; i < tmp.path.Count; i++)
            {   distTemp = PointDistanceSquared(a, tmp.path[i].MoveTo);
                if (distTemp < distance)
                {   distance = distTemp;
                    index = i;
                }
//                Logger.Trace("       path a.x:{0:0.00}  a.y:{1:0.00}  distance:{2:0.00}   i:{3}  index:{4}", a.X, a.Y, distTemp, i, index);
            }
            tmp.Distance = distance;
            tmp.TmpIndex = index;     //tmp.StartAngle = index;
//            Logger.Trace("       path a.x:{0:0.00}  a.y:{1:0.00}  distance:{2:0.00}   i:{3}", a.X, a.Y, distance, index);
        }
        private static Point toPointF(xyPoint tmp)
        { return new Point((float)tmp.X, (float)tmp.Y); }

        private static bool hasSameProperties(ItemPath a, ItemPath b)
        {   bool sameProperties = true;
//            bool checkAll = false;// true;
            if (graphicInformation.GroupEnable)
            {  
                for (int i = 1; i <= 5; i++)    // GroupOptions { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByType = 4, ByTile = 5};
                {   if (logDetailed) Logger.Trace("  hasSameProperties - GroupEnable-Option:{0} a:'{1}'  b:'{2}'", i, a.Info.groupAttributes[i], b.Info.groupAttributes[i]);
                    sameProperties = a.Info.groupAttributes[i] == b.Info.groupAttributes[i];
                    if (!sameProperties)
                        return false;
                }
			}
            bool sameDash = true;
            if (Properties.Settings.Default.importLineDashPattern)
            {
//                Logger.Trace("  hasSameProperties a-count:{0} dash-a:{1}  b-count:{2} dash-b:{3}", a.dashArray.Count(), showArray(a.dashArray), b.dashArray.Count(), showArray(b.dashArray));
                sameDash = false;
                if ((a.dashArray.Count() == 0) && (a.dashArray.Count() == 0))
                    sameDash = true;
                else if (a.dashArray == b.dashArray)
                    sameDash = true;
            }
			return (sameProperties && sameDash);
		}
        private static string showArray(double[] tmp)
        {   string tmps = "";
            foreach (double nr in tmp)
                tmps += string.Format("{0:0.00} ", nr);
            return tmps;
        }
        private static void MergeFigures(List<PathObject> graphicToMerge)
        {
            if (logEnable) Logger.Trace("...MergeFigures before:{0}    ------------------------------------", graphicToMerge.Count);

            int i =0,k=0;

			for (i = graphicToMerge.Count-1; i > 0; i--)
			{
                if (graphicToMerge[i] is ItemDot)
                    continue;

                else if (graphicToMerge[i] is ItemPath)
                {   if (isEqual(graphicToMerge[i].Start, graphicToMerge[i].End))    // closed path can't be merged
                        continue;
                    if (((ItemPath)graphicToMerge[i]).path.Count <= 1)              // 2020-08-10
                        continue;

                    for (k = 0; k < graphicToMerge.Count; k++)
                    {   if ((k == i) || (i >= graphicToMerge.Count))
                            continue;
                        if (graphicToMerge[k] is ItemDot)                           // 2020-08-10
                            continue;
                        if (((ItemPath)graphicToMerge[k]).path.Count <= 1)          // 2020-08-10
                            continue;

                        //                        if (loggerTrace > 0) Logger.Trace("  i:{0}  k:{1}   count:{2}", i, k, graphicToMerge.Count);
                        if (graphicToMerge[k] is ItemPath)
                        {
						// closed path can't be merged
                            if (isEqual(graphicToMerge[k].Start, graphicToMerge[k].End))  
                                continue;

                            // paths with different propertys should not be merged
                            if (!hasSameProperties((ItemPath)graphicToMerge[i], (ItemPath)graphicToMerge[k]))
                            {   //                             Logger.Trace("  not same Properties");
                                continue;
                            }

						// i-start = k-end -> just connect paths (i at the end of k)
                            if (isEqual(graphicToMerge[i].Start, graphicToMerge[k].End))
                            {
              //                  if (loggerTrace) Logger.Trace("   1) merge:{0} start  x1:{1} y1:{2}  addto:{3} end x:{4}  y:{5}", i, graphicToMerge[i].Start.X, graphicToMerge[i].Start.Y, k, graphicToMerge[k].End.X, graphicToMerge[k].End.Y);
                                MergePaths((ItemPath)graphicToMerge[k], (ItemPath)graphicToMerge[i]);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
						// i-end = k-end -> reverse i, then connect paths (i at the end of k)
                            else if (isEqual(graphicToMerge[i].End, graphicToMerge[k].End))
                            {
            //                    if (loggerTrace) Logger.Trace("   2) merge:{0} end  x1:{1} y1:{2}  addto:{3} end x:{4}  y:{5}", i, graphicToMerge[i].End.X, graphicToMerge[i].End.Y, k, graphicToMerge[k].End.X, graphicToMerge[k].End.Y);
                                ReversePath((ItemPath)graphicToMerge[i]);
                                MergePaths((ItemPath)graphicToMerge[k], (ItemPath)graphicToMerge[i]);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
                            // i-start = k-start -> reverse k, then connect paths (i at the end of k)
                            else if (isEqual(graphicToMerge[i].Start, graphicToMerge[k].Start))
                            {
            //                    if (loggerTrace) Logger.Trace("   3) merge:{0} start  x1:{1} y1:{2}  addto:{3} start x:{4}  y:{5}", i, graphicToMerge[i].Start.X, graphicToMerge[i].Start.Y, k, graphicToMerge[k].Start.X, graphicToMerge[k].Start.Y);
                                ReversePath((ItemPath)graphicToMerge[k]);
                                MergePaths((ItemPath)graphicToMerge[k], (ItemPath)graphicToMerge[i]);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
                            // i-end = k-start -> reverse i and k, then connect paths (i at the end of k)
                            else if (isEqual(graphicToMerge[i].End, graphicToMerge[k].Start))
                            {
            //                    if (loggerTrace) Logger.Trace("   4) merge:{0} end  x1:{1} y1:{2}  addto:{3} start x:{4}  y:{5}", i, graphicToMerge[i].End.X, graphicToMerge[i].End.Y, k, graphicToMerge[k].Start.X, graphicToMerge[k].Start.Y);
                                ReversePath((ItemPath)graphicToMerge[i]);
                                ReversePath((ItemPath)graphicToMerge[k]);
                                MergePaths((ItemPath)graphicToMerge[k], (ItemPath)graphicToMerge[i]);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
   			}
            if (logDetailed) ListGraphicObjects(graphicToMerge, true);
            if (logEnable) Logger.Trace("...MergeFigures after :{0}    ------------------------------------", graphicToMerge.Count);
        }
        public static bool isEqual(System.Windows.Point a, System.Windows.Point b)
        {   return ((Math.Abs(a.X - b.X) < equalPrecision) && (Math.Abs(a.Y - b.Y) < equalPrecision));    }
        private static void MergePaths(ItemPath addAtEnd, ItemPath toMerge)
        {
            if (logDetailed) Logger.Trace(".....MergePaths ID:{0} {1}     ID:{2} {3}", addAtEnd.Info.id, addAtEnd.Info.pathGeometry, toMerge.Info.id, toMerge.Info.pathGeometry);
            toMerge.path.RemoveAt(0);   // avoid double coordinate (start = end)
            addAtEnd.path = addAtEnd.path.Concat(toMerge.path).ToList();
            addAtEnd.Dimension.addDimensionXY(toMerge.Dimension);
			addAtEnd.End = toMerge.End;
//            addAtEnd.Info.pathGeometry += ".";// " Merged " + toMerge.Info.id.ToString();
        }

        /*
                private static void SetDotOnly()
                {   foreach (PathObject item in completeGraphic)        // replace original list
                    {   foreach (grblMotion path in item.path)
                        {   path.lineType = MotionType.dot; }
                    }           
                }*/


        private static void RotatePath(ItemPath item)
        {
            List<GCodeMotion> rotatedPath = new List<GCodeMotion>();
            GCodeMotion tmpMove;
            int i, iStart = item.TmpIndex;  // (int)item.StartAngle;
            int listIndex = 0;
            if (logSortMerge) Logger.Trace(".....RotatePath ID:{0}  index:{1}  count:{2}", item.Info.id, iStart, item.path.Count);

            if (logSortMerge && logDetailed)
            {   Logger.Trace("     before rotation");
                if (item.path.Count < listIndex)
                { for (i = 0; i < item.path.Count; i++)
                    { if (item.path[i] is GCodeLine)
                            Logger.Trace("      index:{0} Line  x:{1:0.00}  y:{2:0.00}", i, item.path[i].MoveTo.X, item.path[i].MoveTo.Y);
                        else
                            Logger.Trace("      index:{0} Arc   x:{1:0.00}  y:{2:0.00}  i:{3:0.00}  j:{4:0.00}", i, item.path[i].MoveTo.X, item.path[i].MoveTo.Y, ((GCodeArc)item.path[i]).CenterIJ.X, ((GCodeArc)item.path[i]).CenterIJ.Y);
                    }
                }
            }

            if ((iStart >= 0) && (iStart < item.path.Count))
                item.Start = item.End = item.path[iStart].MoveTo;       // restore start/end position

            if (item.path.Count == 2)       // is circle
            {
                if (item.path[1] is GCodeArc)
                {
                    ArcProperties arcMove = gcodeMath.getArcMoveProperties((xyPoint)item.path[0].MoveTo, (xyPoint)item.path[1].MoveTo, ((GCodeArc)item.path[1]).CenterIJ.X, ((GCodeArc)item.path[1]).CenterIJ.Y, ((GCodeArc)item.path[1]).IsCW);
                    Point ptmp = new Point();
                    Point pij = new Point();
                    ptmp.X = arcMove.center.X + arcMove.radius * Math.Cos(item.StartAngle);
                    ptmp.Y = arcMove.center.Y + arcMove.radius * Math.Sin(item.StartAngle);

                    pij.X = arcMove.center.X - ptmp.X;
                    pij.Y = arcMove.center.Y - ptmp.Y;
                    item.Start = item.path[0].MoveTo = ptmp;
                    item.End   = item.path[1].MoveTo = ptmp;
                    ((GCodeArc)item.path[1]).CenterIJ = pij;
          //          Logger.Trace("       circle angle:{0:0.00}  radius:{1:0.00}  x:{2:0.00} y:{3:0.00}  distance:{4:0.00}  ", aLine, arcMove.radius, ptmp.X, ptmp.Y, distance);
                    rotatedPath.Add(item.path[0]);
                    rotatedPath.Add(item.path[1]);
                }
            }
            else
            {
//                if ((item.StartAngle == 0) || (iStart >= item.path.Count))           // nothing to rotate
                if ((item.TmpIndex == 0) || (iStart >= item.path.Count))           // nothing to rotate
                    return;

                tmpMove = new GCodeLine(item.Start);     	// 
                rotatedPath.Add(tmpMove);

                for (i = iStart + 1; i < item.path.Count; i++)
                { rotatedPath.Add(item.path[i]); }
                for (i = 1; i <= iStart; i++)
                { rotatedPath.Add(item.path[i]); }
            }

            //item.StartAngle = 0;
            item.TmpIndex = 0;

            /* replace original path */
            item.path.Clear();
            foreach (GCodeMotion ent in rotatedPath)
                item.path.Add(ent);

            if (logSortMerge) 
            {   if (item.path.Count < listIndex)
                {   Logger.Trace("     after rotation");
                    for (i = 0; i < item.path.Count; i++)
                    {   if (item.path[i] is GCodeLine)
                            Logger.Trace("      Line i:{0}   x:{1:0.00}  y:{2:0.00}", i, item.path[i].MoveTo.X, item.path[i].MoveTo.Y);
                        else
                            Logger.Trace("      Arc  i:{0}   x:{1:0.00}  y:{2:0.00}  i:{3:0.00}  j:{4:0.00}", i, item.path[i].MoveTo.X, item.path[i].MoveTo.Y, ((GCodeArc)item.path[i]).CenterIJ.X, ((GCodeArc)item.path[i]).CenterIJ.Y);
                    }
                }
            }
        }

        private static void ReversePath( ItemPath item)
        {   List<GCodeMotion> reversedPath = new List<GCodeMotion>();
            if (logDetailed) Logger.Trace(".....ReversePath ID:{0} {1}",item.Info.id, item.Info.pathGeometry);
            GCodeMotion tmpMove;

            if (item.isReversed)	// indicatior if Start/End was already switched (in SortCode()
				tmpMove = new GCodeLine(item.Start, item.path[item.path.Count-1].Depth);     	// first reversed point is End of original (but original is already switched in SortCode() -> use start)
			else
				tmpMove = new GCodeLine(item.End, item.path[item.path.Count-1].Depth);     	// first reversed point is End of original 
			
            reversedPath.Add(tmpMove);

            for (int i = item.path.Count - 2; i >= 0; i--)              // -1 start from rear, with item one before last
            {   if (item.path[i+1] is GCodeLine)         // isLine
                {   tmpMove = new GCodeLine(item.path[i].MoveTo, item.path[i].Depth);
                    reversedPath.Add(tmpMove); 
                }
                else
                {   double cx = item.path[i].MoveTo.X + ((GCodeArc)item.path[i+1]).CenterIJ.X;       // arc is a bit more work
                    double cy = item.path[i].MoveTo.Y + ((GCodeArc)item.path[i+1]).CenterIJ.Y;
                    Point center = new Point(cx, cy);
                    double newi = cx - item.path[i+1].MoveTo.X;
                    double newj = cy - item.path[i+1].MoveTo.Y;
                    tmpMove = new GCodeArc(item.path[i].MoveTo, new Point(newi, newj), !((GCodeArc)item.path[i + 1]).IsCW, item.path[i + 1].Depth);
                    reversedPath.Add(tmpMove);
                }
            }
            item.path.Clear();
            foreach (GCodeMotion ent in reversedPath)
                item.path.Add(ent);
				
			if (!item.isReversed)				// indicatior if Start/End was already switched (in SortCode()
            {	Point tmp = item.End;		// if not, do now
				item.End = item.Start;
				item.Start = tmp;
			}
			item.isReversed = false;			// reset indicator
        }

        #endregion


		private static void setOrderOfFigures(List<PathObject> tmp, int[] sortOrder)
		{   tileGraphicAll = new List<PathObject>();    // 
			foreach(int figureId in sortOrder)
			{	foreach (PathObject graphicItem in tmp)
				{	if (graphicItem.FigureId == figureId)
					{	tileGraphicAll.Add(graphicItem);
                        if (logSortMerge) Logger.Trace(" setOrder {0}", figureId);
                    }
				}
			}
			tmp.Clear();
            foreach (PathObject fig in tileGraphicAll)     
                tmp.Add(fig);          
		}


        public static string ReDoReversePath(int figureNr, xyPoint aP, int[] sortOrderFigure, int[] sortOrderGroup)	// called in MainFormPictureBox
        {
            Logger.Info("ReDoReversePath  figure:{0}  posX:{1:0.00}  posY:{2:0.00}   ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄",figureNr, aP.X,aP.Y);

            if (graphicInformation.GroupEnable && groupedGraphic.Count > 0)
            { foreach (GroupObject tmpGrp in groupedGraphic)
                {
                    for (int i = 0; i < tmpGrp.groupPath.Count; i++)
                    { if (tmpGrp.groupPath[i].FigureId == figureNr)
                        { if (tmpGrp.groupPath[i] is ItemPath)
                            { ReverseOrRotatePath((ItemPath)tmpGrp.groupPath[i], aP); }
                        }
                    }
                    setOrderOfFigures(tmpGrp.groupPath, sortOrderFigure);     // restore order from GCode
                }


                List<GroupObject> groupedGraphicTmp = new List<GroupObject>();
                foreach (int groupId in sortOrderGroup)
                {   foreach (GroupObject tmpGrp in groupedGraphic)
                    {   if (tmpGrp.GroupId == groupId)
                        {   groupedGraphicTmp.Add(tmpGrp);
                            if (logSortMerge) Logger.Trace(" setOrder {0}", groupId);
                        }
                    }
                }
                groupedGraphic.Clear();
                foreach (GroupObject tmpGrp in groupedGraphicTmp)
                    groupedGraphic.Add(tmpGrp);

     //           List<int> codeValueSortOrder = new List<int>(sortOrderGroup);
    //            for (int k = 0; k < sortOrderGroup.Count(); k++)
    //                Logger.Trace("sortOrderGroup {0}  ->  {1}", k, sortOrderGroup[k]);
	//			groupedGraphic.OrderBy(i=> codeValueSortOrder.IndexOf(i.GroupId));
                for (int k = 0; k < groupedGraphic.Count(); k++)
                    Logger.Trace("sortOrderGroup {0}  ->  {1}", k, groupedGraphic[k].GroupId);
            }
            else
			{	foreach (PathObject tmp in completeGraphic)
				{   if (tmp.FigureId == figureNr)
					{	if (tmp is ItemPath)
						{	ReverseOrRotatePath((ItemPath)tmp, aP);		}
					}
				}
				setOrderOfFigures(completeGraphic, sortOrderFigure);		// restore order from GCode
			}
			graphicInformation.ReProcess = true;			

		    if (graphicInformation.GroupEnable)	// group objects by color/width/layer/tile-nr 
			{		
				return Graphic2GCode.CreateGCode(groupedGraphic, headerInfo, graphicInformation);	// Grouped graphic
			}
			return Graphic2GCode.CreateGCode(completeGraphic, headerInfo, graphicInformation);			// complete graphic
        }
		
		private static void ReverseOrRotatePath(ItemPath tmpItemPath, xyPoint aP)
		{
			//ItemPath tmpItemPath = (ItemPath)tmp;
			if (isEqual(tmpItemPath.Start, tmpItemPath.End))
			{
				/* find closest point*/
				Point actualPos = new Point(aP.X,aP.Y);
				if (tmpItemPath.path.Count > 2)
				{
					PathDistanceSquared(actualPos, tmpItemPath);
                    int index = tmpItemPath.TmpIndex;   // (int)tmpItemPath.StartAngle;
					if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  path index:{2}   distance:{3:0.00}", tmpItemPath.Info.id, tmpItemPath.Info.pathGeometry, index, tmpItemPath.Distance);
					if ((index >= 0) && (index < tmpItemPath.path.Count))
					{ tmpItemPath.Start = tmpItemPath.End = tmpItemPath.path[tmpItemPath.TmpIndex].MoveTo; } //(int)tmpItemPath.StartAngle
                }
				else if (tmpItemPath.path.Count == 2)       // is circle
				{
					if (tmpItemPath.path[1] is GCodeArc)
					{
						CircleDistanceSquared(actualPos, tmpItemPath);
						if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  angle:{2:0.0}  distance:{3:0.00}", tmpItemPath.Info.id, tmpItemPath.Info.pathGeometry, (tmpItemPath.StartAngle * 180 / Math.PI), tmpItemPath.Distance);
					}
				}

				RotatePath(tmpItemPath);
			}
			else
				ReversePath(tmpItemPath);
						
		}
    }
}