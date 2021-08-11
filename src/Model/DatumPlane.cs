using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DatumPlane
    {
        public int ID;
        public Point2D Center,RefPoint;
        public Angle Angle0;
        public Vector2D Direction;
        public eDatumType DatumType;

        public DatumPlane(int id,Point2D cc,Point2D refpt,eDatumType dt)
        {
            ID = id;
            Center = cc;
            RefPoint = refpt;
            Angle0= (RefPoint - Center).SignedAngleTo(Vector2D.XAxis);
            Direction = Vector2D.XAxis.Rotate(Angle0);
            DatumType = dt;
        }
        public DatumPlane(int id, Point2D cc, Angle angle, eDatumType dt)
        {
            ID = id;
            Center = cc;
            Direction = Vector2D.XAxis.Rotate(angle);
            RefPoint = cc + Direction;
            Angle0 = angle;
            DatumType = dt;
        }


        public Line2D Line {
            get 
            {
                return new Line2D(Center, RefPoint);

            }
        }

        public override string ToString()
        {
            return string.Format("控制面 x={0} , Ang={1} , 类别={2}",Center.X,Angle0.Degrees, EnumHelper.GetDescription(typeof(eDatumType),DatumType));
        }



    }
}
