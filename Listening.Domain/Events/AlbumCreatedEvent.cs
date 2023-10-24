using Listening.Domain.Entities;
using MediatR;

namespace Listening.Domain.Events;

public record AlbumCreatedEvent(Album Value) : INotification;
