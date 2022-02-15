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
using System.Data;
using System.Diagnostics;

namespace Model
{

    /// <summary>
    /// 主拱2.5D
    /// </summary>
    public class Arch
    {
        public ArchAxis Axis;
        public double H0, H1;
        public double WidthInside, WidthOutside;
        public List<DatumPlane> MainDatum, SecondaryDatum, DiagonalDatum, InstallDatum;

        public List<InstallationSegment> InstPartList;

        public List<MemberPropertyRecord> PropertyTable;
        public delegate double get_z(double x0);
        public List<Point2D> UpSkeleton, LowSkeleton, UpUpSkeleton, UpLowSkeleton, LowUpSkeleton, LowLowSkeleton;
        List<Node2D> NodeTable;
        public List<Column> ColumnList;
        public List<RCColumn> RCColumnList;
        public List<Member> MemberTable;
        public double ElevationOfTop;
        public DataTable ColumnTable;
        double HeightOrder;

        double footLevel;


        /// <summary>
        /// 基本拱模型
        /// </summary>
        /// <param name="ax"></param>
        /// <param name="height0">跨中桁高</param>
        /// <param name="height1">拱脚桁高</param>
        /// <param name="width0">内侧桁架中距</param>
        /// <param name="width1">外侧桁架中距</param>
        /// <param name="order"></param>
        public Arch(ArchAxis ax, double height0, double height1, double width0, double width1, double order = 2, double elevationOfTop = 0)
        {
            Axis = ax;
            H0 = height0;
            H1 = height1;
            WidthInside = width0;
            WidthOutside = width1;
            MainDatum = new List<DatumPlane>();
            ColumnList = new List<Column>();
            RCColumnList = new List<RCColumn>();
            SecondaryDatum = new List<DatumPlane>();
            DiagonalDatum = new List<DatumPlane>();
            InstPartList = new List<InstallationSegment>();
            PropertyTable = new List<MemberPropertyRecord>();

            InstallDatum = new List<DatumPlane>() {
                new DatumPlane(0,new Point2D( -Axis.L1,Axis.GetZ(-Axis.L1)),Axis.GetNormalAngle(-Axis.L1),eDatumType.InstallDatum),
                new DatumPlane(2,new Point2D( Axis.L1,Axis.GetZ(Axis.L1)),Axis.GetNormalAngle(Axis.L1),eDatumType.InstallDatum),
                };
            HeightOrder = order;
            ElevationOfTop = elevationOfTop;
            footLevel = elevationOfTop - ax.f;
        }





        #region 属性
        public double WebTubeDiameter
        {
            get
            {
                return GetTubeProperty(0, eMemberType.SubWeb).Section.Diameter;
            }
        }
        public double MainTubeDiameter
        {
            get
            {
                return GetTubeProperty(0, eMemberType.UpperCoord).Section.Diameter;

            }
        }
        public double CrossBracingDiameter
        {
            get
            {
                return GetTubeProperty(0, eMemberType.CrossCoord).Section.Diameter;

            }
        }
        public double FootLevel
        {
            get
            {
                return footLevel;
            }
        }
        public double Width
        {
            get
            {
                return 2 * WidthOutside + WidthInside;
            }
        }

        public Point2D[] LeftFoot
        {
            get
            {
                var ret = get_3pt(-1.0 * Axis.L1).ToArray();
                return ret;
            }
        }
        public Point2D[] MidPoints
        {
            get
            {
                var ret = get_3pt(0).ToArray();
                return ret;
            }
        }
        #endregion


        #region 基本建模方法
        /// <summary>
        /// 指定截面
        /// </summary>
        /// <param name="MT"></param>
        /// <param name="sect"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AssignProperty(eMemberType MT, Section sect, double from = double.NegativeInfinity, double to = double.PositiveInfinity)
        {
            int idx = PropertyTable.Count;
            PropertyTable.Add(new MemberPropertyRecord(idx, sect, MT, from, to));
        }

        /// <summary>
        /// 生成通用安装节段
        /// </summary>
        /// <param name="curI"></param>
        /// <param name="nexI"></param>
        /// <param name="vs1"></param>
        /// <param name="vs2"></param>
        /// <param name="vs3"></param>
        public void CreateInstallSegment(DatumPlane curI, DatumPlane nexI, 
            double[] dist, double[] ang_degList,
            bool[] is_dia, double e)
        {
            //首先判断参数合理性
            double La = nexI.Center.X - curI.Center.X;
            Debug.Assert(dist.Sum() == La);
            Debug.Assert(dist.Length == ang_degList.Length + 1);
            Debug.Assert(is_dia.Length == dist.Length);

            List<eDatumType> Dtlist = new List<eDatumType>();
            //List<double> ang_degList = new List<double>();


            for (int i = 0; i < dist.Count() - 1; i++)
            {
                if (i == 0)
                {
                    if (Math.Abs(dist[i]) <= 2)
                    {
                        Dtlist.Add(eDatumType.DoubleDatum);
                    }
                    else
                    {
                        if (ang_degList[i] == 90)
                        {
                            Dtlist.Add(eDatumType.VerticalDatum);
                        }
                        else
                        {
                            Dtlist.Add(eDatumType.NormalDatum);
                        }
                    }
                }
                else if (i == ang_degList.Length - 1)
                {
                    if (dist.Last() <= 2)
                    {
                        Dtlist.Add(eDatumType.DoubleDatum);
                    }
                    else
                    {
                        if (ang_degList[i] == 90)
                        {
                            Dtlist.Add(eDatumType.VerticalDatum);
                        }
                        else
                        {
                            Dtlist.Add(eDatumType.NormalDatum);
                        }
                    }
                }
                else
                {
                    if (ang_degList[i] == 90)
                    {
                        Dtlist.Add(eDatumType.VerticalDatum);
                    }
                    else
                    {
                        Dtlist.Add(eDatumType.NormalDatum);
                    }
                }
                //double xi = curI.Center.X + dist.ToList().GetRange(0, i + 1).Sum();

            }
            List<DatumPlane> newMainDatum = new List<DatumPlane>();
            List<DatumPlane> newDiaDatum = new List<DatumPlane>();
            // 判断斜腹杆方向
            bool GreaterThanZero = curI.Center.X > 0;
            bool AnyNorm = curI.Angle0.Degrees != 90.0 || nexI.Angle0.Degrees != 90;
            bool DiaDirect = true;
            if (AnyNorm)
            {
                if (GreaterThanZero)
                {
                    DiaDirect = true;
                }
                else
                {
                    DiaDirect = false;
                }
            }
            else
            {
                if (GreaterThanZero)
                {
                    DiaDirect = false;
                }
                else
                {
                    DiaDirect = true;
                }
            }
            // 判断斜腹杆方向
            for (int i = 0; i < dist.Length - 1; i++)
            {
                double xi = curI.Center.X + dist.ToList().GetRange(0, i + 1).Sum();
                Point2D cc = new Point2D(xi, Axis.GetZ(xi));
                double ang = ang_degList[i];
                newMainDatum.Add(new DatumPlane(0, cc, Angle.FromDegrees(ang), Dtlist[i]));
                if (i != 0)
                {
                    if (is_dia[i])
                    {
                        var dia = CreatDiagonalDatum(newMainDatum[i - 1], newMainDatum[i],
                            
                            e, DiaDirect);
                        var a = (newMainDatum[i - 1].Angle0.Degrees + newMainDatum[i].Angle0.Degrees) * 0.5;
                        SecondaryDatum.Add(new DatumPlane(0, dia.Center, Angle.FromDegrees(a), eDatumType.MiddleDatum));
                        newDiaDatum.Add(dia);
                    }
                }
            }

            foreach (var item in newMainDatum)
            {
                MainDatum.Add(item);
            }
            foreach (var item in newDiaDatum)
            {
                DiagonalDatum.Add(item);
            }
            MainDatum.MySort();
            DiagonalDatum.MySort();
            SecondaryDatum.MySort();
        }


        public void AddInstSeg(int id, double x_start, double x_end, ref InstallationSegment refPart, bool reWrite = false)
        {

            InstallationSegment newInst = refPart.Clone() as InstallationSegment;

            if (InstPartList.Find(x => x.Id == id) != null)
            {
                if (reWrite)
                {
                    var idx = InstPartList.IndexOf(InstPartList.Find(x => x.Id == id));
                    InstPartList.RemoveAt(idx);
                }
                else
                {
                    throw new Exception(string.Format("编号为{0}的吊装节段已定义.", id));
                }
            }
            newInst.Id = id;
            newInst.X_Start = x_start;
            newInst.X_End = x_end;
            newInst.theArch = this;
            InstPartList.Add(newInst);
        }


        /// <summary>
        /// 定义基准平面，无角度时为正交平面
        /// </summary>
        /// <param name="sectid"></param>
        /// <param name="location"></param>
        /// <param name="ST"></param>
        /// <param name="angle_deg"></param>
        public void AddDatum(int sectid, double location, eDatumType ST, double angle_deg = double.NaN)
        {

            if (double.IsNaN(angle_deg))
            {
                angle_deg = Axis.GetAngle(location).Degrees + 90.0;
            }
            Point2D cc = Axis.GetCenter(location);

            if (ST == eDatumType.InstallDatum)
            {
                var datum = new DatumPlane(0, cc, Angle.FromDegrees(angle_deg), ST);
                InstallDatum.Add(datum);
                InstallDatum.MySort();
            }
            else if (ST == eDatumType.MiddleDatum)
            {
                var datum = new DatumPlane(0, cc, Angle.FromDegrees(angle_deg), ST);
                SecondaryDatum.Add(datum);
                SecondaryDatum.MySort();
            }
            else
            {
                var datum = new DatumPlane(0, cc, Angle.FromDegrees(angle_deg), ST);
                MainDatum.Add(datum);
                MainDatum.MySort();

            }

            return;
        }


        public DatumPlane GetMainDatum(int id)
        {
            return MainDatum.Find(x => x.ID == id);
        }

        public DatumPlane GetDatum(double x, eDatumType theType, double tor = 1e-5)
        {
            Point2D pt = new Point2D(x, Axis.GetZ(x));
            foreach (var item in MainDatum)
            {
                if (item.Center.DistanceTo(pt) <= tor)
                {
                    if (item.DatumType == theType)
                    {
                        return item;
                    }

                }
            }
            foreach (var item in SecondaryDatum)
            {
                if (item.Center.DistanceTo(pt) <= tor)
                {
                    if (item.DatumType == theType)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 增加墩柱
        /// </summary>
        /// <param name="colid"></param>
        /// <param name="x0">位置</param>
        /// <param name="relativeH">相对拱脚高差</param>
        /// <param name="columnL">顺桥向柱间距</param>
        /// <param name="capH">盖梁节段高</param>
        /// <param name="stepL">步长</param>
        /// <param name="numStep">节段步数</param>
        /// <param name="offset">节段偏移绝对值</param>
        /// <param name="footW">柱脚横桥向宽度</param>
        /// <param name="footL">柱脚顺桥向长度</param>
        /// <param name="footMinH">柱脚最小高度</param>
        public void AddColumn(int colid, double x0, double relativeH, double columnL, double capH, double stepL, int numStep, double offset,
            double footW, double footL, double footMinH = 0.5)
        {
            Column theCol = new Column(this, x0, relativeH, colid, columnL, capH, stepL, numStep, offset, footW, footL, footMinH);
            ColumnList.Add(theCol);
            if (colid == 0)
            {
                ColumnList.MySort();
            }

            // ColumnList.Sort(new Column());
        }

        /// <summary>
        /// 增加交界墩
        /// </summary>
        /// <param name="colid"></param>
        /// <param name="x0"></param>
        /// <param name="colInst"></param>
        public void AddColumn(int colid, double x0, RCColumn colInst)
        {
            colInst.X = x0;
            colInst.MainArch = this;
            colInst.ID = colid;
            RCColumnList.Add(colInst);
            RCColumnList.Sort(new RCColumn());
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

            for (int i = 0; i < damlist.Count - 1; i++)
            {
                DatumPlane P1 = damlist[i];
                DatumPlane P2 = damlist[i + 1];
                double x = (P1.Center.X + P2.Center.X) * 0.5;
                SecondaryDatum.Add(new DatumPlane(0, Axis.GetCenter(x), Angle.FromDegrees(90), eDatumType.MiddleDatum));
            }
        }

        /// <summary>
        /// 设置拱脚高程
        /// </summary>
        /// <param name="val"></param>
        public void SetFootLevel(double val)
        {
            footLevel = val;
            ElevationOfTop = val + Axis.f;
        }

        /// <summary>
        /// 根据竖腹杆生成相邻法向腹杆
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="isLeft"></param>
        /// <param name="isUp"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public DatumPlane CreatNormalDatumByVertical(DatumPlane P1, bool isLeft, bool isUp, double e)
        {
            //double up_dia = GetTubeProperty(0, eMemberType.UpperCoord).Section.Diameter;
            //double down_dia = GetTubeProperty(0, eMemberType.LowerCoord).Section.Diameter;
            double norm_dia = GetTubeProperty(0, eMemberType.MainWeb).Section.Diameter;
            Point2D KP,KP2;
            Vector2D Direction;
            if (isLeft)
            {
                if (isUp)
                {
                    // 左上
                    KP= Get7Point(P1.Center.X - norm_dia * 0.5, 90.0)[2];
                    KP2 = Get7Point(P1.Center.X - norm_dia * 0.5-0.01, 90.0)[2];
                }
                else
                {
                    // 左下
                    KP = Get7Point(P1.Center.X - norm_dia * 0.5, 90.0)[4];
                    KP2 = Get7Point(P1.Center.X - norm_dia * 0.5-0.01, 90.0)[4];

                }
            }
            else
            {
                if (isUp)
                {
                    // 右上
                    KP = Get7Point(P1.Center.X + norm_dia * 0.5, 90.0)[2];
                    KP2 = Get7Point(P1.Center.X + norm_dia * 0.5+0.01, 90.0)[2];
                }
                else
                {
                    // 右下
                    KP = Get7Point(P1.Center.X + norm_dia * 0.5, 90.0)[4];
                    KP2 = Get7Point(P1.Center.X + norm_dia * 0.5+0.01, 90.0)[4];
                }
            }
            Direction = (KP2 - KP).Normalize();

            Point2D CC = KP + Direction * (e + 0.5 * norm_dia) * 1.05;

            Point2D AxCC = Axis.Intersect(CC);
            
            return new DatumPlane(0,AxCC, Axis.GetNormalAngle(AxCC.X),eDatumType.NormalDatum);

        }

        /// <summary>
        /// 控制面类型与单元类型映射
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public eMemberType MemberTypeFromDamtumType(eDatumType dt)
        {
            eMemberType mt;
            switch (dt)
            {
                case eDatumType.VerticalDatum: // 竖直主控面
                    mt = eMemberType.MainWeb;
                    break;
                case eDatumType.NormalDatum:  // 法向主控面
                    mt = eMemberType.MainWeb;
                    break;
                case eDatumType.DoubleDatum:  // 安装主控面
                    mt = eMemberType.InstallWeb;
                    break;
                default:
                    throw new Exception("不应该有这种。。。");
            }

            return mt;

        }

        /// <summary>
        /// 生成斜腹杆平面
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public DatumPlane CreatDiagonalDatum(DatumPlane P1, DatumPlane P2, double e, bool isLUtoRL, DatumPlane Pmid = null)
        {
            double up_dia = GetTubeProperty(0, eMemberType.UpperCoord).Section.Diameter;
            double down_dia = GetTubeProperty(0, eMemberType.LowerCoord).Section.Diameter;
            double x = (P1.Center.X + P2.Center.X) * 0.5;
            double a = (P1.Angle0.Degrees + P2.Angle0.Degrees) * 0.5;
            DatumPlane Pm = Pmid == null ? new DatumPlane(0, Axis.GetCenter(x), Angle.FromDegrees(a), eDatumType.MiddleDatum) : Pmid;

            Line2D L_UP_1 = new Line2D(Get3Point(P1.Center.X, P1.Angle0.Degrees)[0], Get3Point(Pm.Center.X, Pm.Angle0.Degrees)[0]);
            Line2D L_UP_2 = new Line2D(Get3Point(Pm.Center.X, Pm.Angle0.Degrees)[0], Get3Point(P2.Center.X, P2.Angle0.Degrees)[0]);

            Line2D L_DOWN_1 = new Line2D(Get3Point(P1.Center.X, P1.Angle0.Degrees)[2], Get3Point(Pm.Center.X, Pm.Angle0.Degrees)[2]);
            Line2D L_DOWN_2 = new Line2D(Get3Point(Pm.Center.X, Pm.Angle0.Degrees)[2], Get3Point(P2.Center.X, P2.Angle0.Degrees)[2]);

            eMemberType TP1 = MemberTypeFromDamtumType(P1.DatumType);
            eMemberType TP2= MemberTypeFromDamtumType(P2.DatumType);
            Line2D L1 = P1.Line.Offset(-0.5 * GetTubeProperty(P1.Center.X, TP1).Section.Diameter);
            Line2D L2 = P2.Line.Offset(0.5 * GetTubeProperty(P2.Center.X, TP2).Section.Diameter);

            Circle2D C_UP, C_DOWN;

            double dia = GetTubeProperty(Pm.Center.X, eMemberType.SubWeb).Section.Diameter;

            if (isLUtoRL)
            {
                // 左上到右下

                C_UP = new Circle2D((Point2D)(L1.IntersectWith(L_UP_1.Offset(-0.5 * up_dia))) + L_UP_1.Direction * e, dia);
                C_DOWN = new Circle2D((Point2D)(L2.IntersectWith(L_DOWN_2.Offset(0.5 * down_dia))) - L_DOWN_2.Direction * e, dia);

            }
            else 
            {
                // 左下到右上
                C_UP = new Circle2D((Point2D)(L2.IntersectWith(L_UP_2.Offset(-0.5 * down_dia))) - L_UP_2.Direction * e, dia);
                C_DOWN = new Circle2D((Point2D)(L1.IntersectWith(L_DOWN_1.Offset(0.5 * up_dia))) + L_DOWN_1.Direction * e, dia);
            }
            //else
            //{
            //    throw new Exception();
            //}
            var TangentedPoints = C_UP.Center.Tangent(C_DOWN);
            TangentedPoints.Sort((pa, pb) => pa.X.CompareTo(pb.X));
            Point2D B=new Point2D();
            if (isLUtoRL)
            {
                B = TangentedPoints[0];
            }
            else
            {
                B = TangentedPoints[1];
            }

            int offsetDir = isLUtoRL ? -1 : 1;

            Line2D datumLine = (new Line2D(B, C_UP.Center)).Offset(offsetDir * 0.5 * dia);
            return new DatumPlane(0, Axis.Intersect(datumLine), Vector2D.XAxis.AngleTo(datumLine.Direction), eDatumType.DiagonalDatum);
        }

        /// <summary>
        /// 生成斜腹杆基准面
        /// </summary>
        public void GenerateDiagonalDatum(double e)
        {
            var damlist = (from DatumPlane p in MainDatum
                           where p.DatumType == eDatumType.ColumnDatum || p.DatumType == eDatumType.VerticalDatum
                           select p).ToList();
            damlist.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));

            double up_dia = GetTubeProperty(0, eMemberType.UpperCoord).Section.Diameter;
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

                Line2D L1 = P1.Line.Offset(-0.5 * GetTubeProperty(P1.Center.X, eMemberType.MainWeb).Section.Diameter);
                Line2D L2 = P2.Line.Offset(0.5 * GetTubeProperty(P2.Center.X, eMemberType.MainWeb).Section.Diameter);

                Circle2D C_UP, C_DOWN;

                double dia = GetTubeProperty(Pm.Center.X, eMemberType.SubWeb).Section.Diameter;

                if (P2.Center.X < 0)
                {

                    C_UP = new Circle2D((Point2D)(L1.IntersectWith(L_UP_1.Offset(-0.5 * up_dia))) + L_UP_1.Direction * e, dia);
                    C_DOWN = new Circle2D((Point2D)(L2.IntersectWith(L_DOWN_2.Offset(0.5 * down_dia))) - L_DOWN_2.Direction * e, dia);

                }
                else if (P1.Center.X > 0)
                {
                    C_UP = new Circle2D((Point2D)(L2.IntersectWith(L_UP_2.Offset(-0.5 * down_dia))) - L_UP_2.Direction * e, dia);
                    C_DOWN = new Circle2D((Point2D)(L1.IntersectWith(L_DOWN_1.Offset(0.5 * up_dia))) + L_DOWN_1.Direction * e, dia);
                }
                else
                {
                    continue;
                }
                var B = C_UP.Center.Tangent(C_DOWN)[0];
                Line2D datumLine = (new Line2D(B, C_UP.Center)).Offset(Math.Sign(P1.Center.X) * 0.5 * dia);
                DiagonalDatum.Add(new DatumPlane(0, Axis.Intersect(datumLine), Vector2D.XAxis.AngleTo(datumLine.Direction), eDatumType.DiagonalDatum));
            }
        }

        /// <summary>
        /// 生成拱圈模型
        /// </summary>
        public void GenerateArch()
        {
            MainDatum.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));
            SecondaryDatum.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));
            MemberTable = new List<Member>();
            for (int i = 0; i < UpSkeleton.Count - 1; i++)
            {
                Line2D line = new Line2D(UpSkeleton[i], UpSkeleton[i + 1]);
                MemberTable.Add(new Member(0, line, GetTubeProperty(line.MiddlePoint().X, eMemberType.UpperCoord).Section, eMemberType.UpperCoord));
            }
            for (int i = 0; i < LowSkeleton.Count - 1; i++)
            {
                Line2D line = new Line2D(LowSkeleton[i], LowSkeleton[i + 1]);
                MemberTable.Add(new Member(0, line, GetTubeProperty(line.MiddlePoint().X, eMemberType.LowerCoord).Section, eMemberType.LowerCoord));
            }
            foreach (var item in DiagonalDatum)
            {
                var ret = Get3PointReal(item);
                Line2D line = new Line2D(ret[0], ret[2]);
                MemberTable.Add(new Member(0, line, GetTubeProperty(line.MiddlePoint().X, eMemberType.SubWeb).Section, eMemberType.SubWeb));
            }
            foreach (var item in MainDatum)
            {
                var dt = item.DatumType;
                eMemberType mt = 0;
                switch (dt)
                {

                    case eDatumType.VerticalDatum: // 竖直主控面
                        mt = eMemberType.MainWeb;
                        break;
                    case eDatumType.NormalDatum:  // 法向主控面
                        mt = eMemberType.MainWeb;
                        break;
                    case eDatumType.DoubleDatum:  // 安装主控面
                        mt = eMemberType.InstallWeb;
                        break;
                    case eDatumType.ControlDatum:
                        continue;
                    case eDatumType.MiddleDatum:
                        throw new Exception("不应该有这种。。。");
                    case eDatumType.DiagonalDatum:
                        throw new Exception("不应该有这种。。。");
                    case eDatumType.ColumnDatum:
                        throw new Exception("不应该有这种。。。");
                    default:
                        throw new Exception("不应该有这种。。。");
                }
                var ret = Get3PointReal(item);
                Line2D line = new Line2D(ret[0], ret[2]);
                MemberTable.Add(new Member(0, line, GetTubeProperty(line.MiddlePoint().X, mt).Section, mt));
            }
        }

        /// <summary>
        /// 生成骨架线节点
        /// </summary>
        public void GenerateSkeleton()
        {
            UpSkeleton = new List<Point2D>();
            LowSkeleton = new List<Point2D>();
            UpUpSkeleton = new List<Point2D>();
            UpLowSkeleton = new List<Point2D>();
            LowUpSkeleton = new List<Point2D>();
            LowLowSkeleton = new List<Point2D>();


            foreach (var item in MainDatum)
            {
                var ff = Get3Point(item);
                UpSkeleton.Add(ff[0]);
                LowSkeleton.Add(ff[2]);
            }
            foreach (var item in SecondaryDatum)
            {
                var ff = Get3Point(item);
                UpSkeleton.Add(ff[0]);
                LowSkeleton.Add(ff[2]);
            }

            UpSkeleton.Sort((x, y) => x.X.CompareTo(y.X));
            LowSkeleton.Sort((x, y) => x.X.CompareTo(y.X));

            for (int i = 0; i < 2; i++)
            {
                var SKL = i == 0 ? UpSkeleton : LowSkeleton;
                for (int j = 0; j < 2; j++)
                {
                    if (i == 0)
                    {
                        UpUpSkeleton = OffsetSkeleton(SKL, MainTubeDiameter * 0.5);
                        UpLowSkeleton = OffsetSkeleton(SKL, MainTubeDiameter * -0.5);
                    }
                    else
                    {
                        LowUpSkeleton = OffsetSkeleton(SKL, MainTubeDiameter * 0.5);
                        LowLowSkeleton = OffsetSkeleton(SKL, MainTubeDiameter * -0.5);
                    }
                }
            }
        }

        /// <summary>
        /// 生成平行式桁架（大小井模式）
        /// </summary>
        /// <param name="numCol">立柱数量（偶数）</param>
        /// <param name="distCol">立柱间距</param>
        /// <param name="distVertical1">竖腹杆间距1：立柱间竖腹杆</param>
        /// <param name="distVertical2">竖腹杆间距2：跨中竖杆间距</param>
        /// <param name="distVertical3">竖腹杆间距3：首尾立柱外侧竖腹杆间距</param>
        /// <param name="CellsSide">首尾立柱外侧竖腹杆数量</param>
        public void GenerateTruss(int numCol, int distCol,
            double distVertical1, double distVertical2, double distVertical3, int CellsSide)
        {

            double x0 = (numCol - 1) * distCol * -0.5;
            for (int i = 0; i < numCol; i++)
            {
                double xi = x0 + i * distCol;
                AddDatum(0, xi, eDatumType.ColumnDatum, 90.0);
                int numBetweenCol = (int)Math.Round(distCol / distVertical1 - 1, MidpointRounding.AwayFromZero);
                int numOfMid = (int)(distCol * 0.5 / distVertical2);
                double rest = distCol - numOfMid * 2 * distVertical2;
                if (i == numCol - 1)
                {
                    for (int j = 0; j < CellsSide; j++)
                    {
                        AddDatum(0, xi + distVertical3 * (j + 1), eDatumType.VerticalDatum, 90);
                    }
                }
                else if (i == numCol / 2 - 1)
                {
                    for (int jj = 0; jj < numOfMid; jj++)
                    {
                        AddDatum(0, xi + distVertical2 * (jj + 1), eDatumType.VerticalDatum, 90);
                        AddDatum(0, rest * 0.5 + distVertical2 * (jj), eDatumType.VerticalDatum, 90);
                    }
                }
                else
                {
                    for (int j = 0; j < numBetweenCol; j++)
                    {
                        AddDatum(0, xi + (j + 1) * distVertical1, eDatumType.VerticalDatum, 90);
                    }
                }
                if (i == 0)
                {
                    for (int j = 0; j < CellsSide; j++)
                    {
                        AddDatum(0, xi - distVertical3 * (j + 1), eDatumType.VerticalDatum, 90);
                    }
                }
            }
        }


        /// <summary>
        /// 生成平行式桁架（基于吊装单元，需提前定义所有吊装基准面）
        /// </summary>
        /// <param name="numCol"></param>
        /// <param name="distCol"></param>
        /// <param name="distVertical1"></param>
        /// <param name="distVertical2"></param>
        /// <param name="distVertical3"></param>
        /// <param name="CellsSide"></param>
        public void GenerateTrussUnit(double installGap, double sideDist, double dist, int numMid,
            double sideDistC, double distC, int numMidC)
        {
            var Inst = (from DatumPlane dt in InstallDatum where dt.Angle0 == Angle.FromDegrees(90.0) select dt).ToList();
            double La = installGap + sideDist * 2 + numMid * dist;
            double Lb = installGap + sideDistC * 2 + numMidC * distC;
            double sd, d;
            int numd;

            for (int i = 0; i < Inst.Count; i++)
            {
                var item = Inst[i];
                Point2D cc = new Point2D(item.Center.X - 0.5 * installGap, Axis.GetZ(item.Center.X - 0.5 * installGap));
                MainDatum.Add(new DatumPlane(0, cc, Angle.FromDegrees(90.0), eDatumType.VerticalDatum));
                cc = new Point2D(item.Center.X + 0.5 * installGap, Axis.GetZ(item.Center.X + 0.5 * installGap));
                MainDatum.Add(new DatumPlane(0, cc, Angle.FromDegrees(90.0), eDatumType.VerticalDatum));

                if (i != 0)
                {
                    var previousItem = Inst[i - 1];

                    if ((item.Center.X - previousItem.Center.X) == La)
                    {
                        sd = sideDist;
                        numd = numMid;
                        d = dist;
                    }
                    else if ((item.Center.X - previousItem.Center.X) == Lb)
                    {
                        sd = sideDistC;
                        numd = numMidC;
                        d = distC;
                    }
                    else
                    {
                        throw new Exception();
                    }
                    double xi = previousItem.Center.X + 0.5 * installGap + sd;
                    for (int j = 0; j < numd + 1; j++)
                    {
                        cc = new Point2D(xi, Axis.GetZ(xi));
                        MainDatum.Add(new DatumPlane(0, cc, Angle.FromDegrees(90.0), eDatumType.VerticalDatum));
                        SecondaryDatum.Add(new DatumPlane(0, new Point2D(xi + 0.5 * d, Axis.GetZ(xi + 0.5 * d)), Angle.FromDegrees(90.0), eDatumType.MiddleDatum));
                        if (j == 0)
                        {
                            SecondaryDatum.Add(new DatumPlane(0, new Point2D(xi - 0.5 * d, Axis.GetZ(xi - 0.5 * d)), Angle.FromDegrees(90.0), eDatumType.MiddleDatum));
                        }
                        xi += d;
                    }
                }// 标准节段
            }

            MainDatum.MySort();
            SecondaryDatum.MySort();
            ;
        }


        /// <summary>
        /// 生成立柱模型
        /// </summary>
        public void GenerateColumn()
        {
            #region 立柱参数表
            ColumnTable = new DataTable("拱上立柱参数及标高表");

            DataColumn column;


            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Name";
            column.Caption = "立柱编号";
            column.Unique = false;
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "L(m)";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "A(m)";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "B(m)";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "C(m)";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "N";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "M";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "K";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "H0";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "H1";
            ColumnTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "H2";
            ColumnTable.Columns.Add(column);

            // Make the ID column the primary key column.
            //DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            //PrimaryKeyColumns[0] = ColumnTable.Columns["Name"];
            //ColumnTable.PrimaryKey = PrimaryKeyColumns;

            #endregion

            DataRow row;
            foreach (var item in ColumnList)
            {
                item.Generate();
                row = ColumnTable.NewRow();
                row["name"] = "LZ-" + (item.ID + 1).ToString().PadLeft(2, '0');
                row["L(m)"] = item.H.ToString("F3");
                row["A(m)"] = item.A.ToString("F3");
                row["B(m)"] = item.B.ToString("F3");
                row["C(m)"] = item.C.ToString("F3");
                row["N"] = item.N.ToString();
                row["M"] = item.M.ToString();
                row["K"] = item.K.ToString();
                row["H0"] = (ElevationOfTop + item.Z0).ToString("F3");
                row["H1"] = (ElevationOfTop + item.Z1).ToString("F3");
                row["H2"] = (ElevationOfTop + item.Z2).ToString("F3");

                ColumnTable.Rows.Add(row);

            }

        }

        public void AddTriWeb(DatumPlane fromDatum, DatumPlane targetDatum, double offset, double half_c_offset = 0)
        {
            double centerX = fromDatum.Center.X;
            Vector2D direction_from = Vector2D.XAxis.Rotate(fromDatum.Angle0);
            double DiaNormal = GetTubeProperty(targetDatum.Center.X, eMemberType.MainWeb).Section.Diameter;
            var sect = GetTubeProperty((centerX + targetDatum.Center.X) * 0.5, eMemberType.TriWeb).Section;
            double dia = sect.Diameter;
            Point2D O0 = Axis.GetCenter(centerX);
            Point2D OA = O0 + half_c_offset * direction_from;
            Point2D OB = O0 - half_c_offset * direction_from;

            int direct = targetDatum.Center.X > centerX ? 1 : -1;
            Line2D bd = targetDatum.Line.Offset(direct * DiaNormal * 0.5);

            Point2D A = Get7PointReal(bd.Datum(ref Axis))[2];
            Point2D B = Get7PointReal(bd.Datum(ref Axis))[4];


            Circle2D CA = new Circle2D(A, offset);
            var inter = UpLowSkeleton.Intersection(CA);
            inter.Sort((x, y) => x.X.CompareTo(y.X));
            Point2D A1 = direct == 1 ? inter[0] : inter[1];

            Circle2D CB = new Circle2D(B, offset);
            inter = LowUpSkeleton.Intersection(CB);
            inter.Sort((x, y) => x.X.CompareTo(y.X));
            Point2D B1 = direct == 1 ? inter[0] : inter[1];


            var TanPtsA = OA.Tangent(new Circle2D(A1, dia * 0.5));
            var TanPtsB = OB.Tangent(new Circle2D(B1, dia * 0.5));

            double Ang00 = (TanPtsA[0] - OA).AngleTo(TanPtsB[0] - OB).Degrees;
            double Ang01 = (TanPtsA[0] - OA).AngleTo(TanPtsB[1] - OB).Degrees;
            double Ang10 = (TanPtsA[1] - OA).AngleTo(TanPtsB[0] - OB).Degrees;
            double Ang11 = (TanPtsA[1] - OA).AngleTo(TanPtsB[1] - OB).Degrees;
            double AngMax = new List<double>() { Ang00, Ang01, Ang10, Ang11, }.Max();
            var A2 = TanPtsA[0];
            var B2 = TanPtsB[0];
            if (Ang01==AngMax)
            {
                A2 = TanPtsA[0];
                B2 = TanPtsB[1];
            }
            else if (Ang10==AngMax)
            {
                A2 = TanPtsA[1];
                B2 = TanPtsB[0];
            }
            else if (Ang11 == AngMax)
            {
                A2 = TanPtsA[1];
                B2 = TanPtsB[1];
            }

            var LinA = new Line2D(OA, A2);
            var LinB = new Line2D(OB, B2);
            double st = 0, ed = 0;
            if (OA.X>0)
            {
                ed = Axis.L1+10;
            }
            else
            {
                st = -Axis.L1 - 10;
            }
            var VA = Axis.Intersect(new Line2D(A2 + LinA.Direction * (- 2* LinA.Length), A2),st,ed);
            var VB = Axis.Intersect(new Line2D(B2 + LinB.Direction * (-2 * LinB.Length), B2),st,ed);

            var A3 = Get3PointReal(VA.X, Vector2D.XAxis.AngleTo(A2 - OA).Degrees)[0];
            var B3 = Get3PointReal(VB.X, Vector2D.XAxis.AngleTo(B2 - OB).Degrees)[2];

            var MemberA = new Member(0, new Line2D(OA, A3), sect, eMemberType.TriWeb);
            MemberA.StartDatum = fromDatum;
            MemberTable.Add(MemberA);
            var MemberB = new Member(0, new Line2D(OB, B3), sect, eMemberType.TriWeb);
            MemberB.StartDatum = fromDatum;
            MemberTable.Add(MemberB);
        }

        #endregion

        #region I/O方法

        public void WriteMainDatum(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (DatumPlane item in MainDatum)
                {

                    sw.WriteLine(item.Lisp);


                }
            }
        }

        public void WriteDiagonalDatum(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (DatumPlane item in DiagonalDatum)
                {
                    sw.WriteLine(item.Lisp);
                }
            }
        }

        public void WriteControlPoint(string filepath)
        {




            var alldatum = (MainDatum.Concat(SecondaryDatum)).ToList();
            alldatum.Sort((x, y) => x.Center.X.CompareTo(y.Center.X));
            Point2D v1, v2, v3;
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (var item in UpSkeleton)
                {
                    sw.WriteLine(string.Format("{0},{1}", item.X.ToString("F5").PadLeft(12), item.Y.ToString("F5").PadLeft(12)));
                }
                foreach (var item in LowSkeleton)
                {
                    sw.WriteLine(string.Format("{0},{1}", item.X.ToString("F5").PadLeft(12), item.Y.ToString("F5").PadLeft(12)));
                }
                double x0 = -Axis.L1;

                while (x0 < Axis.L1 + 1)
                {
                    sw.WriteLine(string.Format("{0},{1}", x0, Axis.GetZ(x0)));
                    x0 += 1;
                }

                //foreach (DatumPlane item in alldatum)
                //{
                //    double x = item.Center.X;
                //    var pts = Get3Point(item.Center.X, item.Angle0.Degrees);
                //    v1 = pts[0];
                //    v2 = pts[1];
                //    v3 = pts[2];
                //    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}",
                //        v1.X.ToString("F5").PadLeft(12), v1.Y.ToString("F5").PadLeft(12), v2.X.ToString("F5").PadLeft(12),
                //        v2.Y.ToString("F5").PadLeft(12), v3.X.ToString("F5").PadLeft(12), v3.Y.ToString("F5").PadLeft(12)));
                //}

            }
        }

        public void WriteMember(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (var item in MemberTable)
                {
                    sw.WriteLine(item.Lisp);
                }
            }

        }

        #endregion

        #region 私有方法
        public MemberPropertyRecord GetTubeProperty(double x, eMemberType member)
        {
            if (member == eMemberType.Virtual)
            {

            }
            var prop = PropertyTable.FindLast((pp) => pp.CheckProperty(x, member));
            return prop;
        }

        public List<Angle> Get3AngleReal(double x)
        {
            Angle AngUp, AngCC, AngLow;

            AngCC = Axis.GetAngle(x);
            Line2D theCutLine = new Line2D(Axis.GetCenter(x), Axis.GetCenter(x) + new Vector2D(0, 1));
            List<Angle> tmp = new List<Angle>();
            for (int i = 0; i < UpSkeleton.Count - 1; i++)
            {
                var seg = new List<Point2D>() { UpSkeleton[i], UpSkeleton[i + 1] };
                var pt = Extension.Intersection(seg, theCutLine);
                if (pt != null)
                {
                    tmp.Add(Vector2D.XAxis.SignedAngleTo(UpSkeleton[i + 1] - UpSkeleton[i]));
                }
            }
            AngUp = Angle.FromDegrees((from a in tmp select a.Degrees).ToList().Average());

            tmp = new List<Angle>();
            for (int i = 0; i < LowSkeleton.Count - 1; i++)
            {
                var seg = new List<Point2D>() { LowSkeleton[i], LowSkeleton[i + 1] };
                var pt = Extension.Intersection(seg, theCutLine);
                if (pt != null)
                {
                    tmp.Add(Vector2D.XAxis.SignedAngleTo(LowSkeleton[i + 1] - LowSkeleton[i]));
                }
            }
            AngLow = Angle.FromDegrees((from a in tmp select a.Degrees).ToList().Average());

            List<Angle> res = new List<Angle>() { AngUp, AngCC, AngLow };
            return res;


        }

        /// <summary>
        /// 获取曲线拱法线方向三个轴点
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
        /// 获取折线拱法线方向三个轴点
        /// </summary>
        /// <param name="x0"></param>
        /// <returns></returns>
        public List<Point2D> get_3pt_real(double x0)
        {
            double y0 = Axis.GetZ(x0);
            var xlist1 = (from a in MainDatum select a.Center.X).ToList();
            var xlist2 = (from a in SecondaryDatum select a.Center.X).ToList();

            var xlist = (xlist1.Concat(xlist2)).ToList();
            xlist.Sort();
            if (xlist.Contains(x0))
            {
                return get_3pt(x0);
            }
            else
            {
                var x_1 = xlist.Find((x) => x > x0);
                var x_2 = xlist.FindLast((x) => x < x0);

                var y_1 = Get3Point(x_1, 90)[0].Y;
                var y_2 = Get3Point(x_2, 90)[0].Y;

                double y_up = Extension.Interplate(x_1, x_2, y_1, y_2, x0);


                y_1 = Get3Point(x_1, 90)[2].Y;
                y_2 = Get3Point(x_2, 90)[2].Y;
                double y_down = Extension.Interplate(x_1, x_2, y_1, y_2, x0);

                return new List<Point2D>() { new Point2D(x0, y_up), new Point2D(x0, y0), new Point2D(x0, y_down) };
            }
        }

        /// <summary>
        /// 获取曲线拱法线方向7个轴点
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
        /// 抛弃
        /// </summary>
        /// <param name="x_from"></param>
        /// <param name="x_to"></param>
        /// <param name="e"></param>
        /// <param name="dia"></param>
        /// <returns></returns>
        DatumPlane get_diagnal_sectin(double x_from, double x_to, double e, double dia)
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
            var Pa = Get7Point(Bisection.FindRoot(f, x0, x1, 1e-5), 90)[2];

            f = (t) => Get7Point(t, 90)[4].DistanceTo(P2) * Math.Sign(t - P2.X) + e;
            var Pb = Get7Point(Bisection.FindRoot(f, x0, x1, 1e-5), 90)[4];

            f = (t) => Intersect((new Line2D(Pb, Get7Point(t, 90.0)[2]).Offset(dia)))[2].DistanceTo(Pa)
                  * Math.Sign(Pa.X - Intersect(new Line2D(Pb, Get7Point(t, 90.0)[2]).Offset(dia))[2].X);
            var Pa1 = Get7Point(Bisection.FindRoot(f, x0, x1, 1e-5), 90)[2];

            Line2D CL;

            if (x_from < 0)
            {
                CL = new Line2D(Pb, Pa1).Offset(0.5 * dia);
            }
            else
            {
                CL = new Line2D(new Point2D(-Pb.X, Pb.Y), new Point2D(-Pa1.X, Pa1.Y)).Offset(0.5 * -dia);
            }

            DatumPlane ret = new DatumPlane(0, Axis.Intersect(CL), Vector2D.XAxis.AngleTo(CL.Direction), eDatumType.DiagonalDatum);


            return ret;
        }

        /// <summary>
        /// 抛弃
        /// </summary>
        /// <param name="line2D"></param>
        /// <returns></returns>
        private List<Point2D> Intersect(Line2D line2D)
        {
            Point2D CC = Axis.Intersect(line2D);
            return Get7Point(CC.X, Vector2D.XAxis.SignedAngleTo(line2D.Direction).Degrees);
        }

        #endregion

        #region 几何方法

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
        /// 获取曲线拱任意方向三个轴点
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
            x_new = Bisection.FindRoot(f, x0 - 0.25 * Axis.L1, x0 + 0.25 * Axis.L1, 1e-6);
            Lower = get_3pt(x_new)[2];

            return new List<Point2D>() { Upper, CC, Lower };
        }

        public List<Point2D> Get3PointV22(double x0, double ang_deg)
        {
            Point2D Upper, CC, Lower;
            CC = Axis.GetCenter(x0);
            Angle CutAngle = Angle.FromDegrees(ang_deg);
            Vector2D TargetDir = Vector2D.XAxis.Rotate(CutAngle);
            Func<double, double> f = (x) => ((TargetDir).SignedAngleBetween((get_3pt(x)[0] - Axis.GetCenter(x0))));
            double x_new = 0;
            try
            {
                x_new = x0 > 0 ? Bisection.FindRoot(f, 0, Axis.L1 * 3, 1e-6) : Bisection.FindRoot(f, -3 * Axis.L1, 0, 1e-6);
            }
            catch (Exception)
            {
                f = (x) => ((-TargetDir).SignedAngleBetween((get_3pt(x)[0] - Axis.GetCenter(x0))));
                x_new = x0 > 0 ? Bisection.FindRoot(f, 0, Axis.L1 * 3, 1e-6) : Bisection.FindRoot(f, -3 * Axis.L1, 0, 1e-6);
            }
          
            Upper = get_3pt(x_new)[0];

            f = (x) => ((TargetDir).SignedAngleBetween((get_3pt(x)[2] - Axis.GetCenter(x0))));
            try
            {
                x_new = x0 > 0 ? Bisection.FindRoot(f, 0, Axis.L1 * 3, 1e-6) : Bisection.FindRoot(f, -3 * Axis.L1, 0, 1e-6);
            }
            catch (Exception)
            {
                f = (x) => ((-TargetDir).SignedAngleBetween((get_3pt(x)[2] - Axis.GetCenter(x0))));
                x_new = x0 > 0 ? Bisection.FindRoot(f, 0, Axis.L1 * 3, 1e-6) : Bisection.FindRoot(f, -3 * Axis.L1, 0, 1e-6);
            }

            Lower = get_3pt(x_new)[2];

            return new List<Point2D>() { Upper, CC, Lower };
        }

        public List<Point2D> Get3Point(DatumPlane cutPlane)
        {
            return Get3Point(cutPlane.Center.X, cutPlane.Angle0.Degrees);
        }

        /// <summary>
        /// 获取折线拱任意角度三轴线
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="ang_deg"></param>
        /// <param name="Upper"></param>
        /// <param name="CC"></param>
        /// <param name="Lower"></param>
        public List<Point2D> Get3PointReal(double x0, double ang_deg)
        {
            Point2D Upper, CC, Lower;
            CC = Axis.GetCenter(x0);
            Angle CutAngle = Angle.FromDegrees(ang_deg);
            Vector2D TargetDir = Vector2D.XAxis.Rotate(CutAngle);
            Line2D cutLine = new Line2D(CC, CC + TargetDir);

            var pu = Extension.Intersection(UpSkeleton, cutLine);
            var pb = Extension.Intersection(LowSkeleton, cutLine);

            if (pu != null)
            {
                Upper = (Point2D)pu;
            }
            else
            {
                Upper = Get3Point(cutLine.Datum(ref Axis))[0];
            }

            if (pb != null)
            {
                Lower = (Point2D)pb;
            }
            else
            {
                Lower = Get3Point(cutLine.Datum(ref Axis))[2];
            }


            return new List<Point2D>() { Upper, CC, Lower };
        }

        /// <summary>
        /// 获取折线拱任意角度三轴线交点
        /// </summary>
        /// <param name="cutPlane"></param>
        /// <returns></returns>
        public List<Point2D> Get3PointReal(DatumPlane cutPlane)
        {
            return Get3PointReal(cutPlane.Center.X, cutPlane.Angle0.Degrees);
        }

        /// <summary>
        /// 获取曲线拱任意方向7个轴点
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

        /// <summary>
        /// 获取折线拱任意方向7个轴点
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="ang_deg"></param>
        /// <returns></returns>
        public List<Point2D> Get7PointReal(double x0, double ang_deg)
        {
            Point2D Upper, UU, UL, CC, Lower, LU, LL;
            CC = Axis.GetCenter(x0);
            Angle CutAngle = Angle.FromDegrees(ang_deg);
            Vector2D TargetDir = Vector2D.XAxis.Rotate(CutAngle);
            Line2D cutLine = new Line2D(CC, CC + TargetDir);


            Upper = (Point2D)Extension.Intersection(UpSkeleton, cutLine);
            UU = (Point2D)Extension.Intersection(UpUpSkeleton, cutLine);
            UL = (Point2D)Extension.Intersection(UpLowSkeleton, cutLine);

            Lower = (Point2D)Extension.Intersection(LowSkeleton, cutLine);
            LU = (Point2D)Extension.Intersection(LowUpSkeleton, cutLine);
            LL = (Point2D)Extension.Intersection(LowLowSkeleton, cutLine);

            return new List<Point2D>() { UU, Upper, UL, CC, LU, Lower, LL };
        }

        /// <summary>
        /// 获取折线拱任意方向7轴点
        /// </summary>
        /// <param name="cutPlane"></param>
        /// <returns></returns>
        public List<Point2D> Get7PointReal(DatumPlane cutPlane)
        {
            return Get7PointReal(cutPlane.Center.X, cutPlane.Angle0.Degrees);
        }


        /// <summary>
        /// 骨架线偏移出管壁
        /// </summary>
        /// <param name="input"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        List<Point2D> OffsetSkeleton(List<Point2D> input, double val)
        {
            List<Point2D> ret = new List<Point2D>();
            Vector2D dir;
            double dist = val;
            for (int i = 0; i < input.Count; i++)
            {
                if (i == 0)
                {
                    dir = (input[i + 1] - input[i]).Normalize();
                    dir = dir.Rotate(Angle.FromDegrees(90.0));
                    dist = val;
                }
                else if (i != input.Count - 1)
                {
                    var dir1 = (input[i + 1] - input[i]).Normalize();
                    var dir0 = (input[i] - input[i - 1]).Normalize();
                    dir = (dir1 + dir0).Normalize();
                    dir = dir.Rotate(Angle.FromDegrees(90.0));
                    Angle m = dir1.SignedAngleTo(dir) - Angle.FromDegrees(90.0);
                    dist = val / Math.Cos(m.Radians);

                }
                else
                {
                    dir = (input[i] - input[i - 1]).Normalize();
                    dir = dir.Rotate(Angle.FromDegrees(90.0));
                    dist = val;
                }
                ret.Add(input[i] + dir * dist);
            }
            return ret;
        }


        #endregion

    }
}
