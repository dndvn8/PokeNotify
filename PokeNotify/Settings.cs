using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokeNotify.Core;

namespace PokeNotify
{
    public class Settings : ISettings
    {
        public string Pokesnipe2Path
        {
            get { return PokeSettings.Default.Pokesnipe2Path; }
            set
            {
                PokeSettings.Default.Pokesnipe2Path = value;
                PokeSettings.Default.Save();
            }
        }
        public double Latitude
        {
            get { return PokeSettings.Default.DefaultLatitude; }
            set
            {
                PokeSettings.Default.DefaultLatitude = value;
                PokeSettings.Default.Save();
            }
        }
        public double Longitude
        {
            get { return PokeSettings.Default.DefaultLongitude; }
            set
            {
                PokeSettings.Default.DefaultLongitude = value;
                PokeSettings.Default.Save();
            }
        }
    }
}
