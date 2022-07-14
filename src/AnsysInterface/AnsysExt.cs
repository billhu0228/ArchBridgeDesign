using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace AnsysInterface
{
    public class AnsysExt
    {
        FEMModel theFEMModel;

        public AnsysExt(FEMModel model)
        {
            theFEMModel = model;
        }


        #region 写出Ansys输入文件
        public void WriteAnsys(string dirPath)
        {
            var cwd = Directory.CreateDirectory(dirPath);

            WriteMainInp(Path.Combine(cwd.FullName, "Main.inp"));
            WriteMaterial(Path.Combine(cwd.FullName, "material.inp"));
            WriteSection(Path.Combine(cwd.FullName, "section.inp"));
            WriteNode(Path.Combine(cwd.FullName, "node.inp"));
            WriteElem(Path.Combine(cwd.FullName, "element.inp"));
            WriteSolu(Path.Combine(cwd.FullName, "solu.inp"));
        }

        private void WriteNode(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false))
            {
                sw.WriteLine("/prep7");
                foreach (var item in theFEMModel.NodeList)
                {
                    sw.WriteLine("n,{0},{1:F8},{2:F8},{3:F8}", item.ID, item.X * 1000, item.Y * 1000, item.Z * 1000);
                }
            }
        }

        private void WriteElem(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false))
            {
                sw.WriteLine("/prep7");
                var secn_list = (from FEMElement e in theFEMModel.ElementList select e.Secn).ToList();
                secn_list = secn_list.Distinct().ToList();
                foreach (var secnn in secn_list)
                {
                    sw.WriteLine("secn,{0}", secnn);
                    sw.WriteLine("mat,{0}", 100 + secnn);
                    var eles = (from e in theFEMModel.ElementList where e.Secn == secnn select e).ToList();
                    foreach (var item in eles)
                    {
                        sw.WriteLine("e,{0},{1}", item.Ni, item.Nj);
                    }
                }

                foreach (var item in theFEMModel.RigidGroups)
                {
                    sw.WriteLine("nsel,s,node,,{0}", item.Item1);
                    foreach (var slv in item.Item2)
                    {
                        sw.WriteLine("nsel,a,node,,{0}", slv);
                    }
                    //sw.WriteLine("cerig,{0},all,all",item.Item1);
                    sw.WriteLine("cp,next,all,all");
                }


            }
        }

        private void WriteSection(string filepath)
        {

            var sect = (from e in theFEMModel.RelatedArchBridge.PropertyTable select e.Section as TubeSection).ToList();

            sect = sect.Distinct().ToList();

            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (var item in sect)
                {
                    if (item.IsCFTS)
                    {
                        sw.WriteLine("!----------------SECTION{0}----------------", item.SECN);
                        CFTS PropertyCal = new CFTS(item.Diameter * 1000, item.Thickness * 1000, 80, 420);
                        sw.WriteLine("MP,EX,{0},{1}", item.SECN + 100, PropertyCal.E);
                        sw.WriteLine("MP,DENS,{0},{1}", item.SECN + 100, PropertyCal.density);
                        sw.WriteLine("MP,ALPX,{0},1.2E-5", item.SECN + 100);
                        sw.WriteLine("MP,NUXY,{0},0.3", item.SECN + 100);

                        sw.WriteLine("SECTYPE,{0},BEAM,CSOLID,Tube{0},0", item.SECN);
                        sw.WriteLine("SECDATA,{0},0,0,0,0,0,0,0,0,0,0,0", item.Diameter * 1000 * 0.5);
                    }
                    else
                    {
                        sw.WriteLine("!----------------SECTION{0}----------------", item.SECN);
                        sw.WriteLine("MP,EX,  {0},205E6", item.SECN + 100);
                        sw.WriteLine("MP,DENS,{0},7.85e-9", item.SECN + 100);
                        sw.WriteLine("MP,ALPX,{0},1.2E-5", item.SECN + 100);
                        sw.WriteLine("MP,NUXY,{0},0.3", item.SECN + 100);

                        sw.WriteLine("SECTYPE,{0},BEAM,CTUBE,TubeSection{0},0", item.SECN);
                        sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0",
                            item.Diameter * 1000 * 0.5 - item.Thickness * 1000, item.Diameter * 1000 * 0.5);
                    }

                }
            }
        }

        private void WriteMaterial(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("/prep7");
                sw.WriteLine("SECTYPE,{0},BEAM,CSOLID,Tube{0},0", 99);
                sw.WriteLine("SECDATA,{0},0,0,0,0,0,0,0,0,0,0,0", 500);
                sw.WriteLine("MP,EX,  199,205E16");
                sw.WriteLine("MP,DENS,199,7.85e-19");
                sw.WriteLine("MP,ALPX,199,1.2E-5");
                sw.WriteLine("MP,NUXY,199,0.3");
                sw.WriteLine("et,1,188");
            }
        }

        private void WriteMainInp(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("finish");
                sw.WriteLine("/CLEAR,START");
                sw.WriteLine("/TITLE,YunNanProject");
                sw.WriteLine("/CWD,'C:\\Users\\IBD2\\AnsysBin'");
                sw.WriteLine("/input,material,inp");
                sw.WriteLine("/input,section,inp");
                sw.WriteLine("/input,node,inp");
                sw.WriteLine("/input,element,inp");
                sw.WriteLine("/input,solu,inp");
                sw.WriteLine("eplot");
            }
        }
        private void WriteSolu(string filepath)
        {
            var nd = theFEMModel.NodeList.FindLast((x) => x.ID <= 11999);
            List<int> idlist = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var num = (i + 1) * 10000 + (j + 1) * 1000 + k;
                        idlist.Add(num);
                        num = (i + 1) * 10000 + (j + 1) * 1000 + (nd.ID - 11000 - k);
                        idlist.Add(num);
                    }

                }

            }

            using (StreamWriter sw = new StreamWriter(filepath))
            {

                sw.WriteLine("/SOL");

                foreach (var item in idlist)
                {
                    var ch = "a";
                    if (item == idlist[0])
                    {
                        ch = "s";
                    }
                    sw.WriteLine(string.Format("nsel,{0},node,,{1}", ch, item));
                }
                sw.WriteLine("d,all,all");
                double L1X = theFEMModel.RelatedArchBridge.Axis.L1 * 1000;
                double L1Y = theFEMModel.RelatedArchBridge.Axis.GetCenter(theFEMModel.RelatedArchBridge.Axis.L1).Y * 1000;
                sw.WriteLine("nsel,s,loc,x,{0},{1}", L1X - 1, L1X + 1);
                sw.WriteLine("nsel,r,loc,y,{0},{1}", L1Y - 1, L1Y + 1);
                sw.WriteLine("d,all,all");
                sw.WriteLine("nsel,s,loc,x,{0},{1}", -L1X - 1, -L1X + 1);
                sw.WriteLine("nsel,r,loc,y,{0},{1}", L1Y - 1, L1Y + 1);
                sw.WriteLine("d,all,all");

                sw.WriteLine("allsel");
                sw.WriteLine("antype,0");
                //sw.WriteLine("NLGEOM,1");
                //sw.WriteLine("PSTRES,1");
                sw.WriteLine("allsel");
                sw.WriteLine("acel,0,9800,0");
                sw.WriteLine("NSUBST,1");
                sw.WriteLine("OUTRES,ERASE");
                sw.WriteLine("OUTRES,ALL,LAST");
                sw.WriteLine("time,1");
                sw.WriteLine("solve");

            }
        }
        #endregion
    }
}
