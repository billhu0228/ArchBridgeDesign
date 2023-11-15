using MathNet.Numerics;
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
            double Hf, Ht, m, f, ColDist, CrossBeamDist;
            Hf = 15.5;
            Ht = 7.0;
            m = 2.0;
            f = 4.5;
            ColDist = 49.5;// 或者49.5；
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV63(out ax, m, 518 / f, Hf, Ht);
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 3.9);
            SpaceClaimExt FluExt = new SpaceClaimExt(theFem);
            FluExt.WriteArcRib(Directory.CreateDirectory(
                Path.Combine("G:\\20210717 黑慧江拱桥两阶段设计\\", "051 CFD计算")).FullName, -359, 0, -359, 0);

        }
        static void TrussProcedure()
        {
            var Hts = Generate.LinearSpaced(9, 5, 9);
            var Hfs = Generate.LinearSpaced(10, 12, 16.5);
            //  List<double> Hfs = new List<double>() { 10, 12, 14.5, 15.0, 15.5, 16.0, 16.5, 17.0, 17.5 ,18 };
            // List<double> Hts = new List<double>() { 5.50, 6.00, 6.50, 7.00, 7.50, 8.00, 8.50, 10, };
            foreach (var Hf in Hfs)
            {
                foreach (var Ht in Hts)
                {
                    ANewModel(2.0, 4.5, Hf, Ht);
                    Console.WriteLine(string.Format("{0}-{1} Finish", Hf, Ht));
                }

            }
        }
        static void ANewModel(double m, double f, double Hfoot, double Htop)
        {
            #region 上部结构
            CompositeDeck DeckA, DeckB;

            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1 = new List<double>() { -270.60, -221.40, -172.20, -123.00, -73.80, -24.60, 24.60, 73.80, 123.00, 172.20, 221.40, 270.60, };
            DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 5);
            DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 5);
            var Decks = new List<CompositeDeck>() { DeckA, DeckB };
            foreach (var item in Decks)
            {
                item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.7, 0.060, 0.060, 0.020));
                item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                item.AddSection("EndBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                item.AddSection("UpBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
            }

            double DeckElevation = 11.0;
            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA, 200000, 200000, 5, 2.5, 0, 0.2, DeckElevation);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckB, 300000, 300000, 5, 2.5, 0, -12.76, DeckElevation);
            #endregion

            #region 拱
            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV62(out ax, m, 518 / f, Hfoot, Htop);
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            AnsysExt ansysExt = new AnsysExt(theFem);
            #endregion

            string AnsysPathName = "G:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\";
            string CaseFolder = string.Format("{0:F1}-{1:F1}", Hfoot, Htop);
            string FullPath = Path.Combine(AnsysPathName, CaseFolder);
            if (Directory.Exists(FullPath))
            {
                Directory.Delete(FullPath, true);
            }
            ansysExt.WriteAnsys(Directory.CreateDirectory(FullPath).FullName);
            AnsysDeckExt ansysDeck = new AnsysDeckExt(theFEMDeckA1);
            ansysDeck.WriteAnsys(FullPath, "DeckA.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckA2);
            ansysDeck.WriteAnsys(FullPath, "DeckB.inp");
        }
        static void AnsysProcedure()
        {
            double ColDist, CrossBeamDist;
            ColDist = 49.5;// 或者49.5；
            #region 上部结构
            CompositeDeck DeckA;
            CompositeDeck DeckB;
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1;
            double DeckElevation = 9.0;
            if (ColDist == 42.0)
            {
                sps1 = new List<double>() { -273, -231, -189, -147, -105, -63, -21, 21, 63, 105, 147, 189, 231, 273 };
                DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, -1 }, 6);
                DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, -1 }, 6);
                CrossBeamDist = 6.0;
            }
            else if (ColDist == 49.5)
            {
                sps1 = new List<double>() { -272.25, -222.75, -173.25, -123.75, -74.25, -24.75, 24.75, 74.25, 123.75, 173.25, 222.75, 272.25, };
                DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
                DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
                CrossBeamDist = 4.95;
            }
            else
            {
                throw new Exception();
            }
            var Decks = new List<CompositeDeck>() { DeckA, DeckB };
            foreach (var item in Decks)
            {
                if (ColDist == 42.0)
                {
                    item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.0, 0.060, 0.060, 0.020));
                    item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                    item.AddSection("EndBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                    item.AddSection("UpBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                }
                else
                {
                    item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.5, 0.060, 0.060, 0.020));
                    item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                    item.AddSection("EndBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
                    item.AddSection("UpBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
                }

            }
            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA, 200000, 200000, CrossBeamDist, 2.5, 0, 1.95, DeckElevation);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckB, 300000, 300000, CrossBeamDist, 2.5, 0, -14.5, DeckElevation);
            #endregion

            ArchAxis ax;
            Arch theArchModel;
            double Hfoot = 15.5;
            double Htop = 7.0;
            theArchModel = NamedArch.PhoenixModelV63(out ax, 2.0, 518 / 4.5, Hfoot, Htop);
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 0.4);
            AnsysExt ansysExt = new AnsysExt(theFem);


            string AnsysPathName = "G:\\20210717 黑慧江拱桥两阶段设计\\05 Ansys\\";
            string CaseFolder = string.Format("{0:F1}-{1:F1}", Hfoot, Htop);
            string FullPath = Path.Combine(AnsysPathName, CaseFolder);
            if (Directory.Exists(FullPath))
            {
                Directory.Delete(FullPath, true);
            }

            ansysExt.WriteAnsys(Directory.CreateDirectory(Path.Combine(FullPath)).FullName);

            AnsysDeckExt ansysDeck = new AnsysDeckExt(theFEMDeckA1);
            ansysDeck.WriteAnsys(FullPath, "DeckA.inp");
            ansysDeck = new AnsysDeckExt(theFEMDeckA2);
            ansysDeck.WriteAnsys(FullPath, "DeckB.inp");
            Console.WriteLine("Ansys Procedure Finish!\nPress any key to Exit.");
            Console.ReadKey();
        }
        static void MidasProcedure()
        {
            ArchAxis ax;
            Arch theArchModel;
            double Hf, Ht, m, f, ColDist, CrossBeamDist;
            Hf = 15.5;
            Ht = 7.0;
            m = 2.0;
            f = 4.5;
            ColDist = 49.5;// 或者49.5；

            #region 上部结构
            CompositeDeck DeckA;
            CompositeDeck DeckB;
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1;
            double DeckElevation = 9.0;
            if (ColDist == 42.0)
            {
                sps1 = new List<double>() { -273, -231, -189, -147, -105, -63, -21, 21, 63, 105, 147, 189, 231, 273 };
                DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, -1 }, 6);
                DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, -1 }, 6);
                CrossBeamDist = 6.0;
            }
            else if (ColDist == 49.5)
            {
                sps1 = new List<double>() { -272.25, -222.75, -173.25, -123.75, -74.25, -24.75, 24.75, 74.25, 123.75, 173.25, 222.75, 272.25, };
                DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
                DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
                CrossBeamDist = 4.95;
            }
            else
            {
                throw new Exception();
            }
            var Decks = new List<CompositeDeck>() { DeckA, DeckB };
            foreach (var item in Decks)
            {
                if (ColDist == 42.0)
                {
                    item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.0, 0.060, 0.060, 0.020));
                    item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                    item.AddSection("EndBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                    item.AddSection("UpBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                }
                else
                {
                    item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.5, 0.060, 0.060, 0.020));
                    item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                    item.AddSection("EndBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
                    item.AddSection("UpBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
                }

            }
            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA, 200000, 200000, CrossBeamDist, 2.5, 0, 1.95, DeckElevation);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckB, 300000, 300000, CrossBeamDist, 2.5, 0, -14.5, DeckElevation);
            #endregion


            if (ColDist == 42.0)
            {
                theArchModel = NamedArch.PhoenixModelV4(out ax, m, 518 / f, Hf, Ht);
            }
            else if (ColDist == 49.5)
            {
                theArchModel = NamedArch.PhoenixModelV63(out ax, m, 518 / f, Hf, Ht);
            }
            else
            {
                throw new Exception();
            }
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 3.9);
            MidasExt midasExt = new MidasExt(theFem);

            string savePath = string.Format("G:\\20210717 黑慧江拱桥两阶段设计\\01 总体计算\\Midas\\03-施设总体计算\\C{4:F0}M{0:F0}F{1:F0}-{2:F1}-{3:F1}", m * 10, f * 10, Hf, Ht, ColDist * 10);
            if (Directory.Exists(savePath))
            {
                Directory.Delete(savePath, true);
            }
            Directory.CreateDirectory(savePath);
            midasExt.WriteMidas(Path.Combine(savePath, string.Format("Arch-1400-{0:F1}-{1:F1}-{2:F1}.mct", m, Hf, Ht)));
            midasExt.WriteMidas2021(Path.Combine(savePath, string.Format("(2021)Arch-1400-{0:F1}-{1:F1}-{2:F1}.mct", m, Hf, Ht)));
            MidasDeckExt midasDeckExt = new MidasDeckExt(theFEMDeckA1);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckA.mct"));
            midasDeckExt.WriteMidas2021(Path.Combine(savePath, "(2021)DeckA.mct"));
            midasDeckExt = new MidasDeckExt(theFEMDeckA2);
            midasDeckExt.WriteMidas(Path.Combine(savePath, "DeckB.mct"));
            midasDeckExt.WriteMidas2021(Path.Combine(savePath, "(2021)DeckB.mct"));
            midasExt.WriteLoads(Path.Combine(savePath, "其他荷载.mct"), ref theFEMDeckA1, ref theFEMDeckA2);
            midasExt.WriteLiveLoad(Path.Combine(savePath, "移动荷载.mct"), ref theFEMDeckA1, ref theFEMDeckA2);
            midasExt.WriteFTLoad(Path.Combine(savePath, "疲劳荷载.mct"), ref theFEMDeckA1, ref theFEMDeckA2);
            midasExt.WriteNodeInfo(Path.Combine(savePath, "NodeInfomation.csv"));
            Console.ReadKey();
        }
        private static void OpenSEESProcedure()
        {
            ArchAxis ax;
            Arch theArchModel;
            double Hf, Ht, m, f, ColDist, CrossBeamDist;
            Hf = 15.5;
            Ht = 7.0;
            m = 2.0;
            f = 4.5;
            ColDist = 49.5;// 或者49.5；

            #region 上部结构
            CompositeDeck DeckA;
            CompositeDeck DeckB;
            List<double> g1 = new List<double>() { 1.275, 5, 5, 1.275 };
            List<double> g2 = new List<double>() { 3.775, 5, 3.775 };
            CrossArrangement ca = new CrossArrangement(g1.Sum(), 0.25, 0.05, 0.3, 0, g1, g2);
            List<double> sps1;
            double DeckElevation = 9.0;
            if (ColDist == 42.0)
            {
                sps1 = new List<double>() { -273, -231, -189, -147, -105, -63, -21, 21, 63, 105, 147, 189, 231, 273 };
                DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, -1 }, 6);
                DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, -1 }, 6);
                CrossBeamDist = 6.0;
            }
            else if (ColDist == 49.5)
            {
                sps1 = new List<double>() { -272.25, -222.75, -173.25, -123.75, -74.25, -24.75, 24.75, 74.25, 123.75, 173.25, 222.75, 272.25, };
                DeckA = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
                DeckB = new CompositeDeck(sps1, ca, new List<int>() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1 }, 4.95);
                CrossBeamDist = 4.95;
            }
            else
            {
                throw new Exception();
            }
            var Decks = new List<CompositeDeck>() { DeckA, DeckB };
            foreach (var item in Decks)
            {
                if (ColDist == 42.0)
                {
                    item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.0, 0.060, 0.060, 0.020));
                    item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                    item.AddSection("EndBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                    item.AddSection("UpBeam", new HSection(1, 0.3, 0.3, 0.8, 0.020, 0.020, 0.015));
                }
                else
                {
                    item.AddSection("MGider", new HSection(1, 0.6, 0.6, 2.5, 0.060, 0.060, 0.020));
                    item.AddSection("SGider", new HSection(1, 0.3, 0.3, 0.3, 0.010, 0.010, 0.008));
                    item.AddSection("EndBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
                    item.AddSection("UpBeam", new HSection(1, 0.4, 0.4, 1.2, 0.020, 0.020, 0.015));
                }

            }
            FEMDeck theFEMDeckA1 = new FEMDeck(ref DeckA, 200000, 200000, CrossBeamDist, 2.5, 0, 1.95, DeckElevation);
            FEMDeck theFEMDeckA2 = new FEMDeck(ref DeckB, 300000, 300000, CrossBeamDist, 2.5, 0, -14.5, DeckElevation);
            #endregion


            if (ColDist == 42.0)
            {
                theArchModel = NamedArch.PhoenixModelV4(out ax, m, 518 / f, Hf, Ht);
            }
            else if (ColDist == 49.5)
            {
                theArchModel = NamedArch.PhoenixModelV63(out ax, m, 518 / f, Hf, Ht);
            }
            else
            {
                throw new Exception();
            }
            FEMModel theFem = new FEMModel(ref theArchModel, ref ca, 3.9);


            OPSExt ext = new OPSExt(theFem, "G:\\20210717 黑慧江拱桥两阶段设计\\01 总体计算\\OpenSEES\\bin07");




        }


        static void Main(string[] args)
        {
            SpaceClaimProcedure();
            // AnsysProcedure();
            // MidasProcedure();
            // TrussProcedure();
            // OpenSEESProcedure();
        }


    }
}
