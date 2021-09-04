using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrblPlotter.GUI
{
    public partial class MainFormStreamSection : Form
    {
        private int lineStart = 0;
        private int lineEnd = 10000;
        private int lineMax = 10000;
        private string parser = "";
        private XyzPoint position;
        public int LineStart
        {            
            get { lineStart = (int)NudStreamStart.Value; return lineStart; }
            set
            {
                lineStart = value;
                if (NudStreamStart.Maximum < lineStart)
                    NudStreamStart.Maximum = lineStart;
                NudStreamStart.Value = lineStart;
            }
        }
        public int LineEnd
        {
            get { lineEnd = (int)NudStreamStop.Value; return lineEnd; }
            set
            {
                lineEnd = value;
                if (NudStreamStop.Maximum < lineEnd)
                    NudStreamStop.Maximum = lineEnd;
                NudStreamStop.Value = lineEnd;
            }
        }
        public int LineMax
        {
            set
            {
                lineMax = value;
                NudStreamStart.Maximum = lineMax;
                NudStreamStop.Maximum = lineMax;
            }
        }

        public string Parser
        {
            set
            {
                parser = value;
                textBox1.Text = parser;
            }
        }
        internal XyzPoint Position
        {
            set
            {
                position = value;
                lblPosition.Text = string.Format("X:{0:0.000}  Y:{1:0.000}  Z:{2:0.000}", position.X, position.Y, position.Z);                  
            }
        }

        public MainFormStreamSection()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;  // set icon
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {

        }

        private void NudStreamStart_ValueChanged(object sender, EventArgs e)
        {
            Parser = VisuGCode.GetParserState((int)NudStreamStart.Value - 1);
            Position = VisuGCode.GetActualPosition((int)NudStreamStart.Value - 1);
        }

        private void BtnSetMin_Click(object sender, EventArgs e)
        {
            NudStreamStart.Value = 1;
        }

        private void BtnSetMax_Click(object sender, EventArgs e)
        {
            NudStreamStop.Value = lineMax;
        }
    }
}
