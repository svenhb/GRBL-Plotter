/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
 * 2020-09-13 split file
 * 2020-12-03 Bug fix invoke required in line 59
 * 2020-12-16 line 183 remove lock
 */

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {
        #region Streaming

        TimeSpan elapsed;               //elapsed time 
        DateTime timeInit;              //time start 
		
        private int signalResume = 0;   // blinking button
        private int signalLock = 0;     // blinking button
        private int signalPlay = 0;     // blinking button
        private int delayedSend = 0;
        private bool isStreaming = false;
        private bool isStreamingPause = false;
        private bool isStreamingCheck = false;
        private bool isStreamingRequestStop = false;
        private bool isStreamingOk = true;
        private string lblInfoOkString = "Send G-Code";

/***** thread save update *****/
        private void updateProgressBar(int codeProgress, int buffProgress) 
        {
            int cPrgs = codeProgress;
            if (cPrgs < 0) cPrgs = 0; if (cPrgs > 100) cPrgs = 100;
            int bPrgs = buffProgress;
            if (bPrgs < 0) bPrgs = 0; if (bPrgs > 100) bPrgs = 100;

            if (this.pbFile.InvokeRequired)
            {   this.pbFile.BeginInvoke((MethodInvoker)delegate () { this.pbFile.Value = cPrgs; }); }
            else
            {   this.pbFile.Value = cPrgs;   }

            if (this.pbBuffer.InvokeRequired)
            {   this.pbBuffer.BeginInvoke((MethodInvoker)delegate () { this.pbBuffer.Value = bPrgs; }); }
            else
            {   this.pbBuffer.Value = bPrgs;   }

            string txt =string.Format("Progress {0:0.0}%", codeProgress);
            if (this.lblFileProgress.InvokeRequired)
            {   this.lblFileProgress.BeginInvoke((MethodInvoker)delegate () { this.lblFileProgress.Text = txt; }); }
            else
            {   this.lblFileProgress.Text = txt;   }
        }

        /***** Receive streaming status from serial COM *****/
        private bool notifierUpdateMarker = false;
        private bool notifierUpdateMarkerFinish = false;
        private void OnRaiseStreamEvent(object sender, StreamEventArgs e)
        {
            if (isStreaming)
            {
                updateProgressBar((int)e.CodeProgress, (int)e.BuffProgress);
                if (Properties.Settings.Default.notifierMessageProgressEnable)
                {
                    if ((elapsed.Seconds % (int)(60*Properties.Settings.Default.notifierMessageProgressInterval)) == 5) // offset 5 sec. to get message at start
                    {
                        if (notifierUpdateMarker)
                        {
                            notifierUpdateMarker = false;
                            string msg = string.Format("{0}Duration   : {1} min.\r\nCode line  : {2}\r\nProcessed: {3:0.0} %\r\nGrbl Buffer: {4:0.0} %", "", elapsed.Minutes, e.CodeLineSent, e.CodeProgress, e.BuffProgress);//Properties.Settings.Default.notifierMessageProgress
                            if (Properties.Settings.Default.notifierMessageProgressTitle)
                                Notifier.sendMessage(msg, string.Format("{0:0.0} %", e.CodeProgress));    
                            else
                                Notifier.sendMessage(msg);                             
                        }
                    }
                    else
                        notifierUpdateMarker = true;
                }
            }

            int actualCodeLine = e.CodeLineSent;
            if (actualCodeLine < 0) actualCodeLine = 0;
            if (e.CodeLineSent > fCTBCode.LinesCount)
                actualCodeLine = fCTBCode.LinesCount - 1;
            fCTBCode.Selection = fCTBCode.GetLine(actualCodeLine);

            fCTBCodeClickedLineNow = e.CodeLineSent-1;// - 1;
            fCTBCodeMarkLine();         // set Bookmark and marker in 2D-View
//            fCTBCode.DoCaretVisible();
            if (this.fCTBCode.InvokeRequired)
            { this.fCTBCode.BeginInvoke((MethodInvoker)delegate () { this.fCTBCode.DoCaretVisible(); }); }
            else
            { this.fCTBCode.DoCaretVisible(); }

            if (_diyControlPad != null)
                _diyControlPad.sendFeedback("[" + e.Status.ToString() + "]");

            if (Properties.Settings.Default.guiProgressShow)
                VisuGCode.ProcessedPath.processedPathLine(e.CodeLineConfirmed);

			if (logStreaming)  Logger.Trace("OnRaiseStreamEvent  {0}  line {1} ", e.Status.ToString(), e.CodeLineSent);

            switch (e.Status)
            {
                case grblStreaming.lasermode:
                    showLaserMode();
                    break;

                case grblStreaming.reset:
                    flagResetOffset = true;
                    stopStreaming(false);
                    if (e.CodeProgress < 0)
                    {
                        lbInfo.Text = _serial_form.lastError;
                        lbInfo.BackColor = Color.Fuchsia;
                    }
                    else
                    {
                        lbInfo.Text = "Vers. " + _serial_form.grblVers;
                        lbInfo.BackColor = Color.Lime;
                    }
                    statusStripClear(1, 2);
                    toolTip1.SetToolTip(lbInfo, lbInfo.Text);
                    timerUpdateControls = true; timerUpdateControlSource = "grblStreaming.reset";//updateControls();
                    if (_coordSystem_form != null)
                        _coordSystem_form.showValues();

                    ControlPowerSaving.EnableStandby();
                    VisuGCode.ProcessedPath.processedPathClear();
                    break;

                case grblStreaming.error:
                    Logger.Info("streaming error at line {0}", e.CodeLineConfirmed);
                    statusStripSet(1, grbl.lastMessage, Color.Fuchsia);
                    pbFile.ForeColor = Color.Red;
                    lbInfo.Text = Localization.getString("mainInfoErrorLine") + e.CodeLineSent.ToString();
                    lbInfo.BackColor = Color.Fuchsia;
                    fCTBCode.BookmarkLine(actualCodeLine - 1);
                    fCTBCode.DoSelectionVisible();
                    fCTBCode.CurrentLineColor = Color.Red;
                    break;

                case grblStreaming.ok:
                    if (!isStreamingCheck)
                    {
//                        lbInfo.Text = lblInfoOkString + "(" + (e.CodeLineSent+1).ToString() + ")";
                        if (this.lbInfo.InvokeRequired)
                        { this.lbInfo.BeginInvoke((MethodInvoker)delegate () { this.lbInfo.Text = lblInfoOkString + "(" + (e.CodeLineSent + 1).ToString() + ")"; }); }
                        else
                        { this.lbInfo.Text = lblInfoOkString + "(" + (e.CodeLineSent + 1).ToString() + ")"; }

                        lbInfo.BackColor = Color.Lime;
                        signalPlay = 0;
                        btnStreamStart.BackColor = SystemColors.Control;
                    }
                    break;

                case grblStreaming.finish:
                    Logger.Info("streaming finished ok {0}", isStreamingOk);
                    if (isStreamingOk)
                    {
                        if (isStreamingCheck)
                        { lbInfo.Text = Localization.getString("mainInfoFinishCheck"); }   // "Finish checking G-Code"; }
                        else
                        { lbInfo.Text = Localization.getString("mainInfoFinishSend"); }   // "Finish sending G-Code"; }
                        lbInfo.BackColor = Color.Lime;
                    }
                    MainTimer.Stop();
                    MainTimer.Start();
                    timerUpdateControls = true; timerUpdateControlSource = "grblStreaming.finish";//updateControls();
                    saveStreamingStatus(0);
                    showPicBoxBgImage = false;                     // don't show background image anymore
                    pictureBox1.BackgroundImage = null;
                    resetStreaming();
                    
                    if (!notifierUpdateMarkerFinish)    // just notify once
                    {   notifierUpdateMarkerFinish = true;
                        string msg = string.Format("{0}\r\nDuration  :{1} (hh:mm:ss)\r\nCode line :{2}", Properties.Settings.Default.notifierMessageFinish, elapsed.ToString(@"hh\:mm\:ss"), e.CodeLineSent);
                        if (Properties.Settings.Default.notifierMessageProgressTitle)
                            Notifier.sendMessage(msg, "100 %");
                        else
                            Notifier.sendMessage(msg);
                    }
                    break;

                case grblStreaming.waitidle:
                    timerUpdateControls = true; timerUpdateControlSource = "grblStreaming.waitidle";//updateControls();// true);
                    btnStreamStart.Image = Properties.Resources.btn_play;
                    lbInfo.Text = Localization.getString("mainInfoWaitIdle") + e.CodeLineSent.ToString() + ")";
                    lbInfo.BackColor = Color.Yellow;
                    break;

                case grblStreaming.pause:
          //          lock (this)       2020-12-15 removed
                    {
                        signalPlay = 1;
                        lbInfo.BackColor = Color.Yellow;
                        lbInfo.Text = Localization.getString("mainInfoPause") + e.CodeLineSent.ToString() + ")";
                        btnStreamStart.Image = Properties.Resources.btn_play;
                        isStreamingPause = true;
                        MainTimer.Stop();
                        MainTimer.Start();
                        timerUpdateControls = true; timerUpdateControlSource = "grblStreaming.pause";//updateControls(true);

                        saveStreamingStatus(e.CodeLineSent);

                        if (Properties.Settings.Default.flowControlEnable) // send extra Pause-Code in MainTimer_Tick from Properties.Settings.Default.flowControlText
                            delayedSend = 2;

                        if (logStreaming) Logger.Trace("OnRaiseStreamEvent - pause: {0}  in line:{1}", fCTBCode.Lines[fCTBCodeClickedLineNow], fCTBCodeClickedLineNow);

                        if (fCTBCode.Lines[fCTBCodeClickedLineNow].Contains("M0") && fCTBCode.Lines[fCTBCodeClickedLineNow].Contains("Tool"))  // keyword set in gcodeRelated 1132
                        {   signalShowToolExchangeMessage = true;   if (logStreaming) Logger.Trace("OnRaiseStreamEvent trigger ToolExchangeMessage");}

                        //  if (Properties.Settings.Default.importGCToolChangeCode.Length > 1)
                        //  {   processCommands(Properties.Settings.Default.importGCToolChangeCode); }
                    }
                    break;

                case grblStreaming.toolchange:
                    timerUpdateControls = true; timerUpdateControlSource = "grblStreaming.toolchange";// updateControls();
                    btnStreamStart.Image = Properties.Resources.btn_play;
                    lbInfo.Text = Localization.getString("mainInfoToolChange");
                    lbInfo.BackColor = Color.Yellow;
                    cBTool.Checked = _serial_form.toolInSpindle;
                    break;

                case grblStreaming.stop:
                    timerUpdateControls = true; timerUpdateControlSource = "grblStreaming.stop";// updateControls();
                    lbInfo.Text = Localization.getString("mainInfoStopStream") + e.CodeLineSent.ToString() + ")";
                    lbInfo.BackColor = Color.Fuchsia;

                    if (Properties.Settings.Default.flowControlEnable) // send extra Pause-Code in MainTimer_Tick from Properties.Settings.Default.flowControlText
                        delayedSend = 2;
                    break;

                default:
                    break;
            }

            lastLabelInfoText = lbInfo.Text;
  //          lbInfo.Text += overrideMessage;
            if (this.lbInfo.InvokeRequired)
            { this.lbInfo.BeginInvoke((MethodInvoker)delegate () { this.lbInfo.Text += overrideMessage; }); }
            else
            { this.lbInfo.Text += overrideMessage; }
        }
        public delegate void Del();

        bool signalShowToolExchangeMessage = false;
        private void showToolChangeMessage()
        {
			if (logStreaming) Logger.Trace("showToolChangeMessage");
			Console.Beep();
            using (MessageForm f = new MessageForm())
            {
                string tool = fCTBCode.Lines[fCTBCodeClickedLineNow];
                int c1 = tool.IndexOf('(');
                if (c1 > 0)
                {   tool = tool.Substring(c1 + 1);
                    tool = tool.Substring(0, tool.Length - 1);
                    tool = tool.Replace("Color", "\r  Color");
                }
                string msg = Localization.getString("mainToolChange1") + "  " + tool + "\r" + Localization.getString("mainToolChange2");
                f.showMessage(Localization.getString("mainToolChange"), msg, 1);
                var result = f.ShowDialog(this);
                if (result == DialogResult.Yes)
                {   startStreaming();  }
            }
        }
        private void btnStreamStart_Click(object sender, EventArgs e)
        { updateLogging(); startStreaming(); }
        // if startline > 0 start with pause
        private void startStreaming(int startLine = 0)
        {
            isStreamingRequestStop = false;
            lblInfoOkString = Localization.getString("mainInfoSendCode");
            notifierUpdateMarker = false;
            notifierUpdateMarkerFinish = false;
            if (fCTBCode.LinesCount > 1)
            {
                if (!isStreaming)
                {
                    Logger.Info("Start streaming at line:{0}  showProgress:{1}  backgroundImage:{2}", startLine, Properties.Settings.Default.guiProgressShow, Properties.Settings.Default.guiBackgroundImageEnable);
                    expandCodeBlocksToolStripMenuItem_Click(null, null);
                    VisuGCode.ProcessedPath.processedPathClear();
					MainTimer.Stop();
					MainTimer.Start();

                    isStreaming = true;
                    isStreamingPause = false;
                    isStreamingCheck = false;
                    isStreamingOk = true;
                    VisuGCode.markSelectedFigure(0);
                    if (startLine > 0)
                    {
                        btnStreamStart.Image = Properties.Resources.btn_pause;
          //              isStreamingPause = true;
                    }

                    if (!grbl.isVersion_0)
                    {
                        gBoxOverride.Height = 175;
                        gBoxOverrideBig = true;
                    }

                    timerUpdateControlSource = "startStreaming";
                    updateControls();
                    timeInit = DateTime.UtcNow;
                    elapsed = TimeSpan.Zero;
                    lbInfo.Text = Localization.getString("mainInfoSendCode");// "Send G-Code";
                    lbInfo.BackColor = Color.Lime;
                    for (int i = 0; i < fCTBCode.LinesCount; i++)
                        fCTBCode.UnbookmarkLine(i);

                    //save gcode
                    string fileName = Application.StartupPath + "\\" + fileLastProcessed;
                    string txt = fCTBCode.Text;
                    File.WriteAllText(fileName + ".nc", txt);
                    File.Delete(fileName + ".xml");
                    SaveRecentFile(fileLastProcessed + ".nc");

                    lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
                    _serial_form.startStreaming(fCTBCode.Lines, startLine);
                    btnStreamStart.Image = Properties.Resources.btn_pause;
                    btnStreamCheck.Enabled = false;
                    onPaint_setBackground();                // Generante a background-image for pictureBox to avoid frequent drawing of pen-up/down paths
                    VisuGCode.setPathAsLandMark();
                    ControlPowerSaving.SuppressStandby();
                }
                else
                {
                    if (!isStreamingPause)
                    {
                        Logger.Info("Pause streaming - pause stream");
                        btnStreamStart.Image = Properties.Resources.btn_play;
                        _serial_form.pauseStreaming();
            //            isStreamingPause = true;
                        statusStripSet(0, Localization.getString("statusStripeStreamingStatusSaved"), Color.LightGreen);
                    }
                    else
                    {
                        Logger.Info("Pause streaming - continue stream  ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
                        btnStreamStart.Image = Properties.Resources.btn_pause;
                        _serial_form.pauseStreaming();
                        isStreamingPause = false;
                        statusStripClear(0);
                    }
                }
            }
        }
        private void btnStreamCheck_Click(object sender, EventArgs e)
        {
            if ((fCTBCode.LinesCount > 1) && (!isStreaming))
            {
                Logger.Info("check code");
                isStreaming = true;
                isStreamingCheck = true;
                isStreamingOk = true;
                timerUpdateControlSource = "btnStreamCheck_Click";
                updateControls();
                timeInit = DateTime.UtcNow;
                elapsed = TimeSpan.Zero;
                lbInfo.Text = Localization.getString("mainInfoCheckCode");// "Check G-Code";
                lbInfo.BackColor = SystemColors.Control;
                for (int i = 0; i < fCTBCode.LinesCount; i++)
                    fCTBCode.UnbookmarkLine(i);
                _serial_form.startStreaming(fCTBCode.Lines, 0, true);
                btnStreamStart.Enabled = false;
                onPaint_setBackground();
            }
        }
        private void btnStreamStop_Click(object sender, EventArgs e)
        { updateLogging(); stopStreaming(); }
        private void stopStreaming(bool showMessage = true)
        {
            Logger.Info("stop streaming at line {0}", (fCTBCodeClickedLineNow + 1));
            showPicBoxBgImage = false;                 // don't show background image anymore
                                                       //            pictureBox1.BackgroundImage = null;
            signalPlay = 0;
            isStreamingRequestStop = true;
			
            _serial_form.stopStreaming(showMessage);
						
            if (isStreaming || isStreamingCheck)
            {   lbInfo.Text = Localization.getString("mainInfoStopStream2") + (fCTBCodeClickedLineNow + 1).ToString() + " )";
                lbInfo.BackColor = Color.Fuchsia;
            }

            resetStreaming();
        }
        private void resetStreaming()
        {
            isStreaming = false;
            isStreamingCheck = false;
            pbFile.Value = 0;
            pbBuffer.Value = 0;

            signalPlay = 0;
            VisuGCode.ProcessedPath.processedPathClear();
            btnStreamStart.Image = Properties.Resources.btn_play;
            btnStreamStart.BackColor = SystemColors.Control;
            btnStreamStart.Enabled = true;
            btnStreamCheck.Enabled = true;
            timerUpdateControlSource = "resetStreaming";
            updateControls();
            pictureBox1.Invalidate();
            ControlPowerSaving.EnableStandby();
        }
        private void btnStreamPause_Click(object sender, EventArgs e)
        { updateLogging(); _serial_form.pauseStreaming(); }
				
        #endregion

    }
}