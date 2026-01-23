namespace Core.DTO
{
    public class ClienteDTO
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Telefone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime DataNascimento { get; set; }

    }
}
