using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using SubastaService.Application.Commands;
using SubastaService.Application.Handlers;
using SubastaService.Domain.Repositorios;

namespace SubastaService.Tests.Handlers
{
    public class EliminarSubastaCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CuandoLlamadoCorrectamente_DeberiaCancelarSubastaYRetornarTrue()
        {
            // Arrange
            var mockRepo = new Mock<IAuctionRepository>();
            var handler = new EliminarSubastaCommandHandler(mockRepo.Object);

            var command = new EliminarSubastaCommand
            {
                IdSubasta = Guid.NewGuid(),
                IdUsuario = Guid.NewGuid()
            };

            mockRepo
                .Setup(r => r.CancelarSubastaAsync(command.IdSubasta, command.IdUsuario, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask); // como es void, solo simulamos Task

            // Act
            var resultado = await handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            mockRepo.Verify(r =>
                    r.CancelarSubastaAsync(command.IdSubasta, command.IdUsuario, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}