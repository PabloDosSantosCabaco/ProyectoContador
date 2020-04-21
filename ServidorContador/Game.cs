using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerContador
{
    class Game
    {
        public static void refreshGameConectedPeople(Room room,Dictionary<int,Room> rooms)
        {
            while (!room.GameFinished)
            {
                lock (room)
                {
                    int initialClients = room.Clients.Count;
                    foreach (var client in room.Clients.ToList())
                    {
                        if (!client.Value.isConected())
                        {
                            room.deletePlayer(client.Key);
                            client.Value.disconnect();
                        }
                    }
                    if (initialClients != room.Clients.Count)
                    {
                        sendData(room);
                        if (room.Clients.Count <= 1)
                        {
                            room.GameFinished = true;
                            finishPlayer(room, true);
                            WaitingRoom.closeRoom(room,rooms);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        public static void partida(Room room,Dictionary<int,Room> rooms)
        {
            room.Match = new Match();
            Thread refreshClientsThread = new Thread(() => refreshGameConectedPeople(room,rooms));
            refreshClientsThread.Start();
            int playersInGame = room.Clients.Count;
            int maxCards = 8;
            foreach (var cl in room.Clients)
            {
                List<Card> deck = new List<Card>();
                for (int i = 0; i < maxCards; i++)
                {
                    deck.Add(randomCard());
                }
                room.Match.PlayersDeck.Add(cl.Key, deck);
            }
            room.Match.Turn = room.PlayersNames[0];

            do
            {
                sendData(room);
                Card cardPlayed = null;
                if (room.PlayersNames.Count < 2)
                {
                    room.GameFinished = true;
                }
                else
                {
                    try
                    {
                        switch (room.Clients[room.Match.Turn].getData())
                        {
                            case "jugar":
                                cardPlayed = room.Clients[room.Match.Turn].getCard();
                                //Borro de su baraja la carta jugada
                                room.Match.PlayersDeck[room.Match.Turn].Remove(searchCard(cardPlayed, room.Match.PlayersDeck[room.Match.Turn]));
                                //La carta realiza su función en mesa
                                switch (cardPlayed.Type)
                                {
                                    case Card.eType.Number:
                                        int dif = 0;
                                        bool overLimit = false;
                                        //En función del sentido de la mesa, sumamos o restamos su valor
                                        if (room.Match.TableWay)
                                        {
                                            dif = room.Match.TableValue + cardPlayed.Value;
                                            if (dif >= room.Match.Limit)
                                            {
                                                overLimit = true;
                                                dif -= room.Match.Limit;
                                            }
                                        }
                                        else
                                        {
                                            dif = room.Match.TableValue - cardPlayed.Value;
                                            if (dif <= -room.Match.Limit)
                                            {
                                                overLimit = true;
                                                dif += room.Match.Limit;
                                            }
                                        }
                                        Console.WriteLine("Table limit: " + room.Match.Limit);
                                        Console.WriteLine($"Operation in this last turn: {room.Match.TableValue}+{cardPlayed.Value}={cardPlayed.Value + room.Match.TableValue}");
                                        room.Match.TableValue = dif;
                                        //Si el jugador sobrepasa el límite, se reinicia el valor de mesa y se
                                        //añaden 3 cartas a su baraja.
                                        if (overLimit)
                                        {
                                            for (int i = 0; i < 3; i++)
                                            {
                                                room.Match.PlayersDeck[room.Match.Turn].Add(randomCard());
                                            }
                                        }
                                        break;
                                    case Card.eType.Way:
                                        //Se cambia la operación de la mesa
                                        room.Match.TableWay = cardPlayed.Way;
                                        break;
                                    case Card.eType.Effect:
                                        //Cada jugador recibe 2 cartas a excepción del que ha jugado último
                                        foreach (var cl in room.Match.PlayersDeck)
                                        {
                                            if (cl.Key != room.Match.Turn)
                                            {
                                                for (int i = 0; i < 2; i++)
                                                {
                                                    cl.Value.Add(randomCard());
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                            case "pasar":
                                room.Match.PlayersDeck[room.Match.Turn].Add(randomCard());
                                break;
                            default:
                                room.Clients[room.Match.Turn].sendData("You must play or pass");
                                continue;
                        }
                        if (room.Match.PlayersDeck[room.Match.Turn].Count <= 0)
                        {
                            finishPlayer(room, false);
                        }
                        else
                        {
                            room.nextTurn();
                        }
                    }
                    catch (IOException)
                    {
                        if (!room.GameFinished)
                        {
                            deleteDisconnectedPlayer(room.Match.Turn, room);
                        }
                    }
                }
            } while (!room.GameFinished);
            if (room.PlayersNames.Count > 0)
            {
                finishPlayer(room, true);
                WaitingRoom.closeRoom(room,rooms);
            }
        }
        public static void sendData(Room room)
        {
            foreach (var cl in room.Clients)
            {
                try
                {
                    //Numero de cartas
                    cl.Value.sendData(room.Match.PlayersDeck[cl.Key].Count.ToString());
                    //Cartas
                    foreach (var carta in room.Match.PlayersDeck[cl.Key])
                    {
                        cl.Value.sendCard(carta);
                    }
                    //Valor de mesa
                    cl.Value.sendData(room.Match.TableValue.ToString());
                    //Sentido Mesa
                    cl.Value.sendData(room.Match.TableWay.ToString());
                    //Turno de mesa
                    cl.Value.sendData(room.Match.Turn);
                    //Jugadores con su número de cartas
                    cl.Value.sendData(room.Match.PlayersDeck.Count.ToString());
                    foreach (var v in room.Match.PlayersDeck)
                    {
                        cl.Value.sendData(v.Key);
                        cl.Value.sendData(v.Value.Count.ToString());
                    }
                }
                catch (IOException)
                {
                }
            }
        }
        public static void deleteDisconnectedPlayer(string client, Room room)
        {
            room.Clients[room.Match.Turn].disconnect();
            room.Clients.Remove(client);
            if (room.Match.Turn == client)
            {
                room.nextTurn();
            }
            room.Match.PlayersDeck.Remove(client);
            room.PlayersNames.Remove(client);
        }
        public static void finishPlayer(Room room, bool lastPlayer)
        {
            string leavingPlayer = room.Match.Turn;
            try
            {
                room.Clients[room.Match.Turn].sendData("finPartida");
                room.Clients[room.Match.Turn].sendData(room.Match.Ranking + "");
                room.Match.Ranking++;
                room.Clients[room.Match.Turn].disconnect();
            }
            catch (IOException)
            {

            }
            room.Clients.Remove(room.Match.Turn);
            if (!lastPlayer)
            {
                room.nextTurn();
            }
            room.Match.PlayersDeck.Remove(leavingPlayer);
            room.PlayersNames.Remove(leavingPlayer);
        }
        //Busca dentro de una baraja una carta concreta y la devuelve.
        public static Card searchCard(Card playedCard, List<Card> deck)
        {
            foreach (Card card in deck)
            {
                if (card.Way == playedCard.Way &&
                    card.Value == playedCard.Value &&
                    card.Type == playedCard.Type)
                {
                    return card;
                }
            }
            return null;
        }
        public static Card randomCard()
        {
            Random rand = new Random();
            Card.eType type;
            int num = rand.Next(0, 10);
            //Hay un 70% de probabilidades de que salga Numero
            if (num < 7)
            {
                type = Card.eType.Number;
            }
            //Hay un 20% de probabilidades de que salga Sentido
            else if (num < 9)
            {
                type = Card.eType.Way;
            }
            //Hay un 10% de probabilidades de que salga Efecto
            else
            {
                type = Card.eType.Effect;
            }
            return new Card(
                type, 
                rand.Next(3, 8), 
                rand.Next(0, 2) == 0 ? true : false
            );
        }
    }
}
