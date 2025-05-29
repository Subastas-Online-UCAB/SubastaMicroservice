using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Application.Commands
{
    public class EliminarSubastaCommand : IRequest<bool>
    {
        public Guid IdSubasta { get; set; }
        public Guid IdUsuario { get; set; }
    }
}
