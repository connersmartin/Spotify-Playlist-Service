using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class TrackAlbum:BaseSpotifyObject
    {

        [JsonPropertyName("album_type")]
        public string AlbumType { get; set; }
        [JsonPropertyName("artists")]
        public BaseArtist[] Artists { get; set; }
        [JsonPropertyName("available_markets")]
        public string[] AvailableMarkets { get; set; }
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }
        [JsonPropertyName("release_date_precision")]
        public string ReleaseDatePrecision { get; set; }
        [JsonPropertyName("total_tracks")]
        public int TotalTracks { get; set; }

    }
}
