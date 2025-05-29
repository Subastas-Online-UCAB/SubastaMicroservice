using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SubastaService.Infrastructure.MongoDB.Documents
{
    public class SubastaDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("titulo")]
        public string Titulo { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("precioBase")]
        public decimal PrecioBase { get; set; }

        [BsonElement("duracion")]
        public TimeSpan Duracion { get; set; }

        [BsonElement("condicionParticipacion")]
        public string CondicionParticipacion { get; set; }

        [BsonElement("fechaInicio")]
        public DateTime FechaInicio { get; set; }

        [BsonElement("fechaFin")]
        public DateTime FechaFin { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; }

        [BsonElement("incrementoMinimo")]
        public decimal IncrementoMinimo { get; set; }

        [BsonElement("precioReserva")]
        [BsonIgnoreIfNull]
        public decimal? PrecioReserva { get; set; }

        [BsonElement("tipoSubasta")]
        public string TipoSubasta { get; set; }

        [BsonElement("idUsuario")]
        [BsonRepresentation(BsonType.String)]
        public Guid IdUsuario { get; set; }

        [BsonElement("idProducto")]
        [BsonRepresentation(BsonType.String)]
        public Guid IdProducto { get; set; }
    }
}