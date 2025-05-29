using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Events
{
    public class BidPlaced
    {
        public Guid SubastaId { get; set; }
        public Guid UsuarioId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPuja { get; set; }
    }
}
