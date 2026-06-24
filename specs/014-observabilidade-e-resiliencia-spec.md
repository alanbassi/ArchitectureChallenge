# 014 - Observabilidade e Resiliencia

## Objetivo

Permitir diagnostico, recuperacao e acompanhamento dos requisitos nao funcionais.

## Observabilidade

- Logs estruturados com correlation ID, identificador do comerciante, lancamento e evento.
- Traces distribuidos entre API, banco, Outbox, Service Bus e Function.
- Health checks de dependencias.

## Metricas

- Latencia e taxa de erros das APIs.
- Lancamentos aceitos por segundo.
- Tamanho e idade da fila.
- Atraso da consolidacao.
- Falhas do Outbox.
- Mensagens em dead-letter queue.

## Resiliencia

- Retry para falhas transitorias.
- Inbox para duplicidade.
- Outbox para indisponibilidade da mensageria.
- Procedimento documentado para reprocessar mensagens.

## Criterios de aceite

- Nenhum token, segredo ou dado pessoal e gravado em logs.
- Falhas podem ser correlacionadas por um identificador de requisicao.
