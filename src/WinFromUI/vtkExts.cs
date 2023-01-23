using Kitware.VTK;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFromUI
{
    public static class vtkExts
    {
        public static double[] GetBoundsArray(this vtkBox box)
        {
            double[] bds = new double[6];
            GCHandle gchX = default(GCHandle);
            gchX = GCHandle.Alloc(bds, GCHandleType.Pinned);

            box.GetBounds(gchX.AddrOfPinnedObject());
            return bds;
        }
        public static Point3D GetCenter(this vtkBox box)
        {
            double[] bds = box.GetBoundsArray();
            Point3D ret = new Point3D(
                (bds[0] + bds[1]) * 0.5,
                (bds[2] + bds[3]) * 0.5,
                (bds[4] + bds[5]) * 0.5
                );
            return ret;
        }
        public static Point3D[] GetBoundsPts(this vtkBox box)
        {
            Point3D[] ret = new Point3D[8];
            int i = 0;
            var bds = box.GetBoundsArray().ToList();
            foreach (var x in bds.GetRange(0,2))
            {
                foreach (var y in bds.GetRange(2,2))
                {
                    foreach (var z in bds.GetRange(4,2))
                    {
                        ret[i] = new Point3D(x, y, z);
                        i++;
                    }
                }
            }
            return ret;
        }
        public static double GetXLength(this vtkBox box)
        {
            double[] bds = new double[6];
            GCHandle gchX = default(GCHandle);
            gchX = GCHandle.Alloc(bds, GCHandleType.Pinned);
            box.GetBounds(gchX.AddrOfPinnedObject());
            return bds[1] - bds[0];
        }

        public static double GetYLength(this vtkBox box)
        {
            double[] bds = new double[6];
            GCHandle gchX = default(GCHandle);
            gchX = GCHandle.Alloc(bds, GCHandleType.Pinned);
            box.GetBounds(gchX.AddrOfPinnedObject());
            return bds[3] - bds[2];
        }

        public static double GetZLength(this vtkBox box)
        {
            double[] bds = new double[6];
            GCHandle gchX = default(GCHandle);
            gchX = GCHandle.Alloc(bds, GCHandleType.Pinned);
            box.GetBounds(gchX.AddrOfPinnedObject());
            return bds[5] - bds[4];
        }
    }
}
