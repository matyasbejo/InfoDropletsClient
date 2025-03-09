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
        static string[] GetDataRowsFromFile(string path)
        {
            if(!File.Exists(path)) throw new FileNotFoundException(path);

            var FileLines = File.ReadAllLines(path);

            if (FileLines[0] != "Developed by Soprobotics.") throw new Exception("The selected file is not a log entry");

            int IndexOfSeparator = Array.IndexOf(FileLines, "________;________;________;________;________;________");

            string[] fileData = FileLines.Skip(IndexOfSeparator+1).ToArray();
            return fileData;
        }

        static List<LogEntry> ProcessFile(string path)
        {
            var fileData = GetDataRowsFromFile(path);

            List<LogEntry> convertedValues = new List<LogEntry>();
            foreach (string row in fileData)
            {
                var rowData = row.Split(';');
                if (rowData.Count() != 6) throw new Exception($"The line {row} is not a valid LogEntry.");

                var Lat = double.Parse(rowData[3]);
                var Long = double.Parse(rowData[4]);
                var El = double.Parse(rowData[5]);

                convertedValues.Add(new LogEntry(Lat, Long, El));
            }

            List<LogEntry> result = convertedValues.Where(le => le.Latitude != 0 && le.Longitude != 0).Distinct().ToList();
            return result;
        }

        static string[] FilterABFiles(ref string[] paths)
        {
            var aFiles = paths.Where(p => p.Replace(".txt", "").Last() == 'a').ToArray();
            var bFiles = paths.Where(p => p.Replace(".txt", "").Last() == 'b').ToArray();

            if (aFiles.Length == 0)
            {
                paths = bFiles;
            }
            else if (bFiles.Length == 0)
            {
                paths = aFiles;
            }
            else
            {
                FileInfo aFileInfo = new FileInfo(aFiles[0]);
                FileInfo bFileInfo = new FileInfo(bFiles[0]);
                if (aFileInfo.Length == 0)
                    paths = bFiles;
                else if (bFileInfo.Length == 0)
                    paths = aFiles;
                else paths = aFiles;
            }

            paths = paths.OrderBy(p => p).ToArray();
            return paths;
        }
    }
}
