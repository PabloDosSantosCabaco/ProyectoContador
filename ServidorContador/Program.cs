using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServidorContador
{
    class Program
    {
        IPEndPoint ie = new IPEndPoint(IPAddress.Any, 31416);
        Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        bool serveOpened = false;
        private int contadorSalas = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public void openServe()
        {
            try
            {
                s.Bind(ie);
                s.Listen(100);
                serveOpened = true;
            }
            catch (SocketException sEx)
            {
                
            }
        }
        public void initServer()
        {
            openServe();
            while (serveOpened)
            {
                //Llega un cliente
                Socket sCliente = s.Accept();
                //Decidimos si crea o se une
                //Thread threadPartida = new Thread(() => partida());
                crearSala();
            }
            s.Close();
        }
        public void crearSala()
        {
            Sala sala = new Sala(contadorSalas);
            contadorSalas++;
        }
        public void partida()
        {

        }
    }
}
