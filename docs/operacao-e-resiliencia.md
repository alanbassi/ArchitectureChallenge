# Operação e resiliência

## Correlação e logs

As APIs aceitam ou geram `X-Correlation-ID` e o devolvem na resposta. Tokens, connection strings e payloads completos não devem ser registrados.

## Health checks

- Ledger API: `GET /health`.
- Balance API: `GET /health`.

## Mensageria e recuperação

- Mensagens de integração pendentes permanecem no banco quando o Service Bus está indisponível.
- `PublishedAtUtc` só é preenchido após a confirmação do broker.
- A Function usa Inbox para ignorar eventos duplicados.
- Para reprocessar, reenvie a mensagem da dead-letter queue para a entidade configurada e acompanhe o `eventId` e o correlation ID.

## Segurança

Autenticação JWT, Microsoft Entra ID e Azure API Management não estão implementados no estado atual. Devem ser adicionados antes de uma exposição pública.
