using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaService.Application.Comun;

namespace SubastaService.Application.Commands
{
    public record EditarSubastaCommand(
        Guid SubastaId,
        Guid UsuarioId,
        string Titulo,
        string Descripcion,
        DateTime FechaCierre
    ) : IRequest<MessageResponse>;
}
