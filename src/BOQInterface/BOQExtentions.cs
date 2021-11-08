#define KNo
using MathNet.Spatial.Euclidean;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BOQInterface
{
    public static class BOQExtentions
    {
        public static void CastBOQTable(this Arch theArch,ref BOQTable outTable)
        {


            #region 拱肋
            for (int i = 0; i < theArch.UpSkeleton.Count-1; i++)
            {
                Point2D st = theArch.UpSkeleton[i];
                Point2D ed = theArch.UpSkeleton[i+1];
                double L = ed.DistanceTo(st);
                Point2D mid = st + (ed - st).Normalize() * L * 0.5;
                double T;
                if (Math.Abs(mid.X)<=56)
                {
                    T = 0.032;
                }
                else if (Math.Abs(mid.X) <= 220)
                {
                    T = 0.028;
                }
                else
                {
                    T = 0.036;
                }
                double As = Math.PI * (Math.Pow(0.75, 2) - Math.Pow(0.75 - T, 2));
                double Ac = Math.PI * (Math.Pow(0.75 - T, 2));
                outTable.AddItem(eMemberType.UpperCoord, eMaterial.Q420D_T, 4,L*As*7.85);
                outTable.AddItem(eMemberType.UpperCoord, eMaterial.C80SCC, 4,L*Ac*1.0);
            }

            for (int i = 0; i < theArch.LowSkeleton.Count - 1; i++)
            {
                Point2D st = theArch.LowSkeleton[i];
                Point2D ed = theArch.LowSkeleton[i + 1];
                double L = ed.DistanceTo(st);
                Point2D mid = st + (ed - st).Normalize() * L * 0.5;
                double T;
                if (Math.Abs(mid.X) <= 196)
                {
                    T = 0.028;
                }
                else
                {
                    T = 0.036;
                }
                double As = Math.PI * (Math.Pow(0.75, 2) - Math.Pow(0.75 - T, 2));
                double Ac = Math.PI * (Math.Pow(0.75 - T, 2));
                outTable.AddItem(eMemberType.LowerCoord, eMaterial.Q420D_T, 4, L * As * 7.85);
                outTable.AddItem(eMemberType.LowerCoord, eMaterial.C80SCC, 4, L * Ac * 1.0);
            }


            #endregion

            #region 腹杆
            foreach (var item in theArch.MemberTable)
            {
                if (item.ElemType==eMemberType.InclineWeb || 
                    item.ElemType == eMemberType.InclineWebS|| 
                    item.ElemType == eMemberType.VerticalWeb)
                {
                    double L = item.Line.Length;
                    double T;
                    double D;
                    if (Math.Abs(item.Line.MiddlePoint().X)<=112)
                    {
                        D = 0.8;
                        T = 0.016;
                    }
                    else if (Math.Abs(item.Line.MiddlePoint().X) <= 220.5)
                    {
                        D = 0.8;
                        T = 0.020;
                    }
                    else
                    {
                        D = 0.9;
                        T = 0.020;
                    }
                    double As = Math.PI * (Math.Pow(D*0.5, 2) - Math.Pow(D*0.5 - T, 2));
                    outTable.AddItem(item.ElemType, eMaterial.Q345D_T, 4, L * As * 7.85);
                }

            }
            #endregion

            #region 横联
            foreach (var item in theArch.MainDatum)
            {
                if (item.DatumType==eDatumType.ControlDatum)
                {
                    continue;
                }
                double H = theArch.Get3PointReal(item)[0].Y - theArch.Get3PointReal(item)[2].Y;
                Point3D P1 = new Point3D(theArch.Get3PointReal(item)[0].X, -theArch.WidthInside * 0.5 - 0.5 * theArch.WidthOutside, theArch.Get3PointReal(item)[0].Y);
                Point3D P12= new Point3D(theArch.Get3PointReal(item)[0].X, -theArch.WidthInside * 0.5, theArch.Get3PointReal(item)[0].Y);
                Point3D P2 = new Point3D(theArch.Get3PointReal(item)[1].X, -theArch.WidthInside * 0.5 , theArch.Get3PointReal(item)[1].Y);
                Point3D P23 = new Point3D(theArch.Get3PointReal(item)[2].X, -theArch.WidthInside * 0.5, theArch.Get3PointReal(item)[2].Y);
                Point3D P3 = new Point3D(theArch.Get3PointReal(item)[2].X, -theArch.WidthInside * 0.5 - 0.5 * theArch.WidthOutside, theArch.Get3PointReal(item)[2].Y);
                Point3D P34 = new Point3D(theArch.Get3PointReal(item)[2].X, -theArch.WidthInside * 0.5 - 1.0 * theArch.WidthOutside, theArch.Get3PointReal(item)[2].Y);
                Point3D P4 = new Point3D(theArch.Get3PointReal(item)[1].X, -theArch.WidthInside * 0.5 -1.0*theArch.WidthOutside, theArch.Get3PointReal(item)[1].Y);
                Point3D P41 = new Point3D(theArch.Get3PointReal(item)[0].X, -theArch.WidthInside * 0.5 - 1.0 * theArch.WidthOutside, theArch.Get3PointReal(item)[0].Y);
                double A1 = TubeSection.GetAs(0.273, 0.010);
                double A2 = TubeSection.GetAs(0.508, 0.010);
                if (H<11.0)
                {
                    outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 8, A1 * P1.DistanceTo(P2) * 7.85);
                    outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 2, A1 * P2.DistanceTo(P4) * 7.85);
                }
                else if (H<13.0)
                {
                    outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 8, A1 * P1.DistanceTo(P2+ (P12-P2).ScaleBy(0.33333333333333)) * 7.85);
                    outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 4, A1 * P2.DistanceTo(P4) * 7.85);
                } 
                else
                {
                    outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 16, A1 * P1.DistanceTo(P2 + (P12 - P2).ScaleBy(0.5)) * 7.85);
                    outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 4, A1 * P2.DistanceTo(P4) * 7.85);

                }
                var xlist = (from it in theArch.ColumnList select it.X).ToList();
                if (xlist.Contains(  item.Center.X))
                {

                    ;
                    outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 2, A2 * H * 7.85);
                    if (H<13.0)
                    {
                        outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 2, A2 * Math.Sqrt(H*H+theArch.WidthInside*theArch.WidthInside)* 7.85);
                    }
                    else
                    {
                        outTable.AddItem(eMemberType.WebBracing, eMaterial.Q345D_T, 4, A2 * Math.Sqrt(H * H*0.25 + theArch.WidthInside * theArch.WidthInside) * 7.85);
                    }
                }



#if DEBUGPrtWebBracing
                string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",P1.X, P1.Y, P1.Z, P2.X, P2.Y, P2.Z);
                Debug.WriteLine(lsp);
                lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",P2.X, P2.Y, P2.Z, P3.X, P3.Y, P3.Z);
                Debug.WriteLine(lsp);
                lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",P3.X, P3.Y, P3.Z, P4.X, P4.Y, P4.Z);
                Debug.WriteLine(lsp);
                lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",P4.X, P4.Y, P4.Z, P1.X, P1.Y, P1.Z);
                Debug.WriteLine(lsp);
                lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",P2.X, P2.Y, P2.Z, P4.X, P4.Y, P4.Z);
                Debug.WriteLine(lsp);
#endif
            }
            #endregion

            #region 下平联
            List<DatumPlane> carry = new List<DatumPlane>();
            List<int> idxofcarry = new List<int>();
            foreach (var item in theArch.MainDatum)
            {
                if (item.DatumType == eDatumType.ColumnDatum || item.DatumType == eDatumType.VerticalDatum || item.DatumType == eDatumType.NormalDatum)
                {
                    if (Math.Abs(item.Center.X - (-236.6)) < 0.1)
                    {
                        continue;
                    }           
                    if (Math.Abs(item.Center.X - (236.6)) < 0.1)
                    {
                        continue;
                    }
                    carry.Add(item);
                }
            }
            carry.Sort(new DatumPlane());
            carry.Reverse();
            int WindBraceCount = 0;
            for (int i = 0; i < carry.Count; i++)
            {
                double As = TubeSection.GetAs(0.8, 0.016);
                Point3D P1 = new Point3D(theArch.Get3PointReal(carry[i])[2].X, 0.5 * theArch.WidthInside + theArch.WidthOutside, theArch.Get3PointReal(carry[i])[2].Y);
                Point3D P2 = new Point3D(theArch.Get3PointReal(carry[i])[2].X, 0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i])[2].Y);
                Point3D P3 = new Point3D(theArch.Get3PointReal(carry[i])[2].X, -0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i])[2].Y);
                Point3D P4 = new Point3D(theArch.Get3PointReal(carry[i])[2].X, -0.5 * theArch.WidthInside - theArch.WidthOutside, theArch.Get3PointReal(carry[i])[2].Y);

#if DEBUGPrtWindBraceingV
                string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")", P1.X, P1.Y, P1.Z, P2.X, P2.Y, P2.Z);
                Debug.WriteLine(lsp);
                lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")", P3.X, P3.Y, P3.Z, P4.X, P4.Y, P4.Z);
                Debug.WriteLine(lsp);
#endif
                outTable.AddItem(eMemberType.WindBraceingV, eMaterial.Q345D_T, 2, theArch.WidthOutside * As * 7.85);

                if (i != 0 && Math.Abs(theArch.Get3PointReal(carry[i - 1])[0].X - theArch.Get3PointReal(carry[i])[0].X) > 2)
                {
                    // 平联
                    if (WindBraceCount % 2 == 0)
                    {
                        P1 = new Point3D(theArch.Get3PointReal(carry[i - 1])[2].X, 0, theArch.Get3PointReal(carry[i - 1])[2].Y);
                        P2 = new Point3D(theArch.Get3PointReal(carry[i])[2].X, -0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i])[2].Y);
                        P3 = new Point3D(theArch.Get3PointReal(carry[i])[2].X, 0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i])[2].Y);
#if DEBUGPrtPL
                        string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P1.X, P1.Y, P1.Z, P2.X, P2.Y, P2.Z);
                        Debug.WriteLine(lsp);
                        lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P1.X, P1.Y, P1.Z, P3.X, P3.Y, P3.Z);
                        Debug.WriteLine(lsp);
#endif
                        outTable.AddItem(eMemberType.WindBraceingH, eMaterial.Q345D_T, 2, TubeSection.GetAs(0.6, 0.016) * P1.DistanceTo(P2) * 7.85);
                        idxofcarry.Add(i - 1);
                        WindBraceCount += 1;
                    }
                    else
                    {

                        P3 = new Point3D(theArch.Get3PointReal(carry[i - 1])[2].X, 0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i - 1])[2].Y);
                        P1 = new Point3D(theArch.Get3PointReal(carry[i - 1])[2].X, -0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i - 1])[2].Y);
                        P2 = new Point3D(theArch.Get3PointReal(carry[i])[2].X, 0, theArch.Get3PointReal(carry[i])[2].Y);

#if DEBUGPrtPL
                        string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P1.X, P1.Y, P1.Z, P2.X, P2.Y, P2.Z);
                        Debug.WriteLine(lsp);
                        lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P3.X, P3.Y, P3.Z, P2.X, P2.Y, P2.Z);
                        Debug.WriteLine(lsp);

#endif

                        outTable.AddItem(eMemberType.WindBraceingH, eMaterial.Q345D_T, 2, TubeSection.GetAs(0.6, 0.016) * P1.DistanceTo(P2) * 7.85);
                        idxofcarry.Add(i);
                        WindBraceCount += 1;
                    }

                }

 


            }
            idxofcarry.Add(0);
            idxofcarry.Add(carry.Count - 1);
            idxofcarry = idxofcarry.Distinct().ToList();
            idxofcarry.Sort();
            foreach (var item in idxofcarry)
            {
#if DEBUGPrtWindBraceingV
                Point3D P2 = new Point3D(theArch.Get3PointReal(carry[item])[2].X, 0, theArch.Get3PointReal(carry[item])[2].Y);
                  string  lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                      P2.X, P2.Y - 0.5 * theArch.WidthInside, P2.Z, P2.X, P2.Y + 0.5 * theArch.WidthInside, P2.Z);
                Debug.WriteLine(lsp);
#endif
                double As = TubeSection.GetAs(0.8, 0.016);
                outTable.AddItem(eMemberType.WindBraceingV, eMaterial.Q345D_T, 1, theArch.WidthInside * As * 7.85);
            }
            #endregion

            #region 上平联

            carry = new List<DatumPlane>();
            idxofcarry = new List<int>();
            foreach (var item in theArch.MainDatum)
            {
                if (item.DatumType == eDatumType.ColumnDatum || item.DatumType == eDatumType.VerticalDatum || item.DatumType == eDatumType.NormalDatum)
                {
                    carry.Add(item);
                }
            }
            carry.Sort(new DatumPlane());
            carry.Reverse();
            WindBraceCount = 0;
            for (int i = 0; i < carry.Count; i++)
            {
                double As = TubeSection.GetAs(0.8, 0.016);
                Point3D P1 = new Point3D(theArch.Get3PointReal(carry[i])[0].X, 0.5 * theArch.WidthInside + theArch.WidthOutside, theArch.Get3PointReal(carry[i])[0].Y);
                Point3D P2 = new Point3D(theArch.Get3PointReal(carry[i])[0].X, 0.5 * theArch.WidthInside , theArch.Get3PointReal(carry[i])[0].Y);
                Point3D P3 = new Point3D(theArch.Get3PointReal(carry[i])[0].X, -0.5 * theArch.WidthInside , theArch.Get3PointReal(carry[i])[0].Y);
                Point3D P4 = new Point3D(theArch.Get3PointReal(carry[i])[0].X, -0.5 * theArch.WidthInside-theArch.WidthOutside, theArch.Get3PointReal(carry[i])[0].Y);

#if DEBUGPrtWindBraceingV
                string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")", P1.X, P1.Y, P1.Z, P2.X, P2.Y, P2.Z);
                Debug.WriteLine(lsp);
                lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")", P3.X, P3.Y, P3.Z, P4.X, P4.Y, P4.Z);
                Debug.WriteLine(lsp);               
#endif


                outTable.AddItem(eMemberType.WindBraceingV, eMaterial.Q345D_T, 2, theArch.WidthOutside * As * 7.85);


                if (i != 0 && Math.Abs(theArch.Get3PointReal(carry[i - 1])[0].X - theArch.Get3PointReal(carry[i])[0].X) > 2)
                {
                    // 平联
                    if (WindBraceCount % 2 != 0)
                    {

                        P1 = new Point3D(theArch.Get3PointReal(carry[i - 1])[0].X, 0, theArch.Get3PointReal(carry[i - 1])[0].Y);
                        P2 = new Point3D(theArch.Get3PointReal(carry[i])[0].X, 0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i])[0].Y);
                        P3 = new Point3D(theArch.Get3PointReal(carry[i])[0].X, -0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i])[0].Y);
#if DEBUGPrtPL
                        string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P1.X, P1.Y, P1.Z, P2.X, P2.Y, P2.Z);
                        Debug.WriteLine(lsp);
                        lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P1.X, P1.Y, P1.Z, P3.X, P3.Y, P3.Z);
                        Debug.WriteLine(lsp);
#endif
                        outTable.AddItem(eMemberType.WindBraceingH, eMaterial.Q345D_T, 2, TubeSection.GetAs(0.6, 0.016) * P1.DistanceTo(P2) * 7.85);
                        WindBraceCount += 1;
                        idxofcarry.Add(i - 1);
                    }
                    else
                    {
                        P3 = new Point3D(theArch.Get3PointReal(carry[i - 1])[0].X, -0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i - 1])[0].Y);
                        P1 = new Point3D(theArch.Get3PointReal(carry[i - 1])[0].X, 0.5 * theArch.WidthInside, theArch.Get3PointReal(carry[i - 1])[0].Y);
                        P2 = new Point3D(theArch.Get3PointReal(carry[i])[0].X, 0, theArch.Get3PointReal(carry[i])[0].Y);

#if DEBUGPrtPL
                        string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P1.X, P1.Y, P1.Z, P2.X, P2.Y, P2.Z);
                        Debug.WriteLine(lsp);
                        lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                            P3.X, P3.Y, P3.Z, P2.X, P2.Y, P2.Z);
                        Debug.WriteLine(lsp);
#endif
                        outTable.AddItem(eMemberType.WindBraceingH, eMaterial.Q345D_T, 2, TubeSection.GetAs(0.6, 0.016) * P1.DistanceTo(P2) * 7.85);
                        WindBraceCount += 1;
                        idxofcarry.Add(i);
                    }

                }
            }

            idxofcarry.Add(0);
            idxofcarry.Add(carry.Count - 1);
            idxofcarry = idxofcarry.Distinct().ToList();
            idxofcarry.Sort();
            foreach (var item in idxofcarry)
            {
#if DEBUGPrtWindBraceingV
                Point3D P2 = new Point3D(theArch.Get3PointReal(carry[item])[0].X, 0, theArch.Get3PointReal(carry[item])[0].Y);
                string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8},{2:F8}\" \"{3:F8},{4:F8},{5:F8}\" \"\")",
                    P2.X, P2.Y - 0.5 * theArch.WidthInside, P2.Z, P2.X, P2.Y + 0.5 * theArch.WidthInside, P2.Z);
                Debug.WriteLine(lsp);
#endif
                double As = TubeSection.GetAs(0.8, 0.016);
                outTable.AddItem(eMemberType.WindBraceingV, eMaterial.Q345D_T, 1, theArch.WidthInside * As * 7.85);
            }

            #endregion

            #region 立柱
            foreach (var item in theArch.ColumnList)
            {
                double As = item.L == 4 ? TubeSection.GetAs(0.8, 0.016) : TubeSection.GetAs(0.7, 0.016);
                double Ac = item.L == 4 ? Math.PI * (0.4 - 0.016) * (0.4 - 0.016):0;
                
                // 立柱
                outTable.AddItem(eMemberType.ColumnMain, eMaterial.Q345D_T, 8, (item.H)*As*7.85);
                outTable.AddItem(eMemberType.ColumnMain, eMaterial.C50SCC, 8, item.A*Ac);
                //柱脚
                outTable.AddItem(eMemberType.ColumnMain, eMaterial.Q345D_T, 8, (item.Z1-item.Z0)*As*7.85);
                double footAs = (item.FootL + item.FootW) * 2 * 0.010;
                double footAc = (item.FootL * item.FootW);
                outTable.AddItem(eMemberType.ColumnMain, eMaterial.Q345D_P, 4, (item.Z1-item.Z0)* footAs * 7.85);
                outTable.AddItem(eMemberType.ColumnMain, eMaterial.C50SCC, 4, (item.Z1-item.Z0)* footAc);

                //横撑
                As = TubeSection.GetAs(0.45, 0.016);
                outTable.AddItem(eMemberType.ColumnCrossL, eMaterial.Q345D_T,item.N*6*2, item.L * As * 7.85);
                outTable.AddItem(eMemberType.ColumnCrossW, eMaterial.Q345D_T,item.N*6*2, theArch.WidthOutside * As * 7.85);
                if (item.C!=0)
                {
                    outTable.AddItem(eMemberType.ColumnCrossL, eMaterial.Q345D_T, (item.K + 1) * 4, item.L * As * 7.85);
                    outTable.AddItem(eMemberType.ColumnCrossW, eMaterial.Q345D_T, (item.K + 1) * 4, theArch.WidthOutside * As * 7.85);
                }
                //盖梁
                outTable.AddItem(eMemberType.ColumnMain, eMaterial.Q345D_P, 1, 55.0);

                ;
            }
            outTable.AddItem(eMemberType.ColumnWeb, eMaterial.Q345D_T, 8, 19.5);
            #endregion



        }
    }
}
