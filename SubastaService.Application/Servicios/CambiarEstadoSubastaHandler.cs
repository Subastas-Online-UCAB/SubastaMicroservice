using MediatR;
using MassTransit;
using SubastaService.Domain.Repositorios;
using SubastaService.Domain.Entidades;
using SubastaService.Domain.Events;
using SubastaService.Application.Commands;
using SubastaService.Domain.Excepciones;

public class CambiarEstadoSubastaHandler : IRequestHandler<CambiarEstadoSubastaCommand, bool>
{
    private readonly IAuctionRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint;

    public CambiarEstadoSubastaHandler(IAuctionRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(CambiarEstadoSubastaCommand request, CancellationToken cancellationToken)
    {
        var subasta = await _repo.ObtenerPorIdAsync(request.SubastaId, cancellationToken);
        if (subasta == null)
            throw new SubastaNoEncontradaException(request.SubastaId);

        if (subasta.IdUsuario.ToString() != request.IdUsuario)
            throw new UsuarioSinPermisoException(request.IdUsuario);

        if (!EsTransicionValida(subasta.Estado, request.NuevoEstado))
            throw new TransicionInvalidaException(subasta.Estado, request.NuevoEstado);

        // Actualiza el estado en PostgreSQL
        subasta.Estado = request.NuevoEstado;
        await _repo.ActualizarAsync(subasta, cancellationToken);

        // Publica el evento correspondiente a la saga
        switch (request.NuevoEstado)
        {
            case "Active":
                await _publishEndpoint.Publish(new AuctionStarted
                {
                    SubastaId = subasta.IdSubasta.ToString()
                });
                break;

            case "Ended":
                await _publishEndpoint.Publish(new AuctionEnded
                {
                    SubastaId = subasta.IdSubasta
                });
                break;

            case "Pagada":
                await _publishEndpoint.Publish(new PaymentReceived
                {
                    SubastaId = subasta.IdSubasta
                });
                break;

            case "Canceled":
                await _publishEndpoint.Publish(new AuctionCanceled
                {
                    SubastaId = subasta.IdSubasta
                });
                break;
        }

        // (Opcional) Publicar también evento de sincronización con Mongo
        await _publishEndpoint.Publish(new AuctionStateChangedEvent
        {
            AuctionId = subasta.IdSubasta,
            NuevoEstado = request.NuevoEstado,
            Fecha = DateTime.UtcNow
        });

        return true;
    }

    private bool EsTransicionValida(string actual, string nuevo)
    {
        return actual switch
        {
            "Pending" => nuevo == "Active" || nuevo == "Canceled",
            "Active" => nuevo == "Ended" || nuevo == "Canceled",
            "Ended" => nuevo == "Pagada" || nuevo == "Canceled",
            _ => false
        };
    }
}
