using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Model
{
    public class FEMDeck
    {
        public List<FEMNode> NodeList;
        public List<FEMElement> ElementList;
        public List<NLBearing> Bearings;
        public List<Tuple<int, List<int>>> RigidGroups;
        public List<Tuple<int, int>> LinkGroups;
        public List<LinkGroup> LinkGroups2;
        public CompositeDeck RelatedDeck;
        List<double> main_xlist;
        List<double> main_ylist;
        List<double> sub_ylist;
        List<double> xlist;
        List<double> ylist;
        public int Nstart;
        public int Estart;
        public int NPTS;
        double x0;
        double y0;
        double z0;




        /// <summary>
        /// 生成组合梁
        /// </summary>
        /// <param name="theDeck"></param>
        public FEMDeck(ref CompositeDeck theDeck, int nstart,int estart, double e_size_x, double e_size_y, double x0, double y0, double z0)
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            Bearings = new List<NLBearing>();
            RigidGroups = new List<Tuple<int, List<int>>>();
            LinkGroups = new List<Tuple<int, int>>();
            LinkGroups2 = new List<LinkGroup>();

            RelatedDeck = theDeck;
            Nstart = nstart;
            Estart = estart;
            this.x0 = x0;
            this.y0 = y0;
            this.z0 = z0;
            CreateBearingType();
            CreateCoord(e_size_x, e_size_y);
            CreatePlate();
            CreateGirder();
            CreateCrossBeam();
        }

        private void CreateBearingType()
        {
            Link Fix = new Link(1e6);
            NolinearLink R1 =  new NolinearLink(0, 26002, 78000, 0.001, 50, 0.5, 0.5);
            NolinearLink R2 =  new NolinearLink(0, 26002, 78000, 0.001, 50, 0.5, 0.5);
            NolinearLink R3 = new NolinearLink(0,  55909, 168000, 0.001, 50, 0.5, 0.5);
            NolinearLink R4 = new NolinearLink(0, 28496, 85000, 0.001, 50, 0.5, 0.5);
            NolinearLink R5 = new NolinearLink(1e6, 28496, 85000, 0.001, 50, 0.5, 0.5);
            NLBearing Bdouble = new NLBearing("GQZ-SX(双向)",Fix, R1, R2);
            NLBearing Blong = new NLBearing("GQZ-DX(顺向)", Fix, Fix, R3);
            NLBearing Btrans = new NLBearing("GQZ-DY(横向)", Fix, R4, Fix);
            NLBearing Bnone = new NLBearing("GQZ-GD(固定)", R5, R5, R5);
            Bearings.Add(Bdouble);
            Bearings.Add(Blong);
            Bearings.Add(Btrans);
            Bearings.Add(Bnone);
        }

        private void CreateCrossBeam()
        {
            foreach (var xx in main_xlist)
            {
                var xi = xx + x0;
                var res = NodeList.FindAll(delegate (FEMNode nd) { return nd.X == xi; });
                res.Sort((x, y) => x.Y.CompareTo(y.Y));
                for (int i = 0; i < res.Count; i++)
                {
                    var ny = res[i].Z;
                    if (ny<=main_ylist[1]+y0||ny>main_ylist[main_ylist.Count-2]+y0)
                    {
                        continue;
                    }
                    ElementList.Add(new FEMElement(Estart, res[i - 1].ID, res[i].ID, 121));
                    Estart++;
                }

            }
            ;
        }

        private void CreateGirder()
        {
            foreach (var yy in main_ylist)
            {
                var yi = yy + y0;
                if (yy != main_ylist.First() && yy != main_ylist.Last())
                {
                    var res = NodeList.FindAll(delegate (FEMNode nd) { return nd.Z == yi; });
                    res.Sort((x, y) => x.X.CompareTo(y.X));
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (i!=0)
                        {
                            ElementList.Add(new FEMElement(Estart, res[i - 1].ID, res[i].ID, 101));
                            Estart++;
                        }
                    }                    
                }
            }
            foreach (var yy in sub_ylist)
            {
                var yi = yy + y0;
                if (yy != sub_ylist.First() && yy != sub_ylist.Last())
                {
                    var res = NodeList.FindAll(delegate (FEMNode nd) { return nd.Z == yi; });
                    res.Sort((x, y) => x.X.CompareTo(y.X));
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (i != 0)
                        {
                            ElementList.Add(new FEMElement(Estart, res[i - 1].ID, res[i].ID, 111));
                            Estart++;
                        }
                    }
                }
            }
        }

        private void CreatePlate()
        {
            var n = Nstart + 1;
            foreach (var y in ylist)
            {
                foreach (var x in xlist)
                {
                    NodeList.Add(new FEMNode(n, new Point3D(x0 + x,  z0, y0 + y)));
                    n++;
                    if (RelatedDeck.spans.Contains(x) && main_ylist.Contains(y))
                    {
                        int iix = RelatedDeck.spans.IndexOf(x);
                        int iiy = main_ylist.IndexOf(y);
                        int iib = 0;
                        if (iiy==0|| iiy==main_ylist.Count-1)
                        {
                            continue;

                        }
                        int nj;
                        if (y0 + y > 0)
                        {
                            if (RelatedDeck.ColumnIDList[iix] < 0)
                            {
                                if (iix==0)
                                {
                                    if (iiy == 1)
                                    {
                                        nj = 111007;
                                    }
                                    else if (iiy == 2)
                                    {
                                        nj = 111009;
                                    }
                                    else
                                    {
                                        nj = 111012;
                                    }
                                }
                                else
                                {
                                    if (iiy == 1)
                                    {
                                        nj = 112007;
                                    }
                                    else if (iiy == 2)
                                    {
                                        nj = 112009;
                                    }
                                    else
                                    {
                                        nj = 112012;
                                    }
                                }
                                ;
                            }
                            else if (iiy==1)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 7;
                            }
                            else if (iiy==2)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 9;
                            }
                            else
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 +12;
                            }
                       
                        }
                        else
                        {
                            if (RelatedDeck.ColumnIDList[iix] < 0)
                            {
                                if (iix == 0)
                                {
                                    if (iiy == 1)
                                    {
                                        nj = 111000;
                                    }
                                    else if (iiy == 2)
                                    {
                                        nj = 111003;
                                    }
                                    else
                                    {
                                        nj = 111005;
                                    }
                                }
                                else
                                {
                                    if (iiy == 1)
                                    {
                                        nj = 112000;
                                    }
                                    else if (iiy == 2)
                                    {
                                        nj = 112003;
                                    }
                                    else
                                    {
                                        nj = 112005;
                                    }
                                }
    ;
                            }
                            else if (iiy == 1)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 0;
                            }
                            else if (iiy == 2)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 3;
                            }
                            else
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 5;
                            }
                        }

                        if (iix == 1|| iix == 2 || iix == 9 || iix == 10)
                        {
                            // 高墩
                            iib = iiy == 2 ? 3 : 2;
                        }
                        else
                        {
                            // 一般
                            iib = iiy == 2 ? 1 : 0;
                        }
                        if (iib == 3)
                        {
                            // LinkGroups.Add(new Tuple<int, int>(n - 1, nj));
                            LinkGroups2.Add(new LinkGroup(1, n - 1, nj, Bearings[iib]));
                        }
                        else
                        {
                            LinkGroups2.Add(new LinkGroup(1, n - 1, nj, Bearings[iib]));
                        }
          
                    }
                }
            }
           
            int kk = (n - Nstart - 1) / ylist.Count;
            for (int ny = 0; ny < ylist.Count - 1; ny++)
            {
                for (int nx = 0; nx < xlist.Count - 1; nx++)
                {
                    int A = ny * kk + nx + 1 + Nstart;
                    int B = A + 1;
                    int C = B + kk;
                    int D = C - 1;
                    List<int> ns = new List<int>() { A, B, C, D };
                    ElementList.Add(new FEMElement4(Estart, A, B, C, D, 1));
                    Estart++;
                }

            }

        }

        /// <summary>
        /// 生成控制坐标
        /// </summary>
        /// <param name="e_size_x"></param>
        /// <param name="e_size_y"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CreateCoord(double e_size_x, double e_size_y)
        {
            List<double> tmp = new List<double>();
            for (int i = 0; i < RelatedDeck.spans.Count - 1; i++)
            {
                tmp.AddRange(more_value(RelatedDeck.spans[i], RelatedDeck.spans[i + 1], RelatedDeck.crossDist));
            }
            main_xlist = tmp.Distinct(new AproxiComparer()).ToList();
            main_xlist.Sort();

            tmp = new List<double>();
            for (int i = 0; i < main_xlist.Count - 1; i++)
            {
                tmp.AddRange(more_value(main_xlist[i], main_xlist[i + 1], e_size_x));
            }
            xlist = tmp.Distinct().ToList();
            xlist.Sort();

            tmp = new List<double>() { 0 };
            foreach (var item in RelatedDeck.crossSection.GriderArr)
            {
                tmp.Add(tmp.Last() + item);
            }
            main_ylist = tmp.Distinct().ToList();

            tmp = new List<double>() { 0 };
            foreach (var item in RelatedDeck.crossSection.SubGirderArr)
            {
                tmp.Add(tmp.Last() + item);
            }
            sub_ylist = tmp.Distinct().ToList();

            var kp_y = sub_ylist.ToList();
            kp_y.AddRange(main_ylist);
            kp_y = kp_y.Distinct().ToList();
            kp_y.Sort();

            tmp = new List<double>();
            for (int i = 0; i < kp_y.Count - 1; i++)
            {
                tmp.AddRange(more_value(kp_y[i], kp_y[i + 1], e_size_y));
            }
            ylist = tmp.Distinct().ToList();
            NPTS = xlist.Count();
        }

        private IEnumerable<double> more_value(double start, double end, double apx_dist)
        {
            int npts = (int)(Math.Round((end - start) / apx_dist, 0));
            return Linspace(start, end, npts + 1);
        }

        private IEnumerable<double> Linspace(double start, double end, int v)
        {
            List<double> res = new List<double>();
            double step = (end - start) / (v - 1);
            for (int i = 0; i < v; i++)
            {
                res.Add(Math.Round(start + i * step,6));
            }
            return res;
        }

        public Point3D GetPoint(int p1)
        {
            return NodeList.Find(x => x.ID == p1).location;
        }
    }

    class AproxiComparer : IEqualityComparer<double>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(double x, double y)
        {
            return Math.Abs(x-y)<1e-3;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(double x)
        {
            return 0;        
        }
    }

}
