# 004 - Contrato Publico da API

## Objetivo

Definir contratos HTTP estaveis para o frontend e consumidores futuros.

## Registro de lancamento

```http
POST /v1/lancamentos
Idempotency-Key: <uuid>
Authorization: Bearer <token>
```

```json
{
  "tipo": "credito",
  "valor": 150.00,
  "data": "2026-06-23",
  "descricao": "Venda do dia"
}
```

Resposta `201 Created`:

```json
{
  "id": "uuid",
  "tipo": "credito",
  "valor": 150.00,
  "data": "2026-06-23",
  "descricao": "Venda do dia",
  "registradoEmUtc": "2026-06-23T14:30:00Z"
}
```

## Consultas

```http
GET /v1/saldo?data=2026-06-23
GET /v1/lancamentos?data=2026-06-23
```

Resposta de saldo:

```json
{
  "data": "2026-06-23",
  "totalCreditos": 150.00,
  "totalDebitos": 50.00,
  "saldo": 100.00,
  "ultimaAtualizacaoUtc": "2026-06-23T14:30:00Z"
}
```

## Erros

- `400` para dados invalidos.
- `401` para token ausente ou invalido.
- `403` para acesso sem permissao.
- `409` para conflito de idempotencia.
- `500` somente para falha inesperada, sem detalhes internos.
