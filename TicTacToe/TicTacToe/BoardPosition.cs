using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class BoardPosition
    {
        public int Position;
        public Mark Status;

        public BoardPosition(int position, Mark status)
        {
            this.Position = position;
            this.Status = status;
        }

        public BoardPosition(int position) : this(position, Mark.Unmarked)
        {
        }

    }
}
