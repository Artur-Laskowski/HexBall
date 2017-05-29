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

    public enum PlayerDir
    {
        NoMove,
        Up,
        RightUp,
        Right,
        RightDown,
        Down,
        LeftDown,
        Left,
        LeftUp,
        Shoot
    }

    public class Game
    {

        //public Canvas canv;
        /// <summary>
        ///     Player movement speed. TODO change it based on some conditions?
        /// </summary>
        public readonly double MovementSpeed = 0.2;

        /// <summary>
        ///     Time between updates
        /// </summary>
        public readonly double TimeDelta = 0.04;


        /// <summary>
        ///     Playing field's size.
        /// </summary>
        public static readonly Tuple<int, int> Size = new Tuple<int, int>(800, 400);

        /// <summary>
        ///     List of all entities. TODO make it static so it can be easily accessed by entites when colliding?
        /// </summary>
        public Ball Ball;
        public Player[] Players;

        public int ScoreA = 0;
        public int ScoreB = 0;

        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2 / 2 - 50), new Pair(40, Size.Item2 / 2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2 / 2 - 50), new Pair(Size.Item1 - 0, Size.Item2 / 2 + 50));

        public List<Tuple<Pair, Color, int>> attributes { get; set; }
        public List<Ellipse> shapes { get; set; }

        public Game()
        {
            Players = new Player[4];
            attributes = new List<Tuple<Pair, Color, int>>();
            AddBall();
        }

        private void AddBall()
        {
            var boardCenter = GetCenterOfBoard();
            Ball = new Ball(boardCenter, Ball.MaxSpeed, Ball.Dimension);
        }

        private void AddPlayer()
        {
            var position = new Pair(10, 10);
            var player = new Player(position, Player.MaxSpeed, Player.Dimension, Colour.Red) { game = this };
        }

        private void RemovePlayer(int index)
        {

        }

        private Pair GetCenterOfBoard()
        {
            return new Pair(Size.Item2 / 2 - 3, Size.Item1 / 2 - 3);
        }

        public void UpdatePlayerMovement(PlayerDir mov, int playerIndex)
        {
            this.Players[playerIndex].playerAction = mov;
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

        public int HasScored(Pair a)
        {
            if (a.First > ZoneA.Item1.Second && a.First < ZoneA.Item2.Second && a.Second > ZoneA.Item1.First &&
                a.Second < ZoneA.Item2.First)
            {
                return 0;
            }

            if (a.First > ZoneB.Item1.Second && a.First < ZoneB.Item2.Second && a.Second > ZoneB.Item1.First &&
                a.Second < ZoneB.Item2.First)
            {
                return 1;
            }

            return -1;
        }

        /// <summary>
        ///     Update function. Called from timer every x ticks.
        /// </summary>
        /// <param name="attributes">
        ///     Values sent back to calling method. Used to change appearance and size of objects.
        ///     WARNING Currently entities and attributes are not linked, changing order of one of them will produce unwanted
        ///     behavior.
        /// </param>
        public void Update()
        {
            //Po kolei aktualizujemy obiekty i dodajemy ich atrybuty do listy, z której będą czytane podczas rysowania.
            attributes.Clear();
            foreach (var e in Players.Where(x=>x!=null))
            {
                e.Update();
                attributes.Add(e.GetPositionColorSize());
            }
            attributes.Add(Ball.GetPositionColorSize());
        }
    }
}