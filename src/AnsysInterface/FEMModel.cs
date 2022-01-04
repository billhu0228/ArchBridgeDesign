using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AnsysInterface
{
    public class FEMModel
    {
        public List<FEMNode> NodeList;
        public List<FEMElement> ElementList;
        public Arch RelatedArchBridge;

        public FEMModel()
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            RelatedArchBridge = null;
        }

        public FEMModel(ref Arch theArch)
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            RelatedArchBridge = theArch;

            GenerateRib();

            GenerateCol();
        }
        /// <summary>
        /// 生成立柱
        /// </summary>
        private void GenerateCol()
        {
            foreach (var col in RelatedArchBridge.ColumnList)
            {
                int baseID = col.ID * 1000 + 100000 + 1;
                var wi = RelatedArchBridge.WidthInside;
                var wo = RelatedArchBridge.WidthOutside;
                var x0 = col.X;
                List<double> ylist = new List<double>();
                ylist.Add(col.Z2);
                ylist.Add(col.Z2 - 2.8);


                for (int i = 0; i < col.M; i++)
                {
                    ylist.Add(ylist.Last() - col.BeamStep);
                }

                if (col.C != 0)
                {
                    ylist.Add(ylist.Last() - col.BeamStep);
                }
                for (int i = 0; i < col.K; i++)
                {
                    ylist.Add(ylist.Last() - col.BeamStep);
                }


                foreach (var yi in ylist)
                {
                    foreach (var xi in new double[] { (x0 - col.L * 0.5), (x0 + col.L * 0.5) })
                    {
                        foreach (var zi in new double[] { (-0.5 * wi - wo), (-0.5 * wi), (0.5 * wi), (0.5 * wi + wo), })
                        {

                            NodeList.Add(new FEMNode(baseID, new Point3D(xi, yi, zi)));
                            if (yi != ylist.First())
                            {
                                ElementList.Add(new FEMElement(baseID, baseID - 8, baseID, 8));
                            }
                            baseID += 1;
                        }
                    }
                    ElementList.Add(new FEMElement(0, baseID - 8 + 0, baseID - 8 + 1, 7));
                    ElementList.Add(new FEMElement(0, baseID - 8 + 1, baseID - 8 + 5, 7));
                    ElementList.Add(new FEMElement(0, baseID - 8 + 5, baseID - 8 + 4, 7));
                    ElementList.Add(new FEMElement(0, baseID - 8 + 4, baseID - 8 + 0, 7));

                    ElementList.Add(new FEMElement(0, baseID - 8 + 2, baseID - 8 + 3, 7));
                    ElementList.Add(new FEMElement(0, baseID - 8 + 3, baseID - 8 + 7, 7));
                    ElementList.Add(new FEMElement(0, baseID - 8 + 7, baseID - 8 + 6, 7));
                    ElementList.Add(new FEMElement(0, baseID - 8 + 6, baseID - 8 + 2, 7));
                }
            }
            return;
        }

        /// <summary>
        /// 生成拱肋节点
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void GenerateRib()
        {

            var theArch = RelatedArchBridge;
            List<Point2D> ls = new List<Point2D>();


            var UppNodeFromIncline = (from e in theArch.MemberTable where e.ElemType == eMemberType.InclineWeb select e.Line.UperPT()).ToList();
            var LowerNodeFromIncline = (from e in theArch.MemberTable where e.ElemType == eMemberType.InclineWeb select e.Line.LowerPT()).ToList();

            // 上下
            foreach (var et in new eMemberType[] { eMemberType.UpperCoord, eMemberType.LowerCoord })
            {
                int ul = et == eMemberType.UpperCoord ? 1 : 2;
                var kk0 = et == eMemberType.UpperCoord ? UppNodeFromIncline : LowerNodeFromIncline;
                var kk1 = (from item in theArch.MemberTable where item.ElemType == et select item.Line.StartPoint).ToList();
                var kk2 = (from item in theArch.MemberTable where item.ElemType == et select item.Line.EndPoint).ToList();
                ls = (kk1.Concat(kk2)).ToList();
                ls = (ls.Concat(kk0)).ToList();
                ls = ls.Distinct().ToList();
                ls.Sort((x, y) => x.X.CompareTo(y.X));

                int mID = 1;
                double baseZ = theArch.WidthOutside + theArch.WidthInside * 0.5;
                foreach (double z0 in new double[] { -baseZ, -baseZ + theArch.WidthOutside, +baseZ - theArch.WidthOutside, baseZ })
                {

                    int baseID = mID * 10000 + ul * 1000;
                    foreach (var item in ls)
                    {
                        NodeList.Add(new FEMNode(baseID, item.X, item.Y, z0));

                        if ((baseID) != mID * 10000 + ul * 1000)
                        {
                            ElementList.Add(new FEMElement(baseID - 1, baseID - 1, baseID, 1));
                        }
                        baseID += 1;
                    }
                    mID += 1;
                }
            }
            // 腹杆
            var eleSelected = (from e in theArch.MemberTable where e.ElemType == eMemberType.VerticalWeb || e.ElemType == eMemberType.InclineWeb select e).ToList();
            foreach (var elem in eleSelected)
            {
                var ni = NodeList.Find(x => x.Match(elem.Line.StartPoint));
                var nj = NodeList.Find(x => x.Match(elem.Line.EndPoint));
                int sen = elem.ElemType == eMemberType.VerticalWeb ? 2 : 3;
                if (ni == null || nj == null)
                {
                    Debug.Write(elem.ToString());
                    continue;
                }

                ElementList.Add(new FEMElement(0, ni.ID, nj.ID, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 10000, nj.ID + 10000, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 20000, nj.ID + 20000, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 30000, nj.ID + 30000, sen));

                var ColumnX = (from e in RelatedArchBridge.ColumnList select e.X).ToList();
                List<double> MidColumnList = new List<double>();
                for (int i = 0; i < ColumnX.Count - 1; i++)
                {
                    MidColumnList.Add(ColumnX[i] + 21.0);
                }
                MidColumnList.Add(244.26);
                MidColumnList.Add(-244.26);
                if (elem.ElemType == eMemberType.VerticalWeb)
                {
                    if (Contains(ref ColumnX, elem.Line.StartPoint.X, 1e-3))
                    {
                        // 横撑
                        MakeCrossBeam(ni.ID + 10000, ni.ID + 20000, nj.ID + 10000, nj.ID + 20000, 4);

                    }
                    if (Contains(ref MidColumnList, elem.Line.StartPoint.X, 0.80))
                    {
                        // 横撑
                        MakeCrossBeam(ni.ID + 10000, ni.ID + 20000, nj.ID + 10000, nj.ID + 20000, 4);

                    }

                    // 水平横撑




                }

            }
            // 三角腹杆
            eleSelected = (from e in theArch.MemberTable where e.ElemType == eMemberType.InclineWebS select e).ToList();
            int addID = 80000;
            int ofId = addID;
            foreach (var item in eleSelected)
            {
                cutExistElem(item.Line, ref addID);
                ;
            }
            // 协调节点

            for (int i = 0; i < addID - ofId; i++)
            {
                FEMNode nd = NodeList.Find(x => x.ID == ofId + i);
                var esel = (from e in ElementList where e.Include(nd, ref NodeList) select e).ToList();
                if (esel.Count() == 1)
                {
                    var ee = esel[0];
                    ElementList.Add(new FEMElement(0, ee.Ni, nd.ID, ee.Secn));
                    ee.Ni = nd.ID;
                }
                ;
            }
            // K撑
            MakeMCross(6, false);
            // 内横隔
            eleSelected = (from e in theArch.MemberTable where e.ElemType == eMemberType.VerticalWeb select e).ToList();
            foreach (var elem in eleSelected)
            {
                var ni = NodeList.Find(x => x.Match(elem.Line.StartPoint));
                var frameID = ni.ID % 10000 % 1000;
                MakeDiagram(frameID, 7, false);
            }


        }
        /// <summary>
        /// 生成内横隔
        /// </summary>
        /// <param name="ni"></param>
        private void MakeDiagram(int frameID, int secn, bool Strong)
        {

            ElementList.Add(new FEMElement(0, 11000 + frameID, 21000 + frameID, secn));
            ElementList.Add(new FEMElement(0, 12000 + frameID, 22000 + frameID, secn));
            ElementList.Add(new FEMElement(0, 31000 + frameID, 41000 + frameID, secn));
            ElementList.Add(new FEMElement(0, 32000 + frameID, 42000 + frameID, secn));

            if (new int[] { 2, 3, 232, 233 }.Contains(frameID))
            {
                return;
            }
            Vector3D dir = GetNode(12000 + frameID).location - GetNode(11000 + frameID).location;
            double h = dir.Length;
            if (h < 10)
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 0.5 * dir));
                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                if (Strong)
                {
                    ElementList.Add(new FEMElement(0, 20000 + 1000 + frameID, 10000 + 3000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 20000 + 2000 + frameID, 10000 + 3000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 30000 + 1000 + frameID, 40000 + 3000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 30000 + 2000 + frameID, 40000 + 3000 + frameID, secn));
                }

            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 0.25 * dir));
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 0.5 * dir));
                    NodeList.Add(new FEMNode(i * 10000 + 5000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 0.75 * dir));

                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 4000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 3000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 5000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 5000 + frameID, i * 10000 + 2000 + frameID, e.Secn));

                }
                if (Strong)
                {
                    ElementList.Add(new FEMElement(0, 21000 + frameID, 14000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 14000 + frameID, 23000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 23000 + frameID, 15000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 15000 + frameID, 22000 + frameID, secn));

                    ElementList.Add(new FEMElement(0, 31000 + frameID, 44000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 44000 + frameID, 33000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 33000 + frameID, 45000 + frameID, secn));
                    ElementList.Add(new FEMElement(0, 45000 + frameID, 32000 + frameID, secn));
                }


            }

        }

        private void MakeMCross(int secn, bool AddI)
        {
            var mpt = (from n in NodeList where n.ID > 70000 select n).ToList();

            foreach (var item in mpt)
            {
                int frame = (item.ID % 10000) % 1000;

                var Except = new int[] { 156, 5, 79, 230 };
                if (Except.Contains(frame))
                {
                    continue;
                }
                int ui = (int)((item.ID % 10000) / 1000);
                if (ui == 1 || ui == 2)
                {
                    if (GetNode(item.ID).X > 0)
                    {
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 + 3, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 + 3, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 + 3, item.ID - 40000 + 3, secn));
                        }

                    }
                    else
                    {
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 - 3, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 - 3, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 - 3, item.ID - 40000 - 3, secn));
                        }
                    }
                }


            }

            foreach (var item in mpt)
            {
                int frame = (item.ID % 10000) % 1000;
                if (frame == 79)
                {
                    ;
                }
                var Except = new int[] { 117, 118, 231, 4, 5, 230, 157, 78 };
                var reverse = new int[] { };
                if (Except.Contains(frame))
                {
                    continue;
                }
                int ui = (int)((item.ID % 10000) / 1000);
                if (ui == 1 || ui == 2)
                {
                    if (GetNode(item.ID).X > 0)
                    {
                        if (reverse.Contains(frame))
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 1, item.ID - 50000 - 4, secn));
                            ElementList.Add(new FEMElement(0, item.ID - 1, item.ID - 40000 - 4, secn));
                            if (AddI)
                            {
                                ElementList.Add(new FEMElement(0, item.ID - 50000 - 4, item.ID - 40000 - 4, secn));
                            }
                        }
                        else
                        {
                            ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 - 3, secn));
                            ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 - 3, secn));
                            if (AddI)
                            {
                                ElementList.Add(new FEMElement(0, item.ID - 50000 - 3, item.ID - 40000 - 3, secn));
                            }
                        }



                    }
                    else
                    {
                        if (reverse.Contains(frame))
                        {
                            ElementList.Add(new FEMElement(0, item.ID + 1, item.ID - 50000 - 2, secn));
                            ElementList.Add(new FEMElement(0, item.ID + 1, item.ID - 40000 - 2, secn));
                            if (AddI)
                            {
                                ElementList.Add(new FEMElement(0, item.ID - 50000 - 2, item.ID - 40000 - 2, secn));
                            }
                        }
                        else
                        {
                            ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 + 3, secn));
                            ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 + 3, secn));
                            if (AddI)
                            {
                                ElementList.Add(new FEMElement(0, item.ID - 50000 + 3, item.ID - 40000 + 3, secn));
                            }

                        }


                    }
                }


            }

        }

        private void MakeKCross(int secn, bool AddI)
        {
            var mpt = (from n in NodeList where n.ID > 70000 select n).ToList();

            foreach (var item in mpt)
            {
                int frame = (item.ID % 10000) % 1000;
                var Except = new int[] { 156, 5, 79, 230 };
                if (Except.Contains(frame))
                {
                    continue;
                }
                int ui = (int)((item.ID % 10000) / 1000);
                if (ui == 1)
                {
                    if (GetNode(item.ID).X > 0)
                    {
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 + 3, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 + 3, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 + 3, item.ID - 40000 + 3, secn));
                        }

                    }
                    else
                    {
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 - 3, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 - 3, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 - 3, item.ID - 40000 - 3, secn));
                        }
                    }
                }


            }
        }

        private void MakeCrossBeam(int p1, int p2, int p3, int p4, int secn)
        {
            var ptA = GetNode(p1).location;
            var ptB = GetNode(p2).location;
            var ptC = GetNode(p3).location;
            var ptD = GetNode(p4).location;
            double offset = 1.2;
            NodeList.Add(new FEMNode(p1 + 30000, ptA.X, ptA.Y, ptA.Z + offset));
            NodeList.Add(new FEMNode(p2 + 30000, ptB.X, ptB.Y, ptB.Z - offset));
            NodeList.Add(new FEMNode(p3 + 30000, ptC.X, ptC.Y, ptC.Z + offset));
            NodeList.Add(new FEMNode(p4 + 30000, ptD.X, ptD.Y, ptD.Z - offset));

            NodeList.Add(new FEMNode(p1 + 50000, ptA.X, ptA.Y, ptA.Z + RelatedArchBridge.WidthInside * 0.5));
            NodeList.Add(new FEMNode(p3 + 50000, ptC.X, ptC.Y, ptC.Z + RelatedArchBridge.WidthInside * 0.5));

            ElementList.Add(new FEMElement(0, p1, p1 + 30000, secn));
            ElementList.Add(new FEMElement(0, p1 + 30000, p1 + 50000, secn));
            ElementList.Add(new FEMElement(0, p1 + 50000, p2 + 30000, secn));
            ElementList.Add(new FEMElement(0, p2 + 30000, p2, secn));

            ElementList.Add(new FEMElement(0, p3, p3 + 30000, secn));
            ElementList.Add(new FEMElement(0, p3 + 30000, p3 + 50000, secn));
            ElementList.Add(new FEMElement(0, p3 + 50000, p4 + 30000, secn));
            ElementList.Add(new FEMElement(0, p4 + 30000, p4, secn));
            // 交叉
            ElementList.Add(new FEMElement(0, p1 + 30000, p4 + 30000, secn));
            ElementList.Add(new FEMElement(0, p2 + 30000, p3 + 30000, secn));
            ElementList.Add(new FEMElement(0, p1 + 30000, p3 + 30000, secn));
            ElementList.Add(new FEMElement(0, p2 + 30000, p4 + 30000, secn));

        }
        private Point3D GetPoint(int p1)
        {
            return NodeList.Find(x => x.ID == p1).location;
        }
        private FEMNode GetNode(int p1)
        {
            return NodeList.Find(x => x.ID == p1);
        }

        private bool Contains(ref List<double> theList, double val, double tol)
        {
            foreach (var item in theList)
            {
                if (Math.Abs(val - item) < tol)
                {
                    return true;
                }
            }
            return false;
        }

        private void cutExistElem(Line2D item, ref int nodeID)
        {
            int curID = nodeID;
            double baseZ = RelatedArchBridge.WidthOutside + RelatedArchBridge.WidthInside * 0.5;
            double[] zs = new double[] { -baseZ, -baseZ + RelatedArchBridge.WidthOutside, +baseZ - RelatedArchBridge.WidthOutside, baseZ };
            List<int> nilist = new List<int>();
            List<int> njlist = new List<int>();
            if (NodeList.Find(x => x.Match(item.StartPoint)) == null)
            {
                NodeList.Add(new FEMNode(curID, item.StartPoint.X, item.StartPoint.Y, zs[0]));
                NodeList.Add(new FEMNode(curID + 1, item.StartPoint.X, item.StartPoint.Y, zs[1]));
                NodeList.Add(new FEMNode(curID + 2, item.StartPoint.X, item.StartPoint.Y, zs[2]));
                NodeList.Add(new FEMNode(curID + 3, item.StartPoint.X, item.StartPoint.Y, zs[3]));
                nilist = new List<int>() { curID + 0, curID + 1, curID + 2, curID + 3 };
                curID += 4;
            }
            else
            {
                var k = NodeList.Find(x => x.Match(item.StartPoint)).ID;
                nilist = new List<int>() { k + 0, k + 1, k + 2, k + 3 };
            }

            if (NodeList.Find(x => x.Match(item.EndPoint)) == null)
            {
                NodeList.Add(new FEMNode(curID + 0, item.EndPoint.X, item.EndPoint.Y, zs[0]));
                NodeList.Add(new FEMNode(curID + 1, item.EndPoint.X, item.EndPoint.Y, zs[1]));
                NodeList.Add(new FEMNode(curID + 2, item.EndPoint.X, item.EndPoint.Y, zs[2]));
                NodeList.Add(new FEMNode(curID + 3, item.EndPoint.X, item.EndPoint.Y, zs[3]));
                njlist = new List<int>() { curID + 0, curID + 1, curID + 2, curID + 3 };
                curID += 4;
            }
            else
            {
                var k = NodeList.Find(x => x.Match(item.EndPoint)).ID;
                njlist = new List<int>() { k + 0, k + 1, k + 2, k + 3 };
            }
            if (nilist.Contains(1011))
            {
                ;

            }
            for (int i = 0; i < 4; i++)
            {
                ElementList.Add(new FEMElement(0, nilist[i], njlist[i], 3));
            }

            nodeID = curID;

        }

        #region 写出Ansys输入文件
        public void WriteAnsys(string dirPath)
        {
            var cwd = Directory.CreateDirectory(dirPath);

            WriteMainInp(Path.Combine(cwd.FullName, "Main.inp"));
            WriteMaterial(Path.Combine(cwd.FullName, "material.inp"));
            WriteSection(Path.Combine(cwd.FullName, "section.inp"));
            WriteNode(Path.Combine(cwd.FullName, "node.inp"));
            WriteElem(Path.Combine(cwd.FullName, "element.inp"));
            WriteSolu(Path.Combine(cwd.FullName, "solu.inp"));
        }

        private void WriteNode(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false))
            {
                sw.WriteLine("/prep7");
                foreach (var item in NodeList)
                {
                    sw.WriteLine("n,{0},{1:F8},{2:F8},{3:F8}", item.ID, item.X * 1000, item.Y * 1000, item.Z * 1000);
                }
            }
        }

        private void WriteElem(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false))
            {
                sw.WriteLine("/prep7");
                var secn_list = (from FEMElement e in ElementList select e.Secn).ToList();
                secn_list = secn_list.Distinct().ToList();
                foreach (var secnn in secn_list)
                {
                    sw.WriteLine("secn,{0}", secnn);
                    var eles = (from e in ElementList where e.Secn == secnn select e).ToList();
                    foreach (var item in eles)
                    {
                        sw.WriteLine("e,{0},{1}", item.Ni, item.Nj);
                    }
                }

            }
        }

        private void WriteSection(string filepath)
        {

            var sect = (from e in RelatedArchBridge.MemberTable select e.Sect).ToList();
            sect = sect.Distinct().ToList();
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("SECTYPE,1,BEAM,CTUBE,Tube1,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 1500 * 0.5 - 35, 1500 * 0.5);
                sw.WriteLine("SECTYPE,2,BEAM,CTUBE,Tube2,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 800 * 0.5 - 24, 800 * 0.5);
                sw.WriteLine("SECTYPE,3,BEAM,CTUBE,Tube3,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 800 * 0.5 - 24, 800 * 0.5);
                sw.WriteLine("SECTYPE,4,BEAM,CTUBE,Tube4,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 800 * 0.5 - 24, 800 * 0.5);
                sw.WriteLine("SECTYPE,5,BEAM,CTUBE,Tube5,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 800 * 0.5 - 24, 800 * 0.5);
                sw.WriteLine("SECTYPE,6,BEAM,CTUBE,Tube6,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 800 * 0.5 - 24, 800 * 0.5);
                sw.WriteLine("SECTYPE,7,BEAM,CTUBE,Tube7,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 800 * 0.5 - 24, 800 * 0.5);
                sw.WriteLine("SECTYPE,8,BEAM,CTUBE,Tube8,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 800 * 0.5 - 24, 800 * 0.5);
            }
        }

        private void WriteMaterial(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("/prep7");
                sw.WriteLine("MP,EX,1,200E6");
                sw.WriteLine("MP,DENS,1,7.85e-9");
                sw.WriteLine("MP,ALPX,1,1.2E-5");
                sw.WriteLine("MP,NUXY,1,0.3");
                sw.WriteLine("et,1,188");
            }
        }

        private void WriteMainInp(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("finish");
                sw.WriteLine("/CLEAR,START");
                sw.WriteLine("/TITLE,YunNanProject");
                sw.WriteLine("/CWD,'C:\\Users\\IBD2\\AnsysBin'");
                sw.WriteLine("/input,material,inp");
                sw.WriteLine("/input,section,inp");
                sw.WriteLine("/input,node,inp");
                sw.WriteLine("/input,element,inp");
                sw.WriteLine("/input,solu,inp");
                sw.WriteLine("eplot");
            }
        }
        private void WriteSolu(string filepath)
        {
            List<int> idlist = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var num = (i + 1) * 10000 + (j + 1) * 1000 + k;
                        idlist.Add(num);
                        num = (i + 1) * 10000 + (j + 1) * 1000 + (235 - k);
                        idlist.Add(num);
                    }

                }

            }
            for (int i = 0; i < 4; i++)
            {
                idlist.Add(1000 + i);
                idlist.Add(1044 + i);
            }
            using (StreamWriter sw = new StreamWriter(filepath))
            {

                sw.WriteLine("/SOL");

                foreach (var item in idlist)
                {
                    var ch = "a";
                    if (item == idlist[0])
                    {
                        ch = "s";
                    }
                    sw.WriteLine(string.Format("nsel,{0},node,,{1}", ch, item));
                }
                sw.WriteLine("d,all,all");
                sw.WriteLine("allsel");

                sw.WriteLine("ANTYPE,2");
                sw.WriteLine("MODOPT,LANB,11,0,0, ,OFF");
                sw.WriteLine("MXPAND,11, , ,0 ");
                sw.WriteLine("LUMPM,0 ");
                sw.WriteLine("PSTRES,0");

            }
        }
        #endregion


        #region 写出OpenSEES模型

        #endregion
    }

}
