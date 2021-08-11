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
        }

        static void ModelT1(double f,string name)
        {
            /// 基本步骤
            /// 1. 设置拱系
            /// 2. 配置H0,H1等，生成拱对象
            /// 3. 为拱对象添加关键截面，添加桁片杆件尺寸；
            /// 4. 利用关键截面生成节段对象；
            ArchAxis theArchAxis = new ArchAxis(518 / f, 2, 518);
            Arch m1 = new Arch(theArchAxis, 8.5, 17);

            double x0 = -225;
            for (int i = 0; i < 10; i++)
            {
                double xi = x0 + i * 50;
                m1.AddDatum(0, xi, eDatumType.ColumnDatum, 90.0);
                if (i == 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        m1.AddDatum(0, xi - 11 * (j + 1), eDatumType.VerticalDatum, 90);
                    }
                }
                if (i == 4)
                {
                    for (int jj = 0; jj < 3; jj++)
                    {
                        m1.AddDatum(0, xi + 7.8 * (jj + 1), eDatumType.VerticalDatum, 90);
                        m1.AddDatum(0, 1.6 + 7.8 * (jj), eDatumType.VerticalDatum, 90);
                    }

                }
                else if (i == 9)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        m1.AddDatum(0, xi + 11 * (j + 1), eDatumType.VerticalDatum, 90);
                    }

                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        m1.AddDatum(0, xi + (j + 1) * 10.0, eDatumType.VerticalDatum, 90);
                    }
                }
            }

            m1.GenerateMiddleDatum(); //生成中插平面

            m1.AddDatum(0, -518 * 0.5, eDatumType.NormalDatum);
            m1.AddDatum(0, 518 * 0.5, eDatumType.NormalDatum);

            m1.AddDatum(0, -253, eDatumType.NormalDatum);
            m1.AddDatum(0, 253, eDatumType.NormalDatum);

            // 生成上下弦骨架
            m1.GenerateSkeleton();

            var s1 = new TubeSection(1.4, 0.035);
            m1.AssignProperty(eMemberType.UpperCoord, s1);
            m1.AssignProperty(eMemberType.LowerCoord, s1);
            m1.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));

            // 生成斜杆基准面
            m1.GenerateDiagonalDatum(0.060);
            // 写出基准面
            m1.WriteDiagonalDatum(string.Format("../{0}-dg.lsp",name));
            m1.WriteMainDatum(string.Format("../{0}-mg.lsp", name));

            // 生成模型
            m1.GenerateModel();

            m1.WriteMember(string.Format("../{0}-member.lsp", name));


        }
    }
}
