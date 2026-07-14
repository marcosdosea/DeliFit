namespace DeliFitWeb.Models;

public class ConfigurarAtendimentoViewModel
{
    public uint IdRestaurante { get; set; }
    public List<DiaAtendimentoViewModel> Dias { get; set; } = new();
}

public class DiaAtendimentoViewModel
{
    public uint Id { get; set; }
    public string DiaSemana { get; set; } = "";
    public string NomeDia { get; set; } = "";
    public bool Ativo { get; set; }
    public string HorarioInicio { get; set; } = "08:00";
    public string HorarioFim { get; set; } = "18:00";
}
