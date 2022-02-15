using System;
using System.ComponentModel;
using System.Reflection;

namespace Model
{
    public enum eDatumType
    {
        None = 0,
        [Description("立柱控制面")]
        ColumnDatum = 1,

        [Description("竖向控制面")]
        VerticalDatum = 2,

        [Description("法向控制面")]
        NormalDatum = 3,

        [Description("中点控制面")]
        MiddleDatum = 4,

        [Description("斜杆控制面")]
        DiagonalDatum = 5,

        [Description("无截面控制面")]
        ControlDatum = 6,

        [Description("安装控制面")]
        InstallDatum =7,

        [Description("一般控制面")]
        GeneralDatum =9,


        [Description("安装闭合控制面")]
        DoubleDatum = 10,

    }
    /// <summary>
    /// 七种拱肋轴线定义
    /// </summary>
    public enum eCenterLineType
    {
        [Description("上弦上表面")]
        UU = 11,

        [Description("上弦轴线")]
        UC = 10,

        [Description("上弦下表面")]
        UL = 12,

        [Description("拱轴线")]
        CC = 0,

        [Description("下弦上表面")]
        LU = 21,

        [Description("下弦轴线")]
        LC = 20,

        [Description("下弦下表面")]
        LL = 22,
    }

    public enum eMemberType
    {
        [Description("上弦杆")]
        UpperCoord = 11,
        [Description("下弦杆")]
        LowerCoord = 12,

        [Description("主腹杆")]        
        MainWeb = 21,
        [Description("次腹杆")]
        SubWeb = 22,
        [Description("拼接腹杆")]
        InstallWeb = 23,
        [Description("三角腹杆")]
        TriWeb = 24,

        [Description("横隔弦杆")]
        DiaphragmCoord = 31,
        [Description("横隔斜杆")]
        DiaphragmWeb = 32,

        [Description("横梁弦杆")]
        CrossCoord =41,
        [Description("横梁斜杆")]
        CrossWeb = 42,
        [Description("横梁竖杆")]
        CrossVerical = 43,

        [Description("斜撑")]
        WindBracing =51,

        [Description("立柱主杆")]
        ColumnMain=61,
        [Description("立柱顺桥向横撑")]
        ColumnCrossL = 62,
        [Description("立柱横桥向横撑")]
        ColumnCrossW = 63,
        [Description("立柱盖梁")]
        ColumnCapBeam = 64,
        [Description("立柱小斜撑")]
        ColumnWeb = 65,

        [Description("虚拟截面")]
        Virtual = 70,

    }


    public enum eMaterial
    {
        [Description("混凝土-C35")]
        C35 = 101,

        [Description("混凝土-C40")]
        C40 = 102,

        [Description("混凝土-C50自密实微膨胀")]
        C50SCC = 103,

        [Description("混凝土-C50预制")]
        C50PRE = 104,

        [Description("混凝土-C80自密实微膨胀")]
        C80SCC = 105,

        [Description("铺装-沥青")]
        W_ASPHALT = 201,

        [Description("铺装-防水层")]
        W_PROOF = 202,

        [Description("钢筋-HRB500")]
        HRB500 = 401,

        [Description("钢筋-HBR400")]
        HRB400 = 402,

        [Description("钢材-Q420D钢管")]
        Q420D_T = 501,

        [Description("钢材-Q345D钢管")]
        Q345D_T = 502,

        [Description("钢材-Q345D钢材")]
        Q345D_P = 503,

        [Description("钢材-Q235C钢材")]
        Q235C_P = 504,

        [Description("钢材-Q235C钢管")]
        Q235C_T = 505,

        [Description("螺栓-高强")]
        BOLT_H = 601,

        [Description("螺栓-普通")]
        BOLT_N = 602,

        [Description("防腐涂层-外表面")]
        COAT_E = 701,

        [Description("防腐涂层-内表面")]
        COAT_I = 702,

        [Description("支座-QZ2000GD")]
        QZ2000GD = 801,

        [Description("支座-QZ2000DX")]
        QZ2000DX = 802,

        [Description("伸缩缝-MF240")]
        MF240 = 901,
    }

    public static class EnumHelper
    {
        private static string GetName(Type t, object v)
        {
            try
            {
                return Enum.GetName(t, v);
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        /// <summary>
        /// 返回指定枚举类型的指定值的描述
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="v">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(System.Type t, object v)
        {
            try
            {
                FieldInfo oFieldInfo = t.GetField(GetName(t, v));
                DescriptionAttribute[] attributes = (DescriptionAttribute[])oFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : GetName(t, v);
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }




}
