using MediatR;
using SubastaService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaService.Domain.Events;
using SubastaService.Application.Comun;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Repositorios;
using SubastaService.Domain.Interfaces;

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
            var subasta = await _subastaRepository.ObtenerPorIdAsync(request.SubastaId, cancellationToken);
            if (subasta == null)
                return MessageResponse.CrearError("La subasta no existe.");

            if (subasta.IdUsuario != request.UsuarioId)
                return MessageResponse.CrearError("No tienes permiso para editar esta subasta.");

            if (subasta.Estado != EstadoSubasta.Borrador.ToString())
                return MessageResponse.CrearError("Solo puedes editar subastas que aún no han iniciado.");

            subasta.Editar(request.Titulo, request.Descripcion, request.FechaCierre);
            await _subastaRepository.ActualizarAsync(subasta, cancellationToken);

            await _eventPublisher.PublicarSubastaEditada( new SubastaEditadaEvent
            {
                SubastaId = subasta.IdSubasta,
                Titulo = subasta.Nombre,
                Descripcion = subasta.Descripcion,
                //FechaCierre = subasta.FechaCierre
            });

            return MessageResponse.CrearExito("Subasta editada exitosamente.");
        }
    }
}
