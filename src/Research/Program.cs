using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Research
{
    class Program
    {
        public double CenterY(ref Arch theArch)
        {

            return 0.0;
        }
        static void Main(string[] args)
        {
            ArchAxis ax;
            Arch theArchModel;
            theArchModel = NamedArch.PhoenixModelV4(out ax, 2.0, 518 / (4.5), 14, 6.5);


        }
    }
}
