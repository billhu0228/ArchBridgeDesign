using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
namespace CADInterface
{
    /// <summary>
    /// ACad静态扩展
    /// </summary>
    public static class AcadExt
    {

        public static void AddDimStyle(this Database db,Unit plotunit, Unit paperunit,double scale, string tstyle)
        {
            Dictionary<Unit, double> UnitDic = new Dictionary<Unit, double>()
            {
                { Unit.Meter,1000 },
                { Unit.Centimeter,10 },
                { Unit.Millimeter,1 },
            };      
            Dictionary<Unit, string> UnitDicStr = new Dictionary<Unit, string>()
            {
                { Unit.Meter,"M" },
                { Unit.Centimeter,"CM" },
                { Unit.Millimeter,"MM" },
            };
            
            double factor = UnitDic[plotunit]/UnitDic[paperunit];

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DimStyleTable dst = tr.GetObject(db.DimStyleTableId, OpenMode.ForWrite) as DimStyleTable;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                string DimStyleName=string.Format("{0}-{1}-",UnitDicStr[plotunit],UnitDicStr[paperunit]);
                if (scale < 1)
                {
                    DimStyleName += Math.Round(1 / scale, 0).ToString() + "-1";
                }
                else
                {
                    DimStyleName += "1-" + scale.ToString();
                }
                DimStyleTableRecord dstr = new DimStyleTableRecord();
                if (!dst.Has(DimStyleName))
                {
                    dstr.Name = DimStyleName;
                    dstr.Dimscale = scale;
                    dstr.Dimtxsty = st[tstyle];
                    dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                    dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                    dstr.Dimdli = 5;
                    dstr.Dimsah = true;
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
                    dstr.Dimlfac = factor;          
                    dst.Add(dstr);
                    tr.AddNewlyCreatedDBObject(dstr, true);
                }
                else
                {
                    dstr = tr.GetObject(dst[DimStyleName], OpenMode.ForWrite) as DimStyleTableRecord;
                    dstr.Dimscale = scale;
                    dstr.Dimtxsty = st[tstyle];
                    dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                    dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                    dstr.Dimdli = 5;
                    dstr.Dimsah = true;                 
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
                    dstr.Dimlfac = factor;
                }

                tr.Commit();
            }
        }


        /// <summary>
        /// 获得Line中点Point3D.
        /// </summary>
        /// <param name="aline">目标线.</param>       

        public static Point3d GetMidPoint3d(this Line aline, double xx = 0, double yy = 0)
        {
            double x = 0.5 * (aline.StartPoint.X + aline.EndPoint.X);
            double y = 0.5 * (aline.StartPoint.Y + aline.EndPoint.Y);
            return new Point3d(x + xx, y + yy, 0);
        }

        public static Point2d GetXPoint2d(this Line aline, double part = 0.5, double xx = 0, double yy = 0)
        {
            double x = aline.StartPoint.X + part * (aline.EndPoint.X - aline.StartPoint.X);
            double y = aline.StartPoint.Y + part * (aline.EndPoint.Y - aline.StartPoint.Y);
            return new Point2d(x + xx, y + yy);
        }
        /// <summary>
        /// 获得中点
        /// </summary>
        /// <param name="aline"></param>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <returns></returns>
        public static Point2d GetMidPoint2d(this Line aline, double xx = 0, double yy = 0)
        {
            double x = 0.5 * (aline.StartPoint.X + aline.EndPoint.X);
            double y = 0.5 * (aline.StartPoint.Y + aline.EndPoint.Y);
            return new Point2d(x + xx, y + yy);
        }

        /// <summary>
        /// 设置视口的尺寸，位置和比例
        /// </summary>
        public static void SetSizeAndLoc(this Viewport vp, double w, double h, Point2d cc, double scale)
        {
            vp.Width = w;
            vp.Height = h;
            vp.CenterPoint = cc.Convert3D();
            vp.CustomScale = 1.0 / scale;
            vp.Locked = true;
            vp.ViewDirection = new Vector3d(0, 0, 1);
        }
        public static void CreatViewport(this Database db ,ObjectId LayoutId,
            Extents2d PaperExt,Extents2d SpaceExt,double scale)
        {
            double w = PaperExt.MaxPoint.X - PaperExt.MinPoint.X;
            double h = PaperExt.MaxPoint.Y - PaperExt.MinPoint.Y;
            Point2d vpCenter = PaperExt.Center();
            Viewport vpA = new Viewport();
            vpA.Layer = "Defpoints";
            vpA.SetSizeAndLoc(w, h, vpCenter, scale);
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Layout lay = (Layout)tr.GetObject(LayoutId, OpenMode.ForWrite);
                UcsTable ut = tr.GetObject(db.UcsTableId, OpenMode.ForWrite) as UcsTable;
                var btr = (BlockTableRecord)tr.GetObject(lay.BlockTableRecordId, OpenMode.ForWrite);
                btr.AppendEntity(vpA);
                tr.AddNewlyCreatedDBObject(vpA, true);
                vpA.ViewCenter = SpaceExt.Center();
                vpA.On = true;                
                tr.Commit();
            }
        }

        public static bool XrefAttachAndInsert(this Database db, string path, ObjectId paperSpaceId, Point3d pos, string name = null)
        {
            var ret = false;
            if (!File.Exists(path))
                return ret;

            if (String.IsNullOrEmpty(name))
                name = Path.GetFileNameWithoutExtension(path);

            try
            {
                using (var tr = db.TransactionManager.StartOpenCloseTransaction())
                {
                    var xId = db.AttachXref(path, name);
                    if (xId.IsValid)
                    {
                        Layout tmp = (Layout)tr.GetObject(paperSpaceId, OpenMode.ForWrite);
                        var btr = (BlockTableRecord)tr.GetObject(tmp.BlockTableRecordId, OpenMode.ForWrite);
                        var br = new BlockReference(pos, xId);
                        btr.AppendEntity(br);
                        tr.AddNewlyCreatedDBObject(br, true);
                        ret = true;
                    }
                    tr.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            { }

            return ret;
        }
        /// <summary>
        /// 引入并插入参照（模型空间）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="path"></param>
        /// <param name="paperSpaceId"></param>
        /// <param name="pos"></param>
        /// <param name="scale"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool XrefAttachAndInsertModel(this Database db, string path, ObjectId paperSpaceId, Point3d pos, Scale3d scale, string name = null)
        {
            var ret = false;
            if (!File.Exists(path))
                return ret;

            if (String.IsNullOrEmpty(name))
                name = Path.GetFileNameWithoutExtension(path);

            try
            {
                using (var tr = db.TransactionManager.StartOpenCloseTransaction())
                {
                    var xId = db.AttachXref(path, name);
                    if (xId.IsValid)
                    {
                        //Layout tmp = (Layout)tr.GetObject(paperSpaceId, OpenMode.ForWrite);
                        var btr = (BlockTableRecord)tr.GetObject(paperSpaceId, OpenMode.ForWrite);
                        var br = new BlockReference(pos, xId);


                        br.ScaleFactors = scale;//设置块参照的缩放比例
                        btr.AppendEntity(br);
                        tr.AddNewlyCreatedDBObject(br, true);
                        ret = true;
                    }
                    tr.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            { }

            return ret;
        }

        public static void SetPlotSettings(this Layout lay, string pageSize, string styleSheet, string device)
        {
            using (var ps = new PlotSettings(lay.ModelType))
            {
                ps.CopyFrom(lay);
                var psv = PlotSettingsValidator.Current;
                // Set the device
                var devs = psv.GetPlotDeviceList();
                if (devs.Contains(device))
                {
                    psv.SetPlotConfigurationName(ps, device, null);
                    psv.RefreshLists(ps);
                }
                // Set the media name/size
                var mns = psv.GetCanonicalMediaNameList(ps);
                if (mns.Contains(pageSize))
                {
                    psv.SetCanonicalMediaName(ps, pageSize);
                }
                psv.SetPlotWindowArea(ps, new Extents2d(new Point2d(0, 0), new Point2d(420, 297)));
                //psv.SetPlotRotation(ps, PlotRotation.Degrees000);//设置横向;纵向Degrees270

                //psv.SetStdScale(ps, 1);
                //psv.SetPlotType(ps, PlotType.Layout);//设置打印范围

                psv.RefreshLists(ps);
                psv.SetPlotRotation(ps, PlotRotation.Degrees000);

                //psv.SetPlotCentered(ps, true);
                psv.SetCustomPrintScale(ps, new CustomScale(1, 1));
                psv.SetPlotType(ps, PlotType.Window);
                psv.SetPlotPaperUnits(ps, PlotPaperUnit.Millimeters);//设置的单位
                psv.SetStdScaleType(ps, StdScaleType.StdScale1To1);//设置比例
                //psv.SetPlotCentered(ps, true);
                // Set the pen settings
                var ssl = psv.GetPlotStyleSheetList();
                if (ssl.Contains(styleSheet))
                {
                    psv.SetCurrentStyleSheet(ps, styleSheet);
                }
                // Copy the PlotSettings data back to the Layout

                var upgraded = false;
                if (!lay.IsWriteEnabled)
                {
                    lay.UpgradeOpen();
                    upgraded = true;
                }
                lay.CopyFrom(ps);
                if (upgraded)
                {
                    lay.DowngradeOpen();
                }
            }
        }
        public static void SetA1PlotSettings(this Layout lay, string pageSize, string styleSheet, string device)
        {
            using (var ps = new PlotSettings(lay.ModelType))
            {

                ps.CopyFrom(lay);
                var psv = PlotSettingsValidator.Current;
                // Set the device
                var devs = psv.GetPlotDeviceList();
                if (devs.Contains(device))//设置设备
                {
                    psv.SetPlotConfigurationName(ps, device, null);
                    psv.RefreshLists(ps);
                }
                // Set the media name/size
                var mns = psv.GetCanonicalMediaNameList(ps);

                if (mns.Contains(pageSize))//设置纸张大小
                {
                    psv.SetCanonicalMediaName(ps, pageSize);
                }
                //psv.SetPlotWindowArea(ps, new Extents2d(new Point2d(0, 0), new Point2d(841, 594)));//设置打印纸张大小范围
                //                                                                                   //psv.SetPlotType(ps, PlotType.Window);//设置打印范围
                //                                                                                   //psv.SetStdScale(ps, 1);//设置标准比例

                //psv.SetCustomPrintScale(ps, new CustomScale(1, 1));
                //psv.SetUseEnScale(ps, true);
                psv.SetStdScaleType(ps, StdScaleType.StdScale1To1);//设置比例
                psv.SetPlotPaperUnits(ps, PlotPaperUnit.Millimeters);//设置的单位
                //psv.SetPlotRotation(ps, PlotRotation.Degrees000);//设置横向;纵向Degrees270

                //psv.SetStdScale(ps, 1);
                //psv.SetPlotType(ps, PlotType.Layout);//设置打印范围

                psv.RefreshLists(ps);
                psv.SetPlotRotation(ps, PlotRotation.Degrees000);
                psv.SetCustomPrintScale(ps, new CustomScale(1, 1));
                psv.SetPlotWindowArea(ps, new Extents2d(1, 0, 841, 594));
                psv.SetPlotType(ps, PlotType.Window);
                var ssl = psv.GetPlotStyleSheetList();
                if (ssl.Contains(styleSheet))
                {
                    psv.SetCurrentStyleSheet(ps, styleSheet);
                    //psv.
                }
                // Copy the PlotSettings data back to the Layout

                var upgraded = false;
                if (!lay.IsWriteEnabled)
                {
                    lay.UpgradeOpen();
                    upgraded = true;
                }
                lay.CopyFrom(ps);
                if (upgraded)
                {
                    lay.DowngradeOpen();
                }
            }
        }

        public static ObjectId CreatLayout(this Database db, string Name, bool isA1 = false)
        {

            ObjectId curLayId;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBDictionary lays = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForWrite) as DBDictionary;

                if (!lays.Contains(Name))
                {
                    curLayId = LayoutManager.Current.CreateLayout(Name);
                }
                else
                {
                    LayoutManager.Current.DeleteLayout(Name);
                    curLayId = LayoutManager.Current.CreateLayout(Name);
                    //curLayId = LayoutManager.Current.GetLayoutId(Name);
                }

                LayoutManager.Current.CurrentLayout = Name;
                var lay = (Layout)tr.GetObject(curLayId, OpenMode.ForWrite);
                if (!isA1)
                    lay.SetPlotSettings("ISO_full_bleed_A3_(420.00_x_297.00_MM)", "monochrome.ctb", "DWG To PDF.pc3");
                else
                    lay.SetA1PlotSettings("ISO_full_bleed_A1_(841.00_x_594.00_MM)", "monochrome.ctb", "DWG To PDF.pc3");
                tr.Commit();
            }
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayoutManager LM = LayoutManager.Current;
                DBDictionary LayoutDict = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
                Layout CurrentLo = tr.GetObject(curLayId, OpenMode.ForRead) as Layout;
                BlockTableRecord BlkTblRec = tr.GetObject(CurrentLo.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId ID in BlkTblRec)
                {
                    Viewport VP = tr.GetObject(ID, OpenMode.ForRead) as Viewport;
                    if (VP != null)
                    {
                        VP.UpgradeOpen();
                        VP.Erase();
                    }
                }

                if (LM.LayoutExists("布局1"))
                {
                    LM.DeleteLayout("布局1");
                }
                if (LM.LayoutExists("布局2"))
                {
                    LM.DeleteLayout("布局2");
                }
                LM.CurrentLayout = "Model";

                tr.Commit();
            }

            return curLayId;
        }
        public static ObjectId CreatLayout(this Database db, string Name, string TKPath, bool isA1 = false, int NumPDF = 1)
        {
            ObjectId curLayId;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBDictionary lays = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForWrite) as DBDictionary;

                if (!lays.Contains(Name))
                {
                    curLayId = LayoutManager.Current.CreateLayout(Name);
                }
                else
                {
                    LayoutManager.Current.DeleteLayout(Name);
                    curLayId = LayoutManager.Current.CreateLayout(Name);
                    //curLayId = LayoutManager.Current.GetLayoutId(Name);
                }
                for (int page = 0; page < NumPDF; page++)
                {
                    if (isA1)
                        db.XrefAttachAndInsert(TKPath, curLayId, Point3d.Origin.Convert3D(page * 841));
                    else
                        db.XrefAttachAndInsert(TKPath, curLayId, Point3d.Origin.Convert3D(page * 420));
                }
                LayoutManager.Current.CurrentLayout = Name;
                var lay = (Layout)tr.GetObject(curLayId, OpenMode.ForWrite);
                if (!isA1)
                    lay.SetPlotSettings("ISO_full_bleed_A3_(420.00_x_297.00_MM)", "monochrome.ctb", "DWG To PDF.pc3");
                else
                    lay.SetA1PlotSettings("ISO_full_bleed_A1_(841.00_x_594.00_MM)", "monochrome.ctb", "DWG To PDF.pc3");
                tr.Commit();
            }
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayoutManager LM = LayoutManager.Current;
                DBDictionary LayoutDict = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
                Layout CurrentLo = tr.GetObject(curLayId, OpenMode.ForRead) as Layout;
                BlockTableRecord BlkTblRec = tr.GetObject(CurrentLo.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId ID in BlkTblRec)
                {
                    Viewport VP = tr.GetObject(ID, OpenMode.ForRead) as Viewport;
                    if (VP != null)
                    {
                        VP.UpgradeOpen();
                        VP.Erase();
                    }
                }

                if (LM.LayoutExists("布局1"))
                {
                    LM.DeleteLayout("布局1");
                }
                if (LM.LayoutExists("布局2"))
                {
                    LM.DeleteLayout("布局2");
                }
                if (LM.LayoutExists("layout1"))
                {
                    LM.DeleteLayout("layout1");
                }
                if (LM.LayoutExists("layout2"))
                {
                    LM.DeleteLayout("layout2");
                }
                LM.CurrentLayout = "Model";

                tr.Commit();
            }

            return curLayId;
        }

        public static Point2d Convert2D(this Point3d theP3d, double x = 0, double y = 0)
        {
            return new Point2d(theP3d.X + x, theP3d.Y + y);
        }
        public static Point3d Convert3D(this Point3d theP3d, double x = 0, double y = 0, double z = 0)
        {
            return new Point3d(theP3d.X + x, theP3d.Y + y, theP3d.Z + z);
        }
        public static Vector3d Convert3D(this Vector2d theV2d, double x = 0, double y = 0)
        {
            return new Vector3d(theV2d.X + x, theV2d.Y + y, 0);
        }
        public static Vector2d Convert2D(this Vector3d theV3d,double x=0,double y = 0)
        {
            return new Vector2d(theV3d.X + x, theV3d.Y + y);
        }
        public static Point3d Convert3D(this Point2d theP2d, double x = 0, double y = 0)
        {
            return new Point3d(theP2d.X + x, theP2d.Y + y, 0);
        }
        public static Point2d Convert2D(this Point2d theP2d, double x = 0, double y = 0)
        {
            return new Point2d(theP2d.X + x, theP2d.Y + y);
        }
        public static Point2d MoveDistance(this Point2d theP2d, Vector2d theVec, double dist)
        {
            Vector2d newVec = new Vector2d(dist * Math.Cos(theVec.Angle), dist * Math.Sin(theVec.Angle));
            return theP2d.TransformBy(Matrix2d.Displacement(newVec));
        }
        public static double GetK(this Line cL)
        {
            double k = 0;
            k = (cL.EndPoint.Y - cL.StartPoint.Y) / (cL.EndPoint.X - cL.StartPoint.X);
            return k;
        }

        public static Extents2d Add(this Extents2d ori, Extents2d other)
        {
            double minX = Math.Min(ori.MinPoint.X, other.MinPoint.X);
            double minY = Math.Min(ori.MinPoint.Y, other.MinPoint.Y);
            double maxX = Math.Max(ori.MaxPoint.X, other.MaxPoint.X);
            double maxY = Math.Max(ori.MaxPoint.Y, other.MaxPoint.Y);
            return new Extents2d(minX, minY, maxX, maxY);
        }

        public static Polyline ConvertRec(this Extents2d ori)
        {
            Polyline ret = new Polyline() { Closed=true};

            double w = ori.MaxPoint.X - ori.MinPoint.X;
            double b = ori.MaxPoint.Y - ori.MinPoint.Y;

            ret.AddVertexAt(0, ori.MinPoint, 0, 0, 0);
            ret.AddVertexAt(1, ori.MinPoint.Convert2D(0,b), 0, 0, 0);
            ret.AddVertexAt(2, ori.MaxPoint, 0, 0, 0);
            ret.AddVertexAt(3, ori.MaxPoint.Convert2D(0,-b), 0, 0, 0);

            return ret;
        }

        public static double Width(this Extents2d ext)
        {
            return ext.MaxPoint.X - ext.MinPoint.X;
        }
        public static double Height(this Extents2d ext)
        {
            return ext.MaxPoint.Y - ext.MinPoint.Y;
        }

        public static Point2d Center(this Extents2d ext)
        {
            return ext.MinPoint + (ext.MaxPoint - ext.MinPoint) * 0.5;
        }

        public static Point2d UpCenter(this Extents2d ext)
        {
            return ext.Center().Convert2D(0, 0.5 * ext.Height());
        }

        public static Extents2d Convert2D(this Extents3d ext)
        {
            return new Extents2d(ext.MinPoint.Convert2D(), ext.MaxPoint.Convert2D());
        }

        /// <summary>
        /// 将图形对象添加到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">图形对象</param>
        /// <returns>图形的ObjectId</returns>
        public static ObjectId AddEntityToModeSpace(this Database db, Entity ent)
        {
            // 声明ObjectId 用于返回
            ObjectId entId = ObjectId.Null;
            // 开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                // 添加图形到块表记录
                entId = btr.AppendEntity(ent);
                // 更新数据信息
                trans.AddNewlyCreatedDBObject(ent, true);
                // 提交事务
                trans.Commit();
            }
            return entId;
        }


        /// <summary>
        /// 将图形对象添加到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">图形对象</param>
        /// <returns>图形的ObjectId</returns>
        public static ObjectId AddEntityToPaperSpace(this Database db,ObjectId layout, Entity ent)
        {
            // 声明ObjectId 用于返回
            ObjectId entId = ObjectId.Null;
            // 开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Layout lay = (Layout)trans.GetObject(layout, OpenMode.ForWrite);
                var btr = (BlockTableRecord)trans.GetObject(lay.BlockTableRecordId, OpenMode.ForWrite);
                // 添加图形到块表记录
                entId = btr.AppendEntity(ent);
                // 更新数据信息
                trans.AddNewlyCreatedDBObject(ent, true);
                // 提交事务
                trans.Commit();
            }
            return entId;
        }


        public static Polyline CreatFromList( List<Point2d> ptList)
        {
            Polyline line = new Polyline() { Closed = false };//定义不封闭的Polyline
            for (int i = 0; i < ptList.Count; i++)
            {
                line.AddVertexAt(i, ptList[i], 0, 0, 0);
            }
            return line;
        }

        public static Line UpdateBoundary(this Line theLine, Curve BD1, Curve BD2)
        {
            Point3d St = theLine.StartPoint;
            Point3d Ed = theLine.EndPoint;

            Point3dCollection res = new Point3dCollection();
            if (BD1 != null)
            {
                theLine.IntersectWith(BD1, Intersect.ExtendThis, res, IntPtr.Zero, IntPtr.Zero);

                if (res.Count == 1)
                {
                    St = res[0];
                }
                else if (res.Count == 2)
                {
                    if (res[0].DistanceTo(Ed) < res[1].DistanceTo(Ed))
                    {
                        St = res[0];
                    }
                    else
                    {
                        St = res[1];
                    }
                }
            }

            if (BD2 != null)
            {
                res = new Point3dCollection();

                theLine.IntersectWith(BD2, Intersect.ExtendThis, res, IntPtr.Zero, IntPtr.Zero);


                if (res.Count == 1)
                {
                    Ed = res[0];
                }
                else if (res.Count==2)
                {
                    if (res[0].DistanceTo(St)<res[1].DistanceTo(St))
                    {
                        Ed = res[0];
                    }
                    else
                    {
                        Ed = res[1];
                    }
                }
            }
            Line ret= new Line(St, Ed);
            ret.Layer = theLine.Layer;
            return ret;
        }


        public static double GetWidth(this DBText dbtext)
        {
            if (string.IsNullOrEmpty(dbtext.TextString)) return 0;
            double a = Math.Abs(dbtext.GeometricExtents.MaxPoint.Y - dbtext.GeometricExtents.MinPoint.Y);
            double b = Math.Abs(dbtext.GeometricExtents.MaxPoint.X - dbtext.GeometricExtents.MinPoint.X);
            double angle = new Vector2d(Math.Abs(Math.Cos(dbtext.Rotation)), Math.Abs(Math.Sin(dbtext.Rotation))).GetAngleTo(Vector2d.XAxis);
            double[] t = new double[9]
            {
                 Math.Sin(angle),Math.Cos(angle),0,
                 Math.Cos(angle),Math.Sin(angle),0,
                 0,0,1
            };
            Matrix2d mat = new Matrix2d(t);
            Vector2d vec = mat.Inverse() * new Vector2d(a, b);
            return Math.Abs(vec.X);
        }
        public static double GetHeight(this DBText dbtext)
        {
            if (string.IsNullOrEmpty(dbtext.TextString)) return 0;
            double a = Math.Abs(dbtext.GeometricExtents.MaxPoint.Y - dbtext.GeometricExtents.MinPoint.Y);
            double b = Math.Abs(dbtext.GeometricExtents.MaxPoint.X - dbtext.GeometricExtents.MinPoint.X);
            double angle = new Vector2d(Math.Abs(Math.Cos(dbtext.Rotation)), Math.Abs(Math.Sin(dbtext.Rotation))).GetAngleTo(Vector2d.XAxis);
            double[] t = new double[9]
            {
                 Math.Sin(angle),Math.Cos(angle),0,
                 Math.Cos(angle),Math.Sin(angle),0,
                 0,0,1
            };
            Matrix2d mat = new Matrix2d(t);
            Vector2d vec = mat.Inverse() * new Vector2d(a, b);
            return Math.Abs(vec.Y);
        }

       
    }
}
