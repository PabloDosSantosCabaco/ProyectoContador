using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerContador
{
    class Room
    {
        //Lista de clientes
        public List<string> PlayersNames { get; set; } = new List<string>();
        public Dictionary<string, Client> Clients { get; set; } = new Dictionary<string, Client>();
        public int IdRoom { get; }
        public bool WaitingRoomFinished { get; set; }
        public bool GameFinished { get; set; }
        public string HostName { get; set; }
        public Match Match { get; set; }

        public Room(int idRoom,string name,Client host)
        {
            IdRoom = idRoom;
            WaitingRoomFinished = false;
            GameFinished = false;
            Clients.Add(name, host);
            PlayersNames.Add(name);
            HostName = name;
        }
        public void addCliente(string name,Client client)
        {
            PlayersNames.Add(name);
            Clients.Add(name,client);
        }
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
        public void nextTurn()
        {
            Match.Turn = PlayersNames.IndexOf(Match.Turn) + 1 >= PlayersNames.Count ? PlayersNames[0] : PlayersNames[PlayersNames.IndexOf(Match.Turn) + 1];
        }
    }
}
