using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Application.Commands
{
    public class CambiarEstadoSubastaCommand : IRequest<bool>
    {
        public Guid SubastaId { get; set; }
        public string NuevoEstado { get; set; } = string.Empty;
        public string IdUsuario { get; set; } = string.Empty;
    }
}
