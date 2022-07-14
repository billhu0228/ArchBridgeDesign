using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 混凝土墩
    /// </summary>
    public class RCColumn : IComparer<RCColumn>
    {
        public Arch MainArch;
        public double X;
        public double H0,H1,H2;
        public int ID;
        public RectSection Section0, Section1, SectionCrossBeam;

        public RCColumn() { }

        public RCColumn(int iD,double h0,double h1,double h2,
            RectSection s0, RectSection s1, RectSection s2)
        {
            MainArch = null;
            X = 0;
            ID = iD;
            H0 = h0;
            H1 = h1;
            H2 = h2;
            Section0 = s0;
            Section1 = s1;
            SectionCrossBeam = s2;             
        }

        public int Compare(RCColumn x, RCColumn y)
        {
            return x.X.CompareTo(y.X);
        }

        public override string ToString()
        {
            return string.Format("桥墩 {0}, H2={1}  ", ID, H2);
        }

    }
}
