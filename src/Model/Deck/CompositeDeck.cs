using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CompositeDeck
    {
        public List<double> spans;
        public CrossArrangement crossSection;
        public double crossDist;
        public HSection MGirder, SGirder, UpBeam, LowBeam, DiaBeam, EndBeam;
        public RectSection Deck;
        public Dictionary<string, Section> SectionList;
        public List<int> ColumnIDList;

        public CompositeDeck(List<double> SpanList, CrossArrangement CA,List<int> ColIDList, double CrossBeamDist = 5.0)
        {
            spans = SpanList;
            crossSection = CA;
            crossDist = CrossBeamDist;
            SectionList = new Dictionary<string, Section>();
            ColumnIDList = ColIDList;
        }

        public void AddSection(string Name, Section s)
        {
            SectionList.Add(Name, s);
        }


    }
}
