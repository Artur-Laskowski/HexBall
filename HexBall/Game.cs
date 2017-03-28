using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace HexBall
{
    internal class Game
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
            LeftUp
        }

        /// <summary>
        ///     Player movement speed. TODO change it based on some conditions?
        /// </summary>
        public const double MovementSpeed = 0.2;

        /// <summary>
        ///     Time between updates
        /// </summary>
        public const double TimeDelta = 0.04;
        

        /// <summary>
        ///     Playing field's size.
        /// </summary>
        public static readonly Tuple<int, int> Size = new Tuple<int, int>(800, 400);

        /// <summary>
        ///     List of all entities. TODO make it static so it can be easily accessed by entites when colliding?
        /// </summary>
        public static List<Entity> Entities;

        public static int ScoreA = 0;
        public static int ScoreB = 0;

        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2/2 - 50), new Pair(40, Size.Item2/2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2/2 - 50), new Pair(Size.Item1 - 0, Size.Item2/2 + 50));
        

        public Game()
        {
            Entities = new List<Entity>();
            //Placeholders. Naturally objects will be added dynamicly.
            //TODO make it dynamic.
            var player1 = new Player(new Pair(10, 10), 1, 20, Color.FromRgb(255, 0, 0));
            var ball = new Ball(new Pair(Size.Item2 / 2 - 3, Size.Item1 / 2 - 3), 3, 10);
            Entities.Add(ball);
            Entities.Add(player1);
        }

        /// <summary>
        ///     Player's movement direction. Retrieved by player object and used to change velocity.
        /// </summary>
        public static PlayerDir PlayerDirection { get; set; }

        /// <summary>
        ///     Checks whether position is valid.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>bool - is in bounds</returns>
        public static bool IsInBounds(Pair a)
        {
            return a.First >= 0 && a.First <= Size.Item1 && a.Second >= 0 && a.Second <= Size.Item2;
        }

        public static bool IsInBounds(Pair a, int margin)
        {
            return a.First >= margin && a.First <= Size.Item2 - margin && a.Second >= margin &&
                   a.Second <= Size.Item1 - margin;
        }

        public static int HasScored(Pair a)
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
        public void Update(out List<Tuple<Pair, Color, int>> attributes)
        {
            attributes = new List<Tuple<Pair, Color, int>>();

            //TODO see if this can be done more pretty
            var w = Keyboard.IsKeyDown(Key.D);
            var a = Keyboard.IsKeyDown(Key.W);
            var s = Keyboard.IsKeyDown(Key.A);
            var d = Keyboard.IsKeyDown(Key.S);
            //var space = Keyboard.IsKeyDown(Key.Space);
            PlayerDirection = PlayerDir.NoMove;
            if (w)
            {
                if (a)
                    PlayerDirection = PlayerDir.LeftUp;
                if (d)
                    PlayerDirection = PlayerDir.RightUp;
                if (!a && !d)
                    PlayerDirection = PlayerDir.Up;
            }
            if (s)
            {
                if (a)
                    PlayerDirection = PlayerDir.LeftDown;
                if (d)
                    PlayerDirection = PlayerDir.RightDown;
                if (!a && !d)
                    PlayerDirection = PlayerDir.Down;
            }
            if (!w && !s)
            {
                if (a)
                    PlayerDirection = PlayerDir.Left;
                if (d)
                    PlayerDirection = PlayerDir.Right;
            }

            attributes.Clear();

            //Po kolei aktualizujemy obiekty i dodajemy ich atrybuty do listy, z której będą czytane podczas rysowania.
            foreach (var e in Entities)
            {
                e.Update();
                attributes.Add(new Tuple<Pair, Color, int>(e.Position, e.EntityColor, e.Size));
            }
        }
    }
}