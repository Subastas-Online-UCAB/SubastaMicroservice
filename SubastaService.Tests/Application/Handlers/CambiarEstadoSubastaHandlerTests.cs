using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using MassTransit;
using SubastaService.Aplicacion.Handlers;

using SubastaService.Application.Commands;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Events;
using SubastaService.Domain.Excepciones;
using SubastaService.Domain.Repositorios;

namespace SubastaService.Tests.Handlers
{
    public class CambiarEstadoSubastaHandlerTests
    {
        private Subasta CrearSubastaMock(string estado = "Pending", string? idUsuario = null)
        {
            return new Subasta
            {
                IdSubasta = Guid.NewGuid(),
                Estado = estado,
                IdUsuario = Guid.Parse(idUsuario ?? "3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Nombre = "Subasta de prueba",
                Descripcion = "Mock",
                PrecioBase = 100,
                Duracion = TimeSpan.FromMinutes(60), // ✅ convierte el número a TimeSpan correctamente
                CondicionParticipacion = "Libre",
                FechaInicio = DateTime.UtcNow,
                IncrementoMinimo = 5,
                PrecioReserva = 150,
                TipoSubasta = "Publica",
                IdProducto = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_DeberiaCambiarEstadoDePendingAActive_SiTodoEsValido()
        {
            // Arrange
            var subasta = CrearSubastaMock("Pending");
            var mockRepo = new Mock<IAuctionRepository>();
            var mockBus = new Mock<IPublishEndpoint>();

            mockRepo.Setup(r => r.ObtenerPorIdAsync(subasta.IdSubasta, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(subasta);
            mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Subasta>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            mockBus.Setup(b => b.Publish(It.IsAny<AuctionStarted>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

            var handler = new CambiarEstadoSubastaHandler(mockRepo.Object, mockBus.Object);

            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = subasta.IdSubasta,
                NuevoEstado = "Active",
                IdUsuario = subasta.IdUsuario.ToString()
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            mockRepo.Verify(r => r.ActualizarAsync(It.Is<Subasta>(s => s.Estado == "Active"), It.IsAny<CancellationToken>()), Times.Once);
            mockBus.Verify(b => b.Publish(It.IsAny<AuctionStarted>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeberiaLanzar_SubastaNoEncontradaException()
        {
            // Arrange
            var mockRepo = new Mock<IAuctionRepository>();
            var mockBus = new Mock<IPublishEndpoint>();

            mockRepo.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Subasta)null);

            var handler = new CambiarEstadoSubastaHandler(mockRepo.Object, mockBus.Object);

            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = Guid.NewGuid(),
                NuevoEstado = "Active",
                IdUsuario = Guid.NewGuid().ToString()
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<SubastaNoEncontradaException>();
        }

        [Fact]
        public async Task Handle_DeberiaLanzar_UsuarioSinPermisoException_SiUsuarioNoCoincide()
        {
            // Arrange
            var subasta = CrearSubastaMock("Pending", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var mockRepo = new Mock<IAuctionRepository>();
            var mockBus = new Mock<IPublishEndpoint>();

            mockRepo.Setup(r => r.ObtenerPorIdAsync(subasta.IdSubasta, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(subasta);

            var handler = new CambiarEstadoSubastaHandler(mockRepo.Object, mockBus.Object);

            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = subasta.IdSubasta,
                NuevoEstado = "Active",
                IdUsuario = Guid.NewGuid().ToString()
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UsuarioSinPermisoException>();
        }

        [Fact]
        public async Task Handle_DeberiaLanzar_TransicionInvalidaException_SiTransicionNoEsValida()
        {
            // Arrange
            var subasta = CrearSubastaMock("Pending");
            var mockRepo = new Mock<IAuctionRepository>();
            var mockBus = new Mock<IPublishEndpoint>();

            mockRepo.Setup(r => r.ObtenerPorIdAsync(subasta.IdSubasta, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(subasta);

            var handler = new CambiarEstadoSubastaHandler(mockRepo.Object, mockBus.Object);

            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = subasta.IdSubasta,
                NuevoEstado = "Pagada",
                IdUsuario = subasta.IdUsuario.ToString()
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<TransicionInvalidaException>();
        }

        [Fact]
        public async Task Handle_DeberiaPublicarAuctionEnded_CuandoNuevoEstadoEsEnded()
        {
            // Arrange
            var subasta = CrearSubastaMock("Active");
            var mockRepo = new Mock<IAuctionRepository>();
            var mockBus = new Mock<IPublishEndpoint>();

            mockRepo.Setup(r => r.ObtenerPorIdAsync(subasta.IdSubasta, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subasta);
            mockRepo.Setup(r => r.ActualizarAsync(subasta, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mockBus.Setup(b => b.Publish(It.IsAny<AuctionEnded>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new CambiarEstadoSubastaHandler(mockRepo.Object, mockBus.Object);

            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = subasta.IdSubasta,
                NuevoEstado = "Ended",
                IdUsuario = subasta.IdUsuario.ToString()
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            mockBus.Verify(b => b.Publish(It.IsAny<AuctionEnded>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeberiaPublicarPaymentReceived_CuandoNuevoEstadoEsPagada()
        {
            var subasta = CrearSubastaMock("Ended");
            var mockRepo = new Mock<IAuctionRepository>();
            var mockBus = new Mock<IPublishEndpoint>();

            mockRepo.Setup(r => r.ObtenerPorIdAsync(subasta.IdSubasta, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subasta);
            mockRepo.Setup(r => r.ActualizarAsync(subasta, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mockBus.Setup(b => b.Publish(It.IsAny<PaymentReceived>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new CambiarEstadoSubastaHandler(mockRepo.Object, mockBus.Object);

            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = subasta.IdSubasta,
                NuevoEstado = "Pagada",
                IdUsuario = subasta.IdUsuario.ToString()
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
            mockBus.Verify(b => b.Publish(It.IsAny<PaymentReceived>(), It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
