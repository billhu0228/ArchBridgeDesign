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

        private void WrietMCTLoad(ref StreamWriter sw)
        {
            sw.WriteLine("*CONSTRAINT  ");
            sw.WriteLine("11000to41000by10000 11245to41245by10000 12000to42000by10000 , 111111, ");
            sw.WriteLine("12245to42245by10000 80000to80003 80012to80015, 111111, ");
            sw.WriteLine("*STLDCASE");
            sw.WriteLine("恒载, USER,");
            sw.WriteLine("*USE-STLD, 恒载");
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
            sw.WriteLine("34, CONC , C40               , 0, 0, , C, NO, 0.05, 1, JTG3362-18(RC),            , C40           , NO, 32500");
            sw.WriteLine("35, CONC , C50               , 0, 0, , C, NO, 0.05, 1, JTG3362-18(RC),            , C50           , NO, 34500");           
            sw.WriteLine("37, CONC , C70               , 0, 0, , C, NO, 0.05, 1, JTG3362-18(RC),            , C70           , NO, 37000");
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
            sw.WriteLine("   31, DBUSER    , 内隔-800*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   32, DBUSER    , 内隔-300*10       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 300, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   41, DBUSER    , 横梁-800*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   42, DBUSER    , 横梁-500*10       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 500, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   51, DBUSER    , 平联-600*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 600, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   61, DBUSER    , 立柱-900*20       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   62, DBUSER    , 立柱-800*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 700, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   63, DBUSER    , 立柱-600*12       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 450, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   64, DBUSER    , 立柱-600*12       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 300, 10, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   65, DBUSER  , 冒梁-2m, CT, 0, 0, 0, 0, 0, 0, YES, NO, BSTF, 2, 1800, 2000, 22, 20, 400, 200, 16, 435, 200, 16, 4, 3");
            sw.WriteLine("   66, DBUSER  , 冒梁-4m, CT, 0, 0, 0, 0, 0, 0, YES, NO, BSTF, 2, 1800, 4000, 22, 20, 400, 200, 16, 435, 200, 16, 7, 3");
            //sw.WriteLine("   21, DBUSER    , 扣索              , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 72, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   22, DBUSER    , 缆索吊索塔        , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 2000, 40, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   23, DBUSER    , 拱上立柱-矮       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 700, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            //sw.WriteLine("   24, TAPERED   , 墩-变截面         , CC, 0, 0, 0, 0, 0, 0, 0, 0, YES, YES, B  , 1, 1, USER");
            //sw.WriteLine("       2500, 6000, 700, 700, 0, 0, 0, 0,  5880, 6000, 700, 700, 0, 0, 0, 0");
            //sw.WriteLine("   25, DBUSER    , 引桥墩承台        , CC, 0, 0, 0, 0, 0, 0, YES, NO, SB , 2, 6400, 10000, 0, 0, 0, 0, 0, 0, 0, 0");
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

        private void WrietMCTElement(ref StreamWriter sw)
        {
            Dictionary<int, int> SecnID = new Dictionary<int, int>()
            {
                {11,11 },
                {12,12 },
                {21,21 },
                {22,22 },
                {23,23 },
                {31,31 },
                {32,32 },
                {41,41 },
                {42,42 },
                {51,51 },
                {61,61 },
                {62,62 },
                {63,63 },
                {64,64 },
                {65,65 },
                {66,66 },
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
                int matid = item.Secn <20 ? item.Secn : 21;

                sw.WriteLine(" {0},BEAM,{4},{3},{1},{2},0,0", elemid, item.Ni, item.Nj, SecnID[item.Secn], matid);
                elemid++;
            }

            sw.WriteLine("*RIGIDLINK");
            int rigID = 1;
            foreach (var item in theFEMModel.RigidGroups)
            {
                foreach (var slv in item.Item2)
                {
                    sw.WriteLine("{0},{1},111111,{2},", rigID, item.Item1, slv);
                    rigID++;
                }
            }

            sw.Flush();
        }

        #endregion
    }
}
