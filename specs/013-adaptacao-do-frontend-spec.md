# 013 - Adaptacao do Frontend

## Objetivo

Adaptar o frontend React existente para consumir o backend real de fluxo de caixa.

## Ajustes funcionais

- Substituir `/transactions` por `/v1/lancamentos`.
- Remover MirageJS e dados simulados.
- Substituir o calculo local do resumo pela resposta de `/v1/saldo`.
- Adicionar seletor de data de negocio.
- Alterar o formulario para tipo, valor, data e descricao.
- Remover categoria, pois nao e requisito do desafio.
- Exibir a lista devolvida por `/v1/lancamentos`.

## Integracao HTTP

- URL base por variavel de ambiente `REACT_APP_API_URL`.
- Enviar `Idempotency-Key` em cada criacao.
- Preparar interceptor para token Bearer do Entra ID.
- Exibir estados de carregamento, vazio e erro.

## Regras de experiencia

- O frontend nao calcula saldo consolidado.
- Apos criar um lancamento, recarrega saldo e lista da data selecionada.
- Deve informar que o saldo pode levar alguns instantes para refletir um novo lancamento.

## Criterios de aceite

- Nenhum endpoint simulado permanece em uso.
- O formulario envia o contrato definido na especificacao 004.
- A tela apresenta valores retornados pelo backend.
