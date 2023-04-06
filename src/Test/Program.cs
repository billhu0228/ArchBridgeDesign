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
            ArchAxis theArchAxis;
            // var archModel = NamedArch.PhoenixModelV63(out theArchAxis, 2.0, 518 / 4.5, 15.5, 7.0);

            Console.WriteLine("请输入f，按enter继续...");
            var fstr = Console.ReadLine();
            Console.WriteLine("\n请输入m，按enter继续...");
            var mstr = Console.ReadLine();
            Console.WriteLine("\n请输入Hfoot，按enter继续...");
            var hfstr = Console.ReadLine();
            Console.WriteLine("\n请输入Htop，按enter继续...");
            var htstr = Console.ReadLine();

            Console.WriteLine("\n已输入：f={0:F2}，m={1:F2},拱脚高度={2:F1}m，拱顶高度={3:F1}m", fstr, mstr,hfstr,htstr);

            Console.WriteLine("\n请输入名称，按enter继续...");
            string name = Console.ReadLine();

            double f = double.Parse(fstr);
            double m = double.Parse(mstr);
            double hf = double.Parse(hfstr);
            double ht = double.Parse(htstr);

            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV63(out ax, m, 518 / (f),hf,ht);

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
