using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADInterface.Plotters
{
    public static class TextHelper
    {
        public static double Factor { get { return 1.4; } }
    }

    public class TextPloter
    {
        public enum eTxtLocation
        {
            E_LEFT,
            E_RIGHT,
            E_TOP,
            E_BOTTOM
        }
        /// <summary>
        /// 带比例尺标题
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Point">插入点坐标</param>
        /// <param name="text">文本</param>
        /// <param name="scaleString">比例尺文本</param>
        /// <param name="_scale">比例尺</param>
        /// <param name="tstyle">字体标准</param>
        /// <param name="isModel">是否是模型空间</param>
        public static void PrintScaleText(Database db, Point2d Point, string text, string scaleString,ref Extents2d ext, double _scale = 1, string tstyle ="En", bool isModel = true)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                if (isModel)
                    recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                else
                    recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                MText Mtext = new MText();
                Mtext.TextHeight = 4.0 * _scale;
                Mtext.Contents = text;
                Mtext.TextStyleId = st[tstyle];
                //Mtext.Width = 200 * _scale;

                DBText Title = new DBText()
                {
                    Height = 4.0 * _scale,
                    TextStyleId = st[tstyle],
                    TextString = text,
                    WidthFactor=0.8,
                    Position=Point.Convert3D(0,2*_scale),
                    HorizontalMode = TextHorizontalMode.TextCenter,
                    VerticalMode = TextVerticalMode.TextBase,
                    AlignmentPoint= Point.Convert3D(0, 2 * _scale),                    
                };
                

                
                double width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight) * _scale;
              
                //if (Mtext.ActualWidth / _scale > 80)
                //{
                //    width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight + 3) * _scale;
                //}
                //else if (Mtext.ActualWidth / _scale > 50)
                //{
                //    width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight + 2) * _scale;
                //}
                width = Mtext.ActualWidth;
                Mtext.Location = Point.Convert3D(-width/2, Mtext.TextHeight+2 * _scale);
                recorder.AppendEntity(Title);
                tr.AddNewlyCreatedDBObject(Title, true);

                ext = ext.Add(new Extents2d(Mtext.Bounds.Value.MinPoint.Convert2D(), Mtext.Bounds.Value.MaxPoint.Convert2D()));
                Polyline line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                //line.AddVertexAt(0, Point.Convert2D(-width / 2, 1 * _scale), 0, 4, 4);
                //line.AddVertexAt(1, Point.Convert2D(width/2, 1 * _scale), 0, 4, 4);
                line.AddVertexAt(0, Point.Convert2D(-width / 2, 1 * _scale), 0, 0.5, 0.5);
                line.AddVertexAt(1, Point.Convert2D(width / 2, 1 * _scale), 0, 0.5, 0.5);
                line.LineWeight = LineWeight.LineWeight050;
                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                Extents2d ext0 = new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D());
                Polyline line1 = new Polyline() { Closed = false, Layer = "细线" };//定义不封闭的Polyline 平面虚线
                line1.AddVertexAt(0, Point.Convert2D(-width / 2,  0), 0, 0, 0);
                line1.AddVertexAt(1, Point.Convert2D(width/2, 0), 0, 0, 0);
                line1.LineWeight = LineWeight.LineWeight020;
                recorder.AppendEntity(line1);
                tr.AddNewlyCreatedDBObject(line1, true);

                Extents2d ext1 = new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D());


                DBText num = new DBText();
                num.TextString = scaleString;
                num.Height = 2.5 * _scale;
                num.Position = Point.Convert3D(width/2 + 1 * _scale, 1 * _scale);

                num.HorizontalMode = TextHorizontalMode.TextLeft;
                num.VerticalMode = TextVerticalMode.TextVerticalMid;
                num.AlignmentPoint = num.Position;

                num.TextStyleId = st[tstyle];
                num.WidthFactor = 0.8;
                num.Layer = "标注";
                recorder.AppendEntity(num);
                tr.AddNewlyCreatedDBObject(num, true);
                
               
                ext = ext.Add(ext0);
                ext = ext.Add(ext1);
                if (num.Bounds != null)
                {
                    Extents2d ext2 = new Extents2d(num.Bounds.Value.MinPoint.Convert2D(), num.Bounds.Value.MaxPoint.Convert2D());
                    ext = ext.Add(ext2);
                }
                tr.Commit();
            }
        }
        public static void PrintScaleTextNew(Database db, Point2d Point, string text, string scaleString, ref Extents2d ext, double _scale = 1, string tstyle = "En", bool isModel = true)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                if (isModel)
                    recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                else
                    recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                MText Mtext = new MText();
                Mtext.TextHeight = 4.0 * _scale;
                Mtext.Contents = text;
                Mtext.TextStyleId = st[tstyle];
                //Mtext.Width = 200 * _scale;

                DBText Title = new DBText()
                {
                    Height = 4.0 * _scale,
                    TextStyleId = st[tstyle],
                    TextString = text,
                    WidthFactor = 0.8,
                    Position = Point.Convert3D(0, 2 * _scale),
                    HorizontalMode = TextHorizontalMode.TextCenter,
                    VerticalMode = TextVerticalMode.TextBase,
                    AlignmentPoint = Point.Convert3D(0, 2 * _scale),
                };



                double width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight) * _scale;

                //if (Mtext.ActualWidth / _scale > 80)
                //{
                //    width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight + 3) * _scale;
                //}
                //else if (Mtext.ActualWidth / _scale > 50)
                //{
                //    width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight + 2) * _scale;
                //}
                width = Mtext.ActualWidth;
                Mtext.Location = Point.Convert3D(-width / 2, Mtext.TextHeight + 2 * _scale);
                recorder.AppendEntity(Title);
                tr.AddNewlyCreatedDBObject(Title, true);

                ext = ext.Add(new Extents2d(Mtext.Bounds.Value.MinPoint.Convert2D(), Mtext.Bounds.Value.MaxPoint.Convert2D()));
                ext = ext.Add(new Extents2d(Title.Bounds.Value.MinPoint.Convert2D(), Title.Bounds.Value.MaxPoint.Convert2D()));
                Polyline line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
                line.AddVertexAt(0, Point.Convert2D(-width / 2, 1 * _scale), 0, 0, 0);
                line.AddVertexAt(1, Point.Convert2D(width / 2, 1 * _scale), 0, 0, 0);
                line.LineWeight = LineWeight.LineWeight050;
                recorder.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                Extents2d ext0 = new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D());
                Polyline line1 = new Polyline() { Closed = false, Layer = "细线" };//定义不封闭的Polyline 平面虚线
                line1.AddVertexAt(0, Point.Convert2D(-width / 2, 0), 0, 0, 0);
                line1.AddVertexAt(1, Point.Convert2D(width / 2, 0), 0, 0, 0);
                line1.LineWeight = LineWeight.LineWeight020;
                recorder.AppendEntity(line1);
                tr.AddNewlyCreatedDBObject(line1, true);

                Extents2d ext1 = new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D());


                DBText num = new DBText();
                num.TextString = scaleString;
                num.Height = 2.5 * _scale;
                num.Position = Point.Convert3D(width / 2 + 1 * _scale, 1 * _scale);

                num.HorizontalMode = TextHorizontalMode.TextLeft;
                num.VerticalMode = TextVerticalMode.TextVerticalMid;
                num.AlignmentPoint = num.Position;

                num.TextStyleId = st[tstyle];
                num.WidthFactor = 0.8;
                num.Layer = "标注";
                recorder.AppendEntity(num);
                tr.AddNewlyCreatedDBObject(num, true);


                ext = ext.Add(ext0);
                ext = ext.Add(ext1);
                if (num.Bounds != null)
                {
                    Extents2d ext2 = new Extents2d(num.Bounds.Value.MinPoint.Convert2D(), num.Bounds.Value.MaxPoint.Convert2D());
                    ext = ext.Add(ext2);
                }
                tr.Commit();
            }
        }

        public static DBObjectCollection GetScaleTextDB(Point2d Point, string text, string scaleString, double _scale = 100)
        {
            DBObjectCollection res = new DBObjectCollection();
            //TextStyleTable st;
            //using (Transaction tr = db.TransactionManager.StartTransaction())
            //{
            //    st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
            //}

            MText Mtext = new MText();
            Mtext.TextHeight = 4.0 * _scale;
            Mtext.Contents = text;
            //Mtext.TextStyleId = st[tstyle];
            Mtext.Width = 200 * _scale;

            Mtext.Location = Point.Convert3D();
            double width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight) * _scale;
            if (Mtext.ActualWidth / _scale > 80)
            {
                width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight + 3) * _scale;
            }
            else if (Mtext.ActualWidth / _scale > 50)
            {
                width = (Mtext.ActualWidth * 4 / Mtext.ActualHeight + 2) * _scale;
            }

            res.Add(Mtext);
            Polyline line = new Polyline() { Closed = false, Layer = "粗线" };//定义不封闭的Polyline 平面虚线
            line.AddVertexAt(0, Point.Convert2D(0, -Mtext.TextHeight - 1 * _scale), 0, 0, 0);
            line.AddVertexAt(1, Point.Convert2D(width, -Mtext.TextHeight - 1 * _scale), 0, 0, 0);
            line.LineWeight = LineWeight.LineWeight050;
            res.Add(line);
            Polyline line1 = new Polyline() { Closed = false, Layer = "细线" };//定义不封闭的Polyline 平面虚线
            line1.AddVertexAt(0, Point.Convert2D(0, -Mtext.TextHeight - 2 * _scale), 0, 0, 0);
            line1.AddVertexAt(1, Point.Convert2D(width, -Mtext.TextHeight - 2 * _scale), 0, 0, 0);
            line1.LineWeight = LineWeight.LineWeight020;
            res.Add(line1);

            DBText num = new DBText();
            num.TextString = scaleString;
            num.Height = 2.5 * _scale;
            num.Position = Point.Convert3D(width + 5 * _scale, -4 * _scale);

            num.HorizontalMode = TextHorizontalMode.TextCenter;
            num.VerticalMode = TextVerticalMode.TextVerticalMid;
            num.AlignmentPoint = num.Position;

            //num.TextStyleId = st[tstyle];
            num.WidthFactor = 0.7;
            num.Layer = "标注";
            res.Add(num);

            return res;
        }


        // tyg add 2021-1-26 begin
        public static DBObjectCollection PrintText(Database db,eTxtLocation eLocation, Point2d Point, string text, double _scale, string tstyle = "En", double Rotation = 0)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                //BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                MText Mtext = new MText();
                Mtext.TextHeight = 4.0 * _scale;
                Mtext.Contents = text;
                Mtext.TextStyleId = st[tstyle];
                Mtext.Width = 200 * _scale;
                Mtext.Rotation = Rotation;

                switch (eLocation)
                {
                    case eTxtLocation.E_LEFT:
                        {
                            Mtext.Location = Point.Convert3D(-Mtext.ActualHeight - _scale,-Mtext.ActualWidth/2);
                            break;
                        }
                    case eTxtLocation.E_RIGHT:
                        {
                            Mtext.Location = Point.Convert3D(Mtext.ActualHeight- 2*_scale, -Mtext.ActualWidth / 2);
                            break;
                        }
                    case eTxtLocation.E_TOP:
                        {
                            Mtext.Location = Point.Convert3D(-Mtext.ActualWidth / 2, Mtext.ActualHeight+_scale);
                            break;
                        }
                    case eTxtLocation.E_BOTTOM:
                        {
                            Mtext.Location = Point.Convert3D(-Mtext.ActualWidth / 2, -Mtext.ActualHeight+ 2*_scale);
                            break;
                        }
                    default:
                        break;
                }
                res.Add(Mtext);
//                 recorder.AppendEntity(Mtext);
//                 tr.AddNewlyCreatedDBObject(Mtext, true);
                tr.Commit();
            }

            return res;
        }
        // tyg add 2021-1-26 begin

        public static DBObjectCollection PlotText(Database db,ref Extents2d ext, eTxtLocation eLocation, Point2d Point, string text, double _scale, string tstyle = "En", double Rotation = 0)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //DBText
                MText Mtext = new MText();
                Mtext.TextHeight = 2.5 * _scale;
                Mtext.Contents = text;
                Mtext.TextStyleId = st[tstyle];
                //Mtext.Width = 200 * _scale;
                Mtext.Rotation = Rotation;
                res.Add(Mtext);
                switch (eLocation)
                {
                    case eTxtLocation.E_LEFT:
                        {
                            Mtext.Location = Point.Convert3D(-Mtext.ActualHeight);
                            Mtext.Attachment = AttachmentPoint.TopCenter;
                            break;
                        }
                    case eTxtLocation.E_RIGHT:
                        {
                            Mtext.Location = Point.Convert3D(Mtext.ActualHeight);
                            Mtext.Attachment = AttachmentPoint.BottomCenter;
                            break;
                        }
                    case eTxtLocation.E_TOP:
                        {
                            Mtext.Location = Point.Convert3D(0, Mtext.ActualHeight);
                            Mtext.Attachment = AttachmentPoint.TopCenter;
                            break;
                        }
                    case eTxtLocation.E_BOTTOM:
                        {
                            Mtext.Location = Point.Convert3D(0, -Mtext.ActualHeight);
                            Mtext.Attachment = AttachmentPoint.BottomCenter;
                            break;
                        }
                    default:
                        break;
                }

                recorder.AppendEntity(Mtext);
                tr.AddNewlyCreatedDBObject(Mtext, true);
                ext = ext.Add(new Extents2d(Mtext.Bounds.Value.MinPoint.Convert2D(), Mtext.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
                return res;
            } 
        }

        public static DBObjectCollection PlotText(Database db, ref Extents2d ext,  Point3d Point, string text, double _scale,double height=2, string tstyle = "En", double Rotation = 0)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //DBText
                DBText txt = new DBText();
                txt.Height = height * _scale;
                txt.TextString = text;
                txt.TextStyleId = st[tstyle];
                txt.Position = Point;
                //txt.AlignmentPoint = Point;
                txt.Rotation = Rotation;
                txt.LinetypeScale = 0.7;
                res.Add(txt);
                
                recorder.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                ext = ext.Add(new Extents2d(txt.Bounds.Value.MinPoint.Convert2D(), txt.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
                return res;
            }
        }

        public static void PlotTextNew(Database db, ref Extents2d ext, Point3d Point, string text, double _scale, double height = 2, string tstyle = "En", double Rotation = 0, bool isModel = true)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                if (isModel)
                    recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                else
                    recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                //DBText
                DBText txt = new DBText();
                txt.Height = height * _scale;
                txt.TextString = text;
                txt.TextStyleId = st[tstyle];
                txt.Position = Point;
                //txt.AlignmentPoint = Point;
                txt.Rotation = Rotation;
                txt.LinetypeScale = 0.7;

                recorder.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
				if (isModel)
				{
                	ext = ext.Add(new Extents2d(txt.Bounds.Value.MinPoint.Convert2D(), txt.Bounds.Value.MaxPoint.Convert2D()));
                }
				tr.Commit();

            }
        }
        public static void PlotTextM2P(Database db, ref Extents2d ext, Point3d Point, string text, double _scale, double height = 2, string tstyle = "En", double Rotation = 0, bool isModel=true)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                if(isModel)
                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                else
                    recorder = tr.GetObject(blockTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

                //DBText
                DBText txt = new DBText();
                txt.Height = height * _scale;
                txt.TextString = text;
                txt.TextStyleId = st[tstyle];
                txt.Position = Point;
                //txt.AlignmentPoint = Point;
                txt.Rotation = Rotation;
                txt.LinetypeScale = 0.7;

                recorder.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                if (isModel)
                    ext = ext.Add(new Extents2d(txt.Bounds.Value.MinPoint.Convert2D(), txt.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();

            }
        }

        public static MText GetTextActualWidth(Database db, string text, double scale, double fontSize = 2.5, string tstyle = "En")
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //DBText
                MText Mtext = new MText();
                Mtext.TextHeight = fontSize * scale;
                Mtext.Contents = text;
                Mtext.TextStyleId = st[tstyle];
                //Mtext.Width = 200 * _scale;

                return Mtext;
            }
        }

        public static void PrintMultiText(Database db, ref Extents2d ext, Point3d pt, string text, double scale,
            double fontSize = 2.5,double width=50,string tstyle = "En")
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                MText Mtext = new MText();
                Mtext.TextHeight = fontSize * scale;
                Mtext.Contents = text;
                Mtext.TextStyleId = st[tstyle];
                Mtext.Location = pt.Convert3D(0/*Mtext.ActualWidth/2*/, -Mtext.ActualHeight / 2);
                Mtext.Attachment = AttachmentPoint.MiddleLeft;
                Mtext.Width = width * scale;

                recorder.AppendEntity(Mtext);
                tr.AddNewlyCreatedDBObject(Mtext, true);
                ext = ext.Add(new Extents2d(Mtext.Bounds.Value.MinPoint.Convert2D(), Mtext.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
            }
        }


        public static void PlotTextWithLine(Database db, ref Extents2d ext, Point3d cc, 
            string text, double scale, double height, string tstyle = "En", double Rotation = 0)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;
                TextStyleTableRecord str = tr.GetObject(st[tstyle], OpenMode.ForRead) as TextStyleTableRecord;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                DBText title = new DBText()
                {
                    Layer = "标注",
                    Height = height * scale,
                    TextString = text,
                    TextStyleId = st[tstyle],
                    HorizontalMode = TextHorizontalMode.TextCenter,
                    VerticalMode = TextVerticalMode.TextBase,
                    Position = cc.Convert3D(0, 1 * scale),
                    AlignmentPoint = cc.Convert3D(0, 1 * scale),
                    WidthFactor = str.XScale,
                };

                recorder.AppendEntity(title);
                tr.AddNewlyCreatedDBObject(title, true);

                // line
                double width = title.GetWidth();
               
                Polyline line1 = new Polyline() { Closed = false, Layer = "标注" };
                line1.AddVertexAt(0, cc.Convert2D(-width/2, 0.5*scale), 0,0,0);
                line1.AddVertexAt(1, cc.Convert2D(width/2, 0.5*scale), 0, 0, 0);
                line1.LineWeight = LineWeight.LineWeight030;
                recorder.AppendEntity(line1);
                tr.AddNewlyCreatedDBObject(line1, true);

                Polyline line2 = new Polyline() { Closed = false, Layer = "标注" };
                line2.AddVertexAt(0, cc.Convert2D(-width / 2, 0), 0, 0, 0);
                line2.AddVertexAt(1, cc.Convert2D(width / 2, 0), 0, 0, 0);
                line2.LineWeight = LineWeight.LineWeight009;
                recorder.AppendEntity(line2);
                tr.AddNewlyCreatedDBObject(line2, true);

                var h = title.GetHeight();
                Point2d min = cc.Convert2D(width * -0.5);
                Point2d max = cc.Convert2D(width * 0.5, h + 2 * scale);
                ext = ext.Add(new Extents2d(min,max));
                tr.Commit();

            }
        }
    }
}

