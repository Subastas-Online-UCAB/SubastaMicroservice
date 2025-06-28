using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubastaService.Application.Commands;
using SubastaService.Application.Queries;
using SubastaService.Application.Request;
using SubastaService.Domain.Entidades;

namespace SubastaService.API.Controllers
{
    /// <summary>
    /// Controlador para gestionar operaciones relacionadas con subastas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubastasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubastasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea una nueva subasta.
        /// </summary>
        /// <param name="command">Datos de la subasta a crear.</param>
        /// <returns>ID de la subasta creada.</returns>
        /// <response code="201">Subasta creada exitosamente.</response>
        /// <response code="400">Datos inválidos o incompletos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearSubasta([FromBody] CreateAuctionCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        /// <summary>
        /// Obtiene una subasta por su ID (placeholder temporal).
        /// </summary>
        /// <param name="id">ID de la subasta.</param>
        /// <returns>Objeto con el ID y mensaje de confirmación.</returns>
        /// <response code="200">Subasta encontrada (placeholder).</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetById(Guid id)
        {
            // Este es un placeholder hasta que hagamos un query real
            return Ok(new { Id = id, Mensaje = "Subasta recuperada (placeholder)" });
        }

        /// <summary>
        /// Edita los datos de una subasta existente.
        /// </summary>
        /// <param name="command">Datos actualizados de la subasta.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Subasta editada exitosamente.</response>
        [HttpPut("editar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> EditarSubasta([FromBody] EditarSubastaCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Elimina una subasta por su ID.
        /// </summary>
        /// <param name="id">ID de la subasta a eliminar.</param>
        /// <param name="usuarioId">ID del usuario que solicita la eliminación.</param>
        /// <returns>NoContent si fue eliminada, NotFound si no existe.</returns>
        /// <response code="204">Subasta eliminada exitosamente.</response>
        /// <response code="404">Subasta no encontrada.</response>
        [HttpDelete("eliminar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid usuarioId)
        {
            var resultado = await _mediator.Send(new EliminarSubastaCommand
            {
                IdSubasta = id,
                IdUsuario = usuarioId
            });

            if (!resultado)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Consulta una subasta por su ID.
        /// </summary>
        /// <param name="id">ID de la subasta a consultar.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Detalles de la subasta si existe.</returns>
        /// <response code="200">Subasta encontrada.</response>
        /// <response code="404">Subasta no encontrada.</response>
        [HttpGet("buscar/{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken cancellationToken)
        {
            var resultado = await _mediator.Send(new ConsultarSubastaPorIdQuery(id), cancellationToken);

            if (resultado is null)
                return NotFound();

            return Ok(resultado);
        }


        /// <summary>
        /// Obtiene la lista de todas las subastas.
        /// </summary>
        /// <remarks>
        /// Retorna todas las subastas registradas desde la base de datos de lectura (MongoDB).
        /// No requiere parámetros de entrada.
        /// </remarks>
        /// <returns>Lista de objetos <see cref="Subasta"/>.</returns>
        /// <response code="200">Lista de subastas obtenida exitosamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var resultado = await _mediator.Send(new GetAllSubastasQuery());
            return Ok(resultado);
        }

        [HttpPut("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoRequest request)
        {
            var command = new CambiarEstadoSubastaCommand
            {
                SubastaId = id,
                NuevoEstado = request.NuevoEstado,
                IdUsuario = request.IdUsuario
            };

            var result = await _mediator.Send(command);
            return result ? Ok("Estado actualizado correctamente.") : BadRequest("No se pudo actualizar el estado.");
        }



    }
}
