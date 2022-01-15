using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnsysInterface
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ArchAxis ax;
            Arch theArchModel;
            theArchModel= NamedArch.PhoenixModel(out ax, 2.0, 518 / 4.0);
            //theArchModel = Arch.PreliminaryDesignModel(out ax);
            FEMModel theFem = new FEMModel(ref theArchModel);
            //theFem.WriteAnsys(Directory.CreateDirectory(Path.Combine("C:\\Users\\IBD2\\", "AnsysBin")).FullName);
            theFem.WriteMidas("C:\\Users\\IBD2\\Desktop\\Test.mct");
            var t = 1;
        }
    }
}
