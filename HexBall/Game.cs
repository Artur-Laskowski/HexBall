using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading;

namespace HexBall
{
    class Game
    {
        /// <summary>
        /// Playing field's size.
        /// </summary>
        static Tuple<int, int> size = new Tuple<int, int>(500, 200);

        public enum PlayerDir
        {
            noMove, up, rightUp, right, rightDown, down, leftDown, left, leftUp
        }

        /// <summary>
        /// Player's movement direction. Retrieved by player object and used to change velocity.
        /// </summary>
        public static PlayerDir playerDir { get; set; }

        /// <summary>
        /// Player movement speed. TODO change it based on some conditions?
        /// </summary>
        public const double movementSpeed = 0.1;

        /// <summary>
        /// Time between updates
        /// </summary>
        public const double time_delta = 0.1;

        /// <summary>
        /// List of all entities. TODO make it static so it can be easily accessed by entites when colliding?
        /// </summary>
        private List<Entity> entities;

        public Game()
        {
            entities = new List<Entity>();
            //Placeholders. Naturally objects will be added dynamicly.
            //TODO make it dynamic.
            Player player1 = new Player(new Pair(10, 10), 1, 20, Color.FromRgb(255,0,0));
            Ball ball = new Ball(new Pair(20,20), 5, 10);
            entities.Add(ball);
            entities.Add(player1);
        }

        /// <summary>
        /// Checks whether position is valid.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>bool - is in bounds</returns>
        public static bool IsInBounds(Pair a)
        {
            return a.First >= 0 && a.First <= size.Item1 && a.Second >= 0 && a.Second <= size.Item2;
        }

        /// <summary>
        /// Update function. Called from timer every x ticks.
        /// </summary>
        /// <param name="attributes">
        /// Values sent back to calling method. Used to change appearance and size of objects.
        /// WARNING Currently entities and attributes are not linked, changing order of one of them will produce unwanted behavior.
        /// </param>
        public void Update(out List<Tuple<Pair, Color, int>> attributes)
        {
            attributes = new List<Tuple<Pair, Color, int>>();

            //TODO see if this can be done more pretty
            bool w = Keyboard.IsKeyDown(Key.W);
            bool a = Keyboard.IsKeyDown(Key.A);
            bool s = Keyboard.IsKeyDown(Key.S);
            bool d = Keyboard.IsKeyDown(Key.D);
            bool space = Keyboard.IsKeyDown(Key.Space);
            playerDir = PlayerDir.noMove;
            if (w)
            {
                if (a)
                {
                    playerDir = PlayerDir.leftUp;
                }
                if (d)
                {
                    playerDir = PlayerDir.rightUp;
                }
                if (!a && !d)
                {
                    playerDir = PlayerDir.up;
                }
            }
            if (s)
            {
                if (a)
                {
                    playerDir = PlayerDir.leftDown;
                }
                if (d)
                {
                    playerDir = PlayerDir.rightDown;
                }
                if (!a && !d)
                {
                    playerDir = PlayerDir.down;
                }
            }
            if (!w && !s)
            {
                if (a)
                {
                    playerDir = PlayerDir.left;
                }
                if (d)
                {
                    playerDir = PlayerDir.right;
                }
            }

            attributes.Clear();

            //Po kolei aktualizujemy obiekty i dodajemy ich atrybuty do listy, z której będą czytane podczas rysowania.
            foreach (Entity e in entities)
            {
                e.Update();
                attributes.Add(new Tuple<Pair, Color, int>(e.Position, e.EntityColor, e.Size));
            }
        }
    }
}
