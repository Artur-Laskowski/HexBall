﻿using System.Windows.Media;

namespace HexBall
{
    public class Ball : Entity
    {
        public static double MaxSpeed = 3;
        public static int Dimension = 15;
        public Ball(Pair position, double maxSpeed, int size) : base(position, maxSpeed, size)
        {
            EntityColor = Colour.Black;
            Margin = 20;
        }

        protected override void UpdatePosition(double time)
        {
            var proposedPos = new Pair
            {
                First = Position.First + Velocity.First * time,
                Second = Position.Second + Velocity.Second * time
            };
            
            var result = game.HasScored(proposedPos,Size);
            switch (result)
            {
                case Score.NoScore:
                    int temp = this.game.IsInBounds(proposedPos, Size);
                    if (temp==0)
                    {
                        Position = proposedPos;
                    }else if(temp==1)
                    {
                        Velocity.First = -Velocity.First*0.5;
                        Velocity.Second = -Velocity.Second * 0.5;
                    }
                    else if(temp==2)

                    return;

                case Score.ZoneAGoal:
                    game.Goal(Team.B);
                    break;

                case Score.ZoneBGoal:
                    game.Goal(Team.A);
                    break;
            }
            Position.First = Game.Size.Item2 / 2 - 3;
            Position.Second = Game.Size.Item1 / 2 - 3;
            Velocity.First = 0;
            Velocity.Second = 0;
        }


    }
}