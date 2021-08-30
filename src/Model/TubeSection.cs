using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 截面
    /// </summary>
    public abstract class Section
    {
        public double Diameter;
    }

    public class RectSection : Section
    {
        public double Width, Length;

        public RectSection(double w,double l)
        {
            Width = w;
            Length = l;
            Diameter = double.NaN;
        }
    }

    public class HSection : Section
    {
        public double W1, W2, W3, t1, t2, t3;

        public HSection(double w1, double w2, double w3, double t1, double t2, double t3)
        {
            W1 = w1;
            W2 = w2;
            W3 = w3;
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            Diameter = w1;
        }
        public override string ToString()
        {
            return string.Format("H{0:G}X{1:G}X{2:G}X{3:G}", W3* 1000, W1 * 1000,t3*1000,t1*1000);
        }

    }

    public class TubeSection:Section
    {
        public double Thickness;
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
