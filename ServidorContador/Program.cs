﻿using System;
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
        IPEndPoint ie = new IPEndPoint(IPAddress.Any, 31416);
        Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
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
                Thread hiloGestionCliente = new Thread(() => gestionCliente(sCliente));
                hiloGestionCliente.Start();
            }
            s.Close();
        }
        public bool crearSala(Cliente cliente)
        {
            string nombre;
            nombre=cliente.recibirDatos();
            Console.WriteLine("Mi cliente se llama " + nombre);
            if (nombre.Trim().Length >= 3)
            {
                //Creo la nueva sala
                Sala sala = new Sala(contadorSalas, nombre, cliente);
                Console.WriteLine("Devuelvo el numero de sala: " + contadorSalas);
                cliente.enviarDatos(contadorSalas.ToString());
                //Aumento el identificador para evitar repetir salas
                contadorSalas++;
                //Añado la sala a la colección
                salas.Add(sala.IdSala, sala);
                Thread hiloSala = new Thread(() => salaEspera(sala));
                hiloSala.Start();
                return true;
            }
            return false;
        }
        public bool entrarSala(Cliente cliente)
        {
            //Comprueba si la sala a la que quiere entrar existe y si tiene menos de 8 clientes
            int sala = Convert.ToInt32(cliente.recibirDatos());
            string nombre = cliente.recibirDatos();
            if (salas.ContainsKey(sala) && salas.GetValueOrDefault(sala).Clientes.Count<maxClientes)
            {
                //cliente.enviarDatos("Escribe tu nombre:");
                //El cliente entra en la sala
                lock (salas.GetValueOrDefault(sala)) {
                    if (salas.GetValueOrDefault(sala).Clientes.ContainsKey(nombre))
                    {
                        Console.WriteLine("Este nombre ya existe");
                        cliente.enviarDatos("errorNombre");
                        return false;
                    }
                    salas.GetValueOrDefault(sala).addCliente(nombre,cliente);
                    Console.WriteLine($"El cliente {nombre} ha entrado en la sala");
                    cliente.enviarDatos("true");
                    foreach(Cliente client in salas.GetValueOrDefault(sala).Clientes.Values)
                    {
                        client.refreshWaitingRoom(salas.GetValueOrDefault(sala));
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
        public void gestionCliente(Socket socket)
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
        public void salaEspera(Sala sala)
        {
            //TODO
            //Comprobar cuando se va un cliente
            //¿Qué pasa si se va el host? ¿Cómo se gestiona al resto de clientes?
            Cliente host;
            bool correcto = true;
            lock (sala)
            {
                host = sala.Clientes.GetValueOrDefault(sala.NombreHost);
            }
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
                    }
                }catch(IOException ex)
                {
                    lock (sala)
                    {
                        if (sala.Clientes.Count <= 1)
                        {
                            sala.Acabado = true;
                            correcto = false;
                            Console.WriteLine($"Se ha cerrado la sala {sala.IdSala}");
                        }
                        else
                        {
                            sala.Clientes.Remove(sala.NombreHost);
                            sala.PlayersNames.Remove(sala.NombreHost);
                            sala.NombreHost = sala.PlayersNames.First();
                            host = sala.Clientes.GetValueOrDefault(sala.NombreHost);
                            foreach(Cliente client in sala.Clientes.Values)
                            {
                                client.refreshWaitingRoom(sala);
                            }
                            Console.WriteLine($"El nuevo host de la sala {sala.IdSala} es {sala.NombreHost}");
                            
                        }
                    }
                }
            } while (!sala.Acabado);
            if (correcto)
            {
                partida(sala);
            }
        }
        public void partida(Sala sala)
        {
            Partida partida = new Partida();
            int playersInGame = sala.Clientes.Count;
            PaqueteTurno paquete;
            List<string> nombresJugadores = new List<string>();
            bool partidaAcabada = false;
            int maxCards = 5;
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
                foreach (var cl in sala.Clientes)
                {
                    Console.WriteLine("Envio el numero de cartas");
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
                    foreach(var v in partida.BarajasJugadores)
                    {
                        cl.Value.enviarDatos(v.Key);
                        cl.Value.enviarDatos(v.Value.Count.ToString());
                    }
                }
                Carta cartaJugada = null;
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
                                //En función del sentido de la mesa, sumamos o restamos su valor
                                if (partida.SentidoMesa)
                                {
                                    partida.ValorMesa += cartaJugada.Valor;
                                }
                                else
                                {
                                    partida.ValorMesa -= cartaJugada.Valor;
                                }
                                //Si el jugador sobrepasa el límite, se reinicia el valor de mesa y se
                                //añaden 3 cartas a su baraja.
                                if(partida.ValorMesa>=partida.Limite || partida.ValorMesa <= -partida.Limite)
                                {
                                    partida.ValorMesa = 0;
                                    for(int i=0; i<3; i++)
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
                //Compruebo si el último jugador tiene cartas.
                //En caso de no tener, guardo su nombre para borrarlo de la
                //lista de nombres de jugadores y borrarlo como cliente de la sala.
                string adiosJugador = null;
                if (partida.BarajasJugadores[partida.Turno].Count <= 0)
                {
                    adiosJugador = partida.Turno;
                    sala.Clientes[partida.Turno].enviarDatos("finPartida");
                    sala.Clientes[partida.Turno].enviarDatos(""+(playersInGame-sala.Clientes.Count+1));
                    sala.Clientes[partida.Turno].desconectar();
                    sala.Clientes.Remove(partida.Turno);
                }
                //Avanzar turno
                partida.Turno = avanzarTurno(nombresJugadores, partida.Turno);
                if (adiosJugador != null)
                {
                    partida.BarajasJugadores.Remove(adiosJugador);
                    nombresJugadores.Remove(adiosJugador);
                }
                if (nombresJugadores.Count < 2)
                {
                    partidaAcabada = true;
                }
            } while (!partidaAcabada);
            foreach(var cl in sala.Clientes)
            {
                sala.Clientes[partida.Turno].enviarDatos("finPartida");
                sala.Clientes[partida.Turno].enviarDatos("" + (playersInGame - sala.Clientes.Count + 1));
                cl.Value.desconectar();
            }
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
