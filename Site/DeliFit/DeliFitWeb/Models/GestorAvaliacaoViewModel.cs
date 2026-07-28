namespace DeliFitWeb.Models;

public class GestorAvaliacaoViewModel
{
    public uint Id { get; set; }
    public string NomeCliente { get; set; } = "";
    public string TelefoneCliente { get; set; } = "";
    public decimal Nota { get; set; }
    public string Descricao { get; set; } = "";
}
