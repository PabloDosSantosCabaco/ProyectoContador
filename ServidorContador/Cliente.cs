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
        Socket socket;



        public Cliente(Socket socketCliente)
        {
            socket = socketCliente;
            ns = new NetworkStream(socketCliente);
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
        }
        public void enviarDatos(string datos)
        {
            sw.WriteLine(datos);
            sw.Flush();
        }
        public string recibirDatos()
        {
            return sr.ReadLine();
        }
        public string getIP()
        {
            return socket.RemoteEndPoint.ToString();
        }
        public void enviarPaquete(PaqueteTurno paquete)
        {

        }
        public Carta recibirCarta()
        {
            Carta.eTipo tipo = (Carta.eTipo)Enum.Parse(typeof(Carta.eTipo), sr.ReadLine());
            int valor = Convert.ToInt32(sr.ReadLine());
            bool sentido = Convert.ToBoolean(sr.ReadLine());
            return new Carta(tipo,valor,sentido);
        }
    }
}
