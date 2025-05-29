using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Events
{
    public class AuctionEnded
    {
        public Guid SubastaId { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
