using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Model
{
    public partial  class FEMModel
    {
        #region 写出midas命令
        public void WriteMidas(string filepath)
        {
            StreamWriter sw = new StreamWriter(filepath, false,System.Text.Encoding.Default);

            WriteMCTHead(ref sw);
            WriteMCTMat(ref sw);
            WriteMCTNode(ref sw);
            WrietMCTElement(ref sw);

            sw.Flush();

            sw.Close();
        }

        private void WriteMCTHead(ref StreamWriter sw)
        {
            sw.WriteLine("*UNIT");
            sw.WriteLine("N, mm, KJ, C");
            sw.WriteLine("*STRUCTYPE");
            sw.WriteLine("0,1,1,NO,YES,9806,0,NO,NO,NO");
            sw.WriteLine("*REBAR-MATL-CODE");
            sw.WriteLine("JTG04(RC),HRB335,JTG04(RC),HRB335");
            sw.Flush();
        }
        private void WriteMCTNode(ref StreamWriter sw)
        {
            sw.WriteLine("*NODE");
            foreach (var item in NodeList)
            {
                sw.WriteLine("{0},{1:F8},{2:F8},{3:F8}", item.ID, item.X * 1000, item.Z * 1000, item.Y * 1000);
            }
        }
        private void WriteMCTMat(ref StreamWriter sw)
        {
            sw.WriteLine("*MATERIAL   ");
            sw.WriteLine(" 1, STEEL, Q345D-拱圈联结系  , 0.06, 0, , C, NO, 0.02, 1, GB 50917-13(S),            , Q345          , NO, 206000");
            sw.WriteLine(" 2, CONC , C40               , 0, 0, , C, NO, 0.05, 1, JTG3362-18(RC),            , C40           , NO, 32500");
            sw.WriteLine(" 3, CONC , C50               , 0, 0, , C, NO, 0.05, 1, JTG3362-18(RC),            , C50           , NO, 34500");
            sw.WriteLine(" 4, SRC  , 组合梁-40         , 0.06, 0, , C, YES, 0.05, 2, 2.0600e+005,   0.3, 1.2000e-005, 9.1868e-005, 9.3742e-009,2, 3.5500e+004,   0.2, 1.0000e-005, 2.92558e-005, 2.985e-009");
            sw.WriteLine(" 5, SRC  , 钢管砼-345        , 0.06, 0, , C, NO, 0.05, 1, GB12(S)    ,            , Q345          , NO, 206000, 1, JTG3362-18(RC),            , C70           , NO, 37000");
            sw.WriteLine(" 6, CONC , C70               , 0, 0, , C, NO, 0.05, 1, JTG3362-18(RC),            , C70           , NO, 37000");
            sw.WriteLine(" 7, STEEL, 钢绞线扣索        , 0, 0, , C, NO, 0.02, 1, JTG3362-18(S),            , Strand1860    , NO, 195000");
            sw.WriteLine(" 8, STEEL, Q420D             , 0.06, 0, , C, NO, 0.02, 1, GB50017-17(S),            , Q420          , NO, 206000");
            sw.WriteLine(" 9, SRC  , 钢管砼-420        , 0.06, 0, , C, NO, 0.05, 1, GB12(S)    ,            , Q420          , NO, 206000, 1, JTG3362-18(RC),            , C70           , NO, 37000");
            sw.WriteLine("10, SRC  , 组合梁-50         , 0.06, 0, , C, NO, 0.05, 2, 2.0600e+005,   0.3, 1.2000e-005, 9.7345e-005,     0,2, 3.5500e+004,   0.2, 1.0000e-005, 3.1e-005,     0");
            sw.WriteLine("11, CONC , CFST-1400*36      , 0, 0, , C, YES, 0.05, 2, 5.1070e+004,   0.2, 1.0190e-005, 3.00127e-005, 3.0625e-009");
            sw.WriteLine("12, STEEL, Q345D-立柱盖梁    , 0, 0, , C, NO, 0.02, 1, JTG D64-2015(S),            , Q345          , NO, 206000");
            sw.WriteLine("13, CONC , CFST-1400*32      , 0, 0, , C, YES, 0.05, 2, 4.8500e+004,   0.2, 1.0170e-005, 2.94679e-005, 3.0069e-009");
            sw.WriteLine("14, CONC , CFST-1400*28      , 0, 0, , C, YES, 0.05, 2, 4.6000e+004,   0.2, 1.0000e-005, 2.89201e-005, 2.951e-009");
            sw.WriteLine("15, CONC , 桩C40             , 0, 0, , C, NO, 0.05, 1, JTG3362-18(RC),            , C40           , NO, 32500");
            sw.WriteLine("*SECTION   ");
            sw.WriteLine("    1, DBUSER    , 腹杆-900*24       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 900, 24, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("    2, DBUSER    , SXG1              , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 1400, 35, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("    3, DBUSER    , 立柱              , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 1800, 30, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("    4, DBUSER    , 墩柱              , CC, 0, 0, 0, 0, 0, 0, YES, NO, ROCT, 2, 3000, 6000, 500, 500, 600, 600, 0, 0, 0, 0, 1");
            sw.WriteLine("    5, DBUSER    , 桁间横梁          , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("    6, DBUSER    , 桁间斜撑          , CC, 0, 0, 0, 0, 0, 0, YES, NO, H  , 2, 500, 500, 16, 16, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("    7, DBUSER    , 平联米字撑        , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 600, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("    8, DBUSER    , HL2               , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 700, 20, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("    9, DBUSER    , 立柱钢盖梁        , CT, 0, 0, 0, 0, 0, 0, YES, NO, BSTF, 2, 2000, 2500, 25, 20, 500, 1000, 20, 400, 250, 20, 2, 4");
            sw.WriteLine("   10, DBUSER    , 砼盖梁            , CC, 0, 0, 0, 0, 0, 0, YES, NO, SB , 2, 3000, 3000, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   11, TAPERED   , 过渡墩            , CC, 0, 0, 0, 0, 0, 0, 0, 0, YES, NO, B  , 1, 1, USER");
            sw.WriteLine("       3000, 5000, 600, 600, 0, 0, 0, 0,  5000, 5000, 600, 600, 0, 0, 0, 0");
            sw.WriteLine("   12, DBUSER    , LZ-HL             , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 1, GB-YB05, P 402x16");
            sw.WriteLine("   13, COMPOSITE , 组合梁            , CB, 0, 0, 0, 0, 0, 0, YES, NO, B");
            sw.WriteLine("       2040, 27, 1800, 0, 30, 2100, 0, 30");
            sw.WriteLine("       0, 0, 0, 0, 0, 0");
            sw.WriteLine("       0");
            sw.WriteLine("       0");
            sw.WriteLine("       0");
            sw.WriteLine("       0");
            sw.WriteLine("       0");
            sw.WriteLine("       12550, 1, 12550,   12550, 260, 50,   5.97101, 3.0792, 0.3, 0.2, 1.2, NO, , ");
            sw.WriteLine("   14, DBUSER    , 过渡墩盖梁        , CT, 0, 0, 0, 0, 0, 0, YES, NO, SB , 2, 2500, 3500, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   15, DBUSER    , 弦杆-1500*35      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   16, DBUSER    , 弦杆-1500*32      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   17, DBUSER    , 弦杆-1500*28      , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 1500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   18, DBUSER    , 腹杆-800*20       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 20, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   19, DBUSER    , 腹杆-800*16       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   20, DBUSER    , 腹杆-900*20       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 900, 20, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   21, DBUSER    , 扣索              , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 72, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   22, DBUSER    , 缆索吊索塔        , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 2000, 40, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   23, DBUSER    , 拱上立柱-矮       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 700, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   24, TAPERED   , 墩-变截面         , CC, 0, 0, 0, 0, 0, 0, 0, 0, YES, YES, B  , 1, 1, USER");
            sw.WriteLine("       2500, 6000, 700, 700, 0, 0, 0, 0,  5880, 6000, 700, 700, 0, 0, 0, 0");
            sw.WriteLine("   25, DBUSER    , 引桥墩承台        , CC, 0, 0, 0, 0, 0, 0, YES, NO, SB , 2, 6400, 10000, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   26, DBUSER    , 立柱联系梁        , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 450, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   27, DBUSER    , 立柱斜撑          , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 500, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   28, DBUSER    , 拱上立柱-高       , CC, 0, 0, 0, 0, 0, 0, YES, NO, P  , 2, 800, 16, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.WriteLine("   29, DBUSER    , 桩D2.8            , CC, 0, 0, 0, 0, 0, 0, YES, NO, SR , 2, 2800, 0, 0, 0, 0, 0, 0, 0, 0, 0");
            sw.Flush();
        }

        private void WrietMCTElement(ref StreamWriter sw)
        {
            Dictionary<int, int> SecnID = new Dictionary<int, int>()
            {
                {1,15 },
                {2,20 },
                {3,18 },
                {4,5 },
                {5,5 },
                {6,7 },
                {7,6 },
                {8,23 },
                {9,9 },
                {99,99 },

            };
            sw.WriteLine("*ELEMENT");
            int elemid = 1;
            foreach (var item in ElementList)
            {
                if (item.Secn==99)
                {
                    continue;
                }
                int matid = item.Secn == 1 ? 11 : 1;

                sw.WriteLine(" {0},BEAM,{4},{3},{1},{2},0,0", elemid, item.Ni, item.Nj,SecnID[item.Secn],matid);
                elemid++;
            }
            sw.WriteLine("*RIGIDLINK");
            int rigID = 1;
            foreach (var item in ElementList)
            {
                if (item.Secn==99)
                {
                    sw.WriteLine("{0},{1},111111,{2},",rigID,item.Ni,item.Nj);
                    rigID++;
                }

            }


            sw.Flush();
        }

        #endregion
    }
}
