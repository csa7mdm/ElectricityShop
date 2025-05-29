# ElectricityShop Build Error Fix Guide

## Prerequisites
**CRITICAL**: The NuGet environment issue ("Value cannot be null. Parameter 'path1'") must be resolved first by:
1. Completely reinstalling .NET 9 SDK, OR
2. Using an alternative development environment (Docker, WSL2, etc.)

## Step 1: Fix ElectricityShop.Application.Tests Errors

### 1.1 Fix CreateOrderCommandHandlerTests.cs

**File**: `tests\ElectricityShop.Application.Tests\Features\Orders\Commands\Handlers\CreateOrderCommandHandlerTests.cs`

**Error CS1061 (Line 47)**: Add using directive for Entity Framework
```csharp
// Add this at the top of the file
using Microsoft.EntityFrameworkCore;
```

**Error CS0104 (Lines 93, 95, 182, 184, 229, 231, 262, 264)**: Resolve OrderItemDto ambiguity
```csharp
// Add namespace aliases at the top of the file
using CommandOrderItemDto = ElectricityShop.Application.Features.Orders.Commands.OrderItemDto;
using InterfaceOrderItemDto = ElectricityShop.Application.Common.Interfaces.OrderItemDto;

// Then replace ambiguous references:
// Change: var orderItem = new OrderItemDto { ... };
// To: var orderItem = new CommandOrderItemDto { ... };
```

**Error CS1503 (Line 133)**: Fix argument types - OrderStatus to DateTime
```csharp
// Line 133 - Replace OrderStatus arguments with DateTime values
// Original: SomeMethod(orderStatusModel, orderStatusEnum);
// Fixed: SomeMethod(DateTime.UtcNow, DateTime.UtcNow);
// Note: Use appropriate DateTime values based on your test logic
```

**Error CS0021 (Line 138)**: Fix ICollection indexing
```csharp
// Add at top of file
using System.Linq;

// Line 138 - Replace direct indexing with LINQ
// Original: var item = myCollection[0];
// Fixed: 
var itemList = myCollection.ToList();
var item = itemList[0];
// Or use: var firstItem = myCollection.FirstOrDefault();
```

### 1.2 Fix GetProductByIdQueryTests.cs

**File**: `tests\ElectricityShop.Application.Tests\Features\Products\Queries\GetProductByIdQueryTests.cs`

**Error CS1501 (Lines 55, 57, 71, 72, 97, 112, 113, 121, 134)**: Fix GetByIdAsync method calls
```csharp
// Replace all GetByIdAsync calls with 2 arguments to 1 argument
// Original: await _productRepositoryMock.Object.GetByIdAsync(productId, cancellationToken);
// Fixed: await _productRepositoryMock.Object.GetByIdAsync(productId);
```

### 1.3 Fix UpdateProductCommandTests.cs

**File**: `tests\ElectricityShop.Application.Tests\Features\Products\Commands\UpdateProductCommandTests.cs`

**Error CS1501 - GetByIdAsync (Lines 53, 55, 89)**: Same fix as above
```csharp
// Replace 2-argument calls with 1-argument calls
// Original: await _productRepositoryMock.Object.GetByIdAsync(productId, cancellationToken);
// Fixed: await _productRepositoryMock.Object.GetByIdAsync(productId);
```

**Error CS1501 - UpdateAsync (Lines 82, 109)**: Fix UpdateAsync method calls
```csharp
// Replace 2-argument UpdateAsync calls with 1-argument calls
// Original: await _productRepositoryMock.Object.UpdateAsync(product, cancellationToken);
// Fixed: await _productRepositoryMock.Object.UpdateAsync(product);
```

## Step 2: Build Order

1. **Build Infrastructure first**: `dotnet build src\ElectricityShop.Infrastructure\`
2. **Build Application**: `dotnet build src\ElectricityShop.Application\`
3. **Build API**: `dotnet build src\ElectricityShop.API\`
4. **Build Tests**: `dotnet build tests\`
5. **Build entire solution**: `dotnet build`

## Step 3: Verify Fixes

After applying all fixes and resolving the NuGet environment:

```powershell
cd "E:\Projects\ElectricityShop"
dotnet clean
dotnet restore
dotnet build
```

## Expected Outcome

- All CS0246, CS1061, CS0104, CS1503, CS0021, and CS1501 errors should be resolved
- CS0006 metadata file errors should disappear automatically once dependencies build successfully
- Solution should build without errors

## Notes

- The package reference for Microsoft.EntityFrameworkCore.InMemory has been added to ElectricityShop.Application.Tests.csproj
- RabbitMQ.Client package is already properly referenced in Infrastructure project
- All using directives are present in RabbitMqEventConsumer.cs

## If Issues Persist

1. Check that all package references are restored properly
2. Verify target framework consistency (all projects should target net9.0)
3. Ensure project references are correct in solution file
4. Consider using `dotnet build --no-restore` if packages are restored but build still fails
