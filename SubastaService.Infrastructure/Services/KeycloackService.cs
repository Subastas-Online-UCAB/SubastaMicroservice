using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Infrastructure.Services
{
    public class KeycloakSettings
    {
        public string Authority { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
    }
}
