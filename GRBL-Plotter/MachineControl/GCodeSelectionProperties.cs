using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace GrblPlotter.MachineControl
{
    public partial class GCodeSelectionProperties : Form
    {

        public bool CenterWasChanged { get; set; } = false;
        public double CenterX { get { return (double)NudCenterX.Value; } set { NudCenterX.Value = (decimal)value; } }
        public double CenterY { get { return (double)NudCenterY.Value; } set { NudCenterY.Value = (decimal)value; } }
        public bool AttributeWasChanged { get; set; } = false;
        public double AttributePenWidth
        {
            get { return (double)NudAttributeWidth.Value; }
            set
            {
                if (value > 0)
                {
                    NudAttributeWidth.Value = (decimal)value;
                    NudAttributeWidth.Enabled = true;
                }
            }
        }
        public string AttributePenColor
        {
            get { return GetColorString(BtnAttributeColor.BackColor); }
            set
            {
                if (value.Length > 2)
                {
                    BtnAttributeColor.BackColor = GetColor(value);
                    BtnAttributeColor.Enabled = true;
                }
            }
        }

        public GCodeSelectionProperties()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
            //    BtnAttributeColor.Enabled = false;
            //    NudAttributeWidth.Enabled = false;
            //    AttributePenColor = "000000";
        }
        private void GCodeSelectionProperties_Load(object sender, EventArgs e)
        {
            NudCenterX.Increment = NudIncrement.Value;
            NudCenterY.Increment = NudIncrement.Value;
        }

        private void NudCenterX_ValueChanged(object sender, EventArgs e)
        { CenterWasChanged = true; }

        private void BtnAttributeColor_Click(object sender, EventArgs e)
        { ApplyColor(BtnAttributeColor); AttributeWasChanged = true; }

        private void NudAttributeWidth_ValueChanged(object sender, EventArgs e)
        { AttributeWasChanged = true; }

        private void NudIncrement_ValueChanged(object sender, EventArgs e)
        {
            NudCenterX.Increment = NudIncrement.Value;
            NudCenterY.Increment = NudIncrement.Value;
        }

        private string GetColorString(Color c)
        { return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"); }

        private Color GetColor(string pencolor)
        {   //return (Color)System.Windows.Media.ColorConverter.ConvertFromString("#" + tmp);
            if (UInt32.TryParse(pencolor, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint clr))  // try Hex code #00ff00
            {
                clr |= 0xff000000; // remove alpha
                return System.Drawing.Color.FromArgb((int)clr);
            }
            else
            {
                return Color.FromName(pencolor);
            }
        }
        private void ApplyColor(Button btn)
        {
            using (ColorDialog colorDialog1 = new ColorDialog())
            {
                colorDialog1.AnyColor = true;
                colorDialog1.Color = btn.BackColor;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    SetButtonColors(btn, colorDialog1.Color);
                }
            }
            this.Invalidate();
        }
        private static void SetButtonColors(Button btn, Color col)
        {
            btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
        }
        private static Color ContrastColor(Color myColor)
        {
            int d;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * myColor.R + 0.587 * myColor.G + 0.114 * myColor.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }
    }
}
