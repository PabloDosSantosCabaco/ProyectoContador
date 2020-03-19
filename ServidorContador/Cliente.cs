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
    }
}
