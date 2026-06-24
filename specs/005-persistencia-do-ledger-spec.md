# 005 - Persistencia do Ledger

## Objetivo

Persistir o historico imutavel de lancamentos e os dados necessarios para idempotencia e Outbox.

## Tecnologia

- EF Core.
- Modo de Inicializacao com provedor EF Core InMemory enquanto os recursos definitivos nao estao disponiveis.
- SQL Server/Azure SQL preparado por configuracao para o ambiente de producao.

## Estruturas

- Tabela `Lancamentos`.
- Tabela `RegistrosIdempotencia`.
- Tabela `MensagensOutbox`.

## Indices minimos

- Comerciante e data de negocio para consultas diarias.
- Comerciante e chave de idempotencia com unicidade.
- Mensagens Outbox pendentes por data de criacao.

## Criterios de aceite

- O Modo de Inicializacao persiste lancamentos em memoria durante a execucao da aplicacao.
- A configuracao permite trocar para SQL Server sem alterar o codigo de negocio.
- Migrations versionam o esquema quando o provedor SQL Server for ativado.
- Lancamento, idempotencia e Outbox serao gravados em uma unica transacao apos a implementacao das especificacoes 006 e 008.
