import 'dart:convert';
import 'package:dartz/dartz.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../../core/error/failures.dart';
import '../../domain/entities/cart.dart';
import '../../domain/entities/cart_item.dart';
import '../../domain/repositories/cart_repository.dart';
import '../models/cart_model.dart';
import '../models/cart_item_model.dart';

/// Implementation of CartRepository using SharedPreferences for storage
class CartRepositoryImpl implements CartRepository {
  final SharedPreferences _sharedPreferences;
  static const String _cartKey = 'cart_data';

  CartRepositoryImpl(this._sharedPreferences);

  @override
  Future<Either<Failure, Cart>> getCart() async {
    try {
      final cartJson = _sharedPreferences.getString(_cartKey);
      
      if (cartJson == null) {
        return Right(CartModel.empty());
      }
      
      final cartMap = json.decode(cartJson) as Map<String, dynamic>;
      final cart = CartModel.fromJson(cartMap);
      
      return Right(cart);
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to retrieve cart',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, Cart>> addToCart({
    required String productId,
    required String name,
    required String imageUrl,
    required double price,
    required int quantity,
  }) async {
    try {
      // Get current cart
      final cartResult = await getCart();
      
      return cartResult.fold(
        (failure) => Left(failure),
        (cart) {
          // Check if item already exists
          final existingItemIndex = cart.items.indexWhere(
            (item) => item.productId == productId,
          );
          
          final List<CartItem> updatedItems = List.from(cart.items);
          
          if (existingItemIndex >= 0) {
            // Update existing item
            final existingItem = cart.items[existingItemIndex];
            final updatedItem = existingItem.copyWith(
              quantity: existingItem.quantity + quantity,
              subtotal: (existingItem.quantity + quantity) * price,
            );
            
            updatedItems[existingItemIndex] = updatedItem;
          } else {
            // Add new item
            final newItem = CartItemModel(
              id: DateTime.now().millisecondsSinceEpoch.toString(),
              productId: productId,
              name: name,
              imageUrl: imageUrl,
              price: price,
              quantity: quantity,
              subtotal: price * quantity,
            );
            
            updatedItems.add(newItem);
          }
          
          // Calculate new cart totals
          final updatedCart = CartModel.fromEntity(
            Cart(
              items: updatedItems,
              subtotal: 0,
              tax: 0,
              total: 0,
            ).recalculate(),
          );
          
          // Save updated cart
          _saveCart(updatedCart);
          
          return Right(updatedCart);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to add item to cart',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, Cart>> updateCartItemQuantity({
    required String itemId,
    required int quantity,
  }) async {
    try {
      // Get current cart
      final cartResult = await getCart();
      
      return cartResult.fold(
        (failure) => Left(failure),
        (cart) {
          // Find the item
          final itemIndex = cart.items.indexWhere((item) => item.id == itemId);
          
          if (itemIndex < 0) {
            return Left(NotFoundFailure(
              message: 'Item not found in cart',
              data: {'itemId': itemId},
            ));
          }
          
          final List<CartItem> updatedItems = List.from(cart.items);
          
          if (quantity <= 0) {
            // Remove item if quantity is 0 or negative
            updatedItems.removeAt(itemIndex);
          } else {
            // Update quantity
            final item = cart.items[itemIndex];
            final updatedItem = item.copyWith(
              quantity: quantity,
              subtotal: quantity * item.price,
            );
            
            updatedItems[itemIndex] = updatedItem;
          }
          
          // Calculate new cart totals
          final updatedCart = CartModel.fromEntity(
            Cart(
              items: updatedItems,
              subtotal: 0,
              tax: 0,
              total: 0,
            ).recalculate(),
          );
          
          // Save updated cart
          _saveCart(updatedCart);
          
          return Right(updatedCart);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to update cart item quantity',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, Cart>> removeFromCart({
    required String itemId,
  }) async {
    try {
      // Get current cart
      final cartResult = await getCart();
      
      return cartResult.fold(
        (failure) => Left(failure),
        (cart) {
          // Find the item
          final itemIndex = cart.items.indexWhere((item) => item.id == itemId);
          
          if (itemIndex < 0) {
            return Left(NotFoundFailure(
              message: 'Item not found in cart',
              data: {'itemId': itemId},
            ));
          }
          
          final List<CartItem> updatedItems = List.from(cart.items);
          updatedItems.removeAt(itemIndex);
          
          // Calculate new cart totals
          final updatedCart = CartModel.fromEntity(
            Cart(
              items: updatedItems,
              subtotal: 0,
              tax: 0,
              total: 0,
            ).recalculate(),
          );
          
          // Save updated cart
          _saveCart(updatedCart);
          
          return Right(updatedCart);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to remove item from cart',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, Cart>> clearCart() async {
    try {
      final emptyCart = CartModel.empty();
      _saveCart(emptyCart);
      return Right(emptyCart);
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to clear cart',
        data: e.toString(),
      ));
    }
  }

  @override
  Future<Either<Failure, CartItem?>> getCartItem({
    required String itemId,
  }) async {
    try {
      // Get current cart
      final cartResult = await getCart();
      
      return cartResult.fold(
        (failure) => Left(failure),
        (cart) {
          // Find the item
          final item = cart.items.firstWhere(
            (item) => item.id == itemId,
            orElse: () => null as CartItem,
          );
          
          return Right(item);
        },
      );
    } catch (e) {
      return Left(CacheFailure(
        message: 'Failed to get cart item',
        data: e.toString(),
      ));
    }
  }

  /// Save cart to SharedPreferences
  Future<void> _saveCart(CartModel cart) async {
    final cartJson = json.encode(cart.toJson());
    await _sharedPreferences.setString(_cartKey, cartJson);
  }
}
