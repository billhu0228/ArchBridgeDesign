using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Model
{
    public class FEMModel
    {
        public List<FEMNode> NodeList;
        public List<FEMElement> ElementList;
        public Arch RelatedArchBridge;
        public List<Tuple<int, List<int>>> RigidGroups;
        public CrossArrangement RelatedDeck;
        public double DeckDistance;
        public int MaxFrameID;        
        public FEMModel()
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            RigidGroups = new List<Tuple<int, List<int>>>();
            RelatedArchBridge = null;
            RelatedDeck = null;
            DeckDistance = 0;
        }

        public FEMModel(ref Arch theArch,ref CrossArrangement theCross,double deckDist)
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            RigidGroups = new List<Tuple<int, List<int>>>();
            RelatedArchBridge = theArch;
            RelatedDeck = theCross;
            DeckDistance = deckDist;            
            GenerateRib();

            GenerateCol();


            GenerateRCCol();
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
        /// 生成过渡墩
        /// </summary>
        private void GenerateRCCol()
        {
            int secnColMain = 71;
            var wi = RelatedArchBridge.WidthInside;
            var wo = RelatedArchBridge.WidthOutside;            
            List<double> cbZlist = new List<double>() { 0, 0.5 * DeckDistance };
            for (int i = 0; i < RelatedDeck.GriderArr.Count; i++)
            {
                var item = RelatedDeck.GriderArr[i];
                if (i == RelatedDeck.GriderArr.Count - 1)
                {
                    continue;
                }
                cbZlist.Add(cbZlist.Last() + item);
            }
            cbZlist.Remove(0.5 * DeckDistance);

            List<double> cbZlist2 = new List<double>() { -0.5 * DeckDistance };
            for (int i = 0; i < RelatedDeck.GriderArr.Count; i++)
            {
                var item = RelatedDeck.GriderArr[i];
                if (i == RelatedDeck.GriderArr.Count - 1)
                {
                    continue;
                }
                cbZlist2.Add(cbZlist2.Last() - item);
            }
            cbZlist2.Remove(-0.5 * DeckDistance);
            cbZlist.AddRange(cbZlist2.ToArray());
            cbZlist2 = new List<double>() {
             (-0.5 * wi - wo-1),
             (-0.5 * wi - 0.5*wo),
              (-0.5 * wi),
              0,
              (+0.5 * wi),
              (+0.5 * wi + 0.5*wo),
              (+0.5 * wi + wo+1)
            };
            cbZlist.AddRange(cbZlist2.ToArray());
            cbZlist = cbZlist.Distinct().ToList();
            cbZlist.Sort();

            foreach (var col in RelatedArchBridge.RCColumnList)
            {
                
                int baseID = col.ID * 1000 + 110000;                
                var x0 = col.X;
                List<double> ylist = new List<double>();
                ylist.Add(col.H2);
                ylist.Add(col.H1);
                while (ylist.Last()- col.H0 >6)
                {
                    ylist.Add(ylist.Last() - 5);
                }
                ylist.Add(col.H0);
                ylist.Sort();
                ylist.Reverse();

                foreach (var yi in ylist)
                {
                    if (yi == ylist.First())
                    {
                        // 盖梁
                        int secn = 72;
                        foreach (var zi in cbZlist)
                        {
                            NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, zi)));
                            baseID += 1;
                            if (zi != cbZlist.First())
                            {
                                ElementList.Add(new FEMElement(0, baseID - 1, baseID - 2, secn));
                            }
                        }
                        // 刚臂

                    }
                    else
                    {
                        foreach (var zi in new double[] { (-0.5 * wi - 0.5 * wo), (0.5 * wi + 0.5 * wo), })
                        {
                            NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, zi)));
                            if (yi == ylist[1])
                            {
                                if (zi== (-0.5 * wi - 0.5 * wo))
                                {
                                    ElementList.Add(new FEMElement(baseID, baseID - 11, baseID, secnColMain));                                  
                                }
                                else
                                {
                                    ElementList.Add(new FEMElement(baseID, baseID - 4, baseID, secnColMain));
                                }
                            }
                            else
                            {
                                ElementList.Add(new FEMElement(baseID, baseID - 2, baseID, secnColMain));
                            }
                            baseID += 1;
                        }
                        if (yi == ylist.Last())
                        {
                        }
                        if (yi != ylist.Last() && yi != ylist.First())
                        {


                        }
                    }





                }
            }
            ;

        }

        /// <summary>
        /// 生成立柱
        /// </summary>
        private void GenerateCol()
        {
            int secnColMain;// = RelatedArchBridge.GetTubeProperty(0, eMemberType.ColumnMain).Section.SECN;
            int secnColCross;// = RelatedArchBridge.GetTubeProperty(0, eMemberType.ColumnCrossW).Section.SECN;
            int secnColWeb;// = RelatedArchBridge.GetTubeProperty(0, eMemberType.ColumnCrossW).Section.SECN;
            var wi = RelatedArchBridge.WidthInside;
            var wo = RelatedArchBridge.WidthOutside;

            List<double> cbZlist = new List<double>() { 0,0.5*DeckDistance};
            for (int i = 0; i < RelatedDeck.GriderArr.Count; i++)
            {
                var item = RelatedDeck.GriderArr[i];
                if (i== RelatedDeck.GriderArr.Count-1)
                {
                    continue;
                }
                cbZlist.Add(cbZlist.Last() + item);
            }
            cbZlist.Remove(0.5 * DeckDistance);

            List<double> cbZlist2 = new List<double>() { -0.5 * DeckDistance };
            for (int i = 0; i < RelatedDeck.GriderArr.Count; i++)
            {
                var item = RelatedDeck.GriderArr[i];
                if (i == RelatedDeck.GriderArr.Count - 1)
                {
                    continue;
                }
                cbZlist2.Add(cbZlist2.Last() - item);
            }
            cbZlist2.Remove(-0.5 * DeckDistance);
            cbZlist.AddRange(cbZlist2.ToArray());
            cbZlist2 = new List<double>() {
             (-0.5 * wi - wo-1),
             (-0.5 * wi - wo),
              (-0.5 * wi),
              0,
              (+0.5 * wi),
              (+0.5 * wi + wo),
              (+0.5 * wi + wo+1)
            };
            cbZlist.AddRange(cbZlist2.ToArray());
            cbZlist = cbZlist.Distinct().ToList() ;
            cbZlist.Sort();

            foreach (var col in RelatedArchBridge.ColumnList)
            {
                secnColMain = RelatedArchBridge.GetTubeProperty(col.X, eMemberType.ColumnMain).Section.SECN;
                secnColCross = RelatedArchBridge.GetTubeProperty(col.X, eMemberType.ColumnCrossL).Section.SECN;
                secnColWeb = RelatedArchBridge.GetTubeProperty(col.X, eMemberType.ColumnWeb).Section.SECN;

                int baseID = col.ID * 1000 + 100000;
 
                var x0 = col.X;
                List<double> ylist = new List<double>();
                ylist.Add(col.Z2);
                ylist.Add(col.Z2 - 2.0);
                bool AddDiag = col.ID <= 1 || col.ID >= 8 ? false : false;

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
                        int secn;
                        if (col.L == 4)
                        {
                            secn = 66;
                        }
                        else
                        {
                            secn = 65;
                        }
                        foreach (var zi in cbZlist)
                        {
                            NodeList.Add(new FEMNode(baseID, new Point3D(x0, yi, zi)));
                            baseID += 1;
                            if (zi!=cbZlist.First())
                            {
                                ElementList.Add(new FEMElement(0, baseID - 1, baseID -2, secn));
                            }
                        }                       
                        // 刚臂
                        RigidGroups.Add(new Tuple<int, List<int>>(baseID - 13 + 2, new List<int>() { baseID + 0, baseID + 4 }));
                        RigidGroups.Add(new Tuple<int, List<int>>(baseID - 13 + 4, new List<int>() { baseID + 1, baseID + 5 }));
                        RigidGroups.Add(new Tuple<int, List<int>>(baseID - 13 + 8, new List<int>() { baseID + 2, baseID + 6 }));
                        RigidGroups.Add(new Tuple<int, List<int>>(baseID - 13 + 10, new List<int>() { baseID + 3, baseID + 7 }));
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
                        int MasterNodeID = NodeList.FindAll((x) => Math.Abs(x.X - x0) < 1e-3 && x.ID > 11000 && x.ID <= 11999)[0].ID;
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
                    if (yi != ylist.Last() && yi != ylist.First())
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
                            if ((baseID - 8) % 2 == 0)
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
                MaxFrameID = ls.Count - 1;
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
                            double xmid = 0.5 * (NodeList.Find(x => x.ID == baseID - 1).X + NodeList.Find(x => x.ID == baseID).X);
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
                where e.ElemType == eMemberType.MainWeb || e.ElemType == eMemberType.SubWeb || e.ElemType == eMemberType.InstallWeb
                select e).ToList();
            var ColumnX = (from e in RelatedArchBridge.ColumnList select e.X).ToList(); // 立柱坐标列表
            List<double> MidColumnList = new List<double>();

            var centerX = new List<double>() { - theArch.MidInstallDist * 0.5, theArch.MidInstallDist * 0.5 };

            double xloc = 0;
            var selX = (
    from e in theArch.MemberTable
    where e.ElemType == eMemberType.MainWeb
    select e.Line.MiddlePoint().X).ToList(); // 提取主腹杆坐标
            for (int i = 0; i < ColumnX.Count + 1; i++)
            {
                if (i == 0)
                {
                    MidColumnList.Add(selX[0]);
                    MidColumnList.Add(selX[1]);
                    continue;
                }
                else if (i == ColumnX.Count)
                {
                    MidColumnList.Add(selX[selX.Count-2]);
                    MidColumnList.Add(selX.Last());
                    continue;
                }
                else if (i == 5)
                {
                    MidColumnList.Add(-0.5 * theArch.InstallDist);
                    MidColumnList.Add(0.5 * theArch.InstallDist);
                    continue;
                }
                else
                {
                    xloc = ColumnX[i - 1] + (ColumnX[1]-ColumnX[0])*0.5;
                }
                var deltabs = (from x in selX select Math.Abs(x - xloc)).ToList();
                int idx = deltabs.FindIndex((e) => e == deltabs.Min());
                double xreal = selX[idx];
                MidColumnList.Add(xreal);
                MidColumnList.Sort();

            }

            foreach (var elem in eleSelected)
            {
                var ni = NodeList.Find(x => x.Match(elem.Line.StartPoint));
                var nj = NodeList.Find(x => x.Match(elem.Line.EndPoint));
                double xmid = 0.5 * (ni.X + nj.X);
                int sen = theArch.GetTubeProperty(xmid, elem.ElemType).Section.SECN;
                if (ni == null || nj == null)
                {
                    // Debug.Write(elem.ToString());
                    continue;
                }
                ElementList.Add(new FEMElement(0, ni.ID, nj.ID, sen));                  // 竖腹杆
                ElementList.Add(new FEMElement(0, ni.ID + 10000, nj.ID + 10000, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 20000, nj.ID + 20000, sen));
                ElementList.Add(new FEMElement(0, ni.ID + 30000, nj.ID + 30000, sen));
                if (elem.ElemType == eMemberType.MainWeb || elem.ElemType == eMemberType.InstallWeb)
                {
                    if (Contains(ref ColumnX, elem.Line.StartPoint.X, 1e-3))
                    {
                        // 横撑
                        MakeCrossBeam(ni.ID + 10000, ni.ID + 20000, nj.ID + 10000, nj.ID + 20000);

                    }
                    if (Contains(ref MidColumnList, elem.Line.MiddlePoint().X, 1e-3))
                    {
                        // 横撑
                        MakeCrossBeam(ni.ID + 10000, ni.ID + 20000, nj.ID + 10000, nj.ID + 20000);
                    }
                    if (Contains(ref centerX, elem.Line.MiddlePoint().X,1e-3))
                    {
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
            MakeMCross2(false);
            // 内横隔
            eleSelected = (from e in theArch.MemberTable where e.ElemType == eMemberType.MainWeb || e.ElemType == eMemberType.InstallWeb select e).ToList();

            foreach (var elem in eleSelected)
            {
                var ni = NodeList.Find(x => x.Match(elem.Line.StartPoint));
                var frameID = ni.ID % 10000 % 1000;
                bool deleLower=false;
                int diagramSecn, diagramWebSecn;
                if (frameID == 18 || frameID == 187)
                {
                    deleLower = true;
                }
                var sects = RelatedArchBridge.GetTubeProperties(0, eMemberType.DiaphragmCoord);
                sects.Sort((x, y) => x.Section.Diameter.CompareTo(y.Section.Diameter));
                diagramWebSecn = RelatedArchBridge.GetTubeProperty(0, eMemberType.DiaphragmWeb).Section.SECN;
                if (elem.ElemType==eMemberType.InstallWeb)
                {
                    diagramSecn = sects[0].Section.SECN;
                }
                else 
                {
                    diagramSecn = sects[1].Section.SECN;
                }
                MakeDiagramV3(frameID,diagramSecn,diagramWebSecn, deleLower);
            }


        }

        /// <summary>
        /// 生成内隔——三角式
        /// </summary>
        /// <param name="framID"></param>
        /// <param name="diagramSecn"></param>
        /// <param name="diagramWebSecn"></param>
        private void MakeDiagramV3(int frameID,int diagramSecn,int diagramWebSecn,bool deleLower=false)
        {
            Vector3D Zdir = new Vector3D(0, 0, 1);

            NodeList.Add(new FEMNode(80000 + 1000 + frameID, GetPoint(10000 + 1000 + frameID) + 2 * Zdir));
            NodeList.Add(new FEMNode(80000 + 2000 + frameID, GetPoint(10000 + 2000 + frameID) + 2 * Zdir));
            NodeList.Add(new FEMNode(90000 + 1000 + frameID, GetPoint(30000 + 1000 + frameID) + 2 * Zdir));
            NodeList.Add(new FEMNode(90000 + 2000 + frameID, GetPoint(30000 + 2000 + frameID) + 2 * Zdir));

            ElementList.Add(new FEMElement(0, 11000 + frameID, 81000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 81000 + frameID, 21000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 31000 + frameID, 91000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 91000 + frameID, 41000 + frameID, diagramSecn));

            if (!deleLower)
            {
                ElementList.Add(new FEMElement(0, 12000 + frameID, 82000 + frameID, diagramSecn));
                ElementList.Add(new FEMElement(0, 82000 + frameID, 22000 + frameID, diagramSecn));
                ElementList.Add(new FEMElement(0, 32000 + frameID, 92000 + frameID, diagramSecn));
                ElementList.Add(new FEMElement(0, 92000 + frameID, 42000 + frameID, diagramSecn));
            }


            Vector3D dir = GetNode(12000 + frameID).location - GetNode(11000 + frameID).location;
            double h = dir.Length;
            Vector3D normalized_dir = dir.Normalize().ToVector3D();

            if (h <= 9)
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }

                ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                // ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));

                ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                // ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));

                if (!deleLower)
                {
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    // ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    // ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                }

            }
            else if (h <= 11)
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));

                ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                if (!deleLower)
                {
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));  

                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                }

            }
            else if (h <= 16)
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 5000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 0.5 * h * normalized_dir));
                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 5000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 5000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 10000 + 5000 + frameID, 20000 + 5000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 30000 + 5000 + frameID, 40000 + 5000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                if (!deleLower)
                {
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                }
            }
            else
            {
                throw new Exception("拱脚高度不考虑");
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 5000 + frameID, GetPoint(i * 10000 + 3000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 6000 + frameID, GetPoint(i * 10000 + 4000 + frameID) - 3.0 * normalized_dir));

                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 5000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 5000 + frameID, i * 10000 + 6000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 6000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                if (true)
                {
                    NodeList.Add(new FEMNode(80000 + 5000 + frameID, GetPoint(10000 + 5000 + frameID) + 2 * Zdir));
                    NodeList.Add(new FEMNode(80000 + 6000 + frameID, GetPoint(10000 + 6000 + frameID) + 2 * Zdir));
                    NodeList.Add(new FEMNode(90000 + 5000 + frameID, GetPoint(30000 + 5000 + frameID) + 2 * Zdir));
                    NodeList.Add(new FEMNode(90000 + 6000 + frameID, GetPoint(30000 + 6000 + frameID) + 2 * Zdir));

                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 5000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 5000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 5000 + frameID, 80000 + 5000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 5000 + frameID, 20000 + 5000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 6000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 6000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 6000 + frameID, 80000 + 6000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 6000 + frameID, 20000 + 6000 + frameID, diagramWebSecn));




                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 5000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 5000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 5000 + frameID, 90000 + 5000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 5000 + frameID, 40000 + 5000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 6000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 6000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 6000 + frameID, 90000 + 6000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 6000 + frameID, 40000 + 6000 + frameID, diagramWebSecn));

                }

            }
        }

        /// <summary>
        /// 生成内隔——三角式
        /// </summary>
        /// <param name="framID"></param>
        private void MakeDiagramV2(int frameID)
        {
            int diagramSecn = RelatedArchBridge.GetTubeProperty(0, eMemberType.DiaphragmCoord).Section.SECN;
            int diagramWebSecn = RelatedArchBridge.GetTubeProperty(0, eMemberType.DiaphragmWeb).Section.SECN;

            Vector3D Zdir = new Vector3D(0, 0, 1);

            NodeList.Add(new FEMNode(80000 + 1000 + frameID, GetPoint(10000 + 1000 + frameID) + 2 * Zdir));
            NodeList.Add(new FEMNode(80000 + 2000 + frameID, GetPoint(10000 + 2000 + frameID) + 2 * Zdir));
            NodeList.Add(new FEMNode(90000 + 1000 + frameID, GetPoint(30000 + 1000 + frameID) + 2 * Zdir));
            NodeList.Add(new FEMNode(90000 + 2000 + frameID, GetPoint(30000 + 2000 + frameID) + 2 * Zdir));

            ElementList.Add(new FEMElement(0, 11000 + frameID, 81000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 81000 + frameID, 21000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 12000 + frameID, 82000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 82000 + frameID, 22000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 31000 + frameID, 91000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 91000 + frameID, 41000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 32000 + frameID, 92000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 92000 + frameID, 42000 + frameID, diagramSecn));

            if (new int[] { }.Contains(frameID)) // 排除
            {
                return;
            }
            Vector3D dir = GetNode(12000 + frameID).location - GetNode(11000 + frameID).location;
            double h = dir.Length;
            Vector3D normalized_dir = dir.Normalize().ToVector3D();

            if (h <= 9)
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                if (true)
                {
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                    // ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    // ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    // ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    // ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                }

            }

            else if (h <= 11)
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                if (true)
                {
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                }

            }
            else if (h <= 16)
            {
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 5000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 0.5 * h * normalized_dir));
                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 5000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 5000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                if (true)
                {
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 10000 + 5000 + frameID, 20000 + 5000 + frameID, diagramWebSecn));


                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 30000 + 5000 + frameID, 40000 + 5000 + frameID, diagramWebSecn));
                }
            }
            else
            {
                throw new Exception("拱脚高度不考虑");
                for (int i = 1; i < 5; i++)
                {
                    NodeList.Add(new FEMNode(i * 10000 + 3000 + frameID, GetPoint(i * 10000 + 1000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 4000 + frameID, GetPoint(i * 10000 + 2000 + frameID) - 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 5000 + frameID, GetPoint(i * 10000 + 3000 + frameID) + 3.0 * normalized_dir));
                    NodeList.Add(new FEMNode(i * 10000 + 6000 + frameID, GetPoint(i * 10000 + 4000 + frameID) - 3.0 * normalized_dir));

                    var e = ElementList.Find(x => (x.Ni == i * 10000 + 1000 + frameID && x.Nj == i * 10000 + 2000 + frameID));
                    e.Nj = i * 10000 + 3000 + frameID;
                    ElementList.Add(new FEMElement(0, i * 10000 + 3000 + frameID, i * 10000 + 5000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 5000 + frameID, i * 10000 + 6000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 6000 + frameID, i * 10000 + 4000 + frameID, e.Secn));
                    ElementList.Add(new FEMElement(0, i * 10000 + 4000 + frameID, i * 10000 + 2000 + frameID, e.Secn));
                }
                if (true)
                {
                    NodeList.Add(new FEMNode(80000 + 5000 + frameID, GetPoint(10000 + 5000 + frameID) + 2 * Zdir));
                    NodeList.Add(new FEMNode(80000 + 6000 + frameID, GetPoint(10000 + 6000 + frameID) + 2 * Zdir));
                    NodeList.Add(new FEMNode(90000 + 5000 + frameID, GetPoint(30000 + 5000 + frameID) + 2 * Zdir));
                    NodeList.Add(new FEMNode(90000 + 6000 + frameID, GetPoint(30000 + 6000 + frameID) + 2 * Zdir));

                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 1000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 3000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 5000 + frameID, 10000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 5000 + frameID, 20000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 5000 + frameID, 80000 + 5000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 5000 + frameID, 20000 + 5000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 2000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 4000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 80000 + 6000 + frameID, 10000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 6000 + frameID, 20000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 10000 + 6000 + frameID, 80000 + 6000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 80000 + 6000 + frameID, 20000 + 6000 + frameID, diagramWebSecn));




                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 1000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 3000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 5000 + frameID, 30000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 5000 + frameID, 40000 + 3000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 5000 + frameID, 90000 + 5000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 5000 + frameID, 40000 + 5000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 2000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 4000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));

                    ElementList.Add(new FEMElement(0, 90000 + 6000 + frameID, 30000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 6000 + frameID, 40000 + 4000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 30000 + 6000 + frameID, 90000 + 6000 + frameID, diagramWebSecn));
                    ElementList.Add(new FEMElement(0, 90000 + 6000 + frameID, 40000 + 6000 + frameID, diagramWebSecn));

                }

            }
        }


        /// <summary>
        /// 生成内横隔--弦杆式
        /// </summary>
        /// <param name="ni"></param>
        private void MakeDiagram(int frameID, bool Strong)
        {
            int diagramSecn = RelatedArchBridge.GetTubeProperty(0, eMemberType.DiaphragmCoord).Section.SECN;
            int diagramWebSecn = RelatedArchBridge.GetTubeProperty(0, eMemberType.DiaphragmWeb).Section.SECN;

            ElementList.Add(new FEMElement(0, 11000 + frameID, 21000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 12000 + frameID, 22000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 31000 + frameID, 41000 + frameID, diagramSecn));
            ElementList.Add(new FEMElement(0, 32000 + frameID, 42000 + frameID, diagramSecn));

            if (new int[] { }.Contains(frameID))
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

                var Except = new int[] { };
                if (Except.Contains(frame))
                {
                    continue;
                }
                int ui = (int)((item.ID % 10000) / 1000);
                if (ui == 1 || ui == 2)
                {
                    if (GetNode(item.ID).X > 0)
                    {
                        int step = 3;
                        if (!NodeList.Exists(x => x.ID == item.ID - 50000 + 3))
                        {
                            step = 2;
                        }
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 + step, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 + step, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 + step, item.ID - 40000 + step, secn));
                        }

                    }
                    else
                    {
                        int step = 3;
                        if (!NodeList.Exists(x => x.ID == item.ID - 50000 - 3))
                        {
                            step = 2;
                        }
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 - step, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 - step, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 - step, item.ID - 40000 - step, secn));
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
                var Except = new int[] { 105, 106 }; // 反向不再加斜撑的frame编号
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

        private void MakeMCross2(bool AddI)
        {
            int secn;// = RelatedArchBridge.GetTubeProperty(0, eMemberType.WindBracing).Section.SECN;
            var mpt = (from n in NodeList where n.ID > 70000 select n).ToList();

            foreach (var item in mpt)
            {
                int frame = (item.ID % 10000) % 1000;
                secn = RelatedArchBridge.GetTubeProperty(GetNode(item.ID).X, eMemberType.WindBracing).Section.SECN;
                var Except = new int[] {5,200 };
                if (Except.Contains(frame))
                {
                    continue;
                }
                int ui = (int)((item.ID % 10000) / 1000);
                if (ui == 1 || ui == 2)
                {
                    if (ui==2 && Math.Abs(item.X)<=0)
                    {
                        // 排除下排的非边远平联
                        continue;
                    }
                    if (GetNode(item.ID).X > 0)
                    {
                        int step = 3;
                        if (!NodeList.Exists(x => x.ID == item.ID - 50000 + 3))
                        {
                            step = 2;
                        }
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 + step, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 + step, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 + step, item.ID - 40000 + step, secn));
                        }

                    }
                    else
                    {
                        int step = 3;
                        if (!NodeList.Exists(x => x.ID == item.ID - 50000 - 3))
                        {
                            step = 2;
                        }
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 50000 - step, secn));
                        ElementList.Add(new FEMElement(0, item.ID, item.ID - 40000 - step, secn));
                        if (AddI)
                        {
                            ElementList.Add(new FEMElement(0, item.ID - 50000 - step, item.ID - 40000 - step, secn));
                        }
                    }
                }


            }

            foreach (var item in mpt)
            {
                secn = RelatedArchBridge.GetTubeProperty(GetNode(item.ID).X, eMemberType.WindBracing).Section.SECN;
                int frame = (item.ID % 10000) % 1000;
                if (frame == 79)
                {
                    ;
                }
                var Except = new int[] { 2,102, 103,203 }; // 反向不再加斜撑的frame编号
                var reverse = new int[] { };
                if (Except.Contains(frame))
                {
                    continue;
                }
                int ui = (int)((item.ID % 10000) / 1000);
                if (ui == 1 || ui == 2)
                {
                    if (ui == 2 && Math.Abs(item.X) <= 0)
                    {
                        // 排除下排的非边远平联
                        continue;
                    }
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
            //ElementList.Add(new FEMElement(0, p1 + 30000, p4 + 30000, webSecn));
            //ElementList.Add(new FEMElement(0, p2 + 30000, p3 + 30000, webSecn));
            //ElementList.Add(new FEMElement(0, p1 + 30000, p3 + 30000, vertSecn));
            //ElementList.Add(new FEMElement(0, p2 + 30000, p4 + 30000, vertSecn));
            // V型交叉
            ElementList.Add(new FEMElement(0, p3 + 30000, p1 + 50000, webSecn));
            ElementList.Add(new FEMElement(0, p4 + 30000, p1 + 50000, webSecn));
            ElementList.Add(new FEMElement(0, p1 + 30000, p3 + 30000, vertSecn));
            ElementList.Add(new FEMElement(0, p2 + 30000, p4 + 30000, vertSecn));

        }
        public Point3D GetPoint(int p1)
        {
            return NodeList.Find(x => x.ID == p1).location;
        }
        public FEMNode GetNode(int p1)
        {
            return NodeList.Find(x => x.ID == p1);
        }
        /// <summary>
        /// 判断节点是否大于轴线高度
        /// </summary>
        /// <param name="p1"></param>
        /// <returns>ture 如果大于</returns>
        public bool IsNodeInUpper(int p1)
        {
            double z = GetPoint(p1).Y;
            double x = GetPoint(p1).X;
            double z0 = RelatedArchBridge.Axis.GetZ(x);
            return z > z0;
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
                ElementList.Add(new FEMElement(0, nilist[i], njlist[i], RelatedArchBridge.GetTubeProperty(0, eMemberType.TriWeb).Section.SECN));
            }

            nodeID = curID;

        }

        public double GetElementLength(FEMElement elem)
        {
            //FEMElement elem = ElementList.Find(e => e.ID == elemID);
            FEMNode nodeA = NodeList.Find(n => n.ID == elem.Ni);
            FEMNode nodeB = NodeList.Find(n => n.ID == elem.Nj);
            return nodeA.location.DistanceTo(nodeB.location);

        }

    }

}
