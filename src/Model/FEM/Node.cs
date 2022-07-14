using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 无用
    /// </summary>
    public class Node2D
    {
        public Point2D Pt;
        public int ID;

        public Node2D(int n,double x, double y)
        {
            ID = n;
            Pt = new Point2D(x, y);
        }
    }
}
