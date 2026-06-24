# 009 - Mensageria de Lancamentos

## Objetivo

Definir a comunicacao assincrona entre o servico de lancamentos e a consolidacao diaria.

## Tecnologia

Azure Service Bus.

## Evento

`LedgerEntryRegistered` contem:

- Identificador do evento.
- Identificador do lancamento.
- Identificador do comerciante.
- Tipo, valor e data de negocio.
- Data de ocorrencia UTC.
- Versao do contrato.

## Regras operacionais

- Entrega pode ocorrer mais de uma vez.
- Falhas repetidas seguem para dead-letter queue.
- Mensagens em dead-letter devem permitir diagnostico e reprocessamento controlado.

## Criterios de aceite

- O contrato do evento nao expoe dados sensiveis desnecessarios.
- A versao permite evolucao sem quebrar consumidores existentes.
