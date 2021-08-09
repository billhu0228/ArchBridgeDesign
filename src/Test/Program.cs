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
            m1.AddSection(1, 0, Math.PI * 0.5, SectionType.InstallSection);


            var f=theArchAxis.ArchLength;
        }
    }
}
