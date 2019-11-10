/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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

/* 2019-11-10 new
 * 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class ControlProbing : Form
    {
        private enum probingMode { noProbing, edgeFinder, centerFinder, toolLenght }

        //    private ArrayList gcodeString = new ArrayList();
        private string ProbeCommand = "G91 G38.3";
        private string CoordCommand = "G10 L2 P0";        // G92
        private bool useG92 = false;
        private int probingAxisIndex = 0;
        private int probingTime = 5;
        private decimal[] probingOffset = { 0, 0, 0 };
        private decimal[] probingSave = { 1, 1, 1 };
        private int probingValuesIndex = 0;
        private string[] probingAxis = { "X", "Y", "Z" };
        private const int stateCommandsMax = 40;
        private string[] stateCommands = new string[stateCommandsMax];


        private xyzPoint[] probingValues = { new xyzPoint(), new xyzPoint(), new xyzPoint(), new xyzPoint(), new xyzPoint() };
        private xyPoint probePosOld = new xyPoint(0, 0);
        private xyPoint probePosNew = new xyPoint(0, 0);
        private xyzPoint probePos = new xyzPoint();
        private xyzPoint probeStartMachine = new xyzPoint();

        private string probingStep2 = "";
        private string probingStep3 = "";
        private string probingMoveSave = "";
        private string probingAxisX = "";
        private string probingAxisY = "";
        //        private string probingAxisZ = "";

        private bool probeX = false, probeY = false;
        private int probingCount = 0;
        private bool updateProgress = false;
        private decimal progressDistance = 1;
        private double angle = 0;

        private probingMode probingAction = probingMode.noProbing;
        private grblState grblStateNow = grblState.run;
        private bool isIdle = false;    // trigger to send commands

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ControlProbing()
        {
            Logger.Trace("++++++ ControlProbing START ++++++");
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }

        private void ControlProbing_Load(object sender, EventArgs e)
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(100, 100); }
            gBHardware.Click += ControlProbing_Click;
            gBMovement.Click += ControlProbing_Click;
            gBCoordinates.Click += ControlProbing_Click;

            lblEFProgress.Text = "";
            lblCFProgress.Text = "";
            btnCancelEF.Enabled = false;
            btnCancelCF.Enabled = false;
            rB1.Image = Properties.Resources.efOff1;
            rB2.Image = Properties.Resources.efOff2;
            rB3.Image = Properties.Resources.efOff3;
            rB4.Image = Properties.Resources.efOff4;
            rB5.Image = Properties.Resources.efOff5;
            rB6.Image = Properties.Resources.efOff6;
            rB7.Image = Properties.Resources.efOff7;
            rB8.Image = Properties.Resources.efOff8;
            rB9.Image = Properties.Resources.efOff9;
            rBCF1.Image = Properties.Resources.cfOff1;
            rBCF2.Image = Properties.Resources.cfOff2;
            pBTL.Image = Properties.Resources.tlOn;
        }

        public grblState setGrblSaState
        {   set
            {   grblStateNow = value;
                if (grblStateNow == grblState.idle)
                    isIdle = true;
                if (grblStateNow == grblState.alarm)
                {   lblEFProgress.Text = "ALARM";
                    updateProgress = false;
                    probingCount = -1;
                }
            }
        }

        // get height information from main-GUI OnRaisePosEvent line 192
        public xyzPoint setPosProbe
        {
            set
            {
                probePos = value;   // grbl.getCoord("PRB");
                if (updateProgress)
                {
                    if (grbl.getPRBStatus())
                    {   lblEFProgress.Text = "";
                        lblCFProgress.Text = "";
                        lblTLProgress.Text = "";
                    }
                    else
                    {
                        if (probingAction == probingMode.edgeFinder)
                        {   probingFinishEF();
                            lblEFProgress.Text = Localization.getString("probingFail1");//"Fail: no contact";
                            lblEFStatus.Text = Localization.getString("probingFail2");//"Cancel probing";
                        }
                        else if (probingAction == probingMode.centerFinder)
                        {
                            probingFinishCF();
                            lblCFProgress.Text = Localization.getString("probingFail1");//"Fail: no contact";
                            lblCFStatus.Text = Localization.getString("probingFail1");//"Cancel probing";
                        }
                        else if (probingAction == probingMode.toolLenght)
                        {
                            probingFinishCF();
                            lblCFProgress.Text = Localization.getString("probingFail1");//"Fail: no contact";
                        }
                    }
                }
                probingMoveSave = "";
                updateProgress = false;

                probePosNew.X = probePos.X; probePosNew.Y = probePos.Y;

                double tmpProbePos = probePos.X - (double)probingOffset[0];
                string tmpEF= string.Format("Ok X:{0}", probePos.X);

                //   double angle = 
                if (probingAxisIndex == 1) { tmpProbePos = probePos.Y - (double)probingOffset[1]; tmpEF = string.Format("Ok Y:{0}", probePos.Y); }
                if (probingAxisIndex == 2) { tmpProbePos = probePos.Z - (double)probingOffset[2]; tmpEF = string.Format("Ok Z:{0}", probePos.Z); }

                    if (useG92)
                {
                    tmpProbePos = (grbl.posMachine.X - probePos.X) + (double)probingOffset[0];
                    if (probingAxisIndex == 1) tmpProbePos = (grbl.posMachine.Y - probePos.Y) + (double)probingOffset[1];
                    if (probingAxisIndex == 2) tmpProbePos = (grbl.posMachine.Z - probePos.Z) + (double)probingOffset[2];
                }

                if (probingAction == probingMode.edgeFinder)
                {
                    if ((probingAxisIndex >= 0) && (probingAxisIndex <= 2))
                    {
                        sendCommandEvent(new CmdEventArgs((string.Format("{0} {1}{2}", CoordCommand, probingAxis[probingAxisIndex], tmpProbePos).Replace(',', '.'))));
                        probingMoveSave = string.Format("G90 G00 {0}{1} (move to save pos.)", probingAxis[probingAxisIndex], probingSave[probingAxisIndex]);
                        lblEFProgress.Text = tmpEF;
                    }
                    angle = probePosOld.AngleTo(probePosNew);
                    angle = -angle;
                    if (angle < -90)
                        angle += 180;
                    if (probeX)
                        angle = (angle - 90);
                    //         else if (probeY)
                    //             angle = -angle;
                    tBAngle.Text = string.Format("{0:0.00}", angle);
                }
                else if (probingAction == probingMode.centerFinder)
                {
                    probingValues[probingValuesIndex++] = probePos;

                    if (probingValuesIndex == 2)    // calc center and apply
                    {
                        double centerX = (probingValues[0].X + probingValues[1].X) / 2;
                        sendCommandEvent(new CmdEventArgs((string.Format("{0} X{1}", CoordCommand, centerX).Replace(',', '.'))));
                        if (rBCF1.Checked)
                            stateCommands[4] = string.Format("G90 G00 X{0};", 0);
                        else
                            stateCommands[9] = string.Format("G90 G00 X{0};", 0);

                        lblCFStatus.Text = Localization.getString("probingSetCenter") + " X";//"Set center X";
                        lblCFProgress.Text = string.Format("Ok X:{0}", probePos.X);
                    }

                    if (probingValuesIndex > 3)
                    {   //double centerX = (probingValues[0].X + probingValues[1].X) / 2;
                        double centerY = (probingValues[2].Y + probingValues[3].Y) / 2;
                        sendCommandEvent(new CmdEventArgs((string.Format("{0} Y{1}", CoordCommand, centerY).Replace(',', '.'))));
                        if (rBCF1.Checked)
                            stateCommands[9] = string.Format("G90 G00 Y{0};", 0);
                        else
                            stateCommands[20] = string.Format("G90 G00 Y{0};", 0);
                        lblCFStatus.Text = Localization.getString("probingSetCenter") + " Y";//"Set center Y";
                        lblCFProgress.Text = string.Format("Ok Y:{0}", probePos.Y);
                    }
                }
                else if (probingAction == probingMode.toolLenght)
                {
                    sendCommandEvent(new CmdEventArgs(string.Format("G43.1 Z{0}", probePos.Z)));
                    if (cBSetCoordTL.Checked)
                        sendCommandEvent(new CmdEventArgs(  (string.Format("G92 Z{0}", (grbl.posMachine.Z - probePos.Z + (double)nUDOffsetZ.Value)).Replace(',', '.') )));
                    cBSetCoordTL.Checked = false;
                    lblTLProgress.Text = string.Format("Ok Z:{0}", probePos.Z);
                }

                probePosOld = probePosNew;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updateProgress)
            {
                if (probingAction == probingMode.edgeFinder)
                {
                    progressBarEF.Increment(1);
                    lblEFProgress.Text = string.Format("{0:0.00} units", (progressBarEF.Maximum - progressBarEF.Value) * nUDProbeFeed.Value / 600);
                    if (progressBarEF.Value >= probingTime)
                    {
                        lblEFProgress.Text = Localization.getString("probingTimeOut");// "Time out";
                        updateProgress = false;
                    }
                }
                else if (probingAction == probingMode.centerFinder)
                {
                    progressBarCF.Increment(1);
                    lblCFProgress.Text = string.Format("{0:0.00} units", (progressBarCF.Maximum - progressBarCF.Value) * nUDProbeFeed.Value / 600);
                    if (progressBarCF.Value >= probingTime)
                    {
                        lblCFProgress.Text = Localization.getString("probingTimeOut");// "Time out";
                        updateProgress = false;
                    }
                }
                else if (probingAction == probingMode.toolLenght)
                {
                    progressBarTL.Increment(1);
                    lblTLProgress.Text = string.Format("{0:0.00} units", (progressBarTL.Maximum - progressBarTL.Value) * nUDProbeFeed.Value / 600);
                    if (progressBarTL.Value >= probingTime)
                    {
                        lblTLProgress.Text = Localization.getString("probingTimeOut");// "Time out";
                        updateProgress = false;
                    }
                }
            }
            if (isIdle)
            {
                if (probingMoveSave.Length > 2)
                {
                    sendCommandEvent(new CmdEventArgs(probingMoveSave.Replace(',', '.')));
                    probingMoveSave = "";
                }
                else if (probingCount >= 0)
                {   if (probingAction == probingMode.edgeFinder)
                        isIdle = stateMachineEF();
                    else if (probingAction == probingMode.centerFinder)
                        isIdle = stateMachineCF();
                    else if (probingAction == probingMode.toolLenght)
                        isIdle = stateMachineTL();
                }

        //        if (probingAction == probingMode.noProbing)
        //        { lblEFProgress.Text = ""; lblCFProgress.Text = ""; lblTLProgress.Text = ""; }
            }
        }

        private bool stateMachineEF()
        {
            bool goon = false;
            switch (probingCount)
            {
                case 0:
                case 1:          // X was performed
                    if (probingStep2.Length > 2)
                        sendCommandEvent(new CmdEventArgs(probingStep2.Replace(',', '.')));   // move in front of next edge Y
                    else
                        goon = true;
                    break;
                case 2:    // Y was performed
                    if (!probeAxisY())
                    { if (probingStep3.Length > 2)
                            sendCommandEvent(new CmdEventArgs(probingStep3.Replace(',', '.')));   // move in front of next edge Z
                        else
                            goon = true;
                        probingCount++;
                    }
                    break;
                case 3:
                    if (probingStep3.Length > 2)
                        sendCommandEvent(new CmdEventArgs(probingStep3.Replace(',', '.')));
                    else
                        goon = true;
                    break;
                case 4:
                    if (cBZProbing.Checked)
                        probeAxisZ();
                    else
                        probingFinishEF();
                    break;
                case 5:
                    probingFinishEF();
                    probingCount = -2;
                    break;
            }
            probingCount++;
            return goon;
        }

        private bool stateMachineCF()
        {
            sendCommandEvent(new CmdEventArgs(stateCommands[probingCount].Replace(',', '.')));
            if (rBCF1.Checked)
            {
                if (probingCount==3)
                    setProgressCF(nUDWorkpieceDiameter.Value);
                if (new[] {5, 7 }.Contains(probingCount))
                {   setProgressCF(nUDWorkpieceDiameter.Value);
                    lblEFProgressInfo.Text = Localization.getString("probingProbingOn") + " Y"; //"Probing on X";
                }

                if (probingCount == 8)
                    lblCFStatus.Text = Localization.getString("probingCenter");// "Move to center";

                if (probingCount > 8)
                {   probingFinishCF();
         //           lblCFProgress.Text = Localization.getString("probingFinish");//"Finish";
                }
            }
            else
            {

                if (probingCount == 6)
                    setProgressCF(nUDWorkpieceDiameter.Value);
                if (new[] {12, 17 }.Contains(probingCount))
                {   setProgressCF(nUDWorkpieceDiameter.Value);
                    lblEFProgressInfo.Text = Localization.getString("probingProbingOn") + " Y"; //"Probing on X";
                }

                if (probingCount == 19)
                    lblCFStatus.Text = Localization.getString("probingCenter");//"Move to center";

                if (probingCount > 19)
                    probingFinishCF();
            }
            probingCount++;
            return false;
        }

        private bool stateMachineTL()
        {
            sendCommandEvent(new CmdEventArgs(stateCommands[probingCount].Replace(',', '.')));

            if (probingCount > 1)
            {
                probingFinishTL();
       //         lblTLProgress.Text = Localization.getString("probingFinish");//"Finish";
            }
            probingCount++;
            return false;
        }

        RadioButton cBold = null, cBnow = null;
        private void rBEF_CheckedCHanged(object sender, EventArgs e)
        {
            btnStartEF.Enabled = true;
            cBnow = ((RadioButton)sender);
            setImage(cBnow, true);
            if (cBold != null)
            {
                setImage(cBold, false);
                setText(cBold, "");
            }
            cBold = cBnow;

            setText(cBnow, cBZProbing.Checked ? "Z" : "");

            setNudEnable(0, "134679".Contains(cBnow.Name.Substring(2)));
            setNudEnable(1, "123789".Contains(cBnow.Name.Substring(2)));
            setNudEnable(2, (cBZProbing.Checked || cBnow.Name == "rB5"));
        }
        private void rBCF_CheckedCHanged(object sender, EventArgs e)
        {
            btnStartCF.Enabled = true;
            bool isActive = (((RadioButton)sender).Name == "rBCF1");
            rBCF1.Image = isActive ? Properties.Resources.cfOn1 : Properties.Resources.cfOff1;
            rBCF2.Image = !isActive ? Properties.Resources.cfOn2 : Properties.Resources.cfOff2;

            setNudEnable(0, true);
            setNudEnable(1, true);
            setNudEnable(2, !isActive);
        }



        #region edgeFinder
        private void btnStartEF_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            probeX = false; probeY = false;
            probingCount = 1;
            probingStep2 = "";
            probingStep3 = "";
            probingMoveSave = "";
            lblEFStatus.Text = "";
            ZatX = 0; ZatY = 0;

            fillStrings();
            sendCommandEvent(new CmdEventArgs("G91"));

            if (!probeAxisX())
            {
                probingCount += 2;
                if (!probeAxisY())
                {
                    probingCount += 2;
                    probeAxisZ();
                }
            }
            timer1.Enabled = true;
            isIdle = false;
            btnCancelEF.Enabled = true;
            btnStartEF.Enabled = false;
            tBAngle.Text = "0";
            probingAction = probingMode.edgeFinder;
            setRBEnable(cBnow, false);
        }
        private void btnCancelEF_Click(object sender, EventArgs e)
        {
            probingFinishEF();
            lblEFProgressInfo.Text = Localization.getString("probingCancel1"); //"Probing canceled";
            if (!isIdle)
                lblEFProgress.Text = Localization.getString("probingCancel2"); //"Process last command";
        }
        private void probingFinishEF()
        {
            timer1.Enabled = false;
            probeX = false; probeY = false;
            probingCount = 1;
            probingStep2 = "";
            probingStep3 = "";
            probingMoveSave = "";
            btnCancelEF.Enabled = false;
            btnStartEF.Enabled = true;
            //           btnCancelCF.Enabled = false;
            //           btnStartCF.Enabled = true;
            probingAction = probingMode.noProbing;
            setRBEnable(cBnow, true);
        }

        private void setProgressEF(decimal maxTravel)
        {
            progressDistance = maxTravel;
            probingTime = (int)((maxTravel / nUDProbeFeed.Value) * 60 * 10);    // distance(mm) / speed(mm/min) *60(sec) 100 ms
            progressBarEF.Minimum = 0;
            progressBarEF.Value = 1;
            progressBarEF.Maximum = (int)probingTime;
            updateProgress = true;
        }

        private void cBZProbing_CheckedChanged(object sender, EventArgs e)
        {
            setNudEnable(2, (cBZProbing.Checked || cBnow.Name == "rB5"));
            if ((cBZProbing.Checked) && (cBnow != null))
                setText(cBnow, "Z");
            else
                setText(cBnow, "");
        }

        private void setText(RadioButton tmp, string txt)
        {
            if (tmp.Name != "rB5")
                tmp.Text = txt;
        }

        private void fillStrings()
        {
            string probeString = "";
            if ((rB1.Checked) || (rB4.Checked) || (rB7.Checked))        // probe from left to right
            { probeString = ProbeToward_E(); probeX = true; ZatX = nUDDiameter.Value; }
            else if ((rB3.Checked) || (rB6.Checked) || (rB9.Checked))   // probe from right to left
            { probeString = ProbeToward_W(); probeX = true; ZatX = -nUDDiameter.Value; }
            probingAxisX = probeString;

            probeString = "";
            if ((rB1.Checked) || (rB2.Checked) || (rB3.Checked))
            { probeString = ProbeToward_S(); probeY = true; ZatY = -nUDDiameter.Value; }
            else if ((rB7.Checked) || (rB8.Checked) || (rB9.Checked))
            { probeString = ProbeToward_N(); probeY = true; ZatY = nUDDiameter.Value; }
            probingAxisY = probeString;


            decimal moveY = nUDDiameter.Value + nUDProbeTravelY.Value;
            decimal moveX = nUDDiameter.Value + nUDProbeSaveX.Value;

            // X is save after 1st probing, move to Y pos for Y probing
            if ((rB1.Checked) || (rB3.Checked)) // adjust for -Y probing
            { probingStep2 += string.Format("G91 G00 Y{0};", moveY); }
            else if ((rB7.Checked) || (rB9.Checked)) // adjust for +Y probing
            { probingStep2 += string.Format("G91 G00 Y-{0};", moveY); }

            // move to X pos where Y probing is possible
            if ((rB1.Checked) || (rB7.Checked)) // adjust for  probing
            { probingStep2 += string.Format("G91 G00 X{0};", moveX); }
            else if ((rB3.Checked) || (rB9.Checked)) // adjust for  probing
            { probingStep2 += string.Format("G91 G00 X-{0};", moveX); }

            if (cBZProbing.Checked)
            {
                probingStep3 += string.Format("G91 G00 Z{0};", (nUDProbeTravelZ.Value));
                probingStep3 += "G90 G00 ";
                if (probeX) probingStep3 += string.Format("X{0} ", ZatX);
                if (probeY) probingStep3 += string.Format("Y{0};", ZatY);
            }
        }

        private decimal ZatX = 0, ZatY = 0;
        private bool probeAxisX()
        {
            if (probeX)
            {
                probingAxisIndex = 0;
                setProgressEF(nUDProbeTravelX.Value);
                sendCommandEvent(new CmdEventArgs(probingAxisX.Replace(',', '.')));
                lblEFProgressInfo.Text = Localization.getString("probingProbingOn") + " X"; //"Probing on X";
                lblEFProgress.Text = "";
            }
            return probeX;
        }
        private bool probeAxisY()
        {
            if (probeY)
            {
                probingAxisIndex = 1;
                setProgressEF(nUDProbeTravelY.Value);
                sendCommandEvent(new CmdEventArgs(probingAxisY.Replace(',', '.')));
                lblEFProgressInfo.Text = Localization.getString("probingProbingOn") + " Y"; //"Probing on Y";
                lblEFProgress.Text = "";
            }
            return probeY;
        }
        private bool probeAxisZ()
        {
            probingAxisIndex = 2;
            string probeString = "";
            probeString = ProbeToward_Z();
            sendCommandEvent(new CmdEventArgs(probeString.Replace(',', '.')));
            lblEFProgressInfo.Text = Localization.getString("probingProbingOn") + " Z"; //"Probing on Z";
            lblEFProgress.Text = "";
            return probeY;
        }

        private string ProbeToward_E()    // probe towards direction east
        {
            probingOffset[0] = -nUDOffsetX.Value;
            probingSave[0] = -nUDProbeSaveX.Value;
            return string.Format("{0} X{1} F{2};", ProbeCommand, nUDProbeTravelX.Value, nUDProbeFeed.Value);
        }
        private string ProbeToward_W()    // probe towards direction west
        {
            probingOffset[0] = nUDOffsetX.Value;
            probingSave[0] = nUDProbeSaveX.Value;
            return string.Format("{0} X-{1} F{2};", ProbeCommand, nUDProbeTravelX.Value, nUDProbeFeed.Value);
        }

        private string ProbeToward_N()    // probe towards direction north
        {
            probingOffset[1] = -nUDOffsetY.Value;
            probingSave[1] = -nUDProbeSaveY.Value;
            return string.Format("{0} Y{1} F{2};", ProbeCommand, nUDProbeTravelY.Value, nUDProbeFeed.Value);
        }
        private string ProbeToward_S()    // probe towards direction south
        {
            probingOffset[1] = nUDOffsetY.Value;
            probingSave[1] = nUDProbeSaveY.Value;
            return string.Format("{0} Y-{1} F{2};", ProbeCommand, nUDProbeTravelY.Value, nUDProbeFeed.Value);
        }

        private string ProbeToward_Z()    // probe towards direction east
        {
            probingOffset[2] = nUDOffsetZ.Value;
            probingSave[2] = nUDProbeSaveZ.Value;
            setProgressEF(nUDProbeTravelZ.Value);
            return string.Format("{0} Z-{1} F{2};", ProbeCommand, nUDProbeTravelZ.Value, nUDProbeFeed.Value);
        }

        public void btnGetAngleEF_Click(object sender, EventArgs e)
        { }
        public double getAngle
        {
            set { }
            get { return this.angle; }
        }

        private void setImage(RadioButton tmp, bool on)
        {
            if (tmp.Name == "rB1") { tmp.Image = on ? Properties.Resources.efOn1 : Properties.Resources.efOff1; }
            if (tmp.Name == "rB2") { tmp.Image = on ? Properties.Resources.efOn2 : Properties.Resources.efOff2; }
            if (tmp.Name == "rB3") { tmp.Image = on ? Properties.Resources.efOn3 : Properties.Resources.efOff3; }
            if (tmp.Name == "rB4") { tmp.Image = on ? Properties.Resources.efOn4 : Properties.Resources.efOff4; }
            if (tmp.Name == "rB5") { tmp.Image = on ? Properties.Resources.efOn5 : Properties.Resources.efOff5; }
            if (tmp.Name == "rB6") { tmp.Image = on ? Properties.Resources.efOn6 : Properties.Resources.efOff6; }
            if (tmp.Name == "rB7") { tmp.Image = on ? Properties.Resources.efOn7 : Properties.Resources.efOff7; }
            if (tmp.Name == "rB8") { tmp.Image = on ? Properties.Resources.efOn8 : Properties.Resources.efOff8; }
            if (tmp.Name == "rB9") { tmp.Image = on ? Properties.Resources.efOn9 : Properties.Resources.efOff9; }
        }
        private void setRBEnable(RadioButton tmp, bool en)
        {
            rB1.Enabled = en;
            rB2.Enabled = en;
            rB3.Enabled = en;
            rB4.Enabled = en;
            rB5.Enabled = en;
            rB6.Enabled = en;
            rB7.Enabled = en;
            rB8.Enabled = en;
            rB9.Enabled = en;
            if (tmp != null)
                tmp.Enabled = true;
            cBZProbing.Enabled = en;
            gBHardware.Enabled = en;
            gBMovement.Enabled = en;
        }

        #endregion


        #region centerFinder
        private void btnStartCF_Click(object sender, EventArgs e)
        {
            btnCancelCF.Enabled = true;
            btnStartCF.Enabled = false;
            lblEFProgressInfo.Text = Localization.getString("probingProbingOn") + " X"; //"Probing on X";
            lblCFProgress.Text = "";
            lblCFStatus.Text = "";
            probingValuesIndex = 0;
            probingAction = probingMode.centerFinder;
            setRBEnable(cBnow, false);

            for (int i = 0; i < stateCommandsMax; i++)
                stateCommands[i] = "";

            probeStartMachine = grbl.posMachine;
            probingValuesIndex = 0;
            int k = 1;
            if (rBCF1.Checked)
            {
                stateCommands[k++] = string.Format("{0} X-{1} F{2};", ProbeCommand, nUDWorkpieceDiameter.Value, nUDProbeFeed.Value);    // index=1 will be send below
                stateCommands[k++] = string.Format("G53 G00 X{0};", probeStartMachine.X);
                stateCommands[k++] = string.Format("{0} X{1} F{2};", ProbeCommand, nUDWorkpieceDiameter.Value, nUDProbeFeed.Value);
                stateCommands[k++] = string.Format("G53 G00 X{0};", probeStartMachine.X);                                               // replace 4
                stateCommands[k++] = string.Format("{0} Y-{1} F{2};", ProbeCommand, nUDWorkpieceDiameter.Value, nUDProbeFeed.Value);   
                stateCommands[k++] = string.Format("G53 G00 Y{0};", probeStartMachine.Y);
                stateCommands[k++] = string.Format("{0} Y{1} F{2};", ProbeCommand, nUDWorkpieceDiameter.Value, nUDProbeFeed.Value);
                stateCommands[k++] = string.Format("G53 G00 Y{0};", probeStartMachine.Y);
            }
            else
            {
                stateCommands[k++] = string.Format("{0} X{1} F{2};", ProbeCommand, nUDProbeTravelX.Value, nUDProbeFeed.Value);  // probe 0
                stateCommands[k++] = string.Format("G91 G00 X-{0};", nUDProbeSaveX.Value);
                stateCommands[k++] = string.Format("Z{0};", nUDProbeTravelZ.Value);
                stateCommands[k++] = string.Format("X{0};", (nUDWorkpieceDiameter.Value + 2 * nUDProbeSaveX.Value + nUDDiameter.Value));
                stateCommands[k++] = string.Format("Z-{0}", nUDProbeTravelZ.Value);
                // k=6
                stateCommands[k++] = string.Format("{0} X-{1} F{2};", ProbeCommand, nUDProbeTravelX.Value, nUDProbeFeed.Value); // probe 1
                stateCommands[k++] = string.Format("G91 G00 X{0};", nUDProbeSaveX.Value);
                stateCommands[k++] = string.Format("Z{0};", nUDProbeTravelZ.Value);
                stateCommands[k++] = string.Format("X-{0};", (nUDProbeSaveX.Value + nUDWorkpieceDiameter.Value/2));         // 9 will be replaced
                stateCommands[k++] = string.Format("G91 G00 Y-{0};", (nUDProbeTravelY.Value + nUDWorkpieceDiameter.Value / 2));
                stateCommands[k++] = string.Format("Z-{0}", nUDProbeTravelZ.Value);
                // 12
                stateCommands[k++] = string.Format("{0} Y{1} F{2};", ProbeCommand, nUDWorkpieceDiameter.Value, nUDProbeFeed.Value); // probe 2
                stateCommands[k++] = string.Format("G91 G00 Y-{0};", nUDProbeSaveY.Value);
                stateCommands[k++] = string.Format("Z{0};", nUDProbeTravelZ.Value);
                stateCommands[k++] = string.Format("Y{0};", (nUDWorkpieceDiameter.Value + 2 * nUDProbeSaveY.Value + nUDDiameter.Value));
                stateCommands[k++] = string.Format("Z-{0}", nUDProbeTravelZ.Value);
                // 17
                stateCommands[k++] = string.Format("{0} Y-{1} F{2};", ProbeCommand, nUDWorkpieceDiameter.Value, nUDProbeFeed.Value);    // probe 3
                stateCommands[k++] = string.Format("G91 G00 Y{0};", nUDProbeSaveY.Value);
                stateCommands[k++] = string.Format("Z{0};", nUDProbeTravelZ.Value);
                stateCommands[k++] = string.Format("G53 G00 Y{0};", probeStartMachine.Y);
            }

            probingCount = 1;
            sendCommandEvent(new CmdEventArgs(stateCommands[probingCount++].Replace(',', '.')));
            setProgressCF(nUDWorkpieceDiameter.Value);
            isIdle = false;
            timer1.Enabled = true;
        }
        private void btnCancelCF_Click(object sender, EventArgs e)
        {
            probingFinishCF();
            lblCFProgressInfo.Text = Localization.getString("probingCancel1"); //"Probing canceled";
            if (!isIdle)
                lblCFProgress.Text = Localization.getString("probingCancel2"); //"Process last command";
        }
        private void probingFinishCF()
        {
            timer1.Enabled = false;
            probeX = false; probeY = false;
            probingCount = 1;
            btnCancelCF.Enabled = false;
            btnStartCF.Enabled = true;
            probingAction = probingMode.noProbing;
            setRBEnable(cBnow, true);
        }

        private void setProgressCF(decimal maxTravel)
        {
            progressDistance = maxTravel;
            probingTime = (int)((maxTravel / nUDProbeFeed.Value) * 60 * 10);    // distance(mm) / speed(mm/min) *60(sec) 100 ms
            progressBarCF.Minimum = 0;
            progressBarCF.Value = 1;
            progressBarCF.Maximum = (int)probingTime;
            updateProgress = true;
        }

        #endregion

        #region toolLength
        private void btnStartTL_Click(object sender, EventArgs e)
        {
            lblEFProgressInfo.Text = Localization.getString("probingProbingOn") + " Z"; //"Probing on X";
            lblTLProgress.Text = "";
            lblTLStatus.Text = "";
            probingValuesIndex = 0;
            probingAction = probingMode.toolLenght;
            setRBEnable(cBnow, false);

            for (int i = 0; i < stateCommandsMax; i++)
                stateCommands[i] = "";

            probeStartMachine = grbl.posMachine;
            probingValuesIndex = 0;
            int k = 1;
            stateCommands[k++] = string.Format("{0} Z-{1} F{2};", ProbeCommand, nUDProbeTravelZ.Value, nUDProbeFeed.Value);    // index=1 will be send below
            stateCommands[k++] = string.Format("G53 G00 Z{0};", probeStartMachine.Z);
            //        stateCommands[k++] = string.Format("G91 G00 Z{0};", nUDProbeTravelZ.Value);

            probingCount = 1;
            sendCommandEvent(new CmdEventArgs(stateCommands[probingCount++].Replace(',', '.')));
            setProgressTL(nUDProbeTravelZ.Value);
            isIdle = false;
            timer1.Enabled = true;
        }
        private void probingFinishTL()
        {
            timer1.Enabled = false;
            probeX = false; probeY = false;
            probingCount = 1;
            probingAction = probingMode.noProbing;
            setRBEnable(cBnow, true);
        }

        private void setProgressTL(decimal maxTravel)
        {
            progressDistance = maxTravel;
            probingTime = (int)((maxTravel / nUDProbeFeed.Value) * 60 * 10);    // distance(mm) / speed(mm/min) *60(sec) 100 ms
            progressBarTL.Minimum = 0;
            progressBarTL.Value = 1;
            progressBarTL.Maximum = (int)probingTime;
            updateProgress = true;
        }
        private void btnClearTL_Click(object sender, EventArgs e)
        {   sendCommandEvent(new CmdEventArgs("G49"));   }

        #endregion


        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void sendCommandEvent(CmdEventArgs e)
        {
            EventHandler<CmdEventArgs> handler = RaiseCmdEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        private void ControlProbing_Click(object sender, EventArgs e)
        {
            setNudEnable(0, true);
            setNudEnable(1, true);
            setNudEnable(2, true);
        }

        private void rBProbeCoord1_CheckedChanged(object sender, EventArgs e)
        {
            useG92 = false;
            if (rBProbeCoord1.Checked)
                CoordCommand = "G10 L2 P0";        // G92
            else
            {   CoordCommand = "G92";        // G92
                useG92 = true;
            }
        }

        private void btnProbeCoordClear_Click(object sender, EventArgs e)
        {   sendCommandEvent(new CmdEventArgs("G92.1"));  }

        private void btnSaveTL_Click(object sender, EventArgs e)
        {   sendCommandEvent(new CmdEventArgs(string.Format("G90 G00 Z{0}",nUDProbeSaveZ.Value).Replace(',', '.'))); }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {   setNudEnable(0, true);
                setNudEnable(1, true);
                setNudEnable(2, true);
            }
            if (tabControl1.SelectedIndex == 1)
            {   setNudEnable(0, true);
                setNudEnable(1, true);
                setNudEnable(2, false);
            }
            if (tabControl1.SelectedIndex == 2)
            {   setNudEnable(0, false);
                setNudEnable(1, false);
                setNudEnable(2, true);
            }
        }

        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (probingAction != probingMode.noProbing)
                e.Cancel = true;
        }

        private void setNudEnable(int axis, bool en)
        {
            if (axis == 0)
            {
                nUDOffsetX.Enabled = en;
                nUDProbeFinalX.Enabled = en;
                nUDProbeSaveX.Enabled = en;
                nUDProbeTravelX.Enabled = en;
            }
            if (axis == 1)
            {
                nUDOffsetY.Enabled = en;
                nUDProbeFinalY.Enabled = en;
                nUDProbeSaveY.Enabled = en;
                nUDProbeTravelY.Enabled = en;
            }
            if (axis == 2)
            {
                nUDOffsetZ.Enabled = en;
                nUDProbeFinalZ.Enabled = en;
                nUDProbeSaveZ.Enabled = en;
                nUDProbeTravelZ.Enabled = en;
            }
        }
    }
}
