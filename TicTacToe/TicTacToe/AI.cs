using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class AI
    {
        public static AIBoard CurrentBoard;
        public Mark AIMark;
        public Mark OpponentMark;
        public static int TurnsCounter = 0;
        private static Random rng = new Random();
        private static readonly int[,] PossibleWins = new int[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }, { 1, 4, 7 }, { 2, 5, 8 }, { 3, 6, 9 }, { 1, 5, 9 }, { 3, 5, 7 }
        };

        public AI(Mark aiMark, Mark opponentMark)
        {
            this.AIMark = aiMark;
            this.OpponentMark = opponentMark;
        }

        public int TakeTurn(string[] inBoard)
        {
            //refresh the current board
            CurrentBoard = new AIBoard(inBoard);
            int positionChoice = 0;

            if (GameManager.DifficultyLevel == 3)
            {
                //process specific turns
                if (TurnsCounter == 0)
                    return ProcessFirstTurn();
                if (TurnsCounter == 1)
                    return ProcessSecondTurn();
                if (TurnsCounter == 2)
                    return ProcessThirdTurn();
                if (TurnsCounter == 3 && GameManager.DifficultyLevel == 3) //special situations for unbeatable difficulty
                {
                    if (DiagonalSandwitchSituation(out positionChoice))
                        return positionChoice;
                    if (HorzOrVertSandwitchSituation(out positionChoice))
                        return positionChoice;
                    if (DiagonalSwordSituation(out positionChoice))
                        return positionChoice;
                }
            }
           
                    
            //check if there is anywhere that we can win this turn and do it
            if (CanWin(AIMark, CurrentBoard, out positionChoice))
                return positionChoice;

            if (GameManager.DifficultyLevel >= 2)
            {
                //check if we need to block the player from winning and do it
                if (CanWin(OpponentMark, CurrentBoard, out positionChoice))
                    return positionChoice;
            }
            

            if (GameManager.DifficultyLevel == 3) // lets have easy, normal, and unbeatable.
            {
                //check if we could set ourself up for a double win and do it
                if (TurnsCounter <= 7 && CanSetupDoubleWin(AIMark, out positionChoice))
                    return positionChoice;
            }

            if (GameManager.DifficultyLevel == 3)
            {
                //check if we need to block them from setting up a double win and do it
                if (TurnsCounter <= 7 && CanSetupDoubleWin(OpponentMark, out positionChoice))
                    return positionChoice;
            }
            
            
            //check if we could set ourself up for a single win and do it 
            if (TurnsCounter <= 7 && CanSetupSingleWin(AIMark, out positionChoice))
                    return positionChoice;

            //I think, at this point, we can pick a random open spot //
            int random = rng.Next(0, CurrentBoard.UnmarkedPositions.Count);
            positionChoice = CurrentBoard.UnmarkedPositions[random];

            return positionChoice;
        }

        private int ProcessThirdTurn()
        {
            //for hard difficulty (because it relies on the fact that the AI started in the corner)
            //first, loop through the corners and find the one we marked:
            int markedPosition = 0;
            int i = 1;
            while (true)
            {
                if (CurrentBoard.Positions[i].Mark == AIMark)
                {
                    markedPosition = i;
                    break;
                }

                //increment i to the values of the corners (why loop through everything when we don't need to?
                switch (i)
                {
                    case 1:
                        i = 3;
                        continue;
                    case 3:
                        i = 7;
                        continue;
                    case 7:
                        i = 9;
                        continue;
                    case 9:
                        return 0; //this should never happen (because the AI will have marked a corner)
                    default:
                        return 0; //this should never happen
                }
            }
            //ok, so now we know the corner that we marked in the int markedPosition
            //now we should figure out if the user marked any adjacent corners
            //also, we should figure out if the user marked the immediately adjacent edges
            //then we can respond accordingly
            //so first, lets find and store all those positions:
            int adjacentEdge1;
            int adjacentCorner1;
            int adjacentEdge2;
            int adjacentCorner2;
            int oppositeCorner;

            switch (markedPosition)
            {
                case 1:
                    adjacentEdge1 = 2;
                    adjacentCorner1 = 3;
                    adjacentEdge2 = 4;
                    adjacentCorner2 = 7;
                    oppositeCorner = 9;
                    break;
                case 3:
                    adjacentEdge1 = 2;
                    adjacentCorner1 = 1;
                    adjacentEdge2 = 6;
                    adjacentCorner2 = 9;
                    oppositeCorner = 7;
                    break;
                case 7:
                    adjacentEdge1 = 4;
                    adjacentCorner1 = 1;
                    adjacentEdge2 = 8;
                    adjacentCorner2 = 9;
                    oppositeCorner = 3;
                    break;
                case 9:
                    adjacentEdge1 = 6;
                    adjacentCorner1 = 3;
                    adjacentEdge2 = 8;
                    adjacentCorner2 = 7;
                    oppositeCorner = 1;
                    break;
                default:
                    return 0; //this should never happen
            }

            //here's the decision: 
            if (CurrentBoard.Positions[5].Mark == OpponentMark) //the user went in the center
            {
                //one more chance to trick them (sandwitchdiagonal situation)
                return oppositeCorner;
            }
            else //OK. If AI started in corner, and player did not choose center, we can't lose:
            {
                //if the player has marked one adjacent edge, we should mark the corner that is away from it
                if (CurrentBoard.Positions[adjacentEdge1].Mark == OpponentMark)
                {
                    return adjacentCorner2;
                }
                if (CurrentBoard.Positions[adjacentEdge2].Mark == OpponentMark)
                {
                    return adjacentCorner1;
                }
                
                //if they go in one of the adjacent corners, we should go in the corner opposite from ours
                if (CurrentBoard.Positions[adjacentCorner1].Mark == OpponentMark ||
                    CurrentBoard.Positions[adjacentCorner2].Mark == OpponentMark)
                {
                    return oppositeCorner;
                }
                
                //if they haven't marked either adjacent edge or corner, we can choose a random adjacent corner to mark
                //this will also work if they marked the opposite corner from ours
                int randomChoice = rng.Next(1, 3);
                return randomChoice == 1 ? adjacentCorner1 : adjacentCorner2;
            }
        }

        private int ProcessSecondTurn()
        {
            //where did they go? Let's find out.
            int playerMove = 0;
            for (int i = 1; i <= 9; i++)
            {
                if (CurrentBoard.Positions[i].Mark == OpponentMark)
                    playerMove = i;
            }

            //let's check the most dangerous situation. Did they go in a corner? 
            //Then we need to go in the center. The check for the diagonal sandwitch situation will do the rest.
            if (playerMove == 1 || playerMove == 3 || playerMove == 7 || playerMove == 9)
            {
                return 5;
            }
            else if (playerMove == 5) // if they went in the center, we need to choose a corner
            {
                int random = rng.Next(1, 5);
                switch (random)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 3;
                    case 3:
                        return 7;
                    case 4:
                        return 9;
                    default:
                        return 0; //this should never happen
                }
            }
            else //they went on an edge, so we should go in the middle. the horzorvertsandwitch will do the rest if necessary
            {
                return 5;
            }
        }

        private int ProcessFirstTurn()
        {
            //play in a corner. This would be for the difficult setting.
            int randomNum = rng.Next(1, 5);
            switch (randomNum)
            {
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 7;
                case 4:
                    return 9;
                default:
                    return 0; //this will never happen, but all code paths must return a value
            }
        }

        private bool DiagonalSandwitchSituation(out int positionChoice)
        {
            positionChoice = 0;

            //if sandwitch situation
            if ((CurrentBoard.Positions[1].Mark == OpponentMark && CurrentBoard.Positions[5].Mark == AIMark && CurrentBoard.Positions[9].Mark == OpponentMark) ||
                (CurrentBoard.Positions[3].Mark == OpponentMark && CurrentBoard.Positions[5].Mark == AIMark && CurrentBoard.Positions[7].Mark == OpponentMark))
            {
                //we have to play an edge. pick a random edge
                int random = rng.Next(1, 5);
                positionChoice = random*2; //edges are 2,4,6, and 8
                return true;
            }
            return false;
        }

        private bool HorzOrVertSandwitchSituation(out int positionChoice)
        {
            positionChoice = 0;

            //if we encounter this situation, we can be aggressive and go in a corner. This will give us a better chance of winning.
            if ((CurrentBoard.Positions[4].Mark == OpponentMark && CurrentBoard.Positions[5].Mark == AIMark && CurrentBoard.Positions[6].Mark == OpponentMark) ||
                (CurrentBoard.Positions[2].Mark == OpponentMark && CurrentBoard.Positions[5].Mark == AIMark && CurrentBoard.Positions[8].Mark == OpponentMark))
            {
                //we should go in a corner.
                int random = rng.Next(1, 5);
                switch (random)
                {
                    case 1:
                        positionChoice = 1;
                        return true;
                    case 2:
                        positionChoice = 3;
                        return true;
                    case 3:
                        positionChoice = 7;
                        return true;
                    case 4:
                        positionChoice = 9;
                        return true;
                    default:
                        return false; //this should never happen
                }
            }
            return false;
        }

        private bool DiagonalSwordSituation(out int positionChoice)
        {
            positionChoice = 0;

            if ((CurrentBoard.Positions[1].Mark == AIMark && CurrentBoard.Positions[5].Mark == OpponentMark && CurrentBoard.Positions[9].Mark == OpponentMark) ||
                CurrentBoard.Positions[1].Mark == OpponentMark && CurrentBoard.Positions[5].Mark == OpponentMark && CurrentBoard.Positions[9].Mark == AIMark ||
                CurrentBoard.Positions[3].Mark == AIMark && CurrentBoard.Positions[5].Mark == OpponentMark && CurrentBoard.Positions[7].Mark == OpponentMark ||
                CurrentBoard.Positions[3].Mark == OpponentMark && CurrentBoard.Positions[5].Mark == OpponentMark && CurrentBoard.Positions[7].Mark == AIMark)
            {
                //we've got a diagonal sword situation. We need to go in one of the open corners.
                //first, let's find the open corners:
                int[] corners = new int[] {1, 3, 5, 7};
                List<int> openCorners = new List<int>();
                foreach (int corner in corners)
                {
                    if (CurrentBoard.Positions[corner].Mark == Mark.Unmarked)
                        openCorners.Add(corner);
                }

                //now let's pick a random open corner
                int random = rng.Next(0, 2);
                positionChoice = openCorners[random];
                return true;
            }
            return false;
        }

        private bool CanSetupSingleWin(Mark mark, out int positionChoice)
        {
            AIBoard simulatedBoard = null;
            List<int> singleWinSetupMoves = new List<int>();

            //mark each open spot on the board and check to see if it sets up a single win. Then reset the board and try again.
            foreach (int positionToTry in CurrentBoard.UnmarkedPositions)
            {
                //refresh simulated board
                simulatedBoard = CurrentBoard.Clone();
                //mark the test position
                simulatedBoard.Positions[positionToTry].Mark = mark;

                List<int> simulatedWinPositions = CheckBoardForNearWins(simulatedBoard, mark);
                //if this list count is 1, this simulation set up a single win.
                //if this list count is 2, this simulation set up a double win.
                if (simulatedWinPositions.Count == 1)
                    singleWinSetupMoves.Add(positionToTry);
            }

            //now let's return the single win setup move(s). If there are many single win setup moves, let's pick a random one.
            if (singleWinSetupMoves.Count > 0)
            {
                int random = rng.Next(0, singleWinSetupMoves.Count);
                positionChoice = singleWinSetupMoves[random];
                return true;
            }

            //else no single win setup moves were found.
            positionChoice = 0;
            return false;
        }

        private bool CanSetupDoubleWin(Mark mark, out int positionChoice)
        {
            AIBoard simulatedBoard = null;
            List<int> doubleWinSetupMoves = new List<int>();

            //mark each open spot on the board and check to see if it sets up a double win. Then reset the board and try again.
            foreach (int positionToTry in CurrentBoard.UnmarkedPositions)
            {
                //refresh simulated board
                simulatedBoard = CurrentBoard.Clone();
                //mark the test position
                simulatedBoard.Positions[positionToTry].Mark = mark;

                List<int> simulatedWinPositions = CheckBoardForNearWins(simulatedBoard, mark);
                //if this list count is 1, this simulation set up a single win.
                //if this list count is 2, this simulation set up a double win.
                if (simulatedWinPositions.Count >= 2)
                    doubleWinSetupMoves.Add(positionToTry);                   
            }
    
            //now let's return the double win setup move(s). If there are two double win setup moves, let's pick a random one.
            if (doubleWinSetupMoves.Count > 0)
            {
                int random = rng.Next(0, doubleWinSetupMoves.Count);
                positionChoice = doubleWinSetupMoves[random];
                return true;
            }

            //else no double win setup moves were found.
            positionChoice = 0;
            return false;
        }

        private bool CanWin(Mark mark, AIBoard board, out int positionChoice )
        {
            List<int> winPositions = CheckBoardForNearWins(board, mark);
            switch (winPositions.Count)
            {
                case 0:
                    positionChoice = 0;
                    return false;
                case 1:
                    positionChoice = winPositions[0];
                    return true;
                case 2:
                case 3:
                    int random = rng.Next(0, winPositions.Count);
                    positionChoice = winPositions[random];
                    return true;
                default: //this will never happen
                    positionChoice = 0;
                    return false;
            }
        }

        private List<int> CheckBoardForNearWins(AIBoard board, Mark markToCheck)
        {
            List<int> nearWinLocs = new List<int>();
            Position nearWinLoc = null;

            /*loop through the array of possible wins and check those locations on the board to see if they are near wins (2 out of 3). 
            If they are, it returns the location of the unmarked loc in the triplet. If not, it returns null.*/
            int upperBound = PossibleWins.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                nearWinLoc = CheckTripletForNearWin(board.Positions[PossibleWins[i, 0]], board.Positions[PossibleWins[i, 1]], board.Positions[PossibleWins[i, 2]], markToCheck);
                if (nearWinLoc != null)
                {
                    nearWinLocs.Add(nearWinLoc.PositionNum);
                }
            }

            return nearWinLocs;
        }

        private Position CheckTripletForNearWin(Position loc1, Position loc2, Position loc3, Mark markToCheck)
        {
            if (loc1.Mark == Mark.Unmarked && loc2.Mark == markToCheck && loc3.Mark == markToCheck)
                return new Position() {Mark = loc1.Mark, PositionNum = loc1.PositionNum};

            if (loc1.Mark == markToCheck && loc2.Mark == Mark.Unmarked && loc3.Mark == markToCheck)
                return new Position() { Mark = loc2.Mark, PositionNum = loc2.PositionNum };

            if (loc1.Mark == markToCheck && loc2.Mark == markToCheck && loc3.Mark == Mark.Unmarked)
                return new Position() { Mark = loc3.Mark, PositionNum = loc3.PositionNum };

            return null;
        }
    }
}
