using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HexBall
{
    public class Game
    {
        public enum PlayerDir
        {
            NoMove,
            Up,
            RightUp,
            Right,
            RightDown,
            Down,
            LeftDown,
            Left,
            LeftUp
        }

        /// <summary>
        ///     Player movement speed. TODO change it based on some conditions?
        /// </summary>
        public const double MovementSpeed = 0.2;

        /// <summary>
        ///     Time between updates
        /// </summary>
        public const double TimeDelta = 0.04;
        

        /// <summary>
        ///     Playing field's size.
        /// </summary>
        public static readonly Tuple<int, int> Size = new Tuple<int, int>(800, 400);

        /// <summary>
        ///     List of all entities. TODO make it static so it can be easily accessed by entites when colliding?
        /// </summary>
        public static List<Entity> Entities;

        public static int ScoreA = 0;
        public static int ScoreB = 0;

        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2/2 - 50), new Pair(40, Size.Item2/2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2/2 - 50), new Pair(Size.Item1 - 0, Size.Item2/2 + 50));


        //network stuff
        private Task dataReceiver;
        private UdpClient myUdpClient;
        private UdpClient myUdpClientA;
        private int newPort;
        private readonly int port = 13131;
        private IPEndPoint remoteIPEndPointA;
        public static string server = "";
        private CancellationTokenSource tokenSource2;
        private int userID;
        private bool isConnected;
        BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>();



        public Game()
        {
            Entities = new List<Entity>();
            //Placeholders. Naturally objects will be added dynamicly.
            //TODO make it dynamic.
            var player1 = new Player(new Pair(20, 330), 1, 20);
            var ball = new Ball(new Pair(Size.Item2 / 2 - 3, Size.Item1 / 2 - 3), 3, 10);
            player1.EntityColor = Color.FromRgb(0,0,255);
            Entities.Add(ball);
            Entities.Add(player1);
            player1 = new Player(new Pair(20, 370), 1, 20) {EntityColor = Color.FromRgb(0, 0, 255)};
            Entities.Add(player1);
            player1 = new Player(new Pair(20, 410), 1, 20) {EntityColor = Color.FromRgb(255, 0, 0)};
            Entities.Add(player1);
            player1 = new Player(new Pair(20, 450), 1, 20) {EntityColor = Color.FromRgb(255, 0, 0)};
            Entities.Add(player1);
        }

        /// <summary>
        ///     Player's movement direction. Retrieved by player object and used to change velocity.
        /// </summary>
        public static PlayerDir PlayerDirection { get; set; }

        /// <summary>
        ///     Checks whether position is valid.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>bool - is in bounds</returns>
        public static bool IsInBounds(Pair a)
        {
            return a.First >= 0 && a.First <= Size.Item1 && a.Second >= 0 && a.Second <= Size.Item2;
        }

        public static bool IsInBounds(Pair a, int margin)
        {
            return a.First >= margin && a.First <= Size.Item2 - margin && a.Second >= margin &&
                   a.Second <= Size.Item1 - margin;
        }

        public static int HasScored(Pair a)
        {
            if (a.First > ZoneA.Item1.Second && a.First < ZoneA.Item2.Second && a.Second > ZoneA.Item1.First &&
                a.Second < ZoneA.Item2.First)
            {
                return 0;
            }

            if (a.First > ZoneB.Item1.Second && a.First < ZoneB.Item2.Second && a.Second > ZoneB.Item1.First &&
                a.Second < ZoneB.Item2.First)
            {
                return 1;
            }

            return -1;
        }

        /// <summary>
        ///     Update function. Called from timer every x ticks.
        /// </summary>
        /// <param name="attributes">
        ///     Values sent back to calling method. Used to change appearance and size of objects.
        ///     WARNING Currently entities and attributes are not linked, changing order of one of them will produce unwanted
        ///     behavior.
        /// </param>
        public void Update(out List<Tuple<Pair, Color, int>> attributes)
        {
            attributes = new List<Tuple<Pair, Color, int>>();

            //TODO see if this can be done more pretty
            var w = Keyboard.IsKeyDown(Key.D);
            var a = Keyboard.IsKeyDown(Key.W);
            var s = Keyboard.IsKeyDown(Key.A);
            var d = Keyboard.IsKeyDown(Key.S);
            //var space = Keyboard.IsKeyDown(Key.Space);
            PlayerDirection = PlayerDir.NoMove;
            if (w)
            {
                if (a)
                    PlayerDirection = PlayerDir.LeftUp;
                if (d)
                    PlayerDirection = PlayerDir.RightUp;
                if (!a && !d)
                    PlayerDirection = PlayerDir.Up;
            }
            if (s)
            {
                if (a)
                    PlayerDirection = PlayerDir.LeftDown;
                if (d)
                    PlayerDirection = PlayerDir.RightDown;
                if (!a && !d)
                    PlayerDirection = PlayerDir.Down;
            }
            if (!w && !s)
            {
                if (a)
                    PlayerDirection = PlayerDir.Left;
                if (d)
                    PlayerDirection = PlayerDir.Right;
            }

            attributes.Clear();

            //send movement data here

            //Serializujemy pakiet z punktami
            byte[] data = {(byte)PlayerDirection, (byte)userID};
            if (isConnected)
            {
                try
                {
                    myUdpClientA.SendAsync(data, data.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                //check queue for updates received from the server, if found update objects with positions

                if (queue.Count > 0)
                {
                    var dat = queue.Take();
                    //deserializacja punktów
                    IFormatter formatter = new BinaryFormatter();
                    using (MemoryStream stream = new MemoryStream(dat))
                    {
                        ServerAlt.Packet p = (ServerAlt.Packet)formatter.Deserialize(stream);
                        ScoreA = p.scoreA;
                        ScoreB = p.scoreB;
                        //iterate over list in a packet and update positions
                        for (int i = 0; i < p.positions.Length; i++)
                        {
                            Entities[i].Position.First = p.positions[i].First;
                            Entities[i].Position.Second = p.positions[i].Second;

                        }
                    }
                }

            }

            foreach (var e in Entities)
            {
                attributes.Add(new Tuple<Pair, Color, int>(e.Position, e.EntityColor, e.Size));
            }
        }

        public void Connect()
        {
            //Attempt to connect with the server
            myUdpClient = new UdpClient();
            myUdpClient.Connect(server, port);
            string msg = "Connect";
            byte[] data = Encoding.ASCII.GetBytes(msg);

            try
            {
                myUdpClient.SendAsync(data, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            //Await response
            try
            {
                data = myUdpClient.Receive(ref remoteIPEndPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to server!\n" + ex.Message);
                return;
            }

            //odczytanie portu do rysowania oraz ID użytkownika
            //read user ID and port number used to communicate actual game info
            newPort = data[0] * byte.MaxValue + data[1];
            userID = data[2];
            if (Entities[userID + 1].EntityColor.B == 255)
                Entities[userID + 1].EntityColor = Color.FromRgb(0,1,255);
            else
                Entities[userID + 1].EntityColor = Color.FromRgb(255, 1, 0);

            //label.Content = "Online, ID: " + userID;

            //Attempt connection on a new port
            try
            {
                myUdpClientA = new UdpClient();
                myUdpClientA.Connect(server, newPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;

            dataReceiver = Task.Run(
                () =>
                {
                    ct.ThrowIfCancellationRequested();

                    while (true)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            ct.ThrowIfCancellationRequested();
                        }

                        byte[] dataA;

                        //Await data from the server
                        try
                        {
                            Console.WriteLine(IPAddress.Any.ToString());
                            remoteIPEndPointA = new IPEndPoint(IPAddress.Broadcast, newPort);
                            dataA = myUdpClientA.Receive(ref remoteIPEndPointA);
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message);
                            return;
                        }

                        Packet p;

                        //read user ID
                        byte receivedUserID = dataA[dataA.Length - 4];



                        //packet with all user positions
                        byte[] packetBytes = new byte[dataA.Length - 4];
                        Array.Copy(dataA, packetBytes, dataA.Length - 4);

                        queue.Add(dataA, ct);
                    }
                }, tokenSource2.Token);

            isConnected = true;
            //button1.IsEnabled = false;
            //button2.IsEnabled = true;

        }
    }
}