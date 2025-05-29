using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Application.Comun
{
    public class MessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static MessageResponse CrearExito(string message) => new() { Success = true, Message = message };
        public static MessageResponse CrearError(string message) => new() { Success = false, Message = message };

    }
}
