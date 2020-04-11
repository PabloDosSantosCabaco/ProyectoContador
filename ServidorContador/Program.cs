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
                if (salas.ContainsKey(sala) && salas[sala].Clientes.Count < maxClientes && !salas[sala].Acabado)
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
            while (!sala.Acabado)
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
                                sala.Acabado = true;
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
            } while (!sala.Acabado);
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
                    sala.Acabado = true;
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

        public void partida(Sala sala)
        {
            int ranking = 1;
            Partida partida = new Partida();
            int playersInGame = sala.Clientes.Count;
            PaqueteTurno paquete;
            List<string> nombresJugadores = new List<string>();
            bool partidaAcabada = false;
            int maxCards = 8;
            Console.WriteLine("Generando info");
            foreach(var cl in sala.Clientes)
            {
                List<Carta> baraja = new List<Carta>();
                for (int i=0; i<maxCards; i++)
                {
                    baraja.Add(cartaRandom());
                }
                partida.BarajasJugadores.Add(cl.Key, baraja);
                nombresJugadores.Add(cl.Key);
            }
            partida.Turno = nombresJugadores[0];

            do
            {
                sendCards(partida, sala);
                Carta cartaJugada = null;
                if (nombresJugadores.Count < 2)
                {
                    partidaAcabada = true;
                }
                else
                {
                    try
                    {
                        switch (sala.Clientes[partida.Turno].recibirDatos())
                        {
                            case "jugar":
                                cartaJugada = sala.Clientes[partida.Turno].recibirCarta();
                                //Borro de su baraja la carta jugada
                                partida.BarajasJugadores[partida.Turno].Remove(buscarCarta(cartaJugada, partida.BarajasJugadores[partida.Turno]));
                                //La carta realiza su función en mesa
                                switch (cartaJugada.Tipo)
                                {
                                    case Carta.eTipo.Numero:
                                        int dif = 0;
                                        bool overLimit = false;
                                        //En función del sentido de la mesa, sumamos o restamos su valor
                                        if (partida.SentidoMesa)
                                        {
                                            dif = partida.ValorMesa + cartaJugada.Valor;
                                            if (dif >= partida.Limite) {
                                                overLimit = true;
                                                dif -= partida.Limite; 
                                            }
                                        }
                                        else
                                        {
                                            dif = partida.ValorMesa - cartaJugada.Valor;
                                            if (dif <= -partida.Limite) {
                                                overLimit = true;
                                                dif += partida.Limite; 
                                            }
                                        }
                                        Console.WriteLine("El límite de la mesa es: " + partida.Limite);
                                        Console.WriteLine($"La operación es: {partida.ValorMesa}+{cartaJugada.Valor}={cartaJugada.Valor+partida.ValorMesa}");
                                        partida.ValorMesa = dif;
                                        //Si el jugador sobrepasa el límite, se reinicia el valor de mesa y se
                                        //añaden 3 cartas a su baraja.
                                        if (overLimit)
                                        {
                                            for (int i = 0; i < 3; i++)
                                            {
                                                partida.BarajasJugadores[partida.Turno].Add(cartaRandom());
                                            }
                                        }
                                        break;
                                    case Carta.eTipo.Sentido:
                                        //Se cambia la operación de la mesa
                                        partida.SentidoMesa = cartaJugada.Sentido;
                                        break;
                                    case Carta.eTipo.Efecto:
                                        //Cada jugador recibe 2 cartas a excepción del que ha jugado último
                                        foreach (var cl in partida.BarajasJugadores)
                                        {
                                            if (cl.Key != partida.Turno)
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
                                partida.BarajasJugadores[partida.Turno].Add(cartaRandom());
                                break;
                            default:
                                sala.Clientes[partida.Turno].enviarDatos("Debes jugar o pasar");
                                continue;
                        }
                        string adiosJugador = null;
                        if (partida.BarajasJugadores[partida.Turno].Count <= 0)
                        {
                            finishPlayer(partida, sala, nombresJugadores, false,ref ranking);
                        }
                        else
                        {
                            partida.Turno = avanzarTurno(nombresJugadores, partida.Turno);
                        }
                    }
                    catch (IOException ioex)
                    {
                        deleteDisconnectedPlayer(partida.Turno, partida, sala, nombresJugadores);
                    }
                }
                //Compruebo si el último jugador tiene cartas.
                //En caso de no tener, guardo su nombre para borrarlo de la
                //lista de nombres de jugadores y borrarlo como cliente de la sala.
            } while (!partidaAcabada);
            finishPlayer(partida, sala, nombresJugadores, true,ref ranking);
            closeRoom(sala);
        }
        public void sendCards(Partida partida,Sala sala)
        {
            foreach (var cl in sala.Clientes)
            {
                Console.WriteLine("Envio el numero de cartas");
                try
                {
                    //Numero de cartas
                    cl.Value.enviarDatos(partida.BarajasJugadores[cl.Key].Count.ToString());
                    //Cartas
                    foreach (var carta in partida.BarajasJugadores[cl.Key])
                    {
                        cl.Value.enviarCarta(carta);
                    }
                    //Valor de mesa
                    cl.Value.enviarDatos(partida.ValorMesa.ToString());
                    //Sentido Mesa
                    cl.Value.enviarDatos(partida.SentidoMesa.ToString());
                    //Turno de mesa
                    cl.Value.enviarDatos(partida.Turno);
                    //Jugadores con su número de cartas
                    cl.Value.enviarDatos(partida.BarajasJugadores.Count.ToString());
                    foreach (var v in partida.BarajasJugadores)
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
        public void deleteDisconnectedPlayer(string cliente, Partida partida, Sala sala, List<string> nombresJugadores)
        {
            sala.Clientes[partida.Turno].desconectar();
            sala.Clientes.Remove(cliente);
            if (partida.Turno == cliente)
            {
                partida.Turno = avanzarTurno(nombresJugadores, cliente);
            }
            partida.BarajasJugadores.Remove(cliente);
            nombresJugadores.Remove(cliente);
        }
        public void finishPlayer(Partida partida,Sala sala,List<string> nombresJugadores,bool lastPlayer,ref int ranking)
        {
            string leavingPlayer = partida.Turno;
            try
            {
                sala.Clientes[partida.Turno].enviarDatos("finPartida");
                sala.Clientes[partida.Turno].enviarDatos(ranking+"");
                ranking++;
                sala.Clientes[partida.Turno].desconectar();
            }catch(IOException ex)
            {

            }
            sala.Clientes.Remove(partida.Turno);
            if (!lastPlayer)
            {
                partida.Turno = avanzarTurno(nombresJugadores, partida.Turno);
            }
            partida.BarajasJugadores.Remove(leavingPlayer);
            nombresJugadores.Remove(leavingPlayer);
        }
        public string avanzarTurno(List<string> nombres,string turno)
        {
            return nombres.IndexOf(turno) + 1 >= nombres.Count ? nombres[0] : nombres[nombres.IndexOf(turno) + 1];
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
