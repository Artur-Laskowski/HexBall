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
        public EntityAttr[] attributes { get; set; }
        public Ellipse[] shapes { get; set; }
        public static readonly Tuple<Pair, Pair> ZoneA = new Tuple<Pair, Pair>(new Pair(0, Size.Item2 / 2 - 50), new Pair(40, Size.Item2 / 2 + 50));
        public static readonly Tuple<Pair, Pair> ZoneB = new Tuple<Pair, Pair>(new Pair(Size.Item1 - 40, Size.Item2 / 2 - 50), new Pair(Size.Item1 - 0, Size.Item2 / 2 + 50));




        public MainWindow()
        {
            InitializeComponent();
            this.shapes = new Ellipse[Game.EntityAttrsSize];
            InitCanvas();
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(10000 / 60);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (cc == null)
                return;
            UpdateCanvas();
            UpdateScore();
        }

        private void UpdateScore()
        {
            scoreLabelA.Content = cc.ScoreA.ToString();
            scoreLabelB.Content = cc.ScoreB.ToString();
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
            if (this.cc == null)
                return;
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
            this.cc.playerMovement = playerMovement;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.cc == null)
                return;
            this.cc.playerMovement = PlayerDir.NoMove;
        }



        public void InitCanvas()
        {
            for (var i = 0; i < Game.EntityAttrsSize; i++)
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

                shapes[i] = myEllipse;

                canv.Children.Add(myEllipse);
            }



            AddGoalShape(Game.ZoneA.Item1, Game.ZoneA.Item2);
            AddGoalShape(Game.ZoneB.Item1, Game.ZoneB.Item2);
        }
        private void AddGoalShape(Pair startPair, Pair endPair)
        {
            var brush = new SolidColorBrush { Color = Colour.YellowTransparent };
            var goal = new Rectangle
            {
                Fill = brush,
                Width = Math.Abs(startPair.First - endPair.First),
                Height = Math.Abs(startPair.Second - endPair.Second)
            };
            canv.Children.Add(goal);
            goal.SetValue(Canvas.TopProperty, startPair.Second);
            goal.SetValue(Canvas.LeftProperty, startPair.First);
        }


        public void UpdateCanvas()
        {
            attributes = this.cc.GetSetAttributes();
            for (var i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] == null)
                {
                    if (shapes[i].IsVisible())
                        shapes[i].Hide();
                    continue;
                }

                Ellipse shape = shapes.ElementAt(i);
                EntityAttr attribute = attributes.ElementAt(i);

                SetShapePosition(shape, attribute.Position);

                var mySolidColorBrush = new SolidColorBrush(attribute.GetColor());
                shape.Fill = mySolidColorBrush;
                if (i == playerIndex)
                {
                    shape.Stroke = Brushes.White;
                    shape.StrokeThickness = 2;
                }

                shape.SetSize(attribute.Size);
            }
        }

        private void SetShapePosition(Shape shape, Pair position)
        {
            shape.SetValue(Canvas.TopProperty, position.First);
            shape.SetValue(Canvas.LeftProperty, position.Second);
        }

        private void CloseConnection(object sender, object e)
        {
            this.cc.CloseConnection();
        }
    }
}
