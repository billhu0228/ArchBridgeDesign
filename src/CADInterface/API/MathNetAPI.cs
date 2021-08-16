using Autodesk.AutoCAD.Geometry;
using MathNet.Spatial.Euclidean;

namespace CADInterface.API
{
    public static class MathNetAPI
    {
        public static Point3d ToAcadPoint(this Point2D pt)
        {
            return new Point3d(pt.X, pt.Y,0);
        }  
        public static Point2d ToAcadPoint2d(this Point2D pt)
        {
            return new Point2d(pt.X, pt.Y);
        }

    }
}
