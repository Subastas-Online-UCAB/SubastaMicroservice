using MediatR;
using SubastaService.Application.Commands;
using SubastaService.Domain.Events;
using SubastaService.Domain.Repositorios;

using MassTransit;

namespace SubastaService.Application.Handlers
{
    public class EliminarSubastaCommandHandler : IRequestHandler<EliminarSubastaCommand, bool>
    {
        private readonly IAuctionRepository _repository;

        public EliminarSubastaCommandHandler(IAuctionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(EliminarSubastaCommand request, CancellationToken cancellationToken)
        {
            await _repository.CancelarSubastaAsync(request.IdSubasta, request.IdUsuario, cancellationToken);
            return true;
        }
    }
}
