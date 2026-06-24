# ADR-003: Referência ao MediatR.Contracts no Domain

## Status

Aceito

## Data

2026-03-16

## Contexto

Domain events precisam se integrar com o sistema pub/sub do MediatR, que é usado na camada de Application para despachar eventos após o `SaveChangesAsync`. `BaseEvent` precisa satisfazer a interface marker `INotification` do MediatR para que isso funcione. Clean Architecture exige que a camada de Domain seja livre de dependências externas para que o modelo de domínio possa evoluir independentemente de frameworks e infraestrutura.

## Decisão

Referenciar `MediatR.Contracts` no projeto Domain. `BaseEvent` implementa `INotification` diretamente, tornando os domain events notificações do MediatR de primeira classe, sem nenhuma camada de adaptação ou mapeamento na Application.

## Justificativa

A alternativa é definir uma interface marker local (ex.: `IDomainEvent`) no Domain e adaptá-la para `INotification` na camada de Application antes de publicar via MediatR. Isso preserva o ideal de "zero dependências externas no Domain", mas adiciona um adapter que existe apenas para cobrir uma diferença entre duas interfaces com semântica idêntica.

`MediatR.Contracts` contém apenas definições de interface — sem implementação, sem dependências transitivas, estável entre versões major do MediatR. O acoplamento é real, mas mínimo, e elimina toda uma camada de conversão que precisaria ser escrita e mantida.

## Consequências

**Facilita:**
- Domain events são publicáveis diretamente via `IMediator.Publish()` sem nenhuma etapa de conversão.
- Se o MediatR for substituído, basta alterar `BaseEvent` e uma referência de pacote — uma atualização única e contida.

**Dificulta:**
- Domain.csproj tem uma dependência NuGet (`MediatR.Contracts`). O ideal de "zero dependências NuGet" é intencionalmente relaxado.
