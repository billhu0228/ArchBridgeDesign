using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model
{



    public class Arch
    {
        public ArchAxis Axis;
        public double H0, H1;
        public Dictionary<int, CutSection> ColumnSection, InstallSection, WebSection;
        protected List<MemberPropertyRecord> PropertyTable;
        public delegate double get_z(double x0);


        public Arch(ArchAxis ax, double height0, double height1)
        {
            Axis = ax;
            H0 = height0;
            H1 = height1;
            ColumnSection = new Dictionary<int, CutSection>();
            InstallSection = new Dictionary<int, CutSection>();
            WebSection = new Dictionary<int, CutSection>();
            PropertyTable = new List<MemberPropertyRecord>();
        }

        public void AssignProperty(MemberType MT, TubeSection sect, double from = double.NegativeInfinity, double to = double.PositiveInfinity)
        {
            int idx = PropertyTable.Count;
            PropertyTable.Add(new MemberPropertyRecord(idx, sect, MT, from, to));
        }

        public int AddSection(int sectid, double location, double angle_deg, SectionType ST)
        {
            Point2D cc = Axis.GetCenter(location);
            switch (ST)
            {
                case SectionType.ColumnSection:
                    ColumnSection.Add(sectid, new CutSection(sectid, cc, Angle.FromDegrees(angle_deg)));
                    break;
                case SectionType.InstallSection:
                    InstallSection.Add(sectid, new CutSection(sectid, cc, Angle.FromDegrees(angle_deg)));
                    break;
                case SectionType.WebSection:
                    WebSection.Add(sectid, new CutSection(sectid, cc, Angle.FromDegrees(angle_deg)));
                    break;
                default:
                    throw new Exception();
            }
            return sectid;
        }



        /// <summary>
        /// x0位置桁高，默认变截面次数2次；
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        public double GetH(double x0, int order = 2)
        {
            if (Math.Abs(x0) < Axis.L1)
            {
                var v0 = Math.Abs(Axis.GetLength(x0));
                var kk = (H1 - H0) / Math.Pow((Axis.ArchLength * 0.5), order);
                return kk * Math.Pow(v0, order) + H0;
            }
            else
            {
                return H1;
            }
        }

        /// <summary>
        /// 发现方向三个轴点
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        List<Point2D> get_3pt(double x0)
        {
            List<Point2D> res = new List<Point2D>();
            double HH = GetH(x0);
            Point2D cc = Axis.GetCenter(x0);
            Vector2D dir = Vector2D.XAxis.Rotate(Axis.GetAngle(x0) + Angle.FromDegrees(90.0));
            res.Add(cc + 0.5 * HH * dir);
            res.Add(cc);
            res.Add(cc - 0.5 * HH * dir);
            return res;
        }

        /// <summary>
        /// 获取任意角度三轴线
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="ang_deg"></param>
        /// <param name="Upper"></param>
        /// <param name="CC"></param>
        /// <param name="Lower"></param>
        public void Get3Point(double x0, double ang_deg, out Point2D Upper, out Point2D CC, out Point2D Lower)
        {
            CC = Axis.GetCenter(x0);
            Angle CutAngle = Angle.FromDegrees(ang_deg);
            Vector2D TargetDir = Vector2D.XAxis.Rotate(CutAngle);
            Func<double, double> f = (x) => ((get_3pt(x)[0] - Axis.GetCenter(x0)).SignedAngleBetween(TargetDir));
            double x_new = Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1,1e-6);
            Upper = get_3pt(x_new)[0];
            f = (x) => ((get_3pt(x)[2] - Axis.GetCenter(x0)).SignedAngleBetween(-TargetDir));
            x_new =  Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1);
            Lower = get_3pt(x_new)[2];
        }

        /// <summary>
        /// 上弦上表面
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        public double UCU(double x0)
        {
            return 0;
        }


        List<Point2D> get_7pt(double x0)
        {
            List<Point2D> res = new List<Point2D>();
            Point2D cc = Axis.GetCenter(x0);

            return res;
        }


    }
}
