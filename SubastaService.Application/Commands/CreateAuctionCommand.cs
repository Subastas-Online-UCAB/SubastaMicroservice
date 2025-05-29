using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Application.Commands
{
    public class CreateAuctionCommand : IRequest<Guid>
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioBase { get; set; }
        public TimeSpan Duracion { get; set; }
        public string CondicionParticipacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public decimal IncrementoMinimo { get; set; }
        public decimal? PrecioReserva { get; set; }
        public string TipoSubasta { get; set; } = "Abierta";
        public Guid IdUsuario { get; set; }
        public Guid IdProducto { get; set; }
    }
}
