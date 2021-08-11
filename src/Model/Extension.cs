using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class Extension
    {

        /// <summary>
        /// 值域
        /// </summary>
        /// <param name="vv"></param>
        /// <param name="vto"></param>
        /// <returns></returns>
        public static double SignedAngleBetween(this Vector2D vv,Vector2D vto)
        {
            var ang = vv.SignedAngleTo(vto);

            if (ang.Radians<-Math.PI)
            {
                return ang.Radians + 2 * Math.PI;

            }
            else if(ang.Radians<Math.PI)
            {
                return ang.Radians;
            }
            else
            {
                return ang.Radians - 2 * Math.PI;
            }

        }

        public static double GetY(this Line2D L, double t)
        {
            double kk = L.Direction.Y / L.Direction.X;
            double C0 = L.StartPoint.Y - L.StartPoint.X * kk;

            return t *kk + C0;
        }

        public static Line2D Offset(this Line2D L,double val)
        {
            Vector2D xv=L.Direction.Rotate(Angle.FromDegrees(90.0));
            Point2D NewSt = L.StartPoint + xv * val;
            Point2D NewEd = L.EndPoint + xv * val;
            return new Line2D(NewSt, NewEd);
        }

        public static Point2D MiddlePoint(this Line2D L)
        {
            return L.StartPoint + 0.5 * L.Direction * L.Length;
        }

        public static List<Point2D> Tangent(this Point2D pt,Circle2D cir)
        {
            var r = cir.Radius;
            var po = cir.Center - pt;
            var pob =Angle.FromRadians( Math.Acos(cir.Radius / po.Length));
            var ob = (-po).Rotate(-pob).Normalize();
            var oc = (-po).Rotate(pob).Normalize();
            var res = new List<Point2D>() { cir.Center + ob * r, cir.Center + oc * r };
            res.Sort((x, y) => x.Y.CompareTo(y.Y));
            return res ;

        }

        public static double Interplate(double x_1, double x_2, double y_1, double y_2, double x0)
        {
            double k = (y_2 - y_1) / (x_2 - x_1);
            double c = y_2 - k * x_2;

            return k * x0 + c;
        }


        public static Point2D? Intersection(List<Point2D> polyline, Line2D other)
        {
            Line2D seg;
            for (int i = 0; i < polyline.Count-1; i++)
            {
                seg = new Line2D(polyline[i], polyline[i+1]);
                var pt=(Point2D)seg.IntersectWith(other);
                if (pt != null)
                {
             
                    if (pt.Equals(seg.StartPoint, 1e-4) || pt.Equals(seg.EndPoint, 1e-4))
                    {
                        return pt;
                    }
                    else
                    {
                        var d1 = seg.StartPoint.DistanceTo(pt);
                        var d2 = seg.EndPoint.DistanceTo(pt);
                        if (d1 < seg.Length && d2 < seg.Length)
                        {
                            return pt;
                        }
                    }
                }
            }
            return null;
        }


        //public static Point2D Interse

    }
}
