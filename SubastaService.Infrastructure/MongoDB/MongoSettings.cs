using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Infrastructure.MongoDB
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; } = default;
        public string DatabaseName { get; set; } = default;
    }
}
