using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlooringProgram.Data
{
    public class Logger
    {
        private string _filePath = ".\\log.txt";

        public void Log(string message)
        {
            using (StreamWriter writer = new StreamWriter(_filePath, true))
            {
                writer.WriteLine("{0}  :   {1}", DateTime.Now.ToString("G"), message);
            }
        }
    }
}
