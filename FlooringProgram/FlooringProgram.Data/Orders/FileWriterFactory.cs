using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlooringProgram.Models.Interfaces;

namespace FlooringProgram.Data.Orders
{
    public class FileWriterFactory
    {
        public static IFileHandler GetFileWriter(string filePathStart)
        {
            switch (ConfigurationSettings.GetFileWriteFormat().ToUpper())
            {
                case "CSV":
                    return new CsvFileHandler(filePathStart);
                case "XML":
                    return new XmlFileHandler(filePathStart);
                case "JSON":
                    return new JsonFileHandler(filePathStart);
                default:
                    throw new Exception("Unrecognized file write format.");
            }
        }

    }
}
