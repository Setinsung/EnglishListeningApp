using Listening.Domain.Entities;
using MediatR;

namespace Listening.Domain.Events;

public record CategoryCreatedEvent(Category Value) : INotification;
