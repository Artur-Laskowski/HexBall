using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

    internal class Game
    {

        public Canvas canv;
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
        public Ball ball;
        public List<Player> players;

        public int ScoreA = 0;
        public int ScoreB = 0;

        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2 / 2 - 50), new Pair(40, Size.Item2 / 2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2 / 2 - 50), new Pair(Size.Item1 - 0, Size.Item2 / 2 + 50));

        private List<Tuple<Pair, Color, int>> _attributes;
        private List<Ellipse> _shapes;

        public Game(Canvas c)
        {
            this.canv = c;
            players = new List<Player>();
            //Placeholders. Naturally objects will be added dynamicly.
            //TODO make it dynamic.

            var position = new Pair(10, 10);
            AddPlayer(position);

            AddBall();
        }

        private void AddBall()
        {
            var boardCenter = GetCenterOfBoard();
            ball = new Ball(boardCenter, Ball.MaxSpeed, Ball.Dimension);
        }

        private void AddPlayer(Pair position)
        {
            var player = new Player(position, Player.MaxSpeed, Player.Dimension, Colour.Red) { game = this };
            players.Add(player);
        }

        private Pair GetCenterOfBoard()
        {
            return new Pair(Size.Item2 / 2 - 3, Size.Item1 / 2 - 3);
        }

        public void InitCanvas()
        {
            _shapes = new List<Ellipse>();

            _attributes = new List<Tuple<Pair, Color, int>>();

            //We create 5 ellipses (4 players and 1 ball).
            //Unused ones have 0 size so are not visible.
            //TODO do it dynamically?
            //Having zero size shapes shouldn't cause problems, they have no game object tied to them.
            //If we need them, we change their size and color.
            for (var i = 0; i < 5; i++)
            {
                var myEllipse = new Ellipse();

                // Create a SolidColorBrush with a red color to fill the 
                // Ellipse with.
                var mySolidColorBrush = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 0) };

                // Describes the brush's color using RGB values. 
                // Each value has a range of 0-255.
                myEllipse.Fill = mySolidColorBrush;
                myEllipse.StrokeThickness = 2;
                myEllipse.Stroke = Brushes.Black;

                // Set the width and height of the Ellipse.
                myEllipse.Width = 0;
                myEllipse.Height = 0; //All 5 possible objects

                //myEllipse.SetValue(Canvas.TopProperty, 10);
                //myEllipse.SetValue(Canvas.LeftProperty, 10);

                _shapes.Add(myEllipse);

                canv.Children.Add(myEllipse);
            }

            var brush = new SolidColorBrush { Color = Color.FromArgb(200, 200, 200, 0) };

            var zoneA = new Rectangle
            {
                Fill = brush,
                Width = Math.Abs(Game.ZoneA.Item1.First - Game.ZoneA.Item2.First),
                Height = Math.Abs(Game.ZoneA.Item1.Second - Game.ZoneA.Item2.Second)
            };
            canv.Children.Add(zoneA);
            zoneA.SetValue(Canvas.TopProperty, Game.ZoneA.Item1.Second);
            zoneA.SetValue(Canvas.LeftProperty, Game.ZoneA.Item1.First);

            var zoneB = new Rectangle
            {
                Fill = brush,
                Width = Math.Abs(Game.ZoneB.Item1.First - Game.ZoneB.Item2.First),
                Height = Math.Abs(Game.ZoneB.Item1.Second - Game.ZoneB.Item2.Second)
            };
            canv.Children.Add(zoneB);
            zoneB.SetValue(Canvas.TopProperty, Game.ZoneB.Item1.Second);
            zoneB.SetValue(Canvas.LeftProperty, Game.ZoneB.Item1.First);
        }

        public void UpdateCanvas()
        {
            this.Update(out _attributes);

            for (var i = 0; i < _attributes.Count; i++)
            {
                var shape = _shapes.ElementAt(i);
                var attribute = _attributes.ElementAt(i);

                shape.SetValue(Canvas.TopProperty, attribute.Item1.First);
                shape.SetValue(Canvas.LeftProperty, attribute.Item1.Second);

                var mySolidColorBrush = new SolidColorBrush(attribute.Item2);
                shape.Fill = mySolidColorBrush;

                shape.Width = attribute.Item3;
                shape.Height = attribute.Item3;
            }
        }

        public void UpdatePlayerMovement(PlayerDir mov, int playerIndex)
        {
            this.players[playerIndex].playerAction = mov;
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
        public void Update(out List<Tuple<Pair, Color, int>> attributes)
        {
            attributes = new List<Tuple<Pair, Color, int>>();

            //Po kolei aktualizujemy obiekty i dodajemy ich atrybuty do listy, z której będą czytane podczas rysowania.
            attributes.Clear();
            foreach (var e in players)
            {
                e.Update();
                attributes.Add(new Tuple<Pair, Color, int>(e.Position, e.EntityColor, e.Size));
            }
        }
    }
}