using System.Text.RegularExpressions;
using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Domain.Entities;

namespace PiorEcommerceDoMundo.Application.Services;

/// <summary>
/// Responsável exclusivo por todas as regras de validação de pedidos (Single Responsibility Principle).
/// </summary>
public class PedidoValidator : IPedidoValidator
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public void ValidarRequisicao(PedidoRequest request)
    {
        if (request == null)
            throw new ArgumentException("A requisição do pedido não pode ser nula.");

        if (string.IsNullOrWhiteSpace(request.ClienteNome))
            throw new ArgumentException("Informe o nome do cliente.");

        if (string.IsNullOrWhiteSpace(request.ClienteEmail) || !EmailRegex.IsMatch(request.ClienteEmail))
            throw new ArgumentException("E-mail inválido.");

        if (string.IsNullOrWhiteSpace(request.FormaPagamento))
            throw new ArgumentException("Informe a forma de pagamento.");

        if (request.Itens == null || !request.Itens.Any())
            throw new ArgumentException("Pedido sem itens.");

        foreach (var item in request.Itens)
        {
            if (item.ProdutoId <= 0)
                throw new ArgumentException($"Id do produto inválido: {item.ProdutoId}.");

            if (item.Quantidade <= 0)
                throw new ArgumentException($"A quantidade do produto {item.ProdutoId} deve ser maior que zero.");
        }
    }

    public void ValidarProdutosEEstoque(List<ItemRequest> itens, IDictionary<int, Produto> produtosCadastrados)
    {
        foreach (var item in itens)
        {
            if (!produtosCadastrados.TryGetValue(item.ProdutoId, out var produto) || produto == null)
            {
                throw new KeyNotFoundException($"Produto {item.ProdutoId} não encontrado.");
            }

            if (!produto.Ativo)
            {
                throw new InvalidOperationException($"Produto '{produto.Nome}' está inativo.");
            }

            if (produto.QuantidadeEstoque < item.Quantidade)
            {
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {produto.QuantidadeEstoque}, Solicitado: {item.Quantidade}.");
            }
        }
    }
}
