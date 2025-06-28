using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Excepciones
{
    public class SubastaNoEncontradaException : Exception
    {
        public SubastaNoEncontradaException(Guid subastaId)
            : base($"No se encontró la subasta con ID: {subastaId}") { }
    }
}
