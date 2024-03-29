﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PlaylistRequest : BaseRequest
    {
        public string OldPlaylistId { get; set; }
        public string Name { get; set; }
        public int Length { get; set; }
        public string[] Genres { get; set; }
        public string[] TrackIds { get; set; }
        public string Artist { get; set; }
        public string Tempo { get; set; }
        public string Dance { get; set; }
        public string Energy { get; set; }
        public string Valence { get; set; }
        public bool AudioFeatures { get; set; }
        public bool SavedTracks { get; set; }
        public bool UpdateTracks { get; set; }
        public AudioFeatures StandardDeviation { get; set; }
    }
}
