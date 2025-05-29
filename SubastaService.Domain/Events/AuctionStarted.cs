using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// AuctionStarted.cs
namespace SubastaService.Domain.Events
{
    public class AuctionStarted
    {
        public string SubastaId { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
    }
}
