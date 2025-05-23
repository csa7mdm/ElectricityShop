# ElectricityShop

A full-stack e-commerce application for selling electrical products, built with Clean Architecture principles to ensure separation of concerns, maintainability, and testability.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Common Issues and Solutions](#common-issues-and-solutions)
- [Development Roadmap](#development-roadmap)
- [Contributing](#contributing)
- [License](#license)

## 🔍 Overview

ElectricityShop is a modern e-commerce platform that follows industry best practices and Clean Architecture principles. The application provides a complete shopping experience with product browsing, cart management, order processing, and user account functionality.

## 🏗️ Architecture

This project implements Clean Architecture with the following layers:

- **Domain Layer**: Core business entities and interfaces
- **Application Layer**: Business logic and use cases using CQRS pattern
- **Infrastructure Layer**: External concerns like data persistence and messaging
- **API Layer**: RESTful endpoints for client applications

![Clean Architecture Diagram](https://raw.githubusercontent.com/jasontaylordev/CleanArchitecture/main/.github/clean-architecture.png)

The application also implements CQRS (Command Query Responsibility Segregation) for better separation of read and write operations.

## 💻 Technology Stack

- **Backend**:
  - .NET 9 with ASP.NET Core
  - Entity Framework Core (ORM)
  - Microsoft SQL Server
  - JWT Authentication
  - RabbitMQ for messaging

- **Frontend Options**:
  - React web application
  - Flutter mobile app

## 📁 Project Structure

```
ElectricityShop/
├── ElectricityShop.sln
├── src/
│   ├── ElectricityShop.Domain/          # Core business entities and interfaces
│   ├── ElectricityShop.Application/     # Application logic and use cases
│   ├── ElectricityShop.Infrastructure/  # Infrastructure implementations
│   ├── ElectricityShop.API/             # REST API endpoints
│   ├── electricity-shop-web/            # React web frontend (optional)
│   └── electricity_shop_mobile/         # Flutter mobile app (optional)
└── tests/
    ├── ElectricityShop.Domain.Tests/
    ├── ElectricityShop.Application.Tests/
    └── ElectricityShop.API.Tests/
```

## ✨ Features

- **Product Management**: Backend for CRUD operations, product details, and basic search (name/description) & filtering (CategoryId) implemented. Frontend integration pending.
- **User Authentication**: Register, login, JWT-based auth.
- **Shopping Cart**: Backend for add, update, remove items, and clear cart functionalities implemented. Cart data is persisted. Frontend integration pending.
- **Order Processing**: Backend for creating orders, retrieving user's orders (list and by ID), order cancellation, and payment processing (simulated) implemented. Frontend integration pending.
- **Admin Panel**: Basic admin authorization for product creation/update/deletion. Full admin panel UI/UX pending.

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- [RabbitMQ Server](https://www.rabbitmq.com/download.html)
- [Node.js](https://nodejs.org/) (if using React frontend)
- [Flutter SDK](https://flutter.dev/docs/get-started/install) (if using mobile app)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/ElectricityShop.git
   cd ElectricityShop
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Set up the database:
   The application will automatically create and migrate the database on first run, or you can manually apply migrations:
   ```bash
   cd src/ElectricityShop.API
   dotnet ef database update
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

### Configuration

Update connection strings and other settings in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ElectricityShopDb;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
  "IdentityConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ElectricityShopIdentityDb;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
},
"RabbitMQ": {
  "HostName": "localhost",
  "UserName": "guest",
  "Password": "guest",
  "VirtualHost": "/",
  "ExchangeName": "electricity_shop_exchange"
}
```

## 📝 API Documentation

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/refresh` - Refresh token

### Products
- `GET /api/products` - Get all products (with basic filtering by SearchTerm and CategoryId)
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product (Admin)
- `PUT /api/products/{id}` - Update product (Admin)
- `DELETE /api/products/{id}` - Delete product (Admin)

### Cart
- `GET /api/cart` - Get current user's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{id}` - Update cart item quantity (set to 0 to remove)
- `DELETE /api/cart/items/{id}` - Remove item from cart
- `DELETE /api/cart` - Clear all items from current user's cart

### Orders
- `GET /api/orders` - Get user's orders
- `GET /api/orders/{id}` - Get specific order
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}/cancel` - Cancel order
- `POST /api/orders/{id}/pay` - Process payment (currently simulated)

## 🔧 Common Issues and Solutions

### Expression Trees with Optional Parameters (CS0854)

**Problem**: When using Moq to mock methods with optional parameters, you may encounter error CS0854: "An expression tree may not contain a call or invocation that uses optional arguments."

**Solution**:
```csharp
// Instead of:
mock.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);

// Use:
mock.Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
```

### CancellationToken Parameters in Async Methods

**Problem**: Test mocks incorrectly using bool parameters instead of CancellationToken in async methods.

**Solution**:
```csharp
// Incorrect:
mock.Setup(x => x.SomeMethodAsync(true)).Returns(Task.FromResult(true));

// Correct:
mock.Setup(x => x.SomeMethodAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
```

### Non-nullable Reference Types Not Initialized (CS8618)

**Problem**: Warning CS8618 when non-nullable reference types are not initialized in constructors.

**Solution**:
```csharp
// Option 1: Initialize in constructor
public class AuthenticationResult
{
    public string Token { get; set; }

    public AuthenticationResult(string token)
    {
        Token = token;
    }
}

// Option 2: Use nullable reference type
public class AuthenticationResult
{
    public string? Token { get; set; }
}

// Option 3: Initialize with default value
public class AuthenticationResult
{
    public string Token { get; set; } = string.Empty;
}
```

### Avoiding Lambda Expressions in Moq Setup

**Problem**: When combined with optional parameters, lambda expressions in Moq can trigger CS0854 errors.

**Solution**:
```csharp
// Instead of:
mock.Setup(x => x.SomeMethodAsync()).Returns(() => Task.FromResult(true));

// Create the result object outside the expression tree:
Task<bool> result = Task.FromResult(true);
mock.Setup(x => x.SomeMethodAsync(It.IsAny<CancellationToken>())).Returns(result);
```

## 🛣️ Development Roadmap

1. **Complete Product Management**:
   - Backend implemented, frontend pending.
   - Basic backend search (name/description) & filter (CategoryId) implemented. Advanced filtering/search capabilities and frontend integration pending.

2. **Shopping Cart Implementation**:
   - Backend implemented for add, update, remove, and clear cart. Frontend pending.
   - Backend implemented (cart data is now stored, associated with users).

3. **Order Processing**:
   - Backend for order creation and payment processing (simulated) is implemented. Full checkout flow including UI and comprehensive validation pending.
   - Backend structure for payment processing exists (simulated). Real payment gateway integration pending.
   - Backend supports order statuses (e.g., Processing, Paid, Cancelled through commands); specific UI/notifications for tracking pending.

4. **User Account Management**:
   - Implement profile management
   - Add address management
   - Implement password reset functionality

5. **Frontend Development**:
   - Develop React frontend components
   - Implement API integration with the backend
   - Build responsive UI

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.