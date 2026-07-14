using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

/// <inheritdoc />
public partial class AddMercadoPagoCardVault : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "mercadoPagoCustomerId",
            table: "cliente",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.DropColumn(
            name: "numero",
            table: "cartao");

        migrationBuilder.DropColumn(
            name: "cvv",
            table: "cartao");

        migrationBuilder.AddColumn<string>(
            name: "mercadoPagoCardId",
            table: "cartao",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "mercadoPagoPaymentMethodId",
            table: "cartao",
            type: "varchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ultimosQuatroDigitos",
            table: "cartao",
            type: "char(4)",
            fixedLength: true,
            maxLength: 4,
            nullable: false,
            defaultValue: "0000");

        migrationBuilder.AddColumn<string>(
            name: "bandeira",
            table: "cartao",
            type: "varchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "mercadoPagoCustomerId",
            table: "cliente");

        migrationBuilder.DropColumn(
            name: "mercadoPagoCardId",
            table: "cartao");

        migrationBuilder.DropColumn(
            name: "mercadoPagoPaymentMethodId",
            table: "cartao");

        migrationBuilder.DropColumn(
            name: "ultimosQuatroDigitos",
            table: "cartao");

        migrationBuilder.DropColumn(
            name: "bandeira",
            table: "cartao");

        migrationBuilder.AddColumn<string>(
            name: "numero",
            table: "cartao",
            type: "char(16)",
            fixedLength: true,
            maxLength: 16,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "cvv",
            table: "cartao",
            type: "char(3)",
            fixedLength: true,
            maxLength: 3,
            nullable: false,
            defaultValue: "");
    }
}
