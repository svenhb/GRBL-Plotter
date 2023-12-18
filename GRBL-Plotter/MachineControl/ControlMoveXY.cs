using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using virtualJoystick;

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
            int raster = 5;
            if (!Properties.Settings.Default.guiJoystickApperance1)
            {
                raster = 1;
            }
            virtualJoystickXY.JoystickRaster = raster;
            virtualJoystickXY.Size = new Size(180, 180);
            virtualJoystickXY.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VirtualJoystickXY_Enter(object sender, EventArgs e)
        {
            if (Grbl.isVersion_0) SendCommandEvent(new CmdEventArgs("G91;G1F100"));
        }
        private void VirtualJoystickXY_Leave(object sender, EventArgs e)
        {
        }


        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void SendCommandEvent(CmdEventArgs e)
        {
            RaiseCmdEvent?.Invoke(this, e);
        }

        private void VirtualJoystickXY_JoyStickEvent(object sender, JogEventArgs e)
        { VirtualJoystickXY_move(e.JogPosX, e.JogPosY); }
        private void VirtualJoystickXY_move(int index_X, int index_Y)
        {
            int indexX = Math.Abs(index_X);
            int indexY = Math.Abs(index_Y);
            int dirX = Math.Sign(index_X);
            int dirY = Math.Sign(index_Y);

            if (indexX >= joystickXYStep.Length)
            { indexX = joystickXYStep.Length - 1; index_X = indexX; }
            if (indexX < 0)
            { indexX = 0; index_X = 0; }
            if (indexY >= joystickXYStep.Length)
            { indexY = joystickXYStep.Length - 1; index_Y = indexY; }
            if (indexY < 0)
            { indexY = 0; index_Y = 0; }

            //    if ((index_X == 0) && (index_Y == 0))
            //    { if (!Grbl.isVersion_0) SendRealtimeCommand(133); return; }

            int speed = (int)Math.Max(joystickXYSpeed[indexX], joystickXYSpeed[indexY]);
            String strX = Gcode.FrmtNum(joystickXYStep[indexX] * dirX);
            String strY = Gcode.FrmtNum(joystickXYStep[indexY] * dirY);
            //    Logger.Error("VirtualJoystickXY_move speed==0  x:{0}  y:{1}", index_X, index_Y);
            if (speed > 0)
            {
                if (Properties.Settings.Default.machineLimitsAlarm && Properties.Settings.Default.machineLimitsShow)
                {
                    if (!Dimensions.WithinLimits(Grbl.posMachine, joystickXYStep[indexX] * dirX, joystickXYStep[indexY] * dirY))
                    {
                        decimal minx = Properties.Settings.Default.machineLimitsHomeX;
                        decimal maxx = minx + Properties.Settings.Default.machineLimitsRangeX;
                        decimal miny = Properties.Settings.Default.machineLimitsHomeY;
                        decimal maxy = miny + Properties.Settings.Default.machineLimitsRangeY;

                        string tmp = string.Format(culture, "minX: {0:0.0} moveTo: {1:0.0} maxX: {2:0.0}", minx, (Grbl.posMachine.X + joystickXYStep[indexX] * dirX), maxx);
                        tmp += string.Format(culture, "\r\nminY: {0:0.0} moveTo: {1:0.0} maxY: {2:0.0}", miny, (Grbl.posMachine.Y + joystickXYStep[indexY] * dirY), maxy);
                        System.Media.SystemSounds.Beep.Play();
                        DialogResult dialogResult = MessageBox.Show(Localization.GetString("mainLimits1") + tmp + Localization.GetString("mainLimits2"), Localization.GetString("mainAttention"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                        if (dialogResult == DialogResult.Cancel)
                            return;
                    }
                }
                String s = "G91 ";
                if (Grbl.isMarlin) { s += ";G1 "; }
                if (index_X == 0)
                    s += String.Format(culture, "Y{0} F{1}", strY, speed).Replace(',', '.');
                else if (index_Y == 0)
                    s += String.Format(culture, "X{0} F{1}", strX, speed).Replace(',', '.');
                else
                    s += String.Format(culture, "X{0} Y{1} F{2}", strX, strY, speed).Replace(',', '.');

                SendCommandEvent(new CmdEventArgs(s));
            }
        }

        private void ControlMoveXY_Resize(object sender, EventArgs e)
        {
            virtualJoystickXY.Size = new Size(180, 180);
            virtualJoystickXY.Invalidate();
        }
    }
}
