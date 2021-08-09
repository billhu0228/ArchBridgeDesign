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
        public Angle Angle0;
        public Vector2D Direction;

        public CutSection(int id,Point2D cc,Point2D refpt)
        {
            ID = id;
            Center = cc;
            RefPoint = refpt;
            Angle0= (RefPoint - Center).SignedAngleTo(Vector2D.XAxis);
            Direction = Vector2D.XAxis.Rotate(Angle0);
        }
        public CutSection(int id, Point2D cc, Angle angle)
        {
            ID = id;
            Center = cc;
            Direction = Vector2D.XAxis.Rotate(angle);
            RefPoint = cc + Direction;
        }


        public override string ToString()
        {
            return string.Format("截面 x={0} , Ang={1}",Center.X,Angle0.Degrees);
        }



    }
}
