# AGENTS.md

## Context

CashFlow is a .NET backend application for merchant cash flow management. It has two bounded contexts:

- **Ledger** — registers financial entries (credits and debits) for merchants. Supports idempotent POST via `Idempotency-Key` header. Publishes integration events for downstream consumers.
- **Balance** — consumes ledger events and consolidates daily balances (total credits, total debits, net balance) per merchant and business date. Uses an inbox pattern for idempotent event processing.

Communication between bounded contexts uses the Transactional Outbox pattern (Ledger publishes `IntegrationMessage`) and Inbox pattern (Balance deduplicates via `InboxMessage`). An Azure Function (`CashFlow.Consolidation.Function`) bridges the two by reading from Service Bus.

Authentication via Microsoft Entra ID will be implemented in a future evolution.

## Architecture

Clean Architecture with 4 layers:

```
01.Presentation  → ASP.NET Core APIs (controllers, middleware, request/response models)
02.Application   → CQRS commands/queries, validators, mediator handlers
03.Domain        → Entities, Value Objects, Domain Services, Repository interfaces
04.Infrastructure→ EF Core repositories, Unit of Work, DI registration, messaging
05.Functions     → Azure Functions (Service Bus consumer)
```

Dependencies flow inward: Presentation → Application → Domain ← Infrastructure.

## Stack

- .NET 10.0 / ASP.NET Core
- C# (latest features: primary constructors, file-scoped namespaces, records)
- EF Core 10.0.9 (InMemory for dev, SQL Server for production)
- Mediator.SourceGenerator v3.0.2 (source-generated mediator, no MediatR)
- FluentValidation v12.1.1 (command validation via pipeline behavior)
- Swashbuckle.AspNetCore v10.2.3 (Swagger UI in Development)
- xUnit + Microsoft.AspNetCore.Mvc.Testing (unit and integration tests)
- Azure Service Bus (outbox/inbox messaging)

## Project Structure

```
src/
  01.Presentation/
    CashFlow.Ledger.Api/          → Ledger REST API (POST /v1/lancamentos, GET /v1/lancamentos)
    CashFlow.Balance.Api/         → Balance REST API (GET /v1/saldo)
  02.Application/
    CashFlow.Ledger.Application/  → RegisterLedgerEntryCommand, ListLedgerEntriesQuery
    CashFlow.Balance.Application/ → GetDailyBalanceQuery, ProcessLedgerEntryRegisteredCommand
  03.Domain/
    CashFlow.SharedKernel/        → Guard class, Money value object, integration event contracts
    CashFlow.Ledger.Domain/       → LedgerEntry, IdempotencyRecord, IntegrationMessage, services
    CashFlow.Balance.Domain/      → DailyBalance, InboxMessage, DailyBalanceService
  04.Infrastructure/
    CashFlow.Ledger.Infrastructure/  → EF repositories, UoW, DI (AddLedger extension)
    CashFlow.Balance.Infrastructure/ → EF repositories, UoW, DI (AddBalance extension)
  05.Functions/
    CashFlow.Consolidation.Function/ → Azure Function consuming Service Bus messages
tests/
  CashFlow.Ledger.UnitTests/          → Domain and application unit tests
  CashFlow.Ledger.IntegrationTests/   → API endpoint and persistence integration tests
  CashFlow.Balance.IntegrationTests/  → Balance consolidation integration tests
  CashFlow.ArchitectureTests/         → Architecture rule enforcement
  CashFlow.Consolidation.IntegrationTests/
```

## Key Patterns

- **CQRS** — commands (write) and queries (read) separated via Mediator
- **Guard Clauses** — `Guard.NotEmpty()`, `Guard.MaxLength()`, `Guard.DefinedEnum()`, etc. in `SharedKernel`
- **Idempotency** — SHA256 fingerprint of request body stored in `IdempotencyRecord`; enforced by `IdempotencyKeyMiddleware` (POST only) + `IdempotencyService`
- **Transactional Outbox** — domain events stored as `IntegrationMessage`, published asynchronously
- **Inbox** — `InboxMessage` prevents duplicate event processing in the Balance context
- **Unit of Work** — `ILedgerUnitOfWork` / `IBalanceUnitOfWork` wrap EF `SaveChangesAsync`
- **Money Value Object** — rounds to 2 decimal places, encapsulates monetary logic
- **AAA Test Pattern** — all tests use `// Arrange`, `// Act`, `// Assert` comments

## API Endpoints

### Ledger API

- `POST /v1/lancamentos` — create a ledger entry (requires `Idempotency-Key` header)
  - Body: `{ comercianteId, tipo, valor, data, descricao }`
- `GET /v1/lancamentos?comercianteId=xxx` — list all entries for a merchant
- `GET /health` — health check

### Balance API

- `GET /v1/saldo?comercianteId=xxx&data=yyyy-MM-dd` — get daily balance (date defaults to today)
- `GET /health` — health check

## DI Registration

DI is organized in the Infrastructure layer as extension methods:

- `services.AddLedger(configuration)` → registers domain services, validators, mediator, persistence, messaging
- `services.AddBalance(configuration)` → registers mediator, persistence

## Specifications

Document user requests in the `specs` folder before coding.

Use this file pattern: `specs/001-description-spec.md`

Examples:

- `specs/001-api-key-authentication-spec.md`
- `specs/002-login-validation-spec.md`

Keep specs short, clear, and focused on the requested task.

## Rules

- Do not use git commands or manipulate branches.
- Before creating or editing anything, present a short plan.
- In the plan, list every file that will be created or edited in a simple format.
- After presenting the plan, ask for user confirmation before starting execution.
- Do not create or edit files until the user confirms.
- Inspect existing patterns before changing code.
- Keep changes small and focused.
- Implement only the requested task.
- When adding or changing business rules, follow TDD: write a failing automated test before the definitive implementation, then implement the minimum code needed and refactor safely.
- Base every new implementation on official documentation.
- Do not add libraries without justification.
- Do not change APIs, DTOs, database schemas, or pipelines unless requested.
- Follow SOLID, Clean Code, KISS, YAGNI, and DRY pragmatically.
- Prefer simple, readable, testable code over abstractions.
- Do not force Design Patterns.
- Suggest Strategy, Factory, Adapter, Facade, Decorator, Repository, or Builder only when they clearly fit.
- Never log secrets, tokens, passwords, API keys, or personal data.

## Workflow

Before creating or editing anything, present a short plan.

The plan must include:

- files to create or edit
- approach
- risks

After presenting the plan, ask for user confirmation before starting execution.

Do not create or edit files until the user confirms.

Before coding, create or update the related spec file.

Then list:

- files to inspect
- files to change
- approach
- risks

After coding, summarize:

- changed files
- what changed
- how to validate
