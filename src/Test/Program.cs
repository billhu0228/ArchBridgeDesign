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




            var s1 = new TubeSection(1.36, 0.035);
            m1.AssignProperty(MemberType.UpperCoord, s1);
            m1.AssignProperty(MemberType.LowerCoord, s1);

            var ff=m1.get_diagnal_sectin(134.65, 141.85, 0.08, 0.9);


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
