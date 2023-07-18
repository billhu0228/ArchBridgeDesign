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
    public partial class PropertyViewWindow : Form
    {
        public DataTable Sections;
        public Section sectData;
        public  DataSet Properties;
        public Dictionary<int,Section> sects;

        public PropertyViewWindow()
        {
            InitializeComponent();
            this.tabControl1.TabPages[0].Text = "截面";
            Properties = new DataSet();
            InitTables();
            BindData();
            sects = new Dictionary<int, Section>();
            
        }

        private void Add_Click(object sender, EventArgs e)
        {
            SectionDefDia dia = new SectionDefDia();
            dia.ShowDialog();
            sectData = dia.secData;
            sects[dia.secData.SECN]=sectData;
            DataRow r = Properties.Tables["Sections"].NewRow();
            r["ID"] = sectData.SECN;
            r["Name"] = sectData.Name;
            r["Type"]= EnumHelper.GetDescription(typeof(eSection), sectData.Type);

            Properties.Tables["Sections"].Rows.Add(r);
            
        }

        private void InitTables()
        {
            Sections = new DataTable("Sections");
            DataColumn dtColumn;
            DataRow myDataRow;


            // Create id column
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(Int32);
            dtColumn.ColumnName = "ID";
            dtColumn.Caption = "编号";
            dtColumn.ReadOnly = true;
            dtColumn.Unique = true;
            
            // Add column to the DataColumnCollection.
            Sections.Columns.Add(dtColumn);
            Sections.PrimaryKey= new DataColumn[] { Sections.Columns["ID"] };
            // Create Name column.
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "Name";
            dtColumn.Caption = "名称";
            dtColumn.AutoIncrement = false;
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
           
            /// Add column to the DataColumnCollection.
            Sections.Columns.Add(dtColumn);

            // Create Name column.
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "Type";
            dtColumn.Caption = "类型";
            dtColumn.AutoIncrement = false;
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            /// Add column to the DataColumnCollection.
            Sections.Columns.Add(dtColumn);

            Properties.Tables.Add(Sections);

        }

        //private void sectTreeInit()
        //{
        //    sectTree.Nodes.Clear();
        //    var N0 = new TreeNode();
        //    N0.Name = "Sect";
        //    N0.Text = "截面";
        //    sectTree.Nodes.Add(N0);
        //}


        private void BindData()
        {
            // Create a BindingSource
            BindingSource bs = new BindingSource();
            bs.DataSource = Properties.Tables["Sections"];

            // Bind data to DataGridView.DataSource
            SectionGDV.DataSource = bs;
        }

        private void SectionGDV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataRowView obj=(DataRowView)SectionGDV.Rows[e.RowIndex].DataBoundItem;
            if (obj == null)
            {
                return;
            }
            int id = (int)obj.Row["ID", DataRowVersion.Default];
            SectionDefDia dia = new SectionDefDia();
            dia.SetSection(sects[id]);
            //sectData = dia.secData;
            //sects.Add(sectData);
            //DataRow r = Properties.Tables["Sections"].NewRow();
            //r["ID"] = sectData.SECN;
            //r["Name"] = sectData.Name;
            //r["Type"] = EnumHelper.GetDescription(typeof(eSection), sectData.Type);

            if(dia.ShowDialog() == DialogResult.OK)
            {
                sectData = dia.secData;

                DataRow r = Properties.Tables["Sections"].NewRow();
                r["ID"] = sectData.SECN;
                r["Name"] = sectData.Name;
                r["Type"] = EnumHelper.GetDescription(typeof(eSection), sectData.Type);
                if (Properties.Tables["Sections"].Rows.Contains(sectData.SECN))
                {
                    DataRow old = Properties.Tables["Sections"].Select(string.Format("ID = {0:G}", sectData.SECN)).First();
                    Properties.Tables["Sections"].Rows.Remove(old);
                    sects.Remove(sectData.SECN);
                }
                Properties.Tables["Sections"].Rows.Add(r);
                sects[dia.secData.SECN] = sectData;
            }
            
        }
    }
}
