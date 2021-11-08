using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADInterface.Plotters;
using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using HPDI.DrawingStandard;

namespace CADInterface.API
{

  
    public static class ColumnAPI
    {
        public static void DrawRCColumnSide(this RCColumn col,Vector2d moveVector,out Extents2d ext,double scale=0)
        {
            #region 总体
            DBObjectCollection obj = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();

            Point3d ft = new Point3d(col.X,col.H0, 0);
            Point3d top = new Point3d(col.X, col.H1, 0);
            Point3d topCB = new Point3d(col.X, col.H2, 0);           

            #endregion

            #region 墩身
            double w1 = col.Section1.Width;
            double w0 = col.Section1.Width;
            List<Point2d> piers = new List<Point2d>();
            piers.Add(ft.C2D());
            piers.Add(ft.C2D(-0.5*w0));
            piers.Add(top.C2D(-0.5*w1));
            piers.Add(top.C2D());
            piers.Add(top.C2D(0.5 * w1)) ;
            piers.Add(ft.C2D(0.5 * w0)) ;
            Polyline pl=(Polyline) PLPloter.AddPolylineByPointList(piers, "H粗线", true)[0];
            obj.Add(pl);
            obj.Add(new Line(ft, topCB) { Layer = "H中心线" });

            var cb=MLPloter.AddTube(top.C2D(), topCB.C2D(), w1, 0, "H粗线", null, null, false);
            Extensions.Add(obj, cb);

            obj.Add(new Line(topCB.C3D(-0.5 * w1), topCB.C3D(0.5 * w1)) { Layer = "H粗线" });

            #endregion



            #region 标注
            #endregion



            #region 输出
            foreach (DBObject item in obj)
            {
                Entity ent = (Entity)item;
                ent.TransformBy(Matrix3d.Displacement(moveVector.C3D()));
            }
            ids=Ploter.WriteDatabase(obj);
            ext = Ploter.GetExtends(ids);
            #endregion




        }

        public static void DrawColumnElev(this Model.Column model, Database db, Point2d cc, out Extents2d ext, double scale = 0)
        {
            DBObjectCollection entyList = new DBObjectCollection();
            ObjectIdCollection idList = new ObjectIdCollection();
            Matrix3d mat = Matrix3d.Displacement(cc.C3D().GetAsVector());

            Point2d pt0 = new Point2d(model.X, model.MainArch.Get7PointReal(model.X, 90)[0].Y);
            Point2d pt1 = pt0.C2D(0, model.Z1 - model.Z0);
            Point2d pt2 = pt0.C2D(0, model.Z2 - model.Z0);

            #region 主管
            var ld = model.MainArch.Get7PointReal(model.X, 90)[0].Y - model.MainArch.Get7PointReal(model.X, 90)[2].Y;

            foreach (var x in new double[]{ -0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside, -0.5 * model.MainArch.WidthInside, 
                0.5 * model.MainArch.WidthInside, 0.5 * model.MainArch.WidthInside+model.MainArch.WidthOutside })
            {
                Ellipse ee = new Ellipse(pt0.C3D(x, -0.5 * ld), Vector3d.ZAxis, Vector3d.YAxis * ld*0.5,    model.MainArch.MainTubeDiameter / ld, 0, 2 * Math.PI);
                ee.Layer = "H粗线";
                entyList.Add(ee);

                var MainTube = MLPloter.AddTube(pt0.C2D(x, model.Z1 - model.Z0), pt0.C2D(x, model.Z2 - model.Z0), model.MainDiameter, 0, "H粗线", null, null, false);
                //var Tube = MulitlinePloterO.CreatTube(pt0.C2D(x,model.Z1-model.Z0),pt0.C2D(x, model.Z2-model.Z0), model.MainDiameter, null, null, 0, "粗线", false);
                foreach (var item in MainTube)
                {
                    entyList.Add((Entity)item);
                }

                var Foot = MLPloter.AddTube(pt0.C2D(x, model.Z1 - model.Z0), pt0.C2D(x,-0.5*model.MainArch.MainTubeDiameter), model.FootW,0,  "H粗线",null, ee,  false);
                foreach (var item in Foot)
                {
                    entyList.Add((Entity)item);
                }

                entyList.Add(new Line(pt0.C3D(x-0.5*model.FootW, model.Z1 - model.Z0), pt0.C3D(x+0.5*model.FootW, model.Z1 - model.Z0)) { Layer = "H粗线" });
               
                var STpart = MLPloter.AddTube(pt0.C2D(x, model.Z1 - model.Z0), pt0.C2D(x, -0.5 * model.MainArch.MainTubeDiameter), model.MainDiameter,0, "H虚线", null, ee,  false);
                foreach (var item in STpart)
                {
                    entyList.Add((Entity)item);
                }

                entyList.Add(new Line(pt0.C3D(x, -ld), pt0.C3D(x, -ld + model.Z2 - model.Z0 + ld)) { Layer = "H中心线" });
               
            }
            if (scale != 0)
            {
                Point3d pta = new Point3d(model.X - 0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside, model.Z2 - model.CapHeight + model.InstallOffset, 0);
                Point3d ptb = pta.C3D(0, -model.M * model.BeamStep);
                Point3d ptc = ptb.C3D(0, -model.InstallOffset);
                Point3d ptd = ptc.C3D(0, -model.C);
                Point3d pte = ptd.C3D(0, -model.BeamStep * model.K);
                Point3d ptf = new Point3d(model.X - 0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside, model.Z1, 0);

                string rpstring = string.Format("M×{0:F0}", model.BeamStep * 100);
                entyList.Add(DimPloter.DimRot(pta, ptb, pta.C3D(-10 * scale), 90, 0.25, rpstring, Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(ptb, ptc, pta.C3D(-10 * scale), 90, 0.25, "", Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(ptc, ptd, pta.C3D(-10 * scale), 90, 0.25, "C", Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(ptd, pte, pta.C3D(-10 * scale), 90, 0.25, string.Format("K×{0:F0}", model.BeamStep * 100), Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(pte, ptf, pta.C3D(-10 * scale), 90, 0.25, "B", Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(pta.C3D(0, model.CapHeight - model.InstallOffset), pta, pta.C3D(-10 * scale), 90, 0.25, "", Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(pta.C3D(0, model.CapHeight - model.InstallOffset), ptf, pta.C3D(-15 * scale), 90, 0.25, "L", Unit.Meter, Unit.Centimeter));

                var pa1= pt0.C3D(-0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside);


                idList.Add(  DimPloter.AddPMBJ("A", pa1.C3D(0,-4)+cc.GetAsVector().C3D(),0.25,3,false));
                idList.Add(  DimPloter.AddPMBJ("A", pa1.C3D(0,50) + cc.GetAsVector().C3D(), 0.25,3,true));

            }

            #endregion

            #region 横撑
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
                    entyList.Add(new Line(st.C3D(-model.MainDiameter, yy), st.C3D(0, yy)) { Layer = "H虚线" });
                    entyList.Add(new Line(ed.C3D(0, yy), ed.C3D(+model.MainDiameter, yy)) { Layer = "H虚线" });
                }

                for (int i = 0; i < model.K + model.M + 1; i++)
                {
                    var st = new Point2d(x1, model.Z1 + model.B + i * model.BeamStep);
                    var ed = new Point2d(x2, model.Z1 + model.B + i * model.BeamStep);
                    double dia = model.CrossLDiameter;
                    var STube = MLPloter.AddTube(st, ed, dia,0,"H细线", null, null, false);
                    foreach (var item in STube)
                    {
                        entyList.Add((Entity)item);
                    }
                    entyList.Add(new Circle(st.C3D(-model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "H虚线" });
                    entyList.Add(new Circle(ed.C3D(model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "H虚线" });
                    entyList.Add(new Line(st.C3D(-model.MainDiameter, 0), ed.C3D(model.MainDiameter, 0)) { Layer = "H中心线" });
                }
            }

            #endregion

            #region  冒梁
            var p1 = new Point3d(model.X - 0.5 * model.MainArch.WidthInside - model.MainArch.WidthOutside - model.MainDiameter * 0.5, model.Z2, 0);
            var p2 = p1.C3D(model.MainArch.WidthInside + 2 * model.MainArch.WidthOutside + model.MainDiameter);
            entyList.Add(new Line(p1, p2) { Layer = "H粗线" });
            var p2a = p1.C3D(model.MainDiameter, -model.CapHeight + model.InstallOffset);
            var p2b = p2a.C3D(model.MainArch.WidthOutside - model.MainDiameter);
            var p2c = p2b.C3D(model.MainDiameter);
            var p2d = p2c.C3D(model.MainArch.WidthInside - model.MainDiameter);
            var p2e = p2d.C3D(model.MainDiameter);
            var p2f = p2e.C3D(model.MainArch.WidthOutside - model.MainDiameter);

            entyList.Add(new Line(p2a, p2b) { Layer = "H粗线" });
            entyList.Add(new Line(p2c, p2d) { Layer = "H粗线" });
            entyList.Add(new Line(p2e, p2f) { Layer = "H粗线" });

            if (scale != 0)
            {
                var pa = p1.C3D(0.5 * model.MainDiameter);
                var pb = pa.C3D(model.MainArch.WidthOutside);
                var pc = pb.C3D(model.MainArch.WidthInside);
                var pd = pc.C3D(model.MainArch.WidthOutside);

                entyList.Add(DimPloter.DimRot(pa, pb, p1.C3D(0, 2 * scale), 0, 0.25, "", Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(pb, pc, p1.C3D(0, 2 * scale), 0, 0.25, "", Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(pc, pd, p1.C3D(0, 2 * scale), 0, 0.25, "", Unit.Meter, Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(pa, pd, p1.C3D(0, 7 * scale), 0, 0.25, "", Unit.Meter, Unit.Centimeter));

            }

            #endregion

            //ext = new Extents2d(pt0+cc.GetAsVector(), pt0+cc.GetAsVector());
            foreach (var item in entyList)
            {
                var ent = (Entity)item;
                ent.TransformBy(mat);
            }

            var outID= Ploter.WriteDatabase(entyList);

            foreach (ObjectId item in idList)
            {
                outID.Add(item);
            }
            ext = Ploter.GetExtends(outID);

        }


        public static void DrawSingleSection(this Model.Column theCol,Database db,Point2d cc,out Extents2d ext,double scale = 0)
        {
            DBObjectCollection entyList = new DBObjectCollection();
            Matrix3d mat = Matrix3d.Displacement(cc.C3D().GetAsVector());
            ext = new Extents2d(cc,cc);
            Point2d p1 = Point2d.Origin.C2D(-0.5 * theCol.MainArch.WidthOutside, -0.5 * theCol.L);
            Point2d p2 = p1.C2D(theCol.MainArch.WidthOutside);
            Point2d p3 = p2.C2D(0, theCol.L);
            Point2d p4 = p3.C2D(-theCol.MainArch.WidthOutside);

            var list = new Point2d[] { p1, p2, p3, p4 };
            for (int i = 0; i < 4; i++)
            {
                var item = list[i];            

                Circle Cir = new Circle(item.C3D(), Vector3d.ZAxis, theCol.MainDiameter * 0.5);
                Cir.Layer = "H粗线";

                Line la = new Line(item.C3D(-0.6 * theCol.MainDiameter),
                    item.C3D(0.6 * theCol.MainDiameter))
                { Layer = "H中心线" };
                Line lb = new Line(item.C3D(0, -0.6 * theCol.MainDiameter),
                    item.C3D(0, 0.6 * theCol.MainDiameter))
                { Layer = "H中心线" };
                entyList.Add(Cir);
                entyList.Add(la);
                entyList.Add(lb);

                var st = list[i];
                var ed = i == 3 ? list[0] : list[i + 1];

                var outEnts = MLPloter.AddTube(st, ed, theCol.CrossLDiameter,0,"H细线",
                    new Circle(st.C3D(), Vector3d.ZAxis, theCol.MainDiameter * 0.5),
                    new Circle(ed.C3D(), Vector3d.ZAxis, theCol.MainDiameter * 0.5),false
                    );
                foreach (var kk in outEnts)
                {
                    entyList.Add((Entity)kk);
                }

            }

            if (scale!=0)
            {

                entyList.Add(DimPloter.DimRot(p1.C3D(), p2.C3D(), p1.C3D(0, -10 * scale),0,0.1,"",Unit.Meter,Unit.Centimeter));
                entyList.Add(DimPloter.DimRot(p2.C3D(), p3.C3D(), p2.C3D(10 * scale),90,0.1,"", Unit.Meter, Unit.Centimeter));

            }


            //ext = ext.Add(D1.GeometricExtents.C2D());
            foreach (var item in entyList)
            {
                var ent = (Entity)item;
                ent.TransformBy(mat);
                //db.AddEntityToModeSpace((Entity)item);
                //ext= ext.Add(ent.GeometricExtents.C2D());
            }

            var ids=Ploter.WriteDatabase(entyList);
            ext = Ploter.GetExtends(ids);

        }


        /// <summary>
        /// 绘制立柱
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        public static void DarwColumnSide(this Model.Column model, Point2d dispV, out Extents2d ext,double scale=0)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            DBObjectCollection entyList = new DBObjectCollection();

            Matrix3d mat = Matrix3d.Displacement(dispV.C3D().GetAsVector());
            // 中心
            Point2d pt0 = new Point2d(model.X, model.MainArch.Get3PointReal(model.X,90)[0].Y);
            Point2d pt0a = model.MainArch.Get7PointReal(model.X - 0.5 * model.L,90.0)[0].ToAcadPoint2d();
            Point2d pt0b = model.MainArch.Get7PointReal(model.X + 0.5 * model.L,90.0)[0].ToAcadPoint2d();

            var Eda = new Point3d(pt0a.X, model.Z2, 0);
            var Edb = new Point3d(pt0b.X, model.Z2, 0);

            Line cl = new Line(pt0.C3D(), new Point3d(model.X,model.Z2,0)) { Layer="中心线"} ;
            entyList.Add(cl);
            cl = new Line(pt0a.C3D(), Eda) { Layer = "中心线" };
            entyList.Add(cl);
            cl = new Line(pt0b.C3D(),Edb ) { Layer = "中心线" };
            entyList.Add(cl);

            if (scale!=0)
            {
                entyList.Add( DimPloterO.CreatDimRotated(db,  Eda, Edb, Eda.C3D(0, 2 * scale), "M-CM-4-1", 0));
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
                entyList.Add(DimPloterO.CreatDimRotated(db, PA3.C3D(), PA4.C3D(), pt0.C3D(12 * scale), "M-CM-4-1", 90)) ;
                entyList.Add(DimPloterO.CreatMLeader(db,  PA1.C2D(), PA1.C2D(-2, 1.5),"柱脚"));

            }

            // 冒梁

            entyList.Add(new Line(
                new Point3d(model.X - 0.5 * model.L - 0.5 * model.MainDiameter, model.Z2, 0),
                new Point3d(model.X + 0.5 * model.L + 0.5 * model.MainDiameter, model.Z2, 0))
            { Layer = "粗线" });
            entyList.Add(new Line(
                new Point3d(model.X - 0.5 * model.L + 0.5 * model.MainDiameter, model.Z2 - model.CapHeight + model.InstallOffset, 0),
                new Point3d(model.X + 0.5 * model.L - 0.5 * model.MainDiameter, model.Z2 - model.CapHeight + model.InstallOffset, 0))
            { Layer = "粗线" });
            if (scale != 0)
            {
                Point3d pt = new Point3d(model.X, model.Z2, 0);

                entyList.Add(DimPloterO.CreatDimRotated(db, pt, pt.C3D(0,-model.CapHeight), pt0.C3D(12*scale), "M-CM-4-1", 90, "<>\\X盖梁节段"));

            }
            // 主杆
            var bd = from Point2D pt in model.MainArch.UpUpSkeleton select pt.ToAcadPoint2d();
            Polyline dbr = AcadExt.CreatFromList(bd.ToList());
            for (int i = 0; i < 2; i++)
            {
                var x0 = i == 0 ? model.L * -0.5 + model.X : model.L * 0.5 + model.X;
                var Tube = MulitlinePloterO.CreatTube(new Point2d(x0, model.Z1),
                    new Point2d(x0, model.Z2), model.MainDiameter,null,null,0,"粗线",false);
                foreach (var item in Tube)
                {
                    entyList.Add(item);
                }
                var Tube2 = MulitlinePloterO.CreatTube(new Point2d(x0, model.Z0),
                    new Point2d(x0, model.Z1), model.MainDiameter, dbr, new Line(PA1.C3D(), PA3.C3D()),0,"虚线", false) ;
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
                entyList.Add(DimPloterO.CreatDimRotated(db,  pta, ptb, pt0.C3D(12 * scale),"M-CM-4-1", 90, rpstring));
            }


            double x1 = model.X - 0.5 * model.L + model.MainDiameter * 0.5;
            double x2 = model.X + 0.5 * model.L - model.MainDiameter * 0.5;
            for (int i = 0; i < model.M + model.K+(int)model.C/2; i++)
            {
                var st = new Point2d(x1, model.Z1 + model.B + i * model.BeamStep);
                var ed = new Point2d(x2, model.Z1 + model.B + i * model.BeamStep);
                double dia = model.CrossLDiameter;
                var STube = MulitlinePloterO.CreatTube(st, ed, dia, null, null, 0, "细线", false);
                foreach (var item in STube)
                {
                    entyList.Add(item);
                }
                entyList.Add(new Circle(st.C3D(-model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "虚线" });
                entyList.Add(new Circle(ed.C3D(model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5 * model.CrossWDiameter) { Layer = "虚线" });
                entyList.Add(new Line(st.C3D(-model.MainDiameter, 0), ed.C3D(model.MainDiameter, 0)) { Layer = "中心线" });
            }



            if (scale != 0)
            {
                Point3d pta = new Point3d(model.X, model.Z1, 0);
                Point3d ptb = new Point3d(model.X, model.Z2 - model.CapHeight - model.BeamStep * model.InstallSteps * model.N, 0);
                Point3d ptc = new Point3d(model.X, model.Z2, 0);
                entyList.Add(DimPloterO.CreatDimRotated(db,  pta, ptb, pt0.C3D(12 * scale), "M-CM-4-1", 90, "A\\X起步节段"));
                entyList.Add(DimPloterO.CreatDimRotated(db,  pta, ptc, pt0.C3D(17 * scale), "M-CM-4-1", 90, "L"));
                entyList.Add(DimPloterO.CreatMLeader(db,  ptc.C2D(-0.5*model.L,-18.5),  "立柱竖杆",true));
                entyList.Add(DimPloterO.CreatMLeader(db, ptc.C2D(0,-model.CapHeight-model.BeamStep*model.InstallSteps+model.InstallOffset),
                    "横撑",true));
                entyList.Add(DimPloterO.CreatMLeader(db,  pta.C2D(-0.5*model.L,model.A), "安装接头",true));
            }
            // 横杆
            for (int i = 0; i < model.N+1; i++)
            {
                var st = new Point2d(x1, model.Z1);
                var ed = new Point2d(x2, model.Z1);
                double yy = model.A + model.InstallSteps * model.BeamStep * i;
                entyList.Add(new Line(st.C3D(-model.MainDiameter,yy), st.C3D(0,yy)) { Layer="虚线"});
                entyList.Add(new Line(ed.C3D(0,yy),ed.C3D(+model.MainDiameter,yy)) { Layer="虚线"});                
            }
            ext = new Extents2d(pt0 + dispV.GetAsVector(), pt0 + dispV.GetAsVector());
            foreach (var item in entyList)
            {
                var ent = (Entity)item;
                ent.TransformBy(mat);
                db.AddEntityToModeSpace((Entity)item);
                ext = ext.Add(ent.GeometricExtents.C2D());
            }         

        }



        public static void DrawPlan(this Model.RCColumn col, Point2d dispV, out Extents2d ext)
        {
            DBObjectCollection obj = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();
            Matrix3d mat = Matrix3d.Displacement(dispV.C3D().GetAsVector());
            double y0 = col.MainArch.WidthInside * 0.5 + col.MainArch.WidthOutside * 0.5;
            double x0 = col.X;
            double w = col.Section0.Width;
            double l = col.Section0.Length;
            obj.Add(PLPloter.AddPloy4(new Point2d(x0, y0).C2D(0, 0.5 * w), 0.5 * l, 0.5 * l, w, "H粗线"));
            obj.Add(PLPloter.AddPloy4(new Point2d(x0, -y0).C2D(0, 0.5 * w), 0.5 * l, 0.5 * l, w, "H粗线"));
            obj.Add(new Line(new Point3d(x0, -y0 - w, 0), new Point3d(x0, y0 + w, 0)) { Layer = "H中心线" });

            Ploter.ExertTransform(obj, mat);
            ids= Ploter.WriteDatabase(obj);
            ext= Ploter.GetExtends(ids);
        }

        public static void DrawColumnPlan(this Model.Column col,Point2d dispV, out Extents2d ext,bool onlyHalf=false)
        {
            DBObjectCollection obj = new DBObjectCollection();
            ObjectIdCollection ids = new ObjectIdCollection();
            Matrix3d mat = Matrix3d.Displacement(dispV.C3D().GetAsVector());

            double x0 = col.X;
            double w = col.FootW;
            double l = col.FootL;
            foreach (double y0 in new double[] {0.5*col.MainArch.WidthInside,0.5*col.MainArch.Width })
            {
                Point2d cc = new Point2d(x0, y0);
                obj.Add(PLPloter.AddPloy4(cc.C2D(0, 0.5 * w), 0.5 * l, 0.5 * l, w, "H细线"));
                obj.Add(new Circle(cc.C3D(-0.5 * col.L), Vector3d.ZAxis, 0.5 * col.MainDiameter));
                obj.Add(new Circle(cc.C3D(+0.5 * col.L), Vector3d.ZAxis, 0.5 * col.MainDiameter));
            }
            if (onlyHalf)
            {
                foreach (var item in obj)
                {
                    var ent = (Entity)item;
                    ent.TransformBy(mat);
                }
                ids = Ploter.WriteDatabase(obj);
                ext = Ploter.GetExtends(ids);
                return;
            }
            else
            {
                foreach (double y0 in new double[] { -0.5 * col.MainArch.WidthInside, -0.5 * col.MainArch.Width })
                {
                    Point2d cc = new Point2d(x0, y0);
                    obj.Add(PLPloter.AddPloy4(cc.C2D(0, 0.5 * w), 0.5 * l, 0.5 * l, w, "H细线"));
                    obj.Add(new Circle(cc.C3D(-0.5 * col.L), Vector3d.ZAxis, 0.5 * col.MainDiameter));
                    obj.Add(new Circle(cc.C3D(+0.5 * col.L), Vector3d.ZAxis, 0.5 * col.MainDiameter));
                }
                foreach (var item in obj)
                {
                    var ent = (Entity)item;
                    ent.TransformBy(mat);
                }
                ids = Ploter.WriteDatabase(obj);
                ext = Ploter.GetExtends(ids);
                return;
            }


        }
    }
}
