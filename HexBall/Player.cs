using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HexBall
{
    class Player:Entity
    {
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="size"></param>
        public Player(Pair position, double maxSpeed, int size, Color color) : base(position, maxSpeed, size)
        {
            EntityColor = color;
        }

        protected override void UpdateVelocity()
        {
            base.UpdateVelocity();
            if (Game.playerDir != Game.PlayerDir.noMove)
            {
                Pair velocity = new Pair();
                switch (Game.playerDir)
                {
                    case Game.PlayerDir.up:
                        velocity.Set(0, Game.movementSpeed);
                        break;
                    case Game.PlayerDir.rightUp:
                        velocity.Set(Game.movementSpeed / Math.Sqrt(2), Game.movementSpeed / Math.Sqrt(2));
                        break;
                    case Game.PlayerDir.right:
                        velocity.Set(Game.movementSpeed, 0);
                        break;
                    case Game.PlayerDir.rightDown:
                        velocity.Set(Game.movementSpeed/ Math.Sqrt(2), -Game.movementSpeed / Math.Sqrt(2));
                        break;
                    case Game.PlayerDir.down:
                        velocity.Set(0, -Game.movementSpeed);
                        break;
                    case Game.PlayerDir.leftDown:
                        velocity.Set(-Game.movementSpeed / Math.Sqrt(2), -Game.movementSpeed / Math.Sqrt(2));
                        break;
                    case Game.PlayerDir.left:
                        velocity.Set(-Game.movementSpeed, 0);
                        break;
                    case Game.PlayerDir.leftUp:
                        velocity.Set(-Game.movementSpeed / Math.Sqrt(2), Game.movementSpeed / Math.Sqrt(2));
                        break;
                    default:
                        velocity.Set(0, 0);
                        break;
                }
                AddVelocity(velocity);
            }
        }
    }
}
