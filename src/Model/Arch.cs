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
using System.IO;

namespace Model
{



    public class Arch
    {
        public ArchAxis Axis;
        public double H0, H1;
        public List<DatumPlane> MainDatum, SecondaryDatum, DiagonalDatum;
        protected List<MemberPropertyRecord> PropertyTable;
        public delegate double get_z(double x0);

        double HeightOrder;


        public Arch(ArchAxis ax, double height0, double height1, double order = 2)
        {
            Axis = ax;
            H0 = height0;
            H1 = height1;
            MainDatum = new List<DatumPlane>();
            SecondaryDatum = new List<DatumPlane>();
            DiagonalDatum = new List<DatumPlane>();
            PropertyTable = new List<MemberPropertyRecord>();
            HeightOrder = order;
        }

        public void AssignProperty(eMemberType MT, TubeSection sect, double from = double.NegativeInfinity, double to = double.PositiveInfinity)
        {
            int idx = PropertyTable.Count;
            PropertyTable.Add(new MemberPropertyRecord(idx, sect, MT, from, to));
        }


        public void WriteDiagonalDatum(string filepath)
        {

        }

        public void WriteControlPoint(string filepath)
        {
            var alldatum = (MainDatum.Concat(SecondaryDatum)).ToList();
            alldatum.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));
            Point2D v1, v2, v3;
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (DatumPlane item in alldatum)
                {
                    double x = item.Center.X;
                    var pts=Get3Point(item.Center.X, item.Angle0.Degrees);
                    v1 = pts[0];
                    v2 = pts[1];
                    v3 = pts[2];
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}", 
                        v1.X.ToString("F5").PadLeft(12), v1.Y.ToString("F5").PadLeft(12), v2.X.ToString("F5").PadLeft(12), 
                        v2.Y.ToString("F5").PadLeft(12), v3.X.ToString("F5").PadLeft(12), v3.Y.ToString("F5").PadLeft(12)));
                }

            }
        }

        /// <summary>
        /// 生成中值基准面 
        /// </summary>
        public void GenerateMiddleDatum()
        {
            var damlist = (from DatumPlane p in MainDatum
                           where p.DatumType == eDatumType.ColumnDatum || p.DatumType == eDatumType.VerticalDatum
                           select p).ToList();
            damlist.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));

            for (int i = 0; i < damlist.Count-1; i++)
            {
                DatumPlane P1 = damlist[i];
                DatumPlane P2 = damlist[i + 1];
                double x = (P1.Center.X + P2.Center.X) * 0.5;
                SecondaryDatum.Add(new DatumPlane(0, Axis.GetCenter(x), Angle.FromDegrees(90), eDatumType.MiddleDatum));
            }
        }

        /// <summary>
        /// 生成斜腹杆基准面
        /// </summary>
        public void GenerateDiagonalDatum()
        {
            var damlist = (from DatumPlane p in MainDatum
                           where p.DatumType == eDatumType.ColumnDatum || p.DatumType == eDatumType.VerticalDatum
                           select p).ToList();
            damlist.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));

            double up_dia = GetTubeProperty(0, eMemberType.UpperCoord).Section.Diameter ;
            double down_dia = GetTubeProperty(0, eMemberType.LowerCoord).Section.Diameter;
            for (int i = 0; i < damlist.Count - 1; i++)
            {
     
                DatumPlane P1 = damlist[i];
                DatumPlane P2 = damlist[i + 1];
                double x = (P1.Center.X + P2.Center.X) * 0.5;
                DatumPlane Pm = new DatumPlane(0, Axis.GetCenter(x), Angle.FromDegrees(90), eDatumType.MiddleDatum);

                Line2D L_UP_1 = new Line2D(Get3Point(P1.Center.X, 90.0)[0], Get3Point(Pm.Center.X, 90)[0]);
                Line2D L_UP_2 = new Line2D(Get3Point(Pm.Center.X, 90)[0], Get3Point(P2.Center.X, 90.0)[0]);

                Line2D L_DOWN_1 = new Line2D(Get3Point(P1.Center.X, 90)[2], Get3Point(Pm.Center.X, 90)[2]);
                Line2D L_DOWN_2 = new Line2D(Get3Point(Pm.Center.X, 90)[2], Get3Point(P2.Center.X, 90)[2]);

                Line2D L1 = P1.Line.Offset(-0.5 * GetTubeProperty(P1.Center.X, eMemberType.VerticalWeb).Section.Diameter);
                Line2D L2 = P2.Line.Offset(0.5 * GetTubeProperty(P2.Center.X, eMemberType.VerticalWeb).Section.Diameter);

                Circle2D C_UP, C_DOWN;

                double dia = GetTubeProperty(Pm.Center.X, eMemberType.InclineWeb).Section.Diameter;

                if (P2.Center.X < 0)
                {
                   
                    C_UP = new Circle2D((Point2D)(L1.IntersectWith(L_UP_1.Offset(-0.5 * up_dia))) + L_UP_1.Direction * 0.080,  dia);
                    C_DOWN = new Circle2D((Point2D)(L2.IntersectWith(L_DOWN_2.Offset(0.5*down_dia)))- L_DOWN_2.Direction* 0.080, dia);

                }
                else
                {
                    C_UP = new Circle2D((Point2D)(L2.IntersectWith(L_UP_2.Offset(0.5*down_dia)))- L_UP_2.Direction*0.080, dia);
                    C_DOWN = new Circle2D((Point2D)(L1.IntersectWith(L_DOWN_1.Offset(-0.5*up_dia)))+ L_DOWN_1.Direction*0.080, dia);
                }

                

                var B = C_UP.Center.Tangent(C_DOWN)[0];

                Line2D datumLine = (new Line2D( B, C_UP.Center)).Offset(-0.5*dia);


                DiagonalDatum.Add(new DatumPlane(0, Axis.Intersect(datumLine), Vector2D.XAxis.AngleTo(datumLine.Direction), eDatumType.DiagonalDatum));
                    
                

            }








        }

        private MemberPropertyRecord GetTubeProperty(double x, eMemberType member)
        {
            var prop = PropertyTable.FindLast((pp) => pp.CheckProperty(x, member));
            return prop;
        }

        /// <summary>
        /// 定义基准平面，无角度时为正交平面
        /// </summary>
        /// <param name="sectid"></param>
        /// <param name="location"></param>
        /// <param name="ST"></param>
        /// <param name="angle_deg"></param>
        public void AddDatum(int sectid, double location,  eDatumType ST,double angle_deg=double.NaN)
        {
 
            if (double.IsNaN(angle_deg))
            {
                angle_deg = Axis.GetAngle(location).Degrees+90.0;
            }
            Point2D cc = Axis.GetCenter(location);
            if (ST==eDatumType.MiddleDatum)
            {
                var datum = new DatumPlane(0, cc, Angle.FromDegrees(angle_deg), ST);
                SecondaryDatum.Add(datum);
            }
            else
            {
                var datum = new DatumPlane(0, cc, Angle.FromDegrees(angle_deg), ST);
                MainDatum.Add(datum);

            }
            
            return ;
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
        public List<Point2D> Get3Point(double x0, double ang_deg)
        {
            Point2D Upper, CC, Lower;
            CC = Axis.GetCenter(x0);
            Angle CutAngle = Angle.FromDegrees(ang_deg);
            Vector2D TargetDir = Vector2D.XAxis.Rotate(CutAngle);
            Func<double, double> f = (x) => ((get_3pt(x)[0] - Axis.GetCenter(x0)).SignedAngleBetween(TargetDir));
            double x_new = Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1, 1e-6);
            Upper = get_3pt(x_new)[0];
            f = (x) => ((get_3pt(x)[2] - Axis.GetCenter(x0)).SignedAngleBetween(-TargetDir));
            x_new = Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1,1e-6);
            Lower = get_3pt(x_new)[2];

            return new List<Point2D>() { Upper, CC, Lower };
        }


        public List<Point2D>GetReal3Point(double x0)
        {
            double y0 = Axis.GetZ(x0);
            var xlist = (from a in MainDatum select a.Center.X).ToList();
            if (xlist.Contains(x0))
            {
                return Get3Point(x0,90);
            }
            else
            {
                var x_1 = xlist.Find((x) => x > x0);
                var x_2 = xlist.FindLast((x) => x < x0);

                var y_1 = Get3Point(x_1, 90)[0].Y;
                var y_2 = Get3Point(x_2, 90)[0].Y;

                double y_up= Extension.Interplate(x_1, x_2, y_1, y_2, x0);


                y_1 = Get3Point(x_1, 90)[2].Y;
                y_2 = Get3Point(x_2, 90)[2].Y;
                double y_down = Extension.Interplate(x_1, x_2, y_1, y_2, x0);

                return new List<Point2D>() { new Point2D(x0, y_up), new Point2D(x0, y0), new Point2D(x0, y_down) };
            }
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
            var prop = PropertyTable.FindLast((pp) => pp.CheckProperty(x0, eMemberType.UpperCoord));

            if (prop != null)
            {
                H_UP = prop.Section.Diameter;
            }
            prop = PropertyTable.FindLast((pp) => pp.CheckProperty(x0, eMemberType.LowerCoord));
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


        public DatumPlane get_diagnal_sectin(double x_from, double x_to, double e, double dia)
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

            DatumPlane ret = new DatumPlane(0, Axis.Intersect(CL), Vector2D.XAxis.AngleTo(CL.Direction),eDatumType.DiagonalDatum);


            return ret;
        }

        private List<Point2D> Intersect(Line2D line2D)
        {
            Point2D CC = Axis.Intersect(line2D);
            return Get7Point(CC.X, Vector2D.XAxis.SignedAngleTo(line2D.Direction).Degrees);
        }
    }
}
