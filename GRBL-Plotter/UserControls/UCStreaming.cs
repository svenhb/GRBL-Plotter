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
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCStreaming : UserControl
    {
        private readonly float DpiScaling;
        private bool isLarge = true;
        private readonly int controlHeightFold = MyControl.MinimumHeightFolded;
        private readonly int controlHeightUnfold = 150;

        TimeSpan timeElapsed;           //elapsed time 
        DateTime timeInit;              //time showPlay 
        MyControl m = new MyControl();

        private static System.Timers.Timer timer = new System.Timers.Timer();
        bool enableTime = false;
        bool enableHighlightPlay = false;
        bool highlightPlay = false;

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        protected virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent;
        protected virtual void OnRaiseGuiControlEvent(UserControlGuiControlEventArgs e)
        { RaiseGuiControlEvent?.Invoke(this, e); }

        public UCStreaming()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
            timer.Interval = 300;
            timer.Enabled = true;
            timeInit = DateTime.UtcNow;
            timeElapsed = TimeSpan.Zero;
            timer.Start();

            timer.Elapsed += Timer_Tick;

            GbStreaming.Click += GbStreamings_Click;

            EnableButtonsStreaming(false);
            EnableButtonsSimulation(false);
        }
        internal void SetHeight(bool setLarge)
        {
            if (setLarge)
            {
                GbStreaming.BackColor = MyControl.PanelBackColor;   // Color.WhiteSmoke;
                GbStreaming.ForeColor = MyControl.PanelForeColor;
                SetControlHeight(controlHeightUnfold); 
            }
            else
            {
                GbStreaming.BackColor = MyControl.NotifyYellow;
                GbStreaming.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                SetControlHeight(controlHeightFold); 
            }
            Invalidate();
        }
        internal void SetStatusTextGrbl(string s, Color c)
        {
            m.SetLabelSave(LblStatusGrbl, s, c);
        }
        internal void SetStatusTextStreaming(string s, Color? c)
        {
            if (c != null)
                m.SetLabelSave(LblStatusStreaming, s, (Color)c);
            else
                m.SetLabelSave(LblStatusStreaming, s);
        }
        internal String GetStatusTextStreaming()
        { return LblStatusStreaming.Text; }

        internal void EnableButtonsStreaming(bool en)
        {
            btnStreamCheck.Enabled = en;
            btnStreamStart.Enabled = en;
            btnStreamStop.Enabled = en;
        }

        internal void EnableButtonsSimulation(bool en)
        {
            btnSimulate.Enabled = en;
            btnSimulatePause.Enabled = en;
            btnSimulateFaster.Enabled = en;
            btnSimulateSlower.Enabled = en;
        }

        internal void SetStatusSimulationStart(bool simulate)
        {
            //    timer.Stop();
            btnStreamStart.Enabled = !simulate;
            btnStreamStop.Enabled = !simulate;
            btnStreamCheck.Enabled = !simulate;
            btnSimulateFaster.Enabled = simulate;
            btnSimulateSlower.Enabled = simulate;
            btnSimulatePause.Visible = simulate;
            if (simulate)
                btnSimulate.Text = "STOP";
            else
                btnSimulate.Text = "Start path simulation";
        }
        internal void SetStatusSimulationPause(bool pause)
        {
            if (pause)
                btnSimulatePause.Image = Properties.Resources.btn_play;
            else
                btnSimulatePause.Image = Properties.Resources.btn_pause;
        }
        internal void SetStatusStreamStart(bool showPlay, bool highlight, bool blink)
        {
            if (showPlay)
                btnStreamStart.Image = Properties.Resources.btn_play;
            else
                btnStreamStart.Image = Properties.Resources.btn_pause;
            enableHighlightPlay = blink;
            if (highlight)
                btnStreamStart.BackColor = MyControl.NotifyYellow;
            else
                btnStreamStart.BackColor = SystemColors.Control;
        }

        internal void SetTextTime(String txt)
        {
            enableTime = false;
            m.SetLabelSave(LblTime, txt);
        }
        internal void SetTextProgress(String txt)
        {
            m.SetLabelSave(lblProgress, txt);
        }
        internal void ResetProgress()
        {
            pbFile.Value = 0;
            pbFile.Maximum = 100;
            pbBuffer.Value = 0;
            pbBuffer.Maximum = 100;
        }
        internal void SetProgressFile(int val)
        {
            if (val <= pbFile.Maximum)
            {// pbFile.Value = val;
                if (this.pbFile.InvokeRequired)
                { this.pbFile.BeginInvoke((MethodInvoker)delegate () { this.pbFile.Value = val; }); }
                else
                { this.pbFile.Value = val; }
            }
        }
        internal void SetProgressFileMax(int val)
        {
            pbFile.Maximum = val;
        }
        internal void SetProgressBuffer(int val)
        {
            if (val <= pbBuffer.Maximum)
            {//    pbBuffer.Value = val;
                if (this.pbBuffer.InvokeRequired)
                { this.pbBuffer.BeginInvoke((MethodInvoker)delegate () { this.pbBuffer.Value = val; }); }
                else
                { this.pbBuffer.Value = val; }
            }
        }
        internal void SetProgressBufferMax(int val)
        {
            pbBuffer.Maximum = val;
        }

        internal void TimerStart()
        {
            timeInit = DateTime.UtcNow;
            timeElapsed = TimeSpan.Zero;
            enableTime = true;
            //    timer.Start();
        }
        internal void TimerStop()
        {
            enableTime = false;
            //    timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (enableTime)
            {
                timeElapsed = DateTime.UtcNow - timeInit;
                string txt = "Time " + timeElapsed.ToString(@"hh\:mm\:ss");//, culture);
                m.SetLabelSave(LblTime, txt);
            }

            if (enableHighlightPlay)
            {
                if (highlightPlay)
                { btnStreamStart.BackColor = MyControl.NotifyYellow; }
                else
                { btnStreamStart.BackColor = SystemColors.Control; }
                highlightPlay = !highlightPlay;
            }
        }
        private void GbStreamings_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbStreaming.PointToClient(screenPosition);
            if (clientPosition.Y < MyControl.MinimumHeightFolded)
            {
                isLarge = !isLarge;
                SetHeight(isLarge);
                Invalidate();
            }
        }
        private void SetControlHeight(int h)
        { Height = (int)(DpiScaling * h); }

        private void BtnStreamStart_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.streamStartSection));
            else
                OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.streamStart));
            btnStreamCheck.Enabled = false;
            TimerStart();
        }

        private void BtnStreamStop_Click(object sender, EventArgs e)
        {
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.streamStop));
            enableTime = false;
        }
        private void BtnStreamCheck_Click(object sender, EventArgs e)
        {
            btnStreamStart.Enabled = false;
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.streamCheck));
        }

        private void BtnSimulate_Click(object sender, EventArgs e)
        { OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.simulateStart)); }

        private void BtnSimulatePause_Click(object sender, EventArgs e)
        { OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.simulatePause)); }

        private void BtnSimulateFaster_Click(object sender, EventArgs e)
        { OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.simulateFaster)); }

        private void BtnSimulateSlower_Click(object sender, EventArgs e)
        { OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.simulateSlower)); }

        private void UCStreaming_Resize(object sender, EventArgs e)
        {
            pbBuffer.Left = pbFile.Width-(int)(DpiScaling * 100);
        }
        public void RestoreColors()
        {
            LblStatusGrbl.ForeColor = Color.Black;
            LblStatusStreaming.ForeColor = Color.Black;
            SetHeight(isLarge);
        }
    }
}
