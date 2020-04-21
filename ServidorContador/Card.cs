using System;
using System.Collections.Generic;
using System.Text;

namespace ServerContador
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
        public int Value
        {
            set
            {
                if (value < 3)
                {
                    Value = 3;
                }
                else if (value > 7)
                {
                    Value = 7;
                }
                else
                {
                    Value = value;
                }
            }
            get
            {
                return Value;
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
