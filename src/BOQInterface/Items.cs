using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Model;

namespace BOQInterface
{

    /// <summary>
    /// 一个BOQ计量项
    /// </summary>
    public class BOQItems
    {
        public int id;
        public eMaterial mat;
        public eMemberType loc;
        public int num;
        public double quantity;

        public BOQItems(int id, eMaterial mat, eMemberType loc, int num, double quantity)
        {
            this.id = id;
            this.mat = mat;
            this.loc = loc;
            this.num = num;
            this.quantity = quantity;
        }
    }
}
