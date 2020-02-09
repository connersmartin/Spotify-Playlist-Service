using Microsoft.Extensions.Caching.Memory;
using SpotListAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Services
{
    public class AudioFeaturesService
    {
        private readonly IMemoryCache _cache;
        private readonly SpotifyService _spotifyService;
        private readonly Helper _helper;
        public AudioFeaturesService(IMemoryCache cache, SpotifyService spotifyService, UserService userService, Helper helper)
        {
            _cache = cache;
            _spotifyService = spotifyService;
            _helper = helper;
        }

        //this will be used as the template for the playlist request to send to 
        public async Task<PlaylistRequest> GetAudioDataFromTracks(List<Track> tracks, string auth)
        {
            //TODO This can totally be its own helper function "Get X of something"

            var audioFeatures = await GetAudioFeaturesFromTracks(tracks, auth);

            //get genres from the artists
            var genres = await GetGenresFromArtists(tracks, auth);

            //match those genres with spotify's existing ones
            var genreList = await MatchGenres(genres, auth);

            //calculate and return audio features using standard deviation for now
            var paramData = CalculateAudioFeatures(audioFeatures);

            paramData.Genres = genreList.ToArray();

            return paramData;
        }


        private async Task<List<string>> GetGenresFromArtists(List<Track> tracks, string auth)
        {
            var artistList = new List<Artist>();

            foreach (var t in tracks)
            {
                artistList.AddRange(t.Artists);
            }

            var artistIds = artistList.Select(a => a.Id).ToList();

            var numArtists = artistIds.Count;
            int artistChunks = (numArtists + 49) / 50;
            //break into 100 song chunks
            var artistArray = new List<string>[artistChunks];

            for (int i = 0; i < artistChunks; i++)
            {
                artistArray[i] = artistIds.Take(50).Skip(i * 50).ToList();
            }

            //Get Spotify artist ids from tracks (max 50 per request)
            var artistUrl = "artists?ids=";
            var fullArtistList = new List<FullArtist>();

            foreach (var a in artistArray)
            {
                var ids = string.Join(",", a);
                var getArtistResponse = await _spotifyService.SpotifyApi(auth, artistUrl + ids, "get");
                var getArtists = _helper.Mapper<FullArtistsResponse>(await getArtistResponse.Content.ReadAsByteArrayAsync());
                fullArtistList.AddRange(getArtists.Artists);
            }

            var genresList = new List<string>();

            foreach (var a in fullArtistList)
            {
                genresList.AddRange(a.Genres);
            }
            
            return genresList.Select(g => g.Replace(" ", "-")).ToList();
        }

        private async Task<List<AudioFeatures>> GetAudioFeaturesFromTracks(List<Track> tracks, string auth)
        {
            var numTracks = tracks.Count;
            int trackChunks = (numTracks + 99) / 100;
            //break into 100 song chunks
            var trackArray = new List<string>[trackChunks];

            for (int i = 0; i < trackChunks; i++)
            {
                var intTracks = tracks.Take(100).Skip(i * 100).ToList();
                trackArray[i] = intTracks.Select(i => i.Id).ToList();
            }


            //get audio features from spotify track ids (max 100 per request)
            var afUrl = "audio-features?ids=";
            var listAudioFeatures = new List<AudioFeatures>();
            foreach (var t in trackArray)
            {
                var ids = string.Join(",", t);
                var getAudioFeaturesResponse = await _spotifyService.SpotifyApi(auth, afUrl + ids, "get");
                var getAudioFeatures = _helper.Mapper<AudioFeaturesResponse>(await getAudioFeaturesResponse.Content.ReadAsByteArrayAsync());
                listAudioFeatures.AddRange(getAudioFeatures.AudioFeatures);
            }

            return listAudioFeatures;

        }

        public async Task<List<string>> GetGenres(string auth)
        {
            //Get Genres from spotify
            var url = "recommendations/available-genre-seeds";
            var getGenresResponse = await _spotifyService.SpotifyApi(auth, url, "get");
            var getGenres = _helper.Mapper<Dictionary<string,List<string>>>(await getGenresResponse.Content.ReadAsByteArrayAsync());

            return getGenres["genres"];
        }

        private async Task<List<string>> MatchGenres(List<string> genres, string auth)
        {
            var genreList = new List<string>();

            var spotifyGenres = await GetGenres(auth);
            var availGenres = new List<string>();
            //var availGenres = genres.Intersect(spotifyGenres).ToList();

            //find a better way to match genres.
            //issue is that artist genres are NOT 1:1 with spotify genres

            foreach (var gen in genres)
            {
                foreach (var sg in spotifyGenres)
                {
                    if (gen==sg)
                    {
                        availGenres.Add(sg);
                    }
                    else if (gen.Contains(sg))
                    {
                        availGenres.Add(sg);
                    }
                }
            }

                       
            var topFive = availGenres.GroupBy(g => g)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                }).OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name).Take(5);
            
            foreach(var t in topFive)
            {
                genreList.Add(t.Name);
            }

            return genreList;
        }

        private PlaylistRequest CalculateAudioFeatures(List<AudioFeatures> audioFeatures)
        {
            var averages = GetAverages(audioFeatures);

            var stdDeviations = GetStandardDeviations(audioFeatures, averages);

            return new PlaylistRequest()
            {
                Dance = averages.Danceability.ToString(),
                Energy = averages.Energy.ToString(),
                Tempo = averages.Tempo.ToString(),
                Valence = averages.Valence.ToString(),
                StandardDeviation = stdDeviations
            };
        }

        public AudioFeatures GetAverages(List<AudioFeatures> audioFeatures)
        {
            return new AudioFeatures()
            {
                Danceability = audioFeatures.Average(a => a.Danceability),
                Energy = audioFeatures.Average(a => a.Energy),
                Tempo = audioFeatures.Average(a => a.Tempo),
                Valence = audioFeatures.Average(a => a.Valence)
            };
        }

        public AudioFeatures GetStandardDeviations(List<AudioFeatures> audioFeatures, AudioFeatures avgAudioFeatures)
        {
            var da = (float)Math.Sqrt(audioFeatures.Sum(a => Math.Pow(a.Danceability - avgAudioFeatures.Danceability, 2)) / audioFeatures.Count);
            var ene = (float)Math.Sqrt(audioFeatures.Sum(a => Math.Pow(a.Energy - avgAudioFeatures.Energy, 2)) / audioFeatures.Count);
            var te = (float)Math.Sqrt(audioFeatures.Sum(a => Math.Pow(a.Tempo - avgAudioFeatures.Tempo, 2)) / audioFeatures.Count);
            var val = (float)Math.Sqrt(audioFeatures.Sum(a => Math.Pow(a.Valence - avgAudioFeatures.Valence, 2)) / audioFeatures.Count);
            
            return new AudioFeatures()
            {
                Danceability = da,
                Energy = ene,
                Tempo = te,
                Valence = val
            };

        }


    }
}
