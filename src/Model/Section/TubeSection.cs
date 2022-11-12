using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    public class TubeSection : Section
    {
        public double Area;
        public bool IsCFTS;
        public TubeSection(int id, double dia, double th, bool isCFTS = false) : base(id)
        {
            Thickness = th;
            Diameter = dia;
            Area = Math.PI * (Math.Pow((dia * 0.5), 2) - Math.Pow((dia * 0.5 - th), 2));
            IsCFTS = isCFTS;
        }

        public override string ToString()
        {
            return string.Format("{0:G}x{1:F0}", Diameter * 1000, Thickness * 1000);
        }
        /// <summary>
        /// 内径
        /// </summary>
        public double CircumferenceI
        {
            get { return Math.PI * (Diameter - Thickness * 2); }
        }
        /// <summary>
        /// 外径
        /// </summary>
        public double CircumferenceE
        {
            get { return Math.PI * Diameter; }
        }
        public static double GetAs(double diameter, double thick)
        {
            double r = diameter * 0.5;
            double r2 = diameter * 0.5 - thick;
            return Math.PI * (r * r - r2 * r2);
        }
    }
}
