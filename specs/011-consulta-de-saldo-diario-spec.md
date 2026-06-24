# 011 - Consulta de Saldo Diario

## Objetivo

Disponibilizar ao comerciante o saldo consolidado de uma data de negocio.

## Endpoints

- `GET /v1/saldo?data=YYYY-MM-DD`.
- `GET /v1/lancamentos?data=YYYY-MM-DD` para suportar a tabela do frontend.

## Regras

- O comerciante vem do token autenticado.
- A consulta de saldo acessa somente o modelo de leitura consolidado.
- A consulta de lancamentos acessa somente os lancamentos do comerciante e da data informada.
- Quando a data nao e informada, utilizar o dia atual no fuso de negocio.

## Resposta de saldo

Inclui data, total de creditos, total de debitos, saldo e data da ultima atualizacao.

## Criterios de aceite

- Nenhuma consulta de saldo depende da Function estar online naquele momento.
- A API comunica que o saldo pode ter consistencia eventual.
