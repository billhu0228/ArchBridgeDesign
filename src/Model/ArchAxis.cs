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
    /// <summary>
    /// 拱轴
    /// </summary>
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

        /// <summary>
        /// 自拱顶至x0处的长度
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
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
                if (x0<0)
                {
                    return -1*(v1 + v2);
                }
                else
                {
                    return v1 + v2;
                }
              
            }
        }

        /// <summary>
        /// 获取自拱顶至leng弧长处的x0,与GetLength为反函数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public double GetX0(double length)
        {
            Func<double, double> f = (x) => length - GetLength(x);
            var xx = Bisection.FindRoot(f, -3*L1, 3*L1, 1e-5);
            return xx;
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
        /// 任意x0值下的正交角值，拱顶为90，逆时针为正.
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        public Angle GetNormalAngle(double x0)
        {
            return GetAngle(x0) + Angle.FromDegrees(90.0);
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
        /// 过面内一点求法线
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Point2D Intersect(Point2D pt)
        {

           // Func<double, double> f = (x) => (pt-GetCenter(x)).CrossProduct(Vector2D.XAxis.Rotate(GetAngle(x)));
            Func<double, double> f = (x) => (pt-GetCenter(x)).SignedAngleBetween(Vector2D.XAxis.Rotate(GetAngle(x)))-0.5*Math.PI;
            var xx = Bisection.FindRoot(f, -3 * L1, 3 * L1, 1e-5);
            return GetCenter(xx);
        }
        /// <summary>
        /// 过面内一点求法线V2
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Point2D IntersectV2(Point2D pt)
        {
      
            Func<double, double> f = (x) => (pt - GetCenter(x)).AngleTo(Vector2D.XAxis.Rotate(GetAngle(x))).Radians - 0.5 * Math.PI;


            if (pt.X>0)
            {
                var xx = Bisection.FindRoot(f,0, 2 * L1, 1e-5);
                return GetCenter(xx);
            }
            else if (pt.X<0)
            {
                var xx = Bisection.FindRoot(f, -2*L1, 0, 1e-5);
                return GetCenter(xx);

            }
            else
            {
                return GetCenter(0);
            }


        }


        /// <summary>
        /// 与构造2D线相交
        /// </summary>
        /// <param name="cutLine"></param>
        public Point2D Intersect(Line2D cutLine,double start=double.NaN,double end=double.NaN)
        {
            if (double.IsNaN(start))
            {
                start = cutLine.MiddlePoint().X>0?0: -L1 - 2;
            }
            if (double.IsNaN(end))
            {
                end = cutLine.MiddlePoint().X>0? L1 + 2:0;
            }
            if (cutLine.StartPoint.X==cutLine.EndPoint.X)
            {
                var xx = cutLine.EndPoint.X;
                var yy = GetZ(xx);
                return new Point2D(xx, yy);
            }
            if ((GetZ(start) - cutLine.GetY(start)) * (GetZ(end) - cutLine.GetY(end)) > 0)
            {
                throw new Exception();
            }
            else
            {
                Func<double, double> f = (x) => GetZ(x) - cutLine.GetY(x);
                var xx= Bisection.FindRoot(f, start, end, 1e-5);
                var yy = GetZ(xx);
                return new Point2D(xx, yy);
            }
        }
    }
}
