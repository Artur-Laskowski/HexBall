using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace HexBall
{
    internal class Game
    {

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

        private PlayerDir[] moves;
        private PlayerDir playerDir;

        public static int ScoreA = 0;
        public static int ScoreB = 0;

        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2/2 - 50), new Pair(40, Size.Item2/2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2/2 - 50), new Pair(Size.Item1 - 0, Size.Item2/2 + 50));
        

        public Game()
        {
            Entities = new List<Entity>();
            //Placeholders. Naturally objects will be added dynamicly.
            //TODO make it dynamic.

            var ball = new Ball(new Pair(Size.Item2 / 2 - 3, Size.Item1 / 2 - 3), 3, 10);
            Entities.Add(ball);
            for (int i = 0; i < 4; i++)
            {
                int[] color = {255, 0, 0};
                if (i >= 2)
                {
                    color = new[] {0, 0, 255};
                }
                var player = new Player(new Pair(10 + i * 30, 10), 1, 20, color);
                Entities.Add(player);
            }
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
        public void Update(out List<Tuple<Pair, int>> attributes)
        {
            attributes = new List<Tuple<Pair, int>>();

            

            attributes.Clear();

            //Po kolei aktualizujemy obiekty i dodajemy ich atrybuty do listy, z której będą czytane podczas rysowania.
            for (int i = 0; i < Entities.Count; i++)
            {
                playerDir = moves[i];
                Entities[i].Update();
                attributes.Add(new Tuple<Pair, int>(Entities[i].Position, Entities[i].Size));
                moves[i] = PlayerDir.NoMove;
            }
        }
    }
}