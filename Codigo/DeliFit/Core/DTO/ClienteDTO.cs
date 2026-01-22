using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class ClienteDTO
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Telefone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime DataNascimento { get; set; }

    }
}
