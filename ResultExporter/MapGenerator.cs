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

        internal static string GenerateGV_MapFunctionString(List<List<LogEntry>> logEntries, List<List<LogEntry>> breakEntries)
        {
            if (logEntries == null || logEntries.Count == 0)
                throw new Exception("There are no logEntries to write in GV_Map function!");

            string output = "\t\t\tfunction GV_Map() {\r\n\t\t\t\tGV_Setup_Map();\r\n\t\t\t\t\r\n\t\t\t\t// Track #1\r\n\t\t\t\tt = 1; trk[t] = {info:[],segments:[]};\r\n\t\t\t\ttrk[t].info.name = 'data'; trk[t].info.desc = ''; trk[t].info.clickable = true;\r\n\t\t\t\ttrk[t].info.color = '#e60000'; trk[t].info.width = 3; trk[t].info.opacity = 0.9; trk[t].info.hidden = false; trk[t].info.z_index = null;\r\n\t\t\t\ttrk[t].info.outline_color = 'black'; trk[t].info.outline_width = 0; trk[t].info.fill_color = '#e60000'; trk[t].info.fill_opacity = 0;\r\n\t\t\t\ttrk[t].info.elevation = true;\r\n";
            
            List<string> logTrack = GenerateTrack(logEntries);
            foreach (string segment in logTrack)
            {
                output += $"\t\t\t\t{segment}\r\n";
            }
            output += "\r\n\t\t\t\tGV_Draw_Track(t);\r\n\t\t\t\t\r\n\t\t\t\t t = 1; GV_Add_Track_to_Tracklist({ bullet: '- ', name: trk[t].info.name, desc: trk[t].info.desc, color: trk[t].info.color, number: t });\r\n\r\n";

            if (breakEntries != null || breakEntries.Count != 0)
            {
                output += "                // Track #2\r\n                t = 2; trk[t] = { info: [], segments: [] };\r\n                trk[t].info.name = 'breaks'; trk[t].info.desc = ''; trk[t].info.clickable = true;\r\n                trk[t].info.color = '#53a238'; trk[t].info.width = 3; trk[t].info.opacity = 0.7; trk[t].info.hidden = false; trk[t].info.z_index = null;\r\n                trk[t].info.outline_color = 'black'; trk[t].info.outline_width = 0; trk[t].info.fill_color = '#e60000'; trk[t].info.fill_opacity = 0;\r\n                trk[t].info.elevation = true;\r\n";
                List<string> breakTrack = GenerateTrack(breakEntries);
                foreach (string segment in breakTrack)
                {
                    output += $"\t\t\t\t{segment}\r\n";
                }
                output += "\r\n                GV_Draw_Track(t);\r\n\r\n                t = 2; GV_Add_Track_to_Tracklist({ bullet: '- ', name: trk[t].info.name, desc: trk[t].info.desc, color: trk[t].info.color, number: t });\r\n";
            }

            output += "\t\t\t\t\t\t\t\t\r\n\t\t\t\tGV_Finish_Map();\t\t\r\n\t\t\t}\r\n";
            return output;
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

