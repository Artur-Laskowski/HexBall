using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace HexBall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game;
        List<Ellipse> shapes;
        List<Tuple<Pair, Color, int>> attributes;
        public MainWindow()
        {
            InitializeComponent();

            game = new Game();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(10000/60);
            dispatcherTimer.Start();

            shapes = new List<Ellipse>();

            attributes = new List<Tuple<Pair, Color, int>>();

            //We create 5 ellipses (4 players and 1 ball).
            //Unused ones have 0 size so are not visible.
            //TODO do it dynamically?
            //Having zero size shapes shouldn't cause problems, they have no game object tied to them.
            //If we need them, we change their size and color.
            for (int i = 0; i < 5; i++) {
                Ellipse myEllipse = new Ellipse();

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
                myEllipse.Width = 0;
                myEllipse.Height = 0; //All 5 possible objects


                //myEllipse.SetValue(Canvas.TopProperty, 10);
                //myEllipse.SetValue(Canvas.LeftProperty, 10);

                shapes.Add(myEllipse);

                canvas1.Children.Add(myEllipse);
            }
        }
        
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            game.Update(out attributes);

            

            for (int i = 0; i < attributes.Count; i++)
            {
                Ellipse shape = shapes.ElementAt(i);
                Tuple<Pair, Color, int> attribute = attributes.ElementAt(i);
                shape.SetValue(Canvas.TopProperty, attribute.Item1.First);
                shape.SetValue(Canvas.LeftProperty, attribute.Item1.Second);

                SolidColorBrush mySolidColorBrush = new SolidColorBrush(attribute.Item2);
                shape.Fill = mySolidColorBrush;

                shape.Width = attribute.Item3;
                shape.Height = attribute.Item3;
            }
        }
    }
}
