using InfoDroplets.ResultExporter.Models;
using System.Windows.Markup.Localizer;

namespace ResultExporter
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

        internal static void PrepareValues(int deviceId, int yMax, double ctrLng, double ctrLat)
        {
            string title = $"Droplet{deviceId} flight path {DateTime.Today.ToString("yyyy.MM.dd")}";
            CustomFileValues["_RE_TITLE_"] = title;
            CustomFileValues["_RE_YMAX_"] = yMax.ToString();
            CustomFileValues["_RE_CTR_LNG_"] = ctrLng.ToString();
            CustomFileValues["_RE_CTR_LAT_"] = ctrLat.ToString();
        }

        static string CreateFileContent(List<List<LogEntry>> logEntries, List<List<LogEntry>> breakEntries)
        {
            string sampleMapPath = Path.GetFullPath(@"..\..\..\SampleMap.html");
            var MapContent = File.ReadAllText(sampleMapPath);
            foreach (var item in CustomFileValues)
            {
                MapContent = MapContent.Replace(item.Key, item.Value);
            }
            output += "\r\n\t\t\t\tGV_Draw_Track(t);\r\n\t\t\t\t\r\n\t\t\t\t t = 1; GV_Add_Track_to_Tracklist({ bullet: '- ', name: trk[t].info.name, desc: trk[t].info.desc, color: trk[t].info.color, number: t });\r\n\r\n";

            string logSegmentKey = "_RE_LOGSEGMENT_";
            string breakSegmentKey = "_RE_BREAKSEGMENT_";

            string logSegmentsContent = GenerateSegments(logEntries);
            string breakSegmentsContent = GenerateSegments(breakEntries);

            MapContent = MapContent.Replace(logSegmentKey, logSegmentsContent);
            MapContent = MapContent.Replace(breakSegmentKey, breakSegmentsContent);
            return MapContent;
        }

        static List<string> GenerateTrack(List<List<LogEntry>> entries)
        {
            List<string> trackSegments = new List<string>();

            foreach (var entryList in entries)
            {
                trackSegments.Add(GenerateTrackSegment(entryList));
            }
            return trackSegments;
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

        /*
         * _RE_TITLE_ - cím
         * _RE_CTR_LNG_ - közép lng
         * _RE_CTR_LAT_ - közép lat
         * _RE_INFOS_ - infók a gv_infoboxba
         * _RE_YMAX_ - max elevation a trend boxba
         * 
         * rajzolás logika - valódi adat
         * 
         * t = 1; trk[t] = {info:[],segments:[]};
         * trk[t].info.name = 'data'; trk[t].info.desc = ''; trk[t].info.clickable = true;
         * trk[t].info.color = '#e60000'; trk[t].info.width = 3; trk[t].info.opacity = 0.9; trk[t].info.hidden = false; trk[t].info.z_index = null;
         * trk[t].info.outline_color = 'black'; trk[t].info.outline_width = 0; trk[t].info.fill_color = '#e60000'; trk[t].info.fill_opacity = 0;
         * trk[t].info.elevation = true;
         * trk[t].segments.push({ points:[ [46.321819,18.465038,143.7],[... }); <-- minden egybefüggő adatsor egy ilyen cucc
         * GV_Draw_Track(t);
         * GV_Add_Track_to_Tracklist({ bullet: '- ', name: trk[t].info.name, desc: trk[t].info.desc, color: trk[t].info.color, number: t });
         * 
         * t = 2; trk[t] = { info: [], segments: [] };
         * trk[t].info.name = 'breaks'; trk[t].info.desc = ''; trk[t].info.clickable = true;
         * trk[t].info.color = '#887c54'; trk[t].info.width = 3; trk[t].info.opacity = 0.7; trk[t].info.hidden = false; trk[t].info.z_index = null;
         * trk[t].info.outline_color = 'black'; trk[t].info.outline_width = 0; trk[t].info.fill_color = '#e60000'; trk[t].info.fill_opacity = 0;
         * trk[t].info.elevation = true;
         * trk[t].segments.push({ points: [[46.22662, 18.94051, 12097.2], [46.198009, 19.068056, 12097.2]] }); <--- logika ami a valódi adatok utolsó és első entryt összeköti
         * GV_Draw_Track(t);
         * GV_Add_Track_to_Tracklist({ bullet: '- ', name: trk[t].info.name, desc: trk[t].info.desc, color: trk[t].info.color, number: t });				
         * 
         * GV_Finish_Map();
         * */
    }
}

