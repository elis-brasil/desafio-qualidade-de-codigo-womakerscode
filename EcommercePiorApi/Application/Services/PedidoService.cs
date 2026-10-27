using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Application.Interfaces;
using PiorEcommerceDoMundo.Application.Mappings;
using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Domain.Enums;
using PiorEcommerceDoMundo.Infrastructure.Mocks;

namespace PiorEcommerceDoMundo.Application.Services;

/// <summary>
/// Serviço de orquestração de pedidos.
/// Separa validação, cálculo e criação/persistência de pedidos.
/// Corrige chamadas assíncronas bloqueantes (await ao invés de .Result).
/// </summary>
public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IEstoqueService _estoqueService;
    private readonly ICalculadoraDesconto _calculadoraDesconto;
    private readonly IPedidoValidator _pedidoValidator;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        IEstoqueService estoqueService,
        ICalculadoraDesconto calculadoraDesconto,
        IPedidoValidator pedidoValidator,
        ILogger<PedidoService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _estoqueService = estoqueService;
        _calculadoraDesconto = calculadoraDesconto;
        _pedidoValidator = pedidoValidator;
        _logger = logger;
    }

    public async Task<List<PedidoResponse>> ListarAsync()
    {
        var pedidos = await _pedidoRepository.ListarAsync();
        return pedidos.Select(p => p.ToResponse()).ToList();
    }

    public async Task<PedidoResponse> ObterPorIdAsync(int id)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id);
        if (pedido == null)
            throw new KeyNotFoundException($"Pedido {id} não encontrado.");

        return pedido.ToResponse();
    }

    public async Task<PedidoResponse> CriarAsync(PedidoRequest request)
    {
        // 1. Validação básica da requisição
        _pedidoValidator.ValidarRequisicao(request);

        // 2. Carregar produtos envolvidos e validar disponibilidade de estoque
        var produtos = new Dictionary<int, Produto>();
        foreach (var item in request.Itens)
        {
            if (!produtos.ContainsKey(item.ProdutoId))
            {
                var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                if (produto != null)
                {
                    produtos[item.ProdutoId] = produto;
                }
            }
        }

        _pedidoValidator.ValidarProdutosEEstoque(request.Itens, produtos);

        // 3. Montar itens e calcular o subtotal bruto
        decimal subtotal = 0m;
        var itensPedido = new List<ItemPedido>();

        foreach (var itemReq in request.Itens)
        {
            var produto = produtos[itemReq.ProdutoId];
            itensPedido.Add(new ItemPedido
            {
                ProdutoId = produto.Id,
                Produto = produto,
                Quantidade = itemReq.Quantidade,
                PrecoUnitario = produto.Preco
            });

            subtotal += produto.Preco * itemReq.Quantidade;
        }

        // 4. Calcular desconto aplicando Strategy Pattern (sem números mágicos)
        var valorDesconto = _calculadoraDesconto.CalcularDesconto(subtotal, request.ClienteEmail);
        var valorTotal = Math.Max(0m, subtotal - valorDesconto);

        // 5. Criar entidade com enum StatusPedido (Status = Pendente)
        var pedido = new Pedido
        {
            ClienteNome = request.ClienteNome,
            ClienteEmail = request.ClienteEmail,
            DataPedido = DateTime.UtcNow,
            FormaPagamento = request.FormaPagamento,
            Status = StatusPedido.Pendente,
            ValorDesconto = valorDesconto,
            ValorTotal = valorTotal,
            Itens = itensPedido
        };

        await _pedidoRepository.AdicionarAsync(pedido);
        await _pedidoRepository.SalvarAsync();

        // 6. Atualizar estoque com controle de rollback caso o pagamento falhe
        var itensDebitaods = new List<(int ProdutoId, int Quantidade)>();
        foreach (var item in request.Itens)
        {
            await _estoqueService.DebitarAsync(item.ProdutoId, item.Quantidade);
            itensDebitaods.Add((item.ProdutoId, item.Quantidade));
        }

        // 7. Processar pagamento
        bool pagamentoAprovado = PagamentoMock.ProcessarPagamento(valorTotal, request.FormaPagamento);

        if (!pagamentoAprovado)
        {
            pedido.Status = StatusPedido.Falhou;
            await _pedidoRepository.SalvarAsync();

            // Estorna o estoque debitado para corrigir o bug de falta de transação/estorno
            foreach (var (produtoId, quantidade) in itensDebitaods)
            {
                await _estoqueService.EstornarAsync(produtoId, quantidade);
            }

            _logger.LogWarning("Pagamento recusado para o pedido {PedidoId}. Estoque estornado.", pedido.Id);
            throw new InvalidOperationException("Pagamento recusado.");
        }

        pedido.Status = StatusPedido.Pago;
        await _pedidoRepository.SalvarAsync();

        // 8. Logística 100% assíncrona (await ao invés do bloqueante .Result)
        var codigoRastreio = await LogisticaMock.EnviarAsync(pedido);
        pedido.CodigoRastreio = codigoRastreio;
        pedido.Status = StatusPedido.Enviado;
        await _pedidoRepository.SalvarAsync();

        _logger.LogInformation("Pedido {PedidoId} concluído com sucesso e enviado ({CodigoRastreio}).",
            pedido.Id, codigoRastreio);

        return pedido.ToResponse();
    }
}
