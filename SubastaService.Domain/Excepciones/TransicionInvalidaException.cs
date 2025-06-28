using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Excepciones
{
    public class TransicionInvalidaException : Exception
    {
        public TransicionInvalidaException(string estadoActual, string nuevoEstado)
            : base($"Transición inválida de '{estadoActual}' a '{nuevoEstado}'.") { }
    }
}
