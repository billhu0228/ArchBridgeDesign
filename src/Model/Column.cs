    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 拱上立柱
    /// </summary>
    public class Column : IComparer<Column>
    {
        public Arch MainArch;
        public double X;
        public double RelativeHeight;
        public int ID;
        public double W, L,CapHeight;
        public double BeamStep,InstallOffset;
        public int InstallSteps;
        public TubeSection MainSection, WSection, LSection;
        public double FootW, FootL,FootMinH;

        double a, b, c,h;
        int m, n, k;
        double z0, z1, z2;

        public Column() { }

        /// <summary>
        /// 拱上立柱
        /// </summary>
        /// <param name="mainArch">关联主拱</param>
        /// <param name="x">位置</param>
        /// <param name="relativeHeight">相对拱脚高度</param>
        /// <param name="iD">编号</param>
        /// <param name="len">顺桥向长度</param>
        /// <param name="CapH">盖梁高度</param>
        /// <param name="step">节段长度</param>
        /// <param name="nstep">安装节段数</param>
        /// <param name="offset">节段接缝相对偏移绝对值</param>
        /// <param name="footW">柱脚宽度横桥向</param>
        /// <param name="footL">柱脚长度顺桥向</param>
        /// <param name="footMinH">柱脚最小高度，默认0.5m</param>
        public Column(Arch mainArch, double x, double relativeHeight, int iD, 
            double len,double capH,double step,int nstep,double offset,
            double footW, double footL, double footMinH=0.5)
        {
            MainArch = mainArch ?? throw new ArgumentNullException(nameof(mainArch));
            this.X = x;
            RelativeHeight = relativeHeight;
            ID = iD;
            W = mainArch.WidthInside + mainArch.WidthOutside*2;
            L = len;
            CapHeight = capH;
            BeamStep = step;
            InstallSteps = nstep;
            InstallOffset = offset;
            FootW = footW;
            FootL = footL;
            FootMinH = footMinH;
        }


        public int Compare(Column x, Column y)
        {
            return x.X.CompareTo(y.X);
        }

        public override string ToString()
        {
            return string.Format("拱上立柱 {0}, H={1}  ", ID,RelativeHeight);
        }

        /// <summary>
        /// 拱上立柱生成
        /// </summary>
        public void Generate()
        {
            CalculateParameters();


        }

        #region 立柱尺寸参数
        /// <summary>
        /// 起步节长度
        /// </summary>
        public double A { get { return a; } }

        /// <summary>
        /// 起步段高度
        /// </summary>
        public double B { get { return b; } }

        /// <summary>
        /// 起步终点段高度
        /// </summary>
        public double C { get { return c; } }

        /// <summary>
        /// 净高
        /// </summary>
        public double H { get { return h; } }

        /// <summary>
        /// 柱脚底面标高
        /// </summary>
        public double Z0 { get { return z0; } }
        public double Z1 { get { return z1; } }
        public double Z2 { get { return z2; } }

        /// <summary>
        /// 标准间距数量
        /// </summary>
        public int M { get { return m; } }

        /// <summary>
        /// 标准安装节段数量
        /// </summary>
        public int N { get { return n; } }

        /// <summary>
        /// 起步标准距数量
        /// </summary>
        public int K { get { return k; } }

        public double MainDiameter 
        { 
            get
            {
                return MainArch.GetTubeProperty(X, eMemberType.ColumnMain).Section.Diameter ;
            }
        }
        public double CrossLDiameter
        { 
            get
            {
                return MainArch.GetTubeProperty(X, eMemberType.ColumnCrossL).Section.Diameter ;
            }
        }  
        public double CrossWDiameter
        { 
            get
            {
                return MainArch.GetTubeProperty(X, eMemberType.ColumnCrossW).Section.Diameter ;
            }
        }



        #endregion


        public void CalculateParameters()
        {
            double x1 = X < 0 ? X + 0.5 * FootL : X - 0.5 * FootL;

            z0 = MainArch.Get7PointReal(X, 90.0)[0].Y; // 柱脚底面标高
            z1 = MainArch.Get7PointReal(x1, 90.0)[0].Y + 0.5; // 柱脚顶面标高
            z2 = -MainArch.Axis.f + RelativeHeight;
            h = z2 - z1;
            double InstallLen = BeamStep * InstallSteps;
            n = (int)((h - CapHeight) / InstallLen); // 标准节数量
            a = h - CapHeight - n * InstallLen; // 起步节长度

            if (a < (BeamStep - InstallOffset))
            {
                k = 0;
                b = InstallOffset+a;
                c = 0;
                m =  n * InstallSteps ;
            }
            else
            {
                c = (BeamStep - InstallOffset);
                k = (int)((a - (BeamStep - InstallOffset)) / BeamStep); // 非标准节步数
                b = a - c - k * BeamStep; // 起步段长度
                if (b<0.5)
                {
                    b = b + BeamStep;
                    k = k - 1;
                }
                m = k + n * InstallSteps + 1;
            }
            double tryH = m * BeamStep + (CapHeight - InstallOffset) + b;

            if (Math.Abs(tryH - h)>1e-6)
            {
                throw new Exception("拱上立柱参数有误");
            }
        }
    }
}
