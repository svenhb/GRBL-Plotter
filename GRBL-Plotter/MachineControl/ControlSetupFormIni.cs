/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2025 Sven Hasemann contact: svenhb@web.de

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
 * 2024-02-12 new file
 * 2025-03-14 update / renaming sections and settings
*/

using static System.Collections.Specialized.BitVector32;

namespace GrblPlotter
{
    public partial class IniFile
    {
        internal static string sectionSetupImportParameter = "Graphic Import";
        internal static string[,] keyValueSetupImportParameter = {
            {"Graph Units mm",      		"importUnitmm",					"True"},
            {"Bezier Segment count",		"importBezierLineSegmentsCnt", 	""},
            {"Replace arc by lines",		"importGCNoArcs", 				"False"},
            {"Arc circumfence step",		"importGCSegment", 				""},
            {"Remove short moves enable", 	"importRemoveShortMovesEnable", 	"True"},
            {"Remove short moves limit",  	"importRemoveShortMovesLimit", 		""},
            {"Distance assumed as equal",   "importAssumeAsEqualDistance", 		""},
            {"Objects offset origin enable","importGraphicOffsetOrigin", 		"True"},
            {"Objects offset origin X",     "importGraphicOffsetOriginX", 		""},
            {"Objects offset origin Y",     "importGraphicOffsetOriginY", 		""},
            {"Objects offset large last",   "importGraphicOffsetLargestLast", 	"True"},
            {"Objects offset large remove", "importGraphicOffsetLargestRemove", "False"},
            {"Objects sort distance enable","importGraphicSortDistance", 		"True"},
			{"Objects sort start",    		"importGraphicSortDistanceStart", 	""},
            {"Objects sort rotate",         "importGraphicSortDistanceAllowRotate", ""},
            {"Objects sort large last",     "importGraphicLargestLast", 		""},
            {"Objects sort dimension enable","importGraphicSortDimension", 		""},

            {"Gcode line numbers",      "ctrlLineNumbers", 		"False"},
            {"Gcode end char enable",   "ctrlLineEndEnable", 	"False"},
			{"Gcode end char",      	"ctrlLineEndText", 		""},
            {"Gcode fold",              "importCodeFold", 		"False"},
            {"Gcode add comments",      "importSVGAddComments", "False"}
        };

        /*************************************************************************************/
        internal static string sectionSetupSvgDxfCsv = "Graphic Format";
        internal static string[,] keyValueSetupSvgDxfCsv = {
            {"SVG resize enable",   "importSVGRezise", 		"False"},
            {"SVG resize units",    "importSVGMaxSize", 	""},
            {"SVG addon enable",    "importSVGAddOnEnable", "False"},
            {"SVG addon scale",     "importSVGAddOnScale", 	""},
            {"SVG addon position",  "importSVGAddOnPosition", ""},
            {"SVG addon file",      "importSVGAddOnFile", 	""},
            {"SVG skip hidden",     "importSVGDontPlot", 	"False"},
            {"SVG path new figure", "importSVGPathNewFigure", "False"},
            {"SVG apply fill",      "importSVGApplyFill", 	"False"},
            {"SVG apply metadata",  "importSVGMetaData", 	"False"},

            {"DXF use color index",         "importDXFToolIndex", 	"False"},
            {"DXF handle white as black",   "importDXFSwitchWhite", "True"},
            {"DXF skip hidden",             "importDXFDontPlot", 	"False"},
            {"DXF import Z",                "importDXFUseZ", 		"False"},

            {"CSV automatic",   "importCSVAutomatic", 	"True"},
            {"CSV start line",  "importCSVStartLine", 	""},
            {"CSV delimiter",   "importCSVDelimeter", 	""},
            {"CSV X column",    "importCSVColumnX", 	""},
            {"CSV X scale",     "importCSVScaleX", 		""},
            {"CSV Y column",    "importCSVColumnY", 	""},
            {"CSV Y scale",     "importCSVScaleY", 		""},
            {"CSV Z enable",    "importCSVProzessZ", 	""},
            {"CSV Z column",    "importCSVColumnZ", 	""},
            {"CSV Z scale",     "importCSVScaleZ", 		""},
            {"CSV connect",     "importCSVProzessAsLine", ""},
			
			{"Image format png",	"importVectorizeTypePng", ""},
			{"Image format gif",	"importVectorizeTypeGif", ""},
			{"Image format jpg",	"importVectorizeTypeJpg", ""},
			{"Image format bmp",	"importVectorizeTypeBmp", ""},
			{"Image format clipboard","importVectorizeFromClipboard", ""},

			{"Image PDNJason only visible layers",	"importPDNLayerVisible", ""},
			{"Image transparency detection",		"importVectorizeDetectTransparency", ""},
			{"Image detection threshold",			"importVectorizeThreshold", ""},

			{"Image DPI from image ",	"importVectorizeDpiFromImage", 	""},
			{"Image DPI set",			"importPDNDpi", 				""},
			{"Image DPI width enable",	"importVectorizeSetWidthOfImage", ""},
			{"Image DPI width",			"importPDNWidth", 				""},
			
			{"Image potrace turdsize",			"importVectorizePoTraceTurdsize", ""},
			{"Image potrace alphamax",			"importVectorizePoTraceAlphamax", ""},
			{"Image potrace opttolerance",		"importVectorizePoTraceOpttolerance", ""},
			{"Image potrace curveoptimizing",	"importVectorizePoTraceCurveoptimizing", ""},
						
			{"Image mytrace invert",	"importVectorizeInvertResult", ""},
			{"Image mytrace optimize 1","importVectorizeOptimize1", ""},
			{"Image mytrace optimize 2","importVectorizeOptimize2", ""},
			{"Image mytrace optimize 3","importVectorizeOptimize3", ""},
			{"Image mytrace optimize 4","importVectorizeOptimize4", ""},
			{"Image mytrace smooth",	"importVectorizeSmoothCycles", ""}
        };

        /*************************************************************************************/
        internal static string sectionSetupGcodeGeneration = "GCode generation";
        internal static string[,] keyValueSetupGcodeGeneration = {
            {"Dec Places",      "importGCDecPlaces",""},
            {"Header Code",     "importGCHeader", 	""},
            {"Footer Code",     "importGCFooter", 	""},
        //    {"Tool Change Code","importGCToolChangeCode" },   // not used any where

            {"Tool table enable",        "importGCToolTableUse","False"},
            {"Tool table default enable","importGCToolDefNrUse",""},
            {"Tool table default number","importGCToolDefNr", 	""},

            {"XY Feedrate",         "importGCXYFeed", 		""},
            {"XY Feedrate from TT", "importGCTTXYFeed", 	""},

            {"Spindle Speed",           "importGCSSpeed", 	""},
            {"Spindle Speed from TT",   "importGCTTSSpeed", ""},
            {"Spindle Use Laser",       "importGCSpindleToggleLaser", ""},

            {"Spindle Direction M3","importGCSDirM3", 		""},
            {"Spindle Delay",       "importGCSpindleDelay", ""},

            {"Add Tool Cmd",    "importGCTool", 		"False"},
            {"Add Tool M0",     "importGCToolM0", 		""},
            {"Add Comments",    "importGCAddComments", 	"False"},
/******/
            {"Z enable",        "importGCZEnable", 	"False"},
            {"Z Values from TT","importGCTTZAxis", 	""},
            {"Z Feedrate",      "importGCZFeed", 	""},
            {"Z Up Pos",        "importGCZUp", 		""},
            {"Z Down Pos",      "importGCZDown", 	""},
            {"Z Inc enable",        "importGCZIncEnable", 	""},
            {"Z Increment at zero", "importGCZIncStartZero",""},
            {"Z Increment",         "importGCZIncrement", 	""},
            {"Z Increment no up",   "importGCZIncNoZUp", 	""},
            {"Z Prevent Spindle","importGCZPreventSpindle", ""},

            {"PWM enable",      "importGCPWMEnable", 	"False"},
            {"PWM Up Val",      "importGCPWMUp", 		""},
            {"PWM Up Dly",      "importGCPWMDlyUp", 	""},
            {"PWM Down Val",    "importGCPWMDown", 		""},
            {"PWM Down Dly",    "importGCPWMDlyDown", 	""},
            {"PWM Zero Val",    "importGCPWMZero", 		""},
            {"PWM P93 Val",     "importGCPWMP93", 		""},
            {"PWM P93 Dly",     "importGCPWMDlyP93", 	""},
			{"PWM P93 Text",    "importGCPWMTextP93", 	""},
            {"PWM P94 Val",     "importGCPWMP94", 		""},
            {"PWM P94 Dly",     "importGCPWMDlyP94", 	""},
			{"PWM P94 Text",    "importGCPWMTextP94", 	""},
			{"PWM Values from TT","importGCTTSSpeed", 	""},
            {"PWM Skip M30",    "importGCPWMSkipM30", 	""},

            {"Spindle Toggle",  "importGCSpindleToggle", 	"False"},

            {"Individual enable",   "importGCIndEnable", 	"False"},
            {"Individual PenUp",    "importGCIndPenUp", 	""},
            {"Individual PenDown",  "importGCIndPenDown", 	""}
        };

        /*************************************************************************************/
        internal static string sectionSetupMachineLimits = "Machine Limits";
        internal static string[,] keyValueSetupMachineLimits = {
            {"Limit show", 	"machineLimitsShow" },
            {"Limit alarm",	"machineLimitsAlarm" },
            {"Range X",     "machineLimitsRangeX" },
            {"Range Y",     "machineLimitsRangeY" },
            {"Range Z",     "machineLimitsRangeZ" },
            {"Home X",      "machineLimitsHomeX" },
            {"Home Y",      "machineLimitsHomeY" },
            {"Home Z",      "machineLimitsHomeZ" },
			
			{"RunTime Spindle", "grblRunTimeSpindle" },
			{"RunTime Flood",   "grblRunTimeFlood" },
			{"RunTime Mist",    "grblRunTimeMist" }
        };

        /*************************************************************************************/
        internal static string sectionSetupRotaryAxis = "Rotary axis";
        internal static string[,] keyValueSetupRotaryAxis = {
            {"Rotary enable", 	"rotarySubstitutionEnable",	"False"},
            {"Rotary AxisX", 	"rotarySubstitutionX", 		""},
            {"Rotary Scale", 	"rotarySubstitutionScale", 	""},
            {"Rotary Diameter","rotarySubstitutionDiameter",""},
            {"Rotary Setup enable",	"rotarySubstitutionSetupEnable",""},
            {"Rotary Setup On", 	"rotarySubstitutionSetupOn", 	""},
            {"Rotary Setup Off", 	"rotarySubstitutionSetupOff", 	""}
		};

        /*************************************************************************************/
        internal static string sectionSetupFileLoading = "File loading";
        internal static string[,] keyValueSetupFileLoading = {
            {"Insert enable", 	"fromFormInsertEnable",	"False"},
            {"Multiple always", "multipleLoadAllwaysLoad",	"False"},
            {"Multiple clear", 	"multipleLoadAllwaysClear",	"False"},
            {"Multiple gap", 	"multipleLoadGap",	""},
            {"Multiple nr X", 	"multipleLoadNoX",	""},
            {"Multiple nr Y", 	"multipleLoadNoY",	""},
            {"Multiple order X", 	"multipleLoadByX",	""},
            {"Multiple limit nr", 	"multipleLoadLimitNo",	""},
		};
        /*************************************************************************************/
        internal static string sectionSetupFlowControl = "Flow control";
        internal static string[,] keyValueSetupFlowControl = {
            {"Poll frequecy",   "grblPollIntervalIndex",	""},
            {"Poll reduce", 	"grblPollIntervalReduce",	""},
            {"Buffer automatic","grblBufferAutomatic",	    ""},
            {"Buffer size", 	"grblBufferSize",	        ""},
            {"Streaming char counting", 	"grblStreamingProtocol1",	""},
            {"Streaming disable pause mode","guiDisableProgramPause",	""},
            {"Streaming disable path update","guiBackgroundImageEnable",""},
            {"Streaming show progress",     "guiProgressShow",	""},
            {"Reset restore work coordinates","resetRestoreWorkCoordinates",""},
            {"Reset send string enable","resetSendCodeEnable",	""},
            {"Reset send string", 		"resetSendCode",		""},
            {"Pause send string enable","flowControlEnable",	""},
            {"Pause send string", 		"flowControlText",		""}
		};			
		
         /*************************************************************************************/
        internal static string sectionSetupPathInterpretation = "Path interpretation";
        internal static string[,] keyValueSetupPathInterpretation = {
            {"Process Dashed Lines",    "importLineDashPattern", 	"False"},
            {"Process Dashed Lines G0", "importLineDashPatternG0",	"False"},
            {"Process nodes only",      "importSVGNodesOnly", 		"False"},

            {"Circle to dot",           "importSVGCircleToDot", 	"False"},
			{"Circle to dot script count",	"importCircleToDotScriptCount",	""},
			{"Circle to dot script",       	"importCircleToDotScript", 	""},
			
            {"Depth from width min",        "importDepthFromWidthMin", 	""},
            {"Depth from width max",        "importDepthFromWidthMax", 	""},
            {"Depth from width enable",   	"importDepthFromWidth", 	""},
            {"Depth from width ramp",       "importDepthFromWidthRamp",	""},
            {"Circle to dot with Z",        "importSVGCircleToDotZ", 	"False"},

            {"PWM from width min", 		"importImageSMin", 		""},
            {"PWM from width max", 		"importImageSMax", 		""},
            {"PWM from width enable",	"importPWMFromWidth", 	""},
            {"Circle to dot with S",	"importSVGCircleToDotS","False"}
        };

         /*************************************************************************************/
        internal static string sectionSetupPathAddon = "Path add on";
        internal static string[,] keyValueSetupPathAddon = {
            {"Repeat Code enable", 	"importRepeatEnable",		"False"},
            {"Repeat Code count", 	"importRepeatCnt",			""},
            {"Repeat Code complete", 	"importRepeatComplete",	""},
            {"Repeat Code complete all","importRepeatEnableAll",""},
            {"Multiply enable", 	"importGraphicMultiplyGraphicsEnable",	"False"},
            {"Multiply distance", 	"importGraphicMultiplyGraphicsDistance",""},
            {"Multiply number x",	"importGraphicMultiplyGraphicsDimX",	""},
            {"Multiply number y", 	"importGraphicMultiplyGraphicsDimY",	""},
			{"Add Frame enable",	"importGraphicAddFrameEnable",		"False"},
            {"Add Frame distance", 	"importGraphicAddFrameDistance",	""},
            {"Add Frame add radius","importGraphicAddFrameApplyRadius",	""},
            {"Add Frame pen color",	"importGraphicAddFramePenColor",	""},
            {"Add Frame pen width",	"importGraphicAddFramePenWidth",	""},
            {"Add Frame pen layer",	"importGraphicAddFramePenLayer",	""},

            {"Ramp enable",     "importGraphicLeadInEnable",	"False"},
            {"Ramp distance",   "importGraphicLeadInDistance",	""},

			{"Pause before Path",   "importPauseElement",	"False"},
			{"Pause before Pen",    "importPausePenDown",	"False"}
		};

         /*************************************************************************************/
        internal static string sectionSetupPathModifications = "Path modifications";
        internal static string[,] keyValueSetupPathModifications = {
            {"Drag tool enable", 	"importGCDragKnifeEnable",	"False"},
            {"Drag tool offset", 	"importGCDragKnifeLength",	""},
            {"Drag tool percent", 	"importGCDragKnifePercent",	""},
            {"Drag tool percent enable", "importGCDragKnifePercentEnable",	""},
            {"Drag tool angle", 		"importGCDragKnifeAngle",	""},

            {"Tangential axis enable", 	"importGCTangentialEnable",	"False"},
            {"Tangential axis name", 	"importGCTangentialAxis",	""},
            {"Tangential axis angle", 	"importGCTangentialAngle",	""},
            {"Tangential axis deviation", "importGCTangentialAngleDevi",	""},
            {"Tangential axis turn", 	"importGCTangentialTurn",	""},
            {"Tangential axis range", 	"importGCTangentialRange",	""},
			{"Tangential axis shortening enable", 	"importGCTangentialShorteningEnable",	""},
			{"Tangential axis shortening", 	"importGCTangentialShortening",	""},
			
            {"Hatch fill enable", 	"importGraphicHatchFillEnable",			"False"},
            {"Hatch fill cross", 	"importGraphicHatchFillCross",			""},
            {"Hatch fill distance", 	"importGraphicHatchFillDistance",	""},
            {"Hatch fill distance offset enable","importGraphicHatchFillOffsetInc",	""},
            {"Hatch fill distance offset ", "importGraphicHatchFillOffset",		""},
            {"Hatch fill angle", 			"importGraphicHatchFillAngle",		""},
            {"Hatch fill angle inc enable", "importGraphicHatchFillAngleInc",	""},
            {"Hatch fill angle inc ", 		"importGraphicHatchFillAngle2",		""},
            {"Hatch fill inset enable", 	"importGraphicHatchFillInsetEnable",""},
            {"Hatch fill inset distance", 	"importGraphicHatchFillInset",		""},
            {"Hatch fill inset enable2", 	"importGraphicHatchFillInsetEnable2",""},
            {"Hatch fill delete path", 		"importGraphicHatchFillDeletePath",	""},
            {"Hatch fill add noise",	"importGraphicHatchFillNoise",			""},
            {"Hatch fill delete path",	"importGraphicHatchFillDeletePath",		""},
			
			{"Overlap enable", 	"importGraphicExtendPathEnable","False"},
            {"Overlap distance","importGraphicExtendPathValue",	""},

			{"Noise enable", 	"importGraphicNoiseEnable",		"False"},
			{"Noise amplitude", "importGraphicNoiseAmplitude",	""},
			{"Noise distance", 	"importGraphicNoiseDensity",	""},

			{"Clipping enable", 	"importGraphicClipEnable",	"False"},
			{"Clipping width", 		"importGraphicTileX",		""},
			{"Clipping height", 	"importGraphicTileY",		""},
			{"Clipping offset x", 	"importGraphicClipOffsetX",	""},
			{"Clipping offset y", 	"importGraphicClipOffsetY",	""},
			{"Clipping not tiling",	"importGraphicClip",		""},
			{"Clipping tile offset","importGraphicClipOffsetApply",	""},
			{"Clipping tile show orig pos", "importGraphicClipShowOrigPosition",""},
			{"Clipping tile move while streaming", "importGraphicClipShowOrigPositionShiftTileProcessed",	""},
			{"Clipping tile command", 	"importGraphicClipGCode",	""},
			{"Clipping tile skip 1st", 	"importGraphicClipSkipCode",""},

			{"Grouping enable", 	"importGroupObjects",	"False"},
			{"Grouping item", 		"importGroupItem",		""},
			{"Grouping sort option","importGroupSort",		""},
			{"Grouping sort invert","importGroupSortInvert",""},

			{"Filter enable","importGraphicFilterEnable",			"False"},
			{"Filter remove","importGraphicFilterChoiceRemove",		""},
			{"Filter list remove","importGraphicFilterListRemove",	""},
			{"Filter list keep","importGraphicFilterListKeep",		""}
		};
			
         /*************************************************************************************/
        internal static string sectionSetupCodeConversion = "GCode conversion";
        internal static string[,] keyValueSetupCodeConversion = {
			{"Develop enable","importGraphicDevelopmentEnable",	"False"},
			{"Develop feed X","importGraphicDevelopmentFeedX",	""},
			{"Develop feed invert","importGraphicDevelopmentFeedInvert",	""},
			{"Develop notch length","importGraphicDevelopmentNotchWidth",	""},
			{"Develop notch distance","importGraphicDevelopmentNotchDistance",	""},
			{"Develop notch Z engrave","importGraphicDevelopmentNotchZNotch",	""},
			{"Develop notch Z cut","importGraphicDevelopmentNotchZCut",	""},
			{"Develop feed after","importGraphicDevelopmentFeedAfter",	""},

			{"Wire bender enable","importGraphicWireBenderEnable",	"False"},
			{"Wire bender radius","importGraphicWireBenderRadius",	""},
			{"Wire bender addon","importGraphicWireBenderAngleAddOn",""},
			{"Wire bender absolute","importGraphicWireBenderAngleAbsolute",	""},
			{"Wire bender code on","importGraphicWireBenderCodePegOn",	""},
			{"Wire bender code off","importGraphicWireBenderCodePegOff",""},
			{"Wire bender code cut","importGraphicWireBenderCodeCut",	""}
		};

         /*************************************************************************************/
        internal static string sectionSetupCommandExtension = "Command extension";
        internal static string[,] keyValueSetupCommandExtension = {
			{"Aux1 enable","importGCAux1Enable",	"False"},
			{"Aux1 axis","importGCAux1Axis",		""},
			{"Aux1 factor","importGCAux1Factor",	""},
			{"Aux1 absolute","importGCAux1SumUp",	""},
			{"Aux1 Z include","importGCAux1ZUse",	""},
			{"Aux1 Z factor","importGCAux1ZFactor",	""},
			{"Aux1 Z process","importGCAux1ZMode",	""},
			{"Aux2 enable","importGCAux2Enable",	"False"},
			{"Aux2 axis","importGCAux2Axis",		""},
			{"Aux2 factor","importGCAux2Factor",	""},
			{"Aux2 absolute","importGCAux2SumUp",	""},
			{"Aux2 Z include","importGCAux2ZUse",	""},
			{"Aux2 Z factor","importGCAux2ZFactor",	""},
			{"Aux2 Z process","importGCAux2ZMode",	""}
		};

         /*************************************************************************************/
        internal static string sectionSetupGCodeModification = "GCode modification";
        internal static string[,] keyValueSetupGCodeModification = {
			{"Line segmentation enable","importGCLineSegmentation",	"False"},
			{"Line segmentation length","importGCLineSegmentLength",""},
			{"Line segmentation equidistant","importGCLineSegmentEquidistant",""},
			{"Insert subroutine enable","importGCSubEnable",	"False"},
			{"Insert subroutine file","importGCSubroutine",		""},
			{"Insert subroutine at begin","importGCSubFirst",	""},
			{"Insert subroutine pen up down","importGCSubPenUpDown",""},
			{"Compress","importGCCompress",	"False"},
			{"Relative","importGCRelative",	"False"}
		};

         /*************************************************************************************/
        internal static string sectionSetupToolChange = "Tool change";
        internal static string[,] keyValueSetupToolChange = {
			{"Tool change M6 pass through","ctrlToolChangeM6PassThrough","False"},
			{"Tool enable","ctrlToolChange",			"False"},
			{"Tool remove","ctrlToolScriptPut",			""},
			{"Tool select","ctrlToolScriptSelect",		""},
			{"Tool pick up","ctrlToolScriptGet",		""},
			{"Tool probe","ctrlToolScriptProbe",		""},
			{"Tool delay","ctrlToolScriptDelay",		""},
			{"Tool empty","ctrlToolChangeEmpty",		""},
			{"Tool empty Nr","ctrlToolChangeEmptyNr",	""},
			{"Tool table offset X","toolTableOffsetX",	""},
			{"Tool table offset Y","toolTableOffsetY",	""},
			{"Tool table offset Z","toolTableOffsetZ",	""},
			{"Tool table offset A","toolTableOffsetA",	""},
			{"Tool table loaded","toolTableLastLoaded",	""}
		};
		
         /*************************************************************************************/
        internal static string sectionSetup2DView = "2D View";
        internal static string[,] keyValueSetup2DView = {
            {"Show Ruler",     	"gui2DRulerShow" },
            {"Show Dimension", 	"guiDimensionShow" },
            {"Show Information","gui2DInfoShow" },
            {"Show Background",	"guiBackgroundShow" },
            {"Show PenUp",     	"gui2DPenUpShow" },
            {"Show ToolTable", 	"gui2DToolTableShow" },
            {"Show Machine Limits",	"machineLimitsShow" },
            {"Show Machine Fix View","machineLimitsFix" },

            {"Width Ruler",    	"gui2DWidthRuler" },
            {"Width Tool",     	"gui2DWidthTool" },
            {"Width Marker",  	"gui2DWidthMarker" },
            {"Width PenUp",    	"gui2DWidthPenUp" },
            {"Width PenDown",  	"gui2DWidthPenDown" },
            {"Width Rotary",  	"gui2DWidthRotaryInfo" },
            {"Width HeightMap",	"gui2DWidthHeightMap" },
            {"Width Simulation","gui2DWidthSimulation" },

            {"PenDown Color Mode","gui2DColorPenDownModeEnable" },
            {"PenDown Width Mode","gui2DColorPenDownModeWidth" },
			{"Tool Size",		  "gui2DSizeTool" },

            {"Color Background",		"gui2DColorBackground" },
            {"Color Background Path",	"gui2DColorBackgroundPath" },
            {"Color Dimension",	"gui2DColorDimension" },
            {"Color Ruler",		"gui2DColorRuler" },
            {"Color Tool",		"gui2DColorTool" },
            {"Color Marker",	"gui2DColorMarker" },
            {"Color PenUp",		"gui2DColorPenUp" },
            {"Color PenDown",	"gui2DColorPenDown" },
            {"Color Rotary",	"gui2DColorRotaryInfo" },
            {"Color HeightMap",	"gui2DColorHeightMap" },
            {"Color Simulation","gui2DColorSimulation" }
		};

        /*************************************************************************************/
        internal static string sectionSetupButtons = "Buttons";
        internal static string[,] keyValueSetupButtons = {
            {"Button1",   "guiCustomBtn1" },
            {"Button2",   "guiCustomBtn2" },
            {"Button3",   "guiCustomBtn3" },
            {"Button4",   "guiCustomBtn4" },
            {"Button5",   "guiCustomBtn5" },
            {"Button6",   "guiCustomBtn6" },
            {"Button7",   "guiCustomBtn7" },
            {"Button8",   "guiCustomBtn8" },
            {"Button9",   "guiCustomBtn9" },
            {"Button10",   "guiCustomBtn10" },
            {"Button11",   "guiCustomBtn11" },
            {"Button12",   "guiCustomBtn12" },
            {"Button13",   "guiCustomBtn13" },
            {"Button14",   "guiCustomBtn14" },
            {"Button15",   "guiCustomBtn15" },
            {"Button16",   "guiCustomBtn16" },
            {"Button17",   "guiCustomBtn17" },
            {"Button18",   "guiCustomBtn18" },
            {"Button19",   "guiCustomBtn19" },
            {"Button20",   "guiCustomBtn20" },
            {"Button21",   "guiCustomBtn21" },
            {"Button22",   "guiCustomBtn22" },
            {"Button23",   "guiCustomBtn23" },
            {"Button24",   "guiCustomBtn24" },
            {"Button25",   "guiCustomBtn25" },
            {"Button26",   "guiCustomBtn26" },
            {"Button27",   "guiCustomBtn27" },
            {"Button28",   "guiCustomBtn28" },
            {"Button29",   "guiCustomBtn29" },
            {"Button30",   "guiCustomBtn30" },
            {"Button31",   "guiCustomBtn31" },
            {"Button32",   "guiCustomBtn32" }
		};

        /*************************************************************************************/
        internal static string sectionSetupJoystick = "Joystick";
        internal static string[,] keyValueSetupJoystick = {
            {"XY1 Step",   "guiJoystickXYStep1" },
            {"XY2 Step",   "guiJoystickXYStep2" },
            {"XY3 Step",   "guiJoystickXYStep3" },
            {"XY4 Step",   "guiJoystickXYStep4" },
            {"XY5 Step",   "guiJoystickXYStep5" },
            {"XY1 Speed",   "guiJoystickXYSpeed1" },
            {"XY2 Speed",   "guiJoystickXYSpeed2" },
            {"XY3 Speed",   "guiJoystickXYSpeed3" },
            {"XY4 Speed",   "guiJoystickXYSpeed4" },
            {"XY5 Speed",   "guiJoystickXYSpeed5" },
            {"Z1 Step",   "guiJoystickZStep1" },
            {"Z2 Step",   "guiJoystickZStep2" },
            {"Z3 Step",   "guiJoystickZStep3" },
            {"Z4 Step",   "guiJoystickZStep4" },
            {"Z5 Step",   "guiJoystickZStep5" },
            {"Z1 Speed",   "guiJoystickZSpeed1" },
            {"Z2 Speed",   "guiJoystickZSpeed2" },
            {"Z3 Speed",   "guiJoystickZSpeed3" },
            {"Z4 Speed",   "guiJoystickZSpeed4" },
            {"Z5 Speed",   "guiJoystickZSpeed5" },
            {"A1 Step",   "guiJoystickAStep1" },
            {"A2 Step",   "guiJoystickAStep2" },
            {"A3 Step",   "guiJoystickAStep3" },
            {"A4 Step",   "guiJoystickAStep4" },
            {"A5 Step",   "guiJoystickAStep5" },
            {"A1 Speed",   "guiJoystickASpeed1" },
            {"A2 Speed",   "guiJoystickASpeed2" },
            {"A3 Speed",   "guiJoystickASpeed3" },
            {"A4 Speed",   "guiJoystickASpeed4" },
            {"A5 Speed",   "guiJoystickASpeed5" }
		};
		
        /*************************************************************************************/
        internal static string sectionSetupGamePad = "GamePad";
        internal static string[,] keyValueSetupGamePad = {
            {"gamePadEnable",     "gamePadEnable" },
            {"gamePadButtons0",   "gamePadButtons0" },
            {"gamePadButtons1",   "gamePadButtons1" },
            {"gamePadButtons2",   "gamePadButtons2" },
            {"gamePadButtons3",   "gamePadButtons3" },
            {"gamePadButtons4",   "gamePadButtons4" },
            {"gamePadButtons5",   "gamePadButtons5" },
            {"gamePadButtons6",   "gamePadButtons6" },
            {"gamePadButtons7",   "gamePadButtons7" },
            {"gamePadButtons8",   "gamePadButtons8" },
            {"gamePadButtons9",   "gamePadButtons9" },
            {"gamePadButtons10",  "gamePadButtons10" },
            {"gamePadButtons11",  "gamePadButtons11" },
            {"gamePadButtons12",  "gamePadButtons12" },
            {"gamePadButtons13",  "gamePadButtons13" },
            {"gamePadButtons14",  "gamePadButtons14" },
            {"gamePadButtons15",  "gamePadButtons15" },
						
            {"gamePadPOVC00",     "gamePadPOVC00" },
            {"gamePadPOVC01",     "gamePadPOVC01" },
            {"gamePadPOVC02",     "gamePadPOVC02" },
            {"gamePadPOVC03",     "gamePadPOVC03" },
            {"gamePadPOVC04",     "gamePadPOVC04" },
            {"gamePadPOVC05",     "gamePadPOVC05" },
            {"gamePadPOVC06",     "gamePadPOVC06" },
            {"gamePadPOVC07",     "gamePadPOVC07" },
			
            {"gamePadXAxis",      "gamePadXAxis" },
            {"gamePadYAxis",      "gamePadYAxis" },
            {"gamePadZAxis",      "gamePadZAxis" },
            {"gamePadRAxis",      "gamePadRAxis" },
            {"gamePadXInvert",    "gamePadXInvert" },
            {"gamePadYInvert",    "gamePadYInvert" },
            {"gamePadZInvert",    "gamePadZInvert" },
            {"gamePadRInvert",    "gamePadRInvert" }
        };
		
        /*************************************************************************************/
        internal static string sectionConnections = "Connections";
        internal static string[,] keyValueConnections = {
            {"1st COM Port",      "serialPort1" },
            {"1st COM Baud",      "serialBaud1" },
            {"2nd COM Port",      "serialPort2" },
            {"2nd COM Baud",      "serialBaud2" },
            {"3rd COM Port",      "serialPort3" },
            {"3rd COM Baud",      "serialBaud3" },
            {"3rd COM Ready",     "serial3Ready" },
            {"3rd COM Timeout",   "serial3Timeout" },
            {"DIY COM Port",      "serialPortDIY" },
            {"DIY COM Baud",      "serialBaudDIY" }
		};	
    }
}
