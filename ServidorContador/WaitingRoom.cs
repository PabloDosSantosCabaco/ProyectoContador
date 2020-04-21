using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ServerContador
{
    class WaitingRoom
    {
        public static void refreshConectedPeople(Room room)
        {
            while (!room.WaitingRoomFinished)
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
                        foreach (var client in room.Clients)
                        {
                            client.Value.refreshWaitingRoom(room);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        public static void waitingRoom(Room room,Dictionary<int,Room> rooms)
        {
            Client host;
            bool correct = true;
            lock (room)
            {
                host = room.Clients.GetValueOrDefault(room.HostName);
            }
            Thread refreshClientsThread = new Thread(() => refreshConectedPeople(room));
            refreshClientsThread.Start();
            do
            {
                try
                {
                    string res = host.getData();
                    if (res == "empezar")
                    {
                        lock (room)
                        {
                            if (room.Clients.Count >= 2)
                            {
                                foreach (Client client in room.Clients.Values)
                                {
                                    client.sendData("start");
                                }
                                room.WaitingRoomFinished = true;
                            }
                            else
                            {
                                host.sendData("error");
                            }
                        }
                    }
                    else if (res == null)
                    {
                        deletePlayerOnWaitingRoom(room, ref host, ref correct,rooms);
                    }
                }
                catch (IOException)
                {
                    deletePlayerOnWaitingRoom(room, ref host, ref correct,rooms);
                }
            } while (!room.WaitingRoomFinished);
            refreshClientsThread.Join();
            if (correct)
            {
                Game.partida(room,rooms);
            }
        }
        public static void deletePlayerOnWaitingRoom(Room room, ref Client host, ref bool correct,Dictionary<int, Room> rooms)
        {
            lock (room)
            {
                if (room.Clients.Count <= 1)
                {
                    host.disconnect();
                    closeRoom(room,rooms);
                    room.WaitingRoomFinished = true;
                    room.GameFinished = true;
                    correct = false;
                    Console.WriteLine($"Room {room.IdRoom} closed");
                }
                else
                {
                    host.disconnect();
                    room.Clients.Remove(room.HostName);
                    room.PlayersNames.Remove(room.HostName);
                    room.HostName = room.PlayersNames.First();
                    host = room.Clients.GetValueOrDefault(room.HostName);
                    foreach (Client client in room.Clients.Values)
                    {
                        client.refreshWaitingRoom(room);
                    }
                    Console.WriteLine($"{room.HostName} is the new host of room {room.IdRoom}");
                }
            }
        }
        public static void closeRoom(Room room,Dictionary<int,Room> rooms)
        {
            lock (rooms)
            {
                rooms.Remove(room.IdRoom);
            }
        }
    }
}
