using System.ComponentModel.DataAnnotations;

namespace DeliFitWeb.Models
{
    public class AtendimentoViewModel
    {
        [Key]
        public uint Id { get; set; }

        public uint IdRestaurante { get; set; }

        [Required]
        [Display(Name = "Dia da Semana")]
        public string DiaSemana { get; set; } = null!;

        [Required]
        [Display(Name = "Horário Início")]
        public DateTime? HorarioInicio { get; set; }

        [Required]
        [Display(Name = "Horário Fim")]
        public DateTime? HorarioFim { get; set; }

    }
}
