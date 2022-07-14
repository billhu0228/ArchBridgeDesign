using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    public class InstallationSegment : IComparer<InstallationSegment>, ICloneable
    {

        public int Id;
        public Arch theArch;
        public double X_Start, X_End;
        public double InstallGap;
        public double SideDist, MiddleDist;
        public int NumMiddle;
        public double WebOffset;
        public double NominalLength;

        

        public InstallationSegment()
        {
        }

        /// <summary>
        /// 拱肋吊装节段实例
        /// </summary>
        /// <param name="gap"></param>
        /// <param name="sideD"></param>
        /// <param name="midD"></param>
        /// <param name="numMid"></param>
        /// <param name="offset"></param>
        public InstallationSegment(double gap,double sideD,double midD,int numMid,double offset = 0.060)
        {
            InstallGap = gap;
            SideDist = sideD;
            MiddleDist = midD;
            NumMiddle = numMid;
            WebOffset = offset;
            NominalLength = numMid * midD + 2 * sideD + gap;

            Id = -1;
            theArch = null;
            X_Start = 0;
            X_End = NominalLength;


            


        }

        #region 基本属性

        public double CenterX
        {
            get
            {
                return (X_Start + X_End) * 0.5;
            }
        }
        #endregion

        #region 重载和实现接口
        #endregion





        #region 重载和实现接口
        public override string ToString()
        {
            return string.Format("拼装节段-{0}", Id);
        }
        public int Compare(InstallationSegment x, InstallationSegment y)
        {
            return x.CenterX.CompareTo(y.CenterX);
        }
        public object Clone()
        {
            return MemberwiseClone();
        }


        #endregion
    }
}
