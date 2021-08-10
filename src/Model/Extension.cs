using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class Extension
    {


        /// <summary>
        /// 值域
        /// </summary>
        /// <param name="vv"></param>
        /// <param name="vto"></param>
        /// <returns></returns>
        public static double SignedAngleBetween(this Vector2D vv,Vector2D vto)
        {
            var ang = vv.SignedAngleTo(vto);

            if (ang.Radians<-Math.PI)
            {
                return ang.Radians + 2 * Math.PI;

            }
            else if(ang.Radians<Math.PI)
            {
                return ang.Radians;
            }
            else
            {
                return ang.Radians - 2 * Math.PI;
            }

        }

        public static double GetY(this Line2D L, double t)
        {
            double kk = L.Direction.Y / L.Direction.X;
            double C0 = L.StartPoint.Y - L.StartPoint.X * kk;

            return t *kk + C0;
        }

        public static Line2D Offset(this Line2D L,double val)
        {
            Vector2D xv=L.Direction.Rotate(Angle.FromDegrees(90.0));
            Point2D NewSt = L.StartPoint + xv * val;
            Point2D NewEd = L.EndPoint + xv * val;
            return new Line2D(NewSt, NewEd);
        }


    }
}
