using InfoDroplets.ResultExporter.Models;
using System.Data;
using System.IO;

namespace InfoDroplets.ResultExporter
{
    internal static class LogProcessor
    {
        internal static List<List<LogEntry>> GlobalLogCollection { get; } = new List<List<LogEntry>>();
        internal static List<List<LogEntry>> GlobalBreakCollection { get; } = new List<List<LogEntry>>();

        static string[] GetDataRowsFromFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            var FileLines = File.ReadAllLines(path);

            if (FileLines[0] != "Developed by Soprobotics.") throw new Exception("The selected file is not a log entry");

            int IndexOfSeparator = Array.IndexOf(FileLines, "________;________;________;________;________;________");

            string[] fileData = FileLines.Skip(IndexOfSeparator + 1).ToArray();
            return fileData;
        }

        static List<LogEntry> ProcessFile(string[] fileData)
        {
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

            List<LogEntry> result = convertedValues.Where(le => le.Latitude != 0 && le.Longitude != 0).Distinct().ToList(); //filter out [0,0,0] LogEntry-s
            return result;
        }

        static void FilterABFiles(ref string[] paths)
        {
            var aFiles = paths.Where(p => p.Replace(".txt", "").Last() == 'a').ToArray();
            var bFiles = paths.Where(p => p.Replace(".txt", "").Last() == 'b').ToArray();

            FileInfo aFileInfo = new FileInfo(aFiles[0]);
            FileInfo bFileInfo = new FileInfo(bFiles[0]);
            if (!aFileInfo.Exists || aFileInfo.Length == 0)
                paths = bFiles;
            else if (!bFileInfo.Exists || bFileInfo.Length == 0)
                paths = aFiles;
            else paths = aFiles;

            paths = paths.OrderBy(p => p).ToArray();
        }

        internal static bool ProcessFiles(string[] paths)
        {
            FilterABFiles(ref paths);
            foreach (var path in paths)
            {
                var fileData = GetDataRowsFromFile(path);
                var result = ProcessFile(fileData);
                if (result != null && result.Count > 0)
                {
                    GlobalLogCollection.Add(result);

                    LogEntry lastNewLogEntry = result.Last();
                    LogEntry firstNewLogEntry = result.First();

                    if (GlobalBreakCollection.Count > 0)
                    {
                        GlobalBreakCollection.Last().Add(firstNewLogEntry);
                    }

                    List<LogEntry> NewBreakList = new List<LogEntry>();
                    GlobalBreakCollection.Add(NewBreakList);

                    GlobalBreakCollection.Last().Add(lastNewLogEntry);
                }
            }

            if (GlobalBreakCollection.Last().Count == 1)
                GlobalBreakCollection.Remove(GlobalBreakCollection.Last());

            return true;
        }
    }
}
