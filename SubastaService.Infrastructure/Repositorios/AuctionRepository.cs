using SubastaService.Domain.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsuarioServicio.Infrastructure.Persistencia;
using SubastaService.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SubastaService.Domain.Events;
using SubastaService.Infrastructure.MongoDB.Documents;
using MassTransit;
using SubastaService.Infrastructure.Mongo;
using SubastaService.Application.DTO;
using SubastaService.Infrastructure.MongoDB;

namespace SubastaService.Infrastructure.Repositorios
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ISubastaMongoContext _mongoContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionRepository(ApplicationDbContext context, ISubastaMongoContext mongoContext, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mongoContext = mongoContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Guid> CrearAsync(Subasta subasta, CancellationToken cancellationToken)
        {
            _context.Subastas.Add(subasta);
            await _context.SaveChangesAsync(cancellationToken);
            return subasta.IdSubasta;
        }

        public async Task<Subasta?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Subastas.FirstOrDefaultAsync(s => s.IdSubasta == id);
        }

        public async Task ActualizarAsync(Subasta subasta)
        {
            _context.Subastas.Update(subasta);
            await _context.SaveChangesAsync();
        }

        public async Task<Subasta?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Subastas
                .FirstOrDefaultAsync(s => s.IdSubasta == id, cancellationToken);
        }

        public async Task ActualizarAsync(Subasta subasta, CancellationToken cancellationToken)
        {
            _context.Subastas.Update(subasta);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CancelarSubastaAsync(Guid idSubasta, Guid idUsuario, CancellationToken cancellationToken)
        {
            var subasta = await _context.Subastas.FirstOrDefaultAsync(s => s.IdSubasta == idSubasta, cancellationToken);
            if (subasta is null)
                throw new Exception("Subasta no encontrada.");

            if (subasta.IdUsuario != idUsuario)
                throw new UnauthorizedAccessException("No tienes permiso para cancelar esta subasta.");

            if (subasta.Estado != "Pending")
                throw new InvalidOperationException("Solo se pueden cancelar subastas que están en estado pendiente.");


            // PostgreSQL
            subasta.Estado = "Cancelada";
            _context.Subastas.Update(subasta);
            await _context.SaveChangesAsync(cancellationToken);

            // MongoDB
            var filter = Builders<SubastaDocument>.Filter.Eq(s => s.Id, idSubasta);
            var update = Builders<SubastaDocument>.Update.Set(s => s.Estado, "Cancelada");
            await _mongoContext.Subastas.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            // Saga (máquina de estados)
            await _publishEndpoint.Publish(new AuctionCanceled
            {
                SubastaId = idSubasta
            });
        }

        public async Task<Subasta?> ObtenerSubastaCompletaPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var doc = await _mongoContext.Subastas
                .Find(s => s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (doc is null) return null;

            return new Subasta
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
            };
        }


    }
}