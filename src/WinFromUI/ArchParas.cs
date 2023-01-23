using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFromUI
{
    public class ArchParameters
    {
        public double L, f;
        public double H0, H1;
        public double W0, W1;
        public double halfD;

        bool isFinish;

        public ArchParameters()
        {
            isFinish = false;
        }


    }
}
