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
            ModelT1(4.5, "F45");
            ModelT1(5.5, "F55");
        }

        static void ModelT1(double f,string name)
        {
            /// 基本步骤
            // 1. 设置拱系
            ArchAxis theArchAxis = new ArchAxis(418 / f, 1.55, 518);
            Arch m1 = new Arch(theArchAxis, 8.5, 17);
            // 2. 生成桁架
            m1.GenerateTruss(10, 50, 10, 7.8, 11,2);
            m1.GenerateMiddleDatum(); //生成中插平面
            // 补充
            m1.AddDatum(0, -518 * 0.5, eDatumType.NormalDatum);
            m1.AddDatum(0, 518 * 0.5, eDatumType.NormalDatum);

            m1.AddDatum(0, -253, eDatumType.NormalDatum);
            m1.AddDatum(0, 253, eDatumType.NormalDatum);

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


            // 写出基准面
            m1.WriteDiagonalDatum(string.Format("../{0}-dg.lsp",name));
            m1.WriteMainDatum(string.Format("../{0}-mg.lsp", name));
            m1.WriteMember(string.Format("../{0}-member.lsp", name));

        }
    }
}
