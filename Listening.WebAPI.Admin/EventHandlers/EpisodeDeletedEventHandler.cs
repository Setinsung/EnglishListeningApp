using Commons.EventBus;
using Listening.Domain.Events;
using MediatR;

namespace Listening.WebAPI.Admin.EventHandlers;

public class EpisodeDeletedEventHandler : INotificationHandler<EpisodeDeletedEvent>
{
    private readonly IEventBus _eventBus;

    public EpisodeDeletedEventHandler(IEventBus eventBus)
    {
        this._eventBus = eventBus;
    }

    public Task Handle(EpisodeDeletedEvent notification, CancellationToken cancellationToken)
    {
        _eventBus.Publish("ListeningEpisode.Deleted", new { notification.Id });
        return Task.CompletedTask;
    }
}
