
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
        /// <summary>
        /// Socket del servidor para recibir clientes.
        /// </summary>
        TcpListener s = new TcpListener(IPAddress.Any, 20000);
        /// <summary>
        /// Indica si el servidor está abierto.
        /// </summary>
        bool ServeOpened { get; set; }
        /// <summary>
        /// Indica el número actual disponible para crear una sala.
        /// </summary>
        private int RoomCounter = 0;
        /// <summary>
        /// Colección de salas activas.
        /// </summary>
        Dictionary<int, Room> rooms = new Dictionary<int, Room>();
        /// <summary>
        /// Inicia el servidor.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Program p = new Program();
            p.initServer();
            
        }
        /// <summary>
        /// Abre el servidor.
        /// </summary>
        public void openServe()
        {
            ServeOpened = false;
            try
            {
                s.Start();
                ServeOpened = true;
                Console.WriteLine("------------ SERVER CONTADOR RUNNING ON PORT 20000 ------------");
            }
            catch (SocketException)
            {
                
            }
        }
        /// <summary>
        /// Inicia el servidor y se queda a la espera de la llegada de clientes.
        /// </summary>
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
