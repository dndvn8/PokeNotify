using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeNotify.Core
{
    public interface ISettings
    {
        string Pokesnipe2Path { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
