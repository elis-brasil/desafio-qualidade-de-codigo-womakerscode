using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo.Application.DTOs;
using PiorEcommerceDoMundo.Domain.Entities;
using PiorEcommerceDoMundo.Infrastructure.Data;
using PiorEcommerceDoMundo.Infrastructure.Mocks;
using PiorEcommerceDoMundo.Shared;

namespace PiorEcommerceDoMundo.Api.Controllers;

// um unico controller cuidando de produtos, estoque, pedidos, pagamento e logistica.
// nada de camadas, nada de injecao de dependencia, nada de SOLID. de proposito.
[ApiController]
public class EcommerceController : ControllerBase
{
    private readonly AppDbContext _db;

    public EcommerceController(AppDbContext db)
    {
        _db = db;
    }

    // ==================== PEDIDOS (o fluxo inteiro numa unica funcao gigante) ====================

    [HttpPost("api/pedidos")]
    public IActionResult CriarPedido([FromBody] PedidoRequest request)
    {
        if (request.ClienteNome == null || request.ClienteNome == "")
        {
            return BadRequest("informe o nome do cliente");
        }
        if (!Utils.ValidarEmail(request.ClienteEmail))
        {
            return BadRequest("email invalido");
        }
        if (request.Itens == null || request.Itens.Count == 0)
        {
            return BadRequest("pedido sem itens");
        }


        var pedido = new Pedido();
        pedido.ClienteNome = request.ClienteNome;
        pedido.ClienteEmail = request.ClienteEmail;
        pedido.DataPedido = DateTime.Now;
        pedido.FormaPagamento = request.FormaPagamento;
        pedido.Status = "Pendente";
        pedido.Itens = new List<ItemPedido>();

        decimal total = 0;

        foreach (var itemReq in request.Itens)
        {
            var produto = _db.Produtos.Find(itemReq.ProdutoId);
            if (produto == null)
            {
                return BadRequest("produto " + itemReq.ProdutoId + " nao encontrado");
            }
            if (!produto.Ativo)
            {
                return BadRequest("produto " + produto.Nome + " esta inativo");
            }
            if (produto.QuantidadeEstoque < itemReq.Quantidade)
            {
                return BadRequest("estoque insuficiente para " + produto.Nome);
            }

            // da baixa no estoque direto aqui dentro do loop, sem transacao com o pagamento
            produto.QuantidadeEstoque = produto.QuantidadeEstoque - itemReq.Quantidade;

            var item = new ItemPedido();
            item.ProdutoId = produto.Id;
            item.Quantidade = itemReq.Quantidade;
            item.PrecoUnitario = produto.Preco;
            pedido.Itens.Add(item);

            total = total + (produto.Preco * itemReq.Quantidade);
        }

        // regra de desconto arbitraria, com numeros magicos, direto no meio do fluxo do pedido
        if (total > 500)
        {
            total = total - (total * 0.10m);
        }
        else if (request.ClienteEmail.Contains("vip"))
        {
            total = total - (total * 0.20m);
        }

        pedido.ValorTotal = total;

        _db.Pedidos.Add(pedido);
        _db.SaveChanges(); // ja salva o pedido e o estoque debitado antes mesmo de saber se o pagamento passa

        bool pagamentoOk = PagamentoMock.ProcessarPagamento(total, request.FormaPagamento);

        if (!pagamentoOk)
        {
            pedido.Status = "Falhou";
            _db.SaveChanges();
            Utils.Log("Pagamento falhou pedido " + pedido.Id);
            // estoque ja foi debitado la em cima e NUNCA eh estornado se o pagamento falhar (bug conhecido)
            return BadRequest("pagamento recusado");
        }

        pedido.Status = "Pago";
        _db.SaveChanges();

        var codigoRastreio = LogisticaMock.EnviarAsync(pedido).Result; // chamada assincrona usada de forma bloqueante
        pedido.CodigoRastreio = codigoRastreio;
        pedido.Status = "Enviado";
        _db.SaveChanges();

        Utils.Log("Pedido " + pedido.Id + " criado com sucesso");

        return Ok(pedido);
    }

    [HttpGet("api/pedidos")]
    public IActionResult ListarPedidos()
    {
        var lista = _db.Pedidos.Include(p => p.Itens).ToList();
        return Ok(lista);
    }

    [HttpGet("api/pedidos/{id}")]
    public IActionResult BuscarPedido(int id)
    {
        var pedido = _db.Pedidos.Include(p => p.Itens).FirstOrDefault(p => p.Id == id);
        if (pedido == null) return NotFound();
        return Ok(pedido);
    }

    // ==================== LOGISTICA (idem, copiado do fluxo de criar pedido) ====================

    [HttpPost("api/logistica/reenviar/{pedidoId}")]
    public IActionResult ReenviarLogistica(int pedidoId)
    {
        var pedido = _db.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        var codigo = LogisticaMock.EnviarAsync(pedido).Result;
        pedido.CodigoRastreio = codigo;
        pedido.Status = "Enviado";
        _db.SaveChanges();
        return Ok(pedido);
    }
}
