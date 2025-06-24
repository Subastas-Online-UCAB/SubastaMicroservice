using MassTransit;
using MongoDB.Driver;
using SubastaService.Domain.Events;
using SubastaService.Infrastructure.MongoDB;
using SubastaService.Infrastructure.MongoDB.Documents;

namespace SubastaService.Infrastructure.Consumers
{
    public class SubastaEditadaConsumer : IConsumer<SubastaEditadaEvent>
    {
        private readonly ISubastaMongoContext _mongoContext;

        public SubastaEditadaConsumer(ISubastaMongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        public async Task Consume(ConsumeContext<SubastaEditadaEvent> context)
        {
            var evento = context.Message;

            var filter = Builders<SubastaDocument>.Filter.Eq(s => s.Id, evento.SubastaId);

            var updatedDocument = new SubastaDocument
            {
                Id = evento.SubastaId,
                Titulo = evento.Titulo,
                Descripcion = evento.Descripcion,
                PrecioBase = evento.PrecioBase,
                Duracion = evento.Duracion,
                CondicionParticipacion = evento.CondicionParticipacion,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaCierre,
                Estado = "Actualizada", // o conserva el valor actual si es necesario
                IncrementoMinimo = evento.IncrementoMinimo,
                PrecioReserva = evento.PrecioReserva,
                TipoSubasta = evento.TipoSubasta,
                IdProducto = evento.ProductoId,
                IdUsuario = evento.UsuarioId
            };

            await _mongoContext.Subastas.ReplaceOneAsync(
                filter,
                updatedDocument,
                new ReplaceOptions { IsUpsert = true } // por si aún no existe en Mongo
            );
        }
    }
}