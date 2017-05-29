using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HexBall;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int playerIndex;
        private ConnectionController cc;

        //Game.cs
        public static readonly Tuple<int, int> Size = new Tuple<int, int>(800, 400);
        public List<Tuple<Pair, Color, int>> attributes { get; set; }
        public List<Ellipse> shapes { get; set; }
        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2 / 2 - 50), new Pair(40, Size.Item2 / 2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2 / 2 - 50), new Pair(Size.Item1 - 0, Size.Item2 / 2 + 50));




        public MainWindow()
        {
            InitializeComponent();
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(10000 / 60);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //chyba niepotrzebne bo uaktualniamy plansze na dane z serwera
        }


        private void Connect_Server(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = this.IPBox.Text;
                int port = int.Parse(this.PortBox.Text);
                cc = new ConnectionController(ip, port);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var keyD = Keyboard.IsKeyDown(Key.D);
            var keyW = Keyboard.IsKeyDown(Key.W);
            var keyA = Keyboard.IsKeyDown(Key.A);
            var keyS = Keyboard.IsKeyDown(Key.S);
            //var space = Keyboard.IsKeyDown(Key.Space);
            PlayerDir playerMovement = PlayerDir.NoMove;
            if (keyD)
            {
                if (keyW)
                    playerMovement = PlayerDir.LeftUp;
                if (keyS)
                    playerMovement = PlayerDir.RightUp;
                if (!keyW && !keyS)
                    playerMovement = PlayerDir.Up;
            }
            if (keyA)
            {
                if (keyW)
                    playerMovement = PlayerDir.LeftDown;
                if (keyS)
                    playerMovement = PlayerDir.RightDown;
                if (!keyW && !keyS)
                    playerMovement = PlayerDir.Down;
            }
            if (!keyD && !keyA)
            {
                if (keyW)
                    playerMovement = PlayerDir.Left;
                if (keyS)
                    playerMovement = PlayerDir.Right;
            }
            //TODO wyslac ruch do serwera
            this.cc.playerMovement = playerMovement;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.cc.playerMovement = PlayerDir.NoMove;
        }



        public void InitCanvas()
        {
            shapes = new List<Ellipse>();

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
                myEllipse.Stroke = (i == this.cc.playerIndex) ? Brushes.White : Brushes.Black;

                // Set the width and height of the Ellipse.
                myEllipse.Width = 0;
                myEllipse.Height = 0; //All 5 possible objects

                //myEllipse.SetValue(Canvas.TopProperty, 10);
                //myEllipse.SetValue(Canvas.LeftProperty, 10);

                shapes.Add(myEllipse);

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
            var _attributes = this.cc.GetSetAttributes();

            for (var i = 0; i < _attributes.Count; i++)
            {
                var shape = shapes.ElementAt(i);
                var attribute = _attributes.ElementAt(i);

                shape.SetValue(Canvas.TopProperty, attribute.Item1.First);
                shape.SetValue(Canvas.LeftProperty, attribute.Item1.Second);

                var mySolidColorBrush = new SolidColorBrush(attribute.Item2);
                shape.Fill = mySolidColorBrush;

                shape.Width = attribute.Item3;
                shape.Height = attribute.Item3;
            }
        }

    }
}
