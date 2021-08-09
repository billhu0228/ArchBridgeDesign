using System;
using System.ComponentModel;
using System.Reflection;

namespace Model
{
    /// <summary>
    /// 七种拱肋轴线定义
    /// </summary>
    public enum CenterLineType
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

    public enum MemberType
    {
        [Description("上弦杆")]
        UpperCoord = 1,
        [Description("下弦杆")]
        LowerCoord = 2,
        [Description("竖腹杆")]
        VerticalWeb = 3,
        [Description("斜腹杆")]
        InclineWeb = 4,
    }

    public static class EnumDemo
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


    public enum SectionType
    {
        ColumnSection = 1,
        InstallSection = 2,
        WebSection = 3,
    }

}
