using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.XPath;

namespace HexBall
{
    public class Game
    {
        public static Color TeamAColor = Colour.Red;
        public static Color TeamBColor = Colour.Blue;
        public static Color BallColor = Colour.Black;


        //public Canvas canv;
        /// <summary>
        ///     Player movement speed. TODO change it based on some conditions?
        /// </summary>
        public readonly double MovementSpeed = 0.2;

        /// <summary>
        ///     Playing field's size.
        /// </summary>
        public static readonly Tuple<int, int> Size = new Tuple<int, int>(800, 400);

        public EntityAttr[] Attributes { get; set; }

        /// <summary>
        ///     List of all entities. TODO make it static so it can be easily accessed by entites when colliding?
        /// </summary>
        public Ball Ball;
        public Player[] Players;

        public int ScoreA = 0;
        public int ScoreB = 0;

        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2 / 2 - 50), new Pair(40, Size.Item2 / 2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2 / 2 - 50), new Pair(Size.Item1 - 0, Size.Item2 / 2 + 50));


        public Game()
        {
            Players = new Player[4];
            Attributes = new EntityAttr[4];
            AddBall();
        }


        public int AddPlayer()
        {
            Pair teamSizes = this.GetTeamsSizes();
            if (teamSizes.First + teamSizes.Second == 4)
                return -1;

            var color = GetNewPlayerColor();
            Pair mapCenter = GetCenterOfBoard();
            Pair position;
            int index;
            //team A jest po lewej stronie planszy
            //gracze maja indeksy 0 i 1
            //team B po prawej stronie
            //gracze maja indeksy 2 i 3
            if (color == TeamAColor)
            {
                if (this.Players[0] == null)
                {
                    position = new Pair(mapCenter.First - mapCenter.First / 2, mapCenter.Second - Player.Dimension);
                    index = 0;
                }
                else
                {
                    position = new Pair(mapCenter.First - mapCenter.First / 2, mapCenter.Second + Player.Dimension);
                    index = 1;
                }
            }
            else
            {
                if (this.Players[2] == null)
                {
                    position = new Pair(mapCenter.First + mapCenter.First / 2, mapCenter.Second - Player.Dimension);
                    index = 2;
                }
                else
                {
                    position = new Pair(mapCenter.First + mapCenter.First / 2, mapCenter.Second + Player.Dimension);
                    index = 3;
                }
            }
            var player = new Player(position, Player.MaxSpeed, Player.Dimension, color) { game = this };
            this.Players[index] = player;
            return index;
        }

        public void RemovePlayer(int index)
        {
            this.Players[index] = null;
        }

        private void AddBall()
        {
            var boardCenter = GetCenterOfBoard();
            Ball = new Ball(boardCenter, Ball.MaxSpeed, Ball.Dimension);
        }

        private Color GetNewPlayerColor()
        {
            var teamSizes = GetTeamsSizes();
            return teamSizes.First == teamSizes.Second ? TeamAColor : TeamBColor;
        }

        private Pair GetTeamsSizes()
        {
            var size = new Pair(0, 0);
            foreach (var player in Players)
            {
                if (player == null)
                    continue;
                if (player.GetTeam() == Team.A)
                    size.First++;
                else
                    size.Second++;
            }
            return size;
        }

        private static Pair GetCenterOfBoard()
        {
            return new Pair(Size.Item2 / 2 - 3, Size.Item1 / 2 - 3);
        }

        private Pair GetFreePostion()
        {
            //TODO random
            var random = new Random();
            var x = random.Next(Size.Item1);
            var y = random.Next(Size.Item2);
            return new Pair(x, y);
        }

        /// <summary>
        ///     Checks whether position is valid.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>bool - is in bounds</returns>
        public bool IsInBounds(Pair a)
        {
            return a.First >= 0 && a.First <= Size.Item1 && a.Second >= 0 && a.Second <= Size.Item2;
        }

        public bool IsInBounds(Pair a, int margin)
        {
            return a.First >= margin && a.First <= Size.Item2 - margin && a.Second >= margin &&
                   a.Second <= Size.Item1 - margin;
        }

        public Score HasScored(Pair a)
        {
            if (a.First > ZoneA.Item1.Second && a.First < ZoneA.Item2.Second && a.Second > ZoneA.Item1.First &&
                a.Second < ZoneA.Item2.First)
            {
                return Score.ZoneAGoal;
            }

            if (a.First > ZoneB.Item1.Second && a.First < ZoneB.Item2.Second && a.Second > ZoneB.Item1.First &&
                a.Second < ZoneB.Item2.First)
            {
                return Score.ZoneBGoal;
            }

            return Score.NoScore;
        }

        public void ResetBall()
        {
            //TODO zabezpieczyć przed położeniem piłki na środku gdy jest tam gracz
            Ball.Position = GetCenterOfBoard();
        }

        public void UpdateMovemenet(PlayerDir mov, int index)
        {
            Players[index].playerAction = mov;
        }

        public void Update(bool movement = true,PlayerDir mov= PlayerDir.NoMove, int index=-1)
        {
            lock (Attributes)
            {
                if (movement)
                {
                    this.Players[index].playerAction = mov;
                    return;
                }
                //game update
                var attr = new EntityAttr[4];

                //Po kolei aktualizujemy obiekty i dodajemy ich atrybuty do listy, z której będą czytane podczas rysowania.
                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i] == null)
                        continue;
                    Players[i].Update(1);
                    attr[i] = Players[i].GetAttributies();
                }
                this.Attributes = attr;
            }
        }
    }
}