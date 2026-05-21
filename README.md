# Single Responsibility Principle (SRP)

## Sobre o Projeto

Este projeto e um estudo pratico do **Principio da Responsabilidade Unica (SRP)** - o primeiro principio do SOLID. A aplicacao demonstra como estruturar um projeto .NET seguindo boas praticas de arquitetura, onde cada classe tem uma unica razao para mudar.

## O que e o SRP?

> "Uma classe deve ter um, e somente um, motivo para mudar." - Robert C. Martin

O SRP estabelece que cada classe deve ser responsavel por apenas uma parte da funcionalidade do software. Isso resulta em:

- **Codigo mais coeso**: Classes focadas em uma unica tarefa
- **Menor acoplamento**: Mudancas em uma area nao afetam outras
- **Facilidade de testes**: Classes pequenas sao mais faceis de testar
- **Manutencao simplificada**: Bugs sao mais faceis de localizar e corrigir

---

## Estrutura do Projeto

```
single-responsability-principle/
|
+-- domain/                    # Camada de Dominio
|   +-- Entities/              # Entidades do dominio
|   |   +-- Entity.cs          # Classe base com Domain Events
|   |   +-- Plan.cs            # Entidade Plan
|   |   +-- Service.cs         # Entidade Service
|   |   +-- Feature.cs         # Entidade Feature
|   |
|   +-- Events/                # Domain Events
|   |   +-- IDomainEvent.cs
|   |   +-- IDomainEventDispatcher.cs
|   |   +-- PlanCreatedEvent.cs
|   |   +-- PlanUpdatedEvent.cs
|   |   +-- ServiceCreatedEvent.cs
|   |   +-- ServiceUpdatedEvent.cs
|   |   +-- FeatureCreatedEvent.cs
|   |   +-- FeatureUpdatedEvent.cs
|   |
|   +-- Repositories/          # Interfaces de Repositorios
|       +-- IPlanRepository.cs
|       +-- IServiceRepository.cs
|       +-- IFeatureRepository.cs
|       +-- IUnitOfWork.cs
|
+-- application/               # Camada de Aplicacao
|   +-- UseCases/              # Casos de Uso (Commands)
|   |   +-- Plan/
|   |   |   +-- CreatePlanUseCase.cs
|   |   |   +-- UpdatePlanUseCase.cs
|   |   |   +-- DeletePlanUseCase.cs
|   |   +-- Service/
|   |   |   +-- CreateServiceUseCase.cs
|   |   |   +-- UpdateServiceUseCase.cs
|   |   |   +-- DeleteServiceUseCase.cs
|   |   +-- Feature/
|   |       +-- CreateFeatureUseCase.cs
|   |       +-- UpdateFeatureUseCase.cs
|   |       +-- DeleteFeatureUseCase.cs
|   |
|   +-- Queries/               # Servicos de Consulta (Queries)
|   |   +-- PlanQueryService.cs
|   |   +-- ServiceQueryService.cs
|   |   +-- FeatureQueryService.cs
|   |
|   +-- Validators/            # Validadores (FluentValidation)
|   |   +-- CreatePlanValidator.cs
|   |   +-- CreateServiceValidator.cs
|   |   +-- CreateFeatureValidator.cs
|   |
|   +-- EventHandlers/         # Handlers de Domain Events
|   |   +-- PlanCreatedEventHandler.cs
|   |   +-- PlanUpdatedEventHandler.cs
|   |   +-- ServiceCreatedEventHandler.cs
|   |   +-- ServiceUpdatedEventHandler.cs
|   |   +-- FeatureCreatedEventHandler.cs
|   |   +-- FeatureUpdatedEventHandler.cs
|   |
|   +-- Dtos/                  # Data Transfer Objects
|   |   +-- PlanDto.cs
|   |   +-- ServiceDto.cs
|   |   +-- FeatureDto.cs
|   |
|   +-- Configuration/         # Configuracao de DI
|       +-- DependencyInjection.cs
|
+-- infrastructure/            # Camada de Infraestrutura
|   +-- Context/               # DbContext (EF Core)
|   |   +-- AppDbContext.cs
|   |
|   +-- Database/              # Implementacoes de Repositorios
|   |   +-- PlanRepository.cs
|   |   +-- ServiceRepository.cs
|   |   +-- FeatureRepository.cs
|   |   +-- UnitOfWork.cs
|   |
|   +-- Configurations/        # Configuracao de DI e DB
|       +-- DependencyInjection.cs
|
+-- http/                      # Camada de Apresentacao (API)
|   +-- Controllers/           # Controllers REST
|   |   +-- PlanController.cs
|   |   +-- ServiceController.cs
|   |   +-- FeatureController.cs
|   |
|   +-- Program.cs             # Ponto de entrada da aplicacao
|
+-- test-srp/                  # Projeto de Testes
    +-- Unit/                  # Testes Unitarios
    |   +-- Entities/
    |   +-- UseCases/
    |   +-- Validators/
    |   +-- Queries/
    |
    +-- Integration/           # Testes de Integracao
    |   +-- Repositories/
    |   +-- Controllers/
    |
    +-- Helpers/               # Helpers para testes
        +-- TestDataBuilder.cs
```

---

## Aplicacao do SRP na Arquitetura

### 1. Entidades - Responsabilidade: Representar o Dominio

As entidades representam os conceitos do dominio de negocio. Cada entidade e responsavel apenas por:

- Manter seu estado interno consistente
- Emitir eventos de dominio quando algo relevante acontece

```csharp
public class Plan : Entity
{
    // Responsabilidade: Representar um Plano no dominio
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    
    public void Activate() => IsActive = true;
    
    public void UpdatePlan(string name, string description, decimal price, bool isActive)
    {
        Name = name;
        Description = description;
        Price = price;
        IsActive = isActive;
        AddDomainEvent(new PlanUpdatedEvent(Id, Name, Price));
    }
}
```

**O que a entidade NAO faz:**

- Nao valida dados de entrada (responsabilidade do Validator)
- Nao persiste dados (responsabilidade do Repository)
- Nao envia notificacoes (responsabilidade do EventHandler)

---

### 2. Use Cases - Responsabilidade: Orquestrar a Logica de Negocio

Cada Use Case tem uma unica responsabilidade: executar um caso de uso especifico.

```csharp
public class CreatePlanUseCase(IPlanRepository planRepository, IUnitOfWork unitOfWork)
{
    // Responsabilidade: Criar um novo Plano
    public async Task<Guid> ExecuteAsync(CreatePlanDto createPlan)
    {
        var plan = new Plan(createPlan.Name, createPlan.Description, createPlan.Price);
        
        if(createPlan.IsActive)
            plan.Activate();
        
        _planRepository.AddPlan(plan);
        
        if(!await _uow.CommitAsync())
            throw new InvalidOperationException("Failed to create the plan.");
        
        return plan.Id;
    }
}
```

**Separacao de Use Cases:**

- `CreatePlanUseCase` - Apenas cria planos
- `UpdatePlanUseCase` - Apenas atualiza planos
- `DeletePlanUseCase` - Apenas deleta planos

---

### 3. Validators - Responsabilidade: Validar Dados de Entrada

Os validators sao responsaveis exclusivamente por validar os dados recebidos.

```csharp
public class CreatePlanValidator : AbstractValidator<CreatePlanDto>
{
    // Responsabilidade: Validar dados de criacao de Plano
    public CreatePlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
```

---

### 4. Query Services - Responsabilidade: Consultar Dados

Os Query Services sao responsaveis apenas por consultas (leitura).

```csharp
public class PlanQueryService(IPlanRepository planRepository)
{
    // Responsabilidade: Consultar Planos
    public async Task<PlanDto?> GetByIdAsync(Guid id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        return MapToDto(plan);
    }
    
    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        var plans = await _planRepository.GetAllPlansAsync();
        return plans.Select(MapToDto);
    }
}
```

---

### 5. Repositories - Responsabilidade: Persistir Dados

Os repositories sao responsaveis apenas pela persistencia de dados.

```csharp
public class PlanRepository(AppDbContext context) : IPlanRepository
{
    // Responsabilidade: Persistir Planos no banco de dados
    public void AddPlan(Plan plan) => _context.Plans.Add(plan);
    public async Task<Plan> GetPlanByIdAsync(Guid id) => await _context.Plans.FindAsync(id);
    public void UpdatePlan(Plan plan) => _context.Plans.Update(plan);
    public void DeletePlanAsync(Plan plan) => _context.Plans.Remove(plan);
}
```

---

### 6. Event Handlers - Responsabilidade: Reagir a Eventos

Os Event Handlers sao responsaveis por reagir aos eventos do dominio.

```csharp
public class PlanCreatedEventHandler : INotificationHandler<PlanCreatedEvent>
{
    // Responsabilidade: Reagir ao evento de criacao de Plano
    public Task Handle(PlanCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Plano criado: {notification.Name} - R$ {notification.Price}");
        return Task.CompletedTask;
    }
}
```

---

### 7. Controllers - Responsabilidade: Receber Requisicoes HTTP

Os controllers sao responsaveis apenas por:

- Receber requisicoes HTTP
- Delegar para os servicos apropriados
- Retornar respostas HTTP

```csharp
[ApiController]
[Route("api/[controller]")]
public class PlanController(
    IValidator<CreatePlanDto> validator,
    CreatePlanUseCase createPlanUseCase,
    PlanQueryService planQuery) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanDto newPlan)
    {
        var validationResult = await _validator.ValidateAsync(newPlan);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        
        var planId = await _createPlanUseCase.ExecuteAsync(newPlan);
        return CreatedAtAction(nameof(Create), new { id = planId });
    }
}
```

---

## CQRS Simplificado

Este projeto utiliza uma versao simplificada do padrao CQRS (Command Query Responsibility Segregation):


| Tipo                         | Responsabilidade | Exemplo                                  |
| ---------------------------- | ---------------- | ---------------------------------------- |
| **Commands** (Use Cases)     | Modificar estado | `CreatePlanUseCase`, `UpdatePlanUseCase` |
| **Queries** (Query Services) | Consultar dados  | `PlanQueryService.GetByIdAsync()`        |


---

## Beneficios Alcancados


| Beneficio           | Descricao                                                                |
| ------------------- | ------------------------------------------------------------------------ |
| **Testabilidade**   | Cada classe pode ser testada isoladamente com mocks                      |
| **Manutencao**      | Mudancas em validacao nao afetam persistencia                            |
| **Extensibilidade** | Novos casos de uso podem ser adicionados sem modificar os existentes     |
| **Clareza**         | Facil entender o que cada classe faz pelo seu nome                       |
| **Reutilizacao**    | Validators e Repositories podem ser reutilizados em diferentes contextos |


---

## Como Executar

### Pre-requisitos

- .NET 9.0 SDK
- Visual Studio 2022 / VS Code / Rider

### Executando a API

```bash
# Navegar para o diretorio do projeto HTTP
cd http

# Restaurar dependencias
dotnet restore

# Executar a aplicacao
dotnet run
```

A API estara disponivel em: `https://localhost:5001` ou `http://localhost:5000`

### Acessando o Swagger

Apos iniciar a aplicacao, acesse:

```
https://localhost:5001/swagger
```

---

## Executando os Testes

```bash
# Navegar para o diretorio de testes
cd test-srp

# Executar todos os testes
dotnet test

# Executar com output detalhado
dotnet test --verbosity normal

# Executar apenas testes unitarios
dotnet test --filter "FullyQualifiedName~Unit"

# Executar apenas testes de integracao
dotnet test --filter "FullyQualifiedName~Integration"
```

### Cobertura de Testes


| Categoria                           | Quantidade     |
| ----------------------------------- | -------------- |
| Testes Unitarios - Entidades        | 17             |
| Testes Unitarios - Use Cases        | 35             |
| Testes Unitarios - Validators       | 32             |
| Testes Unitarios - Query Services   | 26             |
| Testes de Integracao - Repositories | 24             |
| Testes de Integracao - Controllers  | 29             |
| **Total**                           | **163 testes** |


---

## Tecnologias Utilizadas


| Tecnologia                | Proposito                     |
| ------------------------- | ----------------------------- |
| **.NET 9.0**              | Framework principal           |
| **Entity Framework Core** | ORM para persistencia         |
| **SQLite**                | Banco de dados                |
| **FluentValidation**      | Validacao de entrada          |
| **MediatR**               | Mediador para Domain Events   |
| **Swashbuckle**           | Documentacao Swagger          |
| **xUnit**                 | Framework de testes           |
| **Moq**                   | Mocking para testes unitarios |
| **FluentAssertions**      | Assertivas fluentes           |
| **EF Core InMemory**      | Banco em memoria para testes  |
| **WebApplicationFactory** | Testes de integracao HTTP     |


---

## Endpoints da API

### Plans


| Metodo | Endpoint         | Descricao              |
| ------ | ---------------- | ---------------------- |
| POST   | `/api/plan`      | Criar novo plano       |
| GET    | `/api/plan/{id}` | Buscar plano por ID    |
| GET    | `/api/plan`      | Listar todos os planos |
| PUT    | `/api/plan/{id}` | Atualizar plano        |
| DELETE | `/api/plan/{id}` | Deletar plano          |


### Services


| Metodo | Endpoint                   | Descricao                |
| ------ | -------------------------- | ------------------------ |
| POST   | `/api/service`             | Criar novo servico       |
| GET    | `/api/service/{id}`        | Buscar servico por ID    |
| GET    | `/api/service/type/{type}` | Buscar servicos por tipo |
| PUT    | `/api/service/{id}`        | Atualizar servico        |
| DELETE | `/api/service/{id}`        | Deletar servico          |


### Features


| Metodo | Endpoint                   | Descricao               |
| ------ | -------------------------- | ----------------------- |
| POST   | `/api/feature`             | Criar nova feature      |
| GET    | `/api/feature/{id}`        | Buscar feature por ID   |
| GET    | `/api/feature/name/{name}` | Buscar feature por nome |
| PUT    | `/api/feature/{id}`        | Atualizar feature       |
| DELETE | `/api/feature/{id}`        | Deletar feature         |


---

## Anti-Patterns Evitados

Este projeto demonstra como evitar violacoes comuns do SRP:


| Anti-Pattern         | Solucao Aplicada                                |
| -------------------- | ----------------------------------------------- |
| **God Class**        | Classes pequenas e focadas                      |
| **Feature Envy**     | Cada classe manipula apenas seus proprios dados |
| **Blob**             | Responsabilidades distribuidas em camadas       |
| **Swiss Army Knife** | Classes especializadas em uma unica tarefa      |


---

## Referencias

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://www.digitalocean.com/community/conceptual-articles/s-o-l-i-d-the-first-five-principles-of-object-oriented-design)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Domain Events](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)

---

