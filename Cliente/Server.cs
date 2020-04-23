using System;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class Server
    {
        TcpClient client;
        NetworkStream ns;
        StreamWriter sw;
        StreamReader sr;

        public Server()
        {
            client = new TcpClient("javi.ink", 20000);
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
        public void sendData(string info)
        {
            sw.WriteLine(info);
            sw.Flush();
        }
        public string getData()
        {
            return sr.ReadLine();
        }
        public int getSala(string name)
        {
            sw.WriteLine("new");
            sw.WriteLine(name);
            sw.Flush();
            return Convert.ToInt32(sr.ReadLine());
        }
    }
}
