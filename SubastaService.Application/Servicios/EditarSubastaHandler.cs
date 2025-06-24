using MediatR;
using SubastaService.Application.Commands;
using SubastaService.Application.Comun;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Events;
using SubastaService.Domain.Interfaces;
using SubastaService.Domain.Repositorios;

namespace SubastaService.Application.Servicios
{
    public class EditarSubastaHandler : IRequestHandler<EditarSubastaCommand, MessageResponse>
    {
        private readonly IAuctionRepository _subastaRepository;
        private readonly IPublicadorSubastaEventos _eventPublisher;

        public EditarSubastaHandler(IAuctionRepository subastaRepository, IPublicadorSubastaEventos eventPublisher)
        {
            _subastaRepository = subastaRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<MessageResponse> Handle(EditarSubastaCommand request, CancellationToken cancellationToken)
        {
            // Buscar la subasta
            var subasta = await _subastaRepository.ObtenerPorIdAsync(request.SubastaId, cancellationToken);
            if (subasta == null)
                return MessageResponse.CrearError("La subasta no existe.");

            // Validar que el usuario sea dueño de la subasta
            if (subasta.IdUsuario != request.UsuarioId)
                return MessageResponse.CrearError("No tienes permiso para editar esta subasta.");

            // Solo se puede editar si está en estado Borrador
            if (subasta.Estado != EstadoSubasta.Pending.ToString())
                return MessageResponse.CrearError("Solo puedes editar subastas que aún no han iniciado.");

            // Aplicar los cambios
            subasta.Editar(
                request.Titulo,
                request.Descripcion,
                request.FechaCierre,
                request.PrecioBase,
                request.Duracion,
                request.CondicionParticipacion,
                request.IncrementoMinimo,
                request.PrecioReserva,
                request.TipoSubasta,
                request.ProductoId
            );

            // Persistir cambios
            await _subastaRepository.ActualizarAsync(subasta, cancellationToken);

            // Publicar evento si es necesario
            await _eventPublisher.PublicarSubastaEditada(new SubastaEditadaEvent
            {
                SubastaId = subasta.IdSubasta,
                Titulo = subasta.Nombre,
                Descripcion = subasta.Descripcion,
                FechaInicio = subasta.FechaInicio,
                FechaCierre = subasta.FechaInicio + subasta.Duracion,
                PrecioBase = subasta.PrecioBase,
                Duracion = subasta.Duracion,
                CondicionParticipacion = subasta.CondicionParticipacion,
                IncrementoMinimo = subasta.IncrementoMinimo,
                PrecioReserva = subasta.PrecioReserva,
                TipoSubasta = subasta.TipoSubasta,
                ProductoId = subasta.IdProducto,
                UsuarioId = subasta.IdUsuario
            });

            return MessageResponse.CrearExito("Subasta editada exitosamente.");
        }
    }
}
