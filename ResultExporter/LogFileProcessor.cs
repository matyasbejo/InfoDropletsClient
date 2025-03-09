using InfoDroplets.ResultExporter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.ResultExporter
{
    internal static class LogFileProcessor
    {
        internal static List<LogEntry> GetFileData(string path)
        {
            if(!File.Exists(path)) throw new FileNotFoundException(path);

            var FileLines = File.ReadAllLines(path);

            if (FileLines[0] != "Developed by Soprobotics.") throw new Exception("The selected file is not a log entry");

            int IndexOfSeparator = Array.IndexOf(FileLines, "________;________;________;________;________;________");

            var fileData = FileLines.Skip(IndexOfSeparator+1).ToList();

            List<LogEntry> result = new List<LogEntry>();
            foreach (string row in fileData)
            {
                var rowData = row.Split(';');
                if (rowData.Count() != 6) throw new Exception($"The line {row} is not a valid LogEntry.");
                
                var Lat = double.Parse(rowData[3]);
                var Long = double.Parse(rowData[4]);
                var El = double.Parse(rowData[5]);

                result.Add(new LogEntry(Lat, Long, El));
            }
            return result;
        }
    }
}
