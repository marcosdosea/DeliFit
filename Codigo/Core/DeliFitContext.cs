using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class DeliFitContext : DbContext
{
    public DeliFitContext()
    {
    }

    public DeliFitContext(DbContextOptions<DeliFitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Atendimento> Atendimentos { get; set; }

    public virtual DbSet<Avaliacao> Avaliacaos { get; set; }

    public virtual DbSet<Carrinho> Carrinhos { get; set; }

    public virtual DbSet<Cartao> Cartaos { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Endereco> Enderecos { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Pagamento> Pagamentos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Pedidoitem> Pedidoitems { get; set; }

    public virtual DbSet<Restaurante> Restaurantes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atendimento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("atendimento");

            entity.HasIndex(e => e.IdRestaurante, "fk_Atendimento_Restaurante1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiaSemana)
                .HasComment("1-Domingo\n2-Segunda\n3-Terça\n4-Quarta\n5-Quinta\n6-Sexta\n7-Sabado")
                .HasColumnType("enum('1','2','3','4','5','6','7')")
                .HasColumnName("diaSemana");
            entity.Property(e => e.HorarioFim)
                .HasColumnType("datetime")
                .HasColumnName("horarioFim");
            entity.Property(e => e.HorarioInicio)
                .HasColumnType("datetime")
                .HasColumnName("horarioInicio");
            entity.Property(e => e.IdRestaurante).HasColumnName("idRestaurante");

            entity.HasOne(d => d.IdRestauranteNavigation).WithMany(p => p.Atendimentos)
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Atendimento_Restaurante1");
        });

        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("avaliacao");

            entity.HasIndex(e => e.IdCliente, "fk_Avaliacao_Cliente1_idx");

            entity.HasIndex(e => e.IdPedido, "fk_Avaliacao_Pedido1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .HasColumnName("descricao");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.IdPedido).HasColumnName("idPedido");
            entity.Property(e => e.Nota)
                .HasPrecision(10)
                .HasColumnName("nota");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Avaliacaos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Avaliacao_Cliente1");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Avaliacaos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Avaliacao_Pedido1");
        });

        modelBuilder.Entity<Carrinho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("carrinho");

            entity.HasIndex(e => e.IdCartao, "fk_Carrinho_Cartao1_idx");

            entity.HasIndex(e => e.IdCliente, "fk_Carrinho_Cliente1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FormaDePagamento)
                .HasComment("P para PIX,C para CARTÃO, D para DINHEIRO ")
                .HasColumnType("enum('P','C','D')")
                .HasColumnName("formaDePagamento");
            entity.Property(e => e.IdCartao).HasColumnName("idCartao");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.Observacao)
                .HasMaxLength(100)
                .HasColumnName("observacao");
            entity.Property(e => e.ValorFrete)
                .HasPrecision(10)
                .HasColumnName("valorFrete");

            entity.HasOne(d => d.IdCartaoNavigation).WithMany(p => p.Carrinhos)
                .HasForeignKey(d => d.IdCartao)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Carrinho_Cartao1");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Carrinhos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Carrinho_Cliente1");
        });

        modelBuilder.Entity<Cartao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cartao");

            entity.HasIndex(e => e.IdCliente, "fk_Cartao_Cliente1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpf");
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("cvv");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Numero)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("numero");
            entity.Property(e => e.Validade)
                .HasColumnType("datetime")
                .HasColumnName("validade");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Cartaos)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("fk_Cartao_Cliente1");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cliente");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpf");
            entity.Property(e => e.DataNascimento)
                .HasColumnType("date")
                .HasColumnName("dataNascimento");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categoria");

            entity.HasIndex(e => e.Nome, "nome_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Endereco>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("endereco");

            entity.HasIndex(e => e.IdCliente, "fk_Endereco_Cliente1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bairro)
                .HasMaxLength(50)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(100)
                .HasColumnName("cidade");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.Label)
                .HasMaxLength(30)
                .HasColumnName("label");
            entity.Property(e => e.Numero)
                .HasMaxLength(10)
                .HasColumnName("numero");
            entity.Property(e => e.Rua)
                .HasMaxLength(50)
                .HasColumnName("rua");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Enderecos)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("fk_Endereco_Cliente1");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("item");

            entity.HasIndex(e => e.IdRestaurante, "fk_Item_Restaurante1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Calorias).HasColumnName("calorias");
            entity.Property(e => e.Carboidratos).HasColumnName("carboidratos");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .HasColumnName("descricao");
            entity.Property(e => e.Foto).HasColumnName("foto");
            entity.Property(e => e.Gordura).HasColumnName("gordura");
            entity.Property(e => e.IdRestaurante).HasColumnName("idRestaurante");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Preco)
                .HasPrecision(10)
                .HasColumnName("preco");
            entity.Property(e => e.Proteina).HasColumnName("proteina");
            entity.Property(e => e.Tamanho)
                .HasColumnType("enum('P','M','G')")
                .HasColumnName("tamanho");
            entity.Property(e => e.Volume)
                .HasMaxLength(10)
                .HasColumnName("volume");

            entity.HasOne(d => d.IdRestauranteNavigation).WithMany(p => p.Items)
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Item_Restaurante1");

            entity.HasMany(d => d.Categorias).WithMany(p => p.Items)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemCategoria",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_ItemCategoria_Categoria1"),
                    l => l.HasOne<Item>().WithMany()
                        .HasForeignKey("IdItem")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_ItemCategoria_Item1"),
                    j =>
                    {
                        j.HasKey("IdItem", "IdCategoria").HasName("PRIMARY");
                        j.ToTable("item_categoria");
                        j.HasIndex("IdCategoria", "fk_ItemCategoria_Categoria1_idx");
                        j.Property("IdItem").HasColumnName("idItem");
                        j.Property("IdCategoria").HasColumnName("idCategoria");
                    });
        });

        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pagamento");

            entity.HasIndex(e => e.IdRestaurante, "fk_pagamento_Restaurante1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataPagamento)
                .HasColumnType("datetime")
                .HasColumnName("dataPagamento");
            entity.Property(e => e.DataVencimento)
                .HasColumnType("datetime")
                .HasColumnName("dataVencimento");
            entity.Property(e => e.IdRestaurante).HasColumnName("idRestaurante");
            entity.Property(e => e.StatusMensalidade)
                .HasComment("P : Pago\nE: Pendente\nA: Atraso")
                .HasColumnType("enum('P','E','A')")
                .HasColumnName("statusMensalidade");
            entity.Property(e => e.ValorMensalidade)
                .HasPrecision(10)
                .HasColumnName("valorMensalidade");

            entity.HasOne(d => d.IdRestauranteNavigation).WithMany(p => p.Pagamentos)
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pagamento_Restaurante1");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pedido");

            entity.HasIndex(e => e.IdCarrinho, "fk_Pedido_Carrinho1_idx");

            entity.HasIndex(e => e.IdRestaurante, "fk_Pedido_Restaurante1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.IdCarrinho).HasColumnName("idCarrinho");
            entity.Property(e => e.IdRestaurante).HasColumnName("idRestaurante");
            entity.Property(e => e.Preco)
                .HasPrecision(10)
                .HasColumnName("preco");
            entity.Property(e => e.Status)
                .HasColumnType("enum('P','E','S','F')")
                .HasComment("P=Pendente, E=EmPreparo, S=EmEntrega, F=Finalizado")
                .HasColumnName("status");

            entity.HasOne(d => d.IdCarrinhoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdCarrinho)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Pedido_Carrinho1");

            entity.HasOne(d => d.IdRestauranteNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_Pedido_Restaurante1");
        });

        modelBuilder.Entity<Pedidoitem>(entity =>
        {
            entity.HasKey(e => new { e.IdPedido, e.IdItem }).HasName("PRIMARY");

            entity.ToTable("pedidoitem");

            entity.HasIndex(e => e.IdItem, "fk_PedidoItem_Item1_idx");

            entity.HasIndex(e => e.IdPedido, "fk_PedidoItem_Pedido1_idx");

            entity.Property(e => e.IdPedido).HasColumnName("idPedido");
            entity.Property(e => e.IdItem).HasColumnName("idItem");
            entity.Property(e => e.Preco)
                .HasPrecision(10)
                .HasColumnName("preco");
            entity.Property(e => e.Quantidade).HasColumnName("quantidade");

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.Pedidoitems)
                .HasForeignKey(d => d.IdItem)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_PedidoItem_Item1");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Pedidoitems)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_PedidoItem_Pedido1");
        });

        modelBuilder.Entity<Restaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("restaurante");

            entity.HasIndex(e => e.Cnpj, "cnpj_UNIQUE").IsUnique();

            entity.HasIndex(e => e.NomeRestaurante, "nomeRestaurante_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bairro)
                .HasMaxLength(50)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(100)
                .HasColumnName("cidade");
            entity.Property(e => e.Cnpj)
                .HasMaxLength(14)
                .IsFixedLength()
                .HasColumnName("cnpj");
            entity.Property(e => e.CpfProprietario)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpfProprietario");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .HasColumnName("descricao");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.Foto).HasColumnName("foto");
            entity.Property(e => e.NomeProprietario)
                .HasMaxLength(50)
                .HasColumnName("nomeProprietario");
            entity.Property(e => e.NomeRestaurante)
                .HasMaxLength(50)
                .HasColumnName("nomeRestaurante");
            entity.Property(e => e.Numero)
                .HasMaxLength(10)
                .HasColumnName("numero");
            entity.Property(e => e.Rua)
                .HasMaxLength(50)
                .HasColumnName("rua");
            entity.Property(e => e.TelefoneProprietario)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("telefoneProprietario");
            entity.Property(e => e.TelefoneRestaurante)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("telefoneRestaurante");
            entity.Property(e => e.Validado).HasColumnName("validado");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
