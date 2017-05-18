using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexBall
{
    [Serializable]
    public class Packet
    {
        //packet with an array of positions of 5 objects
        //0-ball, 1-2 blu 3-4 red
        public Pair[] positions;

        public Packet()
        {
            positions = new Pair[5];
        }

        public void Add(Pair pos, int i)
        {
            positions[i] = pos;
        }


    }
}
