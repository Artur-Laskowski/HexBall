using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ServerAlt
{
    internal static class Program
    {
        private const int connectionPort = 13131;
        private const int drawingPort = 1337;
        private static Game _game = new Game();
        private static int counter = 0;
        private static void Main()
        {
            UdpClient myUdpClient;
            UdpClient myUdpClientA;
            try
            {
                var localIPEndPoint = new IPEndPoint(IPAddress.Any, connectionPort);
                myUdpClient = new UdpClient(localIPEndPoint);
                var localIPEndPointA = new IPEndPoint(IPAddress.Any, drawingPort);
                myUdpClientA = new UdpClient(localIPEndPointA);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }


            //User IPs and IDs
            Dictionary<IPEndPoint, byte[]> clients = new Dictionary<IPEndPoint, byte[]>();

            //queue with movement data
            BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>();

            //BlockingCollection<Dictionary<IPEndPoint, byte[]>> clients = new BlockingCollection<Dictionary<IPEndPoint, byte[]>>();

            //Thread reading movement data from the players
            Task dataReceiver = Task.Run(
                () =>
                {
                    //byte[] color = {0, 0, 0};
                    while (true)
                    {
                        var remoteIPEndPointA = new IPEndPoint(IPAddress.Any, 1337);
                        byte[] dataA;
                        try
                        {
                            dataA = myUdpClientA.Receive(ref remoteIPEndPointA);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }

                        queue.Add(dataA);
                    }
                }
            );

            Task dataSender = Task.Run(
                () =>
                {
                    while (true)
                    {
                        if (queue.Count == 0) continue;
                        byte[] dataBytes = queue.Take();
                        Game.PlayerDirections[dataBytes[1]] = (Game.PlayerDir)dataBytes[0];
                        //if (GameServer.PlayerDirection != GameServer.PlayerDir.NoMove)
                        //Console.WriteLine(Game.PlayerDirections[dataBytes[1]]);

                        _game.Update();
                        counter++;
                        if (counter < 100)
                            continue;

                        counter = 0;

                        Packet packet = new Packet();
                        for (int i = 0; i < 5; i++)
                        {
                            packet.positions[i] = Game.Entities[i].Position;
                        }
                        packet.scoreA = Game.ScoreA;
                        packet.scoreB = Game.ScoreB;
                        byte[] data;
                        BinaryFormatter bf = new BinaryFormatter();
                        using (var ms = new MemoryStream())
                        {
                            bf.Serialize(ms, packet);
                            data = ms.ToArray();
                        }

                        foreach (var client in clients)
                        {
                            myUdpClientA.SendAsync(data, data.Length, client.Key);
                        }

                        //Thread.Sleep(100);
                    }
                }
            );
            while (true)
            {
                var remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var data = myUdpClient.Receive(ref remoteIPEndPoint);
                var msg = Encoding.ASCII.GetString(data, 0, data.Length);

                if (msg == "Connect")
                {
                    byte userID = 0;
                    for (byte i = 0; i < byte.MaxValue; i++)
                    {
                        /*if (!clients.ContainsValue(i))
                        {
                            userID = i;
                            break;
                        }*/
                        bool flag = false;
                        foreach (var c in clients)
                        {
                            if (c.Value[0] == i)
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            userID = i;
                            break;
                        }
                    }

                    byte[] answer = { drawingPort / byte.MaxValue, drawingPort % byte.MaxValue, userID };
                    myUdpClient.SendAsync(answer, answer.Length, remoteIPEndPoint);

                    Console.WriteLine("{0}> {1}", remoteIPEndPoint, msg);

                    remoteIPEndPoint.Port++;
                    clients.Add(remoteIPEndPoint, new byte[] { userID, 0, 0, 0 });
                }
                if (msg == "Disconnect")
                {
                    Console.WriteLine("{0}> {1}", remoteIPEndPoint, msg);

                    remoteIPEndPoint.Port++;
                    var res = clients.Remove(remoteIPEndPoint);
                    Console.WriteLine("Attempting to remove {0} - result: {1}", remoteIPEndPoint, res);
                }
            }
        }
    }
}