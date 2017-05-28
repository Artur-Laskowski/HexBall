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
            
            var w = Keyboard.IsKeyDown(Key.D);
            var a = Keyboard.IsKeyDown(Key.W);
            var s = Keyboard.IsKeyDown(Key.A);
            var d = Keyboard.IsKeyDown(Key.S);
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

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.Window_KeyDown(null, null);
        }
    }
}