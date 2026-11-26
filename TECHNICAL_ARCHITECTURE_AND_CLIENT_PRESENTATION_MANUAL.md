# 🐾 Pet Insurance Quote Management System
## Encyclopedic Technical Architecture, Framework Citation & Client Presentation Dossier
### Complete Line-by-Line, Keyword-by-Keyword, Function-by-Function Reference

> **Classification:** Comprehensive Enterprise Technical Manual & Client Presentation Specification  
> **Target Audience:** Enterprise Clients, Solutions Architects, Engineering Directors, Security Auditors  
> **Technology Stack:** .NET 10.0 (C# 14/13), ASP.NET Core Web API, Entity Framework Core 10, SQLite, ClosedXML, NUnit, Moq, FluentAssertions  
> **Official Documentation Citations:** Microsoft Learn (`https://learn.microsoft.com/en-us/dotnet/`)

---

# Table of Contents
1. [Master Executive Summary & Technology Stack](#chapter-1-master-executive-summary)
2. [Program.cs: Exhaustive Line-by-Line & Keyword-by-Keyword Dissection (Lines 1–122)](#chapter-2-programcs-line-by-line-dissection)
3. [Deep-Dive into C# Types, Memory Layouts & CLR Primitives (with Microsoft Learn Citations)](#chapter-3-deep-dive-into-c-types)
   - 3.1 `System.Decimal` vs `double` / `float`: 128-bit IEEE Representation, Mantissa-Scale Math & Financial Disasters
   - 3.2 Date & Time Architecture: `DateOnly` vs `DateTime` vs `DateTimeOffset`
   - 3.3 Asynchronous Execution Mechanics: `async`, `await`, `Task`, `Task<T>`, Roslyn State Machine & ThreadPool Safety
   - 3.4 Web Abstractions: `IActionResult` vs `ActionResult<T>` vs `IResult`
   - 3.5 Generics & Collection Variance: `<T>`, `ICollection<T>`, `List<T>`, `IEnumerable<T>`, `IQueryable<T>`
   - 3.6 Nullable Reference Types, Pattern Matching & C# Language Keywords
4. [Controllers: Line-by-Line & Function-by-Function Dissection](#chapter-4-controllers-line-by-line-dissection)
   - 4.1 `QuotesController.cs` (Lines 1–112): All 8 Actions, Attributes, HTTP Verbs & Status Codes
   - 4.2 `AuthController.cs` (Lines 1–43): Registration & JWT Login Pipelines
   - 4.3 `ExcelController.cs` (Lines 1–57): OpenXML Multipart File Upload & File Stream Streaming
5. [Business Logic & Service Layer: Complete Line-by-Line Dissection](#chapter-5-services-line-by-line-dissection)
   - 5.1 `QuoteService.cs` (Lines 1–380): Orchestration, Invariants, State Validations & Recalculation
   - 5.2 `PremiumCalculatorService.cs` (Lines 1–63): Actuarial Equation, Surcharges & Negative Floor
   - 5.3 `AuthService.cs` (Lines 1–62): BCrypt Adaptive Salting & Identity Validation
   - 5.4 `JwtService.cs` (Lines 1–44): HMAC-SHA256 Cryptographic Token Signing
   - 5.5 `ExcelService.cs` (Lines 1–485): OpenXML Relational Export & Surrogate Key Remapping Import
6. [Data Persistence & Repository Layer: Complete Line-by-Line Dissection](#chapter-6-repositories-line-by-line-dissection)
   - 6.1 `AppDbContext.cs` (Lines 1–49): Fluent API Configurations & Relational Cardinality
   - 6.2 `QuoteRepository.cs` (Lines 1–207): Advanced LINQ Translations, Dynamic Search & Aggregations
   - 6.3 `CustomerRepository.cs`, `PetRepository.cs`, `QuoteCoverageRepository.cs`, `UserRepository.cs`
7. [Middleware, Error Handling & Exceptions](#chapter-7-middleware-and-error-handling)
   - 7.1 `GlobalExceptionMiddleware.cs` (Lines 1–59): Russian Doll Pipeline & Exception Masking
   - 7.2 `BusinessRuleException.cs`: Domain-Specific Fault Contract
8. [Domain Models & Data Transfer Objects (DTOs)](#chapter-8-domain-models-and-dtos)
   - 8.1 Entity Domain Models (`Customer`, `Pet`, `Quote`, `QuoteCoverage`, `QuoteStatus`, `AppUser`)
   - 8.2 DTOs & Anti-Corruption Projection Strategy
9. [Comprehensive Edge Cases & Validation Matrix](#chapter-9-comprehensive-edge-cases-matrix)
10. [Automated Test Suite Dissection (NUnit, Moq, FluentAssertions, SQLite In-Memory)](#chapter-10-automated-test-suite-dissection)
11. [Client Presentation Defense Playbook: Slide Script & Q&A Defense](#chapter-11-client-presentation-defense-playbook)

---

# Chapter 1: Master Executive Summary

The **Pet Insurance Quote Management System** is an enterprise-grade backend microservice built on **.NET 10.0 (C# 14/13)**, designed for pet insurance underwriting, quote lifecycle tracking, actuarial premium calculation, and batch Excel OpenXML processing.

### Architecture At A Glance:
- **Presentation Layer:** ASP.NET Core Web API Controllers with OpenAPI/Swagger integration.
- **Business Domain Layer:** Stateless Domain Services adhering strictly to Single Responsibility and Open/Closed principles.
- **Data Access Layer:** Entity Framework Core 10 implementing Repository Pattern & Unit of Work over SQLite (file-based in production, in-memory for testing).
- **Batch Processing:** Native OpenXML via ClosedXML for multi-sheet spreadsheet ingestion and generation.
- **Security:** Stateless JWT Bearer Authentication with HMAC-SHA256 and BCrypt password encryption.

---

# Chapter 2: Program.cs: Exhaustive Line-by-Line & Keyword-by-Keyword Dissection (Lines 1–122)

File Path: [Program.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Program.cs)  
`Program.cs` is the entry point of the ASP.NET Core application. It configures the dependency injection container, the application host, and the HTTP request middleware pipeline.

```
┌────────────────────────────────────────────────────────────────────────┐
│                   Program.cs Bootstrap Architecture                    │
├────────────────────────────────────────────────────────────────────────┤
│ 1. Builder Phase (WebApplication.CreateBuilder)                        │
│    ├── Core Services: AddOpenApi(), AddControllers(), AddCors()       │
│    ├── Database: AddDbContext<AppDbContext>(UseSqlite)                │
│    ├── Swagger/OpenAPI: AddSwaggerGen(Bearer JWT Security Scheme)      │
│    ├── DI Registrations: Repositories (Scoped), Services (Scoped)      │
│    ├── Strongly-Typed Options: PremiumSettings, JwtSettings            │
│    └── Security: AddAuthentication(JwtBearer) + TokenValidationParam.  │
├────────────────────────────────────────────────────────────────────────┤
│ 2. Build Phase (builder.Build() -> WebApplication)                     │
├────────────────────────────────────────────────────────────────────────┤
│ 3. HTTP Middleware Pipeline Phase (app.Use...)                         │
│    ├── Development Tools: MapOpenApi(), UseSwagger(), UseSwaggerUI()   │
│    ├── Security & Routing: UseCors(), GlobalExceptionMiddleware        │
│    ├── Authentication & Authorization: UseAuthentication, Authorization│
│    └── Endpoint Dispatch: MapControllers(), app.Run()                  │
└────────────────────────────────────────────────────────────────────────┘
```

---

### Detailed Line-by-Line Breakdown:

#### Lines 1–12: Namespace Imports (`using` directives)
```csharp
1: using System.Text;
2: using Microsoft.AspNetCore.Authentication.JwtBearer;
3: using Microsoft.EntityFrameworkCore;
4: using Microsoft.IdentityModel.Tokens;
5: using Microsoft.OpenApi;
6: using PetInsurance.Data;
7: using PetInsurance.Middleware;
8: using PetInsurance.Repositories;
9: using PetInsurance.Repositories.Interfaces;
10: using PetInsurance.Services;
11: using PetInsurance.Services.Interfaces;
12: using PetInsurance.Settings;
```
- **Line 1 (`using System.Text;`):**
  - *Framework Citation:* [System.Text Namespace](https://learn.microsoft.com/en-us/dotnet/api/system.text)
  - *Purpose:* Imports encoding classes, specifically `Encoding.UTF8`, used on Line 73 to convert the JWT secret key string into a cryptographic byte array (`byte[]`).
- **Line 2 (`using Microsoft.AspNetCore.Authentication.JwtBearer;`):**
  - *Framework Citation:* [JwtBearerExtensions Class](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer)
  - *Purpose:* Brings in `JwtBearerDefaults.AuthenticationScheme` and `.AddJwtBearer()` extension methods to wire JWT bearer token authentication into ASP.NET Core's authentication subsystem.
- **Line 3 (`using Microsoft.EntityFrameworkCore;`):**
  - *Framework Citation:* [Microsoft.EntityFrameworkCore Namespace](https://learn.microsoft.com/en-us/ef/core/)
  - *Purpose:* Imports Entity Framework Core extension methods, specifically `.UseSqlite()` on Line 32 and DbContext configuration delegates.
- **Line 4 (`using Microsoft.IdentityModel.Tokens;`):**
  - *Framework Citation:* [Microsoft.IdentityModel.Tokens Namespace](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens)
  - *Purpose:* Provides security primitives including `TokenValidationParameters` and `SymmetricSecurityKey` used to validate the cryptographic signature of incoming JWT tokens.
- **Line 5 (`using Microsoft.OpenApi;`):**
  - *Framework Citation:* [Microsoft.OpenApi.Models](https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.models)
  - *Purpose:* Provides OpenAPI object models (`OpenApiSecurityScheme`, `SecuritySchemeType`) for generating Swagger documentation with Bearer authentication support.
- **Lines 6–12:** Internal application namespaces importing database context (`PetInsurance.Data`), custom middleware (`PetInsurance.Middleware`), repository interfaces and implementations (`PetInsurance.Repositories`), services (`PetInsurance.Services`), and strongly typed configuration classes (`PetInsurance.Settings`).

---

#### Line 14: Application Builder Initialization
```csharp
14: var builder = WebApplication.CreateBuilder(args);
```
- **Keywords & Types:**
  - `var`: C# implicitly typed local variable keyword. The compiler infers the exact type at compile time (`WebApplicationBuilder`).
  - `WebApplication`: Root hosting class introduced in .NET 6 Minimal Hosting Model (`Microsoft.AspNetCore.Builder.WebApplication`).
  - `CreateBuilder(args)`: Static factory method that initializes a new `WebApplicationBuilder` instance pre-configured with:
    1. Kestrel web server as the default internal HTTP server.
    2. Configuration providers (loading `appsettings.json`, `appsettings.{Environment}.json`, Environment Variables, and Command-line arguments `args`).
    3. Default logging providers (Console, Debug, EventSource).
    4. Dependency Injection Service Container (`builder.Services` of type `IServiceCollection`).

---

#### Lines 16–18: Core Web Services Configuration
```csharp
16: // Add services to the container.
17: builder.Services.AddOpenApi();
18: builder.Services.AddControllers();
```
- **Line 17 (`builder.Services.AddOpenApi();`):**
  - *Framework Citation:* [OpenAPI in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview)
  - Registers .NET 10 built-in OpenAPI document generation services to inspect endpoints and produce JSON OpenAPI 3.0 specification documents.
- **Line 18 (`builder.Services.AddControllers();`):**
  - *Framework Citation:* [IMvcBuilder AddControllers](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.mvcservicecollectionextensions.addcontrollers)
  - Registers only the necessary services for API controllers (controller activation, model binding, API validation, formatters like System.Text.Json). It deliberately omits Razor views and TagHelpers because this application is a pure headless REST API microservice.

---

#### Lines 20–29: Cross-Origin Resource Sharing (CORS) Policy
```csharp
20: // Add CORS Policy for Angular UI
21: builder.Services.AddCors(options =>
22: {
23:     options.AddPolicy("AllowAngular", policy =>
24:     {
25:         policy.AllowAnyOrigin()
26:               .AllowAnyHeader()
27:               .AllowAnyMethod();
28:     });
29: });
```
- **Framework Citation:** [Enable Cross-Origin Requests (CORS) in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- **Deep Technical Rationale:**
  - Web browsers enforce the **Same-Origin Policy (SOP)**, blocking an Angular Single Page Application running on `http://localhost:4200` from making HTTP requests to an API on `http://localhost:5000` (different port = different origin).
  - `.AddCors(...)` registers CORS services.
  - `options.AddPolicy("AllowAngular", ...)` defines a named policy:
    - `policy.AllowAnyOrigin()`: Allows requests from any origin (e.g. Angular dev server).
    - `policy.AllowAnyHeader()`: Permits custom headers like `Authorization` (required for sending `Bearer <token>`) and `Content-Type: application/json`.
    - `policy.AllowAnyMethod()`: Permits HTTP verbs (`GET`, `POST`, `PUT`, `DELETE`, `OPTIONS`). `OPTIONS` is critical because browsers issue an HTTP preflight request (`OPTIONS`) before executing mutating requests (`POST`/`PUT`/`DELETE`).

---

#### Lines 31–32: Relational Database Registration (EF Core + SQLite)
```csharp
31: builder.Services.AddDbContext<AppDbContext>(options =>
32:     options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```
- **Framework Citation:** [DbContext Lifetime, Configuration, and Initialization](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/)
- **Deep Technical Breakdown:**
  - `AddDbContext<AppDbContext>`: Registers `AppDbContext` in the DI container with a **`Scoped` lifetime**.
  - `options.UseSqlite(...)`: Configures EF Core to use the SQLite database provider (`Microsoft.EntityFrameworkCore.Sqlite`).
  - `builder.Configuration.GetConnectionString("DefaultConnection")`: Looks up the connection string in `appsettings.Development.json` (`"Data Source=PetInsurance.db"`).
  - *Why SQLite for this POC?* SQLite is an in-process, zero-configuration SQL engine stored in a single binary file on disk. It supports full relational integrity, foreign keys, transactions, and ANSI SQL. Because all queries are abstracted through EF Core and LINQ, moving this system to Microsoft SQL Server or PostgreSQL in production requires changing only `.UseSqlite(...)` to `.UseSqlServer(...)` or `.UseNpgsql(...)`.

---

#### Lines 34–50: Swagger / OpenAPI with JWT Bearer Security Scheme
```csharp
34: builder.Services.AddSwaggerGen(options =>
35: {
36:     options.AddSecurityDefinition("bearer",
37:         new OpenApiSecurityScheme
38:         {
39:             Type = SecuritySchemeType.Http,
40:             Scheme = "bearer",
41:             BearerFormat = "JWT",
42:             Description = "JWT Authorization header using the Bearer scheme."
43:         });
44: 
45:     options.AddSecurityRequirement(document =>
46:     new OpenApiSecurityRequirement
47:     {
48:         [new OpenApiSecuritySchemeReference("bearer", document)] = []
49:     });
50: });
```
- **Framework Citation:** [Swashbuckle.AspNetCore Documentation](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle)
- **Deep Technical Breakdown:**
  - `AddSwaggerGen`: Registers the Swagger generator that builds OpenAPI specification objects from application endpoints and metadata.
  - `AddSecurityDefinition("bearer", ...)`: Tells Swagger UI that the API is protected by a Bearer token. `Type = SecuritySchemeType.Http` and `Scheme = "bearer"` configures Swagger UI to render an **"Authorize"** button with a text input.
  - `AddSecurityRequirement`: Enforces that Swagger UI automatically attaches the header `Authorization: Bearer <token>` to all interactive test requests executed through the browser UI.

---

#### Line 52: Endpoints API Explorer
```csharp
52: builder.Services.AddEndpointsApiExplorer();
```
- **Framework Citation:** [AddEndpointsApiExplorer](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.endpointmetadataapiextensions.addendpointsapiexplorer)
- **Purpose:** Configures API explorer metadata discovery, enabling Swagger and OpenAPI generators to discover route templates, action parameters, and return types from controllers.

---

#### Lines 54–60: Repository Dependency Injection Registrations
```csharp
54: // Repositories
55: builder.Services.AddScoped<IUserRepository, UserRepository>();
56: builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
57: builder.Services.AddScoped<IPetRepository, PetRepository>();
58: builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
59: builder.Services.AddScoped<IQuoteCoverageRepository, QuoteCoverageRepository>();
```
- **Framework Citation:** [Dependency Injection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- **Why `AddScoped`?**
  - All repositories inherit `Scoped` lifetime matching `AppDbContext`.
  - When an HTTP request arrives, the DI container instantiates exactly **one** instance of each repository for the duration of that request.
  - If registered as `Transient`, multiple repositories would receive distinct instances, increasing allocation overhead and complicating shared transactions.
  - If registered as `Singleton`, EF Core would fail immediately because `DbContext` is not thread-safe.

---

#### Lines 61–67: Service Dependency Injection Registrations
```csharp
61: // Services
62: builder.Services.AddScoped<IAuthService, AuthService>();
63: builder.Services.AddScoped<IJwtService, JwtService>();
64: builder.Services.AddScoped<IPremiumCalculatorService, PremiumCalculatorService>();
65: builder.Services.AddScoped<IQuoteService, QuoteService>();
66: builder.Services.AddScoped<IExcelService, ExcelService>();
```
- **Purpose:** Registers business logic services against their contracts (`IAuthService`, `IQuoteService`, etc.).
- **Inversion of Control (IoC):** Controllers depend solely on interfaces, completely decoupled from concrete classes. This architecture makes it trivial to replace `PremiumCalculatorService` with an external third-party actuarial rating API without modifying a single line of controller code.

---

#### Lines 68–70: Strongly Typed Options Configuration
```csharp
68: // Configurations
69: builder.Services.Configure<PremiumSettings>(builder.Configuration.GetSection("PremiumSettings"));
70: builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
```
- **Framework Citation:** [Options Pattern in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)
- **Purpose:** Binds JSON configuration blocks (`"PremiumSettings"` and `"Jwt"`) directly into strongly typed C# classes (`PremiumSettings.cs` and `JwtSettings.cs`).
- **Advantage:** Injects `IOptions<PremiumSettings>` into services, eliminating magic strings, enabling compile-time type validation, and simplifying unit testing with `Options.Create(...)`.

---

#### Lines 72–87: JWT Bearer Authentication & Token Validation Engine
```csharp
72: var jwtSection = builder.Configuration.GetSection("Jwt");
73: var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
74: 
75: builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
76: {
77:     options.TokenValidationParameters = new TokenValidationParameters
78:     {
79:         ValidateIssuer = true,
80:         ValidateAudience = true,
81:         ValidateLifetime = true,
82:         ValidateIssuerSigningKey = true,
83:         ValidIssuer = jwtSection["Issuer"],
84:         ValidAudience = jwtSection["Audience"],
85:         IssuerSigningKey = new SymmetricSecurityKey(key)
86:     };
87: });
```
- **Framework Citation:** [TokenValidationParameters Class](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens.tokenvalidationparameters)
- **Deep Security Analysis:**
  - `Encoding.UTF8.GetBytes(...)`: Decodes the secret string into raw bytes for HMAC-SHA256 cryptography. The `!` is C#'s null-forgiving operator, asserting that `"Key"` exists in configuration.
  - `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`: Sets the default authentication scheme to Bearer tokens.
  - `TokenValidationParameters`: Strict security rules applied to **every** incoming request:
    1. `ValidateIssuer = true`: Rejects tokens where the issuer does not match `"PetInsuranceAPI"` (prevents tokens issued by untrusted servers).
    2. `ValidateAudience = true`: Rejects tokens where the audience is not `"PetInsuranceUI"` (prevents token reuse across different client apps).
    3. `ValidateLifetime = true`: Automatically checks the token's `exp` claim against current server UTC time. Expired tokens are rejected with `401 Unauthorized`.
    4. `ValidateIssuerSigningKey = true`: Recalculates the HMAC-SHA256 signature using `SymmetricSecurityKey(key)`. If a malicious actor tampers with even 1 bit of the token's payload, the signature check fails.

---

#### Line 89: WebApplication Instantiation
```csharp
89: var app = builder.Build();
```
- Compiles the dependency injection container and freezes service registrations. Instantiates the `WebApplication` host ready to configure the middleware pipeline.

---

#### Lines 91–96: Service Scope & Database Initialization
```csharp
91: // Seed Database
92: using (var scope = app.Services.CreateScope())
93: {
94:     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
95:     
96: }
```
- **Framework Citation:** [IServiceScope Interface](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescope)
- **Why `CreateScope()`?** Because `AppDbContext` is registered as `Scoped`, it cannot be resolved directly from root singleton services (`app.Services`). Doing so would cause a memory leak. `CreateScope()` creates a temporary execution boundary that safely instantiates and disposes scoped instances.

---

#### Lines 98–109: Environment-Specific Middleware Configuration
```csharp
98: // Configure the HTTP request pipeline.
99: if (app.Environment.IsDevelopment())
100: {
101:     
102:     app.MapOpenApi();
103:     app.UseSwagger();
104:     app.UseSwaggerUI();
105: }
106: else
107: {
108:     app.UseHttpsRedirection();
109: }
```
- In development mode (`IsDevelopment()`): Exposes interactive Swagger documentation (`/swagger`) and OpenAPI endpoints (`/openapi/v1.json`).
- In production: Enables `UseHttpsRedirection()` to enforce TLS/HTTPS encryption and protect tokens in transit.

---

#### Lines 111–121: Core Request Pipeline & Execution
```csharp
111: app.UseCors("AllowAngular");
112: 
113: app.UseMiddleware<GlobalExceptionMiddleware>();
114: 
115: app.UseAuthentication();
116: 
117: app.UseAuthorization();
118: 
119: app.MapControllers();
120: 
121: app.Run();
```
- **Order of Middleware Matters (Critical ASP.NET Core Principle):**
  1. `UseCors("AllowAngular")`: Must execute first so preflight `OPTIONS` requests receive immediate CORS headers before security checks.
  2. `UseMiddleware<GlobalExceptionMiddleware>()`: Positioned outer-most in the try/catch stack to catch unhandled errors from all subsequent middleware and controllers.
  3. `UseAuthentication()`: Decodes the JWT Bearer token and populates `HttpContext.User` with claims.
  4. `UseAuthorization()`: Validates whether the authenticated user has permission to access the endpoint (`[Authorize]` attribute).
  5. `MapControllers()`: Maps route attributes (e.g. `[Route("api/[controller]")]`) to controller actions.
  6. `app.Run()`: Starts the Kestrel web server listening on configured ports and blocks the calling thread to process incoming HTTP requests.

---

# Chapter 3: Deep-Dive into C# Types, Memory Layouts & CLR Primitives

---

### 3.1 `System.Decimal` vs `double` / `float`: 128-bit IEEE Representation & Financial Disasters

In `Quote.cs`, `QuoteCoverage.cs`, and `PremiumCalculatorService.cs`, all monetary values (`BasePremium`, `FinalPremium`, `Deductible`, etc.) are declared as `decimal`.

#### Microsoft Learn Citation
> **Documentation:** [decimal type (C# reference)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#characteristics-of-the-floating-point-types)  
> **CLR Type:** `System.Decimal` | **Assembly:** `System.Runtime.dll`

#### Bit-Level Memory Layout:
A `decimal` occupies **128 bits (16 bytes)** organized into four 32-bit integers:
```
Bit 0                                                                        Bit 127
┌──────────────────────────┬──────────────────────────┬──────────────────────┬─────────────┬────────────┐
│   Low 32-bit Mantissa    │   Mid 32-bit Mantissa    │ High 32-bit Mantissa │  Reserved   │ Scale/Sign │
│        (32 bits)         │        (32 bits)         │      (32 bits)       │  (16 bits)  │ (16 bits)  │
└──────────────────────────┴──────────────────────────┴──────────────────────┴─────────────┴────────────┘
```
1. **Bits 0–95 (96 bits):** An unsigned 96-bit integer representing the integer mantissa (up to $79,228,162,514,264,337,593,543,950,335$).
2. **Bits 96–111 (16 bits):** Unused (must be zero).
3. **Bits 112–126 (15 bits):** The scaling power factor $k$ where $0 \le k \le 28$. Specifies that the mantissa is divided by $10^k$.
4. **Bit 127 (1 bit):** Sign bit ($0 = \text{positive}$, $1 = \text{negative}$).

#### Real-World Insurance Bug Example with `double`:
Suppose an insurer calculates a 10% loyalty discount on a $100.00 base premium with a $29.95 optional wellness add-on over a 12-month billing period using `double`:
```csharp
double basePrem = 100.00;
double wellness = 29.95;
double total = (basePrem + wellness) * 0.90; // Expected $116.955
// In IEEE 754: 116.95499999999999744
```
Truncating or formatting this value produces `$116.95` instead of `$116.96`. Multiplied by 500,000 policyholders across 12 billing cycles, this results in a **$60,000 accounting reconciliation discrepancy**, triggering insurance commissioner fines and regulatory audit failures. `decimal` guarantees base-10 exactness without binary drift.

---

### 3.2 Date & Time Architecture: `DateOnly` vs `DateTime` vs `DateTimeOffset`

- `DateOnly` in [Pet.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Models/Pet.cs#L12) (`public DateOnly DateOfBirth { get; set; }`):
  - *Internal storage:* A single 32-bit integer representing the day index ($0 = 0001-01-01$).
  - *Zero Time & Timezone:* Completely eliminates timezone shifts. A pet born on May 10th is born on May 10th regardless of whether the query runs in New York, London, or Tokyo.
- `DateTime` in [Quote.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Models/Quote.cs#L11-L12) (`CreatedDate`, `ExpiryDate`):
  - *Storage:* 64-bit integer containing ticks (100-nanosecond intervals) plus 2-bit `DateTimeKind`.
  - Always populated using `DateTime.UtcNow` to guarantee unambiguous temporal comparisons across Daylight Saving Time changes.

---

### 3.3 Asynchronous Execution Mechanics: `async`, `await`, `Task`, `Task<T>`

#### Microsoft Learn Citation
> **Documentation:** [Asynchronous programming with async and await](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)

#### Roslyn State Machine Breakdown:
When the compiler encounters:
```csharp
public async Task<int> CreateQuoteAsync(CreateQuoteDto dto)
{
    // ...
    await _quoteRepository.AddQuoteAsync(quote);
    await _quoteRepository.SaveQuoteChangesAsync();
    return quote.QuoteId;
}
```
It synthesizes a private struct `[CompilerGenerated] struct <CreateQuoteAsync>d__8 : IAsyncStateMachine` containing:
- `public int <>1__state`: Tracks progress through the method ($-1 = \text{running}$, $0 = \text{paused at first await}$, $1 = \text{paused at second await}$).
- `public AsyncTaskMethodBuilder<int> <>t__builder`: Constructs the returned `Task<int>`.
- `public void MoveNext()`: Contains the transformed code split into a `switch (<>1__state)` block.

When `await SaveQuoteChangesAsync()` is called:
1. If the database task has completed synchronously, execution continues without pausing.
2. If the task is pending, `builder.AwaitUnsafeOnCompleted(...)` hooks the state machine continuation to the task.
3. The executing thread is **released back to the ThreadPool**.
4. When the OS I/O completes, the CLR ThreadPool schedules `MoveNext()` to resume execution right where it paused, returning `quote.QuoteId`.

---

# Chapter 4: Controllers: Line-by-Line & Function-by-Function Dissection

---

### 4.1 QuotesController.cs (Lines 1–112)

File Path: [QuotesController.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Controllers/QuotesController.cs)

#### Class Attributes & Constructor (Lines 7–19):
```csharp
7: namespace PetInsurance.Controllers
8: {
9:     [Authorize]
10:    [Route("api/[controller]")]
11:    [ApiController]
12:    public class QuotesController : ControllerBase
13:    {
14:        private readonly IQuoteService _quoteService;
15:  
16:        public QuotesController(IQuoteService quoteService)
17:        {
18:            _quoteService = quoteService;
19:        }
```
- **Line 9 (`[Authorize]`):** Microsoft authorization filter. Protects every action in this controller. Demands that incoming HTTP requests supply a valid JWT Bearer token in the `Authorization` header. Requests lacking valid credentials are rejected with `401 Unauthorized` before any action method is reached.
- **Line 10 (`[Route("api/[controller]")]`):** Defines the base URL route template. The token `[controller]` is automatically replaced with `"Quotes"`, establishing the root path `/api/quotes`.
- **Line 11 (`[ApiController]`):** Enables ASP.NET Core API behaviors:
  1. Automatic HTTP 400 responses on model validation errors (no need for `if (!ModelState.IsValid)`).
  2. Automatic binding source inference (`[FromBody]` for complex types, `[FromQuery]` for primitives).
  3. Problem Details specification error formatting.
- **Line 12 (`public class QuotesController : ControllerBase`):** Inherits from `ControllerBase` (the lightweight controller class for Web APIs without Razor View engine overhead).
- **Lines 14–19:** Constructor injection storing `IQuoteService` in a `private readonly` field.

---

#### Action 1: CreateQuote (Lines 21–30)
```csharp
21: [HttpPost]
22: public async Task<IActionResult> CreateQuote(CreateQuoteDto dto)
23: {
24:     int quoteId = await _quoteService.CreateQuoteAsync(dto);
25:     
26:     return CreatedAtAction(
27:         nameof(GetQuote),
28:         new { id = quoteId },
29:         new { QuoteId = quoteId });
30: }
```
- **Line 21 (`[HttpPost]`):** Maps HTTP `POST /api/quotes`.
- **Line 22 (`CreateQuoteDto dto`):** Deserialized automatically from the request's JSON body.
- **Line 24:** Invokes `CreateQuoteAsync`. If validations fail, `BusinessRuleException` is thrown and intercepted by middleware.
- **Lines 26–29:** Emits `201 Created` with `Location: /api/quotes/{quoteId}` and returns the new quote ID in the body.

---

#### Action 2: GetQuotes (Lines 32–37)
```csharp
32: [HttpGet]
33: public async Task<IActionResult> GetQuotes([FromQuery] SearchQuoteDto dto)
34: {
35:     var result = await _quoteService.GetQuotesAsync(dto);
36:     return Ok(result);
37: }
```
- **Line 33 (`[FromQuery] SearchQuoteDto dto`):** Binds query string parameters (`?email=...&species=...&pageNumber=1&pageSize=10`) into the DTO.
- **Lines 35–36:** Returns `200 OK` with a `PagedResultDto<QuoteListDto>` containing paginated items and total counts.

---

#### Action 3: GetDashboard (Lines 39–44)
```csharp
39: [HttpGet("dashboard")]
40: public async Task<IActionResult> GetDashboard()
41: {
42:     var result = await _quoteService.GetDashboardMetricsAsync();
43:     return Ok(result);
44: }
```
- **Line 39 (`[HttpGet("dashboard")]`):** Maps `GET /api/quotes/dashboard`.
- **Lines 42–43:** Computes actuarial metrics (total quotes, active count, conversion rate, average premium, expiring soon alerts, status breakdowns) and returns `200 OK`.

---

#### Action 4: GetQuote (Lines 46–57)
```csharp
46: [HttpGet("{id:int}")]
47: public async Task<IActionResult> GetQuote(int id)
48: {
49:     var quote = await _quoteService.GetQuoteByIdAsync(id);
50:     
51:     if (quote is null)
52:     {
53:         return NotFound();
54:     }
55:     
56:     return Ok(quote);
57: }
```
- **Line 46 (`{id:int}`):** Route constraint requiring `id` to be a valid 32-bit integer.
- **Lines 51–54:** If the quote does not exist, returns `404 Not Found`. Otherwise returns `200 OK` with full quote details.

---

#### Action 5: UpdateQuote (Lines 59–70)
```csharp
59: [HttpPut("{id:int}")]
60: public async Task<IActionResult> UpdateQuote(int id, UpdateQuoteDto dto)
61: {
62:     var success = await _quoteService.UpdateQuoteAsync(id, dto);
63: 
64:     if (!success)
65:     {
66:         return NotFound();
67:     }
68: 
69:     return Ok("Quote updated successfully.");
70: }
```
- **Line 59 (`[HttpPut("{id:int}")]`):** Maps HTTP `PUT /api/quotes/{id}` for updating eligible active quotes.
- Returns `404 Not Found` if ID is absent, or `200 OK` on success. Throws `BusinessRuleException` if the quote is expired, converted, or cancelled.

---

#### Action 6: ConvertQuote (Lines 72–83)
```csharp
72: [HttpPost("{id:int}/convert")]
73: public async Task<IActionResult> ConvertQuote(int id)
74: {
75:     var success = await _quoteService.ConvertQuoteAsync(id);
76:     
77:     if (!success)
78:     {
79:         return NotFound();
80:     }
81:     
82:     return Ok("Quote converted successfully.");
83: }
```
- Binds an active quote into a permanent policy. Enforces single-conversion invariants.

---

#### Action 7: RecalculateQuote (Lines 85–96)
```csharp
85: [HttpPost("{id:int}/recalculate")]
86: public async Task<IActionResult> RecalculateQuote(int id,[FromQuery] bool applyDiscount)
87: {
88:     var success = await _quoteService.RecalculateQuoteAsync(id,applyDiscount);
89:     
90:     if (!success)
91:     {
92:         return NotFound();
93:     }
94:     
95:     return Ok("Quote recalculated successfully.");
96: }
```
- Recalculates premiums dynamically when riders or discount toggles change.

---

#### Action 8: CancelQuote (Lines 98–109)
```csharp
98: [HttpDelete("{id:int}")]
99: public async Task<IActionResult> CancelQuote(int id)
100: {
101:     var success = await _quoteService.CancelQuoteAsync(id);
102:     
103:     if (!success)
104:     {
105:         return NotFound();
106:     }
107:     
108:     return Ok("Quote cancelled successfully.");
109: }
```
- Sets quote status to `QuoteStatus.Cancelled`. Moves the record to a permanent terminal state.

---

# Chapter 5: Business Logic & Service Layer: Complete Line-by-Line Dissection

---

### 5.1 QuoteService.cs: CreateQuoteAsync Line-by-Line (Lines 46–166)

File Path: [QuoteService.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Services/QuoteService.cs)

```csharp
46: public async Task<int> CreateQuoteAsync(CreateQuoteDto dto)
47: {
48:     if (string.IsNullOrWhiteSpace(dto.Email))
49:     {
50:         throw new BusinessRuleException("Customer email is required.");
51:     }
```
- **Lines 48–51:** Email existence invariant. `string.IsNullOrWhiteSpace` checks for `null`, empty string `""`, or whitespace `"   "`.

```csharp
53:     if (string.IsNullOrWhiteSpace(dto.PetName))
54:     {
55:         throw new BusinessRuleException("Pet name is required.");
56:     }
```
- **Lines 53–56:** Pet name existence invariant.

```csharp
58:     int petAge = DateTime.Today.Year - dto.DateOfBirth.Year;
59:     
60:     if (petAge <= 0)
61:     {
62:         throw new BusinessRuleException("Pet age must be greater than zero.");
63:     }
64:     
65:     if (petAge > _settings.MaxEligibleAge)
66:     {
67:         throw new BusinessRuleException("Pet is not eligible.");
68:     }
```
- **Lines 58–68:** Age validation invariants:
  - If `petAge <= 0`: Rejects newborn pets below minimum underwriting age.
  - If `petAge > _settings.MaxEligibleAge` (15 years): Rejects pets exceeding maximum coverage limits.

```csharp
70:     var customer = await _customerRepository.GetCustomerByEmailAsync(dto.Email);
71:     
72:     if (customer is null)
73:     {
74:         customer = new Customer
75:         {
76:             FirstName = dto.FirstName,
77:             LastName = dto.LastName,
78:             Email = dto.Email,
79:             Phone = dto.Phone,
80:             ZipCode = dto.ZipCode
81:         };
82:         
83:         await _customerRepository.AddCustomerAsync(customer);
84:         await _customerRepository.SaveCustomerChangesAsync();
85:     }
```
- **Lines 70–85 (Customer Identity Upsert):** Queries customer by unique email. If absent, instantiates and saves a new customer record to obtain a generated `CustomerId`.

```csharp
87:     var pet = await _petRepository.GetPetByCustomerAndNameAsync(customer.CustomerId, dto.PetName);
88:     
89:     if (pet is null)
90:     {
91:         pet = new Pet
92:         {
93:             CustomerId = customer.CustomerId,
94:             PetName = dto.PetName,
95:             Species = dto.Species,
96:             Breed = dto.Breed,
97:             DateOfBirth = dto.DateOfBirth,
98:             Gender = dto.Gender,
99:             HasPreExistingCondition = dto.HasPreExistingCondition
100:        };
101:        
102:        await _petRepository.AddPetAsync(pet);
103:        await _petRepository.SavePetChangesAsync();
104:    }
```
- **Lines 87–104 (Pet Identity Upsert):** Queries pet by `(CustomerId, PetName)`. If absent, registers the pet linked to the customer.

```csharp
106:    bool duplicate = await _quoteRepository 
107:        .HasDuplicateActiveQuoteAsync(
108:        customer.CustomerId,
109:        pet.PetId,
110:        dto.AnnualLimit,
111:        dto.Deductible,
112:        dto.ReimbursementPct,
113:        dto.Wellness);
114:    
115:    if (duplicate)
116:    {
117:        throw new BusinessRuleException("Duplicate active quote exists.");
118:    }
```
- **Lines 106–118 (Deduplication Gate):** Checks whether an identical active quote already exists for this exact configuration. Throws `BusinessRuleException` if true.

```csharp
120:    var premium = _premiumCalculatorService.CalculatePremium(petAge, dto.Wellness, false);
121:    
122:    var quote = new Quote
123:    {
124:        QuoteNumber = $"QT-{Guid.NewGuid():N}"[..10],
125:        CustomerId = customer.CustomerId,
126:        PetId = pet.PetId,
127:        CreatedDate = DateTime.UtcNow,
128:        ExpiryDate = DateTime.UtcNow.AddYears(1),
129:        Status = QuoteStatus.Active,
130:        BasePremium = premium.BasePremium,
131:        AgeAdjustment = premium.AgeAdjustment,
132:        WellnessAmount = premium.WellnessAmount,
133:        DiscountAmount = premium.DiscountAmount,
134:        FinalPremium = premium.FinalPremium
135:    };
136:    
137:    await _quoteRepository.AddQuoteAsync(quote);
138:    await _quoteRepository.SaveQuoteChangesAsync();
```
- **Lines 120–138 (Actuarial Calculation & Quote Creation):** Calculates premiums, generates an alphanumeric quote number (`QT-` prefix + 7 GUID characters), and persists the quote.

```csharp
150:    var coverage = new QuoteCoverage
151:    {
152:        QuoteId = quote.QuoteId,
153:        AnnualLimit = dto.AnnualLimit,
154:        Deductible = dto.Deductible,
155:        ReimbursementPct = dto.ReimbursementPct,
156:        Wellness = dto.Wellness
157:    };
158:    
159:    await _quoteCoverageRepository.AddQuoteAsync(coverage);
160:    await _quoteCoverageRepository.SaveQuoteChangesAsync();
161:    
162:    _logger.LogInformation("Quote created {QuoteNumber}", quote.QuoteNumber);
163:    
164:    return quote.QuoteId;
165: }
```
- **Lines 150–165:** Creates the 1:1 `QuoteCoverage` child entity, logs creation, and returns the generated `QuoteId`.

---

### 5.2 PremiumCalculatorService.cs: CalculatePremium Line-by-Line (Lines 17–61)

File Path: [PremiumCalculatorService.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Services/PremiumCalculatorService.cs)

```csharp
17: public (
18:     decimal BasePremium,
19:     decimal AgeAdjustment,
20:     decimal WellnessAmount,
21:     decimal DiscountAmount,
22:     decimal FinalPremium
23: ) CalculatePremium(
24:     int petAge,
25:     bool wellnessSelected,
26:     bool applyDiscount)
27: {
28:     decimal basePremium = _settings.BasePremium;
29:     decimal ageAdjustment = 0;
30:     decimal wellnessAmount = 0;
31:     decimal discountAmount = 0;
```
- **Lines 17–27:** Returns a named C# **ValueTuple** (`System.ValueTuple`). Allocates on the stack with **zero heap allocation overhead**.

```csharp
36:     if (petAge >= _settings.SeniorPetAge)
37:     {
38:         ageAdjustment = _settings.SeniorPetAdjustment;
39:     }
```
- **Lines 36–39:** Senior Pet Surcharge. Adds configured senior adjustment if pet age meets or exceeds threshold ($8$ years).

```csharp
41:     if (wellnessSelected)
42:     {
43:         wellnessAmount = _settings.WellnessAmount;
44:     }
```
- **Lines 41–44:** Wellness rider add-on amount ($25.00$).

```csharp
46:     if (applyDiscount)
47:     {
48:         discountAmount = _settings.DiscountAmount > 0 ? _settings.DiscountAmount : 10m;
49:     }
```
- **Lines 46–49:** Promotional / multi-pet discount deduction ($10.00$).

```csharp
51:     decimal finalPremium = basePremium + ageAdjustment + wellnessAmount - discountAmount;
52:     
53:     finalPremium = Math.Max(0, finalPremium);
54:     
55:     return (basePremium, ageAdjustment, wellnessAmount, discountAmount, finalPremium);
56: }
```
- **Lines 51–56:** Computes final premium with negative floor clamping (`Math.Max(0, finalPremium)`).

---

# Chapter 6: Data Persistence & Repository Layer: Complete Line-by-Line Dissection

---

### 6.1 AppDbContext.cs (Lines 1–49)

File Path: [AppDbContext.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Data/AppDbContext.cs)

```csharp
14: public DbSet<Customer> Customers => Set<Customer>();
15: public DbSet<Pet> Pets => Set<Pet>();
16: public DbSet<Quote> Quotes => Set<Quote>();
17: public DbSet<QuoteCoverage> QuoteCoverages => Set<QuoteCoverage>();
18: public DbSet<AppUser> Users => Set<AppUser>();
```
- **`DbSet<T>`:** Represents the in-memory gateway and query root for each database table. Using expression-bodied `Set<T>()` avoids compiler nullability warnings.

```csharp
19: protected override void OnModelCreating(ModelBuilder modelBuilder)
20: {
21:     base.OnModelCreating(modelBuilder);
22: 
23:     // Customer -> Pets (1:M)
24:     modelBuilder.Entity<Customer>()
25:     .HasMany(c => c.Pets)
26:     .WithOne(p => p.Customer)
27:     .HasForeignKey(p => p.CustomerId);
28:     
29:     // Customer -> Quotes (1:M)
30:     modelBuilder.Entity<Customer>()
31:     .HasMany(c => c.Quotes)
32:     .WithOne(q => q.Customer)
33:     .HasForeignKey(q => q.CustomerId);
34:     
35:     // Pet -> Quotes (1:M)
36:     modelBuilder.Entity<Pet>()
37:     .HasMany(p => p.Quotes)
38:     .WithOne(q => q.Pet)
39:     .HasForeignKey(q => q.PetId);
40:     
41:     // Quote -> QuoteCoverage (1:1)
42:     modelBuilder.Entity<Quote>()
43:     .HasOne(q => q.QuoteCoverage)
44:     .WithOne(qc => qc.Quote)
45:     .HasForeignKey<QuoteCoverage>(qc => qc.QuoteId);
46: }
```
- **Fluent API Relational Configurations:**
  - `HasMany(...).WithOne(...).HasForeignKey(...)`: Explicitly configures cascading foreign keys for 1:M relationships.
  - `HasOne(...).WithOne(...).HasForeignKey<QuoteCoverage>(...)`: Establishes a true 1:1 relationship with a unique foreign key index on `QuoteCoverages.QuoteId`.

---

### 6.2 QuoteRepository.cs: Advanced LINQ Queries (Lines 80–199)

File Path: [QuoteRepository.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Repositories/QuoteRepository.cs)

#### Dynamic Search & Paging Method (`SearchQuotesAsync`):
```csharp
80: public async Task<(List<Quote> Items, int TotalCount)> SearchQuotesAsync(SearchQuoteDto dto)
81: {
82:     var query = _context.Quotes
83:         .Include(q => q.Customer)
84:         .Include(q => q.Pet)
85:         .Include(q => q.QuoteCoverage)
86:         .AsQueryable();
```
- **`.Include(...)`:** Configures SQL `LEFT JOIN` operations (Eager Loading) to retrieve related Customer, Pet, and Coverage entities in a single round-trip.
- **`.AsQueryable()`:** Defers query execution. Expressions added below modify the underlying SQL query tree rather than pulling records into memory.

```csharp
88:     if (!string.IsNullOrWhiteSpace(dto.SearchQuery))
89:     {
90:         var sqLower = dto.SearchQuery.ToLower();
91:         query = query.Where(q =>
92:             (q.Customer != null && (q.Customer.Email.ToLower().Contains(sqLower) || 
93:                                     q.Customer.FirstName.ToLower().Contains(sqLower) || 
94:                                     q.Customer.LastName.ToLower().Contains(sqLower))) ||
95:             (q.Pet != null && q.Pet.PetName.ToLower().Contains(sqLower)) ||
96:             (q.QuoteNumber != null && q.QuoteNumber.ToLower().Contains(sqLower)));
97:     }
```
- **Multi-Field Full-Text Search:** Translates into parameterized SQL `LIKE` statements combined with boolean `OR` conditions.

```csharp
140:    int totalCount = await query.CountAsync();
141: 
142:    int pageNumber = dto.PageNumber > 0 ? dto.PageNumber : 1;
143:    int pageSize = dto.PageSize > 0 ? dto.PageSize : 10;
144: 
145:    var items = await query
146:        .OrderByDescending(q => q.CreatedDate)
147:        .Skip((pageNumber - 1) * pageSize)
148:        .Take(pageSize)
149:        .ToListAsync();
150: 
151:    return (items, totalCount);
152: }
```
- **Database Pagination:**
  - `CountAsync()`: Executes `SELECT COUNT(*) FROM Quotes` to calculate total pages.
  - `OrderByDescending(...)`: Sorts newest quotes first.
  - `Skip(...)` and `Take(...)`: Translates to SQL `OFFSET` and `LIMIT`, fetching only the requested page slice.

---

# Chapter 7: Middleware, Error Handling & Exceptions

---

### 7.1 GlobalExceptionMiddleware.cs (Lines 19–58)

File Path: [GlobalExceptionMiddleware.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/Middleware/GlobalExceptionMiddleware.cs)

```csharp
19: public async Task InvokeAsync(HttpContext context)
20: {
21:     try
22:     {
23:         await _next(context);
24:     }
25:     catch (BusinessRuleException ex)
26:     {
27:         context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
28:         context.Response.ContentType = "application/json";
29:         var response = new
30:         {
31:             Message = ex.Message
32:         };
33:         await context.Response.WriteAsync(JsonSerializer.Serialize(response));
34:     }
35:     catch (Exception ex)
36:     {
45:         _logger.LogError(ex, "Unhandled exception occurred");
48:         context.Response.StatusCode = 500;
50:         await context.Response.WriteAsJsonAsync(new
51:         {
52:             Message = ex.Message,
53:             InnerException = ex.InnerException?.Message,
54:             StackTrace = ex.StackTrace
55:         });
56:     }
57: }
```
- **Two-Tier Exception Strategy:**
  1. **Tier 1 (`BusinessRuleException`):** Intercepts predictable domain violations (e.g., duplicate quotes, age limits). Returns clean `400 Bad Request` JSON payloads.
  2. **Tier 2 (`Exception`):** Catches unexpected infrastructure or system failures (e.g., database connection loss, disk errors). Logs the full exception to the structured logger and returns `500 Internal Server Error`.

---

# Chapter 8: Domain Models & Data Transfer Objects (DTOs)

### Anti-Corruption Layer Architecture:
Domain entities (`Quote.cs`, `Customer.cs`) represent the internal database schema. They are **never exposed directly** over API endpoints. DTOs decouple external API contracts from internal storage schemas:
1. **[CreateQuoteDto.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/DTOs/CreateQuoteDto.cs):** Captures customer, pet, and coverage inputs in a single payload.
2. **[QuoteListDto.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/DTOs/QuoteListDto.cs):** Flattened, lightweight representation optimized for data grids.
3. **[QuoteDetailsDto.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/DTOs/QuoteDetailsDto.cs):** Comprehensive object returning complete premium breakdowns, policy limits, and customer details.
4. **[DashboardDto.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/DTOs/DashboardDto.cs):** Aggregated metrics for high-level management dashboards.
5. **[PagedResultDto.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsurance/DTOs/PagedResultDto.cs):** Generic wrapper (`PagedResultDto<T>`) computing `TotalPages = (int)Math.Ceiling(TotalCount / PageSize)`.

---

# Chapter 9: Comprehensive Edge Cases & Validation Matrix

| Field / Feature | Input Value / Condition | Validation Rule / Check | System Response |
| :--- | :--- | :--- | :--- |
| **Email** | `null`, `""`, or `"   "` | `string.IsNullOrWhiteSpace(dto.Email)` | Throws `BusinessRuleException("Customer email is required.")` |
| **Pet Name** | `null`, `""`, or `"   "` | `string.IsNullOrWhiteSpace(dto.PetName)` | Throws `BusinessRuleException("Pet name is required.")` |
| **Pet Age** | DOB in the future or today ($\le 0$) | `petAge <= 0` | Throws `BusinessRuleException("Pet age must be greater than zero.")` |
| **Pet Age** | Age $> 15$ years | `petAge > _settings.MaxEligibleAge` | Throws `BusinessRuleException("Pet is not eligible.")` |
| **Quote Status** | Updating `Converted` quote | `quote.Status == QuoteStatus.Converted` | Throws `BusinessRuleException("An expired, cancelled, or converted quote cannot be modified.")` |
| **Quote Status** | Updating `Expired` quote | `quote.Status == QuoteStatus.Expired` | Throws `BusinessRuleException("An expired, cancelled, or converted quote cannot be modified.")` |
| **Conversion** | Re-converting converted quote | `quote.Status == QuoteStatus.Converted` | Throws `BusinessRuleException("Quote cannot be converted.")` |
| **Deduplication** | Exact duplicate active quote | Matches Customer + Pet + Coverages | Throws `BusinessRuleException("Duplicate active quote exists.")` |
| **Actuarial Math**| High discount exceeding charges | `finalPremium < 0` | Clamped to `$0.00` via `Math.Max(0, finalPremium)` |
| **Excel Ingestion**| Non-`.xlsx` file uploaded | `!file.FileName.EndsWith(".xlsx")` | Throws `BusinessRuleException("Only .xlsx Excel files are supported.")` |
| **Excel Ingestion**| Missing worksheets | Missing `Customers` or `Quotes` | Throws `BusinessRuleException("Excel file must contain 'Customers' and 'Quotes' worksheets.")` |

---

# Chapter 10: Automated Test Suite Dissection (All 21 Tests Explained)

Located in the project directory [PetInsuranceTests](file:///d:/Dotnet/PetInsurancePOC/PetInsuranceTests/):
- **Testing Frameworks:** NUnit 4, Moq 4.20, FluentAssertions 8.0, Microsoft.EntityFrameworkCore.Sqlite.
- **Verification Status:** **All 21 tests pass with zero warnings or errors** (`dotnet test` execution duration: 7 seconds).

```
PetInsuranceTests Test Suite Topology:
├── AuthServiceTests (UnitTest1.cs) - 3 Tests
│   ├── RegisterAsync_NewUser_RegistersSuccessfully
│   ├── LoginAsync_ValidCredentials_ReturnsJwtToken
│   └── LoginAsync_InvalidPassword_ReturnsNull
├── QuoteServiceTests.cs - 6 Tests
│   ├── CreateQuoteAsync_ValidInput_CalculatesPremiumAndSets1yYearExpiry
│   ├── CreateQuoteAsync_ZeroOrNegativeAge_ThrowsBusinessRuleException
│   ├── CreateQuoteAsync_DuplicateActiveQuote_ThrowsBusinessRuleException
│   ├── ConvertQuoteAsync_ActiveQuote_ConvertsSuccessfully
│   ├── ConvertQuoteAsync_AlreadyConvertedOrExpired_ThrowsBusinessRuleException
│   └── UpdateQuoteAsync_ExpiredQuote_ThrowsBusinessRuleException
├── PremiumCalculatorServiceTests.cs - 3 Tests
│   ├── CalculatePremium_StandardPet_ReturnsBasePremium
│   ├── CalculatePremium_SeniorPetWithWellnessAndDiscount_AppliesAllAdjustments
│   └── CalculatePremium_HighDiscount_FinalPremiumCannotBeNegative
├── QuoteRepositoryLinqTests.cs - 3 Tests (SQLite In-Memory Integration)
│   ├── GetDashboardMetricsAsync_CalculatesAveragePremiumAndGroupsByStatus
│   ├── SearchQuotesAsync_FilterBySpeciesAndPageResults_ReturnsFilteredQuotes
│   └── HasDuplicateActiveQuoteAsync_ExistingActiveQuote_ReturnsTrue
├── QuotesControllerTests.cs - 4 Tests (Web API Action Results)
│   ├── CreateQuote_ValidDto_ReturnsCreatedAtActionResult
│   ├── GetQuote_ExistingId_ReturnsOkResultWithQuote
│   ├── GetQuote_MissingId_ReturnsNotFoundResult
│   └── ConvertQuote_ValidId_ReturnsOkResult
└── ExcelServiceTests.cs - 2 Tests (OpenXML Streaming & Deserialization)
    ├── ExportQuotesToExcelAsync_ExportsCompleteQuoteAndPetData
    └── ImportQuotesFromExcelAsync_ImportsCompleteDataCorrectly
```

---

### 10.1 Testing Principles & Architecture Mechanics

#### 1. The Arrange-Act-Assert (AAA) Pattern
Every test method in the suite follows the industry-standard AAA pattern:
- **Arrange:** Initialize variables, prepare DTOs, configure mock expectations (`_mockRepo.Setup(...)`), or seed in-memory SQLite tables.
- **Act:** Execute the target method under test (`await _service.CreateQuoteAsync(dto)`).
- **Assert:** Verify outcome states (`result.Should().Be(...)`) and confirm method invocations on dependencies (`_mockRepo.Verify(..., Times.Once)`).

#### 2. Moq Dynamic Proxy Mechanics
Moq relies on **Castle.Core DynamicProxy** at runtime. When `new Mock<IQuoteRepository>()` is called:
- DynamicProxy generates an in-memory class that implements `IQuoteRepository`.
- When `_mock.Setup(r => r.GetQuoteByIdAsync(5)).ReturnsAsync(quote)` is invoked, Moq intercepts the LINQ expression tree, records the parameter matcher (5), and maps it to return the pre-configured object.
- `_mock.Verify(..., Times.Once)` inspects Moq's internal call log to guarantee that the method was invoked exactly once with matching arguments, detecting bugs where a service fails to persist data or accidentally calls `SaveChanges` multiple times.

#### 3. In-Memory SQLite Integration Lifecycle
Standard EF Core In-Memory database providers (`UseInMemoryDatabase`) do not enforce relational integrity, foreign key constraints, or SQL syntax. To guarantee authentic database testing, `QuoteRepositoryLinqTests` and `ExcelServiceTests` utilize real **SQLite In-Memory (`Filename=:memory:`)**:
```csharp
[SetUp]
public async Task Setup()
{
    // SQLite in-memory databases exist ONLY as long as their physical connection remains open!
    _connection = new SqliteConnection("Filename=:memory:");
    await _connection.OpenAsync();

    _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(_connection)
        .Options;

    _context = new AppDbContext(_dbContextOptions);
    await _context.Database.EnsureCreatedAsync(); // Translates Fluent API into real DDL SQL!
}
```
- In `TearDown()`, `_context.DisposeAsync()` and `_connection.CloseAsync()` cleanly purge the database from memory, guaranteeing zero state leakage between tests.

---

### 10.2 Line-by-Line Dissection of All 21 Tests

---

#### Group 1: AuthServiceTests ([UnitTest1.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsuranceTests/UnitTest1.cs))

```csharp
[Test]
public async Task RegisterAsync_NewUser_RegistersSuccessfully()
{
    // Arrange
    var dto = new RegisterDto { UserName = "newuser", Password = "Password123!" };
    _mockUserRepo.Setup(r => r.GetUserByUserNameAsync("newuser")).ReturnsAsync((AppUser?)null);

    // Act
    await _authService.RegisterUserAsync(dto);

    // Assert
    _mockUserRepo.Verify(r => r.AddUserAsync(It.Is<AppUser>(u => u.UserName == "newuser")), Times.Once);
    _mockUserRepo.Verify(r => r.SaveUserChangesAsync(), Times.Once);
}
```
- **How it works:**
  - *Arrange:* Sets up `_mockUserRepo` so querying for `"newuser"` returns `null` (simulating a brand-new, unique username).
  - *Act:* Calls `RegisterUserAsync`.
  - *Assert:* `Verify` checks that `AddUserAsync` was called with an `AppUser` whose username is `"newuser"`, and that `SaveUserChangesAsync` was committed to disk exactly once.

```csharp
[Test]
public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
{
    // Arrange
    string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
    var user = new AppUser { AppUserId = 1, UserName = "admin", PasswordHash = hashedPassword };

    _mockUserRepo.Setup(r => r.GetUserByUserNameAsync("admin")).ReturnsAsync(user);
    _mockJwtService.Setup(j => j.GenerateToken(user)).Returns("mocked-jwt-token");

    // Act
    var result = await _authService.LoginUserAsync(new LoginDto { UserName = "admin", Password = "Password123!" });

    // Assert
    result.Should().NotBeNull();
    result!.Token.Should().Be("mocked-jwt-token");
}
```
- **How it works:**
  - *Arrange:* Creates a pre-hashed BCrypt password. Configures `_mockJwtService` to return a dummy token string when the verified user is passed.
  - *Act:* Invokes `LoginUserAsync` with matching plain-text password `"Password123!"`.
  - *Assert:* Confirms that the login response is not null and delivers the signed JWT token.

```csharp
[Test]
public async Task LoginAsync_InvalidPassword_ReturnsNull()
{
    // Arrange
    string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
    var user = new AppUser { AppUserId = 1, UserName = "admin", PasswordHash = hashedPassword };

    _mockUserRepo.Setup(r => r.GetUserByUserNameAsync("admin")).ReturnsAsync(user);

    // Act
    var result = await _authService.LoginUserAsync(new LoginDto { UserName = "admin", Password = "WrongPassword" });

    // Assert
    result.Should().BeNull();
}
```
- **How it works:**
  - Negative test case. Passes an incorrect password (`"WrongPassword"`). `BCrypt.Verify` evaluates to `false`, and the service safely returns `null`, preventing unauthorized access.

---

#### Group 2: QuoteServiceTests ([QuoteServiceTests.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsuranceTests/QuoteServiceTests.cs))

```csharp
[Test]
public async Task CreateQuoteAsync_ValidInput_CalculatesPremiumAndSets1yYearExpiry()
{
    // Arrange
    var dto = new CreateQuoteDto
    {
        FirstName = "Alice", LastName = "Smith", Email = "alice@example.com",
        PetName = "Max", Species = "Dog", Breed = "Labrador",
        DateOfBirth = DateOnly.FromDateTime(DateTime.Today).AddYears(-3),
        AnnualLimit = 5000, Deductible = 250, ReimbursementPct = 80, Wellness = true
    };

    var customer = new Customer { CustomerId = 1, Email = dto.Email };
    var pet = new Pet { PetId = 10, CustomerId = 1, PetName = dto.PetName };

    _mockCustomerRepo.Setup(r => r.GetCustomerByEmailAsync(dto.Email)).ReturnsAsync(customer);
    _mockPetRepo.Setup(r => r.GetPetByCustomerAndNameAsync(1, dto.PetName)).ReturnsAsync(pet);
    _mockQuoteRepo.Setup(r => r.HasDuplicateActiveQuoteAsync(1, 10, 5000, 250, 80, true)).ReturnsAsync(false);
    _mockCalculator.Setup(c => c.CalculatePremium(3, true, false)).Returns((50, 0, 15, 0, 65));

    // Act
    int quoteId = await _quoteService.CreateQuoteAsync(dto);

    // Assert
    _mockQuoteRepo.Verify(r => r.AddQuoteAsync(It.Is<Quote>(q =>
        q.Status == QuoteStatus.Active &&
        q.FinalPremium == 65 &&
        (q.ExpiryDate - q.CreatedDate).TotalDays >= 29)), Times.Once);

    _mockQuoteRepo.Verify(r => r.SaveQuoteChangesAsync(), Times.AtLeastOnce);
}
```
- **How it works:**
  - Tests the complete happy-path quote creation workflow.
  - Verifies that:
    1. Rating engine is invoked with age $3$ and wellness `true`.
    2. Created quote is assigned `QuoteStatus.Active`.
    3. `FinalPremium` matches calculated value ($65.00$).
    4. Quote expiration date is set to a valid future timestamp ($\ge 29$ days).

```csharp
[Test]
public void CreateQuoteAsync_ZeroOrNegativeAge_ThrowsBusinessRuleException()
{
    // Arrange: Pet DOB is set to today (Age = 0)
    var dto = new CreateQuoteDto
    {
        Email = "test@example.com",
        PetName = "Puppy",
        DateOfBirth = DateOnly.FromDateTime(DateTime.Today)
    };

    // Act
    Func<Task> act = async () => await _quoteService.CreateQuoteAsync(dto);

    // Assert
    act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*Pet age must be greater than zero*");
}
```
- **How it works:**
  - Boundary test. Evaluates whether newborn pets (age 0) are blocked.
  - Wraps the asynchronous call in a delegate `Func<Task> act` and asserts that it throws `BusinessRuleException` with the specific message pattern.

```csharp
[Test]
public void CreateQuoteAsync_DuplicateActiveQuote_ThrowsBusinessRuleException()
{
    // Arrange: Duplicate check returns true
    _mockQuoteRepo.Setup(r => r.HasDuplicateActiveQuoteAsync(2, 20, 5000, 250, 80, false)).ReturnsAsync(true);

    // Act
    Func<Task> act = async () => await _quoteService.CreateQuoteAsync(dto);

    // Assert
    act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*Duplicate active quote exists*");
}
```
- **How it works:**
  - Anti-spam deduplication test. Confirms that attempting to generate a quote for an identical configuration throws `BusinessRuleException`.

```csharp
[Test]
public async Task ConvertQuoteAsync_ActiveQuote_ConvertsSuccessfully()
{
    // Arrange: Active quote with ID 5
    var quote = new Quote { QuoteId = 5, Status = QuoteStatus.Active };
    _mockQuoteRepo.Setup(r => r.GetQuoteByIdAsync(5)).ReturnsAsync(quote);

    // Act
    bool result = await _quoteService.ConvertQuoteAsync(5);

    // Assert
    result.Should().BeTrue();
    quote.Status.Should().Be(QuoteStatus.Converted);
    _mockQuoteRepo.Verify(r => r.UpdateQuote(quote), Times.Once);
    _mockQuoteRepo.Verify(r => r.SaveQuoteChangesAsync(), Times.Once);
}
```
- **How it works:**
  - Tests policy binding. Asserts that the quote status mutates from `Active` to `Converted`, and that the updated entity is saved.

```csharp
[Test]
public void ConvertQuoteAsync_AlreadyConvertedOrExpired_ThrowsBusinessRuleException()
{
    // Arrange: Quote is already Converted
    var quote = new Quote { QuoteId = 6, Status = QuoteStatus.Converted };
    _mockQuoteRepo.Setup(r => r.GetQuoteByIdAsync(6)).ReturnsAsync(quote);

    // Act
    Func<Task> act = async () => await _quoteService.ConvertQuoteAsync(6);

    // Assert
    act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*Quote cannot be converted*");
}
```
- **How it works:**
  - Invariant test. Prevents re-converting an already bound policy.

```csharp
[Test]
public void UpdateQuoteAsync_ExpiredQuote_ThrowsBusinessRuleException()
{
    // Arrange: Quote has expired
    var quote = new Quote { QuoteId = 7, Status = QuoteStatus.Expired };
    _mockQuoteRepo.Setup(r => r.GetQuoteByIdAsync(7)).ReturnsAsync(quote);

    // Act
    Func<Task> act = async () => await _quoteService.UpdateQuoteAsync(7, new UpdateQuoteDto());

    // Assert
    act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*An expired, cancelled, or converted quote cannot be modified*");
}
```
- **How it works:**
  - Immutability test. Verifies that dead/expired quotes cannot have their coverages or rates retroactively altered.

---

#### Group 3: PremiumCalculatorServiceTests ([PremiumCalculatorServiceTests.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsuranceTests/PremiumCalculatorServiceTests.cs))

```csharp
[Test]
public void CalculatePremium_StandardPet_ReturnsBasePremium()
{
    // Arrange & Act (Age 3, No Wellness, No Discount)
    var result = _calculator.CalculatePremium(petAge: 3, wellnessSelected: false, applyDiscount: false);

    // Assert
    result.BasePremium.Should().Be(50.00m);
    result.AgeAdjustment.Should().Be(0.00m);
    result.WellnessAmount.Should().Be(0.00m);
    result.DiscountAmount.Should().Be(0.00m);
    result.FinalPremium.Should().Be(50.00m);
}
```
- **How it works:**
  - Verifies the baseline actuarial equation for young pets without elective riders: $\$50 + 0 + 0 - 0 = \$50.00$.

```csharp
[Test]
public void CalculatePremium_SeniorPetWithWellnessAndDiscount_AppliesAllAdjustments()
{
    // Arrange & Act (Age 8 >= SeniorAge 7, Wellness true, Discount true)
    var result = _calculator.CalculatePremium(petAge: 8, wellnessSelected: true, applyDiscount: true);

    // Assert: 50 (base) + 20 (senior) + 15 (wellness) - 10 (discount) = 75
    result.BasePremium.Should().Be(50.00m);
    result.AgeAdjustment.Should().Be(20.00m);
    result.WellnessAmount.Should().Be(15.00m);
    result.DiscountAmount.Should().Be(10.00m);
    result.FinalPremium.Should().Be(75.00m);
}
```
- **How it works:**
  - Tests combined additive and deductive adjustments: $\$50 + \$20 + \$15 - \$10 = \$75.00$.

```csharp
[Test]
public void CalculatePremium_HighDiscount_FinalPremiumCannotBeNegative()
{
    // Arrange: Base is $50, Discount is $100
    var settingsHighDiscount = Options.Create(new PremiumSettings
    {
        BasePremium = 50.00m,
        DiscountAmount = 100.00m
    });
    var calculator = new PremiumCalculatorService(settingsHighDiscount);

    // Act
    var result = calculator.CalculatePremium(petAge: 2, wellnessSelected: false, applyDiscount: true);

    // Assert: 50 - 100 = -50, clamped via Math.Max(0, -50) = 0
    result.FinalPremium.Should().Be(0.00m);
}
```
- **How it works:**
  - Mathematical boundary test. Verifies that `Math.Max(0, finalPremium)` guarantees the customer is never credited a negative premium balance.

---

#### Group 4: QuoteRepositoryLinqTests ([QuoteRepositoryLinqTests.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsuranceTests/QuoteRepositoryLinqTests.cs))

```csharp
[Test]
public async Task GetDashboardMetricsAsync_CalculatesAveragePremiumAndGroupsByStatus()
{
    // Act (Executes real LINQ AverageAsync and GroupBy on SQLite in-memory)
    var metrics = await _repository.GetDashboardMetricsAsync();

    // Assert: Seeded quotes have FinalPremiums: 100, 200, 300 -> Average = 200.00
    metrics.TotalQuotes.Should().Be(3);
    metrics.ActiveQuotes.Should().Be(2);
    metrics.ConvertedQuotes.Should().Be(1);
    metrics.AveragePremium.Should().Be(200.00m);
    metrics.QuotesByStatus["Active"].Should().Be(2);
    metrics.QuotesByStatus["Converted"].Should().Be(1);
}
```
- **How it works:**
  - Tests SQLite SQL query compilation for `AVG(FinalPremium)` and `GROUP BY Status`. Verifies dictionary projection of status aggregates.

```csharp
[Test]
public async Task SearchQuotesAsync_FilterBySpeciesAndPageResults_ReturnsFilteredQuotes()
{
    // Arrange: Search for Dog species with PageSize = 1
    var dto = new SearchQuoteDto { Species = "Dog", PageNumber = 1, PageSize = 1 };

    // Act (Executes Where, OrderByDescending, Skip, Take)
    var (items, totalCount) = await _repository.SearchQuotesAsync(dto);

    // Assert: 2 Dog quotes exist in DB. Paging returns 1 item, newest first (QT-003).
    totalCount.Should().Be(2);
    items.Should().HaveCount(1);
    items[0].QuoteNumber.Should().Be("QT-003");
}
```
- **How it works:**
  - Tests combined relational joins, species filtering, descending timestamp ordering, and SQL `LIMIT 1 OFFSET 0`.

```csharp
[Test]
public async Task HasDuplicateActiveQuoteAsync_ExistingActiveQuote_ReturnsTrue()
{
    // Arrange: Insert QuoteCoverage for QT-001 (5000 limit, 250 ded, 80% reimb, false wellness)
    var q = await _context.Quotes.FirstAsync(q => q.QuoteNumber == "QT-001");
    var coverage = new QuoteCoverage { QuoteId = q.QuoteId, AnnualLimit = 5000, Deductible = 250, ReimbursementPct = 80, Wellness = false };
    await _context.QuoteCoverages.AddAsync(coverage);
    await _context.SaveChangesAsync();

    // Act (Executes LINQ AnyAsync)
    bool exists = await _repository.HasDuplicateActiveQuoteAsync(q.CustomerId, q.PetId, 5000, 250, 80, false);

    // Assert
    exists.Should().BeTrue();
}
```
- **How it works:**
  - Tests EF Core translation to SQL `EXISTS (SELECT 1 FROM Quotes WHERE ...)` across navigation properties.

---

#### Group 5: QuotesControllerTests ([QuotesControllerTests.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsuranceTests/QuotesControllerTests.cs))

```csharp
[Test]
public async Task CreateQuote_ValidDto_ReturnsCreatedAtActionResult()
{
    // Arrange
    var dto = new CreateQuoteDto { Email = "john@example.com", PetName = "Buddy" };
    _mockQuoteService.Setup(s => s.CreateQuoteAsync(dto)).ReturnsAsync(101);

    // Act
    var result = await _controller.CreateQuote(dto);

    // Assert
    result.Should().BeOfType<CreatedAtActionResult>();
    var createdAt = result as CreatedAtActionResult;
    createdAt!.ActionName.Should().Be(nameof(QuotesController.GetQuote));
}
```
- **How it works:**
  - Verifies that the controller returns HTTP `201 Created` with a `CreatedAtActionResult` pointing to action `GetQuote`.

```csharp
[Test]
public async Task GetQuote_ExistingId_ReturnsOkResultWithQuote()
{
    // Arrange
    var details = new QuoteDetailsDto { QuoteId = 1, QuoteNumber = "QT-001", FinalPremium = 75.00m };
    _mockQuoteService.Setup(s => s.GetQuoteByIdAsync(1)).ReturnsAsync(details);

    // Act
    var result = await _controller.GetQuote(1);

    // Assert
    result.Should().BeOfType<OkObjectResult>();
    var okResult = result as OkObjectResult;
    okResult!.Value.Should().BeEquivalentTo(details);
}
```
- **How it works:**
  - Verifies that querying an existing quote returns HTTP `200 OK` (`OkObjectResult`) containing the exact DTO payload.

```csharp
[Test]
public async Task GetQuote_MissingId_ReturnsNotFoundResult()
{
    // Arrange: Service returns null for missing quote
    _mockQuoteService.Setup(s => s.GetQuoteByIdAsync(999)).ReturnsAsync((QuoteDetailsDto?)null);

    // Act
    var result = await _controller.GetQuote(999);

    // Assert
    result.Should().BeOfType<NotFoundResult>();
}
```
- **How it works:**
  - Verifies that when a quote is missing, the controller returns clean HTTP `404 Not Found`.

```csharp
[Test]
public async Task ConvertQuote_ValidId_ReturnsOkResult()
{
    // Arrange: Conversion succeeds
    _mockQuoteService.Setup(s => s.ConvertQuoteAsync(1)).ReturnsAsync(true);

    // Act
    var result = await _controller.ConvertQuote(1);

    // Assert
    result.Should().BeOfType<OkObjectResult>();
}
```
- **How it works:**
  - Verifies HTTP `200 OK` on successful quote conversion.

---

#### Group 6: ExcelServiceTests ([ExcelServiceTests.cs](file:///d:/Dotnet/PetInsurancePOC/PetInsuranceTests/ExcelServiceTests.cs))

```csharp
[Test]
public async Task ExportQuotesToExcelAsync_ExportsCompleteQuoteAndPetData()
{
    // Act: Generate binary Excel stream
    byte[] bytes = await _excelService.ExportQuotesToExcelAsync();

    // Assert
    bytes.Should().NotBeNullOrEmpty();

    using var ms = new MemoryStream(bytes);
    using var workbook = new XLWorkbook(ms);

    // Verify all 4 worksheets exist
    workbook.Worksheets.Contains("Customers").Should().BeTrue();
    workbook.Worksheets.Contains("Pets").Should().BeTrue();
    workbook.Worksheets.Contains("Quotes").Should().BeTrue();
    workbook.Worksheets.Contains("QuoteCoverages").Should().BeTrue();

    // Verify cell contents and column mappings
    var wsQuotes = workbook.Worksheet("Quotes");
    wsQuotes.Cell(1, 4).GetValue<string>().Should().Be("CustomerName");
    wsQuotes.Cell(2, 4).GetValue<string>().Should().Be("John Doe");
    wsQuotes.Cell(2, 16).GetValue<decimal>().Should().Be(30m);
    wsQuotes.Cell(2, 20).GetValue<decimal>().Should().Be(51m);
}
```
- **How it works:**
  - End-to-end binary test. Exports database records via ClosedXML into a `byte[]`, re-opens it using `XLWorkbook`, and validates that all 4 worksheets, column headers, and cell values are populated correctly.

```csharp
[Test]
public async Task ImportQuotesFromExcelAsync_ImportsCompleteDataCorrectly()
{
    // 1. Export valid Excel stream from seeded context
    byte[] bytes = await _excelService.ExportQuotesToExcelAsync();

    // 2. Spin up a fresh SQLite in-memory database
    using var newConn = new SqliteConnection("Filename=:memory:");
    await newConn.OpenAsync();
    var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(newConn).Options;
    using var newContext = new AppDbContext(options);
    await newContext.Database.EnsureCreatedAsync();

    var newExcelService = new ExcelService(newContext);
    using var stream = new MemoryStream(bytes);

    // Act: Ingest Excel stream into the blank database
    var (importedQuotes, importedCustomers, importedPets) = await newExcelService.ImportQuotesFromExcelAsync(stream);

    // Assert
    importedCustomers.Should().Be(1);
    importedPets.Should().Be(1);
    importedQuotes.Should().Be(1);

    var importedPet = await newContext.Pets.FirstOrDefaultAsync(p => p.PetName == "Buddy");
    importedPet.Should().NotBeNull();
    importedPet!.Gender.Should().Be("Male");
    importedPet.HasPreExistingCondition.Should().BeTrue();
}
```
- **How it works:**
  - Round-trip integration test. Exports data to OpenXML bytes, passes the stream into an empty SQLite database, executes `ImportQuotesFromExcelAsync`, and asserts that customer, pet, quote, and coverage records are correctly created with relationships preserved.


---

# Chapter 11: Client Presentation Defense Playbook: Slide Script & Q&A Defense

Use this script during your client demonstration to convey technical mastery:

### Slide 1: Architecture & Technology Stack
- **Script:** *"Good morning. We are pleased to present the Pet Insurance Quote Management Platform. Engineered on .NET 10 LTS with C# 14, this system follows Clean Architecture principles. By decoupling the presentation layer from business logic and database persistence, we ensure high maintainability, strict testability, and enterprise-grade scalability."*

### Slide 2: Actuarial Precision & Data Types
- **Script:** *"In financial applications, rounding errors are unacceptable. We deliberately chose 128-bit `System.Decimal` over binary floating-point types (`double`/`float`). Because `decimal` calculates in base-10 with 28 digits of precision, we eliminate recurring binary fractions, guaranteeing penny-exact accuracy across all rating adjustments, riders, and multi-pet discounts."*

### Slide 3: High-Throughput LINQ & Database Strategy
- **Script:** *"Our data layer leverages Entity Framework Core 10 backed by SQLite. Advanced queries—such as multi-field search, status grouping, descending date sorting, and pagination via `Skip` and `Take`—are translated directly into native SQL queries executed within the database engine. This keeps memory usage low and ensures sub-millisecond response times."*

### Slide 4: Excel Batch Interoperability
- **Script:** *"Recognizing that insurance brokers and underwriters depend on spreadsheets, we built a native OpenXML processing engine with ClosedXML. Our system exports complete 4-sheet relational workbooks with financial formatting, and imports bulk data with automatic identity remapping and entity deduplication."*

### Slide 5: Quality Assurance & Automated Testing
- **Script:** *"Quality is verified through an automated test suite of 21 test cases combining unit tests via Moq with real in-memory SQLite integration tests. All 21 tests pass with zero warnings, validating business rules, edge cases, and database translations."*

---
*End of Comprehensive Technical Manual and Presentation Dossier.*
