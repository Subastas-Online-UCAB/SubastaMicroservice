using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Application.DTO
{
    public class SubastaCompletaDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal PrecioBase { get; set; }
        public TimeSpan Duracion { get; set; }
        public string CondicionParticipacion { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = null!;
        public decimal IncrementoMinimo { get; set; }
        public decimal? PrecioReserva { get; set; }
        public string TipoSubasta { get; set; } = null!;
        public Guid IdUsuario { get; set; }
        public Guid IdProducto { get; set; }
    }
}
