using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ServerContador
{
    class Client
    {

        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        public TcpClient socket;

        public Client(TcpClient socketCliente)
        {
            socket = socketCliente;
            ns = socket.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
        }
        public void sendData(string data)
        {
            sw.WriteLine(data);
            sw.Flush();
        }
        public bool isConected()
        {
            return !(socket.Client.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }
        public string getData()
        {
            return sr.ReadLine();
        }
        public void sendCard(Card card)
        {
            sendData(card.Type.ToString());
            sendData(card.Value.ToString());
            sendData(card.Way.ToString());
        }
        public Card getCard()
        {
            Card.eType type = (Card.eType)Enum.Parse(typeof(Card.eType), sr.ReadLine());
            int value = Convert.ToInt32(sr.ReadLine());
            bool way = Convert.ToBoolean(sr.ReadLine());
            return new Card(type,value,way);
        }
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
        public void disconnect()
        {
            sw.Close();
            sr.Close();
            ns.Close();
            socket.Close();
        }
    }
}
