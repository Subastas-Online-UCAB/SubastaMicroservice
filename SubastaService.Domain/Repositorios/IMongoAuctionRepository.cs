using SubastaService.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Repositorios
{
    public interface IMongoAuctionRepository
    {
        Task<List<Subasta>> ObtenerTodasAsync(CancellationToken cancellationToken);
    }
}
