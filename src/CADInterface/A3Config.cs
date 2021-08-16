using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;

namespace CADInterface
{
    public static class A3Config
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void IniConfig()
        {
            InitMark();
            InitWrittenWords2Block();
        }
        /// <summary>
        /// 初始化标注
        /// </summary>
        private static void InitMark()
        {
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = acCurDb.TransactionManager.StartTransaction())
            {
                Dictionary<string, short> ldic = new Dictionary<string, short>()
                {

                    ["粗线"] = 4,
                    ["细线"] = 2,
                    ["标注"] = 7,
                    ["中心线"] = 1,
                    ["虚线"] = 3,
                    ["图框"] = 8,
                    ["标注"] = 7,

                };
                List<string> Lname = new List<string>() { "CENTER", "DASHED" };
                LayerTable acLyrTbl;
                acLyrTbl = tr.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;
                LinetypeTable acLinTbl;
                acLinTbl = tr.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                foreach (string ltname in Lname)
                {
                    if (!acLinTbl.Has(ltname))
                    {
                        acCurDb.LoadLineTypeFile(ltname, "acad.lin");
                    }
                }
                LayerTableRecord acLyrTblRec = new LayerTableRecord();
                foreach (string key in ldic.Keys)
                {
                    short cid = ldic[key];
                    acLyrTblRec = new LayerTableRecord();
                    if (!acLyrTbl.Has(key))
                    {
                        acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, cid);
                        if (cid != 4) { acLyrTblRec.LineWeight = LineWeight.LineWeight009; }
                        else { acLyrTblRec.LineWeight = LineWeight.LineWeight030; }
                        if (cid == 1) { acLyrTblRec.LinetypeObjectId = acLinTbl["CENTER"]; }
                        if (cid == 3) { acLyrTblRec.LinetypeObjectId = acLinTbl["DASHED"]; }
                        if (cid == 8) { acLyrTblRec.IsPlottable = false; }
                        acLyrTblRec.Name = key;
                        if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                        acLyrTbl.Add(acLyrTblRec);
                        tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                    else
                    {
                        acLyrTblRec = tr.GetObject(acLyrTbl[key], OpenMode.ForWrite) as LayerTableRecord;
                        if (cid != 4) { acLyrTblRec.LineWeight = LineWeight.LineWeight009; }
                        else { acLyrTblRec.LineWeight = LineWeight.LineWeight030; }
                        if (cid == 1) { acLyrTblRec.LinetypeObjectId = acLinTbl["CENTER"]; }
                        if (cid == 3) { acLyrTblRec.LinetypeObjectId = acLinTbl["DASHED"]; }
                        if (cid == 8) { acLyrTblRec.IsPlottable = false; }
                        acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, cid);
                    }
                }

                if (!acLyrTbl.Has("sjx"))//新建的设置对象
                {
                    acLyrTblRec = new LayerTableRecord();
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    acLyrTblRec.Name = "sjx";
                    acLyrTblRec.LineWeight = LineWeight.LineWeight015;
                    if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }
                if (!acLyrTbl.Has("dmx"))
                {
                    acLyrTblRec = new LayerTableRecord();
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 8);
                    acLyrTblRec.Name = "dmx";
                    acLyrTblRec.LineWeight = LineWeight.LineWeight015;
                    if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }

                tr.Commit();
            }
        }


        /// <summary>
        /// 初始化文字比例图块
        /// </summary>
        private static void InitWrittenWords2Block()
        {
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = acCurDb.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(acCurDb.TextStyleTableId, OpenMode.ForWrite) as TextStyleTable;
                #region 字体设置
                if (!st.Has("仿宋"))//字体设置
                {
                    TextStyleTableRecord str = new TextStyleTableRecord()
                    {
                        Name = "仿宋",
                        Font = new Autodesk.AutoCAD.GraphicsInterface.FontDescriptor("仿宋_GB2312", false, false, 0, 0),
                        XScale = 0.70,
                    };
                    st.Add(str);
                    tr.AddNewlyCreatedDBObject(str, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["仿宋"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.Font = new Autodesk.AutoCAD.GraphicsInterface.FontDescriptor("仿宋_GB2312", false, false, 0, 0);
                    str.XScale = 0.70;
                }
                #endregion

                DimStyleTable dst = (DimStyleTable)tr.GetObject(acCurDb.DimStyleTableId, OpenMode.ForWrite);//设置不同比例的对象属性
                foreach (int thescale in new int[] {  1, 2,50, 75, 100, 125, 150, 200, 300 ,500,1000})
                {
                    string scname = "1-" + thescale.ToString();
                    DimStyleTableRecord dstr = new DimStyleTableRecord();
                    if (!dst.Has(scname))
                    {
                        dstr.Name = "1-" + thescale.ToString();
                        dstr.Dimscale = thescale;
                        dstr.Dimtxsty = st["仿宋"];
                        //dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        //dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        //dstr.Dimclrt = Color.FromColorIndex(ColorMethod.ByAci, 7);
                        dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimdli = 5;
                        dstr.Dimsah = true;
                        //if (thescale == 2)
                        //{
                        //    dstr.Dimblk = DimStyleTool.GetArrowObjectId("_NONE");
                        //    dstr.Dimblk1 = DimStyleTool.GetArrowObjectId("_NONE");
                        //    dstr.Dimblk2 = DimStyleTool.GetArrowObjectId("_NONE");
                        //}
                        //else
                        //{
                        //    dstr.Dimblk = DimStyleTool.GetArrowObjectId("_OBLIQUE");
                        //    dstr.Dimblk1 = DimStyleTool.GetArrowObjectId("_OBLIQUE");
                        //    dstr.Dimblk2 = DimStyleTool.GetArrowObjectId("_OBLIQUE");
                        //}

                        dstr.Dimdle = 0;
                        dstr.Dimexe = 1.0;
                        dstr.Dimexo = 1.0;
                        dstr.DimfxlenOn = true;
                        dstr.Dimfxlen = 4;
                        dstr.Dimtxt = 2.5;
                        dstr.Dimasz = 1.5;
                        dstr.Dimtix = true;
                        dstr.Dimtmove = 1;
                        dstr.Dimtad = 1;
                        dstr.Dimgap = 0.8;
                        dstr.Dimdec = 0;
                        dstr.Dimtih = false;
                        dstr.Dimtoh = false;
                        dstr.Dimdsep = '.';
                        if (thescale == 1)
                        {
                            dstr.Dimlfac = 1000;
                            dstr.Dimscale = 0.8;
                        }
                        else
                            dstr.Dimlfac = 1;
                        dst.Add(dstr);
                        tr.AddNewlyCreatedDBObject(dstr, true);
                    }
                    else
                    {
                        dstr = tr.GetObject(dst[scname], OpenMode.ForWrite) as DimStyleTableRecord;
                        dstr.Name = "1-" + thescale.ToString();
                        dstr.Dimscale = thescale;
                        dstr.Dimtxsty = st["仿宋"];
                        dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimdli = 5;
                        dstr.Dimsah = true;
                        //if (thescale == 2)
                        //{
                        //    dstr.Dimblk = DimStyleTool.GetArrowObjectId("_NONE");
                        //    dstr.Dimblk1 = DimStyleTool.GetArrowObjectId("_NONE");
                        //    dstr.Dimblk2 = DimStyleTool.GetArrowObjectId("_NONE");
                        //}
                        //else
                        //{
                        //    dstr.Dimblk = DimStyleTool.GetArrowObjectId("_OBLIQUE");
                        //    dstr.Dimblk1 = DimStyleTool.GetArrowObjectId("_OBLIQUE");
                        //    dstr.Dimblk2 = DimStyleTool.GetArrowObjectId("_OBLIQUE");
                        //}

                        dstr.Dimdle = 0;
                        dstr.Dimexe = 1.0;
                        dstr.Dimexo = 1.0;
                        dstr.DimfxlenOn = true;
                        dstr.Dimfxlen = 4;
                        dstr.Dimtxt = 2.5;
                        dstr.Dimasz = 1.5;
                        dstr.Dimtix = true;
                        dstr.Dimtmove = 1;
                        dstr.Dimtad = 1;
                        dstr.Dimgap = 0.8;
                        dstr.Dimdec = 0;
                        dstr.Dimtih = false;
                        dstr.Dimtoh = false;
                        dstr.Dimdsep = '.';
                        if (thescale == 1)
                            dstr.Dimlfac = 1000;
                        else
                            dstr.Dimlfac = 1;
                        //dst.Add(dstr);
                    }
                    //DimStyleTableRecord dstr = new DimStyleTableRecord()
                    //{
                    //    Name = "1-" + thescale.ToString(),
                    //    Dimscale = thescale,
                    //    Dimtxsty = st["En"],
                    //    Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6),
                    //    Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6),
                    //    Dimdli = 5.0,
                    //    Dimexe = 1.0,
                    //    Dimexo = 1.0,
                    //    DimfxlenOn = true,
                    //    Dimfxlen = 4,
                    //    Dimtxt = 2,
                    //    Dimasz = 1.5,
                    //    Dimtix = true,
                    //    Dimtmove = 1,
                    //    Dimtad = 1,
                    //    Dimgap = 0.8,
                    //    Dimdec = 0,
                    //    Dimtih = false,
                    //    Dimtoh = false,
                    //    Dimdsep = '.',
                    //    Dimlfac = 0.1,
                    //};
                    //ObjectId dsId = dst.Add(dstr);
                    //tr.AddNewlyCreatedDBObject(dstr, true);
                }

                #region 自定义块
                //-------------------------------------------------------------------------------------------
                // 自定义块
                //-------------------------------------------------------------------------------------------
                BlockTable bt = (BlockTable)tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = new BlockTableRecord();
                if (!bt.Has("KYBG"))
                {
                    btr = new BlockTableRecord();
                    btr.Name = "KYBG";
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                    Polyline Paa = new Polyline()
                    {
                        //Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                        //Layer = "标注",
                    };
                    Paa.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                    Paa.AddVertexAt(1, new Point2d(200 / Math.Sqrt(3), 200), 0, 0, 0);
                    Paa.AddVertexAt(2, new Point2d(-200 / Math.Sqrt(3), 200), 0, 0, 0);
                    Paa.Closed = true;
                    btr.AppendEntity(Paa);
                    tr.AddNewlyCreatedDBObject(Paa, true);

                    Line Laa = new Line(new Point3d(-200 / Math.Sqrt(3), 0, 0), new Point3d(1200, 0, 0));
                    btr.AppendEntity(Laa);
                    tr.AddNewlyCreatedDBObject(Laa, true);

                    AttributeDefinition curbg = new AttributeDefinition();
                    curbg.Position = new Point3d(120, 80, 0);
                    curbg.Height = 300;
                    curbg.WidthFactor = 0.7;
                    curbg.Tag = "标高";
                    //curbg.Layer = "标注";
                    curbg.TextStyleId = st["仿宋"];
                    btr.AppendEntity(curbg);
                    tr.AddNewlyCreatedDBObject(curbg, true);
                }
                if (!bt.Has("BG"))
                {
                    btr = new BlockTableRecord();
                    btr.Name = "BG";
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                    Polyline Paa = new Polyline()
                    {
                        //Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                        //Layer = "标注",
                    };
                    Paa.AddVertexAt(0, new Point2d(0, 0), 0, 0, 200);
                    Paa.AddVertexAt(1, new Point2d(0, 200), 0, 0, 0);
                    btr.AppendEntity(Paa);
                    tr.AddNewlyCreatedDBObject(Paa, true);
                    AttributeDefinition curbg = new AttributeDefinition();
                    curbg.Position = new Point3d(120, 200, 0);
                    curbg.Height = 250;
                    curbg.WidthFactor = 0.70;
                    curbg.Tag = "标高";
                    //curbg.Layer = "标注";
                    curbg.TextStyleId = st["仿宋"];
                    btr.AppendEntity(curbg);
                    tr.AddNewlyCreatedDBObject(curbg, true);
                }
                //-------------------------------------------------------------------------------------------
                if (!bt.Has("YP"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "YP";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);

                    Solid Array = new Solid(
                        new Point3d(774.9048 - 974.2305, 49.6974, 0),
                        new Point3d(710.2305 - 974.2305, 0, 0),
                        new Point3d(974.2305 - 974.2305, 0, 0)
                        )
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(Array);
                    tr.AddNewlyCreatedDBObject(Array, true);

                    Line L = new Line(new Point3d(974.2305 - 974.2305, 0, 0), new Point3d(-225.7695 - 974.2305, 0, 0))
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(-160 - 974.2305, 73.1925, 0);
                    curzp.Height = 300;
                    curzp.WidthFactor = 0.70;
                    curzp.Tag = "右坡";
                    curzp.TextStyleId = st["仿宋"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }


                //-------------------------------------------------------------------------------------------
                if (!bt.Has("ZP"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "ZP";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);

                    Solid Array = new Solid(
                        new Point3d(-184.6346, 49.6974, 0),
                        new Point3d(-119.9603, 0, 0),
                        new Point3d(-383.9603, 0, 0)
                        )
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(Array);
                    tr.AddNewlyCreatedDBObject(Array, true);

                    Line L = new Line(new Point3d(-383.9603, 0, 0), new Point3d(816.0397, 0, 0))
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(-160, 73.1925, 0);
                    curzp.Height = 300;
                    curzp.WidthFactor = 0.70;
                    curzp.Tag = "左坡";
                    curzp.TextStyleId = st["仿宋"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }
                //-------------------------------------------------------------------------------------------
                if (!bt.Has("TP"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "TP";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);

                    Solid Array = new Solid(
                        new Point3d(774.9048 - 974.2305, -49.6974, 0),
                        new Point3d(710.2305 - 974.2305, 0, 0),
                        new Point3d(974.2305 - 974.2305, 0, 0)
                        )
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(Array);
                    tr.AddNewlyCreatedDBObject(Array, true);
                    Polyline L = new Polyline() { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    L.AddVertexAt(0, new Point2d(974.2305 - 974.2305, 0), 0, 0, 0);
                    L.AddVertexAt(1, new Point2d(374.2305 - 974.2305, 0), 0, 0, 0);
                    L.AddVertexAt(2, new Point2d(374.2305 - 974.2305, -400), 0, 0, 0);

                    btr2.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(474 - 974.2305, -373.1925, 0);
                    curzp.Height = 300;
                    curzp.WidthFactor = 0.70;
                    curzp.Tag = "A";
                    curzp.TextStyleId = st["仿宋"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }


                //-------------------------------------------------------------------------------------------
                if (!bt.Has("BP"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "BP";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);

                    Solid Array = new Solid(
                       new Point3d(774.9048 - 974.2305, 49.6974, 0),
                       new Point3d(710.2305 - 974.2305, 0, 0),
                       new Point3d(974.2305 - 974.2305, 0, 0)
                       )
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(Array);
                    tr.AddNewlyCreatedDBObject(Array, true);
                    Polyline L = new Polyline() { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    L.AddVertexAt(0, new Point2d(974.2305 - 974.2305, 0), 0, 0, 0);
                    L.AddVertexAt(1, new Point2d(374.2305 - 974.2305, 0), 0, 0, 0);
                    L.AddVertexAt(2, new Point2d(374.2305 - 974.2305, 400), 0, 0, 0);

                    btr2.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(474 - 974.2305, 73.1925, 0);
                    curzp.Height = 300;
                    curzp.WidthFactor = 0.70;
                    curzp.Tag = "A";
                    curzp.TextStyleId = st["仿宋"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }

                if (!bt.Has("SP"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "SP";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);

                    Solid Array = new Solid(
                        new Point3d(49.6974, 774.9048 - 974.2305, 0),
                        new Point3d(0, 710.2305 - 974.2305, 0),
                        new Point3d(0, 974.2305 - 974.2305, 0)
                        )
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(Array);
                    tr.AddNewlyCreatedDBObject(Array, true);

                    Line L = new Line(new Point3d(0, 974.2305 - 974.2305, 0), new Point3d(0, -225.7695 - 974.2305, 0))
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(373.1925, -60 - 974.2305, 0);
                    curzp.Height = 300;
                    curzp.Rotation = Angle.FromDegrees(90.0).Radians;                       
                    curzp.WidthFactor = 0.70;
                    curzp.Tag = "上坡";
                    curzp.TextStyleId = st["仿宋"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }

                if (!bt.Has("YB"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "YB";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);

                    Solid Array = new Solid(
                       new Point3d(49.6974, -184.6346, 0),
                       new Point3d(0, -119.9603, 0),
                       new Point3d(0, -383.9603, 0)
                       )
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(Array);
                    tr.AddNewlyCreatedDBObject(Array, true);
                    Polyline L = new Polyline() { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    L.AddVertexAt(0, new Point2d(0, -383.9603), 0, 0, 0);
                    L.AddVertexAt(1, new Point2d(0, -383.9603 + 600), 0, 0, 0);
                    L.AddVertexAt(2, new Point2d(400, -383.9603 + 600), 0, 0, 0);
                    //Line L = new Line(new Point3d(974.2305 - 974.2305, 0, 0), new Point3d(-225.7695 - 974.2305, 0, 0))
                    //{ Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(110, -110, 0);
                    curzp.Height = 300;
                    curzp.WidthFactor = 0.70;
                    curzp.Tag = "B";
                    curzp.TextStyleId = st["仿宋"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }


                //-------------------------------------------------------------------------------------------
                if (!bt.Has("ZB"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "ZB";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);

                    Solid Array = new Solid(
                        new Point3d(-49.6974, -184.6346, 0),
                        new Point3d(0, -119.9603, 0),
                        new Point3d(0, -383.9603, 0)
                        )
                    { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(Array);
                    tr.AddNewlyCreatedDBObject(Array, true);
                    Polyline L = new Polyline() { Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    L.AddVertexAt(0, new Point2d(0, -383.9603), 0, 0, 0);
                    L.AddVertexAt(1, new Point2d(0, -383.9603 + 600), 0, 0, 0);
                    L.AddVertexAt(2, new Point2d(-400, -383.9603 + 600), 0, 0, 0);
                    //Line L = new Line(new Point3d(974.2305 - 974.2305, 0, 0), new Point3d(-225.7695 - 974.2305, 0, 0))
                    //{ Color = Color.FromColorIndex(ColorMethod.ByAci, 7), };
                    btr2.AppendEntity(L);
                    tr.AddNewlyCreatedDBObject(L, true);

                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(-373.1925, -110, 0);
                    curzp.Height = 300;
                    curzp.WidthFactor = 0.70;
                    curzp.Tag = "B";
                    curzp.TextStyleId = st["仿宋"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }
                #endregion
                tr.Commit();
            }
        }

    }
}
