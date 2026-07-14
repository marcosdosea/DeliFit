using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models;

public class AtendimentoViewModel
{
    [Key]
    public uint Id { get; set; }

    public uint IdRestaurante { get; set; }

    [Required]
    [Display(Name = "Dia da Semana")]
    public string DiaSemana { get; set; } = null!;

    [Required]
    [Display(Name = "Hor�rio In�cio")]
    public DateTime? HorarioInicio { get; set; }

    [Required]
    [Display(Name = "Hor�rio Fim")]
    public DateTime? HorarioFim { get; set; }

    public string NomeDiaSemana => DiaSemana switch
    {
        "1" => "Domingo",
        "2" => "Segunda-feira",
        "3" => "Terça-feira",
        "4" => "Quarta-feira",
        "5" => "Quinta-feira",
        "6" => "Sexta-feira",
        "7" => "Sábado",
        _ => DiaSemana
    };
}
