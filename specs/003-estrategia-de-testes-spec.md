# 003 - Estrategia de Testes

## Objetivo

Definir a evidencia automatizada de corretude, integracao e resiliencia.

## Testes unitarios

- Regras de `Lancamento` e `Dinheiro`.
- Validadores.
- Handlers da Application.
- Extension methods de mapeamento.

## Testes de integracao

- Persistencia e migrations.
- Idempotencia.
- Outbox e Inbox.
- Endpoints HTTP e autorizacao.

## Testes de carga e resiliencia

- Pelo menos 50 mensagens por segundo na consolidacao.
- Function indisponivel sem interromper novos lancamentos.
- Reprocessamento apos retorno da Function.
- Evento duplicado sem duplicar saldo.

## Criterios de aceite

- Todo requisito funcional relevante possui teste automatizado correspondente.
- Os cenarios de falha documentam comportamento esperado.
