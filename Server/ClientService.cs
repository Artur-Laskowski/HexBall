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
using System.Net.NetworkInformation;

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

        private EntityAttr[] attribs;

        private Thread connectionThread;

        public ClientService(TcpClient inClientSocket, int nmbr, Game g)
        {
            this.socket = inClientSocket;
            this.clientId = nmbr;
            this.game = g;
            this.attribs = new EntityAttr[Game.EntityAttrsSize];

            //this.socket.NoDelay = true;

            bytesIn = new byte[bufferSize];
            bytesOut = new byte[bufferSize];
            connectionThread = new Thread(ConnectionHandler);
            connectionThread.Start();
        }

        private void ConnectionHandler()
        {
            ns = this.socket.GetStream();
            this.SendPlayerIndex();

            Message msg;

            this.SendMessage(new Message { author = MessageAuthor.Server, type = MessageType.Attributes, data = this.attribs });

            while (true)
            {
                msg = this.ReceiveMessage();
                HandleMessage(msg);
                ScoreMessage();
                attribs = game.Attributes;
                this.SendMessage(new Message { author = MessageAuthor.Server, type = MessageType.Attributes, data = this.attribs });
            }
        }

        private void ScoreMessage()
        {
            SendScore();
        }

        private void HandleMessage(Message msg)
        {
            switch (msg.type)
            {
                case MessageType.Movement:
                    this.game.Update(movement: true, index: this.playerIndex, mov: (PlayerDir)msg.data);
                    break;
                case MessageType.Disconnect:
                    Disconnect();

                    break;
            }
        }

        private void Disconnect()
        {
            this.socket.Close();
            this.game.RemovePlayer(this.playerIndex);
            Console.WriteLine(" >> " + "user disconnected");
            this.connectionThread.Abort();
        }

        private void SendPlayerIndex()
        {
            this.playerIndex = game.AddPlayer();
            if (this.playerIndex >= 0)
                this.SendMessage(new Server.Message { author = MessageAuthor.Server, type = MessageType.Player, data = this.playerIndex });
            else
                this.SendMessage(new Server.Message { author = MessageAuthor.Server, type = MessageType.NoSlots });
        }

        private void SendScore()
        {
            var score = new Tuple<int, int>(game.ScoreA, game.ScoreB);
            this.SendMessage(new Server.Message() { author = MessageAuthor.Server, type = MessageType.Score, data = score });
        }

        private void SendMessage(Message msg)
        {
            Serializer.ObjectToByteArray(msg).CopyTo(bytesOut, 0);
            ns.Write(bytesOut, 0, bytesOut.Count());
        }

        private Message ReceiveMessage()
        {
            int remaining = bufferSize;
            int pos = 0;
            while (remaining != 0)
            {
                int bytes = ns.Read(bytesIn, pos, remaining);
                pos += bytes;
                remaining -= bytes;
            }

            return (Message)Serializer.ByteArrayToObject(bytesIn);
        }

        public void Dispose()
        {
            this.socket.GetStream().Close();
            this.socket.Client.Disconnect(false);
        }


    }
}
