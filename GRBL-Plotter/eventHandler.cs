using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRBL_Plotter
{
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
