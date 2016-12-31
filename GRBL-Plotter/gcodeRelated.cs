/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2016 Sven Hasemann contact: svenhb@web.de

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public static class gcode
    {   private static string formatCode = "00";
        private static string formatNumber = "0.000";

        private static int gcodeLines = 0;              // counter for GCode lines
        private static float gcodeDistance = 0;         // counter for GCode move distance
        private static float gcodeTime = 0;             // counter for GCode work time
        private static int gcodePauseCounter = 0;       // counter for GCode pause M0 commands
        private static int gcodeToolCounter = 0;       // counter for GCode Tools
        private static string gcodeToolText = "";       // counter for GCode Tools

        private static float gcodeXYFeed = 1999;        // XY feed to apply for G1
        private static bool gcodeComments = true;       // if true insert additional comments into GCode

        private static bool gcodeToolChange = false;          // Apply tool exchange command

        // Using Z-Axis for Pen up down
        private static bool gcodeZApply = true;         // if true insert Z movements for Pen up/down
        private static float gcodeZUp = 1.999f;         // Z-up position
        private static float gcodeZDown = -1.999f;      // Z-down position
        private static float gcodeZFeed = 499;          // Z feed to apply for G1

        // Using Spindle pwr. to switch on/off laser
        private static bool gcodeSpindleToggle = false; // Switch on/off spindle for Pen down/up (M3/M5)
        private static float gcodeSpindleSpeed = 999; // Spindle speed to apply

        // Using Spindle-Speed als PWM output to control RC-Servo
        private static bool gcodePWMEnable = false;     // Change Spindle speed for Pen down/up
        private static float gcodePWMUp = 199;          // Spindle speed for Pen-up
        private static float gcodePWMDlyUp = 0;         // Delay to apply after Pen-up (because servo is slow)
        private static float gcodePWMDown = 799;        // Spindle speed for Pen-down
        private static float gcodePWMDlyDown = 0;       // Delay to apply after Pen-down (because servo is slow)

        private static bool reduceCode = false;         // reduce code by avoiding sending again same G-Nr and unchanged coordinates

        public static void setup()
        {
            setDecimalPlaces((int)Properties.Settings.Default.importGCDecPlaces);
            gcodeXYFeed = (float)Properties.Settings.Default.importGCXYFeed;

            gcodeComments = Properties.Settings.Default.importGCAddComments;
            gcodeSpindleToggle = Properties.Settings.Default.importGCSpindleToggle;
            gcodeSpindleSpeed = (float)Properties.Settings.Default.importGCSSpeed;

            gcodeZApply = Properties.Settings.Default.importGCZEnable;
            gcodeZUp = (float)Properties.Settings.Default.importGCZUp;
            gcodeZDown = (float)Properties.Settings.Default.importGCZDown;
            gcodeZFeed = (float)Properties.Settings.Default.importGCZFeed;

            gcodePWMEnable = Properties.Settings.Default.importGCPWMEnable;
            gcodePWMUp = (float)Properties.Settings.Default.importGCPWMUp;
            gcodePWMDlyUp = (float)Properties.Settings.Default.importGCPWMDlyUp;
            gcodePWMDown = (float)Properties.Settings.Default.importGCPWMDown;
            gcodePWMDlyDown = (float)Properties.Settings.Default.importGCPWMDlyDown;

            gcodeToolChange = Properties.Settings.Default.importGCTool;

            reduceCode = false;         // reduce code by 
            gcodeLines = 0;              // counter for GCode lines
            gcodeDistance = 0;         // counter for GCode move distance
            gcodeTime = 0;             // counter for GCode work time
            gcodePauseCounter = 0;       // counter for GCode pause M0 commands
            gcodeToolCounter = 0;
            gcodeToolText = "";
            lastx = 0; lasty=0;
        }

        public static bool reduceGCode
        {
            get { return reduceCode; }
            set { reduceCode = value; }
        }

        public static void setDecimalPlaces(int num)
        {   formatNumber = "0.";
            formatNumber = formatNumber.PadRight(num + 2, '0');
        }

        // get GCode one or two digits
        public static int getIntGCode(char code, string tmp)
        {   string cmdG = getStrGCode(code, tmp);
            if (cmdG.Length > 0)
            {  return Convert.ToInt16(cmdG.Substring(1));  }
            return -1;
        }
        public static string getStrGCode(char code,string tmp)
        {
            var cmdG = Regex.Matches(tmp, code+"\\d{1,2}");
            if (cmdG.Count > 0)
            {  return cmdG[0].ToString();  }
            return "";
        }

        // get value from X,Y,Z F, S etc.
        public static int getIntValue(char code, string tmp)
        {
            string cmdG = getStringValue(code, tmp);
//            MessageBox.Show(cmdG);
            if (cmdG.Length > 0)
            {  return Convert.ToInt16(cmdG.Substring(1));  }
            return -1;
        }
        public static string getStringValue(char code, string tmp)
        {
            var cmdG = Regex.Matches(tmp, code+ "-?\\d+(.\\d+)?");
            if (cmdG.Count > 0)
            {  return cmdG[cmdG.Count-1].ToString(); }
            return "";
        }


        public static string frmtCode(int number)      // convert int to string using format pattern
        {   return number.ToString(formatCode); }
        public static string frmtNum(float number)     // convert float to string using format pattern
        {   return number.ToString(formatNumber); }
        public static string frmtNum(double number)     // convert double to string using format pattern
        {   return number.ToString(formatNumber); }

        public static void Pause(StringBuilder gcodeString, string cmt="")
        {
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
            gcodeString.AppendFormat("M{0:00} {1}\r\n",0,cmt);
            gcodeLines++;
            gcodePauseCounter++;
        }

        public static void SpindleOn(StringBuilder gcodeString, string cmt="")
        {
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
            gcodeString.AppendFormat("M{0} S{1} {2}\r\n", frmtCode(3), gcodeSpindleSpeed, cmt);
            gcodeLines++;
        }

        public static void SpindleOff(StringBuilder gcodeString, string cmt="")
        {
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
            gcodeString.AppendFormat("M{0} {1}\r\n", frmtCode(5), cmt);
            gcodeLines++;
        }

        public static void PenDown(StringBuilder gcodeString)
        {
            string cmt = "";
            if (gcodeComments) cmt = "Pen Down";
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);

            if (gcodeSpindleToggle) SpindleOn(gcodeString, cmt);
            if (gcodeZApply)
            {
                gcodeString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", frmtCode(1), frmtNum(gcodeZDown), gcodeZFeed, cmt);
                gcodeTime += Math.Abs((gcodeZUp - gcodeZDown) / gcodeZFeed);
                gcodeLines++; lastg = 1; lastf = gcodeZFeed;
            }
            if (gcodePWMEnable)
            {   gcodeString.AppendFormat("M{0} S{1} {2}\r\n", frmtCode(3), gcodePWMDown, cmt);
                gcodeString.AppendFormat("G{0} P{1}\r\n", frmtCode(4), frmtNum(gcodePWMDlyDown));
                gcodeTime += gcodePWMDlyDown;
                gcodeLines++;
            }
        }

        public static void PenUp(StringBuilder gcodeString)
        {
            string cmt = "";
            if (gcodeComments) cmt = "Pen Up";
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);

            if (gcodePWMEnable)
            {   gcodeString.AppendFormat("M{0} S{1} {2}\r\n", frmtCode(3), gcodePWMUp, cmt);
                gcodeString.AppendFormat("G{0} P{2}\r\n", frmtCode(4), frmtNum(gcodePWMDlyUp));
                gcodeTime += gcodePWMDlyDown;
                gcodeLines++;
            }
            if (gcodeZApply)
            {
                gcodeString.AppendFormat("G{0} Z{1} F{2} {3}\r\n", frmtCode(1), frmtNum(gcodeZUp), gcodeZFeed, cmt);
                gcodeTime += Math.Abs((gcodeZUp - gcodeZDown) / gcodeZFeed);
                gcodeLines++; lastg = 1; lastf = gcodeZFeed;
            }
            if (gcodeSpindleToggle) SpindleOff(gcodeString, cmt);
        }

        private static float lastx, lasty, lastg, lastf;
        public static void Move(StringBuilder gcodeString, int gnr, float x, float y, bool applyFeed, string cmt="")
        {
            string feed = "";
            StringBuilder gcodeTmp = new StringBuilder();
            bool isneeded = false;
            if (applyFeed && (gnr > 0)) { feed = string.Format("F{0}", gcodeXYFeed); }
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
            if (reduceCode)
            {
                if (lastg != gnr) { gcodeTmp.AppendFormat("G{0} ", frmtCode(gnr)); isneeded = true; }
                if (lastx != x) { gcodeTmp.AppendFormat("X{0} ", frmtNum(x)); isneeded = true; }
                if (lasty != y) { gcodeTmp.AppendFormat("Y{0} ", frmtNum(y)); isneeded = true; }
                if ((gnr == 1) && (lastf != gcodeXYFeed) || applyFeed)
                {
                    gcodeTmp.AppendFormat("F{0} ", gcodeXYFeed);
                    lastf = gcodeXYFeed;
                    isneeded = true;              
                }
                gcodeTmp.AppendFormat("{0}\r\n", cmt);
                if (isneeded)
                    gcodeString.Append(gcodeTmp);
            }
            else
                gcodeString.AppendFormat("G{0} X{1} Y{2} {3} {4}\r\n", frmtCode(gnr), frmtNum(x), frmtNum(y), feed, cmt);
            gcodeDistance += fdistance(lastx, lasty, x, y);
            lastx = x; lasty = y; lastg = gnr; 
            gcodeLines++;
        }
        public static void Move(StringBuilder gcodeString, int gnr, float x, float y, float i, float j, bool applyFeed, string cmt="")
        {
            string feed = "";
            if (applyFeed) { feed = string.Format("F{0}", gcodeXYFeed); }
            if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
            gcodeString.AppendFormat("G{0} X{1} Y{2}  I{3} J{4} {5} ({6})\r\n", frmtCode(gnr), frmtNum(x), frmtNum(y), frmtNum(i), frmtNum(j), feed, cmt);
            gcodeDistance += fdistance(lastx, lasty, x, y);
            lastx = x; lasty = y; lastg = gnr; lastf = gcodeXYFeed;
            gcodeLines++;
        }

        public static void Tool(StringBuilder gcodeString, int toolnr, string cmt="")
        {
            if (gcodeToolChange)                // otherweise no command needed
            {
                if (cmt.Length > 0) cmt = string.Format("({0})", cmt);
                gcodeString.AppendFormat("M{0} T{1:D2} {2}\r\n", frmtCode(6), toolnr, cmt);
                gcodeToolCounter++;
                gcodeLines++;
                gcodeToolText += string.Format("( {0}) ToolNr: {1:D2}, Name: {2})\r\n", gcodeToolCounter,toolnr, cmt);
            }
        }

        public static string GetHeader()
        {
            gcodeTime += gcodeDistance / gcodeXYFeed;
            string header ="";
            header += string.Format("( G-Code lines: {0} )\r\n", gcodeLines);
            header += string.Format("( Path length : {0:0.0} units )\r\n", gcodeDistance);
            header += string.Format("( Duration    : {0:0.0} min. )\r\n", gcodeTime);
            header += string.Format("( Tool changes: {0})\r\n", gcodeToolCounter);
            header += gcodeToolText;
            header += string.Format("( M0 count    : {0})\r\n", gcodePauseCounter);
            string[] commands = Properties.Settings.Default.importGCHeader.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                    header += string.Format("{0} (Setup - GCode-Header)\r\n", cmd.Trim());
            return header;
        }

        public static string GetFooter()
        {
            string footer = "";
            string[] commands = Properties.Settings.Default.importGCFooter.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                    footer += string.Format("{0} (Setup - GCode-Footer)\r\n", cmd.Trim());
            return footer;
        }

        // helper functions
        private static float fsqrt(float x) { return (float)Math.Sqrt(x); }
        private static float fvmag(float x, float y) { return fsqrt(x * x + y * y); }
        private static float fdistance(float x1, float y1, float x2, float y2) { return fvmag(x2 - x1, y2 - y1); }
    }
}
