using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Player
    {
        public string Name;
        public string Mark;
        public AI AI;
        public int Score;
        public ConsoleColor TextColor;
        public Player()
        {
            this.Score = 0;
        }
    }
}
