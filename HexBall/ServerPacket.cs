using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HexBall
{
    [Serializable]
    public class ServerPacket
    {
        public class EntityPacket
        {
            public Pair position;
            public int id;

            public EntityPacket(Pair position, int id)
            {
                this.position = position;
                this.id = id;
            }
        }

        public List<EntityPacket> entities;

        public ServerPacket(Pair position)
        {
            addEntity(position);
        }

        public void addEntity(Pair position)
        {
            entities.Add(new EntityPacket(position));
        }
    }
}
