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
        public int SECN;
        public double Diameter;
        public double Thickness;

        public Section(int secnid)
        {
            SECN = secnid;
        }
    }

    public class RectSection : Section
    {
        public double Width, Length;

        public RectSection(int id, double w, double l) : base(id)
        {
            Width = w;
            Length = l;
            Diameter = double.NaN;
            Thickness = 0;
        }
    }

    public class HSection : Section
    {
        public double W1, W2, W3, t1, t2, t3;

        public HSection(int id, double w1, double w2, double w3, double t1, double t2, double t3) : base(id)
        {
            W1 = w1;
            W2 = w2;
            W3 = w3;
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            Diameter = w1;
            Thickness = 0;
        }

        public double Area
        {
            get
            {
                return W1 * t1 + W2 * t2 + (W3 - t1 - t2) * t3;
            }
        }

        public override string ToString()
        {
            return string.Format("H{0:G}X{1:G}X{2:G}X{3:G}", W3 * 1000, W1 * 1000, t3 * 1000, t1 * 1000);
        }

    }

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
