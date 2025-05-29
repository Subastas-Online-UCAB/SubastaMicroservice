using MediatR;
using SubastaService.Application.DTO;
using SubastaService.Application.Queries;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Repositorios;

namespace SubastaService.Application.Handlers
{
    public class ConsultarSubastaPorIdQueryHandler : IRequestHandler<ConsultarSubastaPorIdQuery, SubastaCompletaDto?>
    {
        private readonly IAuctionRepository _repository;

        public ConsultarSubastaPorIdQueryHandler(IAuctionRepository repository)
        {
            _repository = repository;
        }

        public async Task<SubastaCompletaDto?> Handle(ConsultarSubastaPorIdQuery request, CancellationToken cancellationToken)
        {
            var subasta = await _repository.ObtenerPorIdAsync(request.IdSubasta, cancellationToken);

            if (subasta == null)
                return null;

            return new SubastaCompletaDto
            {
                Id = subasta.IdSubasta,
                Titulo = subasta.Nombre,
                Descripcion = subasta.Descripcion,
                PrecioBase = subasta.PrecioBase,
                Duracion = subasta.Duracion,
                FechaInicio = subasta.FechaInicio,
                Estado = subasta.Estado,
                IncrementoMinimo = subasta.IncrementoMinimo,
                PrecioReserva = subasta.PrecioReserva,
                TipoSubasta = subasta.TipoSubasta,
                IdUsuario = subasta.IdUsuario,
                IdProducto = subasta.IdProducto,
                CondicionParticipacion = subasta.CondicionParticipacion
            };
        }

    }
}