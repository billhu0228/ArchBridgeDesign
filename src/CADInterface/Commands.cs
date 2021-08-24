﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADInterface.Plotters;
using CADInterface.UI;
using CADInterface.API;
using Autodesk.AutoCAD.ApplicationServices;

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
            A3Config.IniConfig();
        }

        [CommandMethod("GenModel")]
        public void GenerateModel()
        {
            // properDia.ShowDialog();

            #region 基本步骤
            // 1. 设置拱系
            theArchAxis = new ArchAxis(518 / 5.0, 1.55, 518);
            archModel = new Arch(theArchAxis, 8.5, 17, 12, 4);
            // 2. 生成桁架
            archModel.GenerateTruss(10, 50, 10, 7.8, 11, 2);
            archModel.GenerateMiddleDatum(); //生成中插平面
            // 补充
            archModel.AddDatum(0, -518 * 0.5, eDatumType.ControlDatum);
            archModel.AddDatum(0, 518 * 0.5, eDatumType.ControlDatum);

            archModel.AddDatum(0, -252.2, eDatumType.NormalDatum);
            archModel.AddDatum(0, 252.2, eDatumType.NormalDatum);

            var dx = 1.0 / Math.Cos(archModel.Axis.GetAngle(-518 * 0.5).Radians);
            archModel.AddDatum(0, -518 * 0.5 - dx, eDatumType.ControlDatum);
            archModel.AddDatum(0, +518 * 0.5 + dx, eDatumType.ControlDatum);


            // 3. 配置截面
            var s1 = new TubeSection(1.4, 0.035);
            archModel.AssignProperty(eMemberType.UpperCoord, s1);
            archModel.AssignProperty(eMemberType.LowerCoord, s1);
            archModel.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.CrossBraceing, new TubeSection(0.7, 0.016));
            // 4. 生成上下弦骨架
            archModel.GenerateSkeleton();
            // 5. 生成斜杆基准面
            archModel.GenerateDiagonalDatum(0.060);
            // 6. 生成模型
            archModel.GenerateArch();
            // 7. 立柱建模
            double h0 =15.5;
            foreach (var item in archModel.MainDatum)
            {
                if (item.DatumType==eDatumType.ColumnDatum)
                {
                    var x = item.Center.X;
                    var relH = archModel.Axis.f + h0;

                    archModel.AddColumn(0,x, relH, 1.6, 2.8, 3.0, 3, 1, 1, 2.7, 0.5);
                }

            }
            
            var s2 = new TubeSection(0.6, 0.016);
            var s3= new TubeSection(0.4, 0.016);

            archModel.AssignProperty(eMemberType.ColumnMain, s2);
            archModel.AssignProperty(eMemberType.ColumnCrossL, s3);
            archModel.AssignProperty(eMemberType.ColumnCrossW, s3);

            archModel.GenerateColumn();

            #endregion
        }

        [CommandMethod("GA")]
        public void DrawingGeneralArrangment()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            ObjectId LayoutID = db.CreatLayout("S4-03 主拱圈一般构造","block\\TK.dwg");
            Extents2d theExt = new Extents2d();

            archModel.DrawingLeftElevation(db,ref theExt);

            // 绘制拼接区及标注
            archModel.DrawInstall(db, ref theExt);
            // 绘制立柱

            foreach (var theCol in archModel.ColumnList)
            {
                if (theCol.ID==0)
                {
                    if (theCol.X<0)
                    {
                        theCol.DarwColumnSide(db, Point2d.Origin, out theExt);
                    }
                }
            }

            // 绘制立柱

            //archModel.dr(db, ref theExt);

            //archModel.DrawingPlan(db, new Point2d(0, -200), ref theExt);

            // 绘制边界参考
            //db.AddEntityToModeSpace(new Line(theExt.MinPoint.Convert3D(), theExt.MaxPoint.Convert3D()));
        }

        [CommandMethod("ColumnDraw")]
        public void ColumnDraw()
        {
            if (theArchAxis==null)
            {
                return;
            }
            Database db = HostApplicationServices.WorkingDatabase;

            ObjectId LayoutID = db.CreatLayout("S4-04 立柱一般构造", "block\\TK.dwg");
            double vX = archModel.MainDatum.Find(x => x.DatumType == eDatumType.ColumnDatum).Center.X;
            double vY = archModel.Get3PointReal(vX, 90.0)[0].Y;
            double vH = archModel.Get3PointReal(vX, 90.0)[0].Y+51.8-3+archModel.Axis.f;


            #region 立面
            archModel.AddColumn(91, vX, vH, 1.6, 2.8, 3, 3, 1, 1.0, 2.0+0.6+0.5);
            archModel.AddColumn(92, vX, vH, 2.0, 2.8, 3, 3, 1, 1.0, 2.0+0.6+0.5);
            Extents2d theExt = new Extents2d();
            Extents2d outExt;
            Model.Column theCol = archModel.ColumnList.Find(x => x.ID == 91);
            theCol.CalculateParameters();
            theCol.DarwColumnSide(db, new Point2d(-vX, -vY), out outExt, 0.25);
            TextPloter.PlotTextWithLine(db, ref outExt, new Point3d(0, 52, 0), "A-A", 0.25, 3, "仿宋");
            theExt = theExt.Add(outExt);
            theCol.DrawColumnElev(db, new Point2d(-vX-20, -vY), out outExt, 0.25);
            TextPloter.PlotTextWithLine(db, ref outExt, new Point3d(- 20, 52,0), "立面", 0.25, 3, "仿宋");
            theExt = theExt.Add(outExt);
            Model.Column theCol92;
            theCol92 = archModel.ColumnList.Find(x => x.ID == 92);
            theCol92.CalculateParameters();
            theCol92.DarwColumnSide(db, new Point2d(-vX, -vY).Convert2D(15), out outExt, 0.25);
            TextPloter.PlotTextWithLine(db, ref outExt, new Point3d(15, 52, 0), "B-B", 0.25, 3, "仿宋");
            theExt = theExt.Add(outExt);
            #endregion

            #region 断面图
            var ScenterS = new Point2d(50, 30);
            Extents2d theExtS = new Extents2d(ScenterS, ScenterS);
            theCol.DrawSingleSection(db, ScenterS.Convert2D(-3), out outExt, 0.1);
            TextPloter.PlotTextWithLine(db, ref outExt, ScenterS.Convert3D(-3,2), "C-C", 0.1, 3, "仿宋");
            theExtS = theExtS.Add(outExt);
            theCol92.DrawSingleSection(db, ScenterS.Convert2D(3), out outExt, 0.1);
            TextPloter.PlotTextWithLine(db, ref outExt, ScenterS.Convert3D(3, 2), "D-D", 0.1, 3, "仿宋");
            theExtS = theExtS.Add(outExt);
            #endregion

            #region 布图
            double w1 = theExt.Width() / 0.25*1.1;
            double w2 = 380 - w1;
            Point2d minPt = new Point2d(30, 20);
            Point2d maxPt = minPt.Convert2D(w1, 267);

            Point2d minPt2 = maxPt.Convert2D(0, -40+14.2 - theExtS.Height() / 0.1);
            Point2d maxPt2 = minPt2.Convert2D(w2, theExtS.Height() / 0.1);


            db.CreatViewport(LayoutID, new Extents2d(minPt, maxPt), theExt, 0.25);
            db.CreatViewport(LayoutID, new Extents2d(minPt2, maxPt2), theExtS, 0.1);
            #endregion

            #region 参数表

            DBObjectCollection tables = TablePloter.CreatTable(db, minPt2.Convert2D(w2*0.5,-10),ref archModel.ColumnTable);

            foreach (var item in tables)
            {
                Entity ent = (Entity)item;

                db.AddEntityToPaperSpace(LayoutID,ent);

            }


            //TablePloter.DraTable(db, ref theExtS, new Point3d(), new List<string>() { "A", "B" }, 
            //    2, ref kw, ref kw , "仿宋", 1, new Dictionary<int, List<string>>() { { 1, new List<string>() { "M", "N" } } });





            #endregion






            db.AddEntityToModeSpace(theExt.ConvertRec());
            db.AddEntityToModeSpace(theExtS.ConvertRec());
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
