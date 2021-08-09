using MathNet.Spatial.Euclidean;
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


    }
}
