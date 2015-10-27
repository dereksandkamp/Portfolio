using BattleShip.BLL.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.BLL.Ships;
using BattleShip.BLL.Responses;
using BattleShip.BLL.GameLogic;
using System.Threading;

namespace BattleShip.UI
{
    public class GameWorkflow
    {
        public Player player1 { get; }
        public Player player2 { get; }
        public Player activePlayer;
        public Player opponent;

        public GameWorkflow()
        {
            player1 = new Player("", new Board(), 0);
            player2 = new Player("", new Board(), 0);
            activePlayer = player1;
            opponent = player2;
        }

        public void Start()
        {
            MenuAndInitialize();

            bool isRematch = false;
            do
            {
                PlaceAllActivePlayerShips();
                SwitchActivePlayer();
                PlaceAllActivePlayerShips();
                SwitchActivePlayer();

                TextEffects.PrepareToPlayDisplay(activePlayer.name);

                bool victory = false;
                while (!victory)
                {
                    victory = TakeTurn();
                    if (!victory)
                        SwitchActivePlayer();
                }
                isRematch = PlayAgainPrompt();

            } while (isRematch);

            //say goodbye here if you want to
        }

        private void MenuAndInitialize()
        {
            Console.Title = "Battleship";

            bool validFontSize = false;
            while (!validFontSize)
            {
                if (Console.LargestWindowHeight >= 40 && Console.LargestWindowWidth >= 75)
                {
                    Console.SetWindowSize(75, 40);
                    Console.SetBufferSize(75, 40);
                    validFontSize = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please decrease your console font size to play Battleship.");
                    Console.WriteLine("(It's a big board!). Size 16 or below usually works.");
                    Console.WriteLine();
                    Console.WriteLine("Thank you!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("(Press enter to retry)");
                    Console.ReadLine();
                    Console.Clear();
                    Thread.Sleep(200);
                }
            }
            Console.ResetColor();

            TextEffects.BattleshipLogoColor = ConsoleColor.Blue;
            TextEffects.HighlightColor = ConsoleColor.Yellow;
            Console.Clear();

            TextEffects.AnimateBattleshipLogo(false);

            player1.name = GetPlayerName("Player 1 please enter your name: ");
            player2.name = GetPlayerName("Player 2 please enter your name: ");
        }

        private string GetPlayerName(string prompt)
        {
            TextEffects.CenterAlignWrite(prompt);
            Console.ForegroundColor = TextEffects.HighlightColor;
            string name = Console.ReadLine();
            Console.ResetColor();
            Console.Clear();
            TextEffects.DisplayBattleshipLogo();

            Console.ForegroundColor = TextEffects.HighlightColor;
            TextEffects.CenterAlignWrite(string.Format("Hello, {0}!", name));
            Thread.Sleep(1000);
            Console.ResetColor();
            Console.Clear();
            TextEffects.DisplayBattleshipLogo();
            return name;              
        }

        private void PlaceAllActivePlayerShips()
        {
            if (activePlayer == player2)
            {
                Console.WriteLine();
                Console.WriteLine();
            }
                

            AvertYourEyes(activePlayer.name + ", press enter when it is safe to deploy your ships... ");

            Dictionary<ShipType, int> unplacedShips = new Dictionary<ShipType, int>();
            unplacedShips.Add(ShipType.Carrier, 5);
            unplacedShips.Add(ShipType.Battleship, 4);
            unplacedShips.Add(ShipType.Submarine, 3);
            unplacedShips.Add(ShipType.Cruiser, 3);
            unplacedShips.Add(ShipType.Destroyer, 2);

            while (unplacedShips.Count > 0)
            {
                PlaceShipRequest shipRequest = GetShipPlacementRequestFromUser(activePlayer, unplacedShips);
                ShipPlacement placementStatus = activePlayer.board.PlaceShip(shipRequest);

                switch (placementStatus)
                {
                    case ShipPlacement.Ok:
                        unplacedShips.Remove(shipRequest.ShipType);

                        DisplayScreenTitle(activePlayer.name + " Ship Deployment");
                        activePlayer.board.DisplayAll();
                        Console.ForegroundColor = TextEffects.HighlightColor;
                        if (shipRequest.ShipType == ShipType.Carrier)
                        {
                            TextEffects.CenterAlignWriteLine("Aircraft Carrier deployment sucessful!");
                        }      
                        else
                        {
                            TextEffects.CenterAlignWriteLine(shipRequest.ShipType + " deployment sucessful!");
                        }                            
                        Console.WriteLine();
                        Console.ResetColor();
                        TextEffects.CenterAlignWrite("Press enter to continue...");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    case ShipPlacement.NotEnoughSpace:
                        DisplayScreenTitle(activePlayer.name + " Ship Deployment");
                        activePlayer.board.DisplayAll();
                        Console.ForegroundColor = TextEffects.HighlightColor;
                        TextEffects.CenterAlignWriteLine("But captain, that would place the ship outside of the battle zone!");
                        Console.WriteLine();
                        Console.ResetColor();
                        TextEffects.CenterAlignWrite("Let me ask again. (Press enter)...");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    case ShipPlacement.Overlap:
                        DisplayScreenTitle(activePlayer.name + " Ship Deployment");
                        activePlayer.board.DisplayAll();
                        Console.ForegroundColor = TextEffects.HighlightColor;
                        TextEffects.CenterAlignWriteLine("But captain, that would overlap one of our ships!");
                        Console.WriteLine();
                        Console.ResetColor();
                        TextEffects.CenterAlignWrite("Let me ask again. (Press enter)...");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                }
            }
        }

        private PlaceShipRequest GetShipPlacementRequestFromUser(Player activePlayer, Dictionary<ShipType, int> unplacedShips)
        {
            DisplayShipPlacementAndUnplacedShips(activePlayer, unplacedShips);

            PlaceShipRequest placementRequest = new PlaceShipRequest();
            string invalidInputMessage = "Sorry Captain, we couldn't understand you!\n";
            bool validInput = false;

            //get choice of ship from user
            while (!validInput)
            {
                Console.ForegroundColor = TextEffects.HighlightColor;
                TextEffects.CenterAlignWriteLine("Which ship would you like to deploy?");
                Console.ResetColor();
                TextEffects.CenterAlignWrite("Enter your choice: ");
                Console.ForegroundColor = TextEffects.HighlightColor;
                string shipChoice = Console.ReadLine().Replace(" ", "").ToUpper();
                Console.ResetColor();

                switch (shipChoice)
                {
                    case "D":
                    case "DESTROYER":
                        if (unplacedShips.ContainsKey(ShipType.Destroyer))
                        {
                            placementRequest.ShipType = ShipType.Destroyer;
                            validInput = true;
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            DisplayShipPlacementAndUnplacedShips(activePlayer, unplacedShips);
                            TextEffects.CenterAlignWriteLine("You already deployed the Destroyer.");
                            break;
                        }

                    case "S":
                    case "SUBMARINE":
                        if (unplacedShips.ContainsKey(ShipType.Submarine))
                        {
                            placementRequest.ShipType = ShipType.Submarine;
                            validInput = true;
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            DisplayShipPlacementAndUnplacedShips(activePlayer, unplacedShips);
                            TextEffects.CenterAlignWriteLine("You already deployed the Submarine.");
                            break;
                        }

                    case "C":
                    case "CRUISER":
                        if (unplacedShips.ContainsKey(ShipType.Cruiser))
                        {
                            placementRequest.ShipType = ShipType.Cruiser;
                            validInput = true;
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            DisplayShipPlacementAndUnplacedShips(activePlayer, unplacedShips);
                            TextEffects.CenterAlignWriteLine("You already deployed the Cruiser.");
                            break;
                        }
                    case "B":
                    case "BATTLESHIP":
                        if (unplacedShips.ContainsKey(ShipType.Battleship))
                        {
                            placementRequest.ShipType = ShipType.Battleship;
                            validInput = true;
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            DisplayShipPlacementAndUnplacedShips(activePlayer, unplacedShips);
                            TextEffects.CenterAlignWriteLine("You already deployed the Battleship.");
                            break;
                        }
                    case "A":
                    case "AC":
                    case "CARRIER":
                    case "AIRCRAFTCARRIER":
                        if (unplacedShips.ContainsKey(ShipType.Carrier))
                        {
                            placementRequest.ShipType = ShipType.Carrier;
                            validInput = true;
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            DisplayShipPlacementAndUnplacedShips(activePlayer, unplacedShips);
                            TextEffects.CenterAlignWriteLine("You already deployed the Carrier.");
                            break;
                        }
                    default:
                        Console.Clear();
                        DisplayShipPlacementAndUnplacedShips(activePlayer, unplacedShips);
                        Console.ForegroundColor = TextEffects.HighlightColor;
                        TextEffects.CenterAlignWriteLine(invalidInputMessage);
                        Console.ResetColor();
                        break;
                }
            }

            Console.Clear();
            DisplayScreenTitle(activePlayer.name + " Ship Deployment");
            activePlayer.board.DisplayAll();

            placementRequest.Coordinate = GetShipCoordFromUser("Enter a starting coordinate for the " + placementRequest.ShipType.ToString() + " (length " + unplacedShips[placementRequest.ShipType] + "): ", invalidInputMessage);
            Console.Clear();
            DisplayScreenTitle(activePlayer.name + " Ship Deployment");
            activePlayer.board.DisplayAll();

            //now I have to ask for the direction of the ship.
            validInput = false;
            while (!validInput)
            {
                //write the choices of direction, with color changes and leading spaces for center alignment
                for (int i = 1; i <= 33; i++)
                    Console.Write(" ");
                Console.Write("(");
                Console.ForegroundColor = TextEffects.HighlightColor;
                Console.Write("U");
                Console.ResetColor();
                Console.WriteLine(")p");

                for (int i = 1; i <= 33; i++)
                    Console.Write(" ");
                Console.Write("(");
                Console.ForegroundColor = TextEffects.HighlightColor;
                Console.Write("D");
                Console.ResetColor();
                Console.WriteLine(")own");

                for (int i = 1; i <= 33; i++)
                    Console.Write(" ");
                Console.Write("(");
                Console.ForegroundColor = TextEffects.HighlightColor;
                Console.Write("L");
                Console.ResetColor();
                Console.WriteLine(")eft");

                for (int i = 1; i <= 33; i++)
                    Console.Write(" ");
                Console.Write("(");
                Console.ForegroundColor = TextEffects.HighlightColor;
                Console.Write("R");
                Console.ResetColor();
                Console.WriteLine(")ight");
                Console.WriteLine();

                //prompt
                TextEffects.CenterAlignWrite("Starting at " + placementRequest.Coordinate + ", enter a direction to deploy the ship (length " + unplacedShips[placementRequest.ShipType] + "): ");
                Console.ForegroundColor = TextEffects.HighlightColor;
                string directionChoice = Console.ReadLine().Trim().ToUpper();
                Console.ResetColor();

                switch (directionChoice)
                {
                    case "U":
                    case "UP":
                        placementRequest.Direction = ShipDirection.Up;
                        validInput = true;
                        break;
                    case "D":
                    case "DOWN":
                        placementRequest.Direction = ShipDirection.Down;
                        validInput = true;
                        break;
                    case "L":
                    case "LEFT":
                        placementRequest.Direction = ShipDirection.Left;
                        validInput = true;
                        break;
                    case "R":
                    case "RIGHT":
                        placementRequest.Direction = ShipDirection.Right;
                        validInput = true;
                        break;
                    default:
                        Console.Clear();
                        DisplayScreenTitle(activePlayer.name + " Ship Placement");
                        activePlayer.board.DisplayAll();
                        Console.ForegroundColor = TextEffects.HighlightColor;
                        TextEffects.CenterAlignWriteLine(invalidInputMessage);
                        Console.ResetColor();
                        continue;
                }
            }
            Console.Clear();
            return placementRequest;
        }

        private Coordinate GetShipCoordFromUser(string prompt, string invalidInputMessage)
        {
            
            while (true)
            {
                TextEffects.CenterAlignWrite(prompt);
                Console.ForegroundColor = TextEffects.HighlightColor;
                string coordChoice = Console.ReadLine().Replace(" ", "").ToUpper();
                Console.ResetColor();

                //make sure trimmed input is only length 2 or 3
                if (coordChoice.Length <= 1 || coordChoice.Length >= 4)
                {
                    Console.Clear();
                    DisplayScreenTitle(activePlayer.name + " Ship Deployment");
                    activePlayer.board.DisplayAll();
                    Console.ForegroundColor = TextEffects.HighlightColor;
                    TextEffects.CenterAlignWriteLine(invalidInputMessage);
                    Console.ResetColor();
                    continue;
                }

                int colNum = Coordinate.XLetterToNum(coordChoice[0]);
                int rowNum = 0;
                //convert 2nd part of input to int
                if (!int.TryParse(coordChoice.Substring(1), out rowNum))
                {
                    Console.Clear();
                    DisplayScreenTitle(activePlayer.name + " Ship Deployment");
                    activePlayer.board.DisplayAll();
                    Console.ForegroundColor = TextEffects.HighlightColor;
                    TextEffects.CenterAlignWriteLine(invalidInputMessage);
                    Console.ResetColor();
                    continue;
                }

                //make sure the coord is in range
                if (colNum < 1 || colNum > 10 || rowNum < 1 || rowNum > 10)
                {
                    Console.Clear();
                    DisplayScreenTitle(activePlayer.name + " Ship Deployment");
                    activePlayer.board.DisplayAll();
                    Console.ForegroundColor = TextEffects.HighlightColor;
                    TextEffects.CenterAlignWriteLine("But captain, that's miles from the battle!");
                    Console.ResetColor();
                    Console.WriteLine();
                    continue;
                }

                return new Coordinate(colNum, rowNum);
            }
        }

        private Coordinate ValidateTargetCoord(string userInput)
        {
            string invalidInputMessage = "Sorry Captain, we couldn't understand you!";

            //make sure trimmed input is only length 2 or 3
            if (userInput.Length <= 1 || userInput.Length >= 4)
            {
                Console.Clear();
                DisplayScreenTitle(activePlayer.name + "'s Turn");
                opponent.board.DisplayHitsAndMisses(true);
                Console.ForegroundColor = TextEffects.HighlightColor;
                TextEffects.CenterAlignWriteLine(invalidInputMessage);
                Console.WriteLine();
                Console.ResetColor();
                return null;
            }
                     
            int colNum = Coordinate.XLetterToNum(userInput[0]);
            int rowNum = 0;
            //convert 2nd part of input to int
            if (!int.TryParse(userInput.Substring(1), out rowNum))
            {
                Console.Clear();
                DisplayScreenTitle(activePlayer.name + "'s Turn");
                opponent.board.DisplayHitsAndMisses(true);
                Console.ForegroundColor = TextEffects.HighlightColor;
                TextEffects.CenterAlignWriteLine(invalidInputMessage);
                Console.WriteLine();
                Console.ResetColor();
                return null;
            }

            //make sure the coord is in range
            if (colNum < 1 || colNum > 10 || rowNum < 1 || rowNum > 10)
            {
                Console.Clear();
                DisplayScreenTitle(activePlayer.name + "'s Turn");
                opponent.board.DisplayHitsAndMisses(true);
                Console.ForegroundColor = TextEffects.HighlightColor;
                TextEffects.CenterAlignWriteLine("But captain, that's miles from the battle!");
                Console.ResetColor();
                Console.WriteLine();
                return null;
            }
            Console.Clear();
            return new Coordinate(colNum, rowNum);
        }

        private void DisplayShipPlacementAndUnplacedShips(Player activePlayer, Dictionary<ShipType, int> unplacedShips)
        {
            DisplayScreenTitle(activePlayer.name + " Ship Deployment");
            activePlayer.board.DisplayAll();

            foreach (KeyValuePair<ShipType, int> entry in unplacedShips)
            {
                string shipTypeStr = entry.Key.ToString();

                //leading spaces for center alignment
                for (int i = 1; i <= 27; i++)
                    Console.Write(" ");

                if (shipTypeStr == "Carrier")
                {
                    Console.Write("(");
                    Console.ForegroundColor = TextEffects.HighlightColor;
                    Console.Write("A");
                    Console.ResetColor();
                    Console.WriteLine(")ircraft Carrier: length " + entry.Value);
                }
                else
                {
                    Console.Write("(");
                    Console.ForegroundColor = TextEffects.HighlightColor;
                    Console.Write(shipTypeStr[0]);
                    Console.ResetColor();
                    Console.WriteLine(")" + shipTypeStr.Substring(1) + ": length " + entry.Value);
                }
                    
            }

            Console.WriteLine();
        }

        private void DisplayScreenTitle(string title)
        {
            Console.ForegroundColor = TextEffects.HighlightColor;
            Console.WriteLine();
            TextEffects.CenterAlignWriteLine(title);
            Console.ResetColor();
        }

        private void AvertYourEyes(string message)
        {
            TextEffects.DisplayEyes();
            Console.ForegroundColor = TextEffects.HighlightColor;
            TextEffects.CenterAlignWriteLine("Ok " + opponent.name + ", avert your eyes!");
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();
            TextEffects.CenterAlignWrite(message);
            Console.ReadLine();
            Console.Clear();
        }

        private bool TakeTurn()
        {

            bool victory = FireShot();

            if (victory)
            {
                activePlayer.score++;
                return true;
            }               
            else
                return false;
        }

        private bool FireShot()
        {
            bool validShot = false;
            do
            {
                DisplayScreenTitle(activePlayer.name + "'s Turn");
                opponent.board.DisplayHitsAndMisses(true);
                
                Coordinate targetCoord = null;

                do
                {
                    Console.ForegroundColor = TextEffects.HighlightColor;
                    TextEffects.CenterAlignWrite("Enter a location to fire at: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    string userInput = Console.ReadLine().Replace(" ", "").ToUpper();
                    Console.ResetColor();

                    if (userInput == "M") //view own board, then return to fire a shot
                    {
                        Console.Clear();
                        DisplaySelfBoard(); //then the user presses enter to clear the screen
                        DisplayScreenTitle(activePlayer.name + "'s Turn");
                        opponent.board.DisplayHitsAndMisses(true);
                        continue;
                    }

                    targetCoord = ValidateTargetCoord(userInput);

                } while (targetCoord == null);

                Console.ResetColor();
                FireShotResponse shotResponse = opponent.board.FireShot(targetCoord);

                //display results of shot
                DisplayScreenTitle(activePlayer.name + "'s Turn");
                opponent.board.DisplayHitsAndMisses(false);
                switch (shotResponse.ShotStatus)
                {
                    case ShotStatus.Invalid:
                        TextEffects.CenterAlignWrite("Try that one more time.");
                        Thread.Sleep(800);
                        Console.Clear();
                        break;
                    case ShotStatus.Duplicate:
                        Console.ForegroundColor = TextEffects.HighlightColor;
                        TextEffects.CenterAlignWrite("We've already fired at that coordinate!");
                        Console.ResetColor();
                        Thread.Sleep(1200);
                        Console.Clear();
                        break;
                    case ShotStatus.Miss:
                        Console.ForegroundColor = Board.MissColor;
                        TextEffects.CenterAlignWriteLine("Miss.");
                        Console.WriteLine();
                        Console.ResetColor();
                        TextEffects.CenterAlignWrite("Press enter for " + opponent.name + "'s turn...");
                        Console.ReadLine();
                        Console.Clear();
                        validShot = true;
                        break;
                    case ShotStatus.Hit:
                        //Animate the hit message
                        Console.ForegroundColor = Board.HitColor;
                        int currentCursorTop = Console.CursorTop;
                        for (int i = 1; i <= 3; i++)
                        {
                            //display no hit message first
                            Thread.Sleep(150);
                            //now show the hit message
                            TextEffects.CenterAlignWrite("Hit!");
                            Thread.Sleep(150);
                            Console.SetCursorPosition(0, currentCursorTop);
                            //to erase the hit message
                            TextEffects.CenterAlignWrite("    ");
                            Console.SetCursorPosition(0, currentCursorTop);
                        }
                        TextEffects.CenterAlignWriteLine("Hit!");
                        Console.ResetColor();
                        Thread.Sleep(200);
                        Console.WriteLine();
                        TextEffects.CenterAlignWrite("Press enter for " + opponent.name + "'s turn...");
                        Console.ReadLine();
                        Console.Clear();
                        validShot = true;
                        break;
                    case ShotStatus.HitAndSunk:
                        Console.Clear();
                        TextEffects.SinkShipAnimation(shotResponse.ShipImpacted);
                        //display stuff at top of screen again
                        DisplayScreenTitle(activePlayer.name + "'s Turn");
                        opponent.board.DisplayHitsAndMisses(false);
                        Console.ForegroundColor = Board.HitColor;
                        TextEffects.CenterAlignWriteLine("Hit and sunk!");
                        Console.ResetColor();
                        Console.ForegroundColor = TextEffects.HighlightColor;
                        TextEffects.CenterAlignWriteLine("You sank the enemy's " + shotResponse.ShipImpacted + "!!");
                        Console.ResetColor();
                        Console.WriteLine();
                        TextEffects.CenterAlignWrite("Press enter for " + opponent.name + "'s turn...");
                        Console.ReadLine();
                        Console.Clear();
                        validShot = true;
                        break;
                    case ShotStatus.Victory:
                        Console.Clear();
                        return true;
                }
            } while (!validShot);
            //no victory, so
            return false;
        }

        private void DisplaySelfBoard()
        {
            DisplayScreenTitle(activePlayer.name + "'s Board");
            activePlayer.board.DisplayAll();
            Console.ForegroundColor = TextEffects.HighlightColor;
            TextEffects.CenterAlignWrite("Press enter to fire at " + opponent.name + "... ");
            Console.ResetColor();
            Console.ReadLine();
            Console.Clear();
        }

        private void SwitchActivePlayer()
        {
            Player tempPlayer = opponent;
            opponent = activePlayer;
            activePlayer = tempPlayer;
        }

        private bool PlayAgainPrompt()
        {
            TextEffects.AnimateBattleshipLogo(true, activePlayer.name);

            //display score
            TextEffects.CenterAlignWriteLine("SCORE");
            TextEffects.CenterAlignWriteLine(activePlayer.name + ": " + activePlayer.score);
            TextEffects.CenterAlignWriteLine(opponent.name + ": " + opponent.score);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(1500);
            //prompt to play again
            //write options for entering yes or no
            for (int i = 1; i <= 35; i++)
                Console.Write(" ");
            Console.Write("(");
            Console.ForegroundColor = TextEffects.HighlightColor;
            Console.Write("Y");
            Console.ResetColor();
            Console.WriteLine(")es");
            for (int i = 1; i <= 35; i++)
                Console.Write(" ");
            Console.Write("(");
            Console.ForegroundColor = TextEffects.HighlightColor;
            Console.Write("N");
            Console.ResetColor();
            Console.WriteLine(")o");
            Console.WriteLine();

            while (true)
            {
                Console.ForegroundColor = TextEffects.HighlightColor;
                TextEffects.CenterAlignWrite("Would you like to play again? ");
                //ask if they want to play again and return the result
                string userInput = Console.ReadLine().Trim().ToUpper();
                switch (userInput)
                {
                    case "Y":
                        Console.Clear();
                        return true;
                    case "N":
                        return false;
                    default:
                        int currentCursorTop = Console.CursorTop;
                        Console.SetCursorPosition(0, currentCursorTop-1);
                        Console.ResetColor();
                        TextEffects.CenterAlignWrite("Enter Y for yes or N for no.");
                        Console.Write("   "); //to erase their previous entry
                        Thread.Sleep(1300);
                        Console.SetCursorPosition(0, currentCursorTop-1);
                        break;
                }
           
            }
        }

    }
}
