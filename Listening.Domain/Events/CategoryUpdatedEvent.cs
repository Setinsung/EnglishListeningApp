using Listening.Domain.Entities;
using MediatR;

namespace Listening.Domain.Events;

public record CategoryUpdatedEvent(Category Value) : INotification;
