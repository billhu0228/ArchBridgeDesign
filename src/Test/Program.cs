using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ArchAxis aa = new ArchAxis(100, 1.5, 500);
            
            var f=aa.ArchLength;
        }
    }
}
