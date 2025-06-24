using MongoDB.Driver;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Repositorios;

using SubastaService.Infrastructure.MongoDB;
using SubastaService.Infrastructure.MongoDB.Documents;

namespace SubastaService.Infraestructura.Repositorios
{
    public class MongoAuctionRepository : IMongoAuctionRepository
    {
        private readonly IMongoCollection<SubastaDocument> _collection;

        public MongoAuctionRepository(ISubastaMongoContext context)
        {
            _collection = context.Subastas;
        }

         public async Task<List<Subasta>> ObtenerTodasAsync(CancellationToken cancellationToken)
        {
            var documentos = await _collection.Find(_ => true).ToListAsync(cancellationToken);

            return documentos.Select(doc => new Subasta
            {
                IdSubasta = doc.Id,
                Nombre = doc.Titulo,
                Descripcion = doc.Descripcion,
                PrecioBase = doc.PrecioBase,
                Duracion = doc.Duracion,
                CondicionParticipacion = doc.CondicionParticipacion,
                FechaInicio = doc.FechaInicio,
                Estado = doc.Estado,
                IncrementoMinimo = doc.IncrementoMinimo,
                PrecioReserva = doc.PrecioReserva,
                TipoSubasta = doc.TipoSubasta,
                IdUsuario = doc.IdUsuario,
                IdProducto = doc.IdProducto
            }).ToList();

        }
    }
}