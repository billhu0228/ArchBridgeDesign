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
            theArchModel = Arch.PreliminaryDesignModel(out ax);
            theArchModel.CastBOQTable(ref theTable);
            theTable.SaveCSV("..\\..\\..\\..\\data\\out_boq.csv");
            return;
        }
    }
}
