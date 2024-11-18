using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Utils.Interfaces
{
    public interface IGpsPos
    {
        double Elevation { get; }
        double Latitude { get; }
        double Longitude { get; }
    }
}
