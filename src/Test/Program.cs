using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {

        static void Main(string[] args) 
        {
            ModelT1(5.0, "F50");
            //ModelT1(4.5, "F45");
            //ModelT1(5.5, "F55");
        }

        static void ModelT1(double f,string name)
        {
            /// 基本步骤
            // 1. 设置拱系
            ArchAxis theArchAxis = new ArchAxis(418 / 5.0, 1.55, 518);
            Arch m1 = new Arch(theArchAxis, 8.5, 17, 12, 4);
            // 2. 生成桁架
            m1.GenerateTruss(10, 50, 12.5, 7.8, 11, 2);
            m1.GenerateMiddleDatum(); //生成中插平面
            // 补充
            m1.AddDatum(0, -518 * 0.5, eDatumType.ControlDatum);
            m1.AddDatum(0, 518 * 0.5, eDatumType.ControlDatum);

            m1.AddDatum(0, -252.2, eDatumType.NormalDatum);
            m1.AddDatum(0, 252.2, eDatumType.NormalDatum);

            var dx = 1.0 / Math.Cos(m1.Axis.GetAngle(-518 * 0.5).Radians);
            m1.AddDatum(0, -518 * 0.5 - dx, eDatumType.ControlDatum);
            m1.AddDatum(0, +518 * 0.5 + dx, eDatumType.ControlDatum);


            // 3. 生成上下弦骨架
            m1.GenerateSkeleton();
            // 4. 配置截面
            var s1 = new TubeSection(1.4, 0.035);
            var h1 = new HSection(0.4, 0.4, 0.4, 0.05, 0.05, 0.03);
            m1.AssignProperty(eMemberType.UpperCoord, s1);
            m1.AssignProperty(eMemberType.LowerCoord, s1);
            m1.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.CrossBraceing, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.WindBraceingH, h1);


            // 5. 生成斜杆基准面
            m1.GenerateDiagonalDatum(0.060);
            // 6. 生成模型
            m1.GenerateModel();


            // 写出基准面
            m1.WriteDiagonalDatum(string.Format("../{0}-dg.lsp",name));
            m1.WriteMainDatum(string.Format("../{0}-mg.lsp", name));
            m1.WriteMember(string.Format("../{0}-member.lsp", name));

        }
    }
}
