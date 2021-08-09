using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum SectionType
    {
        ColumnSection=1,
        InstallSection=2,
        WebSection=3,
    }
    public class Arch
    {
        public ArchAxis Axis;
        public double H0, H1;
        public List<CutSection> ColumnSection, InstallSection, WebSection;

        public Arch(ArchAxis ax, double height0, double height1)
        {
            Axis = ax;
            H0 = height0;
            H1 = height1;
        }

        public int AddSection(int sectid,double location,double angle,SectionType ST)
        {
            Point2D cc = Axis.GetCenter(location);
            switch (ST)
            {
                case SectionType.ColumnSection:
                    ColumnSection.Add(new CutSection(sectid, cc, Angle.FromRadians(angle)));
                    break;
                case SectionType.InstallSection:
                    InstallSection.Add(new CutSection(sectid, cc, Angle.FromRadians(angle)));
                    break;
                case SectionType.WebSection:
                    WebSection.Add(new CutSection(sectid, cc, Angle.FromRadians(angle)));
                    break;
                default:
                    throw new Exception();                   
            }
            return sectid;
        }
    }
}
