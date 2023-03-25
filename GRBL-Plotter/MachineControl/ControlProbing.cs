/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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
 * 2019-11-24 save 'checked' properties for edge-Z, edge-center, coordinate G10
 * 2020-08-23 set min. offset to 0 (for touch plate)
 * 2021-01-05 clear progress bar when finished line 490
 * 2021-04-27 adapt save position, if probe plate dimension is higher
 * 2021-07-14 code clean up / code quality
 * 2021-12-22 check if is connected to grbl before sending code - 452, 706, 803
 * 2022-01-07 SetProgressxx .Value=0
 * 2023-03-15 l:900 f:ControlProbing_Click keep nUD..Z disabled
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlProbing : Form
    {
        private enum ProbingMode { noProbing, edgeFinder, centerFinder, toolLenght }

        //    private ArrayList gcodeString = new ArrayList();
        private const string ProbeCommand = "G91 G38.3";
        private string CoordCommand = "G10 L2 P0";        // G92
        private bool useG92 = false;
        private int probingAxisIndex = 0;
        private int probingTime = 5;
        private readonly decimal[] probingOffset = { 0, 0, 0 };      // touch plate
        private readonly decimal[] probingSave = { 1, 1, 1 };
        private int probingValuesIndex = 0;
        private readonly string[] probingAxis = { "X", "Y", "Z" };
        private const int stateCommandsMax = 40;
        private readonly string[] stateCommands = new string[stateCommandsMax];


        private readonly XyzPoint[] probingValues = { new XyzPoint(), new XyzPoint(), new XyzPoint(), new XyzPoint(), new XyzPoint() };
        private XyPoint probePosOld = new XyPoint(0, 0);
        private XyPoint probePosNew = new XyPoint(0, 0);
        private XyzPoint probePos = new XyzPoint();
        private XyzPoint probeStartMachine = new XyzPoint();

        private string probingStep2 = "";
        private string probingStep3 = "";
        private string probingMoveSave = "";
        private string probingAxisX = "";
        private string probingAxisY = "";
        //        private string probingAxisZ = "";

        private bool probeX = false, probeY = false;
        private int probingCount = 0;
        private bool updateProgress = false;
        //    private decimal progressDistance = 1;
        private double angle = 0;

        private ProbingMode probingAction = ProbingMode.noProbing;
        private GrblState grblStateNow = GrblState.run;
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

            if (!Properties.Settings.Default.probingCoordG10)
                rBProbeCoord2.Checked = true;

            NudOffset_ValueChanged(sender, e);  // set minimum values of Save position
        }

        internal GrblState SetGrblSaState
        {
            set
            {
                grblStateNow = value;
                Logger.Trace("setGrblSaState {0}", grblStateNow);
                if (grblStateNow == GrblState.idle)
                    isIdle = true;
                else if (grblStateNow == GrblState.alarm)
                {
                    lblEFProgress.Text = "ALARM";
                    updateProgress = false;
                    probingCount = -1;
                }
                else if (grblStateNow == GrblState.reset)
                {
                    lblEFProgress.Text = "";
                    lblCFProgress.Text = "";
                    btnCancelEF.Enabled = false;
                    btnCancelCF.Enabled = false;
                    ProbingFinishCF();
                    ProbingFinishEF();
                    ProbingFinishTL();
                }
            }
            get { return grblStateNow; }
        }

        /***************************************************************************/
        // get height information from main-GUI OnRaisePosEvent line 192
        /***************************************************************************/
        internal XyzPoint SetPosProbe
        {
            set
            {
                probePos = value;   // grbl.getCoord("PRB");
                Logger.Trace("setPosProbe updateProgress:{0}", updateProgress);

                if (updateProgress)
                {
                    if (Grbl.GetPRBStatus())
                    {
                        lblEFProgress.Text = "";
                        lblCFProgress.Text = "";
                        lblTLProgress.Text = "";
                    }
                    else
                    {
                        if (probingAction == ProbingMode.edgeFinder)
                        {
                            ProbingFinishEF();
                            lblEFProgress.Text = Localization.GetString("probingFail1");//"Fail: no contact";
                            lblEFStatus.Text = Localization.GetString("probingFail2");//"Cancel probing";
                        }
                        else if (probingAction == ProbingMode.centerFinder)
                        {
                            ProbingFinishCF();
                            lblCFProgress.Text = Localization.GetString("probingFail1");//"Fail: no contact";
                            lblCFStatus.Text = Localization.GetString("probingFail1");//"Cancel probing";
                        }
                        else if (probingAction == ProbingMode.toolLenght)
                        {
                            ProbingFinishCF();
                            lblCFProgress.Text = Localization.GetString("probingFail1");//"Fail: no contact";
                        }
                    }
                }
                probingMoveSave = "";
                updateProgress = false;

                probePosNew.X = probePos.X; probePosNew.Y = probePos.Y;

                double tmpProbePos = probePos.X - (double)probingOffset[0];
                string tmpEF = string.Format("Ok X:{0}", probePos.X);

                //   double angle = 
                if (probingAxisIndex == 1) { tmpProbePos = probePos.Y - (double)probingOffset[1]; tmpEF = string.Format("Ok Y:{0}", probePos.Y); }
                if (probingAxisIndex == 2) { tmpProbePos = probePos.Z - (double)probingOffset[2]; tmpEF = string.Format("Ok Z:{0}", probePos.Z); }

                if (useG92)
                {
                    tmpProbePos = (Grbl.posMachine.X - probePos.X) + (double)probingOffset[0];
                    if (probingAxisIndex == 1) tmpProbePos = (Grbl.posMachine.Y - probePos.Y) + (double)probingOffset[1];
                    if (probingAxisIndex == 2) tmpProbePos = (Grbl.posMachine.Z - probePos.Z) + (double)probingOffset[2];
                }

                /*************** Edge finder ************************************************************/
                if (probingAction == ProbingMode.edgeFinder)
                {
                    if ((probingAxisIndex >= 0) && (probingAxisIndex <= 2))
                    {
                        SendCommandEvent(new CmdEventArgs((string.Format("{0} {1}{2}", CoordCommand, probingAxis[probingAxisIndex], tmpProbePos).Replace(',', '.'))));
                        probingMoveSave = string.Format("G90 G00 {0}{1} (move to save pos.)", probingAxis[probingAxisIndex], probingSave[probingAxisIndex]);    // will be sent in timer1_Tick
                        lblEFProgress.Text = tmpEF;
                    }
                    angle = probePosOld.AngleTo(probePosNew);
                    angle = -angle;
                    if (angle < -90)
                        angle += 180;
                    if (probeX)
                        angle -= 90;
                    //         else if (probeY)
                    //             angle = -angle;
                    tBAngle.Text = string.Format("{0:0.00}", angle);
                }
                else if (probingAction == ProbingMode.centerFinder)
                {
                    probingValues[probingValuesIndex++] = probePos;

                    if (probingValuesIndex == 2)    // calc center and apply
                    {
                        double centerX = (probingValues[0].X + probingValues[1].X) / 2;
                        SendCommandEvent(new CmdEventArgs((string.Format("{0} X{1}", CoordCommand, centerX).Replace(',', '.'))));
                        if (rBCF1.Checked)
                            stateCommands[4] = string.Format("G90 G00 X{0};", 0);
                        else
                            stateCommands[9] = string.Format("G90 G00 X{0};", 0);

                        lblCFStatus.Text = Localization.GetString("probingSetCenter") + " X";//"Set center X";
                        lblCFProgress.Text = string.Format("Ok X:{0}", probePos.X);
                    }

                    if (probingValuesIndex > 3)
                    {   //double centerX = (probingValues[0].X + probingValues[1].X) / 2;
                        double centerY = (probingValues[2].Y + probingValues[3].Y) / 2;
                        SendCommandEvent(new CmdEventArgs((string.Format("{0} Y{1}", CoordCommand, centerY).Replace(',', '.'))));
                        if (rBCF1.Checked)
                            stateCommands[9] = string.Format("G90 G00 Y{0};", 0);
                        else
                            stateCommands[20] = string.Format("G90 G00 Y{0};", 0);
                        lblCFStatus.Text = Localization.GetString("probingSetCenter") + " Y";//"Set center Y";
                        lblCFProgress.Text = string.Format("Ok Y:{0}", probePos.Y);
                    }
                }
                else if (probingAction == ProbingMode.toolLenght)
                {
                    SendCommandEvent(new CmdEventArgs(string.Format("G43.1 Z{0}", probePos.Z)));
                    if (cBSetCoordTL.Checked)
                        SendCommandEvent(new CmdEventArgs((string.Format("G92 Z{0}", (Grbl.posMachine.Z - probePos.Z + (double)nUDOffsetZ.Value)).Replace(',', '.'))));
                    cBSetCoordTL.Checked = false;
                    lblTLProgress.Text = string.Format("Ok Z:{0}", probePos.Z);
                }

                probePosOld = probePosNew;
            }
            get { return probePos; }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (updateProgress)
            {
                if (probingAction == ProbingMode.edgeFinder)
                {
                    progressBarEF.Increment(1);
                    lblEFProgress.Text = string.Format("{0:0.00} units", (progressBarEF.Maximum - progressBarEF.Value) * nUDProbeFeed.Value / 600);
                    if (progressBarEF.Value >= probingTime)
                    {
                        lblEFProgress.Text = Localization.GetString("probingTimeOut");// "Time out";
                        updateProgress = false;
                    }
                }
                else if (probingAction == ProbingMode.centerFinder)
                {
                    progressBarCF.Increment(1);
                    lblCFProgress.Text = string.Format("{0:0.00} units", (progressBarCF.Maximum - progressBarCF.Value) * nUDProbeFeed.Value / 600);
                    if (progressBarCF.Value >= probingTime)
                    {
                        lblCFProgress.Text = Localization.GetString("probingTimeOut");// "Time out";
                        updateProgress = false;
                    }
                }
                else if (probingAction == ProbingMode.toolLenght)
                {
                    progressBarTL.Increment(1);
                    lblTLProgress.Text = string.Format("{0:0.00} units", (progressBarTL.Maximum - progressBarTL.Value) * nUDProbeFeed.Value / 600);
                    if (progressBarTL.Value >= probingTime)
                    {
                        lblTLProgress.Text = Localization.GetString("probingTimeOut");// "Time out";
                        updateProgress = false;
                        ProbingFinishTL();
                    }
                }
            }
            if (isIdle)
            {
                if (probingMoveSave.Length > 2)
                {
                    SendCommandEvent(new CmdEventArgs(probingMoveSave.Replace(',', '.')));
                    probingMoveSave = "";
                }
                else if (probingCount >= 0)
                {
                    if (probingAction == ProbingMode.edgeFinder)
                        isIdle = StateMachineEF();
                    else if (probingAction == ProbingMode.centerFinder)
                        isIdle = StateMachineCF();
                    else if (probingAction == ProbingMode.toolLenght)
                        isIdle = StateMachineTL();
                }

                //        if (probingAction == probingMode.noProbing)
                //        { lblEFProgress.Text = ""; lblCFProgress.Text = ""; lblTLProgress.Text = ""; }
            }
        }

        private bool StateMachineEF()
        {
            bool goon = false;
            Logger.Trace("stateMachineEF probingCount:{0}", probingCount);

            switch (probingCount)
            {
                case 0:
                case 1:          // X was performed
                    if (probingStep2.Length > 2)
                        SendCommandEvent(new CmdEventArgs(probingStep2.Replace(',', '.')));   // move in front of next edge Y
                    else
                        goon = true;
                    break;
                case 2:    // Y was performed
                    if (!ProbeAxisY())
                    {
                        if (probingStep3.Length > 2)
                            SendCommandEvent(new CmdEventArgs(probingStep3.Replace(',', '.')));   // move in front of next edge Z
                        else
                            goon = true;
                        probingCount++;
                    }
                    break;
                case 3:
                    if (probingStep3.Length > 2)
                        SendCommandEvent(new CmdEventArgs(probingStep3.Replace(',', '.')));
                    else
                        goon = true;
                    break;
                case 4:
                    if (cBZProbing.Checked)
                        ProbeAxisZ();
                    else
                        ProbingFinishEF();
                    break;
                case 5:
                    ProbingFinishEF();
                    probingCount = -2;
                    break;
            }
            probingCount++;
            return goon;
        }

        private bool StateMachineCF()
        {
            SendCommandEvent(new CmdEventArgs(stateCommands[probingCount].Replace(',', '.')));
            Logger.Trace("stateMachineCF probingCount:{0}", probingCount);
            if (rBCF1.Checked)
            {
                if (probingCount == 3)
                    SetProgressCF(nUDWorkpieceDiameter.Value);
                if (new[] { 5, 7 }.Contains(probingCount))
                {
                    SetProgressCF(nUDWorkpieceDiameter.Value);
                    lblEFProgressInfo.Text = Localization.GetString("probingProbingOn") + " Y"; //"Probing on X";
                }

                if (probingCount == 8)
                    lblCFStatus.Text = Localization.GetString("probingCenter");// "Move to center";

                if (probingCount > 8)
                {
                    ProbingFinishCF();
                    //           lblCFProgress.Text = Localization.getString("probingFinish");//"Finish";
                }
            }
            else
            {

                if (probingCount == 6)
                    SetProgressCF(nUDWorkpieceDiameter.Value);
                if (new[] { 12, 17 }.Contains(probingCount))
                {
                    SetProgressCF(nUDWorkpieceDiameter.Value);
                    lblEFProgressInfo.Text = Localization.GetString("probingProbingOn") + " Y"; //"Probing on X";
                }

                if (probingCount == 19)
                    lblCFStatus.Text = Localization.GetString("probingCenter");//"Move to center";

                if (probingCount > 19)
                    ProbingFinishCF();
            }
            probingCount++;
            return false;
        }

        private bool StateMachineTL()
        {
            SendCommandEvent(new CmdEventArgs(stateCommands[probingCount].Replace(',', '.')));

            if (probingCount > 1)
            {
                ProbingFinishTL();
                //         lblTLProgress.Text = Localization.getString("probingFinish");//"Finish";
            }
            probingCount++;
            return false;
        }

        RadioButton cBold = null, cBnow = null;
        private void RbEF_CheckedCHanged(object sender, EventArgs e)
        {
            btnStartEF.Enabled = true;
            cBnow = ((RadioButton)sender);
            SetImage(cBnow, true);
            if (cBold != null)
            {
                SetImage(cBold, false);
                SetText(cBold, "");
            }
            cBold = cBnow;

            SetText(cBnow, cBZProbing.Checked ? "Z" : "");

            SetNudEnable(0, "134679".Contains(cBnow.Name.Substring(2)));
            SetNudEnable(1, "123789".Contains(cBnow.Name.Substring(2)));
            SetNudEnable(2, (cBZProbing.Checked || cBnow.Name == "rB5"));
        }
        private void RbCF_CheckedCHanged(object sender, EventArgs e)
        {
            btnStartCF.Enabled = true;
            bool isActive = (((RadioButton)sender).Name == "rBCF1");
            rBCF1.Image = isActive ? Properties.Resources.cfOn1 : Properties.Resources.cfOff1;
            rBCF2.Image = !isActive ? Properties.Resources.cfOn2 : Properties.Resources.cfOff2;

            SetNudEnable(0, true);
            SetNudEnable(1, true);
            SetNudEnable(2, !isActive);
        }



        #region edgeFinder
        private void BtnStartEF_Click(object sender, EventArgs e)
        {
            if (!Grbl.isConnected)
            {
                MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Logger.Trace("Start Edge Finder");
            timer1.Enabled = false;
            probeX = false; probeY = false;
            probingCount = 1;
            probingStep2 = "";
            probingStep3 = "";
            probingMoveSave = "";
            lblEFStatus.Text = "";
            ZatX = 0; ZatY = 0;

            FillStrings();      // check which axis to probe, set probeX, probeY, CMD-Strings
            SendCommandEvent(new CmdEventArgs("G91"));

            if (!ProbeAxisX())          // if X, then Y and Z will started via Timer / stateMachine
            {
                probingCount += 2;
                if (!ProbeAxisY())      // if not X but Y, then Z will started via Timer / stateMachine
                {
                    probingCount += 2;
                    ProbeAxisZ();       // only Z
                }
            }
            timer1.Enabled = true;
            isIdle = false;
            btnCancelEF.Enabled = true;
            btnStartEF.Enabled = false;
            tBAngle.Text = "0";
            probingAction = ProbingMode.edgeFinder;
            SetRBEnable(cBnow, false);
        }
        private void BtnCancelEF_Click(object sender, EventArgs e)
        {
            ProbingFinishEF();
            lblEFProgressInfo.Text = Localization.GetString("probingCancel1"); //"Probing canceled";
            if (!isIdle)
                lblEFProgress.Text = Localization.GetString("probingCancel2"); //"Process last command";
        }
        private void ProbingFinishEF()
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
            probingAction = ProbingMode.noProbing;
            progressBarEF.Value = 0;        // 2021-01-05
            SetRBEnable(cBnow, true);
        }

        private void SetProgressEF(decimal maxTravel)
        {
            //    progressDistance = maxTravel;
            probingTime = (int)((maxTravel / nUDProbeFeed.Value) * 60 * 10);    // distance(mm) / speed(mm/min) *60(sec) 100 ms
            progressBarEF.Minimum = 0;
            progressBarEF.Value = 0;
            progressBarEF.Maximum = (int)probingTime;
            updateProgress = true;
        }

        private void CbZProbing_CheckedChanged(object sender, EventArgs e)
        {
            if (cBnow != null)
            {
                SetNudEnable(2, (cBZProbing.Checked || cBnow.Name == "rB5"));
                if (cBZProbing.Checked)
                    SetText(cBnow, "Z");
                else
                    SetText(cBnow, "");
            }
        }

        private static void SetText(RadioButton tmp, string txt)
        {
            if (tmp.Name != "rB5")
                tmp.Text = txt;
        }

        private void FillStrings()
        {
            string probeString = "";
            if (rB5.Checked)
            { probeString = ProbeToward_Z(); }

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
        private bool ProbeAxisX()
        {
            if (probeX)
            {
                probingAxisIndex = 0;
                SetProgressEF(nUDProbeTravelX.Value);
                SendCommandEvent(new CmdEventArgs(probingAxisX.Replace(',', '.')));
                lblEFProgressInfo.Text = Localization.GetString("probingProbingOn") + " X"; //"Probing on X";
                lblEFProgress.Text = "";
                Logger.Trace("probeAxisX()");
            }
            return probeX;
        }
        private bool ProbeAxisY()
        {
            if (probeY)
            {
                probingAxisIndex = 1;
                SetProgressEF(nUDProbeTravelY.Value);
                SendCommandEvent(new CmdEventArgs(probingAxisY.Replace(',', '.')));
                lblEFProgressInfo.Text = Localization.GetString("probingProbingOn") + " Y"; //"Probing on Y";
                lblEFProgress.Text = "";
                Logger.Trace("probeAxisY()");
            }
            return probeY;
        }
        private bool ProbeAxisZ()
        {
            probingAxisIndex = 2;
            string probeString;
            probeString = ProbeToward_Z();
            SendCommandEvent(new CmdEventArgs(probeString.Replace(',', '.')));
            lblEFProgressInfo.Text = Localization.GetString("probingProbingOn") + " Z"; //"Probing on Z";
            lblEFProgress.Text = "";
            Logger.Trace("probeAxisZ()");
            return probeY;
        }

        private string ProbeToward_E()    // probe towards direction east (right = +X)
        {
            decimal addon = 0;
            if (cBSetCenterZero.Checked) { addon = -nUDDiameter.Value / 2; }    // set tool center as zero

            probingOffset[0] = -nUDOffsetX.Value + addon;
            probingSave[0] = -nUDProbeSaveX.Value + addon;
            return string.Format("{0} X{1} F{2};", ProbeCommand, nUDProbeTravelX.Value, nUDProbeFeed.Value);
        }
        private string ProbeToward_W()    // probe towards direction west (left = -X)
        {
            decimal addon = 0;
            if (cBSetCenterZero.Checked) { addon = nUDDiameter.Value / 2; }    // set tool center as zero

            probingOffset[0] = nUDOffsetX.Value + addon;
            probingSave[0] = nUDProbeSaveX.Value + addon;
            return string.Format("{0} X-{1} F{2};", ProbeCommand, nUDProbeTravelX.Value, nUDProbeFeed.Value);
        }

        private string ProbeToward_N()    // probe towards direction north (up = +Y)
        {
            decimal addon = 0;
            if (cBSetCenterZero.Checked) { addon = -nUDDiameter.Value / 2; }    // set tool center as zero

            probingOffset[1] = -nUDOffsetY.Value + addon;
            probingSave[1] = -nUDProbeSaveY.Value + addon;
            return string.Format("{0} Y{1} F{2};", ProbeCommand, nUDProbeTravelY.Value, nUDProbeFeed.Value);
        }
        private string ProbeToward_S()    // probe towards direction south (down = -Y)
        {
            decimal addon = 0;
            if (cBSetCenterZero.Checked) { addon = nUDDiameter.Value / 2; }    // set tool center as zero

            probingOffset[1] = nUDOffsetY.Value + addon;
            probingSave[1] = nUDProbeSaveY.Value + addon;
            return string.Format("{0} Y-{1} F{2};", ProbeCommand, nUDProbeTravelY.Value, nUDProbeFeed.Value);
        }

        private string ProbeToward_Z()    // probe towards direction east
        {
            probingOffset[2] = nUDOffsetZ.Value;
            probingSave[2] = nUDProbeSaveZ.Value;
            SetProgressEF(nUDProbeTravelZ.Value);
            return string.Format("{0} Z-{1} F{2};", ProbeCommand, nUDProbeTravelZ.Value, nUDProbeFeed.Value);
        }

        public void BtnGetAngleEFClick(object sender, EventArgs e)
        { }
        public double GetAngle
        {
            //set { }
            get { return this.angle; }
        }

        private static void SetImage(RadioButton tmp, bool on)
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
        private void SetRBEnable(RadioButton tmp, bool en)
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
            cBSetCenterZero.Enabled = en;
            gBHardware.Enabled = en;
            gBMovement.Enabled = en;
        }

        #endregion


        #region centerFinder
        private void BtnStartCF_Click(object sender, EventArgs e)
        {
            if (!Grbl.isConnected)
            {
                MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            btnCancelCF.Enabled = true;
            btnStartCF.Enabled = false;
            lblEFProgressInfo.Text = Localization.GetString("probingProbingOn") + " X"; //"Probing on X";
            lblCFProgress.Text = "";
            lblCFStatus.Text = "";
            probingValuesIndex = 0;
            probingAction = ProbingMode.centerFinder;
            SetRBEnable(cBnow, false);

            for (int i = 0; i < stateCommandsMax; i++)
                stateCommands[i] = "";

            probeStartMachine = Grbl.posMachine;
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
                stateCommands[k++] = string.Format("X-{0};", (nUDProbeSaveX.Value + nUDWorkpieceDiameter.Value / 2));         // 9 will be replaced
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
            SendCommandEvent(new CmdEventArgs(stateCommands[probingCount++].Replace(',', '.')));
            SetProgressCF(nUDWorkpieceDiameter.Value);
            isIdle = false;
            timer1.Enabled = true;
        }
        private void BtnCancelCF_Click(object sender, EventArgs e)
        {
            ProbingFinishCF();
            lblCFProgressInfo.Text = Localization.GetString("probingCancel1"); //"Probing canceled";
            if (!isIdle)
                lblCFProgress.Text = Localization.GetString("probingCancel2"); //"Process last command";
        }
        private void ProbingFinishCF()
        {
            timer1.Enabled = false;
            probeX = false; probeY = false;
            probingCount = 1;
            btnCancelCF.Enabled = false;
            btnStartCF.Enabled = true;
            probingAction = ProbingMode.noProbing;
            progressBarCF.Value = 0;        // 2021-01-05
            SetRBEnable(cBnow, true);
        }

        private void SetProgressCF(decimal maxTravel)
        {
            //    progressDistance = maxTravel;
            probingTime = (int)((maxTravel / nUDProbeFeed.Value) * 60 * 10);    // distance(mm) / speed(mm/min) *60(sec) 100 ms
            progressBarCF.Minimum = 0;
            progressBarCF.Value = 0;
            progressBarCF.Maximum = (int)probingTime;
            updateProgress = true;
        }

        #endregion

        #region toolLength
        private void BtnStartTL_Click(object sender, EventArgs e)
        {
            if (!Grbl.isConnected)
            {
                MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            lblEFProgressInfo.Text = Localization.GetString("probingProbingOn") + " Z"; //"Probing on X";
            lblTLProgress.Text = "";
            lblTLStatus.Text = "";
            probingValuesIndex = 0;
            probingAction = ProbingMode.toolLenght;
            SetRBEnable(cBnow, false);

            for (int i = 0; i < stateCommandsMax; i++)
                stateCommands[i] = "";

            probeStartMachine = Grbl.posMachine;
            probingValuesIndex = 0;
            int k = 1;
            stateCommands[k++] = string.Format("{0} Z-{1} F{2};", ProbeCommand, nUDProbeTravelZ.Value, nUDProbeFeed.Value);    // index=1 will be send below
            stateCommands[k++] = string.Format("G53 G00 Z{0};", probeStartMachine.Z);
            //        stateCommands[k++] = string.Format("G91 G00 Z{0};", nUDProbeTravelZ.Value);

            probingCount = 1;
            SendCommandEvent(new CmdEventArgs(stateCommands[probingCount++].Replace(',', '.')));
            SetProgressTL(nUDProbeTravelZ.Value);
            isIdle = false;
            timer1.Enabled = true;
        }
        private void ProbingFinishTL()
        {
            timer1.Enabled = false;
            probeX = false; probeY = false;
            probingCount = 1;
            probingAction = ProbingMode.noProbing;
            progressBarTL.Value = 0;        // 2021-01-05
            SetRBEnable(cBnow, true);       // 2020-08-09
        }

        private void SetProgressTL(decimal maxTravel)
        {
            //    progressDistance = maxTravel;
            probingTime = (int)((maxTravel / nUDProbeFeed.Value) * 60 * 10);    // distance(mm) / speed(mm/min) *60(sec) 100 ms
            progressBarTL.Minimum = 0;
            progressBarTL.Value = 0;
            progressBarTL.Maximum = (int)probingTime;
            updateProgress = true;
        }
        private void BtnClearTL_Click(object sender, EventArgs e)
        { SendCommandEvent(new CmdEventArgs("G49")); }

        private void BtnCancelTL_Click(object sender, EventArgs e)
        {
            ProbingFinishTL();
            lblTLProgressInfo.Text = Localization.GetString("probingCancel1"); //"Probing canceled";
            if (!isIdle)
                lblTLProgress.Text = Localization.GetString("probingCancel2"); //"Process last command";
        }

        #endregion


        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void SendCommandEvent(CmdEventArgs e)
        {
            RaiseCmdEvent?.Invoke(this, e);
            /*    EventHandler<CmdEventArgs> handler = RaiseCmdEvent;
                if (handler != null)
                {
                    handler(this, e);
                }*/
        }


        private void ControlProbing_Click(object sender, EventArgs e)
        {
            SetNudEnable(0, true);
            SetNudEnable(1, true);
            bool isActive = (rBCF1.Checked && (tabControl1.SelectedIndex == 1));
            SetNudEnable(2, !isActive);
        }

        private void RbProbeCoord1_CheckedChanged(object sender, EventArgs e)
        {
            useG92 = false;
            if (rBProbeCoord1.Checked)
                CoordCommand = "G10 L2 P0";        // G92
            else
            {
                CoordCommand = "G92";        // G92
                useG92 = true;
            }
        }

        private void BtnProbeCoordClear_Click(object sender, EventArgs e)
        { SendCommandEvent(new CmdEventArgs("G92.1")); }

        private void BtnSaveTL_Click(object sender, EventArgs e)
        { SendCommandEvent(new CmdEventArgs(string.Format("G90 G00 Z{0}", nUDProbeSaveZ.Value).Replace(',', '.'))); }


        private void TabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                SetNudEnable(0, true);
                SetNudEnable(1, true);
                SetNudEnable(2, true);
            }
            if (tabControl1.SelectedIndex == 1)
            {
                SetNudEnable(0, true);
                SetNudEnable(1, true);
                SetNudEnable(2, false);
            }
            if (tabControl1.SelectedIndex == 2)
            {
                SetNudEnable(0, false);
                SetNudEnable(1, false);
                SetNudEnable(2, true);
            }
        }

        private void TabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (probingAction != ProbingMode.noProbing)
                e.Cancel = true;
        }

        private void SetNudEnable(int axis, bool en)
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

        private void NudOffset_ValueChanged(object sender, EventArgs e)
        {
            nUDProbeSaveX.ValueChanged -= NudProbeSave_ValueChanged;
            nUDProbeSaveY.ValueChanged -= NudProbeSave_ValueChanged;
            nUDProbeSaveZ.ValueChanged -= NudProbeSave_ValueChanged;
            nUDOffsetX.ValueChanged -= NudOffset_ValueChanged;
            nUDOffsetY.ValueChanged -= NudOffset_ValueChanged;
            nUDOffsetZ.ValueChanged -= NudOffset_ValueChanged;
            decimal tmpNewVal;

            if (nUDOffsetX.Value >= nUDProbeSaveX.Value)
            {
                tmpNewVal = Math.Round(nUDOffsetX.Value) + 5;
                if (tmpNewVal > nUDProbeSaveX.Maximum) nUDProbeSaveX.Maximum = tmpNewVal;
                nUDProbeSaveX.Value = tmpNewVal;
                label14.BackColor = nUDProbeSaveX.BackColor = Color.Yellow;
            }
            else { label14.BackColor = nUDProbeSaveX.BackColor = SystemColors.Window; }

            if (nUDOffsetY.Value >= nUDProbeSaveY.Value)
            {
                tmpNewVal = Math.Round(nUDOffsetY.Value) + 5;
                if (tmpNewVal > nUDProbeSaveY.Maximum) nUDProbeSaveY.Maximum = tmpNewVal;
                nUDProbeSaveY.Value = tmpNewVal;
                label14.BackColor = nUDProbeSaveY.BackColor = Color.Yellow;
            }
            else { label14.BackColor = nUDProbeSaveY.BackColor = SystemColors.Window; }

            if (nUDOffsetZ.Value >= nUDProbeSaveZ.Value)
            {
                tmpNewVal = Math.Round(nUDOffsetZ.Value) + 5;
                if (tmpNewVal > nUDProbeSaveZ.Maximum) nUDProbeSaveZ.Maximum = tmpNewVal;
                nUDProbeSaveZ.Value = tmpNewVal;
                label14.BackColor = nUDProbeSaveZ.BackColor = Color.Yellow;
            }
            else { label14.BackColor = nUDProbeSaveZ.BackColor = SystemColors.Window; }


            // Adapt min value
            nUDProbeSaveX.Minimum = nUDOffsetX.Value; //nUDProbeSaveX.Value = nUDOffsetX.Value + ofs;
            nUDProbeSaveY.Minimum = nUDOffsetY.Value; //nUDProbeSaveY.Value = nUDOffsetY.Value + ofs;
            nUDProbeSaveZ.Minimum = nUDOffsetZ.Value; //nUDProbeSaveZ.Value = nUDOffsetZ.Value + ofs;

            nUDOffsetX.ValueChanged += NudOffset_ValueChanged;
            nUDOffsetY.ValueChanged += NudOffset_ValueChanged;
            nUDOffsetZ.ValueChanged += NudOffset_ValueChanged;
            nUDProbeSaveX.ValueChanged += NudProbeSave_ValueChanged;
            nUDProbeSaveY.ValueChanged += NudProbeSave_ValueChanged;
            nUDProbeSaveZ.ValueChanged += NudProbeSave_ValueChanged;
        }

        private void NudProbeSave_ValueChanged(object sender, EventArgs e)
        {
            bool showMessage = false;
            bool cX = false, cY = false, cZ = false;
            if ((nUDProbeSaveX.Value > 0) && (nUDProbeSaveX.Value <= nUDProbeSaveX.Minimum))
            { showMessage = true; nUDOffsetX.BackColor = Color.Yellow; cX = true; }
            else { nUDOffsetX.BackColor = SystemColors.Window; }

            if ((nUDProbeSaveY.Value > 0) && (nUDProbeSaveY.Value <= nUDProbeSaveY.Minimum))
            { showMessage = true; nUDOffsetY.BackColor = Color.Yellow; cY = true; }
            else { nUDOffsetY.BackColor = SystemColors.Window; }

            if ((nUDProbeSaveZ.Value > 0) && (nUDProbeSaveZ.Value <= nUDProbeSaveZ.Minimum))
            { showMessage = true; nUDOffsetZ.BackColor = Color.Yellow; cZ = true; }
            else { nUDOffsetZ.BackColor = SystemColors.Window; }

            if (showMessage)
            {
                label4.BackColor = Color.Yellow;
                MessageBox.Show("Value can't be same or lower than touch plate thickness");
                if (cX) nUDProbeSaveX.Value = nUDOffsetX.Value + (decimal)0.1;
                if (cY) nUDProbeSaveY.Value = nUDOffsetY.Value + (decimal)0.1;
                if (cZ) nUDProbeSaveZ.Value = nUDOffsetZ.Value + (decimal)0.1;
                label4.BackColor = SystemColors.Control;
            }
        }
    }
}
