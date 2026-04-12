/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
namespace GrblPlotter.UserControls
{
    public partial class UCJogControlUD : UserControl
    {
        private readonly Image arrow_r = Properties.Resources.a_r;		// arrow to the right
        private readonly Image arrow_d = Properties.Resources.a_d;      // arrow to upper-right
        private readonly Color ColorBtnDisabled = SystemColors.Control;
        private readonly Color ColorBtnHighlight = Color.Yellow;
        private Color ColorJogHighlight = Color.FromArgb(60, Color.Yellow);
        private System.Drawing.Bitmap jogBackground = new System.Drawing.Bitmap(10, 10);
        private Color ColorBtnNormal = Color.Gray;
        private bool jogStart = false;
        private readonly int BtnPos;
        private int BtnIndex = 5;
        private bool showStop = true;   // show stop button
        private bool jogEnabled = false;

        private bool useClassicButtons = true;
        //    private int jogRadius;
        private int jogRaster = 6;
        private readonly int jogRasterMark = 0;
        private int jogSegment = 0, jogIndex = 0;

        private string jogCommand = "$J=G91";
        private double stepWidth = 5;
        private int feed = 1000;
        private List<DistFeed> jogDF = new List<DistFeed>();
        private string axisName = "Z";

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        System.Windows.Forms.Timer jogTimer = new System.Windows.Forms.Timer();

        public UCJogControlUD()
        {
            InitializeComponent();
            jogDF.Add(new DistFeed(1, 1));
            jogDF.Add(new DistFeed(0.1f, 12));
            jogDF.Add(new DistFeed(0.5f, 60));
            jogDF.Add(new DistFeed(1f, 120));
            jogDF.Add(new DistFeed(5f, 600));
            jogDF.Add(new DistFeed(10f, 1200));
            jogDF.Add(new DistFeed(50f, 6000));

            this.jogTimer.Interval = 500;
            this.jogTimer.Tick += new System.EventHandler(this.JogTimer_Tick);
        }
        private void UserControlJogControlUD_Load(object sender, EventArgs e)
        {
            SetToolStripText();
            SetToolTipButtons();
            SetEnabled(jogEnabled);
            Tsmi5.Checked = true;   // BtnIndex
            PanelJoyStick.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(PanelJoyStick, new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true });
        }

        internal void SetDistFeed(int index, float dist, int feed)
        {
            if ((index > 0) && (index < jogDF.Count))
            {
                jogDF[index] = new DistFeed(dist, feed);
                SetToolStripText();
            }
        }
        internal void SetCommandProtocol(CommandProtocol val)
        {
            if (val == CommandProtocol.grblCanJog)
            {
                jogCommand = "$J=G91";
                showStop = true;
            }
            else
            {
                jogCommand = "G91G1";
                showStop = false;
            }
            BtnStop.Visible = showStop;
        }

        public void SetAxisName(string n)
        { axisName = n; }

        internal void TriggerMove(string action)
        {
            if (!useClassicButtons)
            {
                float d = jogDF[jogIndex].Dist;
                int feed = jogDF[jogIndex].Feed;
                if (action.Contains("Dec")) { stepWidth = -d; }
                if (action.Contains("Inc")) { stepWidth = d; }
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}{1}{2:0.0}F{3}", jogCommand, axisName, stepWidth, feed).Replace(",", "."), 0, null, null));
            }
            else
            {
                string sign = "";
                if (action.Contains("Dec")) { sign = "-"; }
                if (action.Contains("Inc")) { sign = ""; }
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}{1}{2}{3:0.0}F{4}", jogCommand, axisName, sign, stepWidth, feed).Replace(",", "."), 0, null, null));
            }
        }

        private void VirtualJoystick_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!useClassicButtons)
                {
                    CalculateJogPosition(e.X, e.Y);
                    SendPositions(jogSegment, jogIndex);
                    if (jogEnabled)
                        ColorJogHighlight = Color.Yellow;
                    else
                        ColorJogHighlight = Color.LightGray;

                    if (Properties.Settings.Default.UserControlJogControlShowLabel)
                    {
                        LblJogProp.Visible = true;
                        LblJogProp.Location = LabelLocation(e.Location);
                        if (jogIndex < 1)
                            LblJogProp.Text = string.Format("Stop");
                        if (jogIndex < jogRaster)
                            LblJogProp.Text = string.Format("D:{0}\nF:{1}", jogDF[jogIndex].Dist, jogDF[jogIndex].Feed);
                    }
                }
                else
                { FindButtonSetStep(sender); }
            }
        }
        private void VirtualJoystick_MouseUp(object sender, MouseEventArgs e)
        {
            JogDisable();
            LblJogProp.Visible = false;
            if (jogEnabled)
                ColorJogHighlight = Color.Yellow;// GroupColor.FromArgb(60, GroupColor.Yellow);
            else
                ColorJogHighlight = Color.LightGray;
        }
        private void VirtualJoystick_MouseMove(object sender, MouseEventArgs e)
        {
            CalculateJogPosition(e.X, e.Y);
            if (Properties.Settings.Default.UserControlJogControlShowLabel)
            {
                LblJogProp.Location = LabelLocation(e.Location);
                if (jogIndex < 1)
                    LblJogProp.Text = string.Format("Stop");
                if (jogIndex < jogRaster)
                    LblJogProp.Text = string.Format("D:{0}\nF:{1}", jogDF[jogIndex].Dist, jogDF[jogIndex].Feed);
            }
            Refresh();
        }
        private void PanelJoyStick_MouseLeave(object sender, EventArgs e)
        {
            jogSegment = 0; jogIndex = 0;
            LblJogProp.Visible = false;
            Refresh();
        }
        private Point LabelLocation(Point p)
        {
            int offy = (p.Y > Height / 2) ? -20 : 20;
            int offx = (p.X > Width / 2) ? -50 : 10;
            return new Point(1, p.Y + offy);
        }

        private void CalculateJogPosition(int X, int Y)
        {
            double dx = X - Width / 2;
            double dy = Y - Height / 2;
            double r = Math.Sqrt(dx * dx + dy * dy);
            int index;
            double a = Math.Atan2(dy, dx) * 180 / Math.PI;
            double range;
            for (int s = -4; s < 4; s++)
            {
                range = Math.Abs(s * 45 - a);
                if (range < 20)
                {
                    index = (int)(r / (Height / 2) * jogRaster);
                    jogSegment = s; jogIndex = index;
                    break;
                }
            }
        }

        private void SendPositions(int s, int r)
        {
            if (r < 1) { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x85, null, null)); }
            else
            {
                if (r > jogDF.Count)
                { r = jogDF.Count - 1; }
                if (r < 1)
                { r = 1; }
                stepWidth = jogDF[r].Dist;
                feed = jogDF[r].Feed;
                if (s > 0)
                    stepWidth *= -1;

                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}{1}{2:0.0}F{3}", jogCommand, axisName, stepWidth, feed).Replace(",", "."), 0, null, null));
            }
        }

        private void Btn_MouseHover(object sender, EventArgs e)
        {
            HighlightButton(sender, true);
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            HighlightButton(sender, false);
        }

        private void FindButtonSetStep(object sender)
        {
            bool btnPressed = false;
            string sign = "";
            if (sender == BtnUp) { sign = ""; btnPressed = true; }     // OnJogTimer(new JogEventArgs(-BtnIndex, BtnIndex));
            else if (sender == BtnDown) { sign = "-"; btnPressed = true; }     //OnJogTimer(new JogEventArgs(0, BtnIndex));
            else if (sender == BtnStop) { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x85, sender, null)); }

            if (btnPressed)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}{1}{2}{3:0.0}F{4}", jogCommand, axisName, sign, stepWidth, feed).Replace(",", "."), 0, sender, null));
        }


        protected virtual void JogTimer_Tick(object sender, EventArgs e)
        {
            if (jogStart)
            {
                if (jogStart)
                {
                    if (!useClassicButtons)
                    {
                        SendPositions(jogSegment, jogIndex);
                    }
                    else
                    {
                        OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}{1}{2:0.0}F{3}", jogCommand, axisName, stepWidth, feed).Replace(",", "."), 0, sender, null));
                    }
                }
            }
        }


        private void JogDisable()
        {
            jogStart = false;
            jogTimer.Stop();
            this.Refresh();
            if (Properties.Settings.Default.ctrlSendStopJog)
            { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x85, null, null)); }
        }

        private void SetButtonImages(bool enabled)
        {
            Image tmp_r = (Image)arrow_r.Clone();
            Image tmp_d = (Image)arrow_d.Clone();
            if (!enabled)
            {
                tmp_r = MyControl.BitmapSetToGray(tmp_r);
                tmp_d = MyControl.BitmapSetToGray(tmp_d);
            }

            BtnUp.BackgroundImage = (Image)tmp_r.Clone(); BtnUp.BackgroundImage.RotateFlip((RotateFlipType.Rotate270FlipNone));
            BtnDown.BackgroundImage = (Image)tmp_r.Clone(); BtnDown.BackgroundImage.RotateFlip((RotateFlipType.Rotate90FlipNone));

            tmp_r = Properties.Resources.stop;
            if (!enabled)
                BtnStop.BackgroundImage = MyControl.BitmapSetToGray(tmp_r);
            else
                BtnStop.BackgroundImage = tmp_r;


            Font stringFont = new Font("Microsoft Sans Serif", 40, GraphicsUnit.Pixel);
            using (Graphics grp = Graphics.FromImage(BtnUp.BackgroundImage))
            {
                grp.DrawString(axisName, stringFont, Brushes.Black, -5, -5);
            }
        }

        private void HighlightButton(object sender, bool highlight)
        {
            if (highlight)
            {
                ColorBtnNormal = ((Button)sender).BackColor;
                ((Button)sender).BackColor = ColorBtnHighlight;
            }
            else
            {
                if (Properties.Settings.Default.guiColorSchemeEnable)
                    ((Button)sender).BackColor = MyControl.ButtonBackColor;
                else
                    ((Button)sender).UseVisualStyleBackColor = true;
            }
        }

        internal void SetEnabled(bool enable)
        {
            SetButtonImages(enable);
            AlignButtons();
            CreateBackgroundPicture();
            if (enable)
            {
                PanelJoyStick.BackgroundImage = jogBackground;
                ColorJogHighlight = Color.Yellow;// GroupColor.FromArgb(60, GroupColor.Yellow);
            }
            else
            {
                PanelJoyStick.BackgroundImage = MyControl.BitmapSetToGray(jogBackground);
                ColorJogHighlight = Color.LightGray;
            }

            jogEnabled = enable;
            Invalidate();
        }

        private void SetToolTipButtons()
        {
            string s1 = string.Format("{0} {1:0.00}; F{2}", axisName, stepWidth, feed);
            string s2 = string.Format("{0} -{1:0.00}; F{2}", axisName, stepWidth, feed);

            toolTip1.SetToolTip(BtnUp, s1);
            toolTip1.SetToolTip(BtnDown, s2);
            toolTip1.SetToolTip(BtnStop, "Stop");
        }

        private void SetToolStripText()
        {
            Tsmi1.Text = string.Format("{0,3:###.##}", jogDF[1].Dist);
            Tsmi2.Text = string.Format("{0,3:###.##}", jogDF[2].Dist);
            Tsmi3.Text = string.Format("{0,3:###.##}", jogDF[3].Dist);
            Tsmi4.Text = string.Format("{0,3:###.##}", jogDF[4].Dist);
            Tsmi5.Text = string.Format("{0,3:###.##}", jogDF[5].Dist);
        }
        private void ContextMenuStrip_Click(object sender, EventArgs e)
        {

            Tsmi1.Checked = Tsmi2.Checked = Tsmi3.Checked = Tsmi4.Checked = Tsmi5.Checked = false;
            var tag = ((ToolStripMenuItem)sender);
            if (tag == Tsmi1) { BtnIndex = 1; Tsmi1.Checked = true; }
            else if (tag == Tsmi2) { BtnIndex = 2; Tsmi2.Checked = true; }
            else if (tag == Tsmi3) { BtnIndex = 3; Tsmi3.Checked = true; }
            else if (tag == Tsmi4) { BtnIndex = 4; Tsmi4.Checked = true; }
            else if (tag == Tsmi5) { BtnIndex = 5; Tsmi5.Checked = true; }
            if ((BtnIndex >= 0) && (BtnIndex < jogDF.Count))
            {
                stepWidth = jogDF[BtnIndex].Dist;
                feed = jogDF[BtnIndex].Feed;
                //        Logger.Trace("Click {0}  {1}  {2}", BtnIndex, stepWidth, feed);
                SetToolTipButtons();
            }
            tag.Checked = true;
        }
        internal void SetApperance(bool classicButtons)
        {
            SetEnabled(jogEnabled);
            useClassicButtons = classicButtons;
            PanelButtons.Visible = useClassicButtons;
            PanelJoyStick.Visible = !useClassicButtons;
            Invalidate();
        }
        private void AlignButtons()
        {
            PanelJoyStick.Size = PanelButtons.Size = new Size(Width, Height);
            int t2 = PanelButtons.Height / 3;
            int t3 = PanelButtons.Height * 2 / 3;
            int w = PanelButtons.Width;
            int h = t2;
            BtnUp.Width = BtnStop.Width = BtnDown.Width = w;
            BtnUp.Height = BtnStop.Height = BtnDown.Height = h;
            BtnStop.Top = t2;
            BtnDown.Top = t3;
        }

        private void PanelJoyStick_Paint(object sender, PaintEventArgs e)
        {
            if ((jogIndex > 0) && (jogIndex < jogRaster))
            {
                Pen myPen = new Pen(ColorJogHighlight, Height / 15);
                //    double a1 = jogSegment * Math.PI / 4;
                //    double a2 = 20 * Math.PI / 180;
                double r = ((double)jogIndex + 0.5) * (Height / 2) / jogRaster;
                if (jogSegment < 0)
                    r *= -1;
                int y = (int)((Height / 2) + r);
                e.Graphics.DrawLine(myPen, 2, y, Width - 2, y);
            }
            base.OnPaint(e);
        }

        private void CreateBackgroundPicture()                  // create background picture for control
        {
            bool border = true;
            jogRaster = 1 + Properties.Settings.Default.guiJoystickRaster;
            PanelJoyStick.Size = PanelButtons.Size = new Size(Width, Height);
            jogBackground = new System.Drawing.Bitmap(Width, Height);
            //     int stepX = Width / (2 * jogRaster);
            int stepY = Height / (2 * jogRaster);
            int fontH = Height / 10;
            Font stringFont = new Font("Microsoft Sans Serif", fontH, GraphicsUnit.Pixel);
            SizeF stringSize;

            //       jogRadius = stepY / 2;
            Pen borderColor = new Pen(Color.Black, 1);
            int i, locationX, locationY, sizeX, sizeY;

            using (Graphics grp = Graphics.FromImage(jogBackground))
            {
                for (i = 0; i < jogRaster; i++)
                {
                    SolidBrush bkgrColor = new SolidBrush(Colors.ColorFromHSV(240, (double)(jogRaster - i) / jogRaster, 1));// GroupColor.FromArgb(255, 0, 0, (jogRaster-i) * (255 / jogRaster)));
                    if (i == (jogRaster - jogRasterMark))
                        bkgrColor = new SolidBrush(Color.FromArgb(255, 0, 255, 0));
                    //          locationX = i * stepX;
                    locationY = i * stepY;
                    sizeX = Width;// - 2 * locationX;
                    sizeY = Height - 2 * locationY;
                    float sweep = 36;
                    grp.FillRectangle(bkgrColor, new Rectangle(0, locationY, sizeX, sizeY));
                    if (border)
                        grp.DrawRectangle(borderColor, new Rectangle(0, locationY, sizeX - 1, sizeY - 1));
                }
                i = jogRaster - 1;
                //      locationX = i * stepX;
                locationY = i * stepY;
                //       sizeX = Width - 2 * locationX;
                sizeY = Height - 2 * locationY;
                grp.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, Height / 2 - sizeY / 2, Width, sizeY));
                if (showStop)
                    grp.DrawImage(Properties.Resources.stop, Width / 2 - sizeY / 2, Height / 2 - sizeY / 2, sizeY, sizeY);

                if (Properties.Settings.Default.UserControlJogControlShowArrow)
                {
                    Color ca = Colors.ColorFromHSV(240, 0.2, 1);
                    Image tmp_r = (Image)arrow_r.Clone();
                    tmp_r.RotateFlip((RotateFlipType.Rotate270FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_r.Clone(), ca), Width / 2 - Width / 2, Height / 10 - sizeY / 2, Width, sizeY * 2);
                    tmp_r.RotateFlip((RotateFlipType.Rotate180FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_r.Clone(), ca), Width / 2 - Width / 2, Height * 7 / 10 - sizeY / 2, Width, sizeY * 2);
                }

                stringSize = grp.MeasureString(axisName, stringFont);
                grp.DrawString(axisName, stringFont, Brushes.White, new RectangleF(3, 1, stringSize.Width, fontH));
            }
        }
    }
}
