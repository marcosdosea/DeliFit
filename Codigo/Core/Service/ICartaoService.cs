using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface ICartaoService
    {
        public uint Create(Cartao Cartao);

        public Cartao? Get(uint id);

        public void Delete(uint id);

        public IEnumerable<Cartao> GetAll();

        IEnumerable<Cartao> GetByCliente(uint idCliente);
    }
}
