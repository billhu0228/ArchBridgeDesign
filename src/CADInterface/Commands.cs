﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Model;
using System;
using System.Configuration;
using System.Collections.Specialized;
using CADInterface.Plotters;
using CADInterface.UI;
using CADInterface.API;
using Autodesk.AutoCAD.ApplicationServices;
using HS = HPDI.DrawingStandard;
using HPDI.DrawingStandard;
using Autodesk.AutoCAD.EditorInput;
using MathNet.Spatial.Units;
using MathNet.Spatial.Euclidean;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

[assembly: CommandClass(typeof(CADInterface.Commands))]
namespace CADInterface
{
    public class Commands
    {
        ArchAxis theArchAxis;
        Arch archModel;     
        ArchBridgeParametersDialog properDia = new ArchBridgeParametersDialog();

        [CommandMethod("init")]
        public void InitDrawingSetting()
        {
            HS.InitConfig.InitUnits(UnitsValue.Meters);
            //HS.InitConfig.InitBlock();

            A3Config.IniConfig();
            HS.InitConfig.InitLayer();
            //HS.InitConfig.InitTextStyle("H仿宋","仿宋_GB2312",0.7);
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Centimeter, 0.25, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Centimeter, 0.5, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Centimeter, 0.1, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Millimeter, 0.1, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Millimeter, 0.125, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Millimeter, Unit.Millimeter, 40, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Millimeter, 0.06666666666666, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Centimeter, 1, "H仿宋");
            HS.InitConfig.InitDimStyle(Unit.Meter, Unit.Centimeter, 2, "H仿宋");
        }

        [CommandMethod("GenModel")]
        public void GenerateModel()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = db.GetEditor();
            ed.WriteMessage("\\n初始化模型..");
            // archModel = NamedArch.PhoenixModelV42(out theArchAxis, 2.0, 518 / 4.5, 17.0,8.5,0.6);            
            archModel = NamedArch.PhoenixModelV63(out theArchAxis, 2.0, 518 / 4.5, 15.5,7.0,0.75);            
        }

        [CommandMethod("ArchDraw")]
        public void DrawingGeneralArrangment()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            ObjectId LayoutID = db.CreatLayout("C-1-05 主拱圈一般构造", "block\\TK.dwg");
            Extents2d ext1, ext2, ext3, ext4;

            archModel.DrawingLeftElevation(out ext1);
            // 绘制拼接区及标注
            archModel.DrawInstall(Point2d.Origin, out ext2);

            archModel.DrawingPlan(new Point2d(0, -180), out ext3);

            #region 断面图块
            BlockPloter.CopyBlockFromFile(db, "block\\BlockDef.dwg", "主拱断面");
            BlockPloter.CopyBlockFromFile(db, "block\\BlockDef.dwg", "主拱断面跨中");


            ObjectIdCollection blkID = new ObjectIdCollection();
            DBObjectCollection blkDB = new DBObjectCollection();
            DBObjectCollection blkNote = new DBObjectCollection();

            blkDB.Add(TextPloter.AddTitle(Point3d.Origin.C2D(80, 10), "立柱断面", "", 4, 2.5, 0.5));
            blkDB.Add(TextPloter.AddTitle(Point3d.Origin.C2D(50, 10), "跨中断面", "", 4, 2.5, 0.5));

            blkID.Add(BlockPloter.InsertBlockReference(Point3d.Origin.C3D(80), 1, "主拱断面", null, null));
            blkID.Add(BlockPloter.InsertBlockReference(Point3d.Origin.C3D(50), 1, "主拱断面跨中", null, null));

            var titleId = Ploter.WriteDatabase(blkDB);
            foreach (ObjectId item in titleId)
            {
                blkID.Add(item);
            }

            Extents2d BlkExt = Ploter.GetExtends(blkID);

            #endregion

            #region 图名注释
            blkNote = TextPloter.AddNoteFromFile(new Point3d(340, 140, 0), "txt\\主拱圈一般构造设计说明.txt", 66);
            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(205.9734, 15, 0), "主拱圈一般构造", 1, 5,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));
            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(361.1744, 15, 0), "C-1-05", 1, 5,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));

            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(380, 282, 0), "1", 1, 4,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));
            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(400, 282, 0), "1", 1, 4,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));

            Ploter.WriteDatabase(blkNote, "C-1-05 主拱圈一般构造");
            #endregion

            #region 布图
            Extents2d ext = ext1.Add(ext3);

            double w1 = ext.Width() * 1.02;
            Point2d minPt = new Point2d(30, 20);
            Point2d maxPt = minPt.C2D(w1, 267);

            db.CreatViewport(LayoutID, new Extents2d(minPt, maxPt), ext, 1);


            Point2d BlcCC = new Point2d(230, 150);
            double bW = BlkExt.Width() * 2;
            double bH = BlkExt.Height() * 2;

            db.CreatViewport(LayoutID, new Extents2d(BlcCC.C2D(-0.5 * bW, -0.5 * bH), BlcCC.C2D(0.5 * bW, 0.5 * bH)), BlkExt, 0.5);




            #endregion

            // 绘制边界参考
            db.AddEntityToModeSpace(ext3.ConvertRec());
        }

        [CommandMethod("ColumnDraw")]
        public void ColumnDraw()
        {
            if (theArchAxis == null)
            {
                return;
            }
            Database db = HostApplicationServices.WorkingDatabase;

            ObjectId LayoutID = db.CreatLayout("C-1-07 拱上立柱一般构造", "block\\TK.dwg");
            double vX = 231;
            double vY = archModel.Get3PointReal(vX, 90.0)[0].Y;
            double vH = archModel.Get3PointReal(vX, 90.0)[0].Y + 51.8 - 3 + archModel.Axis.f;


            #region 立面
            archModel.AddColumn(91, vX, vH, 4.0, 2.8, 3, 3, 1, 1.0, 2.0 + 0.6 + 0.5);
            archModel.AddColumn(92, vX, vH, 3.0, 2.8, 3, 3, 1, 1.0, 2.0 + 0.6 + 0.5);
            archModel.AddColumn(93, vX, vH, 2.0, 2.8, 3, 3, 1, 1.0, 2.0 + 0.6 + 0.5);
            Extents2d theExt = new Extents2d();
            Extents2d outExt;
            Model.Column theCol = archModel.ColumnList.Find(x => x.ID == 91);
            theCol.CalculateParameters();
            theCol.DarwColumnSide(new Point2d(-vX, -vY), out outExt, 0.25);
            TextPloterO.PlotTextWithLine(db, ref outExt, new Point3d(0, 52, 0), "A-A", 0.25, 3, "仿宋");
            theExt = theExt.Add(outExt);
            theCol.DrawColumnElev(db, new Point2d(-vX - 20, -vY), out outExt, 0.25);
            TextPloterO.PlotTextWithLine(db, ref outExt, new Point3d(-20, 52, 0), "立面", 0.25, 3, "仿宋");
            theExt = theExt.Add(outExt);

            Model.Column theCol92;
            theCol92 = archModel.ColumnList.Find(x => x.ID == 92);
            theCol92.CalculateParameters();
            theCol92.DarwColumnSide(new Point2d(-vX, -vY).Convert2D(15), out outExt, 0.25);
            TextPloterO.PlotTextWithLine(db, ref outExt, new Point3d(15, 52, 0), "B-B", 0.25, 3, "仿宋");
            theExt = theExt.Add(outExt);

            Model.Column theCol93;
            theCol92 = archModel.ColumnList.Find(x => x.ID == 93);
            theCol92.CalculateParameters();
            theCol92.DarwColumnSide(new Point2d(-vX, -vY).Convert2D(30), out outExt, 0.25);
            TextPloterO.PlotTextWithLine(db, ref outExt, new Point3d(30, 52, 0), "C-C", 0.25, 3, "仿宋");
            theExt = theExt.Add(outExt);

            #endregion

            #region 断面图
            var ScenterS = new Point2d(50, 30);
            Extents2d theExtS = new Extents2d(ScenterS, ScenterS);
            theCol.DrawSingleSection(db, ScenterS.Convert2D(-3), out outExt, 0.1);
            TextPloterO.PlotTextWithLine(db, ref outExt, ScenterS.Convert3D(-3, 2), "C-C", 0.1, 3, "仿宋");
            theExtS = theExtS.Add(outExt);
            theCol92.DrawSingleSection(db, ScenterS.Convert2D(3), out outExt, 0.1);
            TextPloterO.PlotTextWithLine(db, ref outExt, ScenterS.Convert3D(3, 2), "D-D", 0.1, 3, "仿宋");
            theExtS = theExtS.Add(outExt);
            #endregion

            #region 布图
            double w1 = theExt.Width() / 0.25 * 1.1;
            double w2 = 380 - w1;
            Point2d minPt = new Point2d(30, 20);
            Point2d maxPt = minPt.Convert2D(w1, 267);

            Point2d minPt2 = maxPt.Convert2D(0, -40 + 14.2 - theExtS.Height() / 0.1);
            Point2d maxPt2 = minPt2.Convert2D(w2, theExtS.Height() / 0.1);


            db.CreatViewport(LayoutID, new Extents2d(minPt, maxPt), theExt, 0.25);
            db.CreatViewport(LayoutID, new Extents2d(minPt2, maxPt2), theExtS, 0.1);
            #endregion

            #region 参数表

            DBObjectCollection tables = TablePloterO.CreatTable(db, minPt2.Convert2D(w2 * 0.5, -10), ref archModel.ColumnTable);

            foreach (var item in tables)
            {
                Entity ent = (Entity)item;

                db.AddEntityToPaperSpace(LayoutID, ent);

            }

            #endregion

            #region 图名注释
            DBObjectCollection blkNote;
            blkNote = TextPloter.AddNoteFromFile(new Point3d(275, 100, 0), "txt\\立柱构造设计说明.txt", 130);
            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(205.9734, 15, 0), "拱上立柱一般构造", 1, 5,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));
            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(361.1744, 15, 0), "C-1-07", 1, 5,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));

            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(380, 282, 0), "1", 1, 4,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));
            Extensions.Add(blkNote, TextPloter.AddDBText(new Point3d(400, 282, 0), "1", 1, 4,
                "H仿宋", 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid));

            Ploter.WriteDatabase(blkNote, "C-1-07 拱上立柱一般构造");
            #endregion



            db.AddEntityToModeSpace(theExt.ConvertRec());
            db.AddEntityToModeSpace(theExtS.ConvertRec());
        }


        [CommandMethod("GeneralArrangement")]
        public void GeneralArrangement()
        {


        }

        [CommandMethod("DrawSegment")]
        public void DrawSegment()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId LayoutID = db.CreatLayout("S4-06 主拱圈节段一般构造", "block\\TK.dwg");
            Extents2d ext1, ext2, ext3, ext4;
            archModel.DrawingSegment(out ext1, -119, -105);
        }

        [CommandMethod("ListArch")]
        public void ListArch()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            archModel.ListLeftElevation();
        }



        [CommandMethod("LockDoc", CommandFlags.Session)]
        public static void LockDoc()
        {
            // Create a new drawing
            DocumentCollection acDocMgr = Application.DocumentManager;
            Document acNewDoc = acDocMgr.Add("acad.dwt");
            Database acDbNewDoc = acNewDoc.Database;

            //// Lock the new document
            //using (DocumentLock acLckDoc = acNewDoc.LockDocument())
            //{
            //    // Start a transaction in the new database
            //    using (Transaction acTrans = acDbNewDoc.TransactionManager.StartTransaction())
            //    {
            //        // Open the Block table for read
            //        BlockTable acBlkTbl;
            //        acBlkTbl = acTrans.GetObject(acDbNewDoc.BlockTableId,
            //                                        OpenMode.ForRead) as BlockTable;

            //        // Open the Block table record Model space for write
            //        BlockTableRecord acBlkTblRec;
            //        acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
            //                                        OpenMode.ForWrite) as BlockTableRecord;

            //        // Create a circle with a radius of 3 at 5,5
            //        using (Circle acCirc = new Circle())
            //        {
            //            acCirc.Center = new Point3d(5, 5, 0);
            //            acCirc.Radius = 3;

            //            // Add the new object to Model space and the transaction
            //            acBlkTblRec.AppendEntity(acCirc);
            //            acTrans.AddNewlyCreatedDBObject(acCirc, true);
            //        }

            //        // Save the new object to the database
            //        acTrans.Commit();
            //    }

            //    // Unlock the document
            //}

            // Set the new document current

        }


    }
}
