using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 杆件属性记录
    /// </summary>
    public class MemberPropertyRecord
    {
        public int index;
        public eMemberType Member;
        public double RangeFrom;
        public double RangeTo;
        public Section Section;

        public MemberPropertyRecord(int index, Section sect, eMemberType member, double rangeFrom, double rangeTo)
        {
            this.index = index;
            Section = sect;
            Member = member;
            RangeFrom = rangeFrom;
            RangeTo = rangeTo;
        }

        public override string ToString()
        {
            string name3 = EnumHelper.GetDescription(typeof(eMemberType), Member);

            string wd = string.Format("{0},截面={1}，范围=（{2},{3}）", name3, Section.ToString(), RangeFrom, RangeTo);
            return wd;

        }

        public bool CheckProperty( double loc,eMemberType member)
        {
            if (Member==member && RangeFrom<=loc && RangeTo>=loc)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
