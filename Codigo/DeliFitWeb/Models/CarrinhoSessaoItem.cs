namespace DeliFitWeb.Models;

/// <summary>
/// Representa um item temporário no carrinho de compras armazenado na sessão.
/// Contém os dados suficientes para exibição na tela M14 e criação do pedido.
/// </summary>
public class CarrinhoSessaoItem
{
    public uint IdItem { get; set; }
    public string NomeItem { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public string? Tamanho { get; set; }
    public string? Observacao { get; set; }
    public uint IdRestaurante { get; set; }
    public string NomeRestaurante { get; set; } = string.Empty;

    public decimal Subtotal => PrecoUnitario * Quantidade;
}
