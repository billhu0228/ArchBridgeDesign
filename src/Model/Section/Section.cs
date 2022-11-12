namespace Model
{
    /// <summary>
    /// 截面
    /// </summary>
    public abstract class Section
    {
        public int SECN;
        public double Diameter;
        public double Thickness;

        public Section(int secnid)
        {
            SECN = secnid;
        }
    }
}
