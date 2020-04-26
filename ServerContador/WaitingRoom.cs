using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ServerContador
{
    class WaitingRoom
    {
        /// <summary>
        /// Bloquea la sala y comprueba si los clientes están conectados. Si no
        /// lo están, los eliminar de la sala y cierran sus sockets.
        /// </summary>
        /// <param name="room">Sala a comprobar.</param>
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
        /// <summary>
        /// Función que gestiona la actividad en la sala de espera. Espera la respuesta del host que 
        /// tan solo puede desconectarse o iniciar la partida.
        /// </summary>
        /// <param name="room">Sala a gestionar.</param>
        /// <param name="rooms">Colección de salas.</param>
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
        /// <summary>
        /// Borra el host de la sala y en caso de estar solo, borra la sala. Sino, nombre un nuevo host.
        /// </summary>
        /// <param name="room">Sala donde borrar el host.</param>
        /// <param name="host">Host a borrar.</param>
        /// <param name="correct">Indica si la partida puede llegar a comenzarse.</param>
        /// <param name="rooms">Colección de salas.</param>
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
        /// <summary>
        /// Cierra la sala pasada como parámetro.
        /// </summary>
        /// <param name="room">Sala a cerrar.</param>
        /// <param name="rooms">Colección de salas.</param>
        public static void closeRoom(Room room,Dictionary<int,Room> rooms)
        {
            lock (rooms)
            {
                rooms.Remove(room.IdRoom);
            }
        }
    }
}
