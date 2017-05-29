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
            
            _game = new Game();
            //_game.InitCanvas();
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(10000 / 60);
            dispatcherTimer.Start();

            //canvas1 = _game.canv;

        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //_game.UpdateCanvas();
        }


        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
            var keyD = Keyboard.IsKeyDown(Key.D);
            var keyW = Keyboard.IsKeyDown(Key.W);
            var keyA = Keyboard.IsKeyDown(Key.A);
            var keyS = Keyboard.IsKeyDown(Key.S);
            //var space = Keyboard.IsKeyDown(Key.Space);
            var playerMovement = PlayerDir.NoMove;
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
            _game.UpdatePlayerMovement(playerMovement, 0);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.Window_KeyDown(null, null);
        }
    }
}