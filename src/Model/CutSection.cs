using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CutSection
    {
        public int ID;
        public Point2D Center,RefPoint;
        
        public CutSection(int id,Point2D cc,Point2D refpt)
        {
            ID = id;
            Center = cc;
            RefPoint = refpt;
        }

        public Angle AngleFromHorizental
        {
            get
            {
                return (RefPoint - Center).SignedAngleTo(Vector2D.XAxis);
            }
        }

        public override string ToString()
        {
            return string.Format("截面 x={0} , Ang={1}",Center.X,AngleFromHorizental.Degrees);
        }



    }
}
