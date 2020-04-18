using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServidorContador
{
    class SalaEspera
    {
        public static void refreshConectedPeople(Sala sala)
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
        public static void salaEspera(Sala sala,Dictionary<int,Sala> salas)
        {
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
                    }
                    else if (res == null)
                    {
                        deletePlayerOnWaitingRoom(sala, ref host, ref correcto,salas);
                    }
                }
                catch (IOException)
                {
                    deletePlayerOnWaitingRoom(sala, ref host, ref correcto,salas);
                }
            } while (!sala.WaitingRoomFinished);
            refreshClientsThread.Join();
            if (correcto)
            {
                Juego.partida(sala,salas);
            }
        }
        public static void deletePlayerOnWaitingRoom(Sala sala, ref Cliente host, ref bool correcto,Dictionary<int, Sala> salas)
        {
            lock (sala)
            {
                if (sala.Clientes.Count <= 1)
                {
                    host.desconectar();
                    closeRoom(sala,salas);
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
        public static void closeRoom(Sala sala,Dictionary<int,Sala> salas)
        {
            lock (salas)
            {
                salas.Remove(sala.IdSala);
            }
        }
    }
}
