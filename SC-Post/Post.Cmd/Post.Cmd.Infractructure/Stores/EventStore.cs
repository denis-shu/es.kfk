using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infractructure.Stores;

public class EventStore : IEventstore
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public EventStore(IEventStoreRepository erep)
    {
        _eventStoreRepository = erep;
    }
    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream == null || !eventStream.Any()) throw new AggregateNotFoundException("er");

        return eventStream.OrderBy(x => x.Version).Select(s => s.EventData).ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion) throw new ConcurrencyException();

        var ver = expectedVersion;

        foreach (var e in events)
        {
            ver++;
            e.Version = ver;
            var et = e.GetType().Name;
            var eM = new EventModel
            {
                TimeStamp = DateTime.Now,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = ver,
                EventType = et,
                EventData = e
            };
            await _eventStoreRepository.SaveAsync(eM);
        }

    }
}
