using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Link
    {
        public double K;

        public Link(double k)
        {
            K = k;
        }
    }
    public class NolinearLink: Link
    {        
        public double Fy;
        public double r;// 屈服后强度比
        public int s;//屈服指数
        public double a, b;// 循环参数；
        public double Ke;

        public NolinearLink(double k,double ke, double fy, double r, int s, double a, double b):base(k)
        {
            Ke = ke; 
            Fy = fy;
            this.r = r;
            this.s = s;
            this.a = a;
            this.b = b;
        }
    }
}
