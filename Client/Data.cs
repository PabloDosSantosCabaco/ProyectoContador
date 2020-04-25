using System.Collections.Generic;

namespace Client
{
    class Data
    {
        public Dictionary<string, int> Players { get; set; }
        public int TableValue { get; set; }
        public string Turn { get; set; }
        public bool Way { get; set; }
        public List<Card> Cards { get; set; }

        public Data(List<Card> cards,int tableValue,bool way,string turn,Dictionary<string,int> players)
        {
            Cards = cards;
            Way = way;
            TableValue = tableValue;
            Turn = turn;
            Players = players;
        }
    }
}
