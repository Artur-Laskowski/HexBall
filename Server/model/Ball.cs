using System.Windows.Media;

namespace HexBall
{
    public class Ball : Entity
    {
        public static double MaxSpeed = 3;
        public static int Dimension = 10;
        public Ball(Pair position, double maxSpeed, int size) : base(position, maxSpeed, size)
        {
            EntityColor = Colour.Black;
            Margin = 20;
        }

        protected override void UpdatePosition(double time)
        {
            base.UpdatePosition(time);
            var result = game.HasScored(Position);
            switch (result)
            {
                case Score.NoScore:
                    return;

                case Score.ZoneAGoal:
                    game.ScoreB++;
                    break;

                case Score.ZoneBGoal:
                    game.ScoreA++;
                    break;
            }
            game.ResetBall();
            Position.First = Game.Size.Item2 / 2 - 3;
            Position.Second = Game.Size.Item1 / 2 - 3;
            Velocity.First = 0;
            Velocity.Second = 0;
        }
    }
}