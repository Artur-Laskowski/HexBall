using System.Windows.Media;

namespace HexBall
{
    public class Ball : Entity
    {
        public Ball(Pair position, double maxSpeed, int size) : base(position, maxSpeed, size)
        {
            EntityColor = Color.FromRgb(255, 255, 255);
            Margin = 20;
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            var result = Game.HasScored(Position);
            if (result == -1)
                return;


            if (result == 0)
            {
                Game.ScoreA++;
            }
            if (result == 1)
            {
                Game.ScoreB++;
            }
            Position.First = Game.Size.Item2 / 2 - 3;
            Position.Second = Game.Size.Item1 / 2 - 3;
            Velocity.First = 0;
            Velocity.Second = 0;
        }
    }
}