using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model;

namespace AnsysInterface
{
    public class AnsysDeckExt
    {
        FEMDeck theFEMModel;

        public AnsysDeckExt(FEMDeck model)
        {
            theFEMModel = model;
        }


        #region 写出Ansys输入文件
        public void WriteAnsys(string dirPath,string fileName)
        {
            var cwd = Directory.CreateDirectory(dirPath);
            var filepath = Path.Combine(cwd.FullName, fileName);
            StreamWriter sw = new StreamWriter(filepath);
            WriteMainInp(ref sw);
            WriteMaterial(ref sw);
            WriteSection(ref sw);
            WriteNode(ref sw);
            WriteElem(ref sw);
            sw.Flush();
            sw.Close();



            // WriteNode(Path.Combine(cwd.FullName, "node.inp"));
            // WriteElem(Path.Combine(cwd.FullName, "element.inp"));
        }

        private void WriteNode(ref StreamWriter sw)
        {
            sw.WriteLine("/prep7");
            foreach (var item in theFEMModel.NodeList)
            {
                sw.WriteLine("n,{0},{1:F8},{2:F8},{3:F8}", item.ID, item.X * 1000, item.Y * 1000, item.Z * 1000);
            }
            sw.WriteLine("n,999999,0,1e15,0");
        }

        private void WriteElem(ref StreamWriter sw)
        {
            sw.WriteLine("/prep7");
            var secn_list = (from FEMElement e in theFEMModel.ElementList select e.Secn).ToList();
            secn_list = secn_list.Distinct().ToList();
            foreach (var secnn in secn_list)
            {
                int matn;
                int etn;
                int sn;
                if (secnn==1)
                {
                    etn = 2;
                    matn = 37;
                    sn = 100;
                }
                else
                {
                    matn = secnn;  
                    etn = 1;
                    sn = secnn; 
                }
                sw.WriteLine("type,{0}", etn);
                sw.WriteLine("secn,{0}", sn);
                sw.WriteLine("mat,{0}", matn);
                var eles = (from e in theFEMModel.ElementList where e.Secn == secnn select e).ToList();
                if (secnn==1)
                {
                    foreach (FEMElement4 item in eles)
                    {

                        sw.WriteLine("e,{0},{1},{2},{3}", item.Ni, item.Nj,item.Nk,item.Nl);
                    }
                }
                else
                {
                    foreach (FEMElement item in eles)
                    {
                        sw.WriteLine("e,{0},{1},999999", item.Ni, item.Nj);
                    }
                }       
            }
            sw.WriteLine("type,141");
            sw.WriteLine("real,14");
            sw.WriteLine("mat,14");
            foreach (var item in theFEMModel.LinkGroups)
            {
                sw.WriteLine("e,{0},{1}", item.Item1, item.Item2);
            }
            sw.WriteLine("type,142");
            sw.WriteLine("real,14");
            sw.WriteLine("mat,14");
            foreach (var item in theFEMModel.LinkGroups)
            {
                sw.WriteLine("e,{0},{1}", item.Item1, item.Item2);
            }
            sw.WriteLine("type,143");
            sw.WriteLine("real,14");
            sw.WriteLine("mat,14");
            foreach (var item in theFEMModel.LinkGroups)
            {
                sw.WriteLine("e,{0},{1}", item.Item1, item.Item2);
            }
        }

        private void WriteSection(ref StreamWriter sw)
        {
            sw.WriteLine("/prep7");
            sw.WriteLine("et,1,188");
            sw.WriteLine("et,2,181");
            sw.WriteLine("et,141,14,0,1");
            sw.WriteLine("et,142,14,0,2");
            sw.WriteLine("et,143,14,0,3");
            sw.WriteLine("r,14,1e9");


            sw.WriteLine("sect,100,shell,,th");
            sw.WriteLine("secdata,250,37,0.0,3");
            sw.WriteLine("secoffset,TOP");
            sw.WriteLine("SECTYPE, 101, BEAM, I, MainG, 0");
            sw.WriteLine("SECOFFSET, USER, 0, 2050");
            sw.WriteLine("SECDATA,800,600,2000,32,32,25,0,0,0,0,0,0 ");
            sw.WriteLine("SECTYPE, 111, BEAM, I, SmallG, 0");
            sw.WriteLine("SECOFFSET, USER, 0, 350");
            sw.WriteLine("SECDATA,300,400,300,10,10,8,0,0,0,0,0,0 ");
            sw.WriteLine("SECTYPE, 121, BEAM, I, CrossG, 0");
            sw.WriteLine("SECOFFSET, USER, 0, 1050");
            sw.WriteLine("SECDATA,300,300,700,25,25,18,0,0,0,0,0,0 ");
            sw.WriteLine("SECTYPE,65,BEAM,HREC,CB-2m,0");
            sw.WriteLine("SECOFFSET,USER,1800,1000");
            sw.WriteLine("SECDATA,1800,2000,40,40,40,40,0,0,0,0,0,0");
            sw.WriteLine("SECTYPE,66,BEAM,HREC,CB-4m,0");
            sw.WriteLine("SECOFFSET,USER,1800,2000");
            sw.WriteLine("SECDATA,1800,4000,40,40,40,40,0,0,0,0,0,0");
        }

        private void WriteMaterial(ref StreamWriter sw)
        {
            sw.WriteLine("/prep7");
            sw.WriteLine("MP,EX,  37,3.70e+04");
            sw.WriteLine("MP,DENS,37,2.7e-09");
            sw.WriteLine("MP,ALPX,37,1.2e-05");
            sw.WriteLine("MP,NUXY,37,0.2");
            sw.WriteLine("MP,EX,  101,2.06e+05");
            sw.WriteLine("MP,DENS,101,7.85e-09");
            sw.WriteLine("MP,ALPX,101,1.2e-05");
            sw.WriteLine("MP,NUXY,101,0.2");
            sw.WriteLine("MP,EX,  111,2.06e+05");
            sw.WriteLine("MP,DENS,111,7.85e-09");
            sw.WriteLine("MP,ALPX,111,1.2e-05");
            sw.WriteLine("MP,NUXY,111,0.2");
            sw.WriteLine("MP,EX,  121,2.06e+05");
            sw.WriteLine("MP,DENS,121,7.85e-09");
            sw.WriteLine("MP,ALPX,121,1.2e-05");
            sw.WriteLine("MP,NUXY,121,0.2");
        }

        private void WriteMainInp(ref StreamWriter sw)
        {
            sw.WriteLine("finish");

        }

        #endregion
    }
}
