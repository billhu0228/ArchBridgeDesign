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


            // 3. 生成上下弦骨架
            archModel.GenerateSkeleton();
            // 4. 配置截面
            var s1 = new TubeSection(1.4, 0.035);
            archModel.AssignProperty(eMemberType.UpperCoord, s1);
            archModel.AssignProperty(eMemberType.LowerCoord, s1);
            archModel.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.CrossBraceing, new TubeSection(0.7, 0.016));

            // 5. 生成斜杆基准面
            archModel.GenerateDiagonalDatum(0.060);
            // 6. 生成模型
            archModel.GenerateModel();
            #endregion
        }

        [CommandMethod("GA")]
        public void DrawingGeneralArrangment()
        {
            Database db = HostApplicationServices.WorkingDatabase;




            ObjectId LayoutID = db.CreatLayout("S4-03 主拱圈一般构造","..\\..\\resource\\TK.dwg");
            Extents2d theExt = new Extents2d();

            archModel.DrawingLeftElevation(db,ref theExt);

            // 绘制拼接区及标注
            archModel.DrawInstall(db, ref theExt);

            // 绘制立柱

            archModel.DarwColumn(db, ref theExt);

            //archModel.DrawingPlan(db, new Point2d(0, -200), ref theExt);

            // 绘制边界参考
            //db.AddEntityToModeSpace(new Line(theExt.MinPoint.Convert3D(), theExt.MaxPoint.Convert3D()));
        }





    }
}
