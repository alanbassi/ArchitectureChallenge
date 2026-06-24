# 012 - Seguranca e API Gateway

> Status: pendente. Esta especificacao descreve uma evolucao futura e nao esta implementada no estado atual.

## Objetivo

Definir protecao de acesso e entrada publica da aplicacao.

## Componentes

- Microsoft Entra ID para autenticacao.
- Azure API Management como gateway publico.
- APIs .NET validando JWT como segunda camada.

## Regras

- O token deve possuir audience e scope validos.
- O identificador do comerciante e obtido de uma claim definida.
- API Management aplica rate limit, limite de tamanho e correlation ID.
- A Function nao passa pelo gateway; ela consome o Service Bus diretamente.

## Seguranca de dados

- HTTPS/TLS em transito.
- Criptografia gerenciada em repouso.
- Segredos produtivos em Key Vault e acesso por Managed Identity.

## Criterios de aceite

- Requisicoes sem token valido sao rejeitadas.
- Um comerciante nunca consulta ou registra dados de outro comerciante.
