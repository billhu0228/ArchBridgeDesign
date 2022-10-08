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
        static void SpaceClaimProcedure()
        {
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV4(out ax, 2.0, 518 / 4.5,15.5,7);
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            SpaceClaimExt FluExt = new SpaceClaimExt(theFem);
            FluExt.WriteArcRib(Directory.CreateDirectory(
                Path.Combine("E:\\20210717 黑慧江拱桥两阶段设计\\", "051 CFD计算")).FullName, -259, 0,-259,0);

        }

        static void TrussProcedure()
        {
            List<double> Hfs = new List<double>() { 14.5, 15.0, 15.5, 16.0, 16.5, 17.0, 17.5 };
            List<double> Hts = new List<double>() { 5.50, 6.00, 6.50, 7.00, 7.50, 8.00, 8.50, };
            foreach (var Hf in Hfs)
            {
                foreach (var Ht in Hts)
                {
                    ANewModel(2.0, 4.5, Hf, Ht);
                    Console.WriteLine(string.Format("{0}-{1} Finish", Hf, Ht));
                }

            }
        }

        static void ANewModel(double m,double f,double Hfoot,double Htop)
        {
            #region 上部结构
            CompositeDeck DeckA1, DeckA2, DeckA3;
            CompositeDeck DeckB1, DeckB2, DeckB3;


            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1 = new List<double>() { -273, -231, -189, -147, -105 };
            List<double> sps2 = new List<double>() { -105, -63, -21, 21, 63, 105 };
            List<double> sps3 = new List<double>() { 105, 147, 189, 231, 273 };
            DeckA1 = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3 }, 6);
            DeckA2 = new CompositeDeck(sps2, ca, new List<int>() { 3, 4, 5, 6, 7, 8 }, 6);
            DeckA3 = new CompositeDeck(sps3, ca, new List<int>() { 8, 9, 10, 11, -1 }, 6);
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

            double DeckElevation = 11.0;
            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA1, 200000, 200000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckA2, 300000, 300000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckA3 = new FEMDeck(ref DeckA3, 400000, 400000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckB1 = new FEMDeck(ref DeckA1, 500000, 500000, 6, 2.5, 0, -12.76, DeckElevation);
            FEMDeck theFEMDeckB2 = new FEMDeck(ref DeckA2, 600000, 600000, 6, 2.5, 0, -12.76, DeckElevation);
            FEMDeck theFEMDeckB3 = new FEMDeck(ref DeckA3, 700000, 700000, 6, 2.5, 0, -12.76, DeckElevation);

            #endregion

            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV4(out ax,m, 518 / f, Hfoot,Htop);
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            AnsysExt ansysExt = new AnsysExt(theFem);
            string AnsysPathName = "E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\TrussHeight\\models\\";
            string CaseFolder = string.Format("{0:F1}-{1:F1}", Hfoot, Htop);
            string FullPath = Path.Combine(AnsysPathName, CaseFolder);
            if (Directory.Exists(FullPath))
            {
                Directory.Delete(FullPath, true);
            }
            ansysExt.WriteAnsys(Directory.CreateDirectory(FullPath).FullName);

            AnsysDeckExt ansysDeck = new AnsysDeckExt(theFEMDeckA1);
            ansysDeck.WriteAnsys(FullPath, "DeckA1.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckA2);
            ansysDeck.WriteAnsys(FullPath, "DeckA2.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckA3);
            ansysDeck.WriteAnsys(FullPath, "DeckA3.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckB1);
            ansysDeck.WriteAnsys(FullPath, "DeckB1.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckB2);
            ansysDeck.WriteAnsys(FullPath, "DeckB2.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckB3);
            ansysDeck.WriteAnsys(FullPath, "DeckB3.inp");
        }
        static void AnsysProcedure()
        {
            #region 上部结构
            CompositeDeck DeckA1, DeckA2, DeckA3;
            CompositeDeck DeckB1, DeckB2, DeckB3;


            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1 = new List<double>() { -273, -231, -189, -147, -105 };
            List<double> sps2 = new List<double>() { -105, -63, -21, 21, 63, 105 };
            List<double> sps3 = new List<double>() { 105, 147, 189, 231, 273 };
            DeckA1 = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3 }, 6);
            DeckA2 = new CompositeDeck(sps2, ca, new List<int>() { 3, 4, 5, 6, 7, 8 }, 6);
            DeckA3 = new CompositeDeck(sps3, ca, new List<int>() { 8, 9, 10, 11, -1 }, 6);
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

            double DeckElevation = 11.0;
            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA1, 200000, 200000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckA2, 300000, 300000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckA3 = new FEMDeck(ref DeckA3, 400000, 400000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckB1 = new FEMDeck(ref DeckA1, 500000, 500000, 6, 2.5, 0, -12.76, DeckElevation);
            FEMDeck theFEMDeckB2 = new FEMDeck(ref DeckA2, 600000, 600000, 6, 2.5, 0, -12.76, DeckElevation);
            FEMDeck theFEMDeckB3 = new FEMDeck(ref DeckA3, 700000, 700000, 6, 2.5, 0, -12.76, DeckElevation);

            #endregion

            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV4(out ax, 2.0, 518 / 4.5,15.5,7);
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            AnsysExt ansysExt = new AnsysExt(theFem);
            ansysExt.WriteAnsys(Directory.CreateDirectory(Path.Combine("E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\", "AnsysBin")).FullName);

            AnsysDeckExt ansysDeck = new AnsysDeckExt(theFEMDeckA1);
            ansysDeck.WriteAnsys("E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\AnsysBin", "DeckA1.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckA2);
            ansysDeck.WriteAnsys("E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\AnsysBin", "DeckA2.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckA3);
            ansysDeck.WriteAnsys("E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\AnsysBin", "DeckA3.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckB1);
            ansysDeck.WriteAnsys("E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\AnsysBin", "DeckB1.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckB2);
            ansysDeck.WriteAnsys("E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\AnsysBin", "DeckB2.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckB3);
            ansysDeck.WriteAnsys("E:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\AnsysBin", "DeckB3.inp");
            Console.ReadKey();
        }
        static void MidasProcedure()
        {
            #region 上部结构
            CompositeDeck DeckA1, DeckA2, DeckA3;
            CompositeDeck DeckB1, DeckB2, DeckB3;


            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1 = new List<double>() { -273, -231, -189, -147, -105 };
            List<double> sps2 = new List<double>() { -105, -63, -21, 21, 63, 105 };
            List<double> sps3 = new List<double>() { 105, 147, 189, 231, 273 };
            DeckA1 = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3 }, 6);
            DeckA2 = new CompositeDeck(sps2, ca, new List<int>() { 3, 4, 5, 6, 7, 8 }, 6);
            DeckA3 = new CompositeDeck(sps3, ca, new List<int>() { 8, 9, 10, 11, -1 }, 6);
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

            double DeckElevation = 11.0;
            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA1, 200000, 200000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckA2, 300000, 300000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckA3 = new FEMDeck(ref DeckA3, 400000, 400000, 6, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckB1 = new FEMDeck(ref DeckA1, 500000, 500000, 6, 2.5, 0, -12.76, DeckElevation);
            FEMDeck theFEMDeckB2 = new FEMDeck(ref DeckA2, 600000, 600000, 6, 2.5, 0, -12.76, DeckElevation);
            FEMDeck theFEMDeckB3 = new FEMDeck(ref DeckA3, 700000, 700000, 6, 2.5, 0, -12.76, DeckElevation);

            #endregion

            ArchAxis ax;
            Arch theArchModel;
            double Hf, Ht;
            Hf = 12;
            Ht = 12;
            theArchModel = NamedArch.PhoenixModelV4(out ax, 2.0, 518 / 4.5, Hf, Ht);
            // theArchModel = NamedArch.PhoenixModelV3(out ax, 2.0, 518 /4.5); //初设模型
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            // AnsysExt ansysExt = new AnsysExt(theFem);
            // ansysExt.WriteAnsys(Directory.CreateDirectory(Path.Combine("G:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\", "AnsysBin")).FullName);
            MidasExt midasExt = new MidasExt(theFem);

            string savePath = string.Format("E:\\20210717 黑慧江拱桥两阶段设计\\01 总体计算\\Midas\\mct\\{0:F1}-{1:F1}", Hf, Ht);
            if (Directory.Exists(savePath))
            {
                Directory.Delete(savePath, true);
            }
            Directory.CreateDirectory(savePath);
            midasExt.WriteMidas(Path.Combine(savePath,string.Format( "Arch-1400-20-45-{0:F1}-{1:F1}.mct",Hf,Ht)));

            MidasDeckExt midasDeckExt = new MidasDeckExt(theFEMDeckA1);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckA1.mct"));
            midasDeckExt = new MidasDeckExt(theFEMDeckA2);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckA2.mct"));
            midasDeckExt = new MidasDeckExt(theFEMDeckA3);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckA3.mct"));
            midasDeckExt = new MidasDeckExt(theFEMDeckB1);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckB1.mct"));
            midasDeckExt = new MidasDeckExt(theFEMDeckB2);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckB2.mct"));
            midasDeckExt = new MidasDeckExt(theFEMDeckB3);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckB3.mct"));
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            //SpaceClaimProcedure();
            //AnsysProcedure();
            // MidasProcedure();
            TrussProcedure();
        }
    }
}
