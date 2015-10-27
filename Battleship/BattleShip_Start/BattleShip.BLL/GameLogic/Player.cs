using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BattleShip.BLL.GameLogic
{
    public class Player
    {
        public string name { get; set; }
        public Board board { get; }
        public int score;

        public Player(string inName, Board inBoard, int inScore)
        {
            name = inName;
            board = inBoard;
        }
    }
}
