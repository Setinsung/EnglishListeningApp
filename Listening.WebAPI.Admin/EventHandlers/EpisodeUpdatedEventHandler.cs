using Commons.EventBus;
using Listening.Domain.Events;
using MediatR;

namespace Listening.WebAPI.Admin.EventHandlers;

public class EpisodeUpdatedEventHandler : INotificationHandler<EpisodeUpdatedEvent>
{
    private readonly IEventBus _eventBus;

    public EpisodeUpdatedEventHandler(IEventBus eventBus)
    {
        this._eventBus = eventBus;
    }
    public Task Handle(EpisodeUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var episode = notification.Value;
        if(episode.IsVisible)
        {
            var sentences = episode.ParseSubtitle();
            // 不修改音频地址
            _eventBus.Publish("ListeningEpisode.Update", new
            {
                episode.Id,
                episode.Name,
                Sentences = sentences,
                episode.AlbumId,
                episode.Subtitle,
                episode.SubtitleType
            });
        }
        else
        {
            _eventBus.Publish("ListeningEpisode.Update", new { episode.Id });
        }
        return Task.CompletedTask;
    }
}
