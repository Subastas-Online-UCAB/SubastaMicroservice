using MediatR;
using SubastaService.Application.Queries;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Repositorios;

namespace SubastaService.Aplicacion.Handlers
{
    public class GetAllSubastasHandler : IRequestHandler<GetAllSubastasQuery, List<Subasta>>
    {
        private readonly IMongoAuctionRepository _repository;

        public GetAllSubastasHandler(IMongoAuctionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Subasta>> Handle(GetAllSubastasQuery request, CancellationToken cancellationToken)
        {
            var subastas = await _repository.ObtenerTodasAsync(cancellationToken);
            return subastas;
        }
    }
}