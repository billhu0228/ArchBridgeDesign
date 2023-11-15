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
            theArchModel = NamedArch.PhoenixModelV63(out ax, 2.0, 518 / 4.5, 15.5, 7.0);
            CompositeDeck DeckA;
            CompositeDeck DeckB;
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 3.9);
            theFem.CastBOQTable(ref theTable);
            theTable.SaveCSV("..\\..\\..\\..\\data\\out_boq_new(20230801).csv");
            return;
        }
    }
}
