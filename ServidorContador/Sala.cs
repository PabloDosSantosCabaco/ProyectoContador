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
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public int IdSala { get; }
        public bool Empezado { get; set; }

        public Sala(int idSala,Cliente host)
        {
            IdSala = idSala;
            Clientes.Add(host);
        }
        public void addCliente(Cliente cliente)
        {
            Clientes.Add(cliente);
        }
    }
}
