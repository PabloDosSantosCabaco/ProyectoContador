
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerContador
{
    class Program
    {
        TcpListener s = new TcpListener(IPAddress.Any, 20000);
        bool ServeOpened { get; set; }
        private int RoomCounter = 0;
        Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.initServer();
            
        }
        public void openServe()
        {
            ServeOpened = false;
            try
            {
                s.Start();
                ServeOpened = true;
            }
            catch (SocketException)
            {
                
            }
        }
        public void initServer()
        {
            openServe();
            while (ServeOpened)
            {
                //Llega un cliente
                TcpClient sCliente = s.AcceptTcpClient();
                Thread hiloGestionCliente = new Thread(() => GestionClientes.manageClient(sCliente,rooms, ref RoomCounter));
                hiloGestionCliente.Start();
            }
            s.Stop();
        }
    }
}
