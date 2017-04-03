using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ClientService
    {
        TcpClient clientSocket { get; set; }
        int number { get; set; } //id?

        public ClientService(TcpClient inClientSocket, int nmbr)
        {
            this.clientSocket = inClientSocket;
            this.number = nmbr;
            //
                // init player on game object
            //
            Thread clientThread = new Thread(ClientHandler);
            clientThread.Start();
        }

        public void ClientHandler()
        {
            byte[] bytesIn = new byte[400096];
            byte[] bytesOut = new byte[8192];
            String zxc = "blasladlsadasdjaiosjdaoisjdsaodhsiudfasudfha";
            while (true)
            {
                try
                {
                    NetworkStream networkStream = this.clientSocket.GetStream();
                    networkStream.Read(bytesIn, 0, (int)clientSocket.ReceiveBufferSize);
                    //
                    //deserialization
                    //add event with this player id to game object
                    //serialize map to byte array
                    //send back to client
                    //
                    Serializer.ObjectToByteArray(zxc).CopyTo(bytesOut, 0);
                    networkStream.Write(bytesOut, 0, bytesOut.Count());
                    networkStream.Flush();
                }catch(Exception e)
                {
                    break;
                }
                
            }
        }

        
    }
}
