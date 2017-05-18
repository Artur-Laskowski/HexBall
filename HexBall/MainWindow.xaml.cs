using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HexBall
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private List<Tuple<Pair, Color, int>> _attributes;
        private readonly Game _game;
        private readonly List<Ellipse> _shapes;

        public MainWindow()
        {
            InitializeComponent();
            
            _game = new Game();

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(10000 / 60);
            dispatcherTimer.Start();

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

                canvas1.Children.Add(myEllipse);
            }
            
            var brush = new SolidColorBrush { Color = Color.FromArgb(200, 200, 200, 0) };

            var zoneA = new Rectangle
            {
                Fill = brush,
                Width = Math.Abs(Game.ZoneA.Item1.First - Game.ZoneA.Item2.First),
                Height = Math.Abs(Game.ZoneA.Item1.Second - Game.ZoneA.Item2.Second)
            };
            canvas1.Children.Add(zoneA);
            zoneA.SetValue(Canvas.TopProperty, Game.ZoneA.Item1.Second);
            zoneA.SetValue(Canvas.LeftProperty, Game.ZoneA.Item1.First);

            var zoneB = new Rectangle
            {
                Fill = brush,
                Width = Math.Abs(Game.ZoneB.Item1.First - Game.ZoneB.Item2.First),
                Height = Math.Abs(Game.ZoneB.Item1.Second - Game.ZoneB.Item2.Second)
            };
            canvas1.Children.Add(zoneB);
            zoneB.SetValue(Canvas.TopProperty, Game.ZoneB.Item1.Second);
            zoneB.SetValue(Canvas.LeftProperty, Game.ZoneB.Item1.First);

        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            _game.Update(out _attributes);


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

                scoreLabelA.Content = "Team A: " + Game.ScoreA;
                scoreLabelB.Content = "Team B: " + Game.ScoreB;
            }
        }

        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _game.Connect();
        }
    }
}