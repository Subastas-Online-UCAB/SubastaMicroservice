using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SubastaService.API.Controllers;
using MediatR;
using SubastaService.Application.DTO;
using SubastaService.Application.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;
using SubastaService.Domain.Entidades;
using SubastaService.Application.Commands;

namespace SubastaService.Tests.Controllers
{
    public class SubastasControllerTests
    {
        [Fact]
        public async Task ObtenerPorId_DebeRetornarOkSiExiste()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var subastaId = Guid.NewGuid();
            var subastaEsperada = new SubastaCompletaDto
            {
                Id = subastaId,
                Titulo = "Xbox",
                Estado = "Pending"
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<ConsultarSubastaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(subastaEsperada);

            var controller = new SubastasController(mockMediator.Object);

            // Act
            var resultado = await controller.ObtenerPorId(subastaId, CancellationToken.None);

            // Assert
            var okResult = resultado as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(subastaEsperada);
        }

        [Fact]
        public async Task ObtenerPorId_DebeRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var subastaId = Guid.NewGuid();

            mockMediator
                .Setup(m => m.Send(It.IsAny<ConsultarSubastaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SubastaCompletaDto)null!);

            var controller = new SubastasController(mockMediator.Object);

            // Act
            var resultado = await controller.ObtenerPorId(subastaId, CancellationToken.None);

            // Assert
            resultado.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CrearSubasta_DeberiaRetornarCreatedConId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var idEsperado = Guid.NewGuid();

            var command = new CreateAuctionCommand
            {
                Nombre = "PlayStation 5",
                PrecioBase = 500,
                FechaInicio = DateTime.UtcNow,
                Duracion = TimeSpan.FromDays(7),
                IdUsuario = Guid.NewGuid()
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<CreateAuctionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idEsperado);

            var controller = new SubastasController(mockMediator.Object);

            // Act
            var resultado = await controller.CrearSubasta(command);

            // Assert
            var createdResult = resultado as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.ActionName.Should().Be(nameof(SubastasController.GetById));

            // Aquí el Value es un objeto anónimo: new { id = idEsperado }
            var resultadoId = createdResult.Value!.GetType().GetProperty("id")!.GetValue(createdResult.Value, null);
            resultadoId.Should().Be(idEsperado);
        }

        [Fact]
        public async Task CrearSubasta_CuandoModeloInvalido_DeberiaRetornarBadRequest()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();

            var command = new CreateAuctionCommand
            {
                // Puedes dejar propiedades vacías o inválidas a propósito
                Nombre = "",
                PrecioBase = -10,
                FechaInicio = DateTime.UtcNow,
                Duracion = TimeSpan.Zero,
                IdUsuario = Guid.Empty
            };

            var controller = new SubastasController(mockMediator.Object);

            // Forzar ModelState inválido manualmente
            controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio");

            // Act
            var resultado = await controller.CrearSubasta(command);

            // Assert
            resultado.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = resultado as BadRequestObjectResult;
            badRequest!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Delete_CuandoSubastaExiste_DeberiaRetornarNoContent()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            mockMediator
                .Setup(m => m.Send(It.Is<EliminarSubastaCommand>(c =>
                    c.IdSubasta == idSubasta && c.IdUsuario == idUsuario), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var controller = new SubastasController(mockMediator.Object);

            // Act
            var resultado = await controller.Delete(idSubasta, idUsuario);

            // Assert
            resultado.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_CuandoSubastaNoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var idSubasta = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            mockMediator
                .Setup(m => m.Send(It.Is<EliminarSubastaCommand>(c =>
                    c.IdSubasta == idSubasta && c.IdUsuario == idUsuario), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var controller = new SubastasController(mockMediator.Object);

            // Act
            var resultado = await controller.Delete(idSubasta, idUsuario);

            // Assert
            resultado.Should().BeOfType<NotFoundResult>();
        }

       

    }
}
