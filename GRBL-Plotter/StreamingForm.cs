using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class StreamingForm : Form
    {
        public StreamingForm()
        {
            InitializeComponent();
        }

        private void StreamingForm_Load(object sender, EventArgs e)
        {
            //      SetRange_ValueChanged(sender, e);
                      tBOverrideFR.Minimum = (int)nUDOverrideFRBtm.Value;
                        tBOverrideFR.Maximum = (int)nUDOverrideFRTop.Value;
                        tBOverrideSS.Minimum = (int)nUDOverrideSSBtm.Value;
                       tBOverrideSS.Maximum = (int)nUDOverrideSSTop.Value;
            lblOverrideFRValue.Text = tBOverrideFR.Value.ToString();
            lblOverrideSSValue.Text = tBOverrideSS.Value.ToString();
        }
        private void SetRange_ValueChanged(object sender, EventArgs e)
        {
  //          tBOverrideFR.Minimum = (int)nUDOverrideFRBtm.Value;
//            tBOverrideFR.Maximum = (int)nUDOverrideFRTop.Value;
//            tBOverrideSS.Minimum = (int)nUDOverrideSSBtm.Value;
 //           tBOverrideSS.Maximum = (int)nUDOverrideSSTop.Value;
            lblOverrideFRValue.Text = tBOverrideFR.Value.ToString();
            lblOverrideSSValue.Text = tBOverrideSS.Value.ToString();
        }
        private void nUDOverrideFRTop_ValueChanged(object sender, EventArgs e)
        {   tBOverrideFR.Maximum = (int)nUDOverrideFRTop.Value;  }
        private void nUDOverrideFRBtm_ValueChanged(object sender, EventArgs e)
        {   tBOverrideFR.Minimum = (int)nUDOverrideFRBtm.Value;  }
        private void nUDOverrideSSTop_ValueChanged(object sender, EventArgs e)
        {   tBOverrideSS.Maximum = (int)nUDOverrideSSTop.Value;  }
        private void nUDOverrideSSBtm_ValueChanged(object sender, EventArgs e)
        {   tBOverrideSS.Minimum = (int)nUDOverrideSSBtm.Value;  }

        public void tBOverrideFR_Scroll(object sender, EventArgs e)
        {
            lblOverrideFRValue.Text = tBOverrideFR.Value.ToString();
        }

        public void tBOverrideSS_Scroll(object sender, EventArgs e)
        {
            lblOverrideSSValue.Text = tBOverrideSS.Value.ToString();
        }

        public void show_value_FR(string val)
        { lblFRValue.Text = val; }
        public void show_value_SS(string val)
        { lblSSValue.Text = val; }

        private void tBOverrideFR_MouseUp(object sender, MouseEventArgs e)
        {
            if (cBOverrideFREnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.feedRate, (float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void tBOverrideFR_KeyUp(object sender, KeyEventArgs e)
        {
            if (cBOverrideFREnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.feedRate, (float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void tBOverrideSS_MouseUp(object sender, MouseEventArgs e)
        {
            if (cBOverrideSSEnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }
        private void tBOverrideSS_KeyUp(object sender, KeyEventArgs e)
        {
            if (cBOverrideSSEnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }

        private void cBOverrideFREnable_CheckedChanged(object sender, EventArgs e)
        {
            OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.feedRate,(float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void cBOverrideSSEnable_CheckedChanged(object sender, EventArgs e)
        {
            OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }

        public event EventHandler<OverrideEventArgs> RaiseOverrideEvent;
        protected virtual void OnRaiseOverrideEvent(OverrideEventArgs e)
        {
            EventHandler<OverrideEventArgs> handler = RaiseOverrideEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
    public enum overrideSource { spindleSpeed, feedRate};

    public class OverrideEventArgs : EventArgs
    {
        private overrideSource source;
        private float value;
        private bool enable;
        public OverrideEventArgs(overrideSource in_source, float in_value, bool in_enable)
        {
            source = in_source;
            value = in_value;
            enable = in_enable;
        }
        public overrideSource Source
        { get { return source; } }
        public float Value
        { get { return value; } }
        public bool Enable
        { get { return enable; } }
    }

}
