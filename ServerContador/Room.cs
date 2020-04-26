using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerContador
{
    class Room
    {
        /// <summary>
        /// Colección de nombres de los jugadores.
        /// </summary>
        public List<string> PlayersNames { get; set; } = new List<string>();
        /// <summary>
        /// Colección de Clientes con los respectivos nombres de cada uno.
        /// </summary>
        public Dictionary<string, Client> Clients { get; set; } = new Dictionary<string, Client>();
        /// <summary>
        /// Identificador de la sala.
        /// </summary>
        public int IdRoom { get; }
        /// <summary>
        /// Indica si la sala de espera ha finalizado.
        /// </summary>
        public bool WaitingRoomFinished { get; set; }
        /// <summary>
        /// Indica si el juego ha terminado.
        /// </summary>
        public bool GameFinished { get; set; }
        /// <summary>
        /// Nombre del host de la sala.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// Objeto Match para poder trabajar con su información.
        /// </summary>
        public Match Match { get; set; }
        /// <summary>
        /// Constructor de Room.
        /// </summary>
        /// <param name="idRoom">Número de la sala.</param>
        /// <param name="name">Nombre del host.</param>
        /// <param name="host">Host de la sala.</param>
        public Room(int idRoom,string name,Client host)
        {
            IdRoom = idRoom;
            WaitingRoomFinished = false;
            GameFinished = false;
            Clients.Add(name, host);
            PlayersNames.Add(name);
            HostName = name;
        }
        /// <summary>
        /// Añade un jugador a la sala.
        /// </summary>
        /// <param name="name">Nombre del jugador.</param>
        /// <param name="client">Cliente del jugador.</param>
        public void addCliente(string name,Client client)
        {
            PlayersNames.Add(name);
            Clients.Add(name,client);
        }
        /// <summary>
        /// Borra un jugador de la sala.
        /// </summary>
        /// <param name="name">Jugador a borrar.</param>
        public void deletePlayer(string name)
        {
            Clients.Remove(name);
            PlayersNames.Remove(name);
            if (Match != null) {
                Match.PlayersDeck.Remove(name);
                if (Match.Turn == name)
                {
                    Match.Turn = PlayersNames.IndexOf(name) + 1 >= PlayersNames.Count ? PlayersNames[0] : PlayersNames[PlayersNames.IndexOf(name) + 1];
                }
            }
        }
        /// <summary>
        /// Avanza el turno de la sala.
        /// </summary>
        public void nextTurn()
        {
            Match.Turn = PlayersNames.IndexOf(Match.Turn) + 1 >= PlayersNames.Count ? PlayersNames[0] : PlayersNames[PlayersNames.IndexOf(Match.Turn) + 1];
        }
    }
}
