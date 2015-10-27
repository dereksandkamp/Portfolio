using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class GameManager
    {
        private Player _player1;
        private Player _player2;
        private Player _currentPlayer;
        public static int DifficultyLevel = 3;

        private Board _board;

        public void PlayGame()
        {
            Initialize();
            int numPlayers = PromptForGameMode();

            if (numPlayers == 0)
            {
                _player1 = CreateComputerPlayer(0);
                _player2 = CreateComputerPlayer(0);
            }
            else if (numPlayers == 1)
            {
                _player1 = PromptToCreatePlayer();
                _player2 = CreateComputerPlayer(1);
            }
            else
            {
                _player1 = PromptToCreatePlayer("X");
                _player2 = PromptToCreatePlayer("O");
            }

            bool playAgain = false;
            do
            {
                _board = new Board();
                playAgain = ProcessTurnsAndAskToPlayAgain();
            } while (playAgain);
        }

        private void Initialize()
        {
            if (Console.LargestWindowHeight >= 24 && Console.LargestWindowWidth >= 51)
            {
                Console.SetWindowSize(51, 24);
                Console.SetBufferSize(51, 24);
                Console.Title = "Tic Tac Toe";
            }
        }

        private int PromptForGameMode()
        {
            TextEffects.DisplayLogo();
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("                     0 ");
            Console.ResetColor();
            Console.WriteLine("Players");
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("                     1 ");
            Console.ResetColor();
            Console.WriteLine("Player");
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("                     2 ");
            Console.ResetColor();
            Console.WriteLine("Players");
            Console.WriteLine();

            int currentCursorTop = Console.CursorTop;
            while (true)
            {
                Console.Write("               Enter your choice: ");
                Console.ForegroundColor = TextEffects.NeutralColor;
                string userInput = Console.ReadLine().Trim();
                Console.ResetColor();

                switch (userInput)
                {
                    case "0":
                        Console.Clear();
                        return 0;
                    case "1":
                        Console.Clear();
                        PromptForDifficultyLevel();
                        Console.Clear();
                        return 1;
                    case "2":
                        Console.Clear();
                        return 2;
                    default:
                        //erase prompt, write error message, erase message, reset cursor
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.Write(" ", Console.WindowWidth);
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.ForegroundColor = TextEffects.ErrorColor;
                        Console.Write("              Enter 0, 1, or 2...             ");
                        Thread.Sleep(1000);
                        Console.ResetColor();
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.Write(" ", Console.WindowWidth);
                        Console.SetCursorPosition(0, currentCursorTop);
                        break;
                }
            }
        }

        private void PromptForDifficultyLevel()
        {
            TextEffects.DisplayLogo();
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("                  1");
            Console.ResetColor();
            Console.WriteLine(". Easy");
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("                  2");
            Console.ResetColor();
            Console.WriteLine(". Normal");
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("                  3");
            Console.ResetColor();
            Console.WriteLine(". Unbeatable");
            Console.WriteLine();

            int currentCursorTop = Console.CursorTop;
            while (true)
            {
                Console.Write("               Enter your choice: ");
                Console.ForegroundColor = TextEffects.NeutralColor;
                string userInput = Console.ReadLine().Trim();
                Console.ResetColor();

                switch (userInput)
                {
                    case "1":
                        Console.Clear();
                        DifficultyLevel = 1;
                        return;
                    case "2":
                        Console.Clear();
                        DifficultyLevel = 2;
                        return;
                    case "3":
                        Console.Clear();
                        DifficultyLevel = 3;
                        return;
                    default:
                        //erase prompt, write error message, erase message, reset cursor
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.Write(" ", Console.WindowWidth);
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.ForegroundColor = TextEffects.ErrorColor;
                        Console.Write("              Enter 1, 2, or 3...             ");
                        Thread.Sleep(1000);
                        Console.ResetColor();
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.Write(" ", Console.WindowWidth);
                        Console.SetCursorPosition(0, currentCursorTop);
                        break;
                }
            }
        }

        private bool AskToPlayAgain()
        {
            //display score
            Console.WriteLine();
            Console.WriteLine(TextEffects.offsetString + "  SCORE");
            Console.WriteLine();
            Console.Write("        {0,12}: ", _player1.Name);
            //Console.ResetColor();
            Console.ForegroundColor = _player1.TextColor;
            Console.WriteLine("{0,3}", _player1.Score);
            Console.ResetColor();
            Console.Write("        {0,12}: ", _player2.Name);
            
            Console.ForegroundColor = _player2.TextColor;
            Console.WriteLine("{0,3}", _player2.Score);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            //ask to play again
            Console.Write(TextEffects.offsetString + "  (");
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("Y");
            Console.ResetColor();
            Console.WriteLine(")es");
            Console.Write(TextEffects.offsetString + "  (");
            Console.ForegroundColor = TextEffects.NeutralColor;
            Console.Write("N");
            Console.ResetColor();
            Console.WriteLine(")o");
            Console.WriteLine();
            int currentCursorTop = Console.CursorTop;
            while (true)
            {
                Console.ForegroundColor = TextEffects.NeutralColor;
                Console.Write("          Would you like to play again? ");
                Console.ForegroundColor = TextEffects.NeutralColor;
                string userInput = Console.ReadLine().Trim().ToUpper();
                Console.ResetColor();
                switch (userInput)
                {
                    case "Y":
                        return true;
                    case "N":
                        return false;
                    default:
                        //erase question
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.Write(" ", Console.WindowWidth);
                        //write error message
                        Console.ForegroundColor = TextEffects.ErrorColor;
                        Console.Write("                  Enter Y or N...                                               ");
                        Console.ResetColor();
                        Thread.Sleep(1200);
                        //erase error message and reset cursor
                        Console.SetCursorPosition(0, currentCursorTop);
                        Console.Write(" ", Console.WindowWidth);
                        Console.SetCursorPosition(0, currentCursorTop);
                        break;
                }
            }
            
        }

        private bool ProcessTurnsAndAskToPlayAgain()
        {
            while (true)
            {
                NextPlayer();
                _board.Display();
                if (_currentPlayer.AI != null) //it's an AI player
                {
                    _board.AddMark(_currentPlayer.Mark, _currentPlayer.AI.TakeTurn(_board.BoardArray));
                    Console.Clear();
                    _board.Display();
                    Console.ForegroundColor = _currentPlayer.TextColor;
                    Console.Write("              " + _currentPlayer.Name + " has spoken!");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }                 
                else
                    PromptUser();
                //check for victory or tie and ask to play again
                if (_board.IsVictory(_currentPlayer))
                {
                    _board.Display();
                    Console.ForegroundColor = _currentPlayer.TextColor;
                    TextEffects.CenterAlignWriteLine(_currentPlayer.Name + " wins!");
                    Console.ResetColor();
                    return AskToPlayAgain();
                }
                else if (_board.IsCatGame())
                {
                    _board.Display();
                    Console.ForegroundColor = TextEffects.NeutralColor;
                    Console.WriteLine(TextEffects.offsetString + "Cats game!");
                    Console.ResetColor();
                    return AskToPlayAgain();
                }
            }
        }

        private void PromptUser()
        {
            // get user input and mark the board
            int currentCursorTop = Console.CursorTop;
            int boardPosition = 0;
            while (true)
            {

                Console.ForegroundColor = _currentPlayer.TextColor;
                //space it over a little
                Console.Write("     ");
                Console.Write(_currentPlayer.Name);
                Console.ResetColor();
                Console.Write(", enter a board position to mark: ");
                Console.ForegroundColor = _currentPlayer.TextColor;
                string userInput = Console.ReadLine();
                Console.ResetColor();
                if (int.TryParse(userInput, out boardPosition))
                    if (boardPosition > 0 && boardPosition < 10)
                    {
                        if (_board.AddMark(_currentPlayer.Mark, boardPosition))
                        {
                            Console.Clear();
                            break;
                        }
                        else
                        {
                            //write error message, erase it, and reset the cursor
                            Console.SetCursorPosition(0, currentCursorTop);
                            Console.ForegroundColor = TextEffects.ErrorColor;
                            Console.Write("         That spot has already been marked!                                          ");
                            Console.ResetColor();
                            Thread.Sleep(1500);
                            Console.SetCursorPosition(0, currentCursorTop);
                            Console.Write("                                                                            ");
                            Console.SetCursorPosition(0, currentCursorTop);
                            continue;
                        }                           
                    }

                //if we got to this point, the input was invalid. Write error message, erase it, and reset the cursor
                Console.SetCursorPosition(0, currentCursorTop);
                Console.ForegroundColor = TextEffects.ErrorColor;
                Console.Write("        Must be a number between 1 and 9...                                          ");
                Console.ResetColor();
                Thread.Sleep(1500);
                Console.SetCursorPosition(0, currentCursorTop);
                Console.Write("                                                                            ");
                Console.SetCursorPosition(0, currentCursorTop);
            }  
        }

        private void NextPlayer()
        {
            if (_currentPlayer == null) //start of game - X goes first
            {
                _currentPlayer = _player1.Mark == "X" ? _player1 : _player2;
            }
            else if (_currentPlayer == _player2)
            {
                _currentPlayer = _player1;
            }
            else
            {
                _currentPlayer = _player2;
            }
        }

        private Player PromptToCreatePlayer(string mark = null)
        {
            TextEffects.DisplayLogo();
            int currentCursorTop = Console.CursorTop;
            string userInput = null;
            if (mark == null)
            {
                bool validInput = false;
                //prompt for their choice of mark
                while (!validInput)
                {
                    Console.WriteLine("                   X goes first.                   ");
                    Console.WriteLine();
                    Console.Write("          Would you like to be ");
                    Console.ForegroundColor = TextEffects.XMarkColor;
                    Console.Write("Xs ");
                    Console.ResetColor();
                    Console.Write("or ");
                    Console.ForegroundColor = TextEffects.OMarkColor;
                    Console.Write("Os");
                    Console.ResetColor();
                    Console.Write("? ");

                    Console.ForegroundColor = TextEffects.NeutralColor;
                    userInput = Console.ReadLine().Trim().ToUpper();
                    Console.ResetColor();
                    switch (userInput)
                    {
                        case "X":
                        case "XS":
                            mark = "X";
                            Console.SetCursorPosition(0, currentCursorTop);
                            Console.Write("                                                   ");
                            Console.Write("                                                   ");
                            Console.Write("                                                   ");
                            Console.Write("                                                   ");
                            Console.SetCursorPosition(0, currentCursorTop);
                            validInput = true;
                            break;
                        case "O":
                        case "OS":
                            mark = "O";
                            Console.SetCursorPosition(0, currentCursorTop);
                            Console.Write("                                                   ");
                            Console.Write("                                                   ");
                            Console.Write("                                                   ");
                            Console.Write("                                                   ");
                            Console.SetCursorPosition(0, currentCursorTop);
                            validInput = true;
                            break;
                        default:
                            Console.SetCursorPosition(0, currentCursorTop);
                            Console.WriteLine();
                            Console.WriteLine("                                                   ");
                            Console.WriteLine("                                                   ");
                            Console.WriteLine("                                                   ");
                            Console.SetCursorPosition(0, currentCursorTop);
                            Console.ForegroundColor = TextEffects.ErrorColor;
                            Console.WriteLine("                  Enter X or O...                  ");
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            //erase the error message and reset cursor
                            Console.SetCursorPosition(0, currentCursorTop);
                            Console.Write(" ", Console.WindowWidth);
                            Console.SetCursorPosition(0, currentCursorTop);
                            continue;
                    }
                }
            }

            ConsoleColor playerTextColor = mark == "X" ? TextEffects.XMarkColor : TextEffects.OMarkColor;
            while (true)
            {
                //space it over
                Console.Write("   ");
                Console.Write("Player \"");
                Console.ForegroundColor = playerTextColor;
                Console.Write(mark);
                Console.ResetColor();
                Console.Write("\" please enter your name: ");
                Console.ForegroundColor = playerTextColor;
                userInput = Console.ReadLine();
                Console.ResetColor();
                if (userInput.Length < 1)
                {
                    Console.SetCursorPosition(0, currentCursorTop);
                    Console.ForegroundColor = TextEffects.ErrorColor;
                    Console.WriteLine("                Please try again...                ");
                    Console.ResetColor();
                    Thread.Sleep(800);
                    //erase the error message and reset cursor
                    Console.SetCursorPosition(0, currentCursorTop);
                    Console.Write("                   ");
                    Console.SetCursorPosition(0, currentCursorTop);
                    continue;
                }
                else if (userInput.Length > 12)
                {
                    Console.SetCursorPosition(0, currentCursorTop);
                    Console.ForegroundColor = TextEffects.ErrorColor;
                    Console.WriteLine(" Sorry, your name can only be 12 characters long.                                  ");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    //erase the error message and reset cursor
                    Console.SetCursorPosition(0, currentCursorTop);
                    Console.Write("                                                                            ");
                    Console.SetCursorPosition(0, currentCursorTop);
                    continue;
                }
                Console.SetCursorPosition(0, currentCursorTop);
                Console.Write("                                                                            ");
                Console.SetCursorPosition(0, currentCursorTop);
                break;
            }

            //say hello to the player in their color, then erase message
            Console.ForegroundColor = playerTextColor;
            //space it over
            Console.Write("                    ");
            Console.Write("Hello, " + userInput + "!");
            Thread.Sleep(1000);
            Console.Clear();
            Console.ResetColor();

            //return new Player(userInput, mark, false, playerTextColor);
            return new Player() {Mark = mark, Name = userInput, TextColor = playerTextColor};
        }

        private Player CreateComputerPlayer(int numHumans)
        {
            if (_player1 == null)
            {
                //this is the first player, and we know it's an AI. That means that we are in 0 human player mode. (a human player would have been player 1)
                return new Player() {Mark = "X", AI = new AI(Mark.X, Mark.O), Name = "Computer 1", TextColor = TextEffects.XMarkColor};    
            }

            else //that means we've created a player already (human or otherwise)
            {
                Player computerPlayerToCreate = new Player();

                if (numHumans == 0)
                    computerPlayerToCreate.Name = "Computer 2";
                else
                    computerPlayerToCreate.Name = "The Computer";

                if (_player1.Mark == "X")
                {
                    computerPlayerToCreate.Mark = "O";
                    computerPlayerToCreate.TextColor = TextEffects.OMarkColor;
                    computerPlayerToCreate.AI = new AI(Mark.O, Mark.X);
                }
                else
                {
                    computerPlayerToCreate.Mark = "X";
                    computerPlayerToCreate.TextColor = TextEffects.XMarkColor;
                    computerPlayerToCreate.AI = new AI(Mark.X, Mark.O);
                }
                return computerPlayerToCreate;
            }
        }
    }
}
