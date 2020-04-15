using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    class Servidor
    {
        TcpClient client;
        NetworkStream ns;
        StreamWriter sw;
        StreamReader sr;

        public Servidor()
        {
            client = new TcpClient("127.0.0.1", 20000);
            ns = client.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
        }
        public void closeServer()
        {
            sw.Close();
            sr.Close();
            ns.Close();
            client.Close();
        }
        public void enviarDatos(string info)
        {
            sw.WriteLine(info);
            sw.Flush();
        }
        public string recibirDatos()
        {
            return sr.ReadLine();
        }
        public string getData()
        {
            return sr.ReadLine();
        }
        public int getSala(string nombre)
        {
            sw.WriteLine("new");
            sw.WriteLine(nombre);
            sw.Flush();
            return Convert.ToInt32(sr.ReadLine());
        }
    }
}
