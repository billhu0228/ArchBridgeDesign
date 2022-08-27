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
            CompositeDeck DeckA1, DeckA2,DeckA3;
            CompositeDeck DeckB1, DeckB2,DeckB3;
          
          
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1 = new List<double>() { -273, -231, -189, -147, -105 };
            List<double> sps2 = new List<double>() { -105, -63, -21, 21, 63, 105 };
            List<double> sps3 = new List<double>() { 105, 147, 189, 231, 273 };
            DeckA1 = new CompositeDeck(sps1, ca,new List<int>() {-1,0,1,2,3 }, 6);
            DeckA2 = new CompositeDeck(sps2, ca, new List<int>() { 3, 4, 5, 6, 7,8 },6);
            DeckA3 = new CompositeDeck(sps3, ca, new List<int>() { 8, 9, 10, 11, -1 },6);
            DeckB1 = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3 }, 6);
            DeckB2 = new CompositeDeck(sps2, ca, new List<int>() { 3, 4, 5, 6, 7, 8 }, 6);
            DeckB3 = new CompositeDeck(sps3, ca, new List<int>() { 8, 9, 10, 11, -1 }, 6);
            var Decks = new List<CompositeDeck>() { DeckA1, DeckA2, DeckA3, DeckB1, DeckB2, DeckB3 };
            foreach (var item in Decks)
            {
                item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.0, 0.060, 0.060, 0.020));
                item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                item.AddSection("EndBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                item.AddSection("UpBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
            }

            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA1, 200000, 200000, 6, 2.5, 0, 0.2, 22);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckA2, 300000, 300000, 6, 2.5, 0, 0.2, 22);
            FEMDeck theFEMDeckA3 = new FEMDeck(ref DeckA3, 400000, 400000, 6, 2.5, 0, 0.2, 22);
            FEMDeck theFEMDeckB1 = new FEMDeck(ref DeckA1, 500000, 500000, 6, 2.5, 0, -12.76, 22);
            FEMDeck theFEMDeckB2 = new FEMDeck(ref DeckA2, 600000, 600000, 6, 2.5, 0, -12.76, 22);
            FEMDeck theFEMDeckB3 = new FEMDeck(ref DeckA3, 700000, 700000, 6, 2.5, 0, -12.76, 22);

            #endregion

            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV4(out ax, 2.0, 518 /4.5);
            // theArchModel = NamedArch.PhoenixModelV3(out ax, 2.0, 518 /5); //初设模型
            FEMModel theFem = new FEMModel(ref theArchModel,ref ca,0.4);
            // AnsysExt ansysExt = new AnsysExt(theFem);
            // ansysExt.WriteAnsys(Directory.CreateDirectory(Path.Combine("G:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\", "AnsysBin")).FullName);
            MidasExt midasExt = new MidasExt(theFem);
            midasExt.WriteMidas("C:\\temp\\Arch.mct");

            MidasDeckExt midasDeckExt = new MidasDeckExt(theFEMDeckA1);
            midasDeckExt.WriteMidas("C:\\temp\\DeckA1.mct");
            midasDeckExt = new MidasDeckExt(theFEMDeckA2);
            midasDeckExt.WriteMidas("C:\\temp\\DeckA2.mct");
            midasDeckExt = new MidasDeckExt(theFEMDeckA3);
            midasDeckExt.WriteMidas("C:\\temp\\DeckA3.mct");

            midasDeckExt = new MidasDeckExt(theFEMDeckB1);
            midasDeckExt.WriteMidas("C:\\temp\\DeckB1.mct");
            midasDeckExt = new MidasDeckExt(theFEMDeckB2);
            midasDeckExt.WriteMidas("C:\\temp\\DeckB2.mct");
            midasDeckExt = new MidasDeckExt(theFEMDeckB3);
            midasDeckExt.WriteMidas("C:\\temp\\DeckB3.mct");
            Console.ReadKey();
        }
    }
}
