using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaService.Application.DTO;
using SubastaService.Domain.Entidades;

namespace SubastaService.Application.Queries
{
    public class GetAllSubastasQuery : IRequest<List<Subasta>> { }

}
