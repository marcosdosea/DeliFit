using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

/// <inheritdoc />
public partial class AddMercadoPagoToCarrinho : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "mercadoPagoPaymentId",
            table: "carrinho",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "statusPagamentoCartao",
            table: "carrinho",
            type: "varchar(20)",
            maxLength: 20,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "mercadoPagoPaymentId",
            table: "carrinho");

        migrationBuilder.DropColumn(
            name: "statusPagamentoCartao",
            table: "carrinho");
    }
}
