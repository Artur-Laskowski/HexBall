using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
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
        
        private readonly Game _game;
        

        public MainWindow()
        {
            InitializeComponent();
            
            _game = new Game(canvas1);
            _game.InitCanvas();
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(10000 / 60);
            dispatcherTimer.Start();

            //canvas1 = _game.canv;

        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            _game.UpdateCanvas();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var w = e.Key == Key.D;
            var a = e.Key == Key.W;
            var s = e.Key == Key.A;
            var d = e.Key == Key.S;
            //var space = Keyboard.IsKeyDown(Key.Space);
            var playerMovement = PlayerDir.NoMove;
            if (w)
            {
                if (a)
                    playerMovement = PlayerDir.LeftUp;
                if (d)
                    playerMovement = PlayerDir.RightUp;
                if (!a && !d)
                    playerMovement = PlayerDir.Up;
            }
            if (s)
            {
                if (a)
                    playerMovement = PlayerDir.LeftDown;
                if (d)
                    playerMovement = PlayerDir.RightDown;
                if (!a && !d)
                    playerMovement = PlayerDir.Down;
            }
            if (!w && !s)
            {
                if (a)
                    playerMovement = PlayerDir.Left;
                if (d)
                    playerMovement = PlayerDir.Right;
            }
            _game.UpdatePlayerMovement(playerMovement, 0);
        }
    }
}