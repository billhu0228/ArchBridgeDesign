using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using MathNet.Spatial.Units;

namespace CADInterface.Plotters
{
    public enum ArrowDirection
    {
        East,//东方      右
        West, //西方     左
        North,// 北方   上
        South, //南方    下
    }

    public enum eDirection
    {
        E_LEFT,
        E_RIGHT,
        E_TOP,
        E_BOTTOM
    }

    public enum eArrow
    {
        E_ARROW_NULL,                   // 无箭头
        E_ARROW_LEFT_ARROW,             // 左边箭头
        E_ARROW_RIGHT_ARROW,            // 左边箭头
        E_ARROW_DOUBLE_SIDE_ARROW,      // 双向箭头
    }
    public class DimStyleTool
    {

        /// <summary>
        /// 判断样式是否存在
        /// </summary>
        /// <param name="DimStyleName"></param>
        /// <returns></returns>
        public static bool CheckDimStyleExists(string DimStyleName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                DimStyleTableRecord dstr = new DimStyleTableRecord();
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead, true);
                if (dst.Has(DimStyleName))
                    return true;

                return false;
            }
        }


        /// <summary>
        /// 设置当前标注样式
        /// </summary>
        /// <param name="DimStyleName"></param>
        public static void SetDimStyleCurrent(string DimStyleName)
        {
            // Establish connections to the document and its database
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            // Establish a transaction
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                ObjectId dimId = ObjectId.Null;

                string message = string.Empty;
                if (!dst.Has(DimStyleName))
                {
                    throw new Exception("无此样式");
                }
                else
                    dimId = dst[DimStyleName];

                DimStyleTableRecord dstr = (DimStyleTableRecord)tr.GetObject(dimId, OpenMode.ForRead);

                /* NOTE:
                 * If this code is used, and the updated style is current,
                 * an override is created for that style.
                 * This is not what I wanted.
                 */
                //if (dstr.ObjectId != db.Dimstyle)
                //{
                //    db.Dimstyle = dstr.ObjectId;
                //    db.SetDimstyleData(dstr);
                //}

                /* Simply by running these two lines all the time, any overrides to updated dimstyles get 
                 * cleared away as happens when you select the parent dimstyle in AutoCAD.
                 */
                db.Dimstyle = dstr.ObjectId;
                db.SetDimstyleData(dstr);

                tr.Commit();
            }
        }


        /// <summary>
        /// 获取样式id
        /// </summary>
        /// <param name="LineStyleName"></param>
        /// <returns></returns>
        public static ObjectId GetLinestyleID(string LineStyleName)
        {
            ObjectId result = ObjectId.Null;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction tr = db.TransactionManager.StartTransaction();
            using (tr)
            {
                LinetypeTable ltt = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                result = ltt[LineStyleName];
                tr.Commit();
            }

            return result;
        }
        public static bool CheckTextStyle(string TextStyleName)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            // Ensure that the MR_ROMANS text style exists
            using (Transaction AcTrans = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
            {
                TextStyleTableRecord tstr = new TextStyleTableRecord();
                TextStyleTable tst = (TextStyleTable)AcTrans.GetObject(acDb.TextStyleTableId, OpenMode.ForRead, true, true);

                if (tst.Has(TextStyleName) == true)
                    //if (tst.Has(tst[TextStyleName]) == true)
                    return true;
                return false;
            }
        }



        public static bool CheckLinestyleExists(string LineStyleName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                //LinetypeTableRecord lttr = new LinetypeTableRecord();
                LinetypeTable ltt = (LinetypeTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead, true);
                if (ltt.Has(LineStyleName))
                    return true;

                return false;
            }
        }
        public static void LoadLinetypes(string LinFile, string LinType)
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Linetype table for read
                LinetypeTable acLineTypTbl;
                acLineTypTbl = acTrans.GetObject(acCurDb.LinetypeTableId,
                    OpenMode.ForRead) as LinetypeTable;

                if (acLineTypTbl.Has(LinType) == false)
                {
                    // Load the requested Linetype
                    acCurDb.LoadLineTypeFile(LinType, LinFile);
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        public static ObjectId GetArrowObjectId(string newArrowName)
        {
            ObjectId result = ObjectId.Null;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            string oldArrowName = Application.GetSystemVariable("DIMBLK").ToString();
            Application.SetSystemVariable("DIMBLK", newArrowName);
            if (oldArrowName.Length != 0)
                Application.SetSystemVariable("DIMBLK", oldArrowName);

            Transaction tr = db.TransactionManager.StartTransaction();
            using (tr)
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                result = bt[newArrowName];
                tr.Commit();
            }

            return result;
        }
    }

    public class DimPloter
    {

        public static string GetScaleName(double thescale)
        {
            string scname;
            if (thescale < 1)
            {
                if (1%thescale!=0)
                {
                    throw new Exception("比例不可用");

                }
                scname = Math.Round(1 / thescale, 0).ToString() + "-1";
            }
            else
            {
                scname = "1-" + thescale.ToString();
            }

            return scname;
        }

        /// <summary>
        /// 绘制标高符号
        /// </summary>
        /// <param name="bgdata">标高数据</param>
        /// <param name="refpt">标高点</param>
        /// <param name="ms"></param>
        /// <param name="tr"></param>
        /// <param name="blockTbl"></param>
        /// <param name="s"></param>
        public static void BiaoGao(Database db, ref Extents2d ext, double bgdata, Point3d refpt, double s = 100)
        {
            double factor = s / 100;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                ObjectId blkRecId = blockTbl["KYBG"];
                BlockReference acBlkRef = new BlockReference(refpt, blkRecId);
                //acBlkRef.SetAttributes();
                acBlkRef.ScaleFactors = new Scale3d(factor);
                acBlkRef.Layer = "标注";
                modelSpace.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
                BlockTableRecord zheshiyuankuai;
                zheshiyuankuai = tr.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId gezhongshuxingID in zheshiyuankuai)
                {
                    DBObject gezhongshuxing = tr.GetObject(gezhongshuxingID, OpenMode.ForRead) as DBObject;
                    if (gezhongshuxing is AttributeDefinition)
                    {
                        AttributeDefinition acAtt = gezhongshuxing as AttributeDefinition;
                        using (AttributeReference acAttRef = new AttributeReference())
                        {
                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);
                            acAttRef.TextString = string.Format("{0:f3}", bgdata);
                            //acAttRef.Height = acAttRef.Height * factor;
                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);

                            tr.AddNewlyCreatedDBObject(acAttRef, true);
                            ext = ext.Add(new Extents2d(acBlkRef.Bounds.Value.MinPoint.Convert2D(), acBlkRef.Bounds.Value.MaxPoint.Convert2D()));
                        }
                    }
                }
                tr.Commit();
            }
        }


        /// <summary>
        /// 对齐标注
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="Pref"></param>
        /// <param name="dimID"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static AlignedDimension DimAli(Database db, ref Extents2d ext, Point3d P1, Point3d P2, Point3d Pref, double scale = 20, string layerName = "标注")
        {
            AlignedDimension AD1;


            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                var dimID = dst[string.Format("1-{0:G}", scale)];
                AD1 = new AlignedDimension(P1, P2, Pref, "", dimID);
                AD1.Layer = layerName;
                modelSpace.AppendEntity(AD1);
                tr.AddNewlyCreatedDBObject(AD1, true);
                Polyline line = new Polyline();
                line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                line.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                line.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                if (line.Bounds != null)
                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                tr.Commit();
            }
            return AD1;
        }


        /// <summary>
        /// 水平标注
        /// </summary>
        /// <param name="db"></param>
        /// <param name="P1">起点</param>
        /// <param name="P2">终点</param>
        /// <param name="Pref">标注位置</param>
        /// <param name="dimID">标注样式id</param>
        /// <param name="ang">转角，弧度</param>
        /// <returns></returns>
        public static RotatedDimension DimRotated(Database db, ref Extents2d ext, Point3d P1, Point3d P2, Point3d Pref
            , double ang = 0, double scale = 20, string replaceText = "")
        {

            RotatedDimension D1;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);

                string st = GetScaleName(scale);

                var dimID = dst[st];

                D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P2, Pref, replaceText, dimID);
                D1.Layer = "标注";                
                modelSpace.AppendEntity(D1);
                tr.AddNewlyCreatedDBObject(D1, true);
                Polyline line = new Polyline();
                line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                line.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                line.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                if (line.Bounds != null)
                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
            return D1;

        }
        public static void AddListDimRotated(Database db, ref Extents2d ext,
            Point3d Pref, List<Point3d> npts, int scale, double ang = 0, int pN = 1, string unit = "mm")
        {

            RotatedDimension D1;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                var dimID = dst[string.Format("1-{0:G}", scale)];

                if (npts.Count > 1)
                {
                    if (pN == 1)
                    {
                        for (int i = 0; i < npts.Count - 1; i++)
                        {
                            Point3d P1 = npts[i];
                            Point3d P2 = npts[i + 1];
                            //Point3d Pref = GeTools.MidPoint(P1, P2).Convert3D(3, 3);
                            D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P2, Pref, "", dimID);
                            D1.Layer = "标注";
                            if (unit == "cm")
                            {
                                string replaceText = (Math.Round(D1.Measurement / 10, 1)).ToString();
                                D1.DimensionText = replaceText;
                            }
                            else if (unit == "m")
                            {
                                string replaceText = (Math.Round(D1.Measurement / 1000, 3)).ToString();
                                D1.DimensionText = replaceText;
                            }
                            modelSpace.AppendEntity(D1);
                            tr.AddNewlyCreatedDBObject(D1, true);
                            Polyline line = new Polyline();
                            line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            line.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            if (line.Bounds != null)
                                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                        }
                    }
                    else
                    {
                        if (npts.Count == 2)
                        {
                            Point3d P1 = npts[0];
                            Point3d P2 = npts[1];
                            //Point3d Pref = GeTools.MidPoint(P1, P2).Convert3D(0, 3);
                            D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P2, Pref, "", dimID);
                            D1.Layer = "标注";
                            string replaceText = (D1.Measurement / pN) + "×" + pN;
                            D1.DimensionText = replaceText;
                            if (unit == "cm")
                            {
                                replaceText = (D1.Measurement / pN) + "×" + pN / 10;
                                D1.DimensionText = replaceText;
                            }
                            else if (unit == "m")
                            {
                                replaceText = (D1.Measurement / pN) + "×" + pN / 1000;
                                D1.DimensionText = replaceText;
                            }
                            modelSpace.AppendEntity(D1);
                            tr.AddNewlyCreatedDBObject(D1, true);
                            //if( D1.Bounds !=null)
                            //ext = ext.Add(new Extents2d(D1.Bounds.Value.MinPoint.Convert2D(), D1.Bounds.Value.MaxPoint.Convert2D()));
                            //Line line = new Line(P1.Convert3D(18 * scale, 18 * scale), P2.Convert3D(-18 * scale, -18 * scale));
                            Polyline line = new Polyline();
                            line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            line.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            if (line.Bounds != null)
                                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                        }
                    }
                }
                tr.Commit();
            }

        }


        public static void AddListDimAligned(Database db, ref Extents2d ext, List<Point3d> npts, int scale,double dist)
        {
            Polyline line = new Polyline() { Closed = false };//定义不封闭的Polyline
            for (int i = 0; i < npts.Count; i++)
            {
                line.AddVertexAt(i, npts[i].Convert2D(), 0, 0, 0);
            }

            Point3d pt1, pt2, pt3 ;
            for (int i = 0; i < npts.Count-1; i++)
            {
                pt1 = npts[i];
                pt2 = npts[i + 1];
                var vec = (pt2 - pt1).GetNormal();
                pt3 =pt1+ vec.RotateBy(Angle.FromDegrees(90).Radians, Vector3d.ZAxis)*dist;
                var dim = DimAli(db, ref ext, pt1, pt2, pt3, scale);
            }



        }



        /// <summary>
        /// 承台双层标注
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="Pref">参考点坐标</param>
        /// <param name="PrefMid">中点坐标</param>
        /// <param name="npts">点集合</param>
        /// <param name="scale"></param>
        /// <param name="ang"></param>
        /// <param name="pL"></param>
        /// <param name="firstDimNum">刚开始有多少个标注</param>
        /// <param name="isLeft">是否左侧标注</param>
        public static void AddDoubleListDivHalfDimRotated(Database db, ref Extents2d ext,
         Point3d Pref, Point3d PrefMid, List<Point3d> npts, int scale, double ang = 0, int pL = 100, int firstDimNum = 2, bool isLeft = true)
        {

            RotatedDimension D1;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                var dimID = dst[string.Format("1-{0:G}", scale)];
                Point3d ptSt = new Point3d();
                Point3d ptEnd = new Point3d();
                if (npts.Count == 2)
                {
                    Point3d P1 = npts[0];
                    Point3d P2 = npts[1];
                    ptSt = P1;
                    ptEnd = P2;
                    D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P2, Pref, "", dimID);
                    D1.Layer = "标注";
                    modelSpace.AppendEntity(D1);
                    tr.AddNewlyCreatedDBObject(D1, true);
                    Polyline line = new Polyline();
                    line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                    line.AddVertexAt(1, ptSt.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(2, ptEnd.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                    if (line.Bounds != null)
                        ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                }
                else
                {

                    for (int i = 0; i < npts.Count - 1; i++)
                    {
                        Point3d P1 = npts[i];
                        Point3d P2 = npts[i + 1];

                        if (i == 0)
                        {
                            ptSt = P1;
                        }
                        if (i == npts.Count - 2)
                        {
                            ptEnd = P2;
                        }
                        if (i < firstDimNum)  //开始有几个标注
                        {
                            D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P2, Pref, "", dimID);
                            D1.Layer = "标注";
                            modelSpace.AppendEntity(D1);
                            tr.AddNewlyCreatedDBObject(D1, true);
                            Polyline line1 = new Polyline();
                            line1.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line1.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            line1.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line1.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            if (line1.Bounds != null)
                                ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));
                        }
                        else
                        {
                            if (PrefMid.X != npts[npts.Count - 2].X)
                            {
                                if (i == npts.Count - 2)
                                {

                                    D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P2, Pref, "", dimID);
                                    D1.Layer = "标注";
                                    modelSpace.AppendEntity(D1);
                                    tr.AddNewlyCreatedDBObject(D1, true);
                                    Polyline line1 = new Polyline();
                                    line1.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                                    line1.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                                    line1.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                                    line1.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                                    if (line1.Bounds != null)
                                        ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));
                                }
                                else
                                {
                                    if (i == firstDimNum)
                                    {
                                        Point3d P = npts[npts.Count - 2];
                                        D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P, Pref, "", dimID);
                                        D1.Layer = "标注";
                                        if (pL > 1)
                                        {
                                            string replaceText = (D1.Measurement / pL) + "×" + pL;
                                            D1.DimensionText = replaceText;
                                        }
                                        modelSpace.AppendEntity(D1);
                                        tr.AddNewlyCreatedDBObject(D1, true);
                                        Polyline line1 = new Polyline();
                                        line1.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                                        line1.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                                        line1.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                                        line1.AddVertexAt(2, P.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                                        if (line1.Bounds != null)
                                            ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));
                                    }
                                }
                            }
                            else
                            {
                                if (i == firstDimNum)
                                {
                                    Point3d P = npts[npts.Count - 2];
                                    D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P, Pref, "", dimID);
                                    D1.Layer = "标注";
                                    if (pL > 1)
                                    {
                                        string replaceText = (D1.Measurement / pL) + "×" + pL;
                                        D1.DimensionText = replaceText;
                                    }
                                    modelSpace.AppendEntity(D1);
                                    tr.AddNewlyCreatedDBObject(D1, true);
                                    Polyline line1 = new Polyline();
                                    line1.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                                    line1.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                                    line1.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                                    line1.AddVertexAt(2, P.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                                    if (line1.Bounds != null)
                                        ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));
                                }
                            }
                        }
                    }
                    D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, ptSt, ptEnd, Pref.Convert3D(5 * scale, 5 * scale), "", dimID);
                    D1.Layer = "标注";
                    D1.DimensionText = (D1.Measurement) * 2 + "/2";
                    modelSpace.AppendEntity(D1);
                    Polyline line = new Polyline();
                    line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                    line.AddVertexAt(1, ptSt.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(2, ptEnd.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                    if (line.Bounds != null)
                        ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                    tr.AddNewlyCreatedDBObject(D1, true);
                }


                tr.Commit();
            }

        }

        public static void DimAliList(Database db, ref Extents2d ext, List<Point3d> PL, ObjectId dimID, int pN = 1, string layerName = "标注")
        {
            AlignedDimension D1;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                if (PL.Count > 1)
                {
                    if (pN == 1)
                    {
                        for (int i = 0; i < PL.Count - 1; i++)
                        {
                            Point3d P1 = PL[i];
                            Point3d P2 = PL[i + 1];
                            Point3d Pref = CommonTools.MidPoint(P1, P2).Convert3D(3, 3);
                            D1 = new AlignedDimension(P1, P2, Pref, "", dimID);
                            D1.Layer = "标注";
                            modelSpace.AppendEntity(D1);
                            tr.AddNewlyCreatedDBObject(D1, true);
                            if (D1.Bounds != null)
                                ext = ext.Add(new Extents2d(D1.Bounds.Value.MinPoint.Convert2D(), D1.Bounds.Value.MaxPoint.Convert2D()));
                        }
                    }
                    else
                    {
                        if (PL.Count == 2)
                        {
                            Point3d P1 = PL[0];
                            Point3d P2 = PL[1];
                            Point3d Pref = CommonTools.MidPoint(P1, P2).Convert3D(0, 3);
                            D1 = new AlignedDimension(P1, P2, Pref, "", dimID);
                            D1.Layer = "标注";
                            string replaceText = (D1.Measurement / pN) + "×" + pN;
                            D1.DimensionText = replaceText;
                            modelSpace.AppendEntity(D1);
                            tr.AddNewlyCreatedDBObject(D1, true);
                            if (D1.Bounds != null)
                                ext = ext.Add(new Extents2d(D1.Bounds.Value.MinPoint.Convert2D(), D1.Bounds.Value.MaxPoint.Convert2D()));
                        }
                    }


                    tr.Commit();
                }
            }
        }

        /// <summary>
        /// 绘制引线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point">引线起点坐标</param>
        /// <param name="T1">引线上部参数</param>
        /// <param name="T2">引线下部参数</param>
        /// <param name="T3">圆内参数</param>
        /// <param name="width">第二点距起点X轴距离</param>
        /// <param name="height">第二点距起点Y轴距离</param>
        /// <param name="Rotation">旋转角度</param>
        /// <param name="tstyle">样式</param>
        /// <param name="scale">比例</param>
        /// <returns></returns>
        public static void RebarWire(Database db, ref Extents2d ext, Point2d point, string T1, string T2, int T3,
           double Rotation, string tstyle = "En", double scale = 100)
        {
            tstyle = "仿宋";
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 第一条线
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                Point2d point1 = point;
                Point2d point2 = point.Convert2D(2 * scale, 5 * scale);
                Point2d point3 = point2.Convert2D(15 * scale);
                Line.AddVertexAt(0, point1, 0, 0, 0);
                Line.AddVertexAt(1, point2, 0, 0, 0);
                Line.AddVertexAt(2, point3, 0, 0, 0);

                ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(Line);

                // 圆内文字
                DBObjectCollection res3 = CircularMark(db, ref ext, point3.Convert3D(), T3.ToString(), tstyle, scale);

                // top 文字
                Point2d pt = new Point2d(point2.X + 15 * scale / 2, point2.Y + scale);
                DBObjectCollection res1 = TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T1, scale, tstyle,0);

                // bottom 文字
                pt = new Point2d(point2.X + 15 * scale / 2, point2.Y - scale);
                DBObjectCollection res2 = TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_BOTTOM, pt, T2, scale, tstyle,0);

                var TX1 = Matrix3d.Rotation(Angle.FromDegrees(Rotation).Radians, Vector3d.ZAxis, point.Convert3D());
                foreach (DBObject item in res)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);

                    recorder.AppendEntity((Entity)item);
                    tr.AddNewlyCreatedDBObject(item, true);
                }

                foreach (DBObject item in res1)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }

                foreach (DBObject item in res2)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }

                foreach (DBObject item in res3)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }
                tr.Commit();
            }
        }
        /// <summary>
        /// 添加引线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point"></param>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <param name="T3"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="Rotation"></param>
        /// <param name="tstyle"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static void CreateLeadWire(Database db, ref Extents2d ext, Point3d point, string T1, string T2, int T3,
        string tstyle = "En", double scale = 100)
        {
            tstyle = "仿宋";
            double cR = 2.5;
            DBText txt = new DBText();
            Circle C2 = new Circle();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 第一条线
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                Point2d point1 = point.Convert2D();
                Point2d point2 = point.Convert2D(5 * scale, 5 * scale);
                Point2d point3 = point2.Convert2D(15 * scale);
                Line.AddVertexAt(0, point1, 0, 0, 0);
                Line.AddVertexAt(1, point2, 0, 0, 0);
                Line.AddVertexAt(2, point3, 0, 0, 0);
                recorder.AppendEntity(Line);
                tr.AddNewlyCreatedDBObject(Line, true);
                ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));

                txt.TextString = T3.ToString();
                txt.Height = 2.5 * scale;
                txt.Position = point3.Convert3D(cR * scale);
                txt.HorizontalMode = TextHorizontalMode.TextCenter;
                txt.VerticalMode = TextVerticalMode.TextVerticalMid;
                txt.AlignmentPoint = point3.Convert3D(cR * scale);
                txt.TextStyleId = st["仿宋"];
                txt.Layer = "标注";
                txt.WidthFactor = 0.8;

                C2 = new Circle(point3.Convert3D(cR * scale), Vector3d.ZAxis, cR * scale);
                C2.Layer = "标注";
                recorder.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                recorder.AppendEntity(C2);
                tr.AddNewlyCreatedDBObject(C2, true);
                ext = ext.Add(new Extents2d(txt.Bounds.Value.MinPoint.Convert2D(), txt.Bounds.Value.MaxPoint.Convert2D()));
                ext = ext.Add(new Extents2d(C2.Bounds.Value.MinPoint.Convert2D(), C2.Bounds.Value.MaxPoint.Convert2D()));

                // top 文字
                Point2d pt = new Point2d(point2.X + 15 * scale / 2, point2.Y);
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T1, scale, tstyle, 0);

                // bottom 文字
                pt = new Point2d(point2.X + 15 * scale / 2, point2.Y);
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_BOTTOM, pt, T2, scale, tstyle,0);
                tr.Commit();
            }

        }

        /// <summary>
        /// 不带圆形标记的引线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="point"></param>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <param name="tstyle"></param>
        /// <param name="scale"></param>
        public static void CreateLeadWireWithoutCircleMark(Database db, ref Extents2d ext, Point3d point, string T1, string T2,
        string tstyle = "En", double scale = 100)
        {
            tstyle = "仿宋";
            double cR = 2.5;
            DBText txt = new DBText();
            Circle C2 = new Circle();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder;

                recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 第一条线
                double width1 = 0;
                MText txt1 = TextPloter.GetTextActualWidth(db, T1, scale, 2.5, "仿宋");
                width1 = txt1.ActualWidth;
                double width2 = 0;
                MText txt2 = TextPloter.GetTextActualWidth(db, T2, scale, 2.5, "仿宋");
                width2 = txt2.ActualWidth;
                double lineWidth = width1 > width2 ? width1 : width2;
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                Point2d point1 = point.Convert2D();
                Point2d point2 = point.Convert2D(lineWidth / 6, lineWidth / 2);
                Point2d point3 = point2.Convert2D(lineWidth);
                Line.AddVertexAt(0, point1, 0, 0, 0);
                Line.AddVertexAt(1, point2, 0, 0, 0);
                Line.AddVertexAt(2, point3, 0, 0, 0);
                recorder.AppendEntity(Line);
                tr.AddNewlyCreatedDBObject(Line, true);
                ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));

                // top 文字
                Point2d pt = new Point2d(point2.X + lineWidth / 2, point2.Y);
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T1, scale, tstyle,0);

                // bottom 文字
                pt = new Point2d(point2.X + lineWidth / 2, point2.Y);
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_BOTTOM, pt, T2, scale, tstyle, 0);
                tr.Commit();
            }

        }

        /// <summary>
        /// 绘制上下标记符号
        /// </summary>
        /// <param name="bJdata">标记信息</param>
        /// <param name="refpt">标记点</param>
        /// <param name="ms"></param>
        /// <param name="tr"></param>
        /// <param name="blockTbl"></param>
        /// <param name="s"></param>
        public static void BiaoJi(string bJdata, Point3d refpt, BlockTableRecord ms, Transaction tr, BlockTable blockTbl, double s = 100, double textHeight = 2.5, bool isTop = true)
        {
            ObjectId blkRecId = blockTbl["TP"];
            if (!isTop)
                blkRecId = blockTbl["BP"];
            double factor = s / 100;
            using (BlockReference acBlkRef = new BlockReference(refpt, blkRecId))
            {
                //acBlkRef.SetAttributes();
                acBlkRef.ScaleFactors = new Scale3d(factor);
                acBlkRef.Layer = "标注";
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
                BlockTableRecord zheshiyuankuai;
                zheshiyuankuai = tr.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId gezhongshuxingID in zheshiyuankuai)
                {
                    DBObject gezhongshuxing = tr.GetObject(gezhongshuxingID, OpenMode.ForRead) as DBObject;
                    if (gezhongshuxing is AttributeDefinition)
                    {
                        AttributeDefinition acAtt = gezhongshuxing as AttributeDefinition;
                        using (AttributeReference acAttRef = new AttributeReference())
                        {
                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);
                            acAttRef.TextString = bJdata;
                            acAttRef.Height = 2;
                            acAttRef.Height = textHeight * s;
                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);

                            tr.AddNewlyCreatedDBObject(acAttRef, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   绘制左右标记符号
        /// </summary>
        /// <param name="bJdata">标记信息</param>
        /// <param name="refpt">标记点</param>
        /// <param name="ms"></param>
        /// <param name="tr"></param>
        /// <param name="blockTbl"></param>
        /// <param name="s"></param>
        /// <param name="textHeight"></param>
        /// <param name="isLeft"></param>
        public static void ZYBiaoJi(string bJdata, Point3d refpt, BlockTableRecord ms, Transaction tr, BlockTable blockTbl, double s = 100, double textHeight = 2.5, bool isLeft = true)
        {
            ObjectId blkRecId = blockTbl["ZB"];
            if (!isLeft)
                blkRecId = blockTbl["YB"];
            double factor = s / 100;
            using (BlockReference acBlkRef = new BlockReference(refpt, blkRecId))
            {
                //acBlkRef.SetAttributes();
                acBlkRef.ScaleFactors = new Scale3d(factor);
                acBlkRef.Layer = "标注";
                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);
                BlockTableRecord zheshiyuankuai;
                zheshiyuankuai = tr.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId gezhongshuxingID in zheshiyuankuai)
                {
                    DBObject gezhongshuxing = tr.GetObject(gezhongshuxingID, OpenMode.ForRead) as DBObject;
                    if (gezhongshuxing is AttributeDefinition)
                    {
                        AttributeDefinition acAtt = gezhongshuxing as AttributeDefinition;
                        using (AttributeReference acAttRef = new AttributeReference())
                        {
                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);
                            acAttRef.TextString = bJdata;
                            acAttRef.Height = 2;
                            acAttRef.Height = textHeight * s;
                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);

                            tr.AddNewlyCreatedDBObject(acAttRef, true);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 添加标注
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="Pref"></param>
        /// <param name="dimID"></param>
        /// <param name="ang"></param>
        /// <param name="replaceText"></param>
        /// <returns></returns>
        public static RotatedDimension AddRotDim(Database db, ref Extents2d ext, Point3d P1, Point3d P2, Point3d Pref
            , double scale, double ang = 0, string unit = "mm", string replaceText = "", string D = "")
        {

            ObjectId dimID = DimPloter.GetDimStyle(db, (int)scale);
            RotatedDimension D1 = new RotatedDimension(Angle.FromDegrees(ang).Radians, P1, P2, Pref, D + replaceText, dimID);
            D1.Layer = "标注";
            if (unit == "cm")
            {
                replaceText = (Math.Round(D1.Measurement / 10, 1)).ToString();
                D1.DimensionText = D + replaceText;
            }
            else if (unit == "m")
            {
                replaceText = (Math.Round(D1.Measurement / 1000, 3)).ToString();
                D1.DimensionText = D + replaceText;
            }
            Polyline line = new Polyline();
            line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
            line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
            line.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
            line.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
            if (line.Bounds != null)
                ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

            return D1;

        }

        /// <summary>
        /// 获取标注样式
        /// </summary>
        /// <param name="db"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static ObjectId GetDimStyle(Database db, int scale)
        {
            Transaction tr = db.TransactionManager.StartTransaction();
            BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                OpenMode.ForWrite) as BlockTableRecord;
            DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
            var DimStyleID = dst["1-" + scale.ToString()];
            tr.Commit();
            return DimStyleID;
        }

        /// <summary>
        /// 圆形文字标记
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="point"></param>
        /// <param name="txt"></param>
        /// <param name="tstyle"></param>
        /// <param name="scale"></param>
        public static DBObjectCollection CircularMark(Database db, ref Extents2d ext, Point3d point, string txt, string tstyle = "En", double scale = 100, double Rotation = 0)
        {
            tstyle = "仿宋";
            double cr = 2.5;
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // 圆内文字
                DBText Mtext = new DBText() { Layer = "标注", ColorIndex = 4 };
                Mtext.Height = 2.5 * scale;
                Mtext.TextString = txt.ToString();
                Mtext.TextStyleId = st[tstyle];
                Mtext.WidthFactor = 0.8;
                Mtext.Rotation = Rotation;
                cr = Mtext.Height;
                Mtext.HorizontalMode = TextHorizontalMode.TextCenter;
                Mtext.VerticalMode = TextVerticalMode.TextVerticalMid;
                Mtext.AlignmentPoint = point.Convert3D(cr);
                Mtext.Position = point.Convert3D(cr);
                ext = ext.Add(new Extents2d(Mtext.Bounds.Value.MinPoint.Convert2D(), Mtext.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(Mtext);

                recorder.AppendEntity((Entity)Mtext);
                tr.AddNewlyCreatedDBObject(Mtext, true);

                // 圆
                Circle a = new Circle(point.Convert3D(cr), Vector3d.ZAxis, cr);
                a.ColorIndex = 4;
                ext = ext.Add(new Extents2d(a.Bounds.Value.MinPoint.Convert2D(), a.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(a);
                recorder.AppendEntity((Entity)a);
                tr.AddNewlyCreatedDBObject(a, true);

                tr.Commit();
            }

            return res;
        }

        /// <summary>
        /// 带箭头的直线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="StartpPint"></param>
        /// <param name="lineLength"></param>
        /// <param name="scale"></param>
        /// <param name="Rotation"></param>
        public static DBObjectCollection ArrowLine(Database db, ref Extents2d ext,
             ArrowDirection eDir, Point3d StartpPint, double lineLength, double Rotation = 0, double scale = 80)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 绘制箭头
                Point2d pt1, pt2, pt3, pt4, pt5;
                switch (eDir)
                {
                    case ArrowDirection.North:  //上
                        {
                            // 直线
                            pt1 = StartpPint.Convert2D();
                            pt2 = StartpPint.Convert2D(0, lineLength - 2 * scale);

                            Polyline Line = new Polyline() { Closed = false, Layer = "粗线" };
                            Line.AddVertexAt(0, pt1, 0, 0, 0);
                            Line.AddVertexAt(1, pt2, 0, 0, 0);
                            ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(Line);

                            // 箭头
                            pt3 = pt2.Convert2D(0, 2 * scale);
                            pt4 = pt2.Convert2D(0.5 * scale, 2 * scale);
                            pt5 = pt2.Convert2D(-0.5 * scale, 2 * scale);

                            Solid s1 = new Solid(pt3.Convert3D(), pt4.Convert3D(), pt5.Convert3D()) { Layer = "粗线" };
                            ext = ext.Add(new Extents2d(s1.Bounds.Value.MinPoint.Convert2D(), s1.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(s1);
                            break;
                        }
                    case ArrowDirection.South:  //下
                        {
                            // 直线
                            pt1 = StartpPint.Convert2D();
                            pt2 = StartpPint.Convert2D(0, -lineLength + 2 * scale);

                            Polyline Line = new Polyline() { Closed = false, Layer = "粗线" };
                            Line.AddVertexAt(0, pt1, 0, 0, 0);
                            Line.AddVertexAt(1, pt2, 0, 0, 0);
                            ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(Line);

                            // 箭头
                            pt3 = pt2.Convert2D(0, -2 * scale);
                            pt4 = pt2.Convert2D(0.5 * scale, -2 * scale);
                            pt5 = pt2.Convert2D(-0.5 * scale, -2 * scale);

                            Solid s1 = new Solid(pt3.Convert3D(), pt4.Convert3D(), pt5.Convert3D()) { Layer = "粗线" };
                            ext = ext.Add(new Extents2d(s1.Bounds.Value.MinPoint.Convert2D(), s1.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(s1);

                            break;
                        }
                    case ArrowDirection.West:  // 左
                        {
                            // 直线
                            pt1 = StartpPint.Convert2D();
                            pt2 = StartpPint.Convert2D(-lineLength + 2 * scale);

                            Polyline Line = new Polyline() { Closed = false, Layer = "粗线" };
                            Line.AddVertexAt(0, pt1, 0, 0, 0);
                            Line.AddVertexAt(1, pt2, 0, 0, 0);
                            ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(Line);

                            // 箭头
                            pt3 = pt2.Convert2D(-2 * scale);
                            pt4 = pt2.Convert2D(0, -0.5 * scale);
                            pt5 = pt2.Convert2D(0, 0.5 * scale);

                            Solid s1 = new Solid(pt3.Convert3D(), pt4.Convert3D(), pt5.Convert3D()) { Layer = "粗线" };
                            ext = ext.Add(new Extents2d(s1.Bounds.Value.MinPoint.Convert2D(), s1.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(s1);
                            break;
                        }
                    case ArrowDirection.East:  //右
                        {
                            // 直线
                            pt1 = StartpPint.Convert2D();
                            pt2 = StartpPint.Convert2D(lineLength - 2 * scale);

                            Polyline Line = new Polyline() { Closed = false, Layer = "粗线" };
                            Line.AddVertexAt(0, pt1, 0, 0, 0);
                            Line.AddVertexAt(1, pt2, 0, 0, 0);
                            ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(Line);

                            // 箭头
                            pt3 = pt2.Convert2D(2 * scale);
                            pt4 = pt2.Convert2D(0, 0.5 * scale);
                            pt5 = pt2.Convert2D(0, -0.5 * scale);

                            Solid s1 = new Solid(pt3.Convert3D(), pt4.Convert3D(), pt5.Convert3D()) { Layer = "粗线" };
                            ext = ext.Add(new Extents2d(s1.Bounds.Value.MinPoint.Convert2D(), s1.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(s1);
                            break;
                        }
                }

                foreach (DBObject drawitem in res)
                {
                    recorder.AppendEntity((Entity)drawitem);
                    tr.AddNewlyCreatedDBObject(drawitem, true);
                }
                tr.Commit();
            }

            return res;
        }

        /// <summary>
        /// 带箭头引线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="StartpPint"></param>
        /// <param name="lineLength"></param>
        /// <param name="T"></param>
        /// <param name="scale"></param>
        /// <param name="Rotation"></param>
        /// <returns></returns>
        public static DBObjectCollection ArrowLineCircularMark(Database db, ref Extents2d ext, Point3d StartpPint, double lineLength, string T, double scale = 100, double Rotation = 0)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 计算起始坐标
                Point3d pt = StartpPint.Convert3D(lineLength * scale * Math.Abs(Math.Cos(45)), -lineLength * scale * Math.Abs(Math.Cos(45)));
                // 画箭头
                //DBObjectCollection res1 = ArrowLine(db,ref ext, pt, lineLength,scale,135);

                // 直线
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                Line.AddVertexAt(0, pt.Convert2D(), 0, 0, 0);
                Line.AddVertexAt(1, pt.Convert2D(lineLength * scale), 0, 0, 0);

                //Line line = new Line(pt, pt.Convert3D(lineLength*scale)) { Layer = "标注" };
                recorder.AppendEntity((Entity)Line);
                tr.AddNewlyCreatedDBObject(Line, true);
                res.Add(Line);

                // 绘制圆形标记
                DBObjectCollection res2 = CircularMark(db, ref ext, pt.Convert3D(lineLength * scale), T, "仿宋", scale);

                //foreach (DBObject item in res1)
                //{
                //    res.Add(item);
                //}
                foreach (DBObject item in res2)
                {
                    res.Add(item);
                }
                var TX1 = Matrix3d.Rotation(Angle.FromDegrees(Rotation).Radians, Vector3d.ZAxis, StartpPint);
                foreach (DBObject item in res)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }
                tr.Commit();
            }

            return res;
        }

        public static LineAngularDimension2 DimAng(Database db, Line L1, Line L2, Point3d Pref, double scale)
        {
            LineAngularDimension2 AD1;
            ObjectId dimID = DimPloter.GetDimStyle(db, (int)scale);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                AD1 = new LineAngularDimension2(L1.StartPoint, L1.EndPoint, L2.StartPoint, L2.EndPoint, Pref, "", dimID);
                AD1.Layer = "标注";
                modelSpace.AppendEntity(AD1);
                tr.AddNewlyCreatedDBObject(AD1, true);

                tr.Commit();
            }
            return AD1;
        }





        public static RotatedDimension DimDiameterRotated(Database db, ref Extents2d ext, Point3d P1, Point3d P2, Point3d Pref
            , double ang = 0, double scale = 20, string unit = "mm")
        {

            RotatedDimension D1;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;
                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);

                string st = string.Format("1-{0:G}", scale);
                bool isExist = DimStyleTool.CheckDimStyleExists(st);

                //DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                var dimID = dst[st];

                //ObjectId dimID = DimPloter.GetDimStyle(db, (int)scale);
                D1 = new RotatedDimension(Angle.FromDegrees(0).Radians/*ang*/, P1, P2, Pref, "", dimID);
                D1.Layer = "标注";
                if (unit == "cm")
                {
                    D1.DimensionText = "D" + (Math.Round(D1.Measurement / 10, 1)).ToString();
                }
                else if (unit == "m")
                {
                    D1.DimensionText = "D" + (Math.Round(D1.Measurement / 1000, 1)).ToString();
                }
                else
                {
                    D1.DimensionText = "D" + D1.Measurement.ToString();
                }

                modelSpace.AppendEntity(D1);
                tr.AddNewlyCreatedDBObject(D1, true);
                Polyline line = new Polyline();
                line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                line.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                line.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                if (line.Bounds != null)
                    ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));

                tr.Commit();
            }
            return D1;

        }

    }
}
