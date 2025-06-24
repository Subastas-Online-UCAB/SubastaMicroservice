using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SubastaService.Aplicacion.Handlers;
using SubastaService.Application.Queries;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Repositorios;

namespace SubastaService.Tests.Handlers
{
    public class GetAllSubastasHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsListOfSubastas_WhenDataExists()
        {
            // Arrange
            var mockRepo = new Mock<IMongoAuctionRepository>();

            var subastasMock = new List<Subasta>
            {
                new Subasta
                {
                    IdSubasta = Guid.NewGuid(),
                    Nombre = "Subasta 1",
                    Descripcion = "Ejemplo de prueba",
                    PrecioBase = 100,
                    Duracion = TimeSpan.FromHours(1),
                    CondicionParticipacion = "Libre",
                    FechaInicio = DateTime.UtcNow,
                    Estado = "Pending",
                    IncrementoMinimo = 5,
                    PrecioReserva = 150,
                    TipoSubasta = "Pública",
                    IdUsuario = Guid.NewGuid(),
                    IdProducto = Guid.NewGuid()
                }
            };

            mockRepo.Setup(r => r.ObtenerTodasAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(subastasMock);

            var handler = new GetAllSubastasHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new GetAllSubastasQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Subasta 1", result[0].Nombre);
            mockRepo.Verify(r => r.ObtenerTodasAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}