using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PokeNotify.Core
{
    public class PokeInfoModel
    {
        [JsonProperty("ExpirationTimestamp")]
        public DateTime ExpirationTimestamp { get; set; }
        [JsonProperty("Latitude")]
        public double Latitude { get; set; }
        [JsonProperty("Longitude")]
        public double Longitude { get; set; }
        [JsonProperty("Id")]
        public PokemonId Id { get; set; } = PokemonId.Missingno;
        [JsonProperty("IV")]
        public double IV { get; set; }
        [JsonProperty("Move1")]
        public PokemonMove Move1 { get; set; }
        [JsonProperty("Move2")]
        public PokemonMove Move2 { get; set; }

    }
}
