namespace Model
{
    public class RectSection : Section
    {
        public double Width, Length;

        public RectSection(int id, double w, double l) : base(id)
        {
            Width = w;
            Length = l;
            Diameter = double.NaN;
            Thickness = 0;
        }
    }
}
