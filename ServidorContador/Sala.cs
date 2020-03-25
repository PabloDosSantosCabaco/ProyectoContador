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
        public Dictionary<string, Cliente> Clientes { get; set; } = new Dictionary<string, Cliente>();
        public int IdSala { get; }
        public bool Empezado { get; set; }
        public string NombreHost { get; set; }

        public Sala(int idSala,string nombre,Cliente host)
        {
            IdSala = idSala;
            Clientes.Add(nombre, host);
            NombreHost = nombre;
        }
        public void addCliente(string nombre,Cliente cliente)
        {
            Clientes.Add(nombre,cliente);
        }
    }
}
