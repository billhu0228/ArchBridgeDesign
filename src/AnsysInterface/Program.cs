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


            #region 上部结构
            CompositeDeck DeckA, DeckB;
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1 = new List<double>() { -273, -231, -189, -147, -105 };
            List<double> sps2 = new List<double>() { -105, -64, -42, 42, 63, 105 };
            DeckA = new CompositeDeck(sps1, ca, 6);
            DeckB = new CompositeDeck(sps2, ca, 6);

            DeckA.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.0, 0.060, 0.060, 0.020));
            DeckA.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
            DeckA.AddSection("EndBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
            DeckA.AddSection("UpBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));

            DeckB.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.0, 0.060, 0.060, 0.020));
            DeckB.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
            DeckB.AddSection("EndBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
            DeckB.AddSection("UpBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));

            FEMDeck theDeck = new FEMDeck(ref DeckA, 2, 1.25, 0, 0, 0);


            #endregion
            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV3(out ax, 2.0, 518 / 5.0);
            FEMModel theFem = new FEMModel(ref theArchModel);
            // AnsysExt ansysExt = new AnsysExt(theFem);
            // ansysExt.WriteAnsys(Directory.CreateDirectory(Path.Combine("G:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\", "AnsysBin")).FullName);
            MidasExt midasExt = new MidasExt(theFem);
            midasExt.WriteMidas("C:\\temp\\Test2.mct");
            Console.ReadKey();
        }
    }
}
