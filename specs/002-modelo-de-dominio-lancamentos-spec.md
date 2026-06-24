# 002 - Modelo de Dominio de Lancamentos

## Objetivo

Definir o modelo de negocio para registrar movimentos financeiros diarios.

## Conceitos

- `LedgerEntry`: entidade imutavel que representa um credito ou debito.
- `EntryType`: `Credit` ou `Debit`.
- `Money`: value object com valor decimal positivo e moeda BRL inicial.
- `LedgerEntryService`: domain service que cria um `LedgerEntry` valido.

## Regras

- Um lancamento deve ter valor maior que zero.
- Um lancamento deve ter tipo valido e data de negocio obrigatoria.
- O comerciante e identificado pelo token autenticado, nunca pelo corpo da requisicao.
- Um lancamento confirmado nao pode ser alterado ou excluido.
- Correcao financeira deve gerar lancamento compensatorio.

## Decisao de nomenclatura

Os nomes de classes, entidades e interfaces do codigo usam ingles. A linguagem descritiva da especificacao permanece em portugues.

## Criterios de aceite

- As regras podem ser testadas sem banco, HTTP ou mensageria.
- Nao existe dependencia de Mediator, EF Core ou ASP.NET Core no Domain.
