using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServidorContador
{
    class Juego
    {
        public static void refreshGameConectedPeople(Sala sala,Dictionary<int,Sala> salas)
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
                            SalaEspera.closeRoom(sala,salas);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        public static void partida(Sala sala,Dictionary<int,Sala> salas)
        {
            sala.Partida = new Partida();
            Thread refreshClientsThread = new Thread(() => refreshGameConectedPeople(sala,salas));
            refreshClientsThread.Start();
            int playersInGame = sala.Clientes.Count;
            int maxCards = 8;
            foreach (var cl in sala.Clientes)
            {
                List<Carta> baraja = new List<Carta>();
                for (int i = 0; i < maxCards; i++)
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
                                            if (dif >= sala.Partida.Limite)
                                            {
                                                overLimit = true;
                                                dif -= sala.Partida.Limite;
                                            }
                                        }
                                        else
                                        {
                                            dif = sala.Partida.ValorMesa - cartaJugada.Valor;
                                            if (dif <= -sala.Partida.Limite)
                                            {
                                                overLimit = true;
                                                dif += sala.Partida.Limite;
                                            }
                                        }
                                        Console.WriteLine("El límite de la mesa es: " + sala.Partida.Limite);
                                        Console.WriteLine($"La operación es: {sala.Partida.ValorMesa}+{cartaJugada.Valor}={cartaJugada.Valor + sala.Partida.ValorMesa}");
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
                        if (sala.Partida.BarajasJugadores[sala.Partida.Turno].Count <= 0)
                        {
                            finishPlayer(sala, false);
                        }
                        else
                        {
                            sala.avanzarTurno();
                        }
                    }
                    catch (IOException)
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
                SalaEspera.closeRoom(sala,salas);
            }
        }
        public static void sendData(Sala sala)
        {
            foreach (var cl in sala.Clientes)
            {
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
                catch (IOException)
                {
                }
            }
        }
        public static void deleteDisconnectedPlayer(string cliente, Sala sala)
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
        public static void finishPlayer(Sala sala, bool lastPlayer)
        {
            string leavingPlayer = sala.Partida.Turno;
            try
            {
                sala.Clientes[sala.Partida.Turno].enviarDatos("finPartida");
                sala.Clientes[sala.Partida.Turno].enviarDatos(sala.Partida.Ranking + "");
                sala.Partida.Ranking++;
                sala.Clientes[sala.Partida.Turno].desconectar();
            }
            catch (IOException)
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
        public static Carta buscarCarta(Carta cartaJugada, List<Carta> baraja)
        {
            foreach (Carta card in baraja)
            {
                if (card.Sentido == cartaJugada.Sentido &&
                    card.Valor == cartaJugada.Valor &&
                    card.Tipo == cartaJugada.Tipo)
                {
                    return card;
                }
            }
            return null;
        }
        public static Carta cartaRandom()
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
