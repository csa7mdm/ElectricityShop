import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/cart.dart';
import '../entities/cart_item.dart';

/// Repository interface for cart operations
abstract class CartRepository {
  /// Get the current cart
  Future<Either<Failure, Cart>> getCart();
  
  /// Add an item to the cart
  Future<Either<Failure, Cart>> addToCart({
    required String productId,
    required String name,
    required String imageUrl,
    required double price,
    required int quantity,
  });
  
  /// Update the quantity of an item in the cart
  Future<Either<Failure, Cart>> updateCartItemQuantity({
    required String itemId,
    required int quantity,
  });
  
  /// Remove an item from the cart
  Future<Either<Failure, Cart>> removeFromCart({
    required String itemId,
  });
  
  /// Clear the entire cart
  Future<Either<Failure, Cart>> clearCart();
  
  /// Get a specific cart item by ID
  Future<Either<Failure, CartItem?>> getCartItem({
    required String itemId,
  });
}
