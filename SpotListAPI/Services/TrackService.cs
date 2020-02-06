﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SpotListAPI.Models;

namespace SpotListAPI.Services
{
    public class TrackService
    {
        private readonly IMemoryCache _cache;
        private readonly SpotifyService _spotifyService;
        private readonly UserService _userService;
        private readonly Helper _helper;
        public TrackService(IMemoryCache cache, SpotifyService spotifyService, UserService userService, Helper helper)
        {
            _cache = cache;
            _spotifyService = spotifyService;
            _userService = userService;
            _helper = helper;
        }

        //add tracks to playlist
        public async Task<PlaylistResponse> AddTracksToPlaylist(PlaylistRequest playlistRequest)
        {
            var paramDict = new Dictionary<string, string[]>();
            var url = string.Format("playlists/{0}/tracks",playlistRequest.Id);
            //get the recomendations
            var tracks = await GetRecommendedTracks(playlistRequest);
            //add the tracks
            var trackLength = 0;
            var trackList = new List<string>();
            foreach (var t in tracks)
            {
                if (trackLength <= playlistRequest.Length * 60000)
                {
                    trackLength += t.DurationMs;
                    trackList.Add(t.Uri);
                }
                else
                {
                    break;
                }
            }
    
            paramDict.Add("uris", trackList.ToArray());

            var trackStringJson =JsonSerializer.Serialize(paramDict);

            var addTracksResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "post", trackStringJson);          

            return new PlaylistResponse() { Id = playlistRequest.Id, Length = trackLength, TrackCount = trackList.Count }; 
        }
        //Gets the tracks from a given playlist
        //TODO Cache this
        public async Task<List<Track>> GetTracksFromPlaylist(GetPlaylistTracksRequest playlistTracksRequest)
        {
            var user = await _userService.GetUser(playlistTracksRequest.Auth);
            var playlistTracks = new List<Track>();
            if (!_cache.TryGetValue(user+"/"+ playlistTracksRequest.Id+"/tracks", out playlistTracks))
            {
                var playlistTracksList = new List<Track>();
                var url = string.Format("playlists/{0}/tracks", playlistTracksRequest.Id);
                var tracks = new PaginatedPlaylistTrackResponse()
                {
                    next = ""
                };
                while (tracks.next !=null)
                {
                    //get tracks from spotify
                    var trackResponse = await _spotifyService.SpotifyApi(playlistTracksRequest.Auth, url, "get");
                    Track[] trackArray;
                    //parse the tracks
                    tracks = _helper.Mapper<PaginatedPlaylistTrackResponse>(await trackResponse.Content.ReadAsByteArrayAsync());
                    if (tracks.items.Length>0)
                    {
                        trackArray = tracks.items.Select(t => t.Track).ToArray();
                        playlistTracksList.AddRange(trackArray);
                    }

                    if (tracks.limit.HasValue && tracks.offset.HasValue)
                    {
                        url = string.Format("playlists/{0}/tracks?offset={1}", playlistTracksRequest.Id, tracks.limit + tracks.offset);
                    }
                    
                }
                playlistTracks = playlistTracksList;
                //do some magic to get the proper response
                _cache.Set(user + "/" + playlistTracksRequest.Id + "/tracks", playlistTracksList);
            }
            

            return playlistTracks;
        }
        //Uses specific parameters to return recommended songs
        public async Task<List<Track>> GetRecommendedTracks (PlaylistRequest playlistRequest)
        {
            //get a ballpark limit could make this more precise
            var limitRec = playlistRequest.Length / 3;
            //hard stop at 100 tracks TODO paginate this request at a later date
            if (limitRec>100)
            {
                limitRec = 100;
            }

            var url = string.Format("recommendations?limit={0}&market=from_token&",limitRec.ToString());

            if (playlistRequest.Artist != null && playlistRequest.Artist.Trim() !="")
            {
                playlistRequest.Artist = await SearchArtistFromName(playlistRequest);
            }

            var parsedParams = !playlistRequest.AudioFeatures ? GetParsedParams(playlistRequest) : GetAudioParsedParams(playlistRequest);

            var getRecommendedTracksResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url+parsedParams, "get");

            var getRecommendedTracks = _helper.Mapper<RecommendedTracksResponse>(await getRecommendedTracksResponse.Content.ReadAsByteArrayAsync()).tracks;

            playlistRequest.Length = getRecommendedTracks.Length;

            return getRecommendedTracks.ToList();
        }


        #region Helper Functions
        //Gets artist id from name for use in recommendation
        private async Task<string> SearchArtistFromName(PlaylistRequest playlistRequest)
        {
            var artistString = "";
            var artistSplit = playlistRequest.Artist.Split(',');
            foreach (var artist in artistSplit)
            {
                if (artist != "")
                {
                    var artistEncoded = artist.Replace(" ", "%20");
                    var url = string.Format("search?q={0}&type=artist&limit=1", artistEncoded);

                    var artistSearchResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "get");

                    var artistSearch = _helper.Mapper<ArtistsResponse>(await artistSearchResponse.Content.ReadAsByteArrayAsync());
                    if (artistSearch.artists.items.Length > 0)
                    {
                        artistString += "," + artistSearch.artists.items[0].Id;
                    }
                }
            }
            return artistString.Substring(1,artistString.Length-1);
        }
        //gets parameters into a query string
        public string GetParsedParams(PlaylistRequest p)
        {
            var paramString = "";
            if (p.Genres.Length > 0) { paramString += "seed_genres=" + string.Join(",", p.Genres).Trim(); };
            if (p.Artist != "" && p.Artist!=null) { paramString += "&seed_artists=" + p.Artist; };
            if (p.Tempo != null) { paramString += "&target_tempo=" + p.Tempo; }
            if (p.Dance != null) { paramString += "&target_danceability=" + p.Dance; }
            if (p.Energy != null) { paramString += "&target_energy=" + p.Energy; }
            if (p.Valence != null) { paramString += "&target_valence=" + p.Valence; }
            return paramString;
        }

        private string GetAudioParsedParams(PlaylistRequest p)
        {
            var paramString = "";
            if (p.Genres.Length > 0) { paramString += "seed_genres=" + string.Join(",", p.Genres).Trim(); };
            if (p.Tempo != null)
            {
                var tempo = float.Parse(p.Tempo);
                paramString += "&min_tempo=" + (tempo-p.StandardDeviation).ToString();
                paramString += "&max_tempo=" + (tempo+p.StandardDeviation).ToString();
            }
            if (p.Dance != null)
            {
                var dance = float.Parse(p.Dance);
                paramString += "&min_danceability=" + (dance - p.StandardDeviation).ToString();
                paramString += "&max_danceability=" + (dance + p.StandardDeviation).ToString();
            }
            if (p.Energy != null)
            {
                var energy = float.Parse(p.Energy);
                paramString += "&min_energy=" + (energy - p.StandardDeviation).ToString();
                paramString += "&max_energy=" + (energy + p.StandardDeviation).ToString();
            }
            if (p.Valence != null)
            {
                var valence = float.Parse(p.Valence);
                paramString += "&min_valence=" + (valence - p.StandardDeviation).ToString();
                paramString += "&max_valence=" + (valence + p.StandardDeviation).ToString();
            }
            return paramString;
        }

        //Mapper to return back only info needed
        public List<TrackResponse> TracksToTrackResponse (List<Track> tracks)
        {
            var trackResponse = new List<TrackResponse>();
            foreach (var t in tracks)
            {
                trackResponse.Add(new TrackResponse
                {
                    Title = t.Name,
                    Length = t.DurationMs,
                    Artists = t.Artists.FirstOrDefault().Name
                });
            }
            return trackResponse;
        }

        #endregion
    }
}
