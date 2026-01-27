namespace Core.DTO
{
    public class RestauranteDTO
    {
        public uint Id { get; set; }

        public string NomeRestaurante { get; set; } = null!;

        public bool Validado { get; set; }

        public string Cidade { get; set; } = null!;

        public string Estado { get; set; } = null!;
    }
}
