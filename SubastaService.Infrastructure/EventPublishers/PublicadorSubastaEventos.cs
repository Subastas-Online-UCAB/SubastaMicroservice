using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using SubastaService.Domain.Events;
using SubastaService.Domain.Interfaces;

namespace SubastaService.Infrastructure.EventPublishers
{
    public class PublicadorSubastaEventos : IPublicadorSubastaEventos
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public PublicadorSubastaEventos(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublicarSubastaCreada(SubastaCreadaEvent evento)
        {
            await _publishEndpoint.Publish(evento);
        }


        public async Task PublicarSubastaEditada(SubastaEditadaEvent evento)
        {
            await _publishEndpoint.Publish(evento);
        }

        
        public async Task PublicarAuctionStarted(AuctionStarted evento)
        {
            await _publishEndpoint.Publish(evento);
        }
    }
}