
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
        //Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        TcpListener s = new TcpListener(IPAddress.Any, 20000);
        bool serveOpened = false;
        private int contadorSalas = 0;
        Dictionary<int, Sala> salas = new Dictionary<int, Sala>();
        int maxClientes = 8;

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
                TcpClient sCliente = s.AcceptTcpClient();
                Thread hiloGestionCliente = new Thread(() => gestionCliente(sCliente));
                hiloGestionCliente.Start();
            }
            s.Stop();
        }

        /*******************************************/

        public bool crearSala(Cliente cliente)
        {
            string nombre;
            nombre=cliente.recibirDatos();
            Console.WriteLine("Mi cliente se llama " + nombre);
            if (nombre.Trim().Length >= 3)
            {
                //Creo la nueva sala
                lock (salas)
                {
                    Sala sala = new Sala(contadorSalas, nombre, cliente);
                    Console.WriteLine("Devuelvo el numero de sala: " + contadorSalas);
                    cliente.enviarDatos(contadorSalas.ToString());
                    //Aumento el identificador para evitar repetir salas
                    contadorSalas++;
                    //Añado la sala a la colección
                    salas.Add(sala.IdSala, sala);
                    Thread hiloSala = new Thread(() => salaEspera(sala));
                    hiloSala.Start();
                }
                return true;
            }
            return false;
        }
        public bool entrarSala(Cliente cliente)
        {
            //Comprueba si la sala a la que quiere entrar existe y si tiene menos de 8 clientes
            int sala = Convert.ToInt32(cliente.recibirDatos());
            string nombre = cliente.recibirDatos();
            lock (salas)
            {
                if (salas.ContainsKey(sala) && salas[sala].Clientes.Count < maxClientes && !salas[sala].WaitingRoomFinished)
                {
                    //cliente.enviarDatos("Escribe tu nombre:");
                    //El cliente entra en la sala
                    lock (salas[sala])
                    {
                        if (salas[sala].Clientes.ContainsKey(nombre))
                        {
                            Console.WriteLine("Este nombre ya existe");
                            cliente.enviarDatos("errorNombre");
                            return false;
                        }
                        salas[sala].addCliente(nombre, cliente);
                        Console.WriteLine($"El cliente {nombre} ha entrado en la sala");

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
                    Console.WriteLine($"La sala {sala} no existe");
                    cliente.enviarDatos("errorSala");
                    return false;
                }
            }
        }
        public void gestionCliente(TcpClient socket)
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
                        if (!crearSala(cliente))
                        {
                            gestionado = false;
                        }
                        break;
                    case "join":
                        try
                        {
                            //Gestionamos que el cliente no haya podido entrar en la sala
                            if (!entrarSala(cliente))
                            {
                                //cliente.enviarDatos("La sala a la que intentas entrar o existe o está llena.");
                                gestionado = false;
                            }
                        }
                        catch (FormatException fEx) { }
                        catch (OverflowException oEx) { }
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

        /*******************************************/

        public void refreshConectedPeople(Sala sala)
        {
            while (!sala.WaitingRoomFinished)
            {
                lock (sala)
                {
                    int initialClients = sala.Clientes.Count;
                    foreach (var client in sala.Clientes.ToList())
                    {
                        if (!client.Value.isConected())
                        {
                            sala.deletePlayer(client.Key);
                            client.Value.desconectar();
                        }
                    }
                    if (initialClients != sala.Clientes.Count)
                    {
                        foreach (var client in sala.Clientes)
                        {
                            client.Value.refreshWaitingRoom(sala);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        public void salaEspera(Sala sala)
        {
            //TODO
            //Comprobar cuando se va un cliente
            Cliente host;
            bool correcto = true;
            lock (sala)
            {
                host = sala.Clientes.GetValueOrDefault(sala.NombreHost);
            }
            Thread refreshClientsThread = new Thread(() => refreshConectedPeople(sala));
            refreshClientsThread.Start();
            do
            {
                try
                {
                    string res = host.recibirDatos();
                    Console.WriteLine("Respuesta del cliente: "+res);
                    if (res == "empezar")
                    {
                        lock (sala)
                        {
                            if (sala.Clientes.Count >= 2)
                            {
                                foreach (Cliente client in sala.Clientes.Values)
                                {
                                    client.enviarDatos("start");
                                }
                                sala.WaitingRoomFinished = true;
                            }
                            else
                            {
                                host.enviarDatos("error");
                            }
                        }
                    }else if(res == null)
                    {
                        deletePlayerOnWaitingRoom(sala, ref host, ref correcto);
                    }
                }catch(IOException ex)
                {
                    deletePlayerOnWaitingRoom(sala, ref host, ref correcto);
                }
            } while (!sala.WaitingRoomFinished);
            refreshClientsThread.Join();
            if (correcto)
            {
                partida(sala);
            }
        }
        public void deletePlayerOnWaitingRoom(Sala sala,ref Cliente host,ref bool correcto)
        {
            lock (sala)
            {
                if (sala.Clientes.Count <= 1)
                {
                    host.desconectar();
                    closeRoom(sala);
                    sala.WaitingRoomFinished = true;
                    sala.GameFinished = true;
                    correcto = false;
                    Console.WriteLine($"Se ha cerrado la sala {sala.IdSala}");
                }
                else
                {
                    host.desconectar();
                    sala.Clientes.Remove(sala.NombreHost);
                    sala.PlayersNames.Remove(sala.NombreHost);
                    sala.NombreHost = sala.PlayersNames.First();
                    host = sala.Clientes.GetValueOrDefault(sala.NombreHost);
                    foreach (Cliente client in sala.Clientes.Values)
                    {
                        client.refreshWaitingRoom(sala);
                    }
                    Console.WriteLine($"El nuevo host de la sala {sala.IdSala} es {sala.NombreHost}");
                }
            }
        }
        public void closeRoom(Sala sala)
        {
            lock (salas)
            {
                salas.Remove(sala.IdSala);
            }
        }

        /*******************************************/
        public void refreshGameConectedPeople(Sala sala)
        {
            while (!sala.GameFinished)
            {
                lock (sala)
                {
                    int initialClients = sala.Clientes.Count;
                    foreach (var client in sala.Clientes.ToList())
                    {
                        if (!client.Value.isConected())
                        {
                            sala.deletePlayer(client.Key);
                            client.Value.desconectar();
                        }
                    }
                    if (initialClients != sala.Clientes.Count)
                    {
                        sendData(sala);
                        if (sala.Clientes.Count <= 1)
                        {
                            sala.GameFinished = true;
                            finishPlayer(sala, true);
                            closeRoom(sala);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        public void partida(Sala sala)
        {
            sala.Partida = new Partida();
            Thread refreshClientsThread = new Thread(() => refreshGameConectedPeople(sala));
            refreshClientsThread.Start();
            int playersInGame = sala.Clientes.Count;
            PaqueteTurno paquete;
            int maxCards = 8;
            Console.WriteLine("Generando info");
            foreach(var cl in sala.Clientes)
            {
                List<Carta> baraja = new List<Carta>();
                for (int i=0; i<maxCards; i++)
                {
                    baraja.Add(cartaRandom());
                }
                sala.Partida.BarajasJugadores.Add(cl.Key, baraja);
            }
            sala.Partida.Turno = sala.PlayersNames[0];

            do
            {
                sendData(sala);
                Carta cartaJugada = null;
                if (sala.PlayersNames.Count < 2)
                {
                    sala.GameFinished = true;
                }
                else
                {
                    try
                    {
                        switch (sala.Clientes[sala.Partida.Turno].recibirDatos())
                        {
                            case "jugar":
                                cartaJugada = sala.Clientes[sala.Partida.Turno].recibirCarta();
                                //Borro de su baraja la carta jugada
                                sala.Partida.BarajasJugadores[sala.Partida.Turno].Remove(buscarCarta(cartaJugada, sala.Partida.BarajasJugadores[sala.Partida.Turno]));
                                //La carta realiza su función en mesa
                                switch (cartaJugada.Tipo)
                                {
                                    case Carta.eTipo.Numero:
                                        int dif = 0;
                                        bool overLimit = false;
                                        //En función del sentido de la mesa, sumamos o restamos su valor
                                        if (sala.Partida.SentidoMesa)
                                        {
                                            dif = sala.Partida.ValorMesa + cartaJugada.Valor;
                                            if (dif >= sala.Partida.Limite) {
                                                overLimit = true;
                                                dif -= sala.Partida.Limite; 
                                            }
                                        }
                                        else
                                        {
                                            dif = sala.Partida.ValorMesa - cartaJugada.Valor;
                                            if (dif <= -sala.Partida.Limite) {
                                                overLimit = true;
                                                dif += sala.Partida.Limite; 
                                            }
                                        }
                                        Console.WriteLine("El límite de la mesa es: " + sala.Partida.Limite);
                                        Console.WriteLine($"La operación es: {sala.Partida.ValorMesa}+{cartaJugada.Valor}={cartaJugada.Valor+ sala.Partida.ValorMesa}");
                                        sala.Partida.ValorMesa = dif;
                                        //Si el jugador sobrepasa el límite, se reinicia el valor de mesa y se
                                        //añaden 3 cartas a su baraja.
                                        if (overLimit)
                                        {
                                            for (int i = 0; i < 3; i++)
                                            {
                                                sala.Partida.BarajasJugadores[sala.Partida.Turno].Add(cartaRandom());
                                            }
                                        }
                                        break;
                                    case Carta.eTipo.Sentido:
                                        //Se cambia la operación de la mesa
                                        sala.Partida.SentidoMesa = cartaJugada.Sentido;
                                        break;
                                    case Carta.eTipo.Efecto:
                                        //Cada jugador recibe 2 cartas a excepción del que ha jugado último
                                        foreach (var cl in sala.Partida.BarajasJugadores)
                                        {
                                            if (cl.Key != sala.Partida.Turno)
                                            {
                                                for (int i = 0; i < 2; i++)
                                                {
                                                    cl.Value.Add(cartaRandom());
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                            case "pasar":
                                sala.Partida.BarajasJugadores[sala.Partida.Turno].Add(cartaRandom());
                                break;
                            default:
                                sala.Clientes[sala.Partida.Turno].enviarDatos("Debes jugar o pasar");
                                continue;
                        }
                        string adiosJugador = null;
                        if (sala.Partida.BarajasJugadores[sala.Partida.Turno].Count <= 0)
                        {
                            finishPlayer(sala, false);
                        }
                        else
                        {
                            sala.avanzarTurno();
                        }
                    }
                    catch (IOException ioex)
                    {
                        if (!sala.GameFinished)
                        {
                            deleteDisconnectedPlayer(sala.Partida.Turno, sala);
                        }
                    }
                }
            } while (!sala.GameFinished);
            if (sala.PlayersNames.Count > 0)
            {
                finishPlayer(sala, true);
                closeRoom(sala);
            }
        }
        public void sendData(Sala sala)
        {
            foreach (var cl in sala.Clientes)
            {
                Console.WriteLine("Envio el numero de cartas");
                try
                {
                    //Numero de cartas
                    cl.Value.enviarDatos(sala.Partida.BarajasJugadores[cl.Key].Count.ToString());
                    //Cartas
                    foreach (var carta in sala.Partida.BarajasJugadores[cl.Key])
                    {
                        cl.Value.enviarCarta(carta);
                    }
                    //Valor de mesa
                    cl.Value.enviarDatos(sala.Partida.ValorMesa.ToString());
                    //Sentido Mesa
                    cl.Value.enviarDatos(sala.Partida.SentidoMesa.ToString());
                    //Turno de mesa
                    cl.Value.enviarDatos(sala.Partida.Turno);
                    //Jugadores con su número de cartas
                    cl.Value.enviarDatos(sala.Partida.BarajasJugadores.Count.ToString());
                    foreach (var v in sala.Partida.BarajasJugadores)
                    {
                        cl.Value.enviarDatos(v.Key);
                        cl.Value.enviarDatos(v.Value.Count.ToString());
                    }
                }
                catch (IOException ioex)
                {

                }
            }
        }
        public void deleteDisconnectedPlayer(string cliente, Sala sala)
        {
            sala.Clientes[sala.Partida.Turno].desconectar();
            sala.Clientes.Remove(cliente);
            if (sala.Partida.Turno == cliente)
            {
                sala.avanzarTurno();
            }
            sala.Partida.BarajasJugadores.Remove(cliente);
            sala.PlayersNames.Remove(cliente);
        }
        public void finishPlayer(Sala sala,bool lastPlayer)
        {
            string leavingPlayer = sala.Partida.Turno;
            try
            {
                sala.Clientes[sala.Partida.Turno].enviarDatos("finPartida");
                sala.Clientes[sala.Partida.Turno].enviarDatos(sala.Partida.Ranking+"");
                sala.Partida.Ranking++;
                sala.Clientes[sala.Partida.Turno].desconectar();
            }catch(IOException ex)
            {

            }
            sala.Clientes.Remove(sala.Partida.Turno);
            if (!lastPlayer)
            {
                sala.avanzarTurno();
            }
            sala.Partida.BarajasJugadores.Remove(leavingPlayer);
            sala.PlayersNames.Remove(leavingPlayer);
        }
        //Busca dentro de una baraja una carta concreta y la devuelve.
        public Carta buscarCarta(Carta cartaJugada,List<Carta> baraja)
        {
            foreach(Carta card in baraja)
            {
                if(card.Sentido==cartaJugada.Sentido &&
                    card.Valor==cartaJugada.Valor &&
                    card.Tipo == cartaJugada.Tipo)
                {
                    return card;
                }
            }
            return null;
        }
        public Carta cartaRandom()
        {
            Random rand = new Random();
            int num = 0;
            Carta.eTipo tipo = Carta.eTipo.Numero;
            int valor = 0;
            bool sentido = true;
            num = rand.Next(0, 10);
            //Hay un 70% de probabilidades de que salga Numero
            if (num < 7)
            {
                tipo = Carta.eTipo.Numero;
            }
            //Hay un 20% de probabilidades de que salga Sentido
            else if (num < 9)
            {
                tipo = Carta.eTipo.Sentido;
            }
            //Hay un 10% de probabilidades de que salga Efecto
            else
            {
                tipo = Carta.eTipo.Efecto;
            }
            valor = rand.Next(3, 8);
            sentido = rand.Next(0, 2) == 0 ? true : false;
            return new Carta(tipo, valor, sentido);
        }
    }
}
