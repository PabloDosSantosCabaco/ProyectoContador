using System;
using System.Collections.Generic;
using System.Text;

namespace ServerContador
{
    class Match
    {
        public int TableValue { get; set; }
        public bool TableWay { get; set; }
        public int Limit { get; set; }
        public string Turn { get; set; }
        public int Ranking { get; set; }
        public Dictionary<string, List<Card>> PlayersDeck { get; set; } = new Dictionary<string, List<Card>>();
        public Match()
        {
            TableValue = 0;
            TableWay = true;
            Limit = 10;
            Ranking = 1;
        }
    }
}
