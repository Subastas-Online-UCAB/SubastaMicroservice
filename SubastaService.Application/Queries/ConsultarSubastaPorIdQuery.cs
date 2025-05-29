using MediatR;
using SubastaService.Application.DTO;
using SubastaService.Domain.Entidades;


namespace SubastaService.Application.Queries
{
    public class ConsultarSubastaPorIdQuery : IRequest<SubastaCompletaDto?>
    {
        public Guid IdSubasta { get; }

        public ConsultarSubastaPorIdQuery(Guid idSubasta)
        {
            IdSubasta = idSubasta;
        }
    }
}