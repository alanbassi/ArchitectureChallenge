# ADR-001: Uso do EF Core na camada de Application

## Status

Aceito

## Data

2024-02-29

## Contexto

A camada de Application precisa ler e gravar dados. Clean Architecture restringe camadas internas de depender de camadas externas, e frameworks como ORMs normalmente ficam na camada de Infrastructure. A questão é se a camada de Application deve acessar o EF Core diretamente através de uma interface própria, ou se uma camada de repositório deve ficar entre eles.

## Decisão

A camada de Application define `IApplicationDbContext` — uma interface que expõe propriedades `DbSet<T>` — e a utiliza diretamente nos handlers de commands e queries. `ApplicationDbContext` na camada de Infrastructure implementa essa interface. Não existe camada de repositório entre a Application e o EF Core.

## Justificativa

### Inversão de dependência é satisfeita

`IApplicationDbContext` é definida na camada de Application. `ApplicationDbContext` na Infrastructure a implementa. A seta de dependência aponta para dentro — Infrastructure depende de Application, não o contrário.

A camada de Application tem uma dependência em tempo de compilação das abstrações do EF Core (`DbSet<T>`, `IQueryable<T>`), mas não conhece o `DbContext` concreto, o provider de banco, nem qualquer tipo da Infrastructure. Uma referência a um assembly (`Microsoft.EntityFrameworkCore`) não é o mesmo que uma dependência de uma implementação concreta. Inversão de dependência tem a ver com a direção da seta de dependência, não com ter zero referências a frameworks nas camadas internas.

### Repositórios adicionam indireção sem abstração real

A alternativa é definir interfaces de repositório na camada de Application e implementá-las na Infrastructure. Na prática, isso não remove o acoplamento ao EF Core — apenas o esconde. As implementações dos repositórios continuam usando EF Core internamente, e a camada de Application ainda precisa expressar intenção em termos relevantes ao ORM: quais entidades relacionadas carregar, como filtrar, como projetar. Essas preocupações aparecem na interface do repositório como métodos customizados (`GetByIdWithItems`, `GetActiveOrderedByName`, etc.) que espelham padrões de query do EF Core sem usar a sintaxe do EF Core.

O resultado é uma camada extra de indireção que aumenta a complexidade e reduz a descobribilidade. `IApplicationDbContext` com `DbSet<T>` é mais honesto sobre o que realmente está acontecendo.

### Testar contra um banco real é preferível

Testes funcionais devem usar um banco de dados real e Respawn para resetar o estado entre os testes. Testar contra o provider real detecta problemas que fakes em memória e repositórios mockados não conseguem: comportamento de queries específico do provider, violações de índice, semântica de transações e enforcement de constraints. Essa é a [abordagem recomendada pela Microsoft](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy).

Testes unitários para lógica de domínio pura e validação da camada de Application não precisam de acesso ao banco.

### Substituir o ORM é YAGNI

Um argumento comum para abstrair o EF Core é manter a camada de Application portável — se precisar trocar o ORM depois, apenas a Infrastructure muda. Na prática, projetos que adotam EF Core em greenfield não o substituem. Projetar toda a camada de Application para um cenário que quase nunca ocorre adiciona complexidade real agora para resolver um problema teórico no futuro.

## Consequências

**Facilita:**
- Os handlers da Application podem usar toda a expressividade do EF Core — projeções, includes, SQL puro, compiled queries — sem workarounds ou vazamento de intenção através de interfaces de repositório.
- Não existe camada de repositório para projetar, implementar ou manter sincronizada conforme os requisitos dos handlers evoluem.
- O caminho de acesso a dados é direto: handler → `IApplicationDbContext` → `ApplicationDbContext`.

**Dificulta:**
- Testes funcionais precisam rodar contra um banco real. Não existe substituto leve.
- Trocar o ORM exigiria mudanças nos handlers da Application, não apenas na Infrastructure. Esse é um tradeoff aceito dada a raridade com que isso acontece na prática.

## Quando essa decisão não se aplica

Este ADR se aplica à abordagem padrão do template, que não prescreve DDD. Se você está combinando Clean Architecture com Domain-Driven Design, o padrão Repository é a escolha certa — mas por razões diferentes da abstração de persistência.

Em um modelo DDD, repositórios são um conceito de domínio. Eles são definidos na camada de domínio em termos de agregados, não em termos de mecânica de persistência. A camada de domínio define `IOrderRepository` com métodos como `GetById`, `Save` e `FindByCustomer` — expressos na linguagem do domínio, não na linguagem do EF Core. A camada de Infrastructure implementa essas interfaces. Isso é fundamentalmente diferente de encapsular o `DbContext` em um repositório para escondê-lo da camada de Application, que é a abordagem contra a qual este ADR argumenta.

Se o seu projeto justifica DDD — um modelo de domínio rico, fronteiras de agregados, domain services e invariantes enforçadas dentro do domínio — então repositórios pertencem a esse modelo, e `IApplicationDbContext` não deve ser usado diretamente nos handlers.
