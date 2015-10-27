using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class AIBoard
    {
        public Position[] Positions = new Position[10];
        public List<int> UnmarkedPositions = new List<int>(); 

        public AIBoard(string[] inBoard)
        {
            //make the board
            for (int i = 0; i < inBoard.Length; i++)
            {
                Mark mark = Mark.Unmarked;
                if (inBoard[i] == "X")
                    mark = Mark.X;
                else if (inBoard[i] == "O")
                    mark = Mark.O;

                Positions[i + 1] = new Position()
                {
                    PositionNum = i + 1,
                    Mark = mark,
                };
            }
            //make the unmarked positions array
            for (int i = 1; i < Positions.Length; i++)
            {
                if (Positions[i].Mark == Mark.Unmarked)
                    UnmarkedPositions.Add(Positions[i].PositionNum);
            }
        }

        public AIBoard(Position[] inArray)
        {
            this.Positions = inArray;
        }

        public AIBoard Clone()
        {
            //clone the positions array, and make new position objects. otherwise we would be sending the reference. (oops)
            Position[] clonedPositionsArr = new Position[10];
            for (int i = 1; i <= 9; i++)
            {
                clonedPositionsArr[i] = new Position() {Mark = this.Positions[i].Mark, PositionNum = this.Positions[i].PositionNum };
            }
            return new AIBoard(clonedPositionsArr);
        }
    }
}
