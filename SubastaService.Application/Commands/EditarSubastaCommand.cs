using MediatR;
using SubastaService.Application.Comun;

namespace SubastaService.Application.Commands
{
    public record EditarSubastaCommand(
        Guid SubastaId,
        Guid UsuarioId,
        string Titulo,
        string Descripcion,
        DateTime FechaCierre,
        decimal PrecioBase,
        TimeSpan Duracion,
        string CondicionParticipacion,
        decimal IncrementoMinimo,
        decimal? PrecioReserva,
        string TipoSubasta,
        Guid ProductoId
    ) : IRequest<MessageResponse>;
}