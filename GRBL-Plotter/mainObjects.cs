using System;
using System.Drawing;

namespace GRBL_Plotter
{
    public struct xyzPoint
    {
        public double X, Y, Z, A;
        public xyzPoint(double x, double y, double z, double a = 0)
        { X = x; Y = y; Z = z; A = a; }
        // Overload + operator 
        public static xyzPoint operator +(xyzPoint b, xyzPoint c)
        {
            xyzPoint a = new xyzPoint();
            a.X = b.X + c.X;
            a.Y = b.Y + c.Y;
            a.Z = b.Z + c.Z;
            a.A = b.A + c.A;
            return a;
        }
        public static xyzPoint operator -(xyzPoint b, xyzPoint c)
        {
            xyzPoint a = new xyzPoint();
            a.X = b.X - c.X;
            a.Y = b.Y - c.Y;
            a.Z = b.Z - c.Z;
            a.A = b.A - c.A;
            return a;
        }
        public string Print()
        {
            bool ctrl4thUse = Properties.Settings.Default.ctrl4thUse;
            string ctrl4thName = Properties.Settings.Default.ctrl4thName;

            if (ctrl4thUse)
                return string.Format("X={0:0.000} Y={1:0.000} Z={2:0.000} {3}={4:0.000}", X, Y, Z, ctrl4thName, A);
            else
                return string.Format("X={0:0.000} Y={1:0.000} Z={2:0.000}", X, Y, Z);
        }

    };
    public struct xyPoint
    {
        public double X, Y;
        public xyPoint(double x, double y)
        { X = x; Y = y; }
        public xyPoint(xyPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(Point tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(xyzPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public static explicit operator xyPoint(Point tmp)
        { return new xyPoint(tmp); }
        public static explicit operator xyPoint(xyzPoint tmp)
        { return new xyPoint(tmp); }

        public Point ToPoint()
        { return new Point((int)X, (int)Y); }

        public double DistanceTo(xyPoint anotherPoint)
        {
            double distanceCodeX = X - anotherPoint.X;
            double distanceCodeY = Y - anotherPoint.Y;
            return Math.Sqrt(distanceCodeX * distanceCodeX + distanceCodeY * distanceCodeY);
        }
        public double AngleTo(xyPoint anotherPoint)
        {
            double distanceX = anotherPoint.X - X;
            double distanceY = anotherPoint.Y - Y;
            double radius = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            if (radius == 0) { return 0; }
            double cosinus = distanceX / radius;
            if (cosinus > 1) { cosinus = 1; }
            if (cosinus < -1) { cosinus = -1; }
            double angle = 180 * (float)(Math.Acos(cosinus) / Math.PI);
            if (distanceY > 0) { angle = -angle; }
            return angle;
        }

        // Overload + operator 
        public static xyPoint operator +(xyPoint b, xyPoint c)
        {   xyPoint a = new xyPoint();
            a.X = b.X + c.X;
            a.Y = b.Y + c.Y;
            return a;
        }
        // Overload / operator 
        public static xyPoint operator /(xyPoint b, double c)
        {
            xyPoint a = new xyPoint();
            a.X = b.X / c;
            a.Y = b.Y / c;
            return a;
        }
    };


    /*   class eventArgsTemplates    // just copy and paste 
       {
           public event EventHandler<XYZEventArgs> RaiseXYZEvent;
           protected virtual void OnRaiseXYZEvent(XYZEventArgs e)
           {
               EventHandler<XYZEventArgs> handler = RaiseXYZEvent;
               if (handler != null)
               {
                   handler(this, e);
               }
           }
       }
       */

    public class XYEventArgs : EventArgs
    {
        private double angle,scale;
        private xyPoint point;
        string command;
        public XYEventArgs(double a, double s, xyPoint p, string cmd)
        {
            angle = a;
            scale = s;
            point = p;
            command = cmd;
        }
        public XYEventArgs(double a, double x, double y, string cmd)
        {
            angle = a;
            point.X = x;
            point.Y = y;
            command = cmd;
        }
        public double Angle
        { get { return angle; } }
        public double Scale
        { get { return scale; } }
        public xyPoint Point
        { get { return point; } }
        public double PosX
        { get { return point.X; } }
        public double PosY
        { get { return point.Y; } }
        public string Command
        { get { return command; } }
    }

    public class XYZEventArgs : EventArgs
    {
        private double? posX, posY, posZ;
        string command;
        public XYZEventArgs(double? x, double? y, string cmd)
        {
            posX = x;
            posY = y;
            posZ = null;
            command = cmd;
        }
        public XYZEventArgs(double? x, double? y, double? z, string cmd)
        {
            posX = x;
            posY = y;
            posZ = z;
            command = cmd;
        }
        public double? PosX
        {   get { return posX; } }
        public double? PosY
        {   get { return posY; } }
        public double? PosZ
        { get { return posZ; } }
        public string Command
        {   get { return command; } }
    }

}
