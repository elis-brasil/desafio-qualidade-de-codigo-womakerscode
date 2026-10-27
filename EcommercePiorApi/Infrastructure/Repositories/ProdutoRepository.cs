using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Infrastructure.Data;

namespace PiorEcommerceDoMundo.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _db;

    public ProdutoRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Produto>> ListarAsync()
    {
        return _db.Produtos.ToListAsync();
    }

    public async Task<Produto> ObterPorIdAsync(int id)
    {
        return await _db.Produtos.FindAsync(id);
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _db.Produtos.AddAsync(produto);
    }

    public void Remover(Produto produto)
    {
        _db.Produtos.Remove(produto);
    }

    public Task SalvarAsync()
    {
        return _db.SaveChangesAsync();
    }
}