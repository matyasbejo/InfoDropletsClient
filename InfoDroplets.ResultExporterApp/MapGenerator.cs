using InfoDroplets.ResultExporter.Models;
using System.IO;
using System.Windows.Markup.Localizer;

namespace ResultExporterApp
{
    internal static class MapGenerator
    {
        internal static Dictionary<string, string> CustomFileValues { get; }

        static MapGenerator()
        {
            CustomFileValues = new Dictionary<string, string>
            {
                { "_RE_TITLE_", String.Empty },
                { "_RE_CTR_LNG_", String.Empty },
                { "_RE_CTR_LAT_", String.Empty },
                { "_RE_YMAX_", String.Empty }
            };
        }

        internal static bool CreateMap(int deviceId, int yMax, double ctrLng, double ctrLat, List<List<LogEntry>> logEntries, List<List<LogEntry>> breakEntries)
        {
            if (!FillDictionary(deviceId, yMax, ctrLng, ctrLat))
                return false;

            var NewFileContent = CreateFileContent(logEntries, breakEntries);
            if(NewFileContent.Contains("_RE_"))
                return false;

            File.WriteAllText($"Flight analytics L{deviceId} - {DateTime.Today.ToString("dd.MM.yyyy.")}.html", NewFileContent); //todo file output location selection 
            return true;
        }

        static bool FillDictionary(int deviceId, int yMax, double ctrLng, double ctrLat)
        {
            string title = $"L{deviceId}-{DateTime.Today.ToString("d")}";
            CustomFileValues["_RE_TITLE_"] = title;
            CustomFileValues["_RE_YMAX_"] = yMax.ToString();
            CustomFileValues["_RE_CTR_LNG_"] = ctrLng.ToString();
            CustomFileValues["_RE_CTR_LAT_"] = ctrLat.ToString();

            return true;
        }

        static string CreateFileContent(List<List<LogEntry>> logEntries, List<List<LogEntry>> breakEntries)
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

        static string GenerateSegments(List<List<LogEntry>> entries)
        {
            string output = "";
            foreach (var entryList in entries)
            {
                output += $"\t\t\t\t{GenerateTrackSegment(entryList)} \r\n";
            }
            output += "\r\n";
            return output;
        }

        static string GenerateTrackSegment(List<LogEntry> entryList)
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

