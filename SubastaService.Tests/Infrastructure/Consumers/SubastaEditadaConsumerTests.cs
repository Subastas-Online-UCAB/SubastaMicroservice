using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MongoDB.Driver;
using SubastaService.Domain.Events;
using SubastaService.Infrastructure.Consumers;
using SubastaService.Infrastructure.MongoDB;
using SubastaService.Infrastructure.MongoDB.Documents;

namespace SubastaService.Tests.Consumers
{
    public class SubastaEditadaConsumerTests
    {
        [Fact]
        public async Task Consume_UpdatesSubastaDocumentInMongo_WhenEventIsValid()
        {
            // Arrange
            var evento = new SubastaEditadaEvent
            {
                SubastaId = Guid.NewGuid(),
                Titulo = "Subasta Editada",
                Descripcion = "Actualizada desde test",
                PrecioBase = 100,
                Duracion = TimeSpan.FromHours(1),
                CondicionParticipacion = "Libre",
                FechaInicio = DateTime.UtcNow,
                FechaCierre = DateTime.UtcNow.AddDays(1),
                IncrementoMinimo = 10,
                PrecioReserva = 150,
                TipoSubasta = "Pública",
                ProductoId = Guid.NewGuid(),
                UsuarioId = Guid.NewGuid()
            };

            var mockCollection = new Mock<IMongoCollection<SubastaDocument>>();
            var mockContext = new Mock<ISubastaMongoContext>();
            var mockConsumeContext = new Mock<ConsumeContext<SubastaEditadaEvent>>();

            mockContext.Setup(c => c.Subastas).Returns(mockCollection.Object);
            mockConsumeContext.Setup(c => c.Message).Returns(evento);

            var consumer = new SubastaEditadaConsumer(mockContext.Object);

            // Act
            await consumer.Consume(mockConsumeContext.Object);

            // Assert
            mockCollection.Verify(c => c.ReplaceOneAsync(
                It.Is<FilterDefinition<SubastaDocument>>(f => f != null),
                It.Is<SubastaDocument>(d =>
                    d.Id == evento.SubastaId &&
                    d.Titulo == evento.Titulo &&
                    d.Descripcion == evento.Descripcion &&
                    d.PrecioBase == evento.PrecioBase &&
                    d.Duracion == evento.Duracion &&
                    d.CondicionParticipacion == evento.CondicionParticipacion &&
                    d.FechaInicio == evento.FechaInicio &&
                    d.FechaFin == evento.FechaCierre &&
                    d.IncrementoMinimo == evento.IncrementoMinimo &&
                    d.PrecioReserva == evento.PrecioReserva &&
                    d.TipoSubasta == evento.TipoSubasta &&
                    d.IdProducto == evento.ProductoId &&
                    d.IdUsuario == evento.UsuarioId
                ),
                It.Is<ReplaceOptions>(o => o.IsUpsert == true),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
