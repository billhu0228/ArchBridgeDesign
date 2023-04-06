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

        /// <summary>
        /// 过渡墩
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="h0">墩底</param>
        /// <param name="h1">墩顶</param>
        /// <param name="h2">盖梁顶</param>
        /// <param name="s0"></param>
        /// <param name="s1"></param>
        /// <param name="s_cross"></param>
        public RCColumn(int iD,double h0,double h1,double h2,
            RectSection s0, RectSection s1, RectSection s_cross)
        {
            MainArch = null;
            X = 0;
            ID = iD;
            H0 = h0;
            H1 = h1;
            H2 = h2;
            Section0 = s0;
            Section1 = s1;
            SectionCrossBeam = s_cross;             
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
