using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubastaService.Application.Commands;
using SubastaService.Application.Queries;

namespace SubastaService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubastasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubastasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearSubasta([FromBody] CreateAuctionCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Este es un placeholder hasta que hagamos un query real
            return Ok(new { Id = id, Mensaje = "Subasta recuperada (placeholder)" });
        }


        [HttpPut("editar")]
        public async Task<IActionResult> EditarSubasta([FromBody] EditarSubastaCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("eliminar/{id}")]
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

        [HttpGet("buscar/{id}")]
        public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken cancellationToken)
        {
            var resultado = await _mediator.Send(new ConsultarSubastaPorIdQuery(id), cancellationToken);

            if (resultado is null)
                return NotFound();

            return Ok(resultado);
        }


    }
}
