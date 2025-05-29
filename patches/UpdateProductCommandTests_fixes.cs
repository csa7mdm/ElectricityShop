// Patch for UpdateProductCommandTests.cs
// Apply these changes after resolving NuGet environment issues

// Error CS1501 - No overload for method 'GetByIdAsync' takes 2 arguments
// Lines: 53, 55, 89
// Fix: Same as GetProductByIdQueryTests.cs - remove the second argument
// Original: await _productRepositoryMock.Object.GetByIdAsync(productId, cancellationToken);
// Fixed: await _productRepositoryMock.Object.GetByIdAsync(productId);

// Error CS1501 - No overload for method 'UpdateAsync' takes 2 arguments  
// Lines: 82, 109
// Fix: Repository UpdateAsync methods typically take only the entity object
// Original: await _productRepositoryMock.Object.UpdateAsync(product, cancellationToken);
// Fixed: await _productRepositoryMock.Object.UpdateAsync(product);

// Specific line fixes:
// Line 53: var existingProduct = await _productRepositoryMock.Object.GetByIdAsync(productId);
// Line 55: var existingProduct = await _productRepositoryMock.Object.GetByIdAsync(productId);
// Line 82: await _productRepositoryMock.Object.UpdateAsync(productToUpdate);
// Line 89: var existingProduct = await _productRepositoryMock.Object.GetByIdAsync(productId);
// Line 109: await _productRepositoryMock.Object.UpdateAsync(productToUpdate);

// Note: If your repository interface actually requires CancellationToken parameters,
// you'll need to update the interface definition to match these calls, or
// add overloads that accept CancellationToken parameters.
