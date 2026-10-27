using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Application.Mappings;
using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Services;

public class EstoqueService : IEstoqueService
{
    private readonly IProdutoRepository _repository;
    private readonly ILogger<EstoqueService> _logger;

    public EstoqueService(IProdutoRepository repository, ILogger<EstoqueService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<EstoqueResponse>> ListarAsync()
    {
        var produtos = await _repository.ListarAsync();
        return produtos.Select(p => p.ToEstoqueResponse()).ToList();
    }

    public async Task<EstoqueResponse> ReabastecerAsync(int produtoId, int quantidade)
    {
        ValidarQuantidade(quantidade);
        var produto = await BuscarOuFalharAsync(produtoId);

        if (quantidade > int.MaxValue - produto.QuantidadeEstoque) throw new ArgumentException("A quantidade ultrapassa o limite de estoque deste produto.");

        produto.QuantidadeEstoque += quantidade;
        await _repository.SalvarAsync();

        _logger.LogInformation("Estoque reabastecido: produto {Id} (+{Quantidade})", produtoId, quantidade);
        return produto.ToEstoqueResponse();
    }

    public async Task DebitarAsync(int produtoId, int quantidade)
    {
        ValidarQuantidade(quantidade);
        var produto = await BuscarOuFalharAsync(produtoId);

        if (!produto.Ativo) throw new InvalidOperationException($"O produto {produto.Nome} está inativo.");

        if (produto.QuantidadeEstoque < quantidade)
            throw new InvalidOperationException($"Estoque insuficiente para {produto.Nome}.");

        produto.QuantidadeEstoque -= quantidade;
        await _repository.SalvarAsync();

        _logger.LogInformation("Estoque debitado: produto {Id} (-{Quantidade})", produtoId, quantidade);
    }

    public async Task EstornarAsync(int produtoId, int quantidade)
    {
        ValidarQuantidade(quantidade);
        var produto = await BuscarOuFalharAsync(produtoId);

        if (quantidade > int.MaxValue - produto.QuantidadeEstoque) throw new ArgumentException("A quantidade estornada ultrapassa o limite de estoque deste produto.");

        produto.QuantidadeEstoque += quantidade;
        await _repository.SalvarAsync();

        _logger.LogInformation("Estoque estornado: produto {Id} (+{Quantidade})", produtoId, quantidade);
    }

    private async Task<Produto> BuscarOuFalharAsync(int produtoId)
    {
        var produto = await _repository.ObterPorIdAsync(produtoId);
        if (produto == null)
            throw new KeyNotFoundException($"Produto {produtoId} não encontrado.");
        return produto;
    }

    private static void ValidarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");
    }
}