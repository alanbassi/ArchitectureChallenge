# Arquitetura do Backend

## Objetivo

O backend controla lancamentos de credito e debito de comerciantes e disponibiliza o saldo diario consolidado. A arquitetura segue os diagramas LikeC4 existentes e prioriza independencia entre o registro de lancamentos e a consolidacao diaria.

## Componentes principais

```text
Cliente
  -> Microsoft Entra ID
  -> Azure API Management
     |- Ledger API
     `- Balance API

Ledger API
  -> Base Ledger
  -> Outbox
  -> Azure Service Bus

Azure Service Bus
  -> Consolidation Function
  -> Base de saldo diario consolidado

Balance API
  -> Base de saldo diario consolidado
```

## Limites e responsabilidades

| Componente | Responsabilidade |
| --- | --- |
| Ledger API | Receber, validar e persistir lancamentos com idempotencia. |
| Base Ledger | Manter o historico imutavel dos lancamentos e o Outbox. |
| Outbox publisher | Publicar eventos confirmados no Azure Service Bus. |
| Consolidation Function | Consumir eventos e atualizar o saldo diario consolidado. |
| Base consolidada | Oferecer um modelo de leitura rapido por comerciante e data. |
| Balance API | Consultar somente o modelo de leitura consolidado. |
| API Management | Autenticar, autorizar, aplicar politicas e rotear as chamadas HTTP. |

## Organizacao do codigo

Cada dominio possui as camadas Domain, Application e Infrastructure:

```text
src/
|- 01.Presentation/
|  |- CashFlow.Ledger.Api
|  `- CashFlow.Balance.Api
|- 02.Application/
|  |- CashFlow.Ledger.Application
|  `- CashFlow.Balance.Application
|- 03.Domain/
|  |- CashFlow.SharedKernel
|  |- CashFlow.Ledger.Domain
|  `- CashFlow.Balance.Domain
|- 04.Infrastructure/
   |- CashFlow.Ledger.Infrastructure
   `- CashFlow.Balance.Infrastructure
`- 05.Functions/
   `- CashFlow.Consolidation.Function
```

```text
Domain
|- Entidades, value objects e regras de negocio
|- Eventos de dominio locais
`- Contratos de repositorio de dominio

Application
|- Commands e queries
|- Handlers do Mediator
|- Validadores e casos de uso
`- Orquestracao da aplicacao

Infrastructure
|- EF Core, repositorios e DbContexts
|- Azure SQL, Service Bus, Outbox e Inbox
`- Integracoes externas

API e Function
|- Pontos de entrada HTTP e Service Bus
|- Autenticacao, autorizacao e DI
`- Mapeamentos por extension methods
```

O Domain nao referencia o Mediator. Commands, queries e handlers pertencem a camada Application.

## Persistencia e consistencia

O modelo de escrita e o modelo de leitura sao separados:

- O Ledger registra cada lancamento de forma imutavel.
- O Outbox e salvo na mesma transacao do lancamento.
- O Azure Service Bus desacopla a persistencia do processamento do saldo.
- A Function usa Inbox para ignorar eventos duplicados.
- O saldo diario e eventual e consistentemente atualizado por `MerchantId` e `BusinessDate`.

## Seguranca

- Microsoft Entra ID emite tokens JWT.
- Azure API Management valida tokens, audience e scopes.
- As APIs tambem validam os tokens como segunda camada.
- Comunicacao externa usa HTTPS/TLS.
- Segredos e credenciais de producao deverao usar Managed Identity e Azure Key Vault.

## Requisitos nao funcionais

- A indisponibilidade da consolidacao nao pode impedir novos lancamentos.
- A consolidacao deve suportar pelo menos 50 eventos por segundo.
- A meta de projeto e nenhuma perda de mensagens ja aceitas, usando Outbox, Service Bus, retry, dead-letter queue e Inbox.
- Metricas devem cobrir latencia, tamanho de fila, atraso de consolidacao, falhas de Outbox e mensagens em dead-letter queue.

## Estado atual

Nesta etapa existe apenas a casca compilavel da solucao. Casos de uso, endpoints, persistencia, mensageria, autenticacao e infraestrutura serao implementados somente a partir de especificacoes SDD em `specs`.
