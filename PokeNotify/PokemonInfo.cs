using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeNotify
{
    public class PokemonInfo
    {
        //public string EncounterId { get; set; }
        //public string ExpirationTimestamp { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public PokemonId Id { get; set; }
        //public string SpawnPointId { get; set; }
        //public string Move1 { get; set; }
        //public string Move2 { get; set; }
        public double IV { get; set; }
        //public string VerifiedOn { get; set; }
        //public ChannelInfo ChannelInfo { get; set; }
        //public string ReceivedTimeStamp { get; set; }
    }

    public class ChannelInfo
    {
        public string server { get; set; }
        public string channel { get; set; }
    }
}
