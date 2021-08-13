using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADInterface.Plotters;
using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADInterface
{
    public static class ArchE
    {
        public static Point3d ToAcadPoint(this Point2D pt)
        {
            return new Point3d(pt.X, pt.Y,0);
        }  
        public static Point2d ToAcadPoint2d(this Point2D pt)
        {
            return new Point2d(pt.X, pt.Y);
        }


        public static void Drawing(this Arch model, Database db, ref Extents2d ext)
        {
            List<Point2d> ls;

            Polyline UpPL=null, LowPL=null;

            for (int k = 0; k < 2; k++)
            {
                eMemberType et = k == 0 ? eMemberType.LowerCoord : eMemberType.UpperCoord;
                var kk1 = (from item in model.MemberTable where item.ElemType == et select item.Line.StartPoint.ToAcadPoint2d()).ToList();
                var kk2 = (from item in model.MemberTable where item.ElemType == et select item.Line.EndPoint.ToAcadPoint2d()).ToList();

                ls = (kk1.Concat(kk2)).ToList();
                ls = ls.Distinct().ToList();
                ls.Sort((x, y) => x.X.CompareTo(y.X));

                var CC = PolylinePloter.AddPolylineByList(db, ref ext, ls, "中心线", false);

                var member = (from item in model.MemberTable where item.ElemType == eMemberType.UpperCoord select item).ToList()[0];

                for (int i = 0; i < 2; i++)
                {
                    int dir = i == 0 ? -1 : 1;
                    var mm = (Polyline)CC.GetOffsetCurves(member.Sect.Diameter * 0.5 * dir)[0];
                    mm.Layer = "粗线";
                    db.AddEntityToModeSpace(mm);
                    if (k==0 && i==0)
                    {
                        LowPL = mm;
                    }             
                    if (k==1 && i==1)
                    {
                        UpPL = mm;
                    }
                }
            }

            foreach (var item in model.MemberTable)
            {
                if (item.ElemType!=eMemberType.UpperCoord && item.ElemType!=eMemberType.LowerCoord)
                {
                    MulitlinePloter.PlotTube(db, item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter,
                        LowPL, UpPL,0, "细线") ;



                }


            }




        }


    }
}
