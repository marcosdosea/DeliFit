using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface ICarrinhoService
    {
        public uint Create(Carrinho Carrinho);

        public Carrinho? Get(uint id);

        public void Edit(Carrinho Carrinho);

        public void Delete(uint id);

        public IEnumerable<Carrinho> GetAll();
    }
}
