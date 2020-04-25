using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerContador
{
    class GestionClientes
    {
        public static bool createRoom(Client client, Dictionary<int, Room> rooms,ref int roomsCount)
        {
            string name = client.getData();
            if (name.Trim().Length >= 3)
            {
                //Creo la nueva sala
                lock (rooms)
                {
                    Room room = new Room(roomsCount, name, client);
                    client.sendData(roomsCount.ToString());
                    //Aumento el identificador para evitar repetir salas
                    roomsCount++;
                    //Añado la sala a la colección
                    rooms.Add(room.IdRoom, room);
                    Thread hiloSala = new Thread(() => WaitingRoom.waitingRoom(room,rooms));
                    hiloSala.Start();
                    Console.WriteLine($"El usuario {name} ha creado la sala {roomsCount-1}");
                }
                return true;
            }
            return false;
        }
        public static bool joinRoom(Client client, Dictionary<int, Room> rooms)
        {
            //Comprueba si la sala a la que quiere entrar existe y si tiene menos de 8 clientes
            int room = -1;
            string name = null;
            try
            {
                room = Convert.ToInt32(client.getData());
                name = client.getData();
            }
            catch (FormatException) { }
            catch(OverflowException) { }
            lock (rooms)
            {
                if (rooms.ContainsKey(room) && rooms[room].Clients.Count < 8 && !rooms[room].WaitingRoomFinished)
                {
                    //El cliente entra en la sala
                    lock (rooms[room])
                    {
                        if (name==null || name.Trim().Length<3 || rooms[room].Clients.ContainsKey(name))
                        {
                            Console.WriteLine("Este nombre ya existe");
                            client.sendData("errorNombre");
                            return false;
                        }
                        rooms[room].addCliente(name, client);
                        Console.WriteLine($"El cliente {name} ha entrado en la sala {room}");

                        client.sendData("true");
                        foreach (Client cl in rooms[room].Clients.Values)
                        {
                            cl.refreshWaitingRoom(rooms[room]);
                        }
                    }
                    return true;
                }
                else
                {
                    client.sendData("errorSala");
                    return false;
                }
            }
        }
        public static void manageClient(TcpClient socket,Dictionary<int,Room> rooms,ref int roomCounter)
        {
            Console.WriteLine("New client appears");
            Client client = new Client(socket);
            bool done;
            //Decidimos si crea o se une
            try
            {
                do
                {
                    done = true;
                    string res = client.getData();
                    //Decidimos si crea o se une
                    switch (res)
                    {
                        case "new":
                            //Creamos la sala metiendo al primer cliente como host
                            if (!createRoom(client, rooms, ref roomCounter))
                            {
                                done = false;
                            }
                            break;
                        case "join":
                            //Gestionamos que el cliente no haya podido entrar en la sala
                            if (!joinRoom(client, rooms))
                            {
                                //cliente.enviarDatos("La sala a la que intentas entrar o existe o está llena.");
                                done = false;
                            }
                            break;
                        case null:
                            client.sendData("Cliente desconectado");
                            break;
                        default:
                            client.sendData("Comando no soportado");
                            done = false;
                            break;
                    }
                } while (!done);
            }catch(IOException ex)
            {
                client.disconnect();
                Console.WriteLine($"Error: {ex}");
            }
        }
    }
}
