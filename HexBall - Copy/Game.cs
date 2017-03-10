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
        /// Playing field's size
        /// </summary>
        static Tuple<int, int> size = new Tuple<int, int>(100, 100);

        public enum PlayerDir
        {
            noMove, up, rightUp, right, rightDown, down, leftDown, left, leftUp
        }
        public static PlayerDir playerDir { get; set; }

        public const double movementSpeed = 0.5;

        /// <summary>
        /// Time between updates
        /// </summary>
        public const double time_delta = 0.1;

        private List<Entity> entities;

        private Canvas drawSpace;

        Ellipse myEllipse;

        public Game(Canvas canvas)
        {
            entities = new List<Entity>();
            Player player1 = new Player(new Pair(10, 10), 10, 10);
            entities.Add(player1);
            drawSpace = canvas;



            myEllipse = new Ellipse();

            // Create a SolidColorBrush with a red color to fill the 
            // Ellipse with.
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            myEllipse.Width = 10;
            myEllipse.Height = 10;
            //TranslateTransform tt = new TranslateTransform(e.Position.First, e.Position.Second);
            //myEllipse.RenderTransform = tt;

            drawSpace.Children.Add(myEllipse);
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

        public void Update()
        {
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
            foreach (Entity e in entities)
            {
                e.Update();

                myEllipse.SetValue(Canvas.TopProperty, e.Position.First*10);
                myEllipse.SetValue(Canvas.LeftProperty, e.Position.Second*10);
            }
            Thread.Sleep(100 / 6);
        }


    }
}
