using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace SubastaService.Infraestructura.Configuracion
{
    public static class MassTransitConfig
    {
        public static void AddMassTransitWithMongo(this IServiceCollection services)
        {
            // Configura MassTransit
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                // Más adelante aquí agregaremos la saga: .AddSagaStateMachine<...>()

                cfg.UsingRabbitMq((context, config) =>
                {
                    config.Host("localhost", "/", h => { });

                    config.ConfigureEndpoints(context);
                });
            });

            // Registra MongoClient para uso futuro con Sagas
            services.AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient("mongodb://localhost:27017");
            });
        }
    }
}
