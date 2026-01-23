using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public class ServiceException : SystemException
    {
        public ServiceException(String mensagem, Exception inner)
            : base(mensagem, inner)
        {
        }

        public ServiceException(String mensagem)
            : base(mensagem)
        {

        }
    }
}
