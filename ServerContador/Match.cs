using System;
using System.Collections.Generic;
using System.Text;

namespace ServerContador
{
    class Match
    {
        /// <summary>
        /// Valor de la mesa.
        /// </summary>
        public int TableValue { get; set; }
        /// <summary>
        /// Sentido de la mesa.
        /// </summary>
        public bool TableWay { get; set; }
        /// <summary>
        /// Limite de la mesa.
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// Turno de la mesa.
        /// </summary>
        public string Turn { get; set; }
        /// <summary>
        /// Posición actual del ranking.
        /// </summary>
        public int Ranking { get; set; }
        /// <summary>
        /// Coleccion de barajas de los jugadores.
        /// </summary>
        public Dictionary<string, List<Card>> PlayersDeck { get; set; } = new Dictionary<string, List<Card>>();
        /// <summary>
        /// Constructor de la clase Match.
        /// </summary>
        public Match()
        {
            TableValue = 0;
            TableWay = true;
            Limit = 10;
            Ranking = 1;
        }
    }
}
