
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServidorContador
{
    class Program
    {
        TcpListener s = new TcpListener(IPAddress.Any, 20000);
        bool serveOpened = false;
        private int contadorSalas = 0;
        Dictionary<int, Sala> salas = new Dictionary<int, Sala>();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.initServer();
            
        }

        public void openServe()
        {
            try
            {
                s.Start();
                serveOpened = true;
            }
            catch (SocketException)
            {
                
            }
        }
        public void initServer()
        {
            openServe();
            while (serveOpened)
            {
                //Llega un cliente
                TcpClient sCliente = s.AcceptTcpClient();
                Thread hiloGestionCliente = new Thread(() => GestionClientes.gestionCliente(sCliente,salas, ref contadorSalas));
                hiloGestionCliente.Start();
            }
            s.Stop();
        }
    }
}
