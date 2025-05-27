# Build Issues Fix Plan

Based on the analysis of the error logs and review of the code, here are the steps needed to fix the build issues:

## 1. Missing Entity Framework Core References

The primary issue appears to be missing Entity Framework Core references. The following packages need to be added to the ElectricityShop.Application project:

```
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Relational
```

## 2. Project Reference Issues

There are missing project references between the application layers:

1. ElectricityShop.Application needs a reference to ElectricityShop.Infrastructure for accessing the ApplicationDbContext
2. ElectricityShop.API needs references to both ElectricityShop.Application and ElectricityShop.Infrastructure

Update the project references:

```xml
<!-- In ElectricityShop.Application.csproj -->
<ItemGroup>
  <ProjectReference Include="..\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj" />
</ItemGroup>

<!-- In ElectricityShop.API.csproj -->
<ItemGroup>
  <ProjectReference Include="..\ElectricityShop.Application\ElectricityShop.Application.csproj" />
  <ProjectReference Include="..\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj" />
</ItemGroup>
```

## 3. Target Framework Issues

The error logs show paths with "net9.0", but this might not be the intended target framework. Verify and update the target framework in all project files:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

## 4. Fix Expression Tree Issues

For the expression tree issues in the ApiWebApplicationFactory.cs file, replace direct mock creation in lambdas with helper methods that create the mocks outside of the expression trees.

```csharp
// Instead of:
services.AddSingleton(sp => new Mock<SomeType>(optionalArg: value).Object);

// Use:
services.AddSingleton<SomeType>(sp => CreateMock());

// And add a helper method:
private static SomeType CreateMock()
{
    return new Mock<SomeType>(optionalArg: value).Object;
}
```

## 5. Fix Missing EF Namespace in QueryableExtensions.cs

Add the following using statements at the top of the file:

```csharp
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
```

## 6. Fix Missing ApplicationDbContext in GetProductsQueryHandler.cs

Add a project reference from ElectricityShop.Application to ElectricityShop.Infrastructure and add the following using statement:

```csharp
using ElectricityShop.Infrastructure.Persistence.Context;
```

## 7. Build Order

Make sure the projects are built in the correct order:
1. ElectricityShop.Infrastructure
2. ElectricityShop.Application
3. ElectricityShop.API
4. ElectricityShop.API.Tests

## 8. Additional OpenTelemetry Packages

Don't forget to add the OpenTelemetry packages we identified earlier:

```
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.Runtime
dotnet add package OpenTelemetry.Instrumentation.Process
dotnet add package OpenTelemetry.Instrumentation.EntityFrameworkCore
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

## 9. Clean and Rebuild

After making these changes:
1. Delete all bin and obj folders in all projects
2. Restore NuGet packages: `dotnet restore`
3. Rebuild the solution: `dotnet build`

## Specific Code Fixes

### QueryableExtensions.cs
- Add missing `using Microsoft.EntityFrameworkCore;` statement
- Fix EF.CompileAsyncQuery calls

### GetProductsQueryHandler.cs
- Add missing using statements
- Fix path to ApplicationDbContext

### ApiWebApplicationFactory.cs
- Replace mock creation code to avoid expression trees with optional arguments
