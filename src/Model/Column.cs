using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Column : IComparer<Column>
    {
        public Arch MainArch;
        public double X;
        public double RelativeHeight;
        public int ID;
        public double W, L;
        public double BeamStep,InstallOffset;
        public int InstallSteps;
        public TubeSection MainSection, WSection, LSection;
        public double FootW, FootL,FootMinH;

        public Column() { }

        /// <summary>
        /// 拱上立柱
        /// </summary>
        /// <param name="mainArch">关联主拱</param>
        /// <param name="x">位置</param>
        /// <param name="relativeHeight">相对拱顶高度</param>
        /// <param name="iD">编号</param>
        /// <param name="len">顺桥向长度</param>
        /// <param name="step">节段长度</param>
        /// <param name="nstep">安装节段数</param>
        /// <param name="offset">节段接缝相对偏移绝对值</param>
        /// <param name="footW">柱脚宽度横桥向</param>
        /// <param name="footL">柱脚长度顺桥向</param>
        /// <param name="footMinH">柱脚最小高度，默认0.5m</param>
        public Column(Arch mainArch, double x, double relativeHeight, int iD, 
            double len,double step,int nstep,double offset,
            double footW, double footL, double footMinH=0.5)
        {
            MainArch = mainArch ?? throw new ArgumentNullException(nameof(mainArch));
            this.X = x;
            RelativeHeight = relativeHeight;
            ID = iD;
            W = mainArch.WidthInside + mainArch.WidthOutside*2;
            L = len;
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

    }
}
