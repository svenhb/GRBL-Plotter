/*
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.Helper;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace GrblPlotter.MachineControl
{
    public partial class ControlMoveXY : Form
    {
        private readonly double[] joystickXYStep = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickXYSpeed = { 0, 1, 2, 3, 4, 5 };
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public ControlMoveXY()
        {
            InitializeComponent();
            joystickXYStep[0] = 0;
            joystickXYStep[1] = (double)Properties.Settings.Default.guiJoystickXYStep1;
            joystickXYStep[2] = (double)Properties.Settings.Default.guiJoystickXYStep2;
            joystickXYStep[3] = (double)Properties.Settings.Default.guiJoystickXYStep3;
            joystickXYStep[4] = (double)Properties.Settings.Default.guiJoystickXYStep4;
            joystickXYStep[5] = (double)Properties.Settings.Default.guiJoystickXYStep5;
        }
        private void ControlMoveXY_Load(object sender, EventArgs e)
        {
            ucJogControlXY.SetApperance(false);
            ucJogControlXY.Size= new Size(180, 180);
            ucJogControlXY.SetEnabled(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    
        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void SendCommandEvent(CmdEventArgs e)
        {
            RaiseCmdEvent?.Invoke(this, e);
        }

        private void ControlMoveXY_Resize(object sender, EventArgs e)
        {
     //       virtualJoystickXY.Size = new Size(180, 180);
     //       virtualJoystickXY.Invalidate();
        }

        private void ucJogControlXY_RaiseCmdEvent(object sender, UserControls.UserControlCmdEventArgs e)
        {
            SendCommandEvent(new CmdEventArgs(e.Command));
        }
    }
}
