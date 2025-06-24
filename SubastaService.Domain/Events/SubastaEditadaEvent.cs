using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Events
{
    public class SubastaEditadaEvent
    {
        public Guid SubastaId { get; set; }
        public string Titulo { get; set; } = default!;
        public string Descripcion { get; set; } = default!;
        public DateTime FechaInicio { get; set; }        // <-- ¡Aquí está!
        public DateTime FechaCierre { get; set; }

        public decimal PrecioBase { get; set; }
        public TimeSpan Duracion { get; set; }
        public string CondicionParticipacion { get; set; } = default!;
        public decimal IncrementoMinimo { get; set; }
        public decimal? PrecioReserva { get; set; }
        public string TipoSubasta { get; set; } = default!;
        public Guid ProductoId { get; set; }
        public Guid UsuarioId { get; set; }
    }

}
