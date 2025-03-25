using InfoDroplets.ResultExporter.Models;
using InfoDroplets.ResultExporterApp;
using System.IO;
using System.Windows.Markup.Localizer;

namespace ResultExporterApp
{
    internal class MapGenerator
    {
        internal Dictionary<string, string> CustomFileValues { get; }
        internal LogProcessor logProcessor { get; private set; }

        public MapGenerator(LogProcessor logprocessor)
        {
            this.logProcessor = logprocessor;

            CustomFileValues = new Dictionary<string, string>
            {
                { "_RE_TITLE_", String.Empty },
                { "_RE_CTR_LNG_", String.Empty },
                { "_RE_CTR_LAT_", String.Empty },
                { "_RE_YMAX_", String.Empty }
            };
        }

        internal bool Execute(string outputFolder)
        {
            var newFilePath = GetNewFilePath(outputFolder);

            PrepareOutputFolder(newFilePath);

            FillDictionary(logProcessor.DropletNumber, logProcessor.ElevationRange, logProcessor.CenterPos.Longitude, logProcessor.CenterPos.Latitude);

            var NewFileContent = CreateFileContent(logProcessor.LogCollection, logProcessor.BreakCollection);
            if (NewFileContent.Contains("_RE_"))
                throw new Exception("Map content creation failed");
            
            File.WriteAllText(newFilePath, NewFileContent);

            return File.Exists(newFilePath);
        }

        internal string GetNewFilePath(string outputFolder)
        {
            string newFileName = $"Flight analytics L{logProcessor.DropletNumber} - {DateTime.Today.ToString("dd.MM.yyyy.")}.html";
            return Path.Combine(outputFolder, newFileName);
        }

        bool PrepareOutputFolder(string newFilePath)
        {
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
                Thread.Sleep(1000);
            }

            return !File.Exists(newFilePath);
        }

        bool FillDictionary(int deviceId, int yMax, double ctrLng, double ctrLat)
        {
            string title = $"L{deviceId}-{DateTime.Today.ToString("d")}";
            CustomFileValues["_RE_TITLE_"] = title;
            CustomFileValues["_RE_YMAX_"] = yMax.ToString();
            CustomFileValues["_RE_CTR_LNG_"] = ctrLng.ToString();
            CustomFileValues["_RE_CTR_LAT_"] = ctrLat.ToString();

            return true;
        }

        string CreateFileContent(List<List<LogEntry>> logEntries, List<List<LogEntry>> breakEntries)
        {
            string sampleMapPath = Path.GetFullPath(@"..\..\..\SampleMap.html");
            var MapContent = File.ReadAllText(sampleMapPath);
            foreach (var item in CustomFileValues)
            {
                MapContent = MapContent.Replace(item.Key, item.Value);
            }

            string logSegmentKey = "_RE_LOGSEGMENT_";
            string breakSegmentKey = "_RE_BREAKSEGMENT_";

            string logSegmentsContent = GenerateSegments(logEntries);
            string breakSegmentsContent = GenerateSegments(breakEntries);

            MapContent = MapContent.Replace(logSegmentKey, logSegmentsContent);
            MapContent = MapContent.Replace(breakSegmentKey, breakSegmentsContent);
            return MapContent;
        }

        string GenerateSegments(List<List<LogEntry>> entries)
        {
            string output = "";
            foreach (var entryList in entries)
            {
                output += $"\t\t\t\t{GenerateTrackSegment(entryList)} \r\n";
            }
            output += "\r\n";
            return output;
        }

        string GenerateTrackSegment(List<LogEntry> entryList)
        {
            string output = "trk[t].segments.push({ points:[";
            foreach (var entry in entryList)
            {
                output += $"{entry.ToString()},";
            }
            output = output.TrimEnd(',');
            output += "]});";

            return output;
        }
    }
}

