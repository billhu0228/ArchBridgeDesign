using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace BOQInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            ArchAxis ax;
            Arch theArchModel;
            BOQTable theTable = new BOQTable();
            theArchModel = NamedArch.PhoenixModelV3(out ax, 2.0, 518 / 5.0);
            FEMModel theFem = new FEMModel(ref theArchModel);
            theFem.CastBOQTable(ref theTable);
            theTable.SaveCSV("..\\..\\..\\..\\data\\out_boq_new.csv");
            return;
        }
    }
}
