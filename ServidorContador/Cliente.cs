using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServidorContador
{
    class Cliente
    {
        //bool acabo;
        //List<Carta> cartas;
        Socket socket;
        public Cliente(Socket socketCliente)
        {
            socket = socketCliente;
        }
    }
}
