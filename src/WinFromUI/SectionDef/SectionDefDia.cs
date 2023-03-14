using Model;
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
    public partial class SectionDefDia : Form
    {
        TubeInput inp0;
        HInput inp1;
        public Section secData;
        public SectionDefDia()
        {
            InitializeComponent();
            this.AcceptButton = btAccept;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox thebox = (ComboBox)sender;
            inp0 = new TubeInput();
            inp1 = new HInput();
            this.panel1.Controls.Clear();
            switch (thebox.SelectedIndex)
            {
                case 0:
                    this.panel1.Controls.Add(inp0);
                    break;
                case 1:
                    this.panel1.Controls.Add(inp1);
                    break;
                default:
                    break;
            }
            ;
        }

        private void btAccept_Click(object sender, EventArgs e)
        {
            int idx = comboBox1.SelectedIndex;
            int ID = int.Parse(tbID.Text);
            string Name = tbName.Text;
            switch (idx)
            {
                case 0:
                    bool isCF = inp0.isCFST.Checked;
                    secData = new TubeSection(ID, double.Parse(inp0.tbDia.Text), double.Parse(inp0.tbTh.Text), isCF,Name);
                    break;
                case 1:
                    secData = new HSection(ID, 
                        double.Parse(inp1.tbW1.Text),
                        double.Parse(inp1.tbW2.Text),
                        double.Parse(inp1.tbW3.Text),
                        double.Parse(inp1.btT1.Text),
                        double.Parse(inp1.tbT2.Text),
                        double.Parse(inp1.tbT3.Text),Name );
                    break;
                default:
                    break;
            }

            this.DialogResult = DialogResult.OK;
            Close();

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        public void SetSection(Section s)
        {
            tbID.Text = s.SECN.ToString();
            tbName.Text = s.Name;
            inp0 = new TubeInput();
            inp1 = new HInput();

            if (s.Type==eSection.CFST)
            {
                comboBox1.SelectedIndex = 0;
                inp0.isCFST.Checked = true;
                inp0.isCFST.CheckState = CheckState.Checked;      
                inp0.tbDia.Text = s.Diameter.ToString("F2");
                inp0.tbTh.Text = s.Thickness.ToString("F3");
                this.panel1.Controls.Add(inp0);
            }
        }
    }
}
