using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using SubastaService.Application.Commands;
using SubastaService.Application.Servicios;
using SubastaService.Domain.Interfaces;
using SubastaService.Domain.Repositorios;
using SubastaService.Domain.Events;

namespace SubastaService.Tests.Handlers
{
    public class CreateAuctionCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeberiaCrearSubastaYPublicarEventos()
        {
            // Arrange
            var mockRepo = new Mock<IAuctionRepository>();
            var mockPublisher = new Mock<IPublicadorSubastaEventos>();

            var handler = new CreateAuctionCommandHandler(mockRepo.Object, mockPublisher.Object);

            var command = new CreateAuctionCommand
            {
                Nombre = "PlayStation 5",
                Descripcion = "Subasta especial de consola",
                PrecioBase = 500,
                Duracion = TimeSpan.FromDays(5),
                CondicionParticipacion = "Pago anticipado",
                FechaInicio = DateTime.UtcNow,
                IncrementoMinimo = 10,
                PrecioReserva = 600,
                TipoSubasta = "Estándar",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid()
            };

            // Act
            var resultado = await handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().NotBeEmpty(); // Verifica que el Guid fue generado

            mockRepo.Verify(r => r.CrearAsync(It.IsAny<Domain.Entidades.Subasta>(), It.IsAny<CancellationToken>()), Times.Once);

            mockPublisher.Verify(p => p.PublicarSubastaCreada(It.Is<SubastaCreadaEvent>(e =>
                e.Titulo == command.Nombre &&
                e.Descripcion == command.Descripcion &&
                e.PrecioBase == command.PrecioBase &&
                e.Estado == "Pending" &&
                e.IdUsuario == command.IdUsuario
            )), Times.Once);

            mockPublisher.Verify(p => p.PublicarAuctionStarted(It.Is<AuctionStarted>(e =>
                e.SubastaId != null &&
                e.FechaInicio == command.FechaInicio
            )), Times.Once);
        }
    }
}
