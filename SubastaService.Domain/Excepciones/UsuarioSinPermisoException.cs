using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaService.Domain.Excepciones
{
    public class UsuarioSinPermisoException : Exception
    {
        public UsuarioSinPermisoException(string userId)
            : base($"El usuario con ID {userId} no tiene permiso para modificar esta subasta.") { }
    }
}
