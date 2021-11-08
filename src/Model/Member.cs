using MathNet.Spatial.Euclidean;

namespace Model
{
    /// <summary>
    /// 构件
    /// </summary>
    public class Member
    {
        public Line2D Line;
        public int ID;
        public Section Sect;
        public eMemberType ElemType;
        public DatumPlane StartDatum;

        public Member(int id, Line2D line, Section sect, eMemberType et)
        {
            Line = line;
            ID = id;
            Sect = sect;
            ElemType = et;
        }

        public Member(int id, Point2D pSt,Point2D pEd, Section sect, eMemberType et)
        {
            Line = new Line2D(pSt, pEd);
            ID = id;
            Sect = sect;
            ElemType = et;
        }

        public override string ToString()
        {
            return string.Format("{0} (ID={1}, Section={2})", EnumHelper.GetDescription(typeof(eMemberType),ElemType), ID, Sect.ToString());
        }

        public string Lisp
        {
            get
            {
                string lsp = string.Format("(command \"._line\" \"{0:F8},{1:F8}\" \"{2:F8},{3:F8}\" \"\")", Line.StartPoint.X, Line.StartPoint.Y, Line.EndPoint.X, Line.EndPoint.Y);
                return lsp;
            }
        }




    }
}
