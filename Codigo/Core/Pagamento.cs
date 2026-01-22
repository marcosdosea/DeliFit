using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public uint Id { get; set; }

    public decimal ValorMensalidade { get; set; }

    public DateTime DataPagamento { get; set; }

    public DateTime DataVencimento { get; set; }

    /// <summary>
    /// P : Pago
    /// E: Pendente
    /// A: Atraso
    /// </summary>
    public string StatusMensalidade { get; set; } = null!;

    public uint IdRestaurante { get; set; }

    public virtual Restaurante IdRestauranteNavigation { get; set; } = null!;
}
