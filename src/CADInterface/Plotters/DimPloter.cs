using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADInterface.Plotters
{
    public class DimPloter
    {

        /// <summary>
        /// 绘制标高符号
        /// </summary>
        /// <param name="bgdata">标高数据</param>
        /// <param name="refpt">标高点</param>
        /// <param name="ms"></param>
        /// <param name="tr"></param>
        /// <param name="blockTbl"></param>
        /// <param name="s"></param>
        public static void BiaoGao(Database db, ref Extents2d ext, double bgdata, Point3d refpt,  double s = 100)
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
            ,  double ang = 0,double scale=20,   string unit = "mm", string replaceText = "", string D = "",int pL = 1)
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
                if (!isExist)
                {
                    DimStyleTool.CreateDimStyle(scale);
                }
                //DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                var dimID = dst[st];

                //ObjectId dimID = DimPloter.GetDimStyle(db, (int)scale);
                D1 = new RotatedDimension(GeTools.DegreeToRadian(ang)/*ang*/, P1, P2, Pref, replaceText, dimID);
                D1.Layer = "标注";
                if (pL > 1)
                {
                    replaceText = (D1.Measurement / pL) + "×" + pL;
                    D1.DimensionText = replaceText;
                    if (unit == "cm")
                    {
                        replaceText = (D1.Measurement / pL) + "×" + pL / 10;
                        D1.DimensionText = replaceText;
                    }
                    else if (unit == "m")
                    {
                        replaceText = (D1.Measurement / pL) + "×" + pL / 1000;
                        D1.DimensionText = replaceText;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(replaceText))
                    {
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
                    }
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
        public static void AddListDimRotated(Database db, ref Extents2d ext,
            Point3d Pref, List<Point3d> npts, int scale, double ang = 0, int pN = 1,string unit="mm")
        {

            RotatedDimension D1;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                var dimID = dst[string .Format("1-{0:G}",scale)];

                if (npts.Count > 1)
                {
                    if (pN == 1)
                    {
                        for (int i = 0; i < npts.Count - 1; i++)
                        {
                            Point3d P1 = npts[i];
                            Point3d P2 = npts[i + 1];
                            //Point3d Pref = GeTools.MidPoint(P1, P2).Convert3D(3, 3);
                            D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
                            D1.Layer = "标注";
                            if(unit=="cm")
                            {
                                string replaceText = (Math.Round(D1.Measurement / 10,1)).ToString();
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
                            line.AddVertexAt(0, Pref.Convert2D(-8* scale, -8 * scale), 0, 0, 0);
                            line.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            if (line.Bounds!=null)
                            ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                        }
                    }
                    else
                    {
                        if(npts.Count == 2)
                        {
                            Point3d P1 = npts[0];
                            Point3d P2 = npts[1];
                            //Point3d Pref = GeTools.MidPoint(P1, P2).Convert3D(0, 3);
                            D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
                            D1.Layer = "标注";
                            string replaceText = (D1.Measurement/ pN)+ "×"+ pN;
                            D1.DimensionText = replaceText;
                            if (unit == "cm")
                            {
                                replaceText = (D1.Measurement /pN) + "×" + pN/10;
                                D1.DimensionText = replaceText;
                            }
                            else if (unit == "m")
                            {
                                replaceText = (D1.Measurement / pN) + "×" + pN/1000;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="Pref"></param>
        /// <param name="npts"></param>
        /// <param name="scale"></param>
        /// <param name="ang"></param>
        /// <param name="pL"></param>
        public static void AddDoubleListDimRotated(Database db, ref Extents2d ext,
          Point3d Pref, List<Point3d> npts, int scale, double ang = 0, int pL = 100,ArrowDirection dir=ArrowDirection.North,string unit="mm")
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
                    D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
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
                    line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8* scale), 0, 0, 0);
                    line.AddVertexAt(1, ptSt.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(2, ptEnd.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                    if (line.Bounds != null)
                        ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                }
                else if (npts.Count>2&&npts.Count <=6)
                {

                    for (int i = 0; i < npts.Count - 1; i++)
                    {
                        Point3d P1 = npts[i];
                        Point3d P2 = npts[i + 1];
                        if(i==0)
                        {
                            ptSt = P1;
                        }
                        if (i == npts.Count - 2)
                        {
                            ptEnd = P2;
                        }
                        D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
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
                        Polyline line1 = new Polyline();
                        line1.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                        line1.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8* scale), 0, 0, 0);
                        line1.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                        line1.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                        if (line1.Bounds != null)
                            ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));
                    }
                    D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), ptSt, ptEnd, Pref.Convert3D(5 * scale, 5 * scale), "", dimID);
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
                    Polyline line = new Polyline();
                    line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                    line.AddVertexAt(1, ptSt.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(2, ptEnd.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                    if (line.Bounds != null)
                        ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                    tr.AddNewlyCreatedDBObject(D1, true);
                }
                else if (npts.Count>6)
                {  
                    for (int i = 0; i < npts.Count - 1; i++)
                    {
                        if (i<2||i> npts.Count - 2-2)
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
                            D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
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
                            Polyline line1 = new Polyline();
                            line1.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line1.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            line1.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line1.AddVertexAt(2, P2.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            if (line1.Bounds != null)
                                ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));
                        }
                        else if(i==2)
                        {
                            Point3d P1 = npts[2];
                            Point3d P2 = npts[npts.Count-1 - 2];                            
                            D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
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
                            if (pL > 1)
                            {
                                string replaceText =(D1.Measurement / pL) + "×" + pL;
                                D1.DimensionText = replaceText;
                                if (unit == "cm")
                                {
                                    replaceText =(D1.Measurement / pL) + "×" + pL/10;
                                    D1.DimensionText = replaceText;
                                }
                                else if (unit == "m")
                                {
                                    replaceText = (D1.Measurement / pL) + "×" + pL/1000;
                                    D1.DimensionText = replaceText;
                                }
                            }
                            modelSpace.AppendEntity(D1);
                            tr.AddNewlyCreatedDBObject(D1, true);
                            Polyline line1 = new Polyline();
                            line1.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line1.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8 * scale), 0, 0, 0);
                            line1.AddVertexAt(1, P1.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                            line1.AddVertexAt(2, P2.Convert2D(-8 * scale, -8* scale), 0, 0, 0);
                            if (line1.Bounds != null)
                                ext = ext.Add(new Extents2d(line1.Bounds.Value.MinPoint.Convert2D(), line1.Bounds.Value.MaxPoint.Convert2D()));
                        }
                    }
                    double Offset = 5 * scale;
                    switch(dir)
                    {
                        case ArrowDirection.North: //上
                            Offset = 5 * scale;
                            break;
                        case ArrowDirection.South: // 下
                            Offset = -5 * scale;
                            break;
                        case ArrowDirection.East:  // 右
                            Offset = 5 * scale;
                            break;
                        case ArrowDirection.West:  //  左
                            Offset = -5 * scale;
                            break;
                    }
                    D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), ptSt, ptEnd, Pref.Convert3D(Offset, Offset), "", dimID);
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
                    Polyline line = new Polyline();
                    line.AddVertexAt(0, Pref.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(0, Pref.Convert2D(-8 * scale, -8* scale), 0, 0, 0);
                    line.AddVertexAt(1, ptSt.Convert2D(8 * scale, 8 * scale), 0, 0, 0);
                    line.AddVertexAt(2, ptEnd.Convert2D(-8* scale, -8* scale), 0, 0, 0);
                    if (line.Bounds != null)
                        ext = ext.Add(new Extents2d(line.Bounds.Value.MinPoint.Convert2D(), line.Bounds.Value.MaxPoint.Convert2D()));
                    tr.AddNewlyCreatedDBObject(D1, true);
                }
                tr.Commit();
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
         Point3d Pref, Point3d PrefMid, List<Point3d> npts, int scale, double ang = 0, int pL = 100,int firstDimNum=2,bool isLeft=true)
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
                    D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
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
                            D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
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

                                    D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, "", dimID);
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
                                        D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P, Pref, "", dimID);
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
                                    D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P, Pref, "", dimID);
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
                    D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), ptSt, ptEnd, Pref.Convert3D(5 * scale, 5 * scale), "", dimID);
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
                            Point3d Pref = GeTools.MidPoint(P1, P2).Convert3D(3, 3);
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
                            Point3d Pref = GeTools.MidPoint(P1, P2).Convert3D(0, 3);
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
           double Rotation,string tstyle = "En", double scale = 100)
        {
            tstyle = Extensions.curFont;
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 第一条线
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                Point2d point1 = point;
                Point2d point2 = point.Convert2D(2*scale, 5 * scale);
                Point2d point3 = point2.Convert2D(15*scale);
                Line.AddVertexAt(0, point1, 0, 0, 0);
                Line.AddVertexAt(1, point2, 0, 0, 0);
                Line.AddVertexAt(2, point3, 0, 0, 0);

                ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(Line);

                // 圆内文字
                DBObjectCollection res3 = CircularMark(db, ref ext, point3.Convert3D(),T3.ToString(), tstyle,scale);

                // top 文字
                Point2d pt = new Point2d(point2.X + 15 * scale / 2, point2.Y + scale);
                DBObjectCollection res1 = TextPloter.PlotText(db,ref ext,TextPloter.eTxtLocation.E_TOP, pt, T1, scale,tstyle, GeTools.DegreeToRadian(0));

                // bottom 文字
                pt = new Point2d(point2.X + 15 * scale / 2, point2.Y - scale);
                DBObjectCollection res2 = TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_BOTTOM, pt, T2, scale, tstyle, GeTools.DegreeToRadian(0));

                var TX1 = Matrix3d.Rotation(GeTools.DegreeToRadian(Rotation), Vector3d.ZAxis, point.Convert3D());
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
        string tstyle ="En", double scale = 100)
        {
            tstyle = Extensions.curFont;
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
                txt.Position = point3.Convert3D(cR*scale);
                txt.HorizontalMode = TextHorizontalMode.TextCenter;
                txt.VerticalMode = TextVerticalMode.TextVerticalMid;
                txt.AlignmentPoint = point3.Convert3D(cR * scale);
                txt.TextStyleId = st[Extensions.curFont];
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
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T1, scale, tstyle, GeTools.DegreeToRadian(0));

                // bottom 文字
                pt = new Point2d(point2.X + 15 * scale / 2, point2.Y);
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_BOTTOM, pt, T2, scale, tstyle, GeTools.DegreeToRadian(0));
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
        string tstyle ="En", double scale = 100)
        {
            tstyle = Extensions.curFont;
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
                MText txt1 = TextPloter.GetTextActualWidth(db, T1, scale,2.5, Extensions.curFont);
                width1 = txt1.ActualWidth;
                double width2 = 0;
                MText txt2 = TextPloter.GetTextActualWidth(db, T2, scale,2.5, Extensions.curFont);
                width2 = txt2.ActualWidth;
                double lineWidth = width1 > width2 ? width1 : width2;
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                Point2d point1 = point.Convert2D();
                Point2d point2 = point.Convert2D(lineWidth/6, lineWidth/2);
                Point2d point3 = point2.Convert2D(lineWidth);
                Line.AddVertexAt(0, point1, 0, 0, 0);
                Line.AddVertexAt(1, point2, 0, 0, 0);
                Line.AddVertexAt(2, point3, 0, 0, 0);
                recorder.AppendEntity(Line);
                tr.AddNewlyCreatedDBObject(Line, true);
                ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));

                // top 文字
                Point2d pt = new Point2d(point2.X + lineWidth / 2, point2.Y);
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T1, scale, tstyle, GeTools.DegreeToRadian(0));

                // bottom 文字
                pt = new Point2d(point2.X + lineWidth / 2, point2.Y);
                TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_BOTTOM, pt, T2, scale, tstyle, GeTools.DegreeToRadian(0));
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
            , double scale, double ang = 0,string unit="mm", string replaceText = "",string D="" )
        {
      
                ObjectId dimID = DimPloter.GetDimStyle(db, (int)scale);
                RotatedDimension D1 = new RotatedDimension(GeTools.DegreeToRadian(ang), P1, P2, Pref, D + replaceText, dimID);
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
        public static ObjectId GetDimStyle(Database db,  int scale)
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
        public static DBObjectCollection CircularMark(Database db, ref Extents2d ext, Point3d point,string txt, string tstyle = "En", double scale = 100,double Rotation = 0)
        {
            tstyle = Extensions.curFont;
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
                Mtext.AlignmentPoint = point.Convert3D(cr );
                Mtext.Position = point.Convert3D(cr);
                ext = ext.Add(new Extents2d(Mtext.Bounds.Value.MinPoint.Convert2D(), Mtext.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(Mtext);                

                recorder.AppendEntity((Entity)Mtext);
                tr.AddNewlyCreatedDBObject(Mtext, true);

                // 圆
                Circle a = new Circle(point.Convert3D(cr ), Vector3d.ZAxis, cr );
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
             ArrowDirection eDir,Point3d StartpPint, double lineLength, double Rotation = 0,double scale=80)
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
                            pt2 = StartpPint.Convert2D(0,lineLength - 2 * scale);

                            Polyline Line = new Polyline() { Closed = false, Layer = "粗线" };
                            Line.AddVertexAt(0, pt1, 0, 0, 0);
                            Line.AddVertexAt(1, pt2, 0, 0, 0);
                            ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));
                            res.Add(Line);

                            // 箭头
                            pt3 = pt2.Convert2D(0,2* scale);
                            pt4 = pt2.Convert2D(0.5 * scale, 2 * scale);
                            pt5 = pt2.Convert2D(-0.5 * scale, 2* scale);

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
                            pt4 = pt2.Convert2D(0,-0.5 * scale);
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
        public static DBObjectCollection ArrowLineCircularMark(Database db, ref Extents2d ext, Point3d StartpPint, double lineLength,string T, double scale = 100, double Rotation = 0)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 计算起始坐标
                Point3d pt = StartpPint.Convert3D(lineLength * scale*Math.Abs(Math.Cos(45)),-lineLength * scale*Math.Abs(Math.Cos(45)));
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
                DBObjectCollection res2 = CircularMark(db, ref ext, pt.Convert3D(lineLength * scale),T,Extensions.curFont, scale);

                //foreach (DBObject item in res1)
                //{
                //    res.Add(item);
                //}
                foreach (DBObject item in res2)
                {
                    res.Add(item);
                }
                var TX1 = Matrix3d.Rotation(GeTools.DegreeToRadian(Rotation), Vector3d.ZAxis, StartpPint);
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



        /// <summary>
        /// 直径标注
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="startPoint"></param>
        /// <param name="arrow">箭头方向</param>
        /// <param name="diameter">直径</param>
        /// <param name="diameter">标记字符串</param>
        /// <param name="scale">比例</param>
        public static void CreateDiameterMark(Database db, ref Extents2d ext, Point3d startPoint,
            eArrow arrow, double diameter, string T1, double scale, double rotation)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 半径
                double radius = diameter / 2;

                Point2d pt1, pt2, pt3, pt4, pt5;
                double offsetX = 0;
                double offsetY = 0;
                Polyline pl3 = new Polyline() { Closed = false, Layer = "标注", ColorIndex = 4 };
                pl3.AddVertexAt(0, startPoint.Convert2D(), 0, 0, 0);
                pl3.AddVertexAt(1, startPoint.Convert2D(radius * 2), 0, 0, 0);
                ext = ext.Add(new Extents2d(pl3.Bounds.Value.MinPoint.Convert2D(), pl3.Bounds.Value.MaxPoint.Convert2D()));

                // 左边箭头
                double multi =scale;
                pt3 = startPoint.Convert2D(2* multi, 0.5 * multi);
                pt4 = startPoint.Convert2D(2 * multi, -0.5 * multi);

                Solid s1 = new Solid(startPoint.Convert3D(), pt3.Convert3D(), pt4.Convert3D()) { Layer = "细线",ColorIndex=4 };
                ext = ext.Add(new Extents2d(s1.Bounds.Value.MinPoint.Convert2D(), s1.Bounds.Value.MaxPoint.Convert2D()));

                // 右边箭头
                pt3 = startPoint.Convert2D(radius * 2 - 2* multi, 0.5 * multi);
                pt4 = startPoint.Convert2D(radius * 2 - 2 * multi, -0.5 * multi);

                Solid s2 = new Solid(startPoint.Convert3D(radius * 2), pt3.Convert3D(), pt4.Convert3D()) { Layer = "粗线" };
                ext = ext.Add(new Extents2d(s2.Bounds.Value.MinPoint.Convert2D(), s2.Bounds.Value.MaxPoint.Convert2D()));

                DBObjectCollection res2 = TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, startPoint.Convert2D(radius, 10),
                    string.Format("D{0}", (int)(radius * 2*0.1)), scale, Extensions.curFont);
                switch (arrow)
                {
                    case eArrow.E_ARROW_NULL:           // 无箭头
                        {
                            res.Add(pl3);
                            break;
                        }

                    case eArrow.E_ARROW_LEFT_ARROW:     // 左侧箭头
                        {
                            res.Add(pl3);
                            res.Add(s1);
                            break;
                        }
                    case eArrow.E_ARROW_RIGHT_ARROW:    // 右侧箭头
                        {
                            res.Add(pl3);
                            res.Add(s2);
                            break;
                        }
                    case eArrow.E_ARROW_DOUBLE_SIDE_ARROW:  // 双侧箭头
                        {
                            res.Add(pl3);
                            res.Add(s1);
                            res.Add(s2);
                            break;
                        }
                    default:
                        break;
                }
                // 旋转
                var TX1 = Matrix3d.Rotation(GeTools.DegreeToRadian(rotation), Vector3d.ZAxis, startPoint.Convert3D(radius));
                foreach (DBObject item in res)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }
                foreach (DBObject item in res2)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }
                foreach (DBObject drawitem in res)
                {
                    recorder.AppendEntity((Entity)drawitem);
                    tr.AddNewlyCreatedDBObject(drawitem, true);
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// 钩线标记
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="startPoint"></param>
        /// <param name="T1"></param>
        /// <param name="lineLength"></param>
        /// <param name="scale"></param>
        public static void CreateHookLineMark(Database db, ref Extents2d ext, Point3d startPoint,
            string T1, double lineLength, double scale, double rotation = 0)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 线
                double valueX =1*scale;
                Point3d pt1 = startPoint.Convert3D(valueX, valueX);
                Polyline pl1 = new Polyline() { Closed = false, Layer = "标注",ColorIndex=4 };
                pl1.AddVertexAt(0, startPoint.Convert2D(lineLength), 0, 0, 0);
                pl1.AddVertexAt(1, startPoint.Convert2D(), 0, 0, 0);
                pl1.AddVertexAt(2, startPoint.Convert2D(valueX, valueX), 0, 0, 0);
                ext = ext.Add(new Extents2d(pl1.Bounds.Value.MinPoint.Convert2D(), pl1.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(pl1);

                // 圆形标记
                DBObjectCollection res1 = DimPloter.CircularMark(db, ref ext, startPoint.Convert3D(lineLength), T1, Extensions.curFont, scale, GeTools.DegreeToRadian(360-rotation));

                // 旋转
                var TX1 = Matrix3d.Rotation(GeTools.DegreeToRadian(rotation), Vector3d.ZAxis, startPoint.Convert3D());
                foreach (DBObject item in res)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }
                foreach (DBObject item in res1)
                {
                    Entity pr = (Entity)item;
                    pr.TransformBy(TX1);
                }

                foreach (DBObject drawitem in res)
                {
                    recorder.AppendEntity((Entity)drawitem);
                    tr.AddNewlyCreatedDBObject(drawitem, true);
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// 焊接面
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="center">圆心</param>
        /// <param name="radius">原半径</param>
        /// <param name="scale">比例</param>
        /// <param name="replaceDim">替换焊接面的标注文字</param>
        public static void CreateFaceWeld(Database db, ref Extents2d ext, Point3d center,
            double radius, double scale, string replaceDim)
        {
            DBObjectCollection res = new DBObjectCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord recorder = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // 焊接面夹角
                double angle = 10;

                // 焊接面厚度
                double thick = 20;
                // 计算焊接面在圆上两点
                Polyline pl1 = new Polyline() { Closed = false, Layer = "粗线" };
                double offsetX = radius * Math.Sin(GeTools.DegreeToRadian(angle / 2));
                double offsetY = radius * Math.Cos(GeTools.DegreeToRadian(angle / 2));
                Point3d p1, p2, p3, p4;
                p1 = center.Convert3D(-offsetX, -offsetY);
                p2 = center.Convert3D(offsetX, -offsetY);
                p3 = p1.Convert3D(0, thick);
                p4 = p2.Convert3D(0, thick);

                Solid sod1 = new Solid(p3, p1, center.Convert3D(0, -radius + thick), center.Convert3D(0, -radius));
                ext = ext.Add(new Extents2d(sod1.Bounds.Value.MinPoint.Convert2D(), sod1.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(sod1);

                Solid sod2 = new Solid(center.Convert3D(0, -radius), center.Convert3D(0, -radius + thick), p2, p4);
                ext = ext.Add(new Extents2d(sod2.Bounds.Value.MinPoint.Convert2D(), sod2.Bounds.Value.MaxPoint.Convert2D()));
                res.Add(sod2);

                RotatedDimension dim = DimPloter.AddRotDim(db, ref ext, p1, p2, p1.Convert3D(0, -5 * scale), (int)scale, 0, "mm", replaceDim);
                res.Add(dim);

                foreach (DBObject drawitem in res)
                {
                    recorder.AppendEntity((Entity)drawitem);
                    tr.AddNewlyCreatedDBObject(drawitem, true);
                }
                tr.Commit();
                //ext = ext.Add(new Extents2d(dim.Bounds.Value.MinPoint.Convert2D(), dim.Bounds.Value.MaxPoint.Convert2D()));

            }
        }

        public static void CreateLeadWireMark(Database db, ref Extents2d ext, Point3d point, string T,
    string tstyle = "En", double scale = 100,bool isLeftText=false)
        {
            tstyle = Extensions.curFont;
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
                MText txt1 = TextPloter.GetTextActualWidth(db, T, scale, 2.5, Extensions.curFont);
                width1 = txt1.ActualWidth;               
                double lineWidth = width1;
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                if (isLeftText)
                {
                    Point2d point1 = point.Convert2D();
                    Point2d point2 = point.Convert2D(-lineWidth / 6, lineWidth / 2);
                    Point2d point3 = point2.Convert2D(-lineWidth);
                    Line.AddVertexAt(0, point1, 0, 0, 0);
                    Line.AddVertexAt(1, point2, 0, 0, 0);
                    Line.AddVertexAt(2, point3, 0, 0, 0);
                    recorder.AppendEntity(Line);
                    tr.AddNewlyCreatedDBObject(Line, true);
                    ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));

                    // top 文字
                    Point2d pt = new Point2d(point2.X - lineWidth / 2, point2.Y);
                    TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T, scale, tstyle, GeTools.DegreeToRadian(0));
                }
                else
                {
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
                    TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T, scale, tstyle, GeTools.DegreeToRadian(0));
                }
                
                tr.Commit();
            }

        }
        public static void Create2TopLeadWireMark(Database db, ref Extents2d ext, Point3d point, string T,
string tstyle = "En", double scale = 100, bool isLeftText = false)
        {
            tstyle = Extensions.curFont;
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
                MText txt1 = TextPloter.GetTextActualWidth(db, T, scale, 2.5, Extensions.curFont);
                width1 = txt1.ActualWidth;
                double lineWidth = width1;
                Polyline Line = new Polyline() { Closed = false, Layer = "标注" };
                if (isLeftText)
                {
                    Point2d point1 = point.Convert2D();
                    Point2d point2 = point.Convert2D(lineWidth / 6, -lineWidth / 2);
                    Point2d point3 = point2.Convert2D(lineWidth);
                    Line.AddVertexAt(0, point1, 0, 0, 0);
                    Line.AddVertexAt(1, point2, 0, 0, 0);
                    Line.AddVertexAt(2, point3, 0, 0, 0);
                    recorder.AppendEntity(Line);
                    tr.AddNewlyCreatedDBObject(Line, true);
                    ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));

                    // top 文字
                    Point2d pt = new Point2d(point2.X+ lineWidth / 2, point2.Y);
                    TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T, scale, tstyle, GeTools.DegreeToRadian(0));
                }
                else
                {
                    Point2d point1 = point.Convert2D();
                    Point2d point2 = point.Convert2D(-lineWidth / 6, -lineWidth / 2);
                    Point2d point3 = point2.Convert2D(-lineWidth);
                    Line.AddVertexAt(0, point1, 0, 0, 0);
                    Line.AddVertexAt(1, point2, 0, 0, 0);
                    Line.AddVertexAt(2, point3, 0, 0, 0);
                    recorder.AppendEntity(Line);
                    tr.AddNewlyCreatedDBObject(Line, true);
                    ext = ext.Add(new Extents2d(Line.Bounds.Value.MinPoint.Convert2D(), Line.Bounds.Value.MaxPoint.Convert2D()));

                    // top 文字
                    Point2d pt = new Point2d(point2.X - lineWidth / 2, point2.Y);
                    TextPloter.PlotText(db, ref ext, TextPloter.eTxtLocation.E_TOP, pt, T, scale, tstyle, GeTools.DegreeToRadian(0));
                }

                tr.Commit();
            }

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
                if (!isExist)
                {
                    DimStyleTool.CreateDimStyle(scale);
                }
                //DimStyleTable dst = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                var dimID = dst[st];

                //ObjectId dimID = DimPloter.GetDimStyle(db, (int)scale);
                D1 = new RotatedDimension(GeTools.DegreeToRadian(0)/*ang*/, P1, P2, Pref, "", dimID);
                D1.Layer = "标注";
                if (unit == "cm")
                {
                    D1.DimensionText = "D"+ (Math.Round(D1.Measurement / 10, 1)).ToString();
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

