using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class SocketService
    {
        TcpListener serverSocket { get; set; }
        TcpClient clientSocket { get; set; }

        int connections { get; set; } = 0;

        public SocketService(int port, string ip)
        {
            //server socket obj
            if (String.IsNullOrEmpty(ip))
            {
                //localhost
                this.serverSocket = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), port);
            }
            else
            {
                this.serverSocket = new TcpListener(System.Net.IPAddress.Parse(ip), port);
            }


            //client socket obj
            this.clientSocket = new TcpClient();
        }

        public void Start()
        {
            this.serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");
            while (true)
            {
                this.clientSocket = serverSocket.AcceptTcpClient();
                connections++;
                Console.WriteLine(" >> " + "user " + connections + " connected");
                new ClientService(this.clientSocket, connections);
            }
        }

    }
}
