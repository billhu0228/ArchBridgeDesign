
namespace WinFromUI
{
    partial class TubeInput
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.name1 = new System.Windows.Forms.Label();
            this.name2 = new System.Windows.Forms.Label();
            this.tbDia = new System.Windows.Forms.TextBox();
            this.tbTh = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.isCFST = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.65321F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.34679F));
            this.tableLayoutPanel1.Controls.Add(this.name1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.name2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbDia, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbTh, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.isCFST, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(400, 331);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // name1
            // 
            this.name1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.name1.AutoSize = true;
            this.name1.Location = new System.Drawing.Point(3, 17);
            this.name1.Name = "name1";
            this.name1.Size = new System.Drawing.Size(61, 15);
            this.name1.TabIndex = 0;
            this.name1.Text = "直径(m)";
            // 
            // name2
            // 
            this.name2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.name2.AutoSize = true;
            this.name2.Location = new System.Drawing.Point(3, 67);
            this.name2.Name = "name2";
            this.name2.Size = new System.Drawing.Size(61, 15);
            this.name2.TabIndex = 1;
            this.name2.Text = "壁厚(m)";
            // 
            // tbDia
            // 
            this.tbDia.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbDia.Location = new System.Drawing.Point(105, 12);
            this.tbDia.Name = "tbDia";
            this.tbDia.Size = new System.Drawing.Size(215, 25);
            this.tbDia.TabIndex = 4;
            // 
            // tbTh
            // 
            this.tbTh.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbTh.Location = new System.Drawing.Point(105, 62);
            this.tbTh.Name = "tbTh";
            this.tbTh.Size = new System.Drawing.Size(215, 25);
            this.tbTh.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "是否为CFST";
            // 
            // isCFST
            // 
            this.isCFST.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.isCFST.AutoSize = true;
            this.isCFST.Location = new System.Drawing.Point(105, 116);
            this.isCFST.Name = "isCFST";
            this.isCFST.Size = new System.Drawing.Size(18, 17);
            this.isCFST.TabIndex = 5;
            this.isCFST.UseVisualStyleBackColor = true;
            // 
            // TubeInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximumSize = new System.Drawing.Size(400, 450);
            this.MinimumSize = new System.Drawing.Size(400, 450);
            this.Name = "TubeInput";
            this.Size = new System.Drawing.Size(400, 450);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label name1;
        private System.Windows.Forms.Label name2;
        public System.Windows.Forms.TextBox tbDia;
        public System.Windows.Forms.TextBox tbTh;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox isCFST;
    }
}
