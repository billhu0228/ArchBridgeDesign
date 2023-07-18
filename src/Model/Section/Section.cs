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
        public string Name;
        public eSection Type;

        public Section(int secnid,string name,eSection type)
        {
            SECN = secnid;
            Name = name;
            Type = type;
        }
    }
}
