using System;
using System.Windows.Media;
using Client;

namespace HexBall
{
    public class Player : Entity
    {
        public static double MaxSpeed = 1;
        public static int Dimension = 20;

        public PlayerDir playerAction;

        /// <summary>
        ///     Standard constructor.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public Player(Pair position, double maxSpeed, int size, Color color) : base(position, maxSpeed, size)
        {
            EntityColor = color;
        }

        protected override void UpdateVelocity()
        {
            base.UpdateVelocity();
            if (this.playerAction == PlayerDir.NoMove) return;
            var velocity = new Pair();
            switch (this.playerAction)
            {
                case PlayerDir.Up:
                    velocity.Set(0, this.game.MovementSpeed);
                    break;
                case PlayerDir.RightUp:
                    velocity.Set(this.game.MovementSpeed / Math.Sqrt(2), this.game.MovementSpeed / Math.Sqrt(2));
                    break;
                case PlayerDir.Right:
                    velocity.Set(this.game.MovementSpeed, 0);
                    break;
                case PlayerDir.RightDown:
                    velocity.Set(this.game.MovementSpeed / Math.Sqrt(2), -this.game.MovementSpeed / Math.Sqrt(2));
                    break;
                case PlayerDir.Down:
                    velocity.Set(0, -this.game.MovementSpeed);
                    break;
                case PlayerDir.LeftDown:
                    velocity.Set(-this.game.MovementSpeed / Math.Sqrt(2), -this.game.MovementSpeed / Math.Sqrt(2));
                    break;
                case PlayerDir.Left:
                    velocity.Set(-this.game.MovementSpeed, 0);
                    break;
                case PlayerDir.LeftUp:
                    velocity.Set(-this.game.MovementSpeed / Math.Sqrt(2), this.game.MovementSpeed / Math.Sqrt(2));
                    break;
                case PlayerDir.NoMove:
                    break;
                default:
                    velocity.Set(0, 0);
                    break;
            }
            AddVelocity(velocity);
        }

        public override void Collide(Entity collider)
        {
            var vector = collider.Position - Position;
            collider.AddVelocity(vector);
            vector.First = -vector.First;
            vector.Second = -vector.Second;
            AddVelocity(vector);
        }
    }
}