using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Entidades
{
    public class Subasta
    {
        public Guid IdSubasta { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioBase { get; set; }
        public TimeSpan Duracion { get; set; }
        public string CondicionParticipacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Estado { get; set; }
        public decimal IncrementoMinimo { get; set; }
        public decimal? PrecioReserva { get; set; }
        public string TipoSubasta { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid IdProducto { get; set; }


        public void Editar(string titulo, string descripcion, DateTime fechaCierre)
        {
            Nombre = titulo;
            Descripcion = descripcion;
            //FechaCierre = fechaCierre;
        }
    }

    public enum EstadoSubasta
    {
        Borrador,
        EnCurso,
        Finalizada,
        Cancelada
    }

}
