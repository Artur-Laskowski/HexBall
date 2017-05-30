using HexBall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Server
{
    public class SocketService
    {
        TcpListener serverSocket { get; set; }
        List<ClientService> clients { get; set; }

        private Game game { get; set; }
        private int connections { get; set; } = 0;

        public SocketService(int port, string ip, Game g)
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
            this.clients = new List<ClientService>();

            this.game = g;
        }

        public void Start()
        {
            this.serverSocket.Start();

            Thread listenerThread = new Thread(Listener);
            listenerThread.Start();
        }

        //nasłuchuje połączeń, jeżeli nastąpi to tworzy nowy CilentService i dodaje do listy
        //obsługa połączenia z klientem poprzez ClientService
        private void Listener()
        {
            while (true)
            {
                var clientSocket = serverSocket.AcceptTcpClient();
                connections++;
                Console.WriteLine(" >> " + "user " + connections + " connected");
                this.clients.Add(new ClientService(clientSocket, connections, game));
            }
        }
    }
}
