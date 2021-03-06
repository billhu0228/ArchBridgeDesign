using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Model
{
    public partial class FEMModel
    {
        public List<FEMNode> NodeList;
        public List<FEMElement> ElementList;
        public Arch RelatedArchBridge;
        public List<Tuple<int, List<int>>> RigidGroups;
        

        public FEMModel()
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            RigidGroups = new List<Tuple<int, List<int>>>();
            RelatedArchBridge = null;
        }

        public FEMModel(ref Arch theArch)
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            RigidGroups = new List<Tuple<int, List<int>>>();
            RelatedArchBridge = theArch;

            GenerateRib();

            GenerateCol();
            /// <remarks>
            /// 1:拱肋截面--15
            /// 2: 竖腹杆--20
            /// 3: 斜腹杆--18
            /// 4: 横梁--5
            /// 5：无使用--5
            /// 6：斜撑--7
            /// 7: 内横梁--6
            /// 8：立柱竖杆--23
            /// 9：立柱盖梁--9
            /// 99 :刚臂
            /// </remarks>
        }
        /// <summary>
        /// 生成立柱
        /// </summary>
        private void GenerateCol()
        {
            int secnColMain;// = RelatedArchBridge.GetTubeProperty(0, eMemberType.ColumnMain).Section.SECN;
            int secnColCross;// = RelatedArchBridge.GetTubeProperty(0, eMemberType.ColumnCrossW).Section.SECN;
            int secnColWeb;// = RelatedArchBridge.GetTubeProperty(0, eMemberType.ColumnCrossW).Section.SECN;
            
            foreach (var col in RelatedArchBridge.ColumnList)
            {
                secnColMain = RelatedArchBridge.GetTubeProperty(col.X, eMemberType.ColumnMain).Section.SECN;
                secnColCross = RelatedArchBridge.GetTubeProperty(col.X, eMemberType.ColumnCrossL).Section.SECN;
                secnColWeb = RelatedArchBridge.GetTubeProperty(col.X, eMemberType.ColumnWeb).Section.SECN;

                int baseID = col.ID * 1000 + 100000;
                var wi = RelatedArchBridge.WidthInside;
                var wo = RelatedArchBridge.WidthOutside;
                var x0 = col.X;
                List<double> ylist = new List<double>();
                ylist.Add(col.Z2);
                ylist.Add(col.Z2 - 2.0);
                bool AddDiag = col.ID <= 2 || col.ID >= 11 ? true : false;


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
                ylist.Add(col.Z1);

                foreach (var yi in ylist)
                {
                    if (yi == ylist.First())
                    {
                        // 盖梁
                        NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, (-0.5 * wi - wo))));
                        baseID += 1;
                        NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, (-0.5 * wi))));
                        baseID += 1;
                        NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, 0)));
                        baseID += 1;
                        NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, (+0.5 * wi))));
                        baseID += 1;
                        NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, (0.5 * wi + wo))));
                        baseID += 1;
                        ElementList.Add(new FEMElement(0, baseID - 5 + 0, baseID - 5 + 1, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 1, baseID - 5 + 2, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 2, baseID - 5 + 3, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 3, baseID - 5 + 4, 99));
                        // 刚臂
                        ElementList.Add(new FEMElement(0, baseID - 5 + 0, baseID + 0, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 0, baseID + 4, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 1, baseID + 1, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 1, baseID + 5, 99));

                        ElementList.Add(new FEMElement(0, baseID - 5 + 3, baseID + 2, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 3, baseID + 6, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 4, baseID + 3, 99));
                        ElementList.Add(new FEMElement(0, baseID - 5 + 4, baseID + 7, 99));

                    }
          
                    foreach (var xi in new double[] { (x0 - col.L * 0.5), (x0 + col.L * 0.5) })
                    {
                        foreach (var zi in new double[] { (-0.5 * wi - wo), (-0.5 * wi), (0.5 * wi), (0.5 * wi + wo), })
                        {

                            NodeList.Add(new FEMNode(baseID, new Point3D(xi, yi, zi)));
                            if (yi != ylist.First())
                            {
                                ElementList.Add(new FEMElement(baseID, baseID - 8, baseID, secnColMain));
                            }
                            baseID += 1;
                        }
                    }
                    if (yi == ylist.Last())
                    {
                        //立柱底刚臂
                        int MasterNodeID= NodeList.FindAll((x) => Math.Abs(x.X - x0)<1e-3 && x.ID>11000 && x.ID<=11999)[0].ID;
                        RigidGroups.Add(new Tuple<int, List<int>>(MasterNodeID, new List<int>() { baseID - 4, baseID - 8 }));

                        MasterNodeID = NodeList.FindAll((x) => Math.Abs(x.X - x0) < 1e-3 && x.ID > 21000 && x.ID <= 21999)[0].ID;
                        RigidGroups.Add(new Tuple<int, List<int>>(MasterNodeID, new List<int>() { baseID - 3, baseID - 7 }));

                        MasterNodeID = NodeList.FindAll((x) => Math.Abs(x.X - x0) < 1e-3 && x.ID > 31000 && x.ID <= 31999)[0].ID;
                        RigidGroups.Add(new Tuple<int, List<int>>(MasterNodeID, new List<int>() { baseID - 2, baseID - 6 }));

                        MasterNodeID = NodeList.FindAll((x) => Math.Abs(x.X - x0) < 1e-3 && x.ID > 41000 && x.ID <= 41999)[0].ID;
                        RigidGroups.Add(new Tuple<int, List<int>>(MasterNodeID, new List<int>() { baseID - 1, baseID - 5 }));
                        ;
                        //NodeList.fin

                    }
                    if (yi!=ylist.Last() && yi != ylist.First())
                    {
                        ElementList.Add(new FEMElement(0, baseID - 8 + 0, baseID - 8 + 1, secnColCross));
                        ElementList.Add(new FEMElement(0, baseID - 8 + 1, baseID - 8 + 5, secnColCross));
                        ElementList.Add(new FEMElement(0, baseID - 8 + 5, baseID - 8 + 4, secnColCross));
                        ElementList.Add(new FEMElement(0, baseID - 8 + 4, baseID - 8 + 0, secnColCross));

                        ElementList.Add(new FEMElement(0, baseID - 8 + 2, baseID - 8 + 3, secnColCross));
                        ElementList.Add(new FEMElement(0, baseID - 8 + 3, baseID - 8 + 7, secnColCross));
                        ElementList.Add(new FEMElement(0, baseID - 8 + 7, baseID - 8 + 6, secnColCross));
                        ElementList.Add(new FEMElement(0, baseID - 8 + 6, baseID - 8 + 2, secnColCross));

                        if (AddDiag)
                        {
                            if ((baseID - 8)%2==0)
                            {
                                ElementList.Add(new FEMElement(0, baseID - 8 + 0, baseID + 4, secnColWeb));
                                ElementList.Add(new FEMElement(0, baseID - 8 + 1, baseID + 5, secnColWeb));
                                ElementList.Add(new FEMElement(0, baseID - 8 + 2, baseID + 6, secnColWeb));
                                ElementList.Add(new FEMElement(0, baseID - 8 + 3, baseID + 7, secnColWeb));
                            }
                            else
                            {
                                ElementList.Add(new FEMElement(0, baseID - 8 + 4, baseID + 0, secnColWeb));
                                ElementList.Add(new FEMElement(0, baseID - 8 + 5, baseID + 1, secnColWeb));
                                ElementList.Add(new FEMElement(0, baseID - 8 + 6, baseID + 2, secnColWeb));
                                ElementList.Add(new FEMElement(0, baseID - 8 + 7, baseID + 3, secnColWeb));
                            }

                        }
                    }


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
            int Secn;
            var UppNodeFromIncline = (from e in theArch.MemberTable where e.ElemType == eMemberType.SubWeb select e.Line.UperPT()).ToList();
            var LowerNodeFromIncline = (from e in theArch.MemberTable where e.ElemType == eMemberType.SubWeb select e.Line.LowerPT()).ToList();

            #region 上下弦杆
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
                            double xmid = 0.5*(NodeList.Find(x => x.ID == baseID - 1).X+ NodeList.Find(x => x.ID == baseID).X);
                            Secn = theArch.GetTubeProperty(xmid, et).Section.SECN;
                            ElementList.Add(new FEMElement(baseID - 1, baseID - 1, baseID, Secn));
                        }
                        baseID += 1;
                    }
                    mID += 1;
                }
            }
            #endregion

            #region 腹杆
            var eleSelected = (
                from e in theArch.MemberTable
                where e.ElemType == eMemberType.MainWeb || e.ElemType == eMemberType.SubWeb || e.ElemType==eMemberType.InstallWeb
                select e).ToList();
            var ColumnX = (from e in RelatedArchBridge.ColumnList select e.X).ToList();
            List<double> MidColumnList = new List<double>();

            double xloc = 0;
            var selX = (
    from e in theArch.MemberTable
    where e.ElemType == eMemberType.MainWeb
    select e.Line.MiddlePoint().X).ToList();
            for (int i = 0; i < ColumnX.Count + 1; i++)
            {
                if (i == 0)
                {
                    xloc = ColumnX[0] - 14;
                }
                else if (i==ColumnX.Count)
                {
                    xloc = ColumnX.Last() + 14;
                }
                else
                {
                    xloc = ColumnX[i - 1] + 21;
                }
                var deltabs = (from x in selX select Math.Abs(x - xloc)).ToList();
                int idx = deltabs.FindIndex((e) => e == deltabs.Min());
                double xreal = selX[idx];
                MidColumnList.Add(xreal);

            }




            foreach (var elem in eleSelected)
            {
                var ni = NodeList.Find(x => x.Match(elem.Line.StartPoint));
                var nj = NodeList.Find(x => x.Match(elem.Line.EndPoint));
                double xmid = 0.5 * (ni.X + nj.X);
                int sen = theArch.GetTubeProperty(xmid, elem.ElemType).Section.SECN;                    
                if (ni == null || nj == null)
                {
                    Debug.Write(elem.ToString());
                    continue;
                }
                ElementList.Add(new FEMElement(0, ni.ID, nj.ID, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 10000, nj.ID + 10000, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 20000, nj.ID + 20000, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 30000, nj.ID + 30000, sen));
                if (elem.ElemType == eMemberType.MainWeb)
                {
                    if (Contains(ref ColumnX, elem.Line.StartPoint.X, 1e-3))
                    {
                        // 横撑
                        MakeCrossBeam(ni.ID + 10000, ni.ID + 20000, nj.ID + 10000, nj.ID + 20000);

                    }
                    if (Contains(ref MidColumnList, elem.Line.MiddlePoint().X, 0.80))
                    {
                        // 横撑
                        MakeCrossBeam(ni.ID + 10000, ni.ID + 20000, nj.ID + 10000, nj.ID + 20000);
                    }
                }

            }

            // 三角腹杆
            eleSelected = (from e in theArch.MemberTable where e.ElemType == eMemberType.TriWeb select e).ToList();
            int addID = 80000;
            int ofId = addID;
            foreach (var item in eleSelected)
            {
                cutExistElem(item.Line, ref addID);                
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
            #endregion

            // 平联撑
            MakeMCross(false);
            // 内横隔
            eleSelected = (from e in theArch.MemberTable where e.ElemType == eMemberType.MainWeb|| e.ElemType == eMemberType.InstallWeb select e).ToList();
            
            foreach (var elem in eleSelected)
            {
                var ni = NodeList.Find(x => x.Match(elem.Line.StartPoint));
                var frameID = ni.ID % 10000 % 1000;
                MakeDiagram(frameID, true);
            }


        }
        /// <summary>
        /// 生成内横隔
        /// </summary>
        /// <param name="ni"></param>
        private void MakeDiagram(int frameID,  bool Strong)
        {
            int diagramSecn = RelatedArchBridge.GetTubeProperty(0, eMemberType.DiaphragmCoord).Section.SECN;
            int diagramWebSecn = RelatedArchBridge.GetTubeProperty(0, eMemberType.DiaphragmWeb).Section.SECN;

            ElementList.Add(new FEMElement(0, 11000 + frameID, 21000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 12000 + frameID, 22000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 31000 + frameID, 41000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 32000 + frameID, 42000 + frameID, diagramSecn));

            if (new int[] {  }.Contains(frameID))
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
                    ElementList.Add(new FEMElement(0, 20000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 20000 + 2000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 2000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
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
                    ElementList.Add(new FEMElement(0, 21000 + frameID, 14000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 14000 + frameID, 23000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 23000 + frameID, 15000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 15000 + frameID, 22000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 31000 + frameID, 44000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 44000 + frameID, 33000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 33000 + frameID, 45000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 45000 + frameID, 32000 + frameID, diagramWebSecn));
                }


            }

        }

        private void MakeMCross(bool AddI)
        {
            int secn = RelatedArchBridge.GetTubeProperty(0, eMemberType.WindBracing).Section.SECN;
            var mpt = (from n in NodeList where n.ID > 70000 select n).ToList();

            foreach (var item in mpt)
            {
                int frame = (item.ID % 10000) % 1000;

                var Except = new int[] {};
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

        private void MakeCrossBeam(int p1, int p2, int p3, int p4)
        {

            var ptA = GetNode(p1).location;
            var ptB = GetNode(p2).location;
            var ptC = GetNode(p3).location;
            var ptD = GetNode(p4).location;
            int coordSecn = RelatedArchBridge.GetTubeProperty(ptA.X, eMemberType.CrossCoord).Section.SECN;
            int vertSecn = RelatedArchBridge.GetTubeProperty(ptA.X, eMemberType.CrossVerical).Section.SECN;
            int webSecn = RelatedArchBridge.GetTubeProperty(ptA.X, eMemberType.CrossWeb).Section.SECN;
            double offset = 1.2;
            NodeList.Add(new FEMNode(p1 + 30000, ptA.X, ptA.Y, ptA.Z + offset));
            NodeList.Add(new FEMNode(p2 + 30000, ptB.X, ptB.Y, ptB.Z - offset));
            NodeList.Add(new FEMNode(p3 + 30000, ptC.X, ptC.Y, ptC.Z + offset));
            NodeList.Add(new FEMNode(p4 + 30000, ptD.X, ptD.Y, ptD.Z - offset));

            NodeList.Add(new FEMNode(p1 + 50000, ptA.X, ptA.Y, ptA.Z + RelatedArchBridge.WidthInside * 0.5));
            NodeList.Add(new FEMNode(p3 + 50000, ptC.X, ptC.Y, ptC.Z + RelatedArchBridge.WidthInside * 0.5));

            ElementList.Add(new FEMElement(0, p1, p1 + 30000, coordSecn));// 上弦
            ElementList.Add(new FEMElement(0, p1 + 30000, p1 + 50000, coordSecn));
            ElementList.Add(new FEMElement(0, p1 + 50000, p2 + 30000, coordSecn));
            ElementList.Add(new FEMElement(0, p2 + 30000, p2, coordSecn));

            ElementList.Add(new FEMElement(0, p3, p3 + 30000, coordSecn)); //下弦
            ElementList.Add(new FEMElement(0, p3 + 30000, p3 + 50000, coordSecn));
            ElementList.Add(new FEMElement(0, p3 + 50000, p4 + 30000, coordSecn));
            ElementList.Add(new FEMElement(0, p4 + 30000, p4, coordSecn));
            // 交叉
            ElementList.Add(new FEMElement(0, p1 + 30000, p4 + 30000, webSecn));  
            ElementList.Add(new FEMElement(0, p2 + 30000, p3 + 30000, webSecn));
            ElementList.Add(new FEMElement(0, p1 + 30000, p3 + 30000, vertSecn));
            ElementList.Add(new FEMElement(0, p2 + 30000, p4 + 30000, vertSecn));

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
                ElementList.Add(new FEMElement(0, nilist[i], njlist[i], RelatedArchBridge.GetTubeProperty(0,eMemberType.TriWeb).Section.SECN));
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
                    sw.WriteLine("mat,{0}",100+ secnn);
                    var eles = (from e in ElementList where e.Secn == secnn select e).ToList();
                    foreach (var item in eles)
                    {
                        sw.WriteLine("e,{0},{1}", item.Ni, item.Nj);
                    }
                }

                foreach (var item in RigidGroups)
                {
                    sw.WriteLine("nsel,s,node,,{0}", item.Item1);
                    foreach (var slv in item.Item2)
                    {
                        sw.WriteLine("nsel,a,node,,{0}", slv);
                    }
                    //sw.WriteLine("cerig,{0},all,all",item.Item1);
                    sw.WriteLine("cp,next,all,all");
                }


            }
        }

        private void WriteSection(string filepath)
        {
            
            var sect = (from e in RelatedArchBridge.PropertyTable select e.Section as TubeSection).ToList();
            
            sect = sect.Distinct().ToList();
            
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (var item in sect)
                {
                    if (item.IsCFTS)
                    {
                        sw.WriteLine("!----------------SECTION{0}----------------", item.SECN);
                        CFTS PropertyCal = new CFTS(item.Diameter * 1000, item.Thickness * 1000, 80, 420);
                        sw.WriteLine("MP,EX,{0},{1}",item.SECN+100,PropertyCal.E);
                        sw.WriteLine("MP,DENS,{0},{1}", item.SECN + 100, PropertyCal.density);
                        sw.WriteLine("MP,ALPX,{0},1.2E-5", item.SECN + 100);
                        sw.WriteLine("MP,NUXY,{0},0.3", item.SECN + 100);

                        sw.WriteLine("SECTYPE,{0},BEAM,CSOLID,Tube{0},0", item.SECN);
                        sw.WriteLine("SECDATA,{0},0,0,0,0,0,0,0,0,0,0,0", item.Diameter * 1000 * 0.5);
                    }
                    else
                    {
                        sw.WriteLine("!----------------SECTION{0}----------------", item.SECN);
                        sw.WriteLine("MP,EX,  {0},205E6", item.SECN + 100);
                        sw.WriteLine("MP,DENS,{0},7.85e-9", item.SECN + 100);
                        sw.WriteLine("MP,ALPX,{0},1.2E-5", item.SECN + 100);
                        sw.WriteLine("MP,NUXY,{0},0.3", item.SECN + 100);

                        sw.WriteLine("SECTYPE,{0},BEAM,CTUBE,TubeSection{0},0",item.SECN);
                        sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 
                            item.Diameter * 1000 * 0.5 - item.Thickness * 1000, item.Diameter*1000 * 0.5);
                    }

                }
            }
        }

        private void WriteMaterial(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("/prep7");
                sw.WriteLine("SECTYPE,{0},BEAM,CSOLID,Tube{0},0", 99);
                sw.WriteLine("SECDATA,{0},0,0,0,0,0,0,0,0,0,0,0", 500);
                sw.WriteLine("MP,EX,  199,205E16");
                sw.WriteLine("MP,DENS,199,7.85e-19");
                sw.WriteLine("MP,ALPX,199,1.2E-5");
                sw.WriteLine("MP,NUXY,199,0.3");
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
            var nd=NodeList.FindLast((x) => x.ID <= 11999);
            List<int> idlist = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var num = (i + 1) * 10000 + (j + 1) * 1000 + k;
                        idlist.Add(num);
                        num = (i + 1) * 10000 + (j + 1) * 1000 + (nd.ID-11000 - k);
                        idlist.Add(num);
                    }

                }

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
                double L1X = RelatedArchBridge.Axis.L1*1000;
                double L1Y = RelatedArchBridge.Axis.GetCenter(RelatedArchBridge.Axis.L1).Y*1000;
                sw.WriteLine("nsel,s,loc,x,{0},{1}", L1X - 1, L1X + 1) ;
                sw.WriteLine("nsel,r,loc,y,{0},{1}", L1Y - 1, L1Y + 1) ;
                sw.WriteLine("d,all,all");
                sw.WriteLine("nsel,s,loc,x,{0},{1}", -L1X - 1, -L1X + 1);
                sw.WriteLine("nsel,r,loc,y,{0},{1}", L1Y - 1, L1Y + 1);
                sw.WriteLine("d,all,all");

                sw.WriteLine("allsel");
                sw.WriteLine("antype,0");
                //sw.WriteLine("NLGEOM,1");
                //sw.WriteLine("PSTRES,1");
                sw.WriteLine("allsel");
                sw.WriteLine("acel,0,9800,0");
                sw.WriteLine("NSUBST,1");
                sw.WriteLine("OUTRES,ERASE");
                sw.WriteLine("OUTRES,ALL,LAST");
                sw.WriteLine("time,1");
                sw.WriteLine("solve");

            }
        }
        #endregion


        #region 写出OpenSEES模型

        #endregion


        #region 写出midas命令

        #endregion


        public double GetElementLength(FEMElement elem)
        {
            //FEMElement elem = ElementList.Find(e => e.ID == elemID);
            FEMNode nodeA = NodeList.Find(n => n.ID == elem.Ni);
            FEMNode nodeB = NodeList.Find(n => n.ID == elem.Nj);
            return nodeA.location.DistanceTo(nodeB.location);

        }
    }

}
