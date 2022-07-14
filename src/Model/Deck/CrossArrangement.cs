using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CrossArrangement
    {
        public double Width, SlabTk, SlabGap;
        public double CrossDistA, CrossDistB;
        public List<double> GriderArr, SubGirderArr;

        /// <summary>
        /// 配置正交横截面
        /// </summary>
        /// <param name="width">横截面总宽（护栏外缘）</param>
        /// <param name="slabTk">桥面板厚度</param>
        /// <param name="slabGap">桥面板-主梁上缘间距</param>
        /// <param name="crossDistA">横梁上弦至主梁上表面距离</param>
        /// <param name="crossDistB">横梁下弦至横梁上弦上表面距离，0表示无下弦</param>
        /// <param name="griderArr">主梁间距列表（含边梁至护栏外缘距离）</param>
        /// <param name="subGirderArr">次梁间距列表（含边梁至护栏外缘距离）</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CrossArrangement(double width, double slabTk, double slabGap,
            double crossDistA, double crossDistB, List<double> griderArr, List<double> subGirderArr)
        {
            Width = width;
            SlabTk = slabTk;
            SlabGap = slabGap;
            CrossDistA = crossDistA;
            CrossDistB = crossDistB;
            GriderArr = griderArr ?? throw new ArgumentNullException(nameof(griderArr));
            SubGirderArr = subGirderArr ?? throw new ArgumentNullException(nameof(subGirderArr));
        }
    }
}
