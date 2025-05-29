using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace SubastaService.Application.Sagas
{
    public class SubastaState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }  // antes Guid
        public string CurrentState { get; set; } = null!;
        public string SubastaId { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }

        // 👉 requerido por MongoDb saga storage
        public int Version { get; set; }
    }
}
