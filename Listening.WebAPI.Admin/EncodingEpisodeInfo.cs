using Commons.Domain.Models;

namespace Listening.WebAPI.Admin;

public record EncodingEpisodeInfo(
    Guid Id, 
    MultilingualString Name, 
    Guid AlbumId, 
    double DurationInSecond, 
    string Subtitle, 
    string SubtitleType, 
    string Status
);