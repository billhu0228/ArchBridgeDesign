﻿using MathNet.Spatial.Euclidean;
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
        public List<Tuple<int, List<int>>> RigidGroups;
        public List<Tuple<int, int>> LinkGroups;
        public CompositeDeck RelatedDeck;
        List<double> main_xlist;
        List<double> main_ylist;
        List<double> sub_ylist;
        List<double> xlist;
        List<double> ylist;
        public int Nstart;
        public int Estart;

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
            RigidGroups = new List<Tuple<int, List<int>>>();
            LinkGroups = new List<Tuple<int, int>>();
            RelatedDeck = theDeck;
            Nstart = nstart;
            Estart = estart;
            this.x0 = x0;
            this.y0 = y0;
            this.z0 = z0;
            CreateCoord(e_size_x, e_size_y);
            CreatePlate();
            CreateGirder();
            CreateCrossBeam();
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
                        if (RelatedDeck.ColumnIDList[iix]<0 || iiy==0|| iiy==main_ylist.Count-1)
                        {
                            continue;

                        }
                        int nj;
                        if (y0 + y > 0)
                        {
                            if (iiy==1)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 7;
                            }
                            else if (iiy==2)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 8;
                            }
                            else
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 +11;
                            }
                       
                        }
                        else
                        {
                            if (iiy == 1)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 1;
                            }
                            else if (iiy == 2)
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 4;
                            }
                            else
                            {
                                nj = RelatedDeck.ColumnIDList[iix] * 1000 + 100000 + 5;
                            }
                        }
                        LinkGroups.Add(new Tuple<int, int>(n-1,nj) );
                        
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
            ;
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