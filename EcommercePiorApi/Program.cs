using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // gambiarra pra nao quebrar quando serializar Pedido -> Itens -> Pedido (referencia ciclica)
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// cria o banco e ja popula uns produtos, tudo direto aqui no Program.cs mesmo
using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();

    if (!db.Produtos.Any())
    {
        db.Produtos.Add(new Produto { Nome = "Camiseta", Descricao = "Camiseta basica", Preco = 49.90m, QuantidadeEstoque = 100, Ativo = true });
        db.Produtos.Add(new Produto { Nome = "Tenis", Descricao = "Tenis esportivo", Preco = 299.90m, QuantidadeEstoque = 50, Ativo = true });
        db.Produtos.Add(new Produto { Nome = "Bone", Descricao = "Bone ajustavel", Preco = 39.90m, QuantidadeEstoque = 200, Ativo = true });
        db.Produtos.Add(new Produto { Nome = "Mochila", Descricao = "Mochila para notebook", Preco = 159.90m, QuantidadeEstoque = 30, Ativo = true });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
