# Project References

## ElectricityShop.Application.csproj

```xml
<ItemGroup>
  <ProjectReference Include="..\ElectricityShop.Domain\ElectricityShop.Domain.csproj" />
  <!-- Add this reference if missing -->
  <ProjectReference Include="..\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj" />
</ItemGroup>

<ItemGroup>
  <!-- Add these packages if missing -->
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="8.0.0" />
</ItemGroup>
```

## ElectricityShop.API.csproj

```xml
<ItemGroup>
  <ProjectReference Include="..\ElectricityShop.Application\ElectricityShop.Application.csproj" />
  <ProjectReference Include="..\ElectricityShop.Infrastructure\ElectricityShop.Infrastructure.csproj" />
</ItemGroup>

<ItemGroup>
  <!-- Add OpenTelemetry packages -->
  <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.8.0" />
  <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.8.0" />
  <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.8.0" />
  <PackageReference Include="OpenTelemetry.Instrumentation.Runtime" Version="1.8.0" />
  <PackageReference Include="OpenTelemetry.Instrumentation.Process" Version="1.8.0" />
  <PackageReference Include="OpenTelemetry.Instrumentation.EntityFrameworkCore" Version="1.8.0" />
  <PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.8.0" />
  <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.8.0" />
</ItemGroup>
```

## ElectricityShop.Infrastructure.csproj

```xml
<ItemGroup>
  <ProjectReference Include="..\ElectricityShop.Domain\ElectricityShop.Domain.csproj" />
</ItemGroup>

<ItemGroup>
  <!-- Ensure these packages are present -->
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
</ItemGroup>
```

## ElectricityShop.API.Tests.csproj

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\ElectricityShop.API\ElectricityShop.API.csproj" />
</ItemGroup>

<ItemGroup>
  <!-- Ensure these packages are present -->
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
  <PackageReference Include="Moq" Version="4.20.69" />
  <PackageReference Include="xunit" Version="2.6.1" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
</ItemGroup>
```

## Target Framework Update

Make sure all projects have the correct target framework (adjust version as needed):

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

## Important Notes for Testing

### Moq Usage with Expression Trees

When using Moq to mock methods with optional parameters, especially in expression trees (like when using `Setup` method), avoid using optional parameters directly. Instead:

```csharp
// INCORRECT - Will cause CS0854 error:
mock.Setup(x => x.SomeMethodAsync()).Returns(Task.FromResult(true));

// CORRECT - Explicitly specify the parameter with It.IsAny<T>():
mock.Setup(x => x.SomeMethodAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));
```

This is especially important for async methods that have optional CancellationToken parameters.

### Non-Nullable Reference Types

When using non-nullable reference types (like string properties), ensure they are properly initialized:

```csharp
// INCORRECT - Will cause CS8618 warning:
public class AuthenticationResult
{
    public string Token { get; set; } // Non-nullable not initialized
}

// CORRECT - Initialize the property:
public class AuthenticationResult
{
    public string Token { get; set; } = string.Empty;
    
    // OR use constructor initialization:
    public AuthenticationResult(string token)
    {
        Token = token;
    }
}
```
