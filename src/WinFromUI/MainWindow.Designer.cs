
namespace WinFromUI
{
    partial class MainWindow
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("旧参数表");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("材料");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("截面");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("工作树", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.moninter = new System.Windows.Forms.ToolStripStatusLabel();
            this.pBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.TSI_V6 = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.截面ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.输出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ansysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.midasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spaceClaimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vtkWindow = new Kitware.VTK.RenderWindowControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.paraTree = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btGenerateMd = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btFrontView = new System.Windows.Forms.ToolStripButton();
            this.btTopView = new System.Windows.Forms.ToolStripButton();
            this.btLeftView = new System.Windows.Forms.ToolStripButton();
            this.btRightView = new System.Windows.Forms.ToolStripButton();
            this.btFit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btPerspView = new System.Windows.Forms.ToolStripButton();
            this.btOrthView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moninter,
            this.pBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1353);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(2025, 31);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // moninter
            // 
            this.moninter.AutoSize = false;
            this.moninter.Name = "moninter";
            this.moninter.Size = new System.Drawing.Size(200, 21);
            // 
            // pBar1
            // 
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(150, 19);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.编辑EToolStripMenuItem,
            this.输出ToolStripMenuItem,
            this.帮助HToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(2025, 44);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripMenuItem,
            this.打开OToolStripMenuItem,
            this.保存SToolStripMenuItem,
            this.另存为ToolStripMenuItem,
            this.toolStripSeparator4,
            this.toolStripMenuItem1});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(111, 38);
            this.文件ToolStripMenuItem.Text = "文件(F)";
            // 
            // 新建NToolStripMenuItem
            // 
            this.新建NToolStripMenuItem.Name = "新建NToolStripMenuItem";
            this.新建NToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.新建NToolStripMenuItem.Text = "新建(N)";
            this.新建NToolStripMenuItem.Click += new System.EventHandler(this.新建NToolStripMenuItem_Click);
            // 
            // 打开OToolStripMenuItem
            // 
            this.打开OToolStripMenuItem.Name = "打开OToolStripMenuItem";
            this.打开OToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.打开OToolStripMenuItem.Text = "打开(O)";
            this.打开OToolStripMenuItem.Click += new System.EventHandler(this.打开OToolStripMenuItem_Click);
            // 
            // 保存SToolStripMenuItem
            // 
            this.保存SToolStripMenuItem.Name = "保存SToolStripMenuItem";
            this.保存SToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.保存SToolStripMenuItem.Text = "保存(S)";
            this.保存SToolStripMenuItem.Click += new System.EventHandler(this.保存SToolStripMenuItem_Click);
            // 
            // 另存为ToolStripMenuItem
            // 
            this.另存为ToolStripMenuItem.Name = "另存为ToolStripMenuItem";
            this.另存为ToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.另存为ToolStripMenuItem.Text = "另存为(A)";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(356, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSI_V6});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(359, 44);
            this.toolStripMenuItem1.Text = "已存项目";
            // 
            // TSI_V6
            // 
            this.TSI_V6.Name = "TSI_V6";
            this.TSI_V6.Size = new System.Drawing.Size(483, 44);
            this.TSI_V6.Text = "诗礼黑惠江大桥施工图设计模型";
            this.TSI_V6.Click += new System.EventHandler(this.TSI_V6_Click);
            // 
            // 编辑EToolStripMenuItem
            // 
            this.编辑EToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.截面ToolStripMenuItem});
            this.编辑EToolStripMenuItem.Name = "编辑EToolStripMenuItem";
            this.编辑EToolStripMenuItem.Size = new System.Drawing.Size(111, 38);
            this.编辑EToolStripMenuItem.Text = "编辑(E)";
            // 
            // 截面ToolStripMenuItem
            // 
            this.截面ToolStripMenuItem.Name = "截面ToolStripMenuItem";
            this.截面ToolStripMenuItem.Size = new System.Drawing.Size(195, 44);
            this.截面ToolStripMenuItem.Text = "截面";
            this.截面ToolStripMenuItem.Click += new System.EventHandler(this.截面ToolStripMenuItem_Click);
            // 
            // 输出ToolStripMenuItem
            // 
            this.输出ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ansysToolStripMenuItem,
            this.midasToolStripMenuItem,
            this.spaceClaimToolStripMenuItem});
            this.输出ToolStripMenuItem.Name = "输出ToolStripMenuItem";
            this.输出ToolStripMenuItem.Size = new System.Drawing.Size(118, 38);
            this.输出ToolStripMenuItem.Text = "输出(O)";
            // 
            // ansysToolStripMenuItem
            // 
            this.ansysToolStripMenuItem.Enabled = false;
            this.ansysToolStripMenuItem.Name = "ansysToolStripMenuItem";
            this.ansysToolStripMenuItem.Size = new System.Drawing.Size(351, 44);
            this.ansysToolStripMenuItem.Text = "Ansys";
            this.ansysToolStripMenuItem.Click += new System.EventHandler(this.ansysToolStripMenuItem_Click);
            // 
            // midasToolStripMenuItem
            // 
            this.midasToolStripMenuItem.Enabled = false;
            this.midasToolStripMenuItem.Name = "midasToolStripMenuItem";
            this.midasToolStripMenuItem.Size = new System.Drawing.Size(351, 44);
            this.midasToolStripMenuItem.Text = "Midas MCT(*.mct)";
            this.midasToolStripMenuItem.Click += new System.EventHandler(this.midasToolStripMenuItem_Click);
            // 
            // spaceClaimToolStripMenuItem
            // 
            this.spaceClaimToolStripMenuItem.Enabled = false;
            this.spaceClaimToolStripMenuItem.Name = "spaceClaimToolStripMenuItem";
            this.spaceClaimToolStripMenuItem.Size = new System.Drawing.Size(351, 44);
            this.spaceClaimToolStripMenuItem.Text = "SpaceClaim";
            // 
            // 帮助HToolStripMenuItem
            // 
            this.帮助HToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem});
            this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
            this.帮助HToolStripMenuItem.Size = new System.Drawing.Size(117, 38);
            this.帮助HToolStripMenuItem.Text = "帮助(H)";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(298, 44);
            this.关于ToolStripMenuItem.Text = "关于 建模助手";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.TSI_About_Click);
            // 
            // vtkWindow
            // 
            this.vtkWindow.AddTestActors = false;
            this.vtkWindow.AutoScroll = true;
            this.vtkWindow.AutoSize = true;
            this.vtkWindow.BackColor = System.Drawing.Color.CornflowerBlue;
            this.vtkWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vtkWindow.Location = new System.Drawing.Point(0, 0);
            this.vtkWindow.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.vtkWindow.Name = "vtkWindow";
            this.vtkWindow.Size = new System.Drawing.Size(1470, 1268);
            this.vtkWindow.TabIndex = 0;
            this.vtkWindow.TestText = null;
            this.vtkWindow.Load += new System.EventHandler(this.renderWindowControl1_Load);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 85);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.paraTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.vtkWindow);
            this.splitContainer1.Size = new System.Drawing.Size(2025, 1268);
            this.splitContainer1.SplitterDistance = 549;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 4;
            // 
            // paraTree
            // 
            this.paraTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paraTree.Location = new System.Drawing.Point(0, 0);
            this.paraTree.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.paraTree.Name = "paraTree";
            treeNode1.Name = "ParasNd";
            treeNode1.Text = "旧参数表";
            treeNode2.Name = "MaterialNd";
            treeNode2.Text = "材料";
            treeNode3.Name = "SectionNd";
            treeNode3.Text = "截面";
            treeNode4.Name = "节点0";
            treeNode4.Text = "工作树";
            this.paraTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.paraTree.Size = new System.Drawing.Size(549, 1268);
            this.paraTree.TabIndex = 6;
            this.paraTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.paraTree_AfterSelect);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btGenerateMd,
            this.toolStripSeparator1,
            this.btFrontView,
            this.btTopView,
            this.btLeftView,
            this.btRightView,
            this.btFit,
            this.toolStripSeparator3,
            this.btPerspView,
            this.btOrthView,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 44);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip1.Size = new System.Drawing.Size(2025, 41);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btGenerateMd
            // 
            this.btGenerateMd.Image = global::WinFromUI.Properties.Resources.大桥;
            this.btGenerateMd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btGenerateMd.Name = "btGenerateMd";
            this.btGenerateMd.Size = new System.Drawing.Size(134, 35);
            this.btGenerateMd.Text = "生成模型";
            this.btGenerateMd.Click += new System.EventHandler(this.btGenerateMd_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 41);
            // 
            // btFrontView
            // 
            this.btFrontView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btFrontView.Image = ((System.Drawing.Image)(resources.GetObject("btFrontView.Image")));
            this.btFrontView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btFrontView.Name = "btFrontView";
            this.btFrontView.Size = new System.Drawing.Size(46, 35);
            this.btFrontView.Text = "前视图";
            this.btFrontView.Click += new System.EventHandler(this.btFrontView_Click);
            // 
            // btTopView
            // 
            this.btTopView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btTopView.Image = ((System.Drawing.Image)(resources.GetObject("btTopView.Image")));
            this.btTopView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btTopView.Name = "btTopView";
            this.btTopView.Size = new System.Drawing.Size(46, 35);
            this.btTopView.Text = "顶视图";
            this.btTopView.Click += new System.EventHandler(this.btTopView_Click);
            // 
            // btLeftView
            // 
            this.btLeftView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btLeftView.Image = ((System.Drawing.Image)(resources.GetObject("btLeftView.Image")));
            this.btLeftView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btLeftView.Name = "btLeftView";
            this.btLeftView.Size = new System.Drawing.Size(46, 35);
            this.btLeftView.Text = "左视图";
            this.btLeftView.Click += new System.EventHandler(this.btLeftView_Click);
            // 
            // btRightView
            // 
            this.btRightView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btRightView.Image = ((System.Drawing.Image)(resources.GetObject("btRightView.Image")));
            this.btRightView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btRightView.Name = "btRightView";
            this.btRightView.Size = new System.Drawing.Size(46, 35);
            this.btRightView.Text = "右视图";
            this.btRightView.Click += new System.EventHandler(this.btRightView_Click);
            // 
            // btFit
            // 
            this.btFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btFit.Image = global::WinFromUI.Properties.Resources.fit;
            this.btFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btFit.Name = "btFit";
            this.btFit.Size = new System.Drawing.Size(46, 35);
            this.btFit.Text = "适合";
            this.btFit.Click += new System.EventHandler(this.btFit_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 41);
            // 
            // btPerspView
            // 
            this.btPerspView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btPerspView.Image = ((System.Drawing.Image)(resources.GetObject("btPerspView.Image")));
            this.btPerspView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btPerspView.Name = "btPerspView";
            this.btPerspView.Size = new System.Drawing.Size(46, 35);
            this.btPerspView.Text = "toolStripButton5";
            this.btPerspView.Click += new System.EventHandler(this.btPerspView_Click);
            // 
            // btOrthView
            // 
            this.btOrthView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btOrthView.Image = global::WinFromUI.Properties.Resources.Orthogonal;
            this.btOrthView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btOrthView.Name = "btOrthView";
            this.btOrthView.Size = new System.Drawing.Size(46, 35);
            this.btOrthView.Text = "toolStripButton6";
            this.btOrthView.Click += new System.EventHandler(this.btOrthView_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 41);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(2025, 1384);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "钢管混凝土拱桥建模助手";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 编辑EToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开OToolStripMenuItem;
        private Kitware.VTK.RenderWindowControl vtkWindow;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem 保存SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 另存为ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 输出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ansysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem midasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spaceClaimToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助HToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btGenerateMd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btFit;
        private System.Windows.Forms.ToolStripButton btFrontView;
        private System.Windows.Forms.ToolStripButton btTopView;
        private System.Windows.Forms.ToolStripButton btLeftView;
        private System.Windows.Forms.ToolStripButton btPerspView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripProgressBar pBar1;
        private System.Windows.Forms.ToolStripButton btOrthView;
        private System.Windows.Forms.ToolStripStatusLabel moninter;
        private System.Windows.Forms.ToolStripButton btRightView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem TSI_V6;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 截面ToolStripMenuItem;
        private System.Windows.Forms.TreeView paraTree;
    }
}

