using Xunit;
using Moq;
using FluentAssertions;
using MassTransit;
using System;
using System.Threading.Tasks;
using SubastaService.Domain.Events;
using SubastaService.Domain.Interfaces;
using SubastaService.Infrastructure.EventPublishers;

namespace SubastaService.Tests.EventPublishers
{
    public class PublicadorSubastaEventosTests
    {
        [Fact]
        public async Task PublicarSubastaCreada_DeberiaLlamarPublish()
        {
            // Arrange
            var mockEndpoint = new Mock<IPublishEndpoint>();
            var publisher = new PublicadorSubastaEventos(mockEndpoint.Object);

            var evento = new SubastaCreadaEvent
            {
                Id = Guid.NewGuid(),
                Titulo = "Nueva Subasta",
                PrecioBase = 100,
                FechaInicio = DateTime.UtcNow,
                Duracion = TimeSpan.FromDays(3),
                Estado = "Pending",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid()
            };

            // Act
            await publisher.PublicarSubastaCreada(evento);

            // Assert
            mockEndpoint.Verify(p => p.Publish(evento, default), Times.Once);
        }


        [Fact]
        public async Task PublicarAuctionStarted_DeberiaLlamarPublish()
        {
            // Arrange
            var mockEndpoint = new Mock<IPublishEndpoint>();
            var publisher = new PublicadorSubastaEventos(mockEndpoint.Object);

            var evento = new AuctionStarted
            {
                SubastaId = Guid.NewGuid().ToString(),
                FechaInicio = DateTime.UtcNow
            };

            // Act
            await publisher.PublicarAuctionStarted(evento);

            // Assert
            mockEndpoint.Verify(p => p.Publish(evento, default), Times.Once);
        }

        [Fact]
        public async Task PublicarSubastaEditada_DeberiaLlamarPublish()
        {
            // Arrange
            var mockEndpoint = new Mock<IPublishEndpoint>();
            var publisher = new PublicadorSubastaEventos(mockEndpoint.Object);

            var evento = new SubastaEditadaEvent
            {
                SubastaId = Guid.NewGuid(),
                Titulo = "Editada"
            };

            // Act
            await publisher.PublicarSubastaEditada(evento);

            // Assert
            mockEndpoint.Verify(p => p.Publish(evento, default), Times.Once);
        }
    }
}
