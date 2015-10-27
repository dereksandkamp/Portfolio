using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlooringProgram.Data
{
    public static class ConfigurationSettings
    {
        private static string _mode;
        private static string _fileWriteFormat;

        public static string GetMode()
        {
            if (string.IsNullOrEmpty(_mode))
            {
                _mode = ConfigurationManager.AppSettings["Mode"];
            }

            return _mode;
        }

        public static string GetFileWriteFormat()
        {
            if (string.IsNullOrEmpty(_fileWriteFormat))
            {
                _fileWriteFormat = ConfigurationManager.AppSettings["FileWriteFormat"];
            }
            return _fileWriteFormat;
        }
    }
}
