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

/*  Thanks to martin2250  https://github.com/martin2250/OpenCNCPilot for his HeightMap class
 * 2022-01-12 Except: Nullable object must have a value - Method:InterpolateZ
 * 2022-03-13 add extrudeX and Y option
 * 2022-04-25 line 1044 BtnPosFromCodeDimension_Click change to ((GuiVariables.variable["GMIX"] < Double.MaxValue) 
 * 2022-04-28 line 833 for (int iy = 0; iy < Map.SizeY; iy++)
 * 2022-11-07 line 1367 Nullable object must have a value -> check has value
 * 2022-11-15 line 1332 check if ((x < map.SizeX) && (y < map.SizeY))
 * 2022-11-29 SavePictureAsBMP: size=Map-size; importSTL; GetCoordinates round to 4 decimals
 * 2022-12-05 seperate class into new file
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace GrblPlotter
{


    public class HeightMap
    {
        public double?[,] Points { get; private set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        internal Queue<Tuple<int, int>> NotProbed { get; private set; } = new Queue<Tuple<int, int>>();

        internal Vector2 Min { get; private set; }
        internal Vector2 Max { get; private set; }

        internal Vector2 Delta { get { return Max - Min; } }

        public double MinHeight { get; set; } = double.MaxValue;
        public double MinHeight2nd { get; set; } = double.MaxValue;
        public double MaxHeight { get; set; } = double.MinValue;

        public double GridX { get { return (Max.X - Min.X) / (SizeX - 1); } }
        public double GridY { get { return (Max.Y - Min.Y) / (SizeY - 1); } }

        public string LastError { get; private set; }

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private HeightMap()
        { }
        internal HeightMap(double gridSize, Vector2 min, Vector2 max)
        {
            MinHeight = double.MaxValue;
            MinHeight2nd = double.MaxValue;
            MaxHeight = double.MinValue;
            LastError = "";

            if (min.X == max.X) { max.X = min.X + 1; }
            if (min.Y == max.Y) { max.Y = min.Y + 1; }
            //                throw new Exception("Height map can't be infinitely narrow");

            int pointsX = (int)Math.Ceiling((max.X - min.X) / gridSize) + 1;
            int pointsY = (int)Math.Ceiling((max.Y - min.Y) / gridSize) + 1;

            if (pointsX == 0) { pointsX = 1; }
            if (pointsY == 0) { pointsY = 1; }
            //        throw new Exception("Height map must have at least 4 points");

            Points = new double?[pointsX, pointsY];

            if (max.X < min.X)
            {
                double a = min.X;
                min.X = max.X;
                max.X = a;
            }

            if (max.Y < min.Y)
            {
                double a = min.Y;
                min.Y = max.Y;
                max.Y = a;
            }

            Min = min;
            Max = max;

            SizeX = pointsX;
            SizeY = pointsY;

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));

                if (++x >= SizeX)
                    break;

                for (int y = SizeY - 1; y >= 0; y--)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));
            }
            Logger.Trace("HeightMap min-x:{0} y:{1}  max-x:{2} y:{3}  SizeX:{4} Y:{5}", Min.X, Min.Y, Max.X, Max.Y, SizeX, SizeY);
        }

        public double InterpolateZ(double x, double y)	// find Z for given XY coordinate
        {
            if (x > Max.X || x < Min.X || y > Max.Y || y < Min.Y)	// out of area?
                return MaxHeight;

            x -= Min.X;		// convert coordinate to index
            y -= Min.Y;		// remove offset

            x /= GridX;		// convert coordinate to index
            y /= GridY;		// devide by resolution

            int iLX = (int)Math.Floor(x);   //lower integer part
            int iLY = (int)Math.Floor(y);

            if (iLX < 0) iLX = 0;       // check range
            if (iLY < 0) iLY = 0;

            int iHX = (int)Math.Ceiling(x); //upper integer part
            int iHY = (int)Math.Ceiling(y);

            if (iHX >= SizeX) iHX = SizeX - 1;  // check range
            if (iHY >= SizeY) iHY = SizeY - 1;

            double fX = x - iLX;             //fractional part
            double fY = y - iLY;

            double hxhy = Points[iHX, iHY].GetValueOrDefault(MaxHeight);
            double lxhy = Points[iLX, iHY].GetValueOrDefault(MaxHeight);
            double hxly = Points[iHX, iLY].GetValueOrDefault(MaxHeight);
            double lxly = Points[iLX, iLY].GetValueOrDefault(MaxHeight);
            double linUpper = hxhy * fX + lxhy * (1 - fX);
            double linLower = hxly * fX + lxly * (1 - fX);

            return linUpper * fY + linLower * (1 - fY);     //bilinear result
        }

        public double? GetPoint(int x, int y)
        {
            if ((x >= 0) && (x < SizeX) && (y >= 0) && (y < SizeY))
                return Points[x, y];
            return null;
        }

        public double? GetPoint(double x, double y)	// find Z for given XY coordinate
        {
            if (x >= SizeX || x < 0 || y >= SizeY || y < 0)	// out of area?
                return null;

            int iLX = (int)Math.Floor(x);   //lower integer part
            int iLY = (int)Math.Floor(y);

            if (iLX < 0) iLX = 0;       // check range
            if (iLY < 0) iLY = 0;

            int iHX = (int)Math.Ceiling(x); //upper integer part
            int iHY = (int)Math.Ceiling(y);

            if (iHX == iLX) iHX++;
            if (iHY == iLY) iHY++;
            if (iHX >= SizeX) iHX = SizeX - 1;  // check range
            if (iHY >= SizeY) iHY = SizeY - 1;

            double fX = x - iLX;             //fractional part
            double fY = y - iLY;

            double hxhy = Points[iHX, iHY].GetValueOrDefault(MaxHeight);
            double lxhy = Points[iLX, iHY].GetValueOrDefault(MaxHeight);
            double hxly = Points[iHX, iLY].GetValueOrDefault(MaxHeight);
            double lxly = Points[iLX, iLY].GetValueOrDefault(MaxHeight);
            double linUpper = hxhy * fX + lxhy * (1 - fX);
            double linLower = hxly * fX + lxly * (1 - fX);

            return linUpper * fY + linLower * (1 - fY);     //bilinear result
        }

        internal Vector2 GetCoordinates(int x, int y, bool applyOffset = true)
        {
            int decimals = 4;
            double rx = Math.Round(x * (Delta.X / (SizeX - 1)), decimals);
            double ry = Math.Round(y * (Delta.Y / (SizeY - 1)), decimals);

            if (applyOffset)
                return new Vector2(rx + Min.X, ry + Min.Y);
            else
                return new Vector2(rx, ry);
        }

        public void AddPoint(int x, int y, double height)
        {
            Points[x, y] = height;

            if (height > MaxHeight)
                MaxHeight = height;
            if (height < MinHeight)
            {
                MinHeight2nd = MinHeight;
                MinHeight = height;
            }
        }
        public void SetZOffset(double offset)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] + offset;
                }
            }
            MaxHeight += offset;
            MinHeight += offset;
            MinHeight2nd += offset;
        }
        public void SetZZoom(double zoom)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] * zoom;
                }
            }
            MaxHeight *= zoom;
            MinHeight *= zoom;
            MinHeight2nd *= zoom;
        }
        public void SetZInvert()
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = -Points[ix, iy];
                }
            }
            double tmp = MaxHeight;
            MaxHeight = -MinHeight;
            MinHeight = -tmp;
            MinHeight2nd = -tmp;
        }
        public void SetZCutOff(double limit)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    if (Points[ix, iy] < limit)
                        Points[ix, iy] = limit;
                }
            }
            MinHeight = limit;
            MinHeight2nd = limit;
        }

        public XmlReaderSettings settings = new XmlReaderSettings()
        { DtdProcessing = DtdProcessing.Prohibit };
        public static HeightMap Load(string path)
        {
            HeightMap map = new HeightMap();
            XmlReader content = XmlReader.Create(path);//, settings);
            map.MaxHeight = double.MinValue;
            map.MinHeight = double.MaxValue;
            map.MinHeight2nd = double.MaxValue;

            while (content.Read())
            {
                if (!content.IsStartElement())
                    continue;

                switch (content.Name)
                {
                    case "heightmap":
                        map.Min = new Vector2(double.Parse(content["MinX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo), double.Parse(content["MinY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo));
                        map.Max = new Vector2(double.Parse(content["MaxX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo), double.Parse(content["MaxY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo));
                        map.SizeX = int.Parse(content["SizeX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        map.SizeY = int.Parse(content["SizeY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        map.Points = new double?[map.SizeX, map.SizeY];
                        break;
                    case "point":
                        int x = int.Parse(content["X"].Replace(',', '.')), y = int.Parse(content["Y"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        double height = double.Parse(content.ReadInnerXml().Replace(',', '.'), NumberFormatInfo.InvariantInfo);

                        if ((x < map.SizeX) && (y < map.SizeY))
                            map.Points[x, y] = height;

                        if (height > map.MaxHeight)
                            map.MaxHeight = height;
                        if (height < map.MinHeight)
                        {
                            map.MinHeight2nd = map.MinHeight;
                            map.MinHeight = height;
                        }
                        break;
                }
            }
            for (int x = 0; x < map.SizeX; x++)
            {
                for (int y = 0; y < map.SizeY; y++)
                    if (!map.Points[x, y].HasValue)
                        map.NotProbed.Enqueue(new Tuple<int, int>(x, y));

                if (++x >= map.SizeX)
                    break;

                for (int y = map.SizeY - 1; y >= 0; y--)
                    if (!map.Points[x, y].HasValue)
                        map.NotProbed.Enqueue(new Tuple<int, int>(x, y));
            }
            //       r.Dispose();
            return map;
        }


        private static float minX = float.MaxValue;
        private static float maxX = float.MinValue;
        private static float minY = float.MaxValue;
        private static float maxY = float.MinValue;
        private static float minZ = float.MaxValue;
        private static float minZ2nd = float.MaxValue;
        private static float maxZ = float.MinValue;
        private static Dictionary<float, sbyte> amountX = new Dictionary<float, sbyte>();
        private static Dictionary<float, sbyte> amountY = new Dictionary<float, sbyte>();
        private static List<Vertex> vertices = new List<Vertex>();

        public static HeightMap ImportSTL(string fileName)
        {
            // https://github.com/Ericvf/Dotnet-STL-Files
            minX = float.MaxValue;
            maxX = float.MinValue;
            minY = float.MaxValue;
            maxY = float.MinValue;
            minZ = float.MaxValue;
            minZ2nd = float.MaxValue;
            maxZ = float.MinValue;

            amountX = new Dictionary<float, sbyte>();
            amountY = new Dictionary<float, sbyte>();
            vertices.Clear();   // = new List<Vertex>();				// clear old list

            FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);

            bool isLoaded = ReadBinary(fileStream, vertices);		// try loading binary
            if (!isLoaded)
            {
                fileStream.Position = 0;
                isLoaded = ReadAscii(fileStream);		// else ascii
            }
            fileStream.Dispose();

            if (isLoaded)
            {
                HeightMap map = new HeightMap();
                map.Min = new Vector2(minX, minY);
                map.Max = new Vector2(maxX, maxY);
                map.MinHeight = minZ; map.MaxHeight = maxZ;
                map.MinHeight2nd = minZ2nd;
                map.SizeX = amountX.Count;
                map.SizeY = amountY.Count;
                map.Points = new double?[map.SizeX + 1, map.SizeY + 1];
                map.LastError = "";

                int failCnt = 0;

                // take vertex coordinate and calculate index from it
                if ((map.SizeX > 0) && (map.SizeY > 0))
                {
                    int indexX, indexY;
                    float deltaX = (maxX - minX) / (amountX.Count - 1);
                    float deltaY = (maxY - minY) / (amountY.Count - 1);
                    Logger.Trace("ImportSTL Min X:{0}  Y:{1}  Z:{2}  Max X:{3}  Y:{4}  Z:{5}  Size X:{6}  Y:{7}  Delta X:{8}  Y:{9}  Vertices:{10}", minX, minY, minZ, maxX, maxY, maxZ, map.SizeX, map.SizeY, deltaX, deltaY, vertices.Count);
                    Logger.Trace("ImportSTL MinHeight2nd:{0}  {1}   MinHeight:{2} ", map.MinHeight2nd, minZ2nd, map.MinHeight);

                    double? oldZ;
                    foreach (Vertex tmp in vertices)
                    {
                        indexX = (int)Math.Round((tmp.X - minX) / deltaX);      // compare with InterpolateZ
                        indexY = (int)Math.Round((tmp.Y - minY) / deltaY);
                        if (indexX < 0) { Logger.Error("ImportSTL x<0 indexX:{0}  indexY:{1}  tmp.X:{2}   tmp.Y:{3}", indexX, indexY, tmp.X, tmp.Y); indexX = 0; }
                        if (indexX >= map.SizeX) { Logger.Error("ImportSTL x>  indexX:{0}  indexY:{1}  tmp.X:{2}   tmp.Y:{3}", indexX, indexY, tmp.X, tmp.Y); indexX = map.SizeX - 1; }
                        if (indexY < 0) { Logger.Error("ImportSTL y<0 indexX:{0}  indexY:{1}  tmp.X:{2}   tmp.Y:{3}", indexX, indexY, tmp.X, tmp.Y); indexY = 0; }
                        if (indexY >= map.SizeY) { Logger.Error("ImportSTL y>  indexX:{0}  indexY:{1}  tmp.X:{2}   tmp.Y:{3}", indexX, indexY, tmp.X, tmp.Y); indexY = map.SizeY - 1; }

                        oldZ = map.Points[indexX, indexY];
                        if ((oldZ == null) || (oldZ == 0))
                            map.Points[indexX, indexY] = tmp.Z;
                    }

                    for (int iy = 0; iy < map.SizeY; iy++)
                    {
                        for (int ix = 0; ix < map.SizeX; ix++)
                        {
                            if (map.Points[ix, iy] == null)
                            {
                                failCnt++;
                                map.Points[ix, iy] = InterpolatePos(map, ix, iy);
                            }
                        }
                    }
                    if (failCnt > 0)
                    {
                        map.LastError = string.Format("ImportSTL: {0} Missing values in map were interpolated", failCnt);
                        Logger.Error("ImportSTL some points:{0} are not set -> interpolate value", failCnt);
                    }
                }
                else
                {
                    Logger.Error("ImportSTL amountX or Y =0");
                    map.LastError = "ImportSTL: to less values";
                }

                return map;
            }
            return null;
        }

        private static double InterpolatePos(HeightMap map, int x, int y)
        {
            double? value;
            double sum = 0;
            int count = 0;

            for (int iy = y - 1; iy <= y + 1; iy++)
            {
                for (int ix = x - 1; ix <= x + 1; ix++)
                {
                    if ((ix >= 0) && (iy >= 0) && (ix < map.SizeX) && (iy < map.SizeY))
                    {
                        value = map.Points[ix, iy];
                        if (value != null)
                        { sum += (double)value; count++; }
                    }
                }
            }
            return sum / count;
        }

        public struct Vertex
        {
            public float X, Y, Z;
            public override string ToString() => $"X:{X}, Y:{Y}, Z:{Z}";
            //	public override int GetHashCode() => HashCode.Combine(X, Y, Z);
        }

        public struct Facet
        {
            public Vertex normal;
            public Vertex v1, v2, v3;
        };

        private static bool ReadBinary(FileStream fileStream, List<Vertex> vertices)
        {
            const int HEADER_SIZE = 84;
            const int JUNK_SIZE = 80;
            const int SIZE_OF_FACET = 50;

            var reader = new BinaryReader(fileStream);

            var fileContentSize = reader.BaseStream.Length - HEADER_SIZE;
            var fileSize = reader.BaseStream.Length;

            if (fileContentSize % SIZE_OF_FACET != 0)
            {
                return false;
            }

            for (var i = 0; i < JUNK_SIZE; i++)
            {
                fileStream.ReadByte();
            }

            var numFacets = fileContentSize / SIZE_OF_FACET;
            var headerNumFacets = reader.ReadUInt32();
            if (numFacets != headerNumFacets)
            {
                return false;
            }

            while (numFacets-- > 0)
            {
                Facet facet = default;

                facet.normal = ReadBinaryVertex(reader);
                facet.v1 = ReadBinaryVertex(reader);
                facet.v2 = ReadBinaryVertex(reader);
                facet.v3 = ReadBinaryVertex(reader);

                vertices.Add(facet.v1);
                vertices.Add(facet.v2);
                vertices.Add(facet.v3);

                reader.ReadUInt16();
            }

            return true;
        }

        private static Vertex ReadBinaryVertex(BinaryReader reader)
        {
            float val;
            Vertex vertex = default;
            val = ReadBinaryFloat(reader); vertex.X = val;
            if (val > maxX) maxX = val; if (val < minX) minX = val;
            if (!amountX.ContainsKey(val)) amountX.Add(val, 0);

            val = ReadBinaryFloat(reader); vertex.Y = val;
            if (val > maxY) maxY = val; if (val < minY) minY = val;
            if (!amountY.ContainsKey(val)) amountY.Add(val, 0);

            val = ReadBinaryFloat(reader); vertex.Z = val;
            if (val > maxZ) maxZ = val; if (val < minZ) { minZ = val; }
            if ((val < minZ2nd) && (val > minZ)) { minZ2nd = val; }

            return vertex;
        }

        private static float ReadBinaryFloat(BinaryReader reader)
        {
            StlFloat value = default;
            value.intValue = reader.ReadByte();
            value.intValue |= reader.ReadByte() << 0x08;
            value.intValue |= reader.ReadByte() << 0x10;
            value.intValue |= reader.ReadByte() << 0x18;
            return value.floatValue;
        }

        private static bool ReadAscii(FileStream fileStream)
        {
            var sr = new StreamReader(fileStream);
            sr.ReadLine();                          // 1st line "solid 

            while (!sr.EndOfStream)
            {
                Facet facet = new Facet();
                var line = sr.ReadLine();           // facet normal

                if (line.StartsWith("endsolid"))
                    break;

                if (line == "")
                    continue;

                facet.normal = ReadAsciiVertex(line, 2);	// facet normal

                line = sr.ReadLine(); 				// outer loop
                line = sr.ReadLine(); 				// vertex
                vertices.Add(ReadAsciiVertex(line));

                line = sr.ReadLine(); 				// vertex
                vertices.Add(ReadAsciiVertex(line));

                line = sr.ReadLine(); 				// vertex
                vertices.Add(ReadAsciiVertex(line));

                line = sr.ReadLine(); 				// endloop
                line = sr.ReadLine(); 				// endfacet
            }

            return true;

        }

        private static string ReadAsciiWord(StreamReader sr)
        {
            while (sr.Peek() >= 0)			// discard leading space
            {
                var c = (char)sr.Peek();
                if (!isWhiteSpace(c))		// stop on any char
                    break;
                sr.Read();
            }

            var word = string.Empty;

            while (!sr.EndOfStream)			// collect chars
            {
                var c = (char)sr.Read();
                if (isWhiteSpace(c))
                    break;
                word += c;
            }

            return word;

            bool isWhiteSpace(char c) => c.Equals(' ') || c.Equals('\t') || c.Equals('\n') || c.Equals('\r');
        }

        private static Vertex ReadAsciiVertex(string line, int skip = 1)
        {
            var lineReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(line)));
            bool skipFacetNormal = false;
            if (skip == 2) skipFacetNormal = true;

            string txt;
            while (skip-- > 0)
            {
                ReadAsciiWord(lineReader);		// read "vertex" or "facet" & "normal"
            }

            if (skipFacetNormal)  // don't process "facet normal 0 0 0"
            {
                txt = ReadAsciiWord(lineReader);//.Replace(',', '.');
                txt = ReadAsciiWord(lineReader);//.Replace(',', '.');
                txt = ReadAsciiWord(lineReader);//.Replace(',', '.');
                return new Vertex();
            }

            float val, x, y, z;
            txt = ReadAsciiWord(lineReader).Replace(',', '.');
            val = float.Parse(txt, NumberFormatInfo.InvariantInfo);
            x = val;
            if (val > maxX) maxX = val; if (val < minX) minX = val;
            if (!amountX.ContainsKey(val)) amountX.Add(val, 0);

            txt = ReadAsciiWord(lineReader).Replace(',', '.');
            val = float.Parse(txt, NumberFormatInfo.InvariantInfo);
            y = val;
            if (val > maxY) maxY = val; if (val < minY) minY = val;
            if (!amountY.ContainsKey(val)) amountY.Add(val, 0);

            txt = ReadAsciiWord(lineReader).Replace(',', '.');
            val = float.Parse(txt, NumberFormatInfo.InvariantInfo);
            z = val;
            if (val > maxZ) maxZ = val; if (val < minZ) { minZ = val; }
            if ((val < minZ2nd) && (val > minZ)) { minZ2nd = val; }

            return new Vertex()
            {
                X = x,
                Y = y,
                Z = z,
            };
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct StlFloat
        {
            [FieldOffset(0)] public int intValue;
            [FieldOffset(0)] public float floatValue;
        }


        private static readonly string formatNumber = "0.000";
        private static string FrmtNum(double number)     // convert float to string using format pattern
        { return number.ToString(formatNumber); }

        // vertex coordinates must be positive-definite (nonnegative and nonzero) numbers. 
        // The StL file does not contain any scale information; the coordinates are in arbitrary units.
        public void SaveSTL(string path)
        {
            // check via https://www.viewstl.com/
            StringBuilder data = new StringBuilder();
            data.AppendLine("solid ASCII_STL_GRBL_Plotter");
            double z0, z1, z2, z3;
            double sign = 1;
            Vector2 p0, p1, p2, p3;
            for (int y = 0; y < (SizeY - 1); y++)
            {
                for (int x = 0; x < (SizeX - 1); x++)
                {
                    if (!Points[x, y].HasValue)
                        continue;
                    if (!Points[x, y + 1].HasValue)
                        continue;
                    if (!Points[x + 1, y].HasValue)
                        continue;
                    if (!Points[x + 1, y + 1].HasValue)
                        continue;
                    p0 = GetCoordinates(x, y, false);       	// get real coordinates from index 
                    p1 = GetCoordinates(x, y + 1, false);
                    p2 = GetCoordinates(x + 1, y, false);
                    p3 = GetCoordinates(x + 1, y + 1, false);
                    z0 = sign * Points[x, y].Value;    			// get height at index-pos
                    z1 = sign * Points[x, y + 1].Value;
                    z2 = sign * Points[x + 1, y].Value;
                    z3 = sign * Points[x + 1, y + 1].Value;

                    data.AppendLine(" facet normal 0 0 0");
                    data.AppendLine("  outer loop");
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p0.X, p0.Y, z0);	// p1  p3	clockwise order
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p1.X, p1.Y, z1);	//
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p2.X, p2.Y, z2);	// p0  p2
                    data.AppendLine("  endloop");
                    data.AppendLine(" endfacet");

                    data.AppendLine(" facet normal 0 0 0");
                    data.AppendLine("  outer loop");
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p1.X, p1.Y, z1);	// p1  p3	clockwise order
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p3.X, p3.Y, z3);	//
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p2.X, p2.Y, z2);	// p0  p2
                    data.AppendLine("  endloop");
                    data.AppendLine(" endfacet");
                }
            }
            data.AppendLine("endsolid ASCII_STL_GRBL_Plotter");
            File.WriteAllText(path, data.ToString().Replace(',', '.'));
            data.Clear();
        }

        // https://www.fileformat.info/format/wavefrontobj/egff.htm
        // https://people.sc.fsu.edu/~jburkardt/data/obj/obj.html
        // https://en.wikipedia.org/wiki/Wavefront_.obj_file
        public void SaveOBJ(string path)
        {
            StringBuilder data = new StringBuilder();
            StringBuilder data_point = new StringBuilder();
            StringBuilder data_face = new StringBuilder();
            Dictionary<int, StringBuilder> data_face_color = new Dictionary<int, StringBuilder>();

            OBJdata_color = new StringBuilder();
            OBJused_color = new Dictionary<Color, int>();   // to check if color is already indexed
            OBJcolor_used = 0;

            string mtl_path = Path.ChangeExtension(path, ".mtl");
            data.AppendLine("#\r\n# GRBL_Plotter Height map");
            data.AppendFormat("# SizeX:{0}  SizeY:{1}\r\n", SizeX, SizeY);
            data.AppendFormat("#\r\n\r\nmtllib {0}\r\n\r\n", Path.GetFileName(mtl_path));
            //          data.AppendLine("o HeightMap");
            //          data.AppendLine("g map1");
            double z0, z1;
            Vector2 p0;
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)   // -1 2023-08-01
                {
                    if (!Points[x, y].HasValue)
                        continue;
                    p0 = GetCoordinates(x, y, false);
                    z0 = Points[x, y].Value;
                    data_point.AppendFormat("v {0} {1} {2:0.0000}\r\n", p0.X, p0.Y, z0);

                    if ((y < (SizeY - 1)) && (x < (SizeX - 1)))
                    {
                        int color_index = CheckOBJColorUse(z0);
                        if (data_face_color.ContainsKey(color_index))
                        {
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * y + x + 1, SizeX * y + x + 2, SizeX * (y + 1) + x + 1);		// 1. triangle
                        }
                        else
                        {
                            data_face_color.Add(color_index, new StringBuilder());
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * y + x + 1, SizeX * y + x + 2, SizeX * (y + 1) + x + 1);		// 1. triangle
                        }

                        z1 = Points[x + 1, y].Value;

                        color_index = CheckOBJColorUse(z1);
                        if (data_face_color.ContainsKey(color_index))
                        {
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * (y + 1) + x + 1, SizeX * (y + 1) + x + 2, SizeX * y + x + 2);       // 2. triangle																		
                        }    // 2. triangle																		
                        else
                        {
                            data_face_color.Add(color_index, new StringBuilder());
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * (y + 1) + x + 1, SizeX * (y + 1) + x + 2, SizeX * y + x + 2);       // 2. triangle																		
                        }
                    }
                }
            }
            data_point.AppendLine("o HeightMap");
            data_point.AppendLine("g map1");

            data.Append(data_point.ToString());
            //data.Append(data_face.ToString());
            for (int i = 0; i < OBJcolor_used; i++)
            {
                if (data_face_color.ContainsKey(i))
                {
                    data_face.AppendFormat("usemtl c_{0}\r\n", i);
                    data_face.Append(data_face_color[i].ToString());
                }
            }
            data.Append(data_face.ToString());

            File.WriteAllText(path, data.ToString().Replace(',', '.'));	// save obj file
            File.WriteAllText(mtl_path, OBJdata_color.ToString().Replace(',', '.'));    // save mtl file		
            OBJdata_color.Clear();
            OBJused_color.Clear();
            data.Clear();
            data_point.Clear();
            data_face.Clear();
        }

        private static Dictionary<Color, int> OBJused_color = new Dictionary<Color, int>(); // to check if color is already indexed
        private static int OBJcolor_used = 0;
        private static StringBuilder OBJdata_color = new StringBuilder();

        private int CheckOBJColorUse(double zVal)
        {
            Color tmpColor = GetColor(MinHeight, MaxHeight, zVal, false);
            if (OBJused_color.ContainsKey(tmpColor))
            { return OBJused_color[tmpColor]; }
            else
            {
                OBJused_color.Add(tmpColor, OBJcolor_used);
                //			data_face.AppendFormat("usemtl c_{0}",OBJcolor_used);}
                // create mtl file
                OBJdata_color.AppendFormat("newmtl c_{0}\r\n", OBJcolor_used);
                OBJdata_color.AppendFormat("Kd {0:0.0000} {1:0.0000} {2:0.0000}\r\n", (double)tmpColor.R / 255, (double)tmpColor.G / 255, (double)tmpColor.B / 255);
                OBJdata_color.AppendFormat("Ks {0:0.0000} {1:0.0000} {2:0.0000}\r\n", (double)tmpColor.R / 255, (double)tmpColor.G / 255, (double)tmpColor.B / 255);
                OBJdata_color.AppendFormat("Ka {0:0.0000} {1:0.0000} {2:0.0000}\r\n", (double)tmpColor.R / 255, (double)tmpColor.G / 255, (double)tmpColor.B / 255);
                //         OBJdata_color.AppendFormat("Ks 0.8 0.8 0.8\r\n");
                //		OBJdata_color.AppendFormat("Ka 0.2 0.2 0.2\r\n");
                OBJdata_color.AppendFormat("illum 2\r\n");
                OBJdata_color.AppendFormat("Ns 40\r\n\r\n");

                OBJcolor_used++;
                return OBJcolor_used - 1;
            }
        }

        public void SaveX3D(string path)
        {
            StringBuilder object_code = new StringBuilder();
            StringBuilder color_code = new StringBuilder();
            bool first_val = true;
            if (true)//elevation)
            {
                object_code.AppendLine(" <Transform DEF='elevationgrid' containerField='children' translation='0 0 0'>");
                object_code.AppendLine("  <Shape DEF='GRBL-Plotter Height Map' containerField='children'>");
                object_code.AppendFormat("    <ElevationGrid creaseAngle='3.14159' solid='false' xDimension='{0}' xSpacing='1' zDimension='{1}' zSpacing='1' height='", SizeX, SizeY);
                for (int y = (SizeY - 1); y >= 0; y--) //(int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        if (first_val) { first_val = false; }
                        else { object_code.Append(","); color_code.Append(","); }
                        if (x == 0) { object_code.Append("\r\n      "); color_code.Append("\r\n         "); }
                        object_code.Append(FrmtNum(Points[x, y].Value).Replace(',', '.'));
                        color_code.Append(GetColorString(Points[x, y].Value).Replace(',', '.'));
                    }
                }
                object_code.Append("'>\r\n");
                object_code.AppendFormat("        <Color color='{0}'/>\r\n", color_code);
                object_code.Append("     </ElevationGrid>\r\n");
                object_code.Append("  </Shape>\r\n");
                object_code.Append(" </Transform>\r\n");
            }
            string file_head = "", file_foot = "";
            file_head += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
            file_head += "<!DOCTYPE X3D PUBLIC \"ISO//Web3D//DTD X3D 3.0//EN\" \"http://www.web3d.org/specifications/x3d-3.0.dtd\">\r\n";
            file_head += "<X3D profile='Immersive' >\r\n";
            file_head += "<head></head>\r\n";
            file_head += "<Scene>\r\n";

            file_head += "<NavigationInfo containerField='children' avatarSize='.25 1.6 .75' visibilityLimit='0' speed='10' headlight='true' type='\"EXAMINE\" \"ANY\"'/>\r\n";
            file_head += "<Background containerField='children' skyAngle=' .7854 1.91986' skyColor='  0 .2 .70196 0 .50196 1 1 1 1' groundAngle='1.5708' groundColor='  .2 .2 .2 .8 .8 .8'/>\r\n";

            file_head += "<Transform DEF='dad_Group_light' rotation='-.286 -.914 -.286 1.66'>\r\n";
            file_head += "  <Transform DEF='light1_t' containerField='children' translation='" + SizeY / 2 + " 0 " + 3 * SizeX + "' scale='2 2 2'>\r\n";
            file_head += "    <SpotLight DEF='light1' containerField='children' ambientIntensity='0.000' intensity='1.000' radius='100.000' cutOffAngle='1.309' beamWidth='0.785' attenuation='1 0 0' color='1 1 1' on='true'/>\r\n";
            file_head += "  </Transform>\r\n";
            file_head += "</Transform>\r\n";

            // camera static
            var camera_static_distance = SizeX * 2;
            var camera_static_angle = 45 * Math.PI / 180;
            file_head += "<Transform DEF='dad_Group_static_camera' translation='" + SizeX / 2 + " 0 " + SizeY / 2 + "' rotation='-1 0 0 " + camera_static_angle + "'>\r\n";
            file_head += " <Viewpoint DEF='Viewpoint_static_camera' containerField='children' description='Static camera' jump='true' fieldOfView='0.785' position='0 0 " + camera_static_distance + "' orientation='0 0 1 0'/>\r\n";
            file_head += "</Transform>\r\n";

            //            file_head += navi + back + light + camera + plate + text + legend;
            file_foot += "</Scene>\r\n</X3D>\r\n";
            string file_data = file_head.Replace(',', '.') + object_code.ToString() + file_foot;
            File.WriteAllText(path, file_data);     // .Replace(',', '.')
            object_code.Clear();
            color_code.Clear();
        }

        public void Save(string path)
        {
            XmlWriterSettings set = new XmlWriterSettings
            {
                Indent = true
            };
            XmlWriter w = XmlWriter.Create(path, set);
            w.WriteStartDocument();
            w.WriteStartElement("heightmap");
            w.WriteAttributeString("MinX", Min.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("MinY", Min.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("MaxX", Max.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("MaxY", Max.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("SizeX", SizeX.ToString().Replace(',', '.'));
            w.WriteAttributeString("SizeY", SizeY.ToString().Replace(',', '.'));

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    if (!Points[x, y].HasValue)
                        continue;

                    w.WriteStartElement("point");
                    w.WriteAttributeString("X", x.ToString().Replace(',', '.'));
                    w.WriteAttributeString("Y", y.ToString().Replace(',', '.'));
                    w.WriteString(FrmtNum(Points[x, y].Value).Replace(',', '.'));
                    w.WriteEndElement();
                }
            }
            w.WriteEndElement();
            w.Close();
        }

        public static Color GetColor(double min, double max, double value, bool gray)
        {
            int R = 0, G = 0, B = 0;
            if (gray)
            {
                int valC = (int)(255 * (value - min) / (max - min));
                if (valC < 0) valC = 0;
                if (valC > 255) valC = 255;
                R = G = B = valC;
            }
            else
            {
                int segments = 3;
                int valC = (int)(255 * segments * (value - min) / (max - min));
                if (valC < 0) valC = 0;
                if (valC > 255 * segments) valC = 255 * segments;

                if ((valC >= 0) && (valC < 256 * 1))
                { R = 0; G = valC; B = 255 - valC; }

                else if ((valC >= 256) && (valC < 256 * 2))
                { R = valC - (256 * 1); G = 255; B = 0; }

                else if ((valC >= 256 * 2) && (valC < 256 * 3))
                { R = 255; G = (256 * 3 - 1) - valC; B = 0; }
            }
            return Color.FromArgb(R, G, B);
        }
        public String GetColorString(double value)
        {
            Color tmp = GetColor(MinHeight, MaxHeight, value, false);
            return string.Format("{0:0.00} {1:0.00} {2:0.00}", (double)tmp.R / 255, (double)tmp.G / 255, (double)tmp.B / 255);
        }

        internal void FillWithTestPattern(string pattern)
        {
            DataTable t = new DataTable();

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    double X = (x * (Max.X - Min.X)) / (SizeX - 1) + Min.X;
                    double Y = (y * (Max.Y - Min.Y)) / (SizeY - 1) + Min.Y;

                    decimal d = (decimal)t.Compute(pattern.Replace("x", X.ToString()).Replace("y", Y.ToString()), "");
                    AddPoint(x, y, (double)d);
                }
            }
            t.Dispose();
        }
    }

    public struct Vector2 : IEquatable<Vector2>
    {

        private double x;

        private double y;

        public Vector2(double x, double y)
        {
            // Pre-initialisation initialisation
            // Implemented because a struct's variables always have to be set in the constructor before moving control
            this.x = 0;
            this.y = 0;

            // Initialisation
            X = x;
            Y = y;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        { return Add(v1, v2); }
        public static Vector2 Add(Vector2 v1, Vector2 v2)
        {
            return
            (
                new Vector2
                    (
                        v1.X + v2.X,
                        v1.Y + v2.Y
                    )
            );
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        { return Subtract(v1, v2); }
        public static Vector2 Subtract(Vector2 v1, Vector2 v2)
        {
            return
            (
                new Vector2
                    (
                        v1.X - v2.X,
                        v1.Y - v2.Y
                    )
            );
        }

        public static Vector2 operator *(Vector2 v1, double s2)
        { return Multiply(v1, s2); }
        public static Vector2 Multiply(Vector2 v1, double s2)
        {
            return
           (
                new Vector2
                (
                    v1.X * s2,
                    v1.Y * s2
                )
            );
        }

        public static Vector2 operator *(double s1, Vector2 v2)
        { return Multiply(s1, v2); }
        public static Vector2 Multiply(double s1, Vector2 v2)
        {
            return v2 * s1;
        }

        public static Vector2 operator /(Vector2 v1, double s2)
        { return Divide(v1, s2); }
        public static Vector2 Divide(Vector2 v1, double s2)
        {
            return
            (
                new Vector2
                    (
                        v1.X / s2,
                        v1.Y / s2
                    )
            );
        }

        public static Vector2 operator -(Vector2 v1)
        {
            return Negate(v1);
        }
        public static Vector2 Negate(Vector2 v1)
        {
            return
            (
                new Vector2
                    (
                        -v1.X,
                        -v1.Y
                    )
            );
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return
            (
                Math.Abs(v1.X - v2.X) <= EqualityTolerence &&
                Math.Abs(v1.Y - v2.Y) <= EqualityTolerence
            );
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return !(v1 == v2);
        }

        public bool Equals(Vector2 other)
        {
            return other == this;
        }

        public override bool Equals(object other)
        {
            // Convert object to Vector3
            // Check object other is a Vector3 object
            if (other is Vector2 otherVector)
            {
                // Check for equality
                return otherVector == this;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return
            (
                (int)((X + Y) % Int32.MaxValue)
            );
        }

        public const double EqualityTolerence = double.Epsilon;
    }


}