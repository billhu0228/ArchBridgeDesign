using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADInterface.Plotters;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADInterface.API
{
    public static class ArchAPI
    {

        /// <summary>
        /// 绘制左半拱圈
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        public static void DrawingLeftElevation(this Arch model, Database db, ref Extents2d ext)
        {
            List<Point2d> ls = new List<Point2d>() ;

            Polyline UpPL=null, LowPL=null, CenterLine=null;
            double x0 = model.MainDatum[0].Center.X;
            while (x0<0)
            {
                ls.Add(model.Axis.GetCenter(x0).ToAcadPoint2d());
                x0 += 1;
            }
            ls.Add(model.Axis.GetCenter(0).ToAcadPoint2d());
            ls.Sort((x, y) => x.X.CompareTo(y.X));
            PolylinePloter.AddPolylineByList(db, ref ext, ls, "中心线", false);


            for (int k = 0; k < 2; k++)
            {                
                eMemberType et = k == 0 ? eMemberType.LowerCoord : eMemberType.UpperCoord;
                var kk1 = (from item in model.MemberTable where item.ElemType == et select item.Line.StartPoint.ToAcadPoint2d()).ToList();
                var kk2 = (from item in model.MemberTable where item.ElemType == et select item.Line.EndPoint.ToAcadPoint2d()).ToList();
                ls = (kk1.Concat(kk2)).ToList();
                ls = ls.FindAll(x => x.X <= 0.1);
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
                    if (item.Line.MiddlePoint().X>0)
                    {
                        continue;
                    }
                    MulitlinePloter.PlotTube(db, item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter,
                        LowPL, UpPL,0, "细线") ;
                }
            }
        }

        /// <summary>
        /// 绘制拱圈
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        public static void DrawingElevation(this Arch model, Database db, ref Extents2d ext)
        {
            List<Point2d> ls = new List<Point2d>();

            Polyline UpPL = null, LowPL = null, CenterLine = null;


            double x0 = model.MainDatum[0].Center.X;
            while (x0 < 0)
            {
                ls.Add(model.Axis.GetCenter(x0).ToAcadPoint2d());
                ls.Add(model.Axis.GetCenter(-x0).ToAcadPoint2d());
                x0 += 1;
            }
            ls.Add(model.Axis.GetCenter(0).ToAcadPoint2d());
            ls.Sort((x, y) => x.X.CompareTo(y.X));

            PolylinePloter.AddPolylineByList(db, ref ext, ls, "中心线", false);


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
                    if (k == 0 && i == 0)
                    {
                        LowPL = mm;
                    }
                    if (k == 1 && i == 1)
                    {
                        UpPL = mm;
                    }
                }
            }

            foreach (var item in model.MemberTable)
            {
                if (item.ElemType != eMemberType.UpperCoord && item.ElemType != eMemberType.LowerCoord)
                {
                    MulitlinePloter.PlotTube(db, item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter,
                        LowPL, UpPL, 0, "细线");
                }
            }
        }

        public static void DrawInstall(this Arch model, Database db, ref Extents2d ext)
        {
            var vd = from dt in model.MainDatum
                     where dt.DatumType == eDatumType.ColumnDatum || dt.DatumType == eDatumType.VerticalDatum
                     select dt;
            var datumList= vd.ToList();
            datumList.Sort(new DatumPlane());


            List<Point3d> ptForDim = new List<Point3d>() ;
            for (int i = 0; i < datumList.Count; i++)
            {
                var theDat = datumList[i];
                if (i%2!=0 || theDat.Center.X>0)
                {
                    continue;
                }
                DatumPlane install= model.SecondaryDatum.Find(x => x.Center.X > theDat.Center.X);

                var pts= (from p in  model.Get3PointReal(install) select p.ToAcadPoint2d()).ToList();
                pts.Add(pts[0].Convert2D(0, 2));
                pts.Add( pts[2].Convert2D(0, -2));
                pts.Sort((x, y) => x.Y.CompareTo(y.Y));
                    
                PolylinePloter.AddPolylineByList(db, ref ext, pts, "虚线", false);

                ptForDim.Add(pts[1].Convert3D());
            }

            ptForDim.Add(model.LeftFoot[2].ToAcadPoint());
            ptForDim.Add(model.MidPoints[2].ToAcadPoint());
            ptForDim.Sort((x, y) => x.X.CompareTo(y.X));

            DimPloter.AddListDimAligned(db, ref ext, ptForDim, 1,-5);



        }

        public static void DrawingPlan(this Arch model,Database db,Point2d cc,ref Extents2d ext)
        {
            Matrix2d mat = Matrix2d.Displacement(cc.GetAsVector());
            List<List<Line>> FrameLine=new List<List<Line>>();

            for (int i = 0; i < 4; i++)
            {
                double y0 =new double[]{ -0.5 * model.WidthInside - model.WidthOutside, -0.5 * model.WidthInside, 0.5 * model.WidthInside, 0.5 * model.WidthInside+model.WidthOutside }[i] ;


                var p1 = i<2? new Point2d(model.Get3PointReal(model.MainDatum[0])[2].X, 0):
                     new Point2d(model.Get3PointReal(model.MainDatum[0])[0].X, 0);
                p1 = p1.TransformBy(mat);
                var p2 = new Point2d(-p1.X, 0);
                p2 = p2.TransformBy(mat);


                var ret = MulitlinePloter.PlotTube(db, p1.Convert2D(0, y0), p2.Convert2D(0, y0), model.MainTubeDiameter);
                FrameLine.Add(ret);
            }

            foreach (var item in model.MainDatum)
            {
                if (item.DatumType==eDatumType.ColumnDatum)
                {
                    var p0 = new Point2d(item.Center.X, 0);
                    p0=p0.TransformBy(mat);

                    MulitlinePloter.PlotTube(db,p0.Convert2D(0,-0.5*model.WidthInside+0.5*model.MainTubeDiameter),
                        p0.Convert2D(0, 0.5 * model.WidthInside - 0.5 * model.MainTubeDiameter),model.CrossBracingDiameter,null,null,0,"细线");
                    MulitlinePloter.PlotTube(db, p0.Convert2D(0, -0.5 * model.WidthInside-model.WidthOutside + 0.5 * model.MainTubeDiameter),
                        p0.Convert2D(0, -0.5 * model.WidthInside  - 0.5 * model.MainTubeDiameter), model.CrossBracingDiameter, null, null, 0, "细线");

                    MulitlinePloter.PlotTube(db, p0.Convert2D(0, +0.5 * model.WidthInside + model.WidthOutside - 0.5 * model.MainTubeDiameter),
                        p0.Convert2D(0, +0.5 * model.WidthInside + 0.5 * model.MainTubeDiameter), model.CrossBracingDiameter, null, null, 0, "细线");
                }

            }



        }

    }
}
