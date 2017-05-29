using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HexBall;
using System.Windows.Media;

namespace Server
{
    public class ClientService : IDisposable
    {
        private TcpClient socket;
        private Game game;
        private NetworkStream ns;
        private byte[] bytesIn;
        private byte[] bytesOut;
        private const int bufferSize = 8192;

        private int clientId;
        private int playerIndex;

        public ClientService(TcpClient inClientSocket, int nmbr, Game g)
        {
            this.socket = inClientSocket;
            this.clientId = nmbr;
            this.game = g;

            bytesIn = new byte[bufferSize];
            bytesOut = new byte[bufferSize];

            Thread connectionThread = new Thread(ConnectionHandler);
            connectionThread.Start();
        }

        private void ConnectionHandler()
        {
            ns = this.socket.GetStream();
            this.SendPlayerIndex();

            Message msg;

            while (true)
            {
                try
                {
                    this.SendMessage(new Message { author = MessageAuthor.Server, type = MessageType.Canvas, data = new List<Tuple<Pair, Color, int>>() });
                    msg = this.ReceiveMessage();
                }
                catch(Exception e)
                {
                    break;
                }
                
            }
        }

        private void SendPlayerIndex()
        {
            this.SendMessage(new Server.Message { author = MessageAuthor.Server, type = MessageType.Player, data = clientId });
        }

        private void SendMessage(Message msg)
        {
            Serializer.ObjectToByteArray(msg).CopyTo(bytesOut, 0);
            ns.Write(bytesOut, 0, bytesOut.Count());
            ns.Flush();
        }

        private Message ReceiveMessage()
        {
            ns.Read(bytesIn, 0, bufferSize);
            return (Message)Serializer.ByteArrayToObject(bytesIn);
        }

        public void Dispose()
        {
            this.socket.GetStream().Close();
            this.socket.Client.Disconnect(false);
        }
    }
}
