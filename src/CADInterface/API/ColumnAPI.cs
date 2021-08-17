using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CADInterface.Plotters;
using MathNet.Spatial.Euclidean;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace CADInterface.API
{
    public static class ColumnAPI
    {

        /// <summary>
        /// 绘制立柱
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <
        /// <param name="ext"></param>
        public static void DarwColumnSide(this Model.Column model, Database db,Point2d cc, ref Extents2d ext)
        {
            DBObjectCollection entyList = new DBObjectCollection();

            Matrix3d mat = Matrix3d.Displacement(cc.Convert3D().GetAsVector());
            // 中心
            Point2d pt0 = new Point2d(model.X, model.MainArch.Get3PointReal(model.X,90)[0].Y);
            Point2d pt0a = model.MainArch.Get7PointReal(model.X - 0.5 * model.L,90.0)[0].ToAcadPoint2d();
            Point2d pt0b = model.MainArch.Get7PointReal(model.X + 0.5 * model.L,90.0)[0].ToAcadPoint2d();

            Line cl = new Line(pt0.Convert3D(), new Point3d(model.X,model.Z2,0)) { Layer="中心线"} ;
            entyList.Add(cl);
            cl = new Line(pt0a.Convert3D(), new Point3d(pt0a.X, model.Z2, 0)) { Layer = "中心线" };
            entyList.Add(cl);
            cl = new Line(pt0b.Convert3D(), new Point3d(pt0b.X, model.Z2, 0)) { Layer = "中心线" };
            entyList.Add(cl);


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

            // 冒梁

            entyList.Add(new Line(
                new Point3d(model.X - 0.5 * model.L - 0.5 * model.MainDiameter, model.Z2, 0),
                new Point3d(model.X + 0.5 * model.L + 0.5 * model.MainDiameter, model.Z2, 0))
            { Layer = "细线" });
            entyList.Add(new Line(
                new Point3d(model.X - 0.5 * model.L + 0.5 * model.MainDiameter, model.Z2-model.CapHeight+model.InstallOffset, 0),
                new Point3d(model.X + 0.5 * model.L - 0.5 * model.MainDiameter, model.Z2 - model.CapHeight + model.InstallOffset, 0))
            { Layer = "细线" });

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

            double x1 = model.X - 0.5 * model.L + model.MainDiameter * 0.5;
            double x2 = model.X + 0.5 * model.L - model.MainDiameter * 0.5;
            for (int i = 0; i < model.K; i++)
            {
                var st = new Point2d(x1, model.Z1 + model.B + i * model.BeamStep);
                var ed = new Point2d(x2, model.Z1 + model.B + i * model.BeamStep);
                double dia = model.CrossLDiameter;
                var STube = MulitlinePloter.CreatTube(st, ed, dia, null, null, 0, "细线", false);

                foreach (var item in STube)
                {
                    entyList.Add(item);
                }
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

            for (int i = 0; i < model.M; i++)
            {
                var st = new Point2d(x1, model.Z1 + model.B + i * model.BeamStep);
                var ed = new Point2d(x2, model.Z1 + model.B + i * model.BeamStep);
                double dia = model.CrossLDiameter;
                var STube = MulitlinePloter.CreatTube(st, ed, dia, null, null, 0, "细线", false);
                foreach (var item in STube)
                {
                    entyList.Add(item);
                }

                entyList.Add(new Circle(st.Convert3D(-model.MainDiameter * 0.5, 0), Vector3d.ZAxis, 0.5*model.CrossWDiameter) { Layer = "虚线" });
                entyList.Add(new Circle(ed.Convert3D(model.MainDiameter * 0.5, 0), Vector3d.ZAxis,0.5* model.CrossWDiameter) { Layer = "虚线" });
                entyList.Add(new Line(st.Convert3D(-model.MainDiameter, 0), ed.Convert3D(model.MainDiameter, 0)) { Layer = "中心线" });
            }







            foreach (var item in entyList)
            {
                var ent = (Entity)item;
                ent.TransformBy(mat);
                ext=ext.Add(ent.GeometricExtents.Convert2D());
                db.AddEntityToModeSpace((Entity)item);
            }

            

        }


    }
}
