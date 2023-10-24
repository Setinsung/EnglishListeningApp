using Listening.Domain.Entities;
using MediatR;

namespace Listening.Domain.Events;


public record AlbumUpdatedEvent(Album Value) : INotification;
