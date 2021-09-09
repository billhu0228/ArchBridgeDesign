using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using HPDI.DrawingStandard;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADInterface.Plotters
{
    public static class MyPloter
    {
        public static DBObjectCollection XG(DBObjectCollection SlaveTube, Curve MasterLine, double SlaveD, double MasterD, int dir)
        {
            DBObjectCollection res = new DBObjectCollection();

            Line L1 = (Line)SlaveTube[0];
            Line L2 = (Line)SlaveTube[2];

            Point3dCollection pts = new Point3dCollection();
            L1.IntersectWith(MasterLine, Intersect.ExtendBoth, pts, IntPtr.Zero, IntPtr.Zero);
            var P1 = pts[0];
            pts = new Point3dCollection();
            L2.IntersectWith(MasterLine, Intersect.ExtendBoth, pts, IntPtr.Zero, IntPtr.Zero);
            var P2 = pts[0];
            Point3d St = P1;
            Point3d Ed = P2;
            if (P1.X > P2.X)
            {
                St = P2;
                Ed = P1;
            }

            double CoordL = P1.DistanceTo(P2);

            double offset = 0.5 * MasterD - Math.Sqrt(Math.Pow(MasterD * 0.5, 2) - Math.Pow(SlaveD * 0.5, 2));

            double XR = Math.Sqrt(Math.Pow(CoordL * 0.5, 2) + Math.Pow(offset, 2));

            Math.Sqrt(Math.Pow(XR, 2) - Math.Pow(CoordL * 0.5, 2));


            double R = offset * 0.5 + Math.Pow(CoordL, 2) / (8 * offset);

            Vector3d Vx = (Ed - St).GetNormal();
            Vector3d Vy = Vx.RotateBy(Angle.FromDegrees(90).Radians, Vector3d.ZAxis);
            Point3d Center = (St + 0.5 * CoordL * Vx - dir * Vy * (offset - R));

            Vector3d Vst = (St - Center);
            Vector3d Ved = (Ed - Center);


            Vector3D Xaxis = new Vector3D(1, 0, 0);

            Vector3D v1 = new Vector3D(Vst.X, Vst.Y, Vst.Z);
            Vector3D v2 = new Vector3D(Ved.X, Ved.Y, Ved.Z);

            double StA = Xaxis.SignedAngleTo(v1, UnitVector3D.ZAxis).Radians;
            double EdA = Xaxis.SignedAngleTo(v2, UnitVector3D.ZAxis).Radians;


            if (StA < EdA)
            {
                res.Add(new Arc(Center, R, StA, EdA) { Layer = "H细线" });
            }
            else
            {
                res.Add(new Arc(Center, R, EdA, StA) { Layer = "H细线" });
            }

            return res;
        }


        //public static List<Polyline> Trim(this Polyline PL, Point2d Pa, Point2d Pb)
        //{
        //    List<Polyline> obj = new List<Polyline>();
        //    List<Point2d> lstA = new List<Point2d>();
        //    List<Point2d> lstB = new List<Point2d>();

        //    for (int i = 0; i < PL.NumberOfVertices; i++)
        //    {
        //        var pt = PL.GetPoint2dAt(i);
        //        if (pt.X<Pa.X)
        //        {
        //            lstA.Add(pt);
        //        }
        //        if (pt.X>Pb.X)
        //        {
        //            lstB.Add(pt);
        //        }
        //    }

        //    obj.Add((Polyline)PLPloter.AddPolylineByPointList(lstA, PL.Layer, false)[0]);
        //    obj.Add((Polyline)PLPloter.AddPolylineByPointList(lstB, PL.Layer, false)[0]);
            

        //    obj.Add(  (Polyline)
                
        //        Polyline.CreateFromGeCurve
        //        PL.GetSplitCurves(new Point3dCollection() { PL.StartPoint, Pa.C3D() })
                
                
                
        //        );
        //    obj.Add(  PL.GetSplitCurves(new Point3dCollection() { Pb.C3D(), PL.EndPoint}));
        //    return obj;
        //}

        //public static DBObjectCollection TrimByCollection(this Polyline pl,DBObjectCollection trims)
        //{





        //}


    }
}
