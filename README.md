# 🐾 Pet Insurance Quote Management System

[![NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/ORM-EF%20Core%2010-purple?style=flat-square)](https://docs.microsoft.com/en-us/ef/core/)
[![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?style=flat-square&logo=sqlite)](https://www.sqlite.org/)
[![JWT Auth](https://img.shields.io/badge/Auth-JWT%20Bearer-black?style=flat-square&logo=jsonwebtokens)](https://jwt.io/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)

A robust, enterprise-grade ASP.NET Core Web API microservice for managing Pet Insurance Quotes, Pet Registrations, Customer Profiles, and Excel Batch Import/Export workflows.

---

## 📋 Table of Contents

- [Architecture Overview](#-architecture-overview)
- [Tech Stack](#-tech-stack)
- [Key Features](#-key-features)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [API Reference & Endpoint Guide](#-api-reference--endpoint-guide)
- [Premium Calculation Engine](#-premium-calculation-engine)
- [Excel Batch Processing](#-excel-batch-processing)
- [Testing](#-testing)
- [Security & Authentication](#-security--authentication)

---

## 🏗️ Architecture Overview

The system follows **Clean Architecture / Repository Pattern** principles with explicit separation of concerns:

```
[ HTTP Requests ]
       │
       ▼
┌───────────────────────────────┐
│ Controllers (API Layer)       │ Auth, Validation & Routing
└──────────────┬────────────────┘
               │
               ▼
┌───────────────────────────────┐
│ Services (Business Logic)     │ Premium Calculation, Excel Generation, Auth
└──────────────┬────────────────┘
               │
               ▼
┌───────────────────────────────┐
│ Repositories (Data Access)    │ Generic & Entity-Specific Repositories
└──────────────┬────────────────┘
               │
               ▼
┌───────────────────────────────┐
│ EF Core DbContext & SQLite    │ Database & Migrations
└───────────────────────────────┘
```

---

## 🛠️ Tech Stack

| Component | Technology | Description |
|---|---|---|
| **Framework** | .NET 10.0 ASP.NET Core Web API | High-performance RESTful Web API |
| **Database** | SQLite | Lightweight embedded relational database |
| **ORM** | Entity Framework Core 10 | Code-first data modeling with LINQ |
| **Authentication** | JWT (JSON Web Tokens) | Bearer token authorization |
| **Excel Handling** | ClosedXML | High-performance OpenXML Excel spreadsheet manipulation |
| **Testing** | NUnit, Moq, EF Core InMemory | Unit testing suite (19 test cases) |
| **API Docs** | Swagger / OpenAPI (Scalar UI) | Interactive API exploration |

---

## ✨ Key Features

- **Quote Lifecycle Management**: Full CRUD operations for creating, updating, retrieving, and expiring quotes.
- **Dynamic Premium Calculation Engine**: Automatic computation of base premiums, age adjustments, multi-pet discounts, and optional wellness add-ons.
- **Customer & Pet Profiles**: Centralized entity management for pet owners and their animals.
- **Excel Batch Import & Export**: Streamlined `.xlsx` processing to export existing quotes or batch import new quotes with duplicate detection.
- **JWT Authentication**: Secured endpoints with role-agnostic Bearer token authentication.
- **Global Error Handling**: Standardized problem details and JSON error responses via custom middleware.

---

## 📁 Project Structure

```
PetInsurancePOC/
├── PetInsurance/                      # Main API Project
│   ├── Controllers/                   # Web API Controllers
│   │   ├── AuthController.cs          # Login & Token Generation
│   │   ├── ExcelController.cs         # Excel Import / Export Endpoints
│   │   ├── QuotesController.cs        # Quote Management Endpoints
│   │   └── TestController.cs          # Health Check / Smoke Test
│   ├── Data/                          # Database Context & Seed Data
│   │   ├── ApplicationDbContext.cs    # EF Core DbContext
│   │   └── DbInitializer.cs           # Database Seeder
│   ├── Models/                        # Domain & Entity Models
│   │   ├── Customer.cs
│   │   ├── Pet.cs
│   │   ├── Quote.cs
│   │   ├── QuoteCoverage.cs
│   │   ├── User.cs
│   │   └── DTOs/                      # Data Transfer Objects
│   ├── Repositories/                  # Data Access Layer
│   │   ├── Implementations/           # Generic & Concrete Repositories
│   │   └── Interfaces/                # Repository Interfaces
│   ├── Services/                      # Core Business Logic Layer
│   │   ├── Implementations/           # Excel, Quote, Calculation Services
│   │   └── Interfaces/                # Service Interfaces
│   ├── Middleware/                    # Custom Middleware (Global Exception Handling)
│   ├── Program.cs                     # App Entry Point & DI Container
│   └── appsettings.json               # Application Configuration
└── PetInsuranceTests/                 # NUnit Test Project
    ├── CalculationEngineTests.cs
    ├── ExcelServiceTests.cs
    └── QuoteServiceTests.cs
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or higher
- IDE: Visual Studio 2022 / VS Code / JetBrains Rider

### Quick Start

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Sanjay3005v/PetInsurancePOC.git
   cd PetInsurancePOC
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the API server**:
   ```bash
   dotnet run --project .\PetInsurance
   ```
   The server will start listening at: `http://localhost:5294`

5. **Access Interactive API Docs**:
   Navigate to `http://localhost:5294/scalar/v1` or `http://localhost:5294/swagger` in your browser.

---

## ⚙️ Configuration

Application settings are located in `PetInsurance/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=petinsurance.db"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHereWithMinimum32Chars!",
    "Issuer": "PetInsuranceAPI",
    "Audience": "PetInsuranceUsers",
    "ExpiryInMinutes": 60
  },
  "PremiumSettings": {
    "DogBasePremium": 30.00,
    "CatBasePremium": 20.00,
    "AgeMultiplierPerYear": 0.05,
    "PreExistingConditionSurcharge": 15.00,
    "MultiPetDiscountPercentage": 10.00,
    "WellnessAddonCost": 15.00
  }
}
```

---

## 📡 API Reference & Endpoint Guide

### Authentication (`/api/Auth`)

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| `POST` | `/api/Auth/login` | No | Authenticate user and receive JWT bearer token |

### Quotes (`/api/Quotes`)

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| `GET` | `/api/Quotes` | Yes | Get list of all quotes |
| `GET` | `/api/Quotes/{id}` | Yes | Get quote details by ID |
| `POST` | `/api/Quotes` | Yes | Create new quote (auto-calculates premium) |
| `PUT` | `/api/Quotes/{id}` | Yes | Update existing quote |
| `DELETE` | `/api/Quotes/{id}` | Yes | Delete a quote |

### Excel Workflows (`/api/Excel`)

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| `GET` | `/api/Excel/quotes/export` | Yes | Export all quotes as an `.xlsx` file stream |
| `POST` | `/api/Excel/quotes/import` | Yes | Import quotes from an uploaded `.xlsx` spreadsheet |

---

## 🧮 Premium Calculation Engine

The final monthly quote premium is computed dynamically according to the formula:

$$\text{Final Premium} = (\text{Base Premium} \times (1 + (\text{Age} \times \text{AgeMultiplier})) + \text{Surcharges} - \text{Discounts}) + \text{Wellness}$$

- **Base Premium**: \$30.00 for Dogs, \$20.00 for Cats
- **Age Factor**: $+5\%$ per year of pet age
- **Pre-Existing Conditions**: Flat $+\$15.00$ surcharge
- **Multi-Pet Discount**: $-\$10.00$ discount when customer owns multiple pets
- **Wellness Add-On**: Flat $+\$15.00$ monthly optional add-on

---

## 🧪 Testing

Run the full NUnit test suite using the CLI:

```bash
dotnet test
```

**Test Coverage Summary**:
- **19 Total Test Cases** across `QuoteService`, `ExcelService`, and `CalculationEngine`.
- Tests include: Premium calculations, multi-pet discounts, Excel memory stream exports, invalid file formats, duplicate imports, and entity repository mocks.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
