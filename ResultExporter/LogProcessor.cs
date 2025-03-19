using InfoDroplets.ResultExporter.Models;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace InfoDroplets.ResultExporter
{
    internal static class LogProcessor
    {
        internal static List<List<LogEntry>> GlobalLogCollection { get; private set;  } = new List<List<LogEntry>>();
        internal static List<List<LogEntry>> GlobalBreakCollection { get; private set; } = new List<List<LogEntry>>();
        internal static int DropletNumber { get; private set; }
        internal static int ElevationRange { get; private set; }
        internal static LogEntry? CenterPos { get; private set; }

        internal static bool Execute(string[] paths)
        {
            try
            {
                FilterABFiles(ref paths);
                GetDropletNumber(paths[0]);
                ProcessFiles(paths);
                GetMapCenterPos();
                GetElevationRange();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

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

        static bool FilterABFiles(ref string[] paths)
        {
            var aFiles = paths.Where(p => p.ToLower().Replace(".txt", "").Last() == 'a').ToArray();
            var bFiles = paths.Where(p => p.ToLower().Replace(".txt", "").Last() == 'b').ToArray();

            FileInfo aFileInfo = new FileInfo(aFiles[0]);
            FileInfo bFileInfo = new FileInfo(bFiles[0]);
            if (!aFileInfo.Exists || aFileInfo.Length == 0)
                paths = bFiles;
            else if (!bFileInfo.Exists || bFileInfo.Length == 0)
                paths = aFiles;
            else paths = aFiles;

            paths = paths.OrderBy(p => p).ToArray();

            return true;
        }

        static bool ProcessFiles(string[] paths)
        {
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

        static bool GetMapCenterPos()
        {
            if (GlobalLogCollection.SelectMany(innerList => innerList).Count() == 0)
                throw new Exception("Can't select map center position, because collection is empty.");

            LogEntry? centerPos;
            var maxPossibleCentrePoint = new LogEntry(50.457907, 30.559462, 0);
            var minPossibleCentrePoint = new LogEntry(44.411419, 8.915747, 0);
            try
            {
                var maxLat = GlobalLogCollection.SelectMany(innerList => innerList).Select(pos => pos.Latitude).Max();
                var minLat = GlobalLogCollection.SelectMany(innerList => innerList).Select(pos => pos.Latitude).Min();
                var avgLat = (minLat + maxLat) / 2;

                var maxLong = GlobalLogCollection.SelectMany(innerList => innerList).Select(pos => pos.Longitude).Max();
                var minLong = GlobalLogCollection.SelectMany(innerList => innerList).Select(pos => pos.Longitude).Min();
                var avgLong = (minLong + maxLong) / 2;

                centerPos = new LogEntry(avgLat, avgLong , 0);
                if (centerPos.Latitude > maxPossibleCentrePoint.Latitude || centerPos.Longitude > maxPossibleCentrePoint.Longitude ||
                    centerPos.Latitude < minPossibleCentrePoint.Latitude || centerPos.Longitude < minPossibleCentrePoint.Longitude)
                    throw new ArgumentOutOfRangeException("Unrealistic centerpoint calculated for hungarian test flight.");
            }
            catch (ArgumentOutOfRangeException)
            {
                centerPos = GlobalLogCollection.SelectMany(innerList => innerList).Where(pos => (
                    pos.Latitude > maxPossibleCentrePoint.Latitude || pos.Longitude > pos.Longitude ||
                    pos.Latitude < minPossibleCentrePoint.Latitude || pos.Longitude < minPossibleCentrePoint.Longitude)).FirstOrDefault();
                if (centerPos == null)
                    return false;
            }

            CenterPos = centerPos;
            return true;
        }

        static bool GetElevationRange()
        {
            if (GlobalLogCollection.Count() == 0)
                throw new Exception("Can't select Elevation, because collection is empty.");

            double maxPossibleElevation = 30_000;
            double maxElevationInLogs = GlobalLogCollection.SelectMany(innerList => innerList).Where(pos => pos.Elevation < maxPossibleElevation).Select(pos => pos.Elevation).Max();
            int elevationRange = Convert.ToInt32(Math.Ceiling(maxElevationInLogs * 1.1));
            ElevationRange = elevationRange;
            return true;
        }

        static bool GetDropletNumber(string path)
        {
            Match match = Regex.Match(System.IO.Path.GetFileName(path), @"L(\d{1,2})");

            if (match.Success)
            {
                int dropletNumber = int.Parse(match.Groups[1].Value);
                DropletNumber = dropletNumber;
                return false;
            }
            else
            {
                throw new Exception("Device ID not found.");
            }
        }
    }
}
