using System;
using System.Collections.Generic;
using System.IO;
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
                gestionCliente(sCliente);
            }
            s.Close();
        }
        public void crearSala(Cliente cliente)
        {
            //Creo la nueva sala
            Sala sala = new Sala(contadorSalas,cliente);
            cliente.enviarDatos(contadorSalas.ToString());
            //Aumento el identificador para evitar repetir salas
            contadorSalas++;
            //Añado la sala a la colección
            salas.Add(sala.getID(), sala);
            
        }
        public bool entrarSala(Cliente cliente,int sala)
        {
            //Comprueba si la sala a la que quiere entrar existe
            if (salas.ContainsKey(sala))
            {
                //El cliente entra en la sala
                salas.GetValueOrDefault(sala).addCliente(cliente);
                cliente.enviarDatos($"Cliente añadido a la sala {sala}");
                return true;
            }
            else
            {
                return false;
            }
        }
        public void gestionCliente(Socket socket)
        {
            Cliente cliente = new Cliente(socket);
            cliente.enviarDatos("Bienvenido");
            bool gestionado;
            //Decidimos si crea o se une
            do
            {
                gestionado = true;
                string res = cliente.recibirDatos();
                //Decidimos si crea o se une
                switch (res)
                {
                    case "new":
                            //Creamos la sala metiendo al primer cliente como host
                            crearSala(cliente);
                            break;
                    case "join":
                        try
                        {
                            //Leemos la sala a la que quiere entrar
                            int sala = Convert.ToInt32(cliente.recibirDatos());
                            //Gestionamos que el cliente no haya podido entrar en la sala
                            if (!entrarSala(cliente, sala))
                            {
                                Console.WriteLine($"El cliente {cliente.getIP()} no ha podido conectarse a la sala {sala}");
                                gestionado = false;
                            }
                        }
                        catch (FormatException fEx) { }
                        catch (OverflowException oEx) { }
                        break;
                    case null:
                        Console.WriteLine("Cliente desconectado");
                        break;
                    default:
                        Console.WriteLine("Comando no soportado");
                        gestionado = false;
                        break;
                }
            } while (!gestionado);
        }
    }
}
