using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 基准面
    /// </summary>
    public class DatumPlane: IComparer<DatumPlane>
    {
        public int ID;
        public Point2D Center,RefPoint;
        public Angle Angle0; // 值域0~360
        public Vector2D Direction;
        public eDatumType DatumType;

        public DatumPlane()
        {
           
        }

        public DatumPlane(int id,Point2D cc,Point2D refpt,eDatumType dt)
        {
            ID = id;
            Center = cc;
            RefPoint = refpt;
            // Angle0= (RefPoint - Center).SignedAngleTo(Vector2D.XAxis);
            Angle0=(Vector2D.XAxis).SignedAngleTo(RefPoint - Center);
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

        public string Lisp
        {
            get
            {
                string lsp = string.Format("(command-s \"._xline\" \"a\" \"{0:F12}\" \"{1:F12},{2:F12}\" \"\" \"\")", Angle0.Degrees, Center.X, Center.Y) ;
                return lsp;
            }
        }

        public override string ToString()
        {
            return string.Format("控制面 {3} : x={0} , Ang={1} , 类别={2}",Center.X,Angle0.Degrees, EnumHelper.GetDescription(typeof(eDatumType),DatumType),ID);


        }


        public int Compare(DatumPlane x, DatumPlane y)
        {
            return x.Center.X.CompareTo(y.Center.X);
        }
    }
}
