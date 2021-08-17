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
            if (args.Count()!=0)
            {
                double f = double.Parse(args[0]);
                double m = double.Parse(args[1]);
                ModelT1(f, m, "Output");
            }
            else
            {
                ModelT1(4.0, 1.85, "Test");
            }
        }

        static void ModelT1(double f,double m,string name)
        {
            #region 基本建模步骤
            // 1. 设置拱系
            ArchAxis theArchAxis = new ArchAxis(518 / f, m, 518);
            Arch archModel = new Arch(theArchAxis, 8, 15, 12, 4);
            // 2. 生成桁架
            archModel.GenerateTruss(10, 50, 10, 7.8, 11, 2);
            archModel.GenerateMiddleDatum(); //生成中插平面
            // 2.1补充
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
            // 4. **生成上下弦骨架**
            archModel.GenerateSkeleton();
            // 5. 生成斜杆基准面
            archModel.GenerateDiagonalDatum(0.060);
            // 6. 整理主拱模型
            archModel.GenerateArch();

            // 7. 立柱建模                        
            archModel.AddColumn(0, -225.0, 105.2, 1.6,2.8, 3.0, 3, 1, 1, 2.7, 0.5);
            var s2 = new TubeSection(0.6, 0.016);
            archModel.AssignProperty(eMemberType.ColumnMain, s2);
            // 7.1 整理立柱模型
            archModel.GenerateColumn();
            #endregion


            // 写出基准面
            archModel.WriteDiagonalDatum(string.Format("{0}-dg.lsp",name));
            archModel.WriteMainDatum(string.Format("{0}-mg.lsp", name));
            archModel.WriteMember(string.Format("{0}-member.lsp", name));
        }
    }
}
