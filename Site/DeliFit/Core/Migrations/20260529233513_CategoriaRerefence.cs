using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

/// <inheritdoc />
public partial class CategoriaRerefence : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<uint>(
            name: "IdCategoria",
            table: "item",
            type: "int unsigned",
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.CreateIndex(
            name: "IX_item_IdCategoria",
            table: "item",
            column: "IdCategoria");

        migrationBuilder.AddForeignKey(
            name: "fk_Item_Categoria1",
            table: "item",
            column: "IdCategoria",
            principalTable: "categoria",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_Item_Categoria1",
            table: "item");

        migrationBuilder.DropIndex(
            name: "IX_item_IdCategoria",
            table: "item");

        migrationBuilder.DropColumn(
            name: "IdCategoria",
            table: "item");
    }
}
