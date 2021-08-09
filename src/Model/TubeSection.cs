using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TubeSection
    {
        public double Thickness;
        public double Diameter;
        public double Area;
        public TubeSection(double dia, double th)
        {
            Thickness = th;
            Diameter = dia;
            Area = Math.PI * (Math.Pow((dia * 0.5), 2) - Math.Pow((dia * 0.5 - th), 2));
        }

        public override string ToString()
        {
            return string.Format("{0:G}x{1:F0}", Diameter*1000, Thickness*1000);
        }
    }
}
