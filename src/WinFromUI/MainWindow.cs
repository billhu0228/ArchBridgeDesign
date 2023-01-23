using AnsysInterface;
using Kitware.VTK;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinFromUI
{
    public partial class MainWindow : Form
    {
        FEMModel theFem;
        ArchAxis ax;
        Arch theArchModel;
        List<Parameter> theParas;
        ArchParameters paras;
        vtkBox viewBounding;


        public MainWindow()
        {
            InitializeComponent();
            viewBounding = new vtkBox();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            theParas = new List<Parameter>()
            {
                new Parameter() {Id = 1, Name = "截面参数", Value = -1, ParentId = 0},
                new Parameter() {Id = 2, Name = "拱轴参数", Value = -1, ParentId = 0},
                new Parameter() {Id = 3, Name = "拱肋参数", Value = -1, ParentId = 0},
                new Parameter() {Id = 4, Name = "构造参数", Value = -1, ParentId = 0},

                new Parameter() {Id = 11, Name = "主管直径(D0)", Value = -1, ParentId = 1},
                new Parameter() {Id = 12, Name = "拱脚腹杆直径(D21)", Value = -1, ParentId = 1},
                new Parameter() {Id = 13, Name = "普通腹杆直径(D22)", Value = -1, ParentId = 1},
                new Parameter() {Id = 14, Name = "拼装腹杆直径(D23)", Value = -1, ParentId = 1},

                new Parameter() {Id = 15, Name = "内隔直径(D31)", Value = -1, ParentId = 1}, //
                new Parameter() {Id = 16, Name = "内隔腹杆直径(D32)", Value = -1, ParentId = 1},

                new Parameter() {Id = 17, Name = "横梁弦杆直径(D41)", Value = -1, ParentId = 1},
                new Parameter() {Id = 18, Name = "横梁腹杆直径(D42)", Value = -1, ParentId = 1},
                new Parameter() {Id = 19, Name = "平联直径(D51)", Value = -1, ParentId = 1},

                new Parameter() {Id = 21, Name = "跨径(L)", Value = -1, ParentId = 2},
                new Parameter() {Id = 22, Name = "拱轴系数(m)", Value = -1, ParentId = 2},
                new Parameter() {Id = 23, Name = "跨矢比(L/f)", Value = -1, ParentId = 2},
                new Parameter() {Id = 31, Name = "拱脚高度(H0)", Value = -1, ParentId = 3},
                new Parameter() {Id = 32, Name = "拱顶高度(H1)", Value = -1, ParentId = 3},
                new Parameter() {Id = 33, Name = "拱肋宽度(W0)", Value = -1, ParentId = 3},
                new Parameter() {Id = 34, Name = "拱肋间距(W1)", Value = -1, ParentId = 3},
            };
            paraTree.Nodes.Clear();
            var topNode = new TreeNode();
            topNode.Name = "0";
            topNode.Text = "参数表";
            paraTree.Nodes.Add(topNode);
            DataBindTree(topNode, theParas, 0);
            paraTree.ExpandAll();
        }
        private void btGenerateMd_Click(object sender, EventArgs e)
        {
            theArchModel = null;
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            renderer.RemoveAllViewProps();

            pBar1.Visible = true;
            pBar1.Minimum = 1;
            pBar1.Maximum = 10;
            pBar1.Value = 1;
            pBar1.Step = 1;

            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);

            pBar1.PerformStep();

            double D0 = getPara(11);
            double Hfoot = getPara(31);
            double Htop = getPara(32);
            double L = getPara(21);
            double m = getPara(22);
            double f = getPara(23);
            double W1 = getPara(34);
            double W0 = getPara(33);

            theArchModel = NamedArch.PhoenixModelV6(out ax, m, L / f, Hfoot, Htop);
            pBar1.Step = 5;
            pBar1.PerformStep();
            theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            pBar1.Step = 2;
            pBar1.PerformStep();
            var act11 = create_coord(theFem.RelatedArchBridge.UpSkeleton, D0);
            var act12 = create_coord(theFem.RelatedArchBridge.UpSkeleton, D0);
            var act13 = create_coord(theFem.RelatedArchBridge.UpSkeleton, D0);
            var act14 = create_coord(theFem.RelatedArchBridge.UpSkeleton, D0);
            act11.SetPosition(0, -0.5 * W1 - W0, 0);
            act12.SetPosition(0, -0.5 * W1, 0);
            act13.SetPosition(0, +0.5 * W1, 0);
            act14.SetPosition(0, +0.5 * W1 + W0, 0);


            var act21 = create_coord(theFem.RelatedArchBridge.LowSkeleton, D0);
            var act22 = create_coord(theFem.RelatedArchBridge.LowSkeleton, D0);
            var act23 = create_coord(theFem.RelatedArchBridge.LowSkeleton, D0);
            var act24 = create_coord(theFem.RelatedArchBridge.LowSkeleton, D0);

            act21.SetPosition(0, -0.5 * W1 - W0, 0);
            act22.SetPosition(0, -0.5 * W1, 0);
            act23.SetPosition(0, +0.5 * W1, 0);
            act24.SetPosition(0, +0.5 * W1 + W0, 0);

            pBar1.Step = 1;
            pBar1.PerformStep();

            this.UpdateBounds(act11);
            this.UpdateBounds(act12);
            this.UpdateBounds(act13);
            this.UpdateBounds(act14);
            this.UpdateBounds(act21);
            this.UpdateBounds(act22);
            this.UpdateBounds(act23);
            this.UpdateBounds(act24);

            renderer.AddActor(act11);
            renderer.AddActor(act12);
            renderer.AddActor(act13);
            renderer.AddActor(act14);
            renderer.AddActor(act21);
            renderer.AddActor(act22);
            renderer.AddActor(act23);
            renderer.AddActor(act24);

            foreach (var item in theFem.ElementList)
            {
                double dia;
                if (item.Secn == 21)
                {
                    dia = 0.9;
                }
                else if (item.Secn == 22)
                {
                    dia = 0.8;
                }
                else if (item.Secn == 23)
                {
                    dia = 0.6;
                }

                else if (item.Secn == 31)
                {
                    dia = 0.8;
                }
                else if (item.Secn == 32)
                {
                    dia = 0.3;
                }
                else if (item.Secn == 41)
                {
                    dia = 0.8;
                }
                else if (item.Secn == 42)
                {
                    dia = 0.5;
                }
                else if (item.Secn == 51)
                {
                    dia = 0.6;
                }
                else
                {
                    continue;
                }
                Point3D[] eles = new Point3D[2] { theFem.GetPoint(item.Ni), theFem.GetPoint(item.Nj) };
                vtkActor actor = create_coord3d(eles, dia);
                this.UpdateBounds(actor);
                renderer.AddActor(actor);
            }
            pBar1.PerformStep();
            MessageBox.Show("模型生成成功！", "通知", MessageBoxButtons.OK);
            pBar1.Value = 1;

            btFrontView_Click(sender, e);
        }


        private void renderWindowControl1_Load(object sender, EventArgs e)
        {

            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(.2, .3, .4);

        }
        #region 普通方法
        private vtkActor create_coord3d(IEnumerable<Point3D> inp, double dia)
        {
            vtkPoints pts = vtkPoints.New();
            vtkPolyLine pl = vtkPolyLine.New();
            pl.GetPointIds().SetNumberOfIds(inp.Count());
            int i = 0;
            foreach (var item in inp)
            {
                pts.InsertPoint(i, item.X, item.Z, item.Y);
                pl.GetPointIds().SetId(i, i);
                i++;
            }

            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(pl);

            // Create a polydata to store everything in
            vtkPolyData polyData = vtkPolyData.New();
            polyData.SetPoints(pts);
            polyData.SetLines(cells);

            vtkTubeFilter tubes = vtkTubeFilter.New();
            tubes.SetInput(polyData);
            tubes.SetRadius(0.5 * dia);
            tubes.SetNumberOfSides(24);
            tubes.Update();

            vtkPolyDataMapper mapper2 = vtkPolyDataMapper.New();
            mapper2.SetInputConnection(tubes.GetOutputPort());
            vtkActor actor2 = vtkActor.New();
            actor2.SetMapper(mapper2);

            return actor2;

        }

        private vtkActor create_coord(IEnumerable<Point2D> inp, double dia)
        {
            vtkPoints pts = vtkPoints.New();
            vtkPolyLine pl = vtkPolyLine.New();
            pl.GetPointIds().SetNumberOfIds(inp.Count());
            int i = 0;
            foreach (var item in inp)
            {
                pts.InsertPoint(i, item.X, 0, item.Y);
                pl.GetPointIds().SetId(i, i);
                i++;
            }

            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(pl);

            // Create a polydata to store everything in
            vtkPolyData polyData = vtkPolyData.New();
            polyData.SetPoints(pts);
            polyData.SetLines(cells);

            vtkTubeFilter tubes = vtkTubeFilter.New();
            tubes.SetInput(polyData);
            tubes.SetRadius(0.5 * dia);
            tubes.SetNumberOfSides(24);
            tubes.CappingOn();
            tubes.Update();

            vtkPolyDataMapper mapper2 = vtkPolyDataMapper.New();
            mapper2.SetInputConnection(tubes.GetOutputPort());
            vtkActor actor2 = vtkActor.New();
            actor2.SetMapper(mapper2);

            return actor2;

        }

        private string GetNodeText(string txt, out double val)
        {
            string newT;

            if (txt.Contains("="))
            {
                newT = Regex.Split(txt, @"\s?=")[0];
                val = double.Parse(Regex.Split(txt, @"\s?=")[1].Split(')')[0]);
            }
            else
            {
                newT = txt.Split(')')[0];
                val = double.NaN;
            }

            return newT;
        }

        private void UpdateBounds(vtkActor act)
        {
            double[] bounds = act.GetBounds();
            var gchX = default(GCHandle);
            gchX = GCHandle.Alloc(bounds, GCHandleType.Pinned);
            viewBounding.AddBounds(gchX.AddrOfPinnedObject());
        }

        private double getPara(int id)
        {
            var childList = theParas.FindAll(t => t.Id == id).ToList();
            return childList[0].Value;
        }

        private void DataBindTree(TreeNode parNode, List<Parameter> Plist, int nodeId)
        {
            var childList = Plist.FindAll(t => t.ParentId == nodeId).OrderBy(t => t.Id);
            foreach (var childNode in childList)
            {
                var node = new TreeNode();
                node.Name = childNode.Name;
                node.Text = childNode.Text;
                parNode.Nodes.Add(node);
                DataBindTree(node, Plist, childNode.Id);
            }
        }
        private void Tree2Data()
        {

            foreach (var item in theParas)
            {
                if (item.Id > 9)
                {

                    var nd = SearchNode(paraTree.Nodes, item.Name);
                    item.Value = getValue(nd.Text);
                }

            }
        }

        private TreeNode SearchNode(TreeNodeCollection nds, string Name)
        {
            TreeNode ret;
            ret = nds[Name];

            if (ret != null)
            {
                return ret;
            }
            else
            {
                foreach (TreeNode item in nds)
                {
                    ret = SearchNode(item.Nodes, Name);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }

        private double getValue(string NdText)
        {
            if (NdText.Contains("="))
            {
                return double.Parse(Regex.Split(NdText, @"\s?=")[1].Split(')')[0]);
            }
            else
            {
                return -1;
            }
        }

        #endregion

        private void paraTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level > 1)
            {
                double val;
                string newT = GetNodeText(e.Node.Text, out val);
                SetValue form = new SetValue(newT + ")", val);
                form.ShowDialog();
                if (!double.IsNaN(form.paraValue))
                {
                    newT += string.Format(" = {0:F1})", form.paraValue);
                }
                else
                {
                    newT += ")";
                }

                e.Node.Text = newT;
                TreeView theTree = sender as TreeView;
                theTree.SelectedNode = null;
                Tree2Data();
            }

        }

        #region 文件操作
        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dia = new OpenFileDialog();
            dia.DefaultExt = "json";
            dia.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            if (dia.ShowDialog() == DialogResult.OK)
            {
                string fileName = dia.FileName;
                byte[] jsonString = File.ReadAllBytes(fileName);
                List<Parameter> theParasNew = JsonSerializer.Deserialize<List<Parameter>>(jsonString);
                theParas = theParasNew;
                paraTree.Nodes.Clear();
                var topNode = new TreeNode();
                topNode.Name = "0";
                topNode.Text = "参数表";
                paraTree.Nodes.Add(topNode);
                DataBindTree(topNode, theParas, 0);
                paraTree.ExpandAll();
            }

        }
        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dia = new SaveFileDialog();
            dia.DefaultExt = "json";
            dia.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            dia.OverwritePrompt = true;
            if (dia.ShowDialog() == DialogResult.OK)
            {
                string fileName = dia.FileName;
                byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes(theParas);
                File.WriteAllBytes(fileName, jsonString);
            }


        }

        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow_Load(sender, e);
        }
        #endregion

        #region 视图控制
        private void btOrthView_Click(object sender, EventArgs e)
        {
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera cam = renderer.GetActiveCamera();
            cam.ParallelProjectionOn();
            vtkWindow.RenderWindow.Render();
            cam.Zoom(1.0);
        }

        private void btPerspView_Click(object sender, EventArgs e)
        {
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera cam = renderer.GetActiveCamera();
            cam.ParallelProjectionOff();
            vtkWindow.RenderWindow.Render();
            cam.Zoom(1.0);
        }

        private void btFrontView_Click(object sender, EventArgs e)
        {
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera cam = renderer.GetActiveCamera();
            cam.ParallelProjectionOn();
            Point3D Foc = viewBounding.GetCenter();
            cam.SetFocalPoint(Foc.X, Foc.Y, Foc.Z);
            cam.SetPosition(0, -1000, 0);
            cam.SetViewUp(0, 0, 1);
            cam.SetClippingRange(1e-3, 1e10);
            btFit_Click(sender, e);
        }

        private void btTopView_Click(object sender, EventArgs e)
        {
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera cam = renderer.GetActiveCamera();
            cam.ParallelProjectionOn();
            Point3D Foc = viewBounding.GetCenter();
            cam.SetFocalPoint(Foc.X, Foc.Y, Foc.Z);
            cam.SetPosition(0, 0, 1000);
            cam.SetViewUp(0, -1, 0);
            cam.SetClippingRange(1e-3, 1e10);
            btFit_Click(sender, e);
        }

        private void btLeftView_Click(object sender, EventArgs e)
        {
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera cam = renderer.GetActiveCamera();
            cam.ParallelProjectionOn();
            Point3D Foc = viewBounding.GetCenter();
            cam.SetFocalPoint(Foc.X, Foc.Y, Foc.Z);
            cam.SetPosition(-1000, 0, 0);
            cam.SetViewUp(0, 0, 1);
            cam.SetClippingRange(1e-3, 1e10);
            btFit_Click(sender, e);
        }

        private void btFit_Click(object sender, EventArgs e)
        {
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera cam = renderer.GetActiveCamera();
            var bd = viewBounding.GetBoundsPts().ToList();
            var bdArr = from pt in bd select new double[3] { pt.X, pt.Y, pt.Z };
            var bdView = new List<double>();
            foreach (var item in bdArr)
            {
                renderer.WorldToView(ref item[0], ref item[1], ref item[2]);
                bdView.Add(Math.Abs(item[0]));
                bdView.Add(Math.Abs(item[1]));

            }
            double scale = cam.GetParallelScale() * bdView.Max() * 1.1;
            cam.SetParallelScale(scale);
            vtkWindow.RenderWindow.Render();
        }

        #endregion

        #region 输出      

        private void ansysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnsysExt ansysExt = new AnsysExt(theFem);

            FolderBrowserDialog dia = new FolderBrowserDialog();

            if (dia.ShowDialog() == DialogResult.OK)
            {
                string fileName = dia.SelectedPath;
                ansysExt.WriteAnsys(fileName);
                MessageBox.Show("Ansys命令流生成成功！", "通知", MessageBoxButtons.OK);
            }
        }

        private void midasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MidasExt midasExt = new MidasExt(theFem);

            FolderBrowserDialog dia = new FolderBrowserDialog();

            if (dia.ShowDialog() == DialogResult.OK)
            {
                string savePath = dia.SelectedPath;

                midasExt.WriteMidas(savePath);
                midasExt.WriteNodeInfo(Path.Combine(savePath, "NodeInfomation.csv"));

                MessageBox.Show("Ansys命令流生成成功！", "通知", MessageBoxButtons.OK);
            }

        }
        #endregion
    }
}
