using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Events;
using SubastaService.Infrastructure.Mongo;
using SubastaService.Infrastructure.MongoDB.Documents;
using SubastaService.Infrastructure.Repositorios;
using UsuarioServicio.Infrastructure.Persistencia;
using SubastaService.Infrastructure.MongoDB;

namespace SubastaService.Tests.Repositorios
{
    public class AuctionRepositoryTests
    {
        [Fact]
        public async Task CancelarSubastaAsync_DeberiaActualizarEstadoYPublicarEvento()
        {
            // Arrange
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            var subasta = new Subasta
            {
                IdSubasta = idSubasta,
                Nombre = "Test",
                Descripcion = "Subasta de prueba",
                PrecioBase = 100,
                Duracion = TimeSpan.FromDays(5),
                CondicionParticipacion = "Ninguna",
                FechaInicio = DateTime.UtcNow,
                Estado = "Pending",
                IncrementoMinimo = 10,
                PrecioReserva = 150,
                TipoSubasta = "Estándar",
                IdUsuario = idUsuario,
                IdProducto = Guid.NewGuid()
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Evita colisiones
                .Options;

            await using var dbContext = new ApplicationDbContext(options);
            dbContext.Subastas.Add(subasta);
            await dbContext.SaveChangesAsync();

            // Mongo Mock
            var mockMongoCollection = new Mock<IMongoCollection<SubastaDocument>>();
            var mockMongoContext = new Mock<ISubastaMongoContext>();
            mockMongoContext.Setup(x => x.Subastas).Returns(mockMongoCollection.Object);

            // RabbitMQ Mock
            var mockPublisher = new Mock<IPublishEndpoint>();

            // Crear repositorio con mocks
            var repo = new AuctionRepository(dbContext, mockMongoContext.Object, mockPublisher.Object);

            // Act
            await repo.CancelarSubastaAsync(idSubasta, idUsuario, CancellationToken.None);

            // Assert - PostgreSQL actualizado
            var subastaActualizada = await dbContext.Subastas.FirstAsync(s => s.IdSubasta == idSubasta);
            subastaActualizada.Estado.Should().Be("Cancelada");

            // Assert - Mongo actualizado
            mockMongoCollection.Verify(m =>
                m.UpdateOneAsync(
                    It.IsAny<FilterDefinition<SubastaDocument>>(),
                    It.IsAny<UpdateDefinition<SubastaDocument>>(),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            // Assert - Evento publicado
            mockPublisher.Verify(p =>
                p.Publish(It.Is<AuctionCanceled>(e => e.SubastaId == idSubasta), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelarSubastaAsync_CuandoSubastaNoExiste_DeberiaLanzarExcepcion()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var dbContext = new ApplicationDbContext(options);

            var mockMongo = new Mock<ISubastaMongoContext>();
            var mockPublisher = new Mock<IPublishEndpoint>();

            var repo = new AuctionRepository(dbContext, mockMongo.Object, mockPublisher.Object);

            var idSubastaInexistente = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await repo.CancelarSubastaAsync(idSubastaInexistente, idUsuario, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Subasta no encontrada.");
        }

        [Fact]
        public async Task CancelarSubastaAsync_CuandoUsuarioNoEsPropietario_DeberiaLanzarUnauthorized()
        {
            // Arrange
            var idSubasta = Guid.NewGuid();
            var idPropietario = Guid.NewGuid();
            var idOtroUsuario = Guid.NewGuid();

            var subasta = new Subasta
            {
                IdSubasta = idSubasta,
                Nombre = "Test",
                Descripcion = "Subasta de prueba",
                PrecioBase = 100,
                Duracion = TimeSpan.FromDays(5),
                CondicionParticipacion = "Ninguna",
                FechaInicio = DateTime.UtcNow,
                Estado = "Pending",
                IncrementoMinimo = 10,
                PrecioReserva = 150,
                TipoSubasta = "Estándar",
                IdUsuario = idPropietario,
                IdProducto = Guid.NewGuid()
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var dbContext = new ApplicationDbContext(options);
            dbContext.Subastas.Add(subasta);
            await dbContext.SaveChangesAsync();

            var mockMongo = new Mock<ISubastaMongoContext>();
            var mockPublisher = new Mock<IPublishEndpoint>();

            var repo = new AuctionRepository(dbContext, mockMongo.Object, mockPublisher.Object);

            // Act
            Func<Task> act = async () => await repo.CancelarSubastaAsync(idSubasta, idOtroUsuario, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("No tienes permiso para cancelar esta subasta.");
        }

        [Fact]
        public async Task CrearAsync_DeberiaGuardarSubastaYRetornarId()
        {
            // Arrange
            var idSubasta = Guid.NewGuid();

            var subasta = new Subasta
            {
                IdSubasta = idSubasta,
                Nombre = "Subasta Test",
                Descripcion = "Una gran subasta",
                PrecioBase = 100,
                Duracion = TimeSpan.FromDays(7),
                CondicionParticipacion = "Abierta",
                FechaInicio = DateTime.UtcNow,
                Estado = "Pending",
                IncrementoMinimo = 10,
                PrecioReserva = 200,
                TipoSubasta = "Estándar",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid()
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var dbContext = new ApplicationDbContext(options);

            var mockMongo = new Mock<ISubastaMongoContext>();
            var mockPublisher = new Mock<IPublishEndpoint>();

            var repo = new AuctionRepository(dbContext, mockMongo.Object, mockPublisher.Object);

            // Act
            var resultId = await repo.CrearAsync(subasta, CancellationToken.None);

            // Assert
            resultId.Should().Be(idSubasta);

            var subastaEnDb = await dbContext.Subastas.FirstOrDefaultAsync(s => s.IdSubasta == idSubasta);
            subastaEnDb.Should().NotBeNull();
            subastaEnDb!.Nombre.Should().Be("Subasta Test");
        }

        [Fact]
        public async Task ObtenerSubastaCompletaPorIdAsync_CuandoExiste_DeberiaRetornarEntidadSubasta()
        {
            // Arrange
            var idSubasta = Guid.NewGuid();

            var documento = new SubastaDocument
            {
                Id = idSubasta,
                Titulo = "Subasta Mongo",
                Descripcion = "Desde MongoDB",
                PrecioBase = 300,
                Duracion = TimeSpan.FromDays(5),
                CondicionParticipacion = "Ninguna",
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddDays(5),
                Estado = "Pending",
                IncrementoMinimo = 20,
                PrecioReserva = 400,
                TipoSubasta = "Estándar",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid()
            };

            var mockMongoCollection = new Mock<IMongoCollection<SubastaDocument>>();
            var mockMongoContext = new Mock<ISubastaMongoContext>();
            var mockPublisher = new Mock<IPublishEndpoint>();

            var mockFind = new Mock<IAsyncCursor<SubastaDocument>>();
            mockFind.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                    .Returns(true).Returns(false);
            mockFind.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true).ReturnsAsync(false);
            mockFind.Setup(x => x.Current).Returns(new[] { documento });

            mockMongoCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<SubastaDocument>>(),
                    It.IsAny<FindOptions<SubastaDocument, SubastaDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockFind.Object);

            mockMongoContext.Setup(c => c.Subastas).Returns(mockMongoCollection.Object);

            var repo = new AuctionRepository(
                new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options),
                mockMongoContext.Object,
                mockPublisher.Object
            );

            // Act
            var resultado = await repo.ObtenerSubastaCompletaPorIdAsync(idSubasta, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.IdSubasta.Should().Be(idSubasta);
            resultado.Nombre.Should().Be("Subasta Mongo");
            resultado.Estado.Should().Be("Pending");
        }

        [Fact]
        public async Task ObtenerSubastaCompletaPorIdAsync_CuandoNoExiste_DeberiaRetornarNull()
        {
            // Arrange
            var mockMongoCollection = new Mock<IMongoCollection<SubastaDocument>>();
            var mockMongoContext = new Mock<ISubastaMongoContext>();
            var mockPublisher = new Mock<IPublishEndpoint>();

            var mockEmptyCursor = new Mock<IAsyncCursor<SubastaDocument>>();
            mockEmptyCursor.Setup(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(false);
            mockEmptyCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            mockMongoCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<SubastaDocument>>(),
                    It.IsAny<FindOptions<SubastaDocument, SubastaDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockEmptyCursor.Object);

            mockMongoContext.Setup(c => c.Subastas).Returns(mockMongoCollection.Object);

            var repo = new AuctionRepository(
                new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options),
                mockMongoContext.Object,
                mockPublisher.Object
            );

            // Act
            var resultado = await repo.ObtenerSubastaCompletaPorIdAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task ActualizarAsync_DeberiaActualizarYGuardarCambios()
        {
            // Arrange
            var subasta = new Subasta
            {
                IdSubasta = Guid.NewGuid(),
                Nombre = "Original",
                Descripcion = "Descripción de prueba",
                PrecioBase = 100,
                Duracion = TimeSpan.FromHours(2),
                CondicionParticipacion = "Abierta",
                FechaInicio = DateTime.UtcNow,
                Estado = "Pending",
                IncrementoMinimo = 10,
                PrecioReserva = 200,
                TipoSubasta = "Estándar",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid()
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var dbContext = new ApplicationDbContext(options);
            dbContext.Subastas.Add(subasta);
            await dbContext.SaveChangesAsync();

            var mockMongo = new Mock<ISubastaMongoContext>();
            var mockPublisher = new Mock<IPublishEndpoint>();
            var repo = new AuctionRepository(dbContext, mockMongo.Object, mockPublisher.Object);

            // Act
            subasta.Nombre = "Actualizado desde test";
            await repo.ActualizarAsync(subasta);

            // Assert
            var actualizado = await dbContext.Subastas.FirstOrDefaultAsync(s => s.IdSubasta == subasta.IdSubasta);
            actualizado!.Nombre.Should().Be("Actualizado desde test");
        }

        [Fact]
        public async Task ObtenerPorIdAsync_SinCancellationToken_DeberiaRetornarSubasta()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // mismo contexto para guardar y leer
                .Options;

            var subasta = new Subasta

            {
                IdSubasta = Guid.NewGuid(),
                Nombre = "Subasta test",
                Descripcion = "Descripción",
                PrecioBase = 100,
                Duracion = TimeSpan.FromDays(2),
                CondicionParticipacion = "Participantes verificados",
                FechaInicio = DateTime.UtcNow,
                Estado = "Pending",
                IncrementoMinimo = 5,
                PrecioReserva = 150,
                TipoSubasta = "Pública",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid()
            };

            // Se guarda y se usa el mismo contexto después
            await using var dbContext = new ApplicationDbContext(options);
            dbContext.Subastas.Add(subasta);
            await dbContext.SaveChangesAsync();

            var mockMongo = new Mock<ISubastaMongoContext>();
            var mockPublisher = new Mock<IPublishEndpoint>();
            var repo = new AuctionRepository(dbContext, mockMongo.Object, mockPublisher.Object);

            // Act
            var resultado = await repo.ObtenerPorIdAsync(subasta.IdSubasta);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.IdSubasta.Should().Be(subasta.IdSubasta);
        }


    }
}
