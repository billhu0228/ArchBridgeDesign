using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    
    public abstract class BearingBase
    {
        public double Dx, Dy, Dz;
        public BearingBase()
        {
            Dx = 0; 
            Dy = 0;
            Dz = 0;
        }        



    }



    public class NLBearing
    {
        public string Name;
        public Link Ex, Ey, Ez;
        public bool isNL;

        public NLBearing(string name,Link ex, Link ey, Link ez)
        {
            Name = name;
            Ex = ex;
            Ey = ey;
            Ez = ez;
            if (Ex.GetType()==typeof(NolinearLink)|| Ey.GetType() == typeof(NolinearLink)|| Ez.GetType() == typeof(NolinearLink))
            {
                isNL = true;
            }
            else
            {
                isNL = false;
            }
        }
    }

    public class LinkGroup
    {
        public int ID;
        public int Ni, Nj;
        public NLBearing Bearing;

        public LinkGroup(int iD, int ni, int nj, NLBearing bearing)
        {
            ID = iD;
            Ni = ni;
            Nj = nj;
            Bearing = bearing ?? throw new ArgumentNullException(nameof(bearing));
        }
    }

}
