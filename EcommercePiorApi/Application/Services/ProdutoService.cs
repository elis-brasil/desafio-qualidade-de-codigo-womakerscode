using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Application.Mappings;
using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(IProdutoRepository repository, ILogger<ProdutoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<ProdutoResponse>> ListarAsync()
    {
        var produtos = await _repository.ListarAsync();
        return produtos.Select(p => p.ToResponse()).ToList();
    }

    public async Task<ProdutoResponse> ObterPorIdAsync(int id)
    {
        var produto = await BuscarOuFalharAsync(id);
        return produto.ToResponse();
    }

    public async Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request)
    {
        ValidarDados(request.Nome, request.Preco);

        if (request.QuantidadeEstoqueInicial < 0)
            throw new ArgumentException("A quantidade inicial em estoque não pode ser negativa.");

        var produto = new Produto
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Preco = request.Preco,
            QuantidadeEstoque = request.QuantidadeEstoqueInicial,
            Ativo = true
        };

        await _repository.AdicionarAsync(produto);
        await _repository.SalvarAsync();

        _logger.LogInformation("Produto criado: {Nome} (Id {Id})", produto.Nome, produto.Id);
        return produto.ToResponse();
    }

    public async Task<ProdutoResponse> AtualizarAsync(int id, AtualizarProdutoRequest request)
    {
        ValidarDados(request.Nome, request.Preco);

        var produto = await BuscarOuFalharAsync(id);
        produto.Nome = request.Nome;
        produto.Descricao = request.Descricao;
        produto.Preco = request.Preco;

        // so altera Ativo quando o campo vier no request
        if (request.Ativo.HasValue) produto.Ativo = request.Ativo.Value;

        await _repository.SalvarAsync();
        _logger.LogInformation("Produto atualizado: {Nome} (Id {Id})", produto.Nome, produto.Id);
        return produto.ToResponse();
    }

    public async Task RemoverAsync(int id)
    {
        var produto = await BuscarOuFalharAsync(id);
        _repository.Remover(produto);
        await _repository.SalvarAsync();
        _logger.LogInformation("Produto removido: {Nome} (Id {Id})", produto.Nome, id);
    }

    private async Task<Produto> BuscarOuFalharAsync(int id)
    {
        var produto = await _repository.ObterPorIdAsync(id);
        if (produto == null)
            throw new KeyNotFoundException($"Produto {id} não encontrado.");
        return produto;
    }

    private static void ValidarDados(string nome, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do produto é obrigatório.");
        if (preco <= 0)
            throw new ArgumentException("O preço deve ser maior que zero.");
    }
}