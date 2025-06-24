using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using SubastaService.Application.Commands;
using SubastaService.Application.Comun;
using SubastaService.Application.Servicios;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Events;
using SubastaService.Domain.Interfaces;
using SubastaService.Domain.Repositorios;

namespace SubastaService.Tests.Handlers
{
    public class EditarSubastaHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsSuccess_WhenSubastaIsValidAndEditable()
        {
            // Arrange
            var subastaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var productoId = Guid.NewGuid();

            var subasta = new Subasta
            {
                IdSubasta = subastaId,
                IdUsuario = usuarioId,
                Estado = EstadoSubasta.Pending.ToString(),
                FechaInicio = DateTime.UtcNow
            };

            var mockRepo = new Mock<IAuctionRepository>();
            var mockPublisher = new Mock<IPublicadorSubastaEventos>();

            mockRepo
                .Setup(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subasta);

            mockRepo
                .Setup(r => r.ActualizarAsync(It.IsAny<Subasta>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mockPublisher
                .Setup(p => p.PublicarSubastaEditada(It.IsAny<SubastaEditadaEvent>()))
                .Returns(Task.CompletedTask);

            var handler = new EditarSubastaHandler(mockRepo.Object, mockPublisher.Object);

            var command = new EditarSubastaCommand(
                SubastaId: subastaId,
                UsuarioId: usuarioId,
                Titulo: "Nueva subasta",
                Descripcion: "Descripción actualizada",
                FechaCierre: DateTime.UtcNow.AddDays(3),
                PrecioBase: 100,
                Duracion: TimeSpan.FromHours(2),
                CondicionParticipacion: "Libre",
                IncrementoMinimo: 10,
                PrecioReserva: 200,
                TipoSubasta: "Pública",
                ProductoId: productoId
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Subasta editada exitosamente.", result.Message);

            mockRepo.Verify(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()), Times.Once);
            mockRepo.Verify(r => r.ActualizarAsync(subasta, It.IsAny<CancellationToken>()), Times.Once);
            mockPublisher.Verify(p => p.PublicarSubastaEditada(It.IsAny<SubastaEditadaEvent>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ReturnsError_WhenUsuarioNoEsPropietario()
        {
            // Arrange
            var subastaId = Guid.NewGuid();
            var usuarioPropietario = Guid.NewGuid();
            var usuarioInvalido = Guid.NewGuid(); // <-- otro usuario

            var subasta = new Subasta
            {
                IdSubasta = subastaId,
                IdUsuario = usuarioPropietario,
                Estado = EstadoSubasta.Pending.ToString(),
                FechaInicio = DateTime.UtcNow
            };

            var mockRepo = new Mock<IAuctionRepository>();
            var mockPublisher = new Mock<IPublicadorSubastaEventos>();

            mockRepo
                .Setup(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subasta);

            var handler = new EditarSubastaHandler(mockRepo.Object, mockPublisher.Object);

            var command = new EditarSubastaCommand(
                SubastaId: subastaId,
                UsuarioId: usuarioInvalido, // <-- este no es el dueño
                Titulo: "Hackeo",
                Descripcion: "Intento de editar",
                FechaCierre: DateTime.UtcNow.AddDays(1),
                PrecioBase: 1,
                Duracion: TimeSpan.FromMinutes(10),
                CondicionParticipacion: "Ninguna",
                IncrementoMinimo: 1,
                PrecioReserva: 1,
                TipoSubasta: "Privada",
                ProductoId: Guid.NewGuid()
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No tienes permiso para editar esta subasta.", result.Message);

            mockRepo.Verify(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()), Times.Once);
            mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Subasta>(), It.IsAny<CancellationToken>()), Times.Never);
            mockPublisher.Verify(p => p.PublicarSubastaEditada(It.IsAny<SubastaEditadaEvent>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenSubastaNoEstaEnEstadoPending()
        {
            // Arrange
            var subastaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            var subasta = new Subasta
            {
                IdSubasta = subastaId,
                IdUsuario = usuarioId,
                Estado = EstadoSubasta.Activa.ToString(), // ❌ Ya no está en estado válido para editar
                FechaInicio = DateTime.UtcNow
            };

            var mockRepo = new Mock<IAuctionRepository>();
            var mockPublisher = new Mock<IPublicadorSubastaEventos>();

            mockRepo
                .Setup(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subasta);

            var handler = new EditarSubastaHandler(mockRepo.Object, mockPublisher.Object);

            var command = new EditarSubastaCommand(
                SubastaId: subastaId,
                UsuarioId: usuarioId,
                Titulo: "Intento fuera de tiempo",
                Descripcion: "Ya está activa",
                FechaCierre: DateTime.UtcNow.AddDays(2),
                PrecioBase: 100,
                Duracion: TimeSpan.FromHours(1),
                CondicionParticipacion: "Libre",
                IncrementoMinimo: 5,
                PrecioReserva: 150,
                TipoSubasta: "Pública",
                ProductoId: Guid.NewGuid()
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Solo puedes editar subastas que aún no han iniciado.", result.Message);

            mockRepo.Verify(r => r.ObtenerPorIdAsync(subastaId, It.IsAny<CancellationToken>()), Times.Once);
            mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Subasta>(), It.IsAny<CancellationToken>()), Times.Never);
            mockPublisher.Verify(p => p.PublicarSubastaEditada(It.IsAny<SubastaEditadaEvent>()), Times.Never);
        }


    }
}
