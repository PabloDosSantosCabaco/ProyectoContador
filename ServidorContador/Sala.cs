using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServidorContador
{
    class Sala
    {
        readonly int idSala;
        bool empezado = false;
        //Lista de clientes
        List<Cliente> clientes = new List<Cliente>();

        public Sala(int idSala,Socket host)
        {
            this.idSala = idSala;
            clientes.Add(new Cliente(host));
        }
        
    }
}
