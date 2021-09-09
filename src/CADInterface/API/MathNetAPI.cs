using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using MathNet.Spatial.Euclidean;

namespace CADInterface.API
{
    public static class MathNetAPI
    {


        public static Line ToAcad(this Line2D Li) 
        {
            return new Line(Li.StartPoint.ToAcadPoint(), Li.EndPoint.ToAcadPoint());
        }

        public static Point3d ToAcadPoint(this Point2D pt)
        {
            return new Point3d(pt.X, pt.Y,0);
        }  
        public static Point2d ToAcadPoint2d(this Point2D pt)
        {
            return new Point2d(pt.X, pt.Y);
        }

        public static bool GT(this Line2D li, double value)
        {
            return (li.StartPoint.X > value && li.EndPoint.X > value);
        }
        public static bool LT(this Line2D li, double value)
        {
            return (li.StartPoint.X< value && li.EndPoint.X < value);
        }


        public static bool WithIn(this Line2D li, double value)
        {
            if (li.StartPoint.X > value && li.EndPoint.X < value)
            {
                return true;
            }
            if (li.StartPoint.X < value && li.EndPoint.X > value)
            {
                return true;
            }
            return false;
        }

    }
}
