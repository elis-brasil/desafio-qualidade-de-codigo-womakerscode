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
    private readonly IPagamentoService _pagamentoService;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        IEstoqueService estoqueService,
        ICalculadoraDesconto calculadoraDesconto,
        IPedidoValidator pedidoValidator,
        IPagamentoService pagamentoService,
        ILogger<PedidoService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _estoqueService = estoqueService;
        _calculadoraDesconto = calculadoraDesconto;
        _pedidoValidator = pedidoValidator;
        _pagamentoService = pagamentoService;
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

        // 6. Debitar os itens do estoque
        var itensDebitados = new List<(int ProdutoId, int Quantidade)>();

        try
        {
            foreach (var item in request.Itens)
            {
                await _estoqueService.DebitarAsync(item.ProdutoId, item.Quantidade);

                itensDebitados.Add((item.ProdutoId, item.Quantidade));
            }
        }
        catch
        {
            await EstornarItensAsync(itensDebitados);
            throw;
        }

        // 7. Processar pagamento
        bool pagamentoAprovado;

        try
        {
            pagamentoAprovado = await _pagamentoService.ProcessarPagamentoAsync(valorTotal, request.FormaPagamento);
        }
        catch
        {
            await EstornarItensAsync(itensDebitados);
            throw;
        }

        if (!pagamentoAprovado)
        {
            await EstornarItensAsync(itensDebitados);

            pedido.Status = StatusPedido.Falhou;

            await _pedidoRepository.AdicionarAsync(pedido);
            await _pedidoRepository.SalvarAsync();

            _logger.LogWarning("Pagamento recusado para o pedido {PedidoId}. Estoque estornado.", pedido.Id);
            throw new InvalidOperationException("Pagamento recusado.");
        }

        pedido.Status = StatusPedido.Pago;
        await _pedidoRepository.AdicionarAsync(pedido);
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

    public async Task<PedidoResponse> ReprocessarPagamentoAsync(int pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

        if (pedido == null) throw new KeyNotFoundException($"Pedido {pedidoId} não encontrado.");

        if (pedido.Status != StatusPedido.Falhou) throw new InvalidOperationException("Somente pedidos que o pagamento falhou podem ser reprocessados.");

        var itensDebitados = new List<(int ProdutoId, int Quantidade)>();

        try
        {
            foreach (var item in pedido.Itens)
            {
                await _estoqueService.DebitarAsync(item.ProdutoId, item.Quantidade);
                itensDebitados.Add((item.ProdutoId, item.Quantidade));
            }

            var pagamentoAprovado =
                await _pagamentoService.ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);

            if (!pagamentoAprovado)
            {
                _logger.LogWarning("O pagamento para o pedido {PedidoId} foi recusado novamente.", pedido.Id);
                throw new InvalidOperationException("Pagamento recusado novamente.");
            }

            pedido.Status = StatusPedido.Pago;
            await _pedidoRepository.SalvarAsync();

            itensDebitados.Clear();
        }
        catch
        {
            if (itensDebitados.Count > 0) await EstornarItensAsync(itensDebitados);
            throw;
        }

        _logger.LogInformation("Pagamento reprocessado com sucesso para o pedido {PedidoId}.", pedido.Id);

        return pedido.ToResponse();
    }

    // Elimina a repetição dos laços de estorno
    private async Task EstornarItensAsync(List<(int ProdutoId, int Quantidade)> itens)
    {
        foreach (var item in itens)
        {
            await _estoqueService.EstornarAsync(item.ProdutoId, item.Quantidade);
        }
    }
}
