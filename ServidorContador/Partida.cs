using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorContador
{
    class Partida
    {
        public int ValorMesa { get; set; }
        public bool SentidoMesa { get; set; }
        public int Limite { get; set; }
        public string Turno { get; set; }
        public int Ranking { get; set; }
        public Dictionary<string, List<Carta>> BarajasJugadores { get; set; } = new Dictionary<string, List<Carta>>();
        public Partida()
        {
            ValorMesa = 0;
            SentidoMesa = true;
            Limite = 10;
            Ranking = 1;
        }
    }
}
