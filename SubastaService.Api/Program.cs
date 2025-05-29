using MediatR;
using SubastaService.Application.Commands;
using SubastaService.Domain.Repositorios;
using SubastaService.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using UsuarioServicio.Infrastructure.Persistencia;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MassTransit;
using SubastaService.Application.Sagas;
using SubastaService.Infraestructura.Configuracion;
using SubastaService.Infrastructure.Mongo;
using SubastaService.Infrastructure.MongoDB;
using SubastaService.Infrastructure.Consumers;
using SubastaService.Domain.Interfaces;
using SubastaService.Infrastructure.EventPublishers;






var builder = WebApplication.CreateBuilder(args);

//Swagger
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateAuctionCommand).Assembly));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keycloak = builder.Configuration.GetSection("Keycloak");
        options.Authority = "http://localhost:8081/realms/microservicio-usuarios";
        options.Audience = "account";
        options.RequireHttpsMetadata = false; // solo si estás en desarrollo local
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SubastaService.Api",
        Version = "v1"
    });

    // Configuración de seguridad JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa el token JWT como: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


//Mongo 

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
builder.Services.AddSingleton<MongoDbContext>();


// MassTransit
builder.Services.AddMassTransit(x =>
{
    // 1. Registrar consumidores
    x.AddConsumer<SubastaCreadaConsumer>();

    // 2. Registrar la saga
    x.AddSagaStateMachine<SubastaStateMachine, SubastaState>()
        .MongoDbRepository(r =>
        {
            r.Connection = builder.Configuration["MongoSettings:ConnectionString"];
            r.DatabaseName = builder.Configuration["MongoSettings:DatabaseName"];

            r.CollectionName = "subasta_sagas"; // opcional
        });

    // 3. Configurar RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => { });

        // Consumer normal
        cfg.ReceiveEndpoint("subasta-creada-event", e =>
        {
            e.ConfigureConsumer<SubastaCreadaConsumer>(context);
        });

        // Endpoint para la saga (auto generado por MassTransit)
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPublicadorSubastaEventos, PublicadorSubastaEventos>();
builder.Services.AddSingleton<ISubastaMongoContext, MongoDbContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
