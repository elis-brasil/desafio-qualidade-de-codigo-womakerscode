using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Produtos.Any()) return;

        db.Produtos.AddRange(
            new Produto { Nome = "Camiseta", Descricao = "Camiseta basica", Preco = 49.90m, QuantidadeEstoque = 100, Ativo = true },
            new Produto { Nome = "Tenis", Descricao = "Tenis esportivo", Preco = 299.90m, QuantidadeEstoque = 50, Ativo = true },
            new Produto { Nome = "Bone", Descricao = "Bone ajustavel", Preco = 39.90m, QuantidadeEstoque = 200, Ativo = true },
            new Produto { Nome = "Mochila", Descricao = "Mochila para notebook", Preco = 159.90m, QuantidadeEstoque = 30, Ativo = true }
        );
        db.SaveChanges();
    }
}