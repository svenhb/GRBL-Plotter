/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2022 Sven Hasemann contact: svenhb@web.de

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
 * 2020-12-03 add notification event args
 * 2021-01-22 ListOptions() add frame and multiply
 * 2021-07-02 code clean up / code quality
 * 2021-07-30 check ApplyHatchFill not in constructor (it's only needed for SourceType.SVG)
 * 2021-09-02 add Offset to TileObject
 * 2021-09-21 add new GroupOption 'Label'
 * 2022-04-23 add OptionSpecialWireBend
 * 2022-11-03 add OptionDashPattern to control ConvertArcToLine
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

//#pragma warning disable CA1305

namespace GrblPlotter
{
    public class MyUserState
    {
        public int Value { get; set; }
        public string Content { get; set; }
    }

    public static partial class Graphic
    {
        /// <summary>
        /// General information about imported graphic
        /// </summary>		
        public enum SourceType { none, DXF, SVG, HPGL, CSV, Drill, Gerber, Text, Barcode };
        public enum GroupOption { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByType = 4, ByTile = 5, ByFill = 6, Label = 7 };
        public enum SortOption { none = 0, ByProperty = 1, ByToolNr = 2, ByCodeSize = 3, ByGraphicDimension = 4 };
        public enum CreationOption { none = 0, AddPause = 1, AddPauseBeforePath = 2 };
        internal class GraphicInformationClass
        {
            public string Title { get; set; }
            public string FilePath { get; set; }
            public SourceType SourceType { get; set; }       // public enum SourceTypes  { none, DXF, SVG, HPGL, CSV };
            public GroupOption GroupOption { get; set; }     // public enum GroupOptions { none=0, ByColor= 1, ByWidth=2, ByLayer=3, ByTile=4};
            public SortOption SortOption { get; set; }       // public enum SortOptions  { none=0, ByToolNr=1, ByCodeSize=2, ByGraphicDimension=3};

            public bool ReProcess { get; set; }
            public bool PauseBeforePath { get; set; }
            public double PenWidthMin { get; set; }
            public double PenWidthMax { get; set; }
            public double DotZMin { get; set; }
            public double DotZMax { get; set; }

            public bool DxfImportZ { get; set; }

            public bool ApplyHatchFill { get; set; }		// Format related SVG

            public bool OptionOffsetCode { get; set; }		// General options, Graphics import general
            public bool OptionSortCode { get; set; }
            public bool ConvertArcToLine { get; set; }

            public bool OptionNodesOnly { get; set; }		// General options, Path interpretation
            public bool OptionDashPattern { get; set; }     // generate dashed lines
            public bool OptionZFromWidth { get; set; }
            public bool OptionDotFromCircle { get; set; }	// will be processed in GCodeFromSVG 702
            public bool OptionZFromRadius { get; set; }		// will select GCodeDotOnlyWithZ or GCodeDotOnly
            public bool OptionRepeatCode { get; set; }

            public bool OptionRampOnPenDown { get; set; }   // Path add ons

            public bool OptionDragTool { get; set; }		// Path modifications
            public bool OptionTangentialAxis { get; set; }
            public bool OptionHatchFill { get; set; }
            public bool OptionExtendPath { get; set; }

            public bool OptionClipCode { get; set; }		// Clipping

            public bool GroupEnable { get; set; }			// Grouping and tools
            public bool FigureEnable { get; set; }
            public bool OptionFeedFromToolTable { get; set; }

            public bool OptionSpecialDevelop { get; set; }	// Special conversion
            public bool OptionSpecialWireBend { get; set; }	// Special conversion

            public GraphicInformationClass()
            {
                Title = "";
                FilePath = "";
                SourceType = SourceType.none;		// from where comes the data?
                GroupOption = (GroupOption)Properties.Settings.Default.importGroupItem;	//GroupOption.ByColor;
                SortOption = (SortOption)Properties.Settings.Default.importGroupSort;   //SortOption.ByToolNr;
                GroupEnable = Properties.Settings.Default.importGroupObjects;
                FigureEnable = true;
                ReProcess = false;
                PenWidthMin = 999999;
                PenWidthMax = 0;
                DotZMin = 999999;
                DotZMax = 0;
                DxfImportZ = false;

                OptionSpecialDevelop = Properties.Settings.Default.importGraphicDevelopmentEnable;
                OptionSpecialWireBend = Properties.Settings.Default.importGraphicWireBenderEnable;

                if (OptionSpecialWireBend || OptionSpecialDevelop)
                { ResetOptions(true); }
                else
                {
                    ApplyHatchFill = Properties.Settings.Default.importSVGApplyFill;
                    OptionDashPattern = Properties.Settings.Default.importLineDashPattern;
                    OptionZFromWidth = Properties.Settings.Default.importDepthFromWidth;
                    OptionDotFromCircle = Properties.Settings.Default.importSVGCircleToDot;
                    OptionZFromRadius = Properties.Settings.Default.importSVGCircleToDotZ;
                    OptionRepeatCode = Properties.Settings.Default.importRepeatEnable;
                    OptionHatchFill = Properties.Settings.Default.importGraphicHatchFillEnable;
                    OptionClipCode = Properties.Settings.Default.importGraphicClipEnable;
                    OptionNodesOnly = Properties.Settings.Default.importSVGNodesOnly;
                    OptionTangentialAxis = Properties.Settings.Default.importGCTangentialEnable;
                    OptionDragTool = Properties.Settings.Default.importGCDragKnifeEnable;
                    OptionExtendPath = Properties.Settings.Default.importGraphicExtendPathEnable;
                    OptionRampOnPenDown = Properties.Settings.Default.importGraphicLeadInEnable;
                }
                PauseBeforePath = Properties.Settings.Default.importPauseElement;
                OptionOffsetCode = Properties.Settings.Default.importGraphicOffsetOrigin;
                OptionSortCode = Properties.Settings.Default.importGraphicSortDistance;
                OptionFeedFromToolTable = Properties.Settings.Default.importGCToolTableUse;

                ConvertArcToLine = Properties.Settings.Default.importGCNoArcs || OptionClipCode || OptionDragTool || OptionHatchFill;// only for SVG: || ApplyHatchFill;
                ConvertArcToLine = ConvertArcToLine || OptionSpecialWireBend || OptionSpecialDevelop || OptionRampOnPenDown || OptionDashPattern;
            }
            public void ResetOptions(bool enableFigures)
            {
                DxfImportZ = false;
                FigureEnable = enableFigures;
                OptionDashPattern = false;
                OptionZFromWidth = false;
                OptionDotFromCircle = false;
                OptionZFromRadius = false;
                OptionRepeatCode = false;
                OptionSortCode = false;
                OptionOffsetCode = false;
                ApplyHatchFill = false;
                OptionHatchFill = false;
                OptionClipCode = false;
                OptionNodesOnly = false;
                OptionTangentialAxis = false;
                OptionDragTool = false;
                OptionExtendPath = false;
                OptionRampOnPenDown = false;
            }
            public void SetGroup(GroupOption group, SortOption sort)
            {
                GroupEnable = true;
                GroupOption = group;
                SortOption = sort;
            }

            public void SetPenWidth(string width)
            {
                if (double.TryParse(width, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double nr))
                {
                    PenWidthMin = Math.Min(PenWidthMin, nr);
                    PenWidthMax = Math.Max(PenWidthMax, nr);
                }
            }
            public void SetDotZ(double dz)
            {
                DotZMin = Math.Min(DotZMin, dz);
                DotZMax = Math.Max(DotZMax, dz);
            }

            public string ListOptions()
            {
                string importOptions = "";
                if (DxfImportZ) importOptions += "<DXF Z> ";
                if (OptionSpecialDevelop || OptionSpecialWireBend) importOptions += "<Special conversion!> ";
                if (ConvertArcToLine) importOptions += "<Arc to Line> ";
                if (OptionZFromWidth) importOptions += "<Depth from width> ";
                if (OptionDotFromCircle) importOptions += "<Dot from circle> ";
                if (OptionZFromRadius) importOptions += "<Dot depth from circle radius> ";
                if (Properties.Settings.Default.importGraphicAddFrameEnable) importOptions += "<Add frame> ";
                if (OptionOffsetCode) importOptions += "<Remove offset> ";
                if (Properties.Settings.Default.importGraphicMultiplyGraphicsEnable) importOptions += "<Multiply> ";
                if (OptionSortCode) importOptions += "<Sort objects> ";
                if (ApplyHatchFill) importOptions += "<SVG fill> ";
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
                if (OptionRampOnPenDown) importOptions += "<Ramp on pen-down> ";

                return importOptions;
            }
        }

        /// <summary>
        /// Collect 'GroupObject' to build a tile
        /// </summary>		
        internal class TileObject
        {
            public string Key { get; set; }     // value of collected penColor, penWidth, Layer or TileNr
            public string TileRelatedGCode { get; set; }
            public Point Offset { get; set; }
            public List<GroupObject> Tile { get; set; }      // either collect groups 
            public List<PathObject> GroupPath { get; set; }  // or paths
            protected int TileId { get; set; }		// track GCode group-id

            public TileObject()
            {
                Tile = new List<GroupObject>();
                GroupPath = new List<PathObject>();
                TileRelatedGCode = "";
                TileId = -1;
            }
            public TileObject(string tmpKey, string comand, Point offset)//, PathObject pathObject)
            {
                Key = tmpKey;
                TileRelatedGCode = comand;
                TileId = -1;
                Offset = offset;

                Tile = new List<GroupObject>();
                GroupPath = new List<PathObject>();
                //        groupPath = new List<PathObject>();
                //        groupPath.Add(pathObject);
            }

            /*  public int TileId             // Tile Id from graphic2Gcode?
              {   get { return TileId; }
                  set { TileId = value; }
              }*/
        }

        /// <summary>
        /// Collect 'PathObject' to build a group
        /// </summary>		
        internal class GroupObject
        {
            public string Key { get; set; }     // value of collected penColor, penWidth, Layer or TileNr
            public int ToolNr { get; set; }      // corresponding tool nr
            public string ToolName { get; set; }
            public double PathLength { get; set; }
            public double PathArea { get; set; }
            public string GroupRelatedGCode { get; set; }
            public Dimensions PathDimension { get; set; }
            public List<PathObject> GroupPath { get; set; }
            protected int GroupIdObject { get; set; }		// track GCode group-id

            public GroupObject()
            {
                PathDimension = new Dimensions();
                GroupPath = new List<PathObject>();
                GroupRelatedGCode = "";
                GroupIdObject = -1;
            }
            public GroupObject(string tmpKey, int tnr, string tname, PathObject pathObject)
            {
                Key = tmpKey;
                ToolNr = tnr;
                ToolName = tname;
                GroupRelatedGCode = "";
                PathDimension = new Dimensions();
                if (pathObject != null)
                    PathDimension.AddDimensionXY(pathObject.Dimension);
                GroupIdObject = -1;

                GroupPath = new List<PathObject>
                {
                    pathObject
                };
                AddInfo(pathObject);
            }
            public GroupObject(GroupObject tmp)
            {
                if (tmp != null)
                {
                    Key = tmp.Key;
                    ToolNr = tmp.ToolNr;
                    ToolName = tmp.ToolName;
                    GroupRelatedGCode = tmp.GroupRelatedGCode;
                    PathDimension = new Dimensions();
                    PathDimension.AddDimensionXY(tmp.PathDimension);
                    GroupIdObject = tmp.GroupIdObject;

                    GroupPath = new List<PathObject>();
                    foreach (PathObject tmpPath in tmp.GroupPath)
                    { GroupPath.Add(tmpPath); }
                }
            }

            public void AddInfo(PathObject pathObject)
            {   //groupPath.Add(pathObject);
                if (pathObject != null)
                {
                    PathLength += pathObject.PathLength;
                    PathDimension.AddDimensionXY(pathObject.Dimension);
                    PathArea = PathDimension.GetArea();
                }
            }

            public int GroupId             // Group Id from graphic2Gcode
            {
                get { return GroupIdObject; }
                set { GroupIdObject = value; }
            }
        }

        /// <summary>
        /// Path information for figure and group blocks
        /// </summary>		
        internal class PathInformation
        {
            public int Id { get; set; }                 //
            public List<string> GroupAttributes { get; set; }    // to use with 	public enum GroupOption { none=0, ByColor= 1, ByWidth=2, ByLayer=3, ByTile=4};
            public int PenColorId { get; set; }        	//
            public string PathId { get; set; }          // 
            public string PathGeometry { get; set; }   	// figure information
            public int AuxInfo { get; set; }        	//

            public PathInformation Copy()
            {
                PathInformation n = new PathInformation();
                n.CopyData(this);
                return n;
            }
            public PathInformation()
            {
                Id = PenColorId = AuxInfo = 0; // toolNr = codeSize = codeArea = 0;
                PathGeometry = PathId = "";// pathComment = "";
                GroupAttributes = new List<string>(new string[] { "", "", "", "", "", "", "", "" });        // to use with 		public enum GroupOption { none=0, ByColor= 1, ByWidth=2, ByLayer=3, ByTile=4};
            }
            public void CopyData(PathInformation tmp)
            {
                if (tmp != null)
                {
                    Id = tmp.Id; //groupId = tmp.groupId;
                    PenColorId = tmp.PenColorId; PathId = tmp.PathId; AuxInfo = tmp.AuxInfo;
                    PathGeometry = tmp.PathGeometry; //pathComment = tmp.pathComment;
                                                     //                groupAttributes = tmp.groupAttributes.ToList();
                    for (int i = 0; i < GroupAttributes.Count; i++)
                        GroupAttributes[i] = tmp.GroupAttributes[i];
                }
            }
            public bool IsSameAs(PathInformation tmp)
            {
                if (tmp != null)
                {
                //    Logger.Trace("IsSameAs Id:{0}/{1}  ColId:{2}/{3}  PathId:{4}/{5}  Aux:{6}/{7}  Geo:{8}/{9}", Id, tmp.Id, PenColorId, tmp.PenColorId, PathId, tmp.PathId, AuxInfo, tmp.AuxInfo, PathGeometry, tmp.PathGeometry);
                    if (Id != tmp.Id) return false;
                    if (PenColorId != tmp.PenColorId) return false;
                    if (PathId != tmp.PathId) return false;
                    if (AuxInfo != tmp.AuxInfo) return false;
                    if (PathGeometry != tmp.PathGeometry) return false;
                    //				if (pathComment != tmp.pathComment)	return false;
                    if (GroupAttributes.SequenceEqual(tmp.GroupAttributes))
                    {
                    //    Logger.Trace("IsSameAs true"); 
                        return true;
                    }
                }
             //   Logger.Trace("IsSameAs false");
                return false;
            }
            public bool SetGroupAttribute(int index, string txt)
            {
                if (index < GroupAttributes.Count)
                {
                    GroupAttributes[index] = txt;
                    return true;
                }
                return false;
            }

            public string List()
            {
                string attr = string.Format("Attr[0]:'{0}', AttrColor:'{1}', AttrWidth:'{2}', AttrLayer:'{3}', AttrTile:'{4}', AttrType:'{5}'", this.GroupAttributes[0], this.GroupAttributes[1], this.GroupAttributes[2], this.GroupAttributes[3], this.GroupAttributes[4], this.GroupAttributes[5]);
                return string.Format("Id:{0}, pathId:{1}, penColorId:{2}, PathGeo:{3}, AuxInfo:{4}   {5}", this.Id, this.PathId, this.PenColorId, this.PathGeometry, this.AuxInfo, attr);
            }
        };

        /// <summary>
        /// Collect graphic-objects to build path
        /// </summary>		
        internal abstract class PathObject
        {
            protected Point _start;
            protected Point _end;
            protected double _startAngle;
            protected double _distance;				// needed to sort paths by distance to a given start-point
            protected double _pathLength;			// length of the path
            protected int _tmpIndex;
            protected PathInformation _info;
            protected Dimensions _dimension;
            protected CreationOption _options;
            protected int _figureId;					// track GCode figure-id

            public PathObject Copy()
            {
                if (this is ItemDot)
                {
                    ItemDot n = new ItemDot(this.Start.X, this.Start.Y)
                    {
                        _info = this._info.Copy(),
                        _pathLength = this._pathLength,
                        _distance = this._distance,
                        _start = this._start,
                        _startAngle = this._startAngle,
                        _end = this._end
                    };
                    return n;
                }
                else
                {
                    ItemPath n = new ItemPath
                    {
                        _info = this._info.Copy()
                    };

                    if (((ItemPath)this).DashArray.Length > 0)
                    {
                        n.DashArray = new double[((ItemPath)this).DashArray.Length];
                        ((ItemPath)this).DashArray.CopyTo(n.DashArray, 0);
                    }

                    foreach (GCodeMotion motion in ((ItemPath)this).Path)
                    {
                        if (motion is GCodeLine line)
                        { n.AddMotion(new GCodeLine(line)); }
                        else if (motion is GCodeArc arc)
                        { n.AddMotion(new GCodeArc(arc)); }
                    }
                    n._pathLength = this._pathLength;
                    n._distance = this._distance;
                    n._start = this._start;
                    n._startAngle = this._startAngle;
                    n._end = this._end;
                    n._dimension = new Dimensions(this._dimension);
                    return n;
                }
            }
            public PathObject()
            {
                _info = new PathInformation();
                _start = new Point();
                _end = new Point();
                _distance = _startAngle = _pathLength = 0;
                _dimension = new Dimensions();
                _options = CreationOption.none;
                _figureId = -1;
            }
            public Point Start
            {
                get { return _start; }
                set { _start = value; }
            }
            public Point End
            {
                get { return _end; }
                set { _end = value; }
            }
            public double StartAngle
            {
                get { return _startAngle; }
                set { _startAngle = value; }
            }
            public double Distance
            {
                get { return _distance; }
                set { _distance = value; }
            }
            public double PathLength
            {
                get { return _pathLength; }
                set { _pathLength = value; }
            }
            public int TmpIndex
            {
                get { return _tmpIndex; }
                set { _tmpIndex = value; }
            }
            public PathInformation Info
            {
                get { return _info; }
                set { _info = value; }
            }
            public Dimensions Dimension
            {
                get { return _dimension; }
                set { _dimension = value; }
            }
            public CreationOption Options  // e.g. pause before path
            {
                get { return _options; }
                set { _options = value; }
            }
            public int FigureId             // Figure Id from graphic2Gcode
            {
                get { return _figureId; }
                set { _figureId = value; }
            }
        }



        /// <summary>
        /// Collect single Dot to build path
        /// </summary>		
        internal class ItemDot : PathObject
        {
            private double optionalZ = 0;
            private bool useZ = false;
            public bool UseZ
            {
                get { return useZ; }
                set { useZ = value; }
            }
            public double OptZ
            {
                get { return optionalZ; }
                set { optionalZ = value; }
            }
            public ItemDot(double dz, double dy)
            {
                Start = End = new Point(dz, dy); useZ = false;
                Dimension.SetDimensionXY(dz, dy);
            }
            public ItemDot(double dx, double dy, double dz)
            {
                Start = End = new Point(dx, dy);
                optionalZ = dz; useZ = true;
                Dimension.SetDimensionXY(dx, dy);
            }
        }

        /// <summary>
        /// Collect graphic-objects to build path
        /// </summary>		
        internal class ItemPath : PathObject
        {
            public bool IsReversed { get; set; }
            public bool IsClosed { get; set; }
            public double[] DashArray = new double[0];
            public List<GCodeMotion> Path;

            public bool Reversed
            {
                get { return IsReversed; }
                set { IsReversed = value; }
            }

            public ItemPath()
            {
                IsReversed = false;
                Distance = 0;
                PathLength = 0;
                Path = new List<GCodeMotion>();
                Dimension = new Dimensions();
            }
            public ItemPath(Point tmp)
            {
                IsReversed = false;
                Distance = 0;
                PathLength = 0;
                Path = new List<GCodeMotion>();
                Dimension = new Dimensions();
                GCodeMotion motion = new GCodeLine(tmp);
                Dimension.SetDimensionXY(tmp.X, tmp.Y);
                Path.Add(motion);
                Start = End = tmp;
            }
            public ItemPath(Point tmp, double dz)
            {
                IsReversed = false;
                Distance = 0;
                PathLength = 0;
                Path = new List<GCodeMotion>();
                Dimension = new Dimensions();
                GCodeMotion motion = new GCodeLine(tmp, dz);
                Dimension.SetDimensionXY(tmp.X, tmp.Y);
                Path.Add(motion);
                Start = End = tmp;
            }
            public void AddMotion(GCodeMotion old)
            {
                if (old != null)
                {
                    if (old is GCodeLine)
                        Add(old.MoveTo, old.Depth, old.Angle);
                    else
                        AddArc(((GCodeArc)old).MoveTo, ((GCodeArc)old).CenterIJ, ((GCodeArc)old).IsCW, 0, ((GCodeArc)old).AngleStart, ((GCodeArc)old).Angle);
                }
            }

            public void Add(Point tmp, double dz, double ang)
            {
                GCodeMotion motion = new GCodeLine(tmp, dz, ang);
                Dimension.SetDimensionXY(tmp.X, tmp.Y);
                Path.Add(motion);
                PathLength += PointDistance(End, tmp);    // distance from last to current point
                End = tmp;
                if (Start == End)
                    IsClosed = true;
            }

            public void AddArc(GCodeArc arc, double dz, double angStart, double angEnd)
            { if (arc != null) AddArc(arc.MoveTo, arc.CenterIJ, arc.IsCW, dz, angStart, angEnd); }
            public void AddArc(Point tmp, Point centerIJ, bool isCW, double dz, double angStart, double angEnd)
            {
                GCodeMotion motion;
                motion = new GCodeArc(tmp, centerIJ, isCW, dz, angStart, angEnd);
                Dimension.SetDimensionArc(new XyPoint(End), new XyPoint(tmp), centerIJ.X, centerIJ.Y, isCW);
                Path.Add(motion);

                ArcProperties arcMove;
                Point p1 = Round(End);
                Point p2 = Round(tmp);
                arcMove = GcodeMath.GetArcMoveProperties(p1, p2, centerIJ, isCW);
                PathLength += Math.Abs(arcMove.radius * arcMove.angleDiff);           // distance from last to current point
                End = tmp;
            }

            public void AddArc(Point tmp, Point centerIJ, double dz, bool isCW, bool convertToLine)
            {
                GCodeMotion motion;
                if (!convertToLine)
                {
                    motion = new GCodeArc(tmp, centerIJ, isCW, dz);
                    Dimension.SetDimensionArc(new XyPoint(End), new XyPoint(tmp), centerIJ.X, centerIJ.Y, isCW);
                    Path.Add(motion);

                    ArcProperties arcMove;
                    Point p1 = Round(End);
                    Point p2 = Round(tmp);
                    arcMove = GcodeMath.GetArcMoveProperties(p1, p2, centerIJ, isCW);
                    PathLength += Math.Abs(arcMove.radius * arcMove.angleDiff);           // distance from last to current point
                    End = tmp;
                }
                else
                {
                    ArcProperties arcMove;
                    Point p1 = Round(End);
                    Point p2 = Round(tmp);
                    double x, y;
                    arcMove = GcodeMath.GetArcMoveProperties(p1, p2, centerIJ, isCW);
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
                            motion = new GCodeLine(new Point(x, y), dz);
                            Dimension.SetDimensionXY(x, y);
                            Path.Add(motion);
                            PathLength += PointDistance(End, tmp);    // distance from last to current point
                            End = tmp;
                        }
                    }
                    else                                                       // else go clock wise
                    {
                        for (double angle = (arcMove.angleStart - step); angle > (arcMove.angleStart + arcMove.angleDiff); angle -= step)
                        {
                            x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                            y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                            motion = new GCodeLine(new Point(x, y), dz);
                            Dimension.SetDimensionXY(x, y);
                            Path.Add(motion);
                            PathLength += PointDistance(End, tmp);    // distance from last to current point
                            End = tmp;
                        }
                    }
                    motion = new GCodeLine(new Point(tmp.X, tmp.Y), dz);
                    Dimension.SetDimensionXY(tmp.X, tmp.Y);
                    Path.Add(motion);
                    PathLength += PointDistance(End, tmp);    // distance from last to current point
                    End = tmp;
                }
                if (Start == End)
                    IsClosed = true;
            }
            private static double PointDistance(Point a, Point b)
            {
                double dx = a.X - b.X;
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
        internal abstract class GCodeMotion
        {
            public Point MoveTo { get; set; }     // coordinate to move
            public double Depth { get; set; }     // individual z value
            public double Angle { get; set; }     // angle of movement lastPoint to "moveTo"
            /*    public Point MoveTo
                {   get { return MoveTo; }
                    set { MoveTo = value; }
                }*/
            /*    public double Depth
                {   get { return Depth; }
                    set { Depth = value; }
                }*/
            /*   public double Angle
               {   get { return Angle; }
                   set { Angle = value; }
               }*/
            protected GCodeMotion()
            { MoveTo = new Point(); }

            protected GCodeMotion(GCodeMotion old)
            {
                if (old != null)
                {
                    MoveTo = new Point(old.MoveTo.X, old.MoveTo.Y);
                    Depth = old.Depth;
                    Angle = old.Angle;
                }
            }
        }

        /// <summary>
        /// Collect Line-specific data
        /// </summary>		
        internal class GCodeLine : GCodeMotion
        {
            public GCodeLine(Point tmp)
            { MoveTo = tmp; Depth = 0; }
            public GCodeLine(Point tmp, double dz)
            { MoveTo = tmp; Depth = dz; }
            public GCodeLine(Point tmp, double dz, double ang)
            { MoveTo = tmp; Depth = dz; Angle = ang; }

            public GCodeLine(GCodeLine old)     // Copy
            {
                if (old != null)
                {
                    MoveTo = new Point(old.MoveTo.X, old.MoveTo.Y);
                    Depth = old.Depth;
                    Angle = old.Angle;
                }
            }
        }

        /// <summary>
        /// Collect Arc-specific data
        /// </summary>		
        internal class GCodeArc : GCodeMotion
        {
            private Point center;
            private double angleStart;  // angle of start position of arc
            private bool iscw;          // direction clock-wise
            public Point CenterIJ
            {
                get { return center; }
                set { center = value; }
            }
            public double AngleStart
            {
                get { return angleStart; }
                set { angleStart = value; }
            }
            public bool IsCW
            {
                get { return iscw; }
                set { iscw = value; }
            }
            public GCodeArc(Point tmp, Point centIJ, bool isCW, double dz)
            { MoveTo = tmp; CenterIJ = centIJ; iscw = isCW; Depth = dz; }
            public GCodeArc(Point tmp, Point centIJ, bool isCW, double dz, double angStart, double angEnd)
            { MoveTo = tmp; CenterIJ = centIJ; iscw = isCW; Depth = dz; angleStart = angStart; Angle = angEnd; }

            public GCodeArc(GCodeArc old)       // Copy
            {
                if (old != null)
                {
                    MoveTo = new Point(old.MoveTo.X, old.MoveTo.Y);
                    center = new Point(old.center.X, old.center.Y);
                    Depth = old.Depth;
                    Angle = old.Angle;
                    angleStart = old.angleStart;
                    iscw = old.iscw;
                }
            }
        }
    }
}
