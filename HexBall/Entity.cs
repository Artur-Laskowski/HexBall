using System;
using System.Windows.Media;

namespace HexBall
{
    /// <summary>
    ///     Base class for handing collision, velocity, position, etc.
    /// </summary>
    internal class Entity
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
        public double EntitySlowdown { get; set; } = 0.001;


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
        protected virtual void UpdatePosition()
        {
            var proposedPos = new Pair
            {
                First = Position.First + Velocity.First * this.game.TimeDelta,
                Second = Position.Second + Velocity.Second * this.game.TimeDelta
            };
            if (this.game.IsInBounds(proposedPos, Margin))
                //TODO: additional checks(collision,etc), if fails, set velocity to 0
            {
                var flag = false;

                //player collision
                foreach (var e in this.game.Players)
                {
                    if (e == this)
                        continue;
                    if (Pair.Distance(e.Position, Position) < (double)(e.Size + Size) / 2)
                    {
                        Collide(e);
                        flag = true;
                    }
                }
                    

                //ball collision
                if (Pair.Distance(this.game.Ball.Position, Position) < (double)(this.game.Ball.Size + Size) / 2)
                {
                    Collide(this.game.Ball);
                    flag = true;
                }

                if (!flag)
                {
                    proposedPos.First = Position.First + Velocity.First * this.game.TimeDelta;
                    proposedPos.Second = Position.Second + Velocity.Second * this.game.TimeDelta;
                }
                if (this.game.IsInBounds(proposedPos, Margin))
                {
                    Position = proposedPos;
                }
                else
                {
                    proposedPos.First = Position.First - Velocity.First * this.game.TimeDelta;
                    proposedPos.Second = Position.Second - Velocity.Second * this.game.TimeDelta;
                    Position = proposedPos;
                }
            }
        }

        public Tuple<Pair, Color, int> GetPositionColorSize()
        {
            return new Tuple<Pair, Color, int>(Position,EntityColor,Size);
        }

        /// <summary>
        ///     Update function. Called every frame.
        /// </summary>
        public void Update()
        {
            UpdateVelocity();
            UpdatePosition();
        }
    }
}