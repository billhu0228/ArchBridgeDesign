using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnsysInterface
{
    /// <summary>
    /// OpenSEES 扩展 
    /// </summary>
    public class OPSExt
    {
        FEMModel theFEMModel;
        DirectoryInfo WorkDir, AccDir , OutDir;
        Encoding utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private Dictionary<int, double> MassPerLeng = new Dictionary<int, double>()
        {
            {1,4.4262e-03},{2,4.4941e-03},{3,4.5842e-03},{4,4.7183e-03},{5,4.7628e-03},
            {21,4.340e-04},{22,3.094e-04},{23,1.740e-04},{31,3.094e-04},{32,1.740e-04},{33,7.152e-05},{41,3.094e-04},{42,1.208e-04},{43,1.208e-04},
            {51,2.304e-04},{52,2.304e-04},{53,2.304e-04},{61,4.340e-04},{62,3.094e-04},{63,1.085e-04},{64,1.740e-04},        };
        private Dictionary<int, double> NodeMass = new Dictionary<int, double>();

        private List<int> NodeInUse = new List<int>();
        public OPSExt(FEMModel model,string cwd)
        {
            theFEMModel = model;
            WorkDir = Directory.CreateDirectory(cwd);
            if (WorkDir.Exists)
            {
                WorkDir.Delete(true);
            }
            WorkDir.Create();
            AccDir = Directory.CreateDirectory(Path.Combine(WorkDir.FullName, "Acc"));
            AccDir.Create();
            OutDir = Directory.CreateDirectory(Path.Combine(WorkDir.FullName, "Out"));
            OutDir.Create();
            Init();
        }
        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
        private void Init()
        {
            MakeMain();
            MakeMaterial();
            MakeSection();
            MakeElem();
            MakeElemPy();
            MakeNode();            
            MakeMass();
            MakeBD();
            MakeTHAnalysis();
            MakeStatic();
            CopyFilesRecursively(@"G:\Diss\DynamicShearSpring\C2x\Acc", Path.Combine(WorkDir.FullName, "Acc"));
        }
        private void MakeStatic()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "st.tcl"), false, utf8WithoutBom);
            WriteHead(ref sw);
            sw.WriteLine("recorder Element -file Out/force2.out -ele 1 force;");
            sw.WriteLine("recorder Node -file Out/disp2.out -node 41102 -dof 2 disp;");
            sw.WriteLine("wipeAnalysis; ");
            sw.WriteLine("timeSeries Constant 1;");
            sw.WriteLine("pattern UniformExcitation 1 2 -accel 1 -fact 9806.0;");
            sw.WriteLine("constraints Penalty 1.0e25 1.0e25;");
            sw.WriteLine("numberer RCM ;  ");
            sw.WriteLine("system SparseGeneral ;  ");
            sw.WriteLine("test EnergyIncr 1e-1 25 3 ;");
            sw.WriteLine("algorithm ModifiedNewton ;  ");
            sw.WriteLine("integrator LoadControl 1;");
            sw.WriteLine("analysis Static;");
            sw.WriteLine("analyze 1;");

            sw.Flush();
            sw.Close();
            Console.WriteLine("总体写出完成...");

        }
        private void MakeTHAnalysis()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "TH.tcl"), false, utf8WithoutBom);
            WriteHead(ref sw);
            sw.WriteLine("recorder Node -file Out/D1.out -node 31103 -dof 1 disp");
            sw.WriteLine("set dt 0.01");
            sw.WriteLine("timeSeries Path 13 -dt $dt -filePath Acc/RSN6e.acc -factor 1;");
            sw.WriteLine("pattern UniformExcitation 400 1 -accel 13;");
            sw.WriteLine("wipeAnalysis ;  ");
            sw.WriteLine("constraints Transformation ;");
            sw.WriteLine("numberer RCM ;  ");
            sw.WriteLine("system SparseGeneral ;  ");
            sw.WriteLine("test EnergyIncr 1e-1 25 3 ;");
            sw.WriteLine("algorithm ModifiedNewton ;  ");
            sw.WriteLine("integrator Newmark 0.5 0.25;");
            sw.WriteLine("analysis Transient;");
            sw.WriteLine("analyze 10 $dt;");

            sw.Flush();
            sw.Close();
            Console.WriteLine("总体写出完成...");

        }

        private void MakeMass()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "Mass.tcl"), false, utf8WithoutBom);
            foreach (var item in NodeMass.Keys)
            {
                sw.WriteLine("mass {0:G} {1:E4} {1:E4} {1:E4} 0.0 0.0 0.0",item,NodeMass[item]);
            }
            sw.Flush();
            sw.Close();
        }

        private void MakeBD()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "Boundary.tcl"), false, utf8WithoutBom);
            WriteHead(ref sw);
            sw.WriteLine("fix 11000 1 1 1 0 0 0");
            sw.WriteLine("fix 12000 1 1 1 0 0 0");
            sw.WriteLine("fix 21000 1 1 1 0 0 0");
            sw.WriteLine("fix 22000 1 1 1 0 0 0");
            sw.WriteLine("fix 31000 1 1 1 0 0 0");
            sw.WriteLine("fix 32000 1 1 1 0 0 0");
            sw.WriteLine("fix 41000 1 1 1 0 0 0");
            sw.WriteLine("fix 42000 1 1 1 0 0 0");
            sw.WriteLine("fix 80000 1 1 1 0 0 0");
            sw.WriteLine("fix 80001 1 1 1 0 0 0");
            sw.WriteLine("fix 80002 1 1 1 0 0 0");
            sw.WriteLine("fix 80003 1 1 1 0 0 0");
            sw.WriteLine("fix 11205 1 1 1 0 0 0");
            sw.WriteLine("fix 12205 1 1 1 0 0 0");
            sw.WriteLine("fix 21205 1 1 1 0 0 0");
            sw.WriteLine("fix 22205 1 1 1 0 0 0");
            sw.WriteLine("fix 31205 1 1 1 0 0 0");
            sw.WriteLine("fix 32205 1 1 1 0 0 0");
            sw.WriteLine("fix 41205 1 1 1 0 0 0");
            sw.WriteLine("fix 42205 1 1 1 0 0 0");
            sw.WriteLine("fix 80012 1 1 1 0 0 0");
            sw.WriteLine("fix 80013 1 1 1 0 0 0");
            sw.WriteLine("fix 80014 1 1 1 0 0 0");
            sw.WriteLine("fix 80015 1 1 1 0 0 0");
            sw.Flush();
            sw.Close();
            Console.WriteLine("节点写出完成...");
        }
        private void MakeElemPy()
        {

            int eid = 1;
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "elem.py"), false, utf8WithoutBom);
            var secn_list = (from FEMElement e in theFEMModel.ElementList select e.Secn).ToList();
            secn_list = secn_list.Distinct().ToList();
            foreach (var secnn in secn_list)
            {
                if (secnn > 60)
                {
                    continue;
                }
                var eles = (from e in theFEMModel.ElementList where e.Secn == secnn select e).ToList();
                foreach (var item in eles)
                {
                    var mpl = MassPerLeng[secnn];
                    sw.WriteLine("element.( 'dispBeamColumn', {0:G}, {1:G}, {2:G},10, {3:G},'-cMass', '-mass', {4:E3} )", eid, item.Ni, item.Nj, secnn, mpl);
                    var LL = theFEMModel.GetElementLength(item) * 1000;
                    addMass(item.Ni, LL * 0.5 * MassPerLeng[secnn]);
                    addMass(item.Nj, LL * 0.5 * MassPerLeng[secnn]);
                    if (!NodeInUse.Contains(item.Ni))
                    {
                        NodeInUse.Add(item.Ni);
                    }
                    if (!NodeInUse.Contains(item.Nj))
                    {
                        NodeInUse.Add(item.Nj);
                    }
                    eid++;
                }
            }
            sw.Flush();
            sw.Close();
            NodeInUse.Sort();
            Console.WriteLine("节点写出完成...");
        }
        private void MakeElem()
        {

            int eid = 1;
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "Element.tcl"), false, utf8WithoutBom);
            var secn_list = (from FEMElement e in theFEMModel.ElementList select e.Secn).ToList();
            secn_list = secn_list.Distinct().ToList();
            sw.WriteLine("geomTransf Linear 10 0 1 0");
            foreach (var secnn in secn_list)
            {
                if (secnn>60)
                {
                    continue;
                }
                var eles = (from e in theFEMModel.ElementList where e.Secn == secnn select e).ToList();
                foreach (var item in eles)
                {
                    var mpl = MassPerLeng[secnn];
                    sw.WriteLine("element dispBeamColumn  {0:G} {1:G} {2:G} 5 {3:G} 10 -mass {4:E3} -cMass",eid, item.Ni, item.Nj,secnn,mpl);
                    var LL=theFEMModel.GetElementLength(item)*1000;
                    addMass(item.Ni, LL*0.5*MassPerLeng[secnn]);
                    addMass(item.Nj, LL*0.5*MassPerLeng[secnn]);
                    if (!NodeInUse.Contains(item.Ni))
                    {
                        NodeInUse.Add(item.Ni);
                    }
                    if (!NodeInUse.Contains(item.Nj))
                    {
                        NodeInUse.Add(item.Nj);
                    }
                    eid++;
                }
            }
            sw.Flush();
            sw.Close();
            NodeInUse.Sort();
            Console.WriteLine("节点写出完成...");
        }
        private void addMass(int node,double mass)
        {
            if (NodeMass.ContainsKey(node))
            {
                NodeMass[node] += mass;
            }
            else
            {
                NodeMass.Add(node, mass);
            }
        }


        private void MakeNode()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "Node.tcl"), false, utf8WithoutBom);
            WriteHead(ref sw);
            foreach (var item in theFEMModel.NodeList)
            {
                if (!NodeInUse.Contains( item.ID))
                {
                    continue;
                }
                sw.WriteLine("node {0} {1:F8} {2:F8} {3:F8}", item.ID, item.X * 1000, item.Y * 1000, item.Z * 1000);
            }
            sw.Flush();
            sw.Close();
            Console.WriteLine("节点写出完成...");
        }

        private void MakeSection()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "Section.tcl"), false, utf8WithoutBom);
            WriteHead(ref sw);
            WriteCFST(ref sw, 1, 1400, 25, 1, 2, 1.0133018868289454e+16);
            WriteCFST(ref sw, 2, 1400, 28, 1, 2, 1.0637841054192630e+16);
            WriteCFST(ref sw, 3, 1400, 32, 1, 2, 1.1300177542900596e+16);
            WriteCFST(ref sw, 4, 1400, 38, 1, 2, 1.2272116459740616e+16);
            WriteCFST(ref sw, 5, 1400, 40, 1, 2, 1.2590286444313674e+16);

            WriteCFST(ref sw, 21, 900, 20, 1, 3, 1.0992e+15, false);
            WriteCFST(ref sw, 22, 800, 16, 1, 3, 6.2176e+14, false);
            WriteCFST(ref sw, 23, 600, 12, 1, 3, 1.9674e+14, false);
            WriteCFST(ref sw, 31, 800, 16, 1, 3, 6.2176e+14, false);
            WriteCFST(ref sw, 32, 600, 12, 1, 3, 1.9674e+14, false);
            WriteCFST(ref sw, 33, 300, 10, 1, 3, 1.9690e+13, false);
            WriteCFST(ref sw, 41, 800, 16, 1, 3, 6.2176e+14, false);
            WriteCFST(ref sw, 42, 500, 10, 1, 3, 9.4878e+13, false);
            WriteCFST(ref sw, 43, 500, 10, 1, 3, 9.4878e+13, false);
            WriteCFST(ref sw, 51, 600, 16, 1, 3, 2.5709e+14, false);
            WriteCFST(ref sw, 52, 600, 16, 1, 3, 2.5709e+14, false);
            WriteCFST(ref sw, 53, 600, 16, 1, 3, 2.5709e+14, false);
            WriteCFST(ref sw, 61, 900, 20, 1, 3, 1.0992e+15, false);
            WriteCFST(ref sw, 62, 800, 16, 1, 3, 6.2176e+14, false);
            WriteCFST(ref sw, 63, 450, 10, 1, 3, 6.8705e+13, false);
            WriteCFST(ref sw, 64, 600, 12, 1, 3, 1.9674e+14, false);
            sw.Flush();
            sw.Close();
            Console.WriteLine("材料写出完成...");
        }

        private void WriteHead(ref StreamWriter sw)
        {
            sw.WriteLine("# ----------------------------------------#");
            sw.WriteLine("#          诗礼黑惠江大桥时程分析           #");
            sw.WriteLine("#         Modeled By BILL(IS-mm)          #");
            sw.WriteLine("#                 2023-07                 #");
            sw.WriteLine("# ----------------------------------------#");
        }

        private void WriteCFST(ref StreamWriter sw,int id,double D,double th,int concID,int steelID,double GJ,bool conc=true)
        {
            sw.WriteLine("section Fiber {0} -GJ {1:E4} {{", id,GJ);
            sw.WriteLine("    patch circ {0} 24 1 0.0 0.0 {1:G} {2:G} 0 360", steelID, D/2-th,D/2);
            if (conc)
            {
                sw.WriteLine("    patch circ {0} 24 13 0.0 0.0 0.0 {1:G} 0 360", concID, D / 2 - th);
            }            
            sw.WriteLine("}");
        }


        private void MakeMaterial()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName, "Material.tcl"), false, utf8WithoutBom);
            WriteHead(ref sw);
            sw.WriteLine("uniaxialMaterial Concrete01 1 -50.0 -0.003 -30 -0.01;");
            sw.WriteLine("uniaxialMaterial Steel01    2 420 200000 0.1;");
            sw.WriteLine("uniaxialMaterial Steel01    3 335 200000 0.1;");
            sw.WriteLine("uniaxialMaterial Elastic    9 1e9;");
            sw.Flush();
            sw.Close();
            Console.WriteLine("材料写出完成...");
        }

        private void MakeMain()
        {
            StreamWriter sw = new StreamWriter(Path.Combine(WorkDir.FullName,"main.tcl"), false, utf8WithoutBom);
            WriteHead(ref sw);
            sw.WriteLine("wipe;");
            sw.WriteLine("model BasicBuilder -ndm 3 -ndf 6");
            sw.WriteLine("source Material.tcl");
            sw.WriteLine("source Section.tcl");
            sw.WriteLine("source Node.tcl");
            sw.WriteLine("source Element.tcl");
            sw.WriteLine("# source Mass.tcl");
            sw.WriteLine("source Boundary.tcl");

            sw.Flush();
            sw.Close();
            Console.WriteLine("总体写出完成...");
        }
    }
}
