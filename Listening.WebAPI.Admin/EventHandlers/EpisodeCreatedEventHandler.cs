using Commons.EventBus;
using Listening.Domain.Events;
using MediatR;

namespace Listening.WebAPI.Admin.EventHandlers;

public class EpisodeCreatedEventHandler : INotificationHandler<EpisodeCreatedEvent>
{
    private readonly IEventBus _eventBus;

    public EpisodeCreatedEventHandler(IEventBus eventBus)
    {
        this._eventBus = eventBus;
    }

    public Task Handle(EpisodeCreatedEvent notification, CancellationToken cancellationToken)
    {
        var episode = notification.Value;
        var sentences = episode.ParseSubtitle();
        // 发布集成事件，用于搜索索引、记录日志等功能
        _eventBus.Publish("ListeningEpisode.Created", new 
        {
            episode.Id,
            episode.Name,
            sentences,
            episode.AlbumId,
            episode.Subtitle,
            episode.SubtitleType
        });
        return Task.CompletedTask;
    }
}
