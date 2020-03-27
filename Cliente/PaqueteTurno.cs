using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class PaqueteTurno
    {
        public Dictionary<string, int> Jugadores { get; set; }
        public int ValorMesa { get; set; }
        public string Turno { get; set; }
        public bool Sentido { get; set; }
        public List<Carta> Cartas { get; set; }

        public PaqueteTurno(List<Carta> cartas,int valorMesa,bool sentido,string turno,Dictionary<string,int> jugadores)
        {
            Cartas = cartas;
            Sentido = sentido;
            ValorMesa = valorMesa;
            Turno = turno;
            Jugadores = jugadores;
        }
    }
}
