using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFromUI
{
    public partial class SetValue : Form
    {
        public double paraValue;
        string paraName;
        public SetValue(string ParaName,double value)
        {
            InitializeComponent();
            paraName = ParaName;
            if (!double.IsNaN(value))
            {
                tbValue.Text = string.Format( "{0:F2}",value);
                paraValue = value;
            }

        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btCancel_Click(object sender, EventArgs e)
        {
            paraValue = double.NaN;
            tbValue.Text = "";
            tbValue.SelectAll();

        }
        private void SetValue_Activeted(object sender, EventArgs e)
        {
            tbValue.Focus();
            tbValue.SelectAll();
        }

        private void SetValue_Load(object sender, EventArgs e)
        {   
            lbName.Text = paraName;
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            // btGenerateMd_Click(sender, e);
            
            btConfirm_Click(sender,e);
        }

        private void btConfirm_Click(object sender, EventArgs e)
        {
            if (tbValue.Text != "")
            {
                paraValue = double.Parse(tbValue.Text);
            }

            Close();
        }

        private void CheckEnterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (tbValue.Text != "")
                {
                    paraValue = double.Parse(tbValue.Text);
                }

                Close();
            }
        }
    }
}
