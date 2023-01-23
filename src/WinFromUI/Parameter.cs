using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinFromUI
{
    public class Parameter
    {
        private double v;
        public int Id { get; set; }

        public string Name { get; set; }
        public string Text { get { return Name2Text(); } }

        public string ValueStr { get; set; }
        public double Value { get { return v; } set { v = value; } }

        public int ParentId { get; set; }


        private string Name2Text()
        {
            if (v < 0)
            {
                return Name;
            }
            else
            {
                return Name.Split(')')[0]+string.Format(" = {0:F2} )",v);              
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }


}
