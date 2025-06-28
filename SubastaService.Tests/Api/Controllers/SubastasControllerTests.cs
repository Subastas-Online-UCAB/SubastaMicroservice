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
using SubastaService.Application.Comun;
using SubastaService.Application.Request;

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

        [Fact]
        public async Task GetAll_ReturnsOkWithListOfSubastas()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var subastasMock = new List<Subasta>
            {
                new Subasta
                {
                    IdSubasta = Guid.NewGuid(),
                    Nombre = "Subasta 1",
                    Descripcion = "Prueba",
                    PrecioBase = 100,
                    Duracion = TimeSpan.FromMinutes(60),
                    CondicionParticipacion = "Libre",
                    FechaInicio = DateTime.UtcNow,
                    Estado = "Active",
                    IncrementoMinimo = 10,
                    PrecioReserva = 200,
                    TipoSubasta = "Pública",
                    IdUsuario = Guid.NewGuid(),
                    IdProducto = Guid.NewGuid()
                }
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetAllSubastasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(subastasMock);

            var controller = new SubastasController(mockMediator.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSubastas = Assert.IsType<List<Subasta>>(okResult.Value);
            Assert.Single(returnedSubastas);
            Assert.Equal("Subasta 1", returnedSubastas[0].Nombre);
        }

        /*
                [Fact]
                public async Task EditarSubasta_ReturnsOkWithResult_WhenCommandIsValid()
                {
                    // Arrange
                    var mockMediator = new Mock<IMediator>();

                    var command = new EditarSubastaCommand
                    {
                        IdSubasta = Guid.NewGuid(),
                        Nombre = "Subasta Editada",
                        Descripcion = "Subasta actualizada",
                        PrecioBase = 150,
                        Duracion = TimeSpan.FromHours(2),
                        CondicionParticipacion = "Nuevo requisito",
                        FechaInicio = DateTime.UtcNow.AddDays(1),
                        Estado = "Pending",
                        IncrementoMinimo = 15,
                        PrecioReserva = 300,
                        TipoSubasta = "Privada",
                        IdUsuario = Guid.NewGuid(),
                        IdProducto = Guid.NewGuid()
                    };

                    var expectedResult = true;

                    mockMediator
                        .Setup(m => m.Send(It.IsAny<EditarSubastaCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResult);

                    var controller = new SubastasController(mockMediator.Object);

                    // Act
                    var result = await controller.EditarSubasta(command);

                    // Assert
                    var okResult = Assert.IsType<OkObjectResult>(result);
                    Assert.Equal(expectedResult, okResult.Value);
                }
        */
        [Fact]
        public async Task EditarSubasta_ReturnsOkWithResult_WhenCommandIsValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();

            var command = new EditarSubastaCommand(
                SubastaId: Guid.NewGuid(),
                UsuarioId: Guid.NewGuid(),
                Titulo: "Subasta Editada",
                Descripcion: "Subasta actualizada",
                FechaCierre: DateTime.UtcNow.AddDays(2),
                PrecioBase: 150,
                Duracion: TimeSpan.FromHours(2),
                CondicionParticipacion: "Nuevo requisito",
                IncrementoMinimo: 15,
                PrecioReserva: 300,
                TipoSubasta: "Privada",
                ProductoId: Guid.NewGuid()
            );

            var expectedResponse = MessageResponse.CrearExito("Subasta editada exitosamente.");

            mockMediator
                .Setup(m => m.Send(It.IsAny<EditarSubastaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var controller = new SubastasController(mockMediator.Object);

            // Act
            var result = await controller.EditarSubasta(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Subasta editada exitosamente.", response.Message);
        }

        [Fact]
        public async Task CambiarEstado_DeberiaRetornarOk_SiCambioEsExitoso()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();

            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = Guid.NewGuid(),
                NuevoEstado = "Active",
                IdUsuario = Guid.NewGuid().ToString()
            };

            mockMediator.Setup(m => m.Send(It.IsAny<CambiarEstadoSubastaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Simulamos éxito

            var controller = new SubastasController(mockMediator.Object);

            var request = new CambiarEstadoRequest
            {
                NuevoEstado = command.NuevoEstado,
                IdUsuario = command.IdUsuario
            };

            // Act
            var result = await controller.CambiarEstado(command.SubastaId, request);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().Be("Estado actualizado correctamente.");

            mockMediator.Verify(m => m.Send(It.Is<CambiarEstadoSubastaCommand>(c =>
                c.SubastaId == command.SubastaId &&
                c.NuevoEstado == command.NuevoEstado &&
                c.IdUsuario == command.IdUsuario
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
