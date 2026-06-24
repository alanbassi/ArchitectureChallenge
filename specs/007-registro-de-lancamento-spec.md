# 007 - Registro de Lancamento

## Objetivo

Definir o caso de uso para registrar um lancamento de credito ou debito.

## Fluxo

```text
POST /v1/lancamentos
  -> validacao da requisicao
  -> command RegisterLedgerEntryCommand
  -> handler da Application
  -> LedgerEntryService
  -> criacao do LedgerEntry no Domain
  -> persistencia
  -> resposta HTTP
```

## Entradas

- Tipo do lancamento.
- Valor.
- Data de negocio.
- Descricao opcional.
- Chave de idempotencia no header.

## Saidas

- `201 Created` para um novo lancamento.
- Identificador, tipo, valor, data de negocio, descricao e data de registro UTC.

## Dependencias previstas

- Mediator na camada Application.
- FluentValidation na camada Application/Presentation.

## Criterios de aceite

- O `RegisterLedgerEntryCommand` nao depende de tipos HTTP.
- O endpoint nao contem regra de negocio.
