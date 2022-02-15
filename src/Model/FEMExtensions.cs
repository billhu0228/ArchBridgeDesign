using MathNet.Spatial.Euclidean;
//using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class FEMExtensions
    {
        /// <summary>
        /// 生成ansys模型
        /// </summary>
        /// <param name="theArch"></param>
        /// <param name="dirPath"></param>
        public static void CreateAnsysModel(this Arch theArch, string dirPath)
        {

        }

        //public static Point2D C2D(this Point3D pt)
        //{
        //    return new Point2D(pt.X, pt.Y);
        //}
        public static Point2D UperPT(this Line2D line)
        {
            if (line.StartPoint.Y > line.EndPoint.Y)
            {
                return line.StartPoint;
            }
            else
            {
                return line.EndPoint;
            }
        }
        public static Point2D LowerPT(this Line2D line)
        {
            if (line.StartPoint.Y > line.EndPoint.Y)
            {
                return line.EndPoint;
            }
            else
            {
                return line.StartPoint;
            }
        }
    }
}
