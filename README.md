# Gamma.Kernel

**Gamma.Kernel** is a reusable, lightweight kernel library designed for building scalable .NET applications with a focus on **Clean Architecture**, **DDD**, and **CQRS** principles. It provides core abstractions and building blocks without imposing frameworks or database constraints.

---

## When to Use Gamma.Kernel

- Building enterprise or SaaS systems
- Maintaining a large codebase over years
- Implementing DDD, CQRS, or transaction-heavy workflows
- Avoiding heavy all-in-one frameworks
- Targeting .NET 10+ with modern tooling

---

## Features

- `BaseEntity` abstraction for consistent entity design
- Generic CUD repository using **Dapper**
- Audit helpers (optional per entity)
- Attribute-based mapping: `Identity`, `InsertOnly`, `RowVersion`
- `IClock` and `ICurrentUser` abstractions for time and user context
- `ICommand`/`IQuery` and handlers for CQRS pipelines
- Unit of Work and transaction context management
- Pipeline behaviors: validation, logging, transaction handling

---

## Goals

- Keep **zero business logic** in the kernel
- Remain **database-agnostic** and lightweight
- Fully **Clean Architecture friendly**
- Enable easy **unit testing** with abstracted infrastructure

---

## What Gamma.Kernel Is NOT

- ❌ Not a full framework
- ❌ Not a replacement for ASP.NET Core
- ❌ Not an ORM
- ❌ Not tied to EF Core or any database

It is intentionally **small, focused, and reusable**.

---

## Setup Guide

To run the project locally and execute test scenarios, follow these steps:

1. **Create a Test Database**

Open SQL Server and create a new database for testing:

```sql
CREATE DATABASE [gamma-next];
GO
```

2. **Check appSettings.json**

   Open appsettings.json file and check connection string.

3. **Run script in database**

   Open db-script.sql and run it in [gamma-next] database.

---

## Roadmap (High Level)

- Fine-grained authorization
- Domain events abstraction
- Outbox pattern support
- Improved diagnostics hooks
- Source-generated helpers for repetitive tasks

---

## Suggested Solution Structure

```
/src
  ├─ Gamma.Kernel
  │   ├─ Abstractions
  │   ├─ Behaviors
  │   ├─ Transactions
  │   └─ Common
  │
  ├─ MyApp.Domain
  │   ├─ Entities
  │   ├─ ValueObjects
  │   └─ DomainServices
  │
  ├─ MyApp.Application
  │   ├─ Commands
  │   ├─ Queries
  │   ├─ Services (Application)
  │   └─ DTOs
  │
  ├─ MyApp.Infra
  │   ├─ Data
  │   ├─ Logging
  │   └─ Services (External)
  │
  └─ MyApp.Api
      ├─ EndPoints
```
