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
            Point3d jjd = new Point3d(theArch.RCColumnList[0].X, theArch.RCColumnList[0].H0,0);
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
                    if (item.ElemType==eMemberType.InclineWebS)
                    {
                        double xc = item.Line.StartPoint.X;
                        var cuts=theArch.get_3pt_real(xc);

                        Line bd1 = new Line(cuts[0].ToAcadPoint(), cuts[2].ToAcadPoint());
                        bd1=(Line) bd1.Offset(theArch.WebTubeDiameter * 0.5)[0];

                        var bd2 = LowPL;
                        if (item.Line.EndPoint.Y > theArch.Axis.GetZ(item.Line.EndPoint.X))
                        {
                            bd2 = UpPL;
                        }


                        var web = MLPloter.AddTube(item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter, 0, "H细线", bd1, bd2);

                        Extensions.Add(obj, web);
                    }
                    else
                    {
                        var web = MLPloter.AddTube(item.Line.StartPoint.ToAcadPoint2d(), item.Line.EndPoint.ToAcadPoint2d(), item.Sect.Diameter, 0, "H细线", LowPL, UpPL);
                       
                        Extensions.Add(obj, web);
                    }
                    
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
                    pA = new Point2d(theArch.RCColumnList[0].X, theColA.Z2);
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
            string rep = string.Format("{0}/2", jjd.X * -2);

            obj.Add(DimPloter.DimRot(jjd,cc, pref.C3D(0,5), 0, 1, rep, Unit.Meter, Unit.Centimeter));


            #endregion

            #region 拱脚和交界墩
            RCColumn colRC = theArch.RCColumnList[0];
            colRC.DrawRCColumnSide(new Vector2d(), out colExt);
            extList.Add(colExt);

            Database db = HostApplicationServices.WorkingDatabase;
            BlockPloter.CopyBlockFromFile(db, "block\\BlockDef.dwg", "L拱脚立面");


            obj.Add(TextPloter.AddDBText(jjd.C3D(0,colRC.H2-colRC.H0+2), "交界墩", 1, 2.5, "H仿宋", 0, TextHorizontalMode.TextCenter));
            ids.Add(BlockPloter.InsertBlockReference(ft.C3D(), 1, "L拱脚立面", null, null));

            #endregion





            #region 壁厚表
            #endregion
            #region 断面块
            #endregion

            #region 其他注释
            obj.Add(TextPloter.AddTitle(pref.C2D(-LengthArch * 0.25,15), "1/2拱圈立面",""));
            #endregion

            #region 输出
            var newids = Ploter.WriteDatabase(obj);

            foreach (ObjectId item in newids)
            {
                ids.Add(item);
            }

            Extents2d extArch = Ploter.GetExtends(ids);

            foreach (var item in extList)
            {
                extArch = extArch.Add(item);
            }
            ext = extArch;
            #endregion
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

        public static void DrawInstall(this Arch model,Point2d cc,out Extents2d ext,bool isElev=true)
        {
            DBObjectCollection obj = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();

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
                if (isElev)
                {
                    pts.Add(pts[0].C2D(0, 2));
                    pts.Add(pts[2].C2D(0, -2));
                }
                else
                {
                    pts = new List<Point2d>() { new Point2d(pts[0].X, 0).C2D(0, 0.5*model.Width+2), new Point2d(pts[2].X, 0).C2D(0,-0.5*model.Width -2) };

                }

                pts.Sort((x, y) => x.Y.CompareTo(y.Y));

                var PL=(Polyline)PLPloter.AddPolylineByPointList(pts, "H虚线", false)[0];
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


            #region 输出

            foreach (var item in obj)
            {
                Entity et = (Entity)item;
                et.TransformBy(Matrix3d.Displacement(cc.GetAsVector().C3D()));
            }

            var idsnew = Ploter.WriteDatabase(obj);

            foreach (ObjectId item in idsnew)
            {
                ids.Add(item);
            }
            ext = Ploter.GetExtends(ids);

            #endregion

        }

        public static void DrawingPlan(this Arch theArch,Point2d cc,out Extents2d ext)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            #region 总体
            DBObjectCollection obj = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();
            double L = theArch.Axis.L;
            //Point3d ft = new Point3d(-0.5 * theArch.Axis.L, -theArch.Axis.f, 0);
            //Point3d cc = Point3d.Origin;
            //Point2d pref = new Point2d(0, 25);
            //double LengthArch = theArch.Axis.L;
            //double FArch = theArch.Axis.f;
            #endregion

            #region 轴线

            
            obj.Add(new Line(Point3d.Origin.C3D(0, theArch.WidthInside), Point3d.Origin.C3D(0, -theArch.WidthInside)) {Layer="H中心线" });
            obj.Add(new Line(Point3d.Origin, Point3d.Origin.C3D(-theArch.RCColumnList[1].X-16)) {Layer="H中心线" });

            #endregion

            #region 拱肋

            Point2d PT1 = new Point2d(theArch.get_3pt_real(-0.5 * L)[0].X, theArch.WidthInside * 0.5 + theArch.WidthOutside);
            Point2d PT2 = PT1.C2D(0, -theArch.WidthOutside);
            Point2d PB1 = new Point2d(theArch.get_3pt_real(-0.5 * L)[2].X, -theArch.WidthInside * 0.5 - theArch.WidthOutside);
            Point2d PB2 = PB1.C2D(0, +theArch.WidthOutside);

            List<DBObjectCollection> archRib = new List<DBObjectCollection>();
            foreach (var item in new Point2d[] { PT1, PT2, PB1, PB2 })
            {
                var lines = MLPloter.AddTube(item, item.C2D(-item.X), theArch.MainTubeDiameter, 0, "粗线");
                archRib.Add(lines);
                obj.Add(lines);
            }

            BlockPloter.CopyBlockFromFile(db, "block\\BlockDef.dwg", "L拱脚平面");
            Point2d ft = new Point2d(-0.5 * L, 0);
            ids.Add(BlockPloter.InsertBlockReference((ft + cc.GetAsVector()).C3D(),1, "L拱脚平面", null, null));

            obj.Add(DimPloter.DimRot(Point3d.Origin.C3D(0, -0.5 * theArch.Width), Point3d.Origin.C3D(0, -0.5 * theArch.WidthInside), Point3d.Origin.C3D(5),
                90, 1, "", Unit.Meter, Unit.Centimeter));
            obj.Add(DimPloter.DimRot(Point3d.Origin.C3D(0, -0.5 * theArch.WidthInside), Point3d.Origin.C3D(0, 0.5 * theArch.WidthInside), Point3d.Origin.C3D(5),
                90, 1, "", Unit.Meter, Unit.Centimeter));            
            obj.Add(DimPloter.DimRot(Point3d.Origin.C3D(0, 0.5 * theArch.WidthInside), Point3d.Origin.C3D(0, 0.5 * theArch.Width), Point3d.Origin.C3D(5),
                90, 1, "", Unit.Meter, Unit.Centimeter));            
            obj.Add(DimPloter.DimRot(Point3d.Origin.C3D(0, -0.5 * theArch.Width), Point3d.Origin.C3D(0, 0.5 * theArch.Width), Point3d.Origin.C3D(10),
                90, 1, "", Unit.Meter, Unit.Centimeter));

            #endregion

            #region 下风撑
            List<DatumPlane> carry = new List<DatumPlane>();
            foreach (var item in theArch.MainDatum)
            {
                if (item.DatumType == eDatumType.ColumnDatum || item.DatumType == eDatumType.VerticalDatum || item.DatumType == eDatumType.NormalDatum)
                {
                    if (item.Center.X > 0)
                    {
                        continue;
                    }
                    if (item.Center.X==-240)
                    {
                        continue;
                    }
                    carry.Add(item);
                }
            }
            carry.Sort(new DatumPlane());
            carry.Reverse();
            DBObjectCollection beforeBar = new DBObjectCollection();
            for (int i = 0; i < carry.Count; i++)
            {

                var item = carry[i];

                double xi = item.Center.X;
                if (item.DatumType == eDatumType.NormalDatum)
                {
                    xi = theArch.Get3PointReal(item)[2].X;
                }
                var p0 = new Point2d(xi, 0);

                var curBar = MLPloter.AddTube(p0.Convert2D(0, -0.5 * theArch.WidthInside + 0.5 * theArch.MainTubeDiameter),
                    p0.Convert2D(0,0), theArch.CrossBracingDiameter, 0, "H细线", null, null);
                obj.Add(curBar);

                if (i != 0)
                {
                    if (i % 2 == 0)
                    {
                        var dd = MLPloter.AddTube(new Point2d(theArch.Get3PointReal(carry[i - 1])[2].X, 0), new Point2d(theArch.Get3PointReal(carry[i])[2].X, -0.5 * theArch.WidthInside),
                            theArch.GetTubeProperty(item.Center.X, eMemberType.WebBracing).Section.Diameter, 0, "H细线", (Curve)beforeBar[0], (Curve)archRib[3][0]);
                        obj.Add(dd);
                    }
                    else
                    {
                        var dd = MLPloter.AddTube(new Point2d(theArch.Get3PointReal(carry[i - 1])[2].X, -0.5 * theArch.WidthInside), new Point2d(theArch.Get3PointReal(carry[i])[2].X, 0),
                            theArch.GetTubeProperty(item.Center.X, eMemberType.WebBracing).Section.Diameter, 0, "H细线", (Curve)archRib[3][0], (Curve)curBar[2]);
                        obj.Add(dd);
                    }

                }

                //下平面
                obj.Add(MLPloter.AddTube(p0.Convert2D(0, -0.5 * theArch.WidthInside - theArch.WidthOutside + 0.5 * theArch.MainTubeDiameter),
                    p0.Convert2D(0, -0.5 * theArch.WidthInside - 0.5 * theArch.MainTubeDiameter), theArch.CrossBracingDiameter, 0, "细线", null, null));
                beforeBar.Clear();
                foreach (DBObject kk in curBar)
                {
                    beforeBar.Add(kk);
                }

            }

            #endregion

            #region 上风撑
            carry = new List<DatumPlane>();
            foreach (var item in theArch.MainDatum)
            {
                if (item.DatumType == eDatumType.ColumnDatum || item.DatumType == eDatumType.VerticalDatum || item.DatumType==eDatumType.NormalDatum)
                {
                    if (item.Center.X > 0)
                    {
                        continue;
                    }
                    carry.Add(item);
                }
            }
            carry.Sort(new DatumPlane());
            carry.Reverse();
            beforeBar = new DBObjectCollection();
            for (int i = 0; i < carry.Count; i++)
            {

                var item = carry[i];

                double xi = item.Center.X;
                if (item.DatumType == eDatumType.NormalDatum)
                {
                    xi= theArch.Get3PointReal(item)[0].X;
                }
                var  p0 = new Point2d(xi, 0);

                var curBar = MLPloter.AddTube(p0.Convert2D(0, 0),
                    p0.Convert2D(0, 0.5 * theArch.WidthInside - 0.5 * theArch.MainTubeDiameter), theArch.CrossBracingDiameter, 0, "H细线", null, null);
                obj.Add(curBar);

                if (i != 0)
                {
                    if (i % 2 == 0)
                    {
                        var dd = MLPloter.AddTube(new Point2d(theArch.Get3PointReal(carry[i-1])[0].X, 0), new Point2d(theArch.Get3PointReal(carry[i])[0].X, 0.5 * theArch.WidthInside),
                            theArch.GetTubeProperty(item.Center.X, eMemberType.WebBracing).Section.Diameter, 0, "H细线", (Curve)beforeBar[0], (Curve)archRib[1][2]);
                        obj.Add(dd);
                    }
                    else
                    {
                        var dd = MLPloter.AddTube(new Point2d(theArch.Get3PointReal(carry[i - 1])[0].X, 0.5 * theArch.WidthInside), new Point2d(theArch.Get3PointReal(carry[i])[0].X, 0),
                            theArch.GetTubeProperty(item.Center.X, eMemberType.WebBracing).Section.Diameter, 0, "H细线", (Curve)archRib[1][2], (Curve)curBar[2]);
                        obj.Add(dd);
                    }

                }                
                //上平面
                obj.Add(MLPloter.AddTube(p0.Convert2D(0, +0.5 * theArch.WidthInside + theArch.WidthOutside - 0.5 * theArch.MainTubeDiameter),
                    p0.Convert2D(0, +0.5 * theArch.WidthInside + 0.5 * theArch.MainTubeDiameter), theArch.CrossBracingDiameter, 0, "细线", null, null));
                beforeBar.Clear();
                foreach (DBObject kk in curBar)
                {
                    beforeBar.Add(kk);
                }

            }
            #endregion


            #region 安装阶段


            var vd = from dt in theArch.MainDatum
                     where dt.DatumType == eDatumType.ColumnDatum || dt.DatumType == eDatumType.VerticalDatum
                     select dt;
            var datumList = vd.ToList();
            datumList.Sort(new DatumPlane());


            List<Point3d> ptForDim = new List<Point3d>();
            for (int i = 0; i < datumList.Count; i++)
            {
                var theDat = datumList[i];
                if (i % 2 != 0 || theDat.Center.X > 0)
                {
                    continue;
                }
                DatumPlane install = theArch.SecondaryDatum.Find(x => x.Center.X > theDat.Center.X);

                var pts = new List<Point2d>() {
                    new Point2d(install.Center.X, 0).C2D(0, 0.5 * theArch.Width + 2),
                    new Point2d(install.Center.X, 0),
                    new Point2d(install.Center.X, 0).C2D(0, -0.5 * theArch.Width - 2) };

                var PL = (Polyline)PLPloter.AddPolylineByPointList(pts, "H虚线", false)[0];
                obj.Add(PL);

                ptForDim.Add(pts[1].Convert3D());
            }

            ptForDim.Add(new Point3d( theArch.LeftFoot[1].X,0,0));
            ptForDim.Sort((x, y) => x.X.CompareTo(y.X));

            for (int i = 0; i < ptForDim.Count - 1; i++)
            {
                string rep = string.Format("<>\\X节段{0}", i + 1);
                var pt1 = ptForDim[i];
                var pt2 = ptForDim[i + 1];
                obj.Add( DimPloter.DimRot(pt1, pt2, pt2.C3D(0,- 0.5*theArch.Width-10),0, 1, rep, Unit.Meter, Unit.Centimeter));
            }

            #endregion


            #region 立柱

            foreach (Model.Column item in theArch.ColumnList)
            {
                Extents2d exi;
                if (item.X<0)
                {
                    item.DrawColumnPlan(cc, out exi, true);
                }            
            }

            foreach (var item in theArch.RCColumnList)
            {
                Extents2d exi;
                if (item.X<0)
                {
                    item.DrawPlan(cc, out exi);

                }

            }

            #endregion

            #region 总体标注
            ptForDim = new List<Point3d>();

            foreach (var item in theArch.ColumnList)
            {
                if (item.X<0)
                {
                    ptForDim.Add(new Point3d(item.X, theArch.Width * 0.5, 0));
                }
            }
            foreach (var item in theArch.RCColumnList)
            {
                if (item.X < 0)
                {
                    ptForDim.Add(new Point3d(item.X, theArch.Width * 0.5, 0));
                }
            }
            ptForDim.Add(new Point3d(0, theArch.Width * 0.5, 0));
            ptForDim.Sort((x, y) => x.X.CompareTo(y.X)) ;
            for (int i = 0; i < ptForDim.Count - 1; i++)
            {
                string rep = "";
                var pt1 = ptForDim[i];
                var pt2 = ptForDim[i + 1];
                obj.Add(DimPloter.DimRot(pt1, pt2, pt2.C3D(0, 0.5 * theArch.Width + 3), 0, 1, rep, Unit.Meter, Unit.Centimeter));
            }

            obj.Add(TextPloter.AddTitle(Point2d.Origin.C2D(-0.25 * L, 0.5 * theArch.Width + 20), "1/4拱圈顶平面", ""));
            obj.Add(TextPloter.AddTitle(Point2d.Origin.C2D(-0.25 * L, -0.5 * theArch.Width - 24), "1/4拱圈底平面", ""));



            #endregion


            #region 输出

            foreach (var item in obj)
            {
                Entity et = (Entity)item;
                et.TransformBy(Matrix3d.Displacement(cc.GetAsVector().C3D()));
            }

            var idsnew = Ploter.WriteDatabase(obj);

            foreach (ObjectId item in idsnew)
            {
                ids.Add(item);
            }
            ext = Ploter.GetExtends(ids);

            #endregion


        }

    }
}
