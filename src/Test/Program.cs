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
            ArchAxis aa = new ArchAxis(100, 1.5, 500);
            
            var f=aa.ArchLength;
        }
    }
}
