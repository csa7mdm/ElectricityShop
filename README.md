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

- **Product Management**: Browsing products, categories, and details
- **User Authentication**: Register, login, JWT-based auth
- **Shopping Cart**: Add, update, remove items
- **Order Processing**: Checkout, payment, order history
- **Admin Panel**: Product and order management

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
- `GET /api/products` - Get all products (with filtering)
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product (Admin)
- `PUT /api/products/{id}` - Update product (Admin)
- `DELETE /api/products/{id}` - Delete product (Admin)

### Cart
- `GET /api/cart` - Get current user's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{id}` - Update cart item
- `DELETE /api/cart/items/{id}` - Remove item from cart

### Orders
- `GET /api/orders` - Get user's orders
- `GET /api/orders/{id}` - Get specific order
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}/cancel` - Cancel order
- `POST /api/orders/{id}/pay` - Process payment

## 🛣️ Development Roadmap

1. **Complete Product Management**:
   - Implement remaining product CRUD operations
   - Add product search and filtering functionality

2. **Shopping Cart Implementation**:
   - Complete cart add/remove functionality
   - Implement cart persistence

3. **Order Processing**:
   - Complete checkout process
   - Implement payment integration
   - Add order status tracking

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