# PowerShell script to add required NuGet packages for asynchronous processing

# Ensure we're in the right directory
Set-Location -Path "E:\Projects\ElectricityShop"

# Infrastructure project packages
Write-Host "Adding packages to Infrastructure project..."
dotnet add .\src\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj package Hangfire.AspNetCore
dotnet add .\src\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj package Hangfire.SqlServer
dotnet add .\src\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj package Polly
dotnet add .\src\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj package FluentEmail.Smtp
dotnet add .\src\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj package FluentEmail.Razor

# API project packages
Write-Host "Adding packages to API project..."
dotnet add .\src\ElectricityShop.API\ElectricityShop.API.csproj package Hangfire.AspNetCore
dotnet add .\src\ElectricityShop.API\ElectricityShop.API.csproj package Hangfire.Dashboard.BasicAuthorization

# Test projects packages
Write-Host "Adding packages to Test projects..."
dotnet add .\tests\ElectricityShop.Infrastructure.Tests\ElectricityShop.Infrastructure.Tests.csproj package Moq
dotnet add .\tests\ElectricityShop.Infrastructure.Tests\ElectricityShop.Infrastructure.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory
dotnet add .\tests\ElectricityShop.Application.Tests\ElectricityShop.Application.Tests.csproj package Moq
dotnet add .\tests\ElectricityShop.Application.Tests\ElectricityShop.Application.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory

Write-Host "All packages added successfully!"
