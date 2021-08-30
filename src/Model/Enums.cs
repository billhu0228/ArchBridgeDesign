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

        [Description("一般控制面")]
        GeneralDatum =9,
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
        UpperCoord = 1,
        [Description("下弦杆")]
        LowerCoord = 2,
        [Description("普通竖腹杆")]        
        VerticalWeb = 3,
        [Description("立柱竖腹杆")]
        ColumnWeb =31,
        [Description("斜腹杆")]
        InclineWeb = 4,
        [Description("半斜腹杆")]
        InclineWebS = 41,

        [Description("横撑")]
        CrossBraceing =5,
        [Description("横风撑")]        
        WindBraceingV =6,
        [Description("风撑平联")]
        WindBraceingH =7,
        [Description("斜撑")]
        WebBracing =8,

        [Description("虚拟截面")]
        Virtual = 10,

        [Description("立柱主杆")]
        ColumnMain=20,
        [Description("立柱顺桥向横撑")]
        ColumnCrossL = 21,
        [Description("立柱横桥向横撑")]
        ColumnCrossW = 22,
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
