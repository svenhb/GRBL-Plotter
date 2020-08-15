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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Globalization;

namespace GRBL_Plotter
{

    public static partial class Graphic
    {
        /// <summary>
        /// General information about imported graphic
        /// </summary>		
        public enum SourceTypes { none, DXF, SVG, HPGL, CSV, Drill, Gerber, Text, Barcode };
        public enum GroupOptions { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByTile = 4, ByType = 5};
        public enum SortOptions { none = 0, ByProperty = 1, ByToolNr = 2, ByCodeSize = 3, ByGraphicDimension = 4 };
		public enum CreationOptions { none = 0, AddPause = 1};
        public class GraphicInformation
        {
            public string Title;
            public string FilePath;
            public SourceTypes SourceType;       // public enum SourceTypes  { none, DXF, SVG, HPGL, CSV };
            public GroupOptions GroupOption;     // public enum GroupOptions { none=0, ByColor= 1, ByWidth=2, ByLayer=3, ByTile=4};
            public SortOptions SortOption;       // public enum SortOptions  { none=0, ByToolNr=1, ByCodeSize=2, ByGraphicDimension=3};
			public bool GroupEnable;
			public bool FigureEnable;
			public double PenWidthMin = 0;
			public double PenWidthMax = 0;
            public double DotZMin = 0;
            public double DotZMax = 0;

            public bool ConvertArcToLine;
            public bool OptionZFromWidth;
            public bool OptionDotFromCircle;		// will be processed in GCodeFromSVG 702
            public bool OptionZFromRadius;			// will select GCodeDotOnlyWithZ or GCodeDotOnly
            public bool OptionRepeatCode;
			public bool OptionSortCode;
			public bool OptionOffsetCode;
            public bool OptionHatchFill;
            public bool OptionClipCode;
			public bool OptionNodesOnly;
			public bool OptionTangentialAxis;
			public bool OptionDragTool;
			public bool OptionExtendPath;
			public bool OptionFeedFromToolTable;

            public GraphicInformation()
            {
                Title = "";
                FilePath = "";
                SourceType = SourceTypes.none;		// from where comes the data?
                GroupOption = (GroupOptions)Properties.Settings.Default.importGroupItem;	//GroupOption.ByColor;
                SortOption = (SortOptions)Properties.Settings.Default.importGroupSort;	//SortOption.ByToolNr;
				GroupEnable = Properties.Settings.Default.importGroupObjects;
				FigureEnable = true;
				PenWidthMin = 999999;
				PenWidthMax = 0;
                DotZMin = 999999;
                DotZMax = 0;

                OptionZFromWidth = Properties.Settings.Default.importDepthFromWidth;
                OptionDotFromCircle = Properties.Settings.Default.importSVGCircleToDot;
                OptionZFromRadius = Properties.Settings.Default.importSVGCircleToDotZ;
                OptionRepeatCode = Properties.Settings.Default.importRepeatEnable;
				OptionSortCode = Properties.Settings.Default.importGraphicSortDistance;
				OptionOffsetCode = Properties.Settings.Default.importGraphicOffsetOrigin;
                OptionHatchFill = Properties.Settings.Default.importGraphicHatchFillEnable;
                OptionClipCode = Properties.Settings.Default.importGraphicClipEnable;
				OptionNodesOnly = Properties.Settings.Default.importSVGNodesOnly;
				OptionTangentialAxis = Properties.Settings.Default.importGCTangentialEnable;
				OptionDragTool = Properties.Settings.Default.importGCDragKnifeEnable;
				OptionExtendPath = Properties.Settings.Default.importGraphicExtendPathEnable;
				OptionFeedFromToolTable = Properties.Settings.Default.importGCToolTableUse;

                ConvertArcToLine = Properties.Settings.Default.importGCNoArcs || OptionClipCode || OptionDragTool || OptionHatchFill;
            }
            public void ResetOptions(bool enableFigures = true)
			{	FigureEnable = enableFigures;
				OptionZFromWidth = false;
				OptionDotFromCircle = false;
                OptionZFromRadius = false;
                OptionRepeatCode = false;
				OptionSortCode = false;
				OptionOffsetCode = false;
                OptionHatchFill = false;
                OptionClipCode = false;
				OptionNodesOnly = false;	
				OptionTangentialAxis = false;
				OptionDragTool = false;
				OptionExtendPath = false;
			}
            public void SetGroup(GroupOptions group, SortOptions sort)
            {   GroupEnable = true;
                GroupOption = group;
                SortOption = sort;
            }
			
			public void SetPenWidth(string width)
			{	double nr = 0;
				if (double.TryParse(width, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out nr))
				{	PenWidthMin = Math.Min(PenWidthMin, nr);
					PenWidthMax = Math.Max(PenWidthMax, nr);
				}
			}
            public void SetDotZ(double z)
            {
                    DotZMin = Math.Min(DotZMin, z);
                    DotZMax = Math.Max(DotZMax, z);
            }

            public string ListOptions()
            {
                string importOptions = "";
                if (ConvertArcToLine) importOptions += "<Arc to Line> ";
                if (OptionZFromWidth) importOptions += "<Depth from width> ";
                if (OptionDotFromCircle) importOptions += "<Dot from circle> ";
                if (OptionZFromRadius) importOptions += "<Dot depth from circle radius> ";
                if (OptionOffsetCode) importOptions += "<Remove offset> ";
                if (OptionSortCode) importOptions += "<Sort objects> ";
                if (OptionHatchFill) importOptions += "<Hatch fill> ";
                if (OptionRepeatCode && !Properties.Settings.Default.importRepeatComplete) importOptions += "<Repeat paths> ";
                if (OptionRepeatCode && Properties.Settings.Default.importRepeatComplete) importOptions += "<Repeat code> ";
                if (OptionClipCode) importOptions += "<Clipping> ";
                if (GroupEnable) importOptions += "<Grouping> ";
                if (Properties.Settings.Default.importLineDashPattern) importOptions += "<DashPattern> ";
                if (OptionNodesOnly) importOptions += "<Nodes only> ";

                if (OptionTangentialAxis) importOptions += "<Tangential axis> ";
                if (OptionDragTool) importOptions += "<Drag knife> ";
                if (OptionExtendPath) importOptions += "<Extend path> ";
                if (OptionFeedFromToolTable) importOptions += "<Tool parameters from tool table> ";

                return importOptions;
            }
        }

        /// <summary>
        /// Collect 'PathObject' to build a group
        /// </summary>		
        public class GroupObject
        {
            public string key = "";     // value of collected penColor, penWidth, Layer or TileNr
            public int toolNr = 0;      // corresponding tool nr
            public string toolName = "";
            public double pathLength = 0;
            public double pathArea = 0;
            public string groupRelatedGCode = "";
            public Dimensions pathDimension;
            public List<PathObject> groupPath;

            public GroupObject()
            {
                pathDimension = new Dimensions();
                groupPath = new List<PathObject>();
                groupRelatedGCode = "";
            }
            public GroupObject(string tmpKey, int tnr, string tname, PathObject pathObject)
            {
                key = tmpKey;
                toolNr = tnr;
                toolName = tname;
                groupRelatedGCode = "";
                pathDimension = new Dimensions();
                pathDimension.addDimensionXY(pathObject.Dimension);

                groupPath = new List<PathObject>();
                groupPath.Add(pathObject);
                AddInfo(pathObject);
            }

            public void AddInfo(PathObject pathObject)
            {   //groupPath.Add(pathObject);
                pathLength += pathObject.PathLength;
                pathDimension.addDimensionXY(pathObject.Dimension);
                pathArea = pathDimension.getArea();
            }
        }

        /// <summary>
        /// Path information for figure and group blocks
        /// </summary>		
        public class PathInformation
        {
            public int id;                  //
            public List<string> groupAttributes;    // to use with 	public enum GroupOption { none=0, ByColor= 1, ByWidth=2, ByLayer=3, ByTile=4};
            public int penColorId;          //
            public string pathId;           // 
            public string pathGeometry;     // figure information
//            public string pathComment;      // figure information

            public PathInformation Copy()
            {   PathInformation n = new PathInformation();
                n.CopyData(this);
                return n;
            }
            public PathInformation()
            {
                id = penColorId = 0; // toolNr = codeSize = codeArea = 0;
                pathGeometry = pathId = "";// pathComment = "";
                groupAttributes = new List<string>(new string[] { "", "", "", "", "", "" });        // to use with 		public enum GroupOption { none=0, ByColor= 1, ByWidth=2, ByLayer=3, ByTile=4};
            }
            public void CopyData(PathInformation tmp)
            {
                id = tmp.id; //groupId = tmp.groupId;
                penColorId = tmp.penColorId; pathId = tmp.pathId;
                pathGeometry = tmp.pathGeometry; //pathComment = tmp.pathComment;
//                groupAttributes = tmp.groupAttributes.ToList();
				for (int i=0; i < groupAttributes.Count; i++)
					groupAttributes[i] = tmp.groupAttributes[i];
            }
			public bool IsSameAs(PathInformation tmp)
			{
                if (id != tmp.id)	return false;
				if (penColorId != tmp.penColorId)	return false;
				if (pathId != tmp.pathId)	return false;
				if (pathGeometry != tmp.pathGeometry)	return false;
//				if (pathComment != tmp.pathComment)	return false;
				if (groupAttributes.SequenceEqual(tmp.groupAttributes)) return true;
				return false;
			}
            public bool SetGroupAttribute(int index, string txt)
            {
                if (index < groupAttributes.Count)
                {
                    groupAttributes[index] = txt;
                    return true;
                }
                return false;
            }
			
			public string List()
			{	string attr = string.Format("Attr[0]:'{0}', AttrColor:'{1}', AttrWidth:'{2}', AttrLayer:'{3}', AttrTile:'{4}', AttrType:'{5}'", this.groupAttributes[0], this.groupAttributes[1], this.groupAttributes[2], this.groupAttributes[3], this.groupAttributes[4], this.groupAttributes[5]);
				return string.Format("Id:{0}, pathId:{1}, penColorId:{2}, PathGeo:{3}, {4}",this.id, this.pathId, this.penColorId, this.pathGeometry, attr);
			}
        };

        /// <summary>
        /// Collect graphic-objects to build path
        /// </summary>		
        public abstract class PathObject
        {
            protected Point start;
            protected Point end;
            protected double startAngle;
            protected double distance;				// needed to sort paths by distance to a given start-point
            protected double pathLength;			// length of the path
            protected PathInformation info;
            protected Dimensions dimension;
			protected CreationOptions options;
            protected int figureId;

            public PathObject Copy()
            {
                if (this is ItemDot)
                {
                    ItemDot n = new ItemDot(this.Start.X, this.Start.Y);
                    n.info = this.info.Copy();
                    n.pathLength = this.pathLength;
                    n.distance = this.distance;
                    n.start = this.start;
                    n.startAngle = this.startAngle;
                    n.end = this.end;
                    return n;
                }
                else
                {
                    ItemPath n = new ItemPath();
                    n.info = this.info.Copy();
					
					if (((ItemPath)this).dashArray.Length > 0)
                    {   n.dashArray = new double[((ItemPath)this).dashArray.Length];
                        ((ItemPath)this).dashArray.CopyTo(n.dashArray, 0);
                    }

                    foreach (GCodeMotion motion in ((ItemPath)this).path)
                    { 	if (motion is GCodeLine)
						{	n.AddMotion(new GCodeLine((GCodeLine)motion)); }
						else if (motion is GCodeArc)
						{	n.AddMotion(new GCodeArc((GCodeArc)motion)); }					
					}
                    n.pathLength = this.pathLength;
                    n.distance = this.distance;
                    n.start = this.start;
                    n.startAngle = this.startAngle;
                    n.end = this.end;
                    n.dimension = new Dimensions(this.dimension);
                    return n;
                }
            }
            public PathObject()
            {   info = new PathInformation();
                start = new Point();
                end = new Point();
                distance = startAngle = pathLength = 0;
                dimension = new Dimensions();
				options = CreationOptions.none;
                figureId = -1;
            }
            public Point Start
            {   get { return start; }
                set { start = value; }
            }
            public Point End
            {   get { return end; }
                set { end = value; }
            }
            public double StartAngle
            {   get { return startAngle; }
                set { startAngle = value; }
            }
            public double Distance
            {   get { return distance; }
                set { distance = value; }
            }
            public double PathLength
            {   get { return pathLength; }
                set { pathLength = value; }
            }
            public PathInformation Info
            {   get { return info; }
                set { info = value; }
            }
            public Dimensions Dimension
            {   get { return dimension; }
                set { dimension = value; }
            }
            public CreationOptions Options  // e.g. pause before path
            {   get { return options; }
                set { options = value; }
            }
            public int FigureId             // Figure Id from graphic2Gcode
            {   get { return figureId; }
                set { figureId = value; }
            }
        }

        /// <summary>
        /// Collect single Dot to build path
        /// </summary>		
        public class ItemDot : PathObject
        {
            private double optionalZ = 0;
            private bool useZ = false;
            public bool UseZ
            {   get { return useZ; }
                set { useZ = value; }
            }
            public double Z
            {   get { return optionalZ; }
                set { optionalZ = value; }
            }
            public ItemDot(double x, double y)
            {   start = end = new Point(x, y); useZ = false;
                dimension.setDimensionXY(x, y);
            }
            public ItemDot(double x, double y, double z)
            {   start = end = new Point(x, y);
                optionalZ = z; useZ = true;
                dimension.setDimensionXY(x, y);
            }
        }

        /// <summary>
        /// Collect graphic-objects to build path
        /// </summary>		
        public class ItemPath : PathObject
        {
            public bool isReversed = false;
            public bool isClosed = false;
            public double[] dashArray = new double[0];
            public List<GCodeMotion> path;

            public bool Reversed
            {   get { return isReversed; }
                set { isReversed = value; }
            }
				
            public ItemPath()
            {   isReversed = false;
                distance = 0;
                pathLength = 0;
                path = new List<GCodeMotion>();
                dimension = new Dimensions();
            }
            public ItemPath(Point tmp)
            {   isReversed = false;
                distance = 0;
                pathLength = 0;
                path = new List<GCodeMotion>();
                dimension = new Dimensions();
                GCodeMotion motion = new GCodeLine(tmp);
                dimension.setDimensionXY(tmp.X, tmp.Y);
                path.Add(motion);
                start = end = tmp;
            }
            public void AddMotion(GCodeMotion old)
            {   if (old is GCodeLine)
                    Add(old.MoveTo, old.Depth, old.Angle);               
                else
                    AddArc(((GCodeArc)old).MoveTo, ((GCodeArc)old).CenterIJ, ((GCodeArc)old).IsCW, 0, ((GCodeArc)old).AngleStart, ((GCodeArc)old).Angle);
            }

            public void Add(Point tmp, double z, double ang)
            {   GCodeMotion motion = new GCodeLine(tmp, z, ang);
                dimension.setDimensionXY(tmp.X, tmp.Y);
                path.Add(motion);
                pathLength += PointDistance(end, tmp);    // distance from last to current point
                end = tmp;
                if (start == end)
                    isClosed = true;
            }

            public void AddArc(GCodeArc arc, double z, double angStart, double angEnd)
            { 	AddArc(arc.MoveTo, arc.CenterIJ, arc.IsCW, z, angStart, angEnd); }
            public void AddArc(Point tmp, Point centerIJ, bool isCW, double z, double angStart, double angEnd)
            {
                GCodeMotion motion;
                motion = new GCodeArc(tmp, centerIJ, isCW, z, angStart, angEnd);
                dimension.setDimensionArc(new xyPoint(end), new xyPoint(tmp), centerIJ.X, centerIJ.Y, isCW);
                path.Add(motion);

                ArcProperties arcMove;
                Point p1 = Round(end);
                Point p2 = Round(tmp);
                arcMove = gcodeMath.getArcMoveProperties(p1, p2, centerIJ, isCW);
                pathLength += Math.Abs(arcMove.radius * arcMove.angleDiff);           // distance from last to current point
                end = tmp;
            }

            public void AddArc(Point tmp, Point centerIJ, double z, bool isCW, bool convertToLine)
            {
                GCodeMotion motion;
                if (!convertToLine)
                {
                    motion = new GCodeArc(tmp, centerIJ, isCW, z);
                    dimension.setDimensionArc(new xyPoint(end), new xyPoint(tmp), centerIJ.X, centerIJ.Y, isCW);
                    path.Add(motion);

                    ArcProperties arcMove;
                    Point p1 = Round(end);
                    Point p2 = Round(tmp);
                    arcMove = gcodeMath.getArcMoveProperties(p1, p2, centerIJ, isCW);
                    pathLength += Math.Abs(arcMove.radius * arcMove.angleDiff);           // distance from last to current point
                    end = tmp;
                }
                else
                {
                    ArcProperties arcMove;
                    Point p1 = Round(end);
                    Point p2 = Round(tmp);
                    double x = 0, y = 0;
                    arcMove = gcodeMath.getArcMoveProperties(p1, p2, centerIJ, isCW);
                    double stepwidth = (double)Properties.Settings.Default.importGCSegment;

                    if (stepwidth > arcMove.radius / 2)
                    { stepwidth = arcMove.radius / 5; }
                    double step = Math.Asin(stepwidth / arcMove.radius);     // in RAD
                                                                             //                    double step = Math.Asin((double)Properties.Settings.Default.importGCSegment / arcMove.radius);     // in RAD
                    if (step > Math.Abs(arcMove.angleDiff))
                        step = Math.Abs(arcMove.angleDiff / 2);

                    if (arcMove.angleDiff > 0)   //(da > 0)                                             // if delta >0 go counter clock wise
                    {
                        for (double angle = (arcMove.angleStart + step); angle < (arcMove.angleStart + arcMove.angleDiff); angle += step)
                        {
                            x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                            y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                            motion = new GCodeLine(new Point(x, y), z);
                            dimension.setDimensionXY(x, y);
                            path.Add(motion);
                            pathLength += PointDistance(end, tmp);    // distance from last to current point
                            end = tmp;
                        }
                    }
                    else                                                       // else go clock wise
                    {
                        for (double angle = (arcMove.angleStart - step); angle > (arcMove.angleStart + arcMove.angleDiff); angle -= step)
                        {
                            x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                            y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                            motion = new GCodeLine(new Point(x, y), z);
                            dimension.setDimensionXY(x, y);
                            path.Add(motion);
                            pathLength += PointDistance(end, tmp);    // distance from last to current point
                            end = tmp;
                        }
                    }
                    motion = new GCodeLine(new Point(tmp.X, tmp.Y), z);
                    dimension.setDimensionXY(tmp.X, tmp.Y);
                    path.Add(motion);
                    pathLength += PointDistance(end, tmp);    // distance from last to current point
                    end = tmp;
                }
                if (start == end)
                    isClosed = true;
            }
            private static double PointDistance(Point a, Point b)
            {   double dx = a.X - b.X;
                double dy = a.Y - b.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            private static Point Round(Point tmp, int decimals = 4)
            { return new Point(Math.Round(tmp.X, decimals), Math.Round(tmp.Y, decimals)); }
        }

        // https://www.zeiner.at/informatik/oop/skripten/Kap7-KlassenObjekte2.pdf
        /// <summary>
        /// Collect graphic-object data
        /// </summary>		
        public abstract class GCodeMotion
        {   protected Point moveTo;     // coordinate to move
            protected double depth;     // individual z value
            protected double angle;     // angle of movement lastPoint to "moveTo"
            public Point MoveTo
            {   get { return moveTo; }
                set { moveTo = value; }
            }
            public double Depth
            {   get { return depth; }
                set { depth = value; }
            }
            public double Angle
            {   get { return angle; }
                set { angle = value; }
            }
            public GCodeMotion()
            { 	moveTo = new Point(); }

            public GCodeMotion(GCodeMotion old)
            { 	moveTo = new Point(old.moveTo.X, old.moveTo.Y);
                depth = old.depth;
                angle = old.angle;
			}
        }

        /// <summary>
        /// Collect Line-specific data
        /// </summary>		
        public class GCodeLine : GCodeMotion
        {   public GCodeLine(Point tmp, double z=0)
            { MoveTo = tmp; depth = z;}
            public GCodeLine(Point tmp, double z, double ang)
            { MoveTo = tmp; depth = z; angle = ang;}
			
			public GCodeLine(GCodeLine old)		// Copy
			{	moveTo = new Point(old.moveTo.X, old.moveTo.Y); 
				depth = old.depth;
				angle = old.angle;
			}
        }

        /// <summary>
        /// Collect Arc-specific data
        /// </summary>		
        public class GCodeArc : GCodeMotion
        {   private Point center;
            private double angleStart;  // angle of start position of arc
            private bool iscw;          // direction clock-wise
            public Point CenterIJ
            {   get { return center; }
                set { center = value; }
            }
            public double AngleStart
            {   get { return angleStart; }
                set { angleStart = value; }
            }
            public bool IsCW
            {   get { return iscw; }
                set { iscw = value; }
            }
            public GCodeArc(Point tmp, Point centIJ, bool isCW, double z)
            { MoveTo = tmp; CenterIJ = centIJ; iscw = isCW; depth = z;}
            public GCodeArc(Point tmp, Point centIJ, bool isCW, double z, double angStart, double angEnd)
            { MoveTo = tmp; CenterIJ = centIJ; iscw = isCW; depth = z; angleStart = angStart; angle = angEnd; }

			public GCodeArc(GCodeArc old)		// Copy
			{	moveTo = new Point(old.moveTo.X, old.moveTo.Y); 
				center = new Point(old.center.X, old.center.Y); 
				depth = old.depth;
				angle = old.angle;
				angleStart = old.angleStart;
				iscw = old.iscw;
			}
        }
    }
}
