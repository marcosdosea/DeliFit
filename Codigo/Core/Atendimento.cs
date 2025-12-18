using System;
using System.Collections.Generic;

namespace Core;

public partial class Atendimento
{
    public uint Id { get; set; }

    /// <summary>
    /// 1-Domingo
    /// 2-Segunda
    /// 3-Terça
    /// 4-Quarta
    /// 5-Quinta
    /// 6-Sexta
    /// 7-Sabado
    /// </summary>
    public string DiaSemana { get; set; } = null!;

    public DateTime Horario { get; set; }

    public decimal ValorFrete { get; set; }

    public uint IdRestaurante { get; set; }

    public virtual Restaurante IdRestauranteNavigation { get; set; } = null!;
}
