/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2021-07-30 check ApplyHatchFillSVG not in constructor (it's only needed for SourceType.SVG)
 * 2021-09-02 add Offset to TileObject
 * 2021-09-21 add new GroupOption 'Label'
 * 2022-04-23 add OptionSpecialWireBender
 * 2022-11-03 add OptionDashPattern to control ConvertArcToLine
 * 2023-05-31 add OptionSFromWidth
 * 2023-08-16 l:388 f:IsSameAs pull request Speed up merge and sort #348
 * 2023-08-31 l:686 f:AddArc limit stepwidth - issue #353
 * 2024-02-04 l:720 f:AddArc add noise to line
 * 2024-03-02 l:157 f:GraphicInformationClass seperate processing of OptionDashPattern
 * 2026-03-02 add import options
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

//#pragma warning disable CA1305

namespace GrblPlotter
{
    public class MyUserState
    {
        public int Value { get; set; }
        public string Content { get; set; }
    }
    public static class ImportParameter
    {//ImportParameter.AvoidArcCommand
        public static int BezierControlPointsCnt { get; set; }  // Properties.Settings.Default.importBezierLineSegmentsCnt;
        public static bool AvoidArcCommand { get; set; }        // Properties.Settings.Default.importGCNoArcs;
        public static double ArcCircumfenceStep { get; set; }   // Properties.Settings.Default.importGCSegment;
        public static bool RemoveShortMovesEnable { get; set; } // Properties.Settings.Default.importRemoveShortMovesEnable;
        public static double RemoveShortMovesLimit { get; set; }// Properties.Settings.Default.importRemoveShortMovesLimit;
        public static double AssumeAsEqualDistance { get; set; }// Properties.Settings.Default.importAssumeAsEqualDistance;

        public static void Init()
        {
            BezierControlPointsCnt = (int)Properties.Settings.Default.importBezierLineSegmentsCnt;
            AvoidArcCommand = Properties.Settings.Default.importGCNoArcs;
            ArcCircumfenceStep = (double)Properties.Settings.Default.importGCSegment;
            RemoveShortMovesEnable = Properties.Settings.Default.importRemoveShortMovesEnable;
            RemoveShortMovesLimit = (double)Properties.Settings.Default.importRemoveShortMovesLimit;
            AssumeAsEqualDistance = (double)Properties.Settings.Default.importAssumeAsEqualDistance;
        }
    }
    public static class SortParameter
    {//ImportParameter.AvoidArcCommand

        public static bool ByDistance { get; set; } //Properties.Settings.Default.importGraphicSortDistance;
        public static bool ByDistanceRotate { get; set; } //Properties.Settings.Default.importGraphicSortDistanceAllowRotate;
        public static bool ByDistanceLargestLast { get; set; } //Properties.Settings.Default.importGraphicLargestLast;
        public static bool ByDimension { get; set; } //Properties.Settings.Default.importGraphicSortDimension;
        public static void Init()
        {
            ByDistance = Properties.Settings.Default.importGraphicSortDistance;
            ByDistanceRotate = Properties.Settings.Default.importGraphicSortDistanceAllowRotate;
            ByDistanceLargestLast = Properties.Settings.Default.importGraphicLargestLast;
            ByDimension = Properties.Settings.Default.importGraphicSortDimension;
        }
    }
    public static partial class Graphic
    {
        /// <summary>
        /// General information about imported graphic
        /// </summary>		
        public enum SourceType { none, DXF, SVG, HPGL, CSV, Drill, Gerber, Text, Barcode, Wire, PDNJson, Ink, Image };
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

            //Properties.Settings.Default.multipleLoadLimitNo
            //Properties.Settings.Default.multipleLoadByX
            //Properties.Settings.Default.importGraphicFilterEnable

            /* new */
            public bool ImportOutputRelative { get; set; }  // Properties.Settings.Default.importGCRelative
            public bool ImportRemoveShortMoves { get; set; }// Properties.Settings.Default.importRemoveShortMovesEnable
            public double ImportRemoveShortMovesLimit { get; set; }              //	public double ImportRemoveShortMovesLimit { get; set; }// Properties.Settings.Default.importRemoveShortMovesLimit
            public bool ImportSVGAddOnEnable { get; set; }  // Properties.Settings.Default.importSVGAddOnEnable
                                                            //	public int importSVGAddOnPosition { get; set; }	// Properties.Settings.Default.importSVGAddOnPosition
                                                            //	public double importSVGAddOnScale { get; set; }	// Properties.Settings.Default.importSVGAddOnScale
            public bool ImportDxfConsiderZ { get; set; }    // ImportDxfConsiderZ  ImportDxfConsiderZ
            public bool ImportOptionAddSimpleFrame { get; set; }// Properties.Settings.Default.importGraphicAddFrameEnable

            public bool OptionCodeOffset { get; set; }		// General options, Graphics import general
            public bool OptionCodeOffsetLargestLast { get; set; }		// Properties.Settings.Default.importGraphicOffsetLargestLast
            public bool OptionCodeOffsetLargestRemove { get; set; }     // Properties.Settings.Default.importGraphicOffsetLargestRemove
            public bool OptionCodeSortDistance { get; set; }
            public int OptionCodeSortDistanceStartIndex { get; set; }//            int sort = Properties.Settings.Default.importGraphicSortDistanceStart;

            public bool OptionCodeSortDistanceNewStartOnClosedPath { get; set; }    // Properties.Settings.Default.importGraphicSortDistanceAllowRotate;
            public bool OptionCodeSortDistanceLargestLast { get; set; }	//Properties.Settings.Default.importGraphicLargestLast
            public bool OptionCodeSortDimension { get; set; }


            //Properties.Settings.Default.importGraphicOffsetLargestLast

            // Properties.Settings.Default.importGraphicMultiplyGraphicsEnable

            public bool ImportFilterPathsEnable { get; set; }// Properties.Settings.Default.importGraphicFilterEnable

            public bool OptionRepeatCode { get; set; }
            public bool OptionRepeatCodeZEnable { get; set; }
            public double OptionRepeatCodeZValue { get; set; }
            // Properties.Settings.Default.importRepeatCnt
            public bool OptionRepeatCodeComplete { get; set; } // Properties.Settings.Default.importRepeatComplete
            public bool OptionMultiplyGraphicsEnable { get; set; } //Properties.Settings.Default.importGraphicMultiplyGraphicsEnable

            public bool OptionSpecialDevelopment { get; set; }	// Special conversion
            public bool OptionSpecialWireBender { get; set; }	// Special conversion
            public bool OptionSpecialConvertToPolar { get; set; }   // Special conversion

            //	OptionClipCodeEnable
            public bool OptionClipCode { get; set; } // Properties.Settings.Default.importGraphicClipEnable
            public bool OptionClipCodeClip { get; set; } // Properties.Settings.Default.importGraphicClip

            /* /new */
            public bool ReProcess { get; set; }
            public bool PauseBeforePath { get; set; }
            public double PenWidthMin { get; set; }
            public double PenWidthMax { get; set; }
            public double DotZMin { get; set; }
            public double DotZMax { get; set; }

            public bool ApplyHatchFillSVG { get; set; }        // Format related SVG
            /* umbenennen */
            public bool ConvertArcToLine { get; set; }

            public bool OptionNodesOnly { get; set; }		// General options, Path interpretation
            public bool OptionDashPattern { get; set; }     // generate dashed lines
            public bool OptionZFromWidth { get; set; }
            public bool OptionSFromWidth { get; set; }
            public bool OptionDotFromCircle { get; set; }	// will be processed in GCodeFromSVG 702
            public bool OptionZFromRadius { get; set; }		// will select GCodeDotOnlyWithZ or GCodeDotOnly

            public bool OptionRampOnPenDown { get; set; }   // Path add ons

            public bool OptionDragTool { get; set; }		// Path modifications
            public bool OptionTangentialAxis { get; set; }
            public bool OptionHatchFill { get; set; }
            public bool OptionNoise { get; set; }
            public bool OptionExtendPath { get; set; }


            public bool GroupEnable { get; set; }			// Grouping and toolProp
            public bool FigureEnable { get; set; }


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
                ImportDxfConsiderZ = false;

                ImportParameter.Init();

                OptionSpecialDevelopment = Properties.Settings.Default.importGraphicDevelopmentEnable;
                OptionSpecialWireBender = Properties.Settings.Default.importGraphicWireBenderEnable;
                OptionSpecialConvertToPolar = Properties.Settings.Default.importGCConvertToPolar;

                if (OptionSpecialWireBender || OptionSpecialDevelopment || OptionSpecialConvertToPolar)
                { ResetOptions(true); }
                else
                {
                    ImportOutputRelative = Properties.Settings.Default.importGCRelative;
                    ImportRemoveShortMoves = ImportParameter.RemoveShortMovesEnable;
                    ImportRemoveShortMovesLimit = (double)Properties.Settings.Default.importRemoveShortMovesLimit;
                    ImportSVGAddOnEnable = Properties.Settings.Default.importSVGAddOnEnable;
                    ImportOptionAddSimpleFrame = Properties.Settings.Default.importGraphicAddFrameEnable;

                    ApplyHatchFillSVG = Properties.Settings.Default.importSVGApplyFill;
                    OptionDashPattern = Properties.Settings.Default.importLineDashPattern;
                    OptionZFromWidth = Properties.Settings.Default.importDepthFromWidth;
                    OptionSFromWidth = Properties.Settings.Default.importPWMFromWidth;
                    OptionDotFromCircle = Properties.Settings.Default.importSVGCircleToDot;
                    OptionZFromRadius = Properties.Settings.Default.importSVGCircleToDotZ || Properties.Settings.Default.importSVGCircleToDotS;
                    OptionRepeatCode = Properties.Settings.Default.importRepeatEnable;
                    OptionRepeatCodeZEnable = false;
                    OptionRepeatCodeZValue = 0;

                    OptionRepeatCodeComplete = Properties.Settings.Default.importRepeatComplete;
                    OptionMultiplyGraphicsEnable = Properties.Settings.Default.importGraphicMultiplyGraphicsEnable;
                    ImportFilterPathsEnable = Properties.Settings.Default.importGraphicFilterEnable;
                    OptionHatchFill = Properties.Settings.Default.importGraphicHatchFillEnable;
                    OptionNoise = Properties.Settings.Default.importGraphicNoiseEnable;
                    OptionClipCode = Properties.Settings.Default.importGraphicClipEnable;
                    OptionClipCodeClip = Properties.Settings.Default.importGraphicClip;
                    OptionNodesOnly = Properties.Settings.Default.importSVGNodesOnly;
                    OptionTangentialAxis = Properties.Settings.Default.importGCTangentialEnable;
                    OptionDragTool = Properties.Settings.Default.importGCDragKnifeEnable;
                    OptionExtendPath = Properties.Settings.Default.importGraphicExtendPathEnable;
                    OptionRampOnPenDown = Properties.Settings.Default.importGraphicLeadInEnable;
                }
                PauseBeforePath = Properties.Settings.Default.importPauseElement;
                OptionCodeOffset = Properties.Settings.Default.importGraphicOffsetOrigin;
                OptionCodeOffsetLargestLast = Properties.Settings.Default.importGraphicOffsetLargestLast;
                OptionCodeOffsetLargestRemove = Properties.Settings.Default.importGraphicOffsetLargestRemove;
                OptionCodeSortDistance = Properties.Settings.Default.importGraphicSortDistance;
                OptionCodeSortDistanceStartIndex = Properties.Settings.Default.importGraphicSortDistanceStart;
                OptionCodeSortDistanceNewStartOnClosedPath = Properties.Settings.Default.importGraphicSortDistanceAllowRotate;
                OptionCodeSortDistanceLargestLast = Properties.Settings.Default.importGraphicLargestLast;
                OptionCodeSortDimension = Properties.Settings.Default.importGraphicSortDimension;

                ConvertArcToLine = ImportParameter.AvoidArcCommand || OptionClipCode || OptionDragTool || OptionHatchFill || OptionNoise;// only for SVG: || ApplyHatchFillSVG;
                ConvertArcToLine = ConvertArcToLine || OptionSpecialWireBender || OptionSpecialDevelopment || OptionRampOnPenDown || OptionSpecialConvertToPolar;// || OptionDashPattern;
            }
            public void ResetOptions(bool enableFigures)
            {
                /* path transformations */
                //    OptionSpecialWireBender = false;        // Properties.Settings.Default.importGraphicWireBenderEnable
                //    OptionSpecialDevelopment = false;       // Properties.Settings.Default.importGraphicDevelopmentEnable
                //    OptionSpecialConvertToPolar = false;    // Properties.Settings.Default.importGCConvertToPolar

                /* path modifications */
                ImportFilterPathsEnable = false;// Properties.Settings.Default.importGraphicFilterEnable
                OptionClipCode = false; // Properties.Settings.Default.importGraphicClipEnable
                OptionClipCodeClip = false; // Properties.Settings.Default.importGraphicClip

                OptionNodesOnly = false;
                OptionZFromWidth = false;
                OptionSFromWidth = false;
                OptionDotFromCircle = false;
                OptionZFromRadius = false;

                /* path add ons */
                ImportSVGAddOnEnable = false;   // Properties.Settings.Default.importSVGAddOnEnable
                ImportOptionAddSimpleFrame = false;// Properties.Settings.Default.importGraphicAddFrameEnable
                ImportDxfConsiderZ = false;
                OptionDashPattern = false;
                ApplyHatchFillSVG = false;
                OptionHatchFill = false;
                OptionNoise = false;
                OptionTangentialAxis = false;
                OptionDragTool = false;
                OptionExtendPath = false;
                OptionRampOnPenDown = false;

                /* Output modification */
                OptionCodeSortDistance = false;
                OptionCodeSortDimension = false;
                ImportOutputRelative = false;   // Properties.Settings.Default.importGCRelative
                ImportRemoveShortMoves = false;// Properties.Settings.Default.importRemoveShortMovesEnable
                OptionCodeOffset = false;       // General options, Graphics import general
                OptionCodeOffsetLargestLast = false;
                OptionCodeOffsetLargestRemove = false;      // General options, Graphics import general

                OptionRepeatCode = false;
                OptionRepeatCodeComplete = false;
                OptionMultiplyGraphicsEnable = false;

                FigureEnable = enableFigures;
            }
            public void SetGroup(GroupOption group, SortOption sort)
            {
                GroupEnable = true;
                GroupOption = group;
                SortOption = sort;
            }

            public void SetPenWidth(string width)
            {
                //if (double.TryParse(width, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double nr))
                if (double.TryParse(width, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double nr))
                {
                    if (!Double.IsNaN(nr))
                    {
                        PenWidthMin = Math.Min(PenWidthMin, nr);
                        PenWidthMax = Math.Max(PenWidthMax, nr);
                    }
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
                if (OptionSpecialConvertToPolar) importOptions += "<POLAR> ";
                if (Properties.Settings.Default.importGraphicFilterEnable) importOptions += "<FILTER> ";
                if (ImportDxfConsiderZ) importOptions += "<DXF Z> ";
                if (OptionSpecialDevelopment || OptionSpecialWireBender) importOptions += "<Special conversion!> ";
                if (ConvertArcToLine) importOptions += "<Arc to Line> ";
                if (OptionZFromWidth) importOptions += "<Depth from width> ";
                if (OptionSFromWidth) importOptions += "<S from width> ";
                if (OptionDotFromCircle) importOptions += "<Dot from circle> ";
                if (OptionZFromRadius) importOptions += "<Dot depth from circle radius> ";
                if (Properties.Settings.Default.importGraphicAddFrameEnable) importOptions += "<Add frame> ";
                if (OptionCodeOffset) importOptions += "<Remove offset> ";
                if (OptionCodeOffset && OptionCodeOffsetLargestRemove) importOptions += "<REMOVE LARGEST> ";
                if (Properties.Settings.Default.importGraphicMultiplyGraphicsEnable) importOptions += "<Multiply> ";
                if (OptionCodeSortDistance) importOptions += "<Sort objects> ";
                if (ApplyHatchFillSVG) importOptions += "<SVG fill> ";
                if (OptionNoise) importOptions += "<Noise> ";
                if (OptionHatchFill) importOptions += "<Hatch fill> ";
                if (OptionRepeatCode && !OptionRepeatCodeComplete) importOptions += "<Repeat paths> ";
                if (OptionRepeatCode && OptionRepeatCodeComplete) importOptions += "<Repeat code> ";
                if (OptionClipCode) importOptions += "<Clipping> ";
                if (GroupEnable) importOptions += "<Grouping> ";
                if (Properties.Settings.Default.importLineDashPattern) importOptions += "<DashPattern> ";
                if (OptionNodesOnly) importOptions += "<Nodes only> ";

                if (OptionTangentialAxis) importOptions += "<Tangential axis> ";
                if (OptionDragTool) importOptions += "<Drag knife> ";
                if (OptionExtendPath) importOptions += "<Extend path> ";
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

                    //if (GroupAttributes.SequenceEqual(tmp.GroupAttributes))	// 2023-08-16 pull request Speed up merge and sort #348  Improve comparing GroupAttributes
                    //{
                    //    return true;
                    for (int i = 1; i < GroupAttributes.Count; i++)    // GroupOptions { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByType = 4, ByTile = 5};
                    {
                        if (GroupAttributes[i] != tmp.GroupAttributes[i])
                            return false;
                    }
                    return true;
                    //}
                }
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
            public string GetGroupAttribute(int index)
            {
                if (index < GroupAttributes.Count)
                {
                    return GroupAttributes[index];
                }
                return "";
            }

            public string List()
            {
                string attr = string.Format("Attr[0]:'{0}', AttrColor:'{1}', AttrFill:'{6}', AttrWidth:'{2}', AttrLayer:'{3}', AttrTile:'{4}', AttrType:'{5}'", this.GroupAttributes[0], this.GroupAttributes[1], this.GroupAttributes[2], this.GroupAttributes[3], this.GroupAttributes[4], this.GroupAttributes[5], this.GroupAttributes[6]);
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
                // PathObject n = (PathObject)MemberwiseClone();
                if (this is ItemDot)
                {
                    ItemDot n = (ItemDot)MemberwiseClone();//new ItemDot(this.Start.X, this.Start.Y)
                    n._info = _info.Copy();
                    n._dimension = new Dimensions(_dimension);
                    /*   {
                           _info = this._info.Copy(),
                           _pathLength = this._pathLength,
                           _distance = this._distance,
                           _start = this._start,
                           _startAngle = this._startAngle,
                           _end = this._end
                       };*/
                    return n;
                }
                else
                {
                    ItemPath n = (ItemPath)MemberwiseClone();//new ItemDot(this.Start.X, this.Start.Y)
                    n._info = _info.Copy();
                    n._dimension = new Dimensions(_dimension);
                    if (((ItemPath)this).DashArray.Length > 0)
                    {
                        n.DashArray = new double[((ItemPath)this).DashArray.Length];
                        ((ItemPath)this).DashArray.CopyTo(n.DashArray, 0);
                    }
                    n.Path = new List<GCodeMotion>();
                    foreach (GCodeMotion motion in ((ItemPath)this).Path)
                    {
                        if (motion is GCodeLine line)
                        { n.AddMotion(new GCodeLine(line)); }
                        else if (motion is GCodeArc arc)
                        { n.AddMotion(new GCodeArc(arc)); }
                    }
                    /*   ItemPath n = new ItemPath
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
                                                    n._dimension = new Dimensions(this._dimension);*/
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
            public void AddMotion(GCodeMotion old, double offsetX, double offsetY)
            {
                if (old != null)
                {
                    if (old is GCodeLine)
                    {
                        Point newStart = new Point(old.MoveTo.X + offsetX, old.MoveTo.Y + offsetY);
                        Add(newStart, old.Depth, old.Angle);
                    }
                    else
                    {
                        Point newStart = new Point(((GCodeArc)old).MoveTo.X + offsetX, ((GCodeArc)old).MoveTo.Y + offsetY);
                        AddArc(newStart, ((GCodeArc)old).CenterIJ, ((GCodeArc)old).IsCW, old.Depth, ((GCodeArc)old).AngleStart, ((GCodeArc)old).Angle);
                    }
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

            public void AddArc(Point tmp, Point centerIJ, double dz, bool isCW, bool convertToLine, bool addNoise)
            {
                GCodeMotion motion;

                ArcProperties arcMove;
                Point p1 = Round(End);
                Point p2 = Round(tmp);
                arcMove = GcodeMath.GetArcMoveProperties(p1, p2, centerIJ, isCW);
                PathLength += Math.Abs(arcMove.radius * arcMove.angleDiff);           // distance from last to current point
                Dimension.SetDimensionArc(new XyPoint(End), new XyPoint(tmp), centerIJ.X, centerIJ.Y, isCW);

                if (!convertToLine)
                {
                    motion = new GCodeArc(tmp, centerIJ, isCW, dz);
                    Path.Add(motion);
                }
                else
                {
                    double radius;
                    double noiseAmplitude = (double)Properties.Settings.Default.importGraphicNoiseAmplitude / 2;

                    double x, y;
                    double stepwidth = ImportParameter.ArcCircumfenceStep;

                    if (ImportParameter.RemoveShortMovesEnable)       // 2023-08-31 issue #353
                    { stepwidth = Math.Max(stepwidth, ImportParameter.RemoveShortMovesLimit); }

                    if (stepwidth > arcMove.radius / 2)
                    { stepwidth = arcMove.radius / 5; }
                    double step = Math.Asin(stepwidth / arcMove.radius);     // in RAD
                    if (step > Math.Abs(arcMove.angleDiff))
                        step = Math.Abs(arcMove.angleDiff / 2);

                    if (arcMove.angleDiff > 0) // counter clock wise
                    {
                        for (double angle = (arcMove.angleStart + step); angle < (arcMove.angleStart + arcMove.angleDiff); angle += step)
                        {
                            if (addNoise)
                                radius = arcMove.radius + (Noise.CalcPixel2D(Path.Count, (int)angle, 1) * noiseAmplitude);
                            else
                                radius = arcMove.radius;
                            x = arcMove.center.X + radius * Math.Cos(angle);
                            y = arcMove.center.Y + radius * Math.Sin(angle);
                            motion = new GCodeLine(new Point(x, y), dz);
                            Path.Add(motion);
                            Dimension.SetDimensionXY(x, y);
                        }
                    }
                    else    // else go clock wise
                    {
                        for (double angle = (arcMove.angleStart - step); angle > (arcMove.angleStart + arcMove.angleDiff); angle -= step)
                        {
                            if (addNoise)
                                radius = arcMove.radius + (Noise.CalcPixel2D(Path.Count, (int)angle, 1) * noiseAmplitude);
                            else
                                radius = arcMove.radius;
                            x = arcMove.center.X + radius * Math.Cos(angle);
                            y = arcMove.center.Y + radius * Math.Sin(angle);
                            motion = new GCodeLine(new Point(x, y), dz);
                            Path.Add(motion);
                            Dimension.SetDimensionXY(x, y);
                        }
                    }
                    motion = new GCodeLine(new Point(tmp.X, tmp.Y), dz);
                    Path.Add(motion);
                }
                End = tmp;
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
