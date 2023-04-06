using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace AnsysInterface
{
    public class MidasExt
    {
        FEMModel theFEMModel;

        public MidasExt(FEMModel model)
        {
            theFEMModel = model;
        }


        #region 写出midas命令
        public void WriteMidas(string filepath)
        {
            StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.Default);

            WriteMCTHead(ref sw);
            WriteMCTMat(ref sw);
            WriteMCTNode(ref sw);
            WrietMCTElement(ref sw);
            WrietMCTLoad(ref sw);

            sw.Flush();

            sw.Close();
            Console.WriteLine("MCT写出完成...");
        }
        public void WriteMidas2021(string filepath)
        {
            StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.Default);

            WriteMCTHead(ref sw);
            WriteMCTMat(ref sw);
            WriteMCTNode(ref sw);
            WrietMCTElement(ref sw,false);
            WrietMCTLoad(ref sw);

            sw.Flush();

            sw.Close();
            Console.WriteLine("MCT写出完成...");
        }

        private void WrietMCTLoad(ref StreamWriter sw)
        {
            int frameID = theFEMModel.MaxFrameID;
            var RC1 = (from e in theFEMModel.ElementList where (e.Secn == 71) && (e.Ni <= 112000) select e).ToList();
            var RC2 = (from e in theFEMModel.ElementList where (e.Secn == 71) && (e.Ni > 112000) select e).ToList();
            sw.WriteLine("*CONSTRAINT  ");
            sw.WriteLine("11000to41000by10000 12000to42000by10000 , 111000, ");
            sw.WriteLine("{0}to{1}by10000 {2}to{3}by10000 , 111000, ",11000+frameID,41000+frameID,12000+frameID,42000+frameID);
            sw.WriteLine("80000to80003 80012to80015, 111000, ");
            sw.WriteLine("{0} {1} {2} {3}, 111111, ", RC1[RC1.Count - 2].Nj ,RC1[RC1.Count-1].Nj, RC2[RC2.Count - 2].Nj, RC2[RC2.Count - 1].Nj);
            sw.WriteLine("*STLDCASE");
            sw.WriteLine("自重, USER,");
            sw.WriteLine("二期, USER,");
            sw.WriteLine("*USE-STLD, 自重");
            sw.WriteLine("*SELFWEIGHT");
            sw.WriteLine("0, 0, -1,");
        }

        private void WriteMCTHead(ref StreamWriter sw)
        {
            sw.WriteLine("*UNIT");
            sw.WriteLine("N, mm, KJ, C");
            sw.WriteLine("*STRUCTYPE");
            sw.WriteLine("0,1,1,NO,YES,9806,0,NO,NO,NO");
            //sw.WriteLine("*REBAR-MATL-CODE");
            //sw.WriteLine("JTG04(RC),HRB335,JTG04(RC),HRB335");
            sw.Flush();
        }
        private void WriteMCTNode(ref StreamWriter sw)
        {
            sw.WriteLine("*NODE");
            foreach (var item in theFEMModel.NodeList)
            {
                sw.WriteLine("{0},{1:F8},{2:F8},{3:F8}", item.ID, item.X * 1000, item.Z * 1000, item.Y * 1000);
            }
        }
        private void WriteMCTMat(ref StreamWriter sw)
        {
            sw.WriteLine("*MATERIAL   ");
            sw.WriteLine(" 1,CONC,CFST-1400*25 , 0, 0, , C,YES, 0.05, 2, 4.9786e+04, 0.2, 1.0140e-05, 2.8195e-05, 2.8753e-09");
            sw.WriteLine(" 2,CONC,CFST-1400*28 , 0, 0, , C,YES, 0.05, 2, 5.1171e+04, 0.2, 1.0157e-05, 2.8628e-05, 2.9194e-09");
            sw.WriteLine(" 3,CONC,CFST-1400*32 , 0, 0, , C,YES, 0.05, 2, 5.3009e+04, 0.2, 1.0179e-05, 2.9202e-05, 2.9780e-09");
            sw.WriteLine(" 4,CONC,CFST-1400*38 , 0, 0, , C,YES, 0.05, 2, 5.5745e+04, 0.2, 1.0211e-05, 3.0056e-05, 3.0651e-09");
            sw.WriteLine(" 5,CONC,CFST-1400*40 , 0, 0, , C,YES, 0.05, 2, 5.6651e+04, 0.2, 1.0222e-05, 3.0339e-05, 3.0940e-09");
            sw.WriteLine("11,CONC,CFST-1500*25 , 0, 0, , C,YES, 0.05, 2, 4.9013e+04, 0.2, 1.0131e-05, 2.7954e-05, 2.8507e-09");
            sw.WriteLine("12,CONC,CFST-1500*28 , 0, 0, , C,YES, 0.05, 2, 5.0310e+04, 0.2, 1.0147e-05, 2.8359e-05, 2.8920e-09");
            sw.WriteLine("13,CONC,CFST-1500*32 , 0, 0, , C,YES, 0.05, 2, 5.2030e+04, 0.2, 1.0167e-05, 2.8896e-05, 2.9468e-09");
            sw.WriteLine("14,CONC,CFST-1500*38 , 0, 0, , C,YES, 0.05, 2, 5.4593e+04, 0.2, 1.0198e-05, 2.9696e-05, 3.0284e-09");
            sw.WriteLine("15,CONC,CFST-1500*40 , 0, 0, , C,YES, 0.05, 2, 5.5442e+04, 0.2, 1.0208e-05, 2.9962e-05, 3.0554e-09");
            sw.WriteLine("21, STEEL, Q345D  , 0.06, 0, , C, NO, 0.02, 1, GB 50917-13(S),           , Q345          , NO, 206000");
            sw.WriteLine("22, STEEL, Q420D  , 0.06, 0, , C, NO, 0.02, 1, GB50017-17(S),            , Q420          , NO, 206000");
            sw.WriteLine("34, CONC , C40       , 0, 0, , C, NO, 0.05, 2, 3.2500e+004,   0.2, 1.0000e-005, 2.7e-005,  0");
            sw.WriteLine("35, CONC , C50       , 0, 0, , C, NO, 0.05, 2, 3.4500e+004,   0.2, 1.0000e-005, 2.7e-005,  0");
            sw.WriteLine("37, CONC , C70       , 0, 0, , C, NO, 0.05, 2, 3.7000e+004,   0.2, 1.0000e-005, 2.7e-005,  0");
            sw.WriteLine("41, STEEL, 钢绞线1860        , 0, 0, , C, NO, 0.02, 1, JTG3362-18(S),            , Strand1860    , NO, 195000");

            sw.WriteLine("*SECTION   ");
            sw.WriteLine("   1, DBUSER     , 弦杆-1400*25      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1400, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   2, DBUSER     , 弦杆-1400*28      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1400, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   3, DBUSER     , 弦杆-1400*32      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1400, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   4, DBUSER     , 弦杆-1400*38      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1400, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   5, DBUSER     , 弦杆-1400*40      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1400, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   11, DBUSER    , 弦杆-1500*25      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   12, DBUSER    , 弦杆-1500*28      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   13, DBUSER    , 弦杆-1500*32      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   14, DBUSER    , 弦杆-1500*38      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   15, DBUSER    , 弦杆-1500*40      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   21, DBUSER    , 腹杆-900*20       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 900, 20, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   22, DBUSER    , 腹杆-800*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   23, DBUSER    , 腹杆-600*12       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 600, 12, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   31, DBUSER    , 内隔-800*20       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 20, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   32, DBUSER    , 内隔-600*12       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 600, 12, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   33, DBUSER    , 内隔-300*10       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 300, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   41, DBUSER    , 横梁-800*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   42, DBUSER    , 横梁-600*12       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 600, 12, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   43, DBUSER    , 横梁-500*10       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 500, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   51, DBUSER    , 平联-700*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 700, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   52, DBUSER    , 平联-600*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 600, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   53, DBUSER    , 平联-500*10       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 500, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   61, DBUSER    , 立柱-900*20       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   62, DBUSER    , 立柱-800*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 700, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   63, DBUSER    , 立柱-600*12       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 450, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   64, DBUSER    , 立柱-600*12       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 300, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   65, DBUSER  , 冒梁-2m, CT, 0, 0, 0, 0, 0, 0, YES, NO, BSTF, 2, 1800, 2000, 22, 20, 400, 200, 16, 435, 200, 16, 4, 3");
            sw.WriteLine("   66, DBUSER  , 冒梁-4m, CT, 0, 0, 0, 0, 0, 0, YES, NO, BSTF, 2, 1800, 4000, 22, 20, 400, 200, 16, 435, 200, 16, 7, 3");
            //sw.WriteLine("   21, DBUSER    , 扣索              , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 72, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   22, DBUSER    , 缆索吊索塔        , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 2000, 40, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   23, DBUSER    , 拱上立柱-矮       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 700, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   71, TAPERED   , 过渡墩         , CC, 0, 0, 0, 0, 0, 0, 0, 0, YES, YES, B  , 1, 1, USER");
            sw.WriteLine("       4000, 7000, 500, 500, 0, 0, 0, 0,  6700, 7000, 500, 500, 0, 0, 0, 0");            
            sw.WriteLine("   72, DBUSER    , 盖梁           , CT, 0, 0, 0, 0, 0, 0, YES, NO, SB , 2, 4000, 5000, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   26, DBUSER    , 立柱联系梁        , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 450, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   27, DBUSER    , 立柱斜撑          , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 500, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   28, DBUSER    , 拱上立柱-高       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   29, DBUSER    , 桩D2.8            , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 2800, 0, 0, 0, 0, 0, 0, 0, 0, 0");

            sw.WriteLine("*SECT-SCALE");
            sw.WriteLine("1, 1, 1, 1, 1, 0.956, 0.956, 1, , 1, NO");
            sw.WriteLine("2, 1, 1, 1, 1, 0.985, 0.985, 1, , 1, NO");
            sw.WriteLine("3, 1, 1, 1, 1, 1.020, 1.020, 1, , 1, NO");
            sw.WriteLine("4, 1, 1, 1, 1, 1.067, 1.067, 1, , 1, NO");
            sw.WriteLine("5, 1, 1, 1, 1, 1.081, 1.081, 1, , 1, NO");
            sw.WriteLine("11, 1, 1, 1, 1, 0.939, 0.939, 1, , 1, NO");
            sw.WriteLine("12, 1, 1, 1, 1, 0.967, 0.967, 1, , 1, NO");
            sw.WriteLine("13, 1, 1, 1, 1, 1.002, 1.002, 1, , 1, NO");
            sw.WriteLine("14, 1, 1, 1, 1, 1.048, 1.048, 1, , 1, NO");
            sw.WriteLine("15, 1, 1, 1, 1, 1.062, 1.062, 1, , 1, NO");
            sw.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="includeRigID">是否包含刚彼编号，高版本midas不需要</param>
        private void WrietMCTElement(ref StreamWriter sw,bool includeRigID=true)
        {
            Dictionary<int, int> SecnID = new Dictionary<int, int>()
            {
                {1,1 },
                {2,2 },
                {3,3 },
                {4,4 },
                {5,5 },
                {11,11 },
                {12,12 },
                {13,13 },
                {14,14 },
                {15,15 },
                {21,21 },
                {22,22 },
                {23,23 },
                {31,31 },
                {32,32 },
                {33,33 },
                {41,41 },
                {42,42 },
                {43,43 },
                {51,51 },
                {52,52 },
                {53,53 },
                {61,61 },
                {62,62 },
                {63,63 },
                {64,64 },
                {65,65 },
                {66,66 },
                {71,71 },
                {72,72 },
                {99,65 },
            };
            sw.WriteLine("*ELEMENT");
            int elemid = 1;
            foreach (var item in theFEMModel.ElementList)
            {
                //if (item.Secn == 99)
                //{
                //    continue;
                //}
                int matid = item.Secn < 20 ? SecnID[item.Secn] : 21;
                if (item.Secn>=70)
                {
                    matid = 35;
                }

                sw.WriteLine(" {0},BEAM,{4},{3},{1},{2},0,0", elemid, item.Ni, item.Nj, SecnID[item.Secn], matid);
                item.ID = elemid;
                elemid++;
            }
            var RC1=(from e in theFEMModel.ElementList where (e.Secn==71)&&(e.Ni<=112000) select e).ToList();
            var RC2=(from e in theFEMModel.ElementList where (e.Secn==71)&&(e.Ni>112000) select e).ToList();

            sw.WriteLine("*TS-GROUP");
            sw.WriteLine("  C1, {0}to{1}by2,  LINEAR, , , ,  LINEAR, , , , 0", RC1[0].ID, RC1[RC1.Count - 2].ID);
            sw.WriteLine("  C2, {0}to{1}by2,  LINEAR, , , ,  LINEAR, , , , 0", RC1[1].ID, RC1[RC1.Count - 1].ID);
            sw.WriteLine("  C3, {0}to{1}by2,  LINEAR, , , ,  LINEAR, , , , 0", RC2[0].ID, RC2[RC2.Count - 2].ID);
            sw.WriteLine("  C4, {0}to{1}by2,  LINEAR, , , ,  LINEAR, , , , 0", RC2[1].ID, RC2[RC2.Count - 1].ID);           
            
            
            sw.WriteLine("*RIGIDLINK");
            int rigID = 1;
            foreach (var item in theFEMModel.RigidGroups)
            {
                foreach (var slv in item.Item2)
                {
                    if (includeRigID)
                    {
                        sw.WriteLine("{0},{1},111111,{2},", rigID, item.Item1, slv);
                    }
                    else
                    {
                        sw.WriteLine("{0},111111,{1},",  item.Item1, slv);
                    }

                    rigID++;
                }
            }


            sw.Flush();
        }

        private void WriteLoadHeader(ref StreamWriter sw)
        {
            sw.WriteLine("*UNIT");
            sw.WriteLine(" N , M, KJ, C");
            sw.WriteLine("*STLDCASE");
            sw.WriteLine("; LCNAME, LCTYPE, DESC");
            sw.WriteLine("   自重 , USER, ");
            sw.WriteLine("   二期 , USER, ");
            sw.WriteLine("   W1拱肋横风, USER, ");
            sw.WriteLine("   W1桥面横风, USER, ");
            sw.WriteLine("   W1立柱横风, USER, ");
            sw.WriteLine("   W2拱肋横风, USER, ");
            sw.WriteLine("   W2立柱横风, USER, ");
            sw.WriteLine("   W2桥面横风, USER, ");
            sw.WriteLine("   整体升温, USER, ");
            sw.WriteLine("   整体降温, USER, ");
            sw.WriteLine("   梯度升温, USER, ");
            sw.WriteLine("   梯度降温, USER, ");
            sw.WriteLine("   制动力, USER, ");
        }
        private void WriteBreaking(ref StreamWriter sw, ref FEMDeck DeckA, ref FEMDeck DeckB)
        {
            sw.WriteLine("*UNIT");
            sw.WriteLine(" N , M, KJ, C");
            sw.WriteLine("*USE-STLD, 制动力");
            sw.WriteLine("*PRESSURE ");
            foreach (var item in DeckA.ElementList)
            {
                if (item.GetType() == typeof(FEMElement4))
                {
                    sw.WriteLine("{0,8}, PRES , PLATE, FACE, GX, 0, 0, 0, NO, 208, 0, 0, 0, 0, ", item.ID);
                }
            }
            foreach (var item in DeckB.ElementList)
            {
                if (item.GetType() == typeof(FEMElement4))
                {
                    sw.WriteLine("{0,8}, PRES , PLATE, FACE, GX, 0, 0, 0, NO, 208, 0, 0, 0, 0, ", item.ID);
                }
            }
            sw.Flush();
            Console.WriteLine("制动力MCT写出完成...");

        }
        private void WriteDeckWind(ref StreamWriter sw, string LoadName, int Estart, int Eend, bool WithCar, int ForceDeck)
        {
            sw.WriteLine("*USE-STLD, {0}", LoadName);
            sw.WriteLine("*BEAMLOAD");
            int e0 = Estart;
            while (e0 <= Eend)
            {
                sw.WriteLine("{0,8}, BEAM , UNILOAD, GY, NO , NO, aDir[1], , , , 0, {1}, 1, {2}, 0, 0, 0, 0, , NO, 0, 0, NO,", e0, ForceDeck, ForceDeck);
                if (WithCar)
                {
                    sw.WriteLine("{0,8}, BEAM , UNILOAD, GY, NO , NO, aDir[1], , , , 0, 1500, 1, 1500, 0, 0, 0, 0, , NO, 0, 0, NO,", e0);
                }
                e0++;
            }
            sw.WriteLine(";END OF [{0}]-------------------------------------------------------------------------", LoadName);
        }
        private void WriteColuWind(ref StreamWriter sw, string LoadName, int FC900, int FC800, int FC600)
        {
            sw.WriteLine("*USE-STLD, {0}", LoadName);
            sw.WriteLine("*BEAMLOAD");
            var Col = (from e in theFEMModel.ElementList where (e.Secn <= 64) && (e.Secn >= 61) select e).ToList();
            Col.Sort((x, y) => x.ID.CompareTo(y.ID));
            foreach (var item in Col)
            {
                if (item.Secn == 61)
                {
                    sw.WriteLine("{0,6}, BEAM , UNILOAD, GY, NO , NO, aDir[1], , , , 0, {1}, 1, {2}, 0, 0, 0, 0, , NO, 0, 0, NO,", item.ID, FC900, FC900);
                }
                else if (item.Secn <= 63)
                {
                    sw.WriteLine("{0,6}, BEAM , UNILOAD, GY, NO , NO, aDir[1], , , , 0, {1}, 1, {2}, 0, 0, 0, 0, , NO, 0, 0, NO,", item.ID, FC800, FC800);
                }
                else
                {
                    sw.WriteLine("{0,6}, BEAM , UNILOAD, GY, NO , NO, aDir[1], , , , 0, {1}, 1, {2}, 0, 0, 0, 0, , NO, 0, 0, NO,", item.ID, FC600, FC600);
                }
            }
            sw.WriteLine(";END OF [{0}]-------------------------------------------------------------------------", LoadName);
        }
        private void WriteArchWind(ref StreamWriter sw, string LoadName, int FCoord, int FWeb)
        {
            sw.WriteLine("*USE-STLD, {0}", LoadName);
            sw.WriteLine("*BEAMLOAD");
            var Rib = (from e in theFEMModel.ElementList where e.Secn <= 30 select e).ToList();
            Rib.Sort((x, y) => x.ID.CompareTo(y.ID));
            foreach (var item in Rib)
            {
                if (item.Secn <= 5)
                {
                    sw.WriteLine("{0,6}, BEAM , UNILOAD, GY, NO , NO, aDir[1], , , , 0, {1}, 1, {2}, 0, 0, 0, 0, , NO, 0, 0, NO,", item.ID, FCoord, FCoord);
                }
                else
                {
                    sw.WriteLine("{0,6}, BEAM , UNILOAD, GY, NO , NO, aDir[1], , , , 0, {1}, 1, {2}, 0, 0, 0, 0, , NO, 0, 0, NO,", item.ID, FWeb, FWeb);
                }
            }
            sw.WriteLine(";END OF [{0}]-------------------------------------------------------------------------", LoadName);
        }
        private void WriteTempAndFd(ref StreamWriter sw)
        {
            sw.WriteLine("*USE-STLD, 梯度升温");
            sw.WriteLine("*ELTEMPER    ; Element Temperatures");
            var RibUP = (from e in theFEMModel.ElementList where (e.Secn <= 5)&& (theFEMModel.IsNodeInUpper(e.Ni)) select e).ToList();
            RibUP.Sort((x, y) => x.ID.CompareTo(y.ID));
            foreach (var item in RibUP)
            {
                sw.WriteLine("{0,5:G}, 8, ", item.ID);
            }
            sw.Flush();
            sw.WriteLine("*USE-STLD, 梯度降温");
            sw.WriteLine("*ELTEMPER    ; Element Temperatures");
            var RibLW = (from e in theFEMModel.ElementList where (e.Secn <= 5) && (!theFEMModel.IsNodeInUpper(e.Ni)) select e).ToList();
            RibLW.Sort((x, y) => x.ID.CompareTo(y.ID));
            foreach (var item in RibLW)
            {
                sw.WriteLine("{0,5:G}, -4, ", item.ID);
            }
            sw.Flush();
            sw.WriteLine("*USE-STLD, 整体升温");
            sw.WriteLine("*SYSTEMPER");
            sw.WriteLine("27, ");
            sw.WriteLine("*USE-STLD, 整体降温");
            sw.WriteLine("*SYSTEMPER");
            sw.WriteLine("-26, ");
            sw.WriteLine("*SM-GROUP    ");
            sw.WriteLine("   FT1,0.050, 11000to41000by10000 12000to42000by10000 80000to80003");
            int m = theFEMModel.MaxFrameID;
            sw.WriteLine("   FT2,0.050, {0}to{1}by10000 {2}to{3}by10000 80012to80015", 11000 + m, 41000 + m, 12000 + m, 42000 + m);
            sw.WriteLine("*SMLDCASE ");
            sw.WriteLine("   NAME=沉降, 1, 2, 1, ");
            sw.WriteLine("   FT1, FT2");
            var t = (from e in RibUP select e.ID).ToList();
            string gp = Classify(t);
            sw.WriteLine("*GROUP");
            sw.WriteLine(" 上弦,,{0},0",gp);
            t = (from e in RibLW select e.ID).ToList();
            gp = Classify(t);
            sw.WriteLine(" 下弦,,{0},0",gp);
        }
        public void WriteLoads(string filepath, ref FEMDeck deck1, ref FEMDeck deck2)
        {
            StreamWriter sw = new StreamWriter(filepath, false,Encoding.Default);
            WriteLoadHeader(ref sw);

            WriteArchWind(ref sw, "W1拱肋横风", 486, 228);
            WriteArchWind(ref sw, "W2拱肋横风", 896, 512);
            WriteColuWind(ref sw, "W1立柱横风", 234, 208, 78);
            WriteColuWind(ref sw, "W2立柱横风", 412, 366, 137);
            WriteDeckWind(ref sw, "W1桥面横风", 300660, 300769, true, 2729);
            WriteDeckWind(ref sw, "W2桥面横风", 300660, 300769, false, 5048);
            sw.Flush();
            Console.WriteLine("梯度温度荷载MCT写出完成...");
            WriteTempAndFd(ref sw);
            WriteBreaking(ref sw,ref deck1, ref deck2);
            sw.Close();
        }

        private void WriteLane(ref StreamWriter sw,string Name,int nstart,int nend,int offset,string direction)
        {
            sw.WriteLine("   NAME={0}, 3000, 0, 0, {1}, 1800, NO, 3000",Name,direction);
            int n = nstart;
            int i = 1;
            string isEND = "NO";
            while (n<=nend)
            {
                if (n==nstart || n==nend)
                {
                    isEND = "YES";
                }
                else
                {
                    isEND = "NO";
                }
                if (i%2==1)
                {
                    // 奇数
                    sw.Write("        ");
                    sw.Write("{0}, {1}, 518000, {2}", n,offset,isEND);
                    if (n!=nend)
                    {
                        sw.Write(", ");
                    }
                    else
                    {
                        sw.Write("\n");
                    }
                }
                else
                {
                    sw.Write("{0}, {1}, 518000, {2}", n, offset, isEND);
                    sw.Write("\n");
                }
                n++;
                i++;
            }
   
        }
        public void WriteLiveLoad(string filepath, ref FEMDeck leftDeck, ref FEMDeck rightDeck)
        {
            StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.Default);
            sw.WriteLine("*UNIT");
            sw.WriteLine(" N, mm, KJ, C");
            sw.WriteLine("*MVLDCODE");
            sw.WriteLine(" CODE=CHINA");
            sw.WriteLine("*SURFLANE(CH)");
            int NN = leftDeck.NodeList[0].ID / 100000 * 100000;
            int npts = leftDeck.NPTS;
            WriteLane(ref sw, "L1", NN + npts * 6 + 1, NN + npts * 7, 2050, "BACKWARD");
            WriteLane(ref sw, "L2", NN + npts * 6 + 1, NN + npts * 7, 5050, "BACKWARD");
            WriteLane(ref sw, "L3", NN + npts * 6 + 1, NN + npts * 7, 8050, "BACKWARD");
            NN = rightDeck.NodeList[0].ID / 100000 * 100000;
            npts = rightDeck.NPTS;
            WriteLane(ref sw, "R1", NN + npts * 6 + 1, NN + npts * 7, 2050, "FORWARD");
            WriteLane(ref sw, "R2", NN + npts * 6 + 1, NN + npts * 7, 5050, "FORWARD");
            WriteLane(ref sw, "R3", NN + npts * 6 + 1, NN + npts * 7, 8050, "FORWARD");
            sw.Flush();
            sw.WriteLine("*VEHICLE    ; Vehicles");
            sw.WriteLine("   NAME=CH-CD, 1, CH-CD, JTGB01-2014");
            sw.WriteLine("*MVLDCASE(CH)   ; Moving Load Cases (china)");
            sw.WriteLine("   NAME=活载, ,  NO, 2, 0, 1");
            sw.WriteLine("        1, 1, 0.8, 0.67, 0.6, 0.55, 0.55, 0.55");
            sw.WriteLine("        1, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        1.2, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        VL, CH-CD, 1.05, 0, 3, L1, L2, L3");
            sw.WriteLine("        VL, CH-CD, 1.05, 0, 3, R1, R2, R3");
            sw.WriteLine("*MOVE-CTRL(CH)");            
            sw.WriteLine("   INF, 0, 3, CENTER, NO, NORMAL, YES,  YES, NO, ,   YES, NO, ,   YES, NO, ,   YES, NO,   , NO, 0, 0, YES, 0");
            sw.WriteLine("   0");
            sw.Close();
            Console.WriteLine("MCT写出完成...");
        }

        public void WriteFTLoad(string filepath, ref FEMDeck leftDeck, ref FEMDeck rightDeck)
        {
            StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.Default);
            sw.WriteLine("*UNIT");
            sw.WriteLine(" N, mm, KJ, C");
            sw.WriteLine("*MVLDCODE");
            sw.WriteLine(" CODE=CHINA");
            sw.WriteLine("*SURFLANE(CH)");
            int NN = leftDeck.NodeList[0].ID / 100000 * 100000;
            int npts = leftDeck.NPTS;
            WriteLane(ref sw, "L1", NN + npts * 6 + 1, NN + npts * 7, 2050, "BACKWARD");
            WriteLane(ref sw, "L2", NN + npts * 6 + 1, NN + npts * 7, 5050, "BACKWARD");
            WriteLane(ref sw, "L3", NN + npts * 6 + 1, NN + npts * 7, 8050, "BACKWARD");
            NN = rightDeck.NodeList[0].ID / 100000 * 100000;
            npts = rightDeck.NPTS;
            WriteLane(ref sw, "R1", NN + npts * 6 + 1, NN + npts * 7, 2050, "FORWARD");
            WriteLane(ref sw, "R2", NN + npts * 6 + 1, NN + npts * 7, 5050, "FORWARD");
            WriteLane(ref sw, "R3", NN + npts * 6 + 1, NN + npts * 7, 8050, "FORWARD");
            sw.Flush();
            sw.WriteLine("*VEHICLE    ; Vehicles");
            sw.WriteLine("  NAME=CH-PL1, 1, CH-PL1, JTGB01-2014");
            sw.WriteLine("  NAME=CH-PL2, 1, CH-PL2, JTGB01-2014, 40000");
            sw.WriteLine("  NAME=CH-PL3, 1, CH-PL3, JTGB01-2014");
            sw.WriteLine("*MVLDCASE(CH)   ");
            sw.WriteLine("   NAME=疲劳-I(FL), , NO, 2, 0, 1");
            sw.WriteLine("        1, 1, 0.8, 0.67, 0.6, 0.55, 0.55, 0.55");
            sw.WriteLine("        1, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        1.2, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        VL, CH-PL1, 1, 0, 3, L1, L2, L3");
            sw.WriteLine("        VL, CH-PL1, 1, 0, 3, R1, R2, R3");
            sw.WriteLine("   NAME=疲劳-II(FL), , NO, 2, 0, 1");
            sw.WriteLine("        1, 1, 0.8, 0.67, 0.6, 0.55, 0.55, 0.55");
            sw.WriteLine("        1, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        1.2, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        VL, CH-PL2, 1, 0, 3, L1, L2, L3");
            sw.WriteLine("        VL, CH-PL2, 1, 0, 3, R1, R2, R3");
            sw.WriteLine("   NAME=疲劳-III(FL), , NO, 2, 0, 1");
            sw.WriteLine("        1, 1, 0.8, 0.67, 0.6, 0.55, 0.55, 0.55");
            sw.WriteLine("        1, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        1.2, 1, 0.78, 0.67, 0.6, 0.55, 0.52, 0.5");
            sw.WriteLine("        VL, CH-PL3, 1, 0, 3, L1, L2, L3");
            sw.WriteLine("        VL, CH-PL3, 1, 0, 3, R1, R2, R3");
            sw.WriteLine("*MOVE-CTRL(CH)");
            sw.WriteLine("   INF, 0, 3, CENTER, NO, NORMAL, YES,  YES, NO, ,   YES, NO, ,   YES, NO, ,   YES, NO,   , NO, 0, 0, YES, 0");
            sw.WriteLine("   0");
            sw.Close();
            Console.WriteLine("MCT写出完成...");
        }

        #endregion
        public void WriteNodeInfo(string filepath)
        {
            var NodeSelected = (from e in theFEMModel.ElementList where (e.Secn <= 5) & (Math.Abs(theFEMModel.GetNode(e.Ni).Z - 11.0) < 0.1) select e.Ni).ToList();
            var Njs = (from e in theFEMModel.ElementList where (e.Secn <= 5) & (theFEMModel.GetNode(e.Ni).Z == 11.0) select e.Nj).ToList();
            NodeSelected.AddRange(Njs);
            NodeSelected = NodeSelected.Distinct().ToList();
            NodeSelected.Sort();

            StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.UTF8);
            sw.WriteLine("N,X,Y,Z,IsUp");
            foreach (var item in NodeSelected)
            {
                sw.WriteLine("{0,5},{1:F3},{2:F3},{3:F3},{4}", item,
                    theFEMModel.GetNode(item).X,
                    theFEMModel.GetNode(item).Y,
                    theFEMModel.GetNode(item).Z,
                    theFEMModel.IsNodeInUpper(item));
            }
            sw.Flush();
            sw.Close();
            Console.WriteLine("节点信息写出完成...");
        }

        public static string Classify(List<int> theList)
        {
            List<int>tmp= theList.Distinct().ToList();
            tmp.Sort();
            List<string> output= new List<string>();
            int countSpace;
            for (int i = 0; i < tmp.Count; i++)
            {
                int a = tmp[i];
                if (a==theList[0])
                {
                    output.Add(" ");
                    output.Add(a.ToString());
                }
                else if (a==theList.Last())
                {
                    if (a!=theList[i-1]+1)
                    {
                        output.Add(" ");
                        output.Add(a.ToString());
                    }
                    else
                    {
                        output.Add("to");
                        output.Add(a.ToString());
                    }
                }
                else if (a != theList[i - 1] + 1)
                {
                    output.Add(" ");
                    output.Add(a.ToString());
                }
                else if (a != theList[i + 1] - 1)
                {
                    output.Add("to");
                    output.Add(a.ToString());
                    output.Add(" ");
                }
                else
                {
                    continue;
                }
                countSpace = (from e in output where e == " " select e).ToList().Count;
                if (countSpace %15== 0)
                {
                    output.Add(" \\\n");
                }
            }
            return string.Join("", output);
        }
    }
}
