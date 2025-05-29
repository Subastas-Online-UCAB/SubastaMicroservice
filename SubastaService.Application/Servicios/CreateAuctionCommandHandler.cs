using MediatR;
using SubastaService.Application.Commands;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Repositorios;
using SubastaService.Domain.Interfaces;
using SubastaService.Domain.Events;

namespace SubastaService.Application.Servicios
{
    public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _repository;
        private readonly IPublicadorSubastaEventos _publisher;

        public CreateAuctionCommandHandler(IAuctionRepository repository, IPublicadorSubastaEventos publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<Guid> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            var subasta = new Subasta
            {
                IdSubasta = Guid.NewGuid(),
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                PrecioBase = request.PrecioBase,
                Duracion = request.Duracion,
                CondicionParticipacion = request.CondicionParticipacion,
                FechaInicio = request.FechaInicio,
                Estado = "Pending",
                IncrementoMinimo = request.IncrementoMinimo,
                PrecioReserva = request.PrecioReserva,
                TipoSubasta = request.TipoSubasta,
                IdUsuario = request.IdUsuario,
                IdProducto = request.IdProducto
            };

            // 1. Persistencia en base de datos principal (PostgreSQL)
            await _repository.CrearAsync(subasta, cancellationToken);

            // 2. Publicar evento general (por ejemplo, para vistas o proyecciones)
            var eventoCreado = new SubastaCreadaEvent
            {
                Id = subasta.IdSubasta,
                Titulo = subasta.Nombre,
                Descripcion = subasta.Descripcion,
                PrecioBase = subasta.PrecioBase,
                Duracion = subasta.Duracion,
                CondicionParticipacion = subasta.CondicionParticipacion,
                FechaInicio = subasta.FechaInicio,
                FechaFin = subasta.FechaInicio + subasta.Duracion,
                Estado = "Pending",
                IncrementoMinimo = subasta.IncrementoMinimo,
                PrecioReserva = subasta.PrecioReserva,
                TipoSubasta = subasta.TipoSubasta,
                IdUsuario = subasta.IdUsuario,
                IdProducto = subasta.IdProducto
            };

            await _publisher.PublicarSubastaCreada(eventoCreado);

            // 3. Publicar evento de saga para activar máquina de estados
            await _publisher.PublicarAuctionStarted(new AuctionStarted
            {
                SubastaId = subasta.IdSubasta.ToString(),
                FechaInicio = subasta.FechaInicio
            });

            return subasta.IdSubasta;
        }
    }
}
