using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADInterface.Plotters;
using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CADInterface.API
{
    public static class ColumnAPI
    {

        public static void DrawColumnElev(this Model.Column model, Database db, Point2d cc, ref Extents2d ext, double scale = 0)
        {
            DBObjectCollection entyList = new DBObjectCollection();
            Matrix3d mat = Matrix3d.Displacement(cc.Convert3D().GetAsVector());

            Point2d pt0 = new Point2d(model.X, model.MainArch.Get7PointReal(model.X, 90)[0].Y);
            Point2d pt1 = pt0.Convert2D(0, model.Z1 - model.Z0);
            Point2d pt2 = pt0.Convert2D(0, model.Z2 - model.Z0);

            // 拱肋
            var ld = model.MainArch.Get7PointReal(model.X, 90)[0].Y - model.MainArch.Get7PointReal(model.X, 90)[2].Y;

            foreach (var x in new double[]{ -0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside, -0.5 * model.MainArch.WidthInside, 
                0.5 * model.MainArch.WidthInside, 0.5 * model.MainArch.WidthInside+model.MainArch.WidthOutside })
            {
                Ellipse ee = new Ellipse(pt0.Convert3D(x, -0.5 * ld), Vector3d.ZAxis, Vector3d.YAxis * ld*0.5,    model.MainArch.MainTubeDiameter / ld, 0, 2 * Math.PI);
                ee.Layer = "细线";
                entyList.Add(ee);

                var Tube = MulitlinePloter.CreatTube(pt0.Convert2D(x,model.Z1-model.Z0),pt0.Convert2D(x, model.Z2-model.Z0), model.MainDiameter, null, null, 0, "细线", false);
                foreach (var item in Tube)
                {
                    entyList.Add(item);
                }

                var Foot = MulitlinePloter.CreatTube(pt0.Convert2D(x, model.Z1 - model.Z0), pt0.Convert2D(x,-0.5*model.MainArch.MainTubeDiameter), model.FootW, null, ee, 0, "粗线", false);
                foreach (var item in Foot)
                {
                    entyList.Add(item);
                }

                entyList.Add(new Line(pt0.Convert3D(x-0.5*model.FootW, model.Z1 - model.Z0), pt0.Convert3D(x+0.5*model.FootW, model.Z1 - model.Z0)) { Layer = "粗线" });
               
                var STpart = MulitlinePloter.CreatTube(pt0.Convert2D(x, model.Z1 - model.Z0), pt0.Convert2D(x, -0.5 * model.MainArch.MainTubeDiameter), model.MainDiameter, null, ee, 0, "细线", false);
                foreach (var item in STpart)
                {
                    entyList.Add(item);
                }

                entyList.Add(new Line(pt0.Convert3D(x, -ld), pt0.Convert3D(x, -ld + model.Z2 - model.Z0 + ld)) { Layer = "中心线" });

                if (scale!=0)
                {
                    Point3d pta = new Point3d(model.X-0.5*model.MainArch.WidthInside-model.MainArch.WidthOutside, model.Z2 - model.CapHeight +model.InstallOffset, 0);
                    Point3d ptb = pta.Convert3D(0, -model.M * model.BeamStep);
                    Point3d ptc = ptb.Convert3D(0, -model.InstallOffset);
                    Point3d ptd = ptc.Convert3D(0, -model.C);
                    Point3d pte = ptd.Convert3D(0, -model.BeamStep*model.K);
                    Point3d ptf = new Point3d(model.X - 0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside,model.Z1,0);

                    string rpstring = string.Format("M×{0:F0}", model.BeamStep  * 100);
                    entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pta, ptb, pta.Convert3D(-10 * scale), 90, scale, rpstring));
                    entyList.Add(DimPloter.CreatDimRotated(db, ref ext, ptb, ptc, pta.Convert3D(-10 * scale), 90, scale));
                    entyList.Add(DimPloter.CreatDimRotated(db, ref ext, ptc, ptd, pta.Convert3D(-10 * scale), 90, scale,"C"));
                    entyList.Add(DimPloter.CreatDimRotated(db, ref ext, ptd, pte, pta.Convert3D(-10 * scale), 90, scale,string.Format("K×{0:F0}",model.BeamStep*100)));
                    entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pte, ptf, pta.Convert3D(-10 * scale), 90, scale,"B"));
                    entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pta.Convert3D(0, model.CapHeight - model.InstallOffset),
                        pta, pta.Convert3D(-10 * scale), 90, scale));
                    entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pta.Convert3D(0,model.CapHeight-model.InstallOffset),
                        ptf, pta.Convert3D(-15 * scale), 90, scale,"H"));
                }
            }

            // 横撑

            for (int kk = 0; kk < 2; kk++)
            {
                double x1 = kk == 0 ? model.X - 0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside + 0.5 * model.MainDiameter :
                     model.X + 0.5 * model.MainArch.WidthInside + 0.5 * model.MainDiameter;
                double x2 = kk == 0 ? model.X - 0.5 * model.MainArch.WidthInside - 0.5 * model.MainDiameter :
                             model.X + 0.5 * model.MainArch.WidthInside + model.MainArch.WidthOutside - 0.5 * model.MainDiameter;
                for (int i = 0; i < model.N + 1; i++)
                {
                    var st = new Point2d(x1, model.Z1);
                    var ed = new Point2d(x2, model.Z1);
                    double yy = model.A + model.InstallSteps * model.BeamStep * i;
                    entyList.Add(new Line(st.Convert3D(-model.MainDiameter, yy), st.Convert3D(0, yy)) { Layer = "虚线" });
                    entyList.Add(new Line(ed.Convert3D(0, yy), ed.Convert3D(+model.MainDiameter, yy)) { Layer = "虚线" });
                }

                for (int i = 0; i < model.K+ model.M+1; i++)
                {
                    var st = new Point2d(x1, model.Z1 + model.B + i * model.BeamStep);
                    var ed = new Point2d(x2, model.Z1 + model.B + i * model.BeamStep);
                    double dia = model.CrossLDiameter;
                    var STube = MulitlinePloter.CreatTube(st, ed, dia, null, null, 0, "细线", false);
                    foreach (var item in STube)
                    {
                        entyList.Add(item);
                    }
                    entyList.Add(new Circle(st.Convert3D(-model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "虚线" });
                    entyList.Add(new Circle(ed.Convert3D(model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "虚线" });
                    entyList.Add(new Line(st.Convert3D(-model.MainDiameter, 0), ed.Convert3D(model.MainDiameter, 0)) { Layer = "中心线" });
                }
            }

            // 冒梁
            var p1 = new Point3d(model.X - 0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside - model.MainDiameter * 0.5, model.Z2, 0);
            var p2 = p1.Convert3D(model.MainArch.WidthInside + 2 * model.MainArch.WidthOutside + model.MainDiameter);
            entyList.Add(new Line(p1,p2) { Layer = "细线" });
            var p2a = p1.Convert3D(model.MainDiameter,-model.CapHeight+model.InstallOffset);
            var p2b = p2a.Convert3D(model.MainArch.WidthOutside - model.MainDiameter);
            var p2c = p2b.Convert3D(model.MainDiameter);
            var p2d = p2c.Convert3D(model.MainArch.WidthInside - model.MainDiameter);
            var p2e = p2d.Convert3D(model.MainDiameter);
            var p2f = p2e.Convert3D(model.MainArch.WidthOutside - model.MainDiameter);


            entyList.Add(new Line(p2a, p2b) { Layer = "细线" }) ;
            entyList.Add(new Line(p2c, p2d) { Layer = "细线" }) ;
            entyList.Add(new Line(p2e, p2f) { Layer = "细线" }) ;

            if (scale!=0)
            {
                var pa = p1.Convert3D(0.5 * model.MainDiameter);
                var pb = pa.Convert3D(model.MainArch.WidthOutside);
                var pc = pb.Convert3D(model.MainArch.WidthInside);
                var pd = pc.Convert3D(model.MainArch.WidthOutside);

                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pa, pb, p1.Convert3D(0, 2 * scale), 0, scale));
                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pb, pc, p1.Convert3D(0, 2 * scale), 0, scale));
                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pc, pd, p1.Convert3D(0, 2 * scale), 0, scale));
                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pa, pd, p1.Convert3D(0, 7 * scale), 0, scale));

            }

            foreach (var item in entyList)
            {
                var ent = (Entity)item;
                ent.TransformBy(mat);
                //ext=ext.Add(ent.GeometricExtents.Convert2D());
                db.AddEntityToModeSpace((Entity)item);
            }
        }

        /// <summary>
        /// 绘制立柱
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        public static void DarwColumnSide(this Model.Column model, Database db,Point2d cc, ref Extents2d ext,double scale=0)
        {
            DBObjectCollection entyList = new DBObjectCollection();

            Matrix3d mat = Matrix3d.Displacement(cc.Convert3D().GetAsVector());
            // 中心
            Point2d pt0 = new Point2d(model.X, model.MainArch.Get3PointReal(model.X,90)[0].Y);
            Point2d pt0a = model.MainArch.Get7PointReal(model.X - 0.5 * model.L,90.0)[0].ToAcadPoint2d();
            Point2d pt0b = model.MainArch.Get7PointReal(model.X + 0.5 * model.L,90.0)[0].ToAcadPoint2d();

            var Eda = new Point3d(pt0a.X, model.Z2, 0);
            var Edb = new Point3d(pt0b.X, model.Z2, 0);

            Line cl = new Line(pt0.Convert3D(), new Point3d(model.X,model.Z2,0)) { Layer="中心线"} ;
            entyList.Add(cl);
            cl = new Line(pt0a.Convert3D(), Eda) { Layer = "中心线" };
            entyList.Add(cl);
            cl = new Line(pt0b.Convert3D(),Edb ) { Layer = "中心线" };
            entyList.Add(cl);

            if (scale!=0)
            {
                entyList.Add( DimPloter.CreatDimRotated(db, ref ext, Eda, Edb, Eda.Convert3D(0, 2 * scale),0,scale));
            }
            // 柱脚
            double xa = model.X + model.FootL * -0.5;
            double xb = model.X + model.FootL * +0.5;
            Point2d PA0 = new Point2d(xa, model.MainArch.Get7PointReal(xa, 90.0)[0].Y);
            Point2d PA1 = new Point2d(xa, model.Z1);
            Point2d PA2 = new Point2d(model.X, model.Z1);
            Point2d PA3 = new Point2d(xb, model.Z1);
            Point2d PA4 = new Point2d(xb, model.MainArch.Get7PointReal(xb, 90.0)[0].Y);
            Point2d PA5 = new Point2d(model.X, model.Z0);
            var FootList = new List<Point2d>() { PA0, PA1, PA2, PA3, PA4, PA5 };
            Polyline FT = AcadExt.CreatFromList(FootList);
            FT.Closed = true;
            FT.Layer = "粗线";
            entyList.Add(FT);
            if (scale!=0)
            {
                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, PA3.Convert3D(), PA4.Convert3D(), pt0.Convert3D(12 * scale), 90, scale)) ;
                entyList.Add(DimPloter.CreatMLeader(db, ref ext, PA1.Convert2D(), PA1.Convert2D(-2, 1.5),"柱脚"));

            }

            // 冒梁

            entyList.Add(new Line(
                new Point3d(model.X - 0.5 * model.L - 0.5 * model.MainDiameter, model.Z2, 0),
                new Point3d(model.X + 0.5 * model.L + 0.5 * model.MainDiameter, model.Z2, 0))
            { Layer = "细线" });
            entyList.Add(new Line(
                new Point3d(model.X - 0.5 * model.L + 0.5 * model.MainDiameter, model.Z2 - model.CapHeight + model.InstallOffset, 0),
                new Point3d(model.X + 0.5 * model.L - 0.5 * model.MainDiameter, model.Z2 - model.CapHeight + model.InstallOffset, 0))
            { Layer = "细线" });
            if (scale != 0)
            {
                Point3d pt = new Point3d(model.X, model.Z2, 0);

                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pt, pt.Convert3D(0,-model.CapHeight), pt0.Convert3D(12*scale), 90, scale,"<>\\X盖梁节段"));

            }
            // 主杆
            var bd = from Point2D pt in model.MainArch.UpUpSkeleton select pt.ToAcadPoint2d();
            Polyline dbr = AcadExt.CreatFromList(bd.ToList());
            for (int i = 0; i < 2; i++)
            {
                var x0 = i == 0 ? model.L * -0.5 + model.X : model.L * 0.5 + model.X;
                var Tube = MulitlinePloter.CreatTube(new Point2d(x0, model.Z1),
                    new Point2d(x0, model.Z2), model.MainDiameter,null,null,0,"细线",false);
                foreach (var item in Tube)
                {
                    entyList.Add(item);
                }
                var Tube2 = MulitlinePloter.CreatTube(new Point2d(x0, model.Z0),
                    new Point2d(x0, model.Z1), model.MainDiameter, dbr, new Line(PA1.Convert3D(), PA3.Convert3D()),0,"虚线", false) ;
                foreach (var item in Tube2)
                {
                    entyList.Add(item);
                }
            }
            if (scale != 0)
            {
                Point3d pta = new Point3d(model.X, model.Z2-model.CapHeight, 0);
                Point3d ptb = new Point3d(model.X, model.Z2-model.CapHeight-model.BeamStep*model.InstallSteps*model.N, 0);
                string rpstring = string.Format("N×{0:F0}（标准节段）", model.BeamStep * model.InstallSteps * 100);
                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pta, ptb, pt0.Convert3D(12 * scale), 90, scale, rpstring));
            }


            double x1 = model.X - 0.5 * model.L + model.MainDiameter * 0.5;
            double x2 = model.X + 0.5 * model.L - model.MainDiameter * 0.5;
            for (int i = 0; i < model.M + model.K+1; i++)
            {
                var st = new Point2d(x1, model.Z1 + model.B + i * model.BeamStep);
                var ed = new Point2d(x2, model.Z1 + model.B + i * model.BeamStep);
                double dia = model.CrossLDiameter;
                var STube = MulitlinePloter.CreatTube(st, ed, dia, null, null, 0, "细线", false);
                foreach (var item in STube)
                {
                    entyList.Add(item);
                }
                entyList.Add(new Circle(st.Convert3D(-model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "虚线" });
                entyList.Add(new Circle(ed.Convert3D(model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "虚线" });
                entyList.Add(new Line(st.Convert3D(-model.MainDiameter, 0), ed.Convert3D(model.MainDiameter, 0)) { Layer = "中心线" });
            }



            if (scale != 0)
            {
                Point3d pta = new Point3d(model.X, model.Z1, 0);
                Point3d ptb = new Point3d(model.X, model.Z2 - model.CapHeight - model.BeamStep * model.InstallSteps * model.N, 0);
                Point3d ptc = new Point3d(model.X, model.Z2, 0);
                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pta, ptb, pt0.Convert3D(12 * scale), 90, scale, "A\\X起步节段"));
                entyList.Add(DimPloter.CreatDimRotated(db, ref ext, pta, ptc, pt0.Convert3D(17 * scale), 90, scale, "H"));
                entyList.Add(DimPloter.CreatMLeader(db, ref ext, ptc.Convert2D(-0.5*model.L,-18.5),  "立柱竖杆",true));
                entyList.Add(DimPloter.CreatMLeader(db, ref ext, ptc.Convert2D(0,-model.CapHeight-model.BeamStep*model.InstallSteps+model.InstallOffset),
                    "横撑",true));
                entyList.Add(DimPloter.CreatMLeader(db, ref ext, pta.Convert2D(-0.5*model.L,model.A), "安装接头",true));
            }
            // 横杆

            for (int i = 0; i < model.N+1; i++)
            {
                var st = new Point2d(x1, model.Z1);
                var ed = new Point2d(x2, model.Z1);
                double yy = model.A + model.InstallSteps * model.BeamStep * i;
                entyList.Add(new Line(st.Convert3D(-model.MainDiameter,yy), st.Convert3D(0,yy)) { Layer="虚线"});
                entyList.Add(new Line(ed.Convert3D(0,yy),ed.Convert3D(+model.MainDiameter,yy)) { Layer="虚线"});
                
            }









            foreach (var item in entyList)
            {
                var ent = (Entity)item;
                ent.TransformBy(mat);
                //ext=ext.Add(ent.GeometricExtents.Convert2D());
                db.AddEntityToModeSpace((Entity)item);
            }

            

        }


    }
}
