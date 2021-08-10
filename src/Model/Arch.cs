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

        double HeightOrder;


        public Arch(ArchAxis ax, double height0, double height1, double order = 2)
        {
            Axis = ax;
            H0 = height0;
            H1 = height1;
            ColumnSection = new Dictionary<int, CutSection>();
            InstallSection = new Dictionary<int, CutSection>();
            WebSection = new Dictionary<int, CutSection>();
            PropertyTable = new List<MemberPropertyRecord>();
            HeightOrder = order;
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
        public double GetH(double x0)
        {
            if (Math.Abs(x0) < Axis.L1)
            {
                var v0 = Math.Abs(Axis.GetLength(x0));
                var kk = (H1 - H0) / Math.Pow((Axis.ArchLength * 0.5), HeightOrder);
                return kk * Math.Pow(v0, HeightOrder) + H0;
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
        public List<Point2D> get_3pt(double x0)
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
            double x_new = Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1, 1e-6);
            Upper = get_3pt(x_new)[0];
            f = (x) => ((get_3pt(x)[2] - Axis.GetCenter(x0)).SignedAngleBetween(-TargetDir));
            x_new = Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1,1e-6);
            Lower = get_3pt(x_new)[2];
        }

        /// <summary>
        /// 正交七点坐标，不对外
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        List<Point2D> get_7pt(double x0)
        {
            List<Point2D> res = new List<Point2D>();
            double HH = GetH(x0);
            Point2D cc = Axis.GetCenter(x0);
            Vector2D dir = Vector2D.XAxis.Rotate(Axis.GetAngle(x0) + Angle.FromDegrees(90.0));

            double H_UP = 0, H_LOW = 0;
            var prop = PropertyTable.FindLast((pp) => pp.CheckProperty(x0, MemberType.UpperCoord));

            if (prop != null)
            {
                H_UP = prop.Section.Diameter;
            }
            prop = PropertyTable.FindLast((pp) => pp.CheckProperty(x0, MemberType.LowerCoord));
            if (prop != null)
            {
                H_LOW = prop.Section.Diameter;
            }

            res.Add(cc + 0.5 * (HH + H_UP) * dir);
            res.Add(cc + 0.5 * HH * dir);
            res.Add(cc + 0.5 * (HH - H_UP) * dir);

            res.Add(cc);

            res.Add(cc - 0.5 * HH * dir + 0.5 * H_LOW * dir);
            res.Add(cc - 0.5 * HH * dir);
            res.Add(cc - 0.5 * HH * dir - 0.5 * H_LOW * dir);

            return res;
        }


        /// <summary>
        /// 任意角度求七点坐标
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="ang_deg"></param>
        /// <returns></returns>
        public List<Point2D> Get7Point(double x0, double ang_deg)
        {
            List<Point2D> res = new List<Point2D>();

            Angle CutAngle = Angle.FromDegrees(ang_deg);

            Vector2D TargetDir = Vector2D.XAxis.Rotate(CutAngle);

            Func<double, double> f;

            double x_new;

            Point2D CC = Axis.GetCenter(x0);

            for (int i = 0; i < 7; i++)
            {
                int dir = i < 3 ? 1 : -1;
                f = (x) => ((get_7pt(x)[i] - CC).SignedAngleBetween(dir * TargetDir));

                if (i == 3)
                {
                    x_new = x0;
                }
                else
                {
                    x_new = Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1, 1e-6);
                }
                res.Add(get_7pt(x_new)[i]);
            }
            return res;
        }


        public CutSection get_diagnal_sectin(double x_from, double x_to, double e, double dia)
        {
            double x0, x1;
            if (x_to < 0)
            {
                x0 = x_from;
                x1 = x_to;

            }
            else
            {
                x0 = -x_to;
                x1 = -x_from;
            }
            Func<double, double> f;
            var P1 = Get7Point(x0, 90)[2];
            var P2 = Get7Point(x1, 90)[4];

            f = (t) => Get7Point(t, 90)[2].DistanceTo(P1) * Math.Sign(t - P1.X) - e;
            var Pa = Get7Point(Bisection.FindRoot(f, x0, x1,1e-5), 90)[2];

            f = (t) => Get7Point(t, 90)[4].DistanceTo(P2) * Math.Sign(t - P2.X) + e;
            var Pb = Get7Point(Bisection.FindRoot(f, x0, x1, 1e-5), 90)[4];

            f=(t)=> Intersect((new Line2D(Pb, Get7Point(t, 90.0)[2]).Offset(dia)))[2].DistanceTo(Pa)
                * Math.Sign(Pa.X - Intersect(new Line2D(Pb, Get7Point(t, 90.0)[2]).Offset(dia))[2].X);
            var Pa1 = Get7Point(Bisection.FindRoot(f, x0, x1, 1e-5), 90)[2];

            Line2D CL;

            if (x_from < 0)
            {
                CL = new Line2D(Pb, Pa1).Offset(0.5 * dia);
            }
            else
            {
                CL = new Line2D(new Point2D(-Pb.X, Pb.Y), new Point2D(-Pa1.X, Pa1.Y)).Offset(0.5 * -dia) ;
            }

            CutSection ret = new CutSection(0, Axis.Intersect(CL), Vector2D.XAxis.AngleTo(CL.Direction));


            return ret;
        }

        private List<Point2D> Intersect(Line2D line2D)
        {
            Point2D CC = Axis.Intersect(line2D);
            return Get7Point(CC.X, Vector2D.XAxis.SignedAngleTo(line2D.Direction).Degrees);
        }
    }
}
