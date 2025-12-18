using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    internal interface IRestauranteService
    {
        int Create(Restaurante restaurante);

        void Delete(uint id);

        void Edit(Restaurante restaurante);

        Restaurante Get(uint id);

        IEnumerable<Restaurante> GetAll();

        Restaurante GetByNome(string nome);
    }
}
