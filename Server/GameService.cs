using HexBall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GameService
    {
        public Game game { get; set; }

        public GameService()
        {
            this.game = new Game();
        }

        public void Start()
        {
            Task.Run(UpdateGame);
        }

        public async Task UpdateGame()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                game.Update(movement: false);
                Console.Write("G");
            }
        }
    }
}
