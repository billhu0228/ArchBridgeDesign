using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model;

namespace AnsysInterface
{
    public class MidasDeckExt
    {
        FEMDeck theFEMModel;

        public MidasDeckExt(FEMDeck model)
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
            sw.WriteLine("*USE-STLD, 二期");
            sw.WriteLine("*PRESSURE");
            var e4 = (from e in theFEMModel.ElementList where e.GetType() == typeof(FEMElement4) select e).ToList();
            sw.WriteLine(" {0}to{1}, PRES , PLATE, FACE, LZ, 0, 0, 0, NO, -0.0032669, 0, 0, 0, 0, ",e4.First().ID,e4.Last().ID);
        }

        private void WriteMCTHead(ref StreamWriter sw)
        {
            sw.WriteLine("*UNIT");
            sw.WriteLine("N, mm, KJ, C");

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
            sw.WriteLine("*THICKNESS");
            sw.WriteLine("   1, VALUE, 1, YES, 250, 0, YES, 1, 125");
            sw.WriteLine("*SECTION");
            sw.WriteLine("101,SOD,主梁,CT,0,1,0,0,1,-50,YES,NO,SOD-I");
            sw.WriteLine("YES,300,300,400,400,2000,32,32,25,100,0");
            sw.WriteLine("1,WL,0,250,25,0,0,0,0,0,0");
            sw.WriteLine("1,1,0,腹板,0,2,2,YES,500,WL,2,W1,YES,1000,WL,2,W2");
            sw.WriteLine("111,SOD,小纵梁,CT,0,1,0,0,1,-50,YES,NO,SOD-I");
            sw.WriteLine("YES,200,200,150,150,280,10,10,8,0,50");
            sw.WriteLine("0");
            sw.WriteLine("0");
            sw.WriteLine("121,SOD,横梁,CT,0,1,0,0,1,-350,YES,NO,SOD-I");
            sw.WriteLine("YES,150,150,150,150,700,25,25,18,0,0");
            sw.WriteLine("1,WL,0,250,25,0,0,0,0,0,0");
            sw.WriteLine("0");
            sw.Flush();
        }

        private void WrietMCTElement(ref StreamWriter sw)
        {
            sw.WriteLine("*ELEMENT");
            foreach (var item in theFEMModel.ElementList)
            {
                int elemid = item.ID;
                if (item.GetType()==typeof(FEMElement4))
                {
                    var plt = (FEMElement4)item;
                    sw.WriteLine(" {0},PLATE,37,1,{1},{2},{3},{4},2,0", elemid, plt.Ni, plt.Nj,plt.Nk,plt.Nl);
                }
                else
                {
                    int matid = item.Secn == 101 ? 22 : 21;
                    sw.WriteLine(" {0},BEAM,{4},{3},{1},{2},0,0", elemid, item.Ni, item.Nj, item.Secn,matid);
                }

            }

            sw.WriteLine("*ELASTICLINK");
            int rigID = 1 + theFEMModel.Estart;
            foreach (var item in theFEMModel.LinkGroups)
            {
                sw.WriteLine("{0},{1},{2}, GEN  ,0, NO, NO, NO, NO, NO, NO, 1e9, 1e6, 1e6, 0, 0, 0, NO, 0.5, 0.5, ", rigID, item.Item1, item.Item2);
                rigID++;
            }
            sw.Flush();
        }

        #endregion
    }
}
