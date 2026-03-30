# FinanceAPI - Plano de Desenvolvimento

## Visão Geral do Projeto

**Objetivo:** Sistema de gerenciamento financeiro pessoal com API REST para integração com bot Telegram.

**Stack:** .NET 10 Minimal APIs, Entity Framework Core 10, SQLite

**Funcionalidades Planejadas:**
- Controle de transações (receitas/despesas/transferências)
- Gerenciamento de categorias
- Duas contas: Efetivo e Transferência
- Saldo atual, total de ganhos/gastos semanais e mensais

---

## PARTE 1: Falhas Atuais Identificadas

### 1.1 CRÍTICAS (Impedem funcionamento correto)

#### F1 - CategoriesEndpoints não registrado
- **Localização:** `Program.cs`
- **Problema:** O método `MapCategoriesEndpoints()` não é chamado
- **Impacto:** Todos os endpoints de categorias estão inacessíveis
- **Correção:** Adicionar `app.MapCategoriesEndpoints();` em `Program.cs`

#### F2 - PUT de Transactions aceita Entity ao invés de DTO
- **Localização:** `Endpoints/TransactionsEndpoints.cs:50`
- **Problema:** Endpoint recebe `Transaction` entity diretamente
- **Impacto:** Exposição indevida de campos internos, inconsistência com POST
- **Correção:** Alterar parâmetro para `TransactionDTO`

#### F3 - Validação de CategoryId inexistente
- **Localização:** `Endpoints/TransactionsEndpoints.cs` (POST/PUT)
- **Problema:** Não há verificação se CategoryId existe antes de criar transação
- **Impacto:** Exceção de FK constraint do EF Core, resposta de erro genérica
- **Correção:** Adicionar verificação prévia de existência da categoria

### 1.2 MÉDIAS (Afetam qualidade/manutenibilidade)

#### F4 - Namespaces inconsistentes
- **Localização:** `Models/Transaction.cs` e `Models/Category.cs`
- **Problema:** Usam namespace `Finance_API.Models` (com underscore)
- **Impacto:** Confusão na organização, possível breaking change futuro
- **Correção:** Padronizar para `FinanceAPI.Models`

#### F5 - Falta de CategoryDTO
- **Localização:** `DTOs/`
- **Problema:** Endpoints de categoria usam entity diretamente
- **Impacto:** Exposição de dados internos, acoplamento com banco
- **Correção:** Criar `CategoryDTO` e `CreateCategoryDTO`

#### F6 - PUT de Categories não atualiza campo Type
- **Localização:** `Endpoints/CategoriesEndpoints.cs:35-36`
- **Problema:** Apenas `Name` é atualizado
- **Impacto:** Campo `Type` fica desatualizado
- **Correção:** Adicionar atualização do `Type`

#### F7 - Valores negativos permitidos em Amount
- **Localização:** `DTOs/TransactionDTO.cs`
- **Problema:** Sem validação de `[Range]` no campo Amount
- **Impacto:** Transações com valores negativos ou zero
- **Correção:** Adicionar `[Range(0.01, double.MaxValue)]`

#### F8 - Connection string hardcoded
- **Localização:** `Program.cs:7`
- **Problema:** `Data Source=finance.db` está fixo no código
- **Impacto:** Dificulta configuração por ambiente
- **Correção:** Mover para `appsettings.json`

#### F9 - Seed data com ID 12 ausente
- **Localização:** `Data/AppDbContext.cs`
- **Problema:** IDs vão de 1-11, pulam 12, continuam 13-15
- **Impacto:** Confusão em debug, possível bug em queries
- **Correção:** Reordenar IDs sequencialmente ou remover especificação de ID

### 1.3 MENORES (Boas práticas)

#### F10 - Ausência de middleware de tratamento de erros
- **Problema:** Exceções do EF Core expostas diretamente
- **Correção:** Implementar exception handler global

#### F11 - Sem paginação no GET de transações
- **Problema:** Retorna todas as transações sem filtro
- **Correção:** Implementar paginação e filtros

#### F12 - Pasta Services vazia
- **Problema:** Toda lógica nos endpoints
- **Correção:** Implementar padrão Repository/Service

---

## PARTE 2: Correções Prioritárias

### Passo 1: Corrigir Falhas Críticas

```
Ordem de execução:
1. F1 - Registrar CategoriesEndpoints
2. F2 - Alterar PUT para usar DTO
3. F3 - Adicionar validação de CategoryId
4. F4 - Padronizar namespaces
5. F7 - Adicionar validação de Amount
```

**Arquivos a modificar:**
1. `Program.cs` - Registrar categories endpoints
2. `Endpoints/TransactionsEndpoints.cs` - PUT usar DTO, validar CategoryId
3. `Models/Transaction.cs` - Corrigir namespace
4. `Models/Category.cs` - Corrigir namespace
5. `DTOs/TransactionDTO.cs` - Adicionar Range validation

### Passo 2: Corrigir Falhas Médias

```
Ordem de execução:
1. F5 - Criar CategoryDTO
2. F6 - Corrigir PUT de Categories
3. F8 - Mover connection string para config
4. F9 - Corrigir seed data
```

**Arquivos a criar/modificar:**
1. `DTOs/CategoryDTO.cs` (novo)
2. `DTOs/CreateCategoryDTO.cs` (novo)
3. `Endpoints/CategoriesEndpoints.cs` - Usar DTOs
4. `appsettings.json` - Adicionar connection string
5. `Data/AppDbContext.cs` - Corrigir seed, remover ID hardcoded
6. `Program.cs` - Ler connection string do config

---

## PARTE 3: Implementação do Sistema de Contas

### Visão Funcional

O sistema terá **duas contas**:
- **Efetivo (Cash)** - Dinheiro físico
- **Transferência (Bank)** - Conta bancária/digital

**Tipos de Transação:**
| Tipo | Comportamento |
|------|---------------|
| `Income` | Aumenta saldo de uma conta |
| `Expense` | Diminui saldo de uma conta |
| `Transfer` | Move saldo entre contas |

### Passo 3: Criar Entidade Account

**Arquivo:** `Models/Account.cs`

```csharp
namespace FinanceAPI.Models;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public AccountType Type { get; set; }
    public decimal Balance { get; set; } = 0;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
```

**Arquivo:** `Models/Enums/AccountType.cs`

```csharp
namespace FinanceAPI.Models.Enums;

public enum AccountType
{
    Cash = 0,    // Efetivo
    Bank = 1     // Transferência
}
```

### Passo 4: Modificar Entidade Transaction

**Alterações em `Models/Transaction.cs`:**

```csharp
namespace FinanceAPI.Models;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TransactionType Type { get; set; } = TransactionType.Expense;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Novos campos para conta
    public int? SourceAccountId { get; set; }       // Conta de origem (para Expense, Transfer)
    public Account? SourceAccount { get; set; }
    public int? DestinationAccountId { get; set; }  // Conta de destino (para Income, Transfer)
    public Account? DestinationAccount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```

### Passo 5: Atualizar DbContext

**Arquivo:** `Data/AppDbContext.cs`

- Adicionar `DbSet<Account> Accounts`
- Configurar relacionamentos Account-Transaction
- Seed das duas contas padrão

### Passo 6: Criar Migration

```bash
dotnet ef migrations add AddAccountsAndTransactionAccounts
dotnet ef database update
```

---

## PARTE 4: Sistema de Saldo e Relatórios

### Passo 7: Criar DTOs de Resposta

**Arquivo:** `DTOs/Responses/BalanceResponse.cs`

```csharp
namespace FinanceAPI.DTOs.Responses;

public class BalanceResponse
{
    public decimal TotalBalance { get; set; }
    public decimal CashBalance { get; set; }
    public decimal BankBalance { get; set; }
}
```

**Arquivo:** `DTOs/Responses/WeeklyReportResponse.cs`

```csharp
namespace FinanceAPI.DTOs.Responses;

public class WeeklyReportResponse
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetBalance { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TransactionSummary> Transactions { get; set; } = new();
}

public class TransactionSummary
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
```

### Passo 8: Criar Endpoints de Relatórios

**Arquivo:** `Endpoints/ReportsEndpoints.cs`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/reports/balance` | Saldo atual por conta |
| GET | `/reports/weekly` | Relatório semanal |
| GET | `/reports/monthly` | Relatório mensal |

---

## PARTE 5: Validações e Error Handling

### Passo 9: Implementar Exception Handler

**Arquivo:** `Program.cs`

```csharp
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context) =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    return exception switch
    {
        ArgumentException => Results.BadRequest(new { error = exception.Message }),
        KeyNotFoundException => Results.NotFound(new { error = exception.Message }),
        _ => Results.Problem("An error occurred")
    };
});
```

### Passo 10: Criar Validation Service

**Arquivo:** `Services/ValidationService.cs`

Validações necessárias:
- CategoryId existe no banco
- AccountId existe no banco
- Amount > 0
- Transfer: SourceAccountId ≠ DestinationAccountId
- Transfer: Ambas contas existem

---

## PARTE 6: Filtros e Paginação

### Passo 11: Implementar Query Parameters

**Arquivo:** `DTOs/Queries/TransactionQuery.cs`

```csharp
namespace FinanceAPI.DTOs.Queries;

public class TransactionQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public TransactionType? Type { get; set; }
    public int? CategoryId { get; set; }
    public int? AccountId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
```

### Passo 12: Atualizar GET de Transações

Implementar filtros usando LINQ com IQueryable:
- Filtro por tipo
- Filtro por categoria
- Filtro por conta
- Filtro por período
- Paginação com Skip/Take

---

## PARTE 7: Service Layer

### Passo 13: Criar Repository Pattern

**Arquivo:** `Repositories/ITransactionRepository.cs`

```csharp
namespace FinanceAPI.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(int id);
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<IEnumerable<Transaction>> GetFilteredAsync(TransactionQuery query);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction?> UpdateAsync(int id, TransactionDTO dto);
    Task<bool> DeleteAsync(int id);
}
```

**Arquivo:** `Repositories/TransactionRepository.cs`

Implementação com EF Core.

### Passo 14: Criar Service Layer

**Arquivo:** `Services/ITransactionService.cs`

```csharp
namespace FinanceAPI.Services;

public interface ITransactionService
{
    Task<Result<Transaction>> CreateAsync(CreateTransactionDTO dto);
    Task<Result<Transaction>> UpdateAsync(int id, UpdateTransactionDTO dto);
    Task<Result<BalanceResponse>> GetBalanceAsync();
    Task<Result<WeeklyReportResponse>> GetWeeklyReportAsync();
}
```

---

## PARTE 8: Testes

### Passo 15: Configurar Projeto de Testes

```bash
dotnet new xunit -n FinanceAPI.Tests
dotnet add FinanceAPI.Tests reference FinanceAPI
dotnet add FinanceAPI.Tests package Moq
dotnet add FinanceAPI.Tests package Microsoft.EntityFrameworkCore.InMemory
```

### Passo 16: Testes Unitários

**Cobertura necessária:**
- Validação de DTOs
- Lógica de negócio (saldo, transferência)
- Repositórios
- Services

### Passo 17: Testes de Integração

**Cobertura necessária:**
- Todos os endpoints HTTP
- Cenários de erro
- Validações de banco

---

## PARTE 9: Documentação e Logging

### Passo 18: Swagger/OpenAPI

Já existe integração com NSwag. Melhorias:
- Adicionar exemplos nos endpoints
- Documentar schemas
- Adicionar descrições nos endpoints

### Passo 19: Logging

**Arquivo:** `Program.cs`

```csharp
builder.Logging.AddConsole();
builder.Logging.AddDebug();
```

Implementar logging estruturado em endpoints críticos.

---

## PARTE 10: Deploy

### Passo 20: Configuração por Ambiente

**appsettings.Development.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=finance.db"
  },
  "Logging": { "LogLevel": { "Default": "Debug" } }
}
```

**appsettings.Production.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/data/finance.db"
  },
  "Logging": { "LogLevel": { "Default": "Warning" } }
}
```

### Passo 21: Dockerfile

**Arquivo:** `Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["FinanceAPI.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinanceAPI.dll"]
```

### Passo 22: docker-compose.yml (opcional)

Para ambiente de desenvolvimento com volume persistente.

### Passo 23: CI/CD (opcional)

**GitHub Actions workflow:**
- Build
- Test
- Publish
- Deploy para servidor/bot

---

## Resumo da Ordem de Implementação

### Fase 1: Correções Críticas (Prioridade Alta)
1. ✅ F1 - Registrar CategoriesEndpoints
2. ✅ F2 - PUT usar DTO
3. ✅ F3 - Validar CategoryId
4. ✅ F4 - Padronizar namespaces
5. ✅ F5 - Criar CategoryDTO
6. ✅ F6 - Corrigir PUT Categories
7. ✅ F7 - Validar Amount
8. ✅ F8 - Connection string em config
9. ✅ F9 - Corrigir seed data

### Fase 2: Sistema de Contas
10. ✅ Criar entidade Account
11. ✅ Modificar Transaction
12. ✅ Atualizar DbContext
13. ✅ Criar migration
14. ✅ Criar DTOs de Account
15. ✅ Criar AccountEndpoints

### Fase 3: Relatórios e Saldo
16. ✅ DTOs de resposta
17. ✅ Endpoints de relatórios
18. ✅ Cálculo de saldo

### Fase 4: Qualidade
19. ✅ Exception handler
20. ✅ Validation service
21. ✅ Filtros e paginação
22. ✅ Repository pattern
23. ✅ Service layer

### Fase 5: Validação Final
24. ✅ Testes unitários
25. ✅ Testes de integração
26. ✅ Documentação Swagger
27. ✅ Logging

### Fase 6: Deploy
28. ✅ Configuração produção
29. ✅ Dockerfile
30. ✅ Deploy

---

## Dependências entre Tarefas

```
F1-F9 (Correções)
    ↓
Account Entity → Transaction Update → DbContext → Migration
    ↓
DTOs de Resposta → Reports Endpoints
    ↓
Exception Handler → Validation Service → Filtros
    ↓
Repository → Service Layer
    ↓
Testes → Documentação → Deploy
```

---

## Notas Importantes

1. **Execute as migrations** após cada mudança no modelo
2. **Sempre valide CategoryId e AccountId** antes de criar transação
3. **Para transferências**, debita de uma conta e credita em outra
4. **Use DateTimeOffset** se precisar de timezone awareness
5. **Considere soft delete** para não perder histórico de transações
6. **Implemente concurrency token** se houver múltiplos clientes

---

## Recursos Oficiais Consultados

- [.NET Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Entity Framework Core Transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions)
- [EF Core Best Practices](https://learn.microsoft.com/en-us/ef/core/)