using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Repositorios;
using SubastaService.Application.Handlers;
using SubastaService.Application.Queries;
using SubastaService.Application.DTO;

namespace SubastaService.Tests.Handlers
{
    public class ConsultarSubastaPorIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_CuandoSubastaExiste_DeberiaRetornarDtoCompleto()
        {
            // Arrange
            var mockRepo = new Mock<IAuctionRepository>();
            var handler = new ConsultarSubastaPorIdQueryHandler(mockRepo.Object);

            var subastaId = Guid.NewGuid();

            var subastaFake = new Subasta
            {
                IdSubasta = subastaId,
                Nombre = "Subasta Test",
                Descripcion = "Descripción",
                PrecioBase = 500,
                Duracion = TimeSpan.FromDays(5),
                FechaInicio = DateTime.UtcNow,
                Estado = "Pending",
                IncrementoMinimo = 10,
                PrecioReserva = 600,
                TipoSubasta = "Estándar",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid(),
                CondicionParticipacion = "Condición X"
            };

            mockRepo
                .Setup(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subastaFake);

            var query = new ConsultarSubastaPorIdQuery(subastaId);

            // Act
            var resultado = await handler.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(subastaFake.IdSubasta);
            resultado.Titulo.Should().Be(subastaFake.Nombre);
            resultado.Estado.Should().Be("Pending");

            mockRepo.Verify(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CuandoSubastaNoExiste_DeberiaRetornarNull()
        {
            // Arrange
            var mockRepo = new Mock<IAuctionRepository>();
            var handler = new ConsultarSubastaPorIdQueryHandler(mockRepo.Object);

            var idInexistente = Guid.NewGuid();
            var query = new ConsultarSubastaPorIdQuery(idInexistente);

            mockRepo
                .Setup(r => r.ObtenerPorIdAsync(idInexistente, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Subasta?)null);

            // Act
            var resultado = await handler.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().BeNull();

            mockRepo.Verify(r => r.ObtenerPorIdAsync(idInexistente, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
