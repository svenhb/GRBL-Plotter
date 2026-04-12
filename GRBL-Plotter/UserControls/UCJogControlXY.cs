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
using static AForge.Imaging.Filters.HitAndMiss;
namespace GrblPlotter.UserControls
{
    public partial class UCJogControlXY : UserControl
    {
        private readonly Image arrow_r = Properties.Resources.a_r;		// arrow to the right
        private readonly Image arrow_d = Properties.Resources.a_d;      // arrow to upper-right
        private readonly Color ColorBtnDisabled = SystemColors.Control;
        private readonly Color ColorBtnHighlight = Color.Yellow;
        private Color ColorJogHighlight = Color.FromArgb(60, Color.Yellow);
        private System.Drawing.Image jogBackground = new System.Drawing.Bitmap(10, 10);
        private Color ColorBtnNormal = Color.Gray;

        private bool jogStart = false;  // send comands by timer
        private bool showStop = true;   // show stop button
        private bool jogEnabled = false;// show enabled style (instead of gray)
        private bool useClassicButtons = true;
        //    private int jogRadius;
        private int jogRaster = 6;
        private readonly int jogRasterMark = 0;
        private int jogSegment = 0, jogIndex = 0;

        private string jogCommand = "$J=G91";
        private double stepWidth = 5;
        private double stepX = 0, stepY = 0;
        private int feed = 600;
        private int BtnIndex = 1;
        private List<DistFeed> jogDF = new List<DistFeed>();
        private readonly string axisName = "XY";

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        System.Windows.Forms.Timer jogTimer = new System.Windows.Forms.Timer();

        public UCJogControlXY()
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

        private void UserControlJogControlXY_Load(object sender, EventArgs e)
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
            Btn5.Visible = showStop;
        }
        internal void TriggerMove(string action)
        {
            if (!useClassicButtons)
            {
                float d = jogDF[jogIndex].Dist;
                int feed = jogDF[jogIndex].Feed;
                stepX = 0; stepY = 0;
                if (action.Contains("XDec")) { stepX = -d; }
                if (action.Contains("XInc")) { stepX = d; }
                if (action.Contains("YDec")) { stepY = -d; }
                if (action.Contains("YInc")) { stepY = d; }
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.0}Y{2:0.0}F{3}", jogCommand, stepX, stepY, feed).Replace(",", "."), 0, null, null));
            }
            else
            {
                stepX = stepY = 0;
                if (action.Contains("XDec")) { stepX = -stepWidth; }
                if (action.Contains("XInc")) { stepX = stepWidth; }
                if (action.Contains("YDec")) { stepY = -stepWidth; }
                if (action.Contains("YInc")) { stepY = stepWidth; }
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.0}Y{2:0.0}F{3}", jogCommand, stepX, stepY, feed).Replace(",", "."), 0, null, null));
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
                            LblJogProp.Text = string.Format("Dist: {0}\nFeed: {1}", jogDF[jogIndex].Dist, jogDF[jogIndex].Feed);
                    }
                }
                else
                { FindButtonSetStep(sender); }
            }
        }
        private Point LabelLocation(Point p)
        {
            int offy = (p.Y > Height / 2) ? -20 : 20;
            int offx = (p.X > Width / 2) ? -50 : 10;
            return new Point(p.X + offx, p.Y + offy);
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
                LblJogProp.Visible = true;
                LblJogProp.Location = LabelLocation(e.Location);
                if (jogIndex < 1)
                    LblJogProp.Text = string.Format("Stop");
                else if (jogIndex < jogRaster)
                    LblJogProp.Text = string.Format("Dist: {0}\nFeed:{1}", jogDF[jogIndex].Dist, jogDF[jogIndex].Feed);
            }
            Refresh();
        }
        private void PanelJoyStick_MouseLeave(object sender, EventArgs e)
        {
            jogSegment = 0; jogIndex = 0;
            LblJogProp.Visible = false;
            Refresh();
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
                    index = (int)(r / (Width / 2) * jogRaster);
                    jogSegment = s; jogIndex = index;
                    break;
                }
            }
        }
        private void Btn_MouseHover(object sender, EventArgs e)
        {
            HighlightButton(sender, true);
            var btn = (Button)sender;
            btn.Focus();
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            HighlightButton(sender, false);
            Btn5.Focus();
        }
        private void SendPositions(int s, int r)
        {
            if (r < 1) { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x85, null, null)); }
            else if (r < jogRaster)
            {
                if (r > jogDF.Count)
                { r = jogDF.Count - 1; }
                if (r < 1)
                { r = 1; }
                stepWidth = jogDF[r].Dist;// jogDistance[r];
                feed = jogDF[r].Feed;// (int)jogFeed[r];
                if (s == -3) { stepX = -stepWidth; stepY = stepWidth; }
                else if (s == -2) { stepX = 0; stepY = stepWidth; }
                else if (s == -1) { stepX = stepWidth; stepY = stepWidth; }
                else if (s == -4) { stepX = -stepWidth; stepY = 0; }
                else if (s == 0) { stepX = stepWidth; stepY = 0; }
                else if (s == 3) { stepX = -stepWidth; stepY = -stepWidth; }
                else if (s == 2) { stepX = 0; stepY = -stepWidth; }
                else if (s == 1) { stepX = stepWidth; stepY = -stepWidth; }

                if (true)//WithinAxisLimits(stepX, stepY, false))
                {
                    OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.0}Y{2:0.0}F{3}", jogCommand, stepX, stepY, feed).Replace(",", "."), 0, null, null));
                    jogStart = true;
                    jogTimer.Start();
                }
                else
                {
                    jogStart = false;
                    jogTimer.Stop();
                }
            }
        }

        private void FindButtonSetStep(object sender)
        {
            bool btnPressed = false;
            if (sender == Btn1) { stepX = -stepWidth; stepY = stepWidth; btnPressed = true; }
            else if (sender == Btn2) { stepX = 0; stepY = stepWidth; btnPressed = true; }
            else if (sender == Btn3) { stepX = stepWidth; stepY = stepWidth; btnPressed = true; }
            else if (sender == Btn4) { stepX = -stepWidth; stepY = 0; btnPressed = true; }
            else if (sender == Btn6) { stepX = stepWidth; stepY = 0; btnPressed = true; }
            else if (sender == Btn7) { stepX = -stepWidth; stepY = -stepWidth; btnPressed = true; }
            else if (sender == Btn8) { stepX = 0; stepY = -stepWidth; btnPressed = true; }
            else if (sender == Btn9) { stepX = stepWidth; stepY = -stepWidth; btnPressed = true; }

            else if (sender == Btn5)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x85, sender, null));
                jogStart = false;
                jogTimer.Stop();
            }

            if (btnPressed)
            {
                if (true)//grbl.WithinAxisLimits(stepX, stepY, false))
                {
                    OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.0}Y{2:0.0}F{3}", jogCommand, stepX, stepY, feed).Replace(",", "."), 0, sender, null));
                    jogStart = true;
                    jogTimer.Start();
                }
                else
                {
                    jogStart = false;
                    jogTimer.Stop();
                }
            }
        }


        protected virtual void JogTimer_Tick(object sender, EventArgs e)
        {
            if (jogStart)
            {
                if (!useClassicButtons)
                {
                    SendPositions(jogSegment, jogIndex);
                }
                else
                {
                    OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.0}Y{2:0.0}F{3}", jogCommand, stepX, stepY, feed).Replace(",", "."), 0, sender, null));
                }
            }
        }


        private void JogDisable()
        {
            jogSegment = 0; jogIndex = 0;
            jogStart = false;
            jogTimer.Stop();
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

            Btn6.BackgroundImage = (Image)tmp_r.Clone();
            Btn8.BackgroundImage = (Image)tmp_r.Clone(); Btn8.BackgroundImage.RotateFlip((RotateFlipType.Rotate90FlipNone));
            Btn4.BackgroundImage = (Image)tmp_r.Clone(); Btn4.BackgroundImage.RotateFlip((RotateFlipType.Rotate180FlipNone));
            Btn2.BackgroundImage = (Image)tmp_r.Clone(); Btn2.BackgroundImage.RotateFlip((RotateFlipType.Rotate270FlipNone));

            Btn3.BackgroundImage = (Image)tmp_d.Clone();
            Btn9.BackgroundImage = (Image)tmp_d.Clone(); Btn9.BackgroundImage.RotateFlip((RotateFlipType.Rotate90FlipNone));
            Btn7.BackgroundImage = (Image)tmp_d.Clone(); Btn7.BackgroundImage.RotateFlip((RotateFlipType.Rotate180FlipNone));
            Btn1.BackgroundImage = (Image)tmp_d.Clone(); Btn1.BackgroundImage.RotateFlip((RotateFlipType.Rotate270FlipNone));

            tmp_r = Properties.Resources.stop;
            if (!enabled)
                Btn5.BackgroundImage = MyControl.BitmapSetToGray(tmp_r);
            else
                Btn5.BackgroundImage = tmp_r;

            Font stringFont = new Font("Microsoft Sans Serif", 40, GraphicsUnit.Pixel);
            using (Graphics grp = Graphics.FromImage(Btn1.BackgroundImage))
            {
                grp.DrawString(axisName, stringFont, Brushes.Black, -5, -5);
            }
        }

        private void EnableButton(bool enable)
        {
            Btn2.Enabled = Btn5.Enabled = Btn8.Enabled = enable;
            Btn1.Enabled = Btn4.Enabled = Btn7.Enabled = enable;
            Btn3.Enabled = Btn6.Enabled = Btn9.Enabled = enable;
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
                    ((Button)sender).BackColor = MyControl.ButtonBackColor;// ColorBtnNormal;
                else
                    ((Button)sender).UseVisualStyleBackColor = true;
            }
        }

        private void UserControlJogControlXY_EnabledChanged(object sender, EventArgs e)
        {
            SetEnabled(Enabled);
        }
        internal void SetEnabled(bool enable)
        {
            SetButtonImages(enable);
            AlignButtons();
            CreateBackgroundPicture();
            if (enable)
            {
                PanelJoyStick.BackgroundImage = jogBackground;
                ColorJogHighlight = Color.Yellow;// GroupColor.FromArgb(90, GroupColor.Yellow);
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
            string s = string.Format("{0:0.00}", stepWidth);
            string f = "; F" + feed.ToString();

            toolTip1.SetToolTip(Btn1, "X -" + s + "; Y " + s + f);
            toolTip1.SetToolTip(Btn2, "X 0; Y " + s + f);
            toolTip1.SetToolTip(Btn3, "X " + s + "; Y " + s + f);
            toolTip1.SetToolTip(Btn4, "X -" + s + "; Y 0");
            toolTip1.SetToolTip(Btn5, "Stop");
            toolTip1.SetToolTip(Btn6, "X " + s + "; Y 0" + f);
            toolTip1.SetToolTip(Btn7, "X -" + s + "; Y -" + s + f);
            toolTip1.SetToolTip(Btn8, "X 0; Y -" + s + f);
            toolTip1.SetToolTip(Btn9, "X " + s + "; Y -" + s + f);
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
            int l2 = PanelButtons.Width / 3;
            int l3 = PanelButtons.Width * 2 / 3;
            int t2 = l2, t3 = l3;
            int w = l2, h = t2;
            Btn1.Width = Btn2.Width = Btn3.Width = w;
            Btn4.Width = Btn5.Width = Btn6.Width = w;
            Btn7.Width = Btn8.Width = Btn9.Width = w;
            Btn1.Height = Btn2.Height = Btn3.Height = h;
            Btn4.Height = Btn5.Height = Btn6.Height = h;
            Btn7.Height = Btn8.Height = Btn9.Height = h;
            Btn2.Left = Btn5.Left = Btn8.Left = l2;
            Btn3.Left = Btn6.Left = Btn9.Left = l3;
            Btn4.Top = Btn5.Top = Btn6.Top = t2;
            Btn7.Top = Btn8.Top = Btn9.Top = t3;
        }

        private void PanelJoyStick_Paint(object sender, PaintEventArgs e)
        {
            if ((jogIndex > 0) && (jogIndex < jogRaster))
            {
                Pen myPen = new Pen(ColorJogHighlight, Width / 15);
                double r = ((double)jogIndex + 0.5) * (Width / 2) / jogRaster;
                e.Graphics.DrawArc(myPen, (int)((Width / 2) - r), (int)((Height / 2) - r), (int)r * 2, (int)r * 2, jogSegment * 45 - 15, 30);
                //e.Graphics.DrawString();
            }
            base.OnPaint(e);
        }

        private void CreateBackgroundPicture()                  // create background picture for control
        {
            bool border = true;
            jogRaster = 1 + Properties.Settings.Default.guiJoystickRaster;
            PanelJoyStick.Size = PanelButtons.Size = new Size(Width, Height);
            jogBackground = new System.Drawing.Bitmap(Width, Height);
            int stepX = Width / (2 * jogRaster);
            int stepY = Height / (2 * jogRaster);
            int fontH = Height / 10;
            Font stringFont = new Font("Microsoft Sans Serif", fontH, GraphicsUnit.Pixel);
            SizeF stringSize;// = new SizeF();

            //    jogRadius = stepX / 2;
            Pen borderColor = new Pen(Color.Black, 1);
            int i, locationX, locationY, sizeX, sizeY;
            Color c;
            using (Graphics grp = Graphics.FromImage(jogBackground))
            {
                for (i = 0; i < jogRaster; i++)
                {
                    c = Colors.ColorFromHSV(240, (double)(jogRaster - i) / jogRaster, 1);
                    SolidBrush bkgrColor = new SolidBrush(c);
                    if (i == (jogRaster - jogRasterMark))
                        bkgrColor = new SolidBrush(Color.FromArgb(255, 0, 255, 0));
                    locationX = i * stepX;
                    locationY = i * stepY;
                    sizeX = Width - 2 * locationX;
                    sizeY = Height - 2 * locationY;
                    float sweep = 36;
                    for (float k = -sweep / 2; k < 360; k += 45)
                    {
                        grp.FillPie(bkgrColor, new Rectangle(locationX, locationY, sizeX, sizeY), k, sweep);
                        if (border)
                            grp.DrawPie(borderColor, new Rectangle(locationX, locationY, sizeX - 1, sizeY - 1), k, sweep);
                    }
                }
                i = jogRaster - 1;
                locationX = i * stepX;
                locationY = i * stepY;
                sizeX = Width - 2 * locationX;
                sizeY = Height - 2 * locationY;
                grp.FillEllipse(new SolidBrush(Color.White), new Rectangle(locationX, locationY, sizeX, sizeY));
                if (showStop)
                    grp.DrawImage(Properties.Resources.stop, Width / 2 - sizeX / 2, Height / 2 - sizeY / 2, sizeX, sizeY);

                if (Properties.Settings.Default.UserControlJogControlShowArrow)
                {
                    Color ca = Colors.ColorFromHSV(240, 0.2, 1);
                    Image tmp_r = (Image)arrow_r.Clone();
                    tmp_r.RotateFlip((RotateFlipType.Rotate270FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_r.Clone(), ca), Width / 2 - sizeX / 2, Height / 10 - sizeY / 2, sizeX, sizeY * 2);
                    tmp_r.RotateFlip((RotateFlipType.Rotate270FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_r.Clone(), ca), Width / 10 - sizeX / 2, Height * 5 / 10 - sizeY / 2, sizeX * 2, sizeY);
                    tmp_r.RotateFlip((RotateFlipType.Rotate180FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_r.Clone(), ca), Width * 7 / 10 - sizeX / 2, Height * 5 / 10 - sizeY / 2, sizeX * 2, sizeY);
                    tmp_r.RotateFlip((RotateFlipType.Rotate90FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_r.Clone(), ca), Width / 2 - sizeX / 2, Height * 7 / 10 - sizeY / 2, sizeX, sizeY * 2);

                    Image tmp_d = (Image)arrow_d.Clone();   // 1 3 9 7
                    tmp_d.RotateFlip((RotateFlipType.Rotate270FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_d.Clone(), ca), Width * 3 / 10 - sizeX / 2, Height * 3 / 10 - sizeY / 2, sizeX, sizeY);
                    tmp_d.RotateFlip((RotateFlipType.Rotate90FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_d.Clone(), ca), Width * 7 / 10 - sizeX / 2, Height * 3 / 10 - sizeY / 2, sizeX, sizeY);
                    tmp_d.RotateFlip((RotateFlipType.Rotate90FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_d.Clone(), ca), Width * 7 / 10 - sizeX / 2, Height * 7 / 10 - sizeY / 2, sizeX, sizeY);
                    tmp_d.RotateFlip((RotateFlipType.Rotate90FlipNone));
                    grp.DrawImage(MyControl.BitmapReplaceColor((Image)tmp_d.Clone(), ca), Width * 3 / 10 - sizeX / 2, Height * 7 / 10 - sizeY / 2, sizeX, sizeY);
                }
                stringSize = grp.MeasureString(axisName, stringFont);
                grp.DrawString(axisName, stringFont, Brushes.Black, new RectangleF(3, 1, stringSize.Width, fontH));
            }
        }
        private void HighlightSegment(int s, int index, bool highlight)
        {
            SolidBrush bkgrColor = new SolidBrush(Color.Yellow);
            int i = jogRaster - index;
            int stepX = Width / (2 * jogRaster);
            int stepY = Height / (2 * jogRaster);
            int locationX = i * stepX;
            int locationY = i * stepY;
            int sizeX = Width - 2 * locationX;
            int sizeY = Height - 2 * locationY;
            float sweep = 36;
            float k = -sweep / 2 + s * 45;
            using (Graphics grp = Graphics.FromImage(jogBackground))
            {
                grp.FillPie(bkgrColor, new Rectangle(locationX, locationY, sizeX, sizeY), k, sweep);
            }
            PanelJoyStick.BackgroundImage = jogBackground;
        }
    }
}
