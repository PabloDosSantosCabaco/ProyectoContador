using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServidorContador
{
    class Sala
    {
        readonly int idSala;
        bool empezado = false;
        //Lista de clientes
        List<Cliente> clientes = new List<Cliente>();

        public Sala(int idSala,Cliente host)
        {
            this.idSala = idSala;
            clientes.Add(host);
            Thread threadEspera = new Thread(()=>espera());
            threadEspera.Start();
        }
        public int getID()
        {
            return idSala;
        }
        public void addCliente(Cliente cliente)
        {
            clientes.Add(cliente);
        }
        public void espera()
        {
            while (!empezado)
            {

            }
        }
    }
}
