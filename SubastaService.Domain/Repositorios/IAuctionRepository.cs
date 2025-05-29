using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaService.Domain.Entidades;

namespace SubastaService.Domain.Repositorios
{
    public interface IAuctionRepository
    {
        Task<Guid> CrearAsync(Subasta subasta, CancellationToken cancellationToken);

        Task<Subasta?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task ActualizarAsync(Subasta subasta, CancellationToken cancellationToken);
        Task CancelarSubastaAsync(Guid idSubasta, Guid idUsuario, CancellationToken cancellationToken);

        Task<Subasta?> ObtenerSubastaCompletaPorIdAsync(Guid id, CancellationToken cancellationToken);



    }
}
