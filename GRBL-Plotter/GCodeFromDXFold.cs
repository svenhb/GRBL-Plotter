using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using GRBL_Plotter;

namespace GRBL_Plotter //DXFImporter
{
    class GCodeFromDXF
    {
        private static int svgToolMax = 100;            // max amount of tools
        private static StringBuilder[] gcodeString = new StringBuilder[svgToolMax];
        private static int gcodeStringIndex = 0;
        private static StringBuilder finalString = new StringBuilder();
        private static string dxfInfo = "";
        private static bool gcodeSpindleToggle = false; // Switch on/off spindle for Pen down/up (M3/M5)
        private static string svgInfo = "";

        private static ArrayList drawingList;
        private static ArrayList objectIdentifier;


        // Entrypoint for conversion: apply file-path or file-URL
        // return string with GCODE
        // setting will be read from Properties.Settings.Default
        public static string ConvertFile(string file)
        {
            drawingList = new ArrayList();
            objectIdentifier = new ArrayList();
            if (file == "")
            { MessageBox.Show("Empty file name"); return ""; }

            gcodeStringIndex = 0;
            gcodeString[gcodeStringIndex] = new StringBuilder();
            gcodeString[gcodeStringIndex].Clear();

            dxfInfo = "( SVG information: )\r\n";
            gcode.setup();  // initialize GCode creation (get stored settings for export)

            if (file.Substring(0, 4) == "http")
            {
            }
            else
            {
                if (File.Exists(file))
                {
                    try
                    {
                        ReadFromFile(file);
                        Draw();
                        //                       startConvert(svgCode);
                    }
                    catch (Exception e)
                    { MessageBox.Show("Error '" + e.ToString() + "' in XML file " + file + "\r\n\r\nTry to save file with other encoding e.g. UTF-8"); return ""; }
                }
                else { MessageBox.Show("File does not exist: " + file); return ""; }
            }

            gcodeString[gcodeStringIndex].Replace(',', '.');

            string header = svgInfo;
            string footer = "";
            footer += gcode.GetFooter();

            finalString.Clear();
            if (!gcodeSpindleToggle) gcode.SpindleOn(finalString, "Start spindle");
            finalString.Append(gcodeString[0].Replace(',', '.'));
            if (!gcodeSpindleToggle) gcode.SpindleOff(finalString, "Stop spindle");
            header += gcode.GetHeader();
            return header + finalString.ToString() + footer;
        }




        #region DXF Data Extraction and Interpretation
        private static FileInfo theSourceFile;
        private static double XMax, XMin;
        private static double YMax, YMin;
        private static double scaleX = 1;
        private static double scaleY = 1;
        private static double mainScale = 1;
        private static polyline thePolyLine = null;

        public static void ReadFromFile(string textFile)           //Reads a text file (in fact a DXF file) for importing an Autocad drawing.
                                                            //In the DXF File structure, data is stored in two-line groupings ( or bi-line, coupling line ...whatever you call it)
                                                            //in this grouping the first line defines the data, the second line contains the data value.
                                                           //..as a result there is always even number of lines in the DXF file..
        {
            string line1, line2;                            //these line1 and line2 is used for getting the a/m data groups...

            line1 = "0";                                    //line1 and line2 are are initialized here...
            line2 = "0";

            long position = 0;

            theSourceFile = new FileInfo(textFile);     //the sourceFile is set.

            StreamReader reader = null;                     //a reader is prepared...

            try
            {
                reader = theSourceFile.OpenText();          //the reader is set ...
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.FileName.ToString() + " cannot be found");
            }
            catch
            {
                MessageBox.Show("An error occured while opening the DXF file");
                return;
            }




            do
            {
                ////////////////////////////////////////////////////////////////////
                //This part interpretes the drawing objects found in the DXF file...
                ////////////////////////////////////////////////////////////////////

                if (line1 == "0" && line2 == "LINE")
                    LineModule(reader);

                else if (line1 == "0" && line2 == "LWPOLYLINE")
                    PolylineModule(reader);

                else if (line1 == "0" && line2 == "CIRCLE")
                    CircleModule(reader);

                else if (line1 == "0" && line2 == "ARC")
                    ArcModule(reader);

                ////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////


                GetLineCouple(reader, out line1, out line2);        //the related method is called for iterating through the text file and assigning values to line1 and line2...

            }
            while (line2 != "EOF");



            reader.DiscardBufferedData();                           //reader is cleared...
            theSourceFile = null;


            reader.Close();                                         //...and closed.

        }


        private static void GetLineCouple(StreamReader theReader, out string line1, out string line2)      //this method is used to iterate through the text file and assign values to line1 and line2
        {
            string t1 = "1.500";
            string t2 = "1,500";

            decimal dotcheck = Convert.ToDecimal(t1);
            decimal commacheck = Convert.ToDecimal(t2);

            line1 = line2 = "";

            if (theReader == null)
                return;

            line1 = theReader.ReadLine();
            if (line1 != null)
            {
                line1 = line1.Trim();

                if (dotcheck > commacheck)
                    line1 = line1.Replace('.', ',');

            }
            line2 = theReader.ReadLine();
            if (line2 != null)
            {
                line2 = line2.Trim();

                if (dotcheck > commacheck)
                    line2 = line2.Replace('.', ',');
            }

        }


        private static void LineModule(StreamReader reader)        //Interpretes line objects in the DXF file
        {
            string line1, line2;
            line1 = "0";
            line2 = "0";

            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;

            do
            {
                GetLineCouple(reader, out line1, out line2);

                if (line1 == "10")
                {
                    x1 = Convert.ToDouble(line2);

                    if (x1 > XMax)
                        XMax = x1;

                    if (x1 < XMin)
                        XMin = x1;
                }

                if (line1 == "20")
                {
                    y1 = -Convert.ToDouble(line2);
                    if (y1 > YMax)
                        YMax = y1;

                    if (y1 < YMin)
                        YMin = y1;
                }

                if (line1 == "11")
                {
                    x2 = Convert.ToDouble(line2);

                    if (x2 > XMax)
                        XMax = x2;

                    if (x2 < XMin)
                        XMin = x2;
                }

                if (line1 == "21")
                {
                    y2 = -Convert.ToDouble(line2);

                    if (y2 > YMax)
                        YMax = y2;

                    if (y2 < YMin)
                        YMin = y2;
                }


            }
            while (line1 != "21");


            mainScale = Math.Min(scaleX, scaleY);




            int ix = drawingList.Add(new Line(new Point((int)x1, (int)-y1), new Point((int)x2, (int)-y2), Color.White, 1));
            objectIdentifier.Add(new DrawingObject(2, ix));

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

        }


        private static void PolylineModule(StreamReader reader)    //Interpretes polyline objects in the DXF file
        {
            string line1, line2;
            line1 = "0";
            line2 = "0";

            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;


            thePolyLine = new polyline(Color.White, 1);

            int ix = drawingList.Add(thePolyLine);
            objectIdentifier.Add(new DrawingObject(5, ix));

            int counter = 0;
            int numberOfVertices = 1;
            int openOrClosed = 0;
            ArrayList pointList = new ArrayList();


            do
            {
                GetLineCouple(reader, out line1, out line2);

                if (line1 == "90")
                    numberOfVertices = Convert.ToInt32(line2);

                if (line1 == "70")
                    openOrClosed = Convert.ToInt32(line2);


                if (line1 == "10")
                {
                    x1 = Convert.ToDouble(line2);
                    if (x1 > XMax)
                        XMax = x1;

                    if (x1 < XMin)
                        XMin = x1;
                }

                if (line1 == "20")
                {
                    y1 = -Convert.ToDouble(line2);

                    if (y1 > YMax)
                        YMax = y1;

                    if (y1 < YMin)
                        YMin = y1;

                    pointList.Add(new Point((int)x1, (int)-y1));
                    counter++;
                }

            }
            while (counter < numberOfVertices);

            //****************************************************************************************************//
            //***************This Part is related with the drawing editor...the data taken from the dxf file******//
            //***************is interpreted hereinafter***********************************************************//


            for (int i = 1; i < numberOfVertices; i++)
            {
                thePolyLine.AppendLine(new Line((Point)pointList[i - 1], (Point)pointList[i], Color.White, 1));
            }

            if (openOrClosed == 1)
                thePolyLine.AppendLine(new Line((Point)pointList[numberOfVertices - 1], (Point)pointList[0], Color.White, 1));


            mainScale = Math.Min(scaleX, scaleY);

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////


        }


        private static void CircleModule(StreamReader reader)      //Interpretes circle objects in the DXF file
        {
            string line1, line2;
            line1 = "0";
            line2 = "0";

            double x1 = 0;
            double y1 = 0;

            double radius = 0;

            do
            {
                GetLineCouple(reader, out line1, out line2);

                if (line1 == "10")
                {
                    x1 = Convert.ToDouble(line2);

                }


                if (line1 == "20")
                {
                    y1 = -Convert.ToDouble(line2);

                }


                if (line1 == "40")
                {
                    radius = Convert.ToDouble(line2);

                    if ((x1 + radius) > XMax)
                        XMax = x1 + radius;

                    if ((x1 - radius) < XMin)
                        XMin = x1 - radius;

                    if (y1 + radius > YMax)
                        YMax = y1 + radius;

                    if ((y1 - radius) < YMin)
                        YMin = y1 - radius;

                }



            }
            while (line1 != "40");

            //****************************************************************************************************//
            //***************This Part is related with the drawing editor...the data taken from the dxf file******//
            //***************is interpreted hereinafter***********************************************************//


            mainScale = Math.Min(scaleX, scaleY);


            int ix = drawingList.Add(new circle(new Point((int)x1, (int)-y1), radius, Color.White, Color.Red, 1));
            objectIdentifier.Add(new DrawingObject(4, ix));

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////

        }


        private static void ArcModule(StreamReader reader)     //Interpretes arc objects in the DXF file
        {
            string line1, line2;
            line1 = "0";
            line2 = "0";

            double x1 = 0;
            double y1 = 0;

            double radius = 0;
            double angle1 = 0;
            double angle2 = 0;

            do
            {
                GetLineCouple(reader, out line1, out line2);

                if (line1 == "10")
                {
                    x1 = Convert.ToDouble(line2);
                    if (x1 > XMax)
                        XMax = x1;
                    if (x1 < XMin)
                        XMin = x1;

                }


                if (line1 == "20")
                {
                    y1 = -Convert.ToDouble(line2);
                    if (y1 > YMax)
                        YMax = y1;
                    if (y1 < YMin)
                        YMin = y1;
                }


                if (line1 == "40")
                {
                    radius = Convert.ToDouble(line2);

                    if ((x1 + radius) > XMax)
                        XMax = x1 + radius;

                    if ((x1 - radius) < XMin)
                        XMin = x1 - radius;

                    if (y1 + radius > YMax)
                        YMax = y1 + radius;

                    if ((y1 - radius) < YMin)
                        YMin = y1 - radius;
                }

                if (line1 == "50")
                    angle1 = Convert.ToDouble(line2);

                if (line1 == "51")
                    angle2 = Convert.ToDouble(line2);


            }
            while (line1 != "51");


            //****************************************************************************************************//
            //***************This Part is related with the drawing editor...the data taken from the dxf file******//
            //***************is interpreted hereinafter***********************************************************//


            mainScale = Math.Min(scaleX, scaleY);


            int ix = drawingList.Add(new arc(new Point((int)x1, (int)-y1), radius, angle1, angle2, Color.White, Color.Red, 1));
            objectIdentifier.Add(new DrawingObject(6, ix));

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////

        }


        #endregion




        #region Drawing and Highlighting Methods

        public class DrawingObject
        {
            public int shapeType;
            public int indexNo;

            public DrawingObject(int shapeID, int ix)
            {
                shapeType = shapeID;
                indexNo = ix;

            }
        }

        public static void Draw()// Graphics g)
        {
            Pen lePen = new Pen(Color.White, 3);


            foreach (DrawingObject obj in objectIdentifier)                     //iterates through the objects
            {
                switch (obj.shapeType)
                {
                    case 2:             //line
                        {
                            Line temp = (Line)drawingList[obj.indexNo];

                            lePen.Color = temp.AccessContourColor;
                            lePen.Width = temp.AccessLineWidth;
                            gcodeString[gcodeStringIndex].AppendFormat("(Line color{0} )\r\n", temp.AccessContourColor);

                            temp.Draw(mainScale);

                            break;
                        }
                    case 3:             //rectangle 
                        {

                            rectangle temp = (rectangle)drawingList[obj.indexNo];

                                                       lePen.Color = temp.AccessContourColor;
                                          //            lePen.Width = temp.AccessLineWidth;

                            gcodeString[gcodeStringIndex].AppendFormat("(Rect color{0} )\r\n", temp.AccessContourColor);

                            temp.Draw();// lePen, g);

                            break;
                        }
                    case 4:             //circle
                        {

                            circle temp = (circle)drawingList[obj.indexNo];

                            lePen.Color = temp.AccessContourColor;
                            lePen.Width = temp.AccessLineWidth;
                            gcodeString[gcodeStringIndex].AppendFormat("(Circle color{0} )\r\n", temp.AccessContourColor);

                            if (mainScale == 0)
                                mainScale = 1;

                            temp.Draw();// lePen, g, mainScale);

                            break;
                        }
                    case 5:             //polyline
                        {
                            polyline temp = (polyline)drawingList[obj.indexNo];

                            lePen.Color = temp.AccessContourColor;
                            lePen.Width = temp.AccessLineWidth;
                            gcodeString[gcodeStringIndex].AppendFormat("(Poly color{0} )\r\n", temp.AccessContourColor);

                            if (mainScale == 0)
                                mainScale = 1;

                            temp.Draw(mainScale);

                            break;
                        }
                    case 6:             //arc
                        {
                            arc temp = (arc)drawingList[obj.indexNo];

                            lePen.Color = temp.AccessContourColor;
                            lePen.Width = temp.AccessLineWidth;
                            gcodeString[gcodeStringIndex].AppendFormat("(Arc color{0} )\r\n", temp.AccessContourColor);

                            if (mainScale == 0)
                                mainScale = 1;

                            temp.Draw();// lePen, g, mainScale);

                            break;
                        }
                }
            }


            //	g.Dispose();		//not disposed because "g" is get from the paintbackground event..
            lePen.Dispose();
        }

        #endregion
        #region Line class
        public class Line 
        {
            protected Point startPoint;
            protected Point endPoint;
            protected Color contourColor;
            protected Color fillColor;
            protected int lineWidth;
            protected int rotation;
            protected int shapeIdentifier;


            public Line(Point start, Point end, Color color, int w)
            {
                startPoint = start;
                endPoint = end;
                contourColor = color;
                lineWidth = w;
                shapeIdentifier = 1;
                rotation = 0;

            }

            public Line()
            {

            }


            public  Color AccessContourColor
            {
                get
                {
                    return contourColor;
                }
                set
                {
                    contourColor = value;
                }
            }

            public  Color AccessFillColor
            {
                get
                {
                    return fillColor;
                }
                set
                {
                    fillColor = value;
                }
            }

            public  int AccessLineWidth
            {
                get
                {
                    return lineWidth;
                }
                set
                {
                    lineWidth = value;
                }

            }

            public  int AccessRotation
            {
                get
                {
                    return rotation;
                }
                set
                {
                    rotation = value;
                }
            }

            public  void Draw()//Pen pen, Graphics g)
            {
                gcode.Move(gcodeString[gcodeStringIndex], 0, GetStartPoint.X, GetStartPoint.Y, false, "");
                gcodePenDown();
                gcode.Move(gcodeString[gcodeStringIndex], 1, GetEndPoint.X, GetEndPoint.Y, true, "");
                gcodePenUp();
            }

            public void Draw(double scale)
            {
                gcode.Move(gcodeString[gcodeStringIndex], 0, GetStartPoint.X * (float)scale, GetStartPoint.Y * (float)scale, false, "");
                gcodePenDown();
                gcode.Move(gcodeString[gcodeStringIndex], 1, GetEndPoint.X * (float)scale, GetEndPoint.Y * (float)scale, true, "");
                gcodePenUp();
            }

            public virtual Point GetStartPoint
            {
                get
                {
                    return startPoint;
                }

            }

            public virtual Point GetEndPoint
            {
                get
                {
                    return endPoint;
                }
            }
        }
        #endregion

        #region Rectangle class
        public class rectangle
        {
            protected Point startPoint;
            protected Point endPoint;
            protected Color contourColor;
            protected Color fillColor;
            protected int lineWidth;
            protected int rotation;
            protected int shapeIdentifier;
            public rectangle(Point start, Point end, Color color, Color fill, int w, int angle)
            {
                startPoint = start;
                endPoint = end;
                contourColor = color;
                fillColor = fill;
                lineWidth = w;
                shapeIdentifier = 2;
                rotation = angle;

            }
            public Color AccessContourColor
            {
                get
                {
                    return contourColor;
                }
                set
                {
                    contourColor = value;
                }
            }

            public Color AccessFillColor
            {
                get
                {
                    return fillColor;
                }
                set
                {
                    fillColor = value;
                }
            }

            public void Draw()//Pen pen, Graphics g)
            {


/*                if (AccessRotation != 0)
                {
                    DrawRotatedRectangle(pen, g);
                    return;
                }

                g.DrawLine(pen, GetStartPoint.X, GetStartPoint.Y, GetEndPoint.X, GetStartPoint.Y);
                g.DrawLine(pen, GetEndPoint.X, GetStartPoint.Y, GetEndPoint.X, GetEndPoint.Y);
                g.DrawLine(pen, GetEndPoint.X, GetEndPoint.Y, GetStartPoint.X, GetEndPoint.Y);
                g.DrawLine(pen, GetStartPoint.X, GetEndPoint.Y, GetStartPoint.X, GetStartPoint.Y);
*/
                gcode.Move(gcodeString[gcodeStringIndex], 0, GetStartPoint.X, GetStartPoint.Y,false, "");
                gcodePenDown();
                gcode.Move(gcodeString[gcodeStringIndex], 1, GetEndPoint.X, GetStartPoint.Y, true,"");
                gcode.Move(gcodeString[gcodeStringIndex], 1, GetEndPoint.X, GetEndPoint.Y, false, "");
                gcode.Move(gcodeString[gcodeStringIndex], 1, GetStartPoint.X, GetEndPoint.Y, false, "");
                gcode.Move(gcodeString[gcodeStringIndex], 1, GetStartPoint.X, GetStartPoint.Y, false, "");
                gcodePenUp();

                return;
            }
            public virtual Point GetStartPoint
            {
                get
                {
                    return startPoint;
                }

            }

            public virtual Point GetEndPoint
            {
                get
                {
                    return endPoint;
                }
            }

            public int AccessRotation
            {
                get
                {
                    return rotation;
                }
                set
                {
                    rotation = value;
                }
            }

            private void DrawRotatedRectangle(Pen pen, Graphics g)
            {

                Point P1 = GetStartPoint;
                Point P2 = GetEndPoint;

                Point P3 = new Point(P2.X, P1.Y);
                Point P4 = new Point(P1.X, P2.Y);


                Point center = new Point(P1.X + (P3.X - P1.X) / 2, P1.Y + (P4.Y - P1.Y) / 2);

                int angle = AccessRotation;

                if (angle != 0)
                {

                    P1 = CalculateRotatedNewPoint(P1, center, angle);   //Top left
                    P3 = CalculateRotatedNewPoint(P3, center, angle);   //Bottom right

                    P2 = CalculateRotatedNewPoint(P2, center, angle);   //Top right
                    P4 = CalculateRotatedNewPoint(P4, center, angle);   //Bottom left


                    g.DrawLine(pen, P1, P3);
                    g.DrawLine(pen, P3, P2);
                    g.DrawLine(pen, P2, P4);
                    g.DrawLine(pen, P4, P1);

                    return;

                }

            }

            private Point CalculateRotatedNewPoint(Point P, Point center, int angle)
            {
                double angleRad = angle * 1 / 57.2957;

                Point tempPoint = new Point(P.X - center.X, P.Y - center.Y);

                double radius = Math.Sqrt((tempPoint.X * tempPoint.X) + (tempPoint.Y * tempPoint.Y));


                double radiant1 = Math.Acos(tempPoint.X / radius);

                if (tempPoint.X < 0 && tempPoint.Y < 0)
                    radiant1 = -radiant1;

                if (tempPoint.X > 0 && tempPoint.Y < 0)
                    radiant1 = -radiant1;

                double radiant2 = Math.Asin(tempPoint.Y / radius);

                radiant1 = radiant1 + angleRad;
                radiant2 = radiant2 + angleRad;

                double temp;
                temp = radius * Math.Cos(radiant1);
                P.X = (int)temp + center.X;



                temp = radius * Math.Sin(radiant1);
                P.Y = (int)temp + center.Y;


                return P;
            }


            private double checkPosition(Point P1, Point P2, Point current)
            {
                double m = (double)(P2.Y - P1.Y) / (P2.X - P1.X);
                return ((current.Y - P1.Y) - (m * (current.X - P1.X)));
            }

        }
        #endregion

        #region Circle Class
        public class circle
        {
            private Point centerPoint;
            private double radius;
            protected Point startPoint;
            protected Point endPoint;
            protected Color contourColor;
            protected Color fillColor;
            protected int lineWidth;
            protected int rotation;
            protected int shapeIdentifier;

            public circle(Point center, double r, Color color1, Color color2, int w)
            {
                centerPoint = center;
                radius = r;
                contourColor = color1;
                fillColor = color2;
                lineWidth = w;
                shapeIdentifier = 3;
                rotation = 0;
            }

            public  Color AccessContourColor
            {
                get
                {
                    return contourColor;
                }
                set
                {
                    contourColor = value;
                }
            }

            public  Color AccessFillColor
            {
                get
                {
                    return fillColor;
                }
                set
                {
                    fillColor = value;
                }
            }

            public  int AccessLineWidth
            {
                get
                {
                    return lineWidth;
                }
                set
                {
                    lineWidth = value;
                }

            }

            public  int AccessRotation
            {
                get
                {
                    return rotation;
                }
                set
                {
                    rotation = value;
                }
            }


            public Point AccessCenterPoint
            {
                get
                {
                    return centerPoint;
                }
                set
                {
                    centerPoint = value;
                }
            }

            public double AccessRadius
            {
                get
                {
                    return radius;
                }
            }

            public void Draw()// Pen pen, Graphics g)
            {
                //                g.DrawEllipse(pen, centerPoint.X - (int)radius, centerPoint.Y - (int)radius, (int)radius * 2, (int)radius * 2);
//                gcode.Move(gcodeString[gcodeStringIndex], 0, GetStartPoint.X, GetStartPoint.Y, false, "");
                gcode.Move(gcodeString[gcodeStringIndex],0, centerPoint.X - (int)radius, centerPoint.Y - (int)radius, 0, 0, false, "");
                gcodePenDown();
                gcode.Move(gcodeString[gcodeStringIndex],2, centerPoint.X - (int)radius, centerPoint.Y - (int)radius, -(int)radius*2, 0, false, "");
                gcodePenUp();
            }

            public void Draw(double scale)
            {
                //                g.DrawEllipse(pen, (float)centerPoint.X * (float)scale - (float)radius * (float)scale, (float)centerPoint.Y * (float)scale - (float)radius * (float)scale, (float)radius * 2 * (float)scale, (float)radius * 2 * (float)scale);
                gcode.Move(gcodeString[gcodeStringIndex], 0, centerPoint.X - (int)radius, centerPoint.Y - (int)radius, 0, 0, false, "");
                gcodePenDown();
                gcode.Move(gcodeString[gcodeStringIndex], 2, centerPoint.X - (int)radius, centerPoint.Y - (int)radius, -(int)radius * 2, 0, false, "");
                gcodePenUp();
            }

        }
        #endregion

        #region Freehand Class - Not Completed Yet
        public class FreehandTool 
        {
            private ArrayList linePoint;
            protected Point startPoint;
            protected Point endPoint;
            protected Color contourColor;
            protected Color fillColor;
            protected int lineWidth;
            protected int rotation;
            protected int shapeIdentifier;

            public FreehandTool(ArrayList points, Color color, int w)
            {

                contourColor = color;
                lineWidth = w;
                shapeIdentifier = 4;
                rotation = 0;
                linePoint = points;

            }

            public Color AccessContourColor
            {
                get
                {
                    return contourColor;
                }
                set
                {
                    contourColor = value;
                }

            }

            public Color AccessFillColor
            {
                get
                {
                    return fillColor;
                }
                set
                {
                    fillColor = value;
                }
            }


            public  int AccessLineWidth
            {
                get
                {
                    return lineWidth;
                }
                set
                {
                    lineWidth = value;
                }
            }

            public int AccessRotation
            {
                get
                {
                    return rotation;
                }
                set
                {
                    rotation = value;
                }
            }

            public void Draw(Pen pen, Graphics g)
            {

            }

            public  bool Highlight(Pen pen, Graphics g, Point point)
            {
                return false;
            }


        }
        #endregion

        #region Polyline Class

        public class polyline 
        {
            private ArrayList listOfLines;
            protected Point startPoint;
            protected Point endPoint;
            protected Color contourColor;
            protected Color fillColor;
            protected int lineWidth;
            protected int rotation;
            protected int shapeIdentifier;

            public polyline(Color color, int w)
            {
                listOfLines = new ArrayList();

                contourColor = color;
                lineWidth = w;
            }

            public  Color AccessContourColor
            {
                get
                {
                    return contourColor;
                }
                set
                {
                    contourColor = value;
                }
            }

            public  Color AccessFillColor
            {
                get
                {
                    return fillColor;
                }
                set
                {
                    fillColor = value;
                }
            }


            public  int AccessLineWidth
            {
                get
                {
                    return lineWidth;
                }
                set
                {
                    lineWidth = value;
                }

            }

            public  int AccessRotation
            {
                get
                {
                    return rotation;
                }
                set
                {
                    rotation = value;
                }
            }

            public void Draw()//Pen pen, Graphics g)
            {
                foreach (Line obj in listOfLines)
                {
                    obj.Draw();// pen, g);
                }

            }

            public void Draw( double scale)
            {
                foreach (Line obj in listOfLines)
                {
                    obj.Draw();// pen, g, scale);
                }

            }

            public void AppendLine(Line theLine)
            {
                listOfLines.Add(theLine);
            }

        }

        #endregion

        #region Arc Class

        public class arc 
        {
            private Point centerPoint;
            private double radius;

            private double startAngle;
            private double sweepAngle;
            protected Point startPoint;
            protected Point endPoint;
            protected Color contourColor;
            protected Color fillColor;
            protected int lineWidth;
            protected int rotation;
            protected int shapeIdentifier;

            public arc(Point center, double r, double startangle, double sweepangle, Color color1, Color color2, int w)
            {
                centerPoint = center;
                radius = r;
                startAngle = startangle;
                sweepAngle = sweepangle;
                contourColor = color1;
                fillColor = color2;
                lineWidth = w;
                shapeIdentifier = 3;
                rotation = 0;
            }

            public double AccessStartAngle
            {
                get
                {
                    return startAngle;
                }

            }

            public double AccessSweepAngle
            {
                get
                {
                    return sweepAngle;
                }
            }

            public Color AccessContourColor
            {
                get
                {
                    return contourColor;
                }
                set
                {
                    contourColor = value;
                }
            }

            public  Color AccessFillColor
            {
                get
                {
                    return fillColor;
                }
                set
                {
                    fillColor = value;
                }
            }

            public  int AccessLineWidth
            {
                get
                {
                    return lineWidth;
                }
                set
                {
                    lineWidth = value;
                }

            }

            public  int AccessRotation
            {
                get
                {
                    return rotation;
                }
                set
                {
                    rotation = value;
                }
            }


            public Point AccessCenterPoint
            {
                get
                {
                    return centerPoint;
                }
                set
                {
                    centerPoint = value;
                }
            }

            public double AccessRadius
            {
                get
                {
                    return radius;
                }
            }

            public void Draw()// Pen pen, Graphics g)
            {
 //               g.DrawArc(pen, (float)centerPoint.X - (float)radius, (float)centerPoint.Y - (float)radius, (float)radius * 2, (float)radius * 2, -(float)startAngle, -360 + (float)startAngle - (float)sweepAngle);
            }

            public void Draw(double scale)
            {
                //g.DrawEllipse(pen, (float) centerPoint.X* (float)scale - (float) radius* (float)scale, (float)centerPoint.Y * (float)scale - (float)radius* (float)scale, (float)radius*2* (float)scale, (float)radius*2* (float)scale);

                float tempAngle = 0;

                if (sweepAngle < startAngle)
                {
                    tempAngle = -360 + (float)startAngle - (float)sweepAngle;

                }
                else
                    tempAngle = (float)startAngle - (float)sweepAngle;

  //              g.DrawArc(pen, (float)centerPoint.X * (float)scale - (float)radius * (float)scale, (float)centerPoint.Y * (float)scale - (float)radius * (float)scale, (float)radius * 2 * (float)scale, (float)radius * 2 * (float)scale, -(float)startAngle, tempAngle);
            }



        }

        #endregion

        private static bool svgPausePenDown = true;     // if true insert pause M0 before pen down
        private static bool gcodeZApply = true;         // if true insert Z movements for Pen up/down
        private static bool applyXYFeedRate = true; // apply XY feed after each Pen-move
        private static bool penIsDown = false;

        // GCode for Pen-down
        private static void gcodePenDown()
        {
            if (svgPausePenDown) { gcode.Pause(gcodeString[gcodeStringIndex], "Pause before Pen Down"); }
            gcode.PenDown(gcodeString[gcodeStringIndex]);
            if (gcodeZApply) applyXYFeedRate = true;
            penIsDown = true;
        }
        // GCode for Pen-up
        private static void gcodePenUp(int gnr = 1)
        {
            gcode.PenUp(gcodeString[gcodeStringIndex], gnr);
            applyXYFeedRate = true;
            penIsDown = false;
        }

    }
}
