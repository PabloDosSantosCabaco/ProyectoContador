using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServidorContador
{
    class GestionClientes
    {
        public static bool crearSala(Cliente cliente, Dictionary<int, Sala> salas,ref int contadorSalas)
        {
            string nombre = cliente.recibirDatos();
            if (nombre.Trim().Length >= 3)
            {
                //Creo la nueva sala
                lock (salas)
                {
                    Sala sala = new Sala(contadorSalas, nombre, cliente);
                    cliente.enviarDatos(contadorSalas.ToString());
                    //Aumento el identificador para evitar repetir salas
                    contadorSalas++;
                    //Añado la sala a la colección
                    salas.Add(sala.IdSala, sala);
                    Thread hiloSala = new Thread(() => SalaEspera.salaEspera(sala,salas));
                    hiloSala.Start();
                    Console.WriteLine($"El usuario {nombre} ha creado la sala {contadorSalas-1}");
                }
                return true;
            }
            return false;
        }
        public static bool entrarSala(Cliente cliente, Dictionary<int, Sala> salas)
        {
            //Comprueba si la sala a la que quiere entrar existe y si tiene menos de 8 clientes
            int sala = -1;
            string nombre = null;
            try
            {
                sala = Convert.ToInt32(cliente.recibirDatos());
                nombre = cliente.recibirDatos();
            }
            catch (FormatException) { }
            catch(OverflowException) { }
            lock (salas)
            {
                if (salas.ContainsKey(sala) && salas[sala].Clientes.Count < 8 && !salas[sala].WaitingRoomFinished)
                {
                    //El cliente entra en la sala
                    lock (salas[sala])
                    {
                        if (nombre==null || nombre.Trim().Length<3 || salas[sala].Clientes.ContainsKey(nombre))
                        {
                            Console.WriteLine("Este nombre ya existe");
                            cliente.enviarDatos("errorNombre");
                            return false;
                        }
                        salas[sala].addCliente(nombre, cliente);
                        Console.WriteLine($"El cliente {nombre} ha entrado en la sala {sala}");

                        cliente.enviarDatos("true");
                        foreach (Cliente client in salas[sala].Clientes.Values)
                        {
                            client.refreshWaitingRoom(salas[sala]);
                        }
                    }
                    return true;
                }
                else
                {
                    cliente.enviarDatos("errorSala");
                    return false;
                }
            }
        }
        public static void gestionCliente(TcpClient socket,Dictionary<int,Sala> salas,ref int contadorSalas)
        {
            Console.WriteLine("Ha entrado un cliente");
            Cliente cliente = new Cliente(socket);
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
                        if (!crearSala(cliente,salas,ref contadorSalas))
                        {
                            gestionado = false;
                        }
                        break;
                    case "join":
                        //Gestionamos que el cliente no haya podido entrar en la sala
                        if (!entrarSala(cliente,salas))
                        {
                            //cliente.enviarDatos("La sala a la que intentas entrar o existe o está llena.");
                            gestionado = false;
                        }
                        break;
                    case null:
                        cliente.enviarDatos("Cliente desconectado");
                        break;
                    default:
                        cliente.enviarDatos("Comando no soportado");
                        gestionado = false;
                        break;
                }
            } while (!gestionado);
        }
    }
}
