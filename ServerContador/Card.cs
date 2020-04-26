using System;
using System.Collections.Generic;
using System.Text;

namespace ServerContador
{
    class Card
    {
        /// <summary>
        /// Diferentes tipos de cartas.
        /// </summary>
        public enum eType
        {
            Number,
            Way,
            Effect
        }
        /// <summary>
        /// Indica el tipo de carta.
        /// </summary>
        public eType Type { get; set; }
        /// <summary>
        /// Indica el valor de la carta.
        /// </summary>
        private int _value;
        /// <summary>
        /// Establece el valor de la carta.
        /// </summary>
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
        /// <summary>
        /// Indica y establece el sentido de la carta.
        /// </summary>
        public bool Way { get; set; }
        /// <summary>
        /// Constructor de Card.
        /// </summary>
        /// <param name="type">Tipo de carta.</param>
        /// <param name="value">Valor númerico de la carta.</param>
        /// <param name="way">Sentido de la carta.</param>
        public Card(eType type, int value, bool way)
        {
            Value = value;
            Type = type;
            Way = way;
        }
    }
}
