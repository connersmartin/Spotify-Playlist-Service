using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class Album
    {
    }


//    KEY VALUE TYPE VALUE DESCRIPTION
//   album_group string, optional The field is present when getting an artist’s albums.Possible values are “album”, “single”, “compilation”, “appears_on”. Compare to album_type this field represents relationship between the artist and the album.
//album_type  string The type of the album: one of “album”, “single”, or “compilation”.
//artists array of simplified artist objects  The artists of the album.Each artist object includes a link in href to more detailed information about the artist.
//available_markets array of strings    The markets in which the album is available: ISO 3166-1 alpha-2 country codes. Note that an album is considered available in a market when at least 1 of its tracks is available in that market.
//external_urls an external URL object Known external URLs for this album.
//href    string A link to the Web API endpoint providing full details of the album.
//id  string The Spotify ID for the album.
//images array of image objects The cover art for the album in various sizes, widest first.
//name    string The name of the album. In case of an album takedown, the value may be an empty string.
//release_date    string The date the album was first released, for example 1981. Depending on the precision, it might be shown as 1981-12 or 1981-12-15.
//release_date_precision  string The precision with which release_date value is known: year , month , or day.
//restrictions a restrictions object Part of the response when Track Relinking is applied, the original track is not available in the given market, and Spotify did not have any tracks to relink it with.The track response will still contain metadata for the original track, and a restrictions object containing the reason why the track is not available: "restrictions" : { "reason" : "market"}
//    type string The object type: “album”
//uri string The Spotify URI for the album.
}
