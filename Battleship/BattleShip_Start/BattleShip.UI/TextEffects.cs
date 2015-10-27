using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    public static class TextEffects
    {
        public static System.ConsoleColor HighlightColor { get; set; }
        public static System.ConsoleColor BattleshipLogoColor { get; set; }

        public static void DisplayBattleshipLogo()
        {
            Console.ForegroundColor = BattleshipLogoColor;
            Console.WriteLine();
            Console.WriteLine();
            CenterAlignWriteLine(@"______  ___ _____ _____ _      _____ _____ _   _ ___________ ");
            CenterAlignWriteLine(@"| ___ \/ _ \_   _|_   _| |    |  ___/  ___| | | |_   _| ___ \");
            CenterAlignWriteLine(@"| |_/ / /_\ \| |   | | | |    | |__ \ `--.| |_| | | | | |_/ /");
            CenterAlignWriteLine(@"| ___ \  _  || |   | | | |    |  __| `--. \  _  | | | |  __/ ");
            CenterAlignWriteLine(@"| |_/ / | | || |   | | | |____| |___/\__/ / | | |_| |_| |    ");
            CenterAlignWriteLine(@"\____/\_| |_/\_/   \_/ \_____/\____/\____/\_| |_/\___/\_|    ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void AnimateBattleshipLogo(bool isEndOfGame, string victorName = "")
        {
            string subtitle;
            if (isEndOfGame)
                subtitle = victorName + " wins!!!!";
            else
                subtitle = "Prepare for battle!!";
            string[] logoArray = new string[6] { @"______  ___ _____ _____ _      _____ _____ _   _ ___________ ", @"| ___ \/ _ \_   _|_   _| |    |  ___/  ___| | | |_   _| ___ \", @"| |_/ / /_\ \| |   | | | |    | |__ \ `--.| |_| | | | | |_/ /", @"| ___ \  _  || |   | | | |    |  __| `--. \  _  | | | |  __/ ", @"| |_/ / | | || |   | | | |____| |___/\__/ / | | |_| |_| |    ", @"\____/\_| |_/\_/   \_/ \_____/\____/\____/\_| |_/\___/\_|    " };
            Console.ForegroundColor = BattleshipLogoColor; 
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            int leftStartPosition = 73;
            int leftEndPosition = 7;
            int difference = leftStartPosition - 1;
            int substringLength = 0;
            for (int i = leftStartPosition; i >= leftEndPosition; i--)
            {
                //print a substring of each line of the logo
                for (int j = 0; j < logoArray.Length; j++)
                {
                    if (substringLength < logoArray[0].Length)
                    {
                        substringLength = i - difference;
                    }
                    for (int k = 0; k < i; k++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write(logoArray[j].Substring(0, substringLength) + "\n");
                }

                difference -= 2;

                Thread.Sleep(14);
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine();
            }

            //Now flash the welcome message
            Console.Clear();
            DisplayBattleshipLogo();
            int currentCursorTop = Console.CursorTop;

            for (int i = 1; i <= 3; i++)
            {
                //write over the message
                Console.SetCursorPosition(0, currentCursorTop);
                TextEffects.CenterAlignWrite("                          ");
                Thread.Sleep(200);
                Console.SetCursorPosition(0, currentCursorTop);
                //write message
                Console.ForegroundColor = HighlightColor;
                CenterAlignWriteLine(subtitle);
                Thread.Sleep(200);
            }
            Console.ResetColor();

            if (isEndOfGame)
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
                CenterAlignWrite("Press enter to continue... ");
                Console.ReadLine();
                Console.Clear();
                DisplayBattleshipLogo();
            }
        }

        public static void DisplayEyes()
        {
            Console.ForegroundColor = HighlightColor;
            CenterAlignWriteLine(@"  .---.       .---.  ");
            CenterAlignWriteLine(@"-'()   '.  _.'()   '-");
            CenterAlignWriteLine(@"  `----'     '----'  ");

            Console.ResetColor();
            Console.WriteLine();
        }

        public static void PrepareToPlayDisplay(string playerName)
        { 
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = BattleshipLogoColor;
            //nudge him over a little
            string offset = "  ";
            Console.Write(offset);
            CenterAlignWriteLine(@"          ____             ");
            Console.Write(offset);
            CenterAlignWriteLine(@"          \__/         # ##");
            Console.Write(offset);
            CenterAlignWriteLine(@"         `(  `^=_ p _###_  ");
            Console.Write(offset);
            CenterAlignWriteLine(@"          c   /  )  |   /  ");
            Console.Write(offset);
            CenterAlignWriteLine(@"   _____- //^---~  _c  3   ");
            Console.Write(offset);
            CenterAlignWriteLine(@" /  ----^\ /^_\   / --,-   ");
            Console.Write(offset);
            CenterAlignWriteLine(@"(   |  |  O_| \\_/  ,/     ");
            Console.Write(offset);
            CenterAlignWriteLine(@"|   |  | / \|  `-- /       ");
            Console.Write(offset);
            CenterAlignWriteLine(@"(((G   |-----|             ");
            Console.Write(offset);
            CenterAlignWriteLine(@"      //-----\\            ");
            Console.Write(offset);
            CenterAlignWriteLine(@"     //       \\           ");
            Console.Write(offset);
            CenterAlignWriteLine(@"   /   |     |  ^|         ");
            Console.Write(offset);
            CenterAlignWriteLine(@"   |   |     |   |         ");
            Console.Write(offset);
            CenterAlignWriteLine(@"   |____|    |____|        ");
            Console.Write(offset);
            CenterAlignWriteLine(@"  /______)   (_____\       ");
            Console.WriteLine();
            Console.WriteLine();

            Console.ForegroundColor = HighlightColor;
            CenterAlignWriteLine("All hands on deck!!");
            Console.ResetColor();
            Console.WriteLine();
            CenterAlignWrite("Press enter for " + playerName + "'s turn...");
            Console.ReadLine();
            Console.Clear();
        }

        public static void CenterAlignWriteLine(string str)
        {
            int center = Console.WindowWidth / 2;
            Console.WriteLine("{0," + (center + str.Length / 2) + "}", str);
        }

        public static void CenterAlignWrite(string str)
        {
            int center = Console.WindowWidth / 2;
            Console.Write("{0," + (center + str.Length / 2) + "}", str);
        }

        public static void SinkShipAnimation(string shipType)
        {


            string[] shipAsciiArr = null;

            //use the correct ship ascii art
            switch (shipType)
            {
                case "Battleship":
                    shipAsciiArr = new string[13] { @"                                |__                                      ", @"                                |\/                                      ", @"                                ---                                      ", @"                                / | [                                    ", @"                         !      | |||                                    ", @"                       _/|     _/|-++'                                   ", @"                   +  +--|    |--|--|_ |-                                ", @"                { /|__|  |/\__|  |--- |||__/                             ", @"               +---------------___[}-_===_.'____                 /\      ", @"           ____`-' ||___-{]_| _[}-  |     |_[___\==--            \/   _  ", @" _____--==/___]_|__|_____________________________[___\==--____,------' .7", @"|                                                                      / ", @" \____________________________________________________________________|  " };
                    break;
                case "Carrier":
                    shipAsciiArr = new string[9] { @"                  |                                          ", @"                 -+-                                         ", @"               ---#---                                       ", @"               __|_|__            __                         ", @"               \_____/           ||\________                 ", @" __   __   __  \_____/            ^---------^                ", @"||\__||\__||\__|___  | '-O-`                                 ", @"-^---------^--^----^___.-------------.___.--------.___.------", @"\___________________________________________________________/" };
                    break;
                case "Cruiser":
                    shipAsciiArr = new string[8] { @"                                   # #  ( )                       ", @"                               ___#_#___|__                       ", @"                           _  |____________|  _                   ", @"                    _=====| | |            | | |==== _            ", @"              =====| |.---------------------------. | |====       ", @"<--------------------'   .  .  .  .  .  .  .  .   '--------------/", @"  \                                                             / ", @"   \___________________________________________________________/  " };
                    break;
                case "Destroyer":
                    shipAsciiArr = new string[6] { @"     (`-,-,      ", @"     ('(_,( )    ", @"      _   `_'    ", @"   __|_|__|_|_   ", @" _|___________|__", @"|o o o o o o o o/" };
                    break;
                case "Submarine":
                    shipAsciiArr = new string[7] { @"                          |`-:_                         ", @" ,----....____            |    `+.                      ", @"(             ````----....|___   |                      ", @" \     _                      ````----....____          ", @"  \    _)                                     ```---.._ ", @"   \                                                   \", @"    \                                                  |" };
                    break;
            }

            //animate the ship entering the screen from the right
            int leftStartPosition = 73;
            int leftEndPosition = (75 - shipAsciiArr[0].Length) / 2;
            int difference = leftStartPosition - 1;
            int substringLength = 0;
            for (int i = leftStartPosition; i >= leftEndPosition; i--)
            {
                //space it down a little
                Console.WriteLine();
                Console.WriteLine();

                //print a substring of each line of the ship
                Console.ForegroundColor = ConsoleColor.Yellow;
                for (int j = 0; j < shipAsciiArr.Length; j++)
                {
                    if (substringLength < shipAsciiArr[0].Length)
                    {
                        substringLength = i - difference;
                    }

                    for (int k = 0; k < i; k++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write(shipAsciiArr[j].Substring(0, substringLength) + "\n");
                }

                //print the water
                Console.ForegroundColor = ConsoleColor.Blue;
                for (int l = 1; l <= 37; l++)
                {
                    Console.Write("~^");
                }
                Console.Write("~");
                Console.ResetColor();

                //prepare for next frame
                difference -= 2;
                Thread.Sleep(17);
                Console.Clear();
            }

            //display the whole ship for a moment before it sinks
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (string line in shipAsciiArr)
            {
                //to center the ship5
                for (int i = 0; i < leftEndPosition; i++)
                    Console.Write(" ");
                Console.WriteLine(line);
            }
            //water
            Console.ForegroundColor = ConsoleColor.Blue;
            for (int l = 1; l <= 37; l++)
            {
                Console.Write("~^");
            }
            Console.Write("~");
            Console.ResetColor();

            Thread.Sleep(700);
            Console.Clear();

            //Now. . . sink it!
            for (int i = 1; i <= shipAsciiArr.Length; i++)
            {
                //space it down
                Console.WriteLine();
                Console.WriteLine();
                //space(s) at top
                for (int j = 1; j <= i; j++)
                    Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Red;
                //display a decreasing number of rows of the ship
                for (int k = 0; k < shipAsciiArr.Length - i; k++)
                {
                    //to center the ship
                    for (int l = 0; l < leftEndPosition; l++)
                        Console.Write(" ");
                    Console.WriteLine(shipAsciiArr[k]);
                }
                    

                //water
                Console.ForegroundColor = ConsoleColor.Blue;
                for (int l = 1; l <= 37; l++)
                {
                    Console.Write("~^");
                }
                Console.Write("~");
                Console.ResetColor();

                //pause
                Thread.Sleep(200);
                Console.Clear();
            }

            //pause on the open water for a bit, then end animation
            Console.WriteLine();
            Console.WriteLine();
            for (int i = 1; i <= shipAsciiArr.Length; i++)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            for (int i = 1; i <= 37; i++)
            {
                Console.Write("~^");
            }
            Console.Write("~");
            Console.ResetColor();

            Thread.Sleep(500);
            Console.Clear();
        }
    }
}
