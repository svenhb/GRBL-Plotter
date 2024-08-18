/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
 * 2020-01-10 line 113 set x,y,z=null
 * 2020-12-30 add N-number
 * 2021-01-06 find parsing problem
 * 2021-01-24 extend actualPos to xyzPoint
 * 2021-03-26 add wasSetF/S/XY/Z
 * 2021-07-26 code clean up / code quality
 * 2021-07-29 line 306 add G28 to ismachineCoordG53 = true;
 * 2021-08-02 calc distance to line
 * 2021-09-04 new struct to store simulation data: SimuCoordByLine
 * 2022-04-18 line 630 simplify GetAlpha by use of atan2
*/


using NLog;
using System;

namespace GrblPlotter
{
    /// <summary>
    /// Hold absolute work coordinate for given linenumber of GCode program
    /// </summary>
    internal class CoordByLine
    {
        public int lineNumber;          // line number in fCTBCode
        public int figureNumber;
        public int actualG;
        internal XyzPoint actualPos;       // accumulates position
        internal XyzPoint lastPos;       // accumulates position
        public double alpha;            // angle between old and this position
        public double distance;         // distance to specific point
        public bool isArc;

        internal CoordByLine(int line, int figure, XyzPoint pOld, XyzPoint pNew, int g, double a, bool isarc)
        { lineNumber = line; figureNumber = figure; lastPos = pOld; actualPos = pNew; actualG = g; alpha = a; distance = -1; isArc = isarc; }

        internal CoordByLine(int line, int figure, XyzPoint pOld, XyzPoint pNew, int g, double a, double dist)
        { lineNumber = line; figureNumber = figure; lastPos = pOld; actualPos = pNew; actualG = g; alpha = a; distance = dist; isArc = false; }

        internal void CalcDistance(XyPoint tmp, bool checkDistanceToLine = true)
        {
            // bool checkDistanceToLine = true;
            XyPoint delta = new XyPoint(tmp - (XyPoint)actualPos);
            double distancePoint = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            double distanceLine = -1;
            if (checkDistanceToLine && (actualG == 1))
                distanceLine = CalcDistanceToLine(tmp);
            if (distanceLine >= 0)
                distance = Math.Min(distancePoint, distanceLine);
            else
                distance = distancePoint;
        }

        //https://stackoverflow.com/questions/34171073/how-do-i-detect-click-on-a-line-in-windows-forms
        //https://forums.codeguru.com/showthread.php?412856-How-do-I-recognize-a-mouse-click-on-a-line

        internal double CalcDistanceToLine(XyPoint tmp)	//
        {
            if ((Math.Abs(lastPos.X - actualPos.X) < 1) && (Math.Abs(lastPos.Y - actualPos.Y) < 1))
                return -1;  // if too short
            if ((tmp.X <= Math.Max(lastPos.X, actualPos.X)) && (tmp.X >= Math.Min(lastPos.X, actualPos.X)) ||
                (tmp.Y <= Math.Max(lastPos.Y, actualPos.Y)) && (tmp.Y >= Math.Min(lastPos.Y, actualPos.Y)))
            {
                XyPoint da = new XyPoint(tmp - (XyPoint)lastPos);
                double a = Math.Sqrt(da.X * da.X + da.Y * da.Y);		// side length a
                XyPoint db = new XyPoint(tmp - (XyPoint)actualPos);
                double b = Math.Sqrt(db.X * db.X + db.Y * db.Y);		// side length b
                XyPoint dc = new XyPoint((XyPoint)actualPos - (XyPoint)lastPos);
                double c = Math.Sqrt(dc.X * dc.X + dc.Y * dc.Y);		// side length c
                double s = (a + b + c) / 2;
                double h = 2 / c * Math.Sqrt(s * (s - a) * (s - b) * (s - c));	// height over c
                return h;
            }
            else
                return -1;
        }

        internal double CalcDistanceToLine1(XyPoint tmp)
        {
            double DelX = actualPos.X - lastPos.X;
            double DelY = actualPos.Y - lastPos.Y;
            double D = Math.Sqrt(DelX * DelX + DelY * DelY);
            if (D < 0.1) return -1;

            double Ratio = (double)((tmp.X - lastPos.X) * DelX + (tmp.Y - lastPos.Y) * DelY) / (DelX * DelX + DelY * DelY);
            if (Ratio * (1 - Ratio) < 0)
                return -1;
            return (double)Math.Abs(DelX * (tmp.Y - lastPos.Y) - DelY * (tmp.X - lastPos.X)) / D;
        }

    }

    internal struct CenterByLine
    {
        public int lineNumber;          // line number in fCTBCode
        public int figureNumber;
        public XyzPoint center;
        public XyzPoint actualPos;
        public double alpha;

        public CenterByLine(int linenr, int fig, XyzPoint cent, XyzPoint coord, double a)
        { lineNumber = linenr; figureNumber = fig; center = cent; actualPos = coord; alpha = a; }
        public double CalcDistance(XyPoint tmp)
        {
            double a = center.X - tmp.X;
            double b = center.Y - tmp.Y;
            return Math.Sqrt(a * a + b * b);
        }

    }
    internal struct DistanceByLine
    {
        public int lineNumber;          // line number in fCTBCode
        public int figureNumber;
        public double distance;
        public XyzPoint actualPos;
        public double alpha;
        public bool isArc;

        public DistanceByLine(int x)
        { lineNumber = x; figureNumber = x; actualPos = new XyzPoint(); alpha = 0; distance = 0; isArc = false; }
        public DistanceByLine(CoordByLine tmp, double dist)
        { lineNumber = tmp.lineNumber; figureNumber = tmp.figureNumber; actualPos = tmp.actualPos; alpha = tmp.alpha; distance = dist; isArc = tmp.isArc; }
        public DistanceByLine(CenterByLine tmp, double dist)
        { lineNumber = tmp.lineNumber; figureNumber = tmp.figureNumber; actualPos = tmp.center; alpha = tmp.alpha; distance = dist; isArc = true; }
        public DistanceByLine(int lnr, int fnr, XyzPoint apos, double a, double dist, bool ia)
        { lineNumber = lnr; figureNumber = fnr; actualPos = apos; alpha = a; distance = dist; isArc = ia; }
    }


    internal struct XyzabcuvwPoint
    {
        public double X, Y, Z, A, B, C, U, V, W;
        public XyzabcuvwPoint(XyzPoint tmp)
        { X = tmp.X; Y = tmp.Y; Z = tmp.Z; A = tmp.A; B = 0; C = 0; U = 0; V = 0; W = 0; }

        public static XyzabcuvwPoint operator -(XyzabcuvwPoint a, XyzabcuvwPoint b)
        { return new XyzabcuvwPoint() { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z, A = a.A - b.A, B = a.B - b.B, C = a.C - b.C }; }

        public static explicit operator XyPoint(XyzabcuvwPoint tmp)
        { return new XyPoint(tmp.X, tmp.Y); }
        public static explicit operator XyzPoint(XyzabcuvwPoint tmp)
        { return new XyzPoint(tmp.X, tmp.Y, tmp.Z, tmp.A, tmp.B, tmp.C); }
        public static explicit operator XyArcPoint(XyzabcuvwPoint tmp)
        { return new XyArcPoint(tmp.X, tmp.Y, 0, 0, 0); }
    }

    /// <summary>
    /// Hold parsed GCode line and absolute work coordinate for given linenumber of GCode program
    /// </summary>
    class GcodeByLine
    {
        public int lineNumber;          // line number in fCTBCode
        public int nNumber;             // n number in GCode if given
        public int figureNumber;
        public string codeLine;         // copy of original gcode line
        public double? x, y, z, a, b, c, u, v, w, i, j; // current parameters
        public XyzabcuvwPoint actualPos;      // accumulates position

        // ModalGroups <Parser State="G1 G54 G17 G21 G90 G94 M3 M9 T0 F1000 S10000" />
        public byte motionMode;         // G0,1,2,3
        public byte coordSystem;        // Coordinate System Modes: G54, G55, G56, G57, G58, G59
        public byte planeSelect;        // G17,18,19
        public bool isunitModeG21;      // G20,21
        public bool isdistanceModeG90;  // G90,91
        public bool isfeedrateModeG94;  // Feed Rate Modes: G93, G94
        public byte spindleState;       // M3,4,5
        public byte coolantState;       // M7,8,9
        public byte toolNumber;         // T0
        public int feedRate;            // actual feed rate
        public int spindleSpeed;        // actual spindle speed

        public bool ismachineCoordG53;  // don't apply transform to machine coordinates
        public bool isSubroutine;
        public bool isSetCoordinateSystem;  // don't process x,y,z if set coordinate system
        public bool isNoMove;               // don't process x,y,z if G10, G17, G43...

        public bool wasSetF;
        public bool wasSetS;
        public bool wasSetXY;
        public bool wasSetZ;

        public double alpha;            // angle between old and this position
        public double distance;         // distance to specific point
        public string otherCode;
        //    public string info;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public GcodeByLine()
        { ResetAll(); }
        public GcodeByLine(GcodeByLine tmp)
        {
            ResetAll();
            lineNumber = tmp.lineNumber; nNumber = tmp.nNumber; figureNumber = tmp.figureNumber; codeLine = tmp.codeLine;
            x = tmp.x; y = tmp.y; z = tmp.z; i = tmp.i; j = tmp.j; a = tmp.a; b = tmp.b; c = tmp.c; u = tmp.u; v = tmp.v; w = tmp.w;
            actualPos = tmp.actualPos;
            motionMode = tmp.motionMode; coordSystem = tmp.coordSystem; planeSelect = tmp.planeSelect;
            isunitModeG21 = tmp.isunitModeG21; isdistanceModeG90 = tmp.isdistanceModeG90; isfeedrateModeG94 = tmp.isfeedrateModeG94;
            spindleState = tmp.spindleState; coolantState = tmp.coolantState; toolNumber = tmp.toolNumber;
            feedRate = tmp.feedRate; spindleSpeed = tmp.spindleSpeed;

            ismachineCoordG53 = tmp.ismachineCoordG53; isSubroutine = tmp.isSubroutine;
            isSetCoordinateSystem = tmp.isSetCoordinateSystem; isNoMove = tmp.isNoMove;

            wasSetF = tmp.wasSetF; wasSetS = tmp.wasSetS; wasSetXY = tmp.wasSetXY; wasSetZ = tmp.wasSetZ;

            distance = tmp.distance; alpha = tmp.alpha;
            otherCode = tmp.otherCode;
        }

        public string ListData()
        { return string.Format("{0} mode {1} figure {2}  {3}", lineNumber, motionMode, figureNumber, codeLine); }

        /// <summary>
        /// Reset coordinates and set G90, M5, M9
        /// </summary>
        public void ResetAll()
        {
            lineNumber = 0; nNumber = -1; figureNumber = 0; codeLine = "";
            x = y = z = a = b = c = u = v = w = i = j = null;
            actualPos.X = 0; actualPos.Y = 0; actualPos.Z = 0; actualPos.A = 0; actualPos.B = 0; actualPos.C = 0;
            actualPos.U = 0; actualPos.V = 0; actualPos.W = 0;
            motionMode = 0; coordSystem = 54; planeSelect = 17; isunitModeG21 = true;
            isdistanceModeG90 = true; isfeedrateModeG94 = true; spindleState = 5; coolantState = 9;
            toolNumber = 0; feedRate = 0; spindleSpeed = 0;
            ismachineCoordG53 = false; isSubroutine = false;
            isSetCoordinateSystem = false; isNoMove = false;

            wasSetF = wasSetS = wasSetXY = wasSetZ = false;
            distance = -1; otherCode = ""; alpha = 0;//info = "";

            ResetCoordinates();
        }
        public void ResetAll(XyzPoint tmp)
        {
            ResetAll();
            actualPos = new XyzabcuvwPoint(tmp);
        }
        /// <summary>
        /// Reset coordinates
        /// </summary>
        public void ResetCoordinates()
        {
            x = null; y = null; z = null; a = null; b = null; c = null; u = null; v = null; w = null; i = null; j = null;
        }
        public void PresetParsing(int lineNr, string line)
        {
            ResetCoordinates();
            ismachineCoordG53 = false; isSubroutine = false;
            otherCode = "";
            lineNumber = lineNr;
            nNumber = -1;
            codeLine = line;
        }

        /// <summary>
        /// parse gcode line
        /// </summary>
        public void ParseLine(int lineNr, string line, ref ModalGroup modalState)
        {
            PresetParsing(lineNr, line);
            char cmd = '\0';
            string num = "";
            bool comment = false;
            double value;
            line = line.ToUpper().Trim();   // 2020-07-26
            isSetCoordinateSystem = false;
            isNoMove = false;
            wasSetF = wasSetS = wasSetXY = wasSetZ = false;
            #region parse
            if ((!(line.StartsWith("$") || line.StartsWith("("))) && (line.Length > 1))//do not parse grbl comments
            {
                try
                {
                    foreach (char cil in line)
                    {
                        if (cil == ';')                                   // comment?
                            break;
                        if (cil == '(')                                   // comment starts
                        { comment = true; }
                        if (!comment)
                        {
                            if (Char.IsLetter(cil))                       // if char is letter
                            {
                                if (cmd != '\0')                        // and command is set
                                {
                                    if (double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out value))
                                        ParseGCodeToken(cmd, value, ref modalState);
                                }
                                cmd = cil;                                // char is a command
                                num = "";
                            }
                            else if (Char.IsNumber(cil) || cil == '.' || cil == '-')  // char is not letter but number
                            {
                                num += cil;
                            }
                        }

                        if (cil == ')')                                   // comment ends
                        { comment = false; }
                    }
                    if (cmd != '\0')                                    // finally after for-each process final command and number
                    {   //Logger.Trace("parseLine {0}  {1}",cmd, num);
                        if (double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out value))
                            ParseGCodeToken(cmd, value, ref modalState);
                    }
                }
                catch (Exception er) { Logger.Error(er, "parseLine"); }
            }
            #endregion
            if (isSetCoordinateSystem)
                ResetCoordinates();
        }

        /// <summary>
        /// fill current gcode line structure
        /// </summary>
        private void ParseGCodeToken(char cmd, double value, ref ModalGroup modalState)
        {
            //            Logger.Trace("parseGCodeToken {0}  {1}",cmd, value);
            switch (Char.ToUpper(cmd))
            {
                case 'X':
                    x = value;
                    wasSetXY = true;
                    break;
                case 'Y':
                    y = value;
                    wasSetXY = true;
                    break;
                case 'Z':
                    z = value;
                    wasSetZ = true;
                    break;
                case 'A':
                    a = value;
                    break;
                case 'B':
                    b = value;
                    break;
                case 'C':
                    c = value;
                    break;
                case 'U':
                    u = value;
                    break;
                case 'V':
                    v = value;
                    break;
                case 'W':
                    w = value;
                    break;
                case 'I':
                    i = value;
                    break;
                case 'J':
                    j = value;
                    break;
                case 'N':
                    nNumber = (int)value;
                    break;
                case 'F':
                    modalState.feedRate = feedRate = (int)value;
                    wasSetF = true;
                    break;
                case 'S':
                    modalState.spindleSpeed = spindleSpeed = (int)value;
                    wasSetS = true;
                    break;
                case 'G':
                    if (value <= 3)                                 // Motion Mode 0-3 c
                    {
                        modalState.motionMode = motionMode = (byte)value;
                        if (value >= 2)
                            modalState.containsG2G3 = true;
                    }
                    else
                    {
                        otherCode += "G" + ((int)value).ToString() + " ";
                    }

                    if (value == 10)
                    { isSetCoordinateSystem = true; isNoMove = true; }

                    else if ((value == 17) || (value == 18) || (value == 19))   // planeSelect
                    { modalState.planeSelect = planeSelect = (byte)value; }

                    else if ((value == 20) || (value == 21))                    // Units Mode
                    {
                        modalState.unitsMode = (byte)value; isNoMove = true;
                        isunitModeG21 = (value == 20);
                    }

                    else if ((value >= 43) && (value < 53))
                    { isNoMove = true; }

                    else if ((value == 53) || (value == 28))          		// move in machine coord.
                    { ismachineCoordG53 = true; }

                    else if ((value >= 54) && (value <= 59))             // Coordinate System Select
                    { modalState.coordinateSystem = coordSystem = (byte)value; isNoMove = true; }

                    else if (value == 90)                                // Distance Mode
                    { modalState.distanceMode = (byte)value; modalState.isdistanceModeG90 = true; }
                    else if (value == 91)
                    {
                        modalState.distanceMode = (byte)value; modalState.isdistanceModeG90 = false;
                        modalState.containsG91 = true;
                    }
                    else if ((value == 93) || (value == 94))             // Feed Rate Mode
                    {
                        modalState.feedRateMode = (byte)value;
                        isfeedrateModeG94 = (value == 94);
                    }
                    break;
                case 'M':
                    if ((value < 3) || (value == 30))                   // Program Mode 0, 1 ,2 ,30
                    { modalState.programMode = (byte)value; }
                    else if (value >= 3 && value <= 5)                   // Spindle State
                    { modalState.spindleState = spindleState = (byte)value; }
                    else if (value >= 7 && value <= 9)                   // Coolant State
                    { modalState.coolantState = coolantState = (byte)value; }
                    modalState.mWord = (byte)value;
                    if ((value < 3) || (value > 9))
                        otherCode += "M" + ((int)value).ToString() + " ";
                    break;
                case 'T':
                    modalState.tool = toolNumber = (byte)value;
                    otherCode += "T" + ((int)value).ToString() + " ";
                    break;
                case 'P':
                    modalState.pWord = (int)value;
                    otherCode += "P" + value.ToString() + " ";
                    break;
                case 'O':
                    modalState.oWord = (int)value;
                    break;
                case 'L':
                    modalState.lWord = (int)value;
                    break;
            }
            isdistanceModeG90 = modalState.isdistanceModeG90;
        }
    };

    internal struct SimuCoordByLine
    {
        public int lineNumber;          // line number in fCTBCode
        public XyzPoint actualPos;
        public XyzPoint actualOffset;
        public double? i, j, z;
        public double alpha;            // angle between old and this position
        public byte motionMode;
        public int feedRate;            // actual feed rate
        public string codeLine;         // copy of original gcode line

        public SimuCoordByLine(GcodeByLine tmp, System.Drawing.PointF offset)
        {
            lineNumber = tmp.lineNumber; actualPos = (XyzPoint)tmp.actualPos;
            actualOffset = new XyzPoint
            {
                X = offset.X,
                Y = offset.Y
            };
            i = tmp.i; j = tmp.j; z = tmp.z; alpha = tmp.alpha;
            motionMode = tmp.motionMode; feedRate = tmp.feedRate;
            codeLine = tmp.codeLine;
        }
        public SimuCoordByLine(SimuCoordByLine tmp)
        {
            lineNumber = tmp.lineNumber; actualPos = (XyzPoint)tmp.actualPos;
            actualOffset = tmp.actualOffset;
            i = tmp.i; j = tmp.j; z = tmp.z; alpha = tmp.alpha;
            motionMode = tmp.motionMode; feedRate = tmp.feedRate;
            codeLine = tmp.codeLine;
        }
    }


    internal class ModalGroup
    {
        public byte motionMode;           // G0, G1, G2, G3, //G38.2, G38.3, G38.4, G38.5, G80
        public byte coordinateSystem;     // G54, G55, G56, G57, G58, G59
        public byte planeSelect;          // G17, G18, G19
        public byte distanceMode;         // G90, G91
        public byte feedRateMode;         // G93, G94
        public byte unitsMode;            // G20, G21
        public byte programMode;          // M0, M1, M2, M30
        public byte spindleState;         // M3, M4, M5
        public byte coolantState;         // M7, M8, M9
        public byte tool;                 // T
        public int spindleSpeed;          // S
        public int feedRate;              // F
        public int mWord;
        public int pWord;
        public int oWord;
        public int lWord;
        public bool containsG2G3;
        public bool ismachineCoordG53;
        public bool isdistanceModeG90;
        public bool containsG91;

        public ModalGroup()     // reset state
        { Reset(); }

        public void Reset()
        {
            motionMode = 0;             // G0, G1, G2, G3, G38.2, G38.3, G38.4, G38.5, G80
            coordinateSystem = 54;      // G54, G55, G56, G57, G58, G59
            planeSelect = 17;           // G17, G18, G19
            distanceMode = 90;          // G90, G91
            feedRateMode = 94;          // G93, G94
            unitsMode = 21;             // G20, G21
            programMode = 0;            // M0, M1, M2, M30
            spindleState = 5;           // M3, M4, M5
            coolantState = 9;           // M7, M8, M9
            tool = 0;                   // T
            spindleSpeed = 0;           // S
            feedRate = 0;               // F
            mWord = 0;
            pWord = 0;
            oWord = 0;
            lWord = 1;
            containsG2G3 = false;
            ismachineCoordG53 = false;
            isdistanceModeG90 = true;
            containsG91 = false;
        }
        public void ResetSubroutine()
        {
            mWord = 0;
            pWord = 0;
            oWord = 0;
            lWord = 1;
        }
    }

    struct ArcProperties
    {
        public double angleStart, angleEnd, angleDiff, radius;
        public XyPoint center;
    };

    internal static class GcodeMath
    {
        internal static double precision = 0.00001;

        internal static bool IsEqual(System.Windows.Point a, System.Windows.Point b)
        { return ((Math.Abs(a.X - b.X) < precision) && (Math.Abs(a.Y - b.Y) < precision)); }
        internal static bool IsEqual(XyPoint a, XyPoint b)
        { return ((Math.Abs(a.X - b.X) < precision) && (Math.Abs(a.Y - b.Y) < precision)); }

        internal static double DistancePointToPoint(System.Windows.Point a, System.Windows.Point b)
        { return Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y))); }
        internal static double DistancePointToPoint(XyPoint a, XyPoint b)
        { return Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y))); }

        internal static ArcProperties GetArcMoveProperties(System.Windows.Point pOld, System.Windows.Point pNew, System.Windows.Point centerIJ, bool isG2)
        { return GetArcMoveProperties(new XyPoint(pOld), new XyPoint(pNew), centerIJ.X, centerIJ.Y, isG2); }
        internal static ArcProperties GetArcMoveProperties(XyPoint pOld, XyPoint pNew, XyPoint center, bool isG2)
        { return GetArcMoveProperties(pOld, pNew, pOld.X - center.X, pOld.Y - center.Y, isG2); }

        internal static ArcProperties GetArcMoveProperties(XyPoint pOld, XyPoint pNew, double? I, double? J, bool isG2)
        {
            ArcProperties tmp = GetArcMoveAngle(pOld, pNew, I, J);
            if (!isG2) { tmp.angleDiff = Math.Abs(tmp.angleEnd - tmp.angleStart + 2 * Math.PI); }
            if (tmp.angleDiff > (2 * Math.PI)) { tmp.angleDiff -= (2 * Math.PI); }
            if (tmp.angleDiff < (-2 * Math.PI)) { tmp.angleDiff += (2 * Math.PI); }

            if ((pOld.X == pNew.X) && (pOld.Y == pNew.Y))
            {
                if (isG2) { tmp.angleDiff = -2 * Math.PI; }
                else { tmp.angleDiff = 2 * Math.PI; }
            }
            return tmp;
        }

        internal static ArcProperties GetArcMoveAngle(XyPoint pOld, XyPoint pNew, double? I, double? J)
        {
            ArcProperties tmp;
            if (I == null) { I = 0; }
            if (J == null) { J = 0; }
            double i = (double)I;
            double j = (double)J;
            tmp.radius = Math.Sqrt(i * i + j * j);  // get radius of circle
            tmp.center.X = pOld.X + i;
            tmp.center.Y = pOld.Y + j;
            tmp.angleStart = tmp.angleEnd = tmp.angleDiff = 0;
            if (tmp.radius == 0)
                return tmp;

            double cos1 = i / tmp.radius;
            if (cos1 > 1) cos1 = 1;
            if (cos1 < -1) cos1 = -1;
            tmp.angleStart = Math.PI - Math.Acos(cos1);
            if (j > 0) { tmp.angleStart = -tmp.angleStart; }

            if ((j == 0) && (Math.Abs(pOld.DistanceTo(pNew)) < precision))   // full circle
            {
                tmp.angleDiff = -2 * Math.PI;
                return tmp;
            }

            double cos2 = (tmp.center.X - pNew.X) / tmp.radius;
            if (cos2 > 1) cos2 = 1;
            if (cos2 < -1) cos2 = -1;
            tmp.angleEnd = Math.PI - Math.Acos(cos2);
            if ((tmp.center.Y - pNew.Y) > 0) { tmp.angleEnd = -tmp.angleEnd; }

            tmp.angleDiff = tmp.angleEnd - tmp.angleStart - 2 * Math.PI;
            return tmp;
        }

        internal static double GetAlpha(System.Windows.Point pOld, System.Windows.Point pNew)
        { return GetAlpha(pOld.X, pOld.Y, pNew.X, pNew.Y); }
        internal static double GetAlpha(XyPoint pOld, XyPoint pNew)
        { return GetAlpha(pOld.X, pOld.Y, pNew.X, pNew.Y); }
        internal static double GetAlpha(double P1x, double P1y, double P2x, double P2y)
        {
            double dx = P2x - P1x;
            double dy = P2y - P1y;
            return Math.Atan2(dy, dx);
        }

        internal static double cutAngle = 0, cutAngleLast = 0, angleOffset = 0;
    /*    internal static void ResetAngles()
        { angleOffset = cutAngle = cutAngleLast = 0.0; }
        internal static double /(System.Windows.Point a, System.Windows.Point b, double offset, int dir)
        {
         //   if (dir <= 2)
                return MonitorAngle(GetAlpha(a, b) + offset, dir);  // CW add 
        //    else
       //         return MonitorAngle(GetAlpha(a, b) - offset, dir);
        }*/
    /*    internal static double MonitorAngle(double angle, int direction)		// take care of G2 cw G3 ccw direction
        {
            double diff = angle - cutAngleLast + angleOffset;
            if (direction == 2)
            { if (diff > 0) { angleOffset -= 2 * Math.PI; } }    // clock wise, more negative
            else if (direction == 3)
            { if (diff < 0) { angleOffset += 2 * Math.PI; } }    // counter clock wise, more positive
            else
            {
                if (diff > Math.PI)
                    angleOffset -= 2 * Math.PI;
                if (diff < -Math.PI)
                    angleOffset += 2 * Math.PI;
            }
        //    angle += angleOffset;
            return angle;
        }*/
    }
}
