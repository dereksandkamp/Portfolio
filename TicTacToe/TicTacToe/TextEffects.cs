using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public static class TextEffects
    {
        public static ConsoleColor XMarkColor = ConsoleColor.Blue;
        public static ConsoleColor OMarkColor = ConsoleColor.Green;
        public static ConsoleColor ErrorColor = ConsoleColor.Red;
        public static ConsoleColor NeutralColor = ConsoleColor.Yellow;
        public static string offsetString = "                    ";

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

        public static void DisplayLogo()
        {
            Console.Write(@"      _____ _    _____        _____          
     |_   _(_)  |_   _|      |_   _|         
       | |  _  ___| | __ _  ___| | ___   ___ 
       | | | |/ __| |/ _` |/ __| |/ _ \ / _ \
       | | | | (__| | (_| | (__| | (_) |  __/
       \_/ |_|\___\_/\__,_|\___\_/\___/ \___|");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

    }
}
