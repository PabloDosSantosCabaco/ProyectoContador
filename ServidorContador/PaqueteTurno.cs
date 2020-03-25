using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorContador
{
    class PaqueteTurno
    {
        Dictionary<string, int> jugadores = new Dictionary<string, int>();
        int valorMesa;
        int turno;
        List<Carta> cartas = new List<Carta>();

        public Dictionary<string, int> Jugadores { get; set; }
        public int ValorMesa { get; set; }
        public int Turno { get; set; }
        public List<Carta> Cartas { get; set; }

        public PaqueteTurno(List<Carta> cartas,int valorMesa,int turno,Dictionary<string,int> jugadores)
        {
            Cartas = cartas;
            ValorMesa = ValorMesa;
            Turno = turno;
            Jugadores = jugadores;
        }
    }
}
