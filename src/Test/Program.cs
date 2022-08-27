using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入f，按enter继续...");
            var fstr = Console.ReadLine();
            Console.WriteLine("\n请输入m，按enter继续...");
            var mstr = Console.ReadLine();

            Console.WriteLine("\n已输入：f={0:F2}，m={1:F2}", fstr, mstr);

            Console.WriteLine("\n请输入名称，按enter继续...");
            string name = Console.ReadLine();

            //ArchAxis bx = new ArchAxis(518 / 4.5, 2, 518);
            //var re= bx.Intersect(new Point2D(55, -10));

            double f = double.Parse(fstr);
            double m = double.Parse(mstr);

            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV4(out ax, m, 518 / (f));

            // 写出基准面
            // string name = "PhoenixModel";
            theArchModel.WriteMember(string.Format("{0}-member.lsp", name));

            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        try
            //        {
            //            theArchModel = NamedArch.PhoenixModel(out ax, 1.5 + i * 0.1, 518 / (3.5 + j * 0.1));
            //            Console.WriteLine("f={0},m={1},建模完成..\n", 3.5 + j * 0.1, 1.5 + i * 0.1);
            //        }
            //        catch (Exception)
            //        {
            //            Console.WriteLine("f={0},m={1},建模失败..\n", 3.5 + j * 0.1, 1.5 + i * 0.1);
            //            continue;
            //        }
            //    }
            //}

            return;

            //GenerateModel();
            //return;

        }

    }
}
