using CQRS.Core.Domain;
using CQRS.Core.Events;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Post.Cmd.Infractructure.config;

namespace Post.Cmd.Infractructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mClient = new MongoClient(config.Value.ConnectionString);
        var mongoDB = mClient.GetDatabase(config.Value.Database);

        _eventStoreCollection = mongoDB.GetCollection<EventModel>(config.Value.Collecntion);
    }
    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel e)
    {
        await _eventStoreCollection.InsertOneAsync(e).ConfigureAwait(false);
    }
}
