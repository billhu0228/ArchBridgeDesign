﻿using System.Collections.Generic;

namespace Model
{
    public class MemberPropertyRecord
    {
        public int index;
        public MemberType Member;
        public double RangeFrom;
        public double RangeTo;
        public TubeSection Section;

        public MemberPropertyRecord(int index, TubeSection sect, MemberType member, double rangeFrom, double rangeTo)
        {
            this.index = index;
            Section = sect;
            Member = member;
            RangeFrom = rangeFrom;
            RangeTo = rangeTo;
        }

        public override string ToString()
        {
            string name3 = EnumDemo.GetDescription(typeof(MemberType), Member);

            string wd = string.Format("{0},截面={1}，范围=（{2},{3}）", name3, Section.ToString(), RangeFrom, RangeTo);
            return wd;

        }

        public bool CheckProperty( double loc,MemberType member)
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