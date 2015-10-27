using System;
using System.Collections.Generic;
using System.Linq;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;


namespace BattleShip.BLL.GameLogic
{
    public class Board
    {
        public Dictionary<Coordinate, ShotHistory> ShotHistory;
        private Ship[] _ships;
        private int _currentShipIndex;
        public static System.ConsoleColor GridHeadingsColor { get; set; }
        public static System.ConsoleColor MissColor { get; set; }
        public static System.ConsoleColor HitColor { get; set; }
        public static System.ConsoleColor ShipColor { get; set; }

        public Board()
        {
            ShotHistory = new Dictionary<Coordinate, ShotHistory>();
            _ships = new Ship[5];
            _currentShipIndex = 0;
            GridHeadingsColor = ConsoleColor.Blue;
            MissColor = ConsoleColor.Yellow;
            HitColor = ConsoleColor.Red;
            ShipColor = ConsoleColor.Green;
        }

        public void DisplayHitsAndMisses(bool isExpectingAChoice)
        {
            Console.WriteLine();
            //for center alignment
            Console.Write("       ");
            //write the column headings
            for (int i = 1; i <= 10; i++)
            {
                Console.Write("     ");
                Console.ForegroundColor = GridHeadingsColor;
                Console.Write(Coordinate.XNumToLetter(i));
                Console.ResetColor();
            }
            Console.WriteLine();

            //write a blank row of pipes
            //for center alignment
            Console.Write("       ");
            //start board
            Console.Write("   ");
            for (int i = 1; i <= 9; i++)
            {
                Console.Write("     |");
            }
            Console.WriteLine();

            //write rows 1-9
            for (int i = 1; i <= 9; i++)
            {
                WriteHitsAndMissesRow(i);
                WriteHorizontalLine();
            }

            //write row 10, then a blank row of pipes
            WriteHitsAndMissesRow(10);
            //for center alignment
            Console.Write("       ");
            //start board
            Console.Write("   ");
            for (int i = 1; i <= 9; i++)
            {
                Console.Write("     |");
            }
            Console.WriteLine();
            Console.WriteLine();

            DisplaySunkenShips(false, isExpectingAChoice);
        }

        private void WriteHitsAndMissesRow(int rowNumber)
        {
            //for center alignment
            Console.Write("       ");
            //start board
            Console.ForegroundColor = GridHeadingsColor;
            Console.Write("{0,-3}", rowNumber);
            Console.ResetColor();
            
            //cycle through the columns
            for (int col = 1; col <= 10; col++)
            {
                Coordinate currentCoord = new Coordinate(col, rowNumber);
                if (ShotHistory.ContainsKey(currentCoord))
                {
                    switch (ShotHistory[currentCoord])
                    {
                        case Responses.ShotHistory.Miss:
                            Console.ForegroundColor = MissColor;
                            Console.Write("  M  ");
                            Console.ResetColor();
                            break;
                        case Responses.ShotHistory.Hit:
                            Console.ForegroundColor = HitColor;
                            Console.Write("  H  ");
                            Console.ResetColor();
                            break;
                    }
                }
                else //there's no shot history for this coord
                    Console.Write("     ");

                //I don't want a pipe for the right side of column 10
                if (col != 10)
                    Console.Write("|");                
            }
            Console.WriteLine();
        }

        private void WriteHorizontalLine()
        {
            //for center alignment
            Console.Write("       ");
            //start board
            Console.Write("   ");
            for (int i = 1; i <= 9; i++)
            {
                Console.Write("-----|");
            }
            Console.Write("-----");
            Console.WriteLine();
        }

        public void DisplayAll()
        {
            Console.WriteLine();
            //for center alignment
            Console.Write("       ");
            //write the column headings
            for (int i = 1; i <= 10; i++)
            {
                Console.Write("     ");
                Console.ForegroundColor = GridHeadingsColor;
                Console.Write(Coordinate.XNumToLetter(i));
                Console.ResetColor();
            }
            Console.WriteLine();

            //for center alignment
            Console.Write("       ");
            //write a blank row of pipes
            Console.Write("   ");
            for (int i = 1; i <= 9; i++)
            {
                Console.Write("     |");
            }
            Console.WriteLine();

            //write rows 1-9
            for (int i = 1; i <= 9; i++)
            {
                WriteDisplayAllRow(i);
                WriteHorizontalLine();
            }

            //write row 10, then a blank row of pipes
            WriteDisplayAllRow(10);
            //for center alignment
            Console.Write("       ");
            //start board
            Console.Write("   ");
            for (int i = 1; i <= 9; i++)
            {
                Console.Write("     |");
            }
            Console.WriteLine();
            Console.WriteLine();

            DisplaySunkenShips(true, false);
        }

        private void WriteDisplayAllRow(int rowNumber)
        {
            //for center alignment
            Console.Write("       ");
            //write row number
            Console.ForegroundColor = GridHeadingsColor;
            Console.Write("{0,-3}", rowNumber);
            Console.ResetColor();

            //cycle through and print the columns
            for (int col = 1; col <= 10; col++)
            {
                Coordinate currentCoord = new Coordinate(col, rowNumber);
                
                //first display the coordinate's shot history (that takes precedence over displaying ships)
                if (ShotHistory.ContainsKey(currentCoord))
                {
                    switch (ShotHistory[currentCoord])
                    {
                        case Responses.ShotHistory.Miss:
                            Console.ForegroundColor = MissColor;
                            Console.Write("  M  ");
                            Console.ResetColor();
                            break;
                        case Responses.ShotHistory.Hit:
                            Console.ForegroundColor = HitColor;
                            //display the letter of the ship that was hit, in the HitColor
                            bool foundCoord = false;
                            foreach (var ship in _ships)
                            {
                                //no need to keep checkings ships if it's found
                                if (foundCoord == true)
                                    break;
                                if (ship != null)
                                {
                                    foreach (var shipCoord in ship.BoardPositions)
                                    {
                                        if (shipCoord.Equals(currentCoord))
                                        {
                                            foundCoord = true;
                                            switch (ship.ShipType)
                                            {
                                                case ShipType.Carrier:
                                                    Console.Write("  AC ");
                                                    break;
                                                case ShipType.Battleship:
                                                    Console.Write("  B  ");
                                                    break;
                                                case ShipType.Submarine:
                                                    Console.Write("  S  ");
                                                    break;
                                                case ShipType.Cruiser:
                                                    Console.Write("  C  ");
                                                    break;
                                                case ShipType.Destroyer:
                                                    Console.Write("  D  ");
                                                    break;
                                            }
                                            //found coord, no need to keep searching ship coords
                                            break;
                                        }
                                    }
                                }
                            }
                            Console.ResetColor();
                            break;
                    }
                }
                else //there was no shot history for the coord so now we can display any ships there
                {
                    bool foundCoord = false;
                    foreach (var ship in _ships)
                    {
                        if (ship != null)
                        {
                            foreach (var shipCoord in ship.BoardPositions)
                            {
                                if (shipCoord.Equals(currentCoord))
                                {
                                    Console.ForegroundColor = ShipColor;
                                    switch(ship.ShipType)
                                    {
                                        case ShipType.Carrier:
                                            Console.Write("  AC ");
                                            break;
                                        case ShipType.Battleship:
                                            Console.Write("  B  ");
                                            break;
                                        case ShipType.Submarine:
                                            Console.Write("  S  ");
                                            break;
                                        case ShipType.Cruiser:
                                            Console.Write("  C  ");
                                            break;
                                        case ShipType.Destroyer:
                                            Console.Write("  D  ");
                                            break;
                                    }
                                    Console.ResetColor();
                                    foundCoord = true;
                                    break;
                                }
                            }
                        }
                        if (foundCoord)
                            break; //no need to keep looping through ships
                    }
                    if (!foundCoord) //there was no shot hisory or ship at the location
                        Console.Write("     ");
                }

                //I don't want a pipe for the right side of column 10
                if (col != 10)
                    Console.Write("|");
            }
            Console.WriteLine();
        }

        private void DisplaySunkenShips(bool isSelfBoard, bool isExpectingAChoice)
        {
            //make a list of sunken ships to display
            Dictionary<string, int> sunkenShips = new Dictionary<string, int>();
            foreach (var ship in _ships)
            {
                if (ship != null && ship.IsSunk)
                {
                    if (ship.ShipType == ShipType.Carrier)
                        sunkenShips.Add("Aircraft Carrier", 5);
                    else
                        sunkenShips.Add(ship.ShipType.ToString(), ship.BoardPositions.Length);
                }
            }

            //display the list of sunken ships, and contextually display the "Press M to view your board" message.
            if (sunkenShips.Count > 0)
            {
                if (!isSelfBoard && !isExpectingAChoice)
                    return;
         
                //Console.ForegroundColor = HitColor;
                Console.Write("       ");
                if (isSelfBoard)
                {
                    Console.WriteLine("SHIPS YOUR OPPONENT HAS SUNK");
                    //Console.ResetColor();
                }
                    
                else
                {
                    Console.Write("SHIPS YOU HAVE SUNK");
                    //Console.ResetColor();
                    if (isExpectingAChoice)
                    {
                        Console.Write("             ");
                        Console.Write("Press M to view your own board");
                    }
                    Console.WriteLine();
                    
                }

                int i = 1;
                foreach (KeyValuePair<string, int> ship in sunkenShips)
                {
                    if (i == 1 && isExpectingAChoice)
                    {
                        string sunkenShip = "       " + ship.Key + " (length " + ship.Value + ")";
                        Console.Write(sunkenShip);
                        //space it over the correct amount
                        for (int j = 1; j <= 57-sunkenShip.Length; j++)
                            Console.Write(" ");
                        Console.WriteLine("(in private)");
                    }
                    else
                    {
                        Console.WriteLine("       " + ship.Key + " (length " + ship.Value + ")");
                    }
                        
                    i++;
                }
                Console.WriteLine();
            }
            else if (!isSelfBoard && isExpectingAChoice) //there are no sunken ships and it is the opponent's board-display M message!
            {
                for (int i = 1; i <= 39; i++)
                    Console.Write(" ");
                Console.WriteLine("Press M to view your own board");
                for (int i = 1; i <= 57; i++)
                    Console.Write(" ");
                Console.WriteLine("(in private)");
                Console.WriteLine();
            }
        }

        public FireShotResponse FireShot(Coordinate coordinate)
        {
            var response = new FireShotResponse();

            // is this coordinate on the board?
            if (!IsValidCoordinate(coordinate))
            {
                response.ShotStatus = ShotStatus.Invalid;
                return response;
            }

            // did they already try this position?
            if (ShotHistory.ContainsKey(coordinate))
            {
                response.ShotStatus = ShotStatus.Duplicate;
                return response;
            }

            CheckShipsForHit(coordinate, response);
            CheckForVictory(response);

            return response;            
        }

        private void CheckForVictory(FireShotResponse response)
        {
            if (response.ShotStatus == ShotStatus.HitAndSunk)
            {
                // did they win?
                if (_ships.All(s => s.IsSunk))
                    response.ShotStatus = ShotStatus.Victory;
            }
        }

        private void CheckShipsForHit(Coordinate coordinate, FireShotResponse response)
        {
            response.ShotStatus = ShotStatus.Miss;

            foreach (var ship in _ships)
            {
                // no need to check sunk ships
                if (ship.IsSunk)
                    continue;

                ShotStatus status = ship.FireAtShip(coordinate);

                switch (status)
                {
                    case ShotStatus.HitAndSunk:
                        response.ShotStatus = ShotStatus.HitAndSunk;
                        response.ShipImpacted = ship.ShipName;
                        ShotHistory.Add(coordinate, Responses.ShotHistory.Hit);
                        break;
                    case ShotStatus.Hit:
                        response.ShotStatus = ShotStatus.Hit;
                        response.ShipImpacted = ship.ShipName;
                        ShotHistory.Add(coordinate, Responses.ShotHistory.Hit);
                        break;
                }

                // if they hit something, no need to continue looping
                if (status != ShotStatus.Miss)
                    break;
            }

            if (response.ShotStatus == ShotStatus.Miss)
            {
                ShotHistory.Add(coordinate, Responses.ShotHistory.Miss);
            }
        }

        private bool IsValidCoordinate(Coordinate coordinate)
        {
            return coordinate.XCoordinate >= 1 && coordinate.XCoordinate <= 10 &&
            coordinate.YCoordinate >= 1 && coordinate.YCoordinate <= 10;
        }

        public ShipPlacement PlaceShip(PlaceShipRequest request)
        {
            if (_currentShipIndex > 4)
                throw new Exception("You cannot add another ship, 5 is the limit!");

            if (!IsValidCoordinate(request.Coordinate))
                return ShipPlacement.NotEnoughSpace;

            Ship newShip = ShipCreator.CreateShip(request.ShipType);
            switch (request.Direction)
            {
                case ShipDirection.Down:
                    return PlaceShipDown(request.Coordinate, newShip);
                case ShipDirection.Up:
                    return PlaceShipUp(request.Coordinate, newShip);
                case ShipDirection.Left:
                    return PlaceShipLeft(request.Coordinate, newShip);
                default:
                    return PlaceShipRight(request.Coordinate, newShip);
            }
        }

        private ShipPlacement PlaceShipRight(Coordinate coordinate, Ship newShip)
        {
            // x coordinate gets bigger
            int positionIndex = 0;
            int maxX = coordinate.XCoordinate + newShip.BoardPositions.Length;

            for (int i = coordinate.XCoordinate; i < maxX; i++)
            {
                var currentCoordinate = new Coordinate(i, coordinate.YCoordinate);

                if (!IsValidCoordinate(currentCoordinate))
                    return ShipPlacement.NotEnoughSpace;

                if (OverlapsAnotherShip(currentCoordinate))
                    return ShipPlacement.Overlap;

                newShip.BoardPositions[positionIndex] = currentCoordinate;
                positionIndex++;
            }

            AddShipToBoard(newShip);
            return ShipPlacement.Ok;
        }

        private ShipPlacement PlaceShipLeft(Coordinate coordinate, Ship newShip)
        {
            // x coordinate gets smaller
            int positionIndex = 0;
            int minX = coordinate.XCoordinate - newShip.BoardPositions.Length;

            for (int i = coordinate.XCoordinate; i > minX; i--)
            {
                var currentCoordinate = new Coordinate(i, coordinate.YCoordinate);

                if (!IsValidCoordinate(currentCoordinate))
                    return ShipPlacement.NotEnoughSpace;

                if (OverlapsAnotherShip(currentCoordinate))
                    return ShipPlacement.Overlap;

                newShip.BoardPositions[positionIndex] = currentCoordinate;
                positionIndex++;
            }

            AddShipToBoard(newShip);
            return ShipPlacement.Ok;
        }

        private ShipPlacement PlaceShipUp(Coordinate coordinate, Ship newShip)
        {
            // y coordinate gets smaller
            int positionIndex = 0;
            int minY = coordinate.YCoordinate - newShip.BoardPositions.Length;

            for (int i = coordinate.YCoordinate; i > minY; i--)
            {
                var currentCoordinate = new Coordinate(coordinate.XCoordinate, i);

                if (!IsValidCoordinate(currentCoordinate))
                    return ShipPlacement.NotEnoughSpace;

                if (OverlapsAnotherShip(currentCoordinate))
                    return ShipPlacement.Overlap;

                newShip.BoardPositions[positionIndex] = currentCoordinate; 
                positionIndex++;
            }

            AddShipToBoard(newShip);
            return ShipPlacement.Ok;
        }

        private ShipPlacement PlaceShipDown(Coordinate coordinate, Ship newShip)
        {
            // y coordinate gets bigger
            int positionIndex = 0;
            int maxY = coordinate.YCoordinate + newShip.BoardPositions.Length;
            
            for (int i = coordinate.YCoordinate; i < maxY; i++)
            {
                var currentCoordinate = new Coordinate(coordinate.XCoordinate, i);
                if (!IsValidCoordinate(currentCoordinate))
                    return ShipPlacement.NotEnoughSpace;

                if (OverlapsAnotherShip(currentCoordinate))
                    return ShipPlacement.Overlap;

                newShip.BoardPositions[positionIndex] = currentCoordinate;
                positionIndex++;
            }

            AddShipToBoard(newShip);
            return ShipPlacement.Ok;
        }

        private void AddShipToBoard(Ship newShip)
        {
            _ships[_currentShipIndex] = newShip;
            _currentShipIndex++;
        }

        private bool OverlapsAnotherShip(Coordinate coordinate)
        {
            foreach (var ship in _ships)
            {
                if (ship != null)
                {
                    if (ship.BoardPositions.Contains(coordinate))
                        return true;
                }
            }

            return false;
        }
    }
}
