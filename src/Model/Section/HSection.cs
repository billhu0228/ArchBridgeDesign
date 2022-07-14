namespace Model
{
    public class HSection : Section
    {
        public double W1, W2, W3, t1, t2, t3;

        public HSection(int id, double w1, double w2, double w3, double t1, double t2, double t3) : base(id)
        {
            W1 = w1;
            W2 = w2;
            W3 = w3;
            this.t1 = t1;
            this.t2 = t2;
            this.t3 = t3;
            Diameter = w1;
            Thickness = 0;
        }

        public double Area
        {
            get
            {
                return W1 * t1 + W2 * t2 + (W3 - t1 - t2) * t3;
            }
        }

        public override string ToString()
        {
            return string.Format("H{0:G}X{1:G}X{2:G}X{3:G}", W3 * 1000, W1 * 1000, t3 * 1000, t1 * 1000);
        }

    }
}
