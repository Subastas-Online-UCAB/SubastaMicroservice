using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Events
{
    public class AuctionStateChangedEvent
    {
        public Guid AuctionId { get; set; }
        public string NuevoEstado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }

}
