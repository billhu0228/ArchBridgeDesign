using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ArchAxis
    {
        public readonly double f,m,L;
        public readonly double k, L1;
        

        public ArchAxis(double f,double m, double L)
        {
            this.f = f;
            this.m = m;
            this.L = L;
            L1 = 0.5 * L;
            k= Math.Log(m + Math.Sqrt(m * m - 1));
        }

        public double ArchLength
        {
            get
            {
                return GetLength(L1) * 2.0; 
            }
        }

        double get_kesi(double x0)
        {
            return x0 / L1;
        }

        public double GetLength(double x0)
        {
            double k0 = get_kesi(x0);
            if (Math.Abs(k0)<1)
            {
                double v = Integrate.OnClosedInterval(ds, 0, k0);
                return v;
            }
            else
            {
                double v1= Integrate.OnClosedInterval(ds, 0, 1.0);
                double v2 = Math.Sqrt( Math.Pow((Math.Abs(x0) - L1),2) + Math.Pow((GetZ(x0) - GetZ(L1)),2));
                return v1 + v2;
            }
            

        }

        /// <summary>
        /// 任意x0时对应的拱轴交点.
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        public Point2D GetCenter(double x0)
        {
            return new Point2D(x0, GetZ(x0));
        }

        /// <summary>
        /// 任意x0值下的z值，拱顶为0.
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        public double GetZ(double x0)
        {
            double kesi = get_kesi(x0);
            if (Math.Abs(kesi)<=1)
            {
                return -f / (m - 1) * (Math.Cosh(k * kesi) - 1);
            }
            else
            {
                double zb = GetZ(Math.Sign(kesi) * L1);
                double kk = Math.Tan(GetAngle(Math.Sign(kesi) * L1).Radians);
                return (zb + kk * (kesi - Math.Sign(kesi)) * L1);
            }
       
            
        }

        /// <summary>
        /// 任意x0值下的切角值，拱顶为0，逆时针为正.
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        public Angle GetAngle(double x0)
        {
            double kesi = get_kesi(x0);
            if (Math.Abs(kesi) <= 1)
            {
                double eta = 2 * f * k / (m - 1) / L;
                Angle phi = Angle.FromRadians(-Math.Atan(eta * Math.Sinh(k * kesi)));
                return phi;                
            }
            else
            {
                return GetAngle(Math.Sign(x0) * L1);
            }

        }


        /// <summary>
        /// 弧长微分算子
        /// </summary>
        /// <param name="ks">x0/L1值</param>
        /// <returns></returns>
        double ds(double ks)
        {
            double eta = 2 * f * k / (m - 1) / L;
            return L1 * Math.Sqrt(1 + eta * eta * Math.Sinh(ks * k) * Math.Sinh(ks * k));
        }

        public bool isLeft(Vector2D pt)
        {
            return pt.Y > GetZ(pt.X);

        }

        /// <summary>
        /// 与构造2D线相交
        /// </summary>
        /// <param name="cutLine"></param>
        internal Point2D Intersect(Line2D cutLine)
        {
            if ((GetZ(-L1) - cutLine.GetY(-L1)) * (GetZ(L1) - cutLine.GetY(L1)) > 0)
            {
                throw new Exception();
            }
            else
            {
                Func<double, double> f = (x) => GetZ(x) - cutLine.GetY(x);
                var xx= Bisection.FindRoot(f,-L1,L1, 1e-5);
                var yy = GetZ(xx);
                return new Point2D(xx, yy);
            }
        }
    }
}
