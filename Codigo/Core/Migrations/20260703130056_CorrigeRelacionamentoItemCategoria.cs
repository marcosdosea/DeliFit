using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

/// <inheritdoc />
public partial class CorrigeRelacionamentoItemCategoria : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
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

        migrationBuilder.CreateTable(
            name: "categoria_item",
            columns: table => new
            {
                idCategoria = table.Column<uint>(type: "int unsigned", nullable: false),
                idItem = table.Column<uint>(type: "int unsigned", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PRIMARY", x => new { x.idCategoria, x.idItem });
                table.ForeignKey(
                    name: "fk_categoria_has_item_categoria",
                    column: x => x.idCategoria,
                    principalTable: "categoria",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_categoria_has_item_item",
                    column: x => x.idItem,
                    principalTable: "item",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_categoria_item_idItem",
            table: "categoria_item",
            column: "idItem");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "categoria_item");

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
}
