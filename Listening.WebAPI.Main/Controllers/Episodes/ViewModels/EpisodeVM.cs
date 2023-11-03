using Commons.Domain.Models;
using Listening.Domain.Entities;
using Listening.Domain.ValueObjects;

namespace Listening.WebAPI.Main.Controllers.Episodes.ViewModels;

public record EpisodeVM(Guid Id, MultilingualString Name, Guid AlbumId, Uri AudioUrl, double DurationInSecond, IEnumerable<SentenceVM?>? Sentences)
{
    public static EpisodeVM? Create(Episode? episode, bool loadSubtitle)
    {
        if (episode == null) return null;
        List<SentenceVM?> sentenceVMs = new();
        if (loadSubtitle)
        {
            IEnumerable<Sentence> sentences = episode.ParseSubtitle();
            sentenceVMs = SentenceVM.Create(sentences).ToList();
        }
        return new EpisodeVM(episode.Id, episode.Name, episode.AlbumId, episode.AudioUrl, episode.DurationInSecond, sentenceVMs);
    }
    public static EpisodeVM?[] Create(Episode[] episodes, bool loadSubtitle)
    {
        return episodes.Select(e => Create(e, loadSubtitle)).ToArray();
    }

}