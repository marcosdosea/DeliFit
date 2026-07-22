using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

/// <inheritdoc />
public partial class AddIdEnderecoToCarrinho : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<uint>(
            name: "idEndereco",
            table: "carrinho",
            type: "int unsigned",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "fk_Carrinho_Endereco1_idx",
            table: "carrinho",
            column: "idEndereco");

        migrationBuilder.AddForeignKey(
            name: "fk_Carrinho_Endereco1",
            table: "carrinho",
            column: "idEndereco",
            principalTable: "endereco",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_Carrinho_Endereco1",
            table: "carrinho");

        migrationBuilder.DropIndex(
            name: "fk_Carrinho_Endereco1_idx",
            table: "carrinho");

        migrationBuilder.DropColumn(
            name: "idEndereco",
            table: "carrinho");
    }
}
