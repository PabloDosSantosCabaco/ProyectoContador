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
        bool conectado;
        NetworkStream ns;
        StreamWriter sw;
        StreamReader sr;
        public Servidor()
        {
            for (int i = 31416; i < 31420; i++)
            {
                try
                {
                    client = new TcpClient("127.0.0.1", i);
                    conectado = true;
                    ns = client.GetStream();
                    sr = new StreamReader(ns);
                    sw = new StreamWriter(ns);
                    break;
                }
                catch (SocketException ex)
                {

                }
                conectado = false;
            }
        }
        public void enviarDatos(string info)
        {
            sw.WriteLine(info);
            sw.Flush();
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
        public void getJugadores()
        {
            sw.WriteLine("lista");
            sw.Flush();
        }
    }
}
