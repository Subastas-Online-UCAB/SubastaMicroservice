using MassTransit;
using SubastaService.Domain.Events;
using SubastaService.Infrastructure.Mongo;
using SubastaService.Infrastructure.MongoDB;
using SubastaService.Infrastructure.MongoDB.Documents;

namespace SubastaService.Infrastructure.Consumers
{
    public class SubastaCreadaConsumer : IConsumer<SubastaCreadaEvent>
    {
        private readonly ISubastaMongoContext _context;

        public SubastaCreadaConsumer(ISubastaMongoContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<SubastaCreadaEvent> context)
        {
            var mensaje = context.Message;

            var documento = new SubastaDocument
            {
                Id = mensaje.Id,
                Titulo = mensaje.Titulo,
                Descripcion = mensaje.Descripcion,
                PrecioBase = mensaje.PrecioBase,
                Duracion = mensaje.Duracion,
                CondicionParticipacion = mensaje.CondicionParticipacion,
                FechaInicio = mensaje.FechaInicio,
                FechaFin = mensaje.FechaFin,
                Estado = mensaje.Estado,
                IncrementoMinimo = mensaje.IncrementoMinimo,
                PrecioReserva = mensaje.PrecioReserva,
                TipoSubasta = mensaje.TipoSubasta,
                IdUsuario = mensaje.IdUsuario,
                IdProducto = mensaje.IdProducto
            };

            await _context.Subastas.InsertOneAsync(documento);
        }
    }
}