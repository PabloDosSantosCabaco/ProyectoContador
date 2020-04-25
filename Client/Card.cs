namespace Client
{
    class Card
    {
        public enum eType
        {
            Number,
            Way,
            Effect
        }
        public eType Type { get; set; }
        private int _value;
        public int Value
        {
            set
            {
                if (value < 3)
                {
                    _value = 3;
                }
                else if (value > 7)
                {
                    _value = 7;
                }
                else
                {
                    _value = value;
                }
            }
            get
            {
                return _value;
            }
        }
        public bool Way { get; set; }
        public Card(eType type, int value, bool way)
        {
            Value = value;
            Type = type;
            Way = way;
        }
    }
}
