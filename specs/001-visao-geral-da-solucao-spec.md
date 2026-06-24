# 001 - Visao Geral da Solucao

## Objetivo

Definir a visao funcional e os limites da solucao de fluxo de caixa diario para comerciantes.

## Diretrizes

Os requisitos de negocio e nao funcionais acordados orientam as prioridades desta solucao.

## Escopo

- Registrar lancamentos de credito e debito.
- Exibir saldo diario consolidado.
- Manter o registro de lancamentos disponivel quando a consolidacao estiver indisponivel.
- Disponibilizar frontend React integrado ao backend .NET.

## Limites arquiteturais

- `01.Presentation`: APIs HTTP e DTOs publicos.
- `02.Application`: casos de uso, commands, queries, handlers e validadores.
- `03.Domain`: entidades, value objects e regras de negocio.
- `04.Infrastructure`: persistencia, mensageria e adaptadores externos.
- `05.Functions`: processos hospedados por Azure Functions.

## Fora do escopo inicial

- Deploy produtivo e CI/CD.
- Aplicativo mobile.
- Multi-moeda e conciliacao bancaria.

## Criterios de aceite

- A solucao possui os servicos de lancamentos e saldo diario.
- O frontend utiliza contratos HTTP definidos pelo backend.
- As decisoes arquiteturais estao documentadas no repositorio.
