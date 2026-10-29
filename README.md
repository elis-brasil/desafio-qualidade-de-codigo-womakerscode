# Desafio ECommerce - Refatoração

API .NET (ASP.NET Core Web API, `net10.0`) que simula um monólito de e-commerce.
O código é **intencionalmente ruim**, serve como ponto de partida para exercícios
de refatoração, aplicação de SOLID, organização em camadas e boas práticas.

## Como rodar

```bash
cd EcommercePiorApi
dotnet run
```

O banco SQLite (`ecommerce.db`) é criado automaticamente na primeira execução,
com alguns produtos de exemplo já cadastrados. Swagger disponível em `/swagger`:

```text
http://localhost:5000/swagger/index.html
```

## Domínios cobertos

- CRUD de produtos
- Estoque (baixa e reabastecimento)
- Criação de pedidos (calcula total, valida estoque, aplica desconto)
- Pagamento mockado (`PagamentoMock`)
- Logística mockada (`LogisticaMock`, gera código de rastreio)

Tudo isso vive no mesmo projeto, sem separação por domínio.

## Problemas propositais (para as alunas encontrarem e corrigirem)

- **God Controller**: `EcommerceController` concentra produtos, estoque, pedidos,
  pagamento e logística — viola SRP feio.
- **Sem camadas**: não há Domain/Application/Infrastructure, nem repositórios,
  nem services com interface. Tudo é `static` ou direto no controller.
- **Sem injeção de dependência para dados**: cada método faz `new AppDbContext()`
  na mão em vez de injetar via DI.
- **DbContext com connection string fixa** (`OnConfiguring` hardcoded), ignorando
  configuração via `appsettings.json`/DI.
- **Entidades expostas diretamente na API** (sem DTOs de request/response),
  inclusive recebendo `Produto` completo no `POST`/`PUT`.
- **Regra de negócio (desconto) com números mágicos** espalhada no meio do fluxo
  de criação de pedido.
- **Falta de transação/atomicidade**: o estoque é debitado antes do pagamento
  ser confirmado e **não é estornado** se o pagamento falhar (bug proposital).
- **Chamadas assíncronas usadas de forma bloqueante** (`.Result` em vez de `await`).
- **Duplicação de código**: `ReprocessarPagamento` e `ReenviarLogistica` repetem
  lógica que já existe em `CriarPedido`; `ListarEstoque` repete `ListarProdutos`.
- **Classe utilitária "faz-tudo"** (`Utils`): validação de e-mail, log em arquivo
  e geração de código de rastreio, tudo junto.
- **Mock de pagamento com `Random` local e `Thread.Sleep`**, decidindo uma regra
  de negócio crítica por sorteio.
- **Tratamento de erro genérico/ausente**: sem exceptions customizadas, sem
  middleware de erro, log engolindo exceções em `catch { }`.
- **Status como `string` solta** (`"Pendente"`, `"Pago"`, `"Falhou"`, `"Enviado"`)
  em vez de enum.
- **Tudo em um único projeto/pasta**, sem organização por pastas
  (Models, Controllers, Services e DTOs misturados).

## Comece assim

Este projeto foi construído com vários problemas propositais e desafio é identificar esses problemas e refatorar o código aplicando os conceitos vistos em aula, camadas, SOLID, injeção de dependência, padrões de projeto, tratamento de erros, entre outros.

Não existe uma única forma "certa" de resolver: use o que fizer mais sentido para cada problema encontrado. Algumas perguntas para guiar o raciocínio:

- Onde estão as responsabilidades misturadas? Como separá-las em camadas
  coesas?
- Que princípios SOLID estão sendo violados e como corrigi-los?
- Existe algum padrão de projeto que resolveria naturalmente algum dos
  problemas listados (criação de objetos, troca de comportamento em tempo de
  execução, acesso a dados etc.)?
- Como tornar o código testável e mais fácil de manter?
