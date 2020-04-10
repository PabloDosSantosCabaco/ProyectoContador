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
        public bool Acabado { get; set; }
        public string NombreHost { get; set; }

        public Sala(int idSala,string nombre,Cliente host)
        {
            IdSala = idSala;
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
        }
    }
}
