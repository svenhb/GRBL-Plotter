/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
*/

namespace GrblPlotter
{
    public partial class IniFile
    {
        internal static string[,] keyValueSetupImportParameter = {
            {"Graph Units mm",      "importUnitmm" },
            {"Bezier Segment count","importBezierLineSegmentsCnt" },
            {"Replace arc by lines","importGCNoArcs" },
            {"Arc circumfence step","importGCSegment"},
            {"Remove Moves enable", "importRemoveShortMovesEnable" },
            {"Remove Moves limit",  "importRemoveShortMovesLimit"  },
            {"Distance assumed as equal",   "importAssumeAsEqualDistance"},
            {"Objects offset origin enable","importGraphicOffsetOrigin"  },
            {"Objects offset origin X",     "importGraphicOffsetOriginX" },
            {"Objects offset origin Y",     "importGraphicOffsetOriginY" },
            {"Objects sort by distance",    "importGraphicSortDistance"  },
            {"Objects sort rotate",         "importGraphicSortDistanceAllowRotate"},
            {"Objects sort large last",     "importGraphicLargestLast"   },
            {"Objects sort dimension",      "importGraphicSortDimension" },

            {"Gcode line numbers",      "ctrlLineNumbers"  },
            {"Gcode add end char",      "ctrlLineEndEnable"},
            {"Gcode fold",              "importCodeFold" },
            {"Gcode add comments",      "importSVGAddComments"}
        };
        internal static string sectionSetupImportParameter = "Graphic Import";
		
/*************************************************************************************/
        internal static string[,] keyValueSetupSvgDxfCsv = {
            {"SVG resize enable",   "importSVGRezise" },
            {"SVG resize units",    "importSVGMaxSize" },
            {"SVG addon enable",    "importSVGAddOnEnable"},
            {"SVG addon scale",     "importSVGAddOnScale" },
            {"SVG addon position",  "importSVGAddOnPosition" },
            {"SVG addon file",      "importSVGAddOnFile"  },
            {"SVG skip hidden",     "importSVGDontPlot"   },
            {"SVG apply fill",      "importSVGApplyFill"  },
            {"SVG apply metadata",  "importSVGMetaData"   },

            {"DXF use color index",         "importDXFToolIndex"   },
            {"DXF handle white as black",   "importDXFSwitchWhite" },
            {"DXF skip hidden",             "importDXFDontPlot"    },
            {"DXF import Z",                "importDXFUseZ"    },

            {"CSV automatic",   "importCSVAutomatic"  },
            {"CSV start line",  "importCSVStartLine"},
            {"CSV delimiter",   "importCSVDelimeter" },
            {"CSV X column",    "importCSVColumnX"},
            {"CSV X scale",     "importCSVScaleX"},
            {"CSV Y column",    "importCSVColumnY"},
            {"CSV Y scale",     "importCSVScaleY"},
            {"CSV Z column",    "importCSVColumnZ"},
            {"CSV Z scale",     "importCSVScaleZ"},
            {"CSV Z enable",    "importCSVProzessZ"},
            {"CSV connect",     "importCSVProzessAsLine"}
        };
        internal static string sectionSetupSvgDxfCsv = "Graphic Format";
		
/*************************************************************************************/
        internal static string[,] keyValueSetupGcodeGeneration = {
            {"Dec Places",   	"importGCDecPlaces" },
            {"Header Code",   	"importGCHeader" },
            {"Footer Code",   	"importGCFooter" },
            {"Tool Change Code","importGCToolChangeCode" },

            {"XY Feedrate",   		"importGCXYFeed" },
            {"XY Feedrate from TT",	"importGCTTXYFeed" },

            {"Spindle Speed",   		"importGCSSpeed" },
            {"Spindle Speed from TT",   "importGCTTSSpeed" },
            {"Spindle Use Laser",   	"importGCSpindleToggleLaser" },

            {"Spindle Direction M3","importGCSDirM3" },
            {"Spindle Delay",   	"importGCSpindleDelay" },

            {"Add Tool Cmd",   	"importGCTool" },
            {"Add Tool M0",   	"importGCToolM0" },
            {"Add Comments",   	"importGCAddComments" },
/******/
            {"Z Enable",   		"importGCZEnable" },
            {"Z Values from TT","importGCTTZAxis" },
            {"Z Feedrate",   	"importGCZFeed" },
            {"Z Up Pos",   		"importGCZUp" },
            {"Z Down Pos",   	"importGCZDown" },
            {"Z Inc Enable",   		"importGCZIncEnable" },
            {"Z Increment at zero",	"importGCZIncStartZero" },
            {"Z Increment",   		"importGCZIncrement" },
            {"Z Increment no up",   "importGCZIncNoZUp" },
            {"Z Prevent Spindle","importGCZPreventSpindle" },

            {"PWM Enable",   	"importGCPWMEnable" },
            {"PWM Up Val",   	"importGCPWMUp" },
            {"PWM Up Dly",   	"importGCPWMDlyUp" },
            {"PWM Down Val",   	"importGCPWMDown" },
            {"PWM Down Dly",   	"importGCPWMDlyDown" },
            {"PWM Zero Val",   	"importGCPWMZero" },
            {"PWM P93 Val",   	"importGCPWMP93" },
            {"PWM P93 Dly",   	"importGCPWMDlyP93" },
            {"PWM P94 Val",   	"importGCPWMP94" },
            {"PWM P94 Dly",   	"importGCPWMDlyP94" },
            {"PWM Skip M30",   	"importGCPWMSkipM30" },

            {"Spindle Toggle",   "importGCSpindleToggle" },

            {"Individual enable",	"importGCIndEnable" },
            {"Individual PenUp",   	"importGCIndPenUp" },
            {"Individual PenDown",	"importGCIndPenDown" }
        };
        internal static string sectionSetupGcodeGeneration = "GCode generation";


    }
}
