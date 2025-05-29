using MongoDB.Driver;
using SubastaService.Infrastructure.MongoDB.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Infrastructure.MongoDB
{
    public interface ISubastaMongoContext
    {
        IMongoCollection<SubastaDocument> Subastas { get; }
    }
}
