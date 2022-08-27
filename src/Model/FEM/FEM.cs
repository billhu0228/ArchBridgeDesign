using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FEMNode
    {
        public int ID;
        public double X, Y, Z;
        public Point3D location;

        public FEMNode(int v, Point3D point3D)
        {
            ID = v;
            X = point3D.X;
            Y = point3D.Y;
            Z = point3D.Z;
            location = point3D;
        }

        public FEMNode(int iD, double x, double y, double z)
        {
            ID = iD;
            X = x;
            Y = y;
            Z = z;
            location = new Point3D(x, y, z);
        }

        public bool Match(Point2D theRibPt)
        {
            return location.C2D().DistanceTo(theRibPt) < 1e-4;
        }
    }


    public class FEMElement4 : FEMElement
    {
        public int Nk, Nl;
        public FEMElement4(int iD, int ni, int nj, int nk, int nl, int secn) : base(iD, ni, nj, secn)
        {
            Nk = nk;
            Nl = nl;
        }
        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}-{3}:{4}", Ni, Nj,Nk,Nl, Secn);
        }
    }


    public class FEMElement
    {
        public int ID;
        public int Ni, Nj;
        public int Secn;

        public FEMElement(int iD, int ni, int nj, int secn)
        {
            ID = iD;
            Ni = ni;
            Nj = nj;
            Secn = secn;
        }

        /// <summary>
        /// 在线上且不在端点上。
        /// </summary>
        /// <param name="nd"></param>
        /// <param name="locTable"></param>
        /// <returns></returns>
        public bool Include(FEMNode nd, ref List<FEMNode> locTable)
        {
            FEMNode A = locTable.Find(x => x.ID == Ni);
            FEMNode B = locTable.Find(x => x.ID == Nj);
            Line3D theLine = new Line3D(A.location, B.location);
            var pt = theLine.ClosestPointTo(nd.location, true);
            if (pt.DistanceTo(nd.location) < 1e-4)
            {
                if (pt.DistanceTo(A.location) > 1e-3 && pt.DistanceTo(B.location) > 1e-3)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }

        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", Ni, Nj, Secn);
        }




    }

}
