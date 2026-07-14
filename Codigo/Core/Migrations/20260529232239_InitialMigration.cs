using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    telefone = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false),
                    cpf = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    dataNascimento = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "restaurante",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nomeRestaurante = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    nomeProprietario = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    cpfProprietario = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false),
                    cnpj = table.Column<string>(type: "char(14)", fixedLength: true, maxLength: 14, nullable: false),
                    descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    telefoneProprietario = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false),
                    telefoneRestaurante = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    validado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    rua = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    numero = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    bairro = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    cep = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false),
                    cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    foto = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cartao",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    numero = table.Column<string>(type: "char(16)", fixedLength: true, maxLength: 16, nullable: false),
                    cvv = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false),
                    validade = table.Column<DateTime>(type: "datetime", nullable: false),
                    cpf = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false),
                    idCliente = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Cartao_Cliente1",
                        column: x => x.idCliente,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "endereco",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    rua = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    numero = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    bairro = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    cep = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: false),
                    cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    idCliente = table.Column<uint>(type: "int unsigned", nullable: false),
                    label = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Endereco_Cliente1",
                        column: x => x.idCliente,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "atendimento",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    diaSemana = table.Column<string>(type: "enum('1','2','3','4','5','6','7')", nullable: false, comment: "1-Domingo\n2-Segunda\n3-Terça\n4-Quarta\n5-Quinta\n6-Sexta\n7-Sabado"),
                    horarioInicio = table.Column<DateTime>(type: "datetime", nullable: true),
                    horarioFim = table.Column<DateTime>(type: "datetime", nullable: true),
                    idRestaurante = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Atendimento_Restaurante1",
                        column: x => x.idRestaurante,
                        principalTable: "restaurante",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    calorias = table.Column<float>(type: "float", nullable: false),
                    carboidratos = table.Column<float>(type: "float", nullable: true),
                    gordura = table.Column<float>(type: "float", nullable: true),
                    proteina = table.Column<float>(type: "float", nullable: true),
                    restricao = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    tamanho = table.Column<string>(type: "enum('P','M','G')", nullable: true),
                    volume = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    idRestaurante = table.Column<uint>(type: "int unsigned", nullable: false),
                    foto = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Item_Restaurante1",
                        column: x => x.idRestaurante,
                        principalTable: "restaurante",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pagamento",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    valorMensalidade = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    dataPagamento = table.Column<DateTime>(type: "datetime", nullable: true),
                    dataVencimento = table.Column<DateTime>(type: "datetime", nullable: true),
                    statusMensalidade = table.Column<string>(type: "enum('P','E','A')", nullable: false, comment: "P : Pago\nE: Pendente\nA: Atraso"),
                    idRestaurante = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_pagamento_Restaurante1",
                        column: x => x.idRestaurante,
                        principalTable: "restaurante",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "carrinho",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    observacao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    idCliente = table.Column<uint>(type: "int unsigned", nullable: false),
                    formaDePagamento = table.Column<string>(type: "enum('P','C','D')", nullable: false, comment: "P para PIX,C para CARTÃO, D para DINHEIRO "),
                    valorFrete = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    idCartao = table.Column<uint>(type: "int unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Carrinho_Cartao1",
                        column: x => x.idCartao,
                        principalTable: "cartao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_Carrinho_Cliente1",
                        column: x => x.idCliente,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pedido",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    data = table.Column<DateTime>(type: "datetime", nullable: true),
                    preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    idRestaurante = table.Column<uint>(type: "int unsigned", nullable: false),
                    idCarrinho = table.Column<uint>(type: "int unsigned", nullable: false),
                    status = table.Column<string>(type: "enum('P','E','S','F')", nullable: true, comment: "P=Pendente, E=EmPreparo, S=EmEntrega, F=Finalizado")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Pedido_Carrinho1",
                        column: x => x.idCarrinho,
                        principalTable: "carrinho",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_Pedido_Restaurante1",
                        column: x => x.idRestaurante,
                        principalTable: "restaurante",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "avaliacao",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nota = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    idCliente = table.Column<uint>(type: "int unsigned", nullable: false),
                    idPedido = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Avaliacao_Cliente1",
                        column: x => x.idCliente,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_Avaliacao_Pedido1",
                        column: x => x.idPedido,
                        principalTable: "pedido",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pedidoitem",
                columns: table => new
                {
                    idPedido = table.Column<uint>(type: "int unsigned", nullable: false),
                    idItem = table.Column<uint>(type: "int unsigned", nullable: false),
                    quantidade = table.Column<int>(type: "int", nullable: false),
                    preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idPedido, x.idItem });
                    table.ForeignKey(
                        name: "fk_PedidoItem_Item1",
                        column: x => x.idItem,
                        principalTable: "item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_PedidoItem_Pedido1",
                        column: x => x.idPedido,
                        principalTable: "pedido",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "categoria",
                columns: new[] { "id", "nome" },
                values: new object[,]
                {
                    { 1u, "Vegetariano" },
                    { 2u, "Vegano" },
                    { 3u, "Sem Glúten" },
                    { 4u, "Sem Lactose" },
                    { 5u, "Fitness" },
                    { 6u, "Low Carb" },
                    { 7u, "Zero Lactose" },
                    { 8u, "Proteico" }
                });

            migrationBuilder.CreateIndex(
                name: "fk_Atendimento_Restaurante1_idx",
                table: "atendimento",
                column: "idRestaurante");

            migrationBuilder.CreateIndex(
                name: "fk_Avaliacao_Cliente1_idx",
                table: "avaliacao",
                column: "idCliente");

            migrationBuilder.CreateIndex(
                name: "fk_Avaliacao_Pedido1_idx",
                table: "avaliacao",
                column: "idPedido");

            migrationBuilder.CreateIndex(
                name: "fk_Carrinho_Cartao1_idx",
                table: "carrinho",
                column: "idCartao");

            migrationBuilder.CreateIndex(
                name: "fk_Carrinho_Cliente1_idx",
                table: "carrinho",
                column: "idCliente");

            migrationBuilder.CreateIndex(
                name: "fk_Cartao_Cliente1_idx",
                table: "cartao",
                column: "idCliente");

            migrationBuilder.CreateIndex(
                name: "nome_UNIQUE",
                table: "categoria",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Endereco_Cliente1_idx",
                table: "endereco",
                column: "idCliente");

            migrationBuilder.CreateIndex(
                name: "fk_Item_Restaurante1_idx",
                table: "item",
                column: "idRestaurante");

            migrationBuilder.CreateIndex(
                name: "fk_pagamento_Restaurante1_idx",
                table: "pagamento",
                column: "idRestaurante");

            migrationBuilder.CreateIndex(
                name: "fk_Pedido_Carrinho1_idx",
                table: "pedido",
                column: "idCarrinho");

            migrationBuilder.CreateIndex(
                name: "fk_Pedido_Restaurante1_idx",
                table: "pedido",
                column: "idRestaurante");

            migrationBuilder.CreateIndex(
                name: "fk_PedidoItem_Item1_idx",
                table: "pedidoitem",
                column: "idItem");

            migrationBuilder.CreateIndex(
                name: "fk_PedidoItem_Pedido1_idx",
                table: "pedidoitem",
                column: "idPedido");

            migrationBuilder.CreateIndex(
                name: "cnpj_UNIQUE",
                table: "restaurante",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "nomeRestaurante_UNIQUE",
                table: "restaurante",
                column: "nomeRestaurante",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "atendimento");

            migrationBuilder.DropTable(
                name: "avaliacao");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "endereco");

            migrationBuilder.DropTable(
                name: "pagamento");

            migrationBuilder.DropTable(
                name: "pedidoitem");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "pedido");

            migrationBuilder.DropTable(
                name: "carrinho");

            migrationBuilder.DropTable(
                name: "restaurante");

            migrationBuilder.DropTable(
                name: "cartao");

            migrationBuilder.DropTable(
                name: "cliente");
        }
    }
}
