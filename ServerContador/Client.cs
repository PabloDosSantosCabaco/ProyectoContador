using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ServerContador
{
    class Client
    {

        /// <summary>
        /// Stream de datos del socket.
        /// </summary>
        NetworkStream ns;
        /// <summary>
        /// Permite la lectura de datos del socket.
        /// </summary>
        StreamWriter sw;
        /// <summary>
        /// Permite la escritura de datos del socket.
        /// </summary>
        StreamReader sr;
        /// <summary>
        /// Permite establecer la conexión con el cliente.
        /// </summary>
        public TcpClient socket;
        /// <summary>
        /// Constructor de la clase Cliente.
        /// </summary>
        /// <param name="socketCliente">Socket con el que se realiza la conexión.</param>
        public Client(TcpClient socketCliente)
        {
            socket = socketCliente;
            ns = socket.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
        }
        /// <summary>
        /// Envía datos al cliente.
        /// </summary>
        /// <param name="data">Datos a enviar.</param>
        public void sendData(string data)
        {
            sw.WriteLine(data);
            sw.Flush();
        }
        /// <summary>
        /// Comprueba si el cliente sigue conectado.
        /// </summary>
        /// <returns>Devuelve true si está conectado.</returns>
        public bool isConected()
        {
            return !(socket.Client.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }
        /// <summary>
        /// Recibe datos del cliente.
        /// </summary>
        /// <returns>Datos recibidos.</returns>
        public string getData()
        {
            return sr.ReadLine();
        }
        /// <summary>
        /// Envia una carta al cliente.
        /// </summary>
        /// <param name="card">Carta a enviar.</param>
        public void sendCard(Card card)
        {
            sendData(card.Type.ToString());
            sendData(card.Value.ToString());
            sendData(card.Way.ToString());
        }
        /// <summary>
        /// Devuelve una carta recibida.
        /// </summary>
        /// <returns>Carta recibida.</returns>
        public Card getCard()
        {
            Card.eType type = (Card.eType)Enum.Parse(typeof(Card.eType), sr.ReadLine());
            int value = Convert.ToInt32(sr.ReadLine());
            bool way = Convert.ToBoolean(sr.ReadLine());
            return new Card(type,value,way);
        }
        /// <summary>
        /// Se encarga de avisar a las salas que ha entrado o salido un jugador.
        /// </summary>
        /// <param name="room">Sala afectada.</param>
        public void refreshWaitingRoom(Room room)
        {
            sendData("players");
            sendData(room.Clients.Count.ToString());
            foreach (string nombreJugador in room.PlayersNames)
            {
                sendData(nombreJugador);
            }
            sendData(room.HostName);
        }
        /// <summary>
        /// Se encarga de cerrar el socket.
        /// </summary>
        public void disconnect()
        {
            sw.Close();
            sr.Close();
            ns.Close();
            socket.Close();
        }
    }
}
