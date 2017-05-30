using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            GameService gs = new GameService();
            gs.Start();
            SocketService ss = new SocketService(7172, "",gs.game);
            ss.Start();
        }
    }
}
