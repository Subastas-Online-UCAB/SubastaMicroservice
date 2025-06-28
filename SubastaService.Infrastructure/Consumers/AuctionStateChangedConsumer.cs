using MassTransit;
using MongoDB.Driver;
using SubastaService.Domain.Events;
using SubastaService.Infrastructure.Mongo;
using SubastaService.Infrastructure.MongoDB.Documents;

public class AuctionStateChangedConsumer : IConsumer<AuctionStateChangedEvent>
{
    private readonly MongoDbContext _mongo;

    public AuctionStateChangedConsumer(MongoDbContext mongo)
    {
        _mongo = mongo;
    }

    public async Task Consume(ConsumeContext<AuctionStateChangedEvent> context)
    {
        var mensaje = context.Message;

        var filter = Builders<SubastaDocument>.Filter.Eq(a => a.Id, mensaje.AuctionId);
        var update = Builders<SubastaDocument>.Update
            .Set(a => a.Estado, mensaje.NuevoEstado);

        await _mongo.Subastas.UpdateOneAsync(filter, update);
    }
}