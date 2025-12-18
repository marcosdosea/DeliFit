using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public uint Id { get; set; }

    public DateTime DataPagamento { get; set; }

    public DateTime DataVencimento { get; set; }

    public string StatusMensalidade { get; set; } = null!;

    public uint IdRestaurante { get; set; }

    public virtual Restaurante IdRestauranteNavigation { get; set; } = null!;
}
