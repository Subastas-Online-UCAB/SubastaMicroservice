using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using SubastaService.Domain.Events;

namespace SubastaService.Application.Sagas
{
    public class SubastaStateMachine : MassTransitStateMachine<SubastaState>
    {
        public State Pending { get; private set; } = null!;
        public State Active { get; private set; } = null!;
        public State Ended { get; private set; } = null!;
        public State Canceled { get; private set; } = null!;
        public State Completed { get; private set; } = null!;

        public Event<AuctionStarted> AuctionStarted { get; private set; } = null!;
        public Event<BidPlaced> BidPlaced { get; private set; } = null!;
        public Event<AuctionEnded> AuctionEnded { get; private set; } = null!;
        public Event<PaymentReceived> PaymentReceived { get; private set; } = null!;
        public Event<AuctionCanceled> AuctionCanceled { get; private set; } = null!;

        public SubastaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => AuctionStarted, x => x.CorrelateById(ctx => Guid.Parse(ctx.Message.SubastaId)));
            Event(() => BidPlaced, x => x.CorrelateById(ctx => ctx.Message.SubastaId));
            Event(() => AuctionEnded, x => x.CorrelateById(ctx => ctx.Message.SubastaId));
            Event(() => PaymentReceived, x => x.CorrelateById(ctx => ctx.Message.SubastaId));
            Event(() => AuctionCanceled, x => x.CorrelateById(ctx => ctx.Message.SubastaId));

            Initially(
                When(AuctionStarted)
                    .Then(ctx =>
                    {
                        Console.WriteLine($"Saga activada para SubastaId: {ctx.Message.SubastaId}");
                        ctx.Saga.CorrelationId = Guid.Parse(ctx.Message.SubastaId);
                        ctx.Saga.SubastaId = ctx.Message.SubastaId;
                        ctx.Saga.FechaCreacion = DateTime.UtcNow;
                    })
                    .TransitionTo(Pending)
            );

            During(Active,
                When(BidPlaced)
                    .Then(ctx => { /* opcional: lógica de registro */ })
            );

            During(Active,
                When(AuctionEnded)
                    .TransitionTo(Ended)
            );

            During(Ended,
                When(PaymentReceived)
                    .TransitionTo(Completed)
            );

            During(Pending, When(AuctionCanceled).TransitionTo(Canceled));
            During(Active, When(AuctionCanceled).TransitionTo(Canceled));

            During(Pending, When(AuctionCanceled)
                .Then(ctx =>
                {
                    Console.WriteLine($"Subasta cancelada: {ctx.Saga.SubastaId}");
                })
                .TransitionTo(Canceled));

            During(Active, When(AuctionCanceled)
                .Then(ctx =>
                {
                    Console.WriteLine($"Subasta cancelada desde estado activo: {ctx.Saga.SubastaId}");
                })
                .TransitionTo(Canceled));

            During(Pending,
                When(AuctionCanceled)
                    .Then(ctx => Console.WriteLine($"Subasta cancelada desde Pending"))
                    .TransitionTo(Canceled),

                When(AuctionStarted)
                    .Then(ctx => Console.WriteLine($"Subasta activada"))
                    .TransitionTo(Active)
            );
        }
    }
}