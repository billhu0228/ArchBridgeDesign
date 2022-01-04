using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

namespace Test
{
    class Program
    {
        static void Main(string[] args) 
        {
            ArchAxis ax;
            Arch theArchModel;
            theArchModel = Arch.PreliminaryDesignModelV2(out ax);
            // 写出基准面
            string name = "PreliminaryDesignModel";
            theArchModel.WriteMember(string.Format("{0}-member.lsp", name));
            return;

            //GenerateModel();
            //return;


            if (args.Count()!=0)
            {
                double f = double.Parse(args[0]);
                double m = double.Parse(args[1]);
                ModelT1(f, m, "Output");
            }
            else
            {
                ModelT1(4.0, 1.85, "Test");
            }
        }



        public static void PreDesignModel()
        {
            ArchAxis theArchAxis;
            Arch archModel;
            double L = 518.0;
            double m = 2.0;
            double f = L / 4.5;
            double e = 0.060;
            // 1. 设置拱系
            theArchAxis = new ArchAxis(f, m, L);
            archModel = new Arch(theArchAxis, 8.5, 17, 14, 4);
            // 2. 配置截面
            var MainSection = new TubeSection(1.5, 0.035);
            var WebSection = new TubeSection(0.9, 0.024);
            var s2 = new TubeSection(0.6, 0.016);
            var s3 = new TubeSection(0.4, 0.016);
            archModel.AssignProperty(eMemberType.UpperCoord, MainSection);
            archModel.AssignProperty(eMemberType.LowerCoord, MainSection);
            archModel.AssignProperty(eMemberType.VerticalWeb, WebSection);
            archModel.AssignProperty(eMemberType.ColumnWeb, WebSection);
            archModel.AssignProperty(eMemberType.InclineWeb, WebSection);
            archModel.AssignProperty(eMemberType.CrossBraceing, new TubeSection(0.7, 0.016));
            archModel.AssignProperty(eMemberType.WebBracing, new HSection(0.3, 0.3, 0.3, 0.012, 0.012, 0.008));
            archModel.AssignProperty(eMemberType.InclineWebS, WebSection);
            archModel.AssignProperty(eMemberType.ColumnMain, s2);
            archModel.AssignProperty(eMemberType.ColumnCrossL, s3);
            archModel.AssignProperty(eMemberType.ColumnCrossW, s3);
            //3. 切割拱圈
            double x0 = -224;
            foreach (var dx in new  double[]{0,21,21,21,21,28,28, 28, 28, 28, 28, 28, 28, 28, 28, 21, 21, 21, 21 } )
            {
                x0 = x0 + dx;
                archModel.AddDatum(0, x0, eDatumType.InstallDatum, 90);
            }
            
            archModel.AddDatum(0, -243, eDatumType.InstallDatum,114);
            archModel.AddDatum(0, 243, eDatumType.InstallDatum,66);

            // 4. 布置主平面，生成骨架
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
                                new double[] { 1.15, 7 - 1.15, 7, 7 - 1.15, 1.15 },
                                new double[] { 90, 90,  90, 90 },
                                new bool[] { false,true, true, true,false }, 0.060);
                        }
                        else
                        {
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { 1.15, 7 - 1.15, 7,7, 7 - 1.15, 1.15 },
                                new double[] { 90, 90, 90, 90, 90 },
                                new bool[] { false,  true, true, true, true, false }, 0.060);
                        }
                    }
                    else
                    {          
                        if (NexI.Center.X==theArchAxis.L1)
                        {
                            // 最后一节
                            Line2D theCutLineEd = CurI.Line;
                            theCutLineEd = theCutLineEd.Offset(-1);
                            var cced = theArchAxis.Intersect(theCutLineEd);
                            double d1 =cced.X-CurI.Center.X;
                            double ll = NexI.Center.X - CurI.Center.X;

                            double ang = theArchAxis.GetNormalAngle(251.5).Degrees;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, 251.5-CurI.Center.X-d1,theArchAxis.L1-251.5 },
                                new double[] { CurI.Angle0.Degrees,ang},
                                new bool[] { false, false, false }, 0.060);
                            continue;
                        }
                        else
                        {
                            // 倒数第二节
                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(1);
                            var cced = theArchAxis.Intersect(theCutLineEd);
                            double d1 = NexI.Center.X - cced.X;
                            double ll = NexI.Center.X - CurI.Center.X;
                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { 1.15,7-1.15, 7,ll-14 - d1, d1 },
                                new double[] { 90,90,90, NexI.Angle0.Degrees },
                                new bool[] { false, true,true, false, false }, 0.060);
                        }
                        // 终点为正交面

                    }
                }
                else
                {           
                    if (NexI.Angle0 == Angle.FromDegrees(90)) 
                    {
                        // 第二节
                        Line2D theCutLineEd = CurI.Line;
                        theCutLineEd = theCutLineEd.Offset(-1);
                        var cced = theArchAxis.Intersect(theCutLineEd);
                        double d1 =  cced.X-CurI.Center.X;
                        double ll = NexI.Center.X - CurI.Center.X;
                        archModel.CreateInstallSegment(CurI, NexI,
                            new double[] {   d1 , ll - 14 - d1, 7 , 7 - 1.15, 1.15 },
                            new double[] {CurI.Angle0.Degrees, 90, 90, 90  },
                            new bool[] { false, false, true, true, false }, 0.060);
                    }
                    else
                    {
                        if (NexI.Center.X == theArchAxis.L1)
                        {
                            // 最后一节
                            Line2D theCutLineEd = CurI.Line;
                            theCutLineEd = theCutLineEd.Offset(-1);
                            var cced = theArchAxis.Intersect(theCutLineEd);
                            double d1 = cced.X - CurI.Center.X;
                            double ll = NexI.Center.X - CurI.Center.X;

                            double ang = theArchAxis.GetNormalAngle(251.5).Degrees;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { d1, 251.5 - CurI.Center.X - d1, theArchAxis.L1 - 251.5 },
                                new double[] { CurI.Angle0.Degrees, ang },
                                new bool[] { false, false, false }, 0.060);
                        }
                        else
                        {
                            // 第一节
                            Line2D theCutLineEd = NexI.Line;
                            theCutLineEd = theCutLineEd.Offset(1);
                            var cced = theArchAxis.Intersect(theCutLineEd);
                            double d1 = NexI.Center.X- cced.X;
                            double ll = NexI.Center.X - CurI.Center.X;

                            double ang = theArchAxis.GetNormalAngle(-251.5).Degrees;

                            archModel.CreateInstallSegment(CurI, NexI,
                                new double[] { theArchAxis.L1 - 251.5, NexI.Center.X+ 251.5 - d1, d1 },
                                new double[] {  ang , NexI.Angle0.Degrees },
                                new bool[] { false, false, false }, 0.060);
                        }
                    }
                }
            }

            archModel.AddDatum(0, -theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1, eDatumType.ControlDatum);
            archModel.AddDatum(0, 0, eDatumType.ControlDatum);
            archModel.AddDatum(0, -theArchAxis.L1-2, eDatumType.ControlDatum);
            archModel.AddDatum(0, theArchAxis.L1+2, eDatumType.ControlDatum);

            archModel.GenerateSkeleton();

            // 6. 生成模型
            archModel.GenerateArch();
            // 6.1 增加三角斜腹杆
            int num = archModel.MainDatum.Count;
            archModel.AddTriWeb(archModel.GetMainDatum(1), archModel.GetMainDatum(2), e);
            archModel.AddTriWeb(archModel.GetMainDatum(2), archModel.GetMainDatum(3), e,0.25);
            archModel.AddTriWeb(archModel.GetMainDatum(num - 2), archModel.GetMainDatum(num-3), e);
            archModel.AddTriWeb(archModel.GetMainDatum(num-3), archModel.GetMainDatum(num-4), e,0.25);

            // 7. 立柱建模
            double h0 = 13;
            var relH = archModel.Axis.f + h0;
            double xx = -231;
            double[] Ls = new double[] { 4, 3, 3, 2, 2, 2, 2, 2, 2, 3, 3, 4 };
            for (int i = 0; i < 12; i++)
            {
                var xi = xx + i * 42;
                archModel.AddColumn(0, xi, relH, Ls[i], 2.8, 3.0, 3, 1, 1, Ls[i]+1.5, 0.8);
            }
            archModel.GenerateColumn();

            // 8. 交界墩
            double RtZ0 = -106;
            double RtZ1 = 9;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(6, 3);
            RectSection S2 = new RectSection(7, 3);
            RectSection S0 = new RectSection(6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));

            archModel.AddColumn(0, -273, new RCColumn(0, -106, 9, 11, S0, S1, S2));
            archModel.AddColumn(0, 273, new RCColumn(0, -106, 9, 11, S0, S1, S2));

            // 写出基准面
            string name = "tst";
            archModel.WriteDiagonalDatum(string.Format("{0}-dg.lsp", name));
            archModel.WriteMainDatum(string.Format("{0}-mg.lsp", name));
            archModel.WriteMember(string.Format("{0}-member.lsp", name));

            return;
        }




        public static void GenerateModel()
        {
            ArchAxis theArchAxis;
            Arch archModel;
            double L = 518.0;
            double m = 2.2;
            double f = L / 4.5;
            #region 基本步骤

            // 1. 设置拱系
            theArchAxis = new ArchAxis(L / 4.5, m, L);
            archModel = new Arch(theArchAxis, 8.0, 16.5, 12, 5);
            // 2. 生成桁架
            archModel.GenerateTruss(10, 49, 7, 6, 7, 2);
            archModel.GenerateMiddleDatum(); //生成中插平面
            // 补充
            archModel.AddDatum(0, -L * 0.5, eDatumType.ControlDatum);
            archModel.AddDatum(0, L * 0.5, eDatumType.ControlDatum);

            foreach (var dxx in new double[] { 6, 12.5, 19 })
            {

                archModel.AddDatum(0, -0.5 * L + dxx, eDatumType.NormalDatum);
                archModel.AddDatum(0, 0.5 * L - dxx, eDatumType.NormalDatum);
            }


            var dx = 1.0 / Math.Cos(archModel.Axis.GetAngle(-L * 0.5).Radians);
            archModel.AddDatum(0, -L * 0.5 - dx, eDatumType.ControlDatum);
            archModel.AddDatum(0, +L * 0.5 + dx, eDatumType.ControlDatum);


            // 3. 配置截面
            var s1 = new TubeSection(1.4, 0.035);
            archModel.AssignProperty(eMemberType.UpperCoord, s1);
            archModel.AssignProperty(eMemberType.LowerCoord, s1);
            archModel.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.CrossBraceing, new TubeSection(0.7, 0.016));
            archModel.AssignProperty(eMemberType.WebBracing, new HSection(0.3, 0.3, 0.3, 0.012, 0.012, 0.008));
            archModel.AssignProperty(eMemberType.InclineWebS, new TubeSection(0.3, 0.008));
            // 4. 生成上下弦骨架
            archModel.GenerateSkeleton();
            // 5. 生成斜杆基准面
            archModel.GenerateDiagonalDatum(0.060);
            // 6. 生成模型
            archModel.GenerateArch();

            // 6.1 增加三角斜腹杆
            //archModel.AddTriWeb(-0.5 * L , archModel.GetDatum(-0.5 * L + 6,eDatumType.NormalDatum), 0.06);
            //archModel.AddTriWeb(-0.5 * L + 6, archModel.GetDatum(-0.5 * L + 12.5,eDatumType.NormalDatum), 0.06);
            //archModel.AddTriWeb(-0.5 * L + 12.5, archModel.GetDatum(-0.5 * L + 19, eDatumType.NormalDatum), 0.06);

            //archModel.AddTriWeb(0.5 * L, archModel.GetDatum(0.5 * L - 6, eDatumType.NormalDatum), 0.06);
            //archModel.AddTriWeb(0.5 * L - 6, archModel.GetDatum(0.5 * L - 12.5, eDatumType.NormalDatum), 0.06);
            //archModel.AddTriWeb(0.5 * L - 12.5, archModel.GetDatum(0.5 * L - 19, eDatumType.NormalDatum), 0.06);

            // 7. 立柱建模
            double h0 = 15.5;
            foreach (var item in archModel.MainDatum)
            {
                if (item.DatumType == eDatumType.ColumnDatum)
                {
                    var x = item.Center.X;
                    var relH = archModel.Axis.f + h0;

                    archModel.AddColumn(0, x, relH, 1.6, 2.8, 3.0, 3, 1, 1, 2.7, 0.5);
                }
            }
            var s2 = new TubeSection(0.6, 0.016);
            var s3 = new TubeSection(0.4, 0.016);

            archModel.AssignProperty(eMemberType.ColumnMain, s2);
            archModel.AssignProperty(eMemberType.ColumnCrossL, s3);
            archModel.AssignProperty(eMemberType.ColumnCrossW, s3);

            archModel.GenerateColumn();
            // 8. 交界墩
            double RtZ0 = -106;
            double RtZ1 = 9;
            double wratio = 0.0125;
            RectSection S1 = new RectSection(6, 3);
            RectSection S2 = new RectSection(7, 3);
            RectSection S0 = new RectSection(6 + 2 * wratio * (RtZ1 - RtZ0), 3 + 2 * wratio * (RtZ1 - RtZ0));

            archModel.AddColumn(0, -270.5, new RCColumn(0, -106, 9, 11, S0, S1, S2));
            archModel.AddColumn(0, 270.5, new RCColumn(0, -106, 9, 11, S0, S1, S2));
            #endregion


            // 写出基准面
            string name = "tst";
            archModel.WriteDiagonalDatum(string.Format("{0}-dg.lsp", name));
            archModel.WriteMainDatum(string.Format("{0}-mg.lsp", name));
            archModel.WriteMember(string.Format("{0}-member.lsp", name));
        }


        static void ModelT1(double f,double m,string name)
        {
            #region 基本建模步骤
            // 1. 设置拱系
            ArchAxis theArchAxis = new ArchAxis(518 / f, m, 518);
            Arch archModel = new Arch(theArchAxis, 8, 16.5, 12, 4,2,1260);
            // 2. 生成桁架
            archModel.GenerateTruss(10, 50, 10, 7.8, 11, 2);
            archModel.GenerateMiddleDatum(); //生成中插平面
            // 2.1补充
            archModel.AddDatum(0, -518 * 0.5, eDatumType.ControlDatum);
            archModel.AddDatum(0, 518 * 0.5, eDatumType.ControlDatum);
            archModel.AddDatum(0, -252.2, eDatumType.NormalDatum);
            archModel.AddDatum(0, 252.2, eDatumType.NormalDatum);
            var dx = 1.0 / Math.Cos(archModel.Axis.GetAngle(-518 * 0.5).Radians);
            archModel.AddDatum(0, -518 * 0.5 - dx, eDatumType.ControlDatum);
            archModel.AddDatum(0, +518 * 0.5 + dx, eDatumType.ControlDatum);
            // 3. 配置截面
            var s1 = new TubeSection(1.4, 0.035);
            archModel.AssignProperty(eMemberType.UpperCoord, s1);
            archModel.AssignProperty(eMemberType.LowerCoord, s1);
            archModel.AssignProperty(eMemberType.VerticalWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.ColumnWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.InclineWeb, new TubeSection(0.9, 0.024));
            archModel.AssignProperty(eMemberType.CrossBraceing, new TubeSection(0.7, 0.016));
            // 4. **生成上下弦骨架**
            archModel.GenerateSkeleton();
            // 5. 生成斜杆基准面
            archModel.GenerateDiagonalDatum(0.060);
            // 6. 整理主拱模型
            archModel.GenerateArch();

            // 7. 立柱建模                        
            archModel.AddColumn(0, -225.0, 105.2, 1.6,2.8, 3.0, 3, 1, 1, 2.7, 0.5);
            var s2 = new TubeSection(0.6, 0.016);
            archModel.AssignProperty(eMemberType.ColumnMain, s2);
            // 7.1 整理立柱模型
            archModel.GenerateColumn();
            #endregion


            // 写出基准面
            archModel.WriteDiagonalDatum(string.Format("{0}-dg.lsp",name));
            archModel.WriteMainDatum(string.Format("{0}-mg.lsp", name));
            archModel.WriteMember(string.Format("{0}-member.lsp", name));
        }
    }
}
