using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
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
            ArchAxis theArchAxis = new ArchAxis(100, 1.5, 500);
            Arch m1 = new Arch(theArchAxis, 7, 14);
            m1.AddSection(1, 0, 90, SectionType.InstallSection);


            var s1 = new TubeSection(1.36, 0.035);
            m1.AssignProperty(MemberType.UpperCoord, s1, -100, 100);
            m1.AssignProperty(MemberType.LowerCoord, s1);

            Point2D f1, f2, f3;
            m1.Get3Point(0, 30,out f1, out f2, out f3);

            var f=theArchAxis.ArchLength;
        }
    }
}
