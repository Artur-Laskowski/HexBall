using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexBall
{
    [Serializable]
    public class ClientPacket
    {
        private int playerId;
        private PlayerDir direction;

        public ClientPacket(int playerId, PlayerDir direction)
        {
            this.playerId = playerId;
            this.direction = direction;
        }
    }
}
