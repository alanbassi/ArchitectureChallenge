# Plano de Execucao - Backend

## Objetivo

Implementar o backend do Sistema de Controle de Fluxo de Caixa conforme os diagramas LikeC4 e os requisitos do desafio. O sistema deve registrar lancamentos de debito e credito e disponibilizar o saldo diario consolidado por comerciante.

O foco inicial e somente o backend. O frontend React, o provisionamento completo em Azure e outras evolucoes serao tratados em uma etapa posterior.

## Decisoes tecnicas

| Area | Decisao |
| --- | --- |
| Plataforma | .NET 10 e C# |
| Base estrutural | Equinox Project, adaptado para este desafio |
| Arquitetura | Clean Architecture, DDD e CQRS |
| Commands e queries | [Mediator](https://github.com/martinothamar/Mediator) |
| Validacao | FluentValidation |
| Mapeamento | Extension methods manuais |
| Persistencia | EF Core e Azure SQL Database |
| Mensageria | Azure Service Bus |
| Consolidacao | Azure Functions isolated worker |
| Seguranca | Microsoft Entra ID e Azure API Management |
| Desenvolvimento local | A definir em uma etapa futura |
| Observabilidade | OpenTelemetry |
| Infraestrutura futura | Bicep |

Nao serao usados MediatR, AutoMapper ou Mapperly.

## Escopo desta etapa

Esta etapa cobre o planejamento e a implementacao do backend. O trabalho sera entregue em incrementos, iniciando pela API de lancamentos.

### Incluido

- API de controle de lancamentos.
- API de consulta do saldo diario.
- Processamento assincrono da consolidacao.
- Persistencia do Ledger e do saldo consolidado.
- Idempotencia, Outbox e Inbox.
- Autenticacao e autorizacao preparadas para Entra ID.
- Integracao arquitetural com Azure API Management.
- Testes unitarios, de integracao, carga e resiliencia.
- Documentacao de execucao local e decisoes arquiteturais.

### Fora do escopo por enquanto

- Frontend React.
- Provisionamento real de recursos Azure.
- Pipeline de CI/CD.
- Deploy em ambiente produtivo.
- Cache distribuido, salvo se os testes demonstrarem necessidade.

## Estrutura da solucao

```text
CashFlow.sln
|
|- src/
|  |- 01.Presentation/
|  |  |- CashFlow.Ledger.Api
|  |  `- CashFlow.Balance.Api
|  |- 02.Application/
|  |  |- CashFlow.Ledger.Application
|  |  `- CashFlow.Balance.Application
|  |- 03.Domain/
|  |  |- CashFlow.SharedKernel
|  |  |- CashFlow.Ledger.Domain
|  |  `- CashFlow.Balance.Domain
|  |- 04.Infrastructure/
|     |- CashFlow.Ledger.Infrastructure
|     `- CashFlow.Balance.Infrastructure
|  `- 05.Functions/
|     `- CashFlow.Consolidation.Function
|
`- tests/
   |- CashFlow.Ledger.UnitTests
   |- CashFlow.Ledger.IntegrationTests
   |- CashFlow.Balance.IntegrationTests
   |- CashFlow.Consolidation.IntegrationTests
   `- CashFlow.ArchitectureTests
```

Todos os projetos usarao `net10.0`.

## Separacao de responsabilidades

```text
Domain
|- Entidades, agregados e value objects
|- Regras e invariantes de negocio
|- Eventos de dominio locais
`- Contratos de repositorio de dominio

Application
|- Commands e queries
|- Handlers do Mediator
|- Validadores
|- Casos de uso
`- Despacho de eventos de dominio

Infrastructure
|- DbContexts e repositorios EF Core
|- Azure SQL Database
|- Azure Service Bus
|- Persistencia de Outbox e Inbox
`- Integracoes externas

API e Function
|- Endpoints HTTP e gatilhos do Service Bus
|- Autenticacao e autorizacao
|- Registro de dependencias
`- Extension methods de mapeamento
```

Os projetos de Domain nao devem referenciar o Mediator. Commands, queries e handlers pertencem a camada Application.

## Modelo e regras de negocio

### Lancamento

Um lancamento possui, no minimo:

- identificador;
- comerciante;
- tipo: debito ou credito;
- valor monetario em `decimal`;
- data de negocio;
- data de criacao em UTC;
- descricao opcional;
- chave de idempotencia.

O lancamento e imutavel. Uma correcao deve gerar um lancamento compensatorio, sem alterar ou remover o historico existente.

### Saldo diario consolidado

```text
DailyBalance
|- MerchantId
|- BusinessDate
|- TotalCredits
|- TotalDebits
|- Balance
`- LastUpdatedAtUtc
```

O sistema deve definir o fuso horario de negocio do comerciante. Um lancamento tardio atualiza o saldo da data de negocio correta.

## Contratos HTTP

```text
POST /v1/entries
GET  /v1/balance?date=YYYY-MM-DD
```

O endpoint de criacao exige o header `Idempotency-Key`. Quando a data nao for informada na consulta, a API retorna o saldo do dia atual no fuso horario de negocio.

Principais contratos internos:

```text
CreateEntryRequest
RegisterEntryCommand
EntryResponse
EntryRegisteredIntegrationEvent
GetDailyBalanceQuery
DailyBalanceResponse
```

## Fluxo de lancamento

```text
HTTP Request
  -> Controller
  -> FluentValidation
  -> RegisterEntryCommand
  -> Mediator handler
  -> Ledger repository
  -> Transacao SQL
  -> EntryResponse
```

Na mesma transacao SQL serao gravados:

- `LedgerEntry`;
- `IdempotencyRecord`;
- `IntegrationMessage`.

Uma restricao unica para comerciante e chave de idempotencia impede a criacao duplicada de lancamentos.

### Mapeamento

Os mapeamentos simples serao extension methods manuais, por exemplo:

```text
CreateEntryRequest -> RegisterEntryCommand
LedgerEntry -> EntryResponse
DailyBalance -> DailyBalanceResponse
```

As regras de criacao de `LedgerEntry` permanecem no dominio, em construtores ou factories. Extension methods nao devem conter regras de negocio.

## Publicacao de eventos

Um processo de Outbox publica eventos confirmados no Azure Service Bus.

```text
IntegrationMessage -> Azure Service Bus -> EntryRegisteredIntegrationEvent
```

Responsabilidades:

- buscar mensagens ainda nao processadas;
- publicar usando o identificador do evento como `MessageId`;
- marcar a mensagem como processada somente apos sucesso;
- repetir falhas transitorias de forma segura.

O padrao Outbox garante que um lancamento persistido nao seja perdido se a publicacao do evento falhar.

## Consolidacao diaria

A Azure Function consome eventos de lancamento e atualiza a base de leitura.

```text
Azure Service Bus
  -> Azure Function
  -> Validacao de Inbox
  -> Processador de consolidacao
  -> Base de saldo diario consolidado
```

Responsabilidades:

- consumir `EntryRegisteredIntegrationEvent`;
- registrar o identificador do evento em `InboxMessage`;
- ignorar eventos duplicados;
- atualizar totais de credito, debito e saldo para a data de negocio;
- salvar Inbox e projecao na mesma transacao;
- configurar retry e dead-letter queue.

## Consulta de saldo

```text
HTTP Request
  -> Controller
  -> GetDailyBalanceQuery
  -> Mediator handler
  -> Read repository
  -> Base consolidada
  -> HTTP Response
```

A API de saldo consulta somente a base consolidada. Ela nao acessa a base Ledger.

## Seguranca e API Gateway

O Azure API Management e a porta de entrada publica:

```text
Cliente
  -> Microsoft Entra ID
  -> Azure API Management
     |- POST /entries  -> Ledger API
     `- GET /balance   -> Balance API
```

Responsabilidades do gateway:

- validar tokens JWT do Entra ID;
- validar audience e scopes;
- rotear chamadas para as APIs internas;
- aplicar rate limit e limite de tamanho de requisicao;
- adicionar correlation ID quando necessario;
- padronizar erros de borda.

As APIs tambem validam JWT como segunda camada de seguranca. A Azure Function se conecta diretamente ao Azure Service Bus.

## Requisitos nao funcionais

| Requisito | Estrategia |
| --- | --- |
| Consolidacao indisponivel nao pode parar lancamentos | Ledger, Outbox e Service Bus desacoplados do worker |
| Pico de 50 eventos por segundo | Escala automatica da Function, configuracao de concorrencia e indices SQL |
| Perda maxima de 5% | Meta de projeto: perda zero para mensagens ja aceitas, com Outbox, Service Bus, retry, DLQ e Inbox |
| Duplicacao de eventos | `MessageId`, Inbox e atualizacao idempotente |
| Observabilidade | Logs estruturados, traces, metricas, health checks e alertas |
| Dados em transito e repouso | HTTPS/TLS e criptografia gerenciada pelos servicos Azure |

Metricas a acompanhar:

- taxa de lancamentos aceitos;
- tamanho da fila;
- idade da mensagem mais antiga;
- falhas e tentativas do Outbox;
- mensagens em dead-letter queue;
- latencia da API;
- atraso de consolidacao;
- disponibilidade dos servicos.

## Testes

### Unitarios

- regras de dominio;
- calculos monetarios;
- validadores;
- extension methods de mapeamento;
- handlers de command e query.

### Integracao

- persistencia SQL e constraints;
- idempotencia;
- Outbox;
- Inbox;
- consolidacao do saldo diario;
- autenticacao e autorizacao.

### Carga e resiliencia

- processar pelo menos 50 eventos por segundo;
- interromper a Function e confirmar que a Ledger API continua aceitando lancamentos;
- reiniciar a Function e confirmar processamento do acumulado;
- entregar um evento duplicado e confirmar que o saldo nao e alterado duas vezes;
- enviar mensagens para dead-letter queue e reprocessa-las;
- validar lancamento tardio para um dia anterior.

## Documentacao

O README final deve incluir:

- visao geral da arquitetura;
- pre-requisitos locais;
- como iniciar o ambiente local;
- como executar migrations;
- configuracao local de autenticacao;
- exemplos de chamadas HTTP;
- como rodar os testes;
- limitacoes conhecidas e evolucoes futuras.

Tambem serao mantidos no repositorio:

- diagramas LikeC4;
- diagrama de implantacao;
- diagramas de sequencia para registro e consolidacao;
- ADRs para Mediator, mapeamento manual, Outbox/Inbox, API Management e consolidacao diaria.

## Ordem de implementacao

1. Criar a solucao .NET 10 e adaptar a estrutura do Equinox.
2. Implementar o dominio e a Ledger API, com validacao, Mediator, EF Core, idempotencia e testes.
3. Implementar persistencia e publicacao pelo Outbox.
4. Implementar a Azure Function, Inbox e projecao de saldo diario.
5. Implementar a Balance API.
6. Integrar autenticacao, API Management e observabilidade.
7. Executar testes de integracao, carga e resiliencia.
8. Finalizar README, ADRs e diagramas de implantacao e sequencia.

## Primeiro marco de entrega

O primeiro marco implementa apenas a base do backend:

- solucao .NET 10;
- estrutura adaptada do Equinox;
- projeto Domain sem dependencia do Mediator;
- Ledger API;
- FluentValidation;
- commands e handlers na camada Application;
- extension methods para mapeamentos;
- persistencia SQL;
- idempotencia;
- Outbox persistido;
- testes unitarios e de integracao iniciais.

Frontend, deploy em Azure, CI/CD e evolucoes de operacao ficam explicitamente fora deste primeiro marco.
