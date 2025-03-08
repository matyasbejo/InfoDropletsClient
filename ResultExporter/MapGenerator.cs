using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultExporter
{
    internal static class MapGenerator
    {
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
