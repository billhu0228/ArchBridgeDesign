namespace Model
{
    public class RectSection : Section
    {
        public double Width, Length;

        public RectSection(int id, double w, double l) : base(id,"",eSection.Rect)
        {
            Width = w;
            Length = l;
            Diameter = double.NaN;
            Thickness = 0;
        }
    }
}
