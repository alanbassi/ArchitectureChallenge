# 008 - Outbox de Eventos

## Objetivo

Garantir que cada lancamento persistido gere uma mensagem para consolidacao, mesmo quando a mensageria estiver temporariamente indisponivel.

## Fluxo

```text
Transacao SQL
  -> Lancamento
  -> Registro de idempotencia
  -> Mensagem Outbox

Publicador Outbox
  -> Azure Service Bus
  -> marca mensagem como publicada
```

## Regras

- O publicador tenta novamente falhas transitorias.
- O identificador da mensagem Outbox e usado como `MessageId`.
- Uma mensagem nao e marcada como publicada antes da confirmacao do broker.

## Criterios de aceite

- A queda da mensageria nao impede a confirmacao do lancamento.
- Uma mensagem pendente pode ser publicada posteriormente sem perda.
