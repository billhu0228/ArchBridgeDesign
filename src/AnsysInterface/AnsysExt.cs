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
            WriteSolve(Path.Combine(cwd.FullName, "solve.inp"));
            WriteConstraint(Path.Combine(cwd.FullName, "constraint.inp"));
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
                    int matid;
                    if (secnn<20)
                    {
                        matid = secnn;
                    }
                    else
                    {
                        matid = 21;
                    }
                    sw.WriteLine("secn,{0}", secnn);
                    sw.WriteLine("mat,{0}", matid);
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
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("/prep7");
                sw.WriteLine("SECTYPE,  1,BEAM,CSOLID,CFST-1400*25,0");
                sw.WriteLine("SECDATA,700,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE,  2,BEAM,CSOLID,CFST-1400*28,0");
                sw.WriteLine("SECDATA,700,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE,  3,BEAM,CSOLID,CFST-1400*32,0");
                sw.WriteLine("SECDATA,700,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE,  4,BEAM,CSOLID,CFST-1400*38,0");
                sw.WriteLine("SECDATA,700,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE,  5,BEAM,CSOLID,CFST-1400*40,0");
                sw.WriteLine("SECDATA,700,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE, 11,BEAM,CSOLID,CFST-1500*25,0");
                sw.WriteLine("SECDATA,750,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE, 12,BEAM,CSOLID,CFST-1500*28,0");
                sw.WriteLine("SECDATA,750,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE, 13,BEAM,CSOLID,CFST-1500*32,0");
                sw.WriteLine("SECDATA,750,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE, 14,BEAM,CSOLID,CFST-1500*38,0");
                sw.WriteLine("SECDATA,750,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE, 15,BEAM,CSOLID,CFST-1500*40,0");
                sw.WriteLine("SECDATA,750,0,0,0,0,0,0,0,0,0,0,0");
                sw.WriteLine("SECTYPE, 21,BEAM,CTUBE,TUBE-900*20 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (900 - 20 * 2), 0.5 * 900);
                sw.WriteLine("SECTYPE, 22,BEAM,CTUBE,TUBE-800*16 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (800 - 16 * 2), 0.5 * 800);
                sw.WriteLine("SECTYPE, 23,BEAM,CTUBE,TUBE-600*12 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (600 - 12 * 2), 0.5 * 600);
                sw.WriteLine("SECTYPE, 31,BEAM,CTUBE,TUBE-800*16 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (800 - 16 * 2), 0.5 * 800);
                sw.WriteLine("SECTYPE, 32,BEAM,CTUBE,TUBE-300*10 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (300 - 10 * 2), 0.5 * 300);
                sw.WriteLine("SECTYPE, 41,BEAM,CTUBE,TUBE-800*16 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (800 - 16 * 2), 0.5 * 800);
                sw.WriteLine("SECTYPE, 42,BEAM,CTUBE,TUBE-500*10 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (500 - 10 * 2), 0.5 * 500);
                sw.WriteLine("SECTYPE, 51,BEAM,CTUBE,TUBE-600*16 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (600 - 16 * 2), 0.5 * 600);
                sw.WriteLine("SECTYPE, 61,BEAM,CTUBE,TUBE-900*20 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (800 - 16 * 2), 0.5 * 800);
                sw.WriteLine("SECTYPE, 62,BEAM,CTUBE,TUBE-800*16 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (700 - 16 * 2), 0.5 * 700);
                sw.WriteLine("SECTYPE, 63,BEAM,CTUBE,TUBE-600*12 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (450 - 10 * 2), 0.5 * 450);
                sw.WriteLine("SECTYPE, 64,BEAM,CTUBE,TUBE-600*12 ,0");
                sw.WriteLine("SECDATA,{0},{1},36,0,0,0,0,0,0,0,0,0", 0.5 * (300 - 10 * 2), 0.5 * 300);
            }
        }

        private void WriteMaterial(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("/prep7");
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
                sw.WriteLine("MP,EX,  11,4.9013e+04");
                sw.WriteLine("MP,DENS,11,2.8507e-09");
                sw.WriteLine("MP,ALPX,11,1.0131e-05");
                sw.WriteLine("MP,NUXY,11,0.2");
                sw.WriteLine("MP,EX,  12,5.0310e+04");
                sw.WriteLine("MP,DENS,12,2.8920e-09");
                sw.WriteLine("MP,ALPX,12,1.0147e-05");
                sw.WriteLine("MP,NUXY,12,0.2");
                sw.WriteLine("MP,EX,  13,5.2030e+04");
                sw.WriteLine("MP,DENS,13,2.9468e-09");
                sw.WriteLine("MP,ALPX,13,1.0167e-05");
                sw.WriteLine("MP,NUXY,13,0.2");
                sw.WriteLine("MP,EX,  14,5.4593e+04");
                sw.WriteLine("MP,DENS,14,3.0284e-09");
                sw.WriteLine("MP,ALPX,14,1.0198e-05");
                sw.WriteLine("MP,NUXY,14,0.2");
                sw.WriteLine("MP,EX,  15,5.5442e+04");
                sw.WriteLine("MP,DENS,15,3.0554e-09");
                sw.WriteLine("MP,ALPX,15,1.0208e-05");
                sw.WriteLine("MP,NUXY,15,0.2");
                sw.WriteLine("MP,EX,  21,2.06e+05");
                sw.WriteLine("MP,DENS,21,7.85e-09");
                sw.WriteLine("MP,ALPX,21,1.2e-05");
                sw.WriteLine("MP,NUXY,21,0.2");
                sw.WriteLine("MP,EX,  22,2.06e+05");
                sw.WriteLine("MP,DENS,22,7.85e-09");
                sw.WriteLine("MP,ALPX,22,1.2e-05");
                sw.WriteLine("MP,NUXY,22,0.2");
                sw.WriteLine("MP,EX,  37,3.70e+04");
                sw.WriteLine("MP,DENS,37,2.7e-09");
                sw.WriteLine("MP,ALPX,37,1.2e-05");
                sw.WriteLine("MP,NUXY,37,0.2");

                sw.WriteLine("SECTYPE,{0},BEAM,CSOLID,Tube{0},0", 99);
                sw.WriteLine("SECDATA,{0},0,0,0,0,0,0,0,0,0,0,0", 500);
                sw.WriteLine("MP,EX,  99,205E16");
                sw.WriteLine("MP,DENS,99,7.85e-19");
                sw.WriteLine("MP,ALPX,99,1.2E-5");
                sw.WriteLine("MP,NUXY,99,0.3");
                sw.WriteLine("et,1,188");
                sw.WriteLine("et,2,181");
            }
        }

        private void WriteMainInp(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("finish");
                sw.WriteLine("/CLEAR,START");
                sw.WriteLine("/TITLE,YunNanProject");
                sw.WriteLine("!/CWD,'C:\\Users\\IBD2\\AnsysBin'");
                sw.WriteLine("/input,material,inp");
                sw.WriteLine("/input,section,inp");
                sw.WriteLine("/input,node,inp");
                sw.WriteLine("/input,element,inp");
                sw.WriteLine("/input,DeckA,inp");              
                sw.WriteLine("/input,DeckB,inp");
                sw.WriteLine("/input,constraint,inp");
                sw.WriteLine("/input,solve,inp");
            }
        }
        private void WriteSolve(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("/solu");
                sw.WriteLine("allsel");
                sw.WriteLine("antype,0");
                sw.WriteLine("acel,0,9800,0");
                sw.WriteLine("solve");
                sw.WriteLine("/post26");
                sw.WriteLine("nsel,s,node,,12000");
                sw.WriteLine("esln,s");
                sw.WriteLine("*GET,low,ELEM, 0,num,max");
                sw.WriteLine("allsel");
                sw.WriteLine("esol,2, 1,11000,smisc,1,TopCoord");
                sw.WriteLine("esol,3,low,12000,smisc,1,BotCoord");
                sw.WriteLine("PrVAR,2,3");
            }
        }
        private void WriteConstraint(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.WriteLine("/prep7");
                sw.WriteLine("nsel,s,node,,11000");
                sw.WriteLine("nsel,a,node,,12000");
                sw.WriteLine("nsel,a,node,,21000");
                sw.WriteLine("nsel,a,node,,22000");
                sw.WriteLine("nsel,a,node,,31000");
                sw.WriteLine("nsel,a,node,,32000");
                sw.WriteLine("nsel,a,node,,41000");
                sw.WriteLine("nsel,a,node,,42000");
                sw.WriteLine("nsel,a,node,,80000");
                sw.WriteLine("nsel,a,node,,80001");
                sw.WriteLine("nsel,a,node,,80002");
                sw.WriteLine("nsel,a,node,,80003");
                sw.WriteLine("cm,footA,node");
                sw.WriteLine("nsel,s,node,,11211");
                sw.WriteLine("nsel,a,node,,12211");
                sw.WriteLine("nsel,a,node,,21211");
                sw.WriteLine("nsel,a,node,,22211");
                sw.WriteLine("nsel,a,node,,31211");
                sw.WriteLine("nsel,a,node,,32211");
                sw.WriteLine("nsel,a,node,,41211");
                sw.WriteLine("nsel,a,node,,42211");
                sw.WriteLine("nsel,a,node,,80012");
                sw.WriteLine("nsel,a,node,,80013");
                sw.WriteLine("nsel,a,node,,80014");
                sw.WriteLine("nsel,a,node,,80015");
                sw.WriteLine("cm,footB,node");
                sw.WriteLine("cmsel,a,footA");
                sw.WriteLine("d,all,ux,0");
                sw.WriteLine("d,all,uy,0");
                sw.WriteLine("d,all,uz,0");
                sw.WriteLine("nsel,s,node,,200112");
                sw.WriteLine("nsel,a,node,,200334");
                sw.WriteLine("nsel,a,node,,200556");
                sw.WriteLine("nsel,a,node,,200222");
                sw.WriteLine("nsel,a,node,,200444");
                sw.WriteLine("nsel,a,node,,200666");
                sw.WriteLine("nsel,a,node,,300112");
                sw.WriteLine("nsel,a,node,,300334");
                sw.WriteLine("nsel,a,node,,300556");
                sw.WriteLine("nsel,a,node,,300222");
                sw.WriteLine("nsel,a,node,,300444");
                sw.WriteLine("nsel,a,node,,300666");
                sw.WriteLine("d,all,uy,0");
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
