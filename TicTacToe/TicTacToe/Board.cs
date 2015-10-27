using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Board
    {
        public string[] BoardArray { get; }
        private int _turnsCounter = 0;

        public Board()
        {
            BoardArray = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            AI.TurnsCounter = 0;
        }

        public bool AddMark(string mark, int position)
        {
            if (BoardArray[position-1] == position.ToString())
            {
                BoardArray[position-1] = mark;
                _turnsCounter ++;
                AI.TurnsCounter++;
                return true;
            }

            return false;
        }

        public void Display()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            DisplayRow(1);
            Console.WriteLine(TextEffects.offsetString + "-----------");
            DisplayRow(2);
            Console.WriteLine(TextEffects.offsetString + "-----------");
            DisplayRow(3);

            Console.WriteLine();
            Console.WriteLine();
        }

        private void DisplayRow(int rowNumber)
        {
            int[] rowIndexes = null;
            switch(rowNumber)
            {
                case 1:
                    rowIndexes = new int[] { 0, 1, 2 };
                    break;
                case 2:
                    rowIndexes = new int[] { 3, 4, 5 };
                    break;
                case 3:
                    rowIndexes = new int[] { 6, 7, 8 };
                    break;
            }
            //space it over a little
            Console.Write(TextEffects.offsetString);
            
            //now write the actual row
            Console.Write(" ");
            DisplayMark(rowIndexes[0]);
            Console.Write(" | ");
            DisplayMark(rowIndexes[1]);
            Console.Write(" | ");
            DisplayMark(rowIndexes[2]);

            Console.WriteLine();
        }

        private void DisplayMark(int boardArrayIndex)
        {
            switch(BoardArray[boardArrayIndex])
            {
                case "X":
                    Console.ForegroundColor = TextEffects.XMarkColor;
                    break;
                case "O":
                    Console.ForegroundColor = TextEffects.OMarkColor;
                    break;
            }
            Console.Write(BoardArray[boardArrayIndex]);
            Console.ResetColor();
        }

        public bool IsVictory(Player currentPlayer)
        {
            bool victory = IsHorizontalWin(currentPlayer.Mark) || IsVerticalWin(currentPlayer.Mark) || IsDiagonalWin(currentPlayer.Mark);
            if (victory)
                currentPlayer.Score++;
            return victory;
        }

        private bool IsHorizontalWin(string mark)
        {
            return (BoardArray[0] == mark && BoardArray[1] == mark && BoardArray[2] == mark) ||
                   (BoardArray[3] == mark && BoardArray[4] == mark && BoardArray[5] == mark) ||
                   (BoardArray[6] == mark && BoardArray[7] == mark && BoardArray[8] == mark);
        }

        private bool IsVerticalWin(string mark)
        {
            return (BoardArray[0] == mark && BoardArray[3] == mark && BoardArray[6] == mark) ||
                   (BoardArray[1] == mark && BoardArray[4] == mark && BoardArray[7] == mark) ||
                   (BoardArray[2] == mark && BoardArray[5] == mark && BoardArray[8] == mark);
        }

        private bool IsDiagonalWin(string mark)
        {
            return (BoardArray[0] == mark && BoardArray[4] == mark && BoardArray[8] == mark) ||
                   (BoardArray[2] == mark && BoardArray[4] == mark && BoardArray[6] == mark);
        }

        public bool IsCatGame()
        {
            Display();
            return _turnsCounter == 9;
        }
    }
}
