using HexBall;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client
{
    public class ConnectionController
    {

        private TcpClient socket;
        private NetworkStream ns;
        private byte[] bytesIn;
        private byte[] bytesOut;
        private const int bufferSize = 8192;
        private Thread connectionThread;

        public int playerIndex { get; set; }

        private EntityAttr[] attributes { get; set; }

        public PlayerDir playerMovement { get; set; }

        public int ScoreA;
        public int ScoreB;

        public ConnectionController(string ip, int port)
        {
            bytesIn = new byte[bufferSize];
            bytesOut = new byte[bufferSize];

            this.playerMovement = PlayerDir.NoMove;
            this.attributes = new EntityAttr[5];

            this.socket = new TcpClient(ip, port);

            connectionThread = new Thread(ConnectionHandler);
            connectionThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get">true - odczytujemy attributes, false - zapisujemy</param>
        public EntityAttr[] GetSetAttributes(bool get = true, EntityAttr[] newAttributes = null)
        {
            //semafor
            lock (this.attributes)
            {
                if (!get)
                    this.attributes = newAttributes;
                return this.attributes;
            }
        }

        public void UpdateScore(Tuple<int, int> score)
        {
            ScoreA = score.Item1;
            ScoreB = score.Item2;
        }

        private void ConnectionHandler()
        {
            ns = this.socket.GetStream();

            //player index
            Message msg;
            msg = ReceiveMessage();
            this.playerIndex = (int)msg.data;

            while (true)
            {
                msg = ReceiveMessage();
                HandleMessage(msg);
                if (msg.type != MessageType.Score)
                    SendMessage(new Message { author = MessageAuthor.Client, type = MessageType.Movement, data = this.playerMovement });
            }
        }

        private void HandleMessage(Message msg)
        {
            if (msg == null)
                return;
            switch (msg.type)
            {
                case MessageType.Attributes:
                    var attrs = ((EntityAttr[])msg.data);
                    this.GetSetAttributes(false, attrs);
                    break;
                case MessageType.Score:
                    var scoredTeam = (Tuple<int, int>)msg.data;
                    UpdateScore(scoredTeam);
                    break;
            }
        }

        private void SendMessage(Message msg)
        {
            Serializer.ObjectToByteArray(msg).CopyTo(bytesOut, 0);
            ns.Write(bytesOut, 0, bytesOut.Count());
        }

        private Message ReceiveMessage()
        {
            try
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
            catch (Exception e)
            {

            }
            return null;
        }

        public void CloseConnection()
        {
            SendMessage(new Message { author = MessageAuthor.Client, type = MessageType.Disconnect });
            connectionThread.Abort();
        }
    }
}
