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

        private List<Tuple<Pair, Color, int>> attributes { get; set; }

        public PlayerDir playerMovement;

        public ConnectionController(string ip, int port)
        {
            bytesIn = new byte[bufferSize];
            bytesOut = new byte[bufferSize];

            this.playerMovement = PlayerDir.NoMove;
            this.attributes = new List<Tuple<Pair, Color, int>>();

            this.socket = new TcpClient(ip, port);

            Thread connectionThread = new Thread(ConnectionHandler);
            connectionThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get">true - odczytujemy attributes, false - zapisujemy</param>
        public List<Tuple<Pair, Color, int>> GetSetAttributes(bool get = true, List<Tuple<Pair, Color, int>> newAttributes = null)
        {
            //semafor
            lock (this.attributes)
            {
                if (!get)
                    this.attributes = newAttributes;
                return this.attributes;
            }
        }

        private void ConnectionHandler()
        {
            ns = this.socket.GetStream();
            Message msg;
            msg = ReceiveMessage();

            while (true)
            {
                msg = ReceiveMessage();
                switch (msg.type)
                {
                    case MessageType.Canvas:
                        this.GetSetAttributes(false, (List<Tuple<Pair, Color, int>>)msg.data);
                        break;
                    case MessageType.Goal:
                        break;
                }
                //wyslij movement
                if (playerMovement != PlayerDir.NoMove)
                    SendMessage(new Message { author = MessageAuthor.Client, type = MessageType.Movement, data = this.playerMovement });
            }
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
    }
}
