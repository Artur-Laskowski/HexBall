using System;
using System.Windows.Media;

namespace HexBall
{
    public class Player : Entity
    {
        /// <summary>
        ///     Standard constructor.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public Player(Pair position, double maxSpeed, int size) : base(position, maxSpeed, size)
        {
            EntityColor = Color.FromRgb(123,123,69);
        }

        protected override void UpdateVelocity()
        {
            base.UpdateVelocity();
            if (Game.PlayerDirection == Game.PlayerDir.NoMove) return;
            var velocity = new Pair();
            switch (Game.PlayerDirection)
            {
                case Game.PlayerDir.Up:
                    velocity.Set(0, Game.MovementSpeed);
                    break;
                case Game.PlayerDir.RightUp:
                    velocity.Set(Game.MovementSpeed / Math.Sqrt(2), Game.MovementSpeed / Math.Sqrt(2));
                    break;
                case Game.PlayerDir.Right:
                    velocity.Set(Game.MovementSpeed, 0);
                    break;
                case Game.PlayerDir.RightDown:
                    velocity.Set(Game.MovementSpeed / Math.Sqrt(2), -Game.MovementSpeed / Math.Sqrt(2));
                    break;
                case Game.PlayerDir.Down:
                    velocity.Set(0, -Game.MovementSpeed);
                    break;
                case Game.PlayerDir.LeftDown:
                    velocity.Set(-Game.MovementSpeed / Math.Sqrt(2), -Game.MovementSpeed / Math.Sqrt(2));
                    break;
                case Game.PlayerDir.Left:
                    velocity.Set(-Game.MovementSpeed, 0);
                    break;
                case Game.PlayerDir.LeftUp:
                    velocity.Set(-Game.MovementSpeed / Math.Sqrt(2), Game.MovementSpeed / Math.Sqrt(2));
                    break;
                case Game.PlayerDir.NoMove:
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