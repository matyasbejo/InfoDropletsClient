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

        internal bool CreateMap(string outputFolder)
        {
            if (!FillDictionary(logProcessor.DropletNumber, logProcessor.ElevationRange, logProcessor.CenterPos.Longitude, logProcessor.CenterPos.Latitude))
                return false;

            var NewFileContent = CreateFileContent(logProcessor.LogCollection, logProcessor.BreakCollection);
            if(NewFileContent.Contains("_RE_"))
                return false;

            string newFileName = $"Flight analytics L{logProcessor.DropletNumber} - {DateTime.Today.ToString("dd.MM.yyyy.")}.html";
            
            File.WriteAllText(Path.Combine(outputFolder, newFileName), NewFileContent);
            return true;
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

