using System.Collections.Generic;

namespace Client
{
    class Data
    {
        /// <summary>
        /// Colección de jugadores con sus nombres y el número de cartas que tienen en su mano.
        /// </summary>
        public Dictionary<string, int> Players { get; set; }
        /// <summary>
        /// Valor dl contador de la mesa.
        /// </summary>
        public int TableValue { get; set; }
        /// <summary>
        /// Nombre del jugador al que le toca jugar.
        /// </summary>
        public string Turn { get; set; }
        /// <summary>
        /// Sentido de la mesa.
        /// </summary>
        public bool Way { get; set; }
        /// <summary>
        /// Lista de cartas que conforman la baraja.
        /// </summary>
        public List<Card> Cards { get; set; }
        /// <summary>
        /// Constructor del objeto Data.
        /// </summary>
        /// <param name="cards">Lista de cartas.</param>
        /// <param name="tableValue">Valor de la mesa.</param>
        /// <param name="way">Sentido de la mesa.</param>
        /// <param name="turn">Nombre del jugador al que le toca jugar.</param>
        /// <param name="players">Colección de jugadores con sus nombres y cantidad de cartas.</param>
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
