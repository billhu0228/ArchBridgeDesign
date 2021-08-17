using System;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CADInterface
{

    /// <summary>
    /// 绘图接口封装的通用方法
    /// </summary>
    public static class CommonTools
    {


        /// <summary>
        /// 获取与给定点指定角度和距离的点
        /// </summary>
        /// <param name="point">给定点</param>
        /// <param name="angle">角度</param>
        /// <param name="dist">距离</param>
        /// <returns>返回与给定点指定角度和距离的点</returns>
        public static Point3d PolarPoint(this Point3d point, double angle, double dist)
        {
            return new Point3d(point.X + dist * Math.Cos(angle), point.Y + dist * Math.Sin(angle), point.Z);
        }

        /// <summary>
        /// 根据点 做垂线 获取与多线的交点  
        /// </summary>
        /// <param name="mainLine"></param>
        /// <param name="pt0"></param>
        /// <returns></returns>
        public static Point2d GetIntersectPoint(Polyline mainLine, Point2d pt0)
        {
            Point3dCollection pts = new Point3dCollection();
            Line line1 = new Line(pt0.Convert3D(), pt0.Convert3D(0, -1000));
            pts.Clear();
            mainLine.IntersectWith(line1, Intersect.ExtendBoth, pts, IntPtr.Zero, IntPtr.Zero);
            Point2d pt = pts[0].Convert2D();
            return pt;
        }
        /// <summary>
        /// 获取两个点之间的中点
        /// </summary>
        /// <param name="pt1">第一点</param>
        /// <param name="pt2">第二点</param>
        /// <returns>返回两个点之间的中点</returns>
        public static Point3d MidPoint(Point3d pt1, Point3d pt2)
        {
            Point3d midPoint = new Point3d((pt1.X + pt2.X) / 2.0,
                                        (pt1.Y + pt2.Y) / 2.0,
                                        (pt1.Z + pt2.Z) / 2.0);
            return midPoint;
        }
        public static Point3d DivPoint(Point3d pt1, Point3d pt2, double div = 2, bool top = false)
        {
            Point3d midPoint = new Point3d(pt1.X + (pt2.X - pt1.X) / div,
                                        pt1.Y + (pt2.Y - pt1.Y) / div,
                                        pt1.Z + (pt2.Z - pt1.Z) / div);
            if (top)
            {
                midPoint = new Point3d(pt1.X + (pt2.X - pt1.X) * (div - 1) / div,
                                        pt1.Y + (pt2.Y - pt1.Y) * (div - 1) / div,
                                        pt1.Z + (pt2.Z - pt1.Z) * (div - 1) / div);
            }
            return midPoint;
        }
        /// <summary>
        /// 计算从第一点到第二点所确定的矢量与X轴正方向的夹角
        /// </summary>
        /// <param name="pt1">第一点</param>
        /// <param name="pt2">第二点</param>
        /// <returns>返回两点所确定的矢量与X轴正方向的夹角</returns>
        public static double AngleFromXAxis(this Point3d pt1, Point3d pt2)
        {
            //构建一个从第一点到第二点所确定的矢量
            Vector2d vector = new Vector2d(pt1.X - pt2.X, pt1.Y - pt2.Y);
            //返回该矢量和X轴正半轴的角度（弧度）
            return vector.Angle;
        }

        /// <summary>
        /// 通过三角函数求终点坐标
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="StartPoint">起点</param>
        /// <param name="distance">距离</param>
        /// <returns>终点坐标</returns>
        public static Point2d GetEndPointByTrigonometric(double angle, Point2d StartPoint, double distance)
        {
            Point2d EndPoint = new Point2d();

            //角度转弧度
            var radian = (angle * Math.PI) / 180;

            //计算新坐标 r 就是两者的距离
            double X = StartPoint.X + distance * Math.Cos(radian);
            double Y = StartPoint.Y + distance * Math.Sin(radian);
            EndPoint = new Point2d(X, Y);
            return EndPoint;
        }










    }
}
