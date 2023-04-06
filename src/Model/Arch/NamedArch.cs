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
    public static class NamedArch
    {
        #region 已命名模型


        /// <summary>
        /// 施设模型V6-49.5m跨径、取消下排平联
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV63(out ArchAxis theArchAxis, double m, double f, double H_foot, double H_top, double halfD = 0.75)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            double e = 0.06;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, H_top, H_foot, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            double CenterHalfD = 1.0;
            #endregion

            #region  2. 配置截面            
            var CFTS1400S25 = new TubeSection(1, 1.4, 0.025, true);
            var CFTS1400S28 = new TubeSection(2, 1.4, 0.028, true);
            var CFTS1400S32 = new TubeSection(3, 1.4, 0.032, true);
            var CFTS1400S38 = new TubeSection(4, 1.4, 0.038, true);
            var CFTS1400S40 = new TubeSection(5, 1.4, 0.040, true);

            // 腹杆
            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);

            // 内隔
            var T800S20D = new TubeSection(31, 0.80, 0.020);
            var T600S16D = new TubeSection(32, 0.60, 0.016);
            var T300S10D = new TubeSection(33, 0.30, 0.010);


            // 横梁
            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T600S12B = new TubeSection(42, 0.60, 0.012);
            var T500S10B = new TubeSection(43, 0.50, 0.010);

            // 风撑
            var T700S16W = new TubeSection(51, 0.70, 0.016);
            var T600S16W = new TubeSection(52, 0.60, 0.016);
            var T500S10W = new TubeSection(53, 0.50, 0.010);

            // 立柱

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, double.NegativeInfinity, -239.3699);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -239.3699, -206.25);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, -206.25, -132.0);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, -132.0, -82.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -82.5, 82.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, 82.5, 132.0);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, 132.0, 206.25);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, 206.25, 239.3699);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, 239.3699, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, double.NegativeInfinity, -230.3386);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, -230.3386, -180.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, -180.75, -132.0);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, -132.0, -57.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S25, -57.75, 57.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, 57.75, 132.0);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, 132.0, 180.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, 180.75, 230.3386);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, 230.3386, double.PositiveInfinity);



            List<Tuple<double, double>> WebStrong = new List<Tuple<double, double>>();
            WebStrong.Add(new Tuple<double, double>(24.75, 41.25));
            WebStrong.Add(new Tuple<double, double>(74.25, 90.75));
            WebStrong.Add(new Tuple<double, double>(123.75, 140.25));
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, -WebStrong[2].Item2);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, -WebStrong[2].Item2, -WebStrong[2].Item1);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -WebStrong[2].Item1, -WebStrong[1].Item2);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, -WebStrong[1].Item2, -WebStrong[1].Item1);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -WebStrong[1].Item1, -WebStrong[0].Item2);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, -WebStrong[0].Item2, -WebStrong[0].Item1);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -WebStrong[0].Item1, WebStrong[0].Item1);//
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, WebStrong[0].Item1, WebStrong[0].Item2);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, WebStrong[0].Item2, WebStrong[1].Item1);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, WebStrong[1].Item1, WebStrong[1].Item2);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, WebStrong[1].Item2, WebStrong[2].Item1);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, WebStrong[2].Item1, WebStrong[2].Item2);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, WebStrong[2].Item2, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S20D);
            archModel.AssignProperty(eMemberType.DiaphragmCoord, T600S16D);            
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B, double.NegativeInfinity, -4.125);
            archModel.AssignProperty(eMemberType.CrossCoord, T600S12B, -4.125, 4.125);
            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B, 4.125, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T700S16W, double.NegativeInfinity, -160.5);
            archModel.AssignProperty(eMemberType.WindBracing, T600S16W, -160.5, -12.4);
            archModel.AssignProperty(eMemberType.WindBracing, T500S10W, -12.4, 12.4);
            archModel.AssignProperty(eMemberType.WindBracing, T600S16W, 12.4, 160.5);
            archModel.AssignProperty(eMemberType.WindBracing, T700S16W, 160.5, double.PositiveInfinity);
            // archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 206.25;
            double C10V = 222.75;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置
            // KP0
            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 1;
            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 5;

            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 3);
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL2 * 2);
            List<double> xs = new List<double>() {
                 -CTL0, CTL0,0,
                -33.00, -57.75, -82.50,-107.25,-132.00,-156.75,-181.50,-206.25,
                33.00, 57.75, 82.50,107.25,132.00,156.75,181.50,206.25,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) > (CTL0 - 1))
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架
            double Cell = 8.25;
            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if ((Math.Abs(NexI.Center.X - CurI.Center.X) - Cell * 3) < 1e-3)
                        {
                            // 3节
                            if (Math.Abs(CurI.Center.X + 82.5) < 1e-3)
                            {
                                //GL8
                                archModel.CreateInstallSegmentNew(CurI, NexI,
                                    new double[] { halfD, Cell - halfD, Cell, Cell - halfD, halfD },
                                    new double[] { 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, false }, 0.060,
                                    new bool[] { false, false, true, true, false });
                            }
                            else if (Math.Abs( CurI.Center.X - 57.75) < 1e-3) 
                            {
                                archModel.CreateInstallSegmentNew(CurI, NexI,
                                    new double[] { halfD, Cell - halfD, Cell, Cell - halfD, halfD },
                                    new double[] { 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, false }, 0.060,
                                    new bool[] { false, false, false, true, false });
                            }
                            else
                            {
                                archModel.CreateInstallSegment(CurI, NexI,
                                    new double[] { halfD, Cell - halfD, Cell, Cell - halfD, halfD },
                                    new double[] { 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, false }, 0.060);
                            }


                        }
                        else if ((Math.Abs(NexI.Center.X - CurI.Center.X) - Cell * 2) < 1e-3)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, Cell - halfD, Cell - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {
                            // 首节
                            if (CurI.RefPoint.X < 0)
                            {
                                archModel.CreateInstallSegmentNew(CurI, NexI,
                                    new double[] { halfD, Cell - halfD, Cell, Cell, Cell - CenterHalfD, CenterHalfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060,
                                    new bool[] { false, false, true, true, true, false });
                            }
                            else
                            {
                                archModel.CreateInstallSegmentNew(CurI, NexI,
                                    new double[] { CenterHalfD, Cell - CenterHalfD, Cell, Cell, Cell - halfD, halfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060,
                                    new bool[] { false, false, false, false, true, false });
                            }
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL2);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {
                                halfD,
                                KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2,
                                NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1); // 立柱位置
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL2);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X) - (-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] {
                                    locs[0] - CurI.Center.X,
                                    locs[1] - locs[0],
                                    c2.X - locs[1], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,                                
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1,
                                    locs[0] - c1.X,
                                    locs[1] - locs[0],
                                    NexI.Center.X - locs[1] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                
                                },
                                new bool[] { false, true, true,  false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -C10V;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 4, 4 };
            double[] H2S = new double[]
            {
                1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,
               //1403.463500,1403.760500,1404.057500,1404.302173,1404.424686,
               // 1404.424686,1404.302174,1404.057500,1403.760500,1403.463500,
            };
            for (int i = 0; i < 10; i++)
            {
                var xi = xx + i * 49.5;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double RtZ0 = -archModel.Axis.f + 9.0;
            double RtZ1 = 5.8888888 - 4;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 7, 4);
            RectSection S2 = new RectSection(0, 5, 4);
            RectSection S0 = new RectSection(0, 7, 4 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(1, -272.25, new RCColumn(1, RtZ0, RtZ1, RtZ1+4, S0, S1, S2));
            archModel.AddColumn(2, 272.25, new RCColumn(2, RtZ0, RtZ1, RtZ1+4, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            archModel.MidInstallDist = 2 * CenterHalfD;
            return archModel;
        }




        /// <summary>
        /// 施设模型V6-49.5m跨径、取消下排平联
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV62(out ArchAxis theArchAxis, double m, double f, double H_foot, double H_top, double halfD = 0.75)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            double e = 0.06;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, H_top, H_foot, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            double CenterHalfD = 1.0;
            #endregion

            #region  2. 配置截面            
            var CFTS1400S25 = new TubeSection(1, 1.4, 0.025, true);
            var CFTS1400S28 = new TubeSection(2, 1.4, 0.028, true);
            var CFTS1400S32 = new TubeSection(3, 1.4, 0.032, true);
            var CFTS1400S38 = new TubeSection(4, 1.4, 0.038, true);
            var CFTS1400S40 = new TubeSection(5, 1.4, 0.040, true);

            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);


            var T800S16D = new TubeSection(31, 0.80, 0.016);
            var T300S10D = new TubeSection(32, 0.30, 0.010);

            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T500S10B = new TubeSection(42, 0.50, 0.010);

            var T600S16W = new TubeSection(51, 0.60, 0.016);

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, double.NegativeInfinity, -237.1276);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -237.1276, -206.25);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, -206.25, -132.0);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, -132.0, -82.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -82.5, 82.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, 82.5, 132.0);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, 132.0, 206.25);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, 206.25, 237.1276);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, 237.1276, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, double.NegativeInfinity, -228.2744);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, -228.2744, -180.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, -180.75, -132.0);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, -132.0, -57.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S25, -57.75, 57.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, 57.75, 132.0);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, 132.0, 180.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, 180.75, 228.2744);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, 228.2744, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S16D);
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B);
            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 206.25;
            double C10V = 222.75;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置
            // KP0
            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 1;
            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 6;

            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 3);
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL2 * 2);
            List<double> xs = new List<double>() {
                 -CTL0, CTL0,0,
                -33.00, -57.75, -82.50,-107.25,-132.00,-156.75,-181.50,-206.25,
                33.00, 57.75, 82.50,107.25,132.00,156.75,181.50,206.25,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) > (CTL0 - 1))
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架
            double Cell = 8.25;
            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if ((Math.Abs(NexI.Center.X - CurI.Center.X) - Cell * 3) < 1e-3)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, Cell - halfD, Cell, Cell - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if ((Math.Abs(NexI.Center.X - CurI.Center.X) - Cell * 2) < 1e-3)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, Cell - halfD, Cell - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {

                            if (CurI.RefPoint.X < 0)
                            {
                                archModel.CreateInstallSegment(CurI, NexI,
                                    new double[] { halfD, Cell - halfD, Cell, Cell, Cell - CenterHalfD, CenterHalfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060);
                            }
                            else
                            {
                                archModel.CreateInstallSegment(CurI, NexI,
                                    new double[] { CenterHalfD, Cell - CenterHalfD, Cell, Cell, Cell - halfD, halfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060);
                            }
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL2);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {
                                halfD,
                                KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2,
                                NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1); // 立柱位置
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL2);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X) - (-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 4);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] {
                                    locs[0] - CurI.Center.X,
                                    locs[1] - locs[0],
                                    locs[2]-locs[1],
                                    c2.X - locs[2], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                theArchAxis.GetNormalAngle(locs[2]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 4);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1,
                                    locs[0] - c1.X,
                                    locs[1] - locs[0],
                                    locs[2] - locs[1],
                                    NexI.Center.X - locs[2] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                theArchAxis.GetNormalAngle(locs[2]).Degrees,
                                },
                                new bool[] { false, true, true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -C10V;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 4, 4 };
            double[] H2S = new double[]
            {
                1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,
               //1403.463500,1403.760500,1404.057500,1404.302173,1404.424686,
               // 1404.424686,1404.302174,1404.057500,1403.760500,1403.463500,
            };
            for (int i = 0; i < 10; i++)
            {
                var xi = xx + i * 49.5;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double P2H2 = 1403.166500;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(0, -272.25, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 272.25, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            archModel.MidInstallDist = 2 * CenterHalfD;
            return archModel;
        }



        /// <summary>
        /// 施设模型V6-49.5m跨径、取消下排平联
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV61(out ArchAxis theArchAxis, double m, double f, double H_foot, double H_top, double halfD = 0.75)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            double e = 0.06;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, H_top, H_foot, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            double CenterHalfD = 1.0;
            #endregion

            #region  2. 配置截面            
            var CFTS1400S25 = new TubeSection(1, 1.4, 0.025, true);
            var CFTS1400S28 = new TubeSection(2, 1.4, 0.028, true);
            var CFTS1400S32 = new TubeSection(3, 1.4, 0.032, true);
            var CFTS1400S38 = new TubeSection(4, 1.4, 0.038, true);
            var CFTS1400S40 = new TubeSection(5, 1.4, 0.040, true);

            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);


            var T800S16D = new TubeSection(31, 0.80, 0.016);
            var T300S10D = new TubeSection(32, 0.30, 0.010);

            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T500S10B = new TubeSection(42, 0.50, 0.010);

            var T600S16W = new TubeSection(51, 0.60, 0.016);

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, double.NegativeInfinity, -238.4456);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -238.4456, -206.25);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, -206.25, -132.0);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, -132.0, -82.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -82.5, 82.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, 82.5, 132.0);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, 132.0, 206.25);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, 206.25, 238.4456);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, 238.4456, double.PositiveInfinity);




            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, double.NegativeInfinity, -229.5);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, -229.5, -180.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, -180.75, -132.0);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, -132.0, -57.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S25, -57.75, 57.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, 57.75, 132.0);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, 132.0, 180.75);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, 180.75, 229.5);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, 229.5, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S16D);
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B);
            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 206.25;
            double C10V = 222.75;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置
            // KP0
            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 1;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 * 3);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 7;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 3);

            List<double> xs = new List<double>() {
                 -CTL0, CTL0,0,
                -33.00, -57.75, -82.50,-107.25,-132.00,-156.75,-181.50,-206.25,
                33.00, 57.75, 82.50,107.25,132.00,156.75,181.50,206.25,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) > (CTL0 - 1))
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架
            double Cell = 8.25;
            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if ((Math.Abs(NexI.Center.X - CurI.Center.X) - Cell * 3) < 1e-3)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, Cell - halfD, Cell, Cell - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if ((Math.Abs(NexI.Center.X - CurI.Center.X) - Cell * 2) < 1e-3)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, Cell - halfD, Cell - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {

                            if (CurI.RefPoint.X < 0)
                            {
                                archModel.CreateInstallSegment(CurI, NexI,
                                    new double[] { halfD, Cell - halfD, Cell, Cell, Cell - CenterHalfD, CenterHalfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060);
                            }
                            else
                            {
                                archModel.CreateInstallSegment(CurI, NexI,
                                    new double[] { CenterHalfD, Cell - CenterHalfD, Cell, Cell, Cell - halfD, halfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060);
                            }
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {
                                halfD,
                                KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2,
                                NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X) - (-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 4);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] {
                                    locs[0] - CurI.Center.X,
                                    locs[1] - locs[0],
                                    locs[2]-locs[1],
                                    c2.X - locs[2], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                theArchAxis.GetNormalAngle(locs[2]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 4);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1,
                                    locs[0] - c1.X,
                                    locs[1] - locs[0],
                                    locs[2] - locs[1],
                                    NexI.Center.X - locs[2] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                theArchAxis.GetNormalAngle(locs[2]).Degrees,
                                },
                                new bool[] { false, true, true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -C10V;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 4, 4 };
            double[] H2S = new double[]
            {
                1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,1402.3,
               //1403.463500,1403.760500,1404.057500,1404.302173,1404.424686,
               // 1404.424686,1404.302174,1404.057500,1403.760500,1403.463500,
            };
            for (int i = 0; i < 10; i++)
            {
                var xi = xx + i * 49.5;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double P2H2 = 1403.166500;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(0, -272.25, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 272.25, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            archModel.MidInstallDist = 2 * CenterHalfD;
            return archModel;
        }


        /// <summary>
        /// 彩凤模型V6-50m跨径
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV6(out ArchAxis theArchAxis, double m, double f, double H_foot, double H_top, double halfD = 0.75)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            double e = 0.06;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, H_top, H_foot, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            double CenterHalfD = 1.0;
            #endregion

            #region  2. 配置截面            
            var CFTS1400S25 = new TubeSection(1, 1.4, 0.025, true);
            var CFTS1400S28 = new TubeSection(2, 1.4, 0.028, true);
            var CFTS1400S32 = new TubeSection(3, 1.4, 0.032, true);
            var CFTS1400S38 = new TubeSection(4, 1.4, 0.038, true);
            var CFTS1400S40 = new TubeSection(5, 1.4, 0.040, true);

            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);


            var T800S16D = new TubeSection(31, 0.80, 0.016);
            var T300S10D = new TubeSection(32, 0.30, 0.010);

            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T500S10B = new TubeSection(42, 0.50, 0.010);

            var T600S16W = new TubeSection(51, 0.60, 0.016);

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, double.NegativeInfinity, -244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -244.5, -224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, -224.5, -70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, -70, 70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, 70, 224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, 224.5, 244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, 244.5, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, double.NegativeInfinity, -234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, -234.9, -196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, -196, -154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, -154, -70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S25, -70, 70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, 70, 154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, 154, 196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, 196, 234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, 234.9, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S16D);
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B);
            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 206.25;
            double C10V = 222.75;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置
            // KP0
            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 1;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 * 3);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 7;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 *3);

            List<double> xs = new List<double>() {
                 -CTL0, CTL0,0,
                -33.00, -57.75, -82.50,-107.25,-132.00,-156.75,-181.50,-206.25,
                33.00, 57.75, 82.50,107.25,132.00,156.75,181.50,206.25,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) > (CTL0-1))
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架
            double Cell = 8.25;
            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if ((Math.Abs(NexI.Center.X - CurI.Center.X)-Cell*3) <1e-3)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, Cell - halfD, Cell, Cell - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if ((Math.Abs(NexI.Center.X - CurI.Center.X) - Cell * 2) < 1e-3)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, Cell - halfD, Cell - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {

                            if (CurI.RefPoint.X<0)
                            {
                                archModel.CreateInstallSegment(CurI, NexI,
                                    new double[] { halfD, Cell - halfD, Cell, Cell, Cell - CenterHalfD, CenterHalfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060);
                            }
                            else
                            {
                                archModel.CreateInstallSegment(CurI, NexI,
                                    new double[] { CenterHalfD, Cell - CenterHalfD, Cell, Cell, Cell - halfD, halfD },
                                    new double[] { 90, 90, 90, 90, 90 },
                                    new bool[] { false, true, true, true, true, false }, 0.060);
                            }
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {
                                halfD, 
                                KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2,
                                NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,                                 
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);
                        // var CTLx3 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X) - (-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,                                
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 4);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { 
                                    locs[0] - CurI.Center.X, 
                                    locs[1] - locs[0],
                                    locs[2]-locs[1],
                                    c2.X - locs[2], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                theArchAxis.GetNormalAngle(locs[2]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 4);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1,
                                    locs[0] - c1.X,
                                    locs[1] - locs[0],
                                    locs[2] - locs[1],
                                    NexI.Center.X - locs[2] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                theArchAxis.GetNormalAngle(locs[2]).Degrees,
                                },
                                new bool[] { false, true,true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -C10V;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 4, 4 };
            double[] H2S = new double[]
            {
               1403.463500,1403.760500,1404.057500,1404.302173,1404.424686,
                1404.424686,1404.302174,1404.057500,1403.760500,1403.463500,
            };
            for (int i = 0; i < 10; i++)
            {
                var xi = xx + i * 49.5;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double P2H2 = 1403.166500;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(0, -272.25, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0,  272.25, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            archModel.MidInstallDist = 2 * CenterHalfD;
            return archModel;
        }


        /// <summary>
        /// 彩凤模型V5临时调整立柱间距
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV5(out ArchAxis theArchAxis, double m, double f, double H_foot, double H_top, double halfD = 0.6)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            double e = 0.060;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            // archModel = new Arch(theArchAxis, 6.0, 14.5, 14, 4);
            // archModel = new Arch(theArchAxis, 7.0, 15.5, 14, 4);
            // archModel = new Arch(theArchAxis, 8.5, 17.0, 14, 4);
            archModel = new Arch(theArchAxis, H_top, H_foot, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            //archModel.SetFootLevel(1270);
            #endregion

            #region  2. 配置截面            
            var CFTS1400S25 = new TubeSection(1, 1.4, 0.025, true);
            var CFTS1400S28 = new TubeSection(2, 1.4, 0.028, true);
            var CFTS1400S32 = new TubeSection(3, 1.4, 0.032, true);
            var CFTS1400S38 = new TubeSection(4, 1.4, 0.038, true);
            var CFTS1400S40 = new TubeSection(5, 1.4, 0.040, true);

            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);


            var T800S16D = new TubeSection(31, 0.80, 0.016);
            var T300S10D = new TubeSection(32, 0.30, 0.010);

            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T500S10B = new TubeSection(42, 0.50, 0.010);

            var T600S16W = new TubeSection(51, 0.60, 0.016);

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, double.NegativeInfinity, -244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -244.5, -224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, -224.5, -70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, -70, 70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, 70, 224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, 224.5, 244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, 244.5, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, double.NegativeInfinity, -234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, -234.9, -196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, -196, -154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, -154, -70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S25, -70, 70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, 70, 154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, 154, 196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, 196, 234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, 234.9, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S16D);
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B);
            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 196.0;
            double C10V = 231;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置

            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 4;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 * 3);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 5;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 2);

            List<double> xs = new List<double>() {
                -CTL1, -CTL0, CTL1,CTL0,
                -196, -175, -154, -133, -112, -91, -70, -49, -28, 0,
                 196,  175,  154,  133,  112,  91,  70,  49,  28,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) >= CTL0)
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架

            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if (NexI.Center.X - CurI.Center.X == 21)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (NexI.Center.X - CurI.Center.X == 14)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7 - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, true, false }, 0.060);
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] { halfD, KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2, NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X)-(-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { locs[0] - CurI.Center.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], NexI.Center.X - locs[1] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -231;
            // double[] Ls = new double[] { 4, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 4 };
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };
            // double[] Ls = new double[] { 4,  2, 2, 2, 2, 2, 2, 2, 2, 4 };
            double[] H2S = new double[]
            {
                1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,1404.7,
            };
            //double[] xlist = new double[]
            //{
            //    -231,-175.6,-126,-77,-28.6,28.6,77,126,175.6,231
            //};
            //double[] xlist = new double[]
            //{
            //    -231,-196.6,-161.0,-126.0, -91.6, -56.0, -21.0,21.0, 56.0, 91.6,126.0, 161.0,196.6,231
            //};
            double[] xlist = new double[]
            {
                -231,-189,-147,-105, -63, -21, 21,63,105,147,189,231
            };
            for (int i = 0; i < 12; i++)
            {
                var xi = xlist[i];
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double P2H2 = 1413.676000;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            return archModel;
        }

        /// <summary>
        /// 彩凤模型V4-1.5m管径
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV42(out ArchAxis theArchAxis, double m, double f, double H_foot, double H_top, double halfD = 0.6)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            double e = 0.060;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            // archModel = new Arch(theArchAxis, 6.0, 14.5, 14, 4);
            // archModel = new Arch(theArchAxis, 7.0, 15.5, 14, 4);
            // archModel = new Arch(theArchAxis, 8.5, 17.0, 14, 4);
            archModel = new Arch(theArchAxis, H_top, H_foot, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            //archModel.SetFootLevel(1270);
            #endregion

            #region  2. 配置截面            
            var CFTS1400S25 = new TubeSection(1, 1.5, 0.025, true);
            var CFTS1400S28 = new TubeSection(2, 1.5, 0.028, true);
            var CFTS1400S32 = new TubeSection(3, 1.5, 0.032, true);
            var CFTS1400S38 = new TubeSection(4, 1.5, 0.038, true);
            var CFTS1400S40 = new TubeSection(5, 1.5, 0.040, true);

            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);


            var T800S16D = new TubeSection(31, 0.80, 0.016);
            var T300S10D = new TubeSection(32, 0.30, 0.010);

            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T500S10B = new TubeSection(42, 0.50, 0.010);

            var T600S16W = new TubeSection(51, 0.60, 0.016);

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, double.NegativeInfinity, -244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -244.5, -224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, -224.5, -70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, -70, 70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, 70, 224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, 224.5, 244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, 244.5, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, double.NegativeInfinity, -234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, -234.9, -196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, -196, -154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, -154, -70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S25, -70, 70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, 70, 154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, 154, 196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, 196, 234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, 234.9, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S16D);
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B);
            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 196.0;
            double C10V = 231;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置

            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 4;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 * 3);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 5;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 2);

            List<double> xs = new List<double>() {
                -CTL1, -CTL0, CTL1,CTL0,
                -196, -175, -154, -133, -112, -91, -70, -49, -28, 0,
                 196,  175,  154,  133,  112,  91,  70,  49,  28,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) >= CTL0)
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架

            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if (NexI.Center.X - CurI.Center.X == 21)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (NexI.Center.X - CurI.Center.X == 14)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7 - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, true, false }, 0.060);
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] { halfD, KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2, NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X)-(-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { locs[0] - CurI.Center.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], NexI.Center.X - locs[1] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -231;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };
            //double[] H2S = new double[]
            //{
            //    1403.714000,1403.966000,1404.218000,1404.464375,1404.640775,1404.728975,
            //    1404.728975,1404.640775,1404.464375,1404.218000,1403.966000,1403.714000,
            //};
            double[] H2S = new double[]
            {
                1400.705,1400.957,1401.209,1401.455,1401.631,1401.719,
                1401.719,1401.631,1401.455,1401.208,1400.956,1400.704,
            };
            for (int i = 0; i < 12; i++)
            {
                var xi = xx + i * 42;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double P2H2 = 1413.676000;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            return archModel;
        }


        /// <summary>
        /// 彩凤模型V4-1.4m管径
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV4(out ArchAxis theArchAxis, double m, double f,double H_foot,double H_top, double halfD = 0.6)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            double e = 0.060;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            // archModel = new Arch(theArchAxis, 6.0, 14.5, 14, 4);
            // archModel = new Arch(theArchAxis, 7.0, 15.5, 14, 4);
            // archModel = new Arch(theArchAxis, 8.5, 17.0, 14, 4);
            archModel = new Arch(theArchAxis, H_top, H_foot, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            //archModel.SetFootLevel(1270);
            #endregion

            #region  2. 配置截面            
            var CFTS1400S25 = new TubeSection(1, 1.4, 0.025, true);
            var CFTS1400S28 = new TubeSection(2, 1.4, 0.028, true);
            var CFTS1400S32 = new TubeSection(3, 1.4, 0.032, true);
            var CFTS1400S38 = new TubeSection(4, 1.4, 0.038, true);
            var CFTS1400S40 = new TubeSection(5, 1.4, 0.040, true);

            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);


            var T800S16D = new TubeSection(31, 0.80, 0.016);
            var T300S10D = new TubeSection(32, 0.30, 0.010);

            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T500S10B = new TubeSection(42, 0.50, 0.010);

            var T600S16W = new TubeSection(51, 0.60, 0.016);

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, double.NegativeInfinity, -244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, -244.5, -224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, -224.5, -70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S32, -70, 70);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S25, 70, 224.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S38, 224.5, 244.5);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1400S40, 244.5, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, double.NegativeInfinity, -234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, -234.9, -196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, -196, -154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, -154, -70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S25, -70, 70);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S28, 70, 154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S32, 154, 196);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S38, 196, 234.9);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1400S40, 234.9, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S16D);
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B);
            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 196.0;
            double C10V = 231;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置

            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 4;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 * 3);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 5;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 2);

            List<double> xs = new List<double>() {
                -CTL1, -CTL0, CTL1,CTL0,
                -196, -175, -154, -133, -112, -91, -70, -49, -28, 0,
                 196,  175,  154,  133,  112,  91,  70,  49,  28,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) >= CTL0)
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架

            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if (NexI.Center.X - CurI.Center.X == 21)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (NexI.Center.X - CurI.Center.X == 14)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7 - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, true, false }, 0.060);
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] { halfD, KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2, NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X)-(-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { locs[0] - CurI.Center.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], NexI.Center.X - locs[1] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -231;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };
            //double[] H2S = new double[]
            //{
            //    1403.714000,1403.966000,1404.218000,1404.464375,1404.640775,1404.728975,
            //    1404.728975,1404.640775,1404.464375,1404.218000,1403.966000,1403.714000,
            //};
            double[] H2S = new double[]
            {
                1400.453,1400.705,1400.957,1401.209,1401.455,1401.631,1401.719,
                1401.719,1401.631,1401.455,1401.208,1400.956,1400.704,1400.452,
            };
            for (int i = 0; i < 12; i++)
            {
                var xi = xx + i * 42;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double P2H2 = 1413.676000;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            return archModel;
        }

        /// <summary>
        /// 彩凤模型V3
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV3(out ArchAxis theArchAxis, double m, double f, double halfD = 0.6)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            //double m = 1.5;
            //double f = L / 4.0;
            double e = 0.060;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, 8.5, 17, 14, 4);
            //archModel = new Arch(theArchAxis,9.0, 17, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            //archModel.SetFootLevel(1270);
            #endregion

            #region  2. 配置截面            
            var CFTS1500S35 = new TubeSection(11, 1.5, 0.035, true);
            var CFTS1500S28 = new TubeSection(12, 1.5, 0.028, true);

            var T900S20A = new TubeSection(21, 0.90, 0.020);
            var T800S16A = new TubeSection(22, 0.80, 0.016);
            var T600S12A = new TubeSection(23, 0.60, 0.012);


            var T800S16D = new TubeSection(31, 0.80, 0.016);
            var T300S10D = new TubeSection(32, 0.30, 0.010);

            var T800S16B = new TubeSection(41, 0.80, 0.016);
            var T500S10B = new TubeSection(42, 0.50, 0.010);

            var T600S16W = new TubeSection(51, 0.60, 0.016);

            var T800S16C = new TubeSection(61, 0.80, 0.016);
            var T700S16C = new TubeSection(62, 0.70, 0.016);
            var T450S10C = new TubeSection(63, 0.45, 0.010);
            var T300S10C = new TubeSection(64, 0.30, 0.010);


            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1500S28, double.NegativeInfinity, -49);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1500S35, -49, 49);
            archModel.AssignProperty(eMemberType.UpperCoord, CFTS1500S28, 49, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1500S35, double.NegativeInfinity, -154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1500S28, -154, 154);
            archModel.AssignProperty(eMemberType.LowerCoord, CFTS1500S35, 154, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.MainWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.MainWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, double.NegativeInfinity, -220);
            archModel.AssignProperty(eMemberType.SubWeb, T800S16A, -220, 220);
            archModel.AssignProperty(eMemberType.SubWeb, T900S20A, 220, double.PositiveInfinity);

            archModel.AssignProperty(eMemberType.InstallWeb, T600S12A);
            archModel.AssignProperty(eMemberType.TriWeb, T900S20A);

            archModel.AssignProperty(eMemberType.DiaphragmCoord, T800S16D);
            archModel.AssignProperty(eMemberType.DiaphragmWeb, T300S10D);

            archModel.AssignProperty(eMemberType.CrossCoord, T800S16B);
            archModel.AssignProperty(eMemberType.CrossVerical, T500S10B);
            archModel.AssignProperty(eMemberType.CrossWeb, T500S10B);

            archModel.AssignProperty(eMemberType.WindBracing, T600S16W);

            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, double.NegativeInfinity, -175);
            archModel.AssignProperty(eMemberType.ColumnMain, T700S16C, -175, 175);
            archModel.AssignProperty(eMemberType.ColumnMain, T800S16C, 175, double.PositiveInfinity);
            archModel.AssignProperty(eMemberType.ColumnCrossL, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnCrossW, T450S10C);
            archModel.AssignProperty(eMemberType.ColumnWeb, T300S10C);


            #endregion

            #region 3. 切割拱圈
            // double halfD = 0.75;
            double LastV = 196.0;
            double C10V = 231;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置

            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 4;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 * 3);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 5;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 2);

            List<double> xs = new List<double>() {
                -CTL1, -CTL0, CTL1,CTL0,
                -196, -175, -154, -133, -112, -91, -70, -49, -28, 0,
                 196,  175,  154,  133,  112,  91,  70,  49,  28,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) >= CTL0)
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架

            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if (NexI.Center.X - CurI.Center.X == 21)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (NexI.Center.X - CurI.Center.X == 14)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7 - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, true, false }, 0.060);
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] { halfD, KP0.X - CurI.Center.X - halfD,
                                CTLx1-KP0.X,
                                CTLx2-CTLx1,
                                cced.X - CTLx2, NexI.Center.X - cced.X },
                            new double[] {
                                90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx1).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx2).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, true, false }, 0.060); ;
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx1 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);
                        var CTLx2 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx2) - CurI.Center.X - d1,
                            (-CTLx1) - (-CTLx2),
                            (-KP0.X)-(-CTLx1),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx2).Degrees,
                                theArchAxis.GetNormalAngle(-CTLx1).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { locs[0] - CurI.Center.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], NexI.Center.X - locs[1] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 5. 生成模型
            archModel.GenerateArch();
            // 5.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 6. 立柱建模
            double xx = -231;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };
            double[] H2S = new double[]
            {
                1403.714000,1403.966000,1404.218000,1404.464375,1404.640775,1404.728975,
                1404.728975,1404.640775,1404.464375,1404.218000,1403.966000,1403.714000,
            };
            for (int i = 0; i < 12; i++)
            {
                var xi = xx + i * 42;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 7. 交界墩
            double P2H2 = 1413.676000;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));
            archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            archModel.InstallDist = 2 * halfD;
            return archModel;
        }
        /// <summary>
        /// 彩凤模型V2
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModelV2(out ArchAxis theArchAxis, double m, double f)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            //double m = 1.5;
            //double f = L / 4.0;
            double e = 0.060;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, 8.5, 17, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            #endregion

            #region  2. 配置截面
            /// <remarks>
            /// 1:拱肋截面--15
            /// 2: 竖腹杆--20
            /// 3: 斜腹杆--18
            /// 4: 横梁--5
            /// 5：无使用--5
            /// 6：斜撑--7
            /// 7: 内横梁--6
            /// 8：立柱竖杆--23
            /// 9：立柱盖梁--9
            /// 10：立柱横杆--9
            /// 99 :刚臂
            /// </remarks>
            var MainSection = new TubeSection(1, 1.5, 0.035, true);
            var MainWebSection = new TubeSection(2, 0.8, 0.024);
            var SubWebSection = new TubeSection(3, 0.8, 0.024);
            var s4 = new TubeSection(4, 0.6, 0.016);
            var s8 = new TubeSection(8, 0.6, 0.016);
            var s10 = new TubeSection(10, 0.4, 0.016);
            var s5 = new TubeSection(5, 0.4, 0.016);
            var s6 = new TubeSection(6, 0.4, 0.016);
            var s7 = new TubeSection(7, 0.4, 0.016);

            archModel.AssignProperty(eMemberType.UpperCoord, MainSection);
            archModel.AssignProperty(eMemberType.LowerCoord, MainSection);
            archModel.AssignProperty(eMemberType.MainWeb, MainWebSection);
            archModel.AssignProperty(eMemberType.ColumnWeb, MainWebSection);
            archModel.AssignProperty(eMemberType.SubWeb, SubWebSection);
            archModel.AssignProperty(eMemberType.CrossCoord, s4);
            archModel.AssignProperty(eMemberType.WindBracing, s6);
            archModel.AssignProperty(eMemberType.TriWeb, SubWebSection);
            archModel.AssignProperty(eMemberType.ColumnMain, s8);
            archModel.AssignProperty(eMemberType.ColumnCrossL, s10);
            archModel.AssignProperty(eMemberType.ColumnCrossW, s10);
            archModel.AssignProperty(eMemberType.DiaphragmCoord, s7);

            #endregion

            #region 3. 切割拱圈
            double halfD = 0.6;
            double LastV = 196.0;
            double C10V = 231;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第1个法向安装面
            // CTL1: 第2个法向安装面
            // CTL2: 第3个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置

            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 3;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1 * 2);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 5;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 2);
            //var CTL2 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 4);


            //var ArchLC10 = theArchAxis.GetLength(KP1.X);

            //Point2D KP3 = theArchAxis.GetCenter( theArchAxis.GetX0(ArchLC10 + 40)); // 拱脚起点拼接位置

            //Debug.Assert(KP2.X < theArchAxis.L1);
            //var p7s = archModel.Get7Point(KP2.X, theArchAxis.GetNormalAngle(KP2.X).Degrees);
            //var ft=theArchAxis.GetCenter(theArchAxis.L1);
            //var ft_ang = (p7s[1] - ft).AngleTo(p7s[5] - ft);

            //Debug.Assert(ft_ang.Degrees > 60);
            //Debug.Assert(ft_ang.Degrees < 120);



            // 拱脚控制位置
            //var deg=theArchAxis.GetAngle(theArchAxis.L1).Degrees + 90.0 + 60.0;
            //var FootPT = archModel.Get3Point(theArchAxis.L1, deg)[0];
            //var X0=theArchAxis.IntersectV2(FootPT);
            //var dist = X2.DistanceTo(X0);


            List<double> xs = new List<double>() {
                -CTL1, -CTL0, CTL1,CTL0,
                -196, -175, -154, -133, -112, -91, -70, -49, -28, 0,
                 196,  175,  154,  133,  112,  91,  70,  49,  28,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) >= CTL0)
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架

            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if (NexI.Center.X - CurI.Center.X == 21)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (NexI.Center.X - CurI.Center.X == 14)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7 - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, true, false }, 0.060);
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            
                        var CTLx = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] { halfD, KP0.X - CurI.Center.X - halfD,
                                CTLx-KP0.X,
                                cced.X - CTLx, NexI.Center.X - cced.X },
                            new double[] { 90, theArchAxis.GetNormalAngle(KP0.X).Degrees,
                                 theArchAxis.GetNormalAngle(CTLx).Degrees,
                                NexI.Angle0.Degrees },
                            new bool[] { false, false, true, true, false }, 0.060);
                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        var CTLx = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);

                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-CTLx) - CurI.Center.X - d1,
                            (-KP0.X)-(-CTLx),
                            NexI.Center.X- (-KP0.X)-halfD,
                                halfD},
                            new double[] { CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(-CTLx).Degrees,
                                theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, true, false, false }, 0.060); ;

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);


                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { locs[0] - CurI.Center.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;


                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], NexI.Center.X - locs[1] },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                },
                                new bool[] { false, true, true, false }, 0.060);

                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1-DL2),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1+DL2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            throw new Exception();
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 6. 生成模型
            archModel.GenerateArch();
            // 6.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 7. 立柱建模
            double xx = -231;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };

            //double[] H2S = new double[]
            //{
            //    1414.012000,1414.348000,1414.679775,1414.944375,1415.120775,1415.208975,
            //    1415.208975,1415.120775,1414.944375,1414.679775,1414.348000,1414.012000
            //};
            double[] H2S = new double[]
            {
                1403.714000,1403.966000,1404.218000,1404.464375,1404.640775,1404.728975,
                1404.728975,1404.640775,1404.464375,1404.218000,1403.966000,1403.714000,
            };


            for (int i = 0; i < 12; i++)
            {
                var xi = xx + i * 42;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 8. 交界墩
            double P2H2 = 1413.676000;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));

            archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            return archModel;


        }
        /// <summary>
        /// 彩凤模型
        /// </summary>
        /// <param name="theArchAxis"></param>
        /// <returns></returns>
        public static Arch PhoenixModel(out ArchAxis theArchAxis, double m, double f)
        {
            Arch archModel;

            #region 基本步骤
            double L = 518.0;
            //double m = 1.5;
            //double f = L / 4.0;
            double e = 0.060;
            #endregion

            #region 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, 8.5, 17, 14, 4);
            archModel.SetFootLevel(1270 + 11.3);
            #endregion

            #region  2. 配置截面
            /// <remarks>
            /// 1:拱肋截面--15
            /// 2: 竖腹杆--20
            /// 3: 斜腹杆--18
            /// 4: 横梁--5
            /// 5：无使用--5
            /// 6：斜撑--7
            /// 7: 内横梁--6
            /// 8：立柱竖杆--23
            /// 9：立柱盖梁--9
            /// 10：立柱横杆--9
            /// 99 :刚臂
            /// </remarks>
            var MainSection = new TubeSection(1, 1.5, 0.035, true);
            var MainWebSection = new TubeSection(2, 0.8, 0.024);
            var SubWebSection = new TubeSection(3, 0.8, 0.024);
            var s4 = new TubeSection(4, 0.6, 0.016);
            var s8 = new TubeSection(8, 0.6, 0.016);
            var s10 = new TubeSection(10, 0.4, 0.016);
            var s5 = new TubeSection(5, 0.4, 0.016);
            var s6 = new TubeSection(6, 0.4, 0.016);
            var s7 = new TubeSection(7, 0.4, 0.016);

            archModel.AssignProperty(eMemberType.UpperCoord, MainSection);
            archModel.AssignProperty(eMemberType.LowerCoord, MainSection);
            archModel.AssignProperty(eMemberType.MainWeb, MainWebSection);
            archModel.AssignProperty(eMemberType.ColumnWeb, MainWebSection);
            archModel.AssignProperty(eMemberType.SubWeb, SubWebSection);
            archModel.AssignProperty(eMemberType.CrossCoord, s4);
            archModel.AssignProperty(eMemberType.WindBracing, s6);
            archModel.AssignProperty(eMemberType.TriWeb, SubWebSection);
            archModel.AssignProperty(eMemberType.ColumnMain, s8);
            archModel.AssignProperty(eMemberType.ColumnCrossL, s10);
            archModel.AssignProperty(eMemberType.ColumnCrossW, s10);
            archModel.AssignProperty(eMemberType.DiaphragmCoord, s7);

            #endregion

            #region 3. 切割拱圈
            double halfD = 0.75;
            double LastV = 196.0;
            double C10V = 231;
            // 法向控制位置
            // KP0: 第一个法向面 
            // KP1: C10立柱控制位置
            // KP2: 拱脚
            // CTL0: 第一个法向安装面

            Point2D KP0 = archModel.CreatNormalDatumByVertical(
                new DatumPlane(0, theArchAxis.GetCenter(LastV + halfD),
                Angle.FromDegrees(90.0), eDatumType.VerticalDatum), false, false, 0.06).Center;
            Point2D KP1 = theArchAxis.IntersectV2(archModel.Get7Point(C10V, 90.0)[1]);
            Point2D KP2 = theArchAxis.GetCenter(theArchAxis.L1);
            // C10控制位置

            var LA = theArchAxis.GetLength(KP1.X) - theArchAxis.GetLength(KP0.X);
            double DL1 = LA / 3;
            var CTL0 = theArchAxis.GetX0(theArchAxis.GetLength(KP0.X) + DL1);

            var LB = theArchAxis.GetLength(KP2.X) - theArchAxis.GetLength(KP1.X);
            double DL2 = LB / 5;
            var CTL1 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2);
            var CTL2 = theArchAxis.GetX0(theArchAxis.GetLength(KP1.X) + DL2 * 4);


            //var ArchLC10 = theArchAxis.GetLength(KP1.X);

            //Point2D KP3 = theArchAxis.GetCenter( theArchAxis.GetX0(ArchLC10 + 40)); // 拱脚起点拼接位置

            //Debug.Assert(KP2.X < theArchAxis.L1);
            //var p7s = archModel.Get7Point(KP2.X, theArchAxis.GetNormalAngle(KP2.X).Degrees);
            //var ft=theArchAxis.GetCenter(theArchAxis.L1);
            //var ft_ang = (p7s[1] - ft).AngleTo(p7s[5] - ft);

            //Debug.Assert(ft_ang.Degrees > 60);
            //Debug.Assert(ft_ang.Degrees < 120);



            // 拱脚控制位置
            //var deg=theArchAxis.GetAngle(theArchAxis.L1).Degrees + 90.0 + 60.0;
            //var FootPT = archModel.Get3Point(theArchAxis.L1, deg)[0];
            //var X0=theArchAxis.IntersectV2(FootPT);
            //var dist = X2.DistanceTo(X0);


            List<double> xs = new List<double>() {
                -CTL2, -CTL1, -CTL0,CTL2,CTL1,CTL0,
                -196, -182, -161, -140, -112, -84, -56, -28, 0,   28, 56, 84, 112, 140, 161, 182, 196,
                };

            xs.Sort();

            foreach (var x0 in xs)
            {
                if (Math.Abs(x0) >= CTL0)
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum);
                }
                else
                {
                    archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
                }
            }
            #endregion

            #region 4. 布置主平面，生成骨架

            for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
            {
                var CurI = archModel.InstallDatum[i];
                var NexI = archModel.InstallDatum[i + 1];
                if (CurI.Angle0 == Angle.FromDegrees(90.0))
                {
                    // 起终点为垂直面
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        if (NexI.Center.X - CurI.Center.X == 21)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (NexI.Center.X - CurI.Center.X == 14)
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7 - halfD, halfD },
                                new double[] { 90, 90, 90 },
                                new bool[] { false, true, true, false }, 0.060);
                        }
                        else
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
                                new double[] { 90, 90, 90, 90, 90 },
                                new bool[] { false, true, true, true, true, false }, 0.060);
                        }
                    }
                    else
                    {
                        // 起点垂直，终点法向                                                            

                        Line2D theCutLineEd = NexI.Line;
                        theCutLineEd = theCutLineEd.Offset(halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);

                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] { halfD, KP0.X - CurI.Center.X - halfD, cced.X - KP0.X, NexI.Center.X - cced.X },
                            new double[] { 90, theArchAxis.GetNormalAngle(KP0.X).Degrees, NexI.Angle0.Degrees },
                            new bool[] { false, false, true, false }, 0.060);


                    }
                }
                else
                {
                    if (NexI.Angle0 == Angle.FromDegrees(90))
                    {
                        // 起点为法向面，终点为垂直面
                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-halfD);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        var d1 = cced.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {  d1,
                            (-KP0.X) - CurI.Center.X - d1,
                            NexI.Center.X- (-KP0.X)-halfD,halfD},
                            new double[] { CurI.Angle0.Degrees, theArchAxis.GetNormalAngle(-KP0.X).Degrees, 90, },
                            new bool[] { false, true, false, false }, 0.060);

                    }
                    else
                    {
                        // 均为法向面

                        if (CurI.Center.X == -theArchAxis.L1)
                        {
                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            archModel.AddDatum(0, c2.X, eDatumType.NormalDatum);
                        }
                        else if (NexI.Center.X == theArchAxis.L1)
                        {
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            archModel.AddDatum(0, c1.X, eDatumType.NormalDatum);
                        }
                        else if (CurI.Center.X == -CTL1)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-2*DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(-CTL0)-DL1),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else if (CurI.Center.X == CTL0)
                        {
                            List<double> locs = new List<double>()
                            {
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1),
                                theArchAxis.GetX0(theArchAxis.GetLength(CTL0)+DL1*2),
                            };
                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }
                        else
                        {
                            var locs = EquallyLengthCut(ref theArchAxis, CurI, NexI, 3);

                            Line2D theCutLineSt = CurI.Line;
                            theCutLineSt = theCutLineSt.Offset(-halfD);
                            var c1 = theArchAxis.Intersect(theCutLineSt);
                            var d1 = c1.X - CurI.Center.X;

                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(halfD);
                            var c2 = theArchAxis.Intersect(theCutLineEd);
                            var d2 = NexI.Center.X - c2.X;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, locs[0] - c1.X, locs[1] - locs[0], c2.X - locs[1], d2 },
                                new double[] {
                                CurI.Angle0.Degrees,
                                theArchAxis.GetNormalAngle(locs[0]).Degrees,
                                theArchAxis.GetNormalAngle(locs[1]).Degrees,
                                NexI.Angle0.Degrees
                                },
                                new bool[] { false, true, true, true, false }, 0.060);
                        }


                    }


                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

            archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
            archModel.GenerateSkeleton();
            #endregion

            #region 6. 生成模型
            archModel.GenerateArch();
            // 6.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
            #endregion

            #region 7. 立柱建模
            double xx = -231;
            double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };

            //double[] H2S = new double[]
            //{
            //    1414.012000,1414.348000,1414.679775,1414.944375,1415.120775,1415.208975,
            //    1415.208975,1415.120775,1414.944375,1414.679775,1414.348000,1414.012000
            //};
            double[] H2S = new double[]
            {
                1403.714000,1403.966000,1404.218000,1404.464375,1404.640775,1404.728975,
                1404.728975,1404.640775,1404.464375,1404.218000,1403.966000,1403.714000,
            };


            for (int i = 0; i < 12; i++)
            {
                var xi = xx + i * 42;
                archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
            }
            archModel.GenerateColumn();
            #endregion

            #region 8. 交界墩
            double P2H2 = 1413.676000;
            double RtZ0 = -106;
            double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(0, 6, 3);
            RectSection S2 = new RectSection(0, 7, 3);
            RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));

            archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
            #endregion

            return archModel;


        }
        #endregion




        //public static Arch PreliminaryDesignModel(out ArchAxis theArchAxis)
        //{
        //    Arch archModel;

        //    #region 基本步骤
        //    double L = 518.0;
        //    double m = 2.0;
        //    double f = L / 4.5;
        //    double e = 0.060;
        //    #endregion

        //    #region 1. 设置拱系
        //    theArchAxis = new ArchAxis(f, m, L);
        //    archModel = new Arch(theArchAxis, 8.5, 17, 14, 4);
        //    archModel.SetFootLevel(1270 + 11.3);
        //    #endregion

        //    #region  2. 配置截面
        //    /// <remarks>
        //    /// 1:拱肋截面--15
        //    /// 2: 竖腹杆--20
        //    /// 3: 斜腹杆--18
        //    /// 4: 横梁--5
        //    /// 5：无使用--5
        //    /// 6：斜撑--7
        //    /// 7: 内横梁--6
        //    /// 8：立柱竖杆--23
        //    /// 9：立柱盖梁--9
        //    /// 10：立柱横杆--9
        //    /// 99 :刚臂
        //    /// </remarks>
        //    var MainSection = new TubeSection(1, 1.5, 0.035, true);
        //    var MainWebSection = new TubeSection(2, 0.8, 0.024);
        //    var SubWebSection = new TubeSection(3, 0.8, 0.024);
        //    var s4 = new TubeSection(4, 0.6, 0.016);
        //    var s8 = new TubeSection(8, 0.6, 0.016);
        //    var s10 = new TubeSection(10, 0.4, 0.016);
        //    var s5 = new TubeSection(5, 0.4, 0.016);
        //    var s6 = new TubeSection(6, 0.4, 0.016);
        //    var s7 = new TubeSection(7, 0.4, 0.016);
        //    archModel.AssignProperty(eMemberType.UpperCoord, MainSection);
        //    archModel.AssignProperty(eMemberType.LowerCoord, MainSection);
        //    archModel.AssignProperty(eMemberType.MainWeb, MainWebSection);
        //    archModel.AssignProperty(eMemberType.ColumnWeb, MainWebSection);
        //    archModel.AssignProperty(eMemberType.SubWeb, SubWebSection);
        //    archModel.AssignProperty(eMemberType.CrossCoord, s4);
        //    archModel.AssignProperty(eMemberType.WindBracing, s6);
        //    archModel.AssignProperty(eMemberType.TriWeb, SubWebSection);
        //    archModel.AssignProperty(eMemberType.ColumnMain, s8);
        //    archModel.AssignProperty(eMemberType.ColumnCrossL, s10);
        //    archModel.AssignProperty(eMemberType.ColumnCrossW, s10);
        //    #endregion

        //    #region 3. 切割拱圈
        //    double x0 = -224;
        //    foreach (var dx in new double[] { 0, 21, 21, 21, 21, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 21, 21, 21, 21 })
        //    {
        //        x0 = x0 + dx;
        //        archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
        //    }

        //    var CutAng = 118.2;
        //    var CutX = 240.5;
        //    archModel.AddDatum(0, -CutX, eDatumType.InstallDatum, CutAng);
        //    archModel.AddDatum(0, CutX, eDatumType.InstallDatum, 180 - CutAng);
        //    #endregion

        //    #region 4. 布置主平面，生成骨架
        //    double halfD = 0.75;
        //    for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
        //    {
        //        var CurI = archModel.InstallDatum[i];
        //        var NexI = archModel.InstallDatum[i + 1];
        //        if (CurI.Angle0 == Angle.FromDegrees(90.0))
        //        {
        //            // 起终点为垂直面
        //            if (NexI.Angle0 == Angle.FromDegrees(90))
        //            {
        //                if (NexI.Center.X - CurI.Center.X == 21)
        //                {
        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
        //                        new double[] { 90, 90, 90, 90 },
        //                        new bool[] { false, true, true, true, false }, 0.060);
        //                }
        //                else
        //                {
        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
        //                        new double[] { 90, 90, 90, 90, 90 },
        //                        new bool[] { false, true, true, true, true, false }, 0.060);
        //                }
        //            }
        //            else
        //            {

        //                if (NexI.Center.X == theArchAxis.L1)
        //                {
        //                    continue;
        //                    // 最后一节 241-247-252-259 ,无此情况
        //                    Line2D theCutLineEd = CurI.Line;
        //                    theCutLineEd = theCutLineEd.Offset(-1);
        //                    var cced = theArchAxis.Intersect(theCutLineEd);
        //                    double d1 = cced.X - CurI.Center.X;
        //                    double ll = NexI.Center.X - CurI.Center.X;

        //                    double ang1 = theArchAxis.GetNormalAngle(247).Degrees;
        //                    double ang2 = theArchAxis.GetNormalAngle(252).Degrees;

        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { d1, 6 - d1, 5, 7 },
        //                        new double[] { CurI.Angle0.Degrees, ang1, ang2 },
        //                        new bool[] { false, false, false, false }, 0.060);
        //                }
        //                else
        //                {
        //                    // 倒数第二节
        //                    double X2 = 236.6;
        //                    double A2 = 180 - 95.5144;
        //                    double X1 = 231;

        //                    Line2D theCutLineEd = NexI.Line;
        //                    theCutLineEd = theCutLineEd.Offset(halfD);
        //                    var cced = theArchAxis.Intersect(theCutLineEd);
        //                    double d1 = NexI.Center.X - cced.X;
        //                    double ll = NexI.Center.X - CurI.Center.X;
        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { halfD, 7 - halfD, (X2 - X1), CutX - X2 - d1, d1 },
        //                        new double[] { 90, 90, A2, NexI.Angle0.Degrees },
        //                        new bool[] { false, true, true, false, false }, 0.060);
        //                }
        //                // 终点为正交面

        //            }
        //        }
        //        else
        //        {
        //            if (NexI.Angle0 == Angle.FromDegrees(90))
        //            {
        //                double X1 = -236.6;
        //                double A1 = 95.5144;
        //                double X2 = -231;
        //                // 第二节 -241 -> -224
        //                Line2D theCutLineEd = CurI.Line;
        //                theCutLineEd = theCutLineEd.Offset(-halfD);
        //                var cced = theArchAxis.Intersect(theCutLineEd);
        //                double d1 = cced.X - CurI.Center.X;
        //                double ll = NexI.Center.X - CurI.Center.X;
        //                archModel.CreateInstallSegment(CurI, NexI,
        //                    new double[] { d1, (X1 + CutX) - d1, (X2 - X1), 7 - halfD, halfD },
        //                    new double[] { CurI.Angle0.Degrees, A1, 90, 90 },
        //                    new bool[] { false, false, true, true, false }, 0.060);
        //            }
        //            else
        //            {
        //                if (NexI.Center.X == theArchAxis.L1)
        //                {
        //                    // 最后一节 241-247-252-259
        //                    double X2 = 251.8;
        //                    double X1 = 246.6;
        //                    double A2 = 180 - 128.27;
        //                    double A1 = 180 - 122.47;

        //                    Line2D theCutLineEd = CurI.Line;
        //                    theCutLineEd = theCutLineEd.Offset(-1);
        //                    var cced = theArchAxis.Intersect(theCutLineEd);
        //                    double d1 = cced.X - CurI.Center.X;
        //                    double ll = NexI.Center.X - CurI.Center.X;

        //                    double ang1 = theArchAxis.GetNormalAngle(247).Degrees;
        //                    double ang2 = theArchAxis.GetNormalAngle(252).Degrees;

        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { d1, (X1 - CutX) - d1, X2 - X1, archModel.Axis.L1 - X2 },
        //                        new double[] { CurI.Angle0.Degrees, A1, A2 },
        //                        new bool[] { false, false, false, false }, 0.060);
        //                }
        //                else
        //                {
        //                    // 第一节     241-247-252-259
        //                    // -259:-251.5:-246
        //                    // 132:128.3
        //                    double X1 = -251.8;
        //                    double X2 = -246.6;
        //                    double A1 = 128.27;
        //                    double A2 = 122.47;
        //                    Line2D theCutLineEd = NexI.Line;
        //                    theCutLineEd = theCutLineEd.Offset(halfD);
        //                    var cced = theArchAxis.Intersect(theCutLineEd);
        //                    double d1 = NexI.Center.X - cced.X;
        //                    double ll = NexI.Center.X - CurI.Center.X;

        //                    double ang1 = theArchAxis.GetNormalAngle(-252).Degrees;
        //                    double ang2 = theArchAxis.GetNormalAngle(-247).Degrees;

        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { X1 + archModel.Axis.L1, X2 - X1, (-CutX - X2) - d1, d1 },
        //                        new double[] { A1, A2, NexI.Angle0.Degrees },
        //                        new bool[] { false, false, false, false }, 0.060);
        //                }
        //            }
        //        }
        //    }

        //    archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
        //    archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

        //    archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
        //    archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
        //    archModel.GenerateSkeleton();
        //    #endregion

        //    #region 6. 生成模型
        //    archModel.GenerateArch();
        //    // 6.1 增加三角斜腹杆
        //    int num = archModel.MainDatum.Count;
        //    archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
        //    archModel.AddTriWeb(archModel.GetMainDatum(2), archModel.GetMainDatum(3), e, 0.25);
        //    archModel.AddTriWeb(archModel.GetMainDatum(3), archModel.GetMainDatum(4), e, 0.25);
        //    archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
        //    archModel.AddTriWeb(archModel.GetMainDatum(num - 3), archModel.GetMainDatum(num - 4), e, 0.25);
        //    archModel.AddTriWeb(archModel.GetMainDatum(num - 4), archModel.GetMainDatum(num - 5), e, 0.25);
        //    #endregion

        //    #region 7. 立柱建模
        //    double xx = -231;
        //    double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };

        //    //double[] H2S = new double[]
        //    //{
        //    //    1414.012000,1414.348000,1414.679775,1414.944375,1415.120775,1415.208975,
        //    //    1415.208975,1415.120775,1414.944375,1414.679775,1414.348000,1414.012000
        //    //};
        //    double[] H2S = new double[]
        //    {
        //        1403.714000,1403.966000,1404.218000,1404.464375,1404.640775,1404.728975,
        //        1404.728975,1404.640775,1404.464375,1404.218000,1403.966000,1403.714000,
        //    };


        //    for (int i = 0; i < 12; i++)
        //    {
        //        var xi = xx + i * 42;
        //        archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
        //    }
        //    archModel.GenerateColumn();
        //    #endregion

        //    #region 8. 交界墩
        //    double P2H2 = 1413.676000;
        //    double RtZ0 = -106;
        //    double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
        //    double wratio = 0.0125;
        //    RectSection S1 = new RectSection(0, 6, 3);
        //    RectSection S2 = new RectSection(0, 7, 3);
        //    RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));

        //    archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
        //    archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
        //    #endregion

        //    return archModel;


        //}
        //public static Arch PreliminaryDesignModelV2(out ArchAxis theArchAxis)
        //{
        //    Arch archModel;

        //    #region 基本步骤
        //    double L = 518.0;
        //    double m = 2.0;
        //    double f = L / 4.0;
        //    double e = 0.060;
        //    #endregion

        //    #region 1. 设置拱系
        //    theArchAxis = new ArchAxis(f, m, L);
        //    archModel = new Arch(theArchAxis, 8.5, 17, 14, 4);
        //    archModel.SetFootLevel(1270 + 11.3);
        //    #endregion

        //    #region  2. 配置截面
        //    /// <remarks>
        //    /// 1:拱肋截面--15
        //    /// 2: 竖腹杆--20
        //    /// 3: 斜腹杆--18
        //    /// 4: 横梁--5
        //    /// 5：无使用--5
        //    /// 6：斜撑--7
        //    /// 7: 内横梁--6
        //    /// 8：立柱竖杆--23
        //    /// 9：立柱盖梁--9
        //    /// 10：立柱横杆--9
        //    /// 99 :刚臂
        //    /// </remarks>
        //    var MainSection = new TubeSection(1, 1.5, 0.035, true);
        //    var MainWebSection = new TubeSection(2, 0.8, 0.024);
        //    var SubWebSection = new TubeSection(3, 0.8, 0.024);
        //    var s4 = new TubeSection(4, 0.6, 0.016);
        //    var s8 = new TubeSection(8, 0.6, 0.016);
        //    var s10 = new TubeSection(10, 0.4, 0.016);
        //    var s5 = new TubeSection(5, 0.4, 0.016);
        //    var s6 = new TubeSection(6, 0.4, 0.016);
        //    var s7 = new TubeSection(7, 0.4, 0.016);
        //    archModel.AssignProperty(eMemberType.UpperCoord, MainSection);
        //    archModel.AssignProperty(eMemberType.LowerCoord, MainSection);
        //    archModel.AssignProperty(eMemberType.MainWeb, MainWebSection);
        //    archModel.AssignProperty(eMemberType.ColumnWeb, MainWebSection);
        //    archModel.AssignProperty(eMemberType.SubWeb, SubWebSection);
        //    archModel.AssignProperty(eMemberType.CrossCoord, s4);
        //    archModel.AssignProperty(eMemberType.WindBracing, s6);
        //    archModel.AssignProperty(eMemberType.TriWeb, SubWebSection);
        //    archModel.AssignProperty(eMemberType.ColumnMain, s8);
        //    archModel.AssignProperty(eMemberType.ColumnCrossL, s10);
        //    archModel.AssignProperty(eMemberType.ColumnCrossW, s10);
        //    #endregion

        //    #region 3. 切割拱圈
        //    double x0 = -238;
        //    foreach (var dx in new double[] { 0, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 28, 28, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, })
        //    {
        //        x0 = x0 + dx;
        //        archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
        //    }

        //    //var CutAng = 118.2;
        //    //var CutX = 240.5;
        //    //archModel.AddDatum(0, -CutX, eDatumType.InstallDatum, CutAng);
        //    //archModel.AddDatum(0, CutX, eDatumType.InstallDatum, 180 - CutAng);
        //    #endregion

        //    #region 4. 布置主平面，生成骨架
        //    double halfD = 0.75;
        //    for (int i = 0; i < archModel.InstallDatum.Count - 1; i++)
        //    {
        //        var CurI = archModel.InstallDatum[i];
        //        var NexI = archModel.InstallDatum[i + 1];
        //        if (CurI.Angle0 == Angle.FromDegrees(90.0))
        //        {
        //            // 起终点为垂直面
        //            if (NexI.Angle0 == Angle.FromDegrees(90))
        //            {
        //                if (NexI.Center.X - CurI.Center.X == 21)
        //                {
        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { halfD, 7 - halfD, 7, 7 - halfD, halfD },
        //                        new double[] { 90, 90, 90, 90 },
        //                        new bool[] { false, true, true, true, false }, 0.060);
        //                }
        //                else
        //                {
        //                    archModel.CreateInstallSegment(CurI, NexI,
        //                        new double[] { halfD, 7 - halfD, 7, 7, 7 - halfD, halfD },
        //                        new double[] { 90, 90, 90, 90, 90 },
        //                        new bool[] { false, true, true, true, true, false }, 0.060);
        //                }
        //            }
        //            else
        //            {
        //                // 最后一节 241-247-252-259 ,无此情况
        //                Line2D theCutLineEd = CurI.Line;
        //                theCutLineEd = theCutLineEd.Offset(-1);
        //                var cced = theArchAxis.Intersect(theCutLineEd);
        //                double d1 = cced.X - CurI.Center.X;
        //                double ll = NexI.Center.X - CurI.Center.X;

        //                double ang1 = theArchAxis.GetNormalAngle(247).Degrees;
        //                double ang2 = theArchAxis.GetNormalAngle(252).Degrees;

        //                archModel.CreateInstallSegment(CurI, NexI,
        //                    new double[] { d1, NexI.Center.X - CurI.Center.X - d1 },
        //                    new double[] { CurI.Angle0.Degrees },
        //                    new bool[] { false, false }, 0.060);


        //            }
        //        }
        //        else
        //        {
        //            // 第一节     241-247-252-259
        //            // -259:-251.5:-246
        //            // 132:128.3

        //            Line2D theCutLineEd = NexI.Line;
        //            theCutLineEd = theCutLineEd.Offset(1);
        //            var cced = theArchAxis.Intersect(theCutLineEd);
        //            double d1 = NexI.Center.X - cced.X;
        //            double ll = NexI.Center.X - CurI.Center.X;

        //            archModel.CreateInstallSegment(CurI, NexI,
        //                new double[] { ll - d1, d1 },
        //                new double[] { NexI.Angle0.Degrees },
        //                new bool[] { false, false }, 0.060);


        //        }
        //    }

        //    archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
        //    archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);

        //    archModel.AddDatum(0, -theArchAxis.L1 - 2, eDatumType.ControlDatum);
        //    archModel.AddDatum(0, theArchAxis.L1 + 2, eDatumType.ControlDatum);
        //    archModel.GenerateSkeleton();
        //    #endregion

        //    #region 6. 生成模型
        //    archModel.GenerateArch();
        //    // 6.1 增加三角斜腹杆
        //    int num = archModel.MainDatum.Count;
        //    archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
        //    archModel.AddTriWeb(archModel.GetMainDatum(2), archModel.GetMainDatum(3), e, 0.25);
        //    archModel.AddTriWeb(archModel.GetMainDatum(3), archModel.GetMainDatum(4), e, 0.25);
        //    archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num - 3), e);
        //    archModel.AddTriWeb(archModel.GetMainDatum(num - 3), archModel.GetMainDatum(num - 4), e, 0.25);
        //    archModel.AddTriWeb(archModel.GetMainDatum(num - 4), archModel.GetMainDatum(num - 5), e, 0.25);
        //    #endregion

        //    #region 7. 立柱建模
        //    double xx = -231;
        //    double[] Ls = new double[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4 };

        //    //double[] H2S = new double[]
        //    //{
        //    //    1414.012000,1414.348000,1414.679775,1414.944375,1415.120775,1415.208975,
        //    //    1415.208975,1415.120775,1414.944375,1414.679775,1414.348000,1414.012000
        //    //};
        //    double[] H2S = new double[]
        //    {
        //        1403.714000,1403.966000,1404.218000,1404.464375,1404.640775,1404.728975,
        //        1404.728975,1404.640775,1404.464375,1404.218000,1403.966000,1403.714000,
        //    };


        //    for (int i = 0; i < 12; i++)
        //    {
        //        var xi = xx + i * 42;
        //        archModel.AddColumn(0, xi, H2S[i] - archModel.FootLevel, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i] + 1.5, 0.8);
        //    }
        //    archModel.GenerateColumn();
        //    #endregion

        //    #region 8. 交界墩
        //    double P2H2 = 1413.676000;
        //    double RtZ0 = -106;
        //    double RtZ1 = -archModel.Axis.f + (P2H2 - archModel.FootLevel) - 2;
        //    double wratio = 0.0125;
        //    RectSection S1 = new RectSection(0, 6, 3);
        //    RectSection S2 = new RectSection(0, 7, 3);
        //    RectSection S0 = new RectSection(0, 6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));

        //    archModel.AddColumn(0, -273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
        //    archModel.AddColumn(0, 273, new RCColumn(0, RtZ0, RtZ1, RtZ1 + 2, S0, S1, S2));
        //    #endregion

        //    return archModel;


        //}



        /// <summary>
        /// 等距分隔x至
        /// </summary>
        /// <param name="ax"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static List<double> EquallyLengthCut(ref ArchAxis ax, DatumPlane p1, DatumPlane p2, int step)
        {
            var x1 = ax.Intersect(p1.Line).X;
            var x2 = ax.Intersect(p2.Line).X;
            if (x1 > x2)
            {
                throw new Exception();
            }
            var L1 = ax.GetLength(x1);
            var L2 = ax.GetLength(x2);
            var Length = L2 - L1;

            var dL = Length / step;

            List<double> res = new List<double>();

            for (int i = 0; i < step - 1; i++)
            {
                res.Add(ax.GetX0(L1 + (i + 1) * dL));
            }

            return res;
        }
    }
}
