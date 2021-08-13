using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(CADInterface.Commands))]
namespace CADInterface
{
    public class Commands
    {
        [CommandMethod("init")]
        public void InitDrawingSetting()
        {
            A3Config.IniConfig();
        }

        [CommandMethod("GA")]
        public void DrawingGeneralArrangment()
        {
            /// 基本步骤
            // 1. 设置拱系
            ArchAxis theArchAxis = new ArchAxis(418 / 5.0, 1.55, 518);
            Arch m1 = new Arch(theArchAxis, 8.5, 17);
            // 2. 生成桁架
            m1.GenerateTruss(10, 50, 10, 7.8, 11, 2);
            m1.GenerateMiddleDatum(); //生成中插平面
            // 补充
            m1.AddDatum(0, -518 * 0.5, eDatumType.NormalDatum);
            m1.AddDatum(0, 518 * 0.5, eDatumType.NormalDatum);

            m1.AddDatum(0, -252, eDatumType.NormalDatum);
            m1.AddDatum(0, 252, eDatumType.NormalDatum);

            // 3. 生成上下弦骨架
            m1.GenerateSkeleton();
            // 4. 配置截面
            var s1 = new TubeSection(1.4, 0.035);
            m1.AssignProperty(eMemberType.UpperCoord, s1);
            m1.AssignProperty(eMemberType.LowerCoord, s1);
            m1.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));

            // 5. 生成斜杆基准面
            m1.GenerateDiagonalDatum(0.060);
            // 6. 生成模型
            m1.GenerateModel();

            Extents2d theExt = new Extents2d();

            m1.Drawing(HostApplicationServices.WorkingDatabase,ref theExt);



        }





    }
}
