using SubastaService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaService.Domain.Events;

namespace SubastaService.Domain.Interfaces
{
    public interface IPublicadorSubastaEventos
    {
        Task PublicarSubastaCreada(SubastaCreadaEvent evento);
        Task PublicarSubastaEditada(SubastaEditadaEvent evento);
        Task PublicarAuctionStarted(AuctionStarted evento);

    }
}
