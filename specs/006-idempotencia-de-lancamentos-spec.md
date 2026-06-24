# 006 - Idempotencia de Lancamentos

## Objetivo

Impedir que repeticoes de uma mesma solicitacao criem mais de um lancamento.

## Contrato

- `POST /v1/lancamentos` exige o header `Idempotency-Key`.
- A chave e unica por comerciante.
- A mesma chave e a mesma requisicao retornam o resultado original.
- A mesma chave com conteudo diferente retorna `409 Conflict`.

## Persistencia

O registro de idempotencia deve ser salvo na mesma transacao do lancamento e do Outbox.

## Frontend

- O navegador gera uma chave UUID para cada envio.
- Em uma repeticao por falha de rede, reutiliza a mesma chave da tentativa original.

## Criterios de aceite

- Duas solicitacoes identicas nao criam dois lancamentos.
- Duas solicitacoes diferentes com a mesma chave sao rejeitadas.
