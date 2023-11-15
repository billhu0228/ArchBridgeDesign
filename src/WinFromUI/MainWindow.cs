using AnsysInterface;
using Kitware.VTK;
using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace WinFromUI
{
    public partial class MainWindow : Form
    {
        FEMModel theFem = null;
        FEMDeck theFEMDeckA1 = null;
        FEMDeck theFEMDeckA2 = null;
        ArchAxis ax = null;
        Arch theArchModel = null;
        List<Parameter> theParas;
        // ArchParameters paras;
        vtkBox viewBounding;
        ModelStatus status;
        PropertyViewWindow PropertyView;

        public MainWindow()
        {
            InitializeComponent();
            viewBounding = new vtkBox();
            status = new ModelStatus();
            PropertyView = new PropertyViewWindow();
            status.SetStatus(false);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            theParas = new List<Parameter>()
            {
                new Parameter() {Id = 1, Name = "截面参数", Value = -1, ParentId = 0},
                new Parameter() {Id = 2, Name = "拱轴参数", Value = -1, ParentId = 0},
                new Parameter() {Id = 3, Name = "拱肋参数", Value = -1, ParentId = 0},
                new Parameter() {Id = 4, Name = "构造参数", Value = -1, ParentId = 0},

                new Parameter() {Id = 11, Name = "主桁参数", Value = -1, ParentId = 1},
                new Parameter() {Id = 12, Name = "横梁参数", Value = -1, ParentId = 1},
                new Parameter() {Id = 13, Name = "内隔参数", Value = -1, ParentId = 1},
                new Parameter() {Id = 14, Name = "平联参数", Value = -1, ParentId = 1},
                new Parameter() {Id = 15, Name = "立柱参数", Value = -1, ParentId = 1},

                new Parameter() {Id = 101, Name = "主管直径(D0)", Value = -1, ParentId = 11},
                new Parameter() {Id = 121, Name = "拱脚腹杆直径(D21)", Value = -1, ParentId = 11},
                new Parameter() {Id = 122, Name = "普通腹杆直径(D22)", Value = -1, ParentId = 11},
                new Parameter() {Id = 123, Name = "拼装腹杆直径(D23)", Value = -1, ParentId = 11},

                new Parameter() {Id = 131, Name = "内隔直径(D31)", Value = -1, ParentId = 13}, //
                new Parameter() {Id = 132, Name = "拼接内隔直径(D32)", Value = -1, ParentId = 13},
                new Parameter() {Id = 133, Name = "内隔腹杆直径(D33)", Value = -1, ParentId = 13},


                new Parameter() {Id = 141, Name = "横梁弦杆直径(D41)", Value = -1, ParentId = 12},
                new Parameter() {Id = 142, Name = "横梁腹杆直径(D42)", Value = -1, ParentId = 12},
                new Parameter() {Id = 143, Name = "横梁双腹杆直径(D43)", Value = -1, ParentId = 12},

                new Parameter() {Id = 151, Name = "平联直径(D51)", Value = -1, ParentId = 14},
                new Parameter() {Id = 152, Name = "平联斜杆直径(D52)", Value = -1, ParentId = 14},
                new Parameter() {Id = 153, Name = "合龙口平联直径(D53)", Value = -1, ParentId = 14},

                new Parameter() {Id=161,Name="高立柱截面(D61)",Value=-1,ParentId=15 },
                new Parameter() {Id=162,Name="一般立柱截面(D62)",Value=-1,ParentId=15 },
                new Parameter() {Id=163,Name="立柱横杆截面(D63)",Value=-1,ParentId=15 },

                new Parameter() {Id = 21, Name = "跨径(L)", Value = -1, ParentId = 2},
                new Parameter() {Id = 22, Name = "拱轴系数(m)", Value = -1, ParentId = 2},
                new Parameter() {Id = 23, Name = "跨矢比(L/f)", Value = -1, ParentId = 2},
                new Parameter() {Id = 31, Name = "拱脚高度(H0)", Value = -1, ParentId = 3},
                new Parameter() {Id = 32, Name = "拱顶高度(H1)", Value = -1, ParentId = 3},
                new Parameter() {Id = 33, Name = "拱肋宽度(W0)", Value = -1, ParentId = 3},
                new Parameter() {Id = 34, Name = "拱肋间距(W1)", Value = -1, ParentId = 3},
            };
            var topNode = paraTree.TopNode;
            topNode.Nodes.Clear();
            DataBindTree(topNode, theParas, 0);
            paraTree.ExpandAll();

            theArchModel = null;
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            renderer.RemoveAllViewProps();
            btFrontView_Click(sender, e);

            status.ChangeStatus += new ModelStatus.StatusHandler(OutputEnable);
        }



        private void OutputEnable()
        {
            ansysToolStripMenuItem.Enabled = true;
            midasToolStripMenuItem.Enabled = true;
            spaceClaimToolStripMenuItem.Enabled = true;
            ;
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

            #region 读取参数
            double D0 = getPara(101);
            double Hfoot = getPara(31);
            double Htop = getPara(32);
            double L = getPara(21);
            double m = getPara(22);
            double f = getPara(23);
            double W1 = getPara(34);
            double W0 = getPara(33);
            #endregion

            pBar1.PerformStep();

            #region 上部结构
            CompositeDeck DeckA;
            CompositeDeck DeckB;
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1;
            double DeckElevation = 1281.3 + L / f + 10;

            sps1 = new List<double>() { -272.25, -222.75, -173.25, -123.75, -74.25, -24.75, 24.75, 74.25, 123.75, 173.25, 222.75, 272.25, };
            DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
            DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
            double CrossBeamDist = 4.95;

            var Decks = new List<CompositeDeck>() { DeckA, DeckB };
            foreach (var item in Decks)
            {
                item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.7, 0.060, 0.060, 0.020));
                item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                item.AddSection("EndBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
                item.AddSection("UpBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
            }
            theFEMDeckA1 = new FEMDeck(ref DeckA, 200000, 200000, CrossBeamDist, 2.5, 0, 1.95 + ((W1 + W0) * 0.5 - 9), 11);
            theFEMDeckA2 = new FEMDeck(ref DeckB, 300000, 300000, CrossBeamDist, 2.5, 0, -14.5 - ((W1 + W0) * 0.5 - 9), 11);
            #endregion



            theArchModel = NamedArch.PhoenixModelV7(out ax, L, m, L / f, Hfoot, Htop, W0, W1, DeckElevation);
            pBar1.Step = 5;
            pBar1.PerformStep();

            theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            status.SetStatus(true);
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
                if (item.Secn <= 63 && item.Secn >= 21)
                {
                    dia = getPara(100 + item.Secn);
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

            foreach (var item in theFEMDeckA1.ElementList)
            {
                vtkActor actor;
                if (item.GetType() != typeof(FEMElement4))
                {
                    Point3D[] eles = new Point3D[2] { theFEMDeckA1.GetPoint(item.Ni), theFEMDeckA1.GetPoint(item.Nj) };
                    double[] Sectiondata = new double[6] { 0.3, 0.3, 0.7, 0.015, 0.015, 0.015 };
                    actor = Create_HElement(eles, Sectiondata);
                }
                else
                {
                    var ele = (FEMElement4)item;
                    Point3D[] pts = new Point3D[4] {
                        theFEMDeckA1.GetPoint(ele.Ni),
                        theFEMDeckA1.GetPoint(ele.Nj) ,
                        theFEMDeckA1.GetPoint(ele.Nk) ,
                        theFEMDeckA1.GetPoint(ele.Nl) ,
                    };
                    actor = CreateCubeElement(pts);
                }
                this.UpdateBounds(actor);
                renderer.AddActor(actor);
            }

            foreach (var item in theFEMDeckA2.ElementList)
            {
                vtkActor actor;
                if (item.GetType() != typeof(FEMElement4))
                {
                    Point3D[] eles = new Point3D[2] { theFEMDeckA2.GetPoint(item.Ni), theFEMDeckA2.GetPoint(item.Nj) };
                    double[] Sectiondata = new double[6] { 0.3, 0.3, 0.7, 0.015, 0.015, 0.015 };
                    actor = Create_HElement(eles, Sectiondata);

                }
                else
                {
                    var ele = (FEMElement4)item;
                    Point3D[] pts = new Point3D[4] {
                        theFEMDeckA2.GetPoint(ele.Ni),
                        theFEMDeckA2.GetPoint(ele.Nj) ,
                        theFEMDeckA2.GetPoint(ele.Nk) ,
                        theFEMDeckA2.GetPoint(ele.Nl) ,
                    };
                    actor = CreateCubeElement(pts);
                }
                this.UpdateBounds(actor);
                renderer.AddActor(actor);

            }


            MessageBox.Show("模型生成成功！", "通知", MessageBoxButtons.OK);
            pBar1.Value = 1;

            // btFrontView_Click(sender, e);

            btFit_Click(sender, e);


        }


        private void renderWindowControl1_Load(object sender, EventArgs e)
        {

            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(.2, .3, .4);
            //  renderer.SetBackground(1,1,1);
        }
        #region 普通方法

        private vtkTransform LocalToWorld(Point3D origin, Vector3D xDir, Vector3D yDir, Vector3D zDir)
        {
            xDir = xDir.Normalize().ToVector3D();
            yDir = yDir.Normalize().ToVector3D();
            zDir = zDir.Normalize().ToVector3D();
            double[] A = new double[16] {
                xDir.X, yDir.X, zDir.X, origin.X,
                xDir.Y, yDir.Y, zDir.Y, origin.Y,
                xDir.Z, yDir.Z, zDir.Z, origin.Z,
                0, 0, 0, 1 };
            vtkMatrix4x4 mat = new vtkMatrix4x4();

            var gchX = default(GCHandle);
            gchX = GCHandle.Alloc(A, GCHandleType.Pinned);
            mat.DeepCopy(gchX.AddrOfPinnedObject());
            gchX.Free();
            vtkTransform trans = new vtkTransform();
            trans.SetMatrix(mat);
            return trans;
        }

        private vtkActor CreateCubeElement(Point3D[] inp)
        {
            Point3D Ni = new Point3D(inp[0].X, inp[0].Z, inp[0].Y);
            Point3D Nj = new Point3D(inp[1].X, inp[1].Z, inp[1].Y);
            Point3D Nk = new Point3D(inp[2].X, inp[2].Z, inp[2].Y);
            Point3D Nl = new Point3D(inp[3].X, inp[3].Z, inp[3].Y);

            var xs = (from pt in inp select pt.X).ToList();
            var zs = (from pt in inp select pt.Y).ToList();
            var ys = (from pt in inp select pt.Z).ToList();

            vtkCubeSource cb = new vtkCubeSource();
            cb.SetXLength(xs.Max() - xs.Min());
            cb.SetYLength(ys.Max() - ys.Min());
            cb.SetZLength(0.30);


            vtkPolyDataMapper mapper2 = vtkPolyDataMapper.New();
            mapper2.SetInputConnection(cb.GetOutputPort());
            vtkActor actor2 = vtkActor.New();
            actor2.SetMapper(mapper2);
            actor2.SetPosition(xs.Average(), ys.Average(), zs.Average());
            return actor2;
        }


        private vtkActor Create_HElement(IEnumerable<Point3D> inp, double[] s)
        {
            Point3D Ni = new Point3D(inp.First().X, inp.First().Z, inp.First().Y);
            Point3D Nj = new Point3D(inp.Last().X, inp.Last().Z, inp.Last().Y);
            Vector3D xD = Nj - Ni;
            xD = xD.Normalize().ToVector3D();
            Vector3D zD = new Vector3D(0, 0, 1);
            Vector3D yD = xD.CrossProduct(-zD);
            var trans = LocalToWorld(Ni, xD, yD, zD);
            vtkPoints pts = vtkPoints.New();
            vtkPoints pts_out = vtkPoints.New();
            double w1 = s[0];
            double w2 = s[1];
            double w3 = s[2];
            double t1 = s[3];
            double t2 = s[4];
            double t3 = s[5];
            pts.InsertPoint(0, 0, 0.5 * w2, 0);
            pts.InsertPoint(1, 0, 0.5 * w2, -t2);
            pts.InsertPoint(2, 0, 0.5 * t3, -t2);
            pts.InsertPoint(3, 0, 0.5 * t3, -w3 + t1);
            pts.InsertPoint(4, 0, 0.5 * w1, -w3 + t1);
            pts.InsertPoint(5, 0, 0.5 * w1, -w3);
            pts.InsertPoint(6, 0, -0.5 * w1, -w3);
            pts.InsertPoint(7, 0, -0.5 * w1, -w3 + t1);
            pts.InsertPoint(8, 0, -0.5 * t3, -w3 + t1);
            pts.InsertPoint(9, 0, -0.5 * t3, -t2);
            pts.InsertPoint(10, 0, -0.5 * w2, -t2);
            pts.InsertPoint(11, 0, -0.5 * w2, 0);
            pts.InsertPoint(12, 0, 0.5 * w2, 0);

            trans.TransformPoints(pts, pts_out);

            vtkPolyLine pl = vtkPolyLine.New();
            pl.GetPointIds().SetNumberOfIds(13);
            for (int i = 0; i < 13; i++)
            {
                pl.GetPointIds().SetId(i, i);
            }
            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(pl);
            vtkPolyData polyData = vtkPolyData.New();
            polyData.SetPoints(pts_out);
            polyData.SetLines(cells);

            vtkLinearExtrusionFilter ext = new vtkLinearExtrusionFilter();
            ext.SetInput(polyData);
            ext.SetExtrusionTypeToVectorExtrusion();
            ext.SetScaleFactor(Nj.DistanceTo(Ni));
            ext.SetVector(xD.X, xD.Y, xD.Z);
            ext.CappingOn();
            ext.SetCapping(1);
            ext.Update();

            vtkPolyDataMapper mapper2 = vtkPolyDataMapper.New();
            mapper2.SetInputConnection(ext.GetOutputPort());
            vtkActor actor2 = vtkActor.New();
            actor2.SetMapper(mapper2);
            return actor2;
        }

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
            gchX.Free();
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
            if (e.Node.Nodes.Count == 0)
            {
                double val;
                string newT = GetNodeText(e.Node.Text, out val);
                SetValue form = new SetValue(newT + ")", val);
                DialogResult rr = form.ShowDialog();
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
                if (rr==DialogResult.Yes)
                {
                    btGenerateMd_Click(sender, e);
                }
            }

        }

        #region 文件操作
        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dia = new OpenFileDialog();
            //dia.DefaultExt = "json";
            //dia.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            //if (dia.ShowDialog() == DialogResult.OK)
            //{
            //    string fileName = dia.FileName;
            //    string jsonString = File.ReadAllText(fileName);
            //    JsonSerializerOptions opt = new JsonSerializerOptions()
            //    {
            //        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            //        WriteIndented = true,
            //    };
            //    List<Parameter> theParasNew = JsonSerializer.Deserialize<List<Parameter>>(jsonString,opt);
            //    theParas = theParasNew;
            //    paraTree.Nodes.Clear();
            //    var topNode = new TreeNode();
            //    topNode.Name = "0";
            //    topNode.Text = "参数表";
            //    paraTree.Nodes.Add(topNode);
            //    DataBindTree(topNode, theParas, 0);
            //    paraTree.ExpandAll();
            //}

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
                JsonSerializerOptions opt = new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true,
                };
                byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes(theParas, opt);
                byte[] jsonString2 = JsonSerializer.SerializeToUtf8Bytes(theParas);
                string result = Encoding.UTF8.GetString(jsonString);
                // var s = jsonString.ToString();
                System.IO.File.WriteAllText(fileName, result);
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
            cam.SetPosition(Foc.X, Foc.Y - 1000, Foc.Z);
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
            cam.SetPosition(Foc.X, Foc.Y, Foc.Z + 1000);
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
            cam.SetPosition(Foc.X - 1000, Foc.Y, Foc.Z);
            cam.SetViewUp(0, 0, 1);
            cam.SetClippingRange(1e-3, 1e10);
            btFit_Click(sender, e);
        }

        private void btRightView_Click(object sender, EventArgs e)
        {
            vtkRenderer renderer = vtkWindow.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera cam = renderer.GetActiveCamera();
            cam.ParallelProjectionOn();
            Point3D Foc = viewBounding.GetCenter();
            cam.SetFocalPoint(Foc.X, Foc.Y, Foc.Z);
            cam.SetPosition(Foc.X + 1000, Foc.Y, Foc.Z);
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
                AnsysDeckExt ansysDeck = new AnsysDeckExt(theFEMDeckA1);
                ansysDeck.WriteAnsys(fileName, "DeckA.inp");
                ansysDeck = new AnsysDeckExt(theFEMDeckA2);
                ansysDeck.WriteAnsys(fileName, "DeckB.inp");
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

        private void TSI_V6_Click(object sender, EventArgs e)
        {
            byte[] jsonString = Properties.Resources.v6_json;
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

        private void TSI_About_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void 截面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PropertyView.ShowDialog();
        }

        private void oSISToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OsisExt osisExt = new OsisExt(theFem);
            
            SaveFileDialog dia = new SaveFileDialog();
            dia.DefaultExt = "sml";
            dia.Filter = "OSIS命令流文件 (*.sml)|*.sml|All files (*.*)|*.*";
            dia.OverwritePrompt = true;
            if (dia.ShowDialog() == DialogResult.OK)
            {
                string fileName = dia.FileName;
                osisExt.WriteSML(fileName);
                MessageBox.Show("OSIS命令流文件生成！", "通知", MessageBoxButtons.OK);
            }
        }
    }
}
