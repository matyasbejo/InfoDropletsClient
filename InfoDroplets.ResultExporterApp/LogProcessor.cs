using InfoDroplets.ResultExporter.Models;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace InfoDroplets.ResultExporterApp
{
    public class LogProcessor
    {
        internal List<List<LogEntry>> LogCollection { get; private set;  } = new List<List<LogEntry>>();
        internal List<List<LogEntry>> BreakCollection { get; private set; } = new List<List<LogEntry>>();
        internal int DropletNumber { get; private set; }
        internal int ElevationRange { get; private set; }
        internal LogEntry? CenterPos { get; private set; }

        string[] paths;

        public LogProcessor(string[] paths)
        {
            this.paths = paths;
        }

        public LogProcessor() { }
    
        public bool Execute()
        {
            GlobalLabelHelper.Instance.LabelText = "Selecting a/b file version...";
            FilterABFiles(ref paths);

            GlobalLabelHelper.Instance.LabelText = "Getting droplet id...";
            DropletNumber = GetDropletNumber(paths[0]);

            GlobalLabelHelper.Instance.LabelText = "Reading logfiles...";
            List<string[]> AllFilesLines = new List<string[]>();
            foreach (string path in paths)
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Can't find log file: {path}");
                AllFilesLines.Add(File.ReadAllLines(path));
            }

            GlobalLabelHelper.Instance.LabelText = "Processing position data in log files...";
            ProcessFiles(AllFilesLines);

            GlobalLabelHelper.Instance.LabelText = "Calculating center position of map...";
            CenterPos = GetMapCenterPos(); //TODO: összes teszt megírása. Ez pl tesztelhető lesz ha kap egy minimális mock??? databaset.

            GlobalLabelHelper.Instance.LabelText = "Calculating upper limit of elevation function...";
            GetElevationUpperLimit();

            GlobalLabelHelper.Instance.LabelText = "[Success] Logfiles processed successfully";
            return true;
        }

        public string[] GetDataRowsFromFileLines(string[] fileLines)
        {
            if (fileLines.Length == 0 || fileLines[0] != "Developed by Soprobotics.") throw new Exception("The selected file is not a log entry");

            int IndexOfSeparator = Array.IndexOf(fileLines, "________;________;________;________;________;________");

            string[] fileData = fileLines.Skip(IndexOfSeparator + 1).ToArray();
            return fileData;
        }

        public List<LogEntry> ProcessFile(string[] fileData)
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

        public bool FilterABFiles(ref string[] paths)
        {
            var aFilePaths = paths.Where(p =>
                Path.GetFileName(p).StartsWith("l", StringComparison.OrdinalIgnoreCase) &&
                Path.GetFileName(p).ToLower().Replace(".txt", "").Last() == 'a'
            )?.ToArray();

            var bFilePaths = paths.Where(p =>
                Path.GetFileName(p).StartsWith("l", StringComparison.OrdinalIgnoreCase) &&
                Path.GetFileName(p).ToLower().Replace(".txt", "").Last() == 'b'
            )?.ToArray();

            if (aFilePaths.Length == 0 && bFilePaths.Length == 0)
                throw new Exception("Folder contains no valid log files");

            FileInfo? aFileInfo = aFilePaths.Length == 0 ? null : new FileInfo(aFilePaths.Last());
            FileInfo? bFileInfo = aFilePaths.Length == 0 ? null : new FileInfo(aFilePaths.Last());

            if (aFileInfo == null || !aFileInfo.Exists || aFileInfo.Length == 0)
                paths = bFilePaths;
            else if (bFileInfo == null || !bFileInfo.Exists || bFileInfo.Length == 0)
                paths = aFilePaths;
            else paths = aFilePaths;

            paths = paths.OrderBy(p => p).ToArray();

            return true;
        }

        public bool ProcessFiles(List<string[]> allFilesLines)
        {
            foreach (string[] fileLines in allFilesLines)
            {
                var fileData = GetDataRowsFromFileLines(fileLines);
                var result = ProcessFile(fileData);
                if (result != null && result.Count > 0)
                {
                    LogCollection.Add(result);

                    LogEntry lastNewLogEntry = result.Last();
                    LogEntry firstNewLogEntry = result.First();

                    if (BreakCollection.Count > 0)
                    {
                        BreakCollection.Last().Add(firstNewLogEntry);
                    }

                    List<LogEntry> NewBreakList = new List<LogEntry>();
                    BreakCollection.Add(NewBreakList);

                    BreakCollection.Last().Add(lastNewLogEntry);
                }
            }

            if (BreakCollection.Last().Count == 1)
                BreakCollection.Remove(BreakCollection.Last());

            return true;
        }

        public LogEntry GetMapCenterPos()
        {
            if (LogCollection.SelectMany(innerList => innerList).Count() == 0)
                throw new Exception("Can't select map center position, because collection is empty.");

            LogEntry? centerPos;
            var maxPossibleCentrePoint = new LogEntry(50.457907, 30.559462, 0);
            var minPossibleCentrePoint = new LogEntry(44.411419, 8.915747, 0);

            var maxLat = LogCollection.SelectMany(innerList => innerList).Select(pos => pos.Latitude).Max();
            var minLat = LogCollection.SelectMany(innerList => innerList).Select(pos => pos.Latitude).Min();
            var avgLat = (minLat + maxLat) / 2;

            var maxLong = LogCollection.SelectMany(innerList => innerList).Select(pos => pos.Longitude).Max();
            var minLong = LogCollection.SelectMany(innerList => innerList).Select(pos => pos.Longitude).Min();
            var avgLong = (minLong + maxLong) / 2;

            centerPos = new LogEntry(avgLat, avgLong , 0);
            if (centerPos.Latitude > maxPossibleCentrePoint.Latitude || centerPos.Longitude > maxPossibleCentrePoint.Longitude ||
                centerPos.Latitude < minPossibleCentrePoint.Latitude || centerPos.Longitude < minPossibleCentrePoint.Longitude)
                throw new ArgumentOutOfRangeException("Unrealistic centerpoint calculated for hungarian test flight.");

            return centerPos;
        }

        public bool GetElevationUpperLimit()
        {
            if (LogCollection.Count() == 0)
                throw new Exception("Can't select Elevation, because collection is empty.");

            double maxPossibleElevation = 30_000;
            double maxElevationInLogs = LogCollection.SelectMany(innerList => innerList).Where(pos => pos.Elevation < maxPossibleElevation).Select(pos => pos.Elevation).Max();
            int elevationRange = Convert.ToInt32(Math.Ceiling(maxElevationInLogs * 1.1));
            ElevationRange = elevationRange;
            return true;
        }

        public int GetDropletNumber(string path)
        {
            Match match = Regex.Match(System.IO.Path.GetFileName(path), @"^.*L(\d{1,2})(?!\d)");

            if (match.Success)
            {
                int dropletNumber = int.Parse(match.Groups[1].Value);
                return dropletNumber;
            }
            else
            {
                throw new Exception("Device ID not found.");
            }
        }
    }
}
