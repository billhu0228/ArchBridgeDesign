using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnsysInterface
{
    public class OsisExt
    {
        FEMModel theFEMModel;

        public OsisExt(FEMModel model)
        {
            theFEMModel = model;
        }

        #region 写出OSIS输入文件
        public void WriteSML(string filepath)
        {

            StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.Default);

            WriteSMLHead(ref sw);
            WriteSMLMat(ref sw);
            WriteSMLSection(ref sw);
            WriteSMLNode(ref sw);
            WrietSMLElement(ref sw);
            WrietSMLConstraint(ref sw);
            WrietSMLLoad(ref sw);

            sw.Flush();

            sw.Close();
            Console.WriteLine("SML写出完成...");
        }

        private void WrietSMLConstraint(ref StreamWriter sw)
        {
            int frameID = theFEMModel.MaxFrameID;
            sw.WriteLine("nsel,s,no,,11000");
            sw.WriteLine("nsel,a,no,,12000");
            sw.WriteLine("nsel,a,no,,21000");
            sw.WriteLine("nsel,a,no,,22000");
            sw.WriteLine("nsel,a,no,,31000");
            sw.WriteLine("nsel,a,no,,32000");
            sw.WriteLine("nsel,a,no,,41000");
            sw.WriteLine("nsel,a,no,,42000");
            sw.WriteLine("nsel,a,no,,80000");
            sw.WriteLine("nsel,a,no,,80001");
            sw.WriteLine("nsel,a,no,,80002");
            sw.WriteLine("nsel,a,no,,80003");
            sw.WriteLine("cm,footA,node");
            sw.WriteLine("nsel,s,no,,{0}", 11000 + frameID);
            sw.WriteLine("nsel,a,no,,{0}", 12000 + frameID);
            sw.WriteLine("nsel,a,no,,{0}", 21000 + frameID);
            sw.WriteLine("nsel,a,no,,{0}", 22000 + frameID);
            sw.WriteLine("nsel,a,no,,{0}", 31000 + frameID);
            sw.WriteLine("nsel,a,no,,{0}", 32000 + frameID);
            sw.WriteLine("nsel,a,no,,{0}", 41000 + frameID);
            sw.WriteLine("nsel,a,no,,{0}", 42000 + frameID);
            sw.WriteLine("nsel,a,no,,80012");
            sw.WriteLine("nsel,a,no,,80013");
            sw.WriteLine("nsel,a,no,,80014");
            sw.WriteLine("nsel,a,no,,80015");
            sw.WriteLine("cm,footB,node");
            sw.WriteLine("bc,1,支撑1");
            sw.WriteLine("bv,1,ux,uy,uz,rx,ry,rz");
            sw.WriteLine("bn,1,footA");
            sw.WriteLine("bc,2,支撑2");
            sw.WriteLine("bv,2,ux,uy,uz,rx,ry,rz");
            sw.WriteLine("bn,2,footB");
            sw.WriteLine("nsel,none");
        }

        private void WriteSMLSection(ref StreamWriter sw)
        {
            sw.WriteLine("//-------SECTION------");
            sw.WriteLine("#环形截面,  1,  1 ,  TB1400-25  ,  1400 , 25");
            sw.WriteLine("#环形截面,  2,  1 ,  TB1400-28  ,  1400 , 28");
            sw.WriteLine("#环形截面,  3,  1 ,  TB1400-32  ,  1400 , 32");
            sw.WriteLine("#环形截面,  4,  1 ,  TB1400-38  ,  1400 , 38");
            sw.WriteLine("#环形截面,  5,  1 ,  TB1400-40  ,  1400 , 40");
            sw.WriteLine("#环形截面, 21,  2 ,  TB900-20   , 900  , 20");
            sw.WriteLine("#环形截面, 22,  2 ,  TB800-16   , 800  , 16");
            sw.WriteLine("#环形截面, 23,  2 ,  TB600-12   , 600  , 12");
            sw.WriteLine("#环形截面, 31,  3 ,  TB800-16   , 800  , 16");
            sw.WriteLine("#环形截面, 32,  3 ,  TB600-12   , 600  , 12");
            sw.WriteLine("#环形截面, 33,  3 ,  TB300-10   , 300  , 10");
            sw.WriteLine("#环形截面, 41,  4 ,  TB800-16   , 800  , 16");
            sw.WriteLine("#环形截面, 42,  4 ,  TB500-10   , 500  , 10");
            sw.WriteLine("#环形截面, 43,  4 ,  TB500-10   , 500  , 10");
            sw.WriteLine("#环形截面, 51,  5 ,  TB600-16   , 600  , 16");
            sw.WriteLine("#环形截面, 52,  5 ,  TB600-16   , 600  , 16");
            sw.WriteLine("#环形截面, 53,  5 ,  TB600-16   , 600  , 16");
            sw.WriteLine("#环形截面, 61,  5 ,  TB900-20   , 900  , 20");
            sw.WriteLine("#环形截面, 62,  5 ,  TB800-16   , 800  , 16");
            sw.WriteLine("#环形截面, 63,  5 ,  TB450-10   , 450  , 10");
            sw.WriteLine("#环形截面, 64,  5 ,  TB600-12   , 600  , 12");
            foreach (var item in new List<int>() { 1, 2, 3, 4, 5, 21, 22, 23, 31, 32, 33, 41, 42, 43, 51, 52, 53, 61, 62, 63, 64, })
            {
                sw.WriteLine("*dim,SecProp,1,2");
                sw.WriteLine("SecProp[0,0] = {0};", item);
                sw.WriteLine("SecProp[0,1] = {0};", item);
                sw.WriteLine("R,{0},Beam3D,SecProp", item);
            }
            sw.WriteLine("et,1,beam3D");
        }

        private void WrietSMLLoad(ref StreamWriter sw)
        {
            sw.WriteLine("esel,all");
            sw.WriteLine("cm,AllElem,Elem");
            sw.WriteLine("// 定义荷载");
            sw.WriteLine("lg,1,自重");
            sw.WriteLine("acel,0,0,-1");
            sw.WriteLine("g0,1,AllElem//自重");
            sw.WriteLine("St,CS1_一次落架");
            sw.WriteLine("StSel,CS1_一次落架");
            sw.WriteLine("ESel,None");
            sw.WriteLine("CMSel,A,全桥,Elem");
            sw.WriteLine("StEA,,全桥");
            sw.WriteLine("StBA,,1");
            sw.WriteLine("StBA,,2");
            sw.WriteLine("StBA,,3");
            sw.WriteLine("StBA,,4");
        }

        private void WrietSMLElement(ref StreamWriter sw)
        {
            sw.WriteLine("//-------ELEM------");
            sw.WriteLine("type,1");
            var secn_list = (from FEMElement e in theFEMModel.ElementList select e.Secn).ToList();
            secn_list = secn_list.Distinct().ToList();
            int elemid = 1;
            foreach (var secnn in secn_list)
            {
                int matid;
                if (secnn < 20)
                {
                    matid = secnn;
                }
                else
                {
                    matid = 21;
                }
                if (secnn >= 60)
                {
                    continue;
                }
                sw.WriteLine("real,{0}", secnn);
                sw.WriteLine("mat,{0}", matid);
                var eles = (from e in theFEMModel.ElementList where e.Secn == secnn select e).ToList();
                foreach (var item in eles)
                {
                    sw.WriteLine("e,{0},{1},{2}", elemid, item.Ni, item.Nj);
                    elemid++;
                }
            }

            //foreach (var item in theFEMModel.RigidGroups)
            //{
            //    sw.WriteLine("nsel,s,node,,{0}", item.Item1);
            //    foreach (var slv in item.Item2)
            //    {
            //        sw.WriteLine("nsel,a,node,,{0}", slv);
            //    }
            //    //sw.WriteLine("cerig,{0},all,all",item.Item1);
            //    sw.WriteLine("cp,next,all,all");
            //}
        }

        private void WriteSMLNode(ref StreamWriter sw)
        {
            sw.WriteLine("//-------Nodes------");
            foreach (var item in theFEMModel.NodeList)
            {
                if (item.ID > 99999)
                {
                    continue;
                }
                sw.WriteLine("n,{0},{1:F8},{2:F8},{3:F8}", item.ID, item.X * 1000, item.Z * 1000, item.Y * 1000);
            }
        }

        private void WriteSMLMat(ref StreamWriter sw)
        {
            sw.WriteLine("//-------Material------");
            sw.WriteLine("mt,1,CFST-1400-25,1");
            sw.WriteLine("mt,2,CFST-1400-28,1");
            sw.WriteLine("mt,3,CFST-1400-32,1");
            sw.WriteLine("mt,4,CFST-1400-38,1");
            sw.WriteLine("mt,5,CFST-1400-40,1");
            sw.WriteLine("MP,EX,   1,4.9786e+04");
            sw.WriteLine("MP,DENS, 1,2.8753e-09");
            sw.WriteLine("MP,ALPX, 1,1.0140e-05");
            sw.WriteLine("MP,NUXY, 1,0.2");
            sw.WriteLine("MP,EX,   2,5.1171e+04");
            sw.WriteLine("MP,DENS, 2,2.9194e-09");
            sw.WriteLine("MP,ALPX, 2,1.0157e-05");
            sw.WriteLine("MP,NUXY, 2,0.2");
            sw.WriteLine("MP,EX,   3,5.3009e+04");
            sw.WriteLine("MP,DENS, 3,2.9780e-09");
            sw.WriteLine("MP,ALPX, 3,1.0179e-05");
            sw.WriteLine("MP,NUXY, 3,0.2");
            sw.WriteLine("MP,EX,   4,5.5745e+04");
            sw.WriteLine("MP,DENS, 4,3.0651e-09");
            sw.WriteLine("MP,ALPX, 4,1.0211e-05");
            sw.WriteLine("MP,NUXY, 4,0.2");
            sw.WriteLine("MP,EX,   5,5.6651e+04");
            sw.WriteLine("MP,DENS, 5,3.0940e-09");
            sw.WriteLine("MP,ALPX, 5,1.0222e-05");
            sw.WriteLine("MP,NUXY, 5,0.2");
            sw.WriteLine("mt,21,Q420,1");
            sw.WriteLine("mt,22,Q355,1");
            sw.WriteLine("MP,EX,  21,2.06e+05");
            sw.WriteLine("MP,DENS,21,7.85e-09");
            sw.WriteLine("MP,ALPX,21,1.2e-05");
            sw.WriteLine("MP,NUXY,21,0.2");
            sw.WriteLine("MP,EX,  22,2.06e+05");
            sw.WriteLine("MP,DENS,22,7.85e-09");
            sw.WriteLine("MP,ALPX,22,1.2e-05");
            sw.WriteLine("MP,NUXY,22,0.2");
            sw.WriteLine("mt,37,CONC,1");
            sw.WriteLine("MP,EX,  37,3.70e+04");
            sw.WriteLine("MP,DENS,37,2.7e-09");
            sw.WriteLine("MP,ALPX,37,1.2e-05");
            sw.WriteLine("MP,NUXY,37,0.2");

        }

        private void WriteSMLHead(ref StreamWriter sw)
        {
            sw.WriteLine("clear");
            sw.WriteLine("unit,mm,N");
            sw.WriteLine("/prep7");
        }
        #endregion

    }
}
