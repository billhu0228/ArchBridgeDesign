using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using MathNet.Spatial.Units;

namespace CADInterface.Plotters
{

    public class PolylinePloter
    {

        public static void AddPolyline(Database db, ref Extents2d ext, Point2d sp, Point2d ep, double dx0, double dy0, double dx1, double dy1, string linetypeName, short num = 0)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Polyline line = new Polyline() { Closed = false, Layer = linetypeName };//定义不封闭的Polyline
                line.AddVertexAt(0, sp.Convert2D(dx0, dy0), 0, 0, 0);
                line.AddVertexAt(1, ep.Convert2D(dx1, dy1), 0, 0, 0);
                Color color = Color.FromColorIndex(ColorMethod.ByColor, num);

                line.Color = color;
                line.LineWeight = Autodesk.AutoCAD.DatabaseServices.LineWeight.ByLayer;

                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);

                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
        }

        public static Polyline AddPolylineByList(Database db, ref Extents2d ext, List<Point2d> ptList, string linetypeName, bool isClose = true)
        {
            Polyline line;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                line = new Polyline() { Closed = isClose, Layer = linetypeName ,Plinegen=true};//定义不封闭的Polyline
                for (int i = 0; i < ptList.Count; i++)
                {
                    line.AddVertexAt(i, ptList[i], 0, 0, 0);
                }
                //Color color = Color.FromColorIndex(ColorMethod.ByColor, num);
                
                //line.Color = Color.;
                line.LineWeight = LineWeight.ByLayer;

                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }

            return line;
        }

        public static void AddPolylineByList(Database db, ref Extents2d ext, List<Point2d> ptList, string linetypeName, out Polyline line, short num = 0, bool isClose = true)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                line = new Polyline() { Closed = isClose, Layer = linetypeName };//定义不封闭的Polyline
                for (int i = 0; i < ptList.Count; i++)
                {
                    line.AddVertexAt(i, ptList[i], 0, 0, 0);
                }
                Color color = Color.FromColorIndex(ColorMethod.ByColor, num);

                line.Color = color;
                line.LineWeight = Autodesk.AutoCAD.DatabaseServices.LineWeight.ByLayer;

                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
        }

        public static void AddPloy4WithLink(Database db, ref Extents2d ext, Point2d upcenter, double wl, double wr, double h, double linkH, string layer = "细线")
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point2d ptop, pbot;
                Point2d p11, p12, p13, p14;
                Point2d p21, p22, p23, p24;
                Point2d p31, p32, p33, p34;
                ptop = upcenter;
                pbot = upcenter.Convert2D(0, -h);
                p11 = ptop.Convert2D(-wl);
                p12 = pbot.Convert2D(-wl);
                p13 = ptop.Convert2D(wr);
                p14 = pbot.Convert2D(wr);

                double off = (h - linkH) * 0.5;
                p21 = p11.Convert2D(h);
                p22 = p21.Convert2D(0, -off);
                p24 = p13.Convert2D(-h);
                p23 = p24.Convert2D(0, -off);

                p31 = p12.Convert2D(h);
                p32 = p31.Convert2D(0, off);
                p34 = p14.Convert2D(-h);
                p33 = p34.Convert2D(0, off);
                ptop = ptop.Convert2D(0, -off);
                pbot = pbot.Convert2D(0, off);

                Polyline PL1 = new Polyline() { Closed = true, Layer = layer };
                PL1.AddVertexAt(0, ptop, 0, 0, 0);
                PL1.AddVertexAt(1, p22, 0, 0, 0);
                PL1.AddVertexAt(2, p21, 0, 0, 0);
                PL1.AddVertexAt(3, p11, 0, 0, 0);
                PL1.AddVertexAt(4, p12, 0, 0, 0);
                PL1.AddVertexAt(5, p31, 0, 0, 0);
                PL1.AddVertexAt(6, p32, 0, 0, 0);
                PL1.AddVertexAt(7, pbot, 0, 0, 0);
                PL1.AddVertexAt(8, p33, 0, 0, 0);
                PL1.AddVertexAt(9, p34, 0, 0, 0);
                PL1.AddVertexAt(10, p14, 0, 0, 0);
                PL1.AddVertexAt(11, p13, 0, 0, 0);
                PL1.AddVertexAt(12, p24, 0, 0, 0);
                PL1.AddVertexAt(13, p23, 0, 0, 0);

                ext = ext.Add(new Extents2d(PL1.Bounds.Value.MinPoint.Convert2D(), PL1.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
        }


        /// <summary>
        /// 八边形绘图
        /// </summary>
        /// <param name="AnchorPoint">锚点，八边形下中心点</param>
        /// <param name="Height">总高</param>
        /// <param name="Width">总宽</param>
        /// <param name="F1x">下倒角，x方向</param>
        /// <param name="F1y">下倒角，y方向</param>
        /// <param name="F2x">上倒角，x方向</param>
        /// <param name="F2y">上倒角，y方向</param>
        public static void AddPoly8(Database db, ref Extents2d ext, Point2d AnchorPoint, double Height, double Width,

            double F1x = 50, double F1y = 50, double F2x = 50, double F2y = 50, string layer = "细线")
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Polyline PL1 = new Polyline() { Closed = true, Layer = layer };

                Line2d AxisY = new Line2d(AnchorPoint, AnchorPoint.Convert2D(0, 1));
                Point2d p0, p1, p2, p3, p4, p5, p6, p7;

                p0 = AnchorPoint.Convert2D(-0.5 * Width + F1x, 0);
                p1 = p0.Mirror(AxisY);
                p2 = p1.Convert2D(F1x, -F1y);
                p3 = p2.Convert2D(0, -Height + F1y + F2y);
                p4 = p3.Convert2D(-F2x, -F2y);
                p5 = p4.Mirror(AxisY);
                p6 = p3.Mirror(AxisY);
                p7 = p2.Mirror(AxisY);
                PL1.AddVertexAt(0, p0, 0, 0, 0);
                PL1.AddVertexAt(1, p1, 0, 0, 0);
                PL1.AddVertexAt(2, p2, 0, 0, 0);
                PL1.AddVertexAt(3, p3, 0, 0, 0);
                PL1.AddVertexAt(4, p4, 0, 0, 0);
                PL1.AddVertexAt(5, p5, 0, 0, 0);
                PL1.AddVertexAt(6, p6, 0, 0, 0);
                PL1.AddVertexAt(7, p7, 0, 0, 0);
                recorder.AppendEntity(PL1);
                tr.AddNewlyCreatedDBObject(PL1, true);
                ext = ext.Add(new Extents2d(PL1.Bounds.Value.MinPoint.Convert2D(), PL1.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
        }

        public static void AddPloy4(Database db, ref Extents2d ext, Point2d upcenter, double wl, double wr, double h, string layer = "细线")
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point2d ptop, pbot;
                Point2d p11, p12, p13, p14;
                ptop = upcenter;
                pbot = upcenter.Convert2D(0, -h);
                p11 = ptop.Convert2D(-wl);
                p12 = pbot.Convert2D(-wl);
                p13 = ptop.Convert2D(wr);
                p14 = pbot.Convert2D(wr);
                Polyline PL1 = new Polyline() { Closed = true, Layer = layer };
                PL1.AddVertexAt(0, p12, 0, 0, 0);
                PL1.AddVertexAt(1, pbot, 0, 0, 0);
                PL1.AddVertexAt(2, p14, 0, 0, 0);

                PL1.AddVertexAt(3, p13, 0, 0, 0);
                PL1.AddVertexAt(4, ptop, 0, 0, 0);
                PL1.AddVertexAt(5, p11, 0, 0, 0);
                recorder.AppendEntity(PL1);
                tr.AddNewlyCreatedDBObject(PL1, true);
                ext = ext.Add(new Extents2d(PL1.Bounds.Value.MinPoint.Convert2D(), PL1.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
        }


        public static Polyline CreatPloy4(Point2d upcenter, double wl, double wr, double h, string layer = "细线")
        {
            Point2d ptop, pbot;
            Point2d p11, p12, p13, p14;
            ptop = upcenter;
            pbot = upcenter.Convert2D(0, -h);
            p11 = ptop.Convert2D(-wl);
            p12 = pbot.Convert2D(-wl);
            p13 = ptop.Convert2D(wr);
            p14 = pbot.Convert2D(wr);
            Polyline PL1 = new Polyline() { Closed = true, Layer = layer };
            PL1.AddVertexAt(0, p12, 0, 0, 0);
            PL1.AddVertexAt(1, pbot, 0, 0, 0);
            PL1.AddVertexAt(2, p14, 0, 0, 0);

            PL1.AddVertexAt(3, p13, 0, 0, 0);
            PL1.AddVertexAt(4, ptop, 0, 0, 0);
            PL1.AddVertexAt(5, p11, 0, 0, 0);
            return PL1;

        }

        public static void AddPoly8(Database db, ref Extents2d ext, out Polyline PL1, Point2d AnchorPoint, double Height, double Width,
    double F1x = 50, double F1y = 50, double F2x = 50, double F2y = 50, string layer = "细线")
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                PL1 = new Polyline() { Closed = true, Layer = layer };

                Line2d AxisY = new Line2d(AnchorPoint, AnchorPoint.Convert2D(0, 1));
                Point2d p0, p1, p2, p3, p4, p5, p6, p7;

                p0 = AnchorPoint.Convert2D(-0.5 * Width + F1x, 0);
                p1 = p0.Mirror(AxisY);
                p2 = p1.Convert2D(F1x, -F1y);
                p3 = p2.Convert2D(0, -Height + F1y + F2y);
                p4 = p3.Convert2D(-F2x, -F2y);
                p5 = p4.Mirror(AxisY);
                p6 = p3.Mirror(AxisY);
                p7 = p2.Mirror(AxisY);
                PL1.AddVertexAt(0, p0, 0, 0, 0);
                PL1.AddVertexAt(1, p1, 0, 0, 0);
                PL1.AddVertexAt(2, p2, 0, 0, 0);
                PL1.AddVertexAt(3, p3, 0, 0, 0);
                PL1.AddVertexAt(4, p4, 0, 0, 0);
                PL1.AddVertexAt(5, p5, 0, 0, 0);
                PL1.AddVertexAt(6, p6, 0, 0, 0);
                PL1.AddVertexAt(7, p7, 0, 0, 0);
                recorder.AppendEntity(PL1);
                tr.AddNewlyCreatedDBObject(PL1, true);
                ext = ext.Add(new Extents2d(PL1.Bounds.Value.MinPoint.Convert2D(), PL1.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
        }

        public static void AddPloy4(Database db, ref Extents2d ext, out Polyline PL1, Point2d upcenter, double wl, double wr, double h, string layer = "细线")
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point2d ptop, pbot;
                Point2d p11, p12, p13, p14;
                ptop = upcenter;
                pbot = upcenter.Convert2D(0, -h);
                p11 = ptop.Convert2D(-wl);
                p12 = pbot.Convert2D(-wl);
                p13 = ptop.Convert2D(wr);
                p14 = pbot.Convert2D(wr);
                PL1 = new Polyline() { Closed = true, Layer = layer };
                PL1.AddVertexAt(0, p12, 0, 0, 0);
                PL1.AddVertexAt(1, pbot, 0, 0, 0);
                PL1.AddVertexAt(2, p14, 0, 0, 0);

                PL1.AddVertexAt(3, p13, 0, 0, 0);
                PL1.AddVertexAt(4, ptop, 0, 0, 0);
                PL1.AddVertexAt(5, p11, 0, 0, 0);
                recorder.AppendEntity(PL1);
                tr.AddNewlyCreatedDBObject(PL1, true);
                ext = ext.Add(new Extents2d(PL1.Bounds.Value.MinPoint.Convert2D(), PL1.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
        }


        public static void CreateGirdElev(Database db, ref Extents2d ext, Point2d insertPoint,
            double tWidth, double dia, List<double> topSp,
            int numLayer, double step, out List<Point3d> rebarCC)
        {
            //width = 0;
            double height = 0;
            double RebarWidth = topSp.Sum();
            double radius = dia * 0.5;
            rebarCC = new List<Point3d>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                double x0 = RebarWidth * -0.5;
                double y0 = 0;
                for (int m = 0; m < numLayer; m++)
                {
                    //画线
                    //Polyline line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                    //line.AddVertexAt(0, insertPoint.Convert2D(-0.5*tWidth, -dH), 0, 0, 0);
                    //line.AddVertexAt(1, insertPoint.Convert2D(0.5*tWidth, -dH), 0, 0, 0);
                    Line line = new Line(insertPoint.Convert3D(-0.5 * tWidth, y0), insertPoint.Convert3D(0.5 * tWidth, y0));
                    line.Layer = "粗线";
                    line.ColorIndex = 1;
                    recorder.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                    #region 
                    //x0 = 0;
                    for (int i = 0; i < topSp.Count + 1; i++)
                    {
                        x0 = RebarWidth * (-0.5) + topSp.GetRange(0, i).Sum();
                        //画圆
                        Circle ci = new Circle();
                        ci.Center = insertPoint.Convert3D(x0, y0 + (Math.Sign(step)) * (radius + 1));
                        if (m == 0)
                        {
                            rebarCC.Add(ci.Center);

                        }
                        ci.Radius = radius;
                        ci.SetDatabaseDefaults();//用来把圆的颜色、图层、线型、打印样式、可见性等属性设为实体所在的数据库的默认值
                        ci.Layer = "标注";
                        recorder.AppendEntity(ci);
                        tr.AddNewlyCreatedDBObject(ci, true);
                        ext = ext.Add(new Extents2d(ci.Bounds.Value.MinPoint.Convert2D(), ci.Bounds.Value.MaxPoint.Convert2D()));
                        //圆填充
                        ObjectIdCollection collection = new ObjectIdCollection();
                        collection.Add(ci.ObjectId);
                        Hatch hatch = new Hatch();
                        hatch.Elevation = 0;
                        hatch.HatchStyle = HatchStyle.Normal;
                        hatch.ColorIndex = 7;
                        hatch.SetDatabaseDefaults();
                        hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); //设置填充图案
                        hatch.AppendLoop(HatchLoopTypes.Default, collection); //设置填充边界 //
                        hatch.EvaluateHatch(true);
                        hatch.Layer = "标注";
                        recorder.AppendEntity(hatch);
                        tr.AddNewlyCreatedDBObject(hatch, true);
                    }
                    y0 += step;
                    #endregion
                }

                tr.Commit();
            }
            rebarCC.Sort((x, y) => x.X.CompareTo(y.X));
        }


        public static void CreateLineHatch(Database db, ref Extents2d ext, Point2d Point, double radius,
            List<double> leftSp, List<double> topSp, double tWidth, bool isShowAllLine = false)
        {
            double width = 0;
            double height = 0;
            width = topSp.Sum();
            height = leftSp.Sum();
            double left = (tWidth - width) / 2;
            if (left <= 0)
                left = 0;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                double curWidth = 0;
                double curHeight = 0;
                for (int m = 0; m < leftSp.Count; m++)
                {
                    //画线
                    Polyline line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                    line.AddVertexAt(0, Point.Convert2D(0 - left, -curHeight), 0, 0, 0);
                    line.AddVertexAt(1, Point.Convert2D(width + left, -curHeight), 0, 0, 0);
                    recorder.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                    #region 
                    curWidth = 0;
                    for (int i = 0; i < topSp.Count; i++)
                    {
                        if (m == 0)
                        {
                            if (isShowAllLine)
                            {
                                if (i == 0)
                                {
                                    //画线
                                    line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                                    line.AddVertexAt(0, Point.Convert2D(curWidth - left, 0), 0, 0, 0);
                                    line.AddVertexAt(1, Point.Convert2D(curWidth - left, -height), 0, 0, 0);
                                    recorder.AppendEntity(line);
                                    tr.AddNewlyCreatedDBObject(line, true);
                                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                                }
                                else if (i == topSp.Count - 1)
                                {
                                    //画线
                                    line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                                    line.AddVertexAt(0, Point.Convert2D(curWidth + left, 0), 0, 0, 0);
                                    line.AddVertexAt(1, Point.Convert2D(curWidth + left, -height), 0, 0, 0);
                                    recorder.AppendEntity(line);
                                    tr.AddNewlyCreatedDBObject(line, true);
                                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                                }
                                else
                                {
                                    //画线
                                    line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                                    line.AddVertexAt(0, Point.Convert2D(curWidth, 0), 0, 0, 0);
                                    line.AddVertexAt(1, Point.Convert2D(curWidth, -height), 0, 0, 0);
                                    recorder.AppendEntity(line);
                                    tr.AddNewlyCreatedDBObject(line, true);
                                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    //画线
                                    line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                                    line.AddVertexAt(0, Point.Convert2D(curWidth - left, 0), 0, 0, 0);
                                    line.AddVertexAt(1, Point.Convert2D(curWidth - left, -height), 0, 0, 0);
                                    recorder.AppendEntity(line);
                                    tr.AddNewlyCreatedDBObject(line, true);
                                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                                }
                            }

                        }
                        //画圆
                        Circle ci = new Circle();
                        ci.Center = Point.Convert3D(curWidth + radius + 0.5, -curHeight - radius - 0.5);
                        ci.Radius = radius;
                        ci.SetDatabaseDefaults();//用来把圆的颜色、图层、线型、打印样式、可见性等属性设为实体所在的数据库的默认值
                        recorder.AppendEntity(ci);
                        tr.AddNewlyCreatedDBObject(ci, true);
                        ext = ext.Add(new Extents2d(ci.Bounds.Value.MinPoint.Convert2D(), ci.Bounds.Value.MaxPoint.Convert2D()));
                        //圆填充
                        ObjectIdCollection collection = new ObjectIdCollection();
                        collection.Add(ci.ObjectId);
                        Hatch hatch = new Hatch();
                        hatch.Elevation = 0;
                        hatch.HatchStyle = HatchStyle.Normal;
                        hatch.ColorIndex = 7;
                        hatch.SetDatabaseDefaults();
                        hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); //设置填充图案
                        hatch.AppendLoop(HatchLoopTypes.Default, collection); //设置填充边界 //
                        hatch.EvaluateHatch(true);
                        recorder.AppendEntity(hatch);
                        tr.AddNewlyCreatedDBObject(hatch, true);
                        curWidth += topSp[i];
                        if (i == topSp.Count - 1)
                        {
                            if (m == 0)
                            {
                                line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                                line.AddVertexAt(0, Point.Convert2D(curWidth + left, 0), 0, 0, 0);
                                line.AddVertexAt(1, Point.Convert2D(curWidth + left, -height), 0, 0, 0);
                                recorder.AppendEntity(line);
                                tr.AddNewlyCreatedDBObject(line, true);
                                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                            }
                            //画圆
                            ci = new Circle();
                            ci.Center = Point.Convert3D(curWidth + radius + 0.5, -curHeight - radius - 0.5);
                            ci.Radius = radius;
                            recorder.AppendEntity(ci);
                            tr.AddNewlyCreatedDBObject(ci, true);
                            ext = ext.Add(new Extents2d(ci.Bounds.Value.MinPoint.Convert2D(), ci.Bounds.Value.MaxPoint.Convert2D()));
                            //圆填充
                            collection = new ObjectIdCollection();
                            collection.Add(ci.ObjectId);
                            hatch = new Hatch();
                            hatch.Elevation = 0;
                            hatch.HatchStyle = HatchStyle.Normal;
                            hatch.ColorIndex = 7;
                            hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); //设置填充图案
                            hatch.AppendLoop(HatchLoopTypes.Default, collection); //设置填充边界 //
                            hatch.EvaluateHatch(true);
                            recorder.AppendEntity(hatch);
                            tr.AddNewlyCreatedDBObject(hatch, true);
                        }
                    }
                    curHeight += leftSp[m];
                    #endregion
                }

                tr.Commit();
            }
        }
        /// <summary>
        /// 顶层和底层布筋
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="Point">（最左边作为起点）参考点</param>
        /// <param name="radius">半径</param>
        /// <param name="topSp">间隔列表</param>
        /// <param name="sacle">比例尺</param>
        /// <param name="isOnLineUpper">是否在线上面</param>
        public static void CreateSingleLineHatch(Database db, ref Extents2d ext, Point2d Point, double radius,
         List<double> topSp, double sacle, double Dia, bool isOnLineUpper = true, string layer = "粗线")
        {
            radius = radius * sacle;
            double width = 0;
            width = topSp.Sum();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                double curWidth = 0;

                //画线
                Polyline line = new Polyline() { Closed = false, Layer = layer, ColorIndex = 1 };//定义不封闭的Polyline 平面虚线

                line.AddVertexAt(0, Point.Convert2D(0, 0), 0, 0, 0);
                line.AddVertexAt(1, Point.Convert2D(width + Dia, 0), 0, 0, 0);

                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                #region 
                curWidth = 0;
                for (int i = 0; i < topSp.Count + 1; i++)
                {
                    //画圆
                    Circle ci = new Circle();
                    if (isOnLineUpper)
                    {
                        ci.Center = Point.Convert3D(curWidth + Dia / 2, radius);
                    }
                    else
                    {
                        ci.Center = Point.Convert3D(curWidth + Dia / 2, -radius);
                    }
                    ci.Radius = radius;
                    ci.ColorIndex = 1;
                    ci.SetDatabaseDefaults();//用来把圆的颜色、图层、线型、打印样式、可见性等属性设为实体所在的数据库的默认值
                    recorder.AppendEntity(ci);
                    tr.AddNewlyCreatedDBObject(ci, true);
                    ext = ext.Add(new Extents2d(ci.Bounds.Value.MinPoint.Convert2D(), ci.Bounds.Value.MaxPoint.Convert2D()));
                    //圆填充
                    ObjectIdCollection collection = new ObjectIdCollection();
                    collection.Add(ci.ObjectId);
                    Hatch hatch = new Hatch();
                    hatch.Elevation = 0;
                    hatch.HatchStyle = HatchStyle.Normal;
                    hatch.ColorIndex = 6;
                    hatch.SetDatabaseDefaults();
                    hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); //设置填充图案
                    hatch.AppendLoop(HatchLoopTypes.Default, collection); //设置填充边界 //
                    hatch.EvaluateHatch(true);
                    recorder.AppendEntity(hatch);
                    tr.AddNewlyCreatedDBObject(hatch, true);
                    if (i < topSp.Count)
                        curWidth += topSp[i];
                }

                #endregion


                tr.Commit();
            }
        }
        /// <summary>
        /// 侧面布筋
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="Point">（最下面作为起点）参考点</param>
        /// <param name="radius">半径</param>
        /// <param name="topSp">侧面间隔集合</param>
        /// <param name="sacle">比例尺</param>
        /// <param name="bH">底层钢筋高度</param>
        /// <param name="isOnLineRight">是否在右边</param>
        public static void CreateSideSingleLineHatch(Database db, ref Extents2d ext, Point2d Point, double radius,
    List<double> sideSp, double sacle, double Dia, double bH = 0, bool isOnLineRight = true, string layer = "粗线", bool isAddDia = false)
        {
            radius = radius * sacle;
            double height = 0;
            height = sideSp.Sum();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                double curHeight = 0;

                //画线
                Polyline line = new Polyline() { Closed = false, Layer = layer, ColorIndex = 1 };//定义不封闭的Polyline 平面虚线

                line.AddVertexAt(0, Point.Convert2D(0, -bH), 0, 0, 0);
                if (isAddDia)
                    line.AddVertexAt(1, Point.Convert2D(0, height + Dia / 2 + Dia / 2), 0, 0, 0);
                else
                    line.AddVertexAt(1, Point.Convert2D(0, height + Dia / 2), 0, 0, 0);

                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                #region 
                curHeight = 0;
                for (int i = 0; i < sideSp.Count; i++)
                {
                    if (i > 0)
                    {
                        //画圆
                        Circle ci = new Circle();
                        if (isOnLineRight)
                        {
                            ci.Center = Point.Convert3D(radius, curHeight + Dia / 2);
                        }
                        else
                        {
                            ci.Center = Point.Convert3D(-radius, curHeight + Dia / 2);
                        }
                        ci.Radius = radius;
                        ci.ColorIndex = 1;
                        ci.SetDatabaseDefaults();//用来把圆的颜色、图层、线型、打印样式、可见性等属性设为实体所在的数据库的默认值
                        recorder.AppendEntity(ci);
                        tr.AddNewlyCreatedDBObject(ci, true);
                        ext = ext.Add(new Extents2d(ci.Bounds.Value.MinPoint.Convert2D(), ci.Bounds.Value.MaxPoint.Convert2D()));
                        //圆填充
                        ObjectIdCollection collection = new ObjectIdCollection();
                        collection.Add(ci.ObjectId);
                        Hatch hatch = new Hatch();
                        hatch.Elevation = 0;
                        hatch.HatchStyle = HatchStyle.Normal;
                        hatch.ColorIndex = 6;
                        hatch.SetDatabaseDefaults();
                        hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); //设置填充图案
                        hatch.AppendLoop(HatchLoopTypes.Default, collection); //设置填充边界 //
                        hatch.EvaluateHatch(true);
                        recorder.AppendEntity(hatch);
                        tr.AddNewlyCreatedDBObject(hatch, true);
                    }
                    if (i < sideSp.Count)
                        curHeight += sideSp[i];
                }

                #endregion


                tr.Commit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Point"></param>
        /// <param name="leftSp"></param>
        /// <param name="topSp"></param>
        /// <param name="ext"></param>
        /// <param name="exLength"></param>
        /// <param name="PtsLeft">左侧标注基点</param>
        /// <param name="PtsTop">顶部标注基点</param>
        public static void CreateLineGrid(Database db, Point2d Point, List<double> leftSp, List<double> topSp, ref Extents2d ext, double exLength,
            out List<Point3d> PtsLeft, out List<Point3d> PtsTop)
        {
            double width = 0;
            double height = 0;
            width = topSp.Sum();
            height = leftSp.Sum();
            double curWidth = 0;
            double curHeight = 0;

            //             输出横纵向PT列表
            PtsLeft = new List<Point3d>(); // 左侧列表
            PtsTop = new List<Point3d>();  // 顶部列表
            //             输出横纵向PT列表

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                for (int m = 0; m < leftSp.Count; m++)
                {
                    //画线
                    Polyline line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                    line.AddVertexAt(0, Point.Convert2D(0 - exLength, -curHeight), 0, 0, 0);
                    PtsLeft.Add(Point.Convert3D(0, -curHeight));
                    line.AddVertexAt(1, Point.Convert2D(width + exLength, -curHeight), 0, 0, 0);
                    recorder.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                    #region 
                    curWidth = 0;
                    for (int i = 0; i < topSp.Count; i++)
                    {
                        if (m == 0)
                        {
                            //画线
                            line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                            line.AddVertexAt(0, Point.Convert2D(curWidth, 0 + exLength), 0, 0, 0);
                            PtsTop.Add(Point.Convert3D(curWidth, 0));
                            line.AddVertexAt(1, Point.Convert2D(curWidth, -height - exLength), 0, 0, 0);
                            recorder.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);
                            ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                        }

                        curWidth += topSp[i];
                        if (i == topSp.Count - 1)
                        {
                            if (m == 0)
                            {
                                //画线
                                line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                                line.AddVertexAt(0, Point.Convert2D(curWidth, 0 + exLength), 0, 0, 0);
                                PtsTop.Add(Point.Convert3D(curWidth, 0));
                                line.AddVertexAt(1, Point.Convert2D(curWidth, -height - exLength), 0, 0, 0);
                                recorder.AppendEntity(line);
                                tr.AddNewlyCreatedDBObject(line, true);
                                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                            }
                        }


                    }
                    curHeight += leftSp[m];
                    if (m == leftSp.Count - 1)
                    {
                        //画线
                        line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                        line.AddVertexAt(0, Point.Convert2D(0 - exLength, -height), 0, 0, 0);
                        PtsLeft.Add(Point.Convert3D(0, -height));
                        line.AddVertexAt(1, Point.Convert2D(width + exLength, -height), 0, 0, 0);
                        recorder.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);
                        ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                    }
                    #endregion
                }
                tr.Commit();

            }

            PtsTop.Sort((x, y) => x.X.CompareTo(y.X));
            PtsLeft.Sort((x, y) => x.Y.CompareTo(y.Y));
        }

        //public static void CreateMultiLineCicle(Database db, ref Extents2d ext, Point3d refpt, List<Point3d> listP, string textstring = "1", double scale = 1)
        //{

        //    double cR = 2.5;
        //    DBText txt = new DBText();
        //    Circle C2 = new Circle();
        //    using (Transaction tr = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
        //            OpenMode.ForWrite) as BlockTableRecord;
        //        TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
        //        bool isUp = true;
        //        if (listP.Count > 0)
        //        {
        //            foreach (var p in listP)
        //            {
        //                Line line = new Line(p, refpt);
        //                line.Layer = "细线";
        //                modelSpace.AppendEntity(line);
        //                tr.AddNewlyCreatedDBObject(line, true);
        //                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
        //            }
        //            if (listP[0].Y > refpt.Y)
        //            {
        //                isUp = false;
        //            }
        //        }
        //        double height = cR * scale;
        //        if (!isUp)
        //            height = -cR * scale;
        //        txt.TextString = textstring.ToString();
        //        txt.Height = 2.5 * scale;
        //        txt.Position = refpt.Convert3D(0, height);
        //        txt.HorizontalMode = TextHorizontalMode.TextCenter;
        //        txt.VerticalMode = TextVerticalMode.TextVerticalMid;
        //        txt.AlignmentPoint = refpt.Convert3D(0, height);
        //        txt.TextStyleId = st[Extensions.curFont];
        //        txt.ColorIndex = 4;
        //        txt.Layer = "细线";
        //        txt.WidthFactor = 0.8;

        //        C2 = new Circle(refpt.Convert3D(0, height), Vector3d.ZAxis, cR * scale);
        //        C2.Layer = "细线";
        //        C2.ColorIndex = 4;
        //        modelSpace.AppendEntity(txt);
        //        tr.AddNewlyCreatedDBObject(txt, true);
        //        modelSpace.AppendEntity(C2);
        //        tr.AddNewlyCreatedDBObject(C2, true);
        //        ext = ext.Add(new Extents2d(txt.Bounds.Value.MinPoint.Convert2D(), txt.Bounds.Value.MaxPoint.Convert2D()));
        //        ext = ext.Add(new Extents2d(C2.Bounds.Value.MinPoint.Convert2D(), C2.Bounds.Value.MaxPoint.Convert2D()));
        //        tr.Commit();
        //    }
        //}
        /// <summary>
        /// 画线和圆
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="listLinePoint">多线点集合</param>
        /// <param name="listCirclePoint">圆集合</param>
        /// <param name="tWidth"></param>
        /// <param name="dia"></param>
        /// <param name="scale"></param>
        public static void CreateLineCicles(Database db, ref Extents2d ext, List<Point3d> listLinePoint, List<Point3d> listCirclePoint, double tWidth, double dia, double scale = 1)
        {
            double radius = dia * 0.5;
            Polyline line = new Polyline() { Closed = false, Layer = "粗线" };
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                if (listLinePoint.Count > 0)
                {
                    foreach (var p in listLinePoint)
                    {
                        line.AddVertexAt(0, p.Convert2D(), 0, 0, 0);
                    }
                    recorder.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                }
                if (listCirclePoint.Count > 0)
                {
                    foreach (var p in listCirclePoint)
                    {
                        Circle ci = new Circle();
                        ci.Center = p.Convert3D(0, radius + 1);

                        ci.Radius = radius;
                        ci.SetDatabaseDefaults();//用来把圆的颜色、图层、线型、打印样式、可见性等属性设为实体所在的数据库的默认值
                        ci.Layer = "粗线";
                        recorder.AppendEntity(ci);
                        tr.AddNewlyCreatedDBObject(ci, true);
                        ext = ext.Add(new Extents2d(ci.Bounds.Value.MinPoint.Convert2D(), ci.Bounds.Value.MaxPoint.Convert2D()));
                        //圆填充
                        ObjectIdCollection collection = new ObjectIdCollection();
                        collection.Add(ci.ObjectId);
                        Hatch hatch = new Hatch();
                        hatch.Elevation = 0;
                        hatch.HatchStyle = HatchStyle.Normal;
                        hatch.ColorIndex = 7;
                        hatch.SetDatabaseDefaults();
                        hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); //设置填充图案
                        hatch.AppendLoop(HatchLoopTypes.Default, collection); //设置填充边界 //
                        hatch.EvaluateHatch(true);
                        hatch.Layer = "粗线";
                        recorder.AppendEntity(hatch);
                        tr.AddNewlyCreatedDBObject(hatch, true);
                    }

                }
                tr.Commit();
            }
        }

        ///// <summary>
        ///// 箭头标注
        ///// </summary>
        ///// <param name="db"></param>
        ///// <param name="ext"></param>
        ///// <param name="refpt"></param>
        ///// <param name="arrDir"></param>
        ///// <param name="leftSp"></param>
        ///// <param name="topSp"></param>
        ///// <param name="textstring"></param>
        ///// <param name="scale"></param>
        ///// <param name="isLastVisible"></param>
        ///// <param name="length"></param>
        //public static void CreateLineArrowDB(Database db, ref Extents2d ext, Point3d refpt, ArrowDirection arrDir, List<double> leftSp, List<double> topSp, string textstring = "1", double scale = 1, bool isLastVisible = true, double length = 0)
        //{
        //    double width = 0;
        //    double height = 0;
        //    width = topSp.Sum();
        //    height = leftSp.Sum();
        //    double curWidth = 0;
        //    double curHeight = 0;
        //    double cR = 2.5;
        //    DBText txt = new DBText();
        //    Circle C2 = new Circle();
        //    Point3d PositionPoint;
        //    using (Transaction tr = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
        //            OpenMode.ForWrite) as BlockTableRecord;
        //        TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
        //        Solid Array = new Solid();
        //        Line L = new Line() { Layer = "细线", ColorIndex = 4 };
        //        switch (arrDir)
        //        {
        //            case ArrowDirection.East:  //右
        //                refpt = refpt.Convert3D(width, 0);
        //                if (topSp.Count > 0)
        //                {
        //                    for (int i = 0; i < topSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                         refpt.Convert3D(0 - curWidth, 0, 0),
        //                         refpt.Convert3D(-2.65 * scale - curWidth, 0, 0),
        //                          refpt.Convert3D(-2 * scale - curWidth, 0.5 * scale, 0)
        //                         )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curWidth += topSp[topSp.Count - 1 - i];
        //                        if (i == topSp.Count - 1 && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                         refpt.Convert3D(0 - curWidth, 0, 0),
        //                         refpt.Convert3D(-2.65 * scale - curWidth, 0, 0),
        //                          refpt.Convert3D(-2 * scale - curWidth, 0.5 * scale, 0)
        //                         )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                         refpt.Convert3D(0 - curWidth, 0, 0),
        //                         refpt.Convert3D(-2.65 * scale - curWidth, 0, 0),
        //                          refpt.Convert3D(-2 * scale - curWidth, 0.5 * scale, 0)
        //                         )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(-width - (4 + length) * scale, 0, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(-width - (4 + length) * scale - cR * scale, 0, 0);
        //                break;
        //            case ArrowDirection.West:  //左
        //                if (topSp.Count > 0)
        //                {
        //                    for (int i = 0; i < topSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                     refpt.Convert3D(0 + curWidth, 0, 0),
        //                     refpt.Convert3D(2.65 * scale + curWidth, 0, 0),
        //                      refpt.Convert3D(2 * scale + curWidth, 0.5 * scale, 0)
        //                     )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curWidth += topSp[i];
        //                        if (i == topSp.Count - 1 && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                     refpt.Convert3D(0 + curWidth, 0, 0),
        //                     refpt.Convert3D(2.65 * scale + curWidth, 0, 0),
        //                      refpt.Convert3D(2 * scale + curWidth, 0.5 * scale, 0)
        //                     )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                   refpt.Convert3D(0 + curWidth, 0, 0),
        //                   refpt.Convert3D(2.65 * scale + curWidth, 0, 0),
        //                    refpt.Convert3D(2 * scale + curWidth, 0.5 * scale, 0)
        //                   )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(width + (4 + length) * scale, 0, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(width + (4 + length) * scale + cR * scale, 0, 0);
        //                break;
        //            case ArrowDirection.South:  //下
        //                refpt = refpt.Convert3D(0, -height);
        //                if (leftSp.Count > 0)
        //                {
        //                    for (int i = 0; i < leftSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                     refpt.Convert3D(0, 0 + curHeight, 0),
        //                     refpt.Convert3D(0, 2.65 * scale + curHeight, 0),
        //                      refpt.Convert3D(0.5 * scale, 2 * scale + curHeight, 0)
        //                     )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curHeight += leftSp[leftSp.Count - 1 - i];
        //                        if (i == leftSp.Count - 1 && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                   refpt.Convert3D(0, 0 + curHeight, 0),
        //                   refpt.Convert3D(0, 2.65 * scale + curHeight, 0),
        //                    refpt.Convert3D(0.5 * scale, 2 * scale + curHeight, 0)
        //                   )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                    refpt.Convert3D(0, 0 + curHeight, 0),
        //                    refpt.Convert3D(0, 2.65 * scale + curHeight, 0),
        //                     refpt.Convert3D(0.5 * scale, 2 * scale + curHeight, 0)
        //                    )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(0, height + (4 + length) * scale, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(0, height + (4 + length) * scale + cR * scale, 0);
        //                break;
        //            case ArrowDirection.North:   //上
        //                if (leftSp.Count > 0)
        //                {
        //                    for (int i = 0; i < leftSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                     refpt.Convert3D(0, 0 - curHeight, 0),
        //                     refpt.Convert3D(0, -2.65 * scale - curHeight, 0),
        //                      refpt.Convert3D(0.5 * scale, -2 * scale - curHeight, 0)
        //                     )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curHeight += leftSp[i];
        //                        if (i == leftSp.Count && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                    refpt.Convert3D(0, 0 - curHeight, 0),
        //                    refpt.Convert3D(0, -2.65 * scale - curHeight, 0),
        //                     refpt.Convert3D(0.5 * scale, -2 * scale - curHeight, 0)
        //                    )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                     refpt.Convert3D(0, 0 - curHeight, 0),
        //                     refpt.Convert3D(0, -2.65 * scale - curHeight, 0),
        //                      refpt.Convert3D(0.5 * scale, -2 * scale - curHeight, 0)
        //                     )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(0, -height - (4 + length) * scale, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(0, -height - (4 + length) * scale - cR * scale, 0);
        //                break;
        //            default:  //右
        //                refpt = refpt.Convert3D(width, 0);
        //                for (int i = 0; i < topSp.Count; i++)
        //                {
        //                    Array = new Solid(
        //                     refpt.Convert3D(0 - curWidth, 0, 0),
        //                     refpt.Convert3D(-2.65 * scale - curWidth, 0, 0),
        //                      refpt.Convert3D(-2 * scale - curWidth, 0.5 * scale, 0)
        //                     )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                    curWidth += topSp[topSp.Count - 1 - i];
        //                }
        //                L = new Line(refpt.Convert3D(-width - (4 + length) * scale, 0, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(-width - (4 + length) * scale - cR * scale, 0, 0);
        //                break;
        //        }
        //        ext = ext.Add(new Extents2d(Array.Bounds.Value.MinPoint.Convert2D(), Array.Bounds.Value.MaxPoint.Convert2D()));
        //        ext = ext.Add(new Extents2d(L.Bounds.Value.MinPoint.Convert2D(), L.Bounds.Value.MaxPoint.Convert2D()));
        //        txt.TextString = textstring.ToString();
        //        txt.Height = 2.5 * scale;
        //        txt.Position = PositionPoint.Convert3D();
        //        txt.HorizontalMode = TextHorizontalMode.TextCenter;
        //        txt.VerticalMode = TextVerticalMode.TextVerticalMid;
        //        txt.AlignmentPoint = PositionPoint.Convert3D();
        //        txt.TextStyleId = st[Extensions.curFont];
        //        txt.Layer = "标注";
        //        txt.ColorIndex = 4;
        //        txt.WidthFactor = 0.8;

        //        C2 = new Circle(PositionPoint.Convert3D(), Vector3d.ZAxis, cR * scale);
        //        C2.Layer = "标注";
        //        C2.ColorIndex = 4;
        //        modelSpace.AppendEntity(txt);
        //        tr.AddNewlyCreatedDBObject(txt, true);
        //        modelSpace.AppendEntity(C2);
        //        tr.AddNewlyCreatedDBObject(C2, true);
        //        ext = ext.Add(new Extents2d(txt.Bounds.Value.MinPoint.Convert2D(), txt.Bounds.Value.MaxPoint.Convert2D()));
        //        ext = ext.Add(new Extents2d(C2.Bounds.Value.MinPoint.Convert2D(), C2.Bounds.Value.MaxPoint.Convert2D()));
        //        tr.Commit();
        //    }
        //}

        ///// <summary>
        ///// 箭头
        ///// </summary>
        ///// <param name="db"></param>
        ///// <param name="ext"></param>
        ///// <param name="refpt"></param>
        ///// <param name="arrDir"></param>
        ///// <param name="leftSp"></param>
        ///// <param name="topSp"></param>
        ///// <param name="textstring"></param>
        ///// <param name="scale"></param>
        ///// <param name="isLastVisible"></param>
        ///// <param name="length"></param>
        ///// <param name="leftLength">上下标注折线长度</param>
        //public static void CreateLineArrowDBNew(Database db, ref Extents2d ext, Point3d refpt, ArrowDirection arrDir, List<double> leftSp, List<double> topSp, string textstring = "1", double scale = 1, bool isLastVisible = true, double length = 0, double leftLength = 0)
        //{
        //    double width = 0;
        //    double height = 0;
        //    width = topSp.Sum();
        //    height = leftSp.Sum();
        //    double curWidth = 0;
        //    double curHeight = 0;
        //    double cR = 2.5;
        //    DBText txt = new DBText();
        //    Circle C2 = new Circle();
        //    Point3d PositionPoint;
        //    using (Transaction tr = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
        //            OpenMode.ForWrite) as BlockTableRecord;
        //        TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
        //        Solid Array = new Solid();
        //        Line L = new Line() { Layer = "细线", ColorIndex = 4 };
        //        switch (arrDir)
        //        {
        //            case ArrowDirection.East:  //右
        //                refpt = refpt.Convert3D(width, 0);
        //                if (topSp.Count > 0)
        //                {
        //                    for (int i = 0; i < topSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                         refpt.Convert3D(0 - curWidth, 0, 0),
        //                         refpt.Convert3D(-0.01 * scale - curWidth, 0, 0),
        //                          refpt.Convert3D(-1 * scale - curWidth, 0.5 * scale, 0)
        //                         )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curWidth += topSp[topSp.Count - 1 - i];
        //                        if (i == topSp.Count - 1 && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                         refpt.Convert3D(0 - curWidth, 0, 0),
        //                         refpt.Convert3D(-0.01 * scale - curWidth, 0, 0),
        //                          refpt.Convert3D(-1 * scale - curWidth, 0.5 * scale, 0)
        //                         )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                         refpt.Convert3D(0 - curWidth, 0, 0),
        //                         refpt.Convert3D(-0.01 * scale - curWidth, 0, 0),
        //                          refpt.Convert3D(-1 * scale - curWidth, 0.5 * scale, 0)
        //                         )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(-width - (1 + length) * scale, 0, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(-width - (1 + length) * scale - cR * scale, 0, 0);
        //                break;
        //            case ArrowDirection.West:  //左
        //                if (topSp.Count > 0)
        //                {
        //                    for (int i = 0; i < topSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                     refpt.Convert3D(0 + curWidth, 0, 0),
        //                     refpt.Convert3D(0.01 * scale + curWidth, 0, 0),
        //                      refpt.Convert3D(1 * scale + curWidth, 0.5 * scale, 0)
        //                     )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curWidth += topSp[i];
        //                        if (i == topSp.Count - 1 && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                     refpt.Convert3D(0 + curWidth, 0, 0),
        //                     refpt.Convert3D(0.01 * scale + curWidth, 0, 0),
        //                      refpt.Convert3D(1 * scale + curWidth, 0.5 * scale, 0)
        //                     )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                   refpt.Convert3D(0 + curWidth, 0, 0),
        //                   refpt.Convert3D(0.01 * scale + curWidth, 0, 0),
        //                    refpt.Convert3D(1 * scale + curWidth, 0.5 * scale, 0)
        //                   )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(width + (1 + length) * scale, 0, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(width + (1 + length) * scale + cR * scale, 0, 0);
        //                break;
        //            case ArrowDirection.South:  //下
        //                refpt = refpt.Convert3D(0, -height);
        //                if (leftSp.Count > 0)
        //                {
        //                    for (int i = 0; i < leftSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                     refpt.Convert3D(0, 0 + curHeight, 0),
        //                     refpt.Convert3D(0, 0.01 * scale + curHeight, 0),
        //                      refpt.Convert3D(0.5 * scale, 1 * scale + curHeight, 0)
        //                     )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curHeight += leftSp[leftSp.Count - 1 - i];
        //                        if (i == leftSp.Count - 1 && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                   refpt.Convert3D(0, 0 + curHeight, 0),
        //                   refpt.Convert3D(0, 0.01 * scale + curHeight, 0),
        //                    refpt.Convert3D(0.5 * scale, 1 * scale + curHeight, 0)
        //                   )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                    refpt.Convert3D(0, 0 + curHeight, 0),
        //                    refpt.Convert3D(0, 0.01 * scale + curHeight, 0),
        //                     refpt.Convert3D(0.5 * scale, 1 * scale + curHeight, 0)
        //                    )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(0, height + (1 + length) * scale, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                if (leftLength > 0)
        //                {
        //                    L = new Line(refpt.Convert3D(leftLength, height + (1 + length) * scale, 0), refpt.Convert3D(0, height + (1 + length) * scale, 0))
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                    modelSpace.AppendEntity(L);
        //                    tr.AddNewlyCreatedDBObject(L, true);
        //                    PositionPoint = refpt.Convert3D(leftLength + cR * scale, height + (1 + length) * scale, 0);
        //                }
        //                else
        //                {
        //                    PositionPoint = refpt.Convert3D(0, height + (1 + length) * scale + cR * scale, 0);
        //                }
        //                break;
        //            case ArrowDirection.North:   //上
        //                if (leftSp.Count > 0)
        //                {
        //                    for (int i = 0; i < leftSp.Count; i++)
        //                    {
        //                        Array = new Solid(
        //                     refpt.Convert3D(0, 0 - curHeight, 0),
        //                     refpt.Convert3D(0, -0.01 * scale - curHeight, 0),
        //                      refpt.Convert3D(0.5 * scale, -1 * scale - curHeight, 0)
        //                     )
        //                        { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                        modelSpace.AppendEntity(Array);
        //                        tr.AddNewlyCreatedDBObject(Array, true);
        //                        curHeight += leftSp[i];
        //                        if (i == leftSp.Count && isLastVisible)
        //                        {
        //                            Array = new Solid(
        //                    refpt.Convert3D(0, 0 - curHeight, 0),
        //                    refpt.Convert3D(0, -0.01 * scale - curHeight, 0),
        //                     refpt.Convert3D(0.5 * scale, -1 * scale - curHeight, 0)
        //                    )
        //                            { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                            modelSpace.AppendEntity(Array);
        //                            tr.AddNewlyCreatedDBObject(Array, true);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Array = new Solid(
        //                     refpt.Convert3D(0, 0 - curHeight, 0),
        //                     refpt.Convert3D(0, -0.01 * scale - curHeight, 0),
        //                      refpt.Convert3D(0.5 * scale, -1 * scale - curHeight, 0)
        //                     )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };

        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                }
        //                L = new Line(refpt.Convert3D(0, -height - (1 + length) * scale, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                if (leftLength > 0)
        //                {
        //                    L = new Line(refpt.Convert3D(leftLength, -height - (1 + length) * scale, 0), refpt.Convert3D(0, -height - (1 + length) * scale, 0))
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                    modelSpace.AppendEntity(L);
        //                    tr.AddNewlyCreatedDBObject(L, true);
        //                    PositionPoint = refpt.Convert3D(leftLength + cR * scale, -height - (1 + length) * scale, 0);
        //                }
        //                else
        //                    PositionPoint = refpt.Convert3D(0, -height - (1 + length) * scale - cR * scale, 0);
        //                break;
        //            default:  //右
        //                refpt = refpt.Convert3D(width, 0);
        //                for (int i = 0; i < topSp.Count; i++)
        //                {
        //                    Array = new Solid(
        //                     refpt.Convert3D(0 - curWidth, 0, 0),
        //                     refpt.Convert3D(-0.01 * scale - curWidth, 0, 0),
        //                      refpt.Convert3D(-1 * scale - curWidth, 0.5 * scale, 0)
        //                     )
        //                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                    modelSpace.AppendEntity(Array);
        //                    tr.AddNewlyCreatedDBObject(Array, true);
        //                    curWidth += topSp[topSp.Count - 1 - i];
        //                }
        //                L = new Line(refpt.Convert3D(-width - (1 + length) * scale, 0, 0), refpt.Convert3D(0, 0, 0))
        //                { Color = Color.FromColorIndex(ColorMethod.ByAci, 4), Layer = "细线", };
        //                modelSpace.AppendEntity(L);
        //                tr.AddNewlyCreatedDBObject(L, true);
        //                PositionPoint = refpt.Convert3D(-width - (1 + length) * scale - cR * scale, 0, 0);
        //                break;
        //        }
        //        ext = ext.Add(new Extents2d(Array.Bounds.Value.MinPoint.Convert2D(), Array.Bounds.Value.MaxPoint.Convert2D()));
        //        ext = ext.Add(new Extents2d(L.Bounds.Value.MinPoint.Convert2D(), L.Bounds.Value.MaxPoint.Convert2D()));
        //        txt.TextString = textstring.ToString();
        //        txt.Height = 2.5 * scale;
        //        txt.Position = PositionPoint.Convert3D();
        //        txt.HorizontalMode = TextHorizontalMode.TextCenter;
        //        txt.VerticalMode = TextVerticalMode.TextVerticalMid;
        //        txt.AlignmentPoint = PositionPoint.Convert3D();
        //        txt.TextStyleId = st[Extensions.curFont];
        //        txt.Layer = "标注";
        //        txt.ColorIndex = 4;
        //        txt.WidthFactor = 0.8;

        //        C2 = new Circle(PositionPoint.Convert3D(), Vector3d.ZAxis, cR * scale);
        //        C2.Layer = "标注";
        //        C2.ColorIndex = 4;
        //        modelSpace.AppendEntity(txt);
        //        tr.AddNewlyCreatedDBObject(txt, true);
        //        modelSpace.AppendEntity(C2);
        //        tr.AddNewlyCreatedDBObject(C2, true);
        //        ext = ext.Add(new Extents2d(txt.Bounds.Value.MinPoint.Convert2D(), txt.Bounds.Value.MaxPoint.Convert2D()));
        //        ext = ext.Add(new Extents2d(C2.Bounds.Value.MinPoint.Convert2D(), C2.Bounds.Value.MaxPoint.Convert2D()));
        //        tr.Commit();
        //    }
        //}

        public static void CreateBreakLine(Database db, ref Extents2d ext, Point2d stPoint, Point2d endPoint, double scale = 20)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                #region
                Point2d centerPt = CommonTools.MidPoint(stPoint.Convert3D(), endPoint.Convert3D()).Convert2D();
                //画线
                Polyline line = new Polyline() { Closed = false, Layer = "细线", };//定义不封闭的Polyline 平面虚线
                if (stPoint.Y > endPoint.Y && stPoint.X == endPoint.X)
                {
                    line.AddVertexAt(0, stPoint, 0, 0, 0);
                    line.AddVertexAt(1, centerPt.Convert2D(0, 1.7941 * scale), 0, 0, 0);
                    line.AddVertexAt(2, centerPt.Convert2D(-2.4148 * scale, 1.147 * scale), 0, 0, 0);
                    line.AddVertexAt(3, centerPt, 0, 0, 0);
                    line.AddVertexAt(4, centerPt.Convert2D(2.4148 * scale, -1.147 * scale), 0, 0, 0);
                    line.AddVertexAt(5, centerPt.Convert2D(0, -1.7941 * scale), 0, 0, 0);
                    line.AddVertexAt(6, endPoint, 0, 0, 0);
                }
                else if (stPoint.Y <= endPoint.Y && stPoint.X == endPoint.X)
                {
                    line.AddVertexAt(0, endPoint, 0, 0, 0);
                    line.AddVertexAt(1, centerPt.Convert2D(0, 1.7941 * scale), 0, 0, 0);
                    line.AddVertexAt(2, centerPt.Convert2D(-2.4148 * scale, 1.147 * scale), 0, 0, 0);
                    line.AddVertexAt(3, centerPt, 0, 0, 0);
                    line.AddVertexAt(4, centerPt.Convert2D(2.4148 * scale, -1.147 * scale), 0, 0, 0);
                    line.AddVertexAt(5, centerPt.Convert2D(0, -1.7941 * scale), 0, 0, 0);
                    line.AddVertexAt(6, stPoint, 0, 0, 0);
                }

                if (stPoint.X > endPoint.X && stPoint.Y == endPoint.Y)
                {
                    line.AddVertexAt(0, stPoint, 0, 0, 0);
                    line.AddVertexAt(1, centerPt.Convert2D(1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(2, centerPt.Convert2D(1.147 * scale, -2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(3, centerPt, 0, 0, 0);
                    line.AddVertexAt(4, centerPt.Convert2D(-1.147 * scale, 2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(5, centerPt.Convert2D(-1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(6, endPoint, 0, 0, 0);
                }
                else if (stPoint.X <= endPoint.X && stPoint.Y == endPoint.Y)
                {
                    line.AddVertexAt(0, endPoint, 0, 0, 0);
                    line.AddVertexAt(1, centerPt.Convert2D(1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(2, centerPt.Convert2D(1.147 * scale, -2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(3, centerPt, 0, 0, 0);
                    line.AddVertexAt(4, centerPt.Convert2D(-1.147 * scale, 2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(5, centerPt.Convert2D(-1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(6, stPoint, 0, 0, 0);
                }
                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);

                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

                #endregion
                tr.Commit();
            }
        }


        public static void CreateUpperAndDownBreakLine(Database db, ref Extents2d ext, Point2d stPoint, Point2d endPoint, double scale = 20)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                #region
                Point2d centerPt = CommonTools.MidPoint(stPoint.Convert3D(), endPoint.Convert3D()).Convert2D();
                //画线
                Polyline line = new Polyline() { Closed = false, Layer = "细线", };//定义不封闭的Polyline 平面虚线
                if (stPoint.X > endPoint.X)
                {
                    line.AddVertexAt(0, stPoint, 0, 0, 0);
                    line.AddVertexAt(1, centerPt.Convert2D(1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(2, centerPt.Convert2D(1.147 * scale, -2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(3, centerPt, 0, 0, 0);
                    line.AddVertexAt(4, centerPt.Convert2D(-1.147 * scale, 2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(5, centerPt.Convert2D(-1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(6, endPoint, 0, 0, 0);
                }
                else
                {
                    line.AddVertexAt(0, endPoint, 0, 0, 0);
                    line.AddVertexAt(1, centerPt.Convert2D(1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(2, centerPt.Convert2D(1.147 * scale, -2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(3, centerPt, 0, 0, 0);
                    line.AddVertexAt(4, centerPt.Convert2D(-1.147 * scale, 2.4148 * scale), 0, 0, 0);
                    line.AddVertexAt(5, centerPt.Convert2D(-1.7941 * scale, 0), 0, 0, 0);
                    line.AddVertexAt(6, stPoint, 0, 0, 0);
                }
                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);

                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

                #endregion
                tr.Commit();
            }
        }

        //public static void CreateBreakLineDim(Database db, ref Extents2d ext, Point2d stPoint, Point2d endPoint, string T1, string T2, string T3, int T4, double scale, double topLineWidth = 0, string tstyle = "En")
        //{
        //    tstyle = "仿宋";
        //    using (Transaction tr = db.TransactionManager.StartTransaction())
        //    {
        //        TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
        //        BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord recorder;
        //        recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        //        #region
        //        Point2d centerPt = CommonTools.MidPoint(stPoint.Convert3D(), endPoint.Convert3D()).Convert2D();
        //        Point2d threeDivPt;
        //        //画线
        //        Polyline line = new Polyline() { Closed = false, Layer = "细线", };//定义不封闭的Polyline 平面虚线
        //        Polyline line1 = new Polyline() { Closed = false, Layer = "细线", };
        //        if (stPoint.Y > endPoint.Y)
        //        {
        //            line.AddVertexAt(0, stPoint, 0, 0, 0);
        //            line.AddVertexAt(1, centerPt.Convert2D(0, 1.7941 * scale), 0, 0, 0);
        //            line.AddVertexAt(2, centerPt.Convert2D(-2.4148 * scale, 1.147 * scale), 0, 0, 0);
        //            line.AddVertexAt(3, centerPt, 0, 0, 0);
        //            line.AddVertexAt(4, centerPt.Convert2D(2.4148 * scale, -1.147 * scale), 0, 0, 0);
        //            line.AddVertexAt(5, centerPt.Convert2D(0, -1.7941 * scale), 0, 0, 0);
        //            line.AddVertexAt(6, endPoint, 0, 0, 0);
        //            threeDivPt = new Point2d((stPoint.X * 2 / 3.0 + endPoint.X / 3.0),
        //                               (stPoint.Y * 2 / 3.0 + endPoint.Y / 3.0));

        //            line1.AddVertexAt(0, stPoint, 0, 0, 0);
        //            line1.AddVertexAt(1, stPoint.Convert2D(topLineWidth, 0), 0, 0, 0);
        //        }
        //        else
        //        {
        //            line.AddVertexAt(0, endPoint, 0, 0, 0);
        //            line.AddVertexAt(1, centerPt.Convert2D(1.7941 * scale, 0), 0, 0, 0);
        //            line.AddVertexAt(2, centerPt.Convert2D(1.147 * scale, -2.4148 * scale), 0, 0, 0);
        //            line.AddVertexAt(3, centerPt, 0, 0, 0);
        //            line.AddVertexAt(4, centerPt.Convert2D(-1.147 * scale, 2.4148 * scale), 0, 0, 0);
        //            line.AddVertexAt(5, centerPt.Convert2D(-1.7941 * scale, 0), 0, 0, 0);
        //            line.AddVertexAt(6, stPoint, 0, 0, 0);
        //            threeDivPt = new Point2d((stPoint.X / 3.0 + endPoint.X * 2 / 3.0),
        //                               (stPoint.Y / 3.0 + endPoint.Y * 2 / 3.0));

        //            line1.AddVertexAt(0, endPoint, 0, 0, 0);
        //            line1.AddVertexAt(1, endPoint.Convert2D(topLineWidth, 0), 0, 0, 0);
        //        }
        //        recorder.AppendEntity(line);
        //        tr.AddNewlyCreatedDBObject(line, true);
        //        recorder.AppendEntity(line1);
        //        tr.AddNewlyCreatedDBObject(line1, true);

        //        ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
        //        ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));

        //        TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_BOTTOM, centerPt.Convert2D(-2.5 * scale, -20), T1, scale, tstyle, Angle.FromDegrees(90).Radians);
        //        DimPloter.CreateLeadWire(db, ref ext, threeDivPt.Convert3D(), T2, T3, T4, Extensions.curFont, scale);
        //        #endregion
        //        tr.Commit();
        //    }
        //}


        /// <summary>
        /// 实心圆
        /// </summary>
        /// <param name="pt">圆点</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public static void CreateSolidCircle(Database db, ref Extents2d ext, Point3d pt, double radius)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Circle circle = new Circle(pt, Vector3d.ZAxis, radius) { Layer = "细线", ColorIndex = 6 };
                circle.SetDatabaseDefaults();//默认参数
                recorder.AppendEntity(circle);
                tr.AddNewlyCreatedDBObject(circle, true);
                ext = ext.Add(new Extents2d(circle.Bounds.Value.MinPoint.Convert2D(), circle.Bounds.Value.MaxPoint.Convert2D()));

                // 添加圆到一个 ObjectID 数组中去 
                ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                acObjIdColl.Add(circle.ObjectId);

                // 创建图案填充对象并添加到块表记录中   
                Hatch hatch = new Hatch();
                hatch.ColorIndex = 6;
                recorder.AppendEntity(hatch);
                tr.AddNewlyCreatedDBObject(hatch, true);
                hatch.SetDatabaseDefaults();
                //hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");//ANSI31为金属剖面线
                hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID"); //设置填充图案
                hatch.Associative = true;
                hatch.AppendLoop(HatchLoopTypes.Outermost, acObjIdColl);
                hatch.EvaluateHatch(true);

                tr.Commit();
            }
        }

        public static void AddLine(Database db, ref Extents2d ext, Point3d stPoint, Point3d endPoint, string layer = "粗线", int colorIndex = 1, double scale = 1)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                Line line = new Line(stPoint, endPoint);
                line.Layer = layer;
                line.ColorIndex = colorIndex;
                line.LinetypeScale = scale;
                modelSpace.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
            }
        }

        public static void CreateLine(Database db, ref Extents2d ext, Point3d stPoint, Point3d endPoint, ref Line line, string layer = "粗线", int colorIndex = 1, double scale = 1)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                line = new Line(stPoint, endPoint);
                line.Layer = layer;
                line.ColorIndex = colorIndex;
                line.LinetypeScale = scale;
                modelSpace.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
            }
        }

        public static void CreateLine(Database db, ref Extents2d ext, Point3d stPoint, Point3d endPoint, string layer = "粗线", int colorIndex = 1, double scale = 1)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                Line line = new Line(stPoint, endPoint);
                line.Layer = layer;
                line.ColorIndex = colorIndex;
                line.LinetypeScale = scale;
                modelSpace.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
            }
        }

        public static void CreateLine(Database db, ref Extents2d ext, Point3d stPoint, Point3d endPoint, out Polyline line, string layer = "粗线", int colorIndex = 1, double scale = 1)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                //line = new Polyline(stPoint, endPoint);
                line = new Polyline() { Layer = layer };//定义不封闭的Polyline

                line.AddVertexAt(0, stPoint.Convert2D(), 0, 0, 0);
                line.AddVertexAt(1, endPoint.Convert2D(), 0, 0, 0);
                line.LineWeight = Autodesk.AutoCAD.DatabaseServices.LineWeight.ByLayer;

                line.Layer = layer;
                line.ColorIndex = colorIndex;
                line.LinetypeScale = scale;
                modelSpace.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
            }
        }

       
    }
}
