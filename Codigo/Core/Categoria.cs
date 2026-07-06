using System;
using System.Collections.Generic;

namespace Core;

public partial class Categoria
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
