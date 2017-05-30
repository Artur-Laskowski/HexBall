using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Windows.Media;

namespace HexBall
{
    /// <summary>
    ///     Base class for handing collision, velocity, position, etc.
    /// </summary>
    public class Entity
    {


        /// <summary>
        ///     Entity color. Assigned on creation
        /// </summary>
        public Color EntityColor;

        public Game game;

        public int Margin = 0;

        public Entity()
        {
            Position = new Pair(10, 10);
            MaxVelocity = 10;
            Size = 10;
            EntityColor = Color.FromRgb(128, 128, 128);
        }

        /// <summary>
        ///     Create object at set coordinates, maximum velocity and size.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxVelocity"></param>
        /// <param name="size"></param>
        public Entity(Pair position, double maxVelocity, int size)
        {
            Position = position;
            MaxVelocity = maxVelocity;
            Size = size;
            Velocity = new Pair(0, 0);
        }

        /// <summary>
        ///     Entity's position
        /// </summary>
        public Pair Position { get; set; }

        /// <summary>
        ///     Current speed
        /// </summary>
        public Pair Velocity { get; set; }

        /// <summary>
        ///     Maximum speed of entity. Used in functions updating speed.
        ///     Different for ball and player, can be changed during events.
        /// </summary>
        public double MaxVelocity { get; set; }

        /// <summary>
        ///     Entity's size. Used for collision.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///     How quickly entity looses velocity. For ball it will be less.
        /// </summary>
        public double EntitySlowdown { get; set; } = 0.05;


        /// <summary>
        ///     Handles collision. Called when object collides with stuff.
        /// </summary>
        /// <param name="collider"></param>
        public virtual void Collide(Entity collider)
        {
        }

        /// <summary>
        ///     Increase velocity by given vector.
        ///     If new velocity exceeds maximum, then it's reduced to maximum allowed.
        /// </summary>
        /// <param name="velocity"></param>
        public void AddVelocity(Pair velocity)
        {
            SetVelocity(Velocity + velocity);
            if (GetVelocity() > MaxVelocity)
                SetVelocity(MaxVelocity);
        }

        /// <summary>
        ///     Set velocity to given vector.
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(Pair velocity)
        {
            Velocity = velocity;
        }

        /// <summary>
        ///     Scale current velocity.
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(double velocity)
        {
            if (Math.Abs(GetVelocity()) < 0.01)
            {
                return;
            }

            var ratio = velocity / GetVelocity();
            Velocity *= ratio;
        }

        /// <summary>
        ///     Used to get absolute velocity of entity.
        /// </summary>
        /// <returns>Absolute velocity as double.</returns>
        public double GetVelocity()
        {
            return Math.Sqrt(Math.Pow(Velocity.First, 2) + Math.Pow(Velocity.Second, 2));
        }

        /// <summary>
        ///     Called every frame, handles velocity deteriorating over time, etc.
        ///     Called from Update().
        /// </summary>
        protected virtual void UpdateVelocity()
        {
            var vel = GetVelocity();
            vel -= EntitySlowdown;
            if (vel < 0)
                vel = 0;
            SetVelocity(vel);
        }

        /// <summary>
        ///     Updates object's position every frame, based on it's velocity.
        ///     Called from Update().
        /// </summary>
        protected virtual void UpdatePosition(double time)
        {
            var proposedPos = new Pair
            {
                First = Position.First + Velocity.First * time,
                Second = Position.Second + Velocity.Second * time
            };

            if (!this.game.IsInBounds(proposedPos, Margin))
                return;

            //player collision
            foreach (var otherPlayer in this.game.Players)
            {
                if (otherPlayer == this || otherPlayer == null)
                    continue;

                if (IsColliding(otherPlayer))
                    Collide(otherPlayer);
            }


            //ball collision
            if (IsColliding(game.Ball))
                Collide(this.game.Ball);

            proposedPos.First = Position.First + Velocity.First * time;
            proposedPos.Second = Position.Second + Velocity.Second * time;
            if (this.game.IsInBounds(proposedPos, Margin))
            {
                Position = proposedPos;
            }
            else
            {
                proposedPos.First = Position.First - Velocity.First * time;
                proposedPos.Second = Position.Second - Velocity.Second * time;
                Position = proposedPos;
            }
        }

        private bool IsColliding(Entity other)
        {
            return Pair.Distance(other.GetCenterPostion(), GetCenterPostion()) <= (other.Size + Size) / 2.0;
        }

        public Pair GetCenterPostion()
        {
            return new Pair(Position.First + Size / 2.0, Position.Second + Size / 2.0);
        }

        public Tuple<Pair, Color, int> GetPositionColorSize()
        {
            return new Tuple<Pair, Color, int>(Position, EntityColor, Size);
        }

        /// <summary>
        ///     Update function. Called every frame.
        /// </summary>
        public void Update(double time)
        {
            UpdateVelocity();
            UpdatePosition(time);
        }

        public EntityAttr GetAttributies()
        {
            return new EntityAttr(this);
        }

        public virtual Team GetTeam()
        {
            return Team.None;
        }
    }
}