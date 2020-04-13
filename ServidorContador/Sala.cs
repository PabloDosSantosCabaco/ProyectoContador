using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServidorContador
{
    class Sala
    {
        //Lista de clientes
        public List<string> PlayersNames { get; set; } = new List<string>();
        public Dictionary<string, Cliente> Clientes { get; set; } = new Dictionary<string, Cliente>();
        public int IdSala { get; }
        public bool WaitingRoomFinished { get; set; }
        public bool GameFinished { get; set; }
        public string NombreHost { get; set; }
        public Partida Partida { get; set; }

        public Sala(int idSala,string nombre,Cliente host)
        {
            IdSala = idSala;
            WaitingRoomFinished = false;
            GameFinished = false;
            Clientes.Add(nombre, host);
            PlayersNames.Add(nombre);
            NombreHost = nombre;
        }
        public void addCliente(string nombre,Cliente cliente)
        {
            PlayersNames.Add(nombre);
            Clientes.Add(nombre,cliente);
        }
        public void deletePlayer(string nombre)
        {
            Clientes.Remove(nombre);
            PlayersNames.Remove(nombre);
            if (Partida != null) {
                Partida.BarajasJugadores.Remove(nombre);
                if (Partida.Turno == nombre)
                {
                    Partida.Turno = PlayersNames.IndexOf(nombre) + 1 >= PlayersNames.Count ? PlayersNames[0] : PlayersNames[PlayersNames.IndexOf(nombre) + 1];
                }
            }
        }
        public void avanzarTurno()
        {
            Partida.Turno = PlayersNames.IndexOf(Partida.Turno) + 1 >= PlayersNames.Count ? PlayersNames[0] : PlayersNames[PlayersNames.IndexOf(Partida.Turno) + 1];
        }
    }
}
