using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 钢管混凝土材料
    /// </summary>
    public class CFTS
    {
        public double fcuk,fy;
        public double D,t;

        public CFTS(double Diameter,double tk,double C_grade,double S_grade)
        {
            D = Diameter;
            t = tk;
            fcuk = C_grade;
            fy = S_grade;
        }
        public void ModifiedThickness(double a)
        {
            t = (1 - 1.0 / Math.Sqrt(a + 1)) * D * 0.5;
        }
        public double a { get { return Math.Pow((D / (D - 2 * t)), 2) - 1; } }
        public double Dc { get { return D-2*t; } }
        public double GetArea(double D_out, double D_in)
        {
            return Math.PI * (D_out * D_out - D_in * D_in) / 4.0;
        }

        public double GetMoment(double D_out, double D_in)
        {
            double a = D_in / D_out;
            return Math.PI * Math.Pow(D_out, 4) * (1 - Math.Pow(a, 4)) / 64.0;
        }
        public double As { get { return GetArea(D, Dc); } }
        public double Ac { get { return GetArea(Dc, 0); } }
        public double Ic { get { return GetMoment(Dc, 0); } }
        public double Is { get { return GetMoment(D, Dc); } }
        public double A { get { return GetArea(D, 0); } }
        public double I { get { return GetMoment(D, 0); } }
        public double fck
        {
            get
            {
                double cx=fcuk<=40? 1.0 : Extension.Interplate(40, 80, 1, 0.87, fcuk);
                double a = fcuk <= 50 ? 0.76 : Extension.Interplate(50, 80, 0.76, 0.82, fcuk);
                return 0.88 * a * fcuk * cx;
            }
        }
        public double Ec { get { return 1e5 / (2.2 + 34.74 / fcuk); } }
        public double Es { get { return 2.06e5; } }
        public double kesi { get { return (As * fy) / (Ac * fck); } }
        public double fscy { get { return (1.14+1.02*kesi)*fck; } }
        public double escy { get { return 1300 + 12.5 * fcuk + (600 + 33.3 * fcuk) * Math.Pow(kesi, 0.2); } }
        public double escp { get { return 3.25e-6*fy; } }
        public double fscp { get { return (0.192*(fy/235)+0.488)*fscy; } }
        public double E 
        { 
            get 
            {
                if (t>=16)
                {
                    return fscp / escp*0.96;
                }
                else
                {
                    return fscp / escp;
                }
            }
        }
        public double density
        {
            get
            {
                return (As * 7.85e-9 + Ac * 2.5e-9) / A;
            }
        }
    }
}
