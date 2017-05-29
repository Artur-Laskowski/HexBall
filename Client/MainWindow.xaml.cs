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
            PlayerDir playerMovement=PlayerDir.NoMove;
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
            cc.playerMovement = playerMovement;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.Window_KeyDown(null, null);
        }
    }
}
