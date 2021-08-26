using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADInterface.Plotters;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HPDI.DrawingStandard;
using MathNet.Spatial.Units;

namespace CADInterface.API
{
    public static class ArchAPI
    {

        /// <summary>
        /// 绘制左半拱圈
        /// </summary>
        /// <param name="theArch"></param>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        public static void DrawingLeftElevation(this Arch theArch, out Extents2d ext)
        {
            #region 总体
            DBObjectCollection obj = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();

            Point3d ft = new Point3d(-0.5 * theArch.Axis.L, -theArch.Axis.f, 0);
            Point3d cc = Point3d.Origin;
            Point2d pref = new Point2d(0, 25);
            double LengthArch = theArch.Axis.L;
            double FArch = theArch.Axis.f;
            #endregion



            #region 轴线
            List<Point2d> ls = new List<Point2d>();
            Polyline UpPL = null, LowPL = null, CenterLine = null;
            double x0 = theArch.MainDatum[0].Center.X;
            while (x0 < 0)
            {
                ls.Add(theArch.Axis.GetCenter(x0).ToAcadPoint2d());
                x0 += 1;
            }
            ls.Add(theArch.Axis.GetCenter(0).ToAcadPoint2d());
            ls.Sort((x, y) => x.X.CompareTo(y.X));

            Extensions.Add(obj, PLPloter.AddPolylineByPointList(ls, "H中心线", false));
            obj.Add(new Circle(ft, Vector3d.ZAxis, 2) { Layer = "H中心线" });
            
            var Dim= DimPloter.DimRot(ft, ft.C3D(0.5 * theArch.Axis.L), ft.C3D(0, -15), 0, 1,
                string.Format("{0}/2", LengthArch * 100), Unit.Meter, Unit.Centimeter);
            obj.Add(Dim);
            Dim = DimPloter.DimRot(cc, ft, cc.C3D(10), 90, 1,"f=<>", Unit.Meter, Unit.Centimeter);
            obj.Add(Dim);
            var CL = new Line(cc, cc.C3D(0, -FArch)) { Layer = "H中心线" };
            obj.Add(CL);
            obj.Add(TextPloter.AddDBText(cc.C3D(0, -0.5 * FArch), "主拱中心线", 1, 2.5, "H仿宋", Angle.FromDegrees(-90).Radians,
                TextHorizontalMode.TextCenter,TextVerticalMode.TextTop));
            #endregion

            #region 拱圈
            for (int k = 0; k < 2; k++)
            {
                eMemberType et = k == 0 ? eMemberType.LowerCoord : eMemberType.UpperCoord;
                var kk1 = (from item in theArch.MemberTable where item.ElemType == et select item.Line.StartPoint.ToAcadPoint2d()).ToList();
                var kk2 = (from item in theArch.MemberTable where item.ElemType == et select item.Line.EndPoint.ToAcadPoint2d()).ToList();
                ls = (kk1.Concat(kk2)).ToList();
                ls = ls.FindAll(x => x.X <= 0.1);
                ls = ls.Distinct().ToList();
                ls.Sort((x, y) => x.X.CompareTo(y.X));

                var CC = (Polyline)PLPloter.AddPolylineByPointList(ls, "H中心线", false)[0];
                obj.Add(CC);

                var member = (from item in theArch.MemberTable where item.ElemType == eMemberType.UpperCoord select item).ToList()[0];
                for (int i = 0; i < 2; i++)
                {
                    int dir = i == 0 ? -1 : 1;
                    var mm = (Polyline)CC.GetOffsetCurves(member.Sect.Diameter * 0.5 * dir)[0];
                    mm.Layer = "H粗线";
                    obj.Add(mm);
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
            var pts=theArch.get_3pt_real(-0.5 * LengthArch);
            obj.Add(DimPloter.DimAli(pts[0].ToAcadPoint(), pts[2].ToAcadPoint(), pts[0].ToAcadPoint().C3D(-10),
                1, "", Unit.Meter, Unit.Centimeter));
            pts=theArch.get_3pt_real(0);
            obj.Add(DimPloter.DimAli(pts[0].ToAcadPoint(), pts[2].ToAcadPoint(), pts[0].ToAcadPoint().C3D(5),
                1, "", Unit.Meter, Unit.Centimeter));

            foreach (var item in theArch.MemberTable)
            {
                if (item.ElemType != eMemberType.UpperCoord && item.ElemType != eMemberType.LowerCoord)
                {
                    if (item.Line.MiddlePoint().X > 0)
                    {
                        continue;
                    }
                    var web = MLPloter.AddTube(item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter, 0, "H细线", LowPL, UpPL);
                    //MulitlinePloterO.PlotTube(db, item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter, LowPL, UpPL,0, "细线") ;
                    Extensions.Add(obj, web);
                }
            }
            #endregion

            #region 立柱
            Extents2d colExt= new Extents2d();
            List<Extents2d> extList = new List<Extents2d>();
            foreach (var theCol in theArch.ColumnList)
            {
                if (theCol.ID == 0)
                {
                    if (theCol.X < 0)
                    {
                        theCol.DarwColumnSide(Point2d.Origin, out colExt);
                        extList.Add(colExt) ;
                    }
                }
            }
            for (int i = 0; i < theArch.ColumnList.Count/2; i++)
            {
                Model.Column theColA = theArch.ColumnList[i];
                Model.Column theColB = theArch.ColumnList[i+1];
                Point2d pA = new Point2d(theColA.X, theColA.Z2);
                Point2d pB = new Point2d(theColB.X, theColB.Z2);
                obj.Add(TextPloter.AddDBText(pA.C3D(0, 2), string.Format("LZ-{0}(LZ-{1})", 
                    i + 1, theArch.ColumnList.Count - i), 1, 2.5,  "H仿宋", 0, TextHorizontalMode.TextCenter));

                if (i==0)
                {
                    obj.Add(DimPloter.DimRot(pA.C3D(), pB.C3D(), pref.C3D(), 0, 1, "", Unit.Meter, Unit.Centimeter));
                    pA = new Point2d(-0.5*LengthArch, theColA.Z2);
                    pB = new Point2d(theColA.X, theColA.Z2);
                    obj.Add(DimPloter.DimRot(pA.C3D(), pB.C3D(), pref.C3D(), 0, 1, "", Unit.Meter, Unit.Centimeter));
                }
                if (i == theArch.ColumnList.Count / 2 - 1)
                {
                    pB = new Point2d(0, theColA.Z2);
                    obj.Add(DimPloter.DimRot(pA.C3D(), pB.C3D(), pref.C3D(), 0, 1, "", Unit.Meter, Unit.Centimeter));
                }
                else
                {
                    obj.Add(DimPloter.DimRot(pA.C3D(), pB.C3D(), pref.C3D(), 0, 1, "", Unit.Meter, Unit.Centimeter));
                }


            }
            // obj.Add(DimPloter.DimRot(pA.C3D(), pB.C3D(), pref.C3D(), 0, 1, "", Unit.Meter, Unit.Centimeter));


            #endregion

            #region 拱脚和交界墩
            #endregion
            #region 壁厚表
            #endregion
            #region 断面块
            #endregion

            #region 其他注释
            obj.Add(TextPloter.AddTitle(pref.C2D(-LengthArch * 0.25,15), "1/2拱圈立面",""));
            #endregion

            ids = Ploter.WriteDatabase(obj);
            Extents2d extArch = Ploter.GetExtendds(ids);

            foreach (var item in extList)
            {
                extArch = extArch.Add(item);
            }
            ext = extArch;

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
                    MulitlinePloterO.PlotTube(db, item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter,
                        LowPL, UpPL, 0, "细线");
                }
            }
        }

        public static void DrawInstall(this Arch model,out Extents2d ext)
        {
            DBObjectCollection obj = new DBObjectCollection();

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

                var PL=(Polyline)PLPloter.AddPolylineByPointList(pts, "H虚线", false)[0];
                //PolylinePloter.AddPolylineByList(db, ref ext, pts, "虚线", false);
                obj.Add(PL);

                ptForDim.Add(pts[1].Convert3D());
            }

            ptForDim.Add(model.LeftFoot[2].ToAcadPoint());
            ptForDim.Add(model.MidPoints[2].ToAcadPoint());
            ptForDim.Sort((x, y) => x.X.CompareTo(y.X));

            for (int i = 0; i < ptForDim.Count - 1; i++)
            {
                string rep = string.Format("节段{0}", i + 1);
                var pt1 = ptForDim[i];
                var pt2 = ptForDim[i + 1];
                var dim= DimPloter.DimAli(pt1, pt2, pt2.C3D(0, -8), 1, rep, Unit.Meter, Unit.Centimeter);
                Extensions.Add(obj, dim);
            }


            var ids = Ploter.WriteDatabase(obj);
            ext = Ploter.GetExtendds(ids);

        }

        public static void DrawingPlan(this Arch model,Point2d cc,out Extents2d ext)
        {
            #region 总体
            DBObjectCollection obj = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();

            Point3d ft = new Point3d(-0.5 * theArch.Axis.L, -theArch.Axis.f, 0);
            Point3d cc = Point3d.Origin;
            Point2d pref = new Point2d(0, 25);
            double LengthArch = theArch.Axis.L;
            double FArch = theArch.Axis.f;
            #endregion


            #region 拱肋
            #endregion

            #region 风撑
            #endregion

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


                var ret = MulitlinePloterO.PlotTube(db, p1.Convert2D(0, y0), p2.Convert2D(0, y0), model.MainTubeDiameter);
                FrameLine.Add(ret);
            }

            foreach (var item in model.MainDatum)
            {
                if (item.DatumType==eDatumType.ColumnDatum)
                {
                    var p0 = new Point2d(item.Center.X, 0);
                    p0=p0.TransformBy(mat);

                    MulitlinePloterO.PlotTube(db,p0.Convert2D(0,-0.5*model.WidthInside+0.5*model.MainTubeDiameter),
                        p0.Convert2D(0, 0.5 * model.WidthInside - 0.5 * model.MainTubeDiameter),model.CrossBracingDiameter,null,null,0,"细线");
                    MulitlinePloterO.PlotTube(db, p0.Convert2D(0, -0.5 * model.WidthInside-model.WidthOutside + 0.5 * model.MainTubeDiameter),
                        p0.Convert2D(0, -0.5 * model.WidthInside  - 0.5 * model.MainTubeDiameter), model.CrossBracingDiameter, null, null, 0, "细线");

                    MulitlinePloterO.PlotTube(db, p0.Convert2D(0, +0.5 * model.WidthInside + model.WidthOutside - 0.5 * model.MainTubeDiameter),
                        p0.Convert2D(0, +0.5 * model.WidthInside + 0.5 * model.MainTubeDiameter), model.CrossBracingDiameter, null, null, 0, "细线");
                }

            }



        }

    }
}
