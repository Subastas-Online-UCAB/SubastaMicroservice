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

            var documentoActual = await _mongoContext.Subastas
                .Find(filter)
                .FirstOrDefaultAsync();

            if (documentoActual == null)
            {
                // Si no existe aún, puedes asignar estado inicial 'Pending'
                documentoActual = new SubastaDocument
                {
                    Id = evento.SubastaId,
                    Estado = "Pending" // o cualquier estado por defecto
                };
            }

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
                IncrementoMinimo = evento.IncrementoMinimo,
                PrecioReserva = evento.PrecioReserva,
                TipoSubasta = evento.TipoSubasta,
                IdProducto = evento.ProductoId,
                IdUsuario = evento.UsuarioId,

                // 👇 Conservamos el estado actual
                Estado = documentoActual.Estado
            };

            await _mongoContext.Subastas.ReplaceOneAsync(
                filter,
                updatedDocument,
                new ReplaceOptions { IsUpsert = true }
            );
        }
    }
}
