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
    /// 一些共用扩展
    /// </summary>
    public static class Extension
    {
        public static void MySort(this List<Column> theList)
        {
            theList.Sort((x, y) => x.X.CompareTo(y.X));
            for (int i = 0; i < theList.Count; i++)
            {
                theList[i].ID = i;
            }
        }

        public static void MySort(this List<DatumPlane> theList)
        {
            theList.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));
            for (int i = 0; i < theList.Count; i++)
            {
                theList[i].ID = i;

            }
        }

        public static Point2D C2D(this Point3D pt)
        {
            return new Point2D(pt.X, pt.Y);
        }

        /// <summary>
        /// 值域
        /// </summary>
        /// <param name="vv"></param>
        /// <param name="vto"></param>
        /// <returns></returns>
        public static double SignedAngleBetween(this Vector2D vv, Vector2D vto)
        {
            var ang = vv.SignedAngleTo(vto);// 双否参数，默认值域（0~360）,后来发现不可用

            if (ang.Radians < -Math.PI)
            {
                return ang.Radians + 2 * Math.PI;

            }
            else if (ang.Radians < Math.PI)
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

            return t * kk + C0;
        }

        public static Line2D Offset(this Line2D L, double val)
        {
            Vector2D xv = L.Direction.Rotate(Angle.FromDegrees(90.0));
            Point2D NewSt = L.StartPoint + xv * val;
            Point2D NewEd = L.EndPoint + xv * val;
            return new Line2D(NewSt, NewEd);
        }

        public static Point2D MiddlePoint(this Line2D L)
        {
            return L.StartPoint + 0.5 * L.Direction * L.Length;
        }

        /// <summary>
        /// 通过点PT与的与圆相切的直线的圆周点
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="cir"></param>
        /// <returns></returns>
        public static List<Point2D> Tangent(this Point2D pt, Circle2D cir)
        {
            var r = cir.Radius;
            var po = cir.Center - pt;
            var pob = Angle.FromRadians(Math.Acos(cir.Radius / po.Length));
            var ob = (-po).Rotate(-pob).Normalize();
            var oc = (-po).Rotate(pob).Normalize();
            var res = new List<Point2D>() { cir.Center + ob * r, cir.Center + oc * r };
            res.Sort((x, y) => x.Y.CompareTo(y.Y));
            return res;

        }

        /// <summary>
        /// 点是否在线段上
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="seg"></param>
        /// <returns></returns>
        public static bool OnSegment(this Point2D pt, LineSegment2D seg)
        {
            var f = seg.ClosestPointTo(pt);

            if (f.DistanceTo(pt) < 1e-6)
            {
                return true;
            }
            return false;
        }

        public static double Interplate(double x_1, double x_2, double y_1, double y_2, double x0)
        {
            double k = (y_2 - y_1) / (x_2 - x_1);
            double c = y_2 - k * x_2;

            return k * x0 + c;
        }

        public static Point2D? Intersection(this List<Point2D> polyline, Line2D other)
        {
            List<Point2D> res = new List<Point2D>();
            Line2D seg;
            for (int i = 0; i < polyline.Count - 1; i++)
            {
                seg = new Line2D(polyline[i], polyline[i + 1]);
                var pt = (Point2D)seg.IntersectWith(other);
                if (pt != null)
                {

                    if (pt.Equals(seg.StartPoint, 1e-6) || pt.Equals(seg.EndPoint, 1e-6))
                    {
                        res.Add(pt);
                    }
                    else
                    {
                        var d1 = seg.StartPoint.DistanceTo(pt);
                        var d2 = seg.EndPoint.DistanceTo(pt);
                        if (d1 < seg.Length && d2 < seg.Length)
                        {
                            res.Add(pt);
                        }
                    }
                }
            }
            if (res.Count == 1)
            {
                return res[0];
            }
            else if (res.Count == 0)
            {
                return null;
            }
            else
            {
                List<Tuple<double, Point2D>> arr = new List<Tuple<double, Point2D>>();
                foreach (var item in res)
                {
                    arr.Add(new Tuple<double, Point2D>(item.DistanceTo(other.StartPoint), item));

                }
                arr.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                return arr[0].Item2;
            }
        }

        public static bool IsOutside(this Circle2D c, Point2D pt)
        {
            return pt.DistanceTo(c.Center) > c.Radius;
        }

        public static List<Point2D> IntersectWith(this LineSegment2D seg, Circle2D cir)
        {
            List<Point2D> ret = new List<Point2D>();
            var pt = seg.ClosestPointTo(cir.Center);
            if (pt.DistanceTo(cir.Center) < 1e-7)
            {
                //共线
                Line2D theLine = new Line2D(seg.StartPoint, seg.EndPoint);
                Vector2D v = seg.Direction;
                double m = cir.Radius;

                Point2D p1 = cir.Center + v * m;
                Point2D p2 = cir.Center + v * -m;

                if (p1.OnSegment(seg))
                {
                    ret.Add(p1);
                }
                if (p2.OnSegment(seg))
                {
                    ret.Add(p2);
                }
                return ret;
            }
            if (seg.LineTo(cir.Center).Length > cir.Radius)
            {
                return new List<Point2D>();
            }
            else if (seg.StartPoint.DistanceTo(cir.Center) < cir.Radius &&
                seg.EndPoint.DistanceTo(cir.Center) < cir.Radius)
            {
                return new List<Point2D>();
            }
            else
            {

                Line2D theLine = new Line2D(seg.StartPoint, seg.EndPoint);
                Vector2D n = theLine.ClosestPointTo(cir.Center, false) - cir.Center;

                Vector2D v = n.Rotate(Angle.FromDegrees(90)).Normalize();
                if (n.Length < 1e-6)
                {
                    v = theLine.Direction;
                }
                double m = Math.Sqrt(Math.Pow(cir.Radius, 2) - Math.Pow(n.Length, 2));

                Point2D p1 = cir.Center + n + v * m;
                Point2D p2 = cir.Center + n + v * -m;

                if (p1.OnSegment(seg))
                {
                    ret.Add(p1);
                }
                if (p2.OnSegment(seg))
                {
                    ret.Add(p2);
                }
            }
            return ret;
        }

        public static List<Point2D> Intersection(this List<Point2D> polyline, Circle2D other)
        {
            List<Point2D> res = new List<Point2D>();
            List<Point2D> tmp = new List<Point2D>();
            LineSegment2D seg;
            for (int i = 0; i < polyline.Count - 1; i++)
            {
                seg = new LineSegment2D(polyline[i], polyline[i + 1]);
                var m = seg.IntersectWith(other);
                foreach (Point2D item in m)
                {
                    tmp.Add(item);
                }
            }

            if (tmp.Count > 2)
            {
                var p1 = tmp[0];
                List<double> dist = new List<double>();
                for (int i = 1; i < tmp.Count - 1; i++)
                {
                    dist.Add(tmp[i].DistanceTo(p1));
                }

                var p2 = tmp[dist.FindIndex(x => x == dist.Max()) + 1];
                return new List<Point2D>() { p1, p2 };
            }
            else
            {
                return tmp;
            }

        }




        public static DatumPlane Datum(this Line2D line, ref ArchAxis ax)
        {
            Point2D cc = ax.Intersect(line);
            var dir = line.Direction.Y < 0 ? -line.Direction : line.Direction;
            return new DatumPlane(0, cc, cc + dir, eDatumType.GeneralDatum);
        }
        //public static Point2D Interse

    }
}
