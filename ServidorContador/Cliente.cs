using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ServidorContador
{
    class Cliente
    {

        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        public TcpClient socket;

        public Cliente(TcpClient socketCliente)
        {
            socket = socketCliente;
            ns = socket.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
        }
        public void enviarDatos(string datos)
        {
            sw.WriteLine(datos);
            sw.Flush();
        }
        public bool isConected()
        {
            return !(socket.Client.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }
        public string recibirDatos()
        {
            return sr.ReadLine();
        }
        public void enviarCarta(Carta carta)
        {
            enviarDatos(carta.Tipo.ToString());
            enviarDatos(carta.Valor.ToString());
            enviarDatos(carta.Sentido.ToString());
        }
        public Carta recibirCarta()
        {
            Carta.eTipo tipo = (Carta.eTipo)Enum.Parse(typeof(Carta.eTipo), sr.ReadLine());
            int valor = Convert.ToInt32(sr.ReadLine());
            bool sentido = Convert.ToBoolean(sr.ReadLine());
            return new Carta(tipo,valor,sentido);
        }
        public void refreshWaitingRoom(Sala sala)
        {
            enviarDatos("players");
            enviarDatos(sala.Clientes.Count.ToString());
            foreach (string nombreJugador in sala.PlayersNames)
            {
                enviarDatos(nombreJugador);
            }
            enviarDatos(sala.NombreHost);
        }
        public void desconectar()
        {
            sw.Close();
            sr.Close();
            ns.Close();
            socket.Close();
        }
    }
}
