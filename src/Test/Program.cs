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
            /// 基本步骤
            /// 1. 设置拱系
            /// 2. 配置H0,H1等，生成拱对象
            /// 3. 为拱对象添加关键截面，添加桁片杆件尺寸；
            /// 4. 利用关键截面生成节段对象；
            /// 
            ArchAxis theArchAxis = new ArchAxis(518/4.5, 2, 518);
            Arch m1 = new Arch(theArchAxis, 8.5, 17);

            double x0 = -225;
            for (int i = 0; i < 10; i++)
            {
                double xi = x0 + i * 50;
                m1.AddDatum(0, xi, eDatumType.ColumnDatum, 90.0);
                if (i==0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        m1.AddDatum(0, xi - 11 * (j + 1), eDatumType.VerticalDatum, 90);
                    }

                }
                if (i==4)
                {
                    for (int jj = 0; jj < 3; jj++)
                    {
                        m1.AddDatum(0,xi + 7.8 * (jj + 1), eDatumType.VerticalDatum, 90);
                        m1.AddDatum(0,1.6 + 7.8 * (jj), eDatumType.VerticalDatum, 90);
                    }            

                }
                else if (i==9)
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
                        m1.AddDatum(0, xi + (j + 1) *10.0, eDatumType.VerticalDatum, 90);
                    }
                }
            }

            m1.GenerateMiddleDatum(); //生成中插平面

            m1.AddDatum(0,-518*0.5,  eDatumType.NormalDatum);
            m1.AddDatum(0,518*0.5,  eDatumType.NormalDatum);


            var s1 = new TubeSection(1.36, 0.035);
            m1.AssignProperty(eMemberType.UpperCoord, s1);
            m1.AssignProperty(eMemberType.LowerCoord, s1);

            m1.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            m1.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.7, 0.024),double.NegativeInfinity,0);
            m1.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.5, 0.024),0,double.PositiveInfinity);

            m1.GenerateDiagonalDatum();

            m1.WriteDiagonalDatum("../dg.lsp");

            m1.WriteControlPoint("../1.csv");

            


            //var ff=m1.get_diagnal_sectin(134.65, 141.85, 0.08, 0.9);


            //m1.AddSection(1, 0, 90, SectionType.InstallSection);




            //Point2D f1, f2, f3;
            //var kk=m1.Get7Point(50,60);


            var f=theArchAxis.ArchLength;



            //Point2D v1, v2, v3;
            //using (StreamWriter sw = new StreamWriter("D:/Coords.csv"))
            //{
            //    for (int i = 0; i < 551; i++)
            //    {
            //        double x0 = -275 + i;
            //        var rr = m1.get_3pt(x0);
            //        v1 = rr[0];
            //        v2 = rr[1];
            //        v3 = rr[2];
            //        sw.WriteLine(string.Format("{0:F},{1:F},{2:F},{3:F},{4:F},{5:F}", v1.X, v1.Y, v2.X, v2.Y, v3.X, v3.Y));

            //    }
            //}

        }
    }
}
