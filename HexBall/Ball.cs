using System.Windows.Media;

namespace HexBall
{
    internal class Ball : Entity
    {
        public static double MaxSpeed = 3;
        public static int Dimension = 10;
        public Ball(Pair position, double maxSpeed, int size) : base(position, maxSpeed, size)
        {
            EntityColor = Colour.Black;
            Margin = 20;
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            var result = this.game.HasScored(Position);
            if (result == -1)
                return;


            if (result == 0)
            {
                this.game.ScoreA ++;
            }
            if (result == 1)
            {
                this.game.ScoreB++;
            }
            Position.First = Game.Size.Item2 / 2 - 3;
            Position.Second = Game.Size.Item1 / 2 - 3;
            Velocity.First = 0;
            Velocity.Second = 0;
        }
    }
}