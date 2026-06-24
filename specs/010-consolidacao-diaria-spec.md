# 010 - Consolidacao Diaria

## Objetivo

Construir o saldo diario consolidado a partir dos eventos de lancamentos.

## Fluxo

```text
Azure Service Bus
  -> Azure Function
  -> validacao de Inbox
  -> atualizacao do saldo diario
  -> confirmacao da mensagem
```

## Modelo de leitura

`DailyBalance` possui comerciante, data de negocio, total de creditos, total de debitos, saldo e data da ultima atualizacao UTC.

## Regras

- O evento e processado uma unica vez de forma logica, usando Inbox.
- Credito aumenta total de creditos e saldo.
- Debito aumenta total de debitos e reduz saldo.
- Lancamentos tardios atualizam a data de negocio original.
- O fuso horario de negocio precisa ser definido antes da implementacao final.

## Meta nao funcional

- Suportar pelo menos 50 mensagens por segundo.
- Buscar perda zero para mensagens ja aceitas; o limite do desafio e de ate 5%.

## Criterios de aceite

- Eventos duplicados nao alteram o saldo duas vezes.
- A indisponibilidade da Function nao impede novos lancamentos.
