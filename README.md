# ShopSolution

A full-stack e-commerce application built with **.NET 10, Clean Architecture, Domain-Driven Design (DDD), and Angular**.

This project demonstrates practical software engineering concepts, including separation of concerns, domain modeling, dependency injection, database integration, automated testing, and CI/CD.

## Tech Stack

**Backend**
- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server

**Frontend**
- Angular 22
- TypeScript

**Tools & Infrastructure**
- Docker Compose
- GitHub Actions
- xUnit

## Architecture

The backend follows Clean Architecture principles, separating business logic from infrastructure concerns.

### Project Structure

```text
ShopSolution/
├── Shop.Api/
├── Shop.Application/
├── Shop.Domain/
├── Shop.Infrastructure/
└── Shop.Tests/
```

### Request Flow

The main request flow:

```text
Controller
    ↓
Command / Query
    ↓
Handler
    ↓
Repository Interface
    ↓
Infrastructure Implementation
    ↓
SQL Server
```

## Features

- Product management (CRUD)
- Order management
- Domain entities with business validation
- Repository pattern
- Entity Framework Core integration
- Database migrations
- Angular frontend
- Docker-based development environment
- Automated build and test pipeline with GitHub Actions

## Getting Started

### Prerequisites

Before running the application, make sure you have installed:

- .NET 10 SDK
- Docker Desktop
- Node.js and npm

### Run the application

1. Clone the repository:

```bash
git clone https://github.com/SHK121/ShopSolution.git
cd ShopSolution
```

2. Configure the required environment variables in your `.env` file.

3. Start the application:

```bash
docker compose up --build
```

4. Access the API through Swagger:

```text
http://localhost:8081/swagger
```

## Running Tests

Run the automated tests using:

```bash
dotnet test
```

## Purpose

ShopSolution was developed as a hands-on project to deepen practical knowledge of:

- Clean Architecture and Domain-Driven Design
- ASP.NET Core and Entity Framework Core
- Backend application design and dependency injection
- Full-stack development with Angular
- Automated testing and CI/CD
- Docker-based application development