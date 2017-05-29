using System;
using System.Windows.Media;

namespace HexBall
{
    [Serializable]
    public class EntityAttr
    {
        public Pair Position { get; set; }
        public Team Team { get; set; }
        public int Size { get; set; }

        public EntityAttr(Entity entity)
        {
            Position = entity.Position;
            Team = entity.GetTeam();
            Size = entity.Size;
        }

        public Color GetColor()
        {
            switch (Team)
            {
                case Team.A:
                    return Game.TeamAColor;
                case Team.B:
                    return Game.TeamBColor;
                default:
                    return Game.BallColor;
            }
        }
    }
}