using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using MassTransit;
using MongoDB.Driver;
using SubastaService.Domain.Events;
using SubastaService.Infrastructure.Consumers;
using SubastaService.Infrastructure.Mongo;
using SubastaService.Infrastructure.MongoDB.Documents;
using SubastaService.Infrastructure.MongoDB;

namespace SubastaService.Tests.Consumers
{
    public class SubastaCreadaConsumerTests
    {
        [Fact]
        public async Task Consume_CuandoRecibeEvento_DeberiaInsertarDocumentoEnMongo()
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<SubastaDocument>>();
            var mockContext = new Mock<ISubastaMongoContext>();

            mockContext.Setup(c => c.Subastas).Returns(mockCollection.Object);

            var consumer = new SubastaCreadaConsumer(mockContext.Object);

            var evento = new SubastaCreadaEvent
            {
                Id = Guid.NewGuid(),
                Titulo = "Subasta test",
                Descripcion = "Descripción",
                PrecioBase = 100,
                Duracion = TimeSpan.FromDays(3),
                CondicionParticipacion = "Abierta",
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddDays(3),
                Estado = "Pending",
                IncrementoMinimo = 10,
                PrecioReserva = 150,
                TipoSubasta = "Estándar",
                IdUsuario = Guid.NewGuid(),
                IdProducto = Guid.NewGuid()
            };

            var mockConsumeContext = new Mock<ConsumeContext<SubastaCreadaEvent>>();
            mockConsumeContext.Setup(x => x.Message).Returns(evento);

            // Act
            await consumer.Consume(mockConsumeContext.Object);

            // Assert
            mockCollection.Verify(c =>
                c.InsertOneAsync(It.Is<SubastaDocument>(d =>
                    d.Id == evento.Id &&
                    d.Titulo == evento.Titulo &&
                    d.Estado == "Pending"
                ),
                null, default),
                Times.Once);
        }
    }
}
