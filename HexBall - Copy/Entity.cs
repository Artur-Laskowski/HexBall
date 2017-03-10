using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexBall
{
    /// <summary>
    /// Base class for handing collision, velocity, position, etc.
    /// </summary>
    class Entity
    {
        /// <summary>
        /// Entity's position
        /// </summary>

        public Pair Position { get; set; }
        /// <summary>
        /// Current speed
        /// </summary>
        public Pair Velocity { get; set; }
        /// <summary>
        /// Maximum speed of entity. Used in functions updating speed.
        /// Different for ball and player, can be changed during events.
        /// </summary>
        public double MaxVelocity { get; set; } 
        /// <summary>
        /// Entity's size. Used for collision.
        /// </summary>
        public double Size { get; set; }

        public Entity()
        {
            Position = new Pair(10, 10);
            MaxVelocity = 10;
            Size = 10;
        }

        /// <summary>
        /// Create object at set coordinates, maximum velocity and size.
        /// </summary>
        /// <param name="position"></param>
        public Entity(Pair position, double maxVelocity, double size)
        {
            Position = position;
            MaxVelocity = maxVelocity;
            Size = size;
            Velocity = new Pair(0, 0);
        }


        /// <summary>
        /// Handles collision. Called when object collides with stuff.
        /// </summary>
        /// <param name="collider"></param>
        public virtual void Collide(Entity collider)
        {
            
        }
        /// <summary>
        /// Increase velocity by given vector.
        /// If new velocity exceeds maximum, then it's reduced to maximum allowed.
        /// </summary>
        /// <param name="velocity"></param>
        public void AddVelocity(Pair velocity)
        {
            SetVelocity(Velocity + velocity);
            if (GetVelocity() > MaxVelocity)
            {
                SetVelocity(MaxVelocity);
            }
        }
        /// <summary>
        /// Set velocity to given vector.
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(Pair velocity)
        {
            Velocity = velocity;
        }
        /// <summary>
        /// Scale current velocity.
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(double velocity)
        {
            if (GetVelocity() == 0)
            {
                return;
            }
            double ratio = velocity / GetVelocity();
            Velocity *= ratio;
        }
        /// <summary>
        /// Used to get absolute velocity of entity.
        /// </summary>
        /// <returns>Absolute velocity as double.</returns>
        public double GetVelocity()
        {
            return Math.Sqrt(Math.Pow(Velocity.First, 2) + Math.Pow(Velocity.Second, 2));
        }
        /// <summary>
        /// Called every frame, handles velocity deteriorating over time, etc.
        /// Called from Update().
        /// </summary>
        virtual protected void UpdateVelocity()
        {
            double vel = GetVelocity();
            vel -= 0.5;
            if (vel < 0)
                vel = 0;
            SetVelocity(vel);
        }
        /// <summary>
        /// Updates object's position every frame, based on it's velocity.
        /// Called from Update().
        /// </summary>
        protected void UpdatePosition()
        {
            Pair proposedPos = new Pair()
            {
                First = Position.First + Velocity.First * Game.time_delta,
                Second = Position.Second + Velocity.Second * Game.time_delta
            };
            if (Game.IsInBounds(proposedPos)) //TODO: additional checks, if fails, set velocity to 0
            {
                Position = proposedPos;
            }
        }
        /// <summary>
        /// Update function. Called every frame.
        /// </summary>
        public void Update()
        {
            UpdateVelocity();
            UpdatePosition();
        }
    }
}
