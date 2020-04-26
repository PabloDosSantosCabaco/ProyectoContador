using System;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class Server
    {
        /// <summary>
        /// Permite establecer la conexión con el servidor.
        /// </summary>
        TcpClient client;
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
        /// Constructor de Server.
        /// Se conecta al servidor con una dirección y puerto fijos donde está el servidor.
        /// </summary>
        public Server()
        {
            client = new TcpClient("javi.ink", 20000);
            ns = client.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
        }
        /// <summary>
        /// Cierra el socket y todas sus vías de comunicación.
        /// </summary>
        public void closeServer()
        {
            sw.Close();
            sr.Close();
            ns.Close();
            client.Close();
        }
        /// <summary>
        /// Envía información al servidor.
        /// </summary>
        /// <param name="info">Información a enviar.</param>
        public void sendData(string info)
        {
            sw.WriteLine(info);
            sw.Flush();
        }
        /// <summary>
        /// Recibe información que le envía el servidor.
        /// </summary>
        /// <returns>Información recibida del servidor.</returns>
        public string getData()
        {
            return sr.ReadLine();
        }
        /// <summary>
        /// Obtiene el numero de una nueva sala creada por el host que la llama.
        /// </summary>
        /// <param name="name">Host de la sala</param>
        /// <returns>Numero de la sala</returns>
        public int getSala(string name)
        {
            sw.WriteLine("new");
            sw.WriteLine(name);
            sw.Flush();
            return Convert.ToInt32(sr.ReadLine());
        }
    }
}
