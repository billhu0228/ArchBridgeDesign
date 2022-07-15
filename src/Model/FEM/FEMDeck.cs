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
        public List<Tuple<int, List<int>>> RigidGroups;
        public CompositeDeck RelatedDeck;
        List<double> main_xlist;
        List<double> main_ylist;
        List<double> sub_ylist;
        List<double> xlist;
        List<double> ylist;
        int Nstart;

        double x0;
        double y0;
        double z0;




        /// <summary>
        /// 生成组合梁
        /// </summary>
        /// <param name="theDeck"></param>
        public FEMDeck(ref CompositeDeck theDeck, int nstart, double e_size_x, double e_size_y, double x0, double y0, double z0)
        {
            NodeList = new List<FEMNode>();
            ElementList = new List<FEMElement>();
            RigidGroups = new List<Tuple<int, List<int>>>();
            RelatedDeck = theDeck;
            Nstart = nstart;
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
            throw new NotImplementedException();
        }

        private void CreateGirder()
        {
            throw new NotImplementedException();
        }

        private void CreatePlate()
        {
            var n = Nstart + 1;
            foreach (var y in ylist)
            {
                foreach (var x in xlist)
                {
                    NodeList.Add(new FEMNode(n, new Point3D(x0 + x, y0 + y, z0)));
                    n++;
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
                    List<int> ns = new List<int>() { };

                }

            }
            throw new NotImplementedException();
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
            main_xlist = tmp.Distinct().ToList();
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
            var npts = (int)(Math.Round((end - start) / apx_dist, 0));
            return Linspace(start, end, npts + 1);
        }

        private IEnumerable<double> Linspace(double start, double end, int v)
        {
            List<double> res = new List<double>();
            double step = (end - start) / (v - 1);
            for (int i = 0; i < v; i++)
            {
                res.Add(start + i * step);
            }
            return res;
        }
    }

}
