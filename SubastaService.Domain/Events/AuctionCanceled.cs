using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Events
{
    public class AuctionCanceled
    {
        public Guid SubastaId { get; set; } 

        public string Motivo { get; set; } = string.Empty;
    }
}

