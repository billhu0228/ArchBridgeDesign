using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnsysInterface
{
    public class SpaceClaimExt
    {
        FEMModel theFEMModel;

        public SpaceClaimExt(FEMModel model)
        {
            theFEMModel = model;
        }


        #region 写出SpaceClaim 脚本
        public void WriteArcRib(string dirPath,double x_from,double x_end,double r_from,double r_end)
        {
            var cwd = Directory.CreateDirectory(dirPath);
            WriteElem(Path.Combine(cwd.FullName, "Test.py"),x_from,x_end,r_from,r_end);
        }

        FEMNode getNode(int nid)
        {
            return theFEMModel.NodeList.Find((x) => x.ID == nid);
        }
        string getPoly(FEMNode ni, FEMNode nj, double dia)
        {
            Vector3D vi = new Vector3D(ni.X, ni.Y, ni.Z);
            Vector3D vj = new Vector3D(nj.X, nj.Y, nj.Z);
            Vector3D vM = vj - vi;
            Vector3D vT = new Vector3D();
            if (!vM.IsParallelTo(UnitVector3D.ZAxis))
            {
                vT = vM.CrossProduct(UnitVector3D.ZAxis);
            }
            else
            {
                vT = vM.CrossProduct(UnitVector3D.XAxis);
            }

            var N = vT.Normalize();
            Vector3D v3 = vj + 0.5 * dia * N;
            string ret = "#------------------- "+
                string.Format("start = Point.Create(M({0}), M({1}), M({2}))\n", ni.X, ni.Y, ni.Z) +
                string.Format("start = Point.Create(M({0}), M({1}), M({2}))\n", nj.X, nj.Y, nj.Z) +
                string.Format(" Point.Create(M({0}), M({1}), M({2})),", v3.X, v3.Y, v3.Z) +
                "ExtrudeType.None)";
            return ret;
        }
        
        string getScript(FEMNode ni, FEMNode nj, double dia)
        {
            Vector3D vi = new Vector3D(ni.X, ni.Y, ni.Z);
            Vector3D vj = new Vector3D(nj.X, nj.Y, nj.Z);
            Vector3D vM = vj - vi;
            Vector3D vT = new Vector3D();
            if (!vM.IsParallelTo(UnitVector3D.ZAxis))
            {
                vT = vM.CrossProduct(UnitVector3D.ZAxis);
            }
            else
            {
                vT = vM.CrossProduct(UnitVector3D.XAxis);
            }
         
            var N=vT.Normalize();
            Vector3D v3 = vj + 0.5 * dia * N;
            string ret = "CylinderBody.Create(" +
                string.Format(" Point.Create(M({0}), M({1}), M({2})),", ni.X, ni.Y, ni.Z) +
                string.Format(" Point.Create(M({0}), M({1}), M({2})),", nj.X, nj.Y, nj.Z) +
                string.Format(" Point.Create(M({0}), M({1}), M({2})),", v3.X, v3.Y, v3.Z) +
                "ExtrudeType.None)";
            return ret;
        }

        private void WriteElem(string filepath,double x_from,double x_end,double rib_from,double rib_end)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false))
            {
                int body = 0;
                sw.WriteLine("options = SweepCommandOptions()");
                sw.WriteLine("options.ExtrudeType = ExtrudeType.Add");
                sw.WriteLine("options.Select = True");
                var Up = from pt in theFEMModel.RelatedArchBridge.UpSkeleton
                         where (pt.X >= rib_from && pt.X <= rib_end)
                         select pt;
                var Low = from pt in theFEMModel.RelatedArchBridge.LowSkeleton
                         where (pt.X >= rib_from && pt.X <= rib_end)
                         select pt;
                double wi = theFEMModel.RelatedArchBridge.WidthInside;
                double wo = theFEMModel.RelatedArchBridge.WidthOutside;
                List<double> zs = new List<double>() { -wo - 0.5 * wi, -0.5 * wi, 0.5 * wi, 0.5 * wi + wo };
                foreach (var z in zs)
                {
                    sw.WriteLine("#--------------------------------");
                    sw.WriteLine("pts=List[Point]()");
                    foreach (var item in Up)
                    {
                        sw.WriteLine("pts.Add(Point.Create(M({0}),M({1}),M({2})))",item.X,item.Y,z);
                    }
                    sw.WriteLine("pl=SketchLine.CreatePolyLine(pts)");
                    sw.WriteLine("nn=pts[1]-pts[0]");
                    sw.WriteLine("dX=Direction.DirZ");
                    sw.WriteLine("dY=Vector.Cross(nn,dX.UnitVector).Direction");
                    sw.WriteLine("fra=Plane.Create(Frame.Create(pts[0],dX,dY))");
                    sw.WriteLine("ViewHelper.SetSketchPlane(fra, None)");
                    sw.WriteLine("clc=SketchCircle.Create(pts[0], M(0.7))");
                    sw.WriteLine("ViewHelper.SetViewMode(InteractionMode.Solid, None)");
                    sw.WriteLine("selection = FaceSelection.Create(GetRootPart().Bodies[{0}].Faces[0])", body);
                    body += 1;
                    sw.WriteLine("tra=Selection.Create(pl.CreatedCurves)");
                    sw.WriteLine("Sweep.Execute(selection, tra, options, None)");
                    sw.WriteLine("#--------------------------------");
                    sw.WriteLine("pts=List[Point]()");
                    foreach (var item in Low)
                    {
                        sw.WriteLine("pts.Add(Point.Create(M({0}),M({1}),M({2})))", item.X, item.Y, z);
                    }
                    sw.WriteLine("pl=SketchLine.CreatePolyLine(pts)");
                    sw.WriteLine("nn=pts[1]-pts[0]");
                    sw.WriteLine("dX=Direction.DirZ");
                    sw.WriteLine("dY=Vector.Cross(nn,dX.UnitVector).Direction");
                    sw.WriteLine("fra=Plane.Create(Frame.Create(pts[0],dX,dY))");
                    sw.WriteLine("ViewHelper.SetSketchPlane(fra, None)");
                    sw.WriteLine("clc=SketchCircle.Create(pts[0], M(0.7))");
                    sw.WriteLine("ViewHelper.SetViewMode(InteractionMode.Solid, None)");
                    sw.WriteLine("selection = FaceSelection.Create(GetRootPart().Bodies[{0}].Faces[0])", body);
                    body += 1;
                    sw.WriteLine("tra=Selection.Create(pl.CreatedCurves)");
                    sw.WriteLine("Sweep.Execute(selection, tra, options, None)");
                    sw.WriteLine("#--------------------------------");
                }
                

                var secn_list = (from FEMElement e in theFEMModel.ElementList select e.Secn).ToList();
                secn_list = secn_list.Distinct().ToList();
                List<int> unselectedsec = new List<int>() { 65,66,71,72 };
                foreach (var secnn in secn_list)
                {
                    if (unselectedsec.Contains(secnn))
                    {
                        continue;
                    }
                    double dia = getDiameter(secnn);
                    var theSection = getSection(secnn);
                    var eles = (from e in theFEMModel.ElementList where e.Secn == secnn select e).ToList();

                    sw.WriteLine("# Secn={0}--"+theSection.ToString(), secnn);
                    foreach (var item in eles)
                    {
                        FEMNode Ni = getNode(item.Ni);
                        FEMNode Nj = getNode(item.Nj);
                        if ((Ni.X + Nj.X) * 0.5 > x_end || (Ni.X + Nj.X) * 0.5 < x_from)
                        {
                            continue;
                        }
                        string scp = getScript(Ni, Nj, dia);
                        sw.WriteLine(scp);
                    }
                }
            }
        }


        Section getSection(int secn)
        {
            var section = from s in theFEMModel.RelatedArchBridge.PropertyTable
                          where s.Section.SECN == secn
                          select s.Section;

            return section.ToList()[0];
        }

        double getDiameter(int secn)
        {
            var section = from s in theFEMModel.RelatedArchBridge.PropertyTable
                          where s.Section.SECN == secn
                          select s.Section;

            return section.ToList()[0].Diameter;
        }
        #endregion
    }
}
